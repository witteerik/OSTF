Imports System.IO
Imports MathNet.Numerics
Imports STFN.TestProtocol
Imports STFN.Utils

Public Class IHearProtocolB1SpeechTest
    Inherits SpeechTest

    Public Sub New(SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)

        'TestListOrder holds MeadiaSet index, Sorted SNR-order index, Lists-order
        TestListOrder.Add(1, New Tuple(Of Integer, Integer, Integer())(0, 0, {16, 7, 3, 13, 10, 8, 14, 5}))
        TestListOrder.Add(2, New Tuple(Of Integer, Integer, Integer())(0, 0, {12, 1, 9, 2, 4, 11, 15, 6}))
        TestListOrder.Add(3, New Tuple(Of Integer, Integer, Integer())(1, 1, {15, 1, 2, 10, 7, 13, 14, 11}))
        TestListOrder.Add(4, New Tuple(Of Integer, Integer, Integer())(1, 1, {3, 12, 6, 8, 5, 4, 16, 9}))
        TestListOrder.Add(5, New Tuple(Of Integer, Integer, Integer())(0, 2, {8, 4, 5, 12, 9, 1, 10, 15}))
        TestListOrder.Add(6, New Tuple(Of Integer, Integer, Integer())(0, 2, {7, 14, 16, 13, 3, 11, 6, 2}))
        TestListOrder.Add(7, New Tuple(Of Integer, Integer, Integer())(1, 3, {2, 4, 7, 5, 6, 8, 11, 13}))
        TestListOrder.Add(8, New Tuple(Of Integer, Integer, Integer())(1, 3, {16, 9, 10, 1, 14, 12, 3, 15}))
        TestListOrder.Add(9, New Tuple(Of Integer, Integer, Integer())(0, 4, {14, 16, 12, 4, 6, 13, 10, 3}))
        TestListOrder.Add(10, New Tuple(Of Integer, Integer, Integer())(0, 4, {2, 8, 15, 1, 7, 9, 11, 5}))
        TestListOrder.Add(11, New Tuple(Of Integer, Integer, Integer())(1, 0, {12, 10, 6, 1, 13, 14, 15, 3}))
        TestListOrder.Add(12, New Tuple(Of Integer, Integer, Integer())(1, 0, {4, 8, 16, 7, 9, 11, 2, 5}))
        TestListOrder.Add(13, New Tuple(Of Integer, Integer, Integer())(0, 1, {4, 15, 2, 7, 8, 12, 10, 6}))
        TestListOrder.Add(14, New Tuple(Of Integer, Integer, Integer())(0, 1, {13, 9, 11, 14, 5, 1, 3, 16}))
        TestListOrder.Add(15, New Tuple(Of Integer, Integer, Integer())(1, 2, {3, 4, 12, 14, 13, 11, 2, 8}))
        TestListOrder.Add(16, New Tuple(Of Integer, Integer, Integer())(1, 2, {5, 9, 10, 7, 15, 1, 6, 16}))
        TestListOrder.Add(17, New Tuple(Of Integer, Integer, Integer())(0, 3, {5, 7, 4, 9, 13, 15, 1, 3}))
        TestListOrder.Add(18, New Tuple(Of Integer, Integer, Integer())(0, 3, {10, 6, 14, 2, 8, 16, 11, 12}))
        TestListOrder.Add(19, New Tuple(Of Integer, Integer, Integer())(1, 4, {1, 5, 13, 3, 10, 14, 8, 2}))
        TestListOrder.Add(20, New Tuple(Of Integer, Integer, Integer())(1, 4, {7, 16, 11, 12, 9, 4, 15, 6}))

        Dim TestSnrs() As Double = {-6, -3, 0, 3, 6}
        Dim BaseSnrOrders As New List(Of Double())
        BaseSnrOrders.Add({TestSnrs(0), TestSnrs(1), TestSnrs(2), TestSnrs(3), TestSnrs(4)})
        BaseSnrOrders.Add({TestSnrs(1), TestSnrs(2), TestSnrs(3), TestSnrs(4), TestSnrs(0)})
        BaseSnrOrders.Add({TestSnrs(2), TestSnrs(3), TestSnrs(4), TestSnrs(0), TestSnrs(1)})
        BaseSnrOrders.Add({TestSnrs(3), TestSnrs(4), TestSnrs(0), TestSnrs(1), TestSnrs(2)})
        BaseSnrOrders.Add({TestSnrs(4), TestSnrs(0), TestSnrs(1), TestSnrs(2), TestSnrs(3)})

        For p = 1 To 20
            Dim OrderList As New List(Of Double)
            For i = 1 To 10
                OrderList.AddRange(BaseSnrOrders(TestListOrder(p).Item2))
            Next
            SortedSnrOrders.Add(p, OrderList.ToArray)
        Next

    End Sub

    Public Overrides ReadOnly Property FilePathRepresentation As String
        Get
            Return "ProtocolB1"
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
            Return 5
        End Get
    End Property

    Public Overrides ReadOnly Property HistoricTrialCount As Integer
        Get
            Return 3
        End Get
    End Property

    Private TestListOrder As New SortedList(Of Integer, Tuple(Of Integer, Integer, Integer()))
    Private SortedSnrOrders As New SortedList(Of Integer, Double())


    Private CurrentTestListOrderIndex As Integer
    Private SelectedMediaSetIndex As Integer

    Private CacheLastTestListOrderVariableName As String

    Private PlannedTestData As New List(Of TrialHistory)
    Private ObservedTestData As New List(Of TrialHistory)

    Private MaskerNoise As Audio.Sound = Nothing
    Private ContralateralNoise As Audio.Sound = Nothing

    Private TestWordPresentationTime As Double = 0.5
    Private MaximumResponseTime As Double = 5

    Private TestLength As Integer = 50

    Private MaximumSoundDuration As Double = 10

    Private IsInitialized As Boolean = False

    Public Overrides Function InitializeCurrentTest() As Boolean

        If IsInitialized = True Then Return True

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
            CurrentTestListOrderIndex = 1
        End If

        PlanTrials()

        CustomizableTestOptions.SelectedTestProtocol.InitializeProtocol(New TestProtocol.NextTaskInstruction With {.AdaptiveValue = CustomizableTestOptions.SpeechLevel, .TestLength = TestLength})

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
                NewTrial.MaskerLevel = GetNoiseLevel(CustomizableTestOptions.SpeechLevel, TrialSNR)
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

        'Ranomizing within-list trial order
        For Each List In PlannedTestData
            List.Shuffle(Randomizer)
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
            ProtocolReply = CustomizableTestOptions.SelectedTestProtocol.NewResponse(ObservedTestTrials)

        Else
            'Nothing to correct (this should be the start of a new test, or a resuming of a paused test)
            ProtocolReply = CustomizableTestOptions.SelectedTestProtocol.NewResponse(New TrialHistory)
        End If

        'Preparing next trial if needed
        If ProtocolReply.Decision = SpeechTestReplies.GotoNextTrial Then
            PrepareNextTrial(ProtocolReply)
        End If

        Return ProtocolReply.Decision

    End Function

    Private Sub PrepareNextTrial(ByVal NextTaskInstruction As TestProtocol.NextTaskInstruction)


        'Preparing the next trial
        'Getting next test word
        CurrentTestTrial = PlannedTestTrials(ObservedTestTrials.Count)

        'Creating a new test trial
        DirectCast(CurrentTestTrial, WrsTrial).SpeechLevel = CustomizableTestOptions.SpeechLevel
        DirectCast(CurrentTestTrial, WrsTrial).ContralateralMaskerLevel = CustomizableTestOptions.ContralateralMaskingLevel
        CurrentTestTrial.Tasks = 1

        Dim HistoryToShow As Integer = 3
        Dim HistoricTrialsToAdd As Integer = System.Math.Min(HistoryToShow, ObservedTestTrials.Count)

        Dim CurrentTrialIndex = CurrentTestTrial.ResponseAlternativeSpellings(0).Last.TrialPresentationIndex

        'Adding historic trials
        For index = 1 To HistoricTrialsToAdd

            Dim CurrentHistoricTrialIndex = CurrentTrialIndex - index
            Dim HistoricTrial = ObservedTestTrials(CurrentHistoricTrialIndex)

            'We only add the spelling of first child component here, since displaying history is only supported for sigle words
            Dim HistoricSpeechTestResponseAlternative = New SpeechTestResponseAlternative With {
                .Spelling = HistoricTrial.SpeechMaterialComponent.ChildComponents(0).GetCategoricalVariableValue("Spelling"),
                .IsScoredItem = True,
                .TrialPresentationIndex = CurrentHistoricTrialIndex,
                .ParentTestTrial = HistoricTrial}

            'We insert the history
            CurrentTestTrial.ResponseAlternativeSpellings(0).Insert(0, HistoricSpeechTestResponseAlternative) 'We put it into the first index as this is not multidimensional response alternatives (such as in Matrix tests)
        Next

        'Setting trial events
        CurrentTestTrial.TrialEventList = New List(Of ResponseViewEvent)
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 1, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 2, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = System.Math.Max(1, 1000 * (TestWordPresentationTime + MaximumResponseTime)), .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut})

        'Mixing trial sound
        MixNextTrialSound()

    End Sub

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

        'Adjusts the sound levels
        Audio.DSP.AmplifySection(TestWordSound, NeededSpeechGain)
        'Applying the same gain to the masker. Very important: This requires that the masker is preadjusted to create the intended SNR together with the speech recordings, and have the same nominal level! (I.e. If speech and sound files would be mixed without any adjustment, they would get their desired SNR.)
        Audio.DSP.AmplifySection(TrialNoise, NeededSpeechGain)

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

    End Sub


    Public Overrides Function GetResults() As TestResults

        Dim Output = New TestResults(TestResults.TestResultTypes.WRS)


        'Storing the AdaptiveLevelSeries
        'Output.AdaptiveLevelSeries = New List(Of Double)
        Output.SpeechLevelSeries = New List(Of Double)
        'Output.MaskerLevelSeries = New List(Of Double)
        Output.ContralateralMaskerLevelSeries = New List(Of Double)
        'Output.SNRLevelSeries = New List(Of Double)
        'Output.TestStageSeries = New List(Of String)
        Output.ProportionCorrectSeries = New List(Of String)
        Output.ScoreSeries = New List(Of String)
        Output.Progress = New List(Of Integer)
        Output.ProgressMax = New List(Of Integer)

        Dim ScoreList As New List(Of Double)

        For Each Trial As WrsTrial In ObservedTestTrials
            ScoreList.Add(DirectCast(Trial, WrsTrial).GetProportionTasksCorrect)

            Output.Progress.Add(ObservedTestTrials.Count)
            Output.ProgressMax.Add(TestLength)
            'Output.AdaptiveLevelSeries.Add(System.Math.Round(Trial.AdaptiveValue))
            Output.SpeechLevelSeries.Add(System.Math.Round(Trial.SpeechLevel))
            'Output.MaskerLevelSeries.Add(System.Math.Round(Trial.MaskerLevel))
            Output.ContralateralMaskerLevelSeries.Add(System.Math.Round(Trial.ContralateralMaskerLevel))
            'Output.SNRLevelSeries.Add(System.Math.Round(Trial.SNR))
            'Output.TestStageSeries.Add(Trial.TestStage)
            Output.ProportionCorrectSeries.Add(Trial.GetProportionTasksCorrect)
            'If Trial.IsCorrect = True Then
            '    Output.ScoreSeries.Add("Correct")
            'Else
            '    Output.ScoreSeries.Add("Incorrect")
            'End If
        Next

        If ScoreList.Count > 0 Then Output.ProportionCorrect = ScoreList.Average

        Return Output

    End Function


    Public Overrides Sub FinalizeTest()

        CustomizableTestOptions.SelectedTestProtocol.FinalizeProtocol(ObservedTestTrials)

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