
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


        Public Property ParentTestSpecification As TestSpecification

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
        Public Property TestProcedure As New TestProcedure(AdaptiveTypes.Fixed)


        Public Property SelectedAudiogramData As AudiogramData = Nothing
        Public Property HearingAidGain As HearingAidGainData = Nothing


        Friend Randomizer As Random

        Public Sub New(ByRef ParticipantID As String, ByRef ParentTestSpecification As TestSpecification, Optional RandomSeed As Integer? = Nothing)

            If RandomSeed.HasValue = True Then
                Randomizer = New Random(RandomSeed)
            Else
                Randomizer = New Random
            End If

            Me.ParticipantID = ParticipantID
            Me.ParentTestSpecification = ParentTestSpecification

        End Sub

#Region "Preparation"


        Public Sub PlanTestTrials(ByRef AvailableMediaSet As MediaSetLibrary, ByVal PresetName As String, ByVal MediaSetName As String, Optional ByVal RandomSeed As Integer? = Nothing)

            Dim Preset = ParentTestSpecification.SpeechMaterial.Presets(PresetName)

            PlanTestTrials(AvailableMediaSet, Preset, MediaSetName, RandomSeed)

        End Sub


        Public Sub PlanTestTrials(ByRef AvailableMediaSet As MediaSetLibrary, ByVal Preset As List(Of SpeechMaterialComponent), ByVal MediaSetName As String, Optional ByVal RandomSeed As Integer? = Nothing)

            ClearTrials()

            'MediaSetName ' TODO: should we use the name or the mediaset in the GUI/Measurment?
            'TODO: If media set in not selected we could ranomize between the available ones...

            Select Case TestProcedure.AdaptiveType
                Case AdaptiveTypes.Fixed

                    If RandomSeed.HasValue Then Randomizer = New Random(RandomSeed)

                    For r = 1 To TestProcedure.LengthReduplications

                        For Each PresetComponent In Preset

                            Dim NewTestUnit = New SiPTestUnit(Me)

                            Dim TestWords = PresetComponent.GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.Sentence)
                            NewTestUnit.SpeechMaterialComponents.AddRange(TestWords)

                            If MediaSetName <> "" Then
                                'Adding from the selected media set
                                NewTestUnit.PlanTrials(AvailableMediaSet.GetMediaSet(MediaSetName))
                                TestUnits.Add(NewTestUnit)
                            Else
                                'Adding from random media sets
                                Dim RandomIndex = Randomizer.Next(0, AvailableMediaSet.Count)
                                NewTestUnit.PlanTrials(AvailableMediaSet(RandomIndex))
                                TestUnits.Add(NewTestUnit)
                            End If

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

                    For r = 1 To TestProcedure.LengthReduplications

                        For Each PresetComponent In Preset

                            Dim NewTestUnit = New SiPTestUnit(Me)

                            Dim TestWords = PresetComponent.GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.Sentence)
                            NewTestUnit.SpeechMaterialComponents.AddRange(TestWords)

                            If MediaSetName <> "" Then
                                'Adding from the selected media set
                                NewTestUnit.PlanTrials(AvailableMediaSet.GetMediaSet(MediaSetName))
                                TestUnits.Add(NewTestUnit)
                            Else
                                'Adding from random media sets
                                Dim RandomIndex = Randomizer.Next(0, AvailableMediaSet.Count)
                                NewTestUnit.PlanTrials(AvailableMediaSet(RandomIndex))
                                TestUnits.Add(NewTestUnit)
                            End If

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



        Public Sub ClearTrials()
            TestUnits.Clear()
            PlannedTrials.Clear()
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

                Dim NextTestTrial As SipTrial = PlannedTrials(0)

                Dim NextTestUnit = NextTestTrial.ParentTestUnit

                'Adding the trial to the history
                ObservedTrials.Add(NextTestTrial)
                NextTestUnit.ObservedTrials.Add(NextTestTrial)

                'Removes the next trial from PlannedTrials
                PlannedTrials.Remove(NextTestTrial)
                NextTestUnit.PlannedTrials.Add(NextTestTrial)


                'Returns the next planed trial
                Return NextTestTrial
            End If


            'Dim NextTestUnit = GetNextTestUnit(rnd)

            ''Returns nothing if there are no more test units to present
            'If NextTestUnit Is Nothing Then Return Nothing

            ''Gets the next test trial
            'Dim NextTestTrial = NextTestUnit.GetNextTrial(rnd)

            ''Setting levels

            ''Adding the trial to the history
            'ObservedTrials.Add(NextTestTrial)
            'NextTestUnit.ObservedTrials.Add(NextTestTrial)

            ''Removes the next trial from PlannedTrials
            'PlannedTrials.Remove(NextTestTrial)
            'NextTestUnit.PlannedTrials.Add(NextTestTrial)

            'Return NextTestTrial

        End Function

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

        Public Function GetGuiTableData() As GuiTableData

            Dim Output As New GuiTableData


            'Adding already tested trials
            For i = 0 To ObservedTrials.Count - 1
                Output.TestWords.Add(ObservedTrials(i).SpeechMaterialComponent.PrimaryStringRepresentation)
                Output.Responses.Add(ObservedTrials(i).Response)
                Output.ResponseType.Add(ObservedTrials(i).Result)
            Next

            'Adding trials yet to be tested
            For i = 0 To PlannedTrials.Count - 1
                Output.TestWords.Add(PlannedTrials(i).SpeechMaterialComponent.PrimaryStringRepresentation)
                Output.Responses.Add("")
                Output.ResponseType.Add(PossibleResults.Missing)
            Next

            Dim LastPresentedTrialIndex As Integer = ObservedTrials.Count - 1
            Output.SelectionRow = Math.Max(0, LastPresentedTrialIndex)
                Output.FirstRowToDisplayInScrollmode = Math.Max(0, LastPresentedTrialIndex - 7)

            'Overriding values if no rows exist
            If PlannedTrials.Count = 0 And ObservedTrials.Count = 0 Then
                Output.SelectionRow = Nothing
                Output.FirstRowToDisplayInScrollmode = Nothing
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

        Public Function CalculateEstimatedPsychometricFunction(ByVal ReferenceLevel As Double, Optional ByVal PNRs As List(Of Double) = Nothing) As SortedList(Of Double, Tuple(Of Double, Double, Double))

            If PNRs Is Nothing Then
                PNRs = New List(Of Double) From {-15, -12, -10, -8, -6, -4, -2, 0, 2, 4, 6, 8, 10, 12, 15}
            End If

            'Pnr, Estimate, lower critical boundary, upper critical boundary
            Dim Output As New SortedList(Of Double, Tuple(Of Double, Double, Double))

            For Each pnr In PNRs

                Me.SetLevels(ReferenceLevel, pnr)

                Dim EstimatedScore = Me.CalculateEstimatedMeanScore

                If EstimatedScore IsNot Nothing Then
                    Output.Add(pnr, EstimatedScore)
                End If

            Next

            Return Output

        End Function


        Public Function CalculateEstimatedMeanScore() As Tuple(Of Double, Double, Double)

            Dim TrialSuccessProbabilityList As New List(Of Double)

            For Each TestUnit In Me.TestUnits
                For Each PlannedTestTrial In TestUnit.PlannedTrials
                    TrialSuccessProbabilityList.Add(PlannedTestTrial.EstimatedSuccessProbability(True))
                Next
            Next

            Dim CriticalDifferenceLimits = CriticalDifferences.GetCriticalDifferenceLimits_PBAC(TrialSuccessProbabilityList.ToArray, TrialSuccessProbabilityList.ToArray)

            If TrialSuccessProbabilityList.Count > 0 Then
                Return New Tuple(Of Double, Double, Double)(TrialSuccessProbabilityList.Average(), CriticalDifferenceLimits.Item1, CriticalDifferenceLimits.Item2)
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

            For i = 0 To Me.ObservedTrials.Count - 1
                'Get the unadjusted trial success probability estimates
                UnadjustedEstimates.Add(Me.ObservedTrials(i).EstimatedSuccessProbability(True))

                'Gets the number of response alternatives
                Dim ResponseAlternativeCount As Integer = 0
                Me.ObservedTrials(i).SpeechMaterialComponent.IsContrastingComponent(, ResponseAlternativeCount)
                Me.ObservedTrials(i).ResponseAlternativeCount = ResponseAlternativeCount

                'Calulates the floors of the psychometric functions (of each trial) based on the number of response alternatives
                If ResponseAlternativeCount > 0 Then
                    Floors(i) = 1 / ResponseAlternativeCount
                Else
                    Floors(i) = 0
                End If
            Next

            'Gets the target average score to adjust the unadjusted estimates to
            Dim LocalAverageScore = GetAverageObservedScore()

            'Creates adjusted estimates
            Dim AdjustedEstimates = SpeechTestFramework.CriticalDifferences.AdjustSuccessProbabilities(UnadjustedEstimates.ToArray, LocalAverageScore, Floors)
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

        Public Function CreateExportString() As String

            Dim OutputLines As New List(Of String)

            OutputLines.Add(NewMeasurementMarker)

            Dim Headings As New List(Of String)
            Headings.Add("ParticipantID")
            Headings.Add("MeasurementDateTime")
            Headings.Add("Description")
            Headings.Add("TestUnitIndex")
            Headings.Add("SpeechMaterialComponentID")
            Headings.Add("MediaSetName")
            Headings.Add("PresentationOrder")
            Headings.Add("Reference_SPL")
            Headings.Add("PNR")
            Headings.Add("EstimatedSuccessProbability")
            Headings.Add("AdjustedSuccessProbability")
            Headings.Add("Response")
            Headings.Add("Result")
            Headings.Add("ResponseTime")

            'Plus write-only stuff
            Headings.Add("ResponseAlternativeCount")
            Headings.Add("PhonemeDiscriminabilityLevel")

            OutputLines.Add(String.Join(vbTab, Headings))

            For t = 0 To Me.ObservedTrials.Count - 1

                Dim Trial As SipTrial = Me.ObservedTrials(t)

                Dim TrialList As New List(Of String)
                TrialList.Add(Me.ParticipantID)
                TrialList.Add(Me.MeasurementDateTime.ToString(System.Globalization.CultureInfo.InvariantCulture))
                TrialList.Add(Me.Description)
                TrialList.Add(GetParentTestUnitIndex(Trial))
                TrialList.Add(Trial.SpeechMaterialComponent.Id)
                TrialList.Add(Trial.MediaSet.MediaSetName)
                TrialList.Add(t)
                TrialList.Add(Trial.Reference_SPL)
                TrialList.Add(Trial.PNR)
                TrialList.Add(Trial.EstimatedSuccessProbability(True))
                TrialList.Add(Trial.AdjustedSuccessProbability)
                TrialList.Add(Trial.Response)
                TrialList.Add(Trial.Result.ToString)
                TrialList.Add(Trial.ResponseTime.ToString(System.Globalization.CultureInfo.InvariantCulture))

                'Plus write-only stuff
                TrialList.Add(Trial.ResponseAlternativeCount)
                TrialList.Add(Trial.PhonemeDiscriminabilityLevel)
                'TODO: ... add more

                OutputLines.Add(String.Join(vbTab, TrialList))

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

        Public Shared Function ParseImportLines(ByVal ImportLines() As String, ByRef ParentTestSpecification As TestSpecification, Optional ByVal ParticipantID As String = "") As SipMeasurement

            Dim Output As SipMeasurement = Nothing

            Dim LoadedTestUnits As New SortedList(Of Integer, SiPTestUnit)

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

                If LineColumns.Length < 14 Then
                    MsgBox("Error when loading line " & Line & vbCrLf & vbCrLf & "Not enough columns!", MsgBoxStyle.Exclamation, "Missing columns in imported measurement file!")
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
                Dim Response As String = LineColumns(c)
                c += 1
                Dim Result As SipTest.PossibleResults = [Enum].Parse(GetType(SipTest.PossibleResults), LineColumns(c))
                c += 1
                Dim ResponseTime As Integer = LineColumns(c)
                c += 1

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

                'Getting the SpeechMaterialComponent, media set and (re-)creates the test trial
                Dim SpeechMaterialComponent = ParentTestSpecification.SpeechMaterial.GetComponentById(SpeechMaterialComponentID)
                Dim MediaSet = ParentTestSpecification.MediaSets.GetMediaSet(MediaSetName)
                Dim NewTestTrial As New SipTrial(LoadedTestUnits(ParentTestUnitIndex), SpeechMaterialComponent, MediaSet)

                'Stores the remaining test trial data
                NewTestTrial.AdjustedSuccessProbability = AdjustedSuccessProbability

                'NewTestTrial.PresentationOrder = PresentationOrder 'This is not stored as the export/import should always be ordered in the presentation order, as they are read from and stored into the ObservedTrials object!
                NewTestTrial.Reference_SPL = Reference_SPL
                NewTestTrial.PNR = PNR
                NewTestTrial.OverideEstimatedSuccessProbabilityValue(EstimatedSuccessProbability)
                NewTestTrial.AdjustedSuccessProbability = AdjustedSuccessProbability
                NewTestTrial.Response = Response
                NewTestTrial.Result = Result
                NewTestTrial.ResponseTime = ResponseTime

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

        Public Shared Function ImportSummary(ByRef ParentTestSpecification As TestSpecification, Optional ByVal FilePath As String = "", Optional ByVal Participant As String = "") As SipMeasurement

            'Gets a file path from the user if none is supplied
            If FilePath = "" Then FilePath = Utils.GetOpenFilePath(,, {".txt"}, "Please open a stuctured measurement history .txt file.")
            If FilePath = "" Then
                MsgBox("No file selected!")
                Return Nothing
            End If

            'Parses the input file
            Dim InputLines() As String = System.IO.File.ReadAllLines(FilePath, Text.Encoding.UTF8)

            'Imports a summary
            Return ParseImportLines(InputLines, ParentTestSpecification, Participant)

        End Function


#End Region


    End Class



    Public Class SiPTestUnit

        Public Property ParentMeasurement As SipMeasurement

        Public Property SpeechMaterialComponents As New List(Of SpeechMaterialComponent)

        Public Property PlannedTrials As New List(Of SipTrial)

        Public Property ObservedTrials As New List(Of SipTrial)

        Public Property AdaptiveValue As Double

        Public Sub New(ByRef ParentMeasurement As SipMeasurement)
            Me.ParentMeasurement = ParentMeasurement
        End Sub

        Public Sub PlanTrials(ByRef MediaSet As MediaSet)

            PlannedTrials.Clear()

            Select Case ParentMeasurement.TestProcedure.AdaptiveType
                Case AdaptiveTypes.Fixed

                    'For n = 1 To ParentMeasurement.TestProcedure.LengthReduplications ' Should this be done here, or at a higher level?
                    For c = 0 To SpeechMaterialComponents.Count - 1
                        Dim NewTrial As New SipTrial(Me, SpeechMaterialComponents(c), MediaSet)
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

        ''' <summary>
        ''' Holds a reference to the test unit to which the current test trial belongs
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ParentTestUnit As SiPTestUnit

        ''' <summary>
        ''' Holds a reference to the SpeechMaterialComponent presented in the trial.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property SpeechMaterialComponent As SpeechMaterialComponent

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
        ''' The response time in milliseconds
        ''' </summary>
        ''' <returns></returns>
        Public Property ResponseTime As Integer

        Public Property ResponseMoment As DateTime

        Public Property AdjustedSuccessProbability As Double
        Public Property ResponseAlternativeCount As Integer

        'Sound Levels


        Public Sub New(ByRef ParentTestUnit As SiPTestUnit,
                       ByRef SpeechMaterialComponent As SpeechMaterialComponent,
                       ByRef MediaSet As MediaSet)

            Me.ParentTestUnit = ParentTestUnit
            Me.SpeechMaterialComponent = SpeechMaterialComponent
            Me.MediaSet = MediaSet

            'Setting some levels
            Dim Fs2Spl As Double = Audio.PortAudioVB.DuplexMixer.Simulated_dBFS_dBSPL_Difference

            ReferenceSpeechMaterialLevel_SPL = Fs2Spl + SpeechMaterialComponent.GetAncestorAtLevel(SpeechMaterialComponent.LinguisticLevels.ListCollection).GetNumericMediaSetVariableValue(MediaSet, "Lc")
            ReferenceTestWordLevel_SPL = Fs2Spl + SpeechMaterialComponent.GetNumericMediaSetVariableValue(MediaSet, "Lc") 'TestStimulus.TestWord_ReferenceSPL
            ReferenceContrastingPhonemesLevel_SPL = Fs2Spl + SpeechMaterialComponent.GetAncestorAtLevel(SpeechMaterialComponent.LinguisticLevels.List).GetNumericMediaSetVariableValue(MediaSet, "RLxs")


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
        Public Sub SetLevels(ByVal ReferenceLevel As Double, ByVal PNR As Double)

            'Setting UpdatePdlOnNextCall to True to signal recalculation of PDL when needed next time (TODO: this could be skipped if the ReferenceLevel and PNR are not changed since last call)
            UpdatePdlOnNextCall = True

            Me.Reference_SPL = ReferenceLevel
            Me.PNR = PNR

            'Calculating the difference between the standard ReferenceSpeechMaterialLevel_SPL (68.34 dB SPL) reference level and the one currently used
            Dim RefLevelDifference As Double = ReferenceLevel - ReferenceSpeechMaterialLevel_SPL

            '0. Gettings some levels
            Dim ContrastingPhonemesLevel_SPL = ReferenceContrastingPhonemesLevel_SPL + RefLevelDifference

            '1. Setting the noise level
            Dim SNR_Type As String = "PNR"
            If SNR_Type = "PNR" Then
                'In this procedure, CurrentSNR represents the sound level difference between the average max level of the contrasting test phonemes, and the masker sound
                'Setting the test word masker to Lcp

                'Setting TargetMasking_SPL to ContrastingPhonemesLevel_SPL
                _TargetMasking_SPL = ContrastingPhonemesLevel_SPL

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

                'Decreasing all levels set by Difference, to retain the desired test SNR
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

        End Sub


        Public Function GetCurrentSpeechGain() As Double
            Dim SpeechGain As Double = TestWordLevel - ReferenceTestWordLevel_SPL ' SpeechMaterialComponent.GetNumericMediaSetVariableValue(MediaSet, "Lc") 'TestStimulus.TestWord_ReferenceSPL
            Return SpeechGain
        End Function

        Public Function GetCurrentMaskerGain() As Double
            Dim CurrentMaskerGain = TargetMasking_SPL - 70 'TODO: In the SiP-test maskers sound files the level is set to -30 dB FS across the whole sounds. A more detailed sound level data could be used instead!
            Return CurrentMaskerGain
        End Function



        Private _EstimatedSuccessProbability As Double? = Nothing

        Public ReadOnly Property EstimatedSuccessProbability(ByVal ReCalculate As Boolean) As Double
            Get
                If _EstimatedSuccessProbability.HasValue = False Or ReCalculate = True Then
                    UpdateEstimatedSuccessProbability()
                End If
                Return _EstimatedSuccessProbability
            End Get
        End Property

        Public Sub OverideEstimatedSuccessProbabilityValue(ByVal Newvalue)
            _EstimatedSuccessProbability = _EstimatedSuccessProbability
        End Sub

        Public Sub UpdateEstimatedSuccessProbability()

            'Select Case Me.ParentTestUnit.ParentMeasurement.Prediction.Models.SelectedModel.GetSwedishSipTestA

            'Getting predictors
            Dim PDL As Double = Me.PhonemeDiscriminabilityLevel
            Dim TPD As Double = Me.SpeechMaterialComponent.GetNumericMediaSetVariableValue(MediaSet, "Tc")
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

            'Stores the result in PredictedSuccessProbability
            _EstimatedSuccessProbability = p

        End Sub


        Private _PhonemeDiscriminabilityLevel As Double
        Private UpdatePdlOnNextCall As Boolean = True

        Public ReadOnly Property PhonemeDiscriminabilityLevel(Optional ByVal SpeechSpectrumLevelsVariableNamePrefix As String = "SLs",
                                                              Optional ByVal MaskerSpectrumLevelsVariableNamePrefix As String = "SLm") As Double
            Get
                If UpdatePdlOnNextCall = True Then

                    'Using thresholds and gain data from the side with the best aided thresholds (selecting side separately for each critical band)
                    Dim Thresholds(Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies.Length - 1) As Double
                    Dim Gain(Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies.Length - 1) As Double

                    For i = 0 To Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies.Length - 1
                        'TODO: should we allow for the lack of gain data here, or should we always use a gain of zero when no hearing aid is used?
                        Dim AidedThreshold_Left As Double = Me.ParentTestUnit.ParentMeasurement.SelectedAudiogramData.Cb_Left_AC(i) - Me.ParentTestUnit.ParentMeasurement.HearingAidGain.LeftSideGain(i)
                        Dim AidedThreshold_Right As Double = Me.ParentTestUnit.ParentMeasurement.SelectedAudiogramData.Cb_Right_AC(i) - Me.ParentTestUnit.ParentMeasurement.HearingAidGain.RightSideGain(i)

                        If AidedThreshold_Left < AidedThreshold_Right Then
                            Thresholds(i) = Me.ParentTestUnit.ParentMeasurement.SelectedAudiogramData.Cb_Left_AC(i)
                            Gain(i) = Me.ParentTestUnit.ParentMeasurement.HearingAidGain.LeftSideGain(i)
                        Else
                            Thresholds(i) = Me.ParentTestUnit.ParentMeasurement.SelectedAudiogramData.Cb_Right_AC(i)
                            Gain(i) = Me.ParentTestUnit.ParentMeasurement.HearingAidGain.RightSideGain(i)
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

                    'Getting the current gain, compared to the reference test-word and masker levels
                    Dim CurrentSpeechGain As Double = GetCurrentSpeechGain()
                    Dim CurrentMaskerGain As Double = GetCurrentMaskerGain()

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

                    'Calculating PDL
                    _PhonemeDiscriminabilityLevel = PDL.CalculatePDL(SDRt, SDRcs)

                    UpdatePdlOnNextCall = False
                End If
                Return _PhonemeDiscriminabilityLevel
            End Get
        End Property


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

    Public Class TestProcedure

        Public Property AdaptiveType As AdaptiveTypes

        Public Property LengthReduplications As Integer?

        Public Property RandomizeOrder As Boolean = True

        Public Sub New(ByVal AdaptiveType As AdaptiveTypes)
            Me.AdaptiveType = AdaptiveType
        End Sub

    End Class



    <Serializable>
    Public Class MeasurementHistory

        Public Property Measurements As New List(Of SipMeasurement)

        Public Sub SaveToFile(Optional ByVal FilePath As String = "")

            'Gets a file path from the user if none is supplied
            If FilePath = "" Then FilePath = Utils.GetSaveFilePath(,, {".txt"}, "Save stuctured measurement history .txt file as...")
            If FilePath = "" Then
                MsgBox("No file selected!")
                Exit Sub
            End If

            Dim Output As New List(Of String)

            For Each Measurement In Measurements
                Output.Add(Measurement.CreateExportString)
            Next

            Utils.SendInfoToLog(String.Join(vbCrLf, Output), IO.Path.GetFileNameWithoutExtension(FilePath), IO.Path.GetDirectoryName(FilePath), True, True)

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="FilePath"></param>
        ''' <returns>Returns Nothing if an error occured during loading.</returns>
        Public Function LoadMeasurements(ByRef ParentTestSpecification As TestSpecification, Optional ByVal FilePath As String = "", Optional ByVal ParticipantID As String = "") As MeasurementHistory

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
                Dim LoadedMeasurement = SipMeasurement.ParseImportLines(MeasurementStringArray, ParentTestSpecification, ParticipantID)
                If LoadedMeasurement Is Nothing Then Return Nothing
                Output.Measurements.Add(LoadedMeasurement)
            Next

            'Returns the loaded measurements
            Return Output

        End Function

    End Class


End Namespace