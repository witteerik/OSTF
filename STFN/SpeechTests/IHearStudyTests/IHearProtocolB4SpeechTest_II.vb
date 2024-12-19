Imports STFN.TestProtocol

Public Class IHearProtocolB4SpeechTest_II
    Inherits SpeechTest

    Public Sub New(SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)

    End Sub

    Public Overrides ReadOnly Property FilePathRepresentation As String
        Get
            Return "ProtocolB4_II_ManualSRT"
        End Get
    End Property

    Private PlannedTestWords As List(Of SpeechMaterialComponent)
    Private PlannedFamiliarizationWords As List(Of SpeechMaterialComponent)

    Private ObservedTrials As TrialHistory


#Region "Settings"

    Public Overrides ReadOnly Property TesterInstructions As String
        Get
            Return "(Detta test går ut på att undersöka nya HTT-listor med muntliga svar, med manlig och kvinnlig röst.)" & vbCrLf & vbCrLf &
                "Testet ska användas med normalhörande personer, som inte är hörselvårdspatienter." & vbCrLf &
                "1. Ange experimentnummer." & vbCrLf &
                "2. Välj testöra." & vbCrLf &
                "3. Ställ talnivå till TMV3 + 20 dB, eller maximalt " & MaximumLevel & " dB HL." & vbCrLf &
                "4. Aktivera kontralateralt brus och ställ in brusnivå enligt normal klinisk praxis (OBS. Ha det aktiverat även om brusnivån är väldigt låg. Det går inte aktivera mitt under testet, ifall det skulle behövas.)." & vbCrLf &
                "5. Använd kontrollen provlyssna för att presentera några ord, och kontrollera att deltagaren kan uppfatta dem. Höj talnivån om deltagaren inte kan uppfatta orden. (Dock maximalt till 80 dB HL)" & vbCrLf &
                "(Använd knappen TB för att prata med deltagaren när denna har lurar på sig.)" & vbCrLf &
                "6. Klicka på start för att starta testet." & vbCrLf &
                "7. Rätta manuellt under testet genom att klicka på testorden som kommer upp på skärmen (nivåjusteringen sker automatiskt)"

        End Get
    End Property

    Public Overrides ReadOnly Property ParticipantInstructions As String
        Get
            Return "Deltagarens uppgift: " & vbCrLf & vbCrLf &
                " - Deltagarens ska lyssna efter tvåstaviga ord och efter varje ord repetera ordet muntligt." & vbCrLf &
                " - Deltagarens ska gissa om hen är osäker. " & vbCrLf &
                " - Deltagarens har maximalt " & MaximumResponseTime & " sekunder på sig innan nästa ord kommer." & vbCrLf &
                " - Testet består av åtta 25-ordslistor (med varierande manlig eller kvinnlig röst) som körs direkt efter varandra, med möjlighet till korta pauser mellan varje."

        End Get
    End Property

    Public Overrides ReadOnly Property HasOptionalPractiseTest As Boolean
        Get
            Return False
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
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsManualMediaSetSelection As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsManualSpeechLevelSelection As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsManualMaskingLevelSelection As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsManualBackgroundLevelSelection As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property SupportsPrelistening As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property UseSoundFieldSimulation As Utils.TriState
        Get
            Return Utils.TriState.False
        End Get
    End Property

    Public Overrides ReadOnly Property AvailableTestModes As List(Of TestModes)
        Get
            Return Nothing
        End Get
    End Property

    Public Overrides ReadOnly Property AvailableTestProtocols As List(Of TestProtocol)
        Get
            Return Nothing
        End Get
    End Property

    Public Overrides ReadOnly Property AvailableFixedResponseAlternativeCounts As List(Of Integer)
        Get
            Return New List(Of Integer)
        End Get
    End Property

    Public Overrides ReadOnly Property AvailablePhaseAudiometryTypes As List(Of BmldModes)
        Get
            Return Nothing
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
            Return 0
        End Get
    End Property

    Public Overrides ReadOnly Property MaximumSoundFieldBackgroundNonSpeechLocations As Integer
        Get
            Return 0
        End Get
    End Property

    Public Overrides ReadOnly Property MaximumSoundFieldBackgroundSpeechLocations As Integer
        Get
            Return 0
        End Get
    End Property

    Public Overrides ReadOnly Property MinimumSoundFieldSpeechLocations As Integer
        Get
            Return 1
        End Get
    End Property

    Public Overrides ReadOnly Property MinimumSoundFieldMaskerLocations As Integer
        Get
            Return 0
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
            Return False
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
            Return Utils.Constants.TriState.False
        End Get
    End Property

    Public Overrides ReadOnly Property UseListOrderRandomization As Utils.TriState
        Get
            Return Utils.Constants.TriState.False
        End Get
    End Property

    Public Overrides ReadOnly Property UseWithinListRandomization As Utils.TriState
        Get
            Return Utils.Constants.TriState.True
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

    Public Overrides Property SoundOverlapDuration As Double = 0.25

    Public Overrides ReadOnly Property MinimumLevel As Double = -40
    Public Overrides ReadOnly Property MaximumLevel As Double = 80

    Public Overrides ReadOnly Property AvailableExperimentNumbers As Integer()
        Get
            Dim OutputList As New List(Of Integer)
            For i = 1 To 30
                OutputList.Add(i)
            Next
            Return OutputList.ToArray
        End Get
    End Property


    Private IsInitialized As Boolean = False

    Private TestsCompleted As Integer = 0
    Private TotalNumberOfLists As Integer

    Private ContralateralNoise As Audio.Sound = Nothing
    Private SilentSound As Audio.Sound = Nothing

    Private MaximumSoundDuration As Double = 10
    Private TestWordPresentationTime As Double = 0.5
    Private MaximumResponseTime As Double = 5

    Private ListTalkerCollection As New List(Of Tuple(Of SpeechMaterialComponent, MediaSet))

    ''' <summary>
    ''' Returns a list for counter-balancing two talkers between the eight lists in 30 experiments
    ''' </summary>
    ''' <returns></returns>
    Private Function GetCounterBalanceList() As SortedList(Of Integer, Integer())

        Dim CounterBalanceList As New SortedList(Of Integer, Integer())
        CounterBalanceList.Add(1, {0, 0, 0, 0, 1, 1, 1, 1})
        CounterBalanceList.Add(2, {0, 0, 1, 1, 0, 0, 1, 1})
        CounterBalanceList.Add(3, {0, 1, 0, 1, 0, 1, 0, 1})
        CounterBalanceList.Add(4, {1, 0, 0, 1, 1, 0, 0, 1})
        CounterBalanceList.Add(5, {1, 1, 0, 0, 1, 1, 0, 0})
        CounterBalanceList.Add(6, {1, 0, 1, 0, 1, 0, 1, 0})
        CounterBalanceList.Add(7, {0, 1, 1, 0, 0, 1, 1, 0})
        CounterBalanceList.Add(8, {1, 1, 1, 1, 0, 0, 0, 0})
        CounterBalanceList.Add(9, {0, 0, 0, 0, 1, 1, 1, 1})
        CounterBalanceList.Add(10, {0, 0, 1, 1, 0, 0, 1, 1})
        CounterBalanceList.Add(11, {0, 1, 0, 1, 0, 1, 0, 1})
        CounterBalanceList.Add(12, {1, 0, 0, 1, 1, 0, 0, 1})
        CounterBalanceList.Add(13, {1, 1, 0, 0, 1, 1, 0, 0})
        CounterBalanceList.Add(14, {1, 0, 1, 0, 1, 0, 1, 0})
        CounterBalanceList.Add(15, {0, 1, 1, 0, 0, 1, 1, 0})
        CounterBalanceList.Add(16, {1, 1, 1, 1, 0, 0, 0, 0})
        CounterBalanceList.Add(17, {0, 0, 0, 0, 1, 1, 1, 1})
        CounterBalanceList.Add(18, {0, 0, 1, 1, 0, 0, 1, 1})
        CounterBalanceList.Add(19, {0, 1, 0, 1, 0, 1, 0, 1})
        CounterBalanceList.Add(20, {1, 0, 0, 1, 1, 0, 0, 1})
        CounterBalanceList.Add(21, {1, 1, 0, 0, 1, 1, 0, 0})
        CounterBalanceList.Add(22, {1, 0, 1, 0, 1, 0, 1, 0})
        CounterBalanceList.Add(23, {0, 1, 1, 0, 0, 1, 1, 0})
        CounterBalanceList.Add(24, {1, 1, 1, 1, 0, 0, 0, 0})
        CounterBalanceList.Add(25, {0, 0, 0, 0, 1, 1, 1, 1})
        CounterBalanceList.Add(26, {0, 0, 1, 1, 0, 0, 1, 1})
        CounterBalanceList.Add(27, {0, 1, 0, 1, 0, 1, 0, 1})
        CounterBalanceList.Add(28, {1, 1, 1, 1, 0, 0, 0, 0})
        CounterBalanceList.Add(29, {1, 1, 0, 0, 1, 1, 0, 0})
        CounterBalanceList.Add(30, {1, 0, 1, 0, 1, 0, 1, 0})

        Return CounterBalanceList

    End Function


    Public Sub TestListCombinations()

        ''Temporary code for testing list-level combinations

        TestOptions.SkipGuiUpdates = True

        Dim CounterBalanceList = GetCounterBalanceList()

        Dim TempExportData = New List(Of String)

        For i = CounterBalanceList.Keys.Min To CounterBalanceList.Keys.Max

            ListTalkerCollection.Clear()
            TestsCompleted = 0
            TestOptions.ExperimentNumber = i
            'Initializing the first test
            IsInitialized = False
            InitializeCurrentTest()

            'Storing for export
            If i = CounterBalanceList.Keys.Min Then
                TempExportData.Add(GetPlannedTrialsExportString(i, TestsCompleted, False))
            Else
                TempExportData.Add(GetPlannedTrialsExportString(i, TestsCompleted, True))
            End If

            TestsCompleted += 1

            For j = 1 To TotalNumberOfLists - 1

                'Initializing the second test
                InitializeTestWithNewList()

                'Storing for export
                TempExportData.Add(GetPlannedTrialsExportString(i, TestsCompleted, True))

                TestsCompleted += 1

            Next

        Next

            Utils.SendInfoToLog(String.Join(vbCrLf, TempExportData), "ProtocolB4II_PlannedTestTrials")

    End Sub


    Public Overrides Function InitializeCurrentTest() As Tuple(Of Boolean, String)

        If IsInitialized = True Then Return New Tuple(Of Boolean, String)(True, "")

        Dim ExperimentNumber As Integer = TestOptions.ExperimentNumber
        If ExperimentNumber < 1 Or ExperimentNumber > 30 Then
            Throw New ArgumentException("Invalid experiment number. It must be in the range 1-30!")
        End If

        Dim CounterBalanceList = GetCounterBalanceList()
        Dim CurrentListTalkerCombination = CounterBalanceList(ExperimentNumber)

        'Adds the list talker combinations into a list of objects (so that it can be randomized using Utils.Shuffle)
        Dim ListTalkerCollectionObjects As New List(Of Object)
        Dim AllLists = SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List, True, False)
        TotalNumberOfLists = AllLists.Count
        For ListIndex = 0 To AllLists.Count - 1
            ListTalkerCollectionObjects.Add(New Tuple(Of SpeechMaterialComponent, MediaSet)(AllLists(ListIndex), AvailableMediasets(CurrentListTalkerCombination(ListIndex)))) 'Holds List-level SpeechMaterialComponent and MediaSet to be used
        Next

        'Randomizing the presentation order
        ListTalkerCollectionObjects = Utils.Shuffle(ListTalkerCollectionObjects, Randomizer)

        'Adding the randomized list talker combinations into ListTalkerCollection, from where they're drawn in each stage of the testing
        For Each Item As Tuple(Of SpeechMaterialComponent, MediaSet) In ListTalkerCollectionObjects
            ListTalkerCollection.Add(Item)
        Next

        InitializeTestWithNewList()

        IsInitialized = True

        Return New Tuple(Of Boolean, String)(True, "")

    End Function

    Private Sub InitializeTestWithNewList()

        'Initialize the test
        ObservedTrials = New TrialHistory
        TestOptions.SelectedTestProtocol = New SrtIso8253_TestProtocol
        TestOptions.SelectedTestMode = TestModes.AdaptiveSpeech

        Dim StartAdaptiveLevel As Double = TestOptions.SpeechLevel

        CreatePlannedWordsList()

        TestOptions.SelectedTestProtocol.InitializeProtocol(New TestProtocol.NextTaskInstruction With {.AdaptiveValue = StartAdaptiveLevel, .TestStage = 0})

    End Sub

    Private Function CreatePlannedWordsList() As Boolean

        'Gets the selected List
        Dim CurrentListSMC As SpeechMaterialComponent = ListTalkerCollection(TestsCompleted).Item1

        'Gets the selected MediaSet and stores it into the TestOptions for the current test stage
        TestOptions.SelectedMediaSet = ListTalkerCollection(TestsCompleted).Item2

        'Adding all planned test words, and stopping after NumberOfWordsToAdd have been added
        PlannedTestWords = New List(Of SpeechMaterialComponent)
        PlannedFamiliarizationWords = New List(Of SpeechMaterialComponent)
        Dim AllWords = CurrentListSMC.GetChildren()

        'Adding the first words to be used for familiarization
        Dim FamiliarizationWords As New List(Of SpeechMaterialComponent)
        Dim TestStageWords As New List(Of SpeechMaterialComponent)
        For i = 0 To AllWords.Count - 1
            If i < 5 Then
                FamiliarizationWords.Add(AllWords(i))
            Else
                TestStageWords.Add(AllWords(i))
            End If
        Next

        'Adding familiarization words in randomized order
        Dim RandomizedOrder1 = Utils.SampleWithoutReplacement(FamiliarizationWords.Count, 0, FamiliarizationWords.Count, Randomizer)
        For Each RandomIndex In RandomizedOrder1
            PlannedFamiliarizationWords.Add(FamiliarizationWords(RandomIndex))
        Next

        'Adding test words in randomized order
        Dim RandomizedOrder2 = Utils.SampleWithoutReplacement(TestStageWords.Count, 0, TestStageWords.Count, Randomizer)
        For Each RandomIndex In RandomizedOrder2
            PlannedTestWords.Add(TestStageWords(RandomIndex))
        Next

        'Getting the contralateral noise from the first SMC
        ContralateralNoise = PlannedTestWords.First.GetContralateralMaskerSound(ListTalkerCollection(TestsCompleted).Item2, 0)

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

        'Calculating the speech level
        Dim ProtocolReply = TestOptions.SelectedTestProtocol.NewResponse(ObservedTrials)

        'Preparing next trial if needed
        If ProtocolReply.Decision = SpeechTestReplies.GotoNextTrial Then
            PrepareNextTrial(ProtocolReply)

            'Here we abort the test if any of the levels had to be adjusted above MaximumLevel dB HL
            If DirectCast(CurrentTestTrial, SrtTrial).SpeechLevel > MaximumLevel Or
                DirectCast(CurrentTestTrial, SrtTrial).ContralateralMaskerLevel > MaximumLevel Then

                'And informing the participant
                ProtocolReply.Decision = SpeechTestReplies.AbortTest
                AbortInformation = "Testet har avbrutits på grund av höga ljudnivåer."

            End If
        End If

        'Checking if first test is finished
        If ProtocolReply.Decision = SpeechTestReplies.TestIsCompleted Then

            'Increments TestsCompleted 
            TestsCompleted += 1

            'Store the results of the last test
            TestOptions.SelectedTestProtocol.FinalizeProtocol(ObservedTrials)
            SaveTableFormatedTestResults()

            If TestsCompleted < TotalNumberOfLists Then

                ' Calling this just to store the results of the previous test for the GUI (the last time we don't need to call it since its then called by the speech test view)
                GetResultStringForGui()

                'It's the first test, initializing the second test
                InitializeTestWithNewList()

                'And informing the participant
                ProtocolReply.Decision = SpeechTestReplies.PauseTestingWithCustomInformation
                PauseInformation = "Test " & TestsCompleted & " av " & TotalNumberOfLists & " är klart." & vbCrLf & " Klicka OK och Fortsätt för att starta nästa test."

            End If
        End If

        Return ProtocolReply.Decision

    End Function

    Private Sub PrepareNextTrial(ByVal NextTaskInstruction As TestProtocol.NextTaskInstruction)

        'Preparing the next trial
        'Getting next test word
        Dim NextTestWord = PlannedTestWords(ObservedTrials.Count)

        'Creating a new test trial
        Dim CurrentContralateralMaskingLevel As Double = Double.NegativeInfinity
        If TestOptions.UseContralateralMasking = True Then
            CurrentContralateralMaskingLevel = NextTaskInstruction.AdaptiveValue + TestOptions.ContralateralLevelDifference
        End If

        CurrentTestTrial = New SrtTrial With {.SpeechMaterialComponent = NextTestWord,
                    .AdaptiveValue = NextTaskInstruction.AdaptiveValue,
                    .SpeechLevel = NextTaskInstruction.AdaptiveValue,
                    .ContralateralMaskerLevel = CurrentContralateralMaskingLevel,
                    .TestStage = NextTaskInstruction.TestStage,
                    .Tasks = 1}

        CurrentTestTrial.ResponseAlternativeSpellings = New List(Of List(Of SpeechTestResponseAlternative))

        Dim ResponseAlternatives As New List(Of SpeechTestResponseAlternative)

        If CurrentTestTrial.SpeechMaterialComponent.ChildComponents.Count > 0 Then
            CurrentTestTrial.Tasks = 0
            For Each Child In CurrentTestTrial.SpeechMaterialComponent.ChildComponents()
                ResponseAlternatives.Add(New SpeechTestResponseAlternative With {.Spelling = Child.GetCategoricalVariableValue("Spelling"), .IsScoredItem = True})
                CurrentTestTrial.Tasks += 1
            Next
        End If

        CurrentTestTrial.ResponseAlternativeSpellings.Add(ResponseAlternatives)

        'Mixing trial sound
        MixNextTrialSound()

        'Setting trial events
        CurrentTestTrial.TrialEventList = New List(Of ResponseViewEvent)
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 1, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = System.Math.Max(1, 1000 * TestWordPresentationTime), .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = System.Math.Max(1, 1000 * (TestWordPresentationTime + MaximumResponseTime)), .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut})

    End Sub



    Private Sub MixNextTrialSound()

        Dim RETSPL_Correction As Double = 0
        If TestOptions.UseRetsplCorrection = True Then
            RETSPL_Correction = TestOptions.SelectedTransducer.RETSPL_Speech
        End If

        'Getting the speech signal
        Dim TestWordSound = CurrentTestTrial.SpeechMaterialComponent.GetSound(TestOptions.SelectedMediaSet, 0, 1, , , , , False, False, False, , , False)
        Dim NominalLevel_FS = TestWordSound.SMA.NominalLevel

        'Storing the LinguisticSoundStimulusStartTime and the LinguisticSoundStimulusDuration (assuming that the linguistic recording is in channel 1)
        CurrentTestTrial.LinguisticSoundStimulusStartTime = TestWordPresentationTime
        CurrentTestTrial.LinguisticSoundStimulusDuration = TestWordSound.WaveData.SampleData(1).Length / TestWordSound.WaveFormat.SampleRate
        CurrentTestTrial.MaximumResponseTime = MaximumResponseTime

        'Creating a silent sound (lazy method to get the same length independently of contralateral masking or not)
        Dim SilentSound = Audio.GenerateSound.CreateSilence(ContralateralNoise.WaveFormat, 1, MaximumSoundDuration)

        'Creating contalateral masking noise (with the same length as the masking noise)
        Dim TrialContralateralNoise As Audio.Sound = Nothing
        Dim IntendedNoiseLength As Integer
        If TestOptions.UseContralateralMasking = True Then
            Dim TotalSoundLength = ContralateralNoise.WaveData.SampleData(1).Length
            IntendedNoiseLength = ContralateralNoise.WaveFormat.SampleRate * MaximumSoundDuration
            Dim RandomStartReadSample = Randomizer.Next(0, TotalSoundLength - IntendedNoiseLength)
            TrialContralateralNoise = ContralateralNoise.CopySection(1, RandomStartReadSample - 1, IntendedNoiseLength) ' TODO: Here we should check to ensure that the MaskerNoise is long enough
        End If

        'Checking that Nominal levels agree between signal masker and contralateral masker
        If TestOptions.UseContralateralMasking = True Then If ContralateralNoise.SMA.NominalLevel <> NominalLevel_FS Then Throw New Exception("Nominal level is required to be the same between speech and contralateral noise files!")

        'Calculating presentation levels
        Dim TargetSpeechLevel_FS As Double = Audio.Standard_dBSPL_To_dBFS(DirectCast(CurrentTestTrial, SrtTrial).SpeechLevel) + RETSPL_Correction
        Dim NeededSpeechGain = TargetSpeechLevel_FS - NominalLevel_FS

        'Adjusts the sound levels
        Audio.DSP.AmplifySection(TestWordSound, NeededSpeechGain)

        If TestOptions.UseContralateralMasking = True Then

            'Setting level, 
            'Very important: The contralateral masking sound file cannot be the same as the ipsilateral masker sound. The level of the contralateral masker sound must be set to agree with the Nominal level (while the ipsilateral masker sound sound have a level that deviates from the nominal level to attain the desired SNR!)
            Dim ContralateralMaskingNominalLevel_FS = ContralateralNoise.SMA.NominalLevel
            Dim TargetContralateralMaskingLevel_FS As Double = Audio.Standard_dBSPL_To_dBFS(DirectCast(CurrentTestTrial, SrtTrial).ContralateralMaskerLevel) + TestOptions.SelectedMediaSet.EffectiveContralateralMaskingGain + RETSPL_Correction

            'Calculating the needed gain, also adding the EffectiveContralateralMaskingGain specified in the SelectedMediaSet
            Dim NeededContraLateralMaskerGain = TargetContralateralMaskingLevel_FS - ContralateralMaskingNominalLevel_FS
            Audio.DSP.AmplifySection(TrialContralateralNoise, NeededContraLateralMaskerGain)

        End If

        'Mixing speech and noise
        Dim TestWordInsertionSample As Integer = TestWordSound.WaveFormat.SampleRate * TestWordPresentationTime
        Dim Silence = Audio.GenerateSound.CreateSilence(SilentSound.WaveFormat, 1, TestWordInsertionSample, Audio.BasicAudioEnums.TimeUnits.samples)
        Audio.DSP.InsertSoundAt(TestWordSound, Silence, 0)
        TestWordSound.ZeroPad(IntendedNoiseLength)
        Dim TestSound = Audio.DSP.SuperpositionSounds({TestWordSound, SilentSound}.ToList)

        'Creating an output sound
        CurrentTestTrial.Sound = New Audio.Sound(New Audio.Formats.WaveFormat(TestWordSound.WaveFormat.SampleRate, TestWordSound.WaveFormat.BitDepth, 2,, TestWordSound.WaveFormat.Encoding))

        If TestOptions.SignalLocations(0).HorizontalAzimuth < 0 Then
            'Left test ear
            'Adding speech and noise
            CurrentTestTrial.Sound.WaveData.SampleData(1) = TestSound.WaveData.SampleData(1)
            'Adding contralateral masking
            If TestOptions.UseContralateralMasking = True Then
                CurrentTestTrial.Sound.WaveData.SampleData(2) = TrialContralateralNoise.WaveData.SampleData(1)
            End If

        Else
            'Right test ear
            'Adding speech and noise
            CurrentTestTrial.Sound.WaveData.SampleData(2) = TestSound.WaveData.SampleData(1)
            'Adding contralateral masking
            If TestOptions.UseContralateralMasking = True Then
                CurrentTestTrial.Sound.WaveData.SampleData(1) = TrialContralateralNoise.WaveData.SampleData(1)
            End If
        End If

        'Stores the test ear (added nov 2024)
        Select Case TestOptions.SignalLocations(0).HorizontalAzimuth
            Case -90
                CurrentTestTrial.TestEar = Utils.Constants.SidesWithBoth.Left
            Case 90
                CurrentTestTrial.TestEar = Utils.Constants.SidesWithBoth.Right
            Case Else
                Throw New Exception("Unsupported signal azimuth: " & TestOptions.SignalLocations(0).HorizontalAzimuth)
        End Select

        'Also stores the mediaset
        CurrentTestTrial.MediaSetName = TestOptions.SelectedMediaSet.MediaSetName

        'And the contralateral noise on/off setting
        CurrentTestTrial.UseContralateralNoise = TestOptions.UseContralateralMasking

        'And the EM term
        CurrentTestTrial.EfficientContralateralMaskingTerm = TestOptions.SelectedMediaSet.EffectiveContralateralMaskingGain

        'And the experiment number
        CurrentTestTrial.ExperimentNumber = TestOptions.ExperimentNumber

    End Sub

    Public Overrides Function GetResultStringForGui() As String

        Dim ProtocolThreshold = TestOptions.SelectedTestProtocol.GetFinalResult()

        Dim Output As New List(Of String)

        If ProtocolThreshold IsNot Nothing Then
            ResultSummaryForGUI.Add("HTT för test nr " & TestsCompleted & " = " & vbTab & Math.Round(ProtocolThreshold.Value) & " dB HL")
            Output.AddRange(ResultSummaryForGUI)
        Else
            Output.Add("Testord nummer " & ObservedTrials.Count & " av " & PlannedTestWords.Count)
            If CurrentTestTrial IsNot Nothing Then
                Output.Add("Talnivå = " & Math.Round(DirectCast(CurrentTestTrial, SrtTrial).SpeechLevel) & " dB HL")
                Output.Add("Kontralateral brusnivå = " & Math.Round(DirectCast(CurrentTestTrial, SrtTrial).ContralateralMaskerLevel) & " dB HL")
            End If
        End If

        Return String.Join(vbCrLf, Output)

    End Function

    Private ResultSummaryForGUI As New List(Of String)

    Public Overrides Function GetTestTrialResultExportString() As String
        Return "Export of trial level test results is not yet implemented"
    End Function

    Public Overrides Function GetTestResultsExportString() As String

        Dim ExportStringList As New List(Of String)

        Dim ProtocolThreshold = TestOptions.SelectedTestProtocol.GetFinalResult()

        'Exporting all trials
        Dim TestTrialIndex As Integer = 0
        For i = 0 To ObservedTrials.Count - 1

            If TestTrialIndex = 0 Then
                ExportStringList.Add("TrialIndex" & vbTab & ObservedTrials(i).TestResultColumnHeadings & vbTab & "SRT")
            End If

            If i = ObservedTrials.Count - 1 Then
                'Exporting SRT on last row, last column, if determined
                If ProtocolThreshold.HasValue Then
                    ExportStringList.Add(i & vbTab & ObservedTrials(i).TestResultAsTextRow & vbTab & ProtocolThreshold)
                Else
                    ExportStringList.Add(i & vbTab & ObservedTrials(i).TestResultAsTextRow & vbTab & "SRT not established")
                End If
            Else
                ExportStringList.Add(i & vbTab & ObservedTrials(i).TestResultAsTextRow)
            End If

            TestTrialIndex += 1
        Next

        Return String.Join(vbCrLf, ExportStringList)

    End Function

    Private Function GetPlannedTrialsExportString(ByVal ID As String, ByVal TestIndex As Integer, ByVal SkipHeading As Boolean) As String

        Dim ExportStringList As New List(Of String)

        Dim TestTrialIndex As Integer = 0
        For Each TestWord In PlannedTestWords

            If TestTrialIndex = 0 And SkipHeading = False Then
                ExportStringList.Add("ID" & vbTab & "TestIndex" & vbTab & "TrialIndex" & vbTab & "SMCID" & vbTab & "Spelling" & vbTab & "MediaSetName")
            End If

            ExportStringList.Add(ID & vbTab & TestIndex & vbTab & TestTrialIndex & vbTab & TestWord.Id & vbTab & TestWord.GetCategoricalVariableValue("Spelling") & vbTab & TestOptions.SelectedMediaSet.MediaSetName)
            TestTrialIndex += 1

        Next

        Return String.Join(vbCrLf, ExportStringList)

    End Function

    Public Overrides Sub FinalizeTest()

        TestOptions.SelectedTestProtocol.FinalizeProtocol(ObservedTrials)

    End Sub


    Dim PreTestWordIndex As Integer = 0
    Public Overrides Function CreatePreTestStimulus() As Tuple(Of Audio.Sound, String)

        InitializeCurrentTest()

        'Creating PreTestTrial

        'Selecting pre-test word index
        If PreTestWordIndex >= PlannedFamiliarizationWords.Count - 1 Then
            'Resetting PreTestWordIndex, if all pre-testwords have been used
            PreTestWordIndex = 0
        End If

        'Getting the test word
        Dim NextTestWord = PlannedFamiliarizationWords(PreTestWordIndex)
        PreTestWordIndex += 1

        'Getting the spelling
        Dim TestWordSpelling = NextTestWord.GetCategoricalVariableValue("Spelling")

        'Creating a new pretest trial
        CurrentTestTrial = New SrtTrial With {.SpeechMaterialComponent = NextTestWord,
            .SpeechLevel = TestOptions.SpeechLevel,
            .ContralateralMaskerLevel = TestOptions.ContralateralMaskingLevel}

        'Mixing the test sound
        MixNextTrialSound()

        'Storing the test sound locally
        Dim PreTestSound = CurrentTestTrial.Sound

        'Resetting CurrentTestTrial 
        CurrentTestTrial = Nothing

        Return New Tuple(Of Audio.Sound, String)(PreTestSound, TestWordSpelling)

    End Function


    Public Overrides Sub UpdateHistoricTrialResults(sender As Object, e As SpeechTestInputEventArgs)
        'Not supported
        'Throw New NotImplementedException()
    End Sub

End Class
