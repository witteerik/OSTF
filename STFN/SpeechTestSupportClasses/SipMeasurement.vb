
'TODOs
'Should we neutralize TPD per testord instead of per recording in the SiP-test model?
'We have To fix the randomizers In the SIPtest. There are randomizers at several levels!


'Imports MathNet.Numerics

Imports STFN.Audio
Imports STFN.Audio.SoundScene
Imports STFN.Audio.Formats
Imports STFN.Audio.PortAudioVB.DuplexMixer

Namespace SipTest

    'N.B. Variable name abbreviations used:
    ' RLxs = ReferenceContrastingPhonemesLevel (dB FS)
    ' SLs = Phoneme Spectrum Levels (PSL, dB SPL)
    ' SLm = Environment / Masker Spectrum Levels (ESL, dB SPL)
    ' Lc = Component Level (TestWord_ReferenceSPL, dB SPL), The average sound level of all recordings of the speech material component
    ' Tc = Component temporal duration (in seconds)
    ' V = HasVowelContrast (1 = Yes, 0 = No)

    Public Class SipMeasurement

        Public Property Description As String = ""

        Public Property ParticipantID As String

        Public Property MeasurementDateTime As DateTime


        Public Property ParentTestSpecification As SpeechMaterialSpecification

        ''' <summary>
        ''' Stores the test units presented in the test session
        ''' </summary>
        ''' <returns></returns>
        Public Property TestUnits As New List(Of SiPTestUnit)

        ''' <summary>
        ''' Stores references to SiP-test trials in the order that they were presented.
        ''' </summary>
        ''' <returns></returns>
        Public Property ObservedTrials As New List(Of SipTrial)


        Public Property PlannedTrials As New List(Of SipTrial)

        ''' <summary>
        ''' Holds settings that determine how the test should enfold.
        ''' </summary>
        ''' <returns></returns>
        Public Property TestProcedure As TestProcedure


        Public Property SelectedAudiogramData As AudiogramData = Nothing
        Public Property HearingAidGain As HearingAidGainData = Nothing

        Friend Randomizer As Random

        Public ExportTrialSoundFiles As Boolean = False

        Public TrialResultsExportFolder As String


        Public Sub New(ByRef ParticipantID As String, ByRef ParentTestSpecification As SpeechMaterialSpecification, Optional AdaptiveType As AdaptiveTypes = AdaptiveTypes.Fixed, Optional ByVal TestParadigm As Testparadigm = Testparadigm.Slow, Optional RandomSeed As Integer? = Nothing)

            If RandomSeed.HasValue = True Then
                Randomizer = New Random(RandomSeed)
            Else
                Randomizer = New Random
            End If

            Me.TestProcedure = New TestProcedure(AdaptiveType, TestParadigm)

            Me.ParticipantID = ParticipantID
            Me.ParentTestSpecification = ParentTestSpecification

        End Sub

        Public Sub SetDefaultExportPath()
            Me.TrialResultsExportFolder = IO.Path.Combine(Utils.logFilePath, ParentTestSpecification.Name.Replace(" ", "_") & "_" & Description.Replace(" ", "_") & "_" & ParticipantID.Replace(" ", "_"))
        End Sub

#Region "Preparation"


        Public Sub PlanTestTrials(ByRef AvailableMediaSet As MediaSetLibrary, ByVal PresetName As String, ByVal MediaSetName As String, ByVal SoundPropagationType As SoundPropagationTypes, Optional ByVal RandomSeed As Integer? = Nothing)

            Dim Preset = ParentTestSpecification.SpeechMaterial.Presets.GetPretest(PresetName).Members

            PlanTestTrials(AvailableMediaSet, Preset, MediaSetName, SoundPropagationType, RandomSeed)

        End Sub


        Public Sub PlanTestTrials(ByRef AvailableMediaSet As MediaSetLibrary, ByVal Preset As List(Of SpeechMaterialComponent), ByVal MediaSetName As String, ByVal SoundPropagationType As SoundPropagationTypes, Optional ByVal RandomSeed As Integer? = Nothing)

            ClearTrials()

            'MediaSetName ' TODO: should we use the name or the mediaset in the GUI/Measurment?
            'TODO: If media set is not selected we could randomize between the available ones...

            Dim CurrentTargetLocations = TestProcedure.TargetStimulusLocations(Me.TestProcedure.TestParadigm)
            Dim MaskerLocations = TestProcedure.MaskerLocations(Me.TestProcedure.TestParadigm)
            Dim BackgroundLocations = TestProcedure.BackgroundLocations(Me.TestProcedure.TestParadigm)

            Select Case TestProcedure.AdaptiveType
                Case AdaptiveTypes.Fixed

                    If RandomSeed.HasValue Then Randomizer = New Random(RandomSeed)

                    For Each TargetLocation In CurrentTargetLocations

                        For r = 1 To TestProcedure.LengthReduplications

                            For Each PresetComponent In Preset

                                Dim NewTestUnit = New SiPTestUnit(Me)

                                Dim TestWords = PresetComponent.GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.Sentence)
                                NewTestUnit.SpeechMaterialComponents.AddRange(TestWords)

                                If MediaSetName <> "" Then
                                    'Adding from the selected media set
                                    NewTestUnit.PlanTrials(AvailableMediaSet.GetMediaSet(MediaSetName), SoundPropagationType, TargetLocation, MaskerLocations, BackgroundLocations)
                                    TestUnits.Add(NewTestUnit)
                                Else
                                    'Adding from random media sets
                                    Dim RandomIndex = Randomizer.Next(0, AvailableMediaSet.Count)
                                    NewTestUnit.PlanTrials(AvailableMediaSet(RandomIndex), SoundPropagationType, TargetLocation, MaskerLocations, BackgroundLocations)
                                    TestUnits.Add(NewTestUnit)
                                End If

                            Next
                        Next
                    Next

                    For Each Unit In TestUnits
                        For Each Trial In Unit.PlannedTrials
                            PlannedTrials.Add(Trial)
                        Next
                    Next

                    If TestProcedure.RandomizeOrder = True Then
                        Dim RandomList As New List(Of SipTrial)
                        Do Until PlannedTrials.Count = 0
                            Dim RandomIndex As Integer = Randomizer.Next(0, PlannedTrials.Count)
                            RandomList.Add(PlannedTrials(RandomIndex))
                            PlannedTrials.RemoveAt(RandomIndex)
                        Loop
                        PlannedTrials = RandomList
                    End If



                Case AdaptiveTypes.SimpleUpDown

                    If RandomSeed.HasValue Then Randomizer = New Random(RandomSeed)

                    For Each TargetLocation In CurrentTargetLocations

                        For r = 1 To TestProcedure.LengthReduplications

                            For Each PresetComponent In Preset

                                Dim NewTestUnit = New SiPTestUnit(Me)

                                Dim TestWords = PresetComponent.GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.Sentence)
                                NewTestUnit.SpeechMaterialComponents.AddRange(TestWords)

                                If MediaSetName <> "" Then
                                    'Adding from the selected media set
                                    NewTestUnit.PlanTrials(AvailableMediaSet.GetMediaSet(MediaSetName), SoundPropagationType, TargetLocation, MaskerLocations, BackgroundLocations)
                                    TestUnits.Add(NewTestUnit)
                                Else
                                    'Adding from random media sets
                                    Dim RandomIndex = Randomizer.Next(0, AvailableMediaSet.Count)
                                    NewTestUnit.PlanTrials(AvailableMediaSet(RandomIndex), SoundPropagationType, TargetLocation, MaskerLocations, BackgroundLocations)
                                    TestUnits.Add(NewTestUnit)
                                End If

                            Next
                        Next
                    Next

                    For Each Unit In TestUnits
                        For Each Trial In Unit.PlannedTrials
                            PlannedTrials.Add(Trial)
                        Next
                    Next

                    'If TestProcedure.RandomizeOrder = True Then
                    '    Dim RandomList As New List(Of SipTrial)
                    '    Do Until PlannedTrials.Count = 0
                    '        Dim RandomIndex As Integer = Randomizer.Next(0, PlannedTrials.Count)
                    '        RandomList.Add(PlannedTrials(RandomIndex))
                    '        PlannedTrials.RemoveAt(RandomIndex)
                    '    Loop
                    '    PlannedTrials = RandomList
                    'End If


                Case Else

                    Throw New NotImplementedException

            End Select


        End Sub

        'Looks through all trials and returns True as soon as a trial that requires directional sound field simulation is detected, or false if no trial requires directional sound field simulation.
        Public Function HasSimulatedSoundFieldTrials()

            For Each Trial In PlannedTrials
                If Trial.SoundPropagationType = SoundPropagationTypes.SimulatedSoundField Then Return True
            Next

            For Each Trial In ObservedTrials
                If Trial.SoundPropagationType = SoundPropagationTypes.SimulatedSoundField Then Return True
            Next

            For Each TestUnit In TestUnits
                For Each Trial In TestUnit.PlannedTrials
                    If Trial.SoundPropagationType = SoundPropagationTypes.SimulatedSoundField Then Return True
                Next
                For Each Trial In TestUnit.ObservedTrials
                    If Trial.SoundPropagationType = SoundPropagationTypes.SimulatedSoundField Then Return True
                Next
            Next

            Return False
        End Function

        Public Sub PreMixTestTrialSoundsOnNewTread(ByRef SelectedTransducer As AudioSystemSpecification,
                                         ByVal MinimumStimulusOnsetTime As Double, ByVal MaximumStimulusOnsetTime As Double,
                                         ByRef SipMeasurementRandomizer As Random, ByVal TrialSoundMaxDuration As Double, ByVal UseBackgroundSpeech As Boolean,
                                         Optional ByVal StopAfter As Integer? = 10, Optional SameRandomSeed As Integer? = Nothing, Optional ByVal FixedMaskerIndices As List(Of Integer) = Nothing)

            Dim NewTestTrialSoundMixClass = New TestTrialSoundMixerOnNewThread(Me, SelectedTransducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, SipMeasurementRandomizer, TrialSoundMaxDuration, UseBackgroundSpeech,
                                                                               StopAfter, SameRandomSeed, FixedMaskerIndices)


        End Sub

        Private Class TestTrialSoundMixerOnNewThread

            Public SipMeasurement As SipMeasurement
            Public SelectedTransducer As AudioSystemSpecification
            Public MinimumStimulusOnsetTime As Double
            Public MaximumStimulusOnsetTime As Double
            Public SipMeasurementRandomizer As Random
            Public TrialSoundMaxDuration As Double
            Public UseBackgroundSpeech As Boolean
            Public StopAfter As Integer? = 10

            Public SameRandomSeed As Integer? = Nothing
            Public FixedMaskerIndices As List(Of Integer) = Nothing

            Public Sub New(ByRef SipMeasurement As SipMeasurement, ByRef SelectedTransducer As AudioSystemSpecification,
                                         ByVal MinimumStimulusOnsetTime As Double, ByVal MaximumStimulusOnsetTime As Double,
                                         ByRef SipMeasurementRandomizer As Random, ByVal TrialSoundMaxDuration As Double, ByVal UseBackgroundSpeech As Boolean,
                                         Optional ByVal StopAfter As Integer? = 10,
                           Optional SameRandomSeed As Integer? = Nothing, Optional ByVal FixedMaskerIndices As List(Of Integer) = Nothing)

                Me.SipMeasurement = SipMeasurement
                Me.SelectedTransducer = SelectedTransducer
                Me.MinimumStimulusOnsetTime = MinimumStimulusOnsetTime
                Me.MaximumStimulusOnsetTime = MaximumStimulusOnsetTime
                Me.SipMeasurementRandomizer = SipMeasurementRandomizer
                Me.TrialSoundMaxDuration = TrialSoundMaxDuration
                Me.UseBackgroundSpeech = UseBackgroundSpeech
                Me.StopAfter = StopAfter

                Me.SameRandomSeed = SameRandomSeed
                Me.FixedMaskerIndices = FixedMaskerIndices

                Dim NewTread As New Threading.Thread(AddressOf DoWork)
                NewTread.IsBackground = True
                NewTread.Start()

            End Sub

            Public Sub DoWork()
                SipMeasurement.PreMixTestTrialSounds(SelectedTransducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, SipMeasurementRandomizer, TrialSoundMaxDuration, UseBackgroundSpeech, StopAfter,
                                                     SameRandomSeed, FixedMaskerIndices)
            End Sub

        End Class

        Public Sub PreMixTestTrialSounds(ByRef SelectedTransducer As AudioSystemSpecification,
                                         ByVal MinimumStimulusOnsetTime As Double, ByVal MaximumStimulusOnsetTime As Double,
                                         ByRef SipMeasurementRandomizer As Random, ByVal TrialSoundMaxDuration As Double, ByVal UseBackgroundSpeech As Boolean,
                                         Optional ByVal StopAfter As Integer? = 10, Optional SameRandomSeed As Integer? = Nothing,
                                         Optional ByVal FixedMaskerIndices As List(Of Integer) = Nothing, Optional ByVal FixedSpeechIndex As Integer? = Nothing)

            Dim MixedCount As Integer = 0
            Dim RemainingPlannedTrials = PlannedTrials.GetRange(0, PlannedTrials.Count)

            If LogToConsole = True Then
                If StopAfter.HasValue Then
                    Console.WriteLine("Starts mixing " & StopAfter.Value & " trial sounds...")
                Else
                    Console.WriteLine("Starts mixing new trial sounds...")
                End If
            End If

            Dim LastListId As String = ""

            For Each Trial In RemainingPlannedTrials

                If SameRandomSeed.HasValue Then

                    If LastListId <> "" Then
                        LastListId = Trial.SpeechMaterialComponent.GetAncestorAtLevel(SpeechMaterialComponent.LinguisticLevels.List).Id
                    Else
                        If LastListId <> Trial.SpeechMaterialComponent.GetAncestorAtLevel(SpeechMaterialComponent.LinguisticLevels.List).Id Then
                            'Incrementing SameRandomSeed by 1 to get a new random value for the new list
                            SameRandomSeed += 1
                        End If
                    End If

                    'Forces the same random series for every trial in the same list level (to force the background sound sections in all trials)
                    SipMeasurementRandomizer = New Random(SameRandomSeed)
                End If

                If Trial.Sound Is Nothing Then
                    Trial.MixSound(SelectedTransducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, SipMeasurementRandomizer, TrialSoundMaxDuration, UseBackgroundSpeech, FixedMaskerIndices, FixedSpeechIndex)
                    MixedCount += 1

                    If LogToConsole = True Then
                        If Trial.Sound IsNot Nothing Then
                            Console.WriteLine("Mixed trial sound: " & MixedCount)
                        Else
                            Console.WriteLine("   Failed to mix trial sound: " & MixedCount)
                        End If
                    End If

                    'Stops after mixing StopAfter new sounds (this can be utilized in order not to bulid up too much memory)
                    If StopAfter.HasValue Then
                        If MixedCount >= StopAfter.Value Then Exit For
                    End If
                End If
            Next

        End Sub

        Public Sub ClearTrials()
            TestUnits.Clear()
            PlannedTrials.Clear()
            ObservedTrials.Clear()
        End Sub

        ''' <summary>
        ''' Sets all levels in all trials. (Levels should be set prior to mixing the sounds, or prior to success probability estimation.)
        ''' </summary>
        Public Sub SetLevels(ByVal ReferenceLevel As Double, ByVal PNR As Double)

            Select Case TestProcedure.AdaptiveType
                Case AdaptiveTypes.Fixed

                    'Setting the same reference level and PNR to all test trials
                    For Each TestUnit In Me.TestUnits
                        For Each TestTrial In TestUnit.PlannedTrials
                            TestTrial.SetLevels(ReferenceLevel, PNR)
                        Next
                    Next

                Case Else
                    Throw New NotImplementedException

            End Select


        End Sub

#End Region


#Region "RunTest"

        Public Function GetNextTrial() As SipTrial

            If PlannedTrials.Count = 0 Then
                Return Nothing
            Else

                'Returns the next planed trial
                Return PlannedTrials(0)
            End If

        End Function

        ''' <summary>
        ''' Moves the referenced test trial from the PlannedTrials to ObservedTrials objects, both in the parent TestUnit and in the parent SipMeasurement. This way it will not be presented again.
        ''' </summary>
        ''' <param name="TestTrial"></param>
        ''' <param name="RemoveSounds">If True, removes the Sound in the referenced trial.</param>
        Public Sub MoveTrialToHistory(ByRef TestTrial As SipTrial, Optional ByVal RemoveSounds As Boolean = True)

            Dim ParentTestUnit = TestTrial.ParentTestUnit

            'Adding the trial to the history
            ObservedTrials.Add(TestTrial)
            ParentTestUnit.ObservedTrials.Add(TestTrial)

            'Removes the next trial from PlannedTrials
            PlannedTrials.Remove(TestTrial)
            ParentTestUnit.PlannedTrials.Remove(TestTrial)

            'Increments the number of presented trials, based on the number of trials stored in ObservedTrials so far.
            TestTrial.PresentationOrder = ObservedTrials.Count

            If RemoveSounds = True Then
                TestTrial.RemoveSounds()
            End If

        End Sub



        Public Function GetNextTestUnit(ByRef rnd As Random) As SiPTestUnit

            Dim RemainingTestUnits As New List(Of SiPTestUnit)
            For Each TestUnit In TestUnits
                If TestUnit.IsCompleted = False Then
                    RemainingTestUnits.Add(TestUnit)
                    'Skips after adding the first remainig test unit (thus they get tested in order)
                    If TestProcedure.RandomizeOrder = True Then Exit For
                End If
            Next

            If RemainingTestUnits.Count = 0 Then
                Return Nothing
            ElseIf RemainingTestUnits.Count = 1 Then
                Return RemainingTestUnits(0)
            Else
                'Randomizes among the remaining test units
                Dim RandomIndex = rnd.Next(0, RemainingTestUnits.Count)
                Return RemainingTestUnits(RandomIndex)
            End If

        End Function

        Public Function GetGuiTableData(Optional ByVal ShowUnit As Boolean = True, Optional ByVal ShowPnr As Boolean = True) As GuiTableData

            Dim Output As New GuiTableData

            'Adding already tested trials
            For i = 0 To ObservedTrials.Count - 1

                Dim TestWordPrefix As String = ""
                Dim TestWordSuffix As String = ""

                If ObservedTrials(i).IsTestTrial = False Then
                    TestWordPrefix = "( "
                    TestWordSuffix = " )"
                End If

                'Adding unit description
                If ShowUnit = True Then
                    If ObservedTrials(i).ParentTestUnit IsNot Nothing Then
                        TestWordSuffix = TestWordSuffix & " / [" & ObservedTrials(i).ParentTestUnit.Description & "]"
                    End If
                End If

                'Adding also PNR
                If ShowPnr = True Then TestWordSuffix = TestWordSuffix & " / " & ObservedTrials(i).PNR

                Select Case TestProcedure.TestParadigm
                    Case Testparadigm.Directional2, Testparadigm.Directional3, Testparadigm.Directional5
                        Output.TestWords.Add(TestWordPrefix & ObservedTrials(i).SpeechMaterialComponent.PrimaryStringRepresentation & ", " & ObservedTrials(i).TargetStimulusLocations(0).ActualLocation.HorizontalAzimuth & TestWordSuffix)  'It is also possible to use a custom variable here, such as: ...SpeechMaterial.GetCategoricalVariableValue("Spelling")) 
                    Case Else
                        Output.TestWords.Add(TestWordPrefix & ObservedTrials(i).SpeechMaterialComponent.PrimaryStringRepresentation & TestWordSuffix) 'It is also possible to use a custom variable here, such as: ...SpeechMaterial.GetCategoricalVariableValue("Spelling")) 
                End Select
                Output.Responses.Add(ObservedTrials(i).Response.Replace(vbTab, ", "))
                Output.ResponseType.Add(ObservedTrials(i).Result)
            Next

            'Adding trials yet to be tested
            For i = 0 To PlannedTrials.Count - 1

                Dim TestWordPrefix As String = ""
                Dim TestWordSuffix As String = ""

                If PlannedTrials(i).IsTestTrial = False Then
                    TestWordPrefix = "( "
                    TestWordSuffix = " )"
                End If

                'Adding unit description
                If ShowUnit = True Then
                    If PlannedTrials(i).ParentTestUnit IsNot Nothing Then
                        TestWordSuffix = TestWordSuffix & " / [" & PlannedTrials(i).ParentTestUnit.Description & "]"
                    End If
                End If

                'Adding also PNR
                If ShowPnr = True Then TestWordSuffix = TestWordSuffix & " / " & PlannedTrials(i).PNR

                Select Case TestProcedure.TestParadigm
                    Case Testparadigm.Directional2, Testparadigm.Directional3, Testparadigm.Directional5
                        Output.TestWords.Add(TestWordPrefix & PlannedTrials(i).SpeechMaterialComponent.PrimaryStringRepresentation & ", " & PlannedTrials(i).TargetStimulusLocations(0).ActualLocation.HorizontalAzimuth & TestWordSuffix) 'It is also possible to use a custom variable here, such as: ...SpeechMaterial.GetCategoricalVariableValue("Spelling")) 
                    Case Else
                        Output.TestWords.Add(TestWordPrefix & PlannedTrials(i).SpeechMaterialComponent.PrimaryStringRepresentation & TestWordSuffix) 'It is also possible to use a custom variable here, such as: ...SpeechMaterial.GetCategoricalVariableValue("Spelling")) 
                End Select
                Output.Responses.Add("")
                Output.ResponseType.Add(PossibleResults.Missing)
            Next

            'Getting the index of the last presented trial
            Dim LastPresentedTrialIndex As Integer = ObservedTrials.Count
            'Setting the selection to the next trial, limited by the total number of trials
            Output.SelectionRow = Math.Min(PlannedTrials.Count + ObservedTrials.Count - 1, LastPresentedTrialIndex)
            Output.FirstRowToDisplayInScrollmode = Math.Max(0, LastPresentedTrialIndex - 7)

            'Overriding values if no rows exist
            If PlannedTrials.Count = 0 And ObservedTrials.Count = 0 Then
                Output.SelectionRow = Nothing
                Output.FirstRowToDisplayInScrollmode = Nothing
            End If

            'And also if do not select any row if all trials have been testet
            If PlannedTrials.Count = 0 Then
                Output.SelectionRow = Nothing
            End If

            Return Output

        End Function

        Public Class GuiTableData
            Public TestWords As New List(Of String)
            Public Responses As New List(Of String)
            Public ResponseType As New List(Of SipTest.PossibleResults)
            Public UpdateRow As Integer? = Nothing
            Public SelectionRow As Integer? = Nothing
            Public FirstRowToDisplayInScrollmode As Integer? = Nothing
        End Class


#End Region

#Region "Estimation"

        Public Property EstimatedMeanScore As Double

        Public Function CalculateEstimatedPsychometricFunction(ByVal ReferenceLevel As Double, Optional ByVal PNRs As List(Of Double) = Nothing, Optional ByVal SkipCriticalDifferenceCalculation As Boolean = False) As SortedList(Of Double, Tuple(Of Double, Double, Double))

            If PNRs Is Nothing Then
                PNRs = New List(Of Double) From {-15, -12, -10, -8, -6, -4, -2, 0, 2, 4, 6, 8, 10, 12, 15}
            End If

            'Pnr, Estimate, lower critical boundary, upper critical boundary
            Dim Output As New SortedList(Of Double, Tuple(Of Double, Double, Double))

            For Each pnr In PNRs

                Me.SetLevels(ReferenceLevel, pnr)

                Dim EstimatedScore = Me.CalculateEstimatedMeanScore(SkipCriticalDifferenceCalculation)

                If EstimatedScore IsNot Nothing Then
                    Output.Add(pnr, EstimatedScore)
                End If

            Next

            Return Output

        End Function

        Public Function GetAllTrials() As List(Of SipTrial)

            Dim AllTrials As New List(Of SipTrial)
            For Each PlannedTestTrial In Me.PlannedTrials
                AllTrials.Add(PlannedTestTrial)
            Next
            For Each ObservedTestTrial In Me.ObservedTrials
                AllTrials.Add(ObservedTestTrial)
            Next
            Return AllTrials

        End Function

        Public Function CalculateEstimatedMeanScore(ByVal SkipCriticalDifferenceCalculation As Boolean) As Tuple(Of Double, Double, Double)

            Dim TrialSuccessProbabilityList As New List(Of Double)

            Dim AllTrials = GetAllTrials()

            For Each Trial In AllTrials
                TrialSuccessProbabilityList.Add(Trial.EstimatedSuccessProbability(True))
            Next


            If TrialSuccessProbabilityList.Count > 0 Then
                If SkipCriticalDifferenceCalculation = False Then
                    Dim CriticalDifferenceLimits = CriticalDifferences.GetCriticalDifferenceLimits_PBAC(TrialSuccessProbabilityList.ToArray, TrialSuccessProbabilityList.ToArray)
                    Return New Tuple(Of Double, Double, Double)(TrialSuccessProbabilityList.Average(), CriticalDifferenceLimits.Item1, CriticalDifferenceLimits.Item2)
                Else
                    Return New Tuple(Of Double, Double, Double)(TrialSuccessProbabilityList.Average(), Double.NaN, Double.NaN)
                End If
            Else
                Return Nothing
            End If

        End Function

#End Region






#Region "TestResultSummary"

        Public ReadOnly Property ObservedTestLength As Integer
            Get
                Return Me.ObservedTrials.Count
            End Get
        End Property

        Public Function PercentCorrect() As String
            Dim LocalAverageScore = GetAverageObservedScore()

            If LocalAverageScore = -1 Then
                Return ""
            Else
                Return Math.Round(100 * LocalAverageScore)
            End If
        End Function


        Public Sub SummarizeTestResults()

            'Preparing for significance testing
            Me.CalculateAdjustedSuccessProbabilities()

            'TODO: Perhaps export/save data here?

        End Sub

        Public Sub CalculateAdjustedSuccessProbabilities()

            Dim UnadjustedEstimates As New List(Of Double)
            Dim Floors(Me.ObservedTrials.Count - 1) As Double

            For n = 0 To Me.ObservedTrials.Count - 1
                UnadjustedEstimates.Add(ObservedTrials(n).EstimatedSuccessProbability(True))

                'Gets the number of response alternatives
                Dim ResponseAlternativeCount As Integer = 0
                If ObservedTrials(n).DetermineResponseAlternativeCount() = True Then
                    ResponseAlternativeCount = ObservedTrials(n).ResponseAlternativeCount
                Else
                    Throw New Exception("Unable to determine the number of response alternatives!")
                End If

                'Calulates the floors of the psychometric functions (of each trial) based on the number of response alternatives
                If ResponseAlternativeCount > 0 Then
                    Floors(n) = 1 / ResponseAlternativeCount
                Else
                    Floors(n) = 0
                End If

            Next


            'Gets the target average score to adjust the unadjusted estimates to
            Dim LocalAverageScore = GetAverageObservedScore()

            'Creates adjusted estimates
            Dim AdjustedEstimates = STFN.CriticalDifferences.AdjustSuccessProbabilities(UnadjustedEstimates.ToArray, LocalAverageScore, Floors.ToArray)
            For n = 0 To Me.ObservedTrials.Count - 1
                Me.ObservedTrials(n).AdjustedSuccessProbability = AdjustedEstimates(n)
            Next

        End Sub


        ''' <summary>
        ''' Returns the number of observed correct trials as Item1, counting missing responses as correct every ResponseAlternativeCount:th time, and the total number of observed trials as Item2. Returns Nothing if no tested trials exist.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetNumberObservedScore() As Tuple(Of Integer, Integer)

            If Me.ObservedTrials.Count = 0 Then Return Nothing

            Dim Correct As Integer = 0
            Dim Total As Integer = Me.ObservedTrials.Count
            For n = 0 To Me.ObservedTrials.Count - 1

                If Me.ObservedTrials(n).Result = PossibleResults.Correct Then
                    Correct += 1

                ElseIf Me.ObservedTrials(n).Result = PossibleResults.Missing Then

                    If Me.ObservedTrials(n).ResponseAlternativeCount > 0 Then
                        If n Mod Me.ObservedTrials(n).ResponseAlternativeCount = (Me.ObservedTrials(n).ResponseAlternativeCount - 1) Then
                            Correct += 1
                        End If
                    End If
                End If

            Next

            Return New Tuple(Of Integer, Integer)(Correct, Total)

        End Function


        ''' <summary>
        ''' Returns the average score, counting missing responses as correct every ResponseAlternativeCount:th time. Returns -1 if no tested trials exist.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetAverageObservedScore() As Double

            If Me.ObservedTrials.Count = 0 Then Return -1

            Dim ScoreSoFar = GetNumberObservedScore()
            Return ScoreSoFar.Item1 / ScoreSoFar.Item2

        End Function

        Public Function GetAdjustedSuccessProbabilities() As Double()
            Dim OutputList As New List(Of Double)
            For n = 0 To Me.ObservedTrials.Count - 1
                OutputList.Add(Me.ObservedTrials(n).AdjustedSuccessProbability)
            Next
            Return OutputList.ToArray
        End Function

        Public Const NewMeasurementMarker As String = "<New OSFT measurement>"

        Public Function CreateExportString(Optional ByVal SkipExportOfSoundFiles As Boolean = True) As String

            Dim OutputLines As New List(Of String)

            OutputLines.Add(NewMeasurementMarker)

            OutputLines.Add(SipTrial.CreateExportHeadings())

            For t = 0 To Me.ObservedTrials.Count - 1

                Dim Trial As SipTrial = Me.ObservedTrials(t)

                Dim TrialExportString = Trial.CreateExportString(SkipExportOfSoundFiles)

                OutputLines.Add(TrialExportString)

            Next

            Return String.Join(vbCrLf, OutputLines)

        End Function


        ''' <summary>
        ''' Returns the index at which the ParentTestUnit of the Referenced TestTrial is stored within the TestUnits list of the current instance of SiPMeasurement, or -1 if the test unit does not exist, or cannot be found.
        ''' </summary>
        ''' <param name="TestTrial"></param>
        ''' <returns></returns>
        Public Function GetParentTestUnitIndex(ByRef TestTrial As SipTrial) As Integer

            If TestTrial Is Nothing Then Return -1
            If TestTrial.ParentTestUnit Is Nothing Then Return -1

            For i = 0 To TestUnits.Count - 1
                If TestTrial.ParentTestUnit Is TestUnits(i) Then
                    Return i
                End If
            Next

            Return -1
        End Function

        Public Shared Function ParseImportLines(ByVal ImportLines() As String, ByRef Randomizer As Random, ByRef ParentTestSpecification As SpeechMaterialSpecification, Optional ByVal ParticipantID As String = "") As SipMeasurement

            MsgBox("Parsing on SiP-test scores are temporarily inactivated.")
            Return Nothing

            Dim Output As SipMeasurement = Nothing

            Dim LoadedTestUnits As New SortedList(Of Integer, SiPTestUnit)

            Dim LastMainTrial As SipTrial = Nothing

            For i = 0 To ImportLines.Length - 1
                Dim Line As String = ImportLines(i)

                'Skips the heading line
                If i = 0 Then Continue For

                'Skips empty lines
                If Line.Trim = "" Then Continue For

                'Skips outcommented lines
                If Line.Trim.StartsWith("//") Then Continue For

                'Parses and adds trial
                Dim LineColumns = Line.Split({"//"}, StringSplitOptions.None)(0).Trim.Split(vbTab) ' A comment can be added after // in the input file

                If LineColumns.Length < 21 Then
                    MsgBox("Error when loading line " & Line & vbCrLf & vbCrLf & "Not enough columns!", MsgBoxStyle.Exclamation, "Missing columns in imported measurement file!")
                    Return Nothing
                End If

                Dim c As Integer = 0
                Dim Loaded_ParticipantID As String = LineColumns(c)
                c += 1
                Dim MeasurementDateTime As DateTime = DateTime.Parse(LineColumns(c), System.Globalization.CultureInfo.InvariantCulture)
                c += 1
                Dim Description As String = LineColumns(c)
                c += 1
                Dim ParentTestUnitIndex As Integer = LineColumns(c)
                c += 1
                Dim ParentTestUnitDescription As String = LineColumns(c)
                c += 1
                Dim SpeechMaterialComponentID As String = LineColumns(c)
                c += 1
                Dim MediaSetName As String = LineColumns(c)
                c += 1
                Dim PresentationOrder As Integer = LineColumns(c)
                c += 1
                Dim Reference_SPL As Double = LineColumns(c)
                c += 1
                Dim PNR As Double = LineColumns(c)
                c += 1
                Dim EstimatedSuccessProbability As Double = LineColumns(c)
                c += 1
                Dim AdjustedSuccessProbability As Double = LineColumns(c)
                c += 1
                Dim SoundPropagationType As SoundPropagationTypes = [Enum].Parse(GetType(SoundPropagationTypes), LineColumns(c))
                c += 1

                Dim TargetLocations = New List(Of SoundSourceLocation)
                'TODO: The code below, importing target locations, is not optimal, and will crash if the number of semicolon delimited values differ (if someone has tampered with the exported file)
                Dim TargetLocationDistances = LineColumns(c).Trim.Split(";")
                If TargetLocationDistances.Length > 0 Then
                    For t = 0 To TargetLocationDistances.Length - 1
                        TargetLocations.Add(New SoundSourceLocation)
                    Next
                    For t = 0 To TargetLocationDistances.Length - 1
                        TargetLocations(i).ActualLocation = New SoundSourceLocation
                        TargetLocations(i).Distance = TargetLocationDistances(i)
                    Next
                End If
                c += 1
                Dim TargetLocationHorizontalAzimuths = LineColumns(c).Trim.Split(";")
                If TargetLocationHorizontalAzimuths.Length > 0 Then
                    For t = 0 To TargetLocationHorizontalAzimuths.Length - 1
                        TargetLocations(t).HorizontalAzimuth = LineColumns(c)
                    Next
                End If
                c += 1
                Dim TargetLocationElevations = LineColumns(c).Trim.Split(";")
                If TargetLocationElevations.Length > 0 Then
                    For t = 0 To TargetLocationElevations.Length - 1
                        TargetLocations(t).Elevation = LineColumns(c)
                    Next
                End If
                c += 1
                Dim TargetLocationActualLocationDistances = LineColumns(c).Trim.Split(";")
                If TargetLocationActualLocationDistances.Length > 0 Then
                    For t = 0 To TargetLocationActualLocationDistances.Length - 1
                        TargetLocations(t).ActualLocation.Distance = LineColumns(c)
                    Next
                End If
                c += 1
                Dim TargetLocationActualLocationHorizontalAzimuths = LineColumns(c).Trim.Split(";")
                If TargetLocationActualLocationHorizontalAzimuths.Length > 0 Then
                    For t = 0 To TargetLocationActualLocationHorizontalAzimuths.Length - 1
                        TargetLocations(t).ActualLocation.HorizontalAzimuth = LineColumns(c)
                    Next
                End If
                c += 1
                Dim TargetLocationActualLocationElevations = LineColumns(c).Trim.Split(";")
                If TargetLocationActualLocationElevations.Length > 0 Then
                    For t = 0 To TargetLocationActualLocationElevations.Length - 1
                        TargetLocations(t).ActualLocation.Elevation = LineColumns(c)
                    Next
                End If
                c += 1

                Dim IsBmldTrial As Boolean = Boolean.Parse(LineColumns(c))
                c += 1
                Dim BmldNoiseMode As BmldModes
                If LineColumns(c).Trim.Length > 0 Then
                    BmldNoiseMode = [Enum].Parse(GetType(BmldModes), LineColumns(c))
                End If
                c += 1

                Dim BmldSignalMode As BmldModes
                If LineColumns(c).Trim.Length > 0 Then
                    BmldSignalMode = [Enum].Parse(GetType(BmldModes), LineColumns(c))
                End If
                c += 1

                Dim Response As String = LineColumns(c)
                c += 1
                Dim Result As SipTest.PossibleResults = [Enum].Parse(GetType(SipTest.PossibleResults), LineColumns(c))
                c += 1
                Dim ResponseTime As Integer = LineColumns(c)
                c += 1
                Dim ResponseAlternativeCount As Integer = LineColumns(c)
                c += 1
                Dim IsTestTrial As Boolean = Boolean.Parse(LineColumns(c))
                c += 1
                Dim PDL As Double = LineColumns(c)
                c += 1

                'TODO, these need to be exported and imported. Values are semicolon delimited.
                Dim MaskerLocations As New List(Of SoundSourceLocation)
                Dim BackgroundLocations As New List(Of SoundSourceLocation)

                'Checks, for every data row, that the participant ID is correct, and exits otherwise (in order not to mistakingly mix data from different participants when importing measurments)
                If Loaded_ParticipantID <> ParticipantID Then
                    MsgBox("The participant ID in the imported files differ from the selected participant. Aborts data import.", MsgBoxStyle.Exclamation, "Differing participant IDs")
                    Return Nothing
                End If

                'Creates a new Output only after we've got the participantID from the first data line
                If Output Is Nothing Then Output = New SipMeasurement(Loaded_ParticipantID, ParentTestSpecification)

                'Stores the MeasurementDateTime and Description
                Output.MeasurementDateTime = MeasurementDateTime
                Output.Description = Description

                'Creates a test unit if not existing (and storing it temporarily in LoadedTestUnits
                If Not LoadedTestUnits.ContainsKey(ParentTestUnitIndex) Then LoadedTestUnits.Add(ParentTestUnitIndex, New SiPTestUnit(Output))

                'Getting the SpeechMaterial, media set and (re-)creates the test trial
                Dim SpeechMaterialComponent = ParentTestSpecification.SpeechMaterial.GetComponentById(SpeechMaterialComponentID)
                Dim MediaSet = ParentTestSpecification.MediaSets.GetMediaSet(MediaSetName)
                Dim NewTestTrial As SipTrial
                If IsBmldTrial = False Then
                    NewTestTrial = New SipTrial(LoadedTestUnits(ParentTestUnitIndex), SpeechMaterialComponent, MediaSet, SoundPropagationType, TargetLocations.ToArray, MaskerLocations.ToArray, BackgroundLocations.ToArray, Randomizer)
                Else
                    NewTestTrial = New SipTrial(LoadedTestUnits(ParentTestUnitIndex), SpeechMaterialComponent, MediaSet, SoundPropagationType, BmldSignalMode, BmldNoiseMode, Randomizer)
                End If

                'Stores the remaining test trial data
                'NewTestTrial.PresentationOrder = PresentationOrder 'This is not stored as the export/import should always be ordered in the presentation order, as they are read from and stored into the ObservedTrials object!
                NewTestTrial.ParentTestUnit.Description = ParentTestUnitDescription
                NewTestTrial.Reference_SPL = Reference_SPL
                NewTestTrial.PNR = PNR
                NewTestTrial.OverideEstimatedSuccessProbabilityValue(EstimatedSuccessProbability)
                NewTestTrial.AdjustedSuccessProbability = AdjustedSuccessProbability
                NewTestTrial.Response = Response
                NewTestTrial.Result = Result
                NewTestTrial.ResponseTime = ResponseTime
                NewTestTrial.ResponseAlternativeCount = ResponseAlternativeCount
                NewTestTrial.IsTestTrial = IsTestTrial
                NewTestTrial.SetPhonemeDiscriminabilityLevelExternally(PDL)

                'Adding the loaded trial into ObservedTrials
                Output.ObservedTrials.Add(NewTestTrial)

                'Adding it also to the observed trial in the test unit
                LoadedTestUnits(ParentTestUnitIndex).ObservedTrials.Add(NewTestTrial)

            Next

            'Referencing the temporarily contained values in LoadedTestUnits as Output.TestUnits 
            Output.TestUnits = LoadedTestUnits.Values.ToList

            Return Output

        End Function

        Public Sub ExportMeasurement(Optional ByVal FilePath As String = "")

            'Gets a file path from the user if none is supplied
            If FilePath = "" Then FilePath = Utils.GetSaveFilePath(,, {".txt"}, "Save stuctured measurement history .txt file as...")
            If FilePath = "" Then
                MsgBox("No file selected!")
                Exit Sub
            End If

            Dim ExportString = CreateExportString()

            Utils.SendInfoToLog(ExportString, IO.Path.GetFileNameWithoutExtension(FilePath), IO.Path.GetDirectoryName(FilePath), True, True)

        End Sub

        Public Shared Function ImportSummary(ByRef ParentTestSpecification As SpeechMaterialSpecification, ByRef Randomizer As Random, Optional ByVal FilePath As String = "", Optional ByVal Participant As String = "") As SipMeasurement

            'Gets a file path from the user if none is supplied
            If FilePath = "" Then FilePath = Utils.GetOpenFilePath(,, {".txt"}, "Please open a stuctured measurement history .txt file.")
            If FilePath = "" Then
                MsgBox("No file selected!")
                Return Nothing
            End If

            'Parses the input file
            Dim InputLines() As String = System.IO.File.ReadAllLines(FilePath, Text.Encoding.UTF8)

            'Imports a summary
            Return ParseImportLines(InputLines, Randomizer, ParentTestSpecification, Participant)

        End Function


#End Region

        Public Function GetTargetAzimuths() As List(Of Double)

            Dim AvailableTargetDirections As New SortedSet(Of Double)

            For Each PlannedTrial In PlannedTrials
                For i = 0 To PlannedTrial.TargetStimulusLocations.Length - 1

                    AvailableTargetDirections.Add(PlannedTrial.TargetStimulusLocations(i).HorizontalAzimuth)

                    'TODO we must adjust these to the available and speakers in the selected transducer

                    'Adding tha actual azimuth
                    PlannedTrial.TargetStimulusLocations(i).ActualLocation = New SoundSourceLocation
                    PlannedTrial.TargetStimulusLocations(i).ActualLocation.HorizontalAzimuth = PlannedTrial.TargetStimulusLocations(i).HorizontalAzimuth

                Next

            Next


            Return AvailableTargetDirections.ToList

        End Function


    End Class



    Public Class SiPTestUnit

        Public Property Description As String = ""

        Public Property ParentMeasurement As SipMeasurement

        Public Property SpeechMaterialComponents As New List(Of SpeechMaterialComponent)

        Public Property PlannedTrials As New List(Of SipTrial)

        Public Property ObservedTrials As New List(Of SipTrial)

        Public Property AdaptiveValue As Double

        Public Sub New(ByRef ParentMeasurement As SipMeasurement, Optional ByVal Description As String = "")
            Me.ParentMeasurement = ParentMeasurement
            Me.Description = Description
        End Sub

        Public Sub PlanTrials(ByRef MediaSet As MediaSet, ByVal SoundPropagationType As SoundPropagationTypes,
            ByVal TargetLocation As SoundSourceLocation,
                              ByVal MaskerLocations As SoundSourceLocation(),
                              ByVal BackgroundLocations As SoundSourceLocation())

            PlannedTrials.Clear()

            Select Case ParentMeasurement.TestProcedure.AdaptiveType
                Case AdaptiveTypes.Fixed

                    'For n = 1 To ParentMeasurement.TestProcedure.LengthReduplications ' Should this be done here, or at a higher level?
                    For c = 0 To SpeechMaterialComponents.Count - 1
                        Dim NewTrial As New SipTrial(Me, SpeechMaterialComponents(c), MediaSet, SoundPropagationType, {TargetLocation}, MaskerLocations, BackgroundLocations, ParentMeasurement.Randomizer)
                        PlannedTrials.Add(NewTrial)
                    Next

                    'Next

                    'Case AdaptiveTypes.SimpleUpDown

                    '    'For n = 1 To ParentMeasurement.TestProcedure.LengthReduplications ' Should this be done here, or at a higher level?
                    '    For c = 0 To SpeechMaterialComponents.Count - 1
                    '        Dim NewTrial As New SipTrial(Me, SpeechMaterialComponents(c), MediaSet)
                    '        PlannedTrials.Add(NewTrial)
                    '    Next
                    '    'Next


            End Select

        End Sub

        Public Function GetNextTrial(ByRef rnd As Random) As SipTrial

            Select Case ParentMeasurement.TestProcedure.AdaptiveType
                Case AdaptiveTypes.Fixed

                    If PlannedTrials.Count = 0 Then Return Nothing

                    If ParentMeasurement.TestProcedure.RandomizeOrder = True Then
                        Dim RandomIndex = rnd.Next(0, PlannedTrials.Count)
                        Dim NextTrial = PlannedTrials(RandomIndex)
                        'Removing the trial from PlannedTrials
                        PlannedTrials.RemoveAt(RandomIndex)
                        Return NextTrial
                    Else
                        Dim NextTrial = PlannedTrials(0)
                        'Removing the trial from PlannedTrials
                        PlannedTrials.RemoveAt(0)
                        Return NextTrial
                    End If

                Case Else

                    Throw New NotImplementedException

            End Select

        End Function

        Public Function IsCompleted() As Boolean

            Select Case ParentMeasurement.TestProcedure.AdaptiveType
                Case AdaptiveTypes.Fixed

                    If PlannedTrials.Count = 0 Then
                        Return True
                    Else
                        Return False
                    End If

                Case Else

                    Throw New NotImplementedException

            End Select

        End Function


    End Class


    Public Class SipTrial
        Inherits TestTrial

        ''' <summary>
        ''' The object should store the order in which the trial was presented, and be set by the code presenting the trial.
        ''' </summary>
        ''' <returns></returns>
        Public Property PresentationOrder As Integer

        ''' <summary>
        ''' Holds a reference to the test unit to which the current test trial belongs
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ParentTestUnit As SiPTestUnit

        '''' <summary>
        '''' Holds a reference to the SpeechMaterial presented in the trial.
        '''' </summary>
        '''' <returns></returns>
        'Public ReadOnly Property SpeechMaterialComponent As SpeechMaterialComponent

        Public Property SelectedMediaIndex As Integer

        Public Property SelectedMaskerIndices As New List(Of Integer)

        ''' <summary>
        ''' After sound has been mixed, holds the initial margins of the target sounds (used for export purposes)
        ''' </summary>
        Public TargetInitialMargins As New List(Of Integer)

        ''' <summary>
        ''' Holds a reference to the MediaSet used in the trial.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property MediaSet As MediaSet

        ''' <summary>
        ''' The response given in a test trial
        ''' </summary>
        ''' <returns></returns>
        Public Property Response As String = ""

        'The result of a test trial
        Public Property Result As PossibleResults

        ''' <summary>
        ''' Returns the result converted to a binary score (1=correct, 0=wrong or missing)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Score As Integer
            Get
                If Result = PossibleResults.Correct Then
                    Return 1
                Else
                    Return 0
                End If
            End Get
        End Property

        ''' <summary>
        ''' The response time in milliseconds
        ''' </summary>
        ''' <returns></returns>
        Public Property ResponseTime As Integer

        Public Property ResponseMoment As DateTime

        Public Property AdjustedSuccessProbability As Double

        Public Function DetermineResponseAlternativeCount() As Boolean

            If SpeechMaterialComponent Is Nothing Then Return False

            'Gets the number of response contrasting alternatives
            Dim TempCount As Integer = 0
            SpeechMaterialComponent.IsContrastingComponent(, TempCount)
            _ResponseAlternativeCount = TempCount

            Return True
        End Function

        Private _ResponseAlternativeCount As Integer? = Nothing
        Public Property ResponseAlternativeCount As Integer?
            Get
                Return _ResponseAlternativeCount
            End Get
            Set(value As Integer?)
                _ResponseAlternativeCount = value
            End Set
        End Property


        ''' <summary>
        ''' Indicates whether the trial is a real test trial or not (e.g. a practise trial)
        ''' </summary>
        ''' <returns></returns>
        Public Property IsTestTrial As Boolean = True

        '''' <summary>
        '''' An object that can hold test trial sounds that can be mixed in advance.
        '''' </summary>
        'Public Sound As Audio.Sound = Nothing

        ''' <summary>
        ''' Holds the start sample of the target signal within the test trial sound (without any delay caused by room simulation)
        ''' </summary>
        Public TargetStartSample As Integer

        ''' <summary>
        ''' A list that can hold non-presented (hypothical) test trials containing the contrasting alternatives.
        ''' </summary>
        Public PseudoTrials As List(Of SipTrial)

        ''' <summary>
        ''' A list of Tuples that can hold various sound mixes used in the test trial, to be exported to sound file later. Each Sound should be come with a descriptive string (that can be used in the output file name).)
        ''' </summary>
        Public TrialSoundsToExport As New List(Of Tuple(Of String, Audio.Sound))

        ''' <summary>
        ''' A list of gain applied to all different SoundSceneItems in the presented TestTrial
        ''' </summary>
        Public GainList As New SortedList(Of SoundSceneItem.SoundSceneItemRoles, List(Of String))

        'Strings that hold information about the (random) sound, and sound section, selections made in MixSound (for export purposes only)
        Public SelectedTargetIndexString As String = ""
        Public SelectedMaskerIndicesString As String = ""
        Public BackgroundStartSamplesString As String = ""
        Public BackgroundSpeechStartSamplesString As String = ""

        Public TestWordStartTime As Double
        Public TestWordCompletedTime As Double

        Public BackgroundNonSpeechDucking As Double

        Public SoundPropagationType As SoundPropagationTypes

        ''' <summary>
        ''' Holds the location(s) of the target(s)
        ''' </summary>
        Public TargetStimulusLocations As SoundSourceLocation()

        ''' <summary>
        ''' Holds the location of the maskers
        ''' </summary>
        Public MaskerLocations As SoundSourceLocation()

        ''' <summary>
        ''' Holds the location of the background sounds
        ''' </summary>
        Public BackgroundLocations As SoundSourceLocation()

        ''' <summary>
        ''' Determines whether the trial is a binaural masking level difference (BMLD) trial.
        ''' </summary>
        Public IsBmldTrial As Boolean

        ''' <summary>
        ''' If IsBmldTrial is true, determines the BMLD signal mode
        ''' </summary>
        Public BmldSignalMode As BmldModes

        ''' <summary>
        ''' If IsBmldTrial is true, determines the BMLD noise mode
        ''' </summary>
        Public BmldNoiseMode As BmldModes

        ''' <summary>
        ''' Creates a new SiP-test trial in directional mode
        ''' </summary>
        ''' <param name="ParentTestUnit"></param>
        ''' <param name="SpeechMaterialComponent"></param>
        ''' <param name="MediaSet"></param>
        ''' <param name="TargetStimulusLocations"></param>
        ''' <param name="MaskerLocations"></param>
        ''' <param name="BackgroundLocations"></param>
        ''' <param name="SipMeasurementRandomizer"></param>
        Public Sub New(ByRef ParentTestUnit As SiPTestUnit,
                       ByRef SpeechMaterialComponent As SpeechMaterialComponent,
                       ByRef MediaSet As MediaSet,
                       ByRef SoundPropagationType As SoundPropagationTypes,
                       ByVal TargetStimulusLocations As SoundSourceLocation(),
                       ByVal MaskerLocations As SoundSourceLocation(),
                       ByVal BackgroundLocations As SoundSourceLocation(),
                       ByRef SipMeasurementRandomizer As Random)

            Me.ParentTestUnit = ParentTestUnit
            Me.SpeechMaterialComponent = SpeechMaterialComponent
            Me.MediaSet = MediaSet
            Me.SoundPropagationType = SoundPropagationType
            Me.TargetStimulusLocations = TargetStimulusLocations
            Me.MaskerLocations = MaskerLocations
            Me.BackgroundLocations = BackgroundLocations

            'Setting some levels
            Dim Fs2Spl As Double = Audio.Standard_dBFS_dBSPL_Difference

            'Calculating levels
            ReferenceSpeechMaterialLevel_SPL = Fs2Spl + SpeechMaterialComponent.GetAncestorAtLevel(SpeechMaterialComponent.LinguisticLevels.ListCollection).GetNumericMediaSetVariableValue(MediaSet, "Lc")
            ReferenceTestWordLevel_SPL = Fs2Spl + SpeechMaterialComponent.GetNumericMediaSetVariableValue(MediaSet, "Lc") 'TestStimulus.TestWord_ReferenceSPL
            ReferenceContrastingPhonemesLevel_SPL = Fs2Spl + SpeechMaterialComponent.GetAncestorAtLevel(SpeechMaterialComponent.LinguisticLevels.List).GetNumericMediaSetVariableValue(MediaSet, "RLxs")

            'Randomizing the SelectedMediaIndex 
            'SelectedMediaIndex = 0
            'MsgBox("Re-insert randomizer!")
            SelectedMediaIndex = SipMeasurementRandomizer.Next(0, Me.MediaSet.MediaAudioItems)

        End Sub

        ''' <summary>
        ''' Creates a new SiP-test trial in BMLD mode
        ''' </summary>
        ''' <param name="ParentTestUnit"></param>
        ''' <param name="SpeechMaterialComponent"></param>
        ''' <param name="MediaSet"></param>
        ''' <param name="SipMeasurementRandomizer"></param>
        Public Sub New(ByRef ParentTestUnit As SiPTestUnit,
                       ByRef SpeechMaterialComponent As SpeechMaterialComponent,
                       ByRef MediaSet As MediaSet,
                       ByVal SoundPropagationType As SoundPropagationTypes,
                       ByRef SignalMode As BmldModes,
                       ByRef NoiseMode As BmldModes,
                       ByRef SipMeasurementRandomizer As Random)

            IsBmldTrial = True
            Me.SoundPropagationType = SoundPropagationType

            Me.ParentTestUnit = ParentTestUnit
            Me.SpeechMaterialComponent = SpeechMaterialComponent
            Me.MediaSet = MediaSet
            Me.BmldSignalMode = SignalMode
            Me.BmldNoiseMode = NoiseMode

            'Setting some levels
            Dim Fs2Spl As Double = Audio.Standard_dBFS_dBSPL_Difference

            'Calculating levels
            ReferenceSpeechMaterialLevel_SPL = Fs2Spl + SpeechMaterialComponent.GetAncestorAtLevel(SpeechMaterialComponent.LinguisticLevels.ListCollection).GetNumericMediaSetVariableValue(MediaSet, "Lc")
            ReferenceTestWordLevel_SPL = Fs2Spl + SpeechMaterialComponent.GetNumericMediaSetVariableValue(MediaSet, "Lc") 'TestStimulus.TestWord_ReferenceSPL
            ReferenceContrastingPhonemesLevel_SPL = Fs2Spl + SpeechMaterialComponent.GetAncestorAtLevel(SpeechMaterialComponent.LinguisticLevels.List).GetNumericMediaSetVariableValue(MediaSet, "RLxs")

            'Randomizing the SelectedMediaIndex 
            'SelectedMediaIndex = 0
            'MsgBox("Re-insert randomizer!")
            SelectedMediaIndex = SipMeasurementRandomizer.Next(0, Me.MediaSet.MediaAudioItems)

            'Setting up signal and masker locations
            Select Case BmldSignalMode
                Case BmldModes.BinauralSamePhase, BmldModes.BinauralPhaseInverted
                    TargetStimulusLocations = {
                        New SoundSourceLocation With {.HorizontalAzimuth = -90, .Distance = 1, .Elevation = 0},
                        New SoundSourceLocation With {.HorizontalAzimuth = 90, .Distance = 1, .Elevation = 0}}

                Case BmldModes.LeftOnly
                    TargetStimulusLocations = {
                        New SoundSourceLocation With {.HorizontalAzimuth = -90, .Distance = 1, .Elevation = 0}}

                Case BmldModes.RightOnly
                    TargetStimulusLocations = {
                        New SoundSourceLocation With {.HorizontalAzimuth = 90, .Distance = 1, .Elevation = 0}}

            End Select

            Select Case BmldNoiseMode
                Case BmldModes.BinauralSamePhase, BmldModes.BinauralPhaseInverted, BmldModes.BinauralUncorrelated
                    MaskerLocations = {
                        New SoundSourceLocation With {.HorizontalAzimuth = -90, .Distance = 1, .Elevation = 0},
                        New SoundSourceLocation With {.HorizontalAzimuth = 90, .Distance = 1, .Elevation = 0}}

                    BackgroundLocations = {
                        New SoundSourceLocation With {.HorizontalAzimuth = -90, .Distance = 1, .Elevation = 0},
                        New SoundSourceLocation With {.HorizontalAzimuth = -90, .Distance = 1, .Elevation = 0},
                        New SoundSourceLocation With {.HorizontalAzimuth = 90, .Distance = 1, .Elevation = 0},
                        New SoundSourceLocation With {.HorizontalAzimuth = 90, .Distance = 1, .Elevation = 0}}

                Case BmldModes.LeftOnly
                    MaskerLocations = {
                        New SoundSourceLocation With {.HorizontalAzimuth = -90, .Distance = 1, .Elevation = 0}}

                    BackgroundLocations = {
                        New SoundSourceLocation With {.HorizontalAzimuth = -90, .Distance = 1, .Elevation = 0},
                        New SoundSourceLocation With {.HorizontalAzimuth = -90, .Distance = 1, .Elevation = 0}}

                Case BmldModes.RightOnly
                    MaskerLocations = {
                        New SoundSourceLocation With {.HorizontalAzimuth = 90, .Distance = 1, .Elevation = 0}}

                    BackgroundLocations = {
                        New SoundSourceLocation With {.HorizontalAzimuth = 90, .Distance = 1, .Elevation = 0},
                        New SoundSourceLocation With {.HorizontalAzimuth = 90, .Distance = 1, .Elevation = 0}}

            End Select



        End Sub


        Public Property Reference_SPL As Double
        Public Property PNR As Double

        Private _TargetMasking_SPL As Double? = Nothing

        ''' <summary>
        ''' Returns the intended masker sound level at the position of the listeners head, or Nothing if not calculated
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property TargetMasking_SPL As Double?
            Get
                Return _TargetMasking_SPL
            End Get
        End Property

        Public Property ContextRegionSpeech_SPL As Double

        Private Property _TestWordLevel As Double? = Nothing
        Public ReadOnly Property TestWordLevel As Double?
            Get
                Return _TestWordLevel
            End Get
        End Property
        Public ReadOnly Property ReferenceSpeechMaterialLevel_SPL As Double
        Public ReadOnly Property ReferenceTestWordLevel_SPL As Double
        Public ReadOnly Property ReferenceContrastingPhonemesLevel_SPL As Double

        Public Property TestWordLevelLimit As Double = 82.3 '82.3 Shouted Speech level i SII standard
        Public Property ContextSpeechLimit As Double = 74.85 '74.85 Loud Speech Level From SII standard

        ''' <summary>
        ''' Sets all levels in the current test trial. Levels should be set prior to mixing the sound.
        ''' </summary>
        ''' <param name="ReferenceLevel"></param>
        ''' <param name="PNR"></param>
        Public Sub SetLevels(ByVal ReferenceLevel As Double, ByVal PNR As Double)

            'Setting the levels

            Me.Reference_SPL = ReferenceLevel
            Me.PNR = PNR

            'Calculating the difference between the standard ReferenceSpeechMaterialLevel_SPL (68.34 dB SPL) reference level and the one currently used
            Dim RefLevelDifference As Double = ReferenceLevel - ReferenceSpeechMaterialLevel_SPL

            '0. Gettings some levels
            '1.And setting the noise level
            Dim SNR_Type As String = "PNR"
            If SNR_Type = "PNR" Then
                'In this procedure, CurrentSNR represents the sound level difference between the average max level of the contrasting test phonemes, and the masker sound
                'Setting the test word masker to Lcp

                'Setting TargetMasking_SPL to ContrastingPhonemesLevel_SPL
                _TargetMasking_SPL = ReferenceContrastingPhonemesLevel_SPL + RefLevelDifference

            ElseIf SNR_Type = "SNR_SpeechMaterial" Then
                'In this procedure, CurrentSNR represents the sound level difference between the average level of the whole speech material, and the masker sound
                'Setting the TargetMasking_SPL to Lsm
                _TargetMasking_SPL = ReferenceSpeechMaterialLevel_SPL + RefLevelDifference
            Else
                Throw New NotImplementedException()
            End If

            '2. Adjusting the speech level to attain the desired PNR

            'Calculating the unlimited target test word level
            Dim TargetTestWord_SPL = ReferenceTestWordLevel_SPL + RefLevelDifference + PNR

            'Calculating the average speech material level equivalent to the current TargetTestWord_SPL
            Dim CurrentAverageSpeechMaterial_SPL = ReferenceSpeechMaterialLevel_SPL + RefLevelDifference + PNR

            'Setting the ContextRegionSpeech_SPL to the CurrentAverageSpeechMaterial_SPL                    
            ContextRegionSpeech_SPL = CurrentAverageSpeechMaterial_SPL

            '3. Limiting test word level
            If TargetTestWord_SPL > TestWordLevelLimit Then

                Dim Difference As Double = TargetTestWord_SPL - TestWordLevelLimit

                'Decreasing all levels set by Difference, to retain the desired test AdaptiveValue
                _TargetMasking_SPL -= Difference
                TargetTestWord_SPL -= Difference
                ContextRegionSpeech_SPL -= Difference
            End If

            _TestWordLevel = TargetTestWord_SPL

            '4. Limiting the context speech level
            If ContextRegionSpeech_SPL > ContextSpeechLimit Then

                Dim Difference As Double = ContextRegionSpeech_SPL - ContextSpeechLimit

                'Decreasing the ContextRegionForegroundLevel_SPL to ContextSpeechLimit
                ContextRegionSpeech_SPL -= Difference

            End If

            If PseudoTrials IsNot Nothing Then
                For Each NonpresentedAlternativeTrial In PseudoTrials
                    NonpresentedAlternativeTrial.SetLevels(ReferenceLevel, PNR)
                Next
            End If

        End Sub


        Public Sub MixSound(ByRef SelectedTransducer As AudioSystemSpecification,
                            ByVal MinimumStimulusOnsetTime As Double, ByVal MaximumStimulusOnsetTime As Double,
                            ByRef SipMeasurementRandomizer As Random, ByVal TrialSoundMaxDuration As Double, ByVal UseBackgroundSpeech As Boolean,
                            Optional ByVal FixedMaskerIndices As List(Of Integer) = Nothing, Optional ByVal FixedSpeechIndex As Integer? = Nothing)

            'If Me.IsBmldTrial = True Then
            '    FixedMaskerIndices = New List(Of Integer) From {1, 2}
            'Else
            '    FixedMaskerIndices = New List(Of Integer) From {1}
            'End If
            'FixedSpeechIndex = 1

            Try

                'Setting up the SiP-trial sound mix
                Dim MixStopWatch As New Stopwatch
                MixStopWatch.Start()

                'Sets a List of SoundSceneItem in which to put the sounds to mix
                Dim ItemList = New List(Of SoundSceneItem)

                Dim SoundWaveFormat As Audio.Formats.WaveFormat = Nothing

                'Getting maskers
                SelectedMaskerIndices.Clear()

                Dim NumberOfMaskers As Integer = MaskerLocations.Length

                If FixedMaskerIndices Is Nothing Then
                    SelectedMaskerIndices.AddRange(Utils.SampleWithoutReplacement(NumberOfMaskers, 0, Me.MediaSet.MaskerAudioItems, SipMeasurementRandomizer))
                Else
                    If FixedMaskerIndices.Count <> NumberOfMaskers Then Throw New ArgumentException("FixedMaskerIndices must be either Nothing or contain " & NumberOfMaskers & " integers.")
                    SelectedMaskerIndices.AddRange(FixedMaskerIndices)
                End If

                Dim CurrentSampleRate As Integer
                Dim MaskersLength As Integer
                'Getting the length of the fade-in region before the centralized section (assuming all masker in the test word group to have the same specifications)
                Dim MaskerFadeInLength As Integer
                Dim MaskerFadeOutLength As Integer
                Dim MaskerCentralizedSectionLength As Integer

                Dim Maskers As New List(Of Tuple(Of Audio.Sound, SoundSourceLocation))
                For MaskerIndex = 0 To NumberOfMaskers - 1
                    Dim Masker As Audio.Sound = (Me.SpeechMaterialComponent.GetMaskerSound(Me.MediaSet, SelectedMaskerIndices(MaskerIndex)))

                    If MaskerIndex = 0 Then
                        'Stores the sample rate and the wave format
                        CurrentSampleRate = Masker.WaveFormat.SampleRate
                        SoundWaveFormat = Masker.WaveFormat

                        'Getting the lengths of the maskers (Assuming same lengths of all maskers)
                        MaskersLength = Masker.WaveData.SampleData(1).Length

                        'Getting the length of the fade-in region before the centralized section (assuming all masker in the test word group to have the same specifications)
                        MaskerFadeInLength = Masker.SMA.ChannelData(1)(0)(0).Length ' Should be stored as the length of the first word in the first sentence
                        MaskerFadeOutLength = Masker.SMA.ChannelData(1)(0)(2).Length ' Should be stored as the length of the third (and last) word in the first sentence
                        MaskerCentralizedSectionLength = MaskersLength - MaskerFadeInLength - MaskerFadeOutLength

                    End If

                    Maskers.Add(New Tuple(Of Audio.Sound, SoundSourceLocation)(Masker, MaskerLocations(MaskerIndex)))
                Next

                'Duplicating the maskers for BMLD
                If IsBmldTrial = True Then
                    'Copying masker 1 to masker 2, making them identical
                    Array.Copy(Maskers(0).Item1.WaveData.SampleData(1), Maskers(1).Item1.WaveData.SampleData(1), Maskers(0).Item1.WaveData.SampleData(1).Length)
                End If

                'Randomizing a masker start time, and stores it in TestWordStartTime 

                Dim MaskerStartTime As Double
                If MinimumStimulusOnsetTime = MaximumStimulusOnsetTime Then
                    MaskerStartTime = MinimumStimulusOnsetTime
                Else
                    MaskerStartTime = SipMeasurementRandomizer.Next(Math.Round(MinimumStimulusOnsetTime * 1000), Math.Round(MaximumStimulusOnsetTime * 1000)) / 1000
                End If
                Dim MaskersStartSample As Integer = MaskerStartTime * CurrentSampleRate

                'Calculating a sample region in by which the sound level of the maskers should be defined (Called Centralized Region in Witte's thesis)
                Dim MaskersStartMeasureSample As Integer = Math.Max(0, MaskerFadeInLength - 1)
                Dim MaskersStartMeasureLength As Integer = MaskerCentralizedSectionLength

                'Selects a recording index, and gets the corresponding sound
                'Updating SelectedMediaIndex if FixedSpeechIndex is set. SelectedMediaIndex is otherwise set randomly when the trial is created
                If FixedSpeechIndex.HasValue Then SelectedMediaIndex = FixedSpeechIndex
                Dim Targets As New List(Of Tuple(Of Audio.Sound, SoundSourceLocation))
                Dim NumberOfTargets As Integer = Me.TargetStimulusLocations.Length
                Dim TestWordLength As Integer
                TargetInitialMargins = New List(Of Integer)
                For TargetIndex = 0 To NumberOfTargets - 1
                    'TODO: Here the same signal is taken for all locations. If different signals are needed in different locations, this should be modified
                    Dim InitialMargin As Integer = 0
                    Dim TestWordSound = Me.SpeechMaterialComponent.GetSound(Me.MediaSet, SelectedMediaIndex, 1, , ,, InitialMargin)

                    'Storing the initial margin
                    Me.TargetInitialMargins.Add(InitialMargin)

                    If TargetIndex = 0 Then
                        'Stores the length of the test word sound
                        TestWordLength = TestWordSound.WaveData.SampleData(1).Length
                    End If

                    Targets.Add(New Tuple(Of Audio.Sound, SoundSourceLocation)(TestWordSound, Me.TargetStimulusLocations(TargetIndex)))
                Next

                'Not actually duplicating the targets for BMLD
                If IsBmldTrial = True Then
                    'The same signal will already be in all Targets indices (see above)
                    ''If changed the following code could be used
                    ''Copying signal 1 to signal 2, making them identical
                    'Array.Copy(Targets(0).Item1.WaveData.SampleData(1), Targets(1).Item1.WaveData.SampleData(1), Targets(0).Item1.WaveData.SampleData(1).Length)
                End If

                'Calculating test word start sample, syncronized with the centre of the maskers
                Dim TestWordStartSample As Integer = MaskersStartSample + MaskersLength / 2 - TestWordLength / 2

                'Calculating test word start time
                TestWordStartTime = TestWordStartSample / CurrentSampleRate

                'Calculates the offset of the test word sound
                Dim TestWordCompletedSample As Integer = TestWordStartSample + TestWordLength

                'Calculates and stores the TestWordCompletedTime 
                TestWordCompletedTime = TestWordCompletedSample / CurrentSampleRate

                'Sets a total trial sound length
                Dim TrialSoundLength As Integer = TrialSoundMaxDuration * CurrentSampleRate

                'Getting a background non-speech sound, and copies random sections of it into two sounds
                Dim BackgroundNonSpeech_Sound As Audio.Sound = Me.SpeechMaterialComponent.GetBackgroundNonspeechSound(Me.MediaSet, 0)
                Dim Backgrounds As New List(Of Tuple(Of Audio.Sound, SoundSourceLocation, Integer)) ' N.B. The Item3 holds the start sample of the section taken out of the source sound
                Dim NumberOfBackgrounds As Integer = BackgroundLocations.Length
                For BackgroundIndex = 0 To NumberOfBackgrounds - 1
                    'Getting a background sound and location
                    'TODO: we should make sure the start time for copying the sounds here differ by several seconds (and can be kept exactly the same for BMLD testing) 
                    Dim BackgroundStartSample As Integer = SipMeasurementRandomizer.Next(0, BackgroundNonSpeech_Sound.WaveData.SampleData(1).Length - TrialSoundLength - 2)
                    Backgrounds.Add(New Tuple(Of Audio.Sound, SoundSourceLocation, Integer)(
                            BackgroundNonSpeech_Sound.CopySection(1, BackgroundStartSample, TrialSoundLength),
                            BackgroundLocations(BackgroundIndex), BackgroundStartSample))
                Next

                'Superpositioning the backgrounds for BMLD
                If IsBmldTrial = True Then

                    Dim TempBackgroundSoundList As New List(Of Sound)
                    For Each Background In Backgrounds
                        TempBackgroundSoundList.Add(Background.Item1)
                    Next
                    Dim SuperPositionedBackgroundSounds = Audio.DSP.SuperpositionEqualLengthSounds(TempBackgroundSoundList)

                    'Replacing the individual background sounds with the superpositioned on, to get the same sound mix in all background sources
                    For bi = 0 To Backgrounds.Count - 1
                        'Copying background 1 to background 2, making them identical
                        Array.Copy(SuperPositionedBackgroundSounds.WaveData.SampleData(1), Backgrounds(bi).Item1.WaveData.SampleData(1), SuperPositionedBackgroundSounds.WaveData.SampleData(1).Length)
                    Next

                End If

                'Getting a background speech sound, if needed, and copies a random section of it into a single sound. The (random) start sample is stored in Item3, for export to sound files.
                Dim BackgroundSpeechSelections As New List(Of Tuple(Of Audio.Sound, SoundSourceLocation, Integer))
                'For now skipping with BMLD
                If UseBackgroundSpeech = True Then
                    Dim BackgroundSpeech_Sound As Audio.Sound = Me.SpeechMaterialComponent.GetBackgroundSpeechSound(Me.MediaSet, 0)
                    For TargetIndex = 0 To NumberOfTargets - 1
                        'TODO: we should make sure the start time for copying the sounds here differ by several seconds (and can be kept exactly the same for BMLD testing) 
                        Dim StartSample As Integer = SipMeasurementRandomizer.Next(0, BackgroundSpeech_Sound.WaveData.SampleData(1).Length - TrialSoundLength - 2)
                        Dim CurrentBackgroundSpeechSelection = BackgroundSpeech_Sound.CopySection(1, StartSample, TrialSoundLength)
                        BackgroundSpeechSelections.Add(New Tuple(Of Audio.Sound, SoundSourceLocation, Integer)(CurrentBackgroundSpeechSelection, Me.TargetStimulusLocations(TargetIndex), StartSample))
                    Next

                    'Duplicating the BackgroundSpeechSelections for BMLD
                    If IsBmldTrial = True Then
                        'Copying signal 1 to signal 2, making them identical
                        Array.Copy(BackgroundSpeechSelections(0).Item1.WaveData.SampleData(1), BackgroundSpeechSelections(1).Item1.WaveData.SampleData(1), BackgroundSpeechSelections(0).Item1.WaveData.SampleData(1).Length)
                    End If
                End If

                'Sets up fading specifications for the test word
                Dim FadeSpecs_TestWord = New List(Of STFN.Audio.DSP.Transformations.FadeSpecifications)
                FadeSpecs_TestWord.Add(New STFN.Audio.DSP.Transformations.FadeSpecifications(Nothing, 0, 0, CurrentSampleRate * 0.002))
                FadeSpecs_TestWord.Add(New STFN.Audio.DSP.Transformations.FadeSpecifications(0, Nothing, -CurrentSampleRate * 0.002))

                'Sets up fading specifications for the maskers
                Dim FadeSpecs_Maskers = New List(Of STFN.Audio.DSP.Transformations.FadeSpecifications)
                FadeSpecs_Maskers.Add(New STFN.Audio.DSP.Transformations.FadeSpecifications(Nothing, 0, 0, MaskerFadeInLength))
                FadeSpecs_Maskers.Add(New STFN.Audio.DSP.Transformations.FadeSpecifications(0, Nothing, -MaskerFadeOutLength))

                'Sets up fading specifications for the background signals
                Dim FadeSpecs_Background = New List(Of STFN.Audio.DSP.Transformations.FadeSpecifications)
                FadeSpecs_Background.Add(New STFN.Audio.DSP.Transformations.FadeSpecifications(Nothing, 0, 0, CurrentSampleRate * 0.01))
                FadeSpecs_Background.Add(New STFN.Audio.DSP.Transformations.FadeSpecifications(0, Nothing, -CurrentSampleRate * 0.01))

                'Sets up ducking specifications for the background (non-speech) signals
                Dim DuckSpecs_BackgroundNonSpeech = New List(Of STFN.Audio.DSP.Transformations.FadeSpecifications)
                BackgroundNonSpeechDucking = Math.Max(0, Me.MediaSet.BackgroundNonspeechRealisticLevel - Math.Min(Me.TargetMasking_SPL.Value - 3, Me.MediaSet.BackgroundNonspeechRealisticLevel))
                DuckSpecs_BackgroundNonSpeech.Add(New STFN.Audio.DSP.Transformations.FadeSpecifications(0, BackgroundNonSpeechDucking, Math.Max(0, TestWordStartSample - CurrentSampleRate * 0.5), TestWordStartSample))
                DuckSpecs_BackgroundNonSpeech.Add(New STFN.Audio.DSP.Transformations.FadeSpecifications(BackgroundNonSpeechDucking, 0, TestWordCompletedSample, Math.Max(0, TestWordCompletedSample - CurrentSampleRate * 0.5)))

                'Sets up ducking specifications for the background (speech) signals
                Dim DuckSpecs_BackgroundSpeech = New List(Of STFN.Audio.DSP.Transformations.FadeSpecifications)
                DuckSpecs_BackgroundSpeech.Add(New STFN.Audio.DSP.Transformations.FadeSpecifications(0, Nothing, Math.Max(0, TestWordStartSample - CurrentSampleRate * 1), Math.Max(0, TestWordStartSample - CurrentSampleRate * 0.5)))
                DuckSpecs_BackgroundSpeech.Add(New STFN.Audio.DSP.Transformations.FadeSpecifications(Nothing, 0, Math.Max(0, TestWordCompletedSample + CurrentSampleRate * 0.5), Math.Max(0, TestWordCompletedSample - CurrentSampleRate * 1)))

                'Adds the test word signal, with fade and location specifications
                Dim LevelGroup As Integer = 1 ' The level group value is used to set the added sound level of items sharing the same (arbitrary) LevelGroup value to the indicated sound level. (Thus, the sounds with the same LevelGroup value are measured together.)
                For TargetIndex = 0 To Targets.Count - 1
                    ItemList.Add(New SoundSceneItem(Targets(TargetIndex).Item1, 1, Me.TestWordLevel, LevelGroup, Targets(TargetIndex).Item2, SoundSceneItem.SoundSceneItemRoles.Target, TestWordStartSample,,,, FadeSpecs_TestWord))
                    'Incrementing LevelGroup if ears should be measured separately as in BMLD. (But leaving the last item since it is incremented below)
                    If IsBmldTrial = True And TargetIndex < Targets.Count - 1 Then LevelGroup += 1
                Next
                LevelGroup += 1

                'Adds the Maskers, with fade and location specifications
                For MaskerIndex = 0 To Maskers.Count - 1
                    ItemList.Add(New SoundSceneItem(Maskers(MaskerIndex).Item1, 1, Me.TargetMasking_SPL, LevelGroup, Maskers(MaskerIndex).Item2, SoundSceneItem.SoundSceneItemRoles.Masker, MaskersStartSample, MaskersStartMeasureSample, MaskersStartMeasureLength,, FadeSpecs_Maskers))
                    'Incrementing LevelGroup if ears should be measured separately as in BMLD. (But leaving the last item since it is incremented below)
                    If IsBmldTrial = True And MaskerIndex < Maskers.Count - 1 Then LevelGroup += 1
                Next
                LevelGroup += 1

                'Adds the background (non-speech) signals, with fade, duck and location specifications
                For BackgroundIndex = 0 To Backgrounds.Count - 1
                    ItemList.Add(New SoundSceneItem(Backgrounds(BackgroundIndex).Item1, 1, Me.MediaSet.BackgroundNonspeechRealisticLevel, LevelGroup, Backgrounds(BackgroundIndex).Item2, SoundSceneItem.SoundSceneItemRoles.BackgroundNonspeech, 0,,,, FadeSpecs_Background, DuckSpecs_BackgroundNonSpeech))
                    'Incrementing LevelGroup if ears should be measured separately as in BMLD. (But leaving the last item since it is incremented below)
                    If IsBmldTrial = True Then
                        If BackgroundIndex < Backgrounds.Count - 2 Then
                            'Incrementing LevelGroup if we have a new location, based on the azimuth only (i.e. the other ear.) TODO: This is a bad solution, since backgrounds necessarily have to be ordered from one ear to the other, and never mixed!
                            If Backgrounds(BackgroundIndex).Item2.HorizontalAzimuth <> Backgrounds(BackgroundIndex + 1).Item2.HorizontalAzimuth Then
                                LevelGroup += 1
                            End If
                        End If
                    End If
                Next
                LevelGroup += 1

                'Adds the background (speech) signal, with fade, duck and location specifications
                If UseBackgroundSpeech = True Then
                    For TargetIndex = 0 To BackgroundSpeechSelections.Count - 1
                        'TODO: Here LevelGroup needs to be incremented if ears are measured separately as in BMLD! Possibly also on other similar places, as with the noise...
                        ItemList.Add(New SoundSceneItem(BackgroundSpeechSelections(TargetIndex).Item1, 1, Me.ContextRegionSpeech_SPL, LevelGroup, BackgroundSpeechSelections(TargetIndex).Item2, SoundSceneItem.SoundSceneItemRoles.BackgroundSpeech, 0,,,, FadeSpecs_Background, DuckSpecs_BackgroundSpeech))
                        'Incrementing LevelGroup if ears should be measured separately as in BMLD. (But leaving the last item since it is incremented below)
                        If IsBmldTrial = True And TargetIndex < BackgroundSpeechSelections.Count - 1 Then LevelGroup += 1
                    Next
                    LevelGroup += 1
                End If


                MixStopWatch.Stop()
                If LogToConsole = True Then Console.WriteLine("Prepared sounds in " & MixStopWatch.ElapsedMilliseconds & " ms.")
                MixStopWatch.Restart()

                Dim MixedTestTrialSound As Audio.Sound

                If ParentTestUnit.ParentMeasurement.ExportTrialSoundFiles = True Or Me.IsBmldTrial = True Then

                    'Creating the mix directly by calling CreateSoundScene of the current Mixer and exporting the sound for comparison with the separately exported sounds below
                    Dim ExportAllItemTypes As Boolean = False
                    If ExportAllItemTypes = True Then
                        If Me.IsBmldTrial = True Then Throw New NotImplementedException("Exporting the final mix directly is not supported with BMLD trials.")
                        Dim TempMixedTestTrialSound = SelectedTransducer.Mixer.CreateSoundScene(ItemList, Me.SoundPropagationType)
                        TrialSoundsToExport.Add(New Tuple(Of String, Audio.Sound)("OriginalMix", TempMixedTestTrialSound))
                    End If

                    'Creating separate sound files for target, maskers, and the final mix
                    Dim Target_Items As New List(Of SoundSceneItem)
                    Dim Masker_Items As New List(Of SoundSceneItem)
                    Dim BackgroundNonspeech_Items As New List(Of SoundSceneItem)
                    Dim BackgroundSpeech_Items As New List(Of SoundSceneItem)
                    Dim Nontarget_Items As New List(Of SoundSceneItem)

                    For Each Item In ItemList

                        If Item.Role = SoundSceneItem.SoundSceneItemRoles.Target Then
                            'Collecting the target/s
                            Target_Items.Add(Item)
                        Else
                            'Collecting the non targets
                            Nontarget_Items.Add(Item)
                        End If

                        If ExportAllItemTypes = True Then
                            Select Case Item.Role
                                Case SoundSceneItem.SoundSceneItemRoles.Masker
                                    'Collecting the maskers/s
                                    Masker_Items.Add(Item)
                                Case SoundSceneItem.SoundSceneItemRoles.BackgroundNonspeech
                                    'Collecting the BackgroundNonspeech item/s
                                    BackgroundNonspeech_Items.Add(Item)
                                Case SoundSceneItem.SoundSceneItemRoles.BackgroundSpeech
                                    'Collecting the BackgroundSpeech item/s
                                    BackgroundSpeech_Items.Add(Item)
                            End Select
                        End If

                    Next

                    'Creating the target mix by calling CreateSoundScene of the current Mixer
                    Dim TargetSound = SelectedTransducer.Mixer.CreateSoundScene(Target_Items, Me.SoundPropagationType)

                    Dim MaskerItemsSound As Audio.Sound = Nothing
                    If Masker_Items.Count > 0 Then
                        MaskerItemsSound = SelectedTransducer.Mixer.CreateSoundScene(Masker_Items, Me.SoundPropagationType)
                    End If

                    Dim BackgroundNonspeechItemsSound As Audio.Sound = Nothing
                    If BackgroundNonspeech_Items.Count > 0 Then
                        BackgroundNonspeechItemsSound = SelectedTransducer.Mixer.CreateSoundScene(BackgroundNonspeech_Items, Me.SoundPropagationType)
                    End If

                    Dim BackgroundSpeechItemsSound As Audio.Sound = Nothing
                    If BackgroundSpeech_Items.Count > 0 Then
                        BackgroundSpeechItemsSound = SelectedTransducer.Mixer.CreateSoundScene(BackgroundSpeech_Items, Me.SoundPropagationType)
                    End If

                    'Creating the non-target mix by calling CreateSoundScene of the current Mixer
                    Dim NontargetSound = SelectedTransducer.Mixer.CreateSoundScene(Nontarget_Items, Me.SoundPropagationType)

                    'If, BMLD trial, applying frontal left ear only room simulation,phase inverting the appropriate sounds
                    If Me.IsBmldTrial = True Then

                        'Attains a copy of the appropriate directional FIR-filter kernel
                        Dim FrontalTwoLeftEarsKernel = DirectionalSimulator.GetStereoKernel(DirectionalSimulator.SelectedDirectionalSimulationSetName, 0, 0, 3).TwoLeftEarsKernel
                        'Dim SelectedSimulationKernel = DirectionalSimulator.GetStereoKernel(ImpulseReponseSetName, SoundSceneItem.SourceLocation.HorizontalAzimuth, SoundSceneItem.SourceLocation.Elevation, SoundSceneItem.SourceLocation.Distance)
                        Dim CurrentKernel = FrontalTwoLeftEarsKernel.BinauralIR.CreateSoundDataCopy
                        Dim SelectedActualPoint = FrontalTwoLeftEarsKernel.Point
                        Dim SelectedActualBinauralDelay = FrontalTwoLeftEarsKernel.BinauralDelay

                        'Applies FIR-filtering
                        Dim MyFftFormat As Audio.Formats.FftFormat = New Formats.FftFormat
                        TargetSound = Audio.DSP.FIRFilter(TargetSound, CurrentKernel, MyFftFormat,,,,,, True)
                        If MaskerItemsSound IsNot Nothing Then MaskerItemsSound = Audio.DSP.FIRFilter(MaskerItemsSound, CurrentKernel, MyFftFormat,,,,,, True)
                        If BackgroundNonspeechItemsSound IsNot Nothing Then BackgroundNonspeechItemsSound = Audio.DSP.FIRFilter(BackgroundNonspeechItemsSound, CurrentKernel, MyFftFormat,,,,,, True)
                        If BackgroundSpeechItemsSound IsNot Nothing Then BackgroundSpeechItemsSound = Audio.DSP.FIRFilter(BackgroundSpeechItemsSound, CurrentKernel, MyFftFormat,,,,,, True)
                        NontargetSound = Audio.DSP.FIRFilter(NontargetSound, CurrentKernel, MyFftFormat,,,,,, True)

                        'Storing the actual location values used in the simulation
                        For locationIndex = 0 To Me.TargetStimulusLocations.Length - 1
                            TargetStimulusLocations(locationIndex).ActualLocation = New SoundSourceLocation
                            TargetStimulusLocations(locationIndex).ActualLocation.HorizontalAzimuth = SelectedActualPoint.GetSphericalAzimuth
                            TargetStimulusLocations(locationIndex).ActualLocation.Elevation = SelectedActualPoint.GetSphericalElevation
                            TargetStimulusLocations(locationIndex).ActualLocation.Distance = SelectedActualPoint.GetSphericalDistance
                            TargetStimulusLocations(locationIndex).ActualLocation.BinauralDelay = FrontalTwoLeftEarsKernel.BinauralDelay
                        Next
                        For locationIndex = 0 To Me.MaskerLocations.Length - 1
                            MaskerLocations(locationIndex).ActualLocation = New SoundSourceLocation
                            MaskerLocations(locationIndex).ActualLocation.HorizontalAzimuth = SelectedActualPoint.GetSphericalAzimuth
                            MaskerLocations(locationIndex).ActualLocation.Elevation = SelectedActualPoint.GetSphericalElevation
                            MaskerLocations(locationIndex).ActualLocation.Distance = SelectedActualPoint.GetSphericalDistance
                            MaskerLocations(locationIndex).ActualLocation.BinauralDelay = FrontalTwoLeftEarsKernel.BinauralDelay
                        Next
                        For locationIndex = 0 To Me.BackgroundLocations.Length - 1
                            BackgroundLocations(locationIndex).ActualLocation = New SoundSourceLocation
                            BackgroundLocations(locationIndex).ActualLocation.HorizontalAzimuth = SelectedActualPoint.GetSphericalAzimuth
                            BackgroundLocations(locationIndex).ActualLocation.Elevation = SelectedActualPoint.GetSphericalElevation
                            BackgroundLocations(locationIndex).ActualLocation.Distance = SelectedActualPoint.GetSphericalDistance
                            BackgroundLocations(locationIndex).ActualLocation.BinauralDelay = FrontalTwoLeftEarsKernel.BinauralDelay
                        Next

                        'Phase inverting if needed
                        If Me.BmldSignalMode = BmldModes.BinauralPhaseInverted Then
                            'Phase inverting signal, by inverting the right channel
                            TargetSound.InvertChannel(2)

                            'Inverting also the other sounds if they exist (for export)
                            If BackgroundSpeechItemsSound IsNot Nothing Then BackgroundSpeechItemsSound.InvertChannel(2)

                        End If

                        If Me.BmldNoiseMode = BmldModes.BinauralPhaseInverted Then
                            'Phase inverting noise, by inverting the right channel
                            NontargetSound.InvertChannel(2)

                            'Inverting also the other sounds if they exist (for export)
                            If MaskerItemsSound IsNot Nothing Then MaskerItemsSound.InvertChannel(2)
                            If BackgroundNonspeechItemsSound IsNot Nothing Then BackgroundNonspeechItemsSound.InvertChannel(2)

                        End If
                    End If

                    'Creating the combined mix
                    MixedTestTrialSound = Audio.DSP.SuperpositionSounds({TargetSound, NontargetSound}.ToList)

                    'Stores the sounds in TrialSoundsToExport, to be exported later.

                    'Add target and masker sound file indices, as well as background start samples (i.e. the first samples at which each file was read from the source background sould file)
                    SelectedTargetIndexString = SelectedMediaIndex
                    SelectedMaskerIndicesString = String.Join("_", SelectedMaskerIndices)
                    BackgroundStartSamplesString = ""
                    Dim BackgroundStartSamplesList As New List(Of Integer)
                    For Each Background In Backgrounds
                        BackgroundStartSamplesList.Add(Background.Item3)
                    Next
                    BackgroundStartSamplesString = String.Join("_", BackgroundStartSamplesList)

                    BackgroundSpeechStartSamplesString = ""
                    Dim BackgroundSpeechSectionSampleList As New List(Of Integer)
                    For Each BackgroundSpeechSection In BackgroundSpeechSelections
                        BackgroundSpeechSectionSampleList.Add(BackgroundSpeechSection.Item3)
                    Next
                    If BackgroundSpeechSectionSampleList.Count > 0 Then
                        BackgroundSpeechStartSamplesString = String.Join("_", BackgroundSpeechSectionSampleList)
                    End If

                    'Storing the sounds
                    TrialSoundsToExport.Add(New Tuple(Of String, Audio.Sound)("TargetSound", TargetSound))
                    If MaskerItemsSound IsNot Nothing Then TrialSoundsToExport.Add(New Tuple(Of String, Audio.Sound)("MaskersSound", MaskerItemsSound))
                    If BackgroundNonspeechItemsSound IsNot Nothing Then TrialSoundsToExport.Add(New Tuple(Of String, Audio.Sound)("BackgroundNonspeechSound", BackgroundNonspeechItemsSound))
                    If BackgroundSpeechItemsSound IsNot Nothing Then TrialSoundsToExport.Add(New Tuple(Of String, Audio.Sound)("BackgroundSpeechSound", BackgroundSpeechItemsSound))
                    TrialSoundsToExport.Add(New Tuple(Of String, Audio.Sound)("NontargetSound", NontargetSound))
                    TrialSoundsToExport.Add(New Tuple(Of String, Audio.Sound)("MixedTestTrialSound", MixedTestTrialSound))

                    'Empties the TrialSoundsToExport if no trial sounds should be exported
                    If ParentTestUnit.ParentMeasurement.ExportTrialSoundFiles = False Then TrialSoundsToExport.Clear()

                Else

                    'Creating the mix by calling CreateSoundScene of the current Mixer
                    MixedTestTrialSound = SelectedTransducer.Mixer.CreateSoundScene(ItemList, Me.SoundPropagationType)
                End If

                'Getting the applied gain for all items
                GainList.Clear()
                GainList.Add(SoundSceneItem.SoundSceneItemRoles.Target, New List(Of String))
                GainList.Add(SoundSceneItem.SoundSceneItemRoles.Masker, New List(Of String))
                GainList.Add(SoundSceneItem.SoundSceneItemRoles.BackgroundNonspeech, New List(Of String))
                GainList.Add(SoundSceneItem.SoundSceneItemRoles.BackgroundSpeech, New List(Of String))
                For Each Item In ItemList
                    If Item.AppliedGain IsNot Nothing Then
                        If Item.AppliedGain.Value.HasValue Then
                            GainList(Item.Role).Add(Math.Round(Item.AppliedGain.Value.Value, 1))
                        Else
                            GainList(Item.Role).Add("NA")
                        End If
                    End If
                Next

                If LogToConsole = True Then Console.WriteLine("Mixed sound in " & MixStopWatch.ElapsedMilliseconds & " ms.")

                'TODO: Here we can simulate and/or compensate for hearing loss:
                'SimulateHearingLoss,
                'CompensateHearingLoss

                Sound = MixedTestTrialSound

                'Mixing non-presented trials as well
                If PseudoTrials IsNot Nothing Then
                    For Each NonpresentedAlternativeTrial In PseudoTrials
                        NonpresentedAlternativeTrial.MixSound(SelectedTransducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, SipMeasurementRandomizer, TrialSoundMaxDuration, UseBackgroundSpeech, FixedMaskerIndices, FixedSpeechIndex)
                    Next
                End If

                'Stores the testword start sample
                Me.TargetStartSample = TestWordStartSample

            Catch ex As Exception
                Utils.SendInfoToLog(ex.ToString, "ExceptionsDuringTesting")
            End Try

        End Sub


        Private _EstimatedSuccessProbability As Double? = Nothing

        Public ReadOnly Property EstimatedSuccessProbability(ByVal ReCalculate As Boolean) As Double
            Get
                If _EstimatedSuccessProbability.HasValue = False Or ReCalculate = True Then
                    UpdateEstimatedSuccessProbability()
                End If
                Return _EstimatedSuccessProbability
            End Get
        End Property

        Public Sub OverideEstimatedSuccessProbabilityValue(ByVal Newvalue As Double)
            _EstimatedSuccessProbability = Newvalue
        End Sub

        Public Sub UpdateEstimatedSuccessProbability()

            'Select Case Me.ParentTestUnit.ParentMeasurement.Prediction.Models.SelectedModel.GetSwedishSipTestA

            'Getting predictors
            Dim PDL As Double = Me.PhonemeDiscriminabilityLevel

            'TODO! Decide whether exact duration of the presented phoneme should be used (slower, but as in Witte,2021) or if average test phoneme duration (for the five exemplar recordings of each word) should be used (a little faster)
            Dim DurationList = Me.SpeechMaterialComponent.GetDurationOfContrastingComponents(Me.MediaSet, SpeechMaterialComponent.LinguisticLevels.Phoneme, Me.SelectedMediaIndex, 1)
            Dim TPD As Double = DurationList(0)
            'Dim TPD As Double = Me.SpeechMaterial.GetNumericMediaSetVariableValue(MediaSet, "Tc")

            Dim Z As Double = Me.SpeechMaterialComponent.GetNumericVariableValue("Z")
            Dim iPNDP As Double = 1 / Me.SpeechMaterialComponent.GetNumericVariableValue("PNDP")
            Dim PP As Double = Me.SpeechMaterialComponent.GetNumericVariableValue("PP")
            Dim PT As Double = Me.SpeechMaterialComponent.GetAncestorAtLevel(SpeechMaterialComponent.LinguisticLevels.List).GetNumericVariableValue("V")

            'Calculating centred and scaled values for PDL and PBTAH
            Dim PDL_gmc_div = (PDL - 10.46) / 50
            Dim Z_gmc_div = (Z - 3.7) / 10
            Dim iPNDP_gmc_div = (iPNDP - 20.6) / 50

            'Calculating model eta
            Dim Eta = 0.73 +
                8.22 * PDL_gmc_div +
                5.11 * (TPD - 0.33) +
                4.24 * Z_gmc_div -
                1.44 * iPNDP_gmc_div +
                4.58 * (PP - 0.92) -
                1.1 * (PT - 0.43) -
                7.25 * PDL_gmc_div * Z_gmc_div +
                7.47 * PDL_gmc_div * iPNDP_gmc_div +
                3.32 * PDL_gmc_div * (PT - 0.43)

            'Calculating estimated success probability
            Dim p As Double = 1 / 3 + (2 / 3) * (1 / (1 + Math.Exp(-Eta)))

            If LogToConsole = True Then
                Dim LogList As New List(Of String)
                LogList.Add("PrimaryStringRepresentation:" & vbTab & Me.SpeechMaterialComponent.PrimaryStringRepresentation)
                LogList.Add("Reference_SPL:" & vbTab & Reference_SPL)
                LogList.Add("PNR:" & vbTab & PNR)
                LogList.Add("PDL:" & vbTab & PDL)
                LogList.Add("TPD:" & vbTab & TPD)
                LogList.Add("Z:" & vbTab & Z)
                LogList.Add("iPNDP:" & vbTab & iPNDP)
                LogList.Add("PP:" & vbTab & PP)
                LogList.Add("PT:" & vbTab & PT)
                LogList.Add("PDL_gmc_div:" & vbTab & PDL_gmc_div)
                LogList.Add("Z_gmc_div:" & vbTab & Z_gmc_div)
                LogList.Add("iPNDP_gmc_div:" & vbTab & iPNDP_gmc_div)
                LogList.Add("Eta:" & vbTab & Eta)
                LogList.Add("p:" & vbTab & p)

                Console.WriteLine(String.Join(vbCrLf, LogList))
            End If

            'Stores the result in PredictedSuccessProbability
            _EstimatedSuccessProbability = p

        End Sub

        Private _PhonemeDiscriminabilityLevel As Double? = Nothing

        Public ReadOnly Property PhonemeDiscriminabilityLevel(Optional ByVal ReCalculate As Boolean = True,
                                                              Optional ByVal SpeechSpectrumLevelsVariableNamePrefix As String = "SLs",
                                                              Optional ByVal MaskerSpectrumLevelsVariableNamePrefix As String = "SLm") As Double
            Get
                If _PhonemeDiscriminabilityLevel.HasValue = False Or ReCalculate = True Then
                    UpdatePhonemeDiscriminabilityLevel(SpeechSpectrumLevelsVariableNamePrefix, MaskerSpectrumLevelsVariableNamePrefix)
                End If
                Return _PhonemeDiscriminabilityLevel
            End Get
        End Property

        Public Sub SetPhonemeDiscriminabilityLevelExternally(ByVal Value As Double)
            _PhonemeDiscriminabilityLevel = Value
        End Sub

        Public Sub UpdatePhonemeDiscriminabilityLevel(Optional ByVal SpeechSpectrumLevelsVariableNamePrefix As String = "SLs",
                                                              Optional ByVal MaskerSpectrumLevelsVariableNamePrefix As String = "SLm")

            'Using thresholds and gain data from the side with the best aided thresholds (selecting side separately for each critical band)
            Dim Thresholds(Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies.Length - 1) As Double
            Dim Gain(Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies.Length - 1) As Double

            For i = 0 To Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies.Length - 1
                'TODO: should we allow for the lack of gain data here, or should we always use a gain of zero when no hearing aid is used?
                Dim AidedThreshold_Left As Double = Me.ParentTestUnit.ParentMeasurement.SelectedAudiogramData.Cb_Left_AC(i) - Me.ParentTestUnit.ParentMeasurement.HearingAidGain.LeftSideGain(i).Gain
                Dim AidedThreshold_Right As Double = Me.ParentTestUnit.ParentMeasurement.SelectedAudiogramData.Cb_Right_AC(i) - Me.ParentTestUnit.ParentMeasurement.HearingAidGain.RightSideGain(i).Gain

                If AidedThreshold_Left < AidedThreshold_Right Then
                    Thresholds(i) = Me.ParentTestUnit.ParentMeasurement.SelectedAudiogramData.Cb_Left_AC(i)
                    Gain(i) = Me.ParentTestUnit.ParentMeasurement.HearingAidGain.LeftSideGain(i).Gain
                Else
                    Thresholds(i) = Me.ParentTestUnit.ParentMeasurement.SelectedAudiogramData.Cb_Right_AC(i)
                    Gain(i) = Me.ParentTestUnit.ParentMeasurement.HearingAidGain.RightSideGain(i).Gain
                End If
            Next

            'Getting spectral levels
            Dim CorrectResponseSpectralLevels(Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies.Length - 1) As Double
            Dim MaskerSpectralLevels(Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies.Length - 1) As Double
            Dim Siblings = Me.SpeechMaterialComponent.GetSiblingsExcludingSelf
            Dim IncorrectResponsesSpectralLevels As New List(Of Double())
            For Each Sibling In Siblings
                Dim IncorrectResponseSpectralLevels(Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies.Length - 1) As Double
                IncorrectResponsesSpectralLevels.Add(IncorrectResponseSpectralLevels)
            Next

            'Calculating the difference between the standard ReferenceSpeechMaterialLevel_SPL (68.34 dB SPL) reference level and the one currently used
            Dim RefLevelDifference As Double = Me.Reference_SPL - ReferenceSpeechMaterialLevel_SPL

            'Getting the current gain, compared to the reference test-word and masker levels
            Dim CurrentSpeechGain As Double = TestWordLevel - ReferenceTestWordLevel_SPL + RefLevelDifference ' SpeechMaterial.GetNumericMediaSetVariableValue(MediaSet, "Lc") 'TestStimulus.TestWord_ReferenceSPL
            If LogToConsole = True Then Console.WriteLine(Me.SpeechMaterialComponent.PrimaryStringRepresentation & " PNR: " & Me.PNR & " SpeechGain : " & CurrentSpeechGain)

            Dim CurrentMaskerGain As Double = TargetMasking_SPL - ReferenceContrastingPhonemesLevel_SPL + RefLevelDifference 'Audio.Standard_dBFS_To_dBSPL(Common.SipTestReferenceMaskerLevel_FS) 'TODO: In the SiP-test maskers sound files the level is set to -30 dB FS across the whole sounds. A more detailed sound level data could be used instead!
            If LogToConsole = True Then Console.WriteLine(Me.SpeechMaterialComponent.PrimaryStringRepresentation & " PNR: " & Me.PNR & " CurrentMaskerGain: " & CurrentMaskerGain)

            For i = 0 To Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies.Length - 1
                Dim VariableNameSuffix = Math.Round(Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies(i)).ToString("00000")
                Dim SLsName As String = SpeechSpectrumLevelsVariableNamePrefix & "_" & VariableNameSuffix
                Dim SLmName As String = MaskerSpectrumLevelsVariableNamePrefix & "_" & VariableNameSuffix

                'Retreiving the reference levels and adjusts them by the CurrentSpeechGain and CurrentMaskerGain
                CorrectResponseSpectralLevels(i) = Me.SpeechMaterialComponent.GetNumericMediaSetVariableValue(MediaSet, SLsName) + CurrentSpeechGain
                MaskerSpectralLevels(i) = Me.SpeechMaterialComponent.GetAncestorAtLevel(SpeechMaterialComponent.LinguisticLevels.List).GetNumericMediaSetVariableValue(MediaSet, SLmName) + CurrentMaskerGain
                For s = 0 To Siblings.Count - 1
                    IncorrectResponsesSpectralLevels(s)(i) = Siblings(s).GetNumericMediaSetVariableValue(MediaSet, SLsName) + CurrentSpeechGain
                Next
            Next

            Dim SRFM As Double?() = GetMLD(Nothing, 1.01) ' Cf Witte's Thesis for the value of c_factor

            'N.B. SRFM and SF 30 need to change if presented in other speaker azimuths!

            'Calculating SDRs
            Dim SDRt = PDL.CalculateSDR(CorrectResponseSpectralLevels, MaskerSpectralLevels, Thresholds, Gain, True, True, SRFM)
            Dim SDRcs As New List(Of Double())
            For s = 0 To Siblings.Count - 1
                Dim SDRc = PDL.CalculateSDR(IncorrectResponsesSpectralLevels(s), MaskerSpectralLevels, Thresholds, Gain, True, True, SRFM)
                SDRcs.Add(SDRc)
            Next

            If LogToConsole = True Then
                Dim LogList As New List(Of String)
                LogList.Add("PrimaryStringRepresentation:" & vbTab & Me.SpeechMaterialComponent.PrimaryStringRepresentation)
                LogList.Add("RefLevelDifference:" & vbTab & RefLevelDifference)
                LogList.Add("CurrentSpeechGain:" & vbTab & CurrentSpeechGain)
                LogList.Add("CurrentMaskerGain:" & vbTab & CurrentMaskerGain)

                LogList.Add("CentreFrequencies:" & vbTab & String.Join("; ", Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies))
                LogList.Add("Thresholds:" & vbTab & String.Join("; ", Thresholds))
                LogList.Add("Gain:" & vbTab & String.Join("; ", Gain))

                LogList.Add("CorrectResponseSpectralLevels:" & vbTab & String.Join("; ", CorrectResponseSpectralLevels))
                For s = 0 To Siblings.Count - 1
                    LogList.Add("IncorrectResponsesSpectralLevels_" & s & ":" & vbTab & String.Join("; ", IncorrectResponsesSpectralLevels(s)))
                Next
                LogList.Add("MaskerSpectralLevels:" & vbTab & String.Join("; ", MaskerSpectralLevels))
                LogList.Add("SRFM:" & vbTab & String.Join("; ", SRFM))

                LogList.Add("SDRt:" & vbTab & String.Join("; ", SDRt))
                For s = 0 To Siblings.Count - 1
                    LogList.Add("SDRcs_" & s & ":" & vbTab & String.Join("; ", SDRcs(s)))
                Next

                Console.WriteLine(String.Join(vbCrLf, LogList))
            End If

            'Calculating PDL
            _PhonemeDiscriminabilityLevel = PDL.CalculatePDL(SDRt, SDRcs)

        End Sub

        Public Sub ExportTrialResult(ByVal IncludeHeadings As Boolean, ByVal SkipExportOfSoundFiles As Boolean)

            Dim FilePath = IO.Path.Combine(Me.ParentTestUnit.ParentMeasurement.TrialResultsExportFolder, "TestResults")

            Dim OutputLines As New List(Of String)

            If IncludeHeadings = True Then
                OutputLines.Add(SipMeasurement.NewMeasurementMarker)
                OutputLines.Add(SipTrial.CreateExportHeadings())
            End If

            Dim TrialExportString = Me.CreateExportString(SkipExportOfSoundFiles)

            OutputLines.Add(TrialExportString)

            Utils.SendInfoToLog(String.Join(vbCrLf, OutputLines), IO.Path.GetFileNameWithoutExtension(FilePath), IO.Path.GetDirectoryName(FilePath), True, True)

        End Sub

        Public Shared Function CreateExportHeadings() As String

            Dim Headings As New List(Of String)
            Headings.Add("ParticipantID")
            Headings.Add("MeasurementDateTime")
            Headings.Add("Description")
            Headings.Add("TestUnitIndex")
            Headings.Add("TestUnitDescription")
            Headings.Add("SpeechMaterialComponentID")
            Headings.Add("TestWordGroup")
            Headings.Add("MediaSetName")
            Headings.Add("PresentationOrder")
            Headings.Add("ReferenceSpeechMaterialLevel_SPL")
            Headings.Add("ReferenceContrastingPhonemesLevel_SPL")
            Headings.Add("Reference_SPL")
            Headings.Add("PNR")
            Headings.Add("TargetMasking_SPL")
            Headings.Add("TestWordLevelLimit")
            Headings.Add("ContextSpeechLimit")
            Headings.Add("EstimatedSuccessProbability")
            Headings.Add("AdjustedSuccessProbability")
            Headings.Add("SoundPropagationType")

            Headings.Add("IntendedTargetLocation_Distance")
            Headings.Add("IntendedTargetLocation_HorizontalAzimuth")
            Headings.Add("IntendedTargetLocation_Elevation")
            Headings.Add("PresentedTargetLocation_Distance")
            Headings.Add("PresentedTargetLocation_HorizontalAzimuth")
            Headings.Add("PresentedTargetLocation_Elevation")
            Headings.Add("PresentedTargetLocation_BinauralDelay_Left")
            Headings.Add("PresentedTargetLocation_BinauralDelay_Right")

            Headings.Add("IntendedMaskerLocations_Distance")
            Headings.Add("IntendedMaskerLocations_HorizontalAzimuth")
            Headings.Add("IntendedMaskerLocations_Elevation")
            Headings.Add("PresentedMaskerLocations_Distance")
            Headings.Add("PresentedMaskerLocations_HorizontalAzimuth")
            Headings.Add("PresentedMaskerLocations_Elevation")
            Headings.Add("PresentedMaskerLocation_BinauralDelay_Left")
            Headings.Add("PresentedMaskerLocation_BinauralDelay_Right")

            Headings.Add("IntendedBackgroundLocations_Distance")
            Headings.Add("IntendedBackgroundLocations_HorizontalAzimuth")
            Headings.Add("IntendedBackgroundLocations_Elevation")
            Headings.Add("PresentedBackgroundLocations_Distance")
            Headings.Add("PresentedBackgroundLocations_HorizontalAzimuth")
            Headings.Add("PresentedBackgroundLocations_Elevation")
            Headings.Add("PresentedBackgroundLocation_BinauralDelay_Left")
            Headings.Add("PresentedBackgroundLocation_BinauralDelay_Right")

            Headings.Add("IsBmldTrial")
            Headings.Add("BmldNoiseMode")
            Headings.Add("BmldSignalMode")

            Headings.Add("Response")
            Headings.Add("Result")
            Headings.Add("Score")
            Headings.Add("ResponseTime")
            Headings.Add("ResponseAlternativeCount")
            Headings.Add("IsTestTrial")
            Headings.Add("PhonemeDiscriminabilityLevel")

            Headings.Add("PrimaryStringRepresentation")

            Headings.Add("Spelling")
            Headings.Add("SpellingAFC")
            Headings.Add("Transcription")
            Headings.Add("TranscriptionAFC")
            Headings.Add("PseudoTrialIds")
            Headings.Add("PseudoTrialSpellings")

            Headings.Add("TargetTrial_ExportedTrialSoundFiles")
            Headings.Add("PseudoTrials_ExportedTrialSoundFiles")

            Headings.Add("TargetTrial_SelectedTargetSoundIndex")
            Headings.Add("TargetTrial_SelectedMaskerSoundIndices")
            Headings.Add("TargetTrial_BackgroundNonspeechSoundStartSamples")
            Headings.Add("TargetTrial_BackgroundSpeechSoundStartSamples")

            Headings.Add("PseudoTrials_SelectedTargetSoundIndex")
            Headings.Add("PseudoTrials_SelectedMaskerSoundIndices")
            Headings.Add("PseudoTrials_BackgroundNonspeechSoundStartSamples")
            Headings.Add("PseudoTrials_BackgroundSpeechSoundStartSamples")

            Headings.Add("TargetTrial_BackgroundNonSpeechDucking")
            Headings.Add("PseudoTrials_BackgroundNonSpeechDucking")

            Headings.Add("TargetTrial_ContextRegionSpeech_SPL")
            Headings.Add("TargetTrial_TestWordLevel")
            Headings.Add("TargetTrial_ReferenceTestWordLevel_SPL")

            Headings.Add("PseudoTrials_ContextRegionSpeech_SPL")
            Headings.Add("PseudoTrials_TestWordLevel")
            Headings.Add("PseudoTrials_ReferenceTestWordLevel_SPL")

            Headings.Add("TargetStartSample")
            Headings.Add("PseudoTrials_TargetStartSample")
            Headings.Add("TestPhonemeStartSample")
            Headings.Add("TestPhonemelength")
            Headings.Add("PseudoTrials_TP_StartSamples")
            Headings.Add("PseudoTrials_TP_Length")

            Headings.Add("TargetTrial_Gains")
            Headings.Add("PseudoTrials_Gains")

            Return String.Join(vbTab, Headings)

        End Function

        Public Function CreateExportString(Optional ByVal SkipExportOfSoundFiles As Boolean = True) As String

            Dim TrialList As New List(Of String)
            TrialList.Add(Me.ParentTestUnit.ParentMeasurement.ParticipantID)
            TrialList.Add(Me.ParentTestUnit.ParentMeasurement.MeasurementDateTime.ToString(System.Globalization.CultureInfo.InvariantCulture))
            TrialList.Add(Me.ParentTestUnit.ParentMeasurement.Description)
            TrialList.Add(Me.ParentTestUnit.ParentMeasurement.GetParentTestUnitIndex(Me))
            TrialList.Add(Me.ParentTestUnit.Description)
            TrialList.Add(Me.SpeechMaterialComponent.Id)
            TrialList.Add(Me.SpeechMaterialComponent.ParentComponent.PrimaryStringRepresentation)
            TrialList.Add(Me.MediaSet.MediaSetName)
            TrialList.Add(Me.PresentationOrder)
            TrialList.Add(Me.ReferenceSpeechMaterialLevel_SPL)
            TrialList.Add(Me.ReferenceContrastingPhonemesLevel_SPL)
            TrialList.Add(Me.Reference_SPL)
            TrialList.Add(Me.PNR)
            If TargetMasking_SPL.HasValue = True Then
                TrialList.Add(TargetMasking_SPL)
            Else
                TrialList.Add("NA")
            End If
            TrialList.Add(TestWordLevelLimit)
            TrialList.Add(ContextSpeechLimit)

            If Me.ParentTestUnit.ParentMeasurement.SelectedAudiogramData IsNot Nothing Then
                TrialList.Add(Me.EstimatedSuccessProbability(False))
                TrialList.Add(Me.AdjustedSuccessProbability)
            Else
                TrialList.Add("No audiogram stored - cannot calculate")
                TrialList.Add("No audiogram stored - cannot calculate")
            End If
            TrialList.Add(Me.SoundPropagationType.ToString)

            If Me.TargetStimulusLocations.Length > 0 Then
                Dim Distances As New List(Of String)
                Dim HorizontalAzimuths As New List(Of String)
                Dim Elevations As New List(Of String)
                Dim ActualDistances As New List(Of String)
                Dim ActualHorizontalAzimuths As New List(Of String)
                Dim ActualElevations As New List(Of String)
                Dim ActualBinauralDelay_Left As New List(Of String)
                Dim ActualBinauralDelay_Right As New List(Of String)
                For i = 0 To Me.TargetStimulusLocations.Length - 1
                    Distances.Add(Me.TargetStimulusLocations(i).Distance)
                    HorizontalAzimuths.Add(Me.TargetStimulusLocations(i).HorizontalAzimuth)
                    Elevations.Add(Me.TargetStimulusLocations(i).Elevation)
                    If Me.TargetStimulusLocations(i).ActualLocation Is Nothing Then Me.TargetStimulusLocations(i).ActualLocation = New SoundSourceLocation
                    ActualDistances.Add(Me.TargetStimulusLocations(i).ActualLocation.Distance)
                    ActualHorizontalAzimuths.Add(Me.TargetStimulusLocations(i).ActualLocation.HorizontalAzimuth)
                    ActualElevations.Add(Me.TargetStimulusLocations(i).ActualLocation.Elevation)
                    ActualBinauralDelay_Left.Add(Me.TargetStimulusLocations(i).ActualLocation.BinauralDelay.LeftDelay)
                    ActualBinauralDelay_Right.Add(Me.TargetStimulusLocations(i).ActualLocation.BinauralDelay.RightDelay)
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

            If Me.MaskerLocations.Length > 0 Then
                Dim Distances As New List(Of String)
                Dim HorizontalAzimuths As New List(Of String)
                Dim Elevations As New List(Of String)
                Dim ActualDistances As New List(Of String)
                Dim ActualHorizontalAzimuths As New List(Of String)
                Dim ActualElevations As New List(Of String)
                Dim ActualBinauralDelay_Left As New List(Of String)
                Dim ActualBinauralDelay_Right As New List(Of String)
                For i = 0 To Me.MaskerLocations.Length - 1
                    Distances.Add(Me.MaskerLocations(i).Distance)
                    HorizontalAzimuths.Add(Me.MaskerLocations(i).HorizontalAzimuth)
                    Elevations.Add(Me.MaskerLocations(i).Elevation)
                    If Me.MaskerLocations(i).ActualLocation Is Nothing Then Me.MaskerLocations(i).ActualLocation = New SoundSourceLocation
                    ActualDistances.Add(Me.MaskerLocations(i).ActualLocation.Distance)
                    ActualHorizontalAzimuths.Add(Me.MaskerLocations(i).ActualLocation.HorizontalAzimuth)
                    ActualElevations.Add(Me.MaskerLocations(i).ActualLocation.Elevation)
                    ActualBinauralDelay_Left.Add(Me.MaskerLocations(i).ActualLocation.BinauralDelay.LeftDelay)
                    ActualBinauralDelay_Right.Add(Me.MaskerLocations(i).ActualLocation.BinauralDelay.RightDelay)
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

            If Me.BackgroundLocations.Length > 0 Then
                Dim Distances As New List(Of String)
                Dim HorizontalAzimuths As New List(Of String)
                Dim Elevations As New List(Of String)
                Dim ActualDistances As New List(Of String)
                Dim ActualHorizontalAzimuths As New List(Of String)
                Dim ActualElevations As New List(Of String)
                Dim ActualBinauralDelay_Left As New List(Of String)
                Dim ActualBinauralDelay_Right As New List(Of String)
                For i = 0 To Me.BackgroundLocations.Length - 1
                    Distances.Add(Me.BackgroundLocations(i).Distance)
                    HorizontalAzimuths.Add(Me.BackgroundLocations(i).HorizontalAzimuth)
                    Elevations.Add(Me.BackgroundLocations(i).Elevation)
                    If Me.BackgroundLocations(i).ActualLocation Is Nothing Then Me.BackgroundLocations(i).ActualLocation = New SoundSourceLocation
                    ActualDistances.Add(Me.BackgroundLocations(i).ActualLocation.Distance)
                    ActualHorizontalAzimuths.Add(Me.BackgroundLocations(i).ActualLocation.HorizontalAzimuth)
                    ActualElevations.Add(Me.BackgroundLocations(i).ActualLocation.Elevation)
                    ActualBinauralDelay_Left.Add(Me.BackgroundLocations(i).ActualLocation.BinauralDelay.LeftDelay)
                    ActualBinauralDelay_Right.Add(Me.BackgroundLocations(i).ActualLocation.BinauralDelay.RightDelay)
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

            TrialList.Add(Me.IsBmldTrial)
            If Me.IsBmldTrial = True Then
                TrialList.Add(Me.BmldNoiseMode.ToString)
                TrialList.Add(Me.BmldSignalMode.ToString)
            Else
                TrialList.Add("")
                TrialList.Add("")
            End If

            TrialList.Add(Me.Response)
            TrialList.Add(Me.Result.ToString)
            TrialList.Add(Me.Score)
            TrialList.Add(Me.ResponseTime.ToString(System.Globalization.CultureInfo.InvariantCulture))
            TrialList.Add(Me.ResponseAlternativeCount)
            TrialList.Add(Me.IsTestTrial.ToString)
            If Me.ParentTestUnit.ParentMeasurement.SelectedAudiogramData IsNot Nothing Then
                TrialList.Add(Me.PhonemeDiscriminabilityLevel(False))
            Else
                TrialList.Add("No audiogram stored")
            End If

            TrialList.Add(Me.SpeechMaterialComponent.PrimaryStringRepresentation)
            TrialList.Add(Me.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling"))
            TrialList.Add(Me.SpeechMaterialComponent.GetCategoricalVariableValue("SpellingAFC"))
            TrialList.Add(Me.SpeechMaterialComponent.GetCategoricalVariableValue("Transcription"))
            TrialList.Add(Me.SpeechMaterialComponent.GetCategoricalVariableValue("TranscriptionAFC"))

            Dim PseudoTrialIds As New List(Of String)
            Dim PseudoTrialSpellings As New List(Of String)
            For Each PseudoTrial In PseudoTrials
                PseudoTrialIds.Add(PseudoTrial.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling"))
                PseudoTrialSpellings.Add(PseudoTrial.SpeechMaterialComponent.Id)
            Next
            TrialList.Add(String.Join("; ", PseudoTrialIds))
            TrialList.Add(String.Join("; ", PseudoTrialSpellings))

            'Adding export of sound files,
            Dim ExportedSoundFilesList As New List(Of String)
            If SkipExportOfSoundFiles = False Then
                For i = 0 To Me.TrialSoundsToExport.Count - 1
                    Dim ExportSound = Me.TrialSoundsToExport(i).Item2
                    Dim FileName = IO.Path.Combine(Me.ParentTestUnit.ParentMeasurement.TrialResultsExportFolder, "TrialSoundFiles", "Trial_" & Me.PresentationOrder & "_" & Me.TrialSoundsToExport(i).Item1 & "_" & Me.SpeechMaterialComponent.Id & ".wav")
                    ExportSound.WriteWaveFile(FileName)
                    ExportedSoundFilesList.Add(FileName)
                Next
            End If
            TrialList.Add(String.Join(";", ExportedSoundFilesList))

            Dim ExportedPseudoTrialSoundFilesList As New List(Of String)
            If SkipExportOfSoundFiles = False Then
                If PseudoTrials IsNot Nothing Then
                    For Each PseudoTrial In PseudoTrials
                        For i = 0 To PseudoTrial.TrialSoundsToExport.Count - 1
                            Dim ExportSound = PseudoTrial.TrialSoundsToExport(i).Item2
                            Dim FileName = IO.Path.Combine(Me.ParentTestUnit.ParentMeasurement.TrialResultsExportFolder, "TrialSoundFiles", "Trial_" & Me.PresentationOrder & "_Pseudo_" & PseudoTrial.TrialSoundsToExport(i).Item1 & "_" & PseudoTrial.SpeechMaterialComponent.Id & ".wav")
                            ExportSound.WriteWaveFile(FileName)
                            ExportedPseudoTrialSoundFilesList.Add(FileName)
                        Next
                    Next
                End If
            End If
            TrialList.Add(String.Join(";", ExportedPseudoTrialSoundFilesList))

            TrialList.Add(SelectedTargetIndexString)
            TrialList.Add(SelectedMaskerIndicesString)
            TrialList.Add(BackgroundStartSamplesString)
            TrialList.Add(BackgroundSpeechStartSamplesString)

            Dim PseudoTrial_SelectedTargetIndexStringList As New List(Of String)
            Dim PseudoTrial_SelectedMaskerIndicesStringList As New List(Of String)
            Dim PseudoTrial_BackgroundStartSamplesStringList As New List(Of String)
            Dim PseudoTrial_BackgroundSpeechStartSamplesStringList As New List(Of String)
            For Each PseduTrial In PseudoTrials
                PseudoTrial_SelectedTargetIndexStringList.Add(PseduTrial.SelectedTargetIndexString)
                PseudoTrial_SelectedMaskerIndicesStringList.Add(PseduTrial.SelectedMaskerIndicesString)
                PseudoTrial_BackgroundStartSamplesStringList.Add(PseduTrial.BackgroundStartSamplesString)
                PseudoTrial_BackgroundSpeechStartSamplesStringList.Add(PseduTrial.BackgroundSpeechStartSamplesString)
            Next
            TrialList.Add(String.Join(";", PseudoTrial_SelectedTargetIndexStringList))
            TrialList.Add(String.Join(";", PseudoTrial_SelectedMaskerIndicesStringList))
            TrialList.Add(String.Join(";", PseudoTrial_BackgroundStartSamplesStringList))
            TrialList.Add(String.Join(";", PseudoTrial_BackgroundSpeechStartSamplesStringList))

            TrialList.Add(BackgroundNonSpeechDucking)
            Dim PseudoTrial_BackgroundNonSpeechDuckingList As New List(Of String)
            For Each PseduTrial In PseudoTrials
                PseudoTrial_BackgroundNonSpeechDuckingList.Add(PseduTrial.BackgroundNonSpeechDucking)
            Next
            TrialList.Add(String.Join(";", PseudoTrial_BackgroundNonSpeechDuckingList))

            TrialList.Add(ContextRegionSpeech_SPL)
            If TestWordLevel.HasValue = True Then
                TrialList.Add(TestWordLevel)
            Else
                TrialList.Add("NA")
            End If
            TrialList.Add(ReferenceTestWordLevel_SPL)

            Dim ContextRegionSpeech_SPL_List As New List(Of String)
            Dim TestWordLevel_List As New List(Of String)
            Dim ReferenceTestWordLevel_SPL_List As New List(Of String)
            For Each PseduTrial In PseudoTrials
                ContextRegionSpeech_SPL_List.Add(PseduTrial.ContextRegionSpeech_SPL)
                If PseduTrial.TestWordLevel.HasValue = True Then
                    TestWordLevel_List.Add(PseduTrial.TestWordLevel)
                Else
                    TestWordLevel_List.Add("NA")
                End If
                ReferenceTestWordLevel_SPL_List.Add(PseduTrial.ReferenceTestWordLevel_SPL)
            Next
            TrialList.Add(String.Join(";", ContextRegionSpeech_SPL_List))
            TrialList.Add(String.Join(";", TestWordLevel_List))
            TrialList.Add(String.Join(";", ReferenceTestWordLevel_SPL_List))

            'Target Startsamples
            TrialList.Add(TargetStartSample)
            Dim PseudoTrials_TargetStartSample As New List(Of String)
            For Each PseduTrial In PseudoTrials
                PseudoTrials_TargetStartSample.Add(PseduTrial.TargetStartSample)
            Next
            TrialList.Add(String.Join(";", PseudoTrials_TargetStartSample))

            'Test phoneme start sample and length
            If TargetInitialMargins.Count = 0 Then TargetInitialMargins.Add(0) ' Adding an initial margin of zero if for some reason empty
            Dim TP_SaL = GetTestPhonemeStartAndLength(TargetInitialMargins(0)) ' N.B. / TODO: Here initial margins are assumed only for one target. Need to be changed if several targets with different initial marginsa are to be used.
            Dim TestPhonemeStartSample As Integer = TP_SaL.Item1
            Dim TestPhonemelength As Integer = TP_SaL.Item2
            TrialList.Add(TestPhonemeStartSample)
            TrialList.Add(TestPhonemelength)

            Dim PseudoTrials_TP_StartSamples As New List(Of String)
            Dim PseudoTrials_TP_Length As New List(Of String)
            For pseudoTrialIndex = 0 To PseudoTrials.Count - 1
                If PseudoTrials(pseudoTrialIndex).TargetInitialMargins.Count = 0 Then PseudoTrials(pseudoTrialIndex).TargetInitialMargins.Add(0) ' Adding an initial margin of zero if for some reason empty
                Dim PS_TP_SaL = PseudoTrials(pseudoTrialIndex).GetTestPhonemeStartAndLength(PseudoTrials(pseudoTrialIndex).TargetInitialMargins(0)) ' N.B. / TODO: Here initial margins are assumed only for one target. Need to be changed if several targets with different initial marginsa are to be used.
                PseudoTrials_TP_StartSamples.Add(PS_TP_SaL.Item1)
                PseudoTrials_TP_Length.Add(PS_TP_SaL.Item2)
            Next
            TrialList.Add(String.Join(";", PseudoTrials_TP_StartSamples))
            TrialList.Add(String.Join(";", PseudoTrials_TP_Length))

            'Gains
            Dim TargetTrialGains As New List(Of String)
            For Each Item In GainList
                TargetTrialGains.Add(Item.Key.ToString & ": " & String.Join(";", Item.Value))
            Next
            TrialList.Add(String.Join(" / ", TargetTrialGains))

            Dim PseudoTrialsGains As New List(Of String)
            For pseudoTrialIndex = 0 To PseudoTrials.Count - 1
                Dim PseudoTrialGains As New List(Of String)
                For Each Item In PseudoTrials(pseudoTrialIndex).GainList
                    PseudoTrialGains.Add(Item.Key.ToString & ": " & String.Join(";", Item.Value))
                Next
                PseudoTrialsGains.Add(String.Join(" / ", PseudoTrialGains))
            Next
            TrialList.Add(String.Join(" | ", PseudoTrialsGains))

            Return String.Join(vbTab, TrialList)

        End Function

        Public Function GetTestPhonemeStartAndLength(Optional ByVal InitialMargin As Integer = 0) As Tuple(Of Integer, Integer)

            Dim TestPhonemeStartSample As Integer
            Dim TestPhonemeLength As Integer
            Dim ChildPhonemes = Me.SpeechMaterialComponent.GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.Phoneme)
            For Each ChildPhoneme In ChildPhonemes
                If ChildPhoneme.IsContrastingComponent = True Then
                    Dim TestPhonemeSma = ChildPhoneme.GetCorrespondingSmaComponent(Me.MediaSet, Me.SelectedMediaIndex, 1, True)
                    If TestPhonemeSma.Count > 0 Then
                        TestPhonemeStartSample = TestPhonemeSma(0).StartSample - InitialMargin
                        TestPhonemeLength = TestPhonemeSma(0).Length
                    End If
                    Exit For
                End If
            Next

            Return New Tuple(Of Integer, Integer)(TestPhonemeStartSample, TestPhonemeLength)

        End Function

        'Removes all sounds from the trial free up memory
        Public Sub RemoveSounds()

            'Removes the sound, since it's not going to be used again (and could take up quite some memory if kept...)
            If Sound IsNot Nothing Then Sound = Nothing

            If TrialSoundsToExport IsNot Nothing Then
                For i = 0 To TrialSoundsToExport.Count - 1
                    TrialSoundsToExport(i) = New Tuple(Of String, Audio.Sound)(TrialSoundsToExport(i).Item1, Nothing)
                Next
            End If

            'And also the pseudo trial sounds if any
            If PseudoTrials IsNot Nothing Then
                For Each PseudoTrial In PseudoTrials
                    If PseudoTrial.Sound IsNot Nothing Then PseudoTrial.Sound = Nothing

                    If PseudoTrial.TrialSoundsToExport IsNot Nothing Then
                        For i = 0 To PseudoTrial.TrialSoundsToExport.Count - 1
                            PseudoTrial.TrialSoundsToExport(i) = New Tuple(Of String, Audio.Sound)(PseudoTrial.TrialSoundsToExport(i).Item1, Nothing)
                        Next
                    End If
                Next
            End If

        End Sub



    End Class

    Public Enum PossibleResults
        Correct
        Incorrect
        Missing
    End Enum
    Public Enum AdaptiveTypes
        SimpleUpDown
        Fixed
    End Enum
    Public Enum Testparadigm
        Quick
        Slow
        Directional2
        Directional3
        Directional5
        FlexibleLocations
        BMLD
    End Enum
    Public Class TestProcedure
        Public Property AdaptiveType As AdaptiveTypes
        Public Property TestParadigm As Testparadigm
        Public Property LengthReduplications As Integer?
        Public Property RandomizeOrder As Boolean = True

        Public ReadOnly Property TargetStimulusLocations As New SortedList(Of Testparadigm, SoundSourceLocation())

        Public ReadOnly Property MaskerLocations As New SortedList(Of Testparadigm, SoundSourceLocation())

        Public ReadOnly Property BackgroundLocations As New SortedList(Of Testparadigm, SoundSourceLocation())

        Public Sub New(ByVal AdaptiveType As AdaptiveTypes, ByVal TestParadigm As Testparadigm)
            Me.AdaptiveType = AdaptiveType
            Me.TestParadigm = TestParadigm

            'Setting up TargetStimulusLocations
            'TestParadigm Slow
            TargetStimulusLocations.Add(Testparadigm.Slow, {New SoundSourceLocation With {.HorizontalAzimuth = 0}})
            MaskerLocations.Add(Testparadigm.Slow, {
                                New SoundSourceLocation With {.HorizontalAzimuth = -30, .Elevation = 0, .Distance = 1},
                                New SoundSourceLocation With {.HorizontalAzimuth = 30, .Distance = 1}})
            BackgroundLocations.Add(Testparadigm.Slow, {
                                New SoundSourceLocation With {.HorizontalAzimuth = -30, .Distance = 1},
                                New SoundSourceLocation With {.HorizontalAzimuth = 30, .Distance = 1}})

            ''Testparadigm.Quick
            'TargetStimulusLocations.Add(Testparadigm.Quick, {New SoundSourceLocation With {.HorizontalAzimuth = 0, .Distance = 1.45}})
            'MaskerLocations.Add(Testparadigm.Quick, {
            '                    New SoundSourceLocation With {.HorizontalAzimuth = 180, .Distance = 1.45}})
            'BackgroundLocations.Add(Testparadigm.Quick, {
            '                    New SoundSourceLocation With {.HorizontalAzimuth = 0, .Distance = 1.45},
            '                    New SoundSourceLocation With {.HorizontalAzimuth = 180, .Distance = 1.45}})

            TargetStimulusLocations.Add(Testparadigm.Directional2, {
            New SoundSourceLocation With {.HorizontalAzimuth = -15, .Distance = 1},
            New SoundSourceLocation With {.HorizontalAzimuth = 15, .Distance = 1}})
            MaskerLocations.Add(Testparadigm.Directional2, {
                                New SoundSourceLocation With {.HorizontalAzimuth = -30, .Distance = 1},
                                New SoundSourceLocation With {.HorizontalAzimuth = 30, .Distance = 1}})
            BackgroundLocations.Add(Testparadigm.Directional2, {
                                New SoundSourceLocation With {.HorizontalAzimuth = 45, .Distance = 1},
                                New SoundSourceLocation With {.HorizontalAzimuth = 135, .Distance = 1},
                                New SoundSourceLocation With {.HorizontalAzimuth = -135, .Distance = 1},
                                New SoundSourceLocation With {.HorizontalAzimuth = -45, .Distance = 1}})


            TargetStimulusLocations.Add(Testparadigm.Directional3, {
            New SoundSourceLocation With {.HorizontalAzimuth = -30, .Distance = 1},
            New SoundSourceLocation With {.HorizontalAzimuth = 0, .Distance = 1},
            New SoundSourceLocation With {.HorizontalAzimuth = 30, .Distance = 1}})
            MaskerLocations.Add(Testparadigm.Directional3, {
                                New SoundSourceLocation With {.HorizontalAzimuth = -30, .Distance = 1},
                                New SoundSourceLocation With {.HorizontalAzimuth = 30, .Distance = 1}})
            BackgroundLocations.Add(Testparadigm.Directional3, {
                                New SoundSourceLocation With {.HorizontalAzimuth = 45, .Distance = 1},
                                New SoundSourceLocation With {.HorizontalAzimuth = 135, .Distance = 1},
                                New SoundSourceLocation With {.HorizontalAzimuth = -135, .Distance = 1},
                                New SoundSourceLocation With {.HorizontalAzimuth = -45, .Distance = 1}})


            TargetStimulusLocations.Add(Testparadigm.Directional5, {
            New SoundSourceLocation With {.HorizontalAzimuth = -90, .Distance = 1},
            New SoundSourceLocation With {.HorizontalAzimuth = -30, .Distance = 1},
            New SoundSourceLocation With {.HorizontalAzimuth = 0, .Distance = 1},
            New SoundSourceLocation With {.HorizontalAzimuth = 30, .Distance = 1},
            New SoundSourceLocation With {.HorizontalAzimuth = 90, .Distance = 1}})
            MaskerLocations.Add(Testparadigm.Directional5, {
                                New SoundSourceLocation With {.HorizontalAzimuth = -30, .Distance = 1},
                                New SoundSourceLocation With {.HorizontalAzimuth = 30, .Distance = 1}})
            BackgroundLocations.Add(Testparadigm.Directional5, {
                                New SoundSourceLocation With {.HorizontalAzimuth = 45, .Distance = 1},
                                New SoundSourceLocation With {.HorizontalAzimuth = 135, .Distance = 1},
                                New SoundSourceLocation With {.HorizontalAzimuth = -135, .Distance = 1},
                                New SoundSourceLocation With {.HorizontalAzimuth = -45, .Distance = 1}})


        End Sub

        ''' <summary>
        ''' Sets the supplied horizontal azimuths to the TestParadigm key in the TargetStimulusLocations object, overwriting any previous values.
        ''' </summary>
        ''' <param name="Testparadigm"></param>
        ''' <param name="HorizontalAzimuths"></param>
        Public Sub SetTargetStimulusLocations(ByVal TestParadigm As Testparadigm, ByVal HorizontalAzimuths As List(Of Double),
                                              Optional ByVal Elevations As List(Of Double) = Nothing, Optional ByVal Distances As List(Of Double) = Nothing)

            If _TargetStimulusLocations.ContainsKey(TestParadigm) Then
                _TargetStimulusLocations.Remove(TestParadigm)
            End If
            Dim SourceLocationArrayList As New List(Of SoundSourceLocation)

            If Elevations IsNot Nothing Then
                If Elevations.Count <> HorizontalAzimuths.Count Then Throw New ArgumentException("Unequal lengths of parameters!")
            End If

            If Distances IsNot Nothing Then
                If Distances.Count <> HorizontalAzimuths.Count Then Throw New ArgumentException("Unequal lengths of parameters!")
            End If

            For i = 0 To HorizontalAzimuths.Count - 1
                Dim NewLocation = New SoundSourceLocation With {.HorizontalAzimuth = HorizontalAzimuths(i)}
                If Elevations IsNot Nothing Then NewLocation.Elevation = Elevations(i)
                If Distances IsNot Nothing Then NewLocation.Distance = Distances(i)
                SourceLocationArrayList.Add(NewLocation)
            Next

            _TargetStimulusLocations.Add(TestParadigm, SourceLocationArrayList.ToArray)
        End Sub

        ''' <summary>
        ''' Sets the supplied Locations to the TestParadigm key in the TargetStimulusLocations object, overwriting any previous values.
        ''' </summary>
        ''' <param name="Testparadigm"></param>
        ''' <param name="Locations"></param>
        Public Sub SetTargetStimulusLocations(ByVal TestParadigm As Testparadigm, ByRef Locations As List(Of SoundSourceLocation))

            If _TargetStimulusLocations.ContainsKey(TestParadigm) Then
                _TargetStimulusLocations.Remove(TestParadigm)
            End If
            _TargetStimulusLocations.Add(TestParadigm, Locations.ToArray)
        End Sub

        ''' <summary>
        ''' Sets the supplied horizontal azimuths to the TestParadigm key in the MaskerLocations object, overwriting any previous values.
        ''' </summary>
        ''' <param name="Testparadigm"></param>
        ''' <param name="HorizontalAzimuths"></param>
        Public Sub SetMaskerLocations(ByVal TestParadigm As Testparadigm, ByVal HorizontalAzimuths As List(Of Double), Optional ByVal Elevations As List(Of Double) = Nothing, Optional ByVal Distances As List(Of Double) = Nothing)
            If _MaskerLocations.ContainsKey(TestParadigm) Then
                _MaskerLocations.Remove(TestParadigm)
            End If
            Dim SourceLocationArrayList As New List(Of SoundSourceLocation)

            If Elevations IsNot Nothing Then
                If Elevations.Count <> HorizontalAzimuths.Count Then Throw New ArgumentException("Unequal lengths of parameters!")
            End If

            If Distances IsNot Nothing Then
                If Distances.Count <> HorizontalAzimuths.Count Then Throw New ArgumentException("Unequal lengths of parameters!")
            End If

            For i = 0 To HorizontalAzimuths.Count - 1
                Dim NewLocation = New SoundSourceLocation With {.HorizontalAzimuth = HorizontalAzimuths(i)}
                If Elevations IsNot Nothing Then NewLocation.Elevation = Elevations(i)
                If Distances IsNot Nothing Then NewLocation.Distance = Distances(i)
                SourceLocationArrayList.Add(NewLocation)
            Next

            _MaskerLocations.Add(TestParadigm, SourceLocationArrayList.ToArray)
        End Sub

        ''' <summary>
        ''' Sets the supplied Locations to the TestParadigm key in the MaskerLocations object, overwriting any previous values.
        ''' </summary>
        ''' <param name="Testparadigm"></param>
        ''' <param name="Locations"></param>
        Public Sub SetMaskerLocations(ByVal TestParadigm As Testparadigm, ByRef Locations As List(Of SoundSourceLocation))

            If _MaskerLocations.ContainsKey(TestParadigm) Then
                _MaskerLocations.Remove(TestParadigm)
            End If
            _MaskerLocations.Add(TestParadigm, Locations.ToArray)
        End Sub

        ''' <summary>
        ''' Sets the supplied horizontal azimuths to the TestParadigm key in the BackgroundLocations object, overwriting any previous values.
        ''' </summary>
        ''' <param name="Testparadigm"></param>
        ''' <param name="HorizontalAzimuths"></param>
        Public Sub SetBackgroundLocations(ByVal TestParadigm As Testparadigm, ByVal HorizontalAzimuths As List(Of Double), Optional ByVal Elevations As List(Of Double) = Nothing, Optional ByVal Distances As List(Of Double) = Nothing)
            If _BackgroundLocations.ContainsKey(TestParadigm) Then
                _BackgroundLocations.Remove(TestParadigm)
            End If
            Dim SourceLocationArrayList As New List(Of SoundSourceLocation)

            If Elevations IsNot Nothing Then
                If Elevations.Count <> HorizontalAzimuths.Count Then Throw New ArgumentException("Unequal lengths of parameters!")
            End If

            If Distances IsNot Nothing Then
                If Distances.Count <> HorizontalAzimuths.Count Then Throw New ArgumentException("Unequal lengths of parameters!")
            End If

            For i = 0 To HorizontalAzimuths.Count - 1
                Dim NewLocation = New SoundSourceLocation With {.HorizontalAzimuth = HorizontalAzimuths(i)}
                If Elevations IsNot Nothing Then NewLocation.Elevation = Elevations(i)
                If Distances IsNot Nothing Then NewLocation.Distance = Distances(i)
                SourceLocationArrayList.Add(NewLocation)
            Next

            _BackgroundLocations.Add(TestParadigm, SourceLocationArrayList.ToArray)
        End Sub

        ''' <summary>
        ''' Sets the supplied Locations to the TestParadigm key in the BackgroundLocations object, overwriting any previous values.
        ''' </summary>
        ''' <param name="Testparadigm"></param>
        ''' <param name="Locations"></param>
        Public Sub SetBackgroundLocations(ByVal TestParadigm As Testparadigm, ByRef Locations As List(Of SoundSourceLocation))

            If _BackgroundLocations.ContainsKey(TestParadigm) Then
                _BackgroundLocations.Remove(TestParadigm)
            End If
            _BackgroundLocations.Add(TestParadigm, Locations.ToArray)
        End Sub

    End Class



    <Serializable>
    Public Class MeasurementHistory

        Public Property Measurements As New List(Of SipMeasurement)

        Public Sub SaveToFile(Optional ByVal FilePath As String = "", Optional ByVal SaveOnlyLast As Boolean = False)

            'Gets a file path from the user if none is supplied
            If SaveOnlyLast = False Then
                If FilePath = "" Then FilePath = Utils.GetSaveFilePath(,, {".txt"}, "Save stuctured measurement history .txt file as...")
            Else
                If FilePath = "" Then FilePath = Utils.GetSaveFilePath(,, {".txt"}, "Save stuctured measurement (.txt) file as...")
            End If
            If FilePath = "" Then
                MsgBox("No file selected!")
                Exit Sub
            End If

            Dim Output As New List(Of String)

            If SaveOnlyLast = False Then
                For Each Measurement In Measurements
                    Output.Add(Measurement.CreateExportString)
                Next
            Else
                If Measurements.Count > 0 Then Output.Add(Measurements(Measurements.Count - 1).CreateExportString)
            End If

            Utils.SendInfoToLog(String.Join(vbCrLf, Output), IO.Path.GetFileNameWithoutExtension(FilePath), IO.Path.GetDirectoryName(FilePath), True, True)

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="FilePath"></param>
        ''' <returns>Returns Nothing if an error occured during loading.</returns>
        Public Function LoadMeasurements(ByRef ParentTestSpecification As SpeechMaterialSpecification, ByRef Randomizer As Random, Optional ByVal FilePath As String = "", Optional ByVal ParticipantID As String = "") As MeasurementHistory

            Dim Output As New MeasurementHistory

            'Gets a file path from the user if none is supplied
            If FilePath = "" Then FilePath = Utils.GetOpenFilePath(,, {".txt"}, "Please open a stuctured measurement history .txt file.")
            If FilePath = "" Then
                MsgBox("No file selected!")
                Return Nothing
            End If

            'Parses the input file
            Dim InputLines() As String = System.IO.File.ReadAllLines(FilePath, Text.Encoding.UTF8)

            'Splits InputLines into separate measurements based on the SipMeasurement.NewMeasurementMarker string
            Dim ImportedMeasurementList As New List(Of String())
            Dim CurrentlyImportedMeasurement As List(Of String) = Nothing
            For Each Line In InputLines
                If Line.Trim.StartsWith(SipMeasurement.NewMeasurementMarker) Then
                    If CurrentlyImportedMeasurement Is Nothing Then
                        CurrentlyImportedMeasurement = New List(Of String)
                    Else
                        ImportedMeasurementList.Add(CurrentlyImportedMeasurement.ToArray)
                        CurrentlyImportedMeasurement.Clear()
                    End If

                    'Skips adding the NewMeasurementMarker
                    Continue For
                End If
                'Adds the current line to the current measurement
                CurrentlyImportedMeasurement.Add(Line)
            Next

            'Also adding the last measurement
            If CurrentlyImportedMeasurement IsNot Nothing Then ImportedMeasurementList.Add(CurrentlyImportedMeasurement.ToArray)


            'Parsing each measurement
            For Each MeasurementStringArray In ImportedMeasurementList
                Dim LoadedMeasurement = SipMeasurement.ParseImportLines(MeasurementStringArray, Randomizer, ParentTestSpecification, ParticipantID)
                If LoadedMeasurement Is Nothing Then Return Nothing
                Output.Measurements.Add(LoadedMeasurement)
            Next

            'Returns the loaded measurements
            Return Output

        End Function

    End Class


End Namespace