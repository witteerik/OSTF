Imports System.IO
Imports MathNet.Numerics
Imports MathNet.Numerics.Distributions
Imports STFN.Audio
Imports STFN.TestProtocol
Imports STFN.Utils

Public Class IHearProtocolB5SpeechTest
    Inherits SpeechTest

    Public Sub New(SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)

    End Sub

    Public Overrides ReadOnly Property FilePathRepresentation As String
        Get
            Return "ProtocolB5_UserOperatedWRS"
        End Get
    End Property

#Region "Settings"

    Public Overrides ReadOnly Property TesterInstructions As String
        Get

            Return "(Detta test går ut på att undersöka svårighetsgraden hos listorna med enstaviga ord (för svenska AMTEST) när man varierar ljudnivån.)" & vbCrLf & vbCrLf &
                "1. Ange experimentnummer." & vbCrLf &
                "2. Välj testöra." & vbCrLf &
                "3. Ställ talnivå till deltagarens Fletcher-regel-TMV på testörat (dvs. medelvärdet av de två bästa hörtrösklarna på 500, 1000 och 2000 Hz)" & vbCrLf &
                "     (OBS! Talnivån kommer sedan ändras automatiskt under testet.)" & vbCrLf &
                "4. Aktivera kontralateralt brus och ställ in brusnivå enligt normal klinisk praxis (OBS. Ha det aktiverat även om brusnivån är väldigt låg. Det går inte aktivera mitt under testet, ifall det skulle behövas.)." & vbCrLf &
                "5. Informera patienten om hur testet går till." & vbCrLf &
                "6. Vänd skärmen till patienten. Be sedan patienten klicka på start för att starta testet."

        End Get
    End Property

    Public Overrides ReadOnly Property ParticipantInstructions As String
        Get
            Return "Deltagarens uppgift: " & vbCrLf & vbCrLf &
                " - Deltagaren startar testet genom att klicka på knappen 'Start'" & vbCrLf &
                " - Under testet ska deltagaren lyssna efter enstaviga ord och efter varje ord ange på skärmen vilket ord hen uppfattade. " & vbCrLf &
                " - Deltagaren ska gissa om hen är osäker." & vbCrLf &
                " - Efter varje ord har deltagaren maximalt " & MaximumResponseTime & " sekunder på sig att ange sitt svar." & vbCrLf &
                " - Om svarsalternativen blinkar i röd färg har deltagaren inte svarat i tid." & vbCrLf &
                " - Testet består av åtta 25-ordslistor som körs direkt efter varandra, med möjlighet till en kort paus mellan varje." & vbCrLf &
                " - OBS! I vissa listor är orden mycket svaga, ibland knappast hörbara."

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
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property UseSoundFieldSimulation As Utils.TriState
        Get
            Return TriState.False
        End Get
    End Property

    Public Overrides ReadOnly Property AvailableTestModes As List(Of TestModes)
        Get
            Return New List(Of TestModes) From {TestModes.ConstantStimuli}
        End Get
    End Property

    Public Overrides ReadOnly Property AvailableTestProtocols As List(Of TestProtocol)
        Get
            Return New List(Of TestProtocol)
        End Get
    End Property

    Public Overrides ReadOnly Property AvailableFixedResponseAlternativeCounts As List(Of Integer)
        Get
            Return New List(Of Integer) From {4}
        End Get
    End Property

    Public Overrides ReadOnly Property AvailablePhaseAudiometryTypes As List(Of BmldModes)
        Get
            Return New List(Of BmldModes)
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
            Return Utils.Constants.TriState.True
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
            Return Utils.TriState.False
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
            Return False
        End Get
    End Property

    Public Overrides Sub UpdateHistoricTrialResults(sender As Object, e As SpeechTestInputEventArgs)
        'Ignores, not used
    End Sub

    Public Overrides Property SoundOverlapDuration As Double = 0.25
    Public Overrides ReadOnly Property MinimumLevel As Double = -50
    Public Overrides ReadOnly Property MaximumLevel As Double = 50

    Public Overrides ReadOnly Property AvailableExperimentNumbers As Integer()
        Get
            Dim OutputList As New List(Of Integer)
            For i = 1 To 50
                OutputList.Add(i)
            Next
            Return OutputList.ToArray
        End Get
    End Property

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

        If PlanTrials(CustomizableTestOptions.ExperimentNumber - 1) = False Then
            'Send message
            Return New Tuple(Of Boolean, String)(False, "Unable to plan test trials!")
        End If

        ''Temporary code for testing list-level combinations
        'Dim TempExportData = New List(Of String)
        'For n = 0 To 49
        '    PlannedTestData.Clear()
        '    PlanTrials(n)
        '    TempExportData.Add(GetPlannedTrialsExportString(n))
        '    PlannedTestData.Clear()
        'Next
        'SendInfoToLog(String.Join(vbCrLf, TempExportData), "ProtocolB5_PlannedTestTrials")

        IsInitialized = True

        Return New Tuple(Of Boolean, String)(True, "")

    End Function

    Private Function PlanTrials(ByVal ZeroBasePtcID As Integer)

        Dim AllMediaSets = AvailableMediasets

        'Select MediaSet / voice, using female voice only, as in AMTEST
        For Each MediaSet In AllMediaSets
            If MediaSet.TalkerGender = MediaSet.Genders.Female Then
                CustomizableTestOptions.SelectedMediaSet = MediaSet
                Exit For
            End If
        Next

        'Plan trials, in four test stages
        Dim SMC_Lists = Me.SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List)

        'Creating a repeating sequence of levels that should be tested
        Dim SpeechLevels As New List(Of Double)
        Dim LowestSpeechLevel As Double = 2
        Dim SpeechLevelStep As Double = 4
        Dim SpeechLevelCount As Double = 8
        Dim RepetitionsNeeded As Integer = 1 + Me.AvailableExperimentNumbers.Max / SpeechLevelCount
        For r = 0 To RepetitionsNeeded
            For i = 0 To SpeechLevelCount - 1
                SpeechLevels.Add(LowestSpeechLevel + i * SpeechLevelStep)
            Next
        Next

        'Picking the levels that should be tested in the current session, based on the ZeroBasePtcID 
        Dim CurrentSessionSpeechLevels As New List(Of Double)
        For i = ZeroBasePtcID To ZeroBasePtcID + SpeechLevelCount - 1
            CurrentSessionSpeechLevels.Add(SpeechLevels(i))
        Next

        'Gets the speech and cl-masking levels set by the audiologist (this should equal the Fletcher Rule PTA)
        Dim SpeechLevelReference = CustomizableTestOptions.SpeechLevel
        Dim ContralateralMaskingLeveldBBelowSpeech = CustomizableTestOptions.SpeechLevel - CustomizableTestOptions.ContralateralMaskingLevel

        'Adding each list twice, in order, and assigning the speech levels in the order specified in CurrentSessionSpeechLevels
        For ListBlockIndex As Integer = 0 To 1

            For ListIndex = 0 To SMC_Lists.Count - 1

                Dim List = SMC_Lists(ListIndex)
                Dim SpeechLevel = CurrentSessionSpeechLevels(ListBlockIndex * SMC_Lists.Count + ListIndex) + SpeechLevelReference
                'Setting the contralateral masking to retain the same relation as initially set by the audiologist
                Dim ContralateralMaskingLevel = SpeechLevel - ContralateralMaskingLeveldBBelowSpeech

                Dim NewTestList As New TrialHistory

                For Each Sentence_SMC In List.ChildComponents

                    Dim NewTrial = New WrsTrial
                    NewTrial.SpeechMaterialComponent = Sentence_SMC
                    NewTrial.SpeechLevel = SpeechLevel
                    NewTrial.ContralateralMaskerLevel = ContralateralMaskingLevel

                    Select Case CustomizableTestOptions.SignalLocations(0).HorizontalAzimuth
                        Case -90
                            NewTrial.TestEar = SidesWithBoth.Left
                        Case 90
                            NewTrial.TestEar = SidesWithBoth.Right
                        Case Else
                            Throw New Exception("Unsupported signal azimuth: " & CustomizableTestOptions.SignalLocations(0).HorizontalAzimuth)
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

        Next

        'Ranomizing list order
        If CustomizableTestOptions.RandomizeListOrder = True Then

            Dim SampleOrder = Utils.SampleWithoutReplacement(PlannedTestData.Count, 0, PlannedTestData.Count, Randomizer)
            Dim TempList As New List(Of TrialHistory)
            For Each RandomIndex In SampleOrder
                TempList.Add(PlannedTestData(RandomIndex))
            Next
            PlannedTestData.Clear()
            PlannedTestData.AddRange(TempList)

        End If

        'Getting the contralateral noise from the first trial SMC
        ContralateralNoise = PlannedTestData(0)(0).SpeechMaterialComponent.GetContralateralMaskerSound(CustomizableTestOptions.SelectedMediaSet, 0)

        'Ranomizing within-list trial order
        If CustomizableTestOptions.RandomizeListOrder = True Then
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
        If CustomizableTestOptions.UseRetsplCorrection = True Then
            RETSPL_Correction = CustomizableTestOptions.SelectedTransducer.RETSPL_Speech
        End If

        'Getting the speech signal
        Dim TestWordSound = CurrentTestTrial.SpeechMaterialComponent.GetSound(CustomizableTestOptions.SelectedMediaSet, 0, 1, , , , , False, False, False, , , False)
        Dim NominalLevel_FS = TestWordSound.SMA.NominalLevel

        'Storing the LinguisticSoundStimulusStartTime and the LinguisticSoundStimulusDuration (assuming that the linguistic recording is in channel 1)
        CurrentTestTrial.LinguisticSoundStimulusStartTime = TestWordPresentationTime
        CurrentTestTrial.LinguisticSoundStimulusDuration = TestWordSound.WaveData.SampleData(1).Length / TestWordSound.WaveFormat.SampleRate

        'Creating a silent sound (lazy method to get the same length independently of contralateral masking or not)
        Dim SilentSound = Audio.GenerateSound.CreateSilence(ContralateralNoise.WaveFormat, 1, MaximumSoundDuration)

        'Creating contalateral masking noise (with the same length as the masking noise)
        Dim TrialContralateralNoise As Audio.Sound = Nothing
        Dim IntendedNoiseLength As Integer
        If CustomizableTestOptions.UseContralateralMasking = True Then
            Dim TotalSoundLength = ContralateralNoise.WaveData.SampleData(1).Length
            IntendedNoiseLength = ContralateralNoise.WaveFormat.SampleRate * MaximumSoundDuration
            Dim RandomStartReadSample = Randomizer.Next(0, TotalSoundLength - IntendedNoiseLength)
            TrialContralateralNoise = ContralateralNoise.CopySection(1, RandomStartReadSample - 1, IntendedNoiseLength) ' TODO: Here we should check to ensure that the MaskerNoise is long enough
        End If

        'Checking that Nominal levels agree between signal masker and contralateral masker
        If CustomizableTestOptions.UseContralateralMasking = True Then If ContralateralNoise.SMA.NominalLevel <> NominalLevel_FS Then Throw New Exception("Nominal level is required to be the same between speech and contralateral noise files!")

        'Calculating presentation levels
        Dim TargetSpeechLevel_FS As Double = Audio.Standard_dBSPL_To_dBFS(DirectCast(CurrentTestTrial, WrsTrial).SpeechLevel) + RETSPL_Correction
        Dim NeededSpeechGain = TargetSpeechLevel_FS - NominalLevel_FS

        'Adjusts the sound levels
        Audio.DSP.AmplifySection(TestWordSound, NeededSpeechGain)

        If CustomizableTestOptions.UseContralateralMasking = True Then

            'Setting level, 
            'Very important: The contralateral masking sound file cannot be the same as the ipsilateral masker sound. The level of the contralateral masker sound must be set to agree with the Nominal level (while the ipsilateral masker sound sound have a level that deviates from the nominal level to attain the desired SNR!)
            Dim ContralateralMaskingNominalLevel_FS = ContralateralNoise.SMA.NominalLevel
            Dim TargetContralateralMaskingLevel_FS As Double = Audio.Standard_dBSPL_To_dBFS(DirectCast(CurrentTestTrial, WrsTrial).ContralateralMaskerLevel) + CustomizableTestOptions.SelectedMediaSet.EffectiveContralateralMaskingGain + RETSPL_Correction

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

        If CustomizableTestOptions.SignalLocations(0).HorizontalAzimuth < 0 Then
            'Left test ear
            'Adding speech and noise
            CurrentTestTrial.Sound.WaveData.SampleData(1) = TestSound.WaveData.SampleData(1)
            'Adding contralateral masking
            If CustomizableTestOptions.UseContralateralMasking = True Then
                CurrentTestTrial.Sound.WaveData.SampleData(2) = TrialContralateralNoise.WaveData.SampleData(1)
            End If

        Else
            'Right test ear
            'Adding speech and noise
            CurrentTestTrial.Sound.WaveData.SampleData(2) = TestSound.WaveData.SampleData(1)
            'Adding contralateral masking
            If CustomizableTestOptions.UseContralateralMasking = True Then
                CurrentTestTrial.Sound.WaveData.SampleData(1) = TrialContralateralNoise.WaveData.SampleData(1)
            End If
        End If

        'Also stores the test stage
        CurrentTestTrial.TestStage = TestStage

        'Also stores the mediaset
        CurrentTestTrial.MediaSetName = CustomizableTestOptions.SelectedMediaSet.MediaSetName

        'And the contralateral noise on/off setting
        CurrentTestTrial.UseContralateralNoise = CustomizableTestOptions.UseContralateralMasking

        'And the EM term
        CurrentTestTrial.EfficientContralateralMaskingTerm = CustomizableTestOptions.SelectedMediaSet.EffectiveContralateralMaskingGain

        'And the experiment number
        CurrentTestTrial.ExperimentNumber = CustomizableTestOptions.ExperimentNumber

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
                Output.Add("Lista " & TestStageIndex + 1 & ": Resultat = " & System.Math.Round(100 * ScoreList.Average) & " % korrekt (" & ScoreList.Sum & " / " & ObservedTestData(TestStageIndex).Count & ")")
            End If
        Next

        Return String.Join(vbCrLf, Output)

    End Function

    Public Overrides Function GetExportString() As String

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

    Private Function GetPlannedTrialsExportString(Optional ByVal ID As String = "") As String

        Dim ExportStringList As New List(Of String)

        Dim TestTrialIndex As Integer = 0
        For TestStageIndex = 0 To PlannedTestData.Count - 1
            For Each Trial As WrsTrial In PlannedTestData(TestStageIndex)

                If TestTrialIndex = 0 Then
                    ExportStringList.Add("ID" & vbTab & "TrialIndex" & vbTab & Trial.TestResultColumnHeadings)
                End If
                ExportStringList.Add(ID & vbTab & TestTrialIndex & vbTab & Trial.TestResultAsTextRow)
                TestTrialIndex += 1

            Next
        Next

        Return String.Join(vbCrLf, ExportStringList)

    End Function


    Public Overrides Function CreatePreTestStimulus() As Tuple(Of Sound, String)

        'No pre-test stimulus are available
        Return Nothing

    End Function

End Class
