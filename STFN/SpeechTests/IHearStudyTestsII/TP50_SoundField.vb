Imports System.IO
Imports MathNet.Numerics
Imports STFN.TestProtocol
Imports STFN.Utils

Public Class TP50_SoundField
    Inherits SpeechTest

    Public Sub New(SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)

    End Sub

    Public Overrides ReadOnly Property FilePathRepresentation As String
        Get
            Return "TP50_SoundField"
        End Get
    End Property


#Region "Settings"

    Public Overrides ReadOnly Property TesterInstructions As String
        Get
            Return "1. Ställ talnivå till TMV3 + 20 dB, eller maximalt " & MaximumLevel_Targets & " dB HL." & vbCrLf &
                "2. Använd kontrollen provlyssna för att ställa in 'Lagom-nivån' innan testet börjar." & vbCrLf &
                "3. Klicka på start för att starta testet." & vbCrLf &
                "4. Rätta manuellt under testet genom att klicka på testorden som kommer upp på skärmen" & vbCrLf &
                "5. Patienten har maximalt " & TestWordPresentationTime + MaximumResponseTime & " sekunder på sig innan nästa ord kommer."

        End Get
    End Property

    Public Overrides ReadOnly Property ParticipantInstructions As String
        Get
            Return "Patientens uppgift: " & vbCrLf & vbCrLf &
                " - Under testet ska patienten lyssna efter enstaviga ord i brus och efter varje ord verbalt vilket ord hen uppfattade." & vbCrLf &
                " - Patienten ska gissa om hen är osäker." & vbCrLf &
                " - Patienten har maximalt " & TestWordPresentationTime + MaximumResponseTime & " sekunder på sig innan nästa ord kommer." & vbCrLf &
                " - Patienten ska ha huvudet riktat rakt framåt under mätningen." & vbCrLf &
                " - Testet är 50 ord långt."

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
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsManualMediaSetSelection As Boolean
        Get
            Return True
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
            Return True
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
            Return New List(Of TestProtocol) From {New FixedLengthWordsInNoise_WithPreTestLevelAdjustment_TestProtocol}
        End Get
    End Property

    Public Overrides ReadOnly Property AvailableFixedResponseAlternativeCounts As List(Of Integer)
        Get
            Return New List(Of Integer)
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
            Return 1
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
            Return 1
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
            Return False
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
            Return Utils.Constants.TriState.False
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
            Return Utils.TriState.True
        End Get
    End Property

    Public Overrides ReadOnly Property UseDidNotHearAlternative As Utils.TriState
        Get
            Return Utils.Constants.TriState.False
        End Get
    End Property

    Public Overrides ReadOnly Property UseContralateralMasking As Utils.TriState
        Get
            Return Utils.Constants.TriState.False
        End Get
    End Property

    Public Overrides ReadOnly Property UsePhaseAudiometry As Utils.TriState
        Get
            Return Utils.Constants.TriState.False
        End Get
    End Property

    Public Overrides ReadOnly Property LevelStepSize As Double
        Get
            Return 5
        End Get
    End Property

    Public Overrides ReadOnly Property HistoricTrialCount As Integer
        Get
            Return 3
        End Get
    End Property

    Public Overrides ReadOnly Property SupportsManualPausing As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides Property SoundOverlapDuration As Double = 1

    Public Overrides ReadOnly Property DefaultReferenceLevel As Double = 65
    Public Overrides ReadOnly Property DefaultSpeechLevel As Double = 65
    Public Overrides ReadOnly Property DefaultMaskerLevel As Double = 65
    Public Overrides ReadOnly Property DefaultBackgroundLevel As Double = 50
    Public Overrides ReadOnly Property DefaultContralateralMaskerLevel As Double = 25


    Public Overrides ReadOnly Property MinimumReferenceLevel As Double = -10
    Public Overrides ReadOnly Property MaximumReferenceLevel As Double = 100

    Public Overrides ReadOnly Property MinimumLevel_Targets As Double = -10
    Public Overrides ReadOnly Property MaximumLevel_Targets As Double = 100

    Public Overrides ReadOnly Property MinimumLevel_Maskers As Double = -10
    Public Overrides ReadOnly Property MaximumLevel_Maskers As Double = 100

    Public Overrides ReadOnly Property MinimumLevel_Background As Double = -10
    Public Overrides ReadOnly Property MaximumLevel_Background As Double = 100

    Public Overrides ReadOnly Property MinimumLevel_ContralateralMaskers As Double = -40
    Public Overrides ReadOnly Property MaximumLevel_ContralateralMaskers As Double = 100


    Public Overrides ReadOnly Property AvailableExperimentNumbers As Integer()
        Get
            Dim OutputList As New List(Of Integer)
            Return OutputList.ToArray
        End Get
    End Property


    Dim PreTestListIndex As Integer
    Dim TestListIndex As Integer
    Dim SelectedMediaSetIndex As Integer

    Dim CacheLastAdjustmentStageListVariableName As String
    Dim CacheLastTestListVariableName As String
    Dim CacheLastMediaSetVariableName As String

    Private PlannedLevelAdjustmentWords As List(Of SpeechMaterialComponent) = Nothing

    Private PlannedTestTrials As New TrialHistory
    Private ObservedTestTrials As New TrialHistory

    Private MaskerNoise As Audio.Sound = Nothing
    Private ContralateralNoise As Audio.Sound = Nothing

    Private TestWordPresentationTime As Double = 1.5
    Private MaximumResponseTime As Double = 4.5
    Private TestLength As Integer
    Private MaximumSoundDuration As Double = 10

    Private IsInitialized As Boolean = False

    Private IsInitializeStarted As Boolean = False


    Public Overrides Function InitializeCurrentTest() As Tuple(Of Boolean, String)


        If IsInitialized = True Then Return New Tuple(Of Boolean, String)(True, "")

        If IsInitializeStarted = True Then Return New Tuple(Of Boolean, String)(True, "")

        IsInitializeStarted = True

        Dim AllTestListsNames = AvailableTestListsNames()

        If AllTestListsNames.Count = 0 Then
            Return New Tuple(Of Boolean, String)(False, "No test lists exist in the currently selected speech material!")
        End If

        CreatePreTestWordsList()
        CreatePlannedWordsList()

        'Getting the masker noise only once (this should be a long section of noise with its using nominal level set
        MaskerNoise = PlannedLevelAdjustmentWords(0).GetMaskerSound(TestOptions.SelectedMediaSet, 0)
        'We always load ContralateralNoise even if it's not used, since the test will crash if it's suddenly switched on the the administrator (such as in pretest stimulus generation)
        ContralateralNoise = PlannedLevelAdjustmentWords(0).GetContralateralMaskerSound(TestOptions.SelectedMediaSet, 0)
        'Storing last presented MediaSet to determine which noises were loaded. (These are reloaded in MixNextTrialSound if needed)
        LastPresentedMediaSet = TestOptions.SelectedMediaSet

        TestOptions.SelectedTestProtocol.InitializeProtocol(New TestProtocol.NextTaskInstruction With {.AdaptiveValue = TestOptions.SpeechLevel, .TestLength = TestLength})

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

        If TestOptions.RandomizeItemsWithinLists = True Then
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

            'Calculating the speech level
            ProtocolReply = TestOptions.SelectedTestProtocol.NewResponse(ObservedTestTrials)

        Else
            'Nothing to correct (this should be the start of a new test, or a resuming of a paused test)
            ProtocolReply = TestOptions.SelectedTestProtocol.NewResponse(New TrialHistory)
        End If

        'Preparing next trial if needed
        If ProtocolReply.Decision = SpeechTestReplies.GotoNextTrial Then

            'Preparing the next trial
            'Getting next test word
            CurrentTestTrial = PlannedTestTrials(ObservedTestTrials.Count)

            'Creating a new test trial
            DirectCast(CurrentTestTrial, WrsTrial).SpeechLevel = TestOptions.SpeechLevel
            DirectCast(CurrentTestTrial, WrsTrial).ContralateralMaskerLevel = TestOptions.ContralateralMaskingLevel
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

    Private LastPresentedMediaSet As MediaSet = Nothing

    Private Sub MixNextTrialSound()

        'Updating the noises if needed
        'Only done when MediaSet is changed
        If LastPresentedMediaSet IsNot TestOptions.SelectedMediaSet Then
            'Getting the masker noise only once (this should be a long section of noise with its using nominal level set
            MaskerNoise = PlannedLevelAdjustmentWords(0).GetMaskerSound(TestOptions.SelectedMediaSet, 0)
            'We always load ContralateralNoise even if it's not used, since the test will crash if it's suddenly switched on the the administrator (such as in pretest stimulus generation)
            ContralateralNoise = PlannedLevelAdjustmentWords(0).GetContralateralMaskerSound(TestOptions.SelectedMediaSet, 0)
            LastPresentedMediaSet = TestOptions.SelectedMediaSet
        End If


        Dim RETSPL_Correction As Double = 0
        If TestOptions.UseRetsplCorrection = True Then
            RETSPL_Correction = TestOptions.SelectedTransducer.RETSPL_Speech
        End If

        'Getting the speech signal
        Dim TestWordSound = CurrentTestTrial.SpeechMaterialComponent.GetSound(TestOptions.SelectedMediaSet, 0, 1, , , , , False, False, False, , , False)
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
        If TestOptions.UseContralateralMasking = True Then
            TotalNoiseLength = ContralateralNoise.WaveData.SampleData(1).Length
            IntendedNoiseLength = ContralateralNoise.WaveFormat.SampleRate * MaximumSoundDuration
            RandomStartReadSample = Randomizer.Next(0, TotalNoiseLength - IntendedNoiseLength)
            TrialContralateralNoise = ContralateralNoise.CopySection(1, RandomStartReadSample - 1, IntendedNoiseLength) ' TODO: Here we should check to ensure that the MaskerNoise is long enough
        End If

        'Checking that Nominal levels agree between signal masker and contralateral masker
        If MaskerNoise.SMA.NominalLevel <> NominalLevel_FS Then Throw New Exception("Nominal level is required to be the same between speech and noise files!")
        If TestOptions.UseContralateralMasking = True Then If ContralateralNoise.SMA.NominalLevel <> NominalLevel_FS Then Throw New Exception("Nominal level is required to be the same between speech and contralateral noise files!")

        'Calculating presentation levels
        Dim TargetSpeechLevel_FS As Double = Audio.Standard_dBSPL_To_dBFS(DirectCast(CurrentTestTrial, WrsTrial).SpeechLevel) + RETSPL_Correction
        Dim NeededSpeechGain = TargetSpeechLevel_FS - NominalLevel_FS

        'Here we're using the Speech level to set the Masker level. This means that the level of the masker file it self need to reflect the SNR, so that its mean level does not equal the nominal level, but instead deviated from the nominal level by the intended SNR. (These sound files (the Speech and the Masker) can then be mixed without any adjustment to attain the desired "clicinally" used SNR.
        DirectCast(CurrentTestTrial, WrsTrial).MaskerLevel = DirectCast(CurrentTestTrial, WrsTrial).SpeechLevel
        Dim TargetMaskerLevel_FS As Double = Audio.Standard_dBSPL_To_dBFS(DirectCast(CurrentTestTrial, WrsTrial).MaskerLevel) + RETSPL_Correction
        Dim NeededMaskerGain = TargetMaskerLevel_FS - NominalLevel_FS

        'Adjusts the sound levels
        Audio.DSP.AmplifySection(TestWordSound, NeededSpeechGain)
        Audio.DSP.AmplifySection(TrialNoise, NeededMaskerGain)

        If TestOptions.UseContralateralMasking = True Then

            'Setting level, 
            Dim TargetContralateralMaskingLevel_FS As Double = Audio.Standard_dBSPL_To_dBFS(DirectCast(CurrentTestTrial, WrsTrial).ContralateralMaskerLevel) + TestOptions.SelectedMediaSet.EffectiveContralateralMaskingGain + RETSPL_Correction

            'Calculating the needed gain, also adding the EffectiveContralateralMaskingGain specified in the SelectedMediaSet
            Dim NeededContraLateralMaskerGain = TargetContralateralMaskingLevel_FS - NominalLevel_FS
            Audio.DSP.AmplifySection(TrialContralateralNoise, NeededContraLateralMaskerGain)

        End If

        'Mixing speech and noise
        Dim TestWordInsertionSample As Integer = TestWordSound.WaveFormat.SampleRate * TestWordPresentationTime
        Dim Silence = Audio.GenerateSound.CreateSilence(TrialNoise.WaveFormat, 1, TestWordInsertionSample, Audio.BasicAudioEnums.TimeUnits.samples)
        Audio.DSP.InsertSoundAt(TestWordSound, Silence, 0)
        TestWordSound.ZeroPad(IntendedNoiseLength)

        'Dim TestSound = Audio.DSP.SuperpositionSounds({TestWordSound, TrialNoise}.ToList)

        'Creating an output sound
        CurrentTestTrial.Sound = New Audio.Sound(New Audio.Formats.WaveFormat(TestWordSound.WaveFormat.SampleRate, TestWordSound.WaveFormat.BitDepth, 2,, TestWordSound.WaveFormat.Encoding))

        'Putting speech in channel 1
        CurrentTestTrial.Sound.WaveData.SampleData(1) = TestWordSound.WaveData.SampleData(1)

        'And noise in channel 2
        CurrentTestTrial.Sound.WaveData.SampleData(2) = TrialNoise.WaveData.SampleData(1)


        'Also stores the mediaset
        CurrentTestTrial.MediaSetName = TestOptions.SelectedMediaSet.MediaSetName

        'And the contralateral noise on/off setting
        CurrentTestTrial.UseContralateralNoise = TestOptions.UseContralateralMasking

        'And the EM term
        CurrentTestTrial.EfficientContralateralMaskingTerm = TestOptions.SelectedMediaSet.EffectiveContralateralMaskingGain

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

    Public Overrides Function GetTestTrialResultExportString() As String
        Return "Export of trial level test results is not yet implemented"
    End Function

    Public Overrides Function GetTestResultsExportString() As String

        Dim ExportStringList As New List(Of String)

        For i = 0 To ObservedTestTrials.Count - 1
            If i = 0 Then
                ExportStringList.Add("TrialIndex" & vbTab & ObservedTestTrials(i).TestResultColumnHeadings)
            End If
            ExportStringList.Add(i & vbTab & ObservedTestTrials(i).TestResultAsTextRow)
        Next

        Return String.Join(vbCrLf, ExportStringList)

    End Function


    Public Overrides Sub FinalizeTest()

        TestOptions.SelectedTestProtocol.FinalizeProtocol(ObservedTestTrials)

    End Sub

    Dim PreTestWordIndex As Integer = 0
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
            .SpeechLevel = TestOptions.SpeechLevel,
            .ContralateralMaskerLevel = TestOptions.ContralateralMaskingLevel}

        'Mixing the test sound
        MixNextTrialSound()

        'Storing the test sound locally
        Dim PreTestSound = CurrentTestTrial.Sound

        'Resetting CurrentTestTrial 
        CurrentTestTrial = Nothing

        Return New Tuple(Of Audio.Sound, String)(PreTestSound, TestWordSpelling)

    End Function

End Class