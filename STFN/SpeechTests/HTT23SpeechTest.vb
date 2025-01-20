Imports STFN.TestProtocol

Public Class HTT23SpeechTest
    Inherits SpeechAudiometryTest

    Public Overrides ReadOnly Property FilePathRepresentation As String = "HTT23"

    Public Sub New(SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)
        ApplyTestSpecificSettings()
    End Sub

    Public Shadows Sub ApplyTestSpecificSettings()

        TesterInstructions = "--  Hörtröskel för tal (HTT) med spondéer  --" & vbCrLf & vbCrLf &
            "1. Välj startlista." & vbCrLf &
            "2. Välj mediaset." & vbCrLf &
            "3. Välj testöra." & vbCrLf &
             "4. Ställ talnivå till TMV3 + 20 dB, eller maximalt " & MaximumLevel_Targets & " dB HL." & vbCrLf &
             "5. Aktivera kontralateralt brus och ställ in brusnivå enligt normal klinisk praxis (OBS. Ha det aktiverat även om brusnivån är väldigt låg. Det går inte aktivera mitt under testet, ifall det skulle behövas.)." & vbCrLf &
             "6. Använd kontrollen provlyssna för att presentera några ord, och kontrollera att deltagaren kan uppfatta dem. Höj talnivån om deltagaren inte kan uppfatta orden. (Dock maximalt till 80 dB HL)" & vbCrLf &
             "(Använd knappen TB för att prata med deltagaren när denna har lurar på sig.)" & vbCrLf &
             "7. Klicka på start för att starta testet." & vbCrLf &
             "8. Rätta manuellt under testet genom att klicka på testorden som kommer upp på skärmen (nivåjusteringen sker automatiskt)"

        ParticipantInstructions = "--  Mätning av hörtröskel för tal (HTT)  --" & vbCrLf & vbCrLf &
             "Du ska lyssna efter tvåstaviga ord och efter varje ord upprepa det muntligt." & vbCrLf &
             "Gissa om du är osäker. " & vbCrLf &
             "Du har maximalt " & MaximumResponseTime & " sekunder på dig innan nästa ord kommer." & vbCrLf &
             "Testet är 20 ord långt."

        ParticipantInstructionsButtonText = "Deltagarinstruktion"

        ShowGuiChoice_StartList = True
        ShowGuiChoice_MediaSet = True
        SupportsPrelistening = True
        AvailableTestModes = New List(Of TestModes) From {TestModes.AdaptiveSpeech}
        AvailableTestProtocols = New List(Of TestProtocol) From {New SrtIso8253_TestProtocol}
        AvailableFixedResponseAlternativeCounts = New List(Of Integer) From {4}

        MaximumSoundFieldSpeechLocations = 1
        MaximumSoundFieldMaskerLocations = 0

        MinimumSoundFieldSpeechLocations = 1
        MinimumSoundFieldMaskerLocations = 0

        ShowGuiChoice_WithinListRandomization = False
        WithinListRandomization = True

        ShowGuiChoice_FreeRecall = False
        IsFreeRecall = True

        TargetLevel_StepSize = 1

        SupportsManualPausing = True

        TargetLevel = 60
        ContralateralMaskingLevel = 20

        MinimumLevel_Targets = -20
        MaximumLevel_Targets = 90
        MinimumLevel_ContralateralMaskers = -20
        MaximumLevel_ContralateralMaskers = 90

        SoundOverlapDuration = 0.1

        ShowGuiChoice_TargetLocations = True

        GuiResultType = GuiResultTypes.VisualResults

    End Sub


    Public Overrides ReadOnly Property ShowGuiChoice_TargetLevel As Boolean = True

    Public Overrides ReadOnly Property ShowGuiChoice_MaskingLevel As Boolean = False

    Public Overrides ReadOnly Property ShowGuiChoice_BackgroundLevel As Boolean = False


    Private PlannedTestWords As List(Of SpeechMaterialComponent)
    Private PlannedFamiliarizationWords As List(Of SpeechMaterialComponent)

    Private MaximumSoundDuration As Double = 10
    Private TestWordPresentationTime As Double = 0.5
    Private MaximumResponseTime As Double = 5

    Private ResultSummaryForGUI As New List(Of String)
    Private PreTestWordIndex As Integer = 0


    Public Overrides Function InitializeCurrentTest() As Tuple(Of Boolean, String)

        'Checking/updating things that may have changed since initial initalization on every call
        If SignalLocations.Count = 0 Then
            Return New Tuple(Of Boolean, String)(False, "You must select at least one signal sound source!")
        End If

        'Ensuring that contralateral masking level is always used with contralateral masking
        LockContralateralMaskingLevelToSpeechLevel = ContralateralMasking

        If IsInitialized = True Then Return New Tuple(Of Boolean, String)(True, "")

        CreatePlannedWordsList()

        TestProtocol.InitializeProtocol(New TestProtocol.NextTaskInstruction With {.AdaptiveValue = TargetLevel, .TestStage = 0})

        IsInitialized = True

        Return New Tuple(Of Boolean, String)(True, "")

    End Function


    Private Function CreatePlannedWordsList() As Boolean

        'Adding NumberOfWordsToAdd words, starting from the start list (excluding practise items)
        Dim AllLists = SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List, True, False)

        'Determines the index of the start list
        Dim SelectedStartListIndex As Integer = -1
        For i = 0 To AllLists.Count - 1
            If AllLists(i).PrimaryStringRepresentation = StartList Then
                SelectedStartListIndex = i
                Exit For
            End If
        Next

        If SelectedStartListIndex = -1 Then
            Messager.MsgBox("Unable to find the selected start list",, "An error occurred!")
            Return False
        End If

        Dim CurrentListSMC As SpeechMaterialComponent = AllLists(SelectedStartListIndex)

        'Adding the planned test words
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

        'Locks the list selection control, as it is not possible to swap list after this point
        ListSelectionControlIsEnabled = False

        Return True

    End Function


    Public Overrides Function GetSpeechTestReply(sender As Object, e As SpeechTestInputEventArgs) As SpeechTestReplies

        Dim ProtocolReply As NextTaskInstruction = Nothing

        If e IsNot Nothing Then

            'This is an incoming test trial response

            'Corrects the trial response, based on the given response
            Dim WordsInSentence = CurrentTestTrial.SpeechMaterialComponent.ChildComponents()

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

            'Calculating the speech level
            ProtocolReply = TestProtocol.NewResponse(ObservedTrials)

            'Taking a dump of the SpeechTest before swapping to the new trial, but after the protocol reply so that the protocol results also gets dumped
            CurrentTestTrial.SpeechTestPropertyDump = Utils.Logging.ListObjectPropertyValues(Me.GetType, Me)

        Else
            'Nothing to correct (this should be the start of a new test)

            'Calculating the speech level (of the first trial)
            ProtocolReply = TestProtocol.NewResponse(ObservedTrials)

        End If


        'Preparing next trial if needed
        If ProtocolReply.Decision = SpeechTestReplies.GotoNextTrial Then
            PrepareNextTrial(ProtocolReply)

            'Here we abort the test if any of the levels had to be adjusted above MaximumLevel dB HL
            If TargetLevel > MaximumLevel_Targets Or
                ContralateralMaskingLevel > MaximumLevel_ContralateralMaskers Then

                'And informing the participant
                ProtocolReply.Decision = SpeechTestReplies.AbortTest
                AbortInformation = "Testet har avbrutits på grund av höga ljudnivåer."

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
        If ContralateralMasking = True Then
            CurrentContralateralMaskingLevel = NextTaskInstruction.AdaptiveValue + ContralateralLevelDifference()
        End If

        'Adjusting levels
        TargetLevel = NextTaskInstruction.AdaptiveValue
        'CurrentContralateralMaskingLevel = CurrentContralateralMaskingLevel

        CurrentTestTrial = New TestTrial With {.SpeechMaterialComponent = NextTestWord,
                    .AdaptiveProtocolValue = NextTaskInstruction.AdaptiveValue,
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
        'MixNextTrialSound()

        MixStandardTestTrialSound(UseNominalLevels:=True,
                                  MaximumSoundDuration:=MaximumSoundDuration,
                                  TargetPresentationTime:=TestWordPresentationTime,
                                  ExportSounds:=False)

        'Setting trial events
        CurrentTestTrial.TrialEventList = New List(Of ResponseViewEvent)
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 1, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = System.Math.Max(1, 1000 * TestWordPresentationTime), .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = System.Math.Max(1, 1000 * (TestWordPresentationTime + MaximumResponseTime)), .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut})

    End Sub



    'Private Sub MixNextTrialSound()

    '    Dim RETSPL_Correction As Double = 0
    '    If LevelsAreIn_dBHL = True Then
    '        RETSPL_Correction = Transducer.RETSPL_Speech
    '    End If

    '    'Getting the speech signal
    '    Dim TestWordSound = CurrentTestTrial.SpeechMaterialComponent.GetSound(MediaSet, 0, 1, , , , , False, False, False, , , False)
    '    Dim NominalLevel_FS = TestWordSound.SMA.NominalLevel

    '    'Storing the LinguisticSoundStimulusStartTime and the LinguisticSoundStimulusDuration (assuming that the linguistic recording is in channel 1)
    '    CurrentTestTrial.LinguisticSoundStimulusStartTime = TestWordPresentationTime
    '    CurrentTestTrial.LinguisticSoundStimulusDuration = TestWordSound.WaveData.SampleData(1).Length / TestWordSound.WaveFormat.SampleRate
    '    CurrentTestTrial.MaximumResponseTime = MaximumResponseTime

    '    'Creating a silent sound (lazy method to get the same length independently of contralateral masking or not)
    '    Dim SilentSound = Audio.GenerateSound.CreateSilence(ContralateralNoise.WaveFormat, 1, MaximumSoundDuration)

    '    'Creating contalateral masking noise (with the same length as the masking noise)
    '    Dim TrialContralateralNoise As Audio.Sound = Nothing
    '    Dim IntendedNoiseLength As Integer
    '    If ContralateralMasking = True Then
    '        Dim TotalSoundLength = ContralateralNoise.WaveData.SampleData(1).Length
    '        IntendedNoiseLength = ContralateralNoise.WaveFormat.SampleRate * MaximumSoundDuration
    '        Dim RandomStartReadSample = Randomizer.Next(0, TotalSoundLength - IntendedNoiseLength)
    '        TrialContralateralNoise = ContralateralNoise.CopySection(1, RandomStartReadSample - 1, IntendedNoiseLength) ' TODO: Here we should check to ensure that the MaskerNoise is long enough
    '    End If

    '    'Checking that Nominal levels agree between signal masker and contralateral masker
    '    If ContralateralMasking = True Then If ContralateralNoise.SMA.NominalLevel <> NominalLevel_FS Then Throw New Exception("Nominal level is required to be the same between speech and contralateral noise files!")

    '    'Calculating presentation levels
    '    Dim TargetSpeechLevel_FS As Double = Audio.Standard_dBSPL_To_dBFS(TargetLevel) + RETSPL_Correction
    '    Dim NeededSpeechGain = TargetSpeechLevel_FS - NominalLevel_FS

    '    'Adjusts the sound levels
    '    Audio.DSP.AmplifySection(TestWordSound, NeededSpeechGain)

    '    If ContralateralMasking = True Then

    '        'Setting level, 
    '        'Very important: The contralateral masking sound file cannot be the same as the ipsilateral masker sound. The level of the contralateral masker sound must be set to agree with the Nominal level (while the ipsilateral masker sound sound have a level that deviates from the nominal level to attain the desired SNR!)
    '        Dim ContralateralMaskingNominalLevel_FS = ContralateralNoise.SMA.NominalLevel
    '        Dim TargetContralateralMaskingLevel_FS As Double = Audio.Standard_dBSPL_To_dBFS(ContralateralMaskingLevel) + MediaSet.EffectiveContralateralMaskingGain + RETSPL_Correction

    '        'Calculating the needed gain, also adding the EffectiveContralateralMaskingGain specified in the SelectedMediaSet
    '        Dim NeededContraLateralMaskerGain = TargetContralateralMaskingLevel_FS - ContralateralMaskingNominalLevel_FS
    '        Audio.DSP.AmplifySection(TrialContralateralNoise, NeededContraLateralMaskerGain)

    '    End If

    '    'Mixing speech and noise
    '    Dim TestWordInsertionSample As Integer = TestWordSound.WaveFormat.SampleRate * TestWordPresentationTime
    '    Dim Silence = Audio.GenerateSound.CreateSilence(SilentSound.WaveFormat, 1, TestWordInsertionSample, Audio.BasicAudioEnums.TimeUnits.samples)
    '    Audio.DSP.InsertSoundAt(TestWordSound, Silence, 0)
    '    TestWordSound.ZeroPad(IntendedNoiseLength)
    '    Dim TestSound = Audio.DSP.SuperpositionSounds({TestWordSound, SilentSound}.ToList)

    '    'Creating an output sound
    '    CurrentTestTrial.Sound = New Audio.Sound(New Audio.Formats.WaveFormat(TestWordSound.WaveFormat.SampleRate, TestWordSound.WaveFormat.BitDepth, 2,, TestWordSound.WaveFormat.Encoding))

    '    If SignalLocations(0).HorizontalAzimuth < 0 Then
    '        'Left test ear
    '        'Adding speech and noise
    '        CurrentTestTrial.Sound.WaveData.SampleData(1) = TestSound.WaveData.SampleData(1)
    '        'Adding contralateral masking
    '        If ContralateralMasking = True Then
    '            CurrentTestTrial.Sound.WaveData.SampleData(2) = TrialContralateralNoise.WaveData.SampleData(1)
    '        End If

    '    Else
    '        'Right test ear
    '        'Adding speech and noise
    '        CurrentTestTrial.Sound.WaveData.SampleData(2) = TestSound.WaveData.SampleData(1)
    '        'Adding contralateral masking
    '        If ContralateralMasking = True Then
    '            CurrentTestTrial.Sound.WaveData.SampleData(1) = TrialContralateralNoise.WaveData.SampleData(1)
    '        End If
    '    End If

    '    'Stores the test ear (added nov 2024)
    '    Select Case SignalLocations(0).HorizontalAzimuth
    '        Case -90
    '            CurrentTestTrial.TestEar = Utils.Constants.SidesWithBoth.Left
    '        Case 90
    '            CurrentTestTrial.TestEar = Utils.Constants.SidesWithBoth.Right
    '        Case Else
    '            Throw New Exception("Unsupported signal azimuth: " & SignalLocations(0).HorizontalAzimuth)
    '    End Select

    '    'Also stores the mediaset
    '    CurrentTestTrial.MediaSetName = MediaSet.MediaSetName

    '    'And the EM term
    '    CurrentTestTrial.EfficientContralateralMaskingTerm = MediaSet.EffectiveContralateralMaskingGain

    'End Sub



    Public Overrides Function GetResultStringForGui() As String

        Dim ProtocolThreshold = TestProtocol.GetFinalResultValue()

        Dim Output As New List(Of String)

        If ProtocolThreshold IsNot Nothing Then

            ResultSummaryForGUI.Add("HTT = " & vbTab & Math.Round(ProtocolThreshold.Value) & " " & dBString())
            Output.AddRange(ResultSummaryForGUI)

        Else
            Output.Add("Testord nummer " & ObservedTrials.Count & " av " & PlannedTestWords.Count)
            If CurrentTestTrial IsNot Nothing Then
                Output.Add("Talnivå = " & Math.Round(TargetLevel) & " " & dBString())
                Output.Add("Kontralateral brusnivå = " & Math.Round(ContralateralMaskingLevel) & " " & dBString())
            End If
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


    Public Overrides Sub FinalizeTestAheadOfTime()

        TestProtocol.AbortAheadOfTime(ObservedTrials)

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

        CurrentTestTrial = New TestTrial With {.SpeechMaterialComponent = NextTestWord}

        'Mixing the test sound
        MixStandardTestTrialSound(UseNominalLevels:=True,
                                  MaximumSoundDuration:=MaximumSoundDuration,
                                  TargetPresentationTime:=TestWordPresentationTime,
                                  ExportSounds:=False)

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

    Public Overrides Function GetProgress() As Utils.ProgressInfo

        If GetTotalTrialCount() <> -1 Then
            Dim NewProgressInfo As New Utils.ProgressInfo
            NewProgressInfo.Value = GetObservedTestTrials.Count
            NewProgressInfo.Maximum = GetTotalTrialCount()
            Return NewProgressInfo
        End If

        Return Nothing

    End Function

    Public Overrides Function GetSubGroupResults() As List(Of Tuple(Of String, Double))
        Return Nothing
    End Function

    Public Overrides Function GetTotalTrialCount() As Integer
        If TestProtocol Is Nothing Then Return -1
        Return TestProtocol.TotalTrialCount
    End Function

    Public Overrides Function GetScorePerLevel() As Tuple(Of String, SortedList(Of Double, Double))
        Return Nothing
    End Function


End Class
