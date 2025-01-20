Imports System.IO
Imports MathNet.Numerics
Imports STFN.TestProtocol
Imports STFN.Utils

Public Class TP50SpeechTest
    Inherits SpeechAudiometryTest

    Public Sub New(SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)
        ApplyTestSpecificSettings()
    End Sub

    Public Overrides ReadOnly Property FilePathRepresentation As String = "TP50"

    Public Shadows Sub ApplyTestSpecificSettings()

        TesterInstructions = "--  Taluppfattningspoäng (TP) med enstaviga ord  --" & vbCrLf & vbCrLf &
            "1. Välj testlista." & vbCrLf &
            "2. Välj mediaset." & vbCrLf &
            "3. Välj testöra." & vbCrLf &
             "4. Ställ talnivå till TMV3 + 20 dB, eller maximalt " & MaximumLevel_Targets & " dB HL." & vbCrLf &
             "5. Om kontralateralt brus behövs, akivera kontralateralt brus och ställ in brusnivå enligt normal klinisk praxis." & vbCrLf &
             "6. Använd kontrollen provlyssna för att presentera några ord, och kontrollera att deltagaren kan uppfatta dem. Höj talnivån om deltagaren inte kan uppfatta orden. (Dock maximalt till 80 dB HL)" & vbCrLf &
             "(Använd knappen TB för att prata med deltagaren när denna har lurar på sig.)" & vbCrLf &
             "7. Om du låtit deltagaren provlyssna, välj en ny testlista (annars presenteras samma igen)." & vbCrLf &
             "8. Klicka på start för att starta testet." & vbCrLf &
            "9. Rätta manuellt under testet genom att klicka på testorden som kommer upp på skärmen"

        ParticipantInstructions = "--  Taluppfattningspoäng (TP) med enstaviga ord  --" & vbCrLf & vbCrLf &
             "Du ska lyssna efter enstaviga ord och efter varje ord upprepa det muntligt." & vbCrLf &
             "Gissa om du är osäker." & vbCrLf &
             "Du har maximalt " & TestWordPresentationTime + MaximumResponseTime & " sekunder på dig innan nästa ord kommer." & vbCrLf &
             "Testet är 50 ord långt."

        ParticipantInstructionsButtonText = "Deltagarinstruktion"


        ShowGuiChoice_TargetLocations = True

        ShowGuiChoice_PreSet = False
        ShowGuiChoice_StartList = True
        ShowGuiChoice_MediaSet = True
        SupportsPrelistening = True
        AvailableTestModes = New List(Of TestModes) From {TestModes.ConstantStimuli}
        AvailableTestProtocols = New List(Of TestProtocol) From {New FixedLengthWordsInNoise_WithPreTestLevelAdjustment_TestProtocol}
        AvailableFixedResponseAlternativeCounts = New List(Of Integer) From {4}

        MaximumSoundFieldSpeechLocations = 1
        MaximumSoundFieldMaskerLocations = 1

        MinimumSoundFieldSpeechLocations = 1
        MinimumSoundFieldMaskerLocations = 0

        'ShowGuiChoice_WithinListRandomization = True
        WithinListRandomization = True

        'ShowGuiChoice_FreeRecall = True
        IsFreeRecall = True

        HistoricTrialCount = 3
        SupportsManualPausing = True

        TargetLevel = 65
        ContralateralMaskingLevel = 25
        TargetSNR_StepSize = 1
        ContralateralMaskingLevel_StepSize = 1
        MaskingLevel_StepSize = 1

        MinimumLevel_Targets = -10
        MaximumLevel_Targets = 90
        MinimumLevel_Maskers = -10
        MaximumLevel_Maskers = 90
        MinimumLevel_ContralateralMaskers = -10
        MaximumLevel_ContralateralMaskers = 90

        SoundOverlapDuration = 0.5

        'Setting default SNR. This could ideally be read from the selected MediaSet!
        TargetSNR = 0

    End Sub


    Public Overrides ReadOnly Property ShowGuiChoice_TargetSNRLevel As Boolean = False
    Public Overrides ReadOnly Property ShowGuiChoice_TargetLevel As Boolean = True
    Public Overrides ReadOnly Property ShowGuiChoice_MaskingLevel As Boolean = False
    Public Overrides ReadOnly Property ShowGuiChoice_BackgroundLevel As Boolean = False


    Private PlannedTestTrials As New TestTrialCollection

    Private TestWordPresentationTime As Double = 0.5
    Private MaximumResponseTime As Double = 4.5
    Private TestLength As Integer
    Private MaximumSoundDuration As Double = 10

    Private PreTestWordIndex As Integer = 0



    Public Overrides Function InitializeCurrentTest() As Tuple(Of Boolean, String)

        'Checking/updating things that may have changed since initial initalization on every call
        If SignalLocations.Count = 0 Then
            Return New Tuple(Of Boolean, String)(False, "You must select at least one signal sound source!")
        End If

        'Ensuring that contralateral masking level is always used with contralateral masking
        LockContralateralMaskingLevelToSpeechLevel = ContralateralMasking

        If IsInitialized = True Then Return New Tuple(Of Boolean, String)(True, "")

        'Copying the target location to the masker location
        SelectSameMaskersAsTargetSoundSources()

        CreatePlannedWordsList()

        TestProtocol.InitializeProtocol(New TestProtocol.NextTaskInstruction With {.TestLength = TestLength})

        IsInitialized = True

        Return New Tuple(Of Boolean, String)(True, "")

    End Function


    Private Function CreatePlannedWordsList() As Boolean

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

        Dim PlannedTestListWords As List(Of SpeechMaterialComponent) = AllLists(SelectedStartListIndex).GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.Sentence)

        If PlannedTestListWords.Count = 0 Then
            Messager.MsgBox("Unable to find the test words!", MsgBoxStyle.Exclamation, "An error occurred!")
            Return False
        End If

        PlannedTestTrials = New TestTrialCollection

        For i = 0 To PlannedTestListWords.Count - 1

            Dim CurrentSMC = PlannedTestListWords(i)

            Dim NewTestTrial = New TestTrial With {.SpeechMaterialComponent = CurrentSMC,
                .Tasks = 1,
                .ResponseAlternativeSpellings = New List(Of List(Of SpeechTestResponseAlternative))}

            Dim ResponseAlternatives As New List(Of SpeechTestResponseAlternative)

            If NewTestTrial.SpeechMaterialComponent.ChildComponents.Count > 0 Then
                For Each Child In NewTestTrial.SpeechMaterialComponent.ChildComponents()
                    ResponseAlternatives.Add(New SpeechTestResponseAlternative With {.Spelling = Child.GetCategoricalVariableValue("Spelling"), .IsScoredItem = True, .TrialPresentationIndex = i})
                Next
            End If

            NewTestTrial.ResponseAlternativeSpellings.Add(ResponseAlternatives)

            PlannedTestTrials.Add(NewTestTrial)

        Next

        If WithinListRandomization = True Then
            PlannedTestTrials.Shuffle(Randomizer)
        End If

        'After shuffling we must reassign trial index
        For i = 0 To PlannedTestTrials.Count - 1
            If PlannedTestTrials(i).SpeechMaterialComponent.ChildComponents.Count > 0 Then
                For Each Child In PlannedTestTrials(i).SpeechMaterialComponent.ChildComponents()
                    PlannedTestTrials(i).ResponseAlternativeSpellings(0)(0).TrialPresentationIndex = i

                    Dim HistoricTrialsToAdd As Integer = System.Math.Min(HistoricTrialCount, i)

                    'Adding historic trials
                    For index = 1 To HistoricTrialsToAdd

                        Dim CurrentHistoricTrialIndex = i - index
                        Dim HistoricTrial = PlannedTestTrials(CurrentHistoricTrialIndex)

                        'We only add the spelling of first child component here, since displaying history is only supported for sigle words
                        Dim HistoricSpeechTestResponseAlternative = New SpeechTestResponseAlternative With {
                            .Spelling = HistoricTrial.SpeechMaterialComponent.ChildComponents(0).GetCategoricalVariableValue("Spelling"),
                            .IsScoredItem = True,
                            .TrialPresentationIndex = CurrentHistoricTrialIndex,
                            .ParentTestTrial = HistoricTrial}

                        'We insert the history
                        PlannedTestTrials(i).ResponseAlternativeSpellings(0).Insert(0, HistoricSpeechTestResponseAlternative) 'We put it into the first index as this is not multidimensional response alternatives (such as in Matrix tests)
                    Next

                Next
            End If
        Next


        'Setting TestLength to the number of available words
        TestLength = PlannedTestTrials.Count

        Return True

    End Function


    Public Overrides Sub UpdateHistoricTrialResults(sender As Object, e As SpeechTestInputEventArgs)

        'Stores historic responses
        For Index = 0 To e.LinguisticResponses.Count - 1

            'Determining which trials that should be modified

            Dim CurrentTrialIndex = CurrentTestTrial.ResponseAlternativeSpellings(0).Last.TrialPresentationIndex
            Dim CurrentHistoricTrialIndex = CurrentTrialIndex - HistoricTrialCount + Index

            If CurrentHistoricTrialIndex < 0 Then Continue For 'This means that the index refers to an invisible response button, before the first test trial 

            Dim HistoricTrial = ObservedTrials(CurrentHistoricTrialIndex)

            HistoricTrial.ScoreList = New List(Of Integer)
            If e.LinguisticResponses(Index) = HistoricTrial.ResponseAlternativeSpellings(0).Last.Spelling Then
                HistoricTrial.ScoreList.Add(1)
            Else
                HistoricTrial.ScoreList.Add(0)
            End If

            If HistoricTrial.ScoreList.Sum > 0 Then
                HistoricTrial.IsCorrect = True
            Else
                HistoricTrial.IsCorrect = False
            End If

        Next

    End Sub


    Public Overrides Function GetSpeechTestReply(sender As Object, e As SpeechTestInputEventArgs) As SpeechTestReplies

        Dim ProtocolReply As NextTaskInstruction = Nothing

        If e IsNot Nothing Then

            'This is an incoming test trial response
            'Corrects the trial response, based on the given response
            Dim WordsInSentence = CurrentTestTrial.SpeechMaterialComponent.ChildComponents()

            'Resets the CurrentTestTrial.ScoreList
            CurrentTestTrial.ScoreList = New List(Of Integer)
            For i = 0 To e.LinguisticResponses.Count - 1
                If e.LinguisticResponses(i) = CurrentTestTrial.ResponseAlternativeSpellings(0).Last.Spelling Then
                    CurrentTestTrial.ScoreList.Add(1)
                Else
                    CurrentTestTrial.ScoreList.Add(0)
                End If
            Next

            If CurrentTestTrial.ScoreList.Sum > 0 Then
                CurrentTestTrial.IsCorrect = True
            Else
                CurrentTestTrial.IsCorrect = False
            End If

            'Checks if the trial is finished
            If CurrentTestTrial.ScoreList.Count < CurrentTestTrial.Tasks Then
                'Returns to continue the trial
                Return SpeechTestReplies.ContinueTrial
            End If

            'Adding the test trial
            ObservedTrials.Add(CurrentTestTrial)

            'TODO: We must store the responses and response times!!!

            'Taking a dump of the SpeechTest before swapping to the new trial, but after the protocol reply so that the protocol results also gets dumped
            CurrentTestTrial.SpeechTestPropertyDump = Utils.Logging.ListObjectPropertyValues(Me.GetType, Me)

            'Calculating the speech level
            ProtocolReply = TestProtocol.NewResponse(ObservedTrials)

        Else
            'Nothing to correct (this should be the start of a new test, or a resuming of a paused test)
            ProtocolReply = TestProtocol.NewResponse(New TestTrialCollection)
        End If


        'Preparing next trial if needed
        If ProtocolReply.Decision = SpeechTestReplies.GotoNextTrial Then

            'Preparing the next trial
            'Getting next test word
            CurrentTestTrial = PlannedTestTrials(ObservedTrials.Count)

            'Creating a new test trial
            'CurrentTestTrial.SpeechLevel = TargetLevel
            'CurrentTestTrial.ContralateralMaskerLevel = ContralateralMaskingLevel
            CurrentTestTrial.Tasks = 1

            'Adjusting levels
            'TargetLevel = TargetLevel ' Should be already set
            'ContralateralMaskingLevel = ContralateralMaskingLevel ' Should be already set
            MaskingLevel = TargetLevel - TargetSNR ' Acctually this only needs to be set the first time, but can be also recalculated...

            'Setting trial events
            CurrentTestTrial.TrialEventList = New List(Of ResponseViewEvent)
            CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 1, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})
            CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 2, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})
            CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = System.Math.Max(1, 1000 * (TestWordPresentationTime + MaximumResponseTime)), .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut})

            'Setting masker level, based on the selected TargetSNR (Not that the limitations in MaskingLevel may cause a mismatch between the the intended TargetSNR and the actual SNR (CurrentSNR)
            MaskingLevel = TargetLevel - TargetSNR

            'Mixing trial sound
            'MixNextTrialSound()

            'Mixing trial sound
            MixStandardTestTrialSound(UseNominalLevels:=True, MaximumSoundDuration:=MaximumSoundDuration,
                                      TargetPresentationTime:=TestWordPresentationTime,
                                      ExportSounds:=False)

        End If

        Return ProtocolReply.Decision

    End Function


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

    '    'Getting a random section of the noise
    '    Dim TotalNoiseLength As Integer = MaskerNoise.WaveData.SampleData(1).Length
    '    Dim IntendedNoiseLength As Integer = MaskerNoise.WaveFormat.SampleRate * MaximumSoundDuration
    '    Dim RandomStartReadSample As Integer = Randomizer.Next(0, TotalNoiseLength - IntendedNoiseLength)
    '    Dim TrialNoise = MaskerNoise.CopySection(1, RandomStartReadSample - 1, IntendedNoiseLength) ' TODO: Here we should check to ensure that the MaskerNoise is long enough

    '    'Creating contalateral masking noise (with the same length as the masking noise)
    '    Dim TrialContralateralNoise As Audio.Sound = Nothing
    '    If ContralateralMasking = True Then
    '        TotalNoiseLength = ContralateralNoise.WaveData.SampleData(1).Length
    '        IntendedNoiseLength = ContralateralNoise.WaveFormat.SampleRate * MaximumSoundDuration
    '        RandomStartReadSample = Randomizer.Next(0, TotalNoiseLength - IntendedNoiseLength)
    '        TrialContralateralNoise = ContralateralNoise.CopySection(1, RandomStartReadSample - 1, IntendedNoiseLength) ' TODO: Here we should check to ensure that the MaskerNoise is long enough
    '    End If

    '    'Checking that Nominal levels agree between signal masker and contralateral masker
    '    If MaskerNoise.SMA.NominalLevel <> NominalLevel_FS Then Throw New Exception("Nominal level is required to be the same between speech and noise files!")
    '    If ContralateralMasking = True Then If ContralateralNoise.SMA.NominalLevel <> NominalLevel_FS Then Throw New Exception("Nominal level is required to be the same between speech and contralateral noise files!")

    '    'Calculating presentation levels
    '    Dim TargetSpeechLevel_FS As Double = Audio.Standard_dBSPL_To_dBFS(TargetLevel) + RETSPL_Correction
    '    Dim NeededSpeechGain = TargetSpeechLevel_FS - NominalLevel_FS

    '    'Here we're using the Speech level to set the Masker level. This means that the level of the masker file it self need to reflect the SNR, so that its mean level does not equal the nominal level, but instead deviated from the nominal level by the intended SNR. (These sound files (the Speech and the Masker) can then be mixed without any adjustment to attain the desired "clicinally" used SNR.
    '    'Note from 2024-12-18: This situation has been changed after changing the level definitions in the speech files from average RMS-levels to time weighted (125ms) levels, and the calibration level from -25 to -21 dBFS. The SNR of 0 dB in the materials before the change approximately equals + 6 dB in the new materials, since the speech level dropped by appr 6 dB (more precicely 6,8 dB for Talker1 and 6.3 for Talker2).
    '    MaskingLevel = TargetLevel - TargetSNR
    '    Dim TargetMaskerLevel_FS As Double = Audio.Standard_dBSPL_To_dBFS(MaskingLevel) + RETSPL_Correction
    '    Dim NeededMaskerGain = TargetMaskerLevel_FS - NominalLevel_FS

    '    'Adjusts the sound levels
    '    Audio.DSP.AmplifySection(TestWordSound, NeededSpeechGain)
    '    Audio.DSP.AmplifySection(TrialNoise, NeededMaskerGain)

    '    If ContralateralMasking = True Then

    '        'Setting level, 
    '        Dim TargetContralateralMaskingLevel_FS As Double = Audio.Standard_dBSPL_To_dBFS(ContralateralMaskingLevel) + MediaSet.EffectiveContralateralMaskingGain + RETSPL_Correction

    '        'Calculating the needed gain, also adding the EffectiveContralateralMaskingGain specified in the SelectedMediaSet
    '        Dim NeededContraLateralMaskerGain = TargetContralateralMaskingLevel_FS - NominalLevel_FS
    '        Audio.DSP.AmplifySection(TrialContralateralNoise, NeededContraLateralMaskerGain)

    '    End If

    '    'Mixing speech and noise
    '    Dim TestWordInsertionSample As Integer = TestWordSound.WaveFormat.SampleRate * TestWordPresentationTime
    '    Dim Silence = Audio.GenerateSound.CreateSilence(TrialNoise.WaveFormat, 1, TestWordInsertionSample, Audio.BasicAudioEnums.TimeUnits.samples)
    '    Audio.DSP.InsertSoundAt(TestWordSound, Silence, 0)
    '    TestWordSound.ZeroPad(IntendedNoiseLength)
    '    Dim TestSound = Audio.DSP.SuperpositionSounds({TestWordSound, TrialNoise}.ToList)

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

    '    'Also stores the mediaset
    '    CurrentTestTrial.MediaSetName = MediaSet.MediaSetName

    '    'And the EM term
    '    CurrentTestTrial.EfficientContralateralMaskingTerm = MediaSet.EffectiveContralateralMaskingGain

    'End Sub

    Public Overrides Function GetResultStringForGui() As String

        Dim Output As New List(Of String)

        Dim ScoreList As New List(Of Double)
        For Each Trial As TestTrial In ObservedTrials
            If Trial.IsCorrect = True Then
                ScoreList.Add(1)
            Else
                ScoreList.Add(0)
            End If
        Next

        If ScoreList.Count > 0 Then
            Output.Add("Resultat = " & System.Math.Round(100 * ScoreList.Average) & " % korrekt (" & ScoreList.Sum & " / " & ObservedTrials.Count & ")")
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
        'Getting all words from the currently selected list
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
            Return Nothing
        End If

        Dim PlannedLevelAdjustmentWords As List(Of SpeechMaterialComponent) = AllLists(SelectedStartListIndex).GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.Sentence)

        If PlannedLevelAdjustmentWords.Count = 0 Then
            Messager.MsgBox("Unable to find any test words!", MsgBoxStyle.Exclamation, "An error occurred!")
            Return Nothing
        End If


        'Selecting pre-test word index
        If PreTestWordIndex >= PlannedLevelAdjustmentWords.Count - 1 Then
            'Resetting PreTestWordIndex, if all pre-testwords have been used
            PreTestWordIndex = 0
        End If

        'Getting the test word
        Dim NextTestWord = PlannedLevelAdjustmentWords(PreTestWordIndex)
        PreTestWordIndex += 1

        'Getting the spelling
        Dim TestWordSpelling = NextTestWord.GetCategoricalVariableValue("Spelling")

        'Creating a new pretest trial
        CurrentTestTrial = New TestTrial With {.SpeechMaterialComponent = NextTestWord}

        'Mixing the test sound
        'MixNextTrialSound()

        If SignalLocations.Count = 0 Then
            Messager.MsgBox("You must select at least one signal sound source!", MsgBoxStyle.Exclamation, "Select a signal location!")
            Return Nothing
        End If

        'Copying the target location to the masker location
        SelectSameMaskersAsTargetSoundSources()

        'Setting masker level, based on the selected TargetSNR (Not that the limitations in MaskingLevel may cause a mismatch between the the intended TargetSNR and the actual SNR (CurrentSNR)
        MaskingLevel = TargetLevel - TargetSNR

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