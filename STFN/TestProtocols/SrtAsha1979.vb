Public Class SrtAsha1979
    Inherits TestProtocol

    Public Overrides ReadOnly Property Name As String
        Get
            Return "SRT ASHA 1979"
        End Get
    End Property

    Public Overrides Property StoppingCriterium As StoppingCriteria
        Get
            Return StoppingCriteria.ThresholdReached
        End Get
        Set(value As StoppingCriteria)
            MsgBox("An attempt was made to change the test protocol stopping criterium. The " & Name & " procedure requires that the stopping criterium is 'ThresholdReached'. Ignoring the attempt to set the new value.", , "Unsupported stopping criterium.")
        End Set
    End Property


    Private TestStageMaxTrialCount As Integer = 4

    Private TestStageScoreThreshold As Integer = 3

    Private BallparkStageAdaptiveStepSize As Integer = 10

    Private EndOfBallParkLevelAdjustment As Integer = 15

    Private AdaptiveStepSize As Double = 5

    Private CurrentTestStage As UInteger = 0

    Private NextSpeechLevel As Double = 0

    Private FinalThreshold As Double? = Nothing


    Private MinimumNumberOfAscendingRuns As Integer = 2

    Private InterRunAdeptiveStepSize As Double = 10

    Public Overrides Sub InitializeProtocol(ByRef InitialTaskInstruction As NextTaskInstruction)

        'Setting the (initial) speech level to -10 dB
        NextSpeechLevel = -10

        'Setting a default value for InitialTaskInstruction.AdaptiveStepSize
        If InitialTaskInstruction.AdaptiveStepSize.HasValue = False Then
            InitialTaskInstruction.AdaptiveStepSize = 5
        End If

        'And storing that value
        AdaptiveStepSize = InitialTaskInstruction.AdaptiveStepSize

        'Setting the initial TestStage to 0 (i.e. Ballpark)
        CurrentTestStage = 0

    End Sub

    Public Overrides Function NewResponse(ByRef TrialHistory As TrialHistory) As NextTaskInstruction

        If TrialHistory.Count = 0 Then
            'This is the start of the test, returns the initial settings
            Return New NextTaskInstruction With {.AdaptiveValue = NextSpeechLevel, .TestStage = CurrentTestStage, .Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}
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
                NextSpeechLevel -= EndOfBallParkLevelAdjustment
                Return New NextTaskInstruction With {.AdaptiveValue = NextSpeechLevel, .TestStage = CurrentTestStage, .Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}
            Else
                NextSpeechLevel += BallparkStageAdaptiveStepSize
                Return New NextTaskInstruction With {.AdaptiveValue = NextSpeechLevel, .TestStage = CurrentTestStage, .Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}
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

                'Setting the SRT to the current stage level, but only if it's the first time the code reaches this point
                If FinalThreshold.HasValue = False Then FinalThreshold = NextSpeechLevel
                Return New NextTaskInstruction With {.AdaptiveValue = NextSpeechLevel, .TestStage = CurrentTestStage, .Decision = SpeechTest.SpeechTestReplies.TestIsCompleted}

                'Checking if number of incorrect exceeds the stage length minus the required number correct (i.e. impossible to get the required number of correct, before stage ends)
            ElseIf (TestStageResults(TestStageResults.Count - 1).Item1 - TestStageResults(TestStageResults.Count - 1).Item2) > (TestStageMaxTrialCount - TestStageScoreThreshold) Then

                'Adjusting level and going to the next stage
                CurrentTestStage += 1
                NextSpeechLevel += AdaptiveStepSize
                Return New NextTaskInstruction With {.AdaptiveValue = NextSpeechLevel, .TestStage = CurrentTestStage, .Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}

                'Checking if the current stage is at its maximum number of trials
            ElseIf TestStageResults(TestStageResults.Count - 1).Item1 = TestStageMaxTrialCount Then

                'N.B. TODO: This should never happen.... since it's blocked by the clause above

                'The test stage is complete without reaching the required number of correct trials

                'Adjusting level and going to the next stage
                CurrentTestStage += 1
                NextSpeechLevel += AdaptiveStepSize
                Return New NextTaskInstruction With {.AdaptiveValue = NextSpeechLevel, .TestStage = CurrentTestStage, .Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}

            Else
                'The stage is not yet completed, changing nothing
                Return New NextTaskInstruction With {.AdaptiveValue = NextSpeechLevel, .TestStage = CurrentTestStage, .Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}
            End If

        End If

    End Function

    Public Overrides Function GetResults(ByRef TrialHistory As TrialHistory) As TestResults

        Dim Output = New TestResults(TestResults.TestResultTypes.SRT)
        If FinalThreshold.HasValue Then
            Output.SpeechRecognitionThreshold = FinalThreshold
        Else
            'Storing NaN if no threshold was reached
            Output.SpeechRecognitionThreshold = Double.NaN
        End If

        'Storing the SpeechLevelSeries
        Output.SpeechLevelSeries = New List(Of Double)
        Output.TestStageSeries = New List(Of String)
        Output.ScoreSeries = New List(Of Integer)
        For Each Trial As SrtTrial In TrialHistory
            Output.SpeechLevelSeries.Add(Math.Round(Trial.SpeechLevel))
            Output.TestStageSeries.Add(Trial.TestStage)
            If Trial.IsCorrect = True Then
                Output.ScoreSeries.Add("Correct")
            Else
                Output.ScoreSeries.Add("Incorrect")
            End If
        Next

        Return Output
    End Function
End Class