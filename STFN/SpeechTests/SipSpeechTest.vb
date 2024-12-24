Imports STFN.SipTest
Imports STFN.Audio.SoundScene
Imports STFN.Utils

Public Class SipSpeechTest
    Inherits SipBaseSpeechTest

    Public Overrides ReadOnly Property FilePathRepresentation As String = "SiP"

    Public Sub New(ByVal SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)
        ApplyTestSpecificSettings()
    End Sub

    Public Shadows Sub ApplyTestSpecificSettings()

        TesterInstructions = ""
        ParticipantInstructions = ""
        ShowGuiChoice_PreSet = True
        ShowGuiChoice_SoundFieldSimulation = True
        AvailablePhaseAudiometryTypes = New List(Of BmldModes) From {BmldModes.RightOnly, BmldModes.LeftOnly, BmldModes.BinauralSamePhase, BmldModes.BinauralPhaseInverted, BmldModes.BinauralUncorrelated}
        MaximumSoundFieldSpeechLocations = 1
        MaximumSoundFieldMaskerLocations = 12
        MaximumSoundFieldBackgroundNonSpeechLocations = 12
        MaximumSoundFieldBackgroundSpeechLocations = 12
        MinimumSoundFieldSpeechLocations = 1
        MinimumSoundFieldMaskerLocations = 1
        MinimumSoundFieldBackgroundNonSpeechLocations = 2
        MinimumSoundFieldBackgroundSpeechLocations = 0
        ShowGuiChoice_ReferenceLevel = True
        'ShowGuiChoice_PhaseAudiometry = True
        'SupportsManualPausing = False

        SelectedTestparadigm = Testparadigm.Slow

        SipTestMode = SiPTestModes.Directional

    End Sub


    'TODO: We need to be able to input PNR in the GUI! Something like this is needed:
    'Public Overrides ReadOnly Property ShowGuiChoice_SNR As Boolean = True


    Private NumberOfSimultaneousMaskers As Integer = 1
    Private SelectedPNRs As New List(Of Double)
    Private TestIsStarted As Boolean = False
    Private SipMeasurementRandomizer As Random
    Private TestIsPaused As Boolean = False


    Public Overrides Function InitializeCurrentTest() As Tuple(Of Boolean, String)

        'Creates a new randomizer before each test start
        Dim Seed As Integer? = Nothing
        If Seed.HasValue Then
            SipMeasurementRandomizer = New Random(Seed)
        Else
            SipMeasurementRandomizer = New Random
        End If

        Transducer = AvaliableTransducers(0)

        If SignalLocations.Count = 0 Then
            Return New Tuple(Of Boolean, String)(False, "You must select at least one signal sound source!")
        End If

        If MaskerLocations.Count = 0 Then
            Return New Tuple(Of Boolean, String)(False, "You must select at least one masker sound source location!")
        End If

        If BackgroundNonSpeechLocations.Count = 0 Then
            Return New Tuple(Of Boolean, String)(False, "You must select at least one background sound source location!")
        End If

        If BackgroundSpeechLocations.Count = 0 Then
            UseBackgroundSpeech = False
        Else
            UseBackgroundSpeech = True
        End If

        'SelectedTestProtocol.IsInPretestMode = IsPractiseTest

        'Creates a new test 
        CurrentSipTestMeasurement = New SipMeasurement(CurrentParticipantID, SpeechMaterial.ParentTestSpecification,, SelectedTestparadigm)
        CurrentSipTestMeasurement.TestProcedure.LengthReduplications = 1 'SelectedLengthReduplications
        CurrentSipTestMeasurement.TestProcedure.TestParadigm = Testparadigm.FlexibleLocations 'SelectedTestparadigm
        SelectedTestparadigm = CurrentSipTestMeasurement.TestProcedure.TestParadigm

        CurrentSipTestMeasurement.ExportTrialSoundFiles = False

        If SimulatedSoundField = True Then
            SelectedSoundPropagationType = SoundPropagationTypes.SimulatedSoundField

            Dim AvailableSets = DirectionalSimulator.GetAvailableDirectionalSimulationSets(Transducer)
            DirectionalSimulator.TrySetSelectedDirectionalSimulationSet(AvailableSets(1), Transducer, PhaseAudiometry)

        Else
            SelectedSoundPropagationType = SoundPropagationTypes.PointSpeakers
        End If

        Select Case SipTestMode
            Case SiPTestModes.Directional

                If SelectedTestparadigm = Testparadigm.FlexibleLocations Then

                    CurrentSipTestMeasurement.TestProcedure.SetTargetStimulusLocations(SelectedTestparadigm, SignalLocations)

                    CurrentSipTestMeasurement.TestProcedure.SetMaskerLocations(SelectedTestparadigm, MaskerLocations)

                    CurrentSipTestMeasurement.TestProcedure.SetBackgroundLocations(SelectedTestparadigm, MaskerLocations)

                End If

                ''Checking if enough maskers where selected
                'If NumberOfSimultaneousMaskers > CurrentSipTestMeasurement.TestProcedure.MaskerLocations(SelectedTestparadigm).Count Then
                '    MsgBox("Select more masker locations of fewer maskers!", MsgBoxStyle.Information, "Not enough masker locations selected!")
                '    Exit Sub
                'End If

                'Setting up test trials to run
                SelectedPNRs.Add(SignalToNoiseRatio(TargetLevel, MaskingLevel))

                PlanDirectionalTestTrials(CurrentSipTestMeasurement, ReferenceLevel, Preset.Name, {MediaSet}.ToList, SelectedPNRs, NumberOfSimultaneousMaskers, SelectedSoundPropagationType, RandomSeed)

            Case Else
                Throw New NotImplementedException
        End Select

        'Checks to see if a simulation set is required
        If SelectedSoundPropagationType = SoundPropagationTypes.SimulatedSoundField And DirectionalSimulator.SelectedDirectionalSimulationSetName = "" Then
            Return New Tuple(Of Boolean, String)(False, "No directional simulation set selected!")
        End If

        If CurrentSipTestMeasurement.HasSimulatedSoundFieldTrials = True And DirectionalSimulator.SelectedDirectionalSimulationSetName = "" Then
            Return New Tuple(Of Boolean, String)(False, "The measurement requires a directional simulation set to be selected!")
        End If

        'Displayes the planned test length
        'PlannedTestLength_TextBox.Text = CurrentSipTestMeasurement.PlannedTrials.Count + CurrentSipTestMeasurement.ObservedTrials.Count

        'TODO: Calling GetTargetAzimuths only to ensure that the Actual Azimuths needed for presentation in the TestTrialTable exist. This should probably be done in some other way... (Only applies to the Directional3 and Directional5 Testparadigms)
        Select Case SelectedTestparadigm
            Case Testparadigm.Directional2, Testparadigm.Directional3, Testparadigm.Directional5
                CurrentSipTestMeasurement.GetTargetAzimuths()
        End Select




        'SpeechLevel
        'MaskingLevel
        'ReferenceLevel


        'Dim StartAdaptiveLevel As Double

        'SelectedTestProtocol.InitializeProtocol(New TestProtocol.NextTaskInstruction With {.AdaptiveValue = StartAdaptiveLevel, .TestStage = 0})

        'TryEnableTestStart()

        Return New Tuple(Of Boolean, String)(True, "")

    End Function



    Private Shared Sub PlanDirectionalTestTrials(ByRef SipTestMeasurement As SipMeasurement, ByVal ReferenceLevel As Double, ByVal PresetName As String,
                                      ByVal SelectedMediaSets As List(Of MediaSet), ByVal SelectedPNRs As List(Of Double), ByVal NumberOfSimultaneousMaskers As Integer,
                                                 ByVal SoundPropagationType As SoundPropagationTypes, Optional ByVal RandomSeed As Integer? = Nothing)

        'Creating a new random if seed is supplied
        If RandomSeed.HasValue Then SipTestMeasurement.Randomizer = New Random(RandomSeed)

        'Getting the preset
        Dim Preset = SipTestMeasurement.ParentTestSpecification.SpeechMaterial.Presets.GetPretest(PresetName).Members

        'Clearing any trials that may have been planned by a previous call
        SipTestMeasurement.ClearTrials()

        'Getting the sound source locations
        Dim CurrentTargetLocations = SipTestMeasurement.TestProcedure.TargetStimulusLocations(SipTestMeasurement.TestProcedure.TestParadigm)
        Dim MaskerLocations = SipTestMeasurement.TestProcedure.MaskerLocations(SipTestMeasurement.TestProcedure.TestParadigm)
        Dim BackgroundLocations = SipTestMeasurement.TestProcedure.BackgroundLocations(SipTestMeasurement.TestProcedure.TestParadigm)


        For Each PresetComponent In Preset
            For Each SelectedMediaSet In SelectedMediaSets
                For Each PNR In SelectedPNRs
                    For Each TargetLocation In CurrentTargetLocations

                        'Drawing two random MaskerLocations
                        Dim CurrentMaskerLocations As New List(Of SoundSourceLocation)
                        Dim SelectedMaskerIndices As New List(Of Integer)
                        SelectedMaskerIndices.AddRange(Utils.SampleWithoutReplacement(NumberOfSimultaneousMaskers, 0, MaskerLocations.Length, SipTestMeasurement.Randomizer))
                        For Each RandomIndex In SelectedMaskerIndices
                            CurrentMaskerLocations.Add(MaskerLocations(RandomIndex))
                        Next

                        For Repetition = 1 To SipTestMeasurement.TestProcedure.LengthReduplications

                            Dim NewTestUnit = New SiPTestUnit(SipTestMeasurement)

                            Dim TestWords = PresetComponent.GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.Sentence)
                            NewTestUnit.SpeechMaterialComponents.AddRange(TestWords)

                            For c = 0 To TestWords.Count - 1
                                'TODO/NB: The following line uses only a single TargetLocation, even though several could in principle be set
                                Dim NewTrial As New SipTrial(NewTestUnit, TestWords(c), SelectedMediaSet, SoundPropagationType, {TargetLocation}, CurrentMaskerLocations.ToArray, BackgroundLocations, NewTestUnit.ParentMeasurement.Randomizer)
                                NewTrial.SetLevels(ReferenceLevel, PNR)
                                NewTestUnit.PlannedTrials.Add(NewTrial)
                            Next

                            'Adding from the selected media set
                            SipTestMeasurement.TestUnits.Add(NewTestUnit)

                        Next
                    Next
                Next
            Next
        Next

        'Adding the trials SipTestMeasurement (from which they can be drawn during testing)
        For Each Unit In SipTestMeasurement.TestUnits
            For Each Trial In Unit.PlannedTrials
                SipTestMeasurement.PlannedTrials.Add(Trial)
            Next
        Next

        'Randomizing the order
        If SipTestMeasurement.TestProcedure.RandomizeOrder = True Then
            Dim RandomList As New List(Of SipTrial)
            Do Until SipTestMeasurement.PlannedTrials.Count = 0
                Dim RandomIndex As Integer = SipTestMeasurement.Randomizer.Next(0, SipTestMeasurement.PlannedTrials.Count)
                RandomList.Add(SipTestMeasurement.PlannedTrials(RandomIndex))
                SipTestMeasurement.PlannedTrials.RemoveAt(RandomIndex)
            Loop
            SipTestMeasurement.PlannedTrials = RandomList
        End If

    End Sub



    Private Sub TryEnableTestStart()

        'If SelectedTransducer.CanPlay = False Then
        '    'Aborts if the SelectedTransducer cannot be used to play sound
        '    ShowMessageBox("Unable To play sound Using the selected transducer!", "Sound player Error")
        '    Exit Sub
        'End If

        If CurrentSipTestMeasurement Is Nothing Then
            ShowMessageBox("Inget test är laddat.", "SiP-test")
            Exit Sub
        End If

        'Sets the measurement datetime
        CurrentSipTestMeasurement.MeasurementDateTime = DateTime.Now

        'Getting NeededTargetAzimuths for the Directional2, Directional3 and Directional5 Testparadigms
        Dim NeededTargetAzimuths As List(Of Double) = Nothing
        Select Case SelectedTestparadigm
            Case Testparadigm.Directional2, Testparadigm.Directional3, Testparadigm.Directional5
                NeededTargetAzimuths = CurrentSipTestMeasurement.GetTargetAzimuths()
        End Select



        'Select Case GuiLanguage
        '    Case Utils.Constants.Languages.Swedish
        '        ParticipantControl.ShowMessage("Testet börjar strax")
        '    Case Utils.Constants.Languages.English
        '        ParticipantControl.ShowMessage("The test is about to start")
        'End Select

        TryStartTest()

    End Sub


    Private Sub TryStartTest()

        If TestIsStarted = False Then

            'If SelectedTestDescription = "" Then
            '    ShowMessageBox("Please provide a test description (such As 'test 1, with HA')!", "SiP-test")
            '    Exit Sub
            'End If



            'Storing the test description
            CurrentSipTestMeasurement.Description = "SiP test 1" ' SelectedTestDescription

            'Setting the default export path
            CurrentSipTestMeasurement.SetDefaultExportPath()

            'Things seemed to be in order,
            'Starting the test

            TestIsStarted = True

            InitiateTestByPlayingSound()

        Else
            ''Test is started
            'If TestIsPaused = True Then
            '    ResumeTesting()
            'Else
            '    PauseTesting()
            'End If
        End If

    End Sub


    Protected Overrides Sub InitiateTestByPlayingSound()

        'UpdateTestProgress()
        ''Updates the progress bar
        'If ShowProgressIndication = True Then
        '    ParticipantControl.UpdateTestFormProgressbar(CurrentSipTestMeasurement.ObservedTrials.Count, CurrentSipTestMeasurement.ObservedTrials.Count + CurrentSipTestMeasurement.PlannedTrials.Count)
        'End If


        'Removes the start button
        'ParticipantControl.ResetTestItemPanel()

        'Cretaing a context sound without any test stimulus, that runs for approx TestSetup.PretestSoundDuration seconds, using audio from the first selected MediaSet
        Dim TestSound As Audio.Sound = CreateInitialSound(MediaSet)

        'Plays sound
        SoundPlayer.SwapOutputSounds(TestSound)

        'Setting the interval to the first test stimulus using NewTrialTimer.Interval (N.B. The NewTrialTimer.Interval value has to be reset at the first tick, as the deafault value is overridden here)
        'StartTrialTimer.Interval = Math.Max(1, PretestSoundDuration * 1000)

        'Premixing the first 10 sounds 
        CurrentSipTestMeasurement.PreMixTestTrialSoundsOnNewTread(Transducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, SipMeasurementRandomizer, TrialSoundMaxDuration, UseBackgroundSpeech, 10)

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
            Dim Background1 = BackgroundNonSpeech_Sound.CopySection(1, SipMeasurementRandomizer.Next(0, BackgroundNonSpeech_Sound.WaveData.SampleData(1).Length - TrialSoundLength - 2), TrialSoundLength)
            Dim Background2 = BackgroundNonSpeech_Sound.CopySection(1, SipMeasurementRandomizer.Next(0, BackgroundNonSpeech_Sound.WaveData.SampleData(1).Length - TrialSoundLength - 2), TrialSoundLength)

            'Sets up fading specifications for the background signals
            Dim FadeSpecs_Background = New List(Of Audio.DSP.Transformations.FadeSpecifications)
            FadeSpecs_Background.Add(New Audio.DSP.Transformations.FadeSpecifications(Nothing, 0, 0, CurrentSampleRate * 1))
            FadeSpecs_Background.Add(New Audio.DSP.Transformations.FadeSpecifications(0, Nothing, -CurrentSampleRate * 0.01))

            'Adds the background (non-speech) signals, with fade, duck and location specifications
            Dim LevelGroup As Integer = 1 ' The level group value is used to set the added sound level of items sharing the same (arbitrary) LevelGroup value to the indicated sound level. (Thus, the sounds with the same LevelGroup value are measured together.)
            ItemList.Add(New SoundSceneItem(Background1, 1, SelectedMediaSet.BackgroundNonspeechRealisticLevel, LevelGroup,
                                                                                              New SoundSourceLocation With {.HorizontalAzimuth = -30},
                                                                                              SoundSceneItem.SoundSceneItemRoles.BackgroundSpeech, 0,,,, FadeSpecs_Background))
            ItemList.Add(New SoundSceneItem(Background2, 1, SelectedMediaSet.BackgroundNonspeechRealisticLevel, LevelGroup,
                                                                                              New SoundSourceLocation With {.HorizontalAzimuth = 30},
                                                                                              SoundSceneItem.SoundSceneItemRoles.BackgroundNonspeech, 0,,,, FadeSpecs_Background))
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








    'Private Sub PrepareResponseScreenData()

    '    'Creates a response string
    '    Select Case SelectedTestparadigm
    '        Case Testparadigm.Directional2, Testparadigm.Directional3, Testparadigm.Directional5
    '            'TODO: the following line only uses the first of possible target stimulus locations
    '            CorrectResponse = CurrentSipTrial.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling") & vbTab & CurrentSipTrial.TargetStimulusLocations(0).ActualLocation.HorizontalAzimuth
    '        Case Else
    '            CorrectResponse = CurrentSipTrial.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling")
    '    End Select

    '    'Collects the response alternatives
    '    TestWordAlternatives = New List(Of Tuple(Of String, SoundSourceLocation))
    '    Dim TempList As New List(Of SpeechMaterialComponent)
    '    CurrentSipTrial.SpeechMaterialComponent.IsContrastingComponent(,, TempList)
    '    For Each ContrastingComponent In TempList
    '        'TODO: the following line only uses the first of each possible contrasting response alternative stimulus locations
    '        TestWordAlternatives.Add(New Tuple(Of String, SoundSourceLocation)(ContrastingComponent.GetCategoricalVariableValue("Spelling"), CurrentSipTrial.TargetStimulusLocations(0).ActualLocation))
    '    Next

    '    'Randomizing the order
    '    Dim AlternativesCount As Integer = TestWordAlternatives.Count
    '    Dim TempList2 As New List(Of Tuple(Of String, SoundSourceLocation))
    '    For n = 0 To AlternativesCount - 1
    '        Dim RandomIndex As Integer = SipMeasurementRandomizer.Next(0, TestWordAlternatives.Count)
    '        TempList2.Add(TestWordAlternatives(RandomIndex))
    '        TestWordAlternatives.RemoveAt(RandomIndex)
    '    Next
    '    TestWordAlternatives = TempList2

    'End Sub

    Private Sub PrepareTestTrialSound()

        Try

            'Resetting CurrentTestSound
            'CurrentTestSound = Nothing

            If CurrentSipTestMeasurement.TestProcedure.AdaptiveType <> SipTest.AdaptiveTypes.Fixed Then
                'Levels only need to be set here, and possibly not even here, in adaptive procedures. Its better if the level is set directly upon selection of the trial...
                'CurrentSipTrial.SetLevels()
            End If


            If (CurrentSipTestMeasurement.ObservedTrials.Count + 3) Mod 10 = 0 Then
                'Premixing the next 10 sounds, starting three trials before the next is needed 
                CurrentSipTestMeasurement.PreMixTestTrialSoundsOnNewTread(Transducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, SipMeasurementRandomizer, TrialSoundMaxDuration, UseBackgroundSpeech, 10)
            End If

            'Waiting for the background thread to finish mixing
            Dim WaitPeriods As Integer = 0
            While CurrentTestTrial.Sound Is Nothing
                WaitPeriods += 1
                Threading.Thread.Sleep(100)
                If LogToConsole = True Then Console.WriteLine("Waiting for sound to mix: " & WaitPeriods * 100 & " ms")
            End While

            'If CurrentSipTrial.Sound Is Nothing Then
            '    CurrentSipTrial.MixSound(SelectedTransducer, SelectedTestparadigm, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, SipMeasurementRandomizer, TrialSoundMaxDuration, UseBackgroundSpeech)
            'End If

            'References the sound
            'CurrentTestSound = CurrentSipTrial.Sound


            'Launches the trial if the start timer has ticked, without launching the trial (which happens when the sound preparation was not completed at the tick)
            'If StartTrialTimerHasTicked = True Then
            '    If CurrentTrialIsLaunched = False Then

            '        'Launching the trial
            '        LaunchTrial(CurrentTestSound)

            '    End If
            'End If

        Catch ex As Exception
            Utils.SendInfoToLog(ex.ToString, "ExceptionsDuringTesting")
        End Try

    End Sub



    Protected Overrides Sub PrepareNextTrial(ByVal NextTaskInstruction As TestProtocol.NextTaskInstruction)

        'Preparing the next trial
        'Creating a new test trial
        CurrentTestTrial = CurrentSipTestMeasurement.GetNextTrial()
        CurrentTestTrial.TestStage = NextTaskInstruction.TestStage
        CurrentTestTrial.Tasks = 1

        'CurrentTestTrial = New SrtTrial With {.SpeechMaterialComponent = NextTestWord,
        '                .AdaptiveValue = NextTaskInstruction.AdaptiveValue,
        '                .SpeechLevel = MaskingLevel + NextTaskInstruction.AdaptiveValue,
        '                .MaskerLevel = MaskingLevel,
        '                .TestStage = NextTaskInstruction.TestStage,
        '                .Tasks = 1}

        CurrentTestTrial.ResponseAlternativeSpellings = New List(Of List(Of SpeechTestResponseAlternative))

        Dim ResponseAlternatives As New List(Of SpeechTestResponseAlternative)
        If IsFreeRecall Then
            If CurrentTestTrial.SpeechMaterialComponent.ChildComponents.Count > 0 Then

                CurrentTestTrial.Tasks = 0
                For Each Child In CurrentTestTrial.SpeechMaterialComponent.ChildComponents()

                    If KeyWordScoring = True Then
                        ResponseAlternatives.Add(New SpeechTestResponseAlternative With {.Spelling = Child.GetCategoricalVariableValue("Spelling"), .IsScoredItem = Child.IsKeyComponent})
                    Else
                        ResponseAlternatives.Add(New SpeechTestResponseAlternative With {.Spelling = Child.GetCategoricalVariableValue("Spelling"), .IsScoredItem = True})
                    End If

                    CurrentTestTrial.Tasks += 1
                Next

            End If

        Else
            'Adding the current word spelling as a response alternative

            ResponseAlternatives.Add(New SpeechTestResponseAlternative With {.Spelling = CurrentTestTrial.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling"), .IsScoredItem = CurrentTestTrial.SpeechMaterialComponent.IsKeyComponent})
            CurrentTestTrial.Tasks = 1

            'Picking random response alternatives from all available test words
            Dim AllContrastingWords = CurrentTestTrial.SpeechMaterialComponent.GetSiblingsExcludingSelf()
            For Each ContrastingWord In AllContrastingWords
                ResponseAlternatives.Add(New SpeechTestResponseAlternative With {.Spelling = ContrastingWord.GetCategoricalVariableValue("Spelling"), .IsScoredItem = ContrastingWord.IsKeyComponent})
            Next

            'Shuffling the order of response alternatives
            ResponseAlternatives = Utils.Shuffle(ResponseAlternatives, Randomizer).ToList
        End If

        CurrentTestTrial.ResponseAlternativeSpellings.Add(ResponseAlternatives)

        'Mixing trial sound
        PrepareTestTrialSound()


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
        If IsFreeRecall = False Then CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = MaxResponseTimeTimer_Interval, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut})

    End Sub

    Public Overrides Function GetResultStringForGui() As String
        'Throw New NotImplementedException()
        Return ""
    End Function

End Class