Imports System.IO
Imports MathNet.Numerics
Imports MathNet.Numerics.Distributions
Imports STFN.Audio
Imports STFN.TestProtocol
Imports STFN.Utils

Public Class IHearProtocolB3SpeechTest
    Inherits SpeechTest

    Public Sub New(SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)

    End Sub

    Public Overrides ReadOnly Property FilePathRepresentation As String
        Get
            Return "ProtocolB3"
        End Get
    End Property

#Region "Settings"

    Public Overrides ReadOnly Property TesterInstructions As String
        Get
            Return "1. Välj testöra." & vbCrLf &
                "2. Ställ talnivån till TMV3 + 20 dB (Talnivån är i dB SPL)." & vbCrLf &
                "3. Om kontrlateralt brus behövs, akivera kontralateralt brus och ställ in önskad brusnivå." & vbCrLf &
                "4. Vänd skärmen till patienten. Be sedan patienten klicka på start för att starta testet."
        End Get
    End Property

    Public Overrides ReadOnly Property ParticipantInstructions As String
        Get
            Return "Patientens uppgift: " & vbCrLf &
                "Patienten ska lyssna efter enstaviga ord och efter varje ord ange på skärmen vilket ord hen uppfattade. Patienten ska gissa om hen är osäker. Testet består av fyra 25 ordslistor som körs direkt efter varandra."
        End Get
    End Property

    Public Overrides ReadOnly Property HasOptionalPractiseTest As Boolean
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

    Public Overrides ReadOnly Property UpperLevelLimit_dBSPL As Double
        Get
            Return 100
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


    Public Overrides Sub UpdateHistoricTrialResults(sender As Object, e As SpeechTestInputEventArgs)
        'Ignores, not used
    End Sub

    Private PlannedTestData As New List(Of TrialHistory)
    Private ObservedTestData As New List(Of TrialHistory)

    Private MaximumSoundDuration As Double = 10
    Private TestWordPresentationTime As Double = 0.5
    Private MaximumResponseTime As Double = 5

    Private IsInitialized As Boolean = False
    Private TestStage As Integer = 0

    Private ContralateralNoise As Audio.Sound = Nothing
    Private SilentSound As Audio.Sound = Nothing


    Public Overrides Function InitializeCurrentTest() As Boolean

        If IsInitialized = True Then Return True

        TestStage = 0

        If PlanTrials() = False Then
            'Send message
            Return False
        End If

        IsInitialized = True

        Return True

    End Function

    Private Function PlanTrials()

        Dim AllMediaSets = AvailableMediasets

        'Select MediaSet / voice, Male only?
        For Each MediaSet In AllMediaSets
            If MediaSet.TalkerGender = MediaSet.Genders.Male Then
                CustomizableTestOptions.SelectedMediaSet = MediaSet
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
                NewTrial.SpeechLevel = CustomizableTestOptions.SpeechLevel
                NewTrial.ContralateralMaskerLevel = CustomizableTestOptions.ContralateralMaskingLevel

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
        ContralateralNoise = PlannedTestData(0)(0).SpeechMaterialComponent.GetContralateralMaskerSound(CustomizableTestOptions.SelectedMediaSet, 0)

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

        'Getting the speech signal
        Dim TestWordSound = CurrentTestTrial.SpeechMaterialComponent.GetSound(CustomizableTestOptions.SelectedMediaSet, 0, 1, , , , , False, False, False, , , False)
        Dim NominalLevel_FS = TestWordSound.SMA.NominalLevel

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
        Dim TargetSpeechLevel_FS As Double = Audio.Standard_dBSPL_To_dBFS(DirectCast(CurrentTestTrial, WrsTrial).SpeechLevel)
        Dim NeededSpeechGain = TargetSpeechLevel_FS - NominalLevel_FS

        'Adjusts the sound levels
        Audio.DSP.AmplifySection(TestWordSound, NeededSpeechGain)

        If CustomizableTestOptions.UseContralateralMasking = True Then

            'Setting level, 
            'Very important: The contralateral masking sound file cannot be the same as the ipsilateral masker sound. The level of the contralateral masker sound must be set to agree with the Nominal level (while the ipsilateral masker sound sound have a level that deviates from the nominal level to attain the desired SNR!)
            Dim ContralateralMaskingNominalLevel_FS = ContralateralNoise.SMA.NominalLevel
            Dim TargetContralateralMaskingLevel_FS As Double = Audio.Standard_dBSPL_To_dBFS(DirectCast(CurrentTestTrial, WrsTrial).ContralateralMaskerLevel)

            'Calculating the needed gain, also adding the EffectiveContralateralMaskingGain specified in the SelectedMediaSet
            Dim NeededContraLateralMaskerGain = TargetContralateralMaskingLevel_FS - ContralateralMaskingNominalLevel_FS + CustomizableTestOptions.SelectedMediaSet.EffectiveContralateralMaskingGain
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

    End Sub

    Public Overrides Sub FinalizeTest()
        'This test doesn't need to bi finalized

    End Sub

    Public Overrides Function GetResults() As TestResults

        Dim Output = New TestResults(TestResults.TestResultTypes.IHPB3)

        Output.TrialStringComment = New List(Of String)
        Output.SpeechLevelSeries = New List(Of Double)
        Output.ContralateralMaskerLevelSeries = New List(Of Double)
        Output.ScoreSeries = New List(Of String)

        Output.TestResultSummaryLines = New List(Of String)

        For TestStageIndex = 0 To ObservedTestData.Count - 1

            Dim ScoreList As New List(Of Double)

            For Each Trial As WrsTrial In ObservedTestData(TestStageIndex)

                ScoreList.Add(DirectCast(Trial, WrsTrial).GetProportionTasksCorrect)

                Output.TrialStringComment.Add(Trial.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling"))
                Output.SpeechLevelSeries.Add(System.Math.Round(Trial.SpeechLevel))
                Output.ContralateralMaskerLevelSeries.Add(System.Math.Round(Trial.ContralateralMaskerLevel))
                If Trial.IsCorrect = True Then
                    Output.ScoreSeries.Add("1")
                    ScoreList.Add(1)
                Else
                    Output.ScoreSeries.Add("0")
                    ScoreList.Add(0)
                End If

            Next

            If ScoreList.Count > 0 Then
                Output.TestResultSummaryLines.Add("List " & TestStageIndex & " Score: " & ScoreList.Average)
            End If

        Next

        Return Output


    End Function

    Public Overrides Function CreatePreTestStimulus() As Tuple(Of Sound, String)

        'No pre-test stimulus are available
        Return Nothing

    End Function

End Class
