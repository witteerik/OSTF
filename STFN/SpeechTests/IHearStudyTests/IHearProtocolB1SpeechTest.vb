﻿Imports System.IO
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
        'Dim TestSnrs() As Double = {-12, -9, -6, -2, 4} ' Three dB lower SNR
        Dim TestSnrs() As Double = {-12, -9, -6, -3, 0} ' Adjustment after Pilot testing
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

            'SendInfoToLog(p & vbTab & String.Join(vbCrLf, OrderList))

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
            Return "1. Välj testöra (deltagaren ska ha normal hörsel på testörat)" & vbCrLf &
                "2. Klicka på start för att starta testet." & vbCrLf &
                "3. Rätta manuellt under testet genom att klicka på testorden som kommer upp på skärmen"
        End Get
    End Property

    Public Overrides ReadOnly Property ParticipantInstructions As String
        Get
            Return "Patientens uppgift: " & vbCrLf & vbCrLf &
                " - Under testet ska patienten lyssna efter enstaviga ord i brus och efter varje ord ange på skärmen vilket ord hen uppfattade." & vbCrLf &
                " - Patienten ska gissa om hen är osäker." & vbCrLf &
                " - Patienten har maximalt " & TestWordPresentationTime + MaximumResponseTime & " sekunder på sig innan nästa ord kommer." & vbCrLf &
                " - Testet är 400 ord långt, med pauser efter var femtionde ord."
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
            Return False
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

    Public Overrides Property SoundOverlapDuration As Double = 1

    Public Overrides ReadOnly Property DefaultReferenceLevel As Double = 65
    Public Overrides ReadOnly Property DefaultSpeechLevel As Double = 65
    Public Overrides ReadOnly Property DefaultMaskerLevel As Double = 65
    Public Overrides ReadOnly Property DefaultBackgroundLevel As Double = 50
    Public Overrides ReadOnly Property DefaultContralateralMaskerLevel As Double = 25

    Public Overrides ReadOnly Property MinimumReferenceLevel As Double = -40
    Public Overrides ReadOnly Property MaximumReferenceLevel As Double = 50

    Public Overrides ReadOnly Property MinimumLevel_Targets As Double = -40
    Public Overrides ReadOnly Property MaximumLevel_Targets As Double = 50

    Public Overrides ReadOnly Property MinimumLevel_Maskers As Double = -40
    Public Overrides ReadOnly Property MaximumLevel_Maskers As Double = 50

    Public Overrides ReadOnly Property MinimumLevel_Background As Double = -40
    Public Overrides ReadOnly Property MaximumLevel_Background As Double = 50

    Public Overrides ReadOnly Property MinimumLevel_ContralateralMaskers As Double = -40
    Public Overrides ReadOnly Property MaximumLevel_ContralateralMaskers As Double = 50

    Public Overrides ReadOnly Property AvailableExperimentNumbers As Integer()
        Get
            Return {}
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
    Private MaximumResponseTime As Double = 4.5

    Private TestLength As Integer = 50

    Private MaximumSoundDuration As Double = 10

    Private IsInitialized As Boolean = False

    Public Sub TestCacheIndexation()

        Dim IncludeTestTrialExport As Boolean = True

        'Creating cache variable names for storing last test list index and voice between sessions
        CacheLastTestListOrderVariableName = FilePathRepresentation & "LastTestListOrder"

        AppCache.RemoveAppCacheVariable(CacheLastTestListOrderVariableName)

        For testSession = 0 To 19

            InitializeCurrentTest()

            Utils.SendInfoToLog("testSession:" & testSession & ", CurrentTestListOrderIndex : " & CurrentTestListOrderIndex)

            If IncludeTestTrialExport = True Then
                For TempTestStageIndex = 0 To PlannedTestData.Count - 1
                    Do
                        Dim TempCurrentTestTrial = PlannedTestData(TempTestStageIndex)(0)

                        'Adding the test trial
                        ObservedTestData(TempTestStageIndex).Add(TempCurrentTestTrial)

                        'Removing the trial from the planned data
                        PlannedTestData(TempTestStageIndex).Remove(TempCurrentTestTrial)

                        If PlannedTestData(TempTestStageIndex).Count = 0 Then Exit Do
                    Loop
                Next
            End If

            FinalizeTest()

            If IncludeTestTrialExport = True Then SaveTableFormatedTestResults()

            IsInitialized = False

            PlannedTestData.Clear()
            ObservedTestData.Clear()

        Next

        AppCache.RemoveAppCacheVariable(CacheLastTestListOrderVariableName)

        Messager.MsgBox("Finished test of cache indexation. Results are stored in the log folder.")

    End Sub


    Public Overrides Function InitializeCurrentTest() As Tuple(Of Boolean, String)

        If IsInitialized = True Then Return New Tuple(Of Boolean, String)(True, "")

        'Setting speech level to 40 dB HL, and ContralateralMaskingLevel to 0 dB HL
        TestOptions.SpeechLevel = 40
        TestOptions.ContralateralMaskingLevel = 0

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

        If CurrentTestListOrderIndex > TestListOrder.Keys.Max Then
            Messager.MsgBox("The maximum number of participants ( " & TestListOrder.Keys.Max + 1 & ") has been is reached." & vbCrLf & "Press ok to reset the list order memory.")
            AppCache.RemoveAppCacheVariable(CacheLastTestListOrderVariableName)
            CurrentTestListOrderIndex = 0
        End If

        PlanTrials()

        'Not using any explicit TestProtocol
        TestOptions.SelectedTestProtocol = Nothing

        IsInitialized = True

        Return New Tuple(Of Boolean, String)(True, "Test-list-order index " & CurrentTestListOrderIndex & " was selected. Press OK to start the test!")

    End Function

    Private Function PlanTrials() As Boolean

        Dim TestOrderData = TestListOrder(CurrentTestListOrderIndex)

        Dim MediaSetIndex = TestOrderData.Item1
        Dim SelectedListIndices = TestOrderData.Item3

        Dim AllMediaSets = AvailableMediasets
        TestOptions.SelectedMediaSet = AllMediaSets(MediaSetIndex)

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

                Dim TrialSNR As Double = SortedSnrOrders(CurrentTestListOrderIndex)(i)

                Dim NewTrial = New WrsTrial
                NewTrial.SpeechMaterialComponent = Sentence_SMC
                NewTrial.SpeechLevel = TestOptions.SpeechLevel
                NewTrial.MaskerLevel = GetNoiseLevel(TestOptions.SpeechLevel, TrialSNR) 'We're modifying the Masker level to get the intended SNR
                NewTrial.ContralateralMaskerLevel = TestOptions.ContralateralMaskingLevel
                'Or if we decide to change the signal level: NewTrial.ContralateralMaskerLevel = TestOptions.SpeechLevel + TestOptions.ContralateralLevelDifference

                Select Case TestOptions.SignalLocations(0).HorizontalAzimuth
                    Case -90
                        NewTrial.TestEar = SidesWithBoth.Left
                    Case 90
                        NewTrial.TestEar = SidesWithBoth.Right
                    Case Else
                        Throw New Exception("Unsupported signal azimuth: " & TestOptions.SignalLocations(0).HorizontalAzimuth)
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
        MaskerNoise = PlannedTestData(0)(0).SpeechMaterialComponent.GetMaskerSound(TestOptions.SelectedMediaSet, 0)

        'Getting the contralateral noise from the first trial SMC
        ContralateralNoise = PlannedTestData(0)(0).SpeechMaterialComponent.GetContralateralMaskerSound(TestOptions.SelectedMediaSet, 0)

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
                    SaveTableFormatedTestResults()

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
        Dim TestSound = Audio.DSP.SuperpositionSounds({TestWordSound, TrialNoise}.ToList)

        'Creating an output sound
        CurrentTestTrial.Sound = New Audio.Sound(New Audio.Formats.WaveFormat(TestWordSound.WaveFormat.SampleRate, TestWordSound.WaveFormat.BitDepth, 2,, TestWordSound.WaveFormat.Encoding))

        If TestOptions.SignalLocations(0).HorizontalAzimuth < 0 Then
            'Left test ear
            'Adding speech and noise
            CurrentTestTrial.Sound.WaveData.SampleData(1) = TestSound.WaveData.SampleData(1)
            'Adding contralateral masking
            If TestOptions.UseContralateralMasking = True Then
                CurrentTestTrial.Sound.WaveData.SampleData(2) = TrialContralateralNoise.WaveData.SampleData(1)
            End If

        Else
            'Right test ear
            'Adding speech and noise
            CurrentTestTrial.Sound.WaveData.SampleData(2) = TestSound.WaveData.SampleData(1)
            'Adding contralateral masking
            If TestOptions.UseContralateralMasking = True Then
                CurrentTestTrial.Sound.WaveData.SampleData(1) = TrialContralateralNoise.WaveData.SampleData(1)
            End If
        End If

        'Also stores the mediaset
        CurrentTestTrial.MediaSetName = TestOptions.SelectedMediaSet.MediaSetName

        'And the contralateral noise on/off setting
        CurrentTestTrial.UseContralateralNoise = TestOptions.UseContralateralMasking

        'And the EM term
        CurrentTestTrial.EfficientContralateralMaskingTerm = TestOptions.SelectedMediaSet.EffectiveContralateralMaskingGain

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