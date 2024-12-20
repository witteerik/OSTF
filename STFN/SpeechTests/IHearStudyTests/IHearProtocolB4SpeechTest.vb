Imports STFN.TestProtocol

Public Class IHearProtocolB4SpeechTest
    Inherits SpeechTest

    Public Overrides ReadOnly Property FilePathRepresentation As String = "ProtocolB4_ManualSRT"

    Public Sub New(SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)
        ApplyTestSpecificSettings()
    End Sub

    Public Sub ApplyTestSpecificSettings()

        TesterInstructions = "(Detta test går ut på att undersöka nya HTT-listor med muntliga svar, med manlig och kvinnlig röst.)" & vbCrLf & vbCrLf &
             "Testet ska användas med personer som är hörselvårdspatienter." & vbCrLf &
             "1. Välj testöra." & vbCrLf &
             "2. Ställ talnivå till TMV3 + 20 dB, eller maximalt " & MaximumLevel_Targets & " dB HL." & vbCrLf &
             "3. Aktivera kontralateralt brus och ställ in brusnivå enligt normal klinisk praxis (OBS. Ha det aktiverat även om brusnivån är väldigt låg. Det går inte aktivera mitt under testet, ifall det skulle behövas.)." & vbCrLf &
             "4. Använd kontrollen provlyssna för att presentera några ord, och kontrollera att patienten kan uppfatta dem. Höj talnivån om patienten inte kan uppfatta orden. (Dock maximalt till 80 dB HL)" & vbCrLf &
             "(Använd knappen TB för att prata med patienten när denna har lurar på sig.)" & vbCrLf &
             "5. Klicka på start för att starta testet." & vbCrLf &
             "6. Rätta manuellt under testet genom att klicka på testorden som kommer upp på skärmen (nivåjusteringen sker automatiskt)"

        ParticipantInstructions = "Patientens uppgift: " & vbCrLf & vbCrLf &
             " - Patienten ska lyssna efter tvåstaviga ord och efter varje ord repetera ordet muntligt." & vbCrLf &
             " - Patienten ska gissa om hen är osäker. " & vbCrLf &
             " - Patienten har maximalt " & MaximumResponseTime & " sekunder på sig innan nästa ord kommer." & vbCrLf &
             " - Testet består av två 25-ordslistor (en med manlig och en med kvinnlig röst) som körs direkt efter varandra, med möjlighet till en kort paus mellan varje."

        HasOptionalPractiseTest = False
        AllowsUseRetsplChoice = False
        AllowsManualPreSetSelection = False
        AllowsManualStartListSelection = False
        AllowsManualMediaSetSelection = False
        SupportsPrelistening = True
        UseSoundFieldSimulation = Utils.TriState.False
        AvailableTestModes = Nothing
        AvailableTestProtocols = Nothing
        AvailableFixedResponseAlternativeCounts = New List(Of Integer)
        AvailablePhaseAudiometryTypes = Nothing
        MaximumSoundFieldSpeechLocations = 1
        MaximumSoundFieldMaskerLocations = 0
        MaximumSoundFieldBackgroundNonSpeechLocations = 0
        MaximumSoundFieldBackgroundSpeechLocations = 0
        MinimumSoundFieldSpeechLocations = 1
        MinimumSoundFieldMaskerLocations = 0
        MinimumSoundFieldBackgroundNonSpeechLocations = 0
        MinimumSoundFieldBackgroundSpeechLocations = 0
        AllowsManualReferenceLevelSelection = False
        UseKeyWordScoring = Utils.Constants.TriState.False
        UseListOrderRandomization = Utils.Constants.TriState.False
        UseWithinListRandomization = Utils.Constants.TriState.True
        UseAcrossListRandomization = Utils.Constants.TriState.False
        UseFreeRecall = Utils.TriState.True
        UseDidNotHearAlternative = Utils.Constants.TriState.False
        UsePhaseAudiometry = False
        TargetLevel_StepSize = 1
        HistoricTrialCount = 0
        SupportsManualPausing = True
        ReferenceLevel = 65
        SpeechLevel = 65
        MaskingLevel = 65
        BackgroundLevel = 50
        ContralateralMaskingLevel = 25
        MinimumReferenceLevel = -40
        MaximumReferenceLevel = 80
        MinimumLevel_Targets = -40
        MaximumLevel_Targets = 80
        MinimumLevel_Maskers = -40
        MaximumLevel_Maskers = 80
        MinimumLevel_Background = -40
        MaximumLevel_Background = 80
        MinimumLevel_ContralateralMaskers = -40
        MaximumLevel_ContralateralMaskers = 80
        AvailableExperimentNumbers = {}

        SoundOverlapDuration = 0.25

    End Sub


    Public Overrides ReadOnly Property AllowsManualSpeechLevelSelection As Boolean = True

    Public Overrides ReadOnly Property AllowsManualMaskingLevelSelection As Boolean = False

    Public Overrides ReadOnly Property AllowsManualBackgroundLevelSelection As Boolean = False


    Public Overrides ReadOnly Property CanHaveTargets As Boolean = True
    Public Overrides ReadOnly Property CanHaveMaskers As Boolean = False
    Public Overrides ReadOnly Property CanHaveBackgroundNonSpeech As Boolean = False
    Public Overrides ReadOnly Property CanHaveBackgroundSpeech As Boolean = False

    Public Overrides ReadOnly Property UseContralateralMasking_DefaultValue As Utils.TriState = Utils.Constants.TriState.Optional




    Private PlannedTestWords As List(Of SpeechMaterialComponent)
    Private PlannedFamiliarizationWords As List(Of SpeechMaterialComponent)

    Private ObservedTrials As TrialHistory


    Private IsInitialized As Boolean = False

    Private IsInitializeStarted As Boolean = False

    Private IsSecondTest As Boolean = False

    'TestOrder holds rendomized order in which the two tests are run (specifying MediaSet and List index)
    Private TestOrder As Integer

    Private CacheLastTestListVariableName As String
    Private StoredTestListIndex As Integer

    Private ContralateralNoise As Audio.Sound = Nothing
    Private SilentSound As Audio.Sound = Nothing

    Private MaximumSoundDuration As Double = 10
    Private TestWordPresentationTime As Double = 0.5
    Private MaximumResponseTime As Double = 5

    Private ResultSummaryForGUI As New List(Of String)
    Private HasAddedSRT_Stage1 As Boolean = False
    Private HasAddedSRT_Stage2 As Boolean = False
    Private PreTestWordIndex As Integer = 0


    Public Sub TestCacheIndexation()

        'Creating cache variable names for storing last test list index and voice between sessions
        CacheLastTestListVariableName = FilePathRepresentation & "LastTestListOrder"

        Utils.AppCache.RemoveAppCacheVariable(CacheLastTestListVariableName)

        For testSession = 0 To 25

            InitializeCurrentTest()

            Utils.SendInfoToLog("testSession:" & testSession & ", StoredTestListIndex : " & StoredTestListIndex)

            FinalizeTest()

            IsInitialized = False
            IsInitializeStarted = False

        Next

        Utils.AppCache.RemoveAppCacheVariable(CacheLastTestListVariableName)

        Messager.MsgBox("Finished test of cache indexation. Results are stored in the log folder.")

    End Sub

    Public Overrides Function InitializeCurrentTest() As Tuple(Of Boolean, String)

        If IsInitialized = True Then Return New Tuple(Of Boolean, String)(True, "")

        If IsInitializeStarted = True Then Return New Tuple(Of Boolean, String)(True, "")

        IsInitializeStarted = True

        'Randomizing the order of media sets
        TestOrder = Utils.SampleWithoutReplacement(1, 0, 2, Randomizer)(0)

        ObservedTrials = New TrialHistory
        SelectedTestProtocol = New SrtIso8253_TestProtocol
        SelectedTestMode = TestModes.AdaptiveSpeech

        Dim StartAdaptiveLevel As Double = SpeechLevel

        Dim AllTestListsNames = AvailableTestListsNames()

        'Creating cache variable names for storing last test list index and voice between sessions
        CacheLastTestListVariableName = FilePathRepresentation & "LastTestList"

        'Selecting test list
        If Utils.AppCache.AppCacheVariableExists(CacheLastTestListVariableName) = True Then

            'Getting the last tested index
            StoredTestListIndex = Utils.AppCache.GetAppCacheIntegerVariableValue(CacheLastTestListVariableName)

        Else
            'Randomizing a new list number if no list has been run previously 
            StoredTestListIndex = Randomizer.Next(0, AllTestListsNames.Count)

            'Unwrapping TestListIndex
            If StoredTestListIndex > AllTestListsNames.Count - 1 Then
                StoredTestListIndex = 0
            End If

        End If

        CreatePlannedWordsList()

        SelectedTestProtocol.InitializeProtocol(New TestProtocol.NextTaskInstruction With {.AdaptiveValue = StartAdaptiveLevel, .TestStage = 0})

        IsInitialized = True

        Return New Tuple(Of Boolean, String)(True, "")

    End Function

    Private Sub InitializeSecondTest()

        'Store the results of the first test
        SelectedTestProtocol.FinalizeProtocol(ObservedTrials)
        SaveTableFormatedTestResults()

        ' Calling this just to store the tage 1 resulsts for the GUI
        GetResultStringForGui()

        'Initialize second test
        IsSecondTest = True
        ObservedTrials = New TrialHistory
        SelectedTestProtocol = New SrtIso8253_TestProtocol
        SelectedTestMode = TestModes.AdaptiveSpeech

        Dim StartAdaptiveLevel As Double = SpeechLevel

        CreatePlannedWordsList()

        SelectedTestProtocol.InitializeProtocol(New TestProtocol.NextTaskInstruction With {.AdaptiveValue = StartAdaptiveLevel, .TestStage = 0})


    End Sub

    Private Function CreatePlannedWordsList() As Boolean

        'Adding NumberOfWordsToAdd words, starting from the start list (excluding practise items), and re-using lists if needed 
        Dim TempAvailableLists As New List(Of SpeechMaterialComponent)
        Dim AllLists = SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List, True, False)

        Dim CurrentListSMC As SpeechMaterialComponent

        'Determining which combination of MediaSet and test List that should be run. 
        If IsSecondTest = False Then
            'First test
            If TestOrder = 0 Then
                SelectedMediaSet = AvailableMediasets(0)
                CurrentListSMC = AllLists(StoredTestListIndex)
            Else
                ' I.e. TestOrder = 1
                SelectedMediaSet = AvailableMediasets(1)

                Dim CurrentListIndex As Integer = StoredTestListIndex + 1
                If CurrentListIndex > AllLists.Count - 1 Then
                    CurrentListIndex = 0
                End If
                CurrentListSMC = AllLists(CurrentListIndex)
            End If
        Else
            'Second test
            If TestOrder = 0 Then
                SelectedMediaSet = AvailableMediasets(1)

                Dim CurrentListIndex As Integer = StoredTestListIndex + 1
                If CurrentListIndex > AllLists.Count - 1 Then
                    CurrentListIndex = 0
                End If
                CurrentListSMC = AllLists(CurrentListIndex)
            Else
                ' I.e. TestOrder = 1
                SelectedMediaSet = AvailableMediasets(0)
                CurrentListSMC = AllLists(StoredTestListIndex)
            End If
        End If

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
        ContralateralNoise = PlannedTestWords.First.GetContralateralMaskerSound(SelectedMediaSet, 0)

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
        Dim ProtocolReply = SelectedTestProtocol.NewResponse(ObservedTrials)

        'Preparing next trial if needed
        If ProtocolReply.Decision = SpeechTestReplies.GotoNextTrial Then
            PrepareNextTrial(ProtocolReply)

            'Here we abort the test if any of the levels had to be adjusted above MaximumLevel dB HL
            If DirectCast(CurrentTestTrial, SrtTrial).SpeechLevel > MaximumLevel_Targets Or
                DirectCast(CurrentTestTrial, SrtTrial).ContralateralMaskerLevel > MaximumLevel_ContralateralMaskers Then

                'And informing the participant
                ProtocolReply.Decision = SpeechTestReplies.AbortTest
                AbortInformation = "Testet har avbrutits på grund av höga ljudnivåer."

            End If
        End If

        'Checking if first test is finished
        If ProtocolReply.Decision = SpeechTestReplies.TestIsCompleted Then

            If IsSecondTest = False Then

                'It's the first test, initializing the second test
                InitializeSecondTest()

                'And informing the participant
                ProtocolReply.Decision = SpeechTestReplies.PauseTestingWithCustomInformation
                PauseInformation = "Första delen är klar av testet är klart." & vbCrLf & " Klicka OK för att starta den andra och sista delen."

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
        If UseContralateralMasking = True Then
            CurrentContralateralMaskingLevel = NextTaskInstruction.AdaptiveValue + ContralateralLevelDifference()
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
        If UseRetsplCorrection = True Then
            RETSPL_Correction = SelectedTransducer.RETSPL_Speech
        End If

        'Getting the speech signal
        Dim TestWordSound = CurrentTestTrial.SpeechMaterialComponent.GetSound(SelectedMediaSet, 0, 1, , , , , False, False, False, , , False)
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
        If UseContralateralMasking = True Then
            Dim TotalSoundLength = ContralateralNoise.WaveData.SampleData(1).Length
            IntendedNoiseLength = ContralateralNoise.WaveFormat.SampleRate * MaximumSoundDuration
            Dim RandomStartReadSample = Randomizer.Next(0, TotalSoundLength - IntendedNoiseLength)
            TrialContralateralNoise = ContralateralNoise.CopySection(1, RandomStartReadSample - 1, IntendedNoiseLength) ' TODO: Here we should check to ensure that the MaskerNoise is long enough
        End If

        'Checking that Nominal levels agree between signal masker and contralateral masker
        If UseContralateralMasking = True Then If ContralateralNoise.SMA.NominalLevel <> NominalLevel_FS Then Throw New Exception("Nominal level is required to be the same between speech and contralateral noise files!")

        'Calculating presentation levels
        Dim TargetSpeechLevel_FS As Double = Audio.Standard_dBSPL_To_dBFS(DirectCast(CurrentTestTrial, SrtTrial).SpeechLevel) + RETSPL_Correction
        Dim NeededSpeechGain = TargetSpeechLevel_FS - NominalLevel_FS

        'Adjusts the sound levels
        Audio.DSP.AmplifySection(TestWordSound, NeededSpeechGain)

        If UseContralateralMasking = True Then

            'Setting level, 
            'Very important: The contralateral masking sound file cannot be the same as the ipsilateral masker sound. The level of the contralateral masker sound must be set to agree with the Nominal level (while the ipsilateral masker sound sound have a level that deviates from the nominal level to attain the desired SNR!)
            Dim ContralateralMaskingNominalLevel_FS = ContralateralNoise.SMA.NominalLevel
            Dim TargetContralateralMaskingLevel_FS As Double = Audio.Standard_dBSPL_To_dBFS(DirectCast(CurrentTestTrial, SrtTrial).ContralateralMaskerLevel) + SelectedMediaSet.EffectiveContralateralMaskingGain + RETSPL_Correction

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

        If SignalLocations(0).HorizontalAzimuth < 0 Then
            'Left test ear
            'Adding speech and noise
            CurrentTestTrial.Sound.WaveData.SampleData(1) = TestSound.WaveData.SampleData(1)
            'Adding contralateral masking
            If UseContralateralMasking = True Then
                CurrentTestTrial.Sound.WaveData.SampleData(2) = TrialContralateralNoise.WaveData.SampleData(1)
            End If

        Else
            'Right test ear
            'Adding speech and noise
            CurrentTestTrial.Sound.WaveData.SampleData(2) = TestSound.WaveData.SampleData(1)
            'Adding contralateral masking
            If UseContralateralMasking = True Then
                CurrentTestTrial.Sound.WaveData.SampleData(1) = TrialContralateralNoise.WaveData.SampleData(1)
            End If
        End If

        'Stores the test ear (added nov 2024)
        Select Case SignalLocations(0).HorizontalAzimuth
            Case -90
                CurrentTestTrial.TestEar = Utils.Constants.SidesWithBoth.Left
            Case 90
                CurrentTestTrial.TestEar = Utils.Constants.SidesWithBoth.Right
            Case Else
                Throw New Exception("Unsupported signal azimuth: " & SignalLocations(0).HorizontalAzimuth)
        End Select

        'Also stores the mediaset
        CurrentTestTrial.MediaSetName = SelectedMediaSet.MediaSetName

        'And the contralateral noise on/off setting
        CurrentTestTrial.UseContralateralNoise = UseContralateralMasking

        'And the EM term
        CurrentTestTrial.EfficientContralateralMaskingTerm = SelectedMediaSet.EffectiveContralateralMaskingGain

    End Sub



    Public Overrides Function GetResultStringForGui() As String

        Dim ProtocolThreshold = SelectedTestProtocol.GetFinalResult()

        Dim Output As New List(Of String)

        If ProtocolThreshold IsNot Nothing Then

            If IsSecondTest = False Then
                If HasAddedSRT_Stage1 = False Then
                    ResultSummaryForGUI.Add("HTT första testet = " & vbTab & Math.Round(ProtocolThreshold.Value) & " dB HL")
                    HasAddedSRT_Stage1 = True
                End If
            Else
                If HasAddedSRT_Stage2 = False Then
                    ResultSummaryForGUI.Add("HTT andra testet = " & vbTab & Math.Round(ProtocolThreshold.Value) & " dB HL")
                    HasAddedSRT_Stage2 = True
                End If
            End If
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


    Public Overrides Function GetTestTrialResultExportString() As String
        Return "Export of trial level test results is not yet implemented"
    End Function

    Public Overrides Function GetTestResultsExportString() As String

        Dim ExportStringList As New List(Of String)

        Dim ProtocolThreshold = SelectedTestProtocol.GetFinalResult()

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

    Public Overrides Sub FinalizeTest()

        SelectedTestProtocol.FinalizeProtocol(ObservedTrials)

        If CurrentParticipantID <> NoTestId Then

            'Saving updated cache data values, only if a real test was completed
            Dim AllTestListsNames = AvailableTestListsNames()

            Dim NextTestListIndex As Integer = StoredTestListIndex

            NextTestListIndex += 1

            'Unwrapping NextTestListIndex 
            If NextTestListIndex > AllTestListsNames.Count - 1 Then
                NextTestListIndex = 0
            End If

            'Storing the test list index to be used in the next test session (only if NoTestId was not used)
            Utils.AppCache.SetAppCacheVariableValue(CacheLastTestListVariableName, NextTestListIndex)

        End If

    End Sub


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
            .SpeechLevel = SpeechLevel,
            .ContralateralMaskerLevel = ContralateralMaskingLevel}

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
