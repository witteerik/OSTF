Imports STFN.SipTest
Imports STFN.Audio.SoundScene
Imports STFN.Utils

Public Class QuickSiP

    Inherits SpeechTest

    Public Overrides ReadOnly Property FilePathRepresentation As String
        Get
            Return "QuickSiP"
        End Get
    End Property

    Public Overrides ReadOnly Property TesterInstructions As String
        Get
            Return ""
        End Get
    End Property

    Public Overrides ReadOnly Property ParticipantInstructions As String
        Get
            Return ""
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
            Return TriState.True
        End Get
    End Property

    Public Overrides ReadOnly Property AvailableTestModes As List(Of TestModes)
        Get
            Return New List(Of TestModes) From {TestModes.Custom}
        End Get
    End Property

    Public Overrides ReadOnly Property AvailableTestProtocols As List(Of TestProtocol)
        Get
            Return Nothing
        End Get
    End Property

    Public Overrides ReadOnly Property AvailableFixedResponseAlternativeCounts As List(Of Integer)
        Get
            Return New List(Of Integer) From {3}
        End Get
    End Property

    Public Overrides ReadOnly Property AvailablePhaseAudiometryTypes As List(Of BmldModes)
        Get
            Return New List(Of BmldModes)
        End Get
    End Property

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
            Return 2
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
            Return 2
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
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property CanHaveBackgroundSpeech As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property UseKeyWordScoring As Utils.TriState
        Get
            Return Utils.TriState.False
        End Get
    End Property

    Public Overrides ReadOnly Property UseListOrderRandomization As Utils.TriState
        Get
            Return Utils.TriState.False
        End Get
    End Property

    Public Overrides ReadOnly Property UseWithinListRandomization As Utils.TriState
        Get
            Return Utils.TriState.False
        End Get
    End Property

    Public Overrides ReadOnly Property UseAcrossListRandomization As Utils.TriState
        Get
            Return Utils.TriState.False
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
            Return 0
        End Get
    End Property

    Public Sub New(ByVal SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)

    End Sub

    Private CurrentSipTestMeasurement As SipMeasurement
    Public SelectedSoundPropagationType As SoundPropagationTypes = SoundPropagationTypes.SimulatedSoundField
    Private RandomSeed As Integer? = Nothing
    Private SelectedTestparadigm As Testparadigm = Testparadigm.Quick
    Private SelectedTransducer As AudioSystemSpecification
    Private MinimumStimulusOnsetTime As Double = 0.3
    Private MaximumStimulusOnsetTime As Double = 0.8
    Private TrialSoundMaxDuration As Double = 10
    Private UseBackgroundSpeech As Boolean = False
    Private MaximumResponseTime As Double = 4
    Private PretestSoundDuration As Double = 5
    Private UseVisualQue As Boolean = False
    Private ResponseAlternativeDelay As Double = 0.5
    Private DirectionalSimulationSet As String = "ARC - Harcellen - HATS 256 - 48kHz"
    Private ReferenceLevel As Double = 68.34
    Private PresetName As String = "QuickSiP"

    Private TestIsStarted As Boolean = False
    Private SipMeasurementRandomizer As Random
    Private TestIsPaused As Boolean = False

    Private CurrentTestStage As Integer = 0

    Dim Stage1ResultsSummary As SortedList(Of Double, Tuple(Of QuickSipList, Double))

    Public Overrides Function InitializeCurrentTest() As Boolean


        'Creates a new randomizer before each test start
        Dim Seed As Integer? = Nothing
        If Seed.HasValue Then
            SipMeasurementRandomizer = New Random(Seed)
        Else
            SipMeasurementRandomizer = New Random
        End If

        SelectedTransducer = AvaliableTransducers(0)

        CurrentSipTestMeasurement = New SipMeasurement(CurrentParticipantID, SpeechMaterial.ParentTestSpecification, AdaptiveTypes.Fixed, SelectedTestparadigm)

        CurrentSipTestMeasurement.ExportTrialSoundFiles = False

        If CustomizableTestOptions.UseSimulatedSoundField = True Then
            SelectedSoundPropagationType = SoundPropagationTypes.SimulatedSoundField

            'Dim AvailableSets = DirectionalSimulator.GetAvailableDirectionalSimulationSets(SelectedTransducer)
            'DirectionalSimulator.TrySetSelectedDirectionalSimulationSet(AvailableSets(1), SelectedTransducer, False)

            Dim FoundDirSimulator As Boolean = DirectionalSimulator.TrySetSelectedDirectionalSimulationSet(DirectionalSimulationSet, SelectedTransducer, False)
            If FoundDirSimulator = False Then
                ShowMessageBox("Unable to find the directional simulation set " & DirectionalSimulationSet)
                Return False
            End If

        Else
            SelectedSoundPropagationType = SoundPropagationTypes.PointSpeakers
        End If

        CurrentTestStage = 0

        'Setting up test trials to run
        PlanQuickSiPTrials(SelectedSoundPropagationType, RandomSeed)

        If CurrentSipTestMeasurement.HasSimulatedSoundFieldTrials = True And DirectionalSimulator.SelectedDirectionalSimulationSetName = "" Then
            ShowMessageBox("The measurement requires a directional simulation set to be selected!")
            Return False
        End If

        Return True

    End Function

    Private Enum QuickSipDirections
        SpeechFromFront
        SpeechFromLeft
        SpeechFromRight
    End Enum

    Dim SipTestLists As New List(Of QuickSipList)

    Private Class QuickSipList
        Public SMC As SpeechMaterialComponent
        Public MediaSet As MediaSet
        Public PNR As Double
        Public IsVowelGroup As Boolean
    End Class

    Private Function GetPnrScores(ByVal OnlyConsonantGroups As Boolean) As SortedList(Of Double, Tuple(Of QuickSipList, Double))

        Dim ResultList As New List(Of Tuple(Of QuickSipList, Double)) ' QuickSipList, MeanScore

        For i = 0 To SipTestLists.Count - 1

            Dim CurrentSipTestList = SipTestLists(i)
            If OnlyConsonantGroups = True Then If CurrentSipTestList.IsVowelGroup = True Then Continue For
            Dim CurrentScoresList As New List(Of Double)

            For Each Trial In CurrentSipTestMeasurement.ObservedTrials
                If Trial.MediaSet Is CurrentSipTestList.MediaSet And
                        Trial.PNR = CurrentSipTestList.PNR And
                        Trial.SpeechMaterialComponent.ParentComponent Is CurrentSipTestList.SMC Then

                    CurrentScoresList.Add(Trial.Score)

                End If
            Next

            ResultList.Add(New Tuple(Of QuickSipList, Double)(CurrentSipTestList, CurrentScoresList.Average))
        Next

        Dim PnrSortedList As New SortedList(Of Double, Tuple(Of QuickSipList, Double))
        For Each Result In ResultList
            PnrSortedList.Add(Result.Item1.PNR, Result)
        Next

        Return PnrSortedList

    End Function

    Private Function GetMostDifficultConsonantGroupAbove66() As QuickSipList


        Dim PnrSortedList = GetPnrScores(True) 'As New List(Of Tuple(Of QuickSipList, Double)) ' QuickSipList, MeanScore

        'For i = 0 To SipTestLists.Count - 1

        '    Dim CurrentSipTestList = SipTestLists(i)
        '    If CurrentSipTestList.IsVowelGroup = True Then Continue For
        '    Dim CurrentScoresList As New List(Of Double)

        '    For Each Trial In CurrentSipTestMeasurement.ObservedTrials
        '        If Trial.MediaSet Is CurrentSipTestList.MediaSet And
        '                Trial.PNR = CurrentSipTestList.PNR And
        '                Trial.SpeechMaterialComponent.ParentComponent Is CurrentSipTestList.SMC Then

        '            CurrentScoresList.Add(Trial.Score)

        '        End If
        '    Next

        '    ResultList.Add(New Tuple(Of QuickSipList, Double)(CurrentSipTestList, CurrentScoresList.Average))
        'Next

        'Dim PnrSortedList As New SortedList(Of Double, Tuple(Of QuickSipList, Double))
        'For Each Result In ResultList
        '    PnrSortedList.Add(Result.Item1.PNR, Result)
        'Next

        For i = 0 To PnrSortedList.Values.Count - 1
            If PnrSortedList.Values(i).Item2 > 0.66 Then
                Return PnrSortedList.Values(i).Item1
            End If
        Next

        Return PnrSortedList.Values.Last.Item1

    End Function

    Private Sub PlanQuickSiPTrials(ByVal SoundPropagationType As SoundPropagationTypes, Optional ByVal RandomSeed As Integer? = Nothing)

        Dim SelectedMediaSets As List(Of MediaSet) = AvailableMediasets

        'Creating a new random if seed is supplied
        If RandomSeed.HasValue Then CurrentSipTestMeasurement.Randomizer = New Random(RandomSeed)

        'Getting the preset
        Dim Preset = CurrentSipTestMeasurement.ParentTestSpecification.SpeechMaterial.Presets.GetPretest(PresetName).Members

        'Ordering presets as intended
        'mark_märk_mörk, fyr_skyr_syr, sitt_sytt_sött, kil_fil_sil
        'Dim IntendedPresetOrder As New List(Of String) From {"mark_märk_mörk", "fyr_skyr_syr", "sitt_sytt_sött", "kil_fil_sil"}
        Dim IntendedPresetOrder As New List(Of String) From {"fyr_skyr_syr", "mark_märk_mörk", "kil_fil_sil", "tuff_tuss_tusch", "sitt_sytt_sött"}
        Dim TempPresets As New List(Of SpeechMaterialComponent)
        For i = 0 To 4
            For j = 0 To Preset.Count - 1
                If Preset(j).PrimaryStringRepresentation = IntendedPresetOrder(i) Then
                    TempPresets.Add(Preset(j))
                    Exit For
                End If
            Next
        Next

        Preset = TempPresets

        'Getting the sound source locations
        'Speech from the front
        Dim TargetStimulusLocations_Stage1 As SoundSourceLocation() = {New SoundSourceLocation With {.HorizontalAzimuth = 0, .Distance = 1.45}}
        Dim MaskerLocations_Stage1 As SoundSourceLocation() = {New SoundSourceLocation With {.HorizontalAzimuth = 180, .Distance = 1.45}}
        Dim BackgroundLocations_Stage1 As SoundSourceLocation() = {New SoundSourceLocation With {.HorizontalAzimuth = 0, .Distance = 1.45}, New SoundSourceLocation With {.HorizontalAzimuth = 180, .Distance = 1.45}}

        'Speech on left side
        Dim TargetStimulusLocations_Stage2_Left = {New SoundSourceLocation With {.HorizontalAzimuth = -90, .Distance = 1.45}}
        Dim MaskerLocationsStage2_Left = {New SoundSourceLocation With {.HorizontalAzimuth = 90, .Distance = 1.45}}
        Dim BackgroundLocationsStage2_Left = {New SoundSourceLocation With {.HorizontalAzimuth = -90, .Distance = 1.45}, New SoundSourceLocation With {.HorizontalAzimuth = 90, .Distance = 1.45}}

        'Speech on right side
        Dim TargetStimulusLocations_Stage2_Right = {New SoundSourceLocation With {.HorizontalAzimuth = 90, .Distance = 1.45}}
        Dim MaskerLocationsStage2_Right = {New SoundSourceLocation With {.HorizontalAzimuth = -90, .Distance = 1.45}}
        Dim BackgroundLocationsStage2_Right = {New SoundSourceLocation With {.HorizontalAzimuth = -90, .Distance = 1.45}, New SoundSourceLocation With {.HorizontalAzimuth = 90, .Distance = 1.45}}


        Dim CurrentQuickSipDirections As New List(Of QuickSipDirections)

        If CurrentTestStage = 0 Then
            'Clearing any trials that may have been planned by a previous call
            CurrentSipTestMeasurement.ClearTrials()

            'Talker in front
            SipTestLists.Add(New QuickSipList With {.SMC = Preset(0), .MediaSet = SelectedMediaSets(1), .PNR = 15, .IsVowelGroup = Preset(0).GetNumericVariableValue("V")})
            SipTestLists.Add(New QuickSipList With {.SMC = Preset(1), .MediaSet = SelectedMediaSets(0), .PNR = 12, .IsVowelGroup = Preset(1).GetNumericVariableValue("V")})

            SipTestLists.Add(New QuickSipList With {.SMC = Preset(2), .MediaSet = SelectedMediaSets(1), .PNR = 9, .IsVowelGroup = Preset(2).GetNumericVariableValue("V")})
            SipTestLists.Add(New QuickSipList With {.SMC = Preset(3), .MediaSet = SelectedMediaSets(0), .PNR = 6, .IsVowelGroup = Preset(3).GetNumericVariableValue("V")})

            SipTestLists.Add(New QuickSipList With {.SMC = Preset(4), .MediaSet = SelectedMediaSets(1), .PNR = 3, .IsVowelGroup = Preset(4).GetNumericVariableValue("V")})
            SipTestLists.Add(New QuickSipList With {.SMC = Preset(0), .MediaSet = SelectedMediaSets(0), .PNR = 0, .IsVowelGroup = Preset(0).GetNumericVariableValue("V")})

            SipTestLists.Add(New QuickSipList With {.SMC = Preset(1), .MediaSet = SelectedMediaSets(1), .PNR = -3, .IsVowelGroup = Preset(1).GetNumericVariableValue("V")})
            SipTestLists.Add(New QuickSipList With {.SMC = Preset(2), .MediaSet = SelectedMediaSets(0), .PNR = -6, .IsVowelGroup = Preset(2).GetNumericVariableValue("V")})

            SipTestLists.Add(New QuickSipList With {.SMC = Preset(3), .MediaSet = SelectedMediaSets(1), .PNR = -9, .IsVowelGroup = Preset(3).GetNumericVariableValue("V")})
            SipTestLists.Add(New QuickSipList With {.SMC = Preset(4), .MediaSet = SelectedMediaSets(0), .PNR = -12, .IsVowelGroup = Preset(4).GetNumericVariableValue("V")})

            'SipTestLists.Add(New QuickSipList With {.SMC = Preset(0), .MediaSet = SelectedMediaSets(1), .PNR = 13, .IsVowelGroup = Preset(0).GetNumericVariableValue("V")})
            'SipTestLists.Add(New QuickSipList With {.SMC = Preset(1), .MediaSet = SelectedMediaSets(0), .PNR = 10, .IsVowelGroup = Preset(1).GetNumericVariableValue("V")})

            'SipTestLists.Add(New QuickSipList With {.SMC = Preset(2), .MediaSet = SelectedMediaSets(1), .PNR = 7, .IsVowelGroup = Preset(2).GetNumericVariableValue("V")})
            'SipTestLists.Add(New QuickSipList With {.SMC = Preset(3), .MediaSet = SelectedMediaSets(0), .PNR = 4, .IsVowelGroup = Preset(3).GetNumericVariableValue("V")})

            'SipTestLists.Add(New QuickSipList With {.SMC = Preset(0), .MediaSet = SelectedMediaSets(1), .PNR = 1, .IsVowelGroup = Preset(0).GetNumericVariableValue("V")})
            'SipTestLists.Add(New QuickSipList With {.SMC = Preset(1), .MediaSet = SelectedMediaSets(0), .PNR = -2, .IsVowelGroup = Preset(1).GetNumericVariableValue("V")})

            'SipTestLists.Add(New QuickSipList With {.SMC = Preset(2), .MediaSet = SelectedMediaSets(1), .PNR = -5, .IsVowelGroup = Preset(2).GetNumericVariableValue("V")})
            'SipTestLists.Add(New QuickSipList With {.SMC = Preset(3), .MediaSet = SelectedMediaSets(0), .PNR = -8, .IsVowelGroup = Preset(3).GetNumericVariableValue("V")})


            'SipTestLists.Add(New Tuple(Of SpeechMaterialComponent, MediaSet, Double)(Preset(0), SelectedMediaSets(0), 13))
            'SipTestLists.Add(New Tuple(Of SpeechMaterialComponent, MediaSet, Double)(Preset(1), SelectedMediaSets(1), 10))
            'SipTestLists.Add(New Tuple(Of SpeechMaterialComponent, MediaSet, Double)(Preset(0), SelectedMediaSets(0), 7))
            'SipTestLists.Add(New Tuple(Of SpeechMaterialComponent, MediaSet, Double)(Preset(1), SelectedMediaSets(1), 4))
            'SipTestLists.Add(New Tuple(Of SpeechMaterialComponent, MediaSet, Double)(Preset(2), SelectedMediaSets(0), 1))
            'SipTestLists.Add(New Tuple(Of SpeechMaterialComponent, MediaSet, Double)(Preset(3), SelectedMediaSets(1), -2))
            'SipTestLists.Add(New Tuple(Of SpeechMaterialComponent, MediaSet, Double)(Preset(2), SelectedMediaSets(0), -5))
            'SipTestLists.Add(New Tuple(Of SpeechMaterialComponent, MediaSet, Double)(Preset(3), SelectedMediaSets(1), -8))

            CurrentQuickSipDirections.Add(QuickSipDirections.SpeechFromFront)

        Else

            'Storing stage one results summary
            Stage1ResultsSummary = GetPnrScores(False)

            Dim Stage2Settings = GetMostDifficultConsonantGroupAbove66()

            SipTestLists.Clear()

            SipTestLists.Add(New QuickSipList With {.SMC = Stage2Settings.SMC, .MediaSet = SelectedMediaSets(0), .PNR = Stage2Settings.PNR, .IsVowelGroup = Stage2Settings.IsVowelGroup})
            SipTestLists.Add(New QuickSipList With {.SMC = Stage2Settings.SMC, .MediaSet = SelectedMediaSets(1), .PNR = Stage2Settings.PNR - 3, .IsVowelGroup = Stage2Settings.IsVowelGroup})


            'Dim UseEasyCondition As Boolean = False

            'If UseEasyCondition = True Then
            '    'Easier condition
            '    SipTestLists.Add(New Tuple(Of SpeechMaterialComponent, MediaSet, Double)(Preset(1), SelectedMediaSets(0), 7))
            '    SipTestLists.Add(New Tuple(Of SpeechMaterialComponent, MediaSet, Double)(Preset(1), SelectedMediaSets(1), 3))

            '    'SipTestLists.Add(New Tuple(Of SpeechMaterialComponent, MediaSet, Double)(Preset(1), SelectedMediaSets(0), 7))
            '    'SipTestLists.Add(New Tuple(Of SpeechMaterialComponent, MediaSet, Double)(Preset(1), SelectedMediaSets(1), 4))
            'Else
            '    'Difficult condition
            '    SipTestLists.Add(New Tuple(Of SpeechMaterialComponent, MediaSet, Double)(Preset(3), SelectedMediaSets(0), -9))
            '    SipTestLists.Add(New Tuple(Of SpeechMaterialComponent, MediaSet, Double)(Preset(3), SelectedMediaSets(1), -13))

            '    'SipTestLists.Add(New Tuple(Of SpeechMaterialComponent, MediaSet, Double)(Preset(3), SelectedMediaSets(0), -5))
            '    'SipTestLists.Add(New Tuple(Of SpeechMaterialComponent, MediaSet, Double)(Preset(3), SelectedMediaSets(1), -8))
            'End If

            CurrentQuickSipDirections.Add(QuickSipDirections.SpeechFromRight)
            CurrentQuickSipDirections.Add(QuickSipDirections.SpeechFromLeft)

        End If


        'A list that determines which SipTestLists that will be randomized together
        Dim NewTestUnitIndices As New List(Of Integer) From {0, 2, 4, 6, 8}

        Dim CurrentTestUnit As SiPTestUnit = Nothing

        For Each CurrentQuickSipDirection In CurrentQuickSipDirections

            For i = 0 To SipTestLists.Count - 1

                If NewTestUnitIndices.Contains(i) Then
                    If CurrentTestUnit IsNot Nothing Then
                        CurrentSipTestMeasurement.TestUnits.Add(CurrentTestUnit)
                    End If
                    CurrentTestUnit = New SiPTestUnit(CurrentSipTestMeasurement)
                End If

                Dim PresetComponent = SipTestLists(i).SMC
                Dim MediaSet = SipTestLists(i).MediaSet
                Dim PNR = SipTestLists(i).PNR

                Dim TestWords = PresetComponent.GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.Sentence)
                CurrentTestUnit.SpeechMaterialComponents.AddRange(TestWords)

                For c = 0 To TestWords.Count - 1
                    Dim NewTrial As SipTrial = Nothing
                    Select Case CurrentQuickSipDirection
                        Case QuickSipDirections.SpeechFromFront
                            NewTrial = New SipTrial(CurrentTestUnit, TestWords(c), MediaSet, SoundPropagationType, TargetStimulusLocations_Stage1.ToArray, MaskerLocations_Stage1.ToArray, BackgroundLocations_Stage1, CurrentTestUnit.ParentMeasurement.Randomizer)
                        Case QuickSipDirections.SpeechFromLeft
                            NewTrial = New SipTrial(CurrentTestUnit, TestWords(c), MediaSet, SoundPropagationType, TargetStimulusLocations_Stage2_Left.ToArray, MaskerLocationsStage2_Left.ToArray, BackgroundLocationsStage2_Left, CurrentTestUnit.ParentMeasurement.Randomizer)
                        Case QuickSipDirections.SpeechFromRight
                            NewTrial = New SipTrial(CurrentTestUnit, TestWords(c), MediaSet, SoundPropagationType, TargetStimulusLocations_Stage2_Right.ToArray, MaskerLocationsStage2_Right.ToArray, BackgroundLocationsStage2_Right, CurrentTestUnit.ParentMeasurement.Randomizer)
                    End Select

                    NewTrial.SetLevels(ReferenceLevel, PNR)
                    CurrentTestUnit.PlannedTrials.Add(NewTrial)
                Next
            Next
        Next

        'Adds the last unit
        CurrentSipTestMeasurement.TestUnits.Add(CurrentTestUnit)

        Dim StartUnitIndex As Integer = 0
        If CurrentTestStage > 0 Then
            StartUnitIndex = CurrentSipTestMeasurement.TestUnits.Count - 3
        End If

        'Randomizing the order within units
        For ui = StartUnitIndex To CurrentSipTestMeasurement.TestUnits.Count - 1
            Dim Unit As SiPTestUnit = CurrentSipTestMeasurement.TestUnits(ui)
            Dim RandomList As New List(Of SipTrial)
            Do Until Unit.PlannedTrials.Count = 0
                Dim RandomIndex As Integer = CurrentSipTestMeasurement.Randomizer.Next(0, Unit.PlannedTrials.Count)
                RandomList.Add(Unit.PlannedTrials(RandomIndex))
                Unit.PlannedTrials.RemoveAt(RandomIndex)
            Loop
            Unit.PlannedTrials = RandomList
        Next

        'Adding the trials CurrentSipTestMeasurement (from which they can be drawn during testing)
        For ui = StartUnitIndex To CurrentSipTestMeasurement.TestUnits.Count - 1
            Dim Unit As SiPTestUnit = CurrentSipTestMeasurement.TestUnits(ui)
            For Each Trial In Unit.PlannedTrials
                CurrentSipTestMeasurement.PlannedTrials.Add(Trial)
            Next
        Next

    End Sub

    Private Sub InitiateTestByPlayingSound()

        'Sets the measurement datetime
        CurrentSipTestMeasurement.MeasurementDateTime = DateTime.Now

        'Cretaing a context sound without any test stimulus, that runs for approx TestSetup.PretestSoundDuration seconds, using audio from the first selected MediaSet
        Dim TestSound As Audio.Sound = CreateInitialSound(CustomizableTestOptions.SelectedMediaSet)

        'Plays sound
        SoundPlayer.SwapOutputSounds(TestSound)

        'Premixing the first 10 sounds 
        CurrentSipTestMeasurement.PreMixTestTrialSoundsOnNewTread(SelectedTransducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, SipMeasurementRandomizer, TrialSoundMaxDuration, UseBackgroundSpeech, 10)

    End Sub


    Public Function CreateInitialSound(ByRef SelectedMediaSet As MediaSet, Optional ByVal Duration As Double? = Nothing) As Audio.Sound

        Try

            'Setting up the SiP-trial sound mix
            Dim MixStopWatch As New Stopwatch
            MixStopWatch.Start()

            'Sets a List of SoundSceneItem in which to put the sounds to mix
            Dim ItemList = New List(Of SoundSceneItem)

            Dim SoundWaveFormat As Audio.Formats.WaveFormat = Nothing

            'Getting a background non-speech sound
            Dim BackgroundNonSpeech_Sound As Audio.Sound = SpeechMaterial.GetBackgroundNonspeechSound(SelectedMediaSet, 0)

            'Stores the sample rate and the wave format
            Dim CurrentSampleRate As Integer = BackgroundNonSpeech_Sound.WaveFormat.SampleRate
            SoundWaveFormat = BackgroundNonSpeech_Sound.WaveFormat

            'Sets a total pretest sound length
            Dim TrialSoundLength As Integer
            If Duration.HasValue Then
                TrialSoundLength = Duration * SoundWaveFormat.SampleRate
            Else
                TrialSoundLength = (PretestSoundDuration + 4) * CurrentSampleRate 'Adds 4 seconds to allow for potential delay caused by the mixing time of the first test trial sounds
            End If

            'Copies copies random sections of the background non-speech sound into two sounds
            Dim Background1 = BackgroundNonSpeech_Sound.CopySection(1, SipMeasurementRandomizer.Next(0, BackgroundNonSpeech_Sound.WaveData.SampleData(1).Length - TrialSoundLength - 2), TrialSoundLength)
            Dim Background2 = BackgroundNonSpeech_Sound.CopySection(1, SipMeasurementRandomizer.Next(0, BackgroundNonSpeech_Sound.WaveData.SampleData(1).Length - TrialSoundLength - 2), TrialSoundLength)

            'Sets up fading specifications for the background signals
            Dim FadeSpecs_Background = New List(Of Audio.DSP.Transformations.FadeSpecifications)
            FadeSpecs_Background.Add(New Audio.DSP.Transformations.FadeSpecifications(Nothing, 0, 0, CurrentSampleRate * 1))
            FadeSpecs_Background.Add(New Audio.DSP.Transformations.FadeSpecifications(0, Nothing, -CurrentSampleRate * 0.01))

            'Adds the background (non-speech) signals, with fade, duck and location specifications
            Dim LevelGroup As Integer = 1 ' The level group value is used to set the added sound level of items sharing the same (arbitrary) LevelGroup value to the indicated sound level. (Thus, the sounds with the same LevelGroup value are measured together.)

            ItemList.Add(New SoundSceneItem(Background1, 1, SelectedMediaSet.BackgroundNonspeechRealisticLevel, LevelGroup,
                                            CurrentSipTestMeasurement.TestProcedure.BackgroundLocations(SelectedTestparadigm)(0), SoundSceneItem.SoundSceneItemRoles.BackgroundSpeech, 0,,,, FadeSpecs_Background))
            ItemList.Add(New SoundSceneItem(Background2, 1, SelectedMediaSet.BackgroundNonspeechRealisticLevel, LevelGroup,
                                            CurrentSipTestMeasurement.TestProcedure.BackgroundLocations(SelectedTestparadigm)(1), SoundSceneItem.SoundSceneItemRoles.BackgroundNonspeech, 0,,,, FadeSpecs_Background))
            LevelGroup += 1

            MixStopWatch.Stop()
            If LogToConsole = True Then Console.WriteLine("Prepared sounds in " & MixStopWatch.ElapsedMilliseconds & " ms.")
            MixStopWatch.Restart()

            'Creating the mix by calling CreateSoundScene of the current Mixer
            Dim MixedInitialSound As Audio.Sound = SelectedTransducer.Mixer.CreateSoundScene(ItemList, SelectedSoundPropagationType)

            If LogToConsole = True Then Console.WriteLine("Mixed sound in " & MixStopWatch.ElapsedMilliseconds & " ms.")

            'TODO: Here we can simulate and/or compensate for hearing loss:
            'SimulateHearingLoss,
            'CompensateHearingLoss

            Return MixedInitialSound

        Catch ex As Exception
            Utils.SendInfoToLog(ex.ToString, "ExceptionsDuringTesting")
            Return Nothing
        End Try

    End Function


    Private Sub PrepareTestTrialSound()

        Try

            If (CurrentSipTestMeasurement.ObservedTrials.Count + 3) Mod 10 = 0 Then
                'Premixing the next 10 sounds, starting three trials before the next is needed 
                CurrentSipTestMeasurement.PreMixTestTrialSoundsOnNewTread(SelectedTransducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, SipMeasurementRandomizer, TrialSoundMaxDuration, UseBackgroundSpeech, 10)
            End If

            'Waiting for the background thread to finish mixing
            Dim WaitPeriods As Integer = 0
            While CurrentTestTrial.Sound Is Nothing
                WaitPeriods += 1
                Threading.Thread.Sleep(100)
                If LogToConsole = True Then Console.WriteLine("Waiting for sound to mix: " & WaitPeriods * 100 & " ms")
            End While

        Catch ex As Exception
            'Ignores any exceptions...
            'Utils.SendInfoToLog(ex.ToString, "ExceptionsDuringTesting")
        End Try

    End Sub


    ''' <summary>
    ''' This method can be called by the backend in order to display a message box message to the user.
    ''' </summary>
    ''' <param name="Message"></param>
    Private Sub ShowMessageBox(Message As String, Optional ByVal Title As String = "")

        If Title = "" Then
            Select Case GuiLanguage
                Case Utils.Constants.Languages.Swedish
                    Title = "SiP-testet"
                Case Else
                    Title = "SiP-test"
            End Select
        End If

        Messager.MsgBox(Message, MsgBoxStyle.Information, Title)

    End Sub

    Public Overrides Function GetSpeechTestReply(sender As Object, e As SpeechTestInputEventArgs) As SpeechTestReplies

        If e IsNot Nothing Then

            'Corrects the trial response, based on the given response
            Dim CorrectWordsList As New List(Of String)

            'Resets the CurrentTestTrial.ScoreList
            'And also storing SiP-test type data
            CurrentTestTrial.ScoreList = New List(Of Integer)
            Select Case e.LinguisticResponses(0)
                Case CurrentTestTrial.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling")
                    CurrentTestTrial.ScoreList.Add(1)
                    DirectCast(CurrentTestTrial, SipTrial).Result = PossibleResults.Correct
                    DirectCast(CurrentTestTrial, SipTrial).IsCorrect = True

                Case ""
                    CurrentTestTrial.ScoreList.Add(0)
                    DirectCast(CurrentTestTrial, SipTrial).Result = PossibleResults.Missing
                    DirectCast(CurrentTestTrial, SipTrial).IsCorrect = False

                Case Else
                    CurrentTestTrial.ScoreList.Add(0)
                    DirectCast(CurrentTestTrial, SipTrial).Result = PossibleResults.Incorrect
                    DirectCast(CurrentTestTrial, SipTrial).IsCorrect = False

            End Select

            DirectCast(CurrentTestTrial, SipTrial).Response = e.LinguisticResponses(0)

            'This is an incoming test trial response
            If CurrentTestTrial IsNot Nothing Then
                CurrentSipTestMeasurement.MoveTrialToHistory(CurrentTestTrial)
            End If

        Else
            'Nothing to correct (this should be the start of a new test)
            'Playing initial sound, and premixing trials
            InitiateTestByPlayingSound()

        End If

        'TODO: We must store the responses and response times!!!

        'Calculating the speech level
        'Dim ProtocolReply = CustomizableTestOptions.SelectedTestProtocol.NewResponse(ObservedTrials)
        Dim ProtocolReply = New TestProtocol.NextTaskInstruction With {.Decision = SpeechTestReplies.GotoNextTrial}

        If CurrentSipTestMeasurement.PlannedTrials.Count = 0 Then

            If CurrentTestStage = 0 Then
                'Planning trials for the the second stage
                CurrentTestStage += 1
                PlanQuickSiPTrials(SelectedSoundPropagationType)
                CurrentSipTestMeasurement.PreMixTestTrialSoundsOnNewTread(SelectedTransducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, SipMeasurementRandomizer, TrialSoundMaxDuration, UseBackgroundSpeech, 10)
                ProtocolReply.Decision = SpeechTestReplies.GotoNextTrial

            Else
                Return SpeechTestReplies.TestIsCompleted
            End If

        End If

        'Preparing next trial if needed
        If ProtocolReply.Decision = SpeechTestReplies.GotoNextTrial Then
            PrepareNextTrial(ProtocolReply)
        End If

        Return ProtocolReply.Decision

    End Function


    Private Sub PrepareNextTrial(ByVal NextTaskInstruction As TestProtocol.NextTaskInstruction)

        'Preparing the next trial
        CurrentTestTrial = CurrentSipTestMeasurement.GetNextTrial()
        CurrentTestTrial.TestStage = NextTaskInstruction.TestStage
        CurrentTestTrial.Tasks = 1
        CurrentTestTrial.ResponseAlternativeSpellings = New List(Of List(Of SpeechTestResponseAlternative))
        Dim ResponseAlternatives As New List(Of SpeechTestResponseAlternative)

        'Adding the current word spelling as a response alternative
        ResponseAlternatives.Add(New SpeechTestResponseAlternative With {.Spelling = CurrentTestTrial.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling"), .IsScoredItem = CurrentTestTrial.SpeechMaterialComponent.IsKeyComponent})

        'Picking random response alternatives from all available test words
        Dim AllContrastingWords = CurrentTestTrial.SpeechMaterialComponent.GetSiblingsExcludingSelf()
        For Each ContrastingWord In AllContrastingWords
            ResponseAlternatives.Add(New SpeechTestResponseAlternative With {.Spelling = ContrastingWord.GetCategoricalVariableValue("Spelling"), .IsScoredItem = ContrastingWord.IsKeyComponent})
        Next

        'Shuffling the order of response alternatives
        ResponseAlternatives = Utils.Shuffle(ResponseAlternatives, Randomizer).ToList

        'Adding the response alternatives
        CurrentTestTrial.ResponseAlternativeSpellings.Add(ResponseAlternatives)

        'Mixing trial sound
        PrepareTestTrialSound()

        'Setting visual que intervals
        Dim ShowVisualQueTimer_Interval As Double
        Dim HideVisualQueTimer_Interval As Double
        Dim ShowResponseAlternativesTimer_Interval As Double
        Dim MaxResponseTimeTimer_Interval As Double

        If UseVisualQue = True Then
            ShowVisualQueTimer_Interval = System.Math.Max(1, DirectCast(CurrentTestTrial, SipTrial).TestWordStartTime * 1000)
            HideVisualQueTimer_Interval = System.Math.Max(2, DirectCast(CurrentTestTrial, SipTrial).TestWordCompletedTime * 1000)
            ShowResponseAlternativesTimer_Interval = HideVisualQueTimer_Interval + 1000 * ResponseAlternativeDelay 'TestSetup.CurrentEnvironment.TestSoundMixerSettings.ResponseAlternativeDelay * 1000
            MaxResponseTimeTimer_Interval = System.Math.Max(1, ShowResponseAlternativesTimer_Interval + 1000 * MaximumResponseTime)  ' TestSetup.CurrentEnvironment.TestSoundMixerSettings.MaximumResponseTime * 1000
        Else
            ShowResponseAlternativesTimer_Interval = System.Math.Max(1, DirectCast(CurrentTestTrial, SipTrial).TestWordStartTime * 1000) + 1000 * ResponseAlternativeDelay
            MaxResponseTimeTimer_Interval = System.Math.Max(2, DirectCast(CurrentTestTrial, SipTrial).TestWordCompletedTime * 1000) + 1000 * MaximumResponseTime
        End If

        'Setting trial events
        CurrentTestTrial.TrialEventList = New List(Of ResponseViewEvent)
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 1, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})
        If UseVisualQue = True Then
            CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = ShowVisualQueTimer_Interval, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowVisualCue})
            CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = HideVisualQueTimer_Interval, .Type = ResponseViewEvent.ResponseViewEventTypes.HideVisualCue})
        End If
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = ShowResponseAlternativesTimer_Interval, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = MaxResponseTimeTimer_Interval, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut})

    End Sub



    Private Function GetAverageQuickSipDirectionScores(ByVal QuickSipDirection As QuickSipDirections)

        Dim TestUnitIndices As New List(Of Integer)
        Select Case QuickSipDirection
            Case QuickSipDirections.SpeechFromFront
                For ui = 0 To 3
                    TestUnitIndices.Add(ui)
                Next
            Case QuickSipDirections.SpeechFromRight
                TestUnitIndices.Add(4)
            Case QuickSipDirections.SpeechFromLeft
                TestUnitIndices.Add(5)
        End Select

        Dim TrialScoreList As New List(Of Integer)
        For Each ui In TestUnitIndices
            Dim TestUnit = CurrentSipTestMeasurement.TestUnits(ui)
            For Each Trial In TestUnit.ObservedTrials
                TrialScoreList.Add(Trial.Score)
            Next
        Next

        If TrialScoreList.Count > 0 Then
            Return TrialScoreList.Average
        Else
            Return -1
        End If

    End Function


    Public Overrides Function GetResults() As TestResults

        Dim SkipExportOfSoundFiles As Boolean = True

        Dim TestResult As New TestResults(TestResults.TestResultTypes.QSiP)
        TestResult.FormattedTrialResults = New List(Of String)

        TestResult.TestResultSummaryLines = New List(Of String)

        TestResult.TestResultSummaryLines.Add("Speech in front: " & Math.Rounding(100 * GetAverageQuickSipDirectionScores(QuickSipDirections.SpeechFromFront)) & " %")
        TestResult.TestResultSummaryLines.Add("Speech from right : " & Math.Rounding(100 * GetAverageQuickSipDirectionScores(QuickSipDirections.SpeechFromRight)) & " %")
        TestResult.TestResultSummaryLines.Add("Speech from left: " & Math.Rounding(100 * GetAverageQuickSipDirectionScores(QuickSipDirections.SpeechFromLeft)) & " %")

        If Stage1ResultsSummary IsNot Nothing Then
            TestResult.TestResultSummaryLines.Add("Speech in front:")
            TestResult.TestResultSummaryLines.Add("PNR (dB)" & vbTab & "Score" & vbTab & "List")
            For Each kvp In Stage1ResultsSummary
                TestResult.TestResultSummaryLines.Add(kvp.Value.Item1.PNR & vbTab & Math.Rounding(100 * kvp.Value.Item2) & " %" & vbTab & kvp.Value.Item1.SMC.PrimaryStringRepresentation)
            Next
        End If

        'CurrentSipTestMeasurement.GetAverageObservedScore()

        For t = 0 To CurrentSipTestMeasurement.ObservedTrials.Count - 1

            Dim TrialList As New List(Of String)

            Dim Trial = CurrentSipTestMeasurement.ObservedTrials(t)

            If TestResult.FormattedTrialResultsHeadings = "" Then TestResult.FormattedTrialResultsHeadings = SipTrial.CreateExportHeadings()

            TrialList.Add(Trial.ParentTestUnit.ParentMeasurement.ParticipantID)
            TrialList.Add(Trial.ParentTestUnit.ParentMeasurement.MeasurementDateTime.ToString(System.Globalization.CultureInfo.InvariantCulture))
            TrialList.Add(Trial.ParentTestUnit.ParentMeasurement.Description)
            TrialList.Add(Trial.ParentTestUnit.ParentMeasurement.GetParentTestUnitIndex(Trial))
            TrialList.Add(Trial.ParentTestUnit.Description)
            TrialList.Add(Trial.SpeechMaterialComponent.Id)
            TrialList.Add(Trial.SpeechMaterialComponent.ParentComponent.PrimaryStringRepresentation)
            TrialList.Add(Trial.MediaSet.MediaSetName)
            TrialList.Add(Trial.PresentationOrder)
            TrialList.Add(Trial.ReferenceSpeechMaterialLevel_SPL)
            TrialList.Add(Trial.ReferenceContrastingPhonemesLevel_SPL)
            TrialList.Add(Trial.Reference_SPL)
            TrialList.Add(Trial.PNR)
            If Trial.TargetMasking_SPL.HasValue = True Then
                TrialList.Add(Trial.TargetMasking_SPL)
            Else
                TrialList.Add("NA")
            End If
            TrialList.Add(Trial.TestWordLevelLimit)
            TrialList.Add(Trial.ContextSpeechLimit)

            If Trial.ParentTestUnit.ParentMeasurement.SelectedAudiogramData IsNot Nothing Then
                TrialList.Add(Trial.EstimatedSuccessProbability(False))
                TrialList.Add(Trial.AdjustedSuccessProbability)
            Else
                TrialList.Add("No audiogram stored - cannot calculate")
                TrialList.Add("No audiogram stored - cannot calculate")
            End If
            TrialList.Add(Trial.SoundPropagationType.ToString)

            If Trial.TargetStimulusLocations.Length > 0 Then
                Dim Distances As New List(Of String)
                Dim HorizontalAzimuths As New List(Of String)
                Dim Elevations As New List(Of String)
                Dim ActualDistances As New List(Of String)
                Dim ActualHorizontalAzimuths As New List(Of String)
                Dim ActualElevations As New List(Of String)
                Dim ActualBinauralDelay_Left As New List(Of String)
                Dim ActualBinauralDelay_Right As New List(Of String)
                For i = 0 To Trial.TargetStimulusLocations.Length - 1
                    Distances.Add(Trial.TargetStimulusLocations(i).Distance)
                    HorizontalAzimuths.Add(Trial.TargetStimulusLocations(i).HorizontalAzimuth)
                    Elevations.Add(Trial.TargetStimulusLocations(i).Elevation)
                    If Trial.TargetStimulusLocations(i).ActualLocation Is Nothing Then Trial.TargetStimulusLocations(i).ActualLocation = New SoundSourceLocation
                    ActualDistances.Add(Trial.TargetStimulusLocations(i).ActualLocation.Distance)
                    ActualHorizontalAzimuths.Add(Trial.TargetStimulusLocations(i).ActualLocation.HorizontalAzimuth)
                    ActualElevations.Add(Trial.TargetStimulusLocations(i).ActualLocation.Elevation)
                    ActualBinauralDelay_Left.Add(Trial.TargetStimulusLocations(i).ActualLocation.BinauralDelay.LeftDelay)
                    ActualBinauralDelay_Right.Add(Trial.TargetStimulusLocations(i).ActualLocation.BinauralDelay.RightDelay)
                Next
                TrialList.Add(String.Join(";", Distances))
                TrialList.Add(String.Join(";", HorizontalAzimuths))
                TrialList.Add(String.Join(";", Elevations))
                TrialList.Add(String.Join(";", ActualDistances))
                TrialList.Add(String.Join(";", ActualHorizontalAzimuths))
                TrialList.Add(String.Join(";", ActualElevations))
                TrialList.Add(String.Join(";", ActualBinauralDelay_Left))
                TrialList.Add(String.Join(";", ActualBinauralDelay_Right))
            Else
                For n = 1 To 8
                    TrialList.Add("")
                Next
            End If

            If Trial.MaskerLocations.Length > 0 Then
                Dim Distances As New List(Of String)
                Dim HorizontalAzimuths As New List(Of String)
                Dim Elevations As New List(Of String)
                Dim ActualDistances As New List(Of String)
                Dim ActualHorizontalAzimuths As New List(Of String)
                Dim ActualElevations As New List(Of String)
                Dim ActualBinauralDelay_Left As New List(Of String)
                Dim ActualBinauralDelay_Right As New List(Of String)
                For i = 0 To Trial.MaskerLocations.Length - 1
                    Distances.Add(Trial.MaskerLocations(i).Distance)
                    HorizontalAzimuths.Add(Trial.MaskerLocations(i).HorizontalAzimuth)
                    Elevations.Add(Trial.MaskerLocations(i).Elevation)
                    If Trial.MaskerLocations(i).ActualLocation Is Nothing Then Trial.MaskerLocations(i).ActualLocation = New SoundSourceLocation
                    ActualDistances.Add(Trial.MaskerLocations(i).ActualLocation.Distance)
                    ActualHorizontalAzimuths.Add(Trial.MaskerLocations(i).ActualLocation.HorizontalAzimuth)
                    ActualElevations.Add(Trial.MaskerLocations(i).ActualLocation.Elevation)
                    ActualBinauralDelay_Left.Add(Trial.MaskerLocations(i).ActualLocation.BinauralDelay.LeftDelay)
                    ActualBinauralDelay_Right.Add(Trial.MaskerLocations(i).ActualLocation.BinauralDelay.RightDelay)
                Next
                TrialList.Add(String.Join(";", Distances))
                TrialList.Add(String.Join(";", HorizontalAzimuths))
                TrialList.Add(String.Join(";", Elevations))
                TrialList.Add(String.Join(";", ActualDistances))
                TrialList.Add(String.Join(";", ActualHorizontalAzimuths))
                TrialList.Add(String.Join(";", ActualElevations))
                TrialList.Add(String.Join(";", ActualBinauralDelay_Left))
                TrialList.Add(String.Join(";", ActualBinauralDelay_Right))
            Else
                For n = 1 To 8
                    TrialList.Add("")
                Next
            End If

            If Trial.BackgroundLocations.Length > 0 Then
                Dim Distances As New List(Of String)
                Dim HorizontalAzimuths As New List(Of String)
                Dim Elevations As New List(Of String)
                Dim ActualDistances As New List(Of String)
                Dim ActualHorizontalAzimuths As New List(Of String)
                Dim ActualElevations As New List(Of String)
                Dim ActualBinauralDelay_Left As New List(Of String)
                Dim ActualBinauralDelay_Right As New List(Of String)
                For i = 0 To Trial.BackgroundLocations.Length - 1
                    Distances.Add(Trial.BackgroundLocations(i).Distance)
                    HorizontalAzimuths.Add(Trial.BackgroundLocations(i).HorizontalAzimuth)
                    Elevations.Add(Trial.BackgroundLocations(i).Elevation)
                    If Trial.BackgroundLocations(i).ActualLocation Is Nothing Then Trial.BackgroundLocations(i).ActualLocation = New SoundSourceLocation
                    ActualDistances.Add(Trial.BackgroundLocations(i).ActualLocation.Distance)
                    ActualHorizontalAzimuths.Add(Trial.BackgroundLocations(i).ActualLocation.HorizontalAzimuth)
                    ActualElevations.Add(Trial.BackgroundLocations(i).ActualLocation.Elevation)
                    ActualBinauralDelay_Left.Add(Trial.BackgroundLocations(i).ActualLocation.BinauralDelay.LeftDelay)
                    ActualBinauralDelay_Right.Add(Trial.BackgroundLocations(i).ActualLocation.BinauralDelay.RightDelay)
                Next
                TrialList.Add(String.Join(";", Distances))
                TrialList.Add(String.Join(";", HorizontalAzimuths))
                TrialList.Add(String.Join(";", Elevations))
                TrialList.Add(String.Join(";", ActualDistances))
                TrialList.Add(String.Join(";", ActualHorizontalAzimuths))
                TrialList.Add(String.Join(";", ActualElevations))
                TrialList.Add(String.Join(";", ActualBinauralDelay_Left))
                TrialList.Add(String.Join(";", ActualBinauralDelay_Right))
            Else
                For n = 1 To 8
                    TrialList.Add("")
                Next
            End If

            TrialList.Add(Trial.IsBmldTrial)
            If Trial.IsBmldTrial = True Then
                TrialList.Add(Trial.BmldNoiseMode.ToString)
                TrialList.Add(Trial.BmldSignalMode.ToString)
            Else
                TrialList.Add("")
                TrialList.Add("")
            End If

            TrialList.Add(Trial.Response)
            TrialList.Add(Trial.Result.ToString)
            TrialList.Add(Trial.Score)
            TrialList.Add(Trial.ResponseTime.ToString(System.Globalization.CultureInfo.InvariantCulture))
            Trial.DetermineResponseAlternativeCount()
            If Trial.ResponseAlternativeCount.HasValue = True Then
                TrialList.Add(Trial.ResponseAlternativeCount.Value)
            Else
                TrialList.Add("")
            End If
            TrialList.Add(Trial.IsTestTrial.ToString)
            If Trial.ParentTestUnit.ParentMeasurement.SelectedAudiogramData IsNot Nothing Then
                TrialList.Add(Trial.PhonemeDiscriminabilityLevel(False))
            Else
                TrialList.Add("No audiogram stored")
            End If

            TrialList.Add(Trial.SpeechMaterialComponent.PrimaryStringRepresentation)
            TrialList.Add(Trial.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling"))
            TrialList.Add(Trial.SpeechMaterialComponent.GetCategoricalVariableValue("SpellingAFC"))
            TrialList.Add(Trial.SpeechMaterialComponent.GetCategoricalVariableValue("Transcription"))
            TrialList.Add(Trial.SpeechMaterialComponent.GetCategoricalVariableValue("TranscriptionAFC"))

            Dim PseudoTrialIds As New List(Of String)
            Dim PseudoTrialSpellings As New List(Of String)
            If Trial.PseudoTrials IsNot Nothing Then
                For Each PseudoTrial In Trial.PseudoTrials
                    PseudoTrialIds.Add(PseudoTrial.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling"))
                    PseudoTrialSpellings.Add(PseudoTrial.SpeechMaterialComponent.Id)
                Next
            End If
            TrialList.Add(String.Join("; ", PseudoTrialIds))
            TrialList.Add(String.Join("; ", PseudoTrialSpellings))

            'Adding export of sound files,
            Dim ExportedSoundFilesList As New List(Of String)
            If SkipExportOfSoundFiles = False Then
                For i = 0 To Trial.TrialSoundsToExport.Count - 1
                    Dim ExportSound = Trial.TrialSoundsToExport(i).Item2
                    Dim FileName = IO.Path.Combine(Trial.ParentTestUnit.ParentMeasurement.TrialResultsExportFolder, "TrialSoundFiles", "Trial_" & Trial.PresentationOrder & "_" & Trial.TrialSoundsToExport(i).Item1 & "_" & Trial.SpeechMaterialComponent.Id & ".wav")
                    ExportSound.WriteWaveFile(FileName)
                    ExportedSoundFilesList.Add(FileName)
                Next
            End If
            TrialList.Add(String.Join(";", ExportedSoundFilesList))

            Dim ExportedPseudoTrialSoundFilesList As New List(Of String)
            If SkipExportOfSoundFiles = False Then
                If Trial.PseudoTrials IsNot Nothing Then
                    For Each PseudoTrial In Trial.PseudoTrials
                        For i = 0 To PseudoTrial.TrialSoundsToExport.Count - 1
                            Dim ExportSound = PseudoTrial.TrialSoundsToExport(i).Item2
                            Dim FileName = IO.Path.Combine(Trial.ParentTestUnit.ParentMeasurement.TrialResultsExportFolder, "TrialSoundFiles", "Trial_" & Trial.PresentationOrder & "_Pseudo_" & PseudoTrial.TrialSoundsToExport(i).Item1 & "_" & PseudoTrial.SpeechMaterialComponent.Id & ".wav")
                            ExportSound.WriteWaveFile(FileName)
                            ExportedPseudoTrialSoundFilesList.Add(FileName)
                        Next
                    Next
                End If
            End If
            TrialList.Add(String.Join(";", ExportedPseudoTrialSoundFilesList))

            TrialList.Add(Trial.SelectedTargetIndexString)
            TrialList.Add(Trial.SelectedMaskerIndicesString)
            TrialList.Add(Trial.BackgroundStartSamplesString)
            TrialList.Add(Trial.BackgroundSpeechStartSamplesString)

            Dim PseudoTrial_SelectedTargetIndexStringList As New List(Of String)
            Dim PseudoTrial_SelectedMaskerIndicesStringList As New List(Of String)
            Dim PseudoTrial_BackgroundStartSamplesStringList As New List(Of String)
            Dim PseudoTrial_BackgroundSpeechStartSamplesStringList As New List(Of String)
            If Trial.PseudoTrials IsNot Nothing Then
                For Each PseduTrial In Trial.PseudoTrials
                    PseudoTrial_SelectedTargetIndexStringList.Add(PseduTrial.SelectedTargetIndexString)
                    PseudoTrial_SelectedMaskerIndicesStringList.Add(PseduTrial.SelectedMaskerIndicesString)
                    PseudoTrial_BackgroundStartSamplesStringList.Add(PseduTrial.BackgroundStartSamplesString)
                    PseudoTrial_BackgroundSpeechStartSamplesStringList.Add(PseduTrial.BackgroundSpeechStartSamplesString)
                Next
            End If
            TrialList.Add(String.Join(";", PseudoTrial_SelectedTargetIndexStringList))
            TrialList.Add(String.Join(";", PseudoTrial_SelectedMaskerIndicesStringList))
            TrialList.Add(String.Join(";", PseudoTrial_BackgroundStartSamplesStringList))
            TrialList.Add(String.Join(";", PseudoTrial_BackgroundSpeechStartSamplesStringList))

            TrialList.Add(Trial.BackgroundNonSpeechDucking)
            Dim PseudoTrial_BackgroundNonSpeechDuckingList As New List(Of String)
            If Trial.PseudoTrials IsNot Nothing Then
                For Each PseduTrial In Trial.PseudoTrials
                    PseudoTrial_BackgroundNonSpeechDuckingList.Add(PseduTrial.BackgroundNonSpeechDucking)
                Next
            End If
            TrialList.Add(String.Join(";", PseudoTrial_BackgroundNonSpeechDuckingList))

            TrialList.Add(Trial.ContextRegionSpeech_SPL)
            If Trial.TestWordLevel.HasValue = True Then
                TrialList.Add(Trial.TestWordLevel)
            Else
                TrialList.Add("NA")
            End If
            TrialList.Add(Trial.ReferenceTestWordLevel_SPL)

            Dim ContextRegionSpeech_SPL_List As New List(Of String)
            Dim TestWordLevel_List As New List(Of String)
            Dim ReferenceTestWordLevel_SPL_List As New List(Of String)
            If Trial.PseudoTrials IsNot Nothing Then
                For Each PseduTrial In Trial.PseudoTrials
                    ContextRegionSpeech_SPL_List.Add(PseduTrial.ContextRegionSpeech_SPL)
                    If PseduTrial.TestWordLevel.HasValue = True Then
                        TestWordLevel_List.Add(PseduTrial.TestWordLevel)
                    Else
                        TestWordLevel_List.Add("NA")
                    End If
                    ReferenceTestWordLevel_SPL_List.Add(PseduTrial.ReferenceTestWordLevel_SPL)
                Next
            End If
            TrialList.Add(String.Join(";", ContextRegionSpeech_SPL_List))
            TrialList.Add(String.Join(";", TestWordLevel_List))
            TrialList.Add(String.Join(";", ReferenceTestWordLevel_SPL_List))

            'Target Startsamples
            TrialList.Add(Trial.TargetStartSample)
            Dim PseudoTrials_TargetStartSample As New List(Of String)
            If Trial.PseudoTrials IsNot Nothing Then
                For Each PseduTrial In Trial.PseudoTrials
                    PseudoTrials_TargetStartSample.Add(PseduTrial.TargetStartSample)
                Next
            End If
            TrialList.Add(String.Join(";", PseudoTrials_TargetStartSample))

            'Test phoneme start sample and length
            If Trial.TargetInitialMargins.Count = 0 Then Trial.TargetInitialMargins.Add(0) ' Adding an initial margin of zero if for some reason empty
            Dim TP_SaL = Trial.GetTestPhonemeStartAndLength(Trial.TargetInitialMargins(0)) ' N.B. / TODO: Here initial margins are assumed only for one target. Need to be changed if several targets with different initial marginsa are to be used.
            Dim TestPhonemeStartSample As Integer = TP_SaL.Item1
            Dim TestPhonemelength As Integer = TP_SaL.Item2
            TrialList.Add(TestPhonemeStartSample)
            TrialList.Add(TestPhonemelength)

            Dim PseudoTrials_TP_StartSamples As New List(Of String)
            Dim PseudoTrials_TP_Length As New List(Of String)
            If Trial.PseudoTrials IsNot Nothing Then
                For pseudoTrialIndex = 0 To Trial.PseudoTrials.Count - 1
                    If Trial.PseudoTrials(pseudoTrialIndex).TargetInitialMargins.Count = 0 Then Trial.PseudoTrials(pseudoTrialIndex).TargetInitialMargins.Add(0) ' Adding an initial margin of zero if for some reason empty
                    Dim PS_TP_SaL = Trial.PseudoTrials(pseudoTrialIndex).GetTestPhonemeStartAndLength(Trial.PseudoTrials(pseudoTrialIndex).TargetInitialMargins(0)) ' N.B. / TODO: Here initial margins are assumed only for one target. Need to be changed if several targets with different initial marginsa are to be used.
                    PseudoTrials_TP_StartSamples.Add(PS_TP_SaL.Item1)
                    PseudoTrials_TP_Length.Add(PS_TP_SaL.Item2)
                Next
            End If
            TrialList.Add(String.Join(";", PseudoTrials_TP_StartSamples))
            TrialList.Add(String.Join(";", PseudoTrials_TP_Length))

            'Gains
            Dim TargetTrialGains As New List(Of String)
            For Each Item In Trial.GainList
                TargetTrialGains.Add(Item.Key.ToString & ": " & String.Join(";", Item.Value))
            Next
            TrialList.Add(String.Join(" / ", TargetTrialGains))

            Dim PseudoTrialsGains As New List(Of String)
            If Trial.PseudoTrials IsNot Nothing Then
                For pseudoTrialIndex = 0 To Trial.PseudoTrials.Count - 1
                    Dim PseudoTrialGains As New List(Of String)
                    For Each Item In Trial.PseudoTrials(pseudoTrialIndex).GainList
                        PseudoTrialGains.Add(Item.Key.ToString & ": " & String.Join(";", Item.Value))
                    Next
                    PseudoTrialsGains.Add(String.Join(" / ", PseudoTrialGains))
                Next
            End If
            TrialList.Add(String.Join(" | ", PseudoTrialsGains))

            TestResult.FormattedTrialResults.Add(String.Join(vbTab, TrialList))

        Next

        Return TestResult

    End Function

    Public Overrides Sub FinalizeTest()
        'Throw New NotImplementedException()
    End Sub

    Public Overrides Function CreatePreTestStimulus() As Tuple(Of Audio.Sound, String)
        Return Nothing
    End Function

    Public Overrides Sub UpdateHistoricTrialResults(sender As Object, e As SpeechTestInputEventArgs)
        'Not supported, just ignores any calls
    End Sub


End Class