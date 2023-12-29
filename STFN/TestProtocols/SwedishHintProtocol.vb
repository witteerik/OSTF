Public Class SwedishHintProtocol
    Inherits TestProtocol

    Public Overrides ReadOnly Property Name As String
        Get
            Return "Swedish HINT"
        End Get
    End Property

    Public Overrides Property StoppingCriterium As StoppingCriteria
        Get
            Return StoppingCriteria.TrialCount
        End Get
        Set(value As StoppingCriteria)
            MsgBox("An attempt was made to change the test protocol stopping criterium. The " & Name & " procedure requires that the stopping criterium is 'TrialCount'. Ignoring the attempt to set the new value.", , "Unsupported stopping criterium.")
        End Set
    End Property


    Private LargerAdaptiveStepSize As Double = 2

    Private NextAdaptiveLevel As Double = 0

    Public ReadOnly Property TotalTrialCount As Integer
        Get
            If IsInPractiseMode = True Then
                Return 10
            Else
                Return 20
            End If
        End Get
    End Property



    Private FinalThreshold As Double? = Nothing

    Public Overrides Sub InitializeProtocol(ByRef InitialTaskInstruction As NextTaskInstruction)

        NextAdaptiveLevel = InitialTaskInstruction.AdaptiveValue

    End Sub

    Public Overrides Function NewResponse(ByRef TrialHistory As TrialHistory) As NextTaskInstruction

        If TrialHistory.Count = 0 Then
            'This is the start of the test, returns the initial settings
            Return New NextTaskInstruction With {.AdaptiveValue = NextAdaptiveLevel,
                .Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}
        End If

        Dim ProportionTasksCorrect = TrialHistory.Last.GetProportionTasksCorrect

        'Determines adaptive change
        Select Case ProportionTasksCorrect
            Case 0.5
                'This only happens when there are multiple tasks
                'Leaving the level onchanged and returns

            Case > 0.5
                'Decreasing the level (making the test more difficult)
                NextAdaptiveLevel -= LargerAdaptiveStepSize

            Case Else
                'Increasing the level (making the test more easy)
                NextAdaptiveLevel += LargerAdaptiveStepSize

        End Select

        'Checking if test is complete (presenting max number of trials)
        If TrialHistory.Count >= TotalTrialCount Then

            'Exits the test
            Return New NextTaskInstruction With {.AdaptiveValue = NextAdaptiveLevel, .Decision = SpeechTest.SpeechTestReplies.TestIsCompleted}

        End If

        'Continues the test
        Return New NextTaskInstruction With {.AdaptiveValue = NextAdaptiveLevel, .Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}

    End Function

    Public Overrides Sub CalculateResult(ByRef TrialHistory As TrialHistory)

        If IsInPractiseMode = True Then
            'Storing the last value as threshold
            FinalThreshold = DirectCast(TrialHistory.Last, SrtTrial).SNR

        Else
            'Calculating threshold
            Dim LevelList As New List(Of Double)
            For i As Integer = 4 To 20
                LevelList.Add(DirectCast(TrialHistory(i), SrtTrial).SNR)
            Next

            'Getting the average
            If LevelList.Count > 0 Then
                FinalThreshold = LevelList.Average
            Else
                FinalThreshold = Double.NaN
            End If
        End If

    End Sub

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