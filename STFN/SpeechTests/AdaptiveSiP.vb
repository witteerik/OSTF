Imports STFN.SipTest
Imports STFN.Audio.SoundScene
Imports STFN.Utils
Imports STFN.TestProtocol

Public Class AdaptiveSiP

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


    Private PresetName As String = "IHeAR_CS"
    'Private PresetName As String = "QuickSiP"



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
        PlanSiPTrials(SelectedSoundPropagationType, RandomSeed)

        If CurrentSipTestMeasurement.HasSimulatedSoundFieldTrials = True And DirectionalSimulator.SelectedDirectionalSimulationSetName = "" Then
            Return New Tuple(Of Boolean, String)(False, "The measurement requires a directional simulation set to be selected!")
        End If

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

        If TestLists.Count Mod 2 = 1 Then
            Throw New Exception("We must use an even number of test lists in the AdaptiveSiP!")
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

        Dim NumberOfTrialsPerList As Integer = 14

        For i = 0 To TestLists.Count - 1

            'Creating one test unit per list
            Dim CurrentTestUnit = New SiPTestUnit(CurrentSipTestMeasurement)
            CurrentSipTestMeasurement.TestUnits.Add(CurrentTestUnit)

            'Creating a test protocol
            CurrentTestUnit.TestProtocol = New BrandKollmeier2002_TestProtocol()
            CurrentTestUnit.TestProtocol.InitializeProtocol(New TestProtocol.NextTaskInstruction With {.AdaptiveValue = 10, .TestStage = 0, .TestLength = NumberOfTrialsPerList})

            For t = 0 To NumberOfTrialsPerList - 1

                'Starting with left for every other list (and right for the other) and then swapping between every presentation
                Dim NewSiPTrial As SipTrial
                Dim modValue = i + t Mod 2
                If (i + t) Mod 2 = 0 Then
                    NewSiPTrial = New SipTrial(CurrentTestUnit, TestLists(i), MediaSet, SoundPropagationType, TargetStimulusLocations_HeadTurnedLeft.ToArray, MaskerLocations_HeadTurnedLeft.ToArray, BackgroundLocations_HeadTurnedLeft, CurrentTestUnit.ParentMeasurement.Randomizer)
                Else
                    NewSiPTrial = New SipTrial(CurrentTestUnit, TestLists(i), MediaSet, SoundPropagationType, TargetStimulusLocations_HeadTurnedRight.ToArray, MaskerLocations_HeadTurnedRight.ToArray, BackgroundLocations_HeadTurnedRight, CurrentTestUnit.ParentMeasurement.Randomizer)
                End If

                'Adding the trial
                CurrentTestUnit.PlannedTrials.Add(NewSiPTrial)

                'Planning the word/task presentation order for the list
                NewSiPTrial.TaskPresentationOrderList = Utils.SampleWithoutReplacement(TestLists(i).ChildComponents.Count, 0, TestLists(i).ChildComponents.Count, Randomizer).ToList

                'Inserting a randomly selected repeated components last in the presentation list
                Dim RepeatedWordSmcIndex As Integer = NewSiPTrial.TaskPresentationOrderList(Randomizer.Next(0, NewSiPTrial.TaskPresentationOrderList.Count))
                NewSiPTrial.TaskPresentationOrderList.Add(RepeatedWordSmcIndex)

                'Now RandomWordPresentationOrderList holds the following information:
                'Presentation order (index) -  SMC index
                '0                                               2 'The third SMC in the list, e.g. kil_fil_sil in (sil)
                '1                                               0 'The first SMC in the list, e.g. kil_fil_sil in (kil)
                '2                                               1 'The second SMC in the list, e.g. kil_fil_sil in (fil)
                '3                                               1 'The second SMC in the list, e.g. kil_fil_sil in (fil)


                'Determines which of the tasks/words that should be scored (skipping either the first or the repeated word)
                NewSiPTrial.ScoredTasksPresentationIndices = New List(Of Integer)
                If Randomizer.Next(0, 2) = 0 Then
                    'Scoring the first, ignoring the last presentation
                    For PresentationOrderIndex = 0 To NewSiPTrial.TaskPresentationOrderList.Count - 2
                        'Adding the presentation index for scoring (note that this loop skips the last item in TaskPresentationOrderList)
                        NewSiPTrial.ScoredTasksPresentationIndices.Add(PresentationOrderIndex)
                    Next
                Else
                    'Scoring the repeated presentation (ignoring the first time the later repeated word is presented)
                    Dim HasBeenIgnored As Boolean = False
                    For PresentationOrderIndex = 0 To NewSiPTrial.TaskPresentationOrderList.Count - 1

                        If HasBeenIgnored = False Then
                            If NewSiPTrial.TaskPresentationOrderList(PresentationOrderIndex) = RepeatedWordSmcIndex Then
                                'Noting that the first instence of the repaeted word has been skipped
                                HasBeenIgnored = True
                                'Skipping to next presentation index
                                Continue For
                            End If
                        End If

                        'Adding the presentation index for scoring, if not skipped above
                        NewSiPTrial.ScoredTasksPresentationIndices.Add(PresentationOrderIndex)
                    Next
                End If
            Next
        Next

        'Putting two presentations each of left turn trials in even-index lists in a row, and the swapping to right turn trials, and so on..., in CurrentSipTestMeasurement (from which they can be drawn during testing)
        For TrialIndexInList = 0 To NumberOfTrialsPerList - 1 Step 2
            For TrialIndexInList_shift = 0 To 1
                For u_shift = 0 To 1
                    For u = 0 To CurrentSipTestMeasurement.TestUnits.Count - 1 Step 2
                        CurrentSipTestMeasurement.PlannedTrials.Add(CurrentSipTestMeasurement.TestUnits(u + u_shift).PlannedTrials(TrialIndexInList + TrialIndexInList_shift))
                    Next
                Next
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

        'And also premixing the first sounds in each SipTestUnit

        For Each TestUnit In CurrentSipTestMeasurement.TestUnits

            'Calculating the speech level (of the first trial)
            Dim ProtocolReply = TestUnit.TestProtocol.NewResponse(New TrialHistory)

            'Getting the new adaptive value
            TestUnit.PlannedTrials(0).PNR = ProtocolReply.AdaptiveValue

            'Storing it also in AdaptiveProtocolValue, as this is used by the protocol
            TestUnit.PlannedTrials(0).AdaptiveProtocolValue = ProtocolReply.AdaptiveValue

            'Applying the levels
            TestUnit.PlannedTrials(0).SetLevels(ReferenceLevel, TestUnit.PlannedTrials(0).PNR)

            'Mixing the next sound in the unit
            TestUnit.PlannedTrials(0).MixSound(Transducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, Randomizer, TrialSoundMaxDuration, UseBackgroundSpeech)
            'TestUnit.PlannedTrials(0).PreMixTestTrialSoundOnNewTread(Transducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, Randomizer, TrialSoundMaxDuration, UseBackgroundSpeech)

        Next

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
            DirectCast(CurrentTestTrial, SipTrial).Response = String.Join("-", GivenResponses)

            For i = 0 To GivenResponses.Count - 1

                Dim ResponseSpelling As String = GivenResponses(i)
                Dim CorrectSpelling As String = CurrentTestTrial.SpeechMaterialComponent.ChildComponents(CurrentTestTrial.TaskPresentationOrderList(i)).GetCategoricalVariableValue("Spelling")

                If CurrentTestTrial.ScoredTasksPresentationIndices.Contains(i) Then

                    'Corrects the trial response, based on the given response
                    If ResponseSpelling = CorrectSpelling Then
                        CurrentTestTrial.ScoreList.Add(1)
                    Else
                        CurrentTestTrial.ScoreList.Add(0)
                    End If
                Else

                    'Skipping scoring of the catch trial, but stores it in a separate score list for export
                    If ResponseSpelling = CorrectSpelling Then
                        CurrentTestTrial.CatchTaskScoreList.Add(1)
                    Else
                        CurrentTestTrial.CatchTaskScoreList.Add(0)
                    End If
                End If
            Next

            'Clearing GivenResponses before next trial
            GivenResponses.Clear()

            'Adding the test trial
            CurrentSipTestMeasurement.MoveTrialToHistory(CurrentTestTrial)

            'Calculating the speech level
            ProtocolReply = DirectCast(CurrentTestTrial, SipTrial).ParentTestUnit.TestProtocol.NewResponse(DirectCast(CurrentTestTrial, SipTrial).ParentTestUnit.ObservedTrials)

            'Premixing the next trial in the test unit
            If DirectCast(CurrentTestTrial, SipTrial).ParentTestUnit.PlannedTrials.Count > 0 Then

                'Overriding the protocol reply decicion if there are more trials left (these will have their on protocols)
                ProtocolReply.Decision = SpeechTestReplies.GotoNextTrial

                'We must set the levels of the next trial in the unit here, before mixing it
                Dim NextUnitTrial As SipTrial = DirectCast(CurrentTestTrial, SipTrial).ParentTestUnit.PlannedTrials(0)

                'Getting the new adaptive value
                NextUnitTrial.PNR = ProtocolReply.AdaptiveValue

                'Storing it also in AdaptiveProtocolValue, as this is used by the protocol
                NextUnitTrial.AdaptiveProtocolValue = ProtocolReply.AdaptiveValue

                'Applying the levels
                NextUnitTrial.SetLevels(ReferenceLevel, NextUnitTrial.PNR)

                'Mixing the next sound in the unit
                NextUnitTrial.PreMixTestTrialSoundOnNewTread(Transducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, Randomizer, TrialSoundMaxDuration, UseBackgroundSpeech)
            End If

            'Taking a dump of the SpeechTest before swapping to the new trial
            CurrentTestTrial.SpeechTestPropertyDump = Utils.Logging.ListObjectPropertyValues(Me.GetType, Me)

        Else
            'Nothing to correct (this should be the start of a new test)

            'The protocol repy is not used here, but needed to avoid a null exception
            ProtocolReply = New NextTaskInstruction
            ProtocolReply.Decision = SpeechTestReplies.GotoNextTrial

            'And mixing and playing initial sound, and the first sounds in each test unit
            InitiateTestByPlayingSound()

        End If

        'TODO: We must store the responses and response times!!!

        If CurrentSipTestMeasurement.PlannedTrials.Count = 0 Then
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
        CurrentTestTrial = CurrentSipTestMeasurement.PlannedTrials(0) ' GetNextTrial()
        CurrentTestTrial.Tasks = 4
        CurrentTestTrial.ResponseAlternativeSpellings = New List(Of List(Of SpeechTestResponseAlternative))
        Dim ResponseAlternatives As New List(Of SpeechTestResponseAlternative)

        'Adding the current word spelling as a response alternative
        'ResponseAlternatives.Add(New SpeechTestResponseAlternative With {.Spelling = CurrentTestTrial.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling"), .IsScoredItem = CurrentTestTrial.SpeechMaterialComponent.IsKeyComponent, .ParentTestTrial = CurrentTestTrial})

        'Adding list members as response alternatives
        Dim AllListWords = CurrentTestTrial.SpeechMaterialComponent.GetChildren()
        For Each ContrastingWord In AllListWords
            ResponseAlternatives.Add(New SpeechTestResponseAlternative With {.Spelling = ContrastingWord.GetCategoricalVariableValue("Spelling"), .IsScoredItem = ContrastingWord.IsKeyComponent, .ParentTestTrial = CurrentTestTrial})
        Next

        'Shuffling the order of response alternatives
        ResponseAlternatives = Utils.Shuffle(ResponseAlternatives, Randomizer).ToList

        'Adding the response alternatives
        CurrentTestTrial.ResponseAlternativeSpellings.Add(ResponseAlternatives)

        'Waiting for the trial sound to be mixed, if not yet completed
        WaitForTestTrialSound()

        'Storing the LinguisticSoundStimulusStartTime and the LinguisticSoundStimulusDuration 
        CurrentTestTrial.LinguisticSoundStimulusStartTime = DirectCast(CurrentTestTrial, SipTrial).TestWordStartTime
        CurrentTestTrial.LinguisticSoundStimulusDuration = DirectCast(CurrentTestTrial, SipTrial).TestWordCompletedTime - CurrentTestTrial.LinguisticSoundStimulusStartTime
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
            ShowResponseAlternativesTimer_Interval = System.Math.Max(1, DirectCast(CurrentTestTrial, SipTrial).TestWordStartTime * 1000) + 1000 * ResponseAlternativeDelay
            MaxResponseTimeTimer_Interval = System.Math.Max(2, DirectCast(CurrentTestTrial, SipTrial).TestWordCompletedTime * 1000) + 1000 * MaximumResponseTime
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

        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = ShowResponseAlternativesTimer_Interval, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = MaxResponseTimeTimer_Interval, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut})

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