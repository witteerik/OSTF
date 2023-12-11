Public Class SrtIso8253
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

    Private NextSpeechLevel As Double = 0

    Private FinalThreshold As Double? = Nothing

    Public Overrides Sub InitializeProtocol(ByRef InitialTaskInstruction As NextTaskInstruction)

        'Setting the (initial) speech level specified by the calling code (this should be 20 or 30 dB above the PTA of 0.5, 1 and 2 kHz
        NextSpeechLevel = InitialTaskInstruction.AdaptiveValue

        'Setting the initial TestStage to 0 (i.e. Ballpark)
        CurrentTestStage = 0

    End Sub

    Public Overrides Function NewResponse(ByRef TrialHistory As TrialHistory) As NextTaskInstruction

        If TrialHistory.Count = 0 Then
            'This is the start of the test, returns the initial settings
            Return New NextTaskInstruction With {.AdaptiveValue = NextSpeechLevel, .TestStage = CurrentTestStage, .Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}
        End If

        Dim ProportionTasksCorrect = TrialHistory.Last.GetProportionTasksCorrect

        If CurrentTestStage = 0 Then

            'Checks if all taks were correct
            If ProportionTasksCorrect = 1 Then

                'Increasing the level
                NextSpeechLevel -= BallparkStageAdaptiveStepSize
                Return New NextTaskInstruction With {.AdaptiveValue = NextSpeechLevel, .TestStage = CurrentTestStage, .Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}

            ElseIf ProportionTasksCorrect < 1 Then

                'The ballpark stage is finished, Increasing the level and the CurrentTestStage 
                NextSpeechLevel += EndOfBallParkLevelAdjustment
                CurrentTestStage += 1
                Return New NextTaskInstruction With {.AdaptiveValue = NextSpeechLevel, .TestStage = CurrentTestStage, .Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}

            Else
                Throw New Exception("Proportion correct exceeding 100%, this is a bug. Please report to the developer!")
            End If
        End If

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
            If NumberTrialsAfterBallparkStage = 5 Then
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
                Return New NextTaskInstruction With {.AdaptiveValue = NextSpeechLevel, .TestStage = CurrentTestStage, .Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}

            Case > 0.5
                'Decreasing the level (making the test more difficult)
                NextSpeechLevel -= CurrentAdaptiveStep
                Return New NextTaskInstruction With {.AdaptiveValue = NextSpeechLevel, .TestStage = CurrentTestStage, .Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}

            Case Else

                'Increasing the level (making the test more easy)
                NextSpeechLevel += CurrentAdaptiveStep
                Return New NextTaskInstruction With {.AdaptiveValue = NextSpeechLevel, .TestStage = CurrentTestStage, .Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}

        End Select

        'Checking if test is complete
        If XXX Then

            Dim LevelList As New List(Of Double)
            Dim SkippedSentences As Integer = 0
            For Each Trial In TrialHistory
                If Trial.TestStage > 0 Then
                    If SkippedSentences > 2 Then
                        LevelList.Add(DirectCast(Trial, SrtTrial).SpeechLevel)
                    Else
                        SkippedSentences += 1
                    End If
                End If
            Next

            'And adding the last non-presented trial level
            LevelList.Add(NextSpeechLevel)

            'Getting the average
            If LevelList.Count > 0 Then
                FinalThreshold = LevelList.Average
            Else
                FinalThreshold = Double.NaN
            End If

        End If

        Return New NextTaskInstruction With {.AdaptiveValue = NextSpeechLevel, .TestStage = CurrentTestStage, .Decision = SpeechTest.SpeechTestReplies.TestIsCompleted}


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
        Output.ScoreSeries = New List(Of String)
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