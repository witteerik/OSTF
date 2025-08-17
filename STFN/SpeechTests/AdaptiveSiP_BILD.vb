Imports STFN.AdaptiveSiP_BILD
Imports STFN.Core.Audio.SoundScene
Imports STFN.Core.SipTest
Imports STFN.Core.TestProtocol
Imports STFN.Core.Utils
Imports STFN.Core
Imports STFN.SipTest
Imports STFN.TestProtocol
Imports STFN.Utils
Imports STFN

Public Class AdaptiveSiP_BILD

    Inherits SipBaseSpeechTest

    Public Overrides ReadOnly Property FilePathRepresentation As String = "Adaptive_SipTest_BILD"

    Public Sub New(ByVal SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)
        ApplyTestSpecificSettings()
    End Sub

    Public Shadows Sub ApplyTestSpecificSettings()

        TesterInstructions = "--  SiP-testet (Adaptivt) (BILD) --" & vbCrLf & vbCrLf &
            "För detta test behövs inga inställningar." & vbCrLf & vbCrLf &
            "1. Informera deltagaren om hur testet går till." & vbCrLf &
            "2. Vänd skärmen till deltagaren. Be sedan deltagaren klicka på start för att starta testet."

        ParticipantInstructions = "--  SiP-testet  --" & vbCrLf & vbCrLf &
                "Under testet ska du lyssna efter enstaviga ord som uttalas i en stadsmiljö." & vbCrLf &
                "Orden spelas upp fem i taget och du ska ange så snabbt och korrekt som möjligt vilka ord du uppfattat." & vbCrLf &
                "Svarsalternativen finns i en tabell med tre kolumner och fem rader." & vbCrLf &
                "Första ordet som spelas upp finns i nedersta raden." & vbCrLf &
                "Nästa ord finns i raden näst längst ned. Klicka på det ord du hört så snart du kan." & vbCrLf &
                "Fortsätt uppåt tills du besvarat alla ord." & vbCrLf &
                "Gissa om du är osäker. " & vbCrLf &
                "Du har maximalt " & MaximumResponseTime & " sekunder på dig att svara efter att sista ordet spelats upp." & vbCrLf &
                "Om svarsalternativen ändras till i röd färg har du inte svarat i tid. Testet går då vidare." & vbCrLf &
                "Testet tar ungefär sex minuter. " & vbCrLf &
                "Starta testet genom att klicka på knappen 'Start'"

        ParticipantInstructionsButtonText = "Deltagarinstruktion"

        'SupportsManualPausing = False

        ShowGuiChoice_TargetLocations = False
        ShowGuiChoice_MaskerLocations = False
        ShowGuiChoice_BackgroundNonSpeechLocations = False
        ShowGuiChoice_BackgroundSpeechLocations = False

        MinimumStimulusOnsetTime = 0.3 + 0.3 ' 0.3 in sound field
        MaximumStimulusOnsetTime = 0.8 + 0.3 ' 0.3 in sound field

        ResponseAlternativeDelay = 0.1

        SupportsManualPausing = True

        'Setting up directional simulation
        DirectionalSimulationSet = "ARC - Harcellen - HATS - SiP - HR"
        PopulateSoundSourceLocationCandidates()
        SimulatedSoundField = True


        GuiResultType = GuiResultTypes.VisualResults

    End Sub

    <ExludeFromPropertyListing>
    Public Overrides ReadOnly Property ShowGuiChoice_TargetSNRLevel As Boolean = False

    'Private PresetName As String = "Adaptive SiP - BILDV"
    Private PresetName As String = "Adaptive SiP"
    Private PresetName_PractiseTest As String = "Adaptive SiP - Practise"

    'Defines the number of times each test word group is tested
    Private TestLength As Integer
    'Determines the number of test trials (this must be a multiple of 3, so that each trial is tested an equal number of times)
    Private TrialCount As Integer

    Private TaskStandardTime As Double = 3 ' The time each test word sound occupies

    Private PractisePNR As Double = 8
    Private StartPNR As Double = 8

    Private PlannedTestTrials As New TestTrialCollection
    Protected ObservedTrials As New TestTrialCollection

    Private SpeakerSetup As AdaptiveSiP_BILD_SpeakerSetups = AdaptiveSiP_BILD_SpeakerSetups.Frontal_0_0_0

    Private UseBILDAttenuation As Boolean = False

    Public Enum AdaptiveSiP_BILD_SpeakerSetups
        Bilateral_30_0_30
        Frontal_0_0_0
    End Enum

    Private BILD_FilterKernel As STFN.Core.Audio.Sound = Nothing
    Private BILD_FftFormat = New STFN.Core.Audio.Formats.FftFormat()

    Public Overrides Function GetObservedTestTrials() As IEnumerable(Of TestTrial)
        Return ObservedTrials
    End Function


    Public Overrides Function InitializeCurrentTest() As Tuple(Of Boolean, String)

        'Transducer = AvaliableTransducers(0)

        CurrentSipTestMeasurement = New SipMeasurement(CurrentParticipantID, SpeechMaterial.ParentTestSpecification, AdaptiveTypes.Fixed, SelectedTestparadigm)

        CurrentSipTestMeasurement.ExportTrialSoundFiles = False ' TODO Set to false!

        If SimulatedSoundField = True Then

            'Return New Tuple(Of Boolean, String)(False, "Sound field simulation is not yet available in Adaptive SiP!")

            SelectedSoundPropagationType = SoundPropagationTypes.SimulatedSoundField

            'Dim AvailableSets = DirectionalSimulator.GetAvailableDirectionalSimulationSets(SelectedTransducer)
            'DirectionalSimulator.TrySetSelectedDirectionalSimulationSet(AvailableSets(1), SelectedTransducer, False)

            Dim FoundDirSimulator As Boolean = DirectionalSimulator.TrySetSelectedDirectionalSimulationSet(DirectionalSimulationSet, Transducer, False)
            If FoundDirSimulator = False Then
                Return New Tuple(Of Boolean, String)(False, "Unable to find the directional simulation set " & DirectionalSimulationSet)
            End If

        Else
            SelectedSoundPropagationType = SoundPropagationTypes.PointSpeakers
        End If

        'Defines the number of times each test word group is tested
        If IsPractiseTest = True Then
            TestLength = 1
        Else
            TestLength = 10
        End If
        'Determines the number of test trials (this must be amultiple of 3, so that each trial is tested an equal number of times)
        TrialCount = TestLength * 3

        'Setting up test trials to run
        PlanSiPTrials(SelectedSoundPropagationType, RandomSeed)

        'Preparing sounds to use
        PrepareSounds()

        'If CurrentSipTestMeasurement.HasSimulatedSoundFieldTrials = True And DirectionalSimulator.SelectedDirectionalSimulationSetName = "" Then
        '    Return New Tuple(Of Boolean, String)(False, "The measurement requires a directional simulation set to be selected!")
        'End If

        'Creating a test protocol
        TestProtocol = New BrandKollmeier2002_TestProtocol() With {.TargetScore = 2 / 3} 'Setting the target threshold to 2/3 as this is the expected midpoint of the psychometric function, given that chance score is 1/3.

        'Setting the start PNR value. (Easier in the practise test.)
        Dim InitialAdaptiveValue As Double
        If IsPractiseTest = True Then
            InitialAdaptiveValue = PractisePNR
        Else
            InitialAdaptiveValue = StartPNR
        End If

        TestProtocol.InitializeProtocol(New TestProtocol.NextTaskInstruction With {.AdaptiveValue = InitialAdaptiveValue, .TestStage = 0, .TestLength = TrialCount})

        Return New Tuple(Of Boolean, String)(True, "")

    End Function


    Private Sub PlanSiPTrials(ByVal SoundPropagationType As SoundPropagationTypes, Optional ByVal RandomSeed As Integer? = Nothing)

        'Clearing any trials that may have been planned by a previous call
        CurrentSipTestMeasurement.ClearTrials()

        'Creating a new random if seed is supplied
        If RandomSeed.HasValue Then CurrentSipTestMeasurement.Randomizer = New Random(RandomSeed)

        'Sampling a MediaSet
        'Dim MediaSet = SelectedMediaSet
        Dim SelectedMediaSets As List(Of MediaSet) = AvailableMediasets
        Dim MediaSet = SelectedMediaSets(1) ' Selecting the female voice

        'Getting all lists 

        'Getting the preset
        Dim TestLists As List(Of SpeechMaterialComponent) = Nothing
        If IsPractiseTest = True Then
            TestLists = CurrentSipTestMeasurement.ParentTestSpecification.SpeechMaterial.Presets.GetPretest(PresetName_PractiseTest).Members 'TODO! Specify correct members in text file
        Else
            TestLists = CurrentSipTestMeasurement.ParentTestSpecification.SpeechMaterial.Presets.GetPretest(PresetName).Members 'TODO! Specify correct members in text file
        End If


        'Getting the sound source locations
        'Head slightly turned right (i.e. Speech on left side)
        Dim TargetStimulusLocations() As SoundSourceLocation = Nothing
        Dim MaskerLocations() As SoundSourceLocation = Nothing
        Dim BackgroundLocations() As SoundSourceLocation = Nothing


        Select Case SpeakerSetup
            Case AdaptiveSiP_BILD_SpeakerSetups.Bilateral_30_0_30
                TargetStimulusLocations = {New SoundSourceLocation With {.HorizontalAzimuth = 0, .Distance = 1.45}}
                MaskerLocations = {New SoundSourceLocation With {.HorizontalAzimuth = -30, .Distance = 1.45}, New SoundSourceLocation With {.HorizontalAzimuth = 30, .Distance = 1.45}}
                BackgroundLocations = {New SoundSourceLocation With {.HorizontalAzimuth = -30, .Distance = 1.45}, New SoundSourceLocation With {.HorizontalAzimuth = 30, .Distance = 1.45}}

            Case AdaptiveSiP_BILD_SpeakerSetups.Frontal_0_0_0
                TargetStimulusLocations = {New SoundSourceLocation With {.HorizontalAzimuth = 0, .Distance = 1.45}}
                MaskerLocations = {New SoundSourceLocation With {.HorizontalAzimuth = 0, .Distance = 1.45}, New SoundSourceLocation With {.HorizontalAzimuth = 0, .Distance = 1.45}}
                BackgroundLocations = {New SoundSourceLocation With {.HorizontalAzimuth = 0, .Distance = 1.45}, New SoundSourceLocation With {.HorizontalAzimuth = 0, .Distance = 1.45}}

            Case Else
                Throw New NotImplementedException
        End Select

        'Creating one test unit per list
        Dim CurrentTestUnit = New SiPTestUnit(CurrentSipTestMeasurement)
        CurrentSipTestMeasurement.TestUnits.Add(CurrentTestUnit)

        'Creating test trials
        For i = 0 To TestLength - 1

            'Creating a random order to draw words from 
            Dim RandomList As New SortedList(Of Integer, List(Of Integer)) 'TWG index, TW index presentation order
            For twgi = 0 To TestLists.Count - 1

                If IsPractiseTest = True Then
                    'Sampling with replacement, so that any test word may occur in the practise test
                    Dim PractiseRandomList As New List(Of Integer)
                    PractiseRandomList.Add(CurrentSipTestMeasurement.Randomizer.Next(0, 3))
                    PractiseRandomList.Add(CurrentSipTestMeasurement.Randomizer.Next(0, 3))
                    PractiseRandomList.Add(CurrentSipTestMeasurement.Randomizer.Next(0, 3))
                    RandomList.Add(twgi, PractiseRandomList)

                Else
                    'Sampling without replacement to get en equal number of presentations of all test words
                    RandomList.Add(twgi, DSP.SampleWithoutReplacement(3, 0, 3, CurrentSipTestMeasurement.Randomizer).ToList)
                End If
            Next

            'Iterating over three TWG indices
            For w = 0 To 2

                'Starting with left for every other list (and right for the other) and then swapping between every presentation
                Dim NewTestTrial As New TestTrial
                NewTestTrial.SubTrials = New TestTrialCollection

                For twgi = 0 To TestLists.Count - 1

                    Dim SMC = TestLists(twgi).ChildComponents(RandomList(twgi)(w))
                    Dim NewSipSubTrial = New SipTrial(CurrentTestUnit, SMC, MediaSet, SoundPropagationType, TargetStimulusLocations.ToArray, MaskerLocations.ToArray, BackgroundLocations, CurrentSipTestMeasurement.Randomizer)
                    NewTestTrial.SubTrials.Add(NewSipSubTrial)

                Next

                'Shuffling the order of sub-trials
                NewTestTrial.SubTrials.Shuffle(CurrentSipTestMeasurement.Randomizer)

                'Adding the test trial to the list of planned trials
                PlannedTestTrials.Add(NewTestTrial)

            Next
        Next



        ''Putting two presentations each of left turn trials in even-index lists in a row, and the swapping to right turn trials, and so on..., in CurrentSipTestMeasurement (from which they can be drawn during testing)
        'For TrialIndexInList = 0 To NumberOfTrialsPerList - 1 Step 2
        '    For TrialIndexInList_shift = 0 To 1
        '        For u_shift = 0 To 1
        '            For u = 0 To CurrentSipTestMeasurement.TestUnits.Count - 1 Step 2
        '                CurrentSipTestMeasurement.PlannedTrials.Add(CurrentSipTestMeasurement.TestUnits(u + u_shift).PlannedTrials(TrialIndexInList + TrialIndexInList_shift))
        '            Next
        '        Next
        '    Next
        'Next


    End Sub

    Private Sub PrepareSounds()

        Try



            SpeechMaterialComponent.ClearAllLoadedSounds()

            'Collecting sounds
            Dim SoundsToUse As New SortedList(Of String, STFN.Core.Audio.Sound) ' Path, just remember to not overwrite the files!!!
            For Each Trial In PlannedTestTrials
                For Each SubTrial As SipTrial In Trial.SubTrials

                    'Loading test word sounds
                    For i = 0 To SubTrial.MediaSet.MediaAudioItems - 1

                        Dim SmaComponents As New List(Of STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent)
                        Dim LoadedSound = SubTrial.SpeechMaterialComponent.GetSound(SubTrial.MediaSet, i, 1,,,,,,,,, SmaComponents)

                        If SmaComponents.Count = 1 Then
                            LoadedSound.SourcePath = SmaComponents(0).SourceFilePath
                        Else
                            Throw New Exception("Error in expected test word sound files.")
                        End If

                        If SoundsToUse.ContainsKey(LoadedSound.SourcePath) = False Then

                            'Storing the Nominal level into the sound file as loaded in the (68.34 dB SPL is the reference level used in SiP-test material)
                            SpeechMaterialComponent.SoundLibrary(LoadedSound.SourcePath).SMA.NominalLevel = 68.34 - DSP.Standard_dBFS_dBSPL_Difference
                            SpeechMaterialComponent.SoundLibrary(LoadedSound.SourcePath).SMA.InferNominalLevelToAllDescendants()

                            SoundsToUse.Add(LoadedSound.SourcePath, LoadedSound)
                        End If

                    Next

                    'Loading maskers sounds
                    For i = 0 To SubTrial.MediaSet.MaskerAudioItems - 1
                        Dim LoadedSound = SubTrial.SpeechMaterialComponent.GetMaskerSound(SubTrial.MediaSet, i)

                        If SoundsToUse.ContainsKey(LoadedSound.SourcePath) = False Then

                            'Setting all maskers to their ReferenceContrastingPhonemesLevel_SPL (converted to dB FS)

                            Dim CentralRegionLevel = LoadedSound.SMA.ChannelData(1)(0)(1).UnWeightedLevel
                            Dim TargetCentralRegionLevel = SubTrial.ReferenceContrastingPhonemesLevel_SPL - DSP.Standard_dBFS_dBSPL_Difference

                            Dim GainToTargetLevel = TargetCentralRegionLevel - CentralRegionLevel
                            DSP.AmplifySection(SpeechMaterialComponent.SoundLibrary(LoadedSound.SourcePath), GainToTargetLevel)

                            ''Shortering the maskers if TaskStandardTime is less than three seconds (standard masker length)
                            'Dim MaskerDuration As Double = LoadedSound.WaveData.SampleData(1).Length / LoadedSound.WaveFormat.SampleRate
                            'If TaskStandardTime < MaskerDuration Then

                            '    Dim ProportionToUse = TaskStandardTime / MaskerDuration
                            '    Dim CropLength As Integer = System.Math.Floor(ProportionToUse * LoadedSound.WaveData.SampleData(1).Length)
                            '    Dim CropStartSample As Integer = System.Math.Floor((LoadedSound.WaveData.SampleData(1).Length - CropLength) / 2)
                            '    Audio.DSP.CropSection(SpeechMaterialComponent.SoundLibrary(LoadedSound.SourcePath), CropStartSample, CropLength)

                            'End If

                            If SpeakerSetup = AdaptiveSiP_BILD_SpeakerSetups.Frontal_0_0_0 And UseBILDAttenuation = True Then

                                'Applying a filter that simulates bilaural benefit
                                Dim TempSound = SpeechMaterialComponent.SoundLibrary(LoadedSound.SourcePath)

                                'CurrentSound.WriteWaveFile("C:/FilterSound/Orig.wav")

                                'Creating the filter kernel
                                If BILD_FilterKernel Is Nothing Then
                                    Dim KernelFrequencyResponse As New List(Of Tuple(Of Single, Single))
                                    KernelFrequencyResponse.Add(New Tuple(Of Single, Single)(0, 0))
                                    KernelFrequencyResponse.Add(New Tuple(Of Single, Single)(50, -14.5))
                                    KernelFrequencyResponse.Add(New Tuple(Of Single, Single)(150, -14.5))
                                    KernelFrequencyResponse.Add(New Tuple(Of Single, Single)(250, -13.6))
                                    KernelFrequencyResponse.Add(New Tuple(Of Single, Single)(350, -12.4))
                                    KernelFrequencyResponse.Add(New Tuple(Of Single, Single)(450, -11.3))
                                    KernelFrequencyResponse.Add(New Tuple(Of Single, Single)(570, -10))
                                    KernelFrequencyResponse.Add(New Tuple(Of Single, Single)(700, -8.7))
                                    KernelFrequencyResponse.Add(New Tuple(Of Single, Single)(840, -7.4))
                                    KernelFrequencyResponse.Add(New Tuple(Of Single, Single)(1000, -6.1))
                                    KernelFrequencyResponse.Add(New Tuple(Of Single, Single)(1170, -5))
                                    KernelFrequencyResponse.Add(New Tuple(Of Single, Single)(1370, -3.8))
                                    KernelFrequencyResponse.Add(New Tuple(Of Single, Single)(1600, -2.8))
                                    KernelFrequencyResponse.Add(New Tuple(Of Single, Single)(1850, -1.9))
                                    KernelFrequencyResponse.Add(New Tuple(Of Single, Single)(2150, -1.1))
                                    KernelFrequencyResponse.Add(New Tuple(Of Single, Single)(2500, -0.5))
                                    KernelFrequencyResponse.Add(New Tuple(Of Single, Single)(2900, -0.2))
                                    KernelFrequencyResponse.Add(New Tuple(Of Single, Single)(3400, -0.1))
                                    KernelFrequencyResponse.Add(New Tuple(Of Single, Single)(4000, 0))


                                    BILD_FilterKernel = DSP.CreateCustumImpulseResponse(KernelFrequencyResponse, Nothing, TempSound.WaveFormat, New STFN.Core.Audio.Formats.FftFormat, 8000,, True, False)

                                End If

                                'BILD_FilterKernel.WriteWaveFile("C:/FilterSound/Kernel.wav")

                                TempSound = DSP.FIRFilter(TempSound, BILD_FilterKernel, BILD_FftFormat, ,,,, False, True, True)
                                TempSound.SourcePath = LoadedSound.SourcePath
                                TempSound.SMA = LoadedSound.SMA
                                TempSound.FileName = LoadedSound.FileName
                                TempSound.FFT = Nothing

                                'CurrentSound.WriteWaveFile("C:/FilterSound/Filter.wav")

                                'Replacing the sound (needed?)
                                SpeechMaterialComponent.SoundLibrary(TempSound.SourcePath) = TempSound

                                LoadedSound = TempSound

                            End If

                            'Storing the new CentralRegionLevel as the Nominal level
                            SpeechMaterialComponent.SoundLibrary(LoadedSound.SourcePath).SMA.NominalLevel = TargetCentralRegionLevel
                            SpeechMaterialComponent.SoundLibrary(LoadedSound.SourcePath).SMA.InferNominalLevelToAllDescendants()

                            'Check the level (it should agree with TargetCentralRegionLevel)
                            'SpeechMaterialComponent.SoundLibrary(LoadedSound.SourcePath).SMA.ChannelData(1)(0)(1).MeasureSoundLevels(False, 1, 0, 0)

                            SoundsToUse.Add(LoadedSound.SourcePath, LoadedSound)
                        End If
                    Next

                    'Loading the background sound
                    For i = 0 To 0 ' There's only one such sound available for he SiP-test (TODO: and also, their numbers aren't specified in the MediaSet class. Perhaps that should be changed in the future?)
                        Dim LoadedSound = SubTrial.SpeechMaterialComponent.GetBackgroundNonspeechSound(SubTrial.MediaSet, i)

                        If SoundsToUse.ContainsKey(LoadedSound.SourcePath) = False Then

                            'Measure the LoadedSound
                            Dim ActualLevel_dBFS = DSP.MeasureSectionLevel(LoadedSound, 1)

                            'Setting it to the intended BackgroundNonspeechRealisticLevel (with 2 sound sources, i.e. appr - 3 dB), so that it does not have to be modified when used later
                            Dim TargetLevel_dBFS = SubTrial.MediaSet.BackgroundNonspeechRealisticLevel - DSP.Standard_dBFS_dBSPL_Difference - 10 * System.Math.Log10(2)

                            Dim GainToTargetLevel = TargetLevel_dBFS - ActualLevel_dBFS
                            DSP.AmplifySection(SpeechMaterialComponent.SoundLibrary(LoadedSound.SourcePath), GainToTargetLevel)

                            'Storing the new TargetLevel_dBFS as the Nominal level
                            SpeechMaterialComponent.SoundLibrary(LoadedSound.SourcePath).SMA.NominalLevel = TargetLevel_dBFS
                            SpeechMaterialComponent.SoundLibrary(LoadedSound.SourcePath).SMA.InferNominalLevelToAllDescendants()

                            'Check the level (it should agree with TargetLevel_dBFS)
                            'Dim CheckLevel = Audio.DSP.MeasureSectionLevel(SpeechMaterialComponent.SoundLibrary(LoadedSound.SourcePath), 1)

                            'SubTrial.ReferenceContrastingPhonemesLevel_SPL = 

                            If SpeakerSetup = AdaptiveSiP_BILD_SpeakerSetups.Frontal_0_0_0 And UseBILDAttenuation = True Then

                                'Applying a filter that simulates bilaural benefit
                                Dim TempSound = SpeechMaterialComponent.SoundLibrary(LoadedSound.SourcePath)

                                TempSound = DSP.FIRFilter(TempSound, BILD_FilterKernel, BILD_FftFormat, ,,,, False, True, True)

                                TempSound.SourcePath = LoadedSound.SourcePath
                                TempSound.SMA = LoadedSound.SMA
                                TempSound.FileName = LoadedSound.FileName
                                TempSound.FFT = Nothing

                                'Replacing the sound (needed?)
                                SpeechMaterialComponent.SoundLibrary(TempSound.SourcePath) = TempSound

                                LoadedSound = TempSound

                            End If

                            SoundsToUse.Add(LoadedSound.SourcePath, LoadedSound)
                        End If
                    Next
                Next
            Next

            'Now all sounds needed in the test should have been loaded


        Catch ex As Exception
            Messager.MsgBox(ex.ToString)
        End Try


    End Sub

    Protected Overrides Sub InitiateTestByPlayingSound()

        'Sets the measurement datetime
        CurrentSipTestMeasurement.MeasurementDateTime = DateTime.Now

        'Cretaing a context sound without any test stimulus, that runs for approx TestSetup.PretestSoundDuration seconds, using audio from the first selected MediaSet
        Dim SelectedMediaSets As List(Of MediaSet) = AvailableMediasets

        Dim TestSound As STFN.Core.Audio.Sound = CreateInitialSound(SelectedMediaSets(0))

        'Plays sound
        SoundPlayer.SwapOutputSounds(TestSound)

        'And also premixing the first sounds in each SipTestUnit

        'For Each TestUnit In CurrentSipTestMeasurement.TestUnits

        '    'Calculating the speech level (of the first trial)
        '    Dim ProtocolReply = TestUnit.TestProtocol.NewResponse(New TrialHistory)

        '    'Getting the new adaptive value
        '    TestUnit.PlannedTrials(0).PNR = ProtocolReply.AdaptiveValue

        '    'Storing it also in AdaptiveProtocolValue, as this is used by the protocol
        '    TestUnit.PlannedTrials(0).AdaptiveProtocolValue = ProtocolReply.AdaptiveValue

        '    'Applying the levels
        '    TestUnit.PlannedTrials(0).SetLevels(ReferenceLevel, TestUnit.PlannedTrials(0).PNR)

        '    'Mixing the next sound in the unit
        '    TestUnit.PlannedTrials(0).MixSound(Transducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, CurrentSipTestMeasurement.Randomizer, TrialSoundMaxDuration, UseBackgroundSpeech)
        '    'TestUnit.PlannedTrials(0).PreMixTestTrialSoundOnNewTread(Transducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, CurrentSipTestMeasurement.Randomizer, TrialSoundMaxDuration, UseBackgroundSpeech)

        'Next

    End Sub


    Public Function CreateInitialSound(ByRef SelectedMediaSet As MediaSet, Optional ByVal Duration As Double? = Nothing) As STFN.Core.Audio.Sound

        Try

            'Setting up the SiP-trial sound mix
            Dim MixStopWatch As New Stopwatch
            MixStopWatch.Start()

            'Sets a List of SoundSceneItem in which to put the sounds to mix
            Dim ItemList = New List(Of SoundSceneItem)

            Dim SoundWaveFormat As STFN.Core.Audio.Formats.WaveFormat = Nothing

            'Getting a background non-speech sound
            Dim BackgroundNonSpeech_Sound As STFN.Core.Audio.Sound = SpeechMaterial.GetBackgroundNonspeechSound(SelectedMediaSet, 0)

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
            Dim Background1 = BackgroundNonSpeech_Sound.CopySection(1, CurrentSipTestMeasurement.Randomizer.Next(0, BackgroundNonSpeech_Sound.WaveData.SampleData(1).Length - TrialSoundLength - 2), TrialSoundLength)
            Dim Background2 = BackgroundNonSpeech_Sound.CopySection(1, CurrentSipTestMeasurement.Randomizer.Next(0, BackgroundNonSpeech_Sound.WaveData.SampleData(1).Length - TrialSoundLength - 2), TrialSoundLength)

            'Sets up fading specifications for the background signals
            Dim FadeSpecs_Background = New List(Of DSP.FadeSpecifications)
            FadeSpecs_Background.Add(New DSP.FadeSpecifications(Nothing, 0, 0, CurrentSampleRate * 1))
            FadeSpecs_Background.Add(New DSP.FadeSpecifications(0, Nothing, -CurrentSampleRate * 0.01))

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
            Dim MixedInitialSound As STFN.Core.Audio.Sound = Transducer.Mixer.CreateSoundScene(ItemList, False, False, SelectedSoundPropagationType, Transducer.LimiterThreshold)

            If LogToConsole = True Then Console.WriteLine("Mixed sound in " & MixStopWatch.ElapsedMilliseconds & " ms.")

            'TODO: Here we can simulate and/or compensate for hearing loss:
            'SimulateHearingLoss,
            'CompensateHearingLoss

            Return MixedInitialSound

        Catch ex As Exception
            STFN.Core.Utils.SendInfoToLog(ex.ToString, "ExceptionsDuringTesting")
            Return Nothing
        End Try

    End Function

    Private GivenResponses As New List(Of String)

    Public Overrides Function GetSpeechTestReply(sender As Object, e As SpeechTestInputEventArgs) As SpeechTestReplies

        Dim ProtocolReply As NextTaskInstruction = Nothing

        If e IsNot Nothing Then

            'This is an incoming test trial response. Adding the first response to GivenResponses
            GivenResponses = e.LinguisticResponses

            'Storing the lingustic responses
            CurrentTestTrial.Response = String.Join("-", GivenResponses)

            For i = 0 To CurrentTestTrial.SubTrials.Count - 1

                Dim ResponseSpelling As String = GivenResponses(i)
                'Correcting the response or randomizing a score if no response has been given

                If ResponseSpelling = "" Then
                    'Randomizing a score 
                    Dim PossibleScoresList As New List(Of Integer) From {1, 0, 0}
                    Dim RandomScore = PossibleScoresList(CurrentSipTestMeasurement.Randomizer.Next(0, PossibleScoresList.Count))
                    CurrentTestTrial.ScoreList.Add(RandomScore)

                    'Adding the score also to the subtrial
                    CurrentTestTrial.SubTrials(i).ScoreList.Add(RandomScore)

                Else

                    'Corrects the trial response, based on the given response
                    Dim CorrectSpelling As String = CurrentTestTrial.SubTrials(i).SpeechMaterialComponent.GetCategoricalVariableValue("Spelling")
                    If ResponseSpelling = CorrectSpelling Then
                        CurrentTestTrial.ScoreList.Add(1)

                        'Adding the score also to the subtrial
                        CurrentTestTrial.SubTrials(i).ScoreList.Add(1)

                    Else
                        CurrentTestTrial.ScoreList.Add(0)

                        'Adding the score also to the subtrial
                        CurrentTestTrial.SubTrials(i).ScoreList.Add(0)

                    End If
                End If
            Next

            'Clearing GivenResponses before next trial
            GivenResponses.Clear()

            'Moving test trial to trial history
            ObservedTrials.Add(CurrentTestTrial)
            PlannedTestTrials.RemoveAt(0)

            ProtocolReply = TestProtocol.NewResponse(ObservedTrials)

            'Overriding the adaptive level in the ProtocolReply by the start level if in practise test mode
            If IsPractiseTest = True Then
                ProtocolReply.AdaptiveValue = PractisePNR
                'And also overriding it in the protocol itself
                TestProtocol.OverrideCurrentAdaptiveValue(PractisePNR)
            End If

            ''Updating the speech level only after three trials (i.e. when all words in each TWG has been presented)
            'If ObservedTrials.Count Mod 3 = 0 Then

            '    'Calculating the speech level

            '    'Creating a New temporary TrialHistory in which the scores of the observed trials are concatenated in set of three trials so that the score lists of the temporary trials becoma 5*3, and the number of trials a third of the observed trials so far.
            '    'This means that the TestProtocol will evaluate the results of 15 instead of 5 responses, every third trial.
            '    Dim ObservedTrialsInSetsOfThree As New TrialHistory
            '    For i = 0 To ObservedTrials.Count - 1 Step 3

            '        Dim NewTrial As New TestTrial
            '        NewTrial.AdaptiveProtocolValue = ObservedTrials(i).AdaptiveProtocolValue ' Note that this value should be the same in all three trials (i + n = 0 To 2)
            '        For n = 0 To 2
            '            NewTrial.ScoreList.AddRange(ObservedTrials(i + n).ScoreList)
            '        Next
            '        ObservedTrialsInSetsOfThree.Add(NewTrial)

            '    Next

            '    ProtocolReply = TestProtocol.NewResponse(ObservedTrialsInSetsOfThree)

            'Else
            '    'Manually creating a new ProtocolReply, with its AdaptiveValue copied from the AdaptiveProtocolValue of the last test trial
            '    ProtocolReply = New NextTaskInstruction
            '    ProtocolReply.AdaptiveValue = ObservedTrials.Last.AdaptiveProtocolValue
            '    ProtocolReply.AdaptiveStepSize = 0
            '    ProtocolReply.Decision = SpeechTestReplies.GotoNextTrial

            'End If

            'Taking a dump of the SpeechTest before swapping to the new trial
            CurrentTestTrial.SpeechTestPropertyDump = STFN.Core.Utils.Logging.ListObjectPropertyValues(Me.GetType, Me)

        Else
            'Nothing to correct (this should be the start of a new test)

            'Calculating the speech level
            ProtocolReply = TestProtocol.NewResponse(ObservedTrials)

            'And mixing and playing initial sound, and the first sounds in each test unit
            InitiateTestByPlayingSound()

        End If

        'Exiting the test if the PNR value goes above a specified limit
        If ProtocolReply.AdaptiveValue > 40 Then

            'At PNR 40, there is basically no noise, only signal at max level. No point in continueing 
            'Overriding the protocol decition
            ProtocolReply.Decision = SpeechTestReplies.AbortTest

            Select Case GuiLanguage
                Case Languages.Swedish
                    AbortInformation = "Testet avbröts tidigare eftersom PNR översteg 40 dB"
                Case Languages.English
                    AbortInformation = "The test was aborted early because PNR exceeded 40 dB"
            End Select

        End If

        If PlannedTestTrials.Count = 0 Then
            'Test is completed
            Return SpeechTestReplies.TestIsCompleted
        End If

        'Preparing next trial if needed
        If ProtocolReply.Decision = SpeechTestReplies.GotoNextTrial Then
            PrepareNextTrial(ProtocolReply)
        End If

        Return ProtocolReply.Decision

    End Function


    Private Sub WaitForTestTrialSound()

        Try

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


    Protected Overrides Sub PrepareNextTrial(ByVal NextTaskInstruction As TestProtocol.NextTaskInstruction)

        'N.B. NextTaskInstruction is not used here

        'Preparing the next trial
        CurrentTestTrial = PlannedTestTrials(0)
        CurrentTestTrial.Tasks = 5
        CurrentTestTrial.ResponseAlternativeSpellings = New List(Of List(Of SpeechTestResponseAlternative))

        For Each SubTrial In CurrentTestTrial.SubTrials

            Dim ResponseAlternatives As New List(Of SpeechTestResponseAlternative)

            'Adding all list members as response alternatives
            Dim AllListWords = SubTrial.SpeechMaterialComponent.GetSiblings()
            For Each ListMembers In AllListWords
                ResponseAlternatives.Add(New SpeechTestResponseAlternative With {.Spelling = ListMembers.GetCategoricalVariableValue("Spelling"), .IsScoredItem = ListMembers.IsKeyComponent, .ParentTestTrial = CurrentTestTrial})
            Next

            'Shuffling the order of response alternatives
            ResponseAlternatives = DSP.Shuffle(ResponseAlternatives, CurrentSipTestMeasurement.Randomizer).ToList

            'Adding the response alternatives
            CurrentTestTrial.ResponseAlternativeSpellings.Add(ResponseAlternatives)

        Next


        'Getting the new adaptive value
        'Storing values into the TestTrial
        CurrentTestTrial.AdaptiveProtocolValue = NextTaskInstruction.AdaptiveValue
        'DirectCast(CurrentTestTrial, SipTrial).PNR = NextTaskInstruction.AdaptiveValue
        'DirectCast(CurrentTestTrial, SipTrial).AdaptiveProtocolValue = NextTaskInstruction.AdaptiveValue

        'Storing also in all subtrials, and mixes their sounds
        Dim TrialSounds As New List(Of STFN.Core.Audio.Sound)
        Dim TaskStartTimes As New List(Of Double)
        Dim OverlapTime As Double = ((TaskStandardTime - 1) / 2)

        For i = 0 To CurrentTestTrial.SubTrials.Count - 1

            Dim SubTrial = CurrentTestTrial.SubTrials(i)

            DirectCast(SubTrial, SipTrial).PNR = NextTaskInstruction.AdaptiveValue

            'Storing it also in AdaptiveProtocolValue, as this is used by the protocol
            DirectCast(SubTrial, SipTrial).AdaptiveProtocolValue = NextTaskInstruction.AdaptiveValue

            'Applying the levels
            DirectCast(SubTrial, SipTrial).SetLevels(ReferenceLevel, NextTaskInstruction.AdaptiveValue)

            'Mixing the next sound in the unit

            If i = 0 Then
                DirectCast(SubTrial, SipTrial).MixSound(Transducer, MinimumStimulusOnsetTime, MinimumStimulusOnsetTime, CurrentSipTestMeasurement.Randomizer, MinimumStimulusOnsetTime + TaskStandardTime, UseBackgroundSpeech, ,, False, True, True) ' Using only MinimumStimulusOnsetTime here
            ElseIf i = CurrentTestTrial.SubTrials.Count - 1 Then
                DirectCast(SubTrial, SipTrial).MixSound(Transducer, 0, 0, CurrentSipTestMeasurement.Randomizer, TrialSoundMaxDuration, UseBackgroundSpeech, ,, True, False, True) ' TrialSoundMaxDuration should be shorter!
            Else
                DirectCast(SubTrial, SipTrial).MixSound(Transducer, 0, 0, CurrentSipTestMeasurement.Randomizer, TaskStandardTime, UseBackgroundSpeech, ,, True, True, True)
            End If

            'Exports sound file
            If CurrentSipTestMeasurement.ExportTrialSoundFiles = True Then DirectCast(SubTrial, SipTrial).Sound.WriteWaveFile(IO.Path.Combine(STFN.Core.Utils.logFilePath, "AdaptiveSipSounds", "AdaptiveSipSounds_" & i))

            'Storing the test word start times
            If i = 0 Then
                TaskStartTimes.Add(DirectCast(SubTrial, SipTrial).TestWordStartTime)
            Else
                TaskStartTimes.Add(MinimumStimulusOnsetTime + i * (TaskStandardTime - OverlapTime) + DirectCast(SubTrial, SipTrial).TestWordStartTime)
            End If

            'Adds the sound
            TrialSounds.Add(DirectCast(SubTrial, SipTrial).Sound)

        Next

        'Mix full trial sound
        CurrentTestTrial.Sound = DSP.ConcatenateSounds2(TrialSounds, TrialSounds(0).WaveFormat.SampleRate * OverlapTime)

        'Exports sound file
        If CurrentSipTestMeasurement.ExportTrialSoundFiles = True Then CurrentTestTrial.Sound.WriteWaveFile(IO.Path.Combine(STFN.Core.Utils.logFilePath, "AdaptiveSipSounds", "AdaptiveSipSounds_Mix"))

        'Waiting for the trial sound to be mixed, if not yet completed
        'WaitForTestTrialSound()

        'Storing the LinguisticSoundStimulusStartTime and the LinguisticSoundStimulusDuration 
        CurrentTestTrial.LinguisticSoundStimulusStartTime = TaskStartTimes.First
        CurrentTestTrial.LinguisticSoundStimulusDuration = TaskStartTimes.Last + DirectCast(CurrentTestTrial.SubTrials.Last, SipTrial).TestWordCompletedTime - TaskStartTimes.First
        CurrentTestTrial.MaximumResponseTime = MaximumResponseTime

        'Setting visual que intervals
        Dim ShowResponseAlternativePositions_Interval As Integer = ShowResponseAlternativePositionsTime * 1000
        Dim ShowResponseAlternativesTimer_Interval As Double = ShowResponseAlternativePositions_Interval + 1000 * ResponseAlternativeDelay
        Dim MaxResponseTimeTimer_Interval As Double = System.Math.Max(1, TaskStartTimes.Last + DirectCast(CurrentTestTrial.SubTrials.Last, SipTrial).TestWordCompletedTime + MaximumResponseTime) * 1000


        'Setting trial events
        CurrentTestTrial.TrialEventList = New List(Of ResponseViewEvent)
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 1, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})

        If ShowTestSide = True Then
            CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = ShowResponseAlternativePositions_Interval, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternativePositions})
        End If

        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = ShowResponseAlternativesTimer_Interval, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})

        For Each Value In TaskStartTimes
            CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = Value * 1000, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowVisualCue})
        Next

        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = MaxResponseTimeTimer_Interval, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut})

    End Sub



    Public Overrides Function GetResultStringForGui() As String

        Dim FinalResult = TestProtocol.GetFinalResultValue

        Dim TestResultSummaryLines = New List(Of String)
        If IsPractiseTest = True Then
            'Reporting number of correct words
            Dim PractiseTrialScoreList As New List(Of Integer)
            For Each Trial In ObservedTrials
                PractiseTrialScoreList.AddRange(Trial.ScoreList)
            Next

            If PractiseTrialScoreList.Count = 0 Then
                TestResultSummaryLines.Add("Inga övningsresultat finns. Blev testet avbrutet?")
            Else
                TestResultSummaryLines.Add("Resultat från övningstestet: " & PractiseTrialScoreList.Sum & " av " & PractiseTrialScoreList.Count & " (" & System.Math.Round(100 * (PractiseTrialScoreList.Sum / PractiseTrialScoreList.Count)) & " %) ord var korrekta.")
            End If

        Else
            If FinalResult.HasValue Then
                TestResultSummaryLines.Add("Resultat: " & vbTab & DSP.Rounding(FinalResult.Value, 1) & " dB PNR")
            Else
                TestResultSummaryLines.Add("Resultat: Slutgiltigt tröskelvärde kunde ej bestämmas.")
            End If
        End If

        Return String.Join(vbCrLf, TestResultSummaryLines)

    End Function

    Public Overrides Function GetSubGroupResults() As List(Of Tuple(Of String, Double))

        Dim SubScoreList As New SortedList(Of String, List(Of Integer))

        Dim ObservedTrials = GetObservedTestTrials().ToList
        If ObservedTrials.Count = 0 Then Return Nothing

        Dim TenLastTrials As New List(Of TestTrial)

        'Getting the ten last test trials, or all if ObservedTrials is shorter than ten
        If ObservedTrials.Count < 11 Then
            TenLastTrials.AddRange(ObservedTrials)
        Else
            TenLastTrials.AddRange(ObservedTrials.GetRange(ObservedTrials.Count - 10, 10))
        End If

        Dim OverallList As New List(Of Integer)

        For Each Trial In TenLastTrials

            For Each SubTrial In Trial.SubTrials

                'Getting the sub score name 
                Dim SubGroupName = SubTrial.SpeechMaterialComponent.ParentComponent.PrimaryStringRepresentation.Replace("_", ", ")

                'Adding the sub group is not already done
                If SubScoreList.ContainsKey(SubGroupName) = False Then SubScoreList.Add(SubGroupName, New List(Of Integer))

                'Adding the score to the sub-score list
                SubScoreList(SubGroupName).AddRange(SubTrial.ScoreList)

                'Adding the score to the overall score list
                OverallList.AddRange(SubTrial.ScoreList)

            Next

        Next

        Dim Output As New List(Of Tuple(Of String, Double))
        For Each kvp In SubScoreList
            Output.Add(New Tuple(Of String, Double)(kvp.Key, kvp.Value.Average))
        Next

        'Adding the overall list
        Dim OverallGroupName As String = ""
        Select Case GuiLanguage
            Case Languages.Swedish
                OverallGroupName = "Totalt"
            Case Languages.English
                OverallGroupName = "In total"
            Case Else
                OverallGroupName = "In total"
        End Select

        ' Adding also the overall value
        Output.Add(New Tuple(Of String, Double)(OverallGroupName, OverallList.Average))

        Return Output

    End Function


    ''' <summary>
    ''' This function should list the names of variables included SpeechTestDump of each test trial to be exported in the "selected-variables" export file.
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function GetSelectedExportVariables() As List(Of String)
        Return New List(Of String)
    End Function

    Public Overrides Function GetProgress() As ProgressInfo

        Dim NewProgressInfo As New ProgressInfo
        NewProgressInfo.Value = GetObservedTestTrials.Count + 1 ' Adds one to show started instead of completed trials.
        NewProgressInfo.Maximum = GetTotalTrialCount()
        Return NewProgressInfo

    End Function

    Public Overrides Function GetTotalTrialCount() As Integer
        Return TrialCount
    End Function

    Public Overrides Function GetTestCompletedGuiMessage() As String

        Select Case STFN.Core.SharedSpeechTestObjects.GuiLanguage
            Case STFN.Core.Utils.Constants.Languages.Swedish
                If IsPractiseTest = True Then
                    Return "Övningstestet är klart!"
                Else
                    Return "Testet är klart!"
                End If
            Case Else
                If IsPractiseTest = True Then
                    Return "The practise test is finished!"
                Else
                    Return "The test is finished!"
                End If
        End Select

    End Function

    ''' <summary>
    ''' This method returns a calibration sound mixed to the currently set reference level, presented from the first indicated target sound source.
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function CreateCalibrationCheckSignal() As Tuple(Of STFN.Core.Audio.Sound, String)

        'Referencing the/a mediaset in SpeechTest.MediaSet, which is normally not used in the SiP-test but needed for the calibration signal
        'TODO: This function needs to read the selected SpeechTest.MediaSet. As of now, it just takes the first available MediaSet. The SiP-test does not yet read media set and stuff off the options control, but each test sets up it's own trials (including MediaSet selection in code.
        MediaSet = SpeechMaterial.ParentTestSpecification.MediaSets(0)

        'Overriding any value for LevelsAreIn_dBHL. This should always be False for the SiP-test (it's hard coded that way in the SipTrial.MixSound method.
        LevelsAreIn_dBHL = False

        'Note that the adaptive SiP uses nominal level to generate the mix, and need to override the CreateCalibrationCheckSignal method in SipBaseSpeechTest
        Return MixStandardCalibrationSound(True, CalibrationCheckLevelTypes.ReferenceLevel)

    End Function

    Public Overrides Function GetScorePerLevel() As Tuple(Of String, SortedList(Of Double, Double))
        Return Nothing
    End Function
End Class