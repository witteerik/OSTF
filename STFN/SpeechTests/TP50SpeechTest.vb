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

    Public Overrides ReadOnly Property FilePathRepresentation As String = "ProtocolB2_ManualWRS"

    Public Shadows Sub ApplyTestSpecificSettings()

        TesterInstructions = "(Detta test går ut på att undersöka svårighetsgraden hos listor med nya enstaviga testord i brus.)" & vbCrLf & vbCrLf &
            "1. Välj testöra." & vbCrLf &
            "2. Ställ talnivå till TMV3 + 20 dB, eller maximalt " & MaximumLevel_Targets & " dB HL." & vbCrLf &
            "3. Om kontralateralt brus behövs, akivera kontralateralt brus och ställ in brusnivå enligt normal klinisk praxis." & vbCrLf &
            "4. Använd kontrollen provlyssna för att ställa in 'Lagom-nivån' innan testet börjar. (Använd knappen TB för att prata med patienten när denna har lurar på sig.)" & vbCrLf &
            "5. Klicka på start för att starta testet." & vbCrLf &
            "6. Rätta manuellt under testet genom att klicka på testorden som kommer upp på skärmen" & vbCrLf &
            "7. Patienten har maximalt " & TestWordPresentationTime + MaximumResponseTime & " sekunder på sig innan nästa ord kommer."

        ParticipantInstructions = "Patientens uppgift: " & vbCrLf & vbCrLf &
            " - Under testet ska patienten lyssna efter enstaviga ord i brus och efter varje ord repetera ordet muntligt." & vbCrLf &
            " - Patienten ska gissa om hen är osäker." & vbCrLf &
            " - Patienten har maximalt " & TestWordPresentationTime + MaximumResponseTime & " sekunder på sig innan nästa ord kommer." & vbCrLf &
            " - Testet är 50 ord långt."

        ShowGuiChoice_PractiseTest = False
        ShowGuiChoice_dBHL = False
        ShowGuiChoice_PreSet = False
        ShowGuiChoice_StartList = False
        ShowGuiChoice_MediaSet = False
        SupportsPrelistening = True
        ShowGuiChoice_SoundFieldSimulation = False
        AvailableTestModes = New List(Of TestModes) From {TestModes.ConstantStimuli}
        AvailableTestProtocols = New List(Of TestProtocol) From {New FixedLengthWordsInNoise_WithPreTestLevelAdjustment_TestProtocol}
        AvailableFixedResponseAlternativeCounts = New List(Of Integer)
        AvailablePhaseAudiometryTypes = New List(Of BmldModes)
        MaximumSoundFieldSpeechLocations = 1
        MaximumSoundFieldMaskerLocations = 1
        MaximumSoundFieldBackgroundNonSpeechLocations = 0
        MaximumSoundFieldBackgroundSpeechLocations = 0
        MinimumSoundFieldSpeechLocations = 1
        MinimumSoundFieldMaskerLocations = 0
        MinimumSoundFieldBackgroundNonSpeechLocations = 0
        MinimumSoundFieldBackgroundSpeechLocations = 0
        ShowGuiChoice_ReferenceLevel = False
        ShowGuiChoice_KeyWordScoring = False
        ShowGuiChoice_ListOrderRandomization = False
        ShowGuiChoice_WithinListRandomization = True
        ShowGuiChoice_AcrossListRandomization = False
        ShowGuiChoice_FreeRecall = True
        ShowGuiChoice_DidNotHearAlternative = False
        PhaseAudiometry = False
        TargetLevel_StepSize = 5
        HistoricTrialCount = 3
        SupportsManualPausing = True
        ReferenceLevel = 65
        TargetLevel = 65
        MaskingLevel = 65
        BackgroundLevel = 50
        ContralateralMaskingLevel = 25
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

        SoundOverlapDuration = 1

        ShowGuiChoice_TargetLocations = True
        ShowGuiChoice_MaskerLocations = False
        ShowGuiChoice_BackgroundNonSpeechLocations = False
        ShowGuiChoice_BackgroundSpeechLocations = False


    End Sub



    Public Overrides ReadOnly Property ShowGuiChoice_TargetLevel As Boolean = True
    Public Overrides ReadOnly Property ShowGuiChoice_MaskingLevel As Boolean = False
    Public Overrides ReadOnly Property ShowGuiChoice_BackgroundLevel As Boolean = False





    Private PreTestListIndex As Integer
    Private TestListIndex As Integer
    Private SelectedMediaSetIndex As Integer

    Private CacheLastAdjustmentStageListVariableName As String
    Private CacheLastTestListVariableName As String
    Private CacheLastMediaSetVariableName As String

    Private PlannedLevelAdjustmentWords As List(Of SpeechMaterialComponent) = Nothing

    Private PlannedTestTrials As New TrialHistory
    Private ObservedTestTrials As New TrialHistory

    Private MaskerNoise As Audio.Sound = Nothing
    Private ContralateralNoise As Audio.Sound = Nothing

    Private TestWordPresentationTime As Double = 1.5
    Private MaximumResponseTime As Double = 4.5
    Private TestLength As Integer
    Private MaximumSoundDuration As Double = 10

    Private LastPresentedMediaSet As MediaSet
    Private PreTestWordIndex As Integer = 0

    Private IsInitialized As Boolean = False

    Private IsInitializeStarted As Boolean = False

    Public Sub TestCacheIndexation()

        'Creating cache variable names for storing last test list index and voice between sessions
        CacheLastAdjustmentStageListVariableName = FilePathRepresentation & "LastASList"
        CacheLastTestListVariableName = FilePathRepresentation & "LastTestList"
        CacheLastMediaSetVariableName = FilePathRepresentation & "LastMediaSet"

        AppCache.RemoveAppCacheVariable(CacheLastAdjustmentStageListVariableName)
        AppCache.RemoveAppCacheVariable(CacheLastTestListVariableName)
        AppCache.RemoveAppCacheVariable(CacheLastMediaSetVariableName)

        For testSession = 0 To 40

            InitializeCurrentTest()

            Utils.SendInfoToLog("testSession:" & testSession & ", SelectedMediaSetIndex: " & SelectedMediaSetIndex & ", PreTestListIndex: " & PreTestListIndex & ", TestListIndex: " & TestListIndex)

            Dim TempTrialHistory As New TrialHistory
            TempTrialHistory.Add(New WrsTrial With {.ScoreList = New List(Of Integer) From {1}})
            ObservedTestTrials = TempTrialHistory

            FinalizeTest()

            IsInitialized = False
            IsInitializeStarted = False

        Next

        AppCache.RemoveAppCacheVariable(CacheLastAdjustmentStageListVariableName)
        AppCache.RemoveAppCacheVariable(CacheLastTestListVariableName)
        AppCache.RemoveAppCacheVariable(CacheLastMediaSetVariableName)

        ObservedTestTrials = New TrialHistory

        Messager.MsgBox("Finished test of cache indexation. Results are stored in the log folder.")

    End Sub


    Public Overrides Function InitializeCurrentTest() As Tuple(Of Boolean, String)


        If IsInitialized = True Then Return New Tuple(Of Boolean, String)(True, "")

        If IsInitializeStarted = True Then Return New Tuple(Of Boolean, String)(True, "")

        IsInitializeStarted = True

        Dim AllTestListsNames = AvailableTestListsNames()
        Dim AllMediaSets = AvailableMediasets

        If AllTestListsNames.Count = 0 Then
            Return New Tuple(Of Boolean, String)(False, "No test lists exist in the currently selected speech material!")
        End If

        'Creating cache variable names for storing last test list index and voice between sessions
        CacheLastAdjustmentStageListVariableName = FilePathRepresentation & "LastASList"
        CacheLastTestListVariableName = FilePathRepresentation & "LastTestList"
        CacheLastMediaSetVariableName = FilePathRepresentation & "LastMediaSet"

        ' Getting the last used voice
        If AppCache.AppCacheVariableExists(CacheLastMediaSetVariableName) = False Then
            'Randomizing a new list number if no list has been run previously 
            SelectedMediaSetIndex = Randomizer.Next(0, AllMediaSets.Count)
        Else
            'Getting the last tested media set
            SelectedMediaSetIndex = AppCache.GetAppCacheIntegerVariableValue(CacheLastMediaSetVariableName)
        End If

        'Increasing the media set index and unwrapping it to the number of available media sets.
        SelectedMediaSetIndex += 1
        If SelectedMediaSetIndex > AllMediaSets.Count - 1 Then
            SelectedMediaSetIndex = 0
        End If

        'Selecting test list
        If AppCache.AppCacheVariableExists(CacheLastAdjustmentStageListVariableName) And AppCache.AppCacheVariableExists(CacheLastTestListVariableName) = True Then

            'Getting the last tested index
            PreTestListIndex = AppCache.GetAppCacheIntegerVariableValue(CacheLastAdjustmentStageListVariableName)
            TestListIndex = AppCache.GetAppCacheIntegerVariableValue(CacheLastTestListVariableName)

        Else
            'Randomizing a new list number if no list has been run previously 
            PreTestListIndex = Randomizer.Next(0, AllTestListsNames.Count)

            'Storing the TestListIndex as the next list
            TestListIndex = PreTestListIndex + 1

            'Unwrapping TestListIndex
            If TestListIndex > AllTestListsNames.Count - 1 Then
                TestListIndex = 0
            End If

            'Saving the variables the first time, so they don't get randomized again before they are saved (after the last media set has been tested)
            AppCache.SetAppCacheVariableValue(CacheLastAdjustmentStageListVariableName, PreTestListIndex)
            AppCache.SetAppCacheVariableValue(CacheLastTestListVariableName, TestListIndex)

        End If

        MediaSet = AvailableMediasets(SelectedMediaSetIndex)

        CreatePreTestWordsList()
        CreatePlannedWordsList()

        'Getting the masker noise only once (this should be a long section of noise with its using nominal level set
        MaskerNoise = PlannedLevelAdjustmentWords(0).GetMaskerSound(MediaSet, 0)
        'We always load ContralateralNoise even if it's not used, since the test will crash if it's suddenly switched on the the administrator (such as in pretest stimulus generation)
        ContralateralNoise = PlannedLevelAdjustmentWords(0).GetContralateralMaskerSound(MediaSet, 0)

        TestProtocol.InitializeProtocol(New TestProtocol.NextTaskInstruction With {.AdaptiveValue = TargetLevel, .TestLength = TestLength})

        IsInitialized = True

        Return New Tuple(Of Boolean, String)(True, "")

    End Function


    Private Function CreatePreTestWordsList() As Boolean

        Dim AllLists = SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List, True, False)
        Dim AllTestListsNames = AvailableTestListsNames()

        Dim LevelAdjustmentListName = AllTestListsNames(PreTestListIndex)
        For Each List In AllLists
            If List.PrimaryStringRepresentation = LevelAdjustmentListName Then PlannedLevelAdjustmentWords = List.ChildComponents
        Next

        'Checking that the lists are not empty
        If PlannedLevelAdjustmentWords Is Nothing Then
            Messager.MsgBox("Unable to find the pre-test word list!", MsgBoxStyle.Exclamation, "An error occurred!")
            Return False
        End If

        If PlannedLevelAdjustmentWords.Count = 0 Then
            Messager.MsgBox("Unable to find the pre-test words!", MsgBoxStyle.Exclamation, "An error occurred!")
            Return False
        End If

        Return True

    End Function


    Private Function CreatePlannedWordsList() As Boolean
        Dim AllLists = SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List, True, False)
        Dim AllTestListsNames = AvailableTestListsNames()

        Dim TestListName = AllTestListsNames(TestListIndex)

        Dim PlannedTestListWords As List(Of SpeechMaterialComponent) = Nothing

        For Each List In AllLists
            If List.PrimaryStringRepresentation = TestListName Then PlannedTestListWords = List.ChildComponents
        Next

        'Checking that the lists are not empty
        If PlannedTestListWords Is Nothing Then
            Messager.MsgBox("Unable to find the test word lists!", MsgBoxStyle.Exclamation, "An error occurred!")
            Return False
        End If

        If PlannedTestListWords.Count = 0 Then
            Messager.MsgBox("Unable to find the test words!", MsgBoxStyle.Exclamation, "An error occurred!")
            Return False
        End If

        PlannedTestTrials = New TrialHistory

        For i = 0 To PlannedTestListWords.Count - 1

            Dim CurrentSMC = PlannedTestListWords(i)

            Dim NewTestTrial = New WrsTrial With {.SpeechMaterialComponent = CurrentSMC,
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

            Dim HistoricTrial = ObservedTestTrials(CurrentHistoricTrialIndex)

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
            Dim CorrectWordsList As New List(Of String)

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
            ObservedTestTrials.Add(CurrentTestTrial)

            'TODO: We must store the responses and response times!!!

            'Taking a dump of the SpeechTest before swapping to the new trial, but after the protocol reply so that the protocol results also gets dumped
            CurrentTestTrial.SpeechTestPropertyDump = Utils.Logging.ListObjectPropertyValues(Me.GetType, Me)

            'Calculating the speech level
            ProtocolReply = TestProtocol.NewResponse(ObservedTestTrials)

        Else
            'Nothing to correct (this should be the start of a new test, or a resuming of a paused test)
            ProtocolReply = TestProtocol.NewResponse(New TrialHistory)
        End If


        'Preparing next trial if needed
        If ProtocolReply.Decision = SpeechTestReplies.GotoNextTrial Then

            'Preparing the next trial
            'Getting next test word
            CurrentTestTrial = PlannedTestTrials(ObservedTestTrials.Count)

            'Creating a new test trial
            DirectCast(CurrentTestTrial, WrsTrial).SpeechLevel = TargetLevel
            DirectCast(CurrentTestTrial, WrsTrial).ContralateralMaskerLevel = ContralateralMaskingLevel
            CurrentTestTrial.Tasks = 1

            'Setting trial events
            CurrentTestTrial.TrialEventList = New List(Of ResponseViewEvent)
            CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 1, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})
            CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 2, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})
            CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = System.Math.Max(1, 1000 * (TestWordPresentationTime + MaximumResponseTime)), .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut})

            'Mixing trial sound
            MixNextTrialSound()

        End If

        Return ProtocolReply.Decision

    End Function


    Private Sub MixNextTrialSound()

        Dim RETSPL_Correction As Double = 0
        If LevelsAreIn_dBHL = True Then
            RETSPL_Correction = Transducer.RETSPL_Speech
        End If

        'Getting the speech signal
        Dim TestWordSound = CurrentTestTrial.SpeechMaterialComponent.GetSound(MediaSet, 0, 1, , , , , False, False, False, , , False)
        Dim NominalLevel_FS = TestWordSound.SMA.NominalLevel

        'Storing the LinguisticSoundStimulusStartTime and the LinguisticSoundStimulusDuration (assuming that the linguistic recording is in channel 1)
        CurrentTestTrial.LinguisticSoundStimulusStartTime = TestWordPresentationTime
        CurrentTestTrial.LinguisticSoundStimulusDuration = TestWordSound.WaveData.SampleData(1).Length / TestWordSound.WaveFormat.SampleRate
        CurrentTestTrial.MaximumResponseTime = MaximumResponseTime

        'Getting a random section of the noise
        Dim TotalNoiseLength As Integer = MaskerNoise.WaveData.SampleData(1).Length
        Dim IntendedNoiseLength As Integer = MaskerNoise.WaveFormat.SampleRate * MaximumSoundDuration
        Dim RandomStartReadSample As Integer = Randomizer.Next(0, TotalNoiseLength - IntendedNoiseLength)
        Dim TrialNoise = MaskerNoise.CopySection(1, RandomStartReadSample - 1, IntendedNoiseLength) ' TODO: Here we should check to ensure that the MaskerNoise is long enough

        'Creating contalateral masking noise (with the same length as the masking noise)
        Dim TrialContralateralNoise As Audio.Sound = Nothing
        If ContralateralMasking = True Then
            TotalNoiseLength = ContralateralNoise.WaveData.SampleData(1).Length
            IntendedNoiseLength = ContralateralNoise.WaveFormat.SampleRate * MaximumSoundDuration
            RandomStartReadSample = Randomizer.Next(0, TotalNoiseLength - IntendedNoiseLength)
            TrialContralateralNoise = ContralateralNoise.CopySection(1, RandomStartReadSample - 1, IntendedNoiseLength) ' TODO: Here we should check to ensure that the MaskerNoise is long enough
        End If

        'Checking that Nominal levels agree between signal masker and contralateral masker
        If MaskerNoise.SMA.NominalLevel <> NominalLevel_FS Then Throw New Exception("Nominal level is required to be the same between speech and noise files!")
        If ContralateralMasking = True Then If ContralateralNoise.SMA.NominalLevel <> NominalLevel_FS Then Throw New Exception("Nominal level is required to be the same between speech and contralateral noise files!")

        'Calculating presentation levels
        Dim TargetSpeechLevel_FS As Double = Audio.Standard_dBSPL_To_dBFS(DirectCast(CurrentTestTrial, WrsTrial).SpeechLevel) + RETSPL_Correction
        Dim NeededSpeechGain = TargetSpeechLevel_FS - NominalLevel_FS

        'Here we're using the Speech level to set the Masker level. This means that the level of the masker file it self need to reflect the SNR, so that its mean level does not equal the nominal level, but instead deviated from the nominal level by the intended SNR. (These sound files (the Speech and the Masker) can then be mixed without any adjustment to attain the desired "clicinally" used SNR.
        'Note from 2024-12-18: This situation has been changed after changing the level definitions in the speech files from average RMS-levels to time weighted (125ms) levels, and the calibration level from -25 to -21 dBFS. The SNR of 0 dB in the materials before the change approximately equals + 6 dB in the new materials, since the speech level dropped by appr 6 dB (more precicely 6,8 dB for Talker1 and 6.3 for Talker2).
        DirectCast(CurrentTestTrial, WrsTrial).MaskerLevel = DirectCast(CurrentTestTrial, WrsTrial).SpeechLevel
        Dim TargetMaskerLevel_FS As Double = Audio.Standard_dBSPL_To_dBFS(DirectCast(CurrentTestTrial, WrsTrial).MaskerLevel) + RETSPL_Correction
        Dim NeededMaskerGain = TargetMaskerLevel_FS - NominalLevel_FS

        'Adjusts the sound levels
        Audio.DSP.AmplifySection(TestWordSound, NeededSpeechGain)
        Audio.DSP.AmplifySection(TrialNoise, NeededMaskerGain)

        If ContralateralMasking = True Then

            'Setting level, 
            Dim TargetContralateralMaskingLevel_FS As Double = Audio.Standard_dBSPL_To_dBFS(DirectCast(CurrentTestTrial, WrsTrial).ContralateralMaskerLevel) + MediaSet.EffectiveContralateralMaskingGain + RETSPL_Correction

            'Calculating the needed gain, also adding the EffectiveContralateralMaskingGain specified in the SelectedMediaSet
            Dim NeededContraLateralMaskerGain = TargetContralateralMaskingLevel_FS - NominalLevel_FS
            Audio.DSP.AmplifySection(TrialContralateralNoise, NeededContraLateralMaskerGain)

        End If

        'Mixing speech and noise
        Dim TestWordInsertionSample As Integer = TestWordSound.WaveFormat.SampleRate * TestWordPresentationTime
        Dim Silence = Audio.GenerateSound.CreateSilence(TrialNoise.WaveFormat, 1, TestWordInsertionSample, Audio.BasicAudioEnums.TimeUnits.samples)
        Audio.DSP.InsertSoundAt(TestWordSound, Silence, 0)
        TestWordSound.ZeroPad(IntendedNoiseLength)
        Dim TestSound = Audio.DSP.SuperpositionSounds({TestWordSound, TrialNoise}.ToList)

        'Creating an output sound
        CurrentTestTrial.Sound = New Audio.Sound(New Audio.Formats.WaveFormat(TestWordSound.WaveFormat.SampleRate, TestWordSound.WaveFormat.BitDepth, 2,, TestWordSound.WaveFormat.Encoding))

        If SignalLocations(0).HorizontalAzimuth < 0 Then
            'Left test ear
            'Adding speech and noise
            CurrentTestTrial.Sound.WaveData.SampleData(1) = TestSound.WaveData.SampleData(1)
            'Adding contralateral masking
            If ContralateralMasking = True Then
                CurrentTestTrial.Sound.WaveData.SampleData(2) = TrialContralateralNoise.WaveData.SampleData(1)
            End If

        Else
            'Right test ear
            'Adding speech and noise
            CurrentTestTrial.Sound.WaveData.SampleData(2) = TestSound.WaveData.SampleData(1)
            'Adding contralateral masking
            If ContralateralMasking = True Then
                CurrentTestTrial.Sound.WaveData.SampleData(1) = TrialContralateralNoise.WaveData.SampleData(1)
            End If
        End If

        'Also stores the mediaset
        CurrentTestTrial.MediaSetName = MediaSet.MediaSetName

        'And the contralateral noise on/off setting
        CurrentTestTrial.UseContralateralNoise = ContralateralMasking

        'And the EM term
        CurrentTestTrial.EfficientContralateralMaskingTerm = MediaSet.EffectiveContralateralMaskingGain

    End Sub

    Public Overrides Function GetResultStringForGui() As String

        Dim Output As New List(Of String)

        Dim ScoreList As New List(Of Double)
        For Each Trial As WrsTrial In ObservedTestTrials
            If Trial.IsCorrect = True Then
                ScoreList.Add(1)
            Else
                ScoreList.Add(0)
            End If
        Next

        If ScoreList.Count > 0 Then
            Output.Add("Resultat = " & System.Math.Round(100 * ScoreList.Average) & " % korrekt (" & ScoreList.Sum & " / " & ObservedTestTrials.Count & ")")
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


    Public Overrides Sub FinalizeTest()

        TestProtocol.FinalizeProtocol(ObservedTestTrials)

        If CurrentParticipantID <> NoTestId Then

            'Saving updated cache data values, only if a real test was completed
            Dim AllTestListsNames = AvailableTestListsNames()

            Dim NextTestListIndex As Integer = TestListIndex
            Dim NextAdjustmentStageListIndex As Integer = PreTestListIndex

            If SelectedMediaSetIndex >= AvailableMediasets.Count - 1 Then
                'Increasing list index for the next test session, after the last media set has been tested (each list is run once with each media set before next list is started)
                NextTestListIndex += 1
                NextAdjustmentStageListIndex += 1

                'Unwrapping these
                If NextTestListIndex > AllTestListsNames.Count - 1 Then
                    NextTestListIndex = 0
                End If

                If NextAdjustmentStageListIndex > AllTestListsNames.Count - 1 Then
                    NextAdjustmentStageListIndex = 0
                End If

                'Storing the test list index and media set to be used in the next test session (only if NoTestId was not used)
                AppCache.SetAppCacheVariableValue(CacheLastAdjustmentStageListVariableName, NextAdjustmentStageListIndex)
                AppCache.SetAppCacheVariableValue(CacheLastTestListVariableName, NextTestListIndex)

            End If

            'Saving the media set index every time
            AppCache.SetAppCacheVariableValue(CacheLastMediaSetVariableName, SelectedMediaSetIndex)

        End If

    End Sub

    Public Overrides Function CreatePreTestStimulus() As Tuple(Of Audio.Sound, String)

        InitializeCurrentTest()

        'Creating PreTestTrial

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
        CurrentTestTrial = New WrsTrial With {.SpeechMaterialComponent = NextTestWord,
            .SpeechLevel = TargetLevel,
            .ContralateralMaskerLevel = ContralateralMaskingLevel}

        'Mixing the test sound
        MixNextTrialSound()

        'Storing the test sound locally
        Dim PreTestSound = CurrentTestTrial.Sound

        'Resetting CurrentTestTrial 
        CurrentTestTrial = Nothing

        Return New Tuple(Of Audio.Sound, String)(PreTestSound, TestWordSpelling)

    End Function


End Class