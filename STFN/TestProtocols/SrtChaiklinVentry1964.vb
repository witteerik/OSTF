﻿Public Class SrtChaiklinVentry1964
    Inherits TestProtocol

    Public Overrides ReadOnly Property Name As String
        Get
            Return "Chaiklin-Ventry (1964)"
        End Get
    End Property

    Private _StoppingCriterium As StoppingCriteria = StoppingCriteria.ThresholdReached

    Public Overrides Property StoppingCriterium As StoppingCriteria
        Get
            Return _StoppingCriterium
        End Get
        Set(value As StoppingCriteria)
            _StoppingCriterium = value
        End Set
    End Property

    Private TestStageMaxTrialCount As Integer = 6

    Private TestStageScoreThreshold As Integer = 3

    Private BallparkStageAdaptiveStepSize As Integer = 5

    Private EndOfBallParkLevelAdjustment As Integer = 10

    Private AdaptiveStepSize As Double = 5

    Private CurrentTestStage As UInteger = 0

    Private NextSpeechLevel As Double = 0

    Private FinalThreshold As Double? = Nothing

    Public Overrides Sub InitializeProtocol(ByRef InitialTaskInstruction As NextTaskInstruction)

        'Setting the (initial) speech level specified by the calling code
        NextSpeechLevel = InitialTaskInstruction.AdaptiveValue

        'Setting a default value for InitialTaskInstruction.AdaptiveStepSize
        If InitialTaskInstruction.AdaptiveStepSize.HasValue = False Then
            InitialTaskInstruction.AdaptiveStepSize = 5
        End If

        'And storing that value
        AdaptiveStepSize = InitialTaskInstruction.AdaptiveStepSize

        If AdaptiveStepSize = 2 Or AdaptiveStepSize = 5 Then
            'OK
        Else
            MsgBox("An adaptive step size of " & AdaptiveStepSize & " dB in the " & Name & " method may not have been validated. However, you can still use it!", , "Unvalidated adaptive step size setting.")
        End If

        'Setting the initial TestStage to 0 (i.e. Ballpark)
        CurrentTestStage = 0

    End Sub

    Public Overrides Function NewResponse(ByRef TrialHistory As TrialHistory) As NextTaskInstruction

        If TrialHistory.Count = 0 Then
            'This is the start of the test, returns the initial settings
            Return New NextTaskInstruction With {.AdaptiveValue = NextSpeechLevel, .TestStage = CurrentTestStage, .Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}
        End If

        If CurrentTestStage = 0 Then
            'The ballpark stage

            If TrialHistory(TrialHistory.Count - 1).Score = 0 Then
                CurrentTestStage = 1
                NextSpeechLevel += EndOfBallParkLevelAdjustment
                Return New NextTaskInstruction With {.AdaptiveValue = NextSpeechLevel, .TestStage = CurrentTestStage, .Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}
            Else
                NextSpeechLevel -= BallparkStageAdaptiveStepSize
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
                        StageScore += Trial.Score
                    End If
                Next
                TestStageResults.Add(New Tuple(Of Integer, Integer)(StageTrials, StageScore))
            Next

            'Checking if we have at least TestStageScoreThreshold correct responses
            If TestStageResults(TestStageResults.Count - 1).Item2 >= TestStageScoreThreshold Then
                'Adjusting level and going to the next stage
                CurrentTestStage += 1
                NextSpeechLevel -= AdaptiveStepSize
                Return New NextTaskInstruction With {.AdaptiveValue = NextSpeechLevel, .TestStage = CurrentTestStage, .Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}

                'Checking if the current stage is at its maximum number of trials
            ElseIf TestStageResults(TestStageResults.Count - 1).Item1 = TestStageMaxTrialCount Then

                'The test stage is complete without reaching the required number of correct trials

                'Setting the SRT to the level above this stage, but only if it's the first time the code reaches this point
                If FinalThreshold.HasValue = False Then FinalThreshold = NextSpeechLevel + AdaptiveStepSize

                'The method requires that all trials in the last stage are incorrect. Since this work badly with MAFC responses, this implementation allows for skipping of that criterium
                Select Case StoppingCriterium
                    Case StoppingCriteria.AllIncorrect

                        If TestStageResults(TestStageResults.Count - 1).Item2 = 0 Then
                            'The last stage had a score of zero, and therefore the test is finished
                            Return New NextTaskInstruction With {.AdaptiveValue = NextSpeechLevel, .TestStage = CurrentTestStage, .Decision = SpeechTest.SpeechTestReplies.TestIsCompleted}

                        Else
                            'The last stage had at least one correct trial, continuing to next stage
                            CurrentTestStage += 1
                            NextSpeechLevel -= AdaptiveStepSize
                            Return New NextTaskInstruction With {.AdaptiveValue = NextSpeechLevel, .TestStage = CurrentTestStage, .Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}
                        End If

                    Case StoppingCriteria.ThresholdReached

                        'Skipping the All-incorrect criterium and ends the test
                        Return New NextTaskInstruction With {.AdaptiveValue = NextSpeechLevel, .TestStage = CurrentTestStage, .Decision = SpeechTest.SpeechTestReplies.TestIsCompleted}

                    Case Else
                        Throw New NotImplementedException("The (test protocol) stopping criterium " & StoppingCriterium & " has not been implemented for the " & Name & " method.")
                End Select

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
            Output.ScoreSeries.Add(Trial.Score)
        Next

        Return Output
    End Function


End Class