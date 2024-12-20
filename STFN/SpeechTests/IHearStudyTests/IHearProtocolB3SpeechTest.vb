Imports System.IO
Imports MathNet.Numerics
Imports MathNet.Numerics.Distributions
Imports STFN.Audio
Imports STFN.TestProtocol
Imports STFN.Utils

Public Class IHearProtocolB3SpeechTest
    Inherits SpeechTest

    Public Overrides ReadOnly Property FilePathRepresentation As String = "ProtocolB3_UserOperatedWRS"

    Public Sub New(SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)
        ApplyTestSpecificSettings()
    End Sub

    Public Sub ApplyTestSpecificSettings()

        TesterInstructions = "(Detta test går ut på att undersöka om fyra olika testordslistor är lika svåra.)" & vbCrLf & vbCrLf &
            "1. Välj testöra." & vbCrLf &
            "2. Ställ talnivå till patientes TMV3 på testörat, eller maximalt " & MaximumLevel_Targets & " dB HL." & vbCrLf &
            "3. Om kontralateralt brus behövs, akivera kontralateralt brus och ställ in brusnivå enligt normal klinisk praxis." & vbCrLf &
            "4. Informera patienten om hur testet går till." & vbCrLf &
            "5. Vänd skärmen till patienten. Be sedan patienten klicka på start för att starta testet."

        ParticipantInstructions = "Patientens uppgift: " & vbCrLf & vbCrLf &
            " - Patienten startar testet genom att klicka på knappen 'Start'" & vbCrLf &
            " - Under testet ska patienten lyssna efter enstaviga ord och efter varje ord ange på skärmen vilket ord hen uppfattade. " & vbCrLf &
            " - Patienten ska gissa om hen är osäker." & vbCrLf &
            " - Efter varje ord har patienten maximalt " & MaximumResponseTime & " sekunder på sig att ange sitt svar." & vbCrLf &
            " - Om svarsalternativen blinkar i röd färg har patienten inte svarat i tid." & vbCrLf &
            " - Testet består av fyra 25-ordslistor som körs direkt efter varandra, med möjlighet till en kort paus mellan varje."

        HasOptionalPractiseTest = False
        AllowsUseRetsplChoice = False
        AllowsManualPreSetSelection = False
        AllowsManualStartListSelection = False
        AllowsManualMediaSetSelection = False
        SupportsPrelistening = False
        UseSoundFieldSimulation = TriState.False
        AvailableTestModes = New List(Of TestModes) From {TestModes.ConstantStimuli}
        AvailableTestProtocols = New List(Of TestProtocol)
        AvailableFixedResponseAlternativeCounts = New List(Of Integer) From {4}
        AvailablePhaseAudiometryTypes = New List(Of BmldModes)
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
        UseListOrderRandomization = Utils.Constants.TriState.True
        UseWithinListRandomization = Utils.Constants.TriState.True
        UseAcrossListRandomization = Utils.Constants.TriState.False
        UseFreeRecall = Utils.TriState.False
        UseDidNotHearAlternative = Utils.Constants.TriState.False
        UsePhaseAudiometry_DefaultValue = Utils.Constants.TriState.False
        TargetLevel_StepSize = 1
        HistoricTrialCount = 0
        SupportsManualPausing = False
        DefaultReferenceLevel = 65
        DefaultSpeechLevel = 65
        DefaultMaskerLevel = 65
        DefaultBackgroundLevel = 50
        DefaultContralateralMaskerLevel = 25
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
        AvailableExperimentNumbers() = {}

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





    Private PlannedTestData As New List(Of TrialHistory)
    Private ObservedTestData As New List(Of TrialHistory)

    Private MaximumSoundDuration As Double = 10
    Private TestWordPresentationTime As Double = 0.5
    Private MaximumResponseTime As Double = 4

    Private IsInitialized As Boolean = False
    Private TestStage As Integer = 0

    Private ContralateralNoise As Audio.Sound = Nothing
    Private SilentSound As Audio.Sound = Nothing


    Public Overrides Function InitializeCurrentTest() As Tuple(Of Boolean, String)

        If IsInitialized = True Then Return New Tuple(Of Boolean, String)(True, "")

        TestStage = 0

        If PlanTrials() = False Then
            'Send message
            Return New Tuple(Of Boolean, String)(False, "Unable to plan test trials!")
        End If

        IsInitialized = True

        Return New Tuple(Of Boolean, String)(True, "")

    End Function

    Private Function PlanTrials()

        Dim AllMediaSets = AvailableMediasets

        'Select MediaSet / voice, using female voice only, as in AMTEST
        For Each MediaSet In AllMediaSets
            If MediaSet.TalkerGender = MediaSet.Genders.Female Then
                SelectedMediaSet = MediaSet
                Exit For
            End If
        Next

        'Plan trials, in four test stages
        Dim SMC_Lists = Me.SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List)
        For Each List In SMC_Lists
            Dim NewTestList As New TrialHistory

            For Each Sentence_SMC In List.ChildComponents

                Dim NewTrial = New WrsTrial
                NewTrial.SpeechMaterialComponent = Sentence_SMC
                NewTrial.SpeechLevel = SpeechLevel
                NewTrial.ContralateralMaskerLevel = ContralateralMaskingLevel

                Select Case SignalLocations(0).HorizontalAzimuth
                    Case -90
                        NewTrial.TestEar = SidesWithBoth.Left
                    Case 90
                        NewTrial.TestEar = SidesWithBoth.Right
                    Case Else
                        Throw New Exception("Unsupported signal azimuth: " & SignalLocations(0).HorizontalAzimuth)
                End Select

                'Setting response alternatives
                Dim ResponseAlternatives As New List(Of SpeechTestResponseAlternative)
                ResponseAlternatives.Add(New SpeechTestResponseAlternative With {.Spelling = NewTrial.SpeechMaterialComponent.ChildComponents(0).GetCategoricalVariableValue("Spelling"), .IsScoredItem = True})

                Dim ResponseAlternativeString = NewTrial.SpeechMaterialComponent.ChildComponents(0).GetCategoricalVariableValue("Alternatives")
                Dim ResponseAlternativeStringSplit = ResponseAlternativeString.Split(",")
                For Each ResponseAlternative In ResponseAlternativeStringSplit
                    If ResponseAlternative.Trim <> "" Then
                        ResponseAlternatives.Add(New SpeechTestResponseAlternative With {.Spelling = ResponseAlternative.Trim, .IsScoredItem = True})
                    End If
                Next

                'Shuffling the order of response alternatives
                ResponseAlternatives = Utils.Shuffle(ResponseAlternatives, Randomizer).ToList

                NewTrial.ResponseAlternativeSpellings = New List(Of List(Of SpeechTestResponseAlternative))
                NewTrial.ResponseAlternativeSpellings.Add(ResponseAlternatives)

                'Setting trial events
                NewTrial.TrialEventList = New List(Of ResponseViewEvent)
                NewTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 1, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})
                NewTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = System.Math.Max(1, 1000 * TestWordPresentationTime), .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})
                NewTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = System.Math.Max(1, 1000 * (TestWordPresentationTime + MaximumResponseTime)), .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut})

                NewTestList.Add(NewTrial)

            Next

            PlannedTestData.Add(NewTestList)

            'Also creating a list to hold observed test data, into which obesrved trials should be moved
            ObservedTestData.Add(New TrialHistory)

        Next

        'Getting the contralateral noise from the first trial SMC
        ContralateralNoise = PlannedTestData(0)(0).SpeechMaterialComponent.GetContralateralMaskerSound(SelectedMediaSet, 0)

        'Ranomizing list order
        If RandomizeListOrder = True Then

            Dim SampleOrder = Utils.SampleWithoutReplacement(PlannedTestData.Count, 0, PlannedTestData.Count, Randomizer)
            Dim TempList As New List(Of TrialHistory)
            For Each RandomIndex In SampleOrder
                TempList.Add(PlannedTestData(RandomIndex))
            Next
            PlannedTestData.Clear()
            PlannedTestData.AddRange(TempList)

        End If

        'Ranomizing within-list trial order
        If RandomizeListOrder = True Then
            For Each List In PlannedTestData
                List.Shuffle(Randomizer)
            Next
        End If

        Return True

    End Function

    Public Overrides Function GetSpeechTestReply(sender As Object, e As SpeechTestInputEventArgs) As SpeechTestReplies

        Dim ProtocolReply As New NextTaskInstruction

        If e Is Nothing Then
            'Nothing to correct (this should be the start of a new test, or a resuming of a paused test)
            ProtocolReply.Decision = SpeechTestReplies.GotoNextTrial

        Else

            'This is an incoming test trial response
            'Corrects the trial response, based on the given response

            DirectCast(CurrentTestTrial, WrsTrial).LinguisticResponse = e.LinguisticResponses(0)

            If e.LinguisticResponses(0) = CurrentTestTrial.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling") Then
                CurrentTestTrial.IsCorrect = True
            Else
                CurrentTestTrial.IsCorrect = False
            End If

            'TODO: Store response time

            'Moving the current test trial to the observed data
            ObservedTestData(TestStage).Add(CurrentTestTrial)
            PlannedTestData(TestStage).Remove(CurrentTestTrial)

            If PlannedTestData(TestStage).Count = 0 Then

                If TestStage > PlannedTestData.Count - 2 Then
                    'This is the end of the last list
                    ProtocolReply.Decision = SpeechTestReplies.TestIsCompleted
                Else
                    'This is the end of tha current (not last) list
                    ProtocolReply.Decision = SpeechTestReplies.PauseTestingWithCustomInformation

                    PauseInformation = "Klicka OK för att starta nästa steg av testet"

                    'Incrementing test stage
                    TestStage += 1
                End If

            Else
                ProtocolReply.Decision = SpeechTestReplies.GotoNextTrial

            End If
        End If

        'Preparing next trial if needed
        If ProtocolReply.Decision = SpeechTestReplies.GotoNextTrial Then

            'Assigning the next trial
            CurrentTestTrial = PlannedTestData(TestStage)(0)

            MixNextTrialSound()
        End If

        Return ProtocolReply.Decision

    End Function

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
        Dim TargetSpeechLevel_FS As Double = Audio.Standard_dBSPL_To_dBFS(DirectCast(CurrentTestTrial, WrsTrial).SpeechLevel) + RETSPL_Correction
        Dim NeededSpeechGain = TargetSpeechLevel_FS - NominalLevel_FS

        'Adjusts the sound levels
        Audio.DSP.AmplifySection(TestWordSound, NeededSpeechGain)

        If UseContralateralMasking = True Then

            'Setting level, 
            'Very important: The contralateral masking sound file cannot be the same as the ipsilateral masker sound. The level of the contralateral masker sound must be set to agree with the Nominal level (while the ipsilateral masker sound sound have a level that deviates from the nominal level to attain the desired SNR!)
            Dim ContralateralMaskingNominalLevel_FS = ContralateralNoise.SMA.NominalLevel
            Dim TargetContralateralMaskingLevel_FS As Double = Audio.Standard_dBSPL_To_dBFS(DirectCast(CurrentTestTrial, WrsTrial).ContralateralMaskerLevel) + SelectedMediaSet.EffectiveContralateralMaskingGain + RETSPL_Correction

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

        'Also stores the mediaset
        CurrentTestTrial.MediaSetName = SelectedMediaSet.MediaSetName

        'And the contralateral noise on/off setting
        CurrentTestTrial.UseContralateralNoise = UseContralateralMasking

        'And the EM term
        CurrentTestTrial.EfficientContralateralMaskingTerm = SelectedMediaSet.EffectiveContralateralMaskingGain

    End Sub

    Public Overrides Sub FinalizeTest()
        'This test doesn't need to bi finalized

    End Sub

    Public Overrides Function GetResultStringForGui() As String

        Dim Output As New List(Of String)

        For TestStageIndex = 0 To ObservedTestData.Count - 1

            Dim ScoreList As New List(Of Double)
            For Each Trial As WrsTrial In ObservedTestData(TestStageIndex)
                If Trial.IsCorrect = True Then
                    ScoreList.Add(1)
                Else
                    ScoreList.Add(0)
                End If
            Next

            If ScoreList.Count > 0 Then
                Output.Add("Lista " & TestStageIndex & ": Resultat = " & System.Math.Round(100 * ScoreList.Average) & " % korrekt (" & ScoreList.Sum & " / " & ObservedTestData(TestStageIndex).Count & ")")
            End If
        Next

        Return String.Join(vbCrLf, Output)

    End Function

    Public Overrides Function GetTestTrialResultExportString() As String
        Return "Export of trial level test results is not yet implemented"
    End Function

    Public Overrides Function GetTestResultsExportString() As String

        Dim ExportStringList As New List(Of String)

        Dim TestTrialIndex As Integer = 0
        For TestStageIndex = 0 To ObservedTestData.Count - 1
            For Each Trial As WrsTrial In ObservedTestData(TestStageIndex)

                If TestTrialIndex = 0 Then
                    ExportStringList.Add("TrialIndex" & vbTab & Trial.TestResultColumnHeadings)
                End If
                ExportStringList.Add(TestTrialIndex & vbTab & Trial.TestResultAsTextRow)
                TestTrialIndex += 1

            Next
        Next

        Return String.Join(vbCrLf, ExportStringList)

    End Function


    Public Overrides Function CreatePreTestStimulus() As Tuple(Of Sound, String)

        'No pre-test stimulus are available
        Return Nothing

    End Function

    Public Overrides Sub UpdateHistoricTrialResults(sender As Object, e As SpeechTestInputEventArgs)
        'Ignores, not used
    End Sub


End Class
