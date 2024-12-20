Imports STFN.SipTest
Imports STFN.Audio.SoundScene
Imports STFN.Utils

Public Class IHearProtocolB7SpeechTest

    Inherits SpeechTest

    Public Overrides ReadOnly Property FilePathRepresentation As String = "ProtocolB7_SipTest"

    Public Sub New(ByVal SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)
        ApplyTestSpecificSettings()
    End Sub

    Public Sub ApplyTestSpecificSettings()

        TesterInstructions = "(Detta test går ut på att undersöka svårighetsgraden i SiP-testet.)" & vbCrLf & vbCrLf &
             "För detta test behövs inga inställningar." & vbCrLf & vbCrLf &
             "1. Informera patienten om hur testet går till." & vbCrLf &
             "2. Vänd skärmen till patienten. Be sedan patienten klicka på start för att starta testet."

        ParticipantInstructions = "Patientens uppgift: " & vbCrLf & vbCrLf &
             " - Patienten startar testet genom att klicka på knappen 'Start'" & vbCrLf &
             " - Under testet ska patienten lyssna efter enstaviga ord i olika ljudmiljöer och efter varje ord ange på skärmen vilket ord hen uppfattade. " & vbCrLf &
             " - Patienten ska gissa om hen är osäker. Många ord är mycket svåra att höra!" & vbCrLf &
             " - Efter varje ord har patienten maximalt " & MaximumResponseTime & " sekunder på sig att ange sitt svar." & vbCrLf &
             " - Om svarsalternativen blinkar i röd färg har patienten inte svarat i tid." & vbCrLf &
             " - Testet består av två testomgångar med " & TestListCount * 3 & " ord i varje. testomgångarna körs direkt efter varandra, med möjlighet till en kort paus mellan varje."

        HasOptionalPractiseTest = False
        AllowsUseRetsplChoice = False
        AllowsManualPreSetSelection = False
        AllowsManualStartListSelection = False
        AllowsManualMediaSetSelection = False
        SupportsPrelistening = False
        UseSoundFieldSimulation = TriState.True
        AvailableTestModes = New List(Of TestModes) From {TestModes.Custom}
        AvailableTestProtocols = Nothing
        AvailableFixedResponseAlternativeCounts = New List(Of Integer) From {3}
        AvailablePhaseAudiometryTypes = New List(Of BmldModes)
        MaximumSoundFieldSpeechLocations = 1
        MaximumSoundFieldMaskerLocations = 1
        MaximumSoundFieldBackgroundNonSpeechLocations = 2
        MaximumSoundFieldBackgroundSpeechLocations = 0
        MinimumSoundFieldSpeechLocations = 1
        MinimumSoundFieldMaskerLocations = 1
        MinimumSoundFieldBackgroundNonSpeechLocations = 2
        MinimumSoundFieldBackgroundSpeechLocations = 0
        AllowsManualReferenceLevelSelection = False
        UseKeyWordScoring = Utils.TriState.False
        UseListOrderRandomization = Utils.TriState.False
        UseWithinListRandomization = Utils.TriState.False
        UseAcrossListRandomization = Utils.TriState.False
        UseFreeRecall = Utils.TriState.False
        UseDidNotHearAlternative = Utils.Constants.TriState.False
        UsePhaseAudiometry_DefaultValue = Utils.Constants.TriState.False
        TargetLevel_StepSize = 1
        HistoricTrialCount = 0
        SupportsManualPausing = False
        DefaultReferenceLevel = 68.34
        DefaultSpeechLevel = 65
        DefaultMaskerLevel = 65
        DefaultBackgroundLevel = 50
        DefaultContralateralMaskerLevel = 25
        MinimumReferenceLevel = 0 ' Not used
        MaximumReferenceLevel = 80 ' Not used
        MinimumLevel_Targets = 0 ' Not used
        MaximumLevel_Targets = 80 ' Not used
        MinimumLevel_Maskers = 0 ' Not used
        MaximumLevel_Maskers = 80 ' Not used
        MinimumLevel_Background = 0 ' Not used
        MaximumLevel_Background = 80 ' Not used
        MinimumLevel_ContralateralMaskers = 0 ' Not used
        MaximumLevel_ContralateralMaskers = 80 ' Not used
        AvailableExperimentNumbers = {}

        SoundOverlapDuration = 0.5

    End Sub



    Public Overrides ReadOnly Property AllowsManualSpeechLevelSelection As Boolean = False

    Public Overrides ReadOnly Property AllowsManualMaskingLevelSelection As Boolean = False

    Public Overrides ReadOnly Property AllowsManualBackgroundLevelSelection As Boolean = False

    Public Overrides ReadOnly Property CanHaveTargets As Boolean = False
    Public Overrides ReadOnly Property CanHaveMaskers As Boolean = False
    Public Overrides ReadOnly Property CanHaveBackgroundNonSpeech As Boolean = False
    Public Overrides ReadOnly Property CanHaveBackgroundSpeech As Boolean = False

    Public Overrides ReadOnly Property UseContralateralMasking_DefaultValue As Utils.TriState = Utils.Constants.TriState.False






    Private CurrentSipTestMeasurement As SipMeasurement
    Public SelectedSoundPropagationType As SoundPropagationTypes = SoundPropagationTypes.SimulatedSoundField
    Private RandomSeed As Integer? = Nothing
    Private SelectedTestparadigm As Testparadigm = Testparadigm.Quick
    Private MinimumStimulusOnsetTime As Double = 0.3
    Private MaximumStimulusOnsetTime As Double = 0.8
    Private TrialSoundMaxDuration As Double = 10
    Private UseBackgroundSpeech As Boolean = False
    Private MaximumResponseTime As Double = 4
    Private PretestSoundDuration As Double = 5
    Private UseVisualQue As Boolean = False
    Private ResponseAlternativeDelay As Double = 0.5
    Private DirectionalSimulationSet As String = "ARC - Harcellen - HATS - SiP"

    Private TestListCount As Integer = 11
    Private CurrentTestStage As Integer = 0

    Public Overrides Function InitializeCurrentTest() As Tuple(Of Boolean, String)

        SelectedTransducer = AvaliableTransducers(0)

        CurrentSipTestMeasurement = New SipMeasurement(CurrentParticipantID, SpeechMaterial.ParentTestSpecification, AdaptiveTypes.Fixed, SelectedTestparadigm)

        CurrentSipTestMeasurement.ExportTrialSoundFiles = False

        If UseSimulatedSoundField = True Then
            SelectedSoundPropagationType = SoundPropagationTypes.SimulatedSoundField

            'Dim AvailableSets = DirectionalSimulator.GetAvailableDirectionalSimulationSets(SelectedTransducer)
            'DirectionalSimulator.TrySetSelectedDirectionalSimulationSet(AvailableSets(1), SelectedTransducer, False)

            Dim FoundDirSimulator As Boolean = DirectionalSimulator.TrySetSelectedDirectionalSimulationSet(DirectionalSimulationSet, SelectedTransducer, False)
            If FoundDirSimulator = False Then
                Return New Tuple(Of Boolean, String)(False, "Unable to find the directional simulation set " & DirectionalSimulationSet)
            End If

        Else
            SelectedSoundPropagationType = SoundPropagationTypes.PointSpeakers
        End If

        'Setting up test trials to run
        PlanSiPTrials(SelectedSoundPropagationType, RandomSeed)

        If CurrentSipTestMeasurement.HasSimulatedSoundFieldTrials = True And DirectionalSimulator.SelectedDirectionalSimulationSetName = "" Then
            Return New Tuple(Of Boolean, String)(False, "The measurement requires a directional simulation set to be selected!")
        End If

        Return New Tuple(Of Boolean, String)(True, "")

    End Function


    Private Sub PlanSiPTrials(ByVal SoundPropagationType As SoundPropagationTypes, Optional ByVal RandomSeed As Integer? = Nothing)

        'Clearing any trials that may have been planned by a previous call
        CurrentSipTestMeasurement.ClearTrials()

        'Using all media sets
        Dim SelectedMediaSets As List(Of MediaSet) = AvailableMediasets

        'Creating a new random if seed is supplied
        If RandomSeed.HasValue Then CurrentSipTestMeasurement.Randomizer = New Random(RandomSeed)

        'Getting all lists 
        Dim AllLists = CurrentSipTestMeasurement.ParentTestSpecification.SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List)

        'Getting the sound source locations
        'Head slightly turned right (i.e. Speech on left side)
        Dim TargetStimulusLocations_HeadTurnedRight = {New SoundSourceLocation With {.HorizontalAzimuth = -10, .Distance = 1.45}}
        Dim MaskerLocations_HeadTurnedRight = {New SoundSourceLocation With {.HorizontalAzimuth = 170, .Distance = 1.45}}
        Dim BackgroundLocations_HeadTurnedRight = {New SoundSourceLocation With {.HorizontalAzimuth = -10, .Distance = 1.45}, New SoundSourceLocation With {.HorizontalAzimuth = 170, .Distance = 1.45}}

        'Head slightly turned left (i.e. Speech on right side)
        Dim TargetStimulusLocations_HeadTurnedLeft = {New SoundSourceLocation With {.HorizontalAzimuth = 10, .Distance = 1.45}}
        Dim MaskerLocations_HeadTurnedLeft = {New SoundSourceLocation With {.HorizontalAzimuth = 190, .Distance = 1.45}}
        Dim BackgroundLocations_HeadTurnedLeft = {New SoundSourceLocation With {.HorizontalAzimuth = 10, .Distance = 1.45}, New SoundSourceLocation With {.HorizontalAzimuth = 190, .Distance = 1.45}}

        'Creating Test units
        Dim PractiseTestUnit = New SiPTestUnit(CurrentSipTestMeasurement)
        Dim TestUnit1 = New SiPTestUnit(CurrentSipTestMeasurement)
        Dim TestUnit2 = New SiPTestUnit(CurrentSipTestMeasurement)
        'N.B. We're inserting the PractiseTestUnit later, since the code below adding test trials iterates over all added test units
        CurrentSipTestMeasurement.TestUnits.Add(TestUnit1)
        CurrentSipTestMeasurement.TestUnits.Add(TestUnit2)

        'Setting up Practise trials
        'Inserting practise trials test initially, at PNR 15 dB
        Dim PractiseLists As New List(Of SpeechMaterialComponent)
        For Each List In AllLists
            If List.IsPractiseComponent = True Then PractiseLists.Add(List)
        Next

        For i = 0 To PractiseLists.Count - 1

            Dim PractiseWords = PractiseLists(i).GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.Sentence)

            'Setting the first MediaSet as practise MediaSet
            Dim MediaSet = SelectedMediaSets(0)

            'Setting PNR to 15 dB
            Dim PNR = 15

            PractiseTestUnit.SpeechMaterialComponents.AddRange(PractiseWords)

            For c = 0 To PractiseWords.Count - 1
                Dim NewTrial As SipTrial = Nothing

                'Randomizing head turn for the practise trials
                Dim HeadTurn As Integer = Randomizer.Next(0, 2)
                If HeadTurn = 0 Then
                    NewTrial = New SipTrial(PractiseTestUnit, PractiseWords(c), MediaSet, SoundPropagationType, TargetStimulusLocations_HeadTurnedRight.ToArray, MaskerLocations_HeadTurnedRight.ToArray, BackgroundLocations_HeadTurnedRight, PractiseTestUnit.ParentMeasurement.Randomizer)
                ElseIf HeadTurn = 1 Then
                    NewTrial = New SipTrial(PractiseTestUnit, PractiseWords(c), MediaSet, SoundPropagationType, TargetStimulusLocations_HeadTurnedLeft.ToArray, MaskerLocations_HeadTurnedLeft.ToArray, BackgroundLocations_HeadTurnedLeft, PractiseTestUnit.ParentMeasurement.Randomizer)
                Else
                    Throw New Exception("This is a bug. The value of HeadTurn should always be 0 or 1.")
                End If

                'Setting levels
                NewTrial.SetLevels(ReferenceLevel, PNR)

                'Indicating that it's a practise trial
                NewTrial.IsTestTrial = False

                'Adding the trial to the test unit
                PractiseTestUnit.PlannedTrials.Add(NewTrial)
            Next
        Next

        'Setting up test trials

        Dim TestWordLists As New List(Of SpeechMaterialComponent)
        For Each List In AllLists
            If List.IsPractiseComponent = False Then TestWordLists.Add(List)
        Next

        'Selecting TestListCount random test lists to run
        Dim SelectedListsIndices = Utils.SampleWithoutReplacement(TestListCount, 0, TestWordLists.Count, Randomizer)
        Dim SelectedTestLists As New List(Of SpeechMaterialComponent)
        For Each TestListIndex In SelectedListsIndices
            SelectedTestLists.Add(TestWordLists(TestListIndex))
        Next


        'Creating a list of PNRs from which to sample
        Dim OrderedPNRs As New List(Of Double)
        Dim TempPnr As Double = 15
        For i = 0 To TestListCount - 1
            OrderedPNRs.Add(TempPnr)
            TempPnr -= 3.5
        Next

        'Sampling a PNR-order
        Dim RandomizedPnrIndices = Utils.SampleWithoutReplacement(OrderedPNRs.Count, 0, OrderedPNRs.Count, Randomizer)
        Dim RandomizedPNRs As New List(Of Double)
        For Each Index In RandomizedPnrIndices
            RandomizedPNRs.Add(OrderedPNRs(Index))
        Next


        For i = 0 To SelectedTestLists.Count - 1

            Dim TestWords = SelectedTestLists(i).GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.Sentence)
            'Sampling a MediaSet
            Dim RandomizedMediaSetIndex = Utils.SampleWithoutReplacement(1, 0, SelectedMediaSets.Count, Randomizer)(0)
            Dim MediaSet = SelectedMediaSets(RandomizedMediaSetIndex)

            Dim PNR = RandomizedPNRs(i)

            TestUnit1.SpeechMaterialComponents.AddRange(TestWords)
            TestUnit2.SpeechMaterialComponents.AddRange(TestWords)

            For Each CurrentTestUnit In CurrentSipTestMeasurement.TestUnits

                For c = 0 To TestWords.Count - 1
                    Dim NewTrial As SipTrial = Nothing

                    'Randomizing head turn for every single trial, freely between test-retest units (which reflects a real life test situation)
                    Dim HeadTurn As Integer = Randomizer.Next(0, 2)
                    If HeadTurn = 0 Then
                        NewTrial = New SipTrial(CurrentTestUnit, TestWords(c), MediaSet, SoundPropagationType, TargetStimulusLocations_HeadTurnedRight.ToArray, MaskerLocations_HeadTurnedRight.ToArray, BackgroundLocations_HeadTurnedRight, CurrentTestUnit.ParentMeasurement.Randomizer)
                    ElseIf HeadTurn = 1 Then
                        NewTrial = New SipTrial(CurrentTestUnit, TestWords(c), MediaSet, SoundPropagationType, TargetStimulusLocations_HeadTurnedLeft.ToArray, MaskerLocations_HeadTurnedLeft.ToArray, BackgroundLocations_HeadTurnedLeft, CurrentTestUnit.ParentMeasurement.Randomizer)
                    Else
                        Throw New Exception("This is a bug. The value of HeadTurn should always be 0 or 1.")
                    End If

                    'Setting levels
                    NewTrial.SetLevels(ReferenceLevel, PNR)

                    'Adding the trial to the test unit
                    CurrentTestUnit.PlannedTrials.Add(NewTrial)

                Next
            Next
        Next

        'Inserting the practise trials TestUnit test initially
        CurrentSipTestMeasurement.TestUnits.Insert(0, PractiseTestUnit)

        'Randomizing the order within units
        For Each TestUnit In CurrentSipTestMeasurement.TestUnits
            Dim RandomList As New List(Of SipTrial)
            Do Until TestUnit.PlannedTrials.Count = 0
                Dim RandomIndex As Integer = CurrentSipTestMeasurement.Randomizer.Next(0, TestUnit.PlannedTrials.Count)
                RandomList.Add(TestUnit.PlannedTrials(RandomIndex))
                TestUnit.PlannedTrials.RemoveAt(RandomIndex)
            Loop
            TestUnit.PlannedTrials = RandomList
        Next

        'Noting the test stage in each trial
        For i = 0 To CurrentSipTestMeasurement.TestUnits.Count - 1
            For Each TU In CurrentSipTestMeasurement.TestUnits
                For Each Trial In TU.PlannedTrials
                    Trial.TestStage = CurrentTestStage
                Next
            Next
        Next

        'Sorting Trials so that the city envoriment either gets randomized first or last in each test unit
        For Each TestUnit In CurrentSipTestMeasurement.TestUnits
            Dim CityTrials As New List(Of SipTrial)
            Dim HomeTrials As New List(Of SipTrial)

            For Each TestTrial In TestUnit.PlannedTrials
                If TestTrial.MediaSet.MediaSetName.StartsWith("City") Then
                    CityTrials.Add(TestTrial)
                ElseIf TestTrial.MediaSet.MediaSetName.StartsWith("Home") Then
                    HomeTrials.Add(TestTrial)
                Else
                    Throw New Exception("Unknown MediaSet name")
                End If
            Next

            TestUnit.PlannedTrials.Clear()

            Dim RandomizedMediaSetOrder As Integer = Randomizer.Next(0, 2)
            If RandomizedMediaSetOrder = 0 Then
                TestUnit.PlannedTrials.AddRange(CityTrials)
                TestUnit.PlannedTrials.AddRange(HomeTrials)
            ElseIf RandomizedMediaSetOrder = 1 Then
                TestUnit.PlannedTrials.AddRange(HomeTrials)
                TestUnit.PlannedTrials.AddRange(CityTrials)
            Else
                Throw New Exception("This is a bug. The value of RandomizedMediaSetOrder should always be 0 or 1.")
            End If

        Next


        'Adding the trials CurrentSipTestMeasurement (from which they can be drawn during testing)
        For Each TestUnit In CurrentSipTestMeasurement.TestUnits
            For Each Trial In TestUnit.PlannedTrials
                CurrentSipTestMeasurement.PlannedTrials.Add(Trial)
            Next
        Next

    End Sub

    Private Sub InitiateTestByPlayingSound()

        'Sets the measurement datetime
        CurrentSipTestMeasurement.MeasurementDateTime = DateTime.Now

        'Cretaing a context sound without any test stimulus, that runs for approx TestSetup.PretestSoundDuration seconds, using audio from the first selected MediaSet
        Dim SelectedMediaSets As List(Of MediaSet) = AvailableMediasets

        Dim TestSound As Audio.Sound = CreateInitialSound(SelectedMediaSets(0))

        'Plays sound
        SoundPlayer.SwapOutputSounds(TestSound)

        'Premixing the first 10 sounds 
        CurrentSipTestMeasurement.PreMixTestTrialSoundsOnNewTread(SelectedTransducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, Randomizer, TrialSoundMaxDuration, UseBackgroundSpeech, 10)

    End Sub


    Public Function CreateInitialSound(ByRef SelectedMediaSet As MediaSet, Optional ByVal Duration As Double? = Nothing) As Audio.Sound

        Try

            'Setting up the SiP-trial sound mix
            Dim MixStopWatch As New Stopwatch
            MixStopWatch.Start()

            'Sets a List of SoundSceneItem in which to put the sounds to mix
            Dim ItemList = New List(Of SoundSceneItem)

            Dim SoundWaveFormat As Audio.Formats.WaveFormat = Nothing

            'Getting a background non-speech sound
            Dim BackgroundNonSpeech_Sound As Audio.Sound = SpeechMaterial.GetBackgroundNonspeechSound(SelectedMediaSet, 0)

            'Stores the sample rate and the wave format
            Dim CurrentSampleRate As Integer = BackgroundNonSpeech_Sound.WaveFormat.SampleRate
            SoundWaveFormat = BackgroundNonSpeech_Sound.WaveFormat

            'Sets a total pretest sound length
            Dim TrialSoundLength As Integer
            If Duration.HasValue Then
                TrialSoundLength = Duration * SoundWaveFormat.SampleRate
            Else
                TrialSoundLength = (PretestSoundDuration + 4) * CurrentSampleRate 'Adds 4 seconds to allow for potential delay caused by the mixing time of the first test trial sounds
            End If

            'Copies copies random sections of the background non-speech sound into two sounds
            Dim Background1 = BackgroundNonSpeech_Sound.CopySection(1, Randomizer.Next(0, BackgroundNonSpeech_Sound.WaveData.SampleData(1).Length - TrialSoundLength - 2), TrialSoundLength)
            Dim Background2 = BackgroundNonSpeech_Sound.CopySection(1, Randomizer.Next(0, BackgroundNonSpeech_Sound.WaveData.SampleData(1).Length - TrialSoundLength - 2), TrialSoundLength)

            'Sets up fading specifications for the background signals
            Dim FadeSpecs_Background = New List(Of Audio.DSP.Transformations.FadeSpecifications)
            FadeSpecs_Background.Add(New Audio.DSP.Transformations.FadeSpecifications(Nothing, 0, 0, CurrentSampleRate * 1))
            FadeSpecs_Background.Add(New Audio.DSP.Transformations.FadeSpecifications(0, Nothing, -CurrentSampleRate * 0.01))

            'Adds the background (non-speech) signals, with fade, duck and location specifications
            Dim LevelGroup As Integer = 1 ' The level group value is used to set the added sound level of items sharing the same (arbitrary) LevelGroup value to the indicated sound level. (Thus, the sounds with the same LevelGroup value are measured together.)

            Dim BackgroundLocations_HeadTurnedRight = {New SoundSourceLocation With {.HorizontalAzimuth = -10, .Distance = 1.45}, New SoundSourceLocation With {.HorizontalAzimuth = 170, .Distance = 1.45}}
            ItemList.Add(New SoundSceneItem(Background1, 1, SelectedMediaSet.BackgroundNonspeechRealisticLevel, LevelGroup,
                                            BackgroundLocations_HeadTurnedRight(0), SoundSceneItem.SoundSceneItemRoles.BackgroundNonspeech, 0,,,, FadeSpecs_Background))
            ItemList.Add(New SoundSceneItem(Background2, 1, SelectedMediaSet.BackgroundNonspeechRealisticLevel, LevelGroup,
                                            BackgroundLocations_HeadTurnedRight(1), SoundSceneItem.SoundSceneItemRoles.BackgroundNonspeech, 0,,,, FadeSpecs_Background))
            LevelGroup += 1

            MixStopWatch.Stop()
            If LogToConsole = True Then Console.WriteLine("Prepared sounds in " & MixStopWatch.ElapsedMilliseconds & " ms.")
            MixStopWatch.Restart()

            'Creating the mix by calling CreateSoundScene of the current Mixer
            Dim MixedInitialSound As Audio.Sound = SelectedTransducer.Mixer.CreateSoundScene(ItemList, False, False, SelectedSoundPropagationType)

            If LogToConsole = True Then Console.WriteLine("Mixed sound in " & MixStopWatch.ElapsedMilliseconds & " ms.")

            'TODO: Here we can simulate and/or compensate for hearing loss:
            'SimulateHearingLoss,
            'CompensateHearingLoss

            Return MixedInitialSound

        Catch ex As Exception
            Utils.SendInfoToLog(ex.ToString, "ExceptionsDuringTesting")
            Return Nothing
        End Try

    End Function


    Private Sub PrepareTestTrialSound()

        Try

            If (CurrentSipTestMeasurement.ObservedTrials.Count + 3) Mod 10 = 0 Then
                'Premixing the next 10 sounds, starting three trials before the next is needed 
                CurrentSipTestMeasurement.PreMixTestTrialSoundsOnNewTread(SelectedTransducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, Randomizer, TrialSoundMaxDuration, UseBackgroundSpeech, 10)
            End If

            'Waiting for the background thread to finish mixing
            Dim WaitPeriods As Integer = 0
            While CurrentTestTrial.Sound Is Nothing
                WaitPeriods += 1
                Threading.Thread.Sleep(100)
                If LogToConsole = True Then Console.WriteLine("Waiting for sound to mix: " & WaitPeriods * 100 & " ms")
            End While

        Catch ex As Exception
            'Ignores any exceptions...
            'Utils.SendInfoToLog(ex.ToString, "ExceptionsDuringTesting")
        End Try

    End Sub


    ''' <summary>
    ''' This method can be called by the backend in order to display a message box message to the user.
    ''' </summary>
    ''' <param name="Message"></param>
    Private Sub ShowMessageBox(Message As String, Optional ByVal Title As String = "")

        If Title = "" Then
            Select Case GuiLanguage
                Case Utils.Constants.Languages.Swedish
                    Title = "SiP-testet"
                Case Else
                    Title = "SiP-test"
            End Select
        End If

        Messager.MsgBox(Message, MsgBoxStyle.Information, Title)

    End Sub

    Public Overrides Function GetSpeechTestReply(sender As Object, e As SpeechTestInputEventArgs) As SpeechTestReplies

        If e IsNot Nothing Then

            'Corrects the trial response, based on the given response
            Dim CorrectWordsList As New List(Of String)

            'Resets the CurrentTestTrial.ScoreList
            'And also storing SiP-test type data
            CurrentTestTrial.ScoreList = New List(Of Integer)
            Select Case e.LinguisticResponses(0)
                Case CurrentTestTrial.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling")
                    CurrentTestTrial.ScoreList.Add(1)
                    DirectCast(CurrentTestTrial, SipTrial).Result = PossibleResults.Correct
                    DirectCast(CurrentTestTrial, SipTrial).IsCorrect = True

                Case ""
                    CurrentTestTrial.ScoreList.Add(0)
                    DirectCast(CurrentTestTrial, SipTrial).Result = PossibleResults.Missing

                    'Randomizing IsCorrect with a 1/3 chance for True
                    Dim ChanceList As New List(Of Boolean) From {True, False, False}
                    Dim RandomIndex As Integer = Randomizer.Next(ChanceList.Count)
                    DirectCast(CurrentTestTrial, SipTrial).IsCorrect = ChanceList(RandomIndex)

                Case Else
                    CurrentTestTrial.ScoreList.Add(0)
                    DirectCast(CurrentTestTrial, SipTrial).Result = PossibleResults.Incorrect
                    DirectCast(CurrentTestTrial, SipTrial).IsCorrect = False

            End Select

            DirectCast(CurrentTestTrial, SipTrial).Response = e.LinguisticResponses(0)

            'This is an incoming test trial response
            If CurrentTestTrial IsNot Nothing Then
                CurrentSipTestMeasurement.MoveTrialToHistory(CurrentTestTrial)
            End If

        Else
            'Nothing to correct (this should be the start of a new test)
            'Playing initial sound, and premixing trials
            InitiateTestByPlayingSound()

        End If

        'TODO: We must store the responses and response times!!!

        'Calculating the speech level
        'Dim ProtocolReply = SelectedTestProtocol.NewResponse(ObservedTrials)
        Dim ProtocolReply = New TestProtocol.NextTaskInstruction With {.Decision = SpeechTestReplies.GotoNextTrial}

        If CurrentSipTestMeasurement.TestUnits(CurrentTestStage).PlannedTrials.Count = 0 Then

            Select Case CurrentTestStage
                Case 0
                    'Going to next test stage
                    CurrentTestStage += 1

                    'Informing the participant
                    ProtocolReply.Decision = SpeechTestReplies.PauseTestingWithCustomInformation
                    PauseInformation = "Övningstestet är klart." & vbCrLf & vbCrLf & " Klicka på Ok för att starta det riktiga testet!"

                Case 1
                    'Going to next test stage
                    CurrentTestStage += 1

                    'Informing the participant
                    ProtocolReply.Decision = SpeechTestReplies.PauseTestingWithCustomInformation
                    PauseInformation = "Den första delen av testet är klar." & vbCrLf & vbCrLf & " Klicka på OK för att starta den andra och sista delen av testet!"

                Case 2
                    'Test is completed
                    Return SpeechTestReplies.TestIsCompleted
            End Select
        End If

        'Preparing next trial if needed
        If ProtocolReply.Decision = SpeechTestReplies.GotoNextTrial Then
            PrepareNextTrial(ProtocolReply)
        End If

        Return ProtocolReply.Decision

    End Function


    Private Sub PrepareNextTrial(ByVal NextTaskInstruction As TestProtocol.NextTaskInstruction)

        'Preparing the next trial
        CurrentTestTrial = CurrentSipTestMeasurement.TestUnits(CurrentTestStage).PlannedTrials(0) ' GetNextTrial()
        CurrentTestTrial.TestStage = NextTaskInstruction.TestStage
        CurrentTestTrial.Tasks = 1
        CurrentTestTrial.ResponseAlternativeSpellings = New List(Of List(Of SpeechTestResponseAlternative))
        Dim ResponseAlternatives As New List(Of SpeechTestResponseAlternative)

        'Adding the current word spelling as a response alternative
        ResponseAlternatives.Add(New SpeechTestResponseAlternative With {.Spelling = CurrentTestTrial.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling"), .IsScoredItem = CurrentTestTrial.SpeechMaterialComponent.IsKeyComponent, .ParentTestTrial = CurrentTestTrial})

        'Picking random response alternatives from all available test words
        Dim AllContrastingWords = CurrentTestTrial.SpeechMaterialComponent.GetSiblingsExcludingSelf()
        For Each ContrastingWord In AllContrastingWords
            ResponseAlternatives.Add(New SpeechTestResponseAlternative With {.Spelling = ContrastingWord.GetCategoricalVariableValue("Spelling"), .IsScoredItem = ContrastingWord.IsKeyComponent, .ParentTestTrial = CurrentTestTrial})
        Next

        'Shuffling the order of response alternatives
        ResponseAlternatives = Utils.Shuffle(ResponseAlternatives, Randomizer).ToList

        'Adding the response alternatives
        CurrentTestTrial.ResponseAlternativeSpellings.Add(ResponseAlternatives)

        'Mixing trial sound
        PrepareTestTrialSound()

        'Storing the LinguisticSoundStimulusStartTime and the LinguisticSoundStimulusDuration 
        CurrentTestTrial.LinguisticSoundStimulusStartTime = DirectCast(CurrentTestTrial, SipTrial).TestWordStartTime
        CurrentTestTrial.LinguisticSoundStimulusDuration = DirectCast(CurrentTestTrial, SipTrial).TestWordCompletedTime - CurrentTestTrial.LinguisticSoundStimulusStartTime
        CurrentTestTrial.MaximumResponseTime = MaximumResponseTime

        'Setting visual que intervals
        Dim ShowVisualQueTimer_Interval As Double
        Dim HideVisualQueTimer_Interval As Double
        Dim ShowResponseAlternativesTimer_Interval As Double
        Dim MaxResponseTimeTimer_Interval As Double

        If UseVisualQue = True Then
            ShowVisualQueTimer_Interval = System.Math.Max(1, DirectCast(CurrentTestTrial, SipTrial).TestWordStartTime * 1000)
            HideVisualQueTimer_Interval = System.Math.Max(2, DirectCast(CurrentTestTrial, SipTrial).TestWordCompletedTime * 1000)
            ShowResponseAlternativesTimer_Interval = HideVisualQueTimer_Interval + 1000 * ResponseAlternativeDelay 'TestSetup.CurrentEnvironment.TestSoundMixerSettings.ResponseAlternativeDelay * 1000
            MaxResponseTimeTimer_Interval = System.Math.Max(1, ShowResponseAlternativesTimer_Interval + 1000 * MaximumResponseTime)  ' TestSetup.CurrentEnvironment.TestSoundMixerSettings.MaximumResponseTime * 1000
        Else
            ShowResponseAlternativesTimer_Interval = System.Math.Max(1, DirectCast(CurrentTestTrial, SipTrial).TestWordStartTime * 1000) + 1000 * ResponseAlternativeDelay
            MaxResponseTimeTimer_Interval = System.Math.Max(2, DirectCast(CurrentTestTrial, SipTrial).TestWordCompletedTime * 1000) + 1000 * MaximumResponseTime
        End If



        'Setting trial events
        CurrentTestTrial.TrialEventList = New List(Of ResponseViewEvent)
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 1, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})
        Dim ShowTestSide As Boolean = False
        If ShowTestSide = True Then
            CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 400, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternativePositions})
        End If

        If UseVisualQue = True Then
            CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = ShowVisualQueTimer_Interval, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowVisualCue})
            CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = HideVisualQueTimer_Interval, .Type = ResponseViewEvent.ResponseViewEventTypes.HideVisualCue})
        End If
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = ShowResponseAlternativesTimer_Interval, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = MaxResponseTimeTimer_Interval, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut})

    End Sub


    Private Function GetAverageHeadTurnScores(Optional ByVal TurnedRight As Boolean? = Nothing)

        Dim TrialScoreList As New List(Of Integer)
        For Each TestUnit In CurrentSipTestMeasurement.TestUnits
            For Each Trial In TestUnit.ObservedTrials

                'Skipping to next if it's a practise trial
                If Trial.IsTestTrial = False Then Continue For

                If TurnedRight.HasValue = False Then

                    'Getting all results
                    If Trial.IsCorrect = True Then
                        TrialScoreList.Add(1)
                    Else
                        TrialScoreList.Add(0)
                    End If

                Else

                    Dim TrialIsTurnRight As Boolean
                    If Trial.TargetStimulusLocations(0).HorizontalAzimuth = -10 Then
                        TrialIsTurnRight = True
                    ElseIf Trial.TargetStimulusLocations(0).HorizontalAzimuth = 10 Then
                        TrialIsTurnRight = False
                    Else
                        Throw New Exception("Incompatible head-turn data. This is a bug!")
                    End If

                    'Getting results only from the indicated head turn
                    If TurnedRight = True And TrialIsTurnRight = True Then
                        If Trial.IsCorrect = True Then
                            TrialScoreList.Add(1)
                        Else
                            TrialScoreList.Add(0)
                        End If
                    End If

                    If TurnedRight = False And TrialIsTurnRight = False Then
                        If Trial.IsCorrect = True Then
                            TrialScoreList.Add(1)
                        Else
                            TrialScoreList.Add(0)
                        End If
                    End If

                End If

            Next
        Next

        If TrialScoreList.Count > 0 Then
            Return TrialScoreList.Average
        Else
            Return -1
        End If

    End Function


    Public Overrides Function GetResultStringForGui() As String

        Dim TestResultSummaryLines = New List(Of String)
        TestResultSummaryLines.Add("Resultat: " & vbTab & Math.Rounding(100 * GetAverageHeadTurnScores(Nothing)) & " % rätt")
        'TestResult.TestResultSummaryLines.Add("Head turned left: " & Math.Rounding(100 * GetAverageHeadTurnScores(False)) & " %")
        'TestResult.TestResultSummaryLines.Add("Head turned right : " & Math.Rounding(100 * GetAverageHeadTurnScores(True)) & " %")

        Return String.Join(vbCrLf, TestResultSummaryLines)

    End Function

    Public Overrides Function GetTestTrialResultExportString() As String
        Return "Export of trial level test results is not yet implemented"
    End Function

    Public Overrides Function GetTestResultsExportString() As String

        Dim ExportStringList As New List(Of String)

        For i = 0 To CurrentSipTestMeasurement.ObservedTrials.Count - 1
            If i = 0 Then
                ExportStringList.Add("TrialIndex" & vbTab & CurrentSipTestMeasurement.ObservedTrials(i).TestResultColumnHeadings)
            End If
            ExportStringList.Add(i & vbTab & CurrentSipTestMeasurement.ObservedTrials(i).TestResultAsTextRow)
        Next

        Return String.Join(vbCrLf, ExportStringList)

    End Function

    Public Overrides Sub FinalizeTest()
        'Throw New NotImplementedException()
    End Sub

    Public Overrides Function CreatePreTestStimulus() As Tuple(Of Audio.Sound, String)
        Return Nothing
    End Function

    Public Overrides Sub UpdateHistoricTrialResults(sender As Object, e As SpeechTestInputEventArgs)
        'Not supported, just ignores any calls
    End Sub


End Class