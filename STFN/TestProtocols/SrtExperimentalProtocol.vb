﻿
Imports STFN.SpeechTest

Public Class SrtExperimentalProtocol
    Inherits TestProtocol

    Public Overrides ReadOnly Property Name As String
        Get
            Return "SRT Experimental Test Protocol"
        End Get
    End Property

#Region "Protocol-specific settings"

    Public FixedStageTrialCount As Integer = 5

    Private _SearchStageMinimumTrialCount As Integer = 6
    Public Property SearchStageMinimumTrialCount As Integer
        Get
            Return _SearchStageMinimumTrialCount
        End Get
        Set(value As Integer)
            'Limiting the value to one or above
            _SearchStageMinimumTrialCount = Math.Max(1, value)
        End Set
    End Property

    Public SearchStageThresholdDeviation As Double = 0.17

    Public MaximumSearchStageLength As Integer = 20

    Public SearchStageLevelAdjustment As Integer = 5

    Public InterFixedStageLevelAdjustment As Double = 10

    Private CurrentTestStage As UInteger = 0

    Private NextSpeechLevel As Double = 0


#End Region


    Public Overrides Sub InitializeProtocol(ByVal InitialTaskInstruction As NextTaskInstruction)

        'Setting the speech level to be presented in the next trial
        NextSpeechLevel = InitialTaskInstruction.AdaptiveValue

        'Setting the initial TestStage to 0 (i.e. Ballpark)
        CurrentTestStage = InitialTaskInstruction.TestStage

    End Sub


    Public Overrides Function NewResponse(ByRef ObservedTrials As TrialHistory) As NextTaskInstruction


        'Determines test stage
        Dim TrialsInFixedStage1 As New List(Of SrtTrial)
        Dim TrialsInFixedStage2 As New List(Of SrtTrial)
        For Each Trial As SrtTrial In ObservedTrials
            If Trial.TestStage = 1 Then TrialsInFixedStage1.Add(Trial)
            If Trial.TestStage = 2 Then TrialsInFixedStage2.Add(Trial)
        Next

        'Checking if all trials in fixed stage 2 have been run
        If TrialsInFixedStage2.Count = FixedStageTrialCount Then
            Return New NextTaskInstruction With {.Decision = SpeechTestReplies.TestIsCompleted}
        End If

        'Checking if fixed stage 1 is complete
        If TrialsInFixedStage1.Count = FixedStageTrialCount Then

            'Check if we have no trials in fixed stage 2
            If TrialsInFixedStage2.Count = 0 Then

                'Evaluating the results of fixed stage 1 
                Select Case GetAverageScore(TrialsInFixedStage1)
                    Case 0.5

                        'We have exacly 50 % correct in first stage
                        'Quitting the test as the threshold has been detected
                        Return New NextTaskInstruction With {.Decision = SpeechTestReplies.TestIsCompleted}

                    Case > 0.5
                        'Decreasing level by InterFixedStageLevelAdjustment
                        NextSpeechLevel -= InterFixedStageLevelAdjustment

                        'And incrementing Test stage
                        CurrentTestStage = 2

                    Case Else
                        'Increasing level by InterFixedStageLevelAdjustment
                        NextSpeechLevel += InterFixedStageLevelAdjustment

                        'And incrementing Test stage
                        CurrentTestStage = 2
                End Select
            Else
                'We've in the middle of fixed stage 2, no need to alter the level. Just continueing.
            End If

        ElseIf TrialsInFixedStage1.Count > 0 Then
            'We've in the middle of fixed stage 1, no need to alter the level. Just continueing.
        Else

            'We're in the Search stage
            'Checking if it's the first trial
            If ObservedTrials.Count = 0 Then
                'Do nothing
            Else

                'Adjusting the speech level, depending on the last response
                If ObservedTrials.Last.Score = 1 Then
                    NextSpeechLevel -= SearchStageLevelAdjustment
                Else
                    NextSpeechLevel += SearchStageLevelAdjustment
                End If

                'Checking first that we're not past the maximum length of the search stage
                If ObservedTrials.Count >= MaximumSearchStageLength Then
                    Return New NextTaskInstruction With {.Decision = SpeechTestReplies.AbortTest}
                End If

                'We present at least SearchStageMinimumTrialCount trials before we can move to the next stage
                If ObservedTrials.Count > SearchStageMinimumTrialCount Then

                    'Checking if we should move to fixed stage 1
                    'Checking the score of the last six trials.
                    Dim LastTrialList = ObservedTrials.GetRange(ObservedTrials.Count - SearchStageMinimumTrialCount, SearchStageMinimumTrialCount)
                    Dim AverageScore = GetAverageScore(LastTrialList)
                    If AverageScore <= 0.5 + SearchStageThresholdDeviation Then
                        If AverageScore >= 0.5 - SearchStageThresholdDeviation Then
                            'We've in the target score range. Incrementing test stage
                            CurrentTestStage = 1
                        End If
                    End If
                End If
            End If
        End If

        Return New NextTaskInstruction With {.Decision = SpeechTestReplies.GotoNextTrial, .AdaptiveValue = NextSpeechLevel, .TestStage = CurrentTestStage}

    End Function

    Public Overrides Function GetResults(ByRef ObservedTrials As TrialHistory) As TestResults

        Dim Output = New TestResults(TestResults.TestResultTypes.SRT)

        ' Calculating speech recognition threshold
        Dim TrialsInFixedStage1 As New List(Of SrtTrial)
        Dim Stage1Level As Double
        Dim TrialsInFixedStage2 As New List(Of SrtTrial)
        Dim Stage2Level As Double
        For Each Trial As SrtTrial In ObservedTrials
            If Trial.TestStage = 1 Then
                TrialsInFixedStage1.Add(Trial)
                Stage1Level = Trial.SpeechLevel
            End If
            If Trial.TestStage = 2 Then
                TrialsInFixedStage2.Add(Trial)
                Stage2Level = Trial.SpeechLevel
            End If
        Next

        If TrialsInFixedStage2.Count > 0 Then

            Dim Stage1Score = GetAverageScore(TrialsInFixedStage1)
            Dim Stage2Score = GetAverageScore(TrialsInFixedStage2)

            If Stage2Score = Stage1Score Then
                'Using the average level
                Output.SpeechRecognitionThreshold = (Stage1Level + Stage2Level) / 2
            Else
                'Interpolating level for 50 % correct score
                Dim k = (Stage2Score - Stage1Score) / (Stage2Level - Stage1Level)
                Dim m = Stage1Score - (k * Stage1Level)
                If k = 0 Then
                    'Using the average level (actually, the levels should be the same if k = 0!)
                    Output.SpeechRecognitionThreshold = (Stage1Level + Stage2Level) / 2
                Else
                    Output.SpeechRecognitionThreshold = (0.5 - m) / k
                End If
            End If

        Else

            'This means that the score in the first stage was exactly 50 %
            'No need for interpolation, just using stage 1 level as the speech recognition threshold
            Output.SpeechRecognitionThreshold = Stage1Level
        End If

        'Storing the SpeechLevelSeries
        Output.SpeechLevelSeries = New List(Of Double)
        Output.TestStageSeries = New List(Of String)
        Output.ScoreSeries = New List(Of Integer)
        For Each Trial As SrtTrial In ObservedTrials
            Output.SpeechLevelSeries.Add(Math.Round(Trial.SpeechLevel))
            Output.TestStageSeries.Add(Trial.TestStage)
            Output.ScoreSeries.Add(Trial.Score)
        Next

        Return Output


    End Function




End Class
