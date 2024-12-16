Public Class SrtChaiklinFontDixon1967_TestProtocol
    Inherits TestProtocol

    Public Overrides ReadOnly Property Name As String
        Get
            Return "Chaiklin-Font-Dixon (1967)"
        End Get
    End Property

    Public Overrides ReadOnly Property Information As String
        Get
            Return "Chaiklin-Font-Dixon (1967)"
        End Get
    End Property

    Public Overrides Function GetPatientInstructions() As String
        Return ""
    End Function

    Public Overrides Function GetSuggestedStartlevel(Optional ReferenceValue As Double? = Nothing) As Double
        Throw New NotImplementedException()
    End Function

    Public Overrides Property StoppingCriterium As StoppingCriteria
        Get
            Return StoppingCriteria.ThresholdReached
        End Get
        Set(value As StoppingCriteria)
            MsgBox("An attempt was made to change the test protocol stopping criterium. The " & Name & " procedure requires that the stopping criterium is 'ThresholdReached'. Ignoring the attempt to set the new value.", , "Unsupported stopping criterium.")
        End Set
    End Property


    Private TestStageMaxTrialCount As Integer = 6

    Private TestStageScoreThreshold As Integer = 3

    Private BallparkStageAdaptiveStepSize As Integer = 10

    Private EndOfBallParkLevelAdjustment As Integer = 20

    Private AdaptiveStepSize As Double = 5

    Private CurrentTestStage As UInteger = 0

    Private NextAdaptiveLevel As Double = 0

    Private FinalThreshold As Double? = Nothing


    Public Overrides Function InitializeProtocol(ByRef InitialTaskInstruction As NextTaskInstruction) As Boolean

        'Setting the (initial) adaptive level to -10 dB
        NextAdaptiveLevel = -10

        'Setting a default value for InitialTaskInstruction.AdaptiveStepSize
        If InitialTaskInstruction.AdaptiveStepSize.HasValue = False Then
            InitialTaskInstruction.AdaptiveStepSize = 5
        End If

        'And storing that value
        AdaptiveStepSize = InitialTaskInstruction.AdaptiveStepSize

        'Setting the initial TestStage to 0 (i.e. Ballpark)
        CurrentTestStage = 0

        Return True

    End Function

    ''' <summary>
    ''' Returns the number of trials remaining or -1 if this is not possible to determine.
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function TotalTrialCount() As Integer
        Return -1
    End Function

    Public Overrides Function NewResponse(ByRef TrialHistory As TrialHistory) As NextTaskInstruction

        If TrialHistory.Count = 0 Then
            'This is the start of the test, returns the initial settings
            Return New NextTaskInstruction With {.AdaptiveValue = NextAdaptiveLevel, .TestStage = CurrentTestStage, .Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}
        End If

        'Corrects the last given response
        If TrialHistory.Last.GetProportionTasksCorrect > 0 Then
            TrialHistory.Last.IsCorrect = True
        Else
            TrialHistory.Last.IsCorrect = False
        End If

        If CurrentTestStage = 0 Then
            'The ballpark stage

            If TrialHistory(TrialHistory.Count - 1).IsCorrect = True Then
                CurrentTestStage = 1
                NextAdaptiveLevel -= EndOfBallParkLevelAdjustment
                Return New NextTaskInstruction With {.AdaptiveValue = NextAdaptiveLevel, .TestStage = CurrentTestStage, .Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}
            Else
                NextAdaptiveLevel += BallparkStageAdaptiveStepSize
                Return New NextTaskInstruction With {.AdaptiveValue = NextAdaptiveLevel, .TestStage = CurrentTestStage, .Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}
            End If

        Else

            'Getting the scores in all stages presented
            Dim TestStageResults As New List(Of Tuple(Of Integer, Integer))
            For stage As UInteger = 1 To CurrentTestStage
                Dim StageTrials As Integer = 0
                Dim StageScore As Integer = 0
                For Each Trial In TrialHistory
                    If Trial.TestStage = stage Then
                        StageTrials += 1
                        If Trial.IsCorrect = True Then
                            StageScore += 1
                        End If
                    End If
                Next
                TestStageResults.Add(New Tuple(Of Integer, Integer)(StageTrials, StageScore))
            Next

            'Checking if we have at least TestStageScoreThreshold correct responses
            If TestStageResults(TestStageResults.Count - 1).Item2 >= TestStageScoreThreshold Then

                'Setting the FinalThreshold to the current stage level, but only if it's the first time the code reaches this point
                If FinalThreshold.HasValue = False Then FinalThreshold = NextAdaptiveLevel
                Return New NextTaskInstruction With {.AdaptiveValue = NextAdaptiveLevel, .TestStage = CurrentTestStage, .Decision = SpeechTest.SpeechTestReplies.TestIsCompleted}

                'Checking if number of incorrect exceeds the stage length minus the required number correct (i.e. impossible to get the required number of correct, before stage ends)
            ElseIf (TestStageResults(TestStageResults.Count - 1).Item1 - TestStageResults(TestStageResults.Count - 1).Item2) > (TestStageMaxTrialCount - TestStageScoreThreshold) Then

                'Adjusting level and going to the next stage
                CurrentTestStage += 1
                NextAdaptiveLevel += AdaptiveStepSize
                Return New NextTaskInstruction With {.AdaptiveValue = NextAdaptiveLevel, .TestStage = CurrentTestStage, .Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}

            Else
                'The stage is not yet completed, changing nothing
                Return New NextTaskInstruction With {.AdaptiveValue = NextAdaptiveLevel, .TestStage = CurrentTestStage, .Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}
            End If

        End If

    End Function

    Public Overrides Function GetFinalResult() As Double?

        Return FinalThreshold

    End Function


    Public Overrides Sub FinalizeProtocol(ByRef TrialHistory As TrialHistory)
        'This is ignored, since results are determines in NewResponse
    End Sub
End Class