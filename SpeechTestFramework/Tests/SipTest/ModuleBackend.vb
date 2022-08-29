
Namespace SipTest

    Public Class ModuleBackend

        'Friend CurrentUser As User
        Friend CurrentPatient As Patient
        Friend AvailableAudiograms As New List(Of AudiogramData)
        Friend AvailablePNRs As New List(Of Double) From {-15, -12, -10, -8, -6, -4, -2, 0, 2, 4, 6, 8, 10, 12, 15}
        Friend AvailableMediaSets As MediaSetLibrary
        Friend CurrentSipTestMeasurement As TestSession
        'Friend SoundPlayer As Audio.PaOverlappingSoundPlayerC

        Friend CompleteSpeechMaterial As SpeechMaterialComponent

        'Friend BlueToothConnection As BlueToothConnection
        Friend WithEvents SipGui As ISipGui

        Public Language As Languages = Languages.Swedish
        Private NumberSpeakerChannels As Integer = 3

        'Public Settings As SiPSettings

        Public Enum Languages
            Swedish
            English
        End Enum

        Public Sub New(ByRef SipGui As ISipGui)

            'Referencing the GUI
            Me.SipGui = SipGui

        End Sub

        ''' <summary>
        ''' Initializing all ModuleBackend components
        ''' </summary>
        Public Sub InitializeComponents() Handles SipGui.InitiateBackend

            'Initializing all components

            'Creating a sound format for the output sound (Same sample rate, bit depth and encoding as used in the SiP-test sound files)
            Dim PlayBackWaveFormat = New Audio.Formats.WaveFormat(48000, 32, NumberSpeakerChannels,, Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints)

            Dim CalibrationData = LookForCalibrationData()

            OstfSettings.LoadAvailableTestSpecifications()

            Dim SelectedTest As TestSpecification = Nothing
            For Each ts In OstfSettings.AvailableTests
                If ts.Name = "Swedish SiP-test" Then
                    SelectedTest = ts
                    Exit For
                End If
            Next

            If SelectedTest IsNot Nothing Then
                CompleteSpeechMaterial = SpeechMaterialComponent.LoadSpeechMaterial(SelectedTest.GetSpeechMaterialFilePath, SelectedTest.GetTestRootPath)
                CompleteSpeechMaterial.ParentTestSpecification = SelectedTest
                SelectedTest.SpeechMaterial = CompleteSpeechMaterial
            Else
                MsgBox("SiP-test not found. Exiting!")
                Exit Sub
            End If

            'Loading media sets
            CompleteSpeechMaterial.ParentTestSpecification.LoadAvailableMediaSetSpecifications()
            AvailableMediaSets = CompleteSpeechMaterial.ParentTestSpecification.MediaSets

            'Me.TestStimulusLibrary = New TestStimulusLibrary(PlayBackWaveFormat, CalibrationData)

            'TODO: should probably send a message to the Gui if something went wrong!

        End Sub

        Private Function LookForCalibrationData() As SortedList(Of Integer, Double)

            MsgBox("Here we should look for calibration data and offer some options...")

            Dim CalibrationData As SortedList(Of Integer, Double) = Nothing

            Return CalibrationData

        End Function

        'Public Sub DebugFunction(FunctionIndex As Integer)


        '    Dim TestIndex As Integer = 0

        '    Select Case TestIndex
        '        Case 0

        '            StartTest()

        '        Case 1

        '            Dim Result = CreateNewSipTestMeasurement()

        '            CurrentSipTestMeasurement.SetLevels(70, 5)

        '            If Result.Item1 = False Then
        '                SipGui.ShowMessageBox("Testet kunde inte skapas!")
        '            End If

        '    End Select

        'End Sub


#Region "ImportExport"


        Public Sub SearchPatient(SocialSecurityNumber As String) Handles SipGui.SearchPatient

            'Then look up a patient in a file or database, create a patient from it and reference that patient into the CurrentPatient property

            'Checking SocialSecurityNumber
            If SocialSecurityNumber = "" Or SocialSecurityNumber.Length <> 12 Then
                SipGui.ShowMessageBox("Du har fyllt i ett ogiltigt personnummer! Försök igen", "Ogiltigt personnummer")
                Exit Sub
            End If

            'TODO: namsn should be also be looked up
            Dim FirstName As String = "Erik"
            Dim LastName As String = "Witte"

            'Locking ID controls in the Gui
            SipGui.LockPatientDetails(SocialSecurityNumber, FirstName, LastName)

            'Creating a new patient
            CurrentPatient = New Patient(SocialSecurityNumber)

            'Creating a new session
            CurrentPatient.Sessions.Add(New TestSession(CurrentPatient, CompleteSpeechMaterial.ParentTestSpecification))

            'Looking up audiogram data
            'Temporarily just adding some data
            MsgBox("Adding some standard audiograms")
            AvailableAudiograms = New List(Of AudiogramData)

            Dim AudiogramsToAdd As New List(Of AudiogramData.BisgaardAudiograms) From {
                AudiogramData.BisgaardAudiograms.NH,
                AudiogramData.BisgaardAudiograms.N1,
                AudiogramData.BisgaardAudiograms.N2,
                AudiogramData.BisgaardAudiograms.N3,
                AudiogramData.BisgaardAudiograms.N4,
                AudiogramData.BisgaardAudiograms.N5,
                AudiogramData.BisgaardAudiograms.N6,
                AudiogramData.BisgaardAudiograms.N7,
                AudiogramData.BisgaardAudiograms.S1,
                AudiogramData.BisgaardAudiograms.S2,
                AudiogramData.BisgaardAudiograms.S3}

            For Each AudiogramToAdd In AudiogramsToAdd
                Dim NewAudiogram As New AudiogramData() 'CurrentPatient.GetCurrentSession())
                NewAudiogram.CreateTypicalAudiogramData(AudiogramToAdd)
                AvailableAudiograms.Add(NewAudiogram)
            Next

            ''Debugging code
            'For Each AudiogramToAdd In AudiogramsToAdd
            '    Dim NewAudiogram As New AudiogramData(CurrentPatient.GetCurrentSession())

            '    NewAudiogram.CreateIncompleteAudiogramData(AudiogramToAdd, 8)

            '    AvailableAudiograms.Add(NewAudiogram)
            'Next

            'Populating the audiogram data list and selecting the last audiogram added
            SipGui.PopulateAudiogramList(AvailableAudiograms)

            'Initiating a re-calculation chain
            TriggerRecalculationChain(RecalculationStartpoints.NewMeasurement)

        End Sub

        Public Sub SaveFileButtonPressed() Handles SipGui.SaveFileButtonPressed
            Throw New NotImplementedException()
        End Sub

        Public Sub OpenFileButtonPressed() Handles SipGui.OpenFileButtonPressed
            Throw New NotImplementedException()
        End Sub

        Public Sub ExportDataButtonPressed() Handles SipGui.ExportDataButtonPressed
            Throw New NotImplementedException()
        End Sub

#End Region

#Region "Measurement settings"



        ''' <summary>
        ''' The master reference levels, available for testing is hard coded here.
        ''' </summary>
        Private ReadOnly AvailableReferenceLevels As New List(Of Double) From {68 - 10, 68 - 5, 68, 68 + 5, 68 + 10}
        ''' <summary>
        ''' Holds the (zero-based) index of the default reference level in the AvailableReferenceLevels object
        ''' </summary>
        Private ReadOnly DefaultReferenceLevelIndex As Integer = 2

        Private SelectedHearingAidGainType As Nullable(Of HearingAidGainData.GainTypes) = Nothing

        Private DefaultHearingAidGainType As HearingAidGainData.GainTypes = HearingAidGainData.GainTypes.Fig6


        Private Enum RecalculationStartpoints
            NewMeasurement
            AudiogramAdded
            AudiogramData
            ReferenceLevel
            HearingAidGain
            TestPreset
            TestSituation
            TestLength
            PNR
        End Enum

        ''' <summary>
        ''' Triggers a recalculation of data presented in the Gui, based on a recalculation start point indicating which type of data that was modified by the user.
        ''' </summary>
        ''' <param name="Startpoint"></param>
        Private Sub TriggerRecalculationChain(ByVal Startpoint As RecalculationStartpoints)


            If Startpoint <= RecalculationStartpoints.NewMeasurement Then
                CreateNewSipTestMeasurement()

                'If there is no audiogram selected
                If CurrentSipTestMeasurement.SelectedAudiogramData Is Nothing Then

                    ' Repopulates the audiogram list and selects the last audiogram
                    SipGui.PopulateAudiogramList(AvailableAudiograms, AvailableAudiograms.Count - 1)

                End If

            End If

            If Startpoint <= RecalculationStartpoints.AudiogramAdded Then

                ' Repopulates the audiogram list and selects the last audiogram
                SipGui.PopulateAudiogramList(AvailableAudiograms, AvailableAudiograms.Count - 1)

            End If

            'Checks if the audiogram contains data, and stops if not
            If CurrentSipTestMeasurement.SelectedAudiogramData.ContainsAcData = False Then Exit Sub
            If CurrentSipTestMeasurement.SelectedAudiogramData.ContainsCbData = False Then CurrentSipTestMeasurement.SelectedAudiogramData.CalculateCriticalBandValues()


            If Startpoint <= RecalculationStartpoints.AudiogramData Then

                'The audiogram data was updated. Updating the available reference levels in the Gui, and selecting any previously selected value
                If CurrentSipTestMeasurement.ReferenceLevel IsNot Nothing Then

                    'Determining the index of any previously selected value, and then populating the list
                    SipGui.PopulateReferenceLevelList(AvailableReferenceLevels, AvailableReferenceLevels.IndexOf(CurrentSipTestMeasurement.ReferenceLevel))
                Else
                    'Populating the list with the default value
                    SipGui.PopulateReferenceLevelList(AvailableReferenceLevels, DefaultReferenceLevelIndex)
                End If

            End If


            If Startpoint <= RecalculationStartpoints.ReferenceLevel Then

                'The reference level was updated. Updating the choice of hearing-aid gain type
                Dim AvailableGainTypes = HearingAidGainData.GetAvailableGainTypes
                If CurrentSipTestMeasurement.HearingAidGainType IsNot Nothing Then

                    'Determining the index of any previously selected value, and then populating the list
                    SipGui.PopulateHearingAidGainTypeList(AvailableGainTypes, AvailableGainTypes.IndexOf(CurrentSipTestMeasurement.HearingAidGainType))
                Else
                    'Populating the list with the default value
                    SipGui.PopulateHearingAidGainTypeList(AvailableGainTypes, DefaultHearingAidGainType)
                End If


            End If

            If Startpoint <= RecalculationStartpoints.HearingAidGain Then

                'Hearing aid gain type was changed. Calculating the new gain, and then updates the gain plot
                CurrentSipTestMeasurement.HearingAidGain = New HearingAidGainData(CurrentSipTestMeasurement.HearingAidGainType)
                If CurrentSipTestMeasurement.HearingAidGainType = HearingAidGainData.GainTypes.Measured Then
                    'TODO: Here we must use measures real-ear data. It may require a re-structuring of the code!
                    MsgBox("Implement loading of Real-ear measurments!")
                    Dim TempREM As New HearingAidGainData.RealEarData
                    CurrentSipTestMeasurement.HearingAidGain.CalculateGain(TempREM)
                Else
                    CurrentSipTestMeasurement.HearingAidGain.CalculateGain(CurrentSipTestMeasurement.SelectedAudiogramData, CurrentSipTestMeasurement.ReferenceLevel)
                End If

                SipGui.DisplayHearingAidGain(CurrentSipTestMeasurement.HearingAidGain)


                'Updating the choice of preset
                Dim AvailablePresetsNames = CurrentSipTestMeasurement.ParentTestSpecification.SpeechMaterial.Presets.Keys.ToList
                If CurrentSipTestMeasurement.SelectedPresetName IsNot Nothing Then
                    'Determining the index of any previously selected value, and then populating the list
                    SipGui.PopulatePresetList(AvailablePresetsNames.ToList, AvailablePresetsNames.IndexOf(CurrentSipTestMeasurement.SelectedPresetName))
                Else
                    'Populating the list with the default value
                    SipGui.PopulatePresetList(AvailablePresetsNames.ToList, AvailablePresetsNames(0))
                End If

            End If

            If Startpoint <= RecalculationStartpoints.TestPreset Then

                'The preset was updated. Updating the choice of TestSituation
                Dim AvailableMediaSetNames = AvailableMediaSets.GetNames
                If CurrentSipTestMeasurement.SelectedMediaSetName <> "" Then

                    'Determining the index of any previously selected value, and then populating the list
                    SipGui.PopulateTestSituationList(AvailableMediaSetNames, AvailableMediaSetNames.IndexOf(CurrentSipTestMeasurement.SelectedMediaSetName))
                Else

                    SipGui.PopulateTestSituationList(AvailableMediaSetNames, Nothing)

                    'Halting the recalculation chain, since no media set is selected
                    Exit Sub
                End If

            End If

            If Startpoint <= RecalculationStartpoints.TestSituation Then

                'The media set was updated. Updating the test lengths
                Dim AvailableLengthReduplications As New List(Of Integer) From {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 15, 20, 30, 60}

                If CurrentSipTestMeasurement.TestProcedure.LengthReduplications IsNot Nothing Then

                    'Determining the index of any previously selected value, and then populating the list
                    SipGui.PopulateTestLengthList(AvailableLengthReduplications, AvailableLengthReduplications.IndexOf(CurrentSipTestMeasurement.TestProcedure.LengthReduplications))
                Else

                    'Dim DefaultLengthReduplication As Integer = 0 ' TODO: this should be customized in some way!
                    'SipGui.PopulateTestLengthList(AvailableLengthReduplications, AvailableLengthReduplications.IndexOf(DefaultLengthReduplication))

                    SipGui.PopulateTestLengthList(AvailableLengthReduplications, Nothing)

                    'Halting the recalculation chain, since no LengthReduplications is selected
                    Exit Sub
                End If

            End If

            If Startpoint <= RecalculationStartpoints.TestLength Then

                'Test length was updated, adds test trials to the measurement
                CurrentSipTestMeasurement.PlanTestTrials(AvailableMediaSets)

                'Calculates the psychometric function
                Dim PsychoMetricFunction = CurrentSipTestMeasurement.CalculateEstimatedPsychometricFunction()

                Dim PNRs(PsychoMetricFunction.Count - 1) As Single
                Dim PredictedScores(PsychoMetricFunction.Count - 1) As Single
                Dim LowerCriticalBoundary(PsychoMetricFunction.Count - 1) As Single
                Dim UpperCriticalBoundary(PsychoMetricFunction.Count - 1) As Single

                Dim n As Integer = 0
                For Each kvp In PsychoMetricFunction
                    PNRs(n) = kvp.Key
                    PredictedScores(n) = kvp.Value.Item1
                    LowerCriticalBoundary(n) = kvp.Value.Item2
                    UpperCriticalBoundary(n) = kvp.Value.Item3
                    n += 1
                Next

                'Updates the psychometric function diagram
                SipGui.DisplayPredictedPsychometricCurve(PNRs, PredictedScores, LowerCriticalBoundary, UpperCriticalBoundary)


                'Populates the PNR list
                If CurrentSipTestMeasurement.SelectedPnr IsNot Nothing Then

                    'Determining the index of any previously selected value, and then populating the list
                    SipGui.PopulatePnrList(AvailablePNRs, AvailablePNRs.IndexOf(CurrentSipTestMeasurement.SelectedPnr))
                Else

                    Dim DefaultPnr As Integer = 4 ' TODO: this should be customized in some way!
                    SipGui.PopulatePnrList(AvailablePNRs, AvailablePNRs.IndexOf(DefaultPnr))

                End If

            End If

            If Startpoint <= RecalculationStartpoints.PNR Then

                'PNR was set. Initiating testing.
                InitiateNewMeasurement()

            End If


        End Sub

        Public Function CreateNewSipTestMeasurement() As Tuple(Of Boolean, String)

            'TODO: This should come earlier in the recalculation chain!
            If CurrentSipTestMeasurement IsNot Nothing Then
                MsgBox("A question should be sent to the GUI to save unsaved measurement or not")

                'CurrentPatient.Sessions(CurrentPatient.Sessions.Count - 1).Actions.Add(CurrentSipTestMeasurement)

            End If

            CurrentSipTestMeasurement = New TestSession(CurrentPatient, CompleteSpeechMaterial.ParentTestSpecification)
            'CurrentSipTestMeasurement = New SipTestMeasurement(CurrentPatient.Sessions(CurrentPatient.Sessions.Count - 1), Me)


            'Probably this does not need any return, could anything go wrong?
            Return New Tuple(Of Boolean, String)(True, "")

        End Function


        Public Sub CreateNewAudiogram() Handles SipGui.CreateNewAudiogram

            'Stores the selected audiogram data
            Dim NewAudiogram = New AudiogramData
            NewAudiogram.Name = DateTime.Now

            AvailableAudiograms.Add(NewAudiogram)

            CurrentSipTestMeasurement.SelectedAudiogramData = NewAudiogram

            TriggerRecalculationChain(RecalculationStartpoints.AudiogramAdded)

        End Sub

        Public Sub SelectAudiogram(ByRef SelectedAudiogramData As AudiogramData) Handles SipGui.SelectAudiogram

            'Stores the selected audiogram data
            CurrentSipTestMeasurement.SelectedAudiogramData = SelectedAudiogramData

            SipGui.DisplaySelectedAudiogram(CurrentSipTestMeasurement.SelectedAudiogramData)

            TriggerRecalculationChain(RecalculationStartpoints.AudiogramData)


        End Sub

        Public Sub SelectReferenceLevel(SelectedReferenceLevel As Double) Handles SipGui.SelectReferenceLevel

            'Stores the selected reference level
            CurrentSipTestMeasurement.ReferenceLevel = SelectedReferenceLevel

            TriggerRecalculationChain(RecalculationStartpoints.ReferenceLevel)

        End Sub

        Public Sub SelectHearingAidGainType(SelectedHearingAidGainType As String) Handles SipGui.SelectHearingAidGainType

            'Stores the selected hearing-aid gain type
            CurrentSipTestMeasurement.HearingAidGainType = SelectedHearingAidGainType

            TriggerRecalculationChain(RecalculationStartpoints.HearingAidGain)

        End Sub

        Public Sub SelectPreset(SelectedPreset As String) Handles SipGui.SelectPreset

            'Stores the selected preset
            CurrentSipTestMeasurement.SelectedPresetName = SelectedPreset

            TriggerRecalculationChain(RecalculationStartpoints.TestPreset)

        End Sub

        Public Sub SelectSituation(SelectedSituation As String) Handles SipGui.SelectSituation

            'Stores the selected preset
            CurrentSipTestMeasurement.SelectedMediaSetName = SelectedSituation

            TriggerRecalculationChain(RecalculationStartpoints.TestSituation)

        End Sub

        Public Sub SelectTestLength(SelectedTestLength As Integer) Handles SipGui.SelectTestLength

            'Stores the selected preset
            CurrentSipTestMeasurement.TestProcedure.LengthReduplications = SelectedTestLength

            TriggerRecalculationChain(RecalculationStartpoints.TestLength)

        End Sub

        Public Sub SelectPNR(SelectedPNR As Double) Handles SipGui.SelectPNR

            'Stores the selected selected PNR
            CurrentSipTestMeasurement.SelectedPnr = SelectedPNR

            TriggerRecalculationChain(RecalculationStartpoints.PNR)

        End Sub




#End Region

#Region "Active measurement"

        Public Sub InitiateNewMeasurement()

            Dim GetGuiTableData = CurrentSipTestMeasurement.GetGuiTableData()

            SipGui.UpdateTestTrialTable(GetGuiTableData.TestWords.ToArray, GetGuiTableData.Responses.ToArray, GetGuiTableData.ResultResponseTypes.ToArray,
                                            GetGuiTableData.UpdateRow, GetGuiTableData.SelectionRow, GetGuiTableData.FirstRowToDisplayInScrollmode)

            'SipGui.UpdateTestProgress(CurrentSipTestMeasurement.TestLength, CurrentSipTestMeasurement.NumberPresented, CurrentSipTestMeasurement.NumberCorrect, CurrentSipTestMeasurement.PercentCorrect)

            SimulateAdaptiveTests()

            'SipGui.SubEnablePlayButton()


        End Sub

        Private Sub SimulateAdaptiveTests()

            Dim Ms As New List(Of Integer) From {1, 2, 3}

            For Each M In Ms


                Dim Simulations As Integer = 500

                Dim Seed As Integer = 42
                Dim Rnd As New Random(Seed)

                Dim TestResults As New List(Of String)
                Dim ProgressDisplay As New ProgressDisplay
                ProgressDisplay.Initialize(Simulations, 0, "Simulating adaptive tests...", 1)
                ProgressDisplay.Show()

                For i = 1 To Simulations

                    ProgressDisplay.UpdateProgress(i)

                    Dim NewResult = SimulateAdaptiveTest(Rnd, M)

                    TestResults.Add(i.ToString() & vbTab & vbTab &
                                    NewResult.Item1.Last.ToString & vbTab & vbTab &
                                    NewResult.Item1.Average.ToString & vbTab & vbTab &
                                    String.Join(vbTab, NewResult.Item1) & vbTab & vbTab &
                                    String.Join(vbTab, NewResult.Item2))

                Next

                Utils.SendInfoToLog(vbCrLf & vbCrLf & "Method: " & M & vbCrLf & String.Join(vbCrLf, TestResults), "SimulationResults")

                ProgressDisplay.Close()

            Next

            MsgBox("Simulation Complete")

        End Sub


        Private Function SimulateAdaptiveTest(ByRef Rnd As Random, ByVal Method As Integer) As Tuple(Of List(Of Double), List(Of String))

            Dim CurrentPNR As Double = 0
            Dim Elapses As Integer = 0
            Dim PNRList As New List(Of Double)
            Dim ResponseList As New List(Of String)

            Dim ResultList As New List(Of Integer)

            For Each Trial In CurrentSipTestMeasurement.PlannedTrials

                PNRList.Add(CurrentPNR)

                Trial.SetLevels(CurrentSipTestMeasurement.ReferenceLevel, CurrentPNR)

                Dim p = Trial.EstimatedSuccessProbability(True)

                'Simulating a test trial response by sampling from a bernoulli distribution

                Dim BernoulliTrialResult = MathNet.Numerics.Distributions.Bernoulli.Sample(Rnd, p)
                Dim SimulatedResponse As String = ""

                ResultList.Add(BernoulliTrialResult)

                Dim StepSize As Double
                If Elapses >= 30 Then
                    StepSize = 1
                Else
                    StepSize = 2
                End If


                Select Case Method
                    Case 1
                        If ResultList.Count = 3 Then
                            ChangePNR(CurrentPNR, ResultList, StepSize)
                            ResultList.Clear()
                        End If
                    Case 2
                        If ResultList.Count = 6 Then
                            ChangePNR(CurrentPNR, ResultList, StepSize)
                            ResultList.Clear()
                        End If
                    Case 3
                        If ResultList.Count = 12 Then
                            ChangePNR(CurrentPNR, ResultList, StepSize)
                            ResultList.Clear()
                        End If
                End Select

                If BernoulliTrialResult = 1 Then
                    SimulatedResponse = "Correct"
                Else
                    SimulatedResponse = "Incorrect"
                End If

                Elapses += 1

                ResponseList.Add(SimulatedResponse)

            Next

            Return New Tuple(Of List(Of Double), List(Of String))(PNRList, ResponseList)

        End Function

        Private Sub ChangePNR(ByRef CurrentPNR As Double, ResultList As List(Of Integer), ByVal StepSize As Double)
            Select Case ResultList.Average
                Case <= (2 / 3)
                    CurrentPNR += StepSize
                Case 1
                    CurrentPNR -= StepSize
                Case Else
                    'Do not change!
            End Select
        End Sub

        Public Sub StartButtonPressed() Handles SipGui.StartButtonPressed
            StartTest()
        End Sub

        Public Sub StopButtonPressed() Handles SipGui.StopButtonPressed
            Throw New NotImplementedException()
        End Sub


        Public Sub StartTest()

            'Should initiate a test-launch sequence

            If CurrentSipTestMeasurement Is Nothing Then
                SipGui.ShowMessageBox("Inget test är laddat.", "SiP-testet")
            End If


            'Tries to launch the next test trial
            'Dim TrialLaunchResult = CurrentSipTestMeasurement.LaunchNextTrial()
            'Select Case TrialLaunchResult
            '    Case SipTestMeasurement.LaunchNextTrialReturnValues.TrialWasLaunched
            '        'No need to send and message? Or send message to alter GUI-layout?

            '    Case SipTestMeasurement.LaunchNextTrialReturnValues.TestingCompleted
            '        SipGui.ShowMessageBox("Testet är klart.", "SiP-testet")

            '    Case SipTestMeasurement.LaunchNextTrialReturnValues.NoTestTrials
            '        SipGui.ShowMessageBox("Inget test är laddat.", "SiP-testet")
            '    Case Else
            '        Throw New NotImplementedException("Unsupported error" & TrialLaunchResult.ToString)
            'End Select

        End Sub



#End Region


#Region "Test-result comparison"

        Public Function GetTestHistoryListData() As TestHistoryListData

            Return GetTestHistoryListAndComparisonData(Nothing).Item1

        End Function

        ''' <summary>
        ''' Performs a statistical analysis of the score difference between the SiP-testet refered to in ComparedMeasurementGuiDescriptions by their GuiDescription strings
        ''' </summary>
        ''' <param name="ComparedMeasurementGuiDescriptions"></param>
        Sub CompareTwoSipTestScores(ByRef ComparedMeasurementGuiDescriptions As List(Of String)) Handles SipGui.CompareTwoSipTestScores

            'Clears the Gui significance test result box if not exaclty two measurements descriptions are recieved. And the exits the sub
            If ComparedMeasurementGuiDescriptions.Count <> 2 Then
                SipGui.UpdateSignificanceTestResult("")
                Exit Sub
            End If

            'We've got two measurements, runs the significance test
            'TODO: Uncomment line below
            MsgBox("Implement the Agresti-Caffo corrected PB-method of statistical significance testing between the two tests here!")
            'Dim ComparisonScores = GetTestHistoryListAndComparisonData(ComparedMeasurementGuiDescriptions).Item2


            SipGui.UpdateSignificanceTestResult("T.ex: Statistiskt signifikant skillnad")

        End Sub

        Public Function GetTestHistoryListAndComparisonData(ByRef ComparedMeasurementGuiDescriptions As List(Of String)) As Tuple(Of TestHistoryListData, List(Of TestSession))

            'If ComparedMeasurementGuiDescriptions IsNot Nothing Then
            '    If ComparedMeasurementGuiDescriptions.Count <> 2 Then Throw New ArgumentException("Only two SipTestMeasurements can be selected for comparison.")
            'End If

            'Dim OutputList As New Tuple(Of TestHistoryListData, List(Of TestSession))(New TestHistoryListData, New List(Of TestSession))

            'If CurrentPatient Is Nothing Then Return OutputList

            'If CurrentPatient.Sessions Is Nothing Then Return OutputList

            'If CurrentPatient.Sessions.Count > 0 Then

            '    'Adding the current session data (the current session should be the one added last to Sessions)
            '    Dim CurrentSession = CurrentPatient.Sessions(CurrentPatient.Sessions.Count - 1)

            '    For Each a In CurrentSession.Actions

            '        'Checking that the action is a SipTestMeasurement
            '        If a.GetType = GetType(TestSession) Then

            '            MsgBox("Check this type comparison!")
            '            Dim CastSipTestMeasurement = DirectCast(a, TestSession)
            '            Dim GuiDescription As String = CastSipTestMeasurement.Description
            '            OutputList.Item1.CurrentTestSessionData.Add(New TestHistoryListData.SipTestMeasurementGuiDescription(GuiDescription, CastSipTestMeasurement.TestLength.ToString, CastSipTestMeasurement.PercentCorrect))

            '            'Checking if the session should be a compared session
            '            If ComparedMeasurementGuiDescriptions IsNot Nothing Then
            '                If ComparedMeasurementGuiDescriptions.Contains(GuiDescription) Then
            '                    OutputList.Item2.Add(CastSipTestMeasurement)
            '                End If
            '            End If
            '        End If
            '    Next
            'End If

            'If CurrentPatient.Sessions.Count > 1 Then

            '    'Adding the previous session data (all sessions before last one added to Sessions)
            '    For s = 0 To CurrentPatient.Sessions.Count - 2
            '        Dim CurrentSession = CurrentPatient.Sessions(CurrentPatient.Sessions.Count - 1)

            '        For Each a In CurrentSession.Actions

            '            'Checking that the action is a SipTestMeasurement
            '            If a.GetType = GetType(TestSession) Then

            '                MsgBox("Check this type comparison!")
            '                Dim CastSipTestMeasurement = DirectCast(a, TestSession)
            '                Dim GuiDescription As String = CastSipTestMeasurement.Description & " " & CastSipTestMeasurement.CreateDateFormatted
            '                OutputList.Item1.CurrentTestSessionData.Add(New TestHistoryListData.SipTestMeasurementGuiDescription(GuiDescription, CastSipTestMeasurement.TestLength.ToString, CastSipTestMeasurement.PercentCorrect))

            '                'Checking if the session should be a compared session
            '                If ComparedMeasurementGuiDescriptions IsNot Nothing Then
            '                    If ComparedMeasurementGuiDescriptions.Contains(GuiDescription) Then
            '                        OutputList.Item2.Add(CastSipTestMeasurement)
            '                    End If
            '                End If

            '            End If
            '        Next

            '    Next
            'End If

            'Return OutputList

        End Function

#End Region


#Region "BlueToothConnection"


        Public Sub SearchForBluetoothDevices() Handles SipGui.SearchForBluetoothDevices
            Throw New NotImplementedException()
        End Sub

        Public Sub SelectBluetoothDevice(SelectedBluetoothDeviceDescription As String) Handles SipGui.SelectBluetoothDevice
            Throw New NotImplementedException()
        End Sub

#End Region

#Region "SoundDevice"

        Public Sub SearchForSoundDevices() Handles SipGui.SearchForSoundDevices
            Throw New NotImplementedException()
        End Sub

        Public Sub SelectSoundDevice(SelectedSoundDeviceDescription As String) Handles SipGui.SelectSoundDevice
            Throw New NotImplementedException()
        End Sub

#End Region



    End Class

End Namespace
