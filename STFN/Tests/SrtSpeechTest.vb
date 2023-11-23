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

    Public StartLevel As Double = 0

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

    End Sub

    Public Function InitializeCurrentTest() As Boolean

        ObservedTrials = New List(Of SrtTrial)

        'Adding four lists, starting from the start list
        Dim TempAvailableLists As New List(Of SpeechMaterialComponent)
        Dim AllLists = SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.Sentence, True, False)
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


    Public Overrides Function GetNextTrial() As TestTrial
        'Not used in the current derived class
        Return Nothing
    End Function


    Public Overrides Function HandleResponse(sender As Object, e As ResponseGivenEventArgs) As HandleResponseOutcomes

        'Correcting response
        Dim PresentedThing As String = "CorrectWord"

        If e.LinguisticResponse = PresentedThing Then
            CurrentTestTrial.Score = 1
            'Correct
        Else
            'Not correct
            CurrentTestTrial.Score = 0
        End If

        ObservedTrials.Add(CurrentTestTrial)

        Return PrepareNextTrial()

    End Function

    Public Overrides Function PrepareNextTrial() As HandleResponseOutcomes

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
            Dim RandomIndices = Utils.SampleWithoutReplacement(FixedResponseAlternativeCount, 0, AllContrastingWords.Count, Randomizer)
            For Each RandomIndex In RandomIndices
                ResponseAlternatives.Add(AllContrastingWords(RandomIndex).GetCategoricalVariableValue("Spelling"))
            Next

            'Shuffling the order of response alternatives
            CurrentTestTrial.ResponseAlternativeSpellings = Utils.Shuffle(ResponseAlternatives, Randomizer)
        End If

        'Calculating the speech level
        Dim NextLevelResult = CalculateNextSpeechLevel()
        If NextLevelResult <> HandleResponseOutcomes.ContinueTrial Then
            Return NextLevelResult
        End If

        'Mixing trial sound
        MixNextTrialSound()

        'Setting trial events
        CurrentTestTrial.TrialEventList = New List(Of ResponseViewEvent)
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 500, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 501, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 4500, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 4700, .Type = ResponseViewEvent.ResponseViewEventTypes.HideAll})

        Return HandleResponseOutcomes.GotoNextTrial

    End Function


    Private Function CalculateNextSpeechLevel() As HandleResponseOutcomes

        'Determines test stage
        Dim TrialsInFixedStage1 As New List(Of SrtTrial)
        Dim TrialsInFixedStage2 As New List(Of SrtTrial)
        For Each Trial In ObservedTrials
            If Trial.AdaptiveStage = TestStage.Fixed1 Then TrialsInFixedStage1.Add(Trial)
            If Trial.AdaptiveStage = TestStage.Fixed2 Then TrialsInFixedStage2.Add(Trial)
        Next

        'Checking if all trials in fixed stage 2 have been run
        If TrialsInFixedStage2.Count = FixedStageTrialCount Then
            Return HandleResponseOutcomes.TestIsCompleted
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
                        Return HandleResponseOutcomes.TestIsCompleted

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
        Else

            'We're in the Search stage
            'Adjusting the speech level, depending on the last response
            If ObservedTrials.Last.Score = 1 Then
                NextSpeechLevel -= SearchStageLevelAdjustment
            Else
                NextSpeechLevel += SearchStageLevelAdjustment
            End If

            'Checking first that we're not past the maximum length of the search stage
            If ObservedTrials.Count >= MaximumSearchStageLength Then
                Return HandleResponseOutcomes.AbortTest
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

        Return HandleResponseOutcomes.GotoNextTrial

    End Function

    Private Sub MixNextTrialSound()

        Dim TestWordSound = CurrentTestTrial.SpeechMaterialComponent.GetSound(SelectedMediaSet, 0, 1, , , , , False, False, False, , , False)

        'Setting level
        Audio.DSP.MeasureAndAdjustSectionLevel(TestWordSound, Audio.Standard_dBSPL_To_dBFS(NextSpeechLevel))

        'Copying to stereo and storing in CurrentTestTrial.Sound 
        CurrentTestTrial.Sound = TestWordSound.ConvertMonoToMultiChannel(2, True)

    End Sub


    Public Overrides Function SaveResults() As Boolean
        'Throw New NotImplementedException()
        Return True
    End Function

    Public Overrides Function GetResults() As Object
        ' Throw New NotImplementedException()
        Return Nothing
    End Function
End Class



