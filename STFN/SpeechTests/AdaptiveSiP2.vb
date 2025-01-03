Imports STFN.SipTest
Imports STFN.Audio.SoundScene
Imports STFN.Utils
Imports STFN.TestProtocol

Public Class AdaptiveSiP2

    Inherits SipBaseSpeechTest

    Public Overrides ReadOnly Property FilePathRepresentation As String = "Adaptive_SipTest"

    Public Sub New(ByVal SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)
        ApplyTestSpecificSettings()
    End Sub

    Public Shadows Sub ApplyTestSpecificSettings()

        TesterInstructions = "För detta test behövs inga inställningar." & vbCrLf & vbCrLf &
                "1. Informera patienten om hur testet går till." & vbCrLf &
                "2. Vänd skärmen till patienten. Be sedan patienten klicka på start för att starta testet."


        ParticipantInstructions = "Patientens uppgift: " & vbCrLf & vbCrLf &
                " - Patienten startar testet genom att klicka på knappen 'Start'" & vbCrLf &
                " - Under testet ska patienten lyssna efter enstaviga ord i olika ljudmiljöer och efter varje ord ange på skärmen vilket ord hen uppfattade. " & vbCrLf &
                " - Patienten ska gissa om hen är osäker. Många ord är mycket svåra att höra!" & vbCrLf &
                " - Efter varje ord har patienten maximalt " & MaximumResponseTime & " sekunder på sig att ange sitt svar." & vbCrLf &
                " - Om svarsalternativen blinkar i röd färg har patienten inte svarat i tid."

        'SupportsManualPausing = False

        ShowGuiChoice_TargetLocations = False
        ShowGuiChoice_MaskerLocations = False
        ShowGuiChoice_BackgroundNonSpeechLocations = False
        ShowGuiChoice_BackgroundSpeechLocations = False

        MinimumStimulusOnsetTime = 0.3 + 0.3 ' 0.3 in sound field
        MaximumStimulusOnsetTime = 0.8 + 0.3 ' 0.3 in sound field

        ResponseAlternativeDelay = 0.02

        'DirectionalSimulationSet = "ARC - Harcellen - HATS - SiP"

    End Sub

    Public Overrides ReadOnly Property ShowGuiChoice_TargetSNRLevel As Boolean = False


    'Private PresetName As String = "IHeAR_CS"
    Private PresetName As String = "AdaptiveSiP"

    'Defines the number of times each test word group is tested
    Private TestLength As Integer
    'Determines the number of test trials (this must be amultiple of 3, so that each trial is tested an equal number of times)
    Private TrialCount As Integer

    Private PlannedTestTrials As New TestTrialCollection
    Protected ObservedTrials As New TestTrialCollection

    Public Overrides Function GetObservedTestTrials() As IEnumerable(Of TestTrial)
        Return ObservedTrials
    End Function


    Public Overrides Function InitializeCurrentTest() As Tuple(Of Boolean, String)

        Transducer = AvaliableTransducers(0)

        CurrentSipTestMeasurement = New SipMeasurement(CurrentParticipantID, SpeechMaterial.ParentTestSpecification, AdaptiveTypes.Fixed, SelectedTestparadigm)

        CurrentSipTestMeasurement.ExportTrialSoundFiles = False ' TODO Set to false!

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

        'Defines the number of times each test word group is tested
        TestLength = 10
        'Determines the number of test trials (this must be amultiple of 3, so that each trial is tested an equal number of times)
        TrialCount = TestLength * 3

        'Setting up test trials to run
        PlanSiPTrials(SelectedSoundPropagationType, RandomSeed)

        If CurrentSipTestMeasurement.HasSimulatedSoundFieldTrials = True And DirectionalSimulator.SelectedDirectionalSimulationSetName = "" Then
            Return New Tuple(Of Boolean, String)(False, "The measurement requires a directional simulation set to be selected!")
        End If


        'Creating a test protocol
        TestProtocol = New BrandKollmeier2002_TestProtocol() With {.TargetThreshold = 2 / 3} 'Setting the target threshold to 2/3 as this is the expected midpoint of the psychometric function, given that chance score is 1/3.
        'TestProtocol = New AdaptiveSiP_TestProtocol() With {.TargetThreshold = 2 / 3} 'Setting the target threshold to 2/3 as this is the expected midpoint of the psychometric function, given that chance score is 1/3.
        TestProtocol.InitializeProtocol(New TestProtocol.NextTaskInstruction With {.AdaptiveValue = 10, .TestStage = 0, .TestLength = TestLength})


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
        If IsPractiseTest Then
            TestLists = CurrentSipTestMeasurement.ParentTestSpecification.SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List, False, True)
        Else
            TestLists = CurrentSipTestMeasurement.ParentTestSpecification.SpeechMaterial.Presets.GetPretest(PresetName).Members 'TODO! Specify correct members in text file
        End If


        'Getting the sound source locations
        'Head slightly turned right (i.e. Speech on left side)
        Dim TargetStimulusLocations_HeadTurnedRight = {New SoundSourceLocation With {.HorizontalAzimuth = -10, .Distance = 1.45}}
        Dim MaskerLocations_HeadTurnedRight = {New SoundSourceLocation With {.HorizontalAzimuth = 170, .Distance = 1.45}}
        Dim BackgroundLocations_HeadTurnedRight = {New SoundSourceLocation With {.HorizontalAzimuth = -10, .Distance = 1.45}, New SoundSourceLocation With {.HorizontalAzimuth = 170, .Distance = 1.45}}

        'Head slightly turned left (i.e. Speech on right side)
        Dim TargetStimulusLocations_HeadTurnedLeft = {New SoundSourceLocation With {.HorizontalAzimuth = 10, .Distance = 1.45}}
        Dim MaskerLocations_HeadTurnedLeft = {New SoundSourceLocation With {.HorizontalAzimuth = 190, .Distance = 1.45}}
        Dim BackgroundLocations_HeadTurnedLeft = {New SoundSourceLocation With {.HorizontalAzimuth = 10, .Distance = 1.45}, New SoundSourceLocation With {.HorizontalAzimuth = 190, .Distance = 1.45}}

        'Creating one test unit per list
        Dim CurrentTestUnit = New SiPTestUnit(CurrentSipTestMeasurement)
        CurrentSipTestMeasurement.TestUnits.Add(CurrentTestUnit)

        'Creating test trials
        For i = 0 To TestLength - 1

            'Creating a random order to draw words from 
            Dim RandomList As New SortedList(Of Integer, List(Of Integer)) 'TWG index, TW index presentation order
            For twgi = 0 To TestLists.Count - 1
                RandomList.Add(twgi, Utils.SampleWithoutReplacement(3, 0, 3, Randomizer).ToList)
            Next

            'Iterating over three TWG indices
            For w = 0 To 2

                'Starting with left for every other list (and right for the other) and then swapping between every presentation
                Dim NewTestTrial As New TestTrial
                NewTestTrial.SubTrials = New TestTrialCollection

                Dim modValue = i Mod 2
                If (i) Mod 2 = 0 Then

                    For twgi = 0 To TestLists.Count - 1

                        Dim SMC = TestLists(twgi).ChildComponents(RandomList(twgi)(w))
                        Dim NewSipSubTrial = New SipTrial(CurrentTestUnit, SMC, MediaSet, SoundPropagationType, TargetStimulusLocations_HeadTurnedLeft.ToArray, MaskerLocations_HeadTurnedLeft.ToArray, BackgroundLocations_HeadTurnedLeft, CurrentTestUnit.ParentMeasurement.Randomizer)
                        NewTestTrial.SubTrials.Add(NewSipSubTrial)

                    Next

                Else

                    For twgi = 0 To TestLists.Count - 1

                        Dim SMC = TestLists(twgi).ChildComponents(RandomList(twgi)(w))
                        Dim NewSipSubTrial = New SipTrial(CurrentTestUnit, SMC, MediaSet, SoundPropagationType, TargetStimulusLocations_HeadTurnedRight.ToArray, MaskerLocations_HeadTurnedRight.ToArray, BackgroundLocations_HeadTurnedRight, CurrentTestUnit.ParentMeasurement.Randomizer)
                        NewTestTrial.SubTrials.Add(NewSipSubTrial)

                    Next

                End If

                'Shuffling the order of sub-trials
                NewTestTrial.SubTrials.Shuffle(CurrentTestUnit.ParentMeasurement.Randomizer)

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

    Protected Overrides Sub InitiateTestByPlayingSound()

        'Sets the measurement datetime
        CurrentSipTestMeasurement.MeasurementDateTime = DateTime.Now

        'Cretaing a context sound without any test stimulus, that runs for approx TestSetup.PretestSoundDuration seconds, using audio from the first selected MediaSet
        Dim SelectedMediaSets As List(Of MediaSet) = AvailableMediasets

        Dim TestSound As Audio.Sound = CreateInitialSound(SelectedMediaSets(0))

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
        '    TestUnit.PlannedTrials(0).MixSound(Transducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, Randomizer, TrialSoundMaxDuration, UseBackgroundSpeech)
        '    'TestUnit.PlannedTrials(0).PreMixTestTrialSoundOnNewTread(Transducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, Randomizer, TrialSoundMaxDuration, UseBackgroundSpeech)

        'Next

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
            Dim MixedInitialSound As Audio.Sound = Transducer.Mixer.CreateSoundScene(ItemList, False, False, SelectedSoundPropagationType, Transducer.LimiterThreshold)

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

    Private GivenResponses As New List(Of String)

    Public Overrides Function GetSpeechTestReply(sender As Object, e As SpeechTestInputEventArgs) As SpeechTestReplies

        Dim ProtocolReply As NextTaskInstruction = Nothing

        If e IsNot Nothing Then

            'This is an incoming test trial response. Adding the first response to GivenResponses
            GivenResponses = e.LinguisticResponses

            'Storing the lingustic responses
            'DirectCast(CurrentTestTrial, SipTrial).Response = String.Join("-", GivenResponses)

            For i = 0 To GivenResponses.Count - 1

                Dim ResponseSpelling As String = GivenResponses(i)
                Dim CorrectSpelling As String = CurrentTestTrial.SubTrials(i).SpeechMaterialComponent.GetCategoricalVariableValue("Spelling")

                'Corrects the trial response, based on the given response
                If ResponseSpelling = CorrectSpelling Then
                    CurrentTestTrial.ScoreList.Add(1)
                Else
                    CurrentTestTrial.ScoreList.Add(0)
                End If

            Next

            'Clearing GivenResponses before next trial
            GivenResponses.Clear()

            'Moving test trial to trial history
            ObservedTrials.Add(CurrentTestTrial)
            PlannedTestTrials.RemoveAt(0)

            ProtocolReply = TestProtocol.NewResponse(ObservedTrials)

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
            CurrentTestTrial.SpeechTestPropertyDump = Utils.Logging.ListObjectPropertyValues(Me.GetType, Me)

        Else
            'Nothing to correct (this should be the start of a new test)

            'Calculating the speech level
            ProtocolReply = TestProtocol.NewResponse(ObservedTrials)

            'And mixing and playing initial sound, and the first sounds in each test unit
            InitiateTestByPlayingSound()

        End If

        'TODO: We must store the responses and response times!!!

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
            ResponseAlternatives = Utils.Shuffle(ResponseAlternatives, Randomizer).ToList

            'Adding the response alternatives
            CurrentTestTrial.ResponseAlternativeSpellings.Add(ResponseAlternatives)

        Next


        'Getting the new adaptive value
        'Storing values into the TestTrial
        CurrentTestTrial.AdaptiveProtocolValue = NextTaskInstruction.AdaptiveValue
        'DirectCast(CurrentTestTrial, SipTrial).PNR = NextTaskInstruction.AdaptiveValue
        'DirectCast(CurrentTestTrial, SipTrial).AdaptiveProtocolValue = NextTaskInstruction.AdaptiveValue

        'Storing also in all subtrials, and mixes their sounds
        Dim TrialSounds As New List(Of Audio.Sound)
        Dim TaskStartTimes As New List(Of Double)
        For i = 0 To CurrentTestTrial.SubTrials.Count - 1

            Dim SubTrial = CurrentTestTrial.SubTrials(i)

            DirectCast(SubTrial, SipTrial).PNR = NextTaskInstruction.AdaptiveValue

            'Storing it also in AdaptiveProtocolValue, as this is used by the protocol
            DirectCast(SubTrial, SipTrial).AdaptiveProtocolValue = NextTaskInstruction.AdaptiveValue

            'Applying the levels
            DirectCast(SubTrial, SipTrial).SetLevels(ReferenceLevel, NextTaskInstruction.AdaptiveValue)

            'Mixing the next sound in the unit

            If i = 0 Then
                DirectCast(SubTrial, SipTrial).MixSound(Transducer, MinimumStimulusOnsetTime, MinimumStimulusOnsetTime, Randomizer, MinimumStimulusOnsetTime + 3, UseBackgroundSpeech, ,, False, True) ' Using only MinimumStimulusOnsetTime here
            ElseIf i = CurrentTestTrial.SubTrials.Count - 1 Then
                DirectCast(SubTrial, SipTrial).MixSound(Transducer, 0, 0, Randomizer, TrialSoundMaxDuration, UseBackgroundSpeech, ,, True, False) ' TrialSoundMaxDuration should be shorter!
            Else
                DirectCast(SubTrial, SipTrial).MixSound(Transducer, 0, 0, Randomizer, 3, UseBackgroundSpeech, ,, True, True)
            End If

            'Exports sound file
            If CurrentSipTestMeasurement.ExportTrialSoundFiles = True Then DirectCast(SubTrial, SipTrial).Sound.WriteWaveFile(IO.Path.Combine(Utils.logFilePath, "AdaptiveSipSounds", "AdaptiveSipSounds_" & i & "wav"))

            'Storing the test word start times
            If i = 0 Then
                TaskStartTimes.Add(DirectCast(SubTrial, SipTrial).TestWordStartTime)
            Else
                TaskStartTimes.Add(MinimumStimulusOnsetTime + i * 2 + DirectCast(SubTrial, SipTrial).TestWordStartTime)
            End If

            'Adds the sound
            TrialSounds.Add(DirectCast(SubTrial, SipTrial).Sound)

        Next

        'Mix full trial sound
        CurrentTestTrial.Sound = Audio.DSP.ConcatenateSounds(TrialSounds, ,,,,, TrialSounds(0).WaveFormat.SampleRate * 1)

        'Exports sound file
        If CurrentSipTestMeasurement.ExportTrialSoundFiles = True Then CurrentTestTrial.Sound.WriteWaveFile(IO.Path.Combine(Utils.logFilePath, "AdaptiveSipSounds", "AdaptiveSipSounds_Mix" & "wav"))

        'Waiting for the trial sound to be mixed, if not yet completed
        'WaitForTestTrialSound()

        'Storing the LinguisticSoundStimulusStartTime and the LinguisticSoundStimulusDuration 
        CurrentTestTrial.LinguisticSoundStimulusStartTime = DirectCast(CurrentTestTrial.SubTrials(0), SipTrial).TestWordStartTime
        CurrentTestTrial.LinguisticSoundStimulusDuration = (CurrentTestTrial.Sound.WaveData.SampleData(1).Length / CurrentTestTrial.Sound.WaveFormat.SampleRate) - DirectCast(CurrentTestTrial.SubTrials(4), SipTrial).TestWordCompletedTime - CurrentTestTrial.LinguisticSoundStimulusStartTime
        CurrentTestTrial.MaximumResponseTime = MaximumResponseTime

        'Setting visual que intervals
        Dim ShowVisualQueTimer_Interval As Double
        Dim HideVisualQueTimer_Interval As Double
        Dim ShowResponseAlternativePositions_Interval As Integer
        Dim ShowResponseAlternativesTimer_Interval As Double
        Dim MaxResponseTimeTimer_Interval As Double

        If UseVisualQue = True Then
            ShowVisualQueTimer_Interval = System.Math.Max(1, DirectCast(CurrentTestTrial, SipTrial).TestWordStartTime * 1000)
            HideVisualQueTimer_Interval = System.Math.Max(2, DirectCast(CurrentTestTrial, SipTrial).TestWordCompletedTime * 1000)
            ShowResponseAlternativesTimer_Interval = HideVisualQueTimer_Interval + 1000 * ResponseAlternativeDelay 'TestSetup.CurrentEnvironment.TestSoundMixerSettings.ResponseAlternativeDelay * 1000
            MaxResponseTimeTimer_Interval = System.Math.Max(1, ShowResponseAlternativesTimer_Interval + 1000 * MaximumResponseTime)  ' TestSetup.CurrentEnvironment.TestSoundMixerSettings.MaximumResponseTime * 1000
        Else
            ShowResponseAlternativePositions_Interval = ShowResponseAlternativePositionsTime * 1000

            ShowResponseAlternativesTimer_Interval = ShowResponseAlternativePositions_Interval + 10 'System.Math.Max(1, CurrentTestTrial.LinguisticSoundStimulusDuration * 1000) + 1000 * ResponseAlternativeDelay
            'ShowResponseAlternativesTimer_Interval = System.Math.Max(1, DirectCast(CurrentTestTrial.SubTrials(0), SipTrial).TestWordStartTime * 1000) + 1000 * ResponseAlternativeDelay
            MaxResponseTimeTimer_Interval = System.Math.Max(2, CurrentTestTrial.LinguisticSoundStimulusDuration * 1000) + 1000 * MaximumResponseTime
            'MaxResponseTimeTimer_Interval = System.Math.Max(2, DirectCast(CurrentTestTrial, SipTrial).TestWordCompletedTime * 1000) + 1000 * MaximumResponseTime
        End If

        'Setting trial events
        CurrentTestTrial.TrialEventList = New List(Of ResponseViewEvent)
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 1, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})

        If UseVisualQue = False Then
            ' Test word alternatives on the sides are only supported when the visual que is not shown
            If ShowTestSide = True Then
                CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = ShowResponseAlternativePositions_Interval, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternativePositions})
            End If
        Else
            CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = ShowVisualQueTimer_Interval, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowVisualCue})
            CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = HideVisualQueTimer_Interval, .Type = ResponseViewEvent.ResponseViewEventTypes.HideVisualCue})
        End If

        For Each Value In TaskStartTimes
            CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = Value * 1000, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowVisualCue})
        Next

        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = ShowResponseAlternativesTimer_Interval, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})
        'CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = MaxResponseTimeTimer_Interval, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut})

    End Sub


    Private Function GetAverageHeadTurnScores(Optional ByVal TurnedRight As Boolean? = Nothing)

        Dim TrialScoreList As New List(Of Integer)
        For Each Trial In CurrentSipTestMeasurement.ObservedTrials

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

    ''' <summary>
    ''' This function should list the names of variables included SpeechTestDump of each test trial to be exported in the "selected-variables" export file.
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function GetSelectedExportVariables() As List(Of String)
        Return New List(Of String)
    End Function


End Class