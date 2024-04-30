Imports System.IO
Imports MathNet.Numerics
Imports STFN.TestProtocol
Imports STFN.Utils

Public Class IHearProtocolB1SpeechTest
    Inherits SpeechTest

    Public Sub New(SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)

        'TestListOrder holds MeadiaSet index, Sorted SNR-order index, Lists-order (zero-based list index)
        TestListOrder.Add(0, New Tuple(Of Integer, Integer, Integer())(0, 0, {15, 6, 2, 12, 9, 7, 13, 4}))
        TestListOrder.Add(1, New Tuple(Of Integer, Integer, Integer())(0, 0, {11, 0, 8, 1, 3, 10, 14, 5}))
        TestListOrder.Add(2, New Tuple(Of Integer, Integer, Integer())(1, 1, {14, 0, 1, 9, 6, 12, 13, 10}))
        TestListOrder.Add(3, New Tuple(Of Integer, Integer, Integer())(1, 1, {2, 11, 5, 7, 4, 3, 15, 8}))
        TestListOrder.Add(4, New Tuple(Of Integer, Integer, Integer())(0, 2, {7, 3, 4, 11, 8, 0, 9, 14}))
        TestListOrder.Add(5, New Tuple(Of Integer, Integer, Integer())(0, 2, {6, 13, 15, 12, 2, 10, 5, 1}))
        TestListOrder.Add(6, New Tuple(Of Integer, Integer, Integer())(1, 3, {1, 3, 6, 4, 5, 7, 10, 12}))
        TestListOrder.Add(7, New Tuple(Of Integer, Integer, Integer())(1, 3, {15, 8, 9, 0, 13, 11, 2, 14}))
        TestListOrder.Add(8, New Tuple(Of Integer, Integer, Integer())(0, 4, {13, 15, 11, 3, 5, 12, 9, 2}))
        TestListOrder.Add(9, New Tuple(Of Integer, Integer, Integer())(0, 4, {1, 7, 14, 0, 6, 8, 10, 4}))
        TestListOrder.Add(10, New Tuple(Of Integer, Integer, Integer())(1, 0, {11, 9, 5, 0, 12, 13, 14, 2}))
        TestListOrder.Add(11, New Tuple(Of Integer, Integer, Integer())(1, 0, {3, 7, 15, 6, 8, 10, 1, 4}))
        TestListOrder.Add(12, New Tuple(Of Integer, Integer, Integer())(0, 1, {3, 14, 1, 6, 7, 11, 9, 5}))
        TestListOrder.Add(13, New Tuple(Of Integer, Integer, Integer())(0, 1, {12, 8, 10, 13, 4, 0, 2, 15}))
        TestListOrder.Add(14, New Tuple(Of Integer, Integer, Integer())(1, 2, {2, 3, 11, 13, 12, 10, 1, 7}))
        TestListOrder.Add(15, New Tuple(Of Integer, Integer, Integer())(1, 2, {4, 8, 9, 6, 14, 0, 5, 15}))
        TestListOrder.Add(16, New Tuple(Of Integer, Integer, Integer())(0, 3, {4, 6, 3, 8, 12, 14, 0, 2}))
        TestListOrder.Add(17, New Tuple(Of Integer, Integer, Integer())(0, 3, {9, 5, 13, 1, 7, 15, 10, 11}))
        TestListOrder.Add(18, New Tuple(Of Integer, Integer, Integer())(1, 4, {0, 4, 12, 2, 9, 13, 7, 1}))
        TestListOrder.Add(19, New Tuple(Of Integer, Integer, Integer())(1, 4, {6, 15, 10, 11, 8, 3, 14, 5}))

        'Dim TestSnrs() As Double = {-9, -6, -3, 1, 7} ' Initial attempt, based on Magnusson, 1995 to get 10, 30, 50 70 and 90 % correct
        Dim TestSnrs() As Double = {-12, -9, -6, -2, 4} ' Three dB lower SNR
        Dim BaseSnrOrders As New List(Of Double())
        BaseSnrOrders.Add({TestSnrs(0), TestSnrs(1), TestSnrs(2), TestSnrs(3), TestSnrs(4)})
        BaseSnrOrders.Add({TestSnrs(1), TestSnrs(2), TestSnrs(3), TestSnrs(4), TestSnrs(0)})
        BaseSnrOrders.Add({TestSnrs(2), TestSnrs(3), TestSnrs(4), TestSnrs(0), TestSnrs(1)})
        BaseSnrOrders.Add({TestSnrs(3), TestSnrs(4), TestSnrs(0), TestSnrs(1), TestSnrs(2)})
        BaseSnrOrders.Add({TestSnrs(4), TestSnrs(0), TestSnrs(1), TestSnrs(2), TestSnrs(3)})

        For p = 0 To 19
            Dim OrderList As New List(Of Double)
            For i = 0 To 9
                OrderList.AddRange(BaseSnrOrders(TestListOrder(p).Item2))
            Next
            SortedSnrOrders.Add(p, OrderList.ToArray)
        Next

    End Sub

    Public Overrides ReadOnly Property FilePathRepresentation As String
        Get
            Return "ProtocolB1_WRS800"
        End Get
    End Property


#Region "Settings"

    Public Overrides ReadOnly Property TesterInstructions As String
        Get
            Return ""
            '"1. Välj testöra." & vbCrLf &
            '    "2. Ställ talnivå till TMV3 + 40 dB (Talnivån är i dB SPL)." & vbCrLf &
            '    "3. Om kontrlateralt brus behövs, akivera kontralateralt brus och ställ in önskad brusnivå." & vbCrLf &
            '    "4. Använd kontrollen provlyssna för att ställa in 'Lagom-nivån' innan testet börjar. (Använd knappen TB för att prata med patienten när denna har lurar på sig.)" & vbCrLf &
            '    "5. Klicka på start för att starta testet." & vbCrLf &
            '    "6. Rätta manuellt under testet genom att klicka på testorden som kommer upp på skärmen"
        End Get
    End Property

    Public Overrides ReadOnly Property ParticipantInstructions As String
        Get
            Return "Patientens uppgift: " & vbCrLf &
                "Patienten ska lyssna efter enstaviga ord och efter varje ord repetera ordet muntligt. Patienten ska gissa om hen är osäker. Testet är 400 ord långt, med pauser efter 50."
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
            Return True
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
            Return 3
        End Get
    End Property

    Public Overrides ReadOnly Property SupportsManualPausing As Boolean
        Get
            Return True
        End Get
    End Property

    Private TestListOrder As New SortedList(Of Integer, Tuple(Of Integer, Integer, Integer()))
    Private SortedSnrOrders As New SortedList(Of Integer, Double())
    Private CurrentTestListOrderIndex As Integer
    Private CacheLastTestListOrderVariableName As String

    Private PlannedTestData As New List(Of TrialHistory)
    Private ObservedTestData As New List(Of TrialHistory)

    Private CurrentTestStage As Integer

    Private MaskerNoise As Audio.Sound = Nothing
    Private ContralateralNoise As Audio.Sound = Nothing

    Private TestWordPresentationTime As Double = 1.5
    Private MaximumResponseTime As Double = 5

    Private TestLength As Integer = 50

    Private MaximumSoundDuration As Double = 10

    Private IsInitialized As Boolean = False

    Public Overrides Function InitializeCurrentTest() As Boolean

        If IsInitialized = True Then Return True

        CurrentTestStage = 0

        Dim AllTestListsNames = AvailableTestListsNames()
        Dim AllMediaSets = AvailableMediasets

        'Creating cache variable names for storing last test list index and voice between sessions
        CacheLastTestListOrderVariableName = FilePathRepresentation & "LastTestListOrder"

        'Selecting test list
        If AppCache.AppCacheVariableExists(CacheLastTestListOrderVariableName) = True Then

            'Getting the new list order
            CurrentTestListOrderIndex = AppCache.GetAppCacheIntegerVariableValue(CacheLastTestListOrderVariableName)

        Else
            'Setting CurrentTestListOrderIndex to 1 for the first participant
            CurrentTestListOrderIndex = 0
        End If

        PlanTrials()

        'Not using any explicit TestProtocol
        CustomizableTestOptions.SelectedTestProtocol = Nothing

        IsInitialized = True

        Return True

    End Function

    Private Function PlanTrials() As Boolean

        Dim TestOrderData = TestListOrder(CurrentTestListOrderIndex)

        Dim MediaSetIndex = TestOrderData.Item1
        Dim SnrOrderIndex = TestOrderData.Item2
        Dim SelectedListIndices = TestOrderData.Item3

        Dim AllMediaSets = AvailableMediasets
        CustomizableTestOptions.SelectedMediaSet = AllMediaSets(MediaSetIndex)

        Dim AllLists = SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List, True, False)

        Dim SelectedLists As New List(Of SpeechMaterialComponent)
        For Each Index In SelectedListIndices
            SelectedLists.Add(AllLists(Index))
        Next

        'Plan trials, in eight test stages
        For Each List In SelectedLists
            Dim NewTestList As New TrialHistory

            For i = 0 To List.ChildComponents.Count - 1

                Dim Sentence_SMC = List.ChildComponents(i)

                Dim TrialSNR As Double = SortedSnrOrders(SnrOrderIndex)(i)

                Dim NewTrial = New WrsTrial
                NewTrial.SpeechMaterialComponent = Sentence_SMC
                NewTrial.SpeechLevel = CustomizableTestOptions.SpeechLevel
                NewTrial.MaskerLevel = GetNoiseLevel(CustomizableTestOptions.SpeechLevel, TrialSNR) 'We're modifying the Masker level to get the intended SNR
                NewTrial.ContralateralMaskerLevel = CustomizableTestOptions.ContralateralMaskingLevel
                'Or if we decide to change the signal level: NewTrial.ContralateralMaskerLevel = CustomizableTestOptions.SpeechLevel + CustomizableTestOptions.ContralateralLevelDifference

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

        'Getting the masker noise from the first trial SMC
        MaskerNoise = PlannedTestData(0)(0).SpeechMaterialComponent.GetMaskerSound(CustomizableTestOptions.SelectedMediaSet, 0)

        'Getting the contralateral noise from the first trial SMC
        ContralateralNoise = PlannedTestData(0)(0).SpeechMaterialComponent.GetContralateralMaskerSound(CustomizableTestOptions.SelectedMediaSet, 0)

        'Ranomizing within-list trial order
        For Each List In PlannedTestData
            List.Shuffle(Randomizer)
        Next

        'Setting TrialPresentationIndex
        For Each List In PlannedTestData
            For i = 0 To List.Count - 1
                List(i).ResponseAlternativeSpellings(0)(0).TrialPresentationIndex = i

                Dim HistoricTrialsToAdd As Integer = System.Math.Min(HistoricTrialCount, i)

                'Adding historic trials
                For index = 1 To HistoricTrialsToAdd

                    Dim CurrentHistoricTrialIndex = i - index
                    Dim HistoricTrial = List(CurrentHistoricTrialIndex)

                    'We only add the spelling of first child component here, since displaying history is only supported for sigle words
                    Dim HistoricSpeechTestResponseAlternative = New SpeechTestResponseAlternative With {
                        .Spelling = HistoricTrial.SpeechMaterialComponent.ChildComponents(0).GetCategoricalVariableValue("Spelling"),
                        .IsScoredItem = True,
                        .TrialPresentationIndex = CurrentHistoricTrialIndex,
                        .ParentTestTrial = HistoricTrial}

                    'We insert the history
                    List(i).ResponseAlternativeSpellings(0).Insert(0, HistoricSpeechTestResponseAlternative) 'We put it into the first index as this is not multidimensional response alternatives (such as in Matrix tests)
                Next

            Next
        Next


        Return True

    End Function


    Public Overrides Sub UpdateHistoricTrialResults(sender As Object, e As SpeechTestInputEventArgs)

        'Stores historic responses
        For Index = 0 To e.LinguisticResponses.Count - 1

            'Determining which trials that should be modified

            Dim CurrentTrialIndex = CurrentTestTrial.ResponseAlternativeSpellings(0).Last.TrialPresentationIndex
            Dim CurrentHistoricTrialIndex = CurrentTrialIndex - HistoricTrialCount + Index

            If CurrentHistoricTrialIndex < 0 Then Continue For 'This means that the index refers to an invisible response button, before the first test trial 

            Dim HistoricTrial = ObservedTestData(CurrentTestStage)(CurrentHistoricTrialIndex)

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

        Dim NewNextTaskInstruction As New NextTaskInstruction

        If e IsNot Nothing Then

            'This is an incoming test trial response
            'Corrects the trial response, based on the given response

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
            ObservedTestData(CurrentTestStage).Add(CurrentTestTrial)

            'Removing the trial from the planned data
            PlannedTestData(CurrentTestStage).Remove(CurrentTestTrial)

            'TODO: We must store the responses and response times!!!

            'Checking if first test is finished
            If PlannedTestData(CurrentTestStage).Count = 0 Then

                'Increasing test stage
                CurrentTestStage += 1

                If CurrentTestStage < 8 Then

                    'Save data
                    Dim Results = GetResults()
                    SaveTextFormattedResults(Results)

                    'And informing the participant
                    NewNextTaskInstruction.Decision = SpeechTestReplies.PauseTestingWithCustomInformation
                    PauseInformation = "Del " & CurrentTestStage & " av 8 är klar." & vbCrLf & " Klicka på Fortsätt för att starta nästa del."

                Else
                    'Test is completed after eight test stages (i.e. lists)
                    NewNextTaskInstruction.Decision = SpeechTestReplies.TestIsCompleted
                End If
            Else
                NewNextTaskInstruction.Decision = SpeechTestReplies.GotoNextTrial
            End If

        Else
            'Nothing to correct (this should be the start of a new test, or a resuming of a paused test)
            NewNextTaskInstruction.Decision = SpeechTestReplies.GotoNextTrial
        End If

        'Preparing next trial if needed
        If NewNextTaskInstruction.Decision = SpeechTestReplies.GotoNextTrial Then

            'Getting next test trial (the first one among the remaining planned trials in the current test stage
            CurrentTestTrial = PlannedTestData(CurrentTestStage)(0)

            'Mixing trial sound
            MixNextTrialSound()

        End If

        Return NewNextTaskInstruction.Decision

    End Function

    Private Sub MixNextTrialSound()

        'Getting the speech signal
        Dim TestWordSound = CurrentTestTrial.SpeechMaterialComponent.GetSound(CustomizableTestOptions.SelectedMediaSet, 0, 1, , , , , False, False, False, , , False)
        Dim NominalLevel_FS = TestWordSound.SMA.NominalLevel

        'Getting a random section of the noise
        Dim TotalNoiseLength As Integer = MaskerNoise.WaveData.SampleData(1).Length
        Dim IntendedNoiseLength As Integer = MaskerNoise.WaveFormat.SampleRate * MaximumSoundDuration
        Dim RandomStartReadSample As Integer = Randomizer.Next(0, TotalNoiseLength - IntendedNoiseLength)
        Dim TrialNoise = MaskerNoise.CopySection(1, RandomStartReadSample - 1, IntendedNoiseLength) ' TODO: Here we should check to ensure that the MaskerNoise is long enough

        'Creating contalateral masking noise (with the same length as the masking noise)
        Dim TrialContralateralNoise As Audio.Sound = Nothing
        If CustomizableTestOptions.UseContralateralMasking = True Then
            TotalNoiseLength = ContralateralNoise.WaveData.SampleData(1).Length
            IntendedNoiseLength = ContralateralNoise.WaveFormat.SampleRate * MaximumSoundDuration
            RandomStartReadSample = Randomizer.Next(0, TotalNoiseLength - IntendedNoiseLength)
            TrialContralateralNoise = ContralateralNoise.CopySection(1, RandomStartReadSample - 1, IntendedNoiseLength) ' TODO: Here we should check to ensure that the MaskerNoise is long enough
        End If

        'Checking that Nominal levels agree between signal masker and contralateral masker
        If MaskerNoise.SMA.NominalLevel <> NominalLevel_FS Then Throw New Exception("Nominal level is required to be the same between speech and noise files!")
        If CustomizableTestOptions.UseContralateralMasking = True Then If ContralateralNoise.SMA.NominalLevel <> NominalLevel_FS Then Throw New Exception("Nominal level is required to be the same between speech and contralateral noise files!")

        'Calculating presentation levels
        Dim TargetSpeechLevel_FS As Double = Audio.Standard_dBSPL_To_dBFS(DirectCast(CurrentTestTrial, WrsTrial).SpeechLevel)
        Dim NeededSpeechGain = TargetSpeechLevel_FS - NominalLevel_FS

        Dim TargetMaskerLevel_FS As Double = Audio.Standard_dBSPL_To_dBFS(DirectCast(CurrentTestTrial, WrsTrial).MaskerLevel)
        Dim NeededMaskerGain = TargetMaskerLevel_FS - NominalLevel_FS

        'Adjusts the sound levels
        Audio.DSP.AmplifySection(TestWordSound, NeededSpeechGain)
        Audio.DSP.AmplifySection(TrialNoise, NeededMaskerGain)

        If CustomizableTestOptions.UseContralateralMasking = True Then

            'Setting level, 
            Dim TargetContralateralMaskingLevel_FS As Double = Audio.Standard_dBSPL_To_dBFS(DirectCast(CurrentTestTrial, WrsTrial).ContralateralMaskerLevel)

            'Calculating the needed gain, also adding the EffectiveContralateralMaskingGain specified in the SelectedMediaSet
            Dim NeededContraLateralMaskerGain = TargetContralateralMaskingLevel_FS - NominalLevel_FS + CustomizableTestOptions.SelectedMediaSet.EffectiveContralateralMaskingGain
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

        'Storing the LinguisticSoundStimulusStartTime and the LinguisticSoundStimulusDuration (assuming that the linguistic recording is in channel 1)
        CurrentTestTrial.LinguisticSoundStimulusStartTime = TestWordPresentationTime
        CurrentTestTrial.LinguisticSoundStimulusDuration = TestWordSound.WaveData.SampleData(1).Length / TestWordSound.WaveFormat.SampleRate

    End Sub

    Public Overrides Function GetResults() As TestResults

        Dim Output = New TestResults(TestResults.TestResultTypes.IHPB1)

        Output.TrialStringComment = New List(Of String)
        Output.SpeechLevelSeries = New List(Of Double)
        Output.MaskerLevelSeries = New List(Of Double)
        'Output.ContralateralMaskerLevelSeries = New List(Of Double)
        Output.ScoreSeries = New List(Of String)

        Output.TestResultSummaryLines = New List(Of String)

        For TestStageIndex = 0 To ObservedTestData.Count - 1

            Dim ScoreList As New List(Of Double)

            For Each Trial As WrsTrial In ObservedTestData(TestStageIndex)

                Output.TrialStringComment.Add(Trial.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling"))
                Output.SpeechLevelSeries.Add(System.Math.Round(Trial.SpeechLevel))
                Output.MaskerLevelSeries.Add(System.Math.Round(Trial.MaskerLevel))
                'Output.ContralateralMaskerLevelSeries.Add(System.Math.Round(Trial.ContralateralMaskerLevel))
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



    Public Overrides Sub FinalizeTest()

        If CurrentParticipantID <> NoTestId Then

            'Saving updated cache data values, only if a real test was completed
            Dim AllTestListsNames = AvailableTestListsNames()

            Dim NextTestListOrderIndex As Integer = CurrentTestListOrderIndex

            'Increasing list index for the next test session
            NextTestListOrderIndex += 1

            'Storing the test list order index to be used in the next test session (only if NoTestId was not used)
            AppCache.SetAppCacheVariableValue(CacheLastTestListOrderVariableName, NextTestListOrderIndex)

        End If

    End Sub

    Dim PreTestWordIndex As Integer = 0
    Public Overrides Function CreatePreTestStimulus() As Tuple(Of Audio.Sound, String)

        Throw New NotImplementedException

    End Function

End Class