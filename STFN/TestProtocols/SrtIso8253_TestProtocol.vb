Public Class SrtIso8253_TestProtocol
    Inherits TestProtocol

    Public Overrides ReadOnly Property Name As String
        Get
            Return "SRT ISO 8253"
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


    Private BallparkStageAdaptiveStepSize As Integer = 5

    Private EndOfBallParkLevelAdjustment As Integer = 5

    Private LargerAdaptiveStepSize As Double = 2
    Private SmallerAdaptiveStepSize As Double = 1

    Private CurrentTestStage As UInteger = 0

    Private NextAdaptiveLevel As Double = 0

    Private FinalThreshold As Double? = Nothing

    Public Property TotalTrialCount As Integer = 20

    Public Overrides Sub InitializeProtocol(ByRef InitialTaskInstruction As NextTaskInstruction)

        'Setting the (initial) speech level specified by the calling code (this should be 20 or 30 dB above the PTA of 0.5, 1 and 2 kHz
        NextAdaptiveLevel = InitialTaskInstruction.AdaptiveValue

        'Setting the initial TestStage to 0 (i.e. Ballpark)
        CurrentTestStage = 0

    End Sub

    Public Overrides Function NewResponse(ByRef TrialHistory As TrialHistory) As NextTaskInstruction

        If TrialHistory.Count = 0 Then
            'This is the start of the test, returns the initial settings
            Return New NextTaskInstruction With {.AdaptiveValue = NextAdaptiveLevel, .TestStage = CurrentTestStage, .Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}
        End If

        Dim ProportionTasksCorrect = TrialHistory.Last.GetProportionTasksCorrect

        If CurrentTestStage = 0 Then

            'We're in the ballbark stage

            'Checks if all taks were correct
            If ProportionTasksCorrect = 1 Then

                'Increasing the level
                NextAdaptiveLevel -= BallparkStageAdaptiveStepSize

            ElseIf ProportionTasksCorrect < 1 Then

                'The ballpark stage is finished, Increasing the level and the CurrentTestStage 
                NextAdaptiveLevel += EndOfBallParkLevelAdjustment
                CurrentTestStage += 1

            Else
                Throw New Exception("Proportion correct exceeding 100%, this is a bug. Please report to the developer!")
            End If

        Else


            'We're in the main adaptive stage

            'Counting the number of trials after the ballpark stage
            Dim NumberTrialsAfterBallparkStage As Integer = 0
            For Each Trial In TrialHistory
                If Trial.TestStage > 0 Then
                    NumberTrialsAfterBallparkStage += 1
                End If
            Next

            'Checking if it's a sentence test
            If TrialHistory(TrialHistory.Count - 1).Tasks > 1 Then
                If NumberTrialsAfterBallparkStage = 6 Then
                    'Goes to next test stage
                    CurrentTestStage += 1
                End If
            End If

            'Selecting step size
            Dim CurrentAdaptiveStep As Double = LargerAdaptiveStepSize
            If CurrentTestStage = 2 Then
                'Changing to lower step size for the remaining sentences
                CurrentAdaptiveStep = SmallerAdaptiveStepSize
            End If

            'Determines adaptive change
            Select Case ProportionTasksCorrect
                Case 0.5
                'This only happens when there are multiple tasks
                'Leaving the level onchanged and returns

                Case > 0.5
                    'Decreasing the level (making the test more difficult)
                    NextAdaptiveLevel -= CurrentAdaptiveStep

                Case Else

                    'Increasing the level (making the test more easy)
                    NextAdaptiveLevel += CurrentAdaptiveStep

            End Select

            'Checking if test is complete (presenting max number of trials)
            If TrialHistory.Count >= TotalTrialCount Then

                'Exits the test
                Return New NextTaskInstruction With {.AdaptiveValue = NextAdaptiveLevel, .TestStage = CurrentTestStage, .Decision = SpeechTest.SpeechTestReplies.TestIsCompleted}

            End If

        End If

        'Continues the test
        Return New NextTaskInstruction With {.AdaptiveValue = NextAdaptiveLevel, .TestStage = CurrentTestStage, .Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}

    End Function

    Public Overrides Sub FinalizeProtocol(ByRef TrialHistory As TrialHistory)

        Dim LevelList As New List(Of Double)
        Dim SkippedSentences As Integer = 0
        For Each Trial In TrialHistory
            If Trial.TestStage > 0 Then
                If SkippedSentences > 2 Then
                    LevelList.Add(DirectCast(Trial, SrtTrial).AdaptiveValue)
                Else
                    SkippedSentences += 1
                End If
            End If
        Next

        'And adding the last non-presented trial level
        LevelList.Add(NextAdaptiveLevel)

        'Getting the average
        If LevelList.Count > 0 Then
            FinalThreshold = LevelList.Average
        Else
            FinalThreshold = Double.NaN
        End If

    End Sub

    Public Overrides Function GetResults(ByRef TrialHistory As TrialHistory) As TestResults

        Dim Output = New TestResults(TestResults.TestResultTypes.SRT)
        If FinalThreshold.HasValue Then
            Output.AdaptiveLevelThreshold = FinalThreshold
        Else
            'Storing NaN if no threshold was reached
            Output.AdaptiveLevelThreshold = Double.NaN
        End If

        'Storing the AdaptiveLevelSeries
        Output.AdaptiveLevelSeries = New List(Of Double)
        Output.SpeechLevelSeries = New List(Of Double)
        Output.MaskerLevelSeries = New List(Of Double)
        Output.SNRLevelSeries = New List(Of Double)
        Output.TestStageSeries = New List(Of String)
        Output.ProportionCorrectSeries = New List(Of String)

        'Trial.IsCorrect  is not used
        'Output.ScoreSeries = New List(Of String)
        For Each Trial As SrtTrial In TrialHistory
            Output.AdaptiveLevelSeries.Add(Math.Round(Trial.AdaptiveValue))
            Output.SpeechLevelSeries.Add(Math.Round(Trial.SpeechLevel))
            Output.MaskerLevelSeries.Add(Math.Round(Trial.MaskerLevel))
            Output.SNRLevelSeries.Add(Math.Round(Trial.SNR))
            Output.TestStageSeries.Add(Trial.TestStage)
            Output.ProportionCorrectSeries.Add(Trial.GetProportionTasksCorrect)
        Next

        Return Output

    End Function


End Class