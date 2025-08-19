
'TODOs
'Should we neutralize TPD per testord instead of per recording in the SiP-test model?
'We have To fix the randomizers In the SIPtest. There are randomizers at several levels!

Imports STFN.Core
Imports STFN.Core.Utils
Imports STFN.Core.Audio
Imports STFN.Core.Audio.SoundScene
Imports STFN.Core.Audio.Formats
Imports System.Reflection
Imports System.Runtime.Serialization

Namespace SipTest



    Public Class SipMeasurement
        Inherits STFN.Core.SipTest.SipMeasurement


        Public Sub New(ByRef ParticipantID As String, ByRef ParentTestSpecification As SpeechMaterialSpecification, Optional AdaptiveType As SiPTestProcedure.AdaptiveTypes = SiPTestProcedure.AdaptiveTypes.Fixed,
                       Optional ByVal TestParadigm As SiPTestProcedure.SiPTestparadigm = SiPTestProcedure.SiPTestparadigm.Slow, Optional RandomSeed As Integer? = Nothing)

            MyBase.New(ParticipantID, ParentTestSpecification, AdaptiveType, TestParadigm, RandomSeed)

        End Sub



#Region "Preparation"


        Public Sub PlanTestTrials(ByRef AvailableMediaSet As MediaSetLibrary, ByVal PresetName As String, ByVal MediaSetName As String, ByVal SoundPropagationType As SoundPropagationTypes, Optional ByVal RandomSeed As Integer? = Nothing)

            Dim Preset = ParentTestSpecification.SpeechMaterial.Presets.GetPreset(PresetName).Members

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
                Case SiPTestProcedure.AdaptiveTypes.Fixed

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

                        'PlannedTrials = RandomList
                        PlannedTrials.Clear()
                        For Each Item In RandomList
                            PlannedTrials.Add(Item)
                        Next

                    End If



                Case SiPTestProcedure.AdaptiveTypes.SimpleUpDown

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





        ''' <summary>
        ''' Sets all levels in all trials. (Levels should be set prior to mixing the sounds, or prior to success probability estimation.)
        ''' </summary>
        Public Sub SetLevels(ByVal ReferenceLevel As Double, ByVal PNR As Double)

            Select Case TestProcedure.AdaptiveType
                Case SiPTestProcedure.AdaptiveTypes.Fixed

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


        ''' <summary>
        ''' Moves the referenced test trial from the PlannedTrials to ObservedTrials objects, both in the parent TestUnit and in the parent SipMeasurement. This way it will not be presented again.
        ''' </summary>
        ''' <param name="TestTrial"></param>
        ''' <param name="RemoveSounds">If True, removes the Sound in the referenced trial.</param>
        Public Shadows Sub MoveTrialToHistory(ByRef TestTrial As SipTrial, Optional ByVal RemoveSounds As Boolean = True)

            'Calling the base method
            MyBase.MoveTrialToHistory(TestTrial, RemoveSounds)

            'And then the RemoveSounds of the STFN.SiPTrial, which may contain more sounds
            If RemoveSounds = True Then
                TestTrial.RemoveSounds()
            End If

        End Sub

#End Region


#Region "RunTest"







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
                    Case SiPTestProcedure.SiPTestparadigm.Directional2, SiPTestProcedure.SiPTestparadigm.Directional3, SiPTestProcedure.SiPTestparadigm.Directional5
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
                    Case SiPTestProcedure.SiPTestparadigm.Directional2, SiPTestProcedure.SiPTestparadigm.Directional3, SiPTestProcedure.SiPTestparadigm.Directional5
                        Output.TestWords.Add(TestWordPrefix & PlannedTrials(i).SpeechMaterialComponent.PrimaryStringRepresentation & ", " & PlannedTrials(i).TargetStimulusLocations(0).ActualLocation.HorizontalAzimuth & TestWordSuffix) 'It is also possible to use a custom variable here, such as: ...SpeechMaterial.GetCategoricalVariableValue("Spelling")) 
                    Case Else
                        Output.TestWords.Add(TestWordPrefix & PlannedTrials(i).SpeechMaterialComponent.PrimaryStringRepresentation & TestWordSuffix) 'It is also possible to use a custom variable here, such as: ...SpeechMaterial.GetCategoricalVariableValue("Spelling")) 
                End Select
                Output.Responses.Add("")
                Output.ResponseType.Add(SipTrial.PossibleResults.Missing)
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
            Public ResponseType As New List(Of SipTrial.PossibleResults)
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
                UnadjustedEstimates.Add(DirectCast(ObservedTrials(n), SipTrial).EstimatedSuccessProbability(True))

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
            Dim AdjustedEstimates = CriticalDifferences.AdjustSuccessProbabilities(UnadjustedEstimates.ToArray, LocalAverageScore, Floors.ToArray)
            For n = 0 To Me.ObservedTrials.Count - 1
                DirectCast(ObservedTrials(n), SipTrial).AdjustedSuccessProbability = AdjustedEstimates(n)
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

                If Me.ObservedTrials(n).Result = SipTrial.PossibleResults.Correct Then
                    Correct += 1

                ElseIf Me.ObservedTrials(n).Result = SipTrial.PossibleResults.Missing Then

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
                OutputList.Add(DirectCast(ObservedTrials(n), SipTrial).AdjustedSuccessProbability)
            Next
            Return OutputList.ToArray
        End Function



        Public Function CreateExportString(Optional ByVal SkipExportOfSoundFiles As Boolean = True) As String

            Dim OutputLines As New List(Of String)

            OutputLines.Add(SiPTestMeasurementHistory.NewMeasurementMarker)

            OutputLines.Add(SipTrial.CreateExportHeadings())

            For t = 0 To Me.ObservedTrials.Count - 1

                Dim Trial As SipTrial = Me.ObservedTrials(t)

                Dim TrialExportString = Trial.CreateExportString(SkipExportOfSoundFiles)

                OutputLines.Add(TrialExportString)

            Next

            Return String.Join(vbCrLf, OutputLines)

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
                Dim Result As SipTrial.PossibleResults = [Enum].Parse(GetType(SipTrial.PossibleResults), LineColumns(c))
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

            'Referencing the temporarily contained values in LoadedTestUnits in Output.TestUnits 
            'Output.TestUnits = LoadedTestUnits.Values.ToList
            Output.TestUnits.Clear()
            For Each Item In LoadedTestUnits.Values
                Output.TestUnits.Add(Item)
            Next

            Return Output

        End Function

        Public Sub ExportMeasurement(Optional ByVal FilePath As String = "")

            'Gets a file path from the user if none is supplied
            If FilePath = "" Then FilePath = Utils.GeneralIO.GetSaveFilePath(,, {".txt"}, "Save stuctured measurement history .txt file as...")
            If FilePath = "" Then
                MsgBox("No file selected!")
                Exit Sub
            End If

            Dim ExportString = CreateExportString()

            Logging.SendInfoToLog(ExportString, IO.Path.GetFileNameWithoutExtension(FilePath), IO.Path.GetDirectoryName(FilePath), True, True)

        End Sub

        Public Shared Function ImportSummary(ByRef ParentTestSpecification As SpeechMaterialSpecification, ByRef Randomizer As Random, Optional ByVal FilePath As String = "", Optional ByVal Participant As String = "") As SipMeasurement

            'Gets a file path from the user if none is supplied
            If FilePath = "" Then FilePath = Utils.GeneralIO.GetOpenFilePath(,, {".txt"}, "Please open a stuctured measurement history .txt file.")
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




    End Class






End Namespace