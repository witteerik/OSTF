Imports System.IO
Imports MathNet.Numerics
Imports MathNet.Numerics.Distributions
Imports STFN.TestProtocol

Public Class HintSpeechTest
    Inherits SpeechTest
    Public Overrides ReadOnly Property FilePathRepresentation As String
        Get
            Return "HINT"
        End Get
    End Property

    ''' <summary>
    ''' This collection contains MaximumNumberOfTestWords which can be used troughout the test, in sequential order.
    ''' </summary>
    Private PlannedTestSentencess As List(Of SpeechMaterialComponent)

    Private MaximumNumberOfTestSentences As Integer = 40

    Private HasNoise As Boolean

    Private ObservedTrials As TrialHistory


#Region "Settings"

    Public Overrides ReadOnly Property TesterInstructions As String
        Get
            Return ""
        End Get
    End Property

    Public Overrides ReadOnly Property ParticipantInstructions As String
        Get
            Return ""
        End Get
    End Property


    Public Overrides ReadOnly Property HasOptionalPractiseTest As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsUseRetsplChoice As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsManualPreSetSelection As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsManualStartListSelection As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsManualMediaSetSelection As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsManualSpeechLevelSelection As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsManualMaskingLevelSelection As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsManualBackgroundLevelSelection As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property SupportsPrelistening As Boolean
        Get
            Return True
        End Get
    End Property


    Public Overrides ReadOnly Property UseSoundFieldSimulation As Utils.TriState
        Get
            Return TriState.True
        End Get
    End Property



    Public Overrides ReadOnly Property AvailableTestModes As List(Of TestModes)
        Get
            'Return New List(Of TestModes) From {TestModes.AdaptiveSpeech, TestModes.AdaptiveNoise}
            Return New List(Of TestModes) From {TestModes.AdaptiveNoise}
        End Get
    End Property

    Public Overrides ReadOnly Property AvailableTestProtocols As List(Of TestProtocol)
        Get
            Return New List(Of TestProtocol) From {New SrtSwedishHint2018_TestProtocol}
        End Get
    End Property

    Public Overrides ReadOnly Property AvailableFixedResponseAlternativeCounts As List(Of Integer)
        Get
            Return New List(Of Integer)
        End Get
    End Property

    Public Overrides ReadOnly Property AvailablePhaseAudiometryTypes As List(Of BmldModes)
        Get
            Return New List(Of BmldModes) From {BmldModes.RightOnly, BmldModes.LeftOnly, BmldModes.BinauralSamePhase, BmldModes.BinauralPhaseInverted, BmldModes.BinauralUncorrelated}
        End Get
    End Property

#End Region

    Public Overrides ReadOnly Property MaximumSoundFieldSpeechLocations As Integer
        Get
            Return 1
        End Get
    End Property

    Public Overrides ReadOnly Property MaximumSoundFieldMaskerLocations As Integer
        Get
            Return 1000
        End Get
    End Property

    Public Overrides ReadOnly Property MaximumSoundFieldBackgroundNonSpeechLocations As Integer
        Get
            Return 1000
        End Get
    End Property

    Public Overrides ReadOnly Property MaximumSoundFieldBackgroundSpeechLocations As Integer
        Get
            Return 1000
        End Get
    End Property

    Public Overrides ReadOnly Property MinimumSoundFieldSpeechLocations As Integer
        Get
            Return 1
        End Get
    End Property

    Public Overrides ReadOnly Property MinimumSoundFieldMaskerLocations As Integer
        Get
            Return 1
        End Get
    End Property

    Public Overrides ReadOnly Property MinimumSoundFieldBackgroundNonSpeechLocations As Integer
        Get
            Return 0
        End Get
    End Property

    Public Overrides ReadOnly Property MinimumSoundFieldBackgroundSpeechLocations As Integer
        Get
            Return 0
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsManualReferenceLevelSelection As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property CanHaveTargets As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property CanHaveMaskers As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property CanHaveBackgroundNonSpeech As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property CanHaveBackgroundSpeech As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property UseKeyWordScoring As Utils.TriState
        Get
            Return Utils.Constants.TriState.True
        End Get
    End Property

    Public Overrides ReadOnly Property UseListOrderRandomization As Utils.TriState
        Get
            Return Utils.Constants.TriState.False
        End Get
    End Property

    Public Overrides ReadOnly Property UseWithinListRandomization As Utils.TriState
        Get
            Return Utils.Constants.TriState.False
        End Get
    End Property

    Public Overrides ReadOnly Property UseAcrossListRandomization As Utils.TriState
        Get
            Return Utils.Constants.TriState.False
        End Get
    End Property

    Public Overrides ReadOnly Property UseFreeRecall As Utils.TriState
        Get
            Return Utils.TriState.True
        End Get
    End Property

    Public Overrides ReadOnly Property UseDidNotHearAlternative As Utils.TriState
        Get
            Return Utils.Constants.TriState.False
        End Get
    End Property

    Public Overrides ReadOnly Property UseContralateralMasking As Utils.TriState
        Get
            Return Utils.Constants.TriState.Optional
        End Get
    End Property

    Public Overrides ReadOnly Property UsePhaseAudiometry As Utils.TriState
        Get
            Return Utils.Constants.TriState.False
        End Get
    End Property

    Public Overrides ReadOnly Property LevelStepSize As Double
        Get
            Return 1
        End Get
    End Property

    Public Overrides ReadOnly Property HistoricTrialCount As Integer
        Get
            Return 0
        End Get
    End Property

    Public Overrides ReadOnly Property SupportsManualPausing As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides Property SoundOverlapDuration As Double = 0.1

    Public Overrides ReadOnly Property MinimumLevel As Double = 35
    Public Overrides ReadOnly Property MaximumLevel As Double = 85

    Public Overrides ReadOnly Property AvailableExperimentNumbers As Integer()
        Get
            Return {}
        End Get
    End Property

    Public Sub New(ByVal SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)

    End Sub

    Public Overrides Function InitializeCurrentTest() As Tuple(Of Boolean, String)

        ObservedTrials = New TrialHistory

        ' Using a fixed speech level
        CustomizableTestOptions.SpeechLevel = 65

        If CustomizableTestOptions.SignalLocations.Count = 0 Then
            Return New Tuple(Of Boolean, String)(False, "You must select at least one signal sound source!")
        End If

        If CustomizableTestOptions.MaskerLocations.Count = 0 And CustomizableTestOptions.SelectedTestMode = TestModes.AdaptiveNoise Then
            Return New Tuple(Of Boolean, String)(False, "You must select at least one masker sound source in tests with adaptive noise!")
        End If

        Dim StartAdaptiveLevel As Double
        If CustomizableTestOptions.MaskerLocations.Count > 0 Then
            'It's a speech in noise test, using adaptive SNR
            HasNoise = True
            Dim InitialSNR = SignalToNoiseRatio(CustomizableTestOptions.SpeechLevel, CustomizableTestOptions.MaskingLevel)
            StartAdaptiveLevel = InitialSNR
        Else
            'It's a speech only test, using adaptive speech level
            HasNoise = False
            StartAdaptiveLevel = CustomizableTestOptions.SpeechLevel
        End If

        CustomizableTestOptions.SelectedTestProtocol.IsInPretestMode = CustomizableTestOptions.IsPractiseTest

        CreatePlannedWordsList()

        CustomizableTestOptions.SelectedTestProtocol.InitializeProtocol(New TestProtocol.NextTaskInstruction With {.TestStage = 0, .AdaptiveValue = StartAdaptiveLevel})

        Return New Tuple(Of Boolean, String)(True, "")

    End Function

    Private Function CreatePlannedWordsList() As Boolean

        'Adding MaximumNumberOfTestSentences words, starting from the start list (excluding practise items), and re-using lists if needed 
        Dim TempAvailableLists As New List(Of SpeechMaterialComponent)
        Dim AllAvailableLists = SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List, False, False)

        Dim AllLists As New List(Of SpeechMaterialComponent)
        'Filtering out lists which are or are not pracise lists depending on the selected value in CustomizableTestOptions
        For Each List In AllAvailableLists
            If List.IsPractiseComponent = CustomizableTestOptions.IsPractiseTest Then
                AllLists.Add(List)
            End If
        Next

        Dim ListCount As Integer = AllLists.Count
        Dim TotalSentenceCount As Integer = 0
        For Each List In AllLists
            TotalSentenceCount += List.ChildComponents.Count
        Next

        'Calculating the number of loops around the material that is needed to get TotalSentenceCount sentences, and adding one loop to compensate for not starting the adding of sentences at the first list
        Dim LoopsNeeded As Integer = Math.Ceiling(TotalSentenceCount / MaximumNumberOfTestSentences) + 1
        'Adding the number of lists needed 
        For i = 1 To LoopsNeeded
            TempAvailableLists.AddRange(AllLists)
        Next
        'Determines the index of the start list
        Dim SelectedStartListIndex As Integer = -1
        For i = 0 To AllLists.Count - 1
            If AllLists(i).PrimaryStringRepresentation = CustomizableTestOptions.StartList Then
                SelectedStartListIndex = i
                Exit For
            End If
        Next
        'Collecting the lists to use, starting with the start list
        Dim ListsToUse As New List(Of SpeechMaterialComponent)
        If SelectedStartListIndex > -1 Then
            For i = SelectedStartListIndex To TempAvailableLists.Count - 1
                ListsToUse.Add(TempAvailableLists(i))
            Next
        Else
            'This should not happen unless there are no lists loaded!
            Messager.MsgBox("Unable to add test sentences, probably since the selected speech material only contains " & TotalSentenceCount & " sentences!",, "An error occurred!")
            Return False
        End If

        'Adding all planned test sentences, and stopping after MaximumNumberOfTestSentences have been added
        PlannedTestSentencess = New List(Of SpeechMaterialComponent)
        Dim TargetNumberOfSentencesReached As Boolean = False
        For Each List In ListsToUse
            Dim CurrentWords = List.GetChildren()

            If CustomizableTestOptions.RandomizeItemsWithinLists = False Then
                For Each Word In CurrentWords
                    PlannedTestSentencess.Add(Word)
                    'Checking if enough sentences have been added
                    If PlannedTestSentencess.Count = MaximumNumberOfTestSentences Then
                        TargetNumberOfSentencesReached = True
                        Exit For
                    End If
                Next
            Else
                'Randomizing order
                Dim RandomizedOrder = Utils.SampleWithoutReplacement(CurrentWords.Count, 0, CurrentWords.Count, Randomizer)
                For Each RandomIndex In RandomizedOrder
                    PlannedTestSentencess.Add(CurrentWords(RandomIndex))
                    'Checking if enough words have been added
                    If PlannedTestSentencess.Count = MaximumNumberOfTestSentences Then
                        TargetNumberOfSentencesReached = True
                        Exit For
                    End If
                Next
            End If

            If TargetNumberOfSentencesReached = True Then
                'Breaking out of the outer loop if we have enough sentences
                Exit For
            End If

        Next

        'Checking that we really have MaximumNumberOfTestSentences words
        If MaximumNumberOfTestSentences <> PlannedTestSentencess.Count Then
            Messager.MsgBox("The wrong number of test items were added. It should have been " & MaximumNumberOfTestSentences & " but instead " & PlannedTestSentencess.Count & " items were added!",, "An error occurred!")
            Return False
        End If

        Return True

    End Function


    Public Overrides Function GetSpeechTestReply(sender As Object, e As SpeechTestInputEventArgs) As SpeechTestReplies

        If e IsNot Nothing Then

            'This is an incoming test trial response

            'Corrects the trial response, based on the given response
            Dim WordsInSentence = CurrentTestTrial.SpeechMaterialComponent.ChildComponents()
            Dim CorrectWordsList As New List(Of String)

            'Resets the CurrentTestTrial.ScoreList
            CurrentTestTrial.ScoreList = New List(Of Integer)
            For i = 0 To e.LinguisticResponses.Count - 1

                If CustomizableTestOptions.ScoreOnlyKeyWords = True Then
                    If WordsInSentence(i).IsKeyComponent = False Then
                        'In keyword correction mode, skipping to next if the word is not a keyword
                        Continue For
                    End If
                End If

                'Correcting the word
                If e.LinguisticResponses(i) = WordsInSentence(i).GetCategoricalVariableValue("Spelling") Then
                    CurrentTestTrial.ScoreList.Add(1)
                Else
                    CurrentTestTrial.ScoreList.Add(0)
                End If

            Next

            'Checks if the trial is finished
            If CurrentTestTrial.ScoreList.Count < CurrentTestTrial.Tasks Then
                'Returns to continue the trial
                Return SpeechTestReplies.ContinueTrial
            End If

            'Adding the test trial
            ObservedTrials.Add(CurrentTestTrial)

        Else
            'Nothing to correct (this should be the start of a new test)
        End If

        'TODO: We must store the responses and response times!!!

        'Calculating the speech level
        Dim ProtocolReply = CustomizableTestOptions.SelectedTestProtocol.NewResponse(ObservedTrials)

        'Preparing next trial if needed
        If ProtocolReply.Decision = SpeechTestReplies.GotoNextTrial Then
            PrepareNextTrial(ProtocolReply)
        End If

        Return ProtocolReply.Decision


    End Function

    Private Sub PrepareNextTrial(ByVal NextTaskInstruction As TestProtocol.NextTaskInstruction)

        'Preparing the next trial
        'Getting next test word
        Dim NextTestWord = PlannedTestSentencess(ObservedTrials.Count)

        'Creating a new test trial
        Select Case CustomizableTestOptions.SelectedTestMode
            Case TestModes.AdaptiveSpeech

                If HasNoise = True Then

                    CurrentTestTrial = New SrtTrial With {.SpeechMaterialComponent = NextTestWord,
            .AdaptiveValue = NextTaskInstruction.AdaptiveValue,
            .SpeechLevel = CustomizableTestOptions.MaskingLevel + NextTaskInstruction.AdaptiveValue,
            .MaskerLevel = CustomizableTestOptions.MaskingLevel,
            .TestStage = NextTaskInstruction.TestStage,
            .Tasks = 1}

                Else

                    CurrentTestTrial = New SrtTrial With {.SpeechMaterialComponent = NextTestWord,
            .AdaptiveValue = NextTaskInstruction.AdaptiveValue,
            .SpeechLevel = NextTaskInstruction.AdaptiveValue,
            .MaskerLevel = Double.NegativeInfinity,
            .TestStage = NextTaskInstruction.TestStage,
            .Tasks = 1}

                End If


            Case TestModes.AdaptiveNoise

                CurrentTestTrial = New SrtTrial With {.SpeechMaterialComponent = NextTestWord,
            .AdaptiveValue = NextTaskInstruction.AdaptiveValue,
            .SpeechLevel = CustomizableTestOptions.SpeechLevel,
            .MaskerLevel = CustomizableTestOptions.SpeechLevel - NextTaskInstruction.AdaptiveValue,
            .TestStage = NextTaskInstruction.TestStage,
            .Tasks = 1}

            Case Else
                Throw New NotImplementedException
        End Select


        CurrentTestTrial.ResponseAlternativeSpellings = New List(Of List(Of SpeechTestResponseAlternative))

        Dim ResponseAlternatives As New List(Of SpeechTestResponseAlternative)
        If CustomizableTestOptions.IsFreeRecall Then
            If CurrentTestTrial.SpeechMaterialComponent.ChildComponents.Count > 0 Then

                CurrentTestTrial.Tasks = 0
                For Each Child In CurrentTestTrial.SpeechMaterialComponent.ChildComponents()

                    If CustomizableTestOptions.ScoreOnlyKeyWords = True Then
                        Dim IsKeyComponent = Child.IsKeyComponent
                        ResponseAlternatives.Add(New SpeechTestResponseAlternative With {.Spelling = Child.GetCategoricalVariableValue("Spelling"), .IsScoredItem = IsKeyComponent})
                        If IsKeyComponent = True Then
                            CurrentTestTrial.Tasks += 1
                        End If
                    Else
                        ResponseAlternatives.Add(New SpeechTestResponseAlternative With {.Spelling = Child.GetCategoricalVariableValue("Spelling"), .IsScoredItem = True})
                    End If

                Next
            End If

        Else
            Throw New NotImplementedException
        End If

        CurrentTestTrial.ResponseAlternativeSpellings.Add(ResponseAlternatives)

        'Mixing trial sound
        MixNextTrialSound()

        'Setting trial events
        CurrentTestTrial.TrialEventList = New List(Of ResponseViewEvent)
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 1000, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 1001, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})

    End Sub



    Private Sub MixNextTrialSound()

        Dim RETSPL_Correction As Double = 0
        If CustomizableTestOptions.UseRetsplCorrection = True Then
            RETSPL_Correction = CustomizableTestOptions.SelectedTransducer.RETSPL_Speech
        End If

        Dim TestWordSound = CurrentTestTrial.SpeechMaterialComponent.GetSound(CustomizableTestOptions.SelectedMediaSet, 0, 1, , , , , False, False, False, , , False)

        Dim NominalLevel_FS = TestWordSound.SMA.NominalLevel
        Dim TargetLevel_FS = Audio.Standard_dBSPL_To_dBFS(DirectCast(CurrentTestTrial, SrtTrial).SpeechLevel) + RETSPL_Correction
        Dim NeededGain = TargetLevel_FS - NominalLevel_FS

        Audio.DSP.AmplifySection(TestWordSound, NeededGain)

        'Setting level
        If HasNoise = True Then
            Dim Noise = CurrentTestTrial.SpeechMaterialComponent.GetMaskerSound(CustomizableTestOptions.SelectedMediaSet, 0)
            Audio.DSP.MeasureAndAdjustSectionLevel(Noise, Audio.Standard_dBSPL_To_dBFS(DirectCast(CurrentTestTrial, SrtTrial).MaskerLevel))

            Dim MixedSound = Audio.DSP.SuperpositionSounds({TestWordSound, Noise}.ToList)

            'Copying to stereo and storing in CurrentTestTrial.Sound 
            CurrentTestTrial.Sound = MixedSound.ConvertMonoToMultiChannel(2, True)
        Else

            'Copying to stereo and storing in CurrentTestTrial.Sound 
            CurrentTestTrial.Sound = TestWordSound.ConvertMonoToMultiChannel(2, True)

        End If

        'Also stores the mediaset
        CurrentTestTrial.MediaSetName = CustomizableTestOptions.SelectedMediaSet.MediaSetName

        'And the contralateral noise on/off setting
        CurrentTestTrial.UseContralateralNoise = CustomizableTestOptions.UseContralateralMasking

        'And the EM term
        CurrentTestTrial.EfficientContralateralMaskingTerm = CustomizableTestOptions.SelectedMediaSet.EffectiveContralateralMaskingGain

    End Sub

    Public Overrides Sub FinalizeTest()

        CustomizableTestOptions.SelectedTestProtocol.FinalizeProtocol(ObservedTrials)

    End Sub

    'Public Overrides Function GetResults() As TestResults

    '    Dim ProtocolThreshold = CustomizableTestOptions.SelectedTestProtocol.GetFinalResult()

    '    Dim RawResults = New TestResults(TestResults.TestResultTypes.SRT)
    '    If ProtocolThreshold.HasValue Then
    '        RawResults.AdaptiveLevelThreshold = ProtocolThreshold
    '    Else
    '        'Storing NaN if no threshold was reached
    '        RawResults.AdaptiveLevelThreshold = Double.NaN
    '    End If

    '    'Calculating the SRT based on the adaptive threshold
    '    Select Case CustomizableTestOptions.SelectedTestMode
    '        Case TestModes.AdaptiveSpeech
    '            RawResults.SpeechRecognitionThreshold = RawResults.AdaptiveLevelThreshold
    '        Case TestModes.AdaptiveNoise
    '            RawResults.SpeechRecognitionThreshold = RawResults.AdaptiveLevelThreshold
    '        Case Else
    '            Throw New NotImplementedException
    '    End Select

    '    'Storing the AdaptiveLevelSeries
    '    RawResults.AdaptiveLevelSeries = New List(Of Double)
    '    RawResults.SpeechLevelSeries = New List(Of Double)
    '    RawResults.MaskerLevelSeries = New List(Of Double)
    '    RawResults.SNRLevelSeries = New List(Of Double)
    '    RawResults.TestStageSeries = New List(Of String)
    '    RawResults.ProportionCorrectSeries = New List(Of String)
    '    'Trial.IsCorrect  is not used
    '    'RawResults.ScoreSeries = New List(Of String)
    '    For Each Trial As SrtTrial In ObservedTrials
    '        RawResults.AdaptiveLevelSeries.Add(Math.Round(Trial.AdaptiveValue))
    '        RawResults.SpeechLevelSeries.Add(Math.Round(Trial.SpeechLevel))
    '        RawResults.MaskerLevelSeries.Add(Math.Round(Trial.MaskerLevel))
    '        RawResults.SNRLevelSeries.Add(Math.Round(Trial.SNR))
    '        RawResults.TestStageSeries.Add(Trial.TestStage)
    '        RawResults.ProportionCorrectSeries.Add(Trial.GetProportionTasksCorrect)
    '    Next

    '    Return RawResults

    'End Function

    Public Overrides Function CreatePreTestStimulus() As Tuple(Of Audio.Sound, String)
        'Throw New NotImplementedException
    End Function

    Public Overrides Sub UpdateHistoricTrialResults(sender As Object, e As SpeechTestInputEventArgs)
        'Throw New NotImplementedException()
    End Sub

    Public Overrides Function GetResultStringForGui() As String
        'Throw New NotImplementedException()
    End Function

    Public Overrides Function GetExportString() As String
        'Throw New NotImplementedException()
    End Function
End Class




