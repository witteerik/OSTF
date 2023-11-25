Public Class SrtSpeechTest

    Inherits SpeechTest

    Private PlannedTestWords As List(Of SpeechMaterialComponent)

    Private NextSpeechLevel As Double = 0

    Private ObservedTrials As List(Of SrtTrial)

    Public Enum TestStage
        Search
        Fixed1
        Fixed2
    End Enum

    Private CurrentTestStage As TestStage = TestStage.Search

#Region "Settings"

    Public SelectedMediaSet As MediaSet

    Public StartList As String = ""

    Public StartLevel As Double

    Public FixedStageTrialCount As Integer = 10

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

#End Region

    Public Sub New(ByVal SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)

        'Some initial settings which should be overridden by the settings editor
        SelectedMediaSet = GetAvailableMediasets(0)
        StartList = "Lista 1"
        FixedResponseAlternativeCount = 4
        StartLevel = 40

    End Sub

    Public Overrides Function InitializeCurrentTest() As Boolean

        ObservedTrials = New List(Of SrtTrial)

        'Adding four lists, starting from the start list
        Dim TempAvailableLists As New List(Of SpeechMaterialComponent)
        Dim AllLists = SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List, True, False)
        For i = 0 To 1
            TempAvailableLists.AddRange(AllLists)
        Next
        Dim ListsToAdd As New List(Of SpeechMaterialComponent)
        Dim SelectedStartListIndex As Integer = -1
        For i = 0 To AllLists.Count - 1
            If AllLists(i).PrimaryStringRepresentation = StartList Then
                SelectedStartListIndex = i
                Exit For
            End If
        Next
        Dim ListsToUse As New List(Of SpeechMaterialComponent)
        If SelectedStartListIndex > -1 Then
            For i = SelectedStartListIndex To Math.Min(SelectedStartListIndex + 3, AllLists.Count - 1)
                ListsToUse.Add(TempAvailableLists(i))
            Next
        Else
            'This should not happen unless there are no lists loaded!
            Return False
        End If

        'Adding all planned test words
        PlannedTestWords = New List(Of SpeechMaterialComponent)
        For Each List In ListsToUse
            Dim CurrentWords = List.GetChildren()

            If RandomizeWordsWithinLists = False Then
                PlannedTestWords.AddRange(CurrentWords)
            Else
                'Randomizing order
                Dim RandomizedOrder = Utils.SampleWithoutReplacement(CurrentWords.Count, 0, CurrentWords.Count, Randomizer)
                For Each RandomIndex In RandomizedOrder
                    PlannedTestWords.Add(CurrentWords(RandomIndex))
                Next
            End If
        Next

        'Setting the speech level to be presented in the next trial
        NextSpeechLevel = StartLevel

        'Setting the initial TestStage to Search
        CurrentTestStage = TestStage.Search

        Return True

    End Function


    Public Overrides Function GetSpeechTestReply(sender As Object, e As SpeechTestInputEventArgs) As SpeechTestReplies

        If e IsNot Nothing Then

            'This is an incoming test trial response
            'Correcting response
            If e.LinguisticResponse = CurrentTestTrial.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling") Then
                'Correct
                CurrentTestTrial.Score = 1
            Else
                'Not correct
                CurrentTestTrial.Score = 0
            End If

            ObservedTrials.Add(CurrentTestTrial)

        Else
            'Nothing to correct (this should be the start of a new test)
        End If

        Return PrepareNextTrial()

    End Function

    Private Function PrepareNextTrial() As SpeechTestReplies

        'Getting next test word
        Dim NextTestWord = PlannedTestWords(ObservedTrials.Count)

        'Creating a new test trial
        CurrentTestTrial = New SrtTrial With {.SpeechMaterialComponent = NextTestWord}

        If IsFreeRecall Then
            CurrentTestTrial.ResponseAlternativeSpellings = New List(Of String) From {"Rätt", "Fel"}
        Else
            'Adding the current word pselling as a response alternative
            Dim ResponseAlternatives As New List(Of String)
            ResponseAlternatives.Add(CurrentTestTrial.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling"))

            'Picking random response alternatives from all available test words
            Dim AllContrastingWords = NextTestWord.GetAllRelativesAtLevelExludingSelf(SpeechMaterialComponent.LinguisticLevels.Sentence, True, False)
            Dim RandomIndices = Utils.SampleWithoutReplacement(Math.Max(0, FixedResponseAlternativeCount - 1), 0, AllContrastingWords.Count, Randomizer)
            For Each RandomIndex In RandomIndices
                ResponseAlternatives.Add(AllContrastingWords(RandomIndex).GetCategoricalVariableValue("Spelling"))
            Next

            'Shuffling the order of response alternatives
            CurrentTestTrial.ResponseAlternativeSpellings = Utils.Shuffle(ResponseAlternatives, Randomizer)
        End If

        'Calculating the speech level
        Dim NextLevelResult = CalculateNextSpeechLevel()

        'Updating test trial test stage and speech level (as this may have been changed by CalculateNextSpeechLevel
        DirectCast(CurrentTestTrial, SrtTrial).AdaptiveStage = CurrentTestStage
        DirectCast(CurrentTestTrial, SrtTrial).SpeechLevel = NextSpeechLevel

        ' Returning if we should not move to the next trial
        If NextLevelResult <> SpeechTestReplies.GotoNextTrial Then
            Return NextLevelResult
        End If

        'Mixing trial sound
        MixNextTrialSound()

        'Setting trial events
        CurrentTestTrial.TrialEventList = New List(Of ResponseViewEvent)
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 500, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 501, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 9500, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 9700, .Type = ResponseViewEvent.ResponseViewEventTypes.HideAll})

        Return SpeechTestReplies.GotoNextTrial

    End Function


    Private Function CalculateNextSpeechLevel() As SpeechTestReplies

        'Determines test stage
        Dim TrialsInFixedStage1 As New List(Of SrtTrial)
        Dim TrialsInFixedStage2 As New List(Of SrtTrial)
        For Each Trial In ObservedTrials
            If Trial.AdaptiveStage = TestStage.Fixed1 Then TrialsInFixedStage1.Add(Trial)
            If Trial.AdaptiveStage = TestStage.Fixed2 Then TrialsInFixedStage2.Add(Trial)
        Next

        'Checking if all trials in fixed stage 2 have been run
        If TrialsInFixedStage2.Count = FixedStageTrialCount Then
            Return SpeechTestReplies.TestIsCompleted
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
                        Return SpeechTestReplies.TestIsCompleted

                    Case > 0.5
                        'Decreasing level by InterFixedStageLevelAdjustment
                        NextSpeechLevel -= InterFixedStageLevelAdjustment

                        'And incrementing Test stage
                        CurrentTestStage = TestStage.Fixed2

                    Case Else
                        'Increasing level by InterFixedStageLevelAdjustment
                        NextSpeechLevel += InterFixedStageLevelAdjustment

                        'And incrementing Test stage
                        CurrentTestStage = TestStage.Fixed2
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
                    Return SpeechTestReplies.AbortTest
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
                            CurrentTestStage = TestStage.Fixed1
                        End If
                    End If
                End If
            End If
        End If

        Return SpeechTestReplies.GotoNextTrial

    End Function

    Private Sub MixNextTrialSound()

        Dim TestWordSound = CurrentTestTrial.SpeechMaterialComponent.GetSound(SelectedMediaSet, 0, 1, , , , , False, False, False, , , False)

        'Dim TestWordSound = Audio.Sound.LoadWaveFile("C:\Temp10\M_000_004_Klas.wav")

        'Setting level
        Audio.DSP.MeasureAndAdjustSectionLevel(TestWordSound, Audio.Standard_dBSPL_To_dBFS(NextSpeechLevel))

        'Copying to stereo and storing in CurrentTestTrial.Sound 
        CurrentTestTrial.Sound = TestWordSound.ConvertMonoToMultiChannel(2, True)

    End Sub


    Public Overrides Function SaveResults(TestResults As TestResults) As Boolean

        'Not finished!

        Return True
    End Function


    Public Overrides Function GetResults() As TestResults

        Dim Output = New TestResults(TestResults.TestResultTypes.SRT)

        ' Calculating speech recognition threshold
        Dim TrialsInFixedStage1 As New List(Of SrtTrial)
        Dim Stage1Level As Double
        Dim TrialsInFixedStage2 As New List(Of SrtTrial)
        Dim Stage2Level As Double
        For Each Trial In ObservedTrials
            If Trial.AdaptiveStage = TestStage.Fixed1 Then
                TrialsInFixedStage1.Add(Trial)
                Stage1Level = Trial.SpeechLevel
            End If
            If Trial.AdaptiveStage = TestStage.Fixed2 Then
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
                Dim k = (Stage2Level - Stage1Level) / (Stage2Score - Stage1Score)
                Dim m = Stage1Score / (k * Stage1Level)
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
        For Each Trial In ObservedTrials
            Output.SpeechLevelSeries.Add(Math.Round(Trial.SpeechLevel))
            Output.TestStageSeries.Add(Trial.AdaptiveStage.ToString)
            Output.ScoreSeries.Add(Trial.Score)
        Next

        Return Output
    End Function
End Class

Public Class TestResults

    Public ReadOnly TestResultType As TestResultTypes

    Public Enum TestResultTypes
        SRT
    End Enum

    Public SpeechRecognitionThreshold As Double
    Public TestStageSeries As List(Of String)
    Public SpeechLevelSeries As List(Of Double)
    Public ScoreSeries As List(Of Integer)

    Public Sub New(ByVal TestResultType As TestResultTypes)
        Me.TestResultType = TestResultType
    End Sub

    Public Function GetFormattedTestResultsSummaryString() As String

        Dim ResultsRowList = New List(Of String)

        Select Case TestResultType
            Case TestResultTypes.SRT
                ResultsRowList.Add("Speech recognition threshold:" & vbCrLf & Math.Round(SpeechRecognitionThreshold) & " dB SPL")
                ResultsRowList.Add("Test stage:" & vbCrLf & String.Join(vbTab, TestStageSeries))
                ResultsRowList.Add("Speech level:" & vbCrLf & String.Join(vbTab, SpeechLevelSeries))
                ResultsRowList.Add("Trial score:" & vbCrLf & String.Join(vbTab, ScoreSeries))

            Case Else


        End Select

        Return String.Join(vbCrLf, ResultsRowList)

    End Function

End Class



