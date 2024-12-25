Imports STFN.SipTest
Imports STFN.Audio.SoundScene
Imports STFN.Utils

Public Class QuickSiP
    Inherits SipBaseSpeechTest

    Public Overrides ReadOnly Property FilePathRepresentation As String = "QuickSiP"

    Public Sub New(ByVal SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)
        ApplyTestSpecificSettings()
    End Sub

    Public Shadows Sub ApplyTestSpecificSettings()

        TesterInstructions = "(Detta test går ut på att undersöka en screening-version av SiP-testet.)" & vbCrLf & vbCrLf &
             "För detta test behövs inga inställningar." & vbCrLf & vbCrLf &
             "1. Informera patienten om hur testet går till." & vbCrLf &
             "2. Vänd skärmen till patienten. Be sedan patienten klicka på start för att starta testet."

        ParticipantInstructions = "Patientens uppgift: " & vbCrLf & vbCrLf &
             " - Patienten startar testet genom att klicka på knappen 'Start'" & vbCrLf &
             " - Under testet ska patienten lyssna efter enstaviga ord som uttalas i en stadsmiljö och efter varje ord ange på skärmen vilket ord hen uppfattade. " & vbCrLf &
             " - Patienten ska gissa om hen är osäker." & vbCrLf &
             " - Efter varje ord har patienten maximalt " & MaximumResponseTime & " sekunder på sig att ange sitt svar." & vbCrLf &
             " - Om svarsalternativen blinkar i röd färg har patienten inte svarat i tid." & vbCrLf &
             " - Testet består av totalt 30 ord, som blir svårare och svårare ju längre testet går."

        'ShowGuiChoice_SoundFieldSimulation = True

        DirectionalSimulationSet = "ARC - Harcellen - HATS - SiP"
        PopulateSoundSourceLocationCandidates()
        SimulatedSoundField = True

        ShowGuiChoice_TargetLocations = False
        ShowGuiChoice_MaskerLocations = False
        ShowGuiChoice_BackgroundNonSpeechLocations = False
        ShowGuiChoice_BackgroundSpeechLocations = False

        ' DirectionalSimulationSet = "ARC - Harcellen - HATS 256 - 48kHz"

    End Sub


    Private PresetName As String = "QuickSiP"

    Private ResultsSummary As SortedList(Of Double, Tuple(Of QuickSipList, Double))

    Public Overrides Function InitializeCurrentTest() As Tuple(Of Boolean, String)

        Transducer = AvaliableTransducers(0)

        CurrentSipTestMeasurement = New SipMeasurement(CurrentParticipantID, SpeechMaterial.ParentTestSpecification, AdaptiveTypes.Fixed, SelectedTestparadigm)

        CurrentSipTestMeasurement.ExportTrialSoundFiles = False

        If SimulatedSoundField = True Then
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

        'Setting up test trials to run
        PlanQuickSiPTrials(SelectedSoundPropagationType, RandomSeed)

        If CurrentSipTestMeasurement.HasSimulatedSoundFieldTrials = True And DirectionalSimulator.SelectedDirectionalSimulationSetName = "" Then
            Return New Tuple(Of Boolean, String)(False, "The measurement requires a directional simulation set to be selected!")
        End If

        Return New Tuple(Of Boolean, String)(True, "")

    End Function

    Dim SipTestLists As New List(Of QuickSipList)

    Private Class QuickSipList
        Public SMC As SpeechMaterialComponent
        Public MediaSet As MediaSet
        Public PNR As Double
    End Class

    Private Function GetPnrScores() As SortedList(Of Double, Tuple(Of QuickSipList, Double))

        Dim ResultList As New List(Of Tuple(Of QuickSipList, Double)) ' QuickSipList, MeanScore

        For i = 0 To SipTestLists.Count - 1

            Dim CurrentSipTestList = SipTestLists(i)
            Dim CurrentScoresList As New List(Of Double)

            For Each Trial In CurrentSipTestMeasurement.ObservedTrials
                If Trial.MediaSet Is CurrentSipTestList.MediaSet And
                        Trial.PNR = CurrentSipTestList.PNR And
                        Trial.SpeechMaterialComponent.ParentComponent Is CurrentSipTestList.SMC Then

                    If Trial.IsCorrect = True Then
                        CurrentScoresList.Add(1)
                    Else
                        CurrentScoresList.Add(0)
                    End If

                End If
            Next

            Dim AverageScore As Double = -1
            If CurrentScoresList.Count > 0 Then
                AverageScore = CurrentScoresList.Average
            End If

            ResultList.Add(New Tuple(Of QuickSipList, Double)(CurrentSipTestList, AverageScore))
        Next

        Dim PnrSortedList As New SortedList(Of Double, Tuple(Of QuickSipList, Double))
        For Each Result In ResultList
            PnrSortedList.Add(Result.Item1.PNR, Result)
        Next

        Return PnrSortedList

    End Function


    Private Sub PlanQuickSiPTrials(ByVal SoundPropagationType As SoundPropagationTypes, Optional ByVal RandomSeed As Integer? = Nothing)

        Dim AllMediaSets As List(Of MediaSet) = AvailableMediasets

        Dim SelectedMediaSets As New List(Of MediaSet)
        Dim IncludedMediaSetNames As New List(Of String) From {"City-Talker1-RVE", "City-Talker2-RVE"}
        For Each AvailableMediaSet In AllMediaSets
            If IncludedMediaSetNames.Contains(AvailableMediaSet.MediaSetName) Then
                SelectedMediaSets.Add(AvailableMediaSet)
            End If
        Next


        'Creating a new random if seed is supplied
        If RandomSeed.HasValue Then CurrentSipTestMeasurement.Randomizer = New Random(RandomSeed)

        'Getting the preset
        Dim Preset = CurrentSipTestMeasurement.ParentTestSpecification.SpeechMaterial.Presets.GetPretest(PresetName).Members

        'Ordering presets as intended
        'mark_märk_mörk, fyr_skyr_syr, sitt_sytt_sött, kil_fil_sil
        'Dim IntendedPresetOrder As New List(Of String) From {"mark_märk_mörk", "fyr_skyr_syr", "sitt_sytt_sött", "kil_fil_sil"}
        Dim IntendedPresetOrder As New List(Of String) From {"fyr_skyr_syr", "mark_märk_mörk", "kil_fil_sil", "tuff_tuss_tusch", "sitt_sytt_sött"}
        Dim TempPresets As New List(Of SpeechMaterialComponent)
        For i = 0 To 4
            For j = 0 To Preset.Count - 1
                If Preset(j).PrimaryStringRepresentation = IntendedPresetOrder(i) Then
                    TempPresets.Add(Preset(j))
                    Exit For
                End If
            Next
        Next

        Preset = TempPresets

        'Getting the sound source locations
        'Head slightly turned right (i.e. Speech on left side)
        Dim TargetStimulusLocations_HeadTurnedRight As SoundSourceLocation()
        Dim MaskerLocations_HeadTurnedRight As SoundSourceLocation()
        Dim BackgroundLocations_HeadTurnedRight As SoundSourceLocation()

        'Head slightly turned left (i.e. Speech on right side)
        Dim TargetStimulusLocations_HeadTurnedLeft As SoundSourceLocation()
        Dim MaskerLocations_HeadTurnedLeft As SoundSourceLocation()
        Dim BackgroundLocations_HeadTurnedLeft As SoundSourceLocation()

        If SimulatedSoundField = True Then

            'Head slightly turned right (i.e. Speech on left side)
            TargetStimulusLocations_HeadTurnedRight = {New SoundSourceLocation With {.HorizontalAzimuth = -10, .Distance = 1.45}}
            MaskerLocations_HeadTurnedRight = {New SoundSourceLocation With {.HorizontalAzimuth = 170, .Distance = 1.45}}
            BackgroundLocations_HeadTurnedRight = {New SoundSourceLocation With {.HorizontalAzimuth = -10, .Distance = 1.45}, New SoundSourceLocation With {.HorizontalAzimuth = 170, .Distance = 1.45}}

            'Head slightly turned left (i.e. Speech on right side)
            TargetStimulusLocations_HeadTurnedLeft = {New SoundSourceLocation With {.HorizontalAzimuth = 10, .Distance = 1.45}}
            MaskerLocations_HeadTurnedLeft = {New SoundSourceLocation With {.HorizontalAzimuth = 190, .Distance = 1.45}}
            BackgroundLocations_HeadTurnedLeft = {New SoundSourceLocation With {.HorizontalAzimuth = 10, .Distance = 1.45}, New SoundSourceLocation With {.HorizontalAzimuth = 190, .Distance = 1.45}}

        Else

            'Not using head turn simulation in sound field presentation, but instead a S0N180 situation
            'Still using the same object names (these names should be changed in future verisons)
            TargetStimulusLocations_HeadTurnedRight = {New SoundSourceLocation With {.HorizontalAzimuth = 0, .Distance = 1.45}}
            MaskerLocations_HeadTurnedRight = {New SoundSourceLocation With {.HorizontalAzimuth = 180, .Distance = 1.45}}
            BackgroundLocations_HeadTurnedRight = {New SoundSourceLocation With {.HorizontalAzimuth = 0, .Distance = 1.45}, New SoundSourceLocation With {.HorizontalAzimuth = 180, .Distance = 1.45}}

            'Head slightly turned left (i.e. Speech on right side)
            TargetStimulusLocations_HeadTurnedLeft = {New SoundSourceLocation With {.HorizontalAzimuth = 0, .Distance = 1.45}}
            MaskerLocations_HeadTurnedLeft = {New SoundSourceLocation With {.HorizontalAzimuth = 180, .Distance = 1.45}}
            BackgroundLocations_HeadTurnedLeft = {New SoundSourceLocation With {.HorizontalAzimuth = 0, .Distance = 1.45}, New SoundSourceLocation With {.HorizontalAzimuth = 180, .Distance = 1.45}}

        End If


        'Clearing any trials that may have been planned by a previous call
        CurrentSipTestMeasurement.ClearTrials()

        'Talker in front
        Dim PNRs As New List(Of Double)
        Dim TempPnr As Double = 15
        For i = 0 To 9
            PNRs.Add(TempPnr)
            TempPnr -= 3.5
        Next

        SipTestLists.Add(New QuickSipList With {.SMC = Preset(0), .MediaSet = SelectedMediaSets(1), .PNR = PNRs(0)})
        SipTestLists.Add(New QuickSipList With {.SMC = Preset(1), .MediaSet = SelectedMediaSets(0), .PNR = PNRs(1)})

        SipTestLists.Add(New QuickSipList With {.SMC = Preset(2), .MediaSet = SelectedMediaSets(1), .PNR = PNRs(2)})
        SipTestLists.Add(New QuickSipList With {.SMC = Preset(3), .MediaSet = SelectedMediaSets(0), .PNR = PNRs(3)})

        SipTestLists.Add(New QuickSipList With {.SMC = Preset(4), .MediaSet = SelectedMediaSets(1), .PNR = PNRs(4)})
        SipTestLists.Add(New QuickSipList With {.SMC = Preset(0), .MediaSet = SelectedMediaSets(0), .PNR = PNRs(5)})

        SipTestLists.Add(New QuickSipList With {.SMC = Preset(1), .MediaSet = SelectedMediaSets(1), .PNR = PNRs(6)})
        SipTestLists.Add(New QuickSipList With {.SMC = Preset(2), .MediaSet = SelectedMediaSets(0), .PNR = PNRs(7)})

        SipTestLists.Add(New QuickSipList With {.SMC = Preset(3), .MediaSet = SelectedMediaSets(1), .PNR = PNRs(8)})
        SipTestLists.Add(New QuickSipList With {.SMC = Preset(4), .MediaSet = SelectedMediaSets(0), .PNR = PNRs(9)})


        'A list that determines which SipTestLists that will be randomized together
        Dim NewTestUnitIndices As New List(Of Integer) From {0, 2, 4, 6, 8}

        Dim CurrentTestUnit As SiPTestUnit = Nothing
        Dim TrialsAdded As Integer = 0

        For i = 0 To SipTestLists.Count - 1

            If NewTestUnitIndices.Contains(i) Then
                If CurrentTestUnit IsNot Nothing Then
                    CurrentSipTestMeasurement.TestUnits.Add(CurrentTestUnit)
                End If
                CurrentTestUnit = New SiPTestUnit(CurrentSipTestMeasurement)
            End If

            Dim PresetComponent = SipTestLists(i).SMC
            Dim MediaSet = SipTestLists(i).MediaSet
            Dim PNR = SipTestLists(i).PNR

            Dim TestWords = PresetComponent.GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.Sentence)
            CurrentTestUnit.SpeechMaterialComponents.AddRange(TestWords)

            For c = 0 To TestWords.Count - 1
                Dim NewTrial As SipTrial = Nothing

                'Adding left and right head turns systematically to every other trial
                If TrialsAdded Mod 2 = 0 Then
                    NewTrial = New SipTrial(CurrentTestUnit, TestWords(c), MediaSet, SoundPropagationType, TargetStimulusLocations_HeadTurnedRight.ToArray, MaskerLocations_HeadTurnedRight.ToArray, BackgroundLocations_HeadTurnedRight, CurrentTestUnit.ParentMeasurement.Randomizer)
                Else
                    NewTrial = New SipTrial(CurrentTestUnit, TestWords(c), MediaSet, SoundPropagationType, TargetStimulusLocations_HeadTurnedLeft.ToArray, MaskerLocations_HeadTurnedLeft.ToArray, BackgroundLocations_HeadTurnedLeft, CurrentTestUnit.ParentMeasurement.Randomizer)
                End If

                NewTrial.SetLevels(ReferenceLevel, PNR)
                CurrentTestUnit.PlannedTrials.Add(NewTrial)

                TrialsAdded += 1
            Next
        Next

        'Adds the last unit
        CurrentSipTestMeasurement.TestUnits.Add(CurrentTestUnit)

        'Randomizing the order within units
        For ui = 0 To CurrentSipTestMeasurement.TestUnits.Count - 1
            Dim Unit As SiPTestUnit = CurrentSipTestMeasurement.TestUnits(ui)
            Dim RandomList As New List(Of SipTrial)
            Do Until Unit.PlannedTrials.Count = 0
                Dim RandomIndex As Integer = CurrentSipTestMeasurement.Randomizer.Next(0, Unit.PlannedTrials.Count)
                RandomList.Add(Unit.PlannedTrials(RandomIndex))
                Unit.PlannedTrials.RemoveAt(RandomIndex)
            Loop
            Unit.PlannedTrials = RandomList
        Next

        'Adding the trials CurrentSipTestMeasurement (from which they can be drawn during testing)
        For ui = 0 To CurrentSipTestMeasurement.TestUnits.Count - 1
            Dim Unit As SiPTestUnit = CurrentSipTestMeasurement.TestUnits(ui)
            For Each Trial In Unit.PlannedTrials
                CurrentSipTestMeasurement.PlannedTrials.Add(Trial)
            Next
        Next

    End Sub

    Protected Overrides Sub InitiateTestByPlayingSound()

        'Sets the measurement datetime
        CurrentSipTestMeasurement.MeasurementDateTime = DateTime.Now

        'Cretaing a context sound without any test stimulus, that runs for approx TestSetup.PretestSoundDuration seconds, using audio from the first selected MediaSet
        Dim SelectedMediaSets As List(Of MediaSet) = AvailableMediasets

        Dim TestSound As Audio.Sound = CreateInitialSound(SelectedMediaSets(0))

        'Plays sound
        SoundPlayer.SwapOutputSounds(TestSound)

        'Premixing the first 10 sounds 
        CurrentSipTestMeasurement.PreMixTestTrialSoundsOnNewTread(Transducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, Randomizer, TrialSoundMaxDuration, UseBackgroundSpeech, 10)

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
            Dim MixedInitialSound As Audio.Sound = Transducer.Mixer.CreateSoundScene(ItemList, False, False, SelectedSoundPropagationType)

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
                CurrentSipTestMeasurement.PreMixTestTrialSoundsOnNewTread(Transducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, Randomizer, TrialSoundMaxDuration, UseBackgroundSpeech, 10)
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


    Protected Overrides Sub PrepareNextTrial(ByVal NextTaskInstruction As TestProtocol.NextTaskInstruction)

        'Preparing the next trial
        CurrentTestTrial = CurrentSipTestMeasurement.GetNextTrial()
        CurrentTestTrial.TestStage = NextTaskInstruction.TestStage
        CurrentTestTrial.Tasks = 1
        CurrentTestTrial.ResponseAlternativeSpellings = New List(Of List(Of SpeechTestResponseAlternative))
        Dim ResponseAlternatives As New List(Of SpeechTestResponseAlternative)

        'Adding the current word spelling as a response alternative
        ResponseAlternatives.Add(New SpeechTestResponseAlternative With {.Spelling = CurrentTestTrial.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling"), .IsScoredItem = CurrentTestTrial.SpeechMaterialComponent.IsKeyComponent})

        'Picking random response alternatives from all available test words
        Dim AllContrastingWords = CurrentTestTrial.SpeechMaterialComponent.GetSiblingsExcludingSelf()
        For Each ContrastingWord In AllContrastingWords
            ResponseAlternatives.Add(New SpeechTestResponseAlternative With {.Spelling = ContrastingWord.GetCategoricalVariableValue("Spelling"), .IsScoredItem = ContrastingWord.IsKeyComponent})
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
        If UseVisualQue = True Then
            CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = ShowVisualQueTimer_Interval, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowVisualCue})
            CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = HideVisualQueTimer_Interval, .Type = ResponseViewEvent.ResponseViewEventTypes.HideVisualCue})
        End If
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = ShowResponseAlternativesTimer_Interval, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = MaxResponseTimeTimer_Interval, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut})

    End Sub


    Private Function GetAverageQuickSipHeadTurnScores(Optional ByVal TurnedRight As Boolean? = Nothing)

        Dim TrialScoreList As New List(Of Integer)
        For Each TestUnit In CurrentSipTestMeasurement.TestUnits
            For Each Trial In TestUnit.ObservedTrials

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

        Dim Output As New List(Of String)

        Output.Add("Overall score: " & Math.Rounding(100 * GetAverageQuickSipHeadTurnScores(Nothing)) & " %")
        'Output.Add("Head turned left: " & Math.Rounding(100 * GetAverageQuickSipHeadTurnScores(False)) & " %")
        'Output.Add("Head turned right : " & Math.Rounding(100 * GetAverageQuickSipHeadTurnScores(True)) & " %")

        ResultsSummary = GetPnrScores()

        If ResultsSummary IsNot Nothing Then
            Output.Add("Scores per PNR level:")
            Output.Add("PNR (dB)" & vbTab & "Score" & vbTab & "List")
            For Each kvp In ResultsSummary
                Output.Add(kvp.Value.Item1.PNR & vbTab & Math.Rounding(100 * kvp.Value.Item2) & " %" & vbTab & kvp.Value.Item1.SMC.PrimaryStringRepresentation)
            Next
        End If

        Return String.Join(vbCrLf, Output)

    End Function

    ''' <summary>
    ''' This function should list the names of variables included SpeechTestDump of each test trial to be exported in the "selected-variables" export file.
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function GetSelectedExportVariables() As List(Of String)
        Return New List(Of String)
    End Function


End Class