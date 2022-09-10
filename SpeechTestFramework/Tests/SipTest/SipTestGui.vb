Imports SpeechTestFramework
Imports SpeechTestFramework.SipTest
Imports SpeechTestFramework.WinFormControls
Imports System.Windows.Forms
Imports System.Drawing

Public Class SipTestGui

    Private CompleteSpeechMaterial As SpeechMaterialComponent

    Private WithEvents Audiogram As Audiogram
    Private GainDiagram As GainDiagram
    Private ExpectedScoreDiagram As PsychometricFunctionDiagram


    Private AvailableAudiograms As New List(Of AudiogramData)
    Private ReadOnly AvailableReferenceLevels As New List(Of Double) From {68 - 10, 68 - 5, 68, 68 + 5, 68 + 10}
    Private AvailableHaGains As New List(Of HearingAidGainData)
    Private AvailablePresetsNames As List(Of String)
    Private AvailableMediaSets As MediaSetLibrary
    Private AvailableLengthReduplications As New List(Of Integer) From {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 15, 20, 30, 60}


    Friend AvailablePNRs As New List(Of Double) From {-15, -12, -10, -8, -6, -4, -2, 0, 2, 4, 6, 8, 10, 12, 15}

    Friend CurrentSipTestMeasurement As SipMeasurement

    ''' <summary>
    ''' Holds the (zero-based) index of the default reference level in the AvailableReferenceLevels object
    ''' </summary>
    Private ReadOnly DefaultReferenceLevelIndex As Integer = 2



    'Friend SoundPlayer As Audio.PaOverlappingSoundPlayerC
    'Friend BlueToothConnection As BlueToothConnection

    Private NumberSpeakerChannels As Integer = 3





    Private Sub SipTestGui_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'Initializing all components
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

        'TODO: We should get sound format data from the speech material before creating the PlayBackWaveFormat
        'TODO: and NumberSpeakerChannels from where? Gui setting? Headphones?

        'Creating a sound format for the output sound (Same sample rate, bit depth and encoding as used in the SiP-test sound files)
        Dim PlayBackWaveFormat = New Audio.Formats.WaveFormat(48000, 32, NumberSpeakerChannels,, Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints)

        Dim CalibrationData = LookForCalibrationData()




        'Hiding things that should not be visible from the start
        StatAnalysisLabel.Visible = False

        'Createing diagrams and adding their references to the corresponding private fields
        AudiogramPanel.Controls.Add(New Audiogram)
        Audiogram = AudiogramPanel.Controls(AudiogramPanel.Controls.Count - 1)

        GainPanel.Controls.Add(New GainDiagram)
        GainDiagram = GainPanel.Controls(GainPanel.Controls.Count - 1)

        ExpectedScorePanel.Controls.Add(New PsychometricFunctionDiagram)
        ExpectedScoreDiagram = ExpectedScorePanel.Controls(ExpectedScorePanel.Controls.Count - 1)

        'Docks the diagrams in their parent controls
        Audiogram.Dock = DockStyle.Fill
        GainDiagram.Dock = DockStyle.Fill
        ExpectedScoreDiagram.Dock = DockStyle.Fill

        'Setting diagram border style
        Audiogram.BorderStyle = BorderStyle.FixedSingle
        GainDiagram.BorderStyle = BorderStyle.FixedSingle
        ExpectedScoreDiagram.BorderStyle = BorderStyle.FixedSingle

        'Adding available reference levels
        ReferenceLevelComboBox.Items.Clear()
        For Each RefLevel In AvailableReferenceLevels
            ReferenceLevelComboBox.Items.Add(RefLevel)
        Next
        ReferenceLevelComboBox.SelectedIndex = DefaultReferenceLevelIndex

        'Adding a default no-gain hearing aid gain
        Dim NoGainData = HearingAidGainData.CreateNewNoGainData
        NoGainData.Name = "No gain"
        AvailableHaGains.Add(NoGainData)

        'Adding available preset names
        AvailablePresetsNames = CompleteSpeechMaterial.Presets.Keys.ToList
        PresetComboBox.Items.Clear()
        For Each Preset In AvailablePresetsNames
            PresetComboBox.Items.Add(Preset)
        Next
        'We don't select a default here...?? 

        'Adding available test situations
        TestSituationComboBox.Items.Clear()
        For Each Value In AvailableMediaSets
            TestSituationComboBox.Items.Add(Value)
        Next
        'We don't select a default here...?? 

        'Adding available test length reduplications
        TestLengthComboBox.Items.Clear()
        For Each TestLength In AvailableLengthReduplications
            TestLengthComboBox.Items.Add(TestLength)
        Next
        'We don't select a default here...?? 

        'Adding possible PNR values
        PnrComboBox.Items.Clear()
        For Each Pnr In AvailablePNRs
            PnrComboBox.Items.Add(Pnr)
        Next
        'We don't yet select a default here...?? It could possibly be done automatically at a later stage...


    End Sub



    Private Enum RecalculationStartpoints
        ParticipantLocked
        AudiogramSelected
        ReferenceLevelSelected
        HearingAidGainSelected
        TestPresetSelected
        TestSituationSelected
        TestLengthSelected
    End Enum

    Private CurrentPatient As Participant = Nothing
    Private SelectedAudiogramData As AudiogramData = Nothing
    Private SelectedReferenceLevel As Double?
    Private SelectedHearingAidGain As HearingAidGainData = Nothing
    Private SelectedPresetName As String = ""
    Private SelectedMediaSet As MediaSet = Nothing
    Private SelectedLengthReduplications As Integer?
    Private SelectedPnr As Double?
    Private SelectedTestDescription As String = ""

    Private TestHistorySummary As New TestHistorySummary

#Region "Participant"

    Public Sub LockParticipant(sender As Object, e As EventArgs) Handles ParticipantLockButton.Click

        'Then look up a patient in a file or database, create a patient from it and reference that patient into the CurrentPatient property

        Dim ParticipantID As String = ParticipantIdTextBox.Text

        'Checking SocialSecurityNumber
        If ParticipantID = "" Then
            ShowMessageBox("Please enter a participant ID", "Invalid participant ID")
            Exit Sub
        End If

        'Locking ID controls in the Gui
        LockPatientDetails(ParticipantID)

        'Creating a new patient
        CurrentPatient = New Participant(ParticipantID)

    End Sub

    Public Sub LockPatientDetails(ID As String)

        ParticipantIdTextBox.ReadOnly = True
        ParticipantIdTextBox.Text = ID
        ParticipantLockButton.Enabled = False

    End Sub

#End Region

#Region "Audiogram"

    Public Sub CreateNewAudiogram(sender As Object, e As EventArgs) Handles NewAudiogram_Button.Click

        'Stores the selected audiogram data
        Dim NewAudiogram = New AudiogramData
        NewAudiogram.Name = CurrentPatient.ID & "_" & DateTime.Now
        AvailableAudiograms.Add(NewAudiogram)

        UpdateAudiogramList()

    End Sub


    Private Sub AddTypicalAudiograms_Button_Click(sender As Object, e As EventArgs) Handles AddTypicalAudiograms_Button.Click

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

        UpdateAudiogramList()

        'Preventing the user from adding these again by disabling the button
        AddTypicalAudiograms_Button.Enabled = False

    End Sub

    ''' <summary>
    ''' Updates the audiogram list and selects the audiogram last added 
    ''' </summary>
    Public Sub UpdateAudiogramList()

        AudiogramComboBox.Items.Clear()
        AudiogramComboBox.Items.AddRange(AvailableAudiograms.ToArray)
        If AudiogramComboBox.Items.Count > 0 Then
            AudiogramComboBox.SelectedIndex = AudiogramComboBox.Items.Count - 1
        End If

    End Sub

    Public Sub SelectAudiogram(sender As Object, e As EventArgs) Handles AudiogramComboBox.SelectedIndexChanged

        'Stores the selected audiogram data
        SelectedAudiogramData = AudiogramComboBox.SelectedItem

        'Displays it in the Audiogram
        Audiogram.AudiogramData = SelectedAudiogramData

        'Checks if the audiogram contains data, and stops if not
        If SelectedAudiogramData.ContainsAcData = False Then Exit Sub
        If SelectedAudiogramData.ContainsCbData = False Then SelectedAudiogramData.CalculateCriticalBandValues()

        TryCalculatePsychometricFunction()

    End Sub

    Public Sub AudiogramDataChanged() Handles Audiogram.DataChanged

        'Checks if the audiogram contains data, and stops if not
        If SelectedAudiogramData.ContainsAcData = False Then Exit Sub

        'Updating the critical band values
        SelectedAudiogramData.CalculateCriticalBandValues()

        'Triggers recalculation based on a change in the selected audiogram data
        TryCalculatePsychometricFunction()

    End Sub

#End Region

    Public Sub SelectReferenceLevel(sender As Object, e As EventArgs) Handles ReferenceLevelComboBox.SelectedIndexChanged

        'Stores the selected reference level
        SelectedReferenceLevel = ReferenceLevelComboBox.SelectedItem

        TryCalculatePsychometricFunction()

    End Sub

#Region "HaGain"

    Private Sub CreateNewGain_Button_Click(sender As Object, e As EventArgs) Handles CreateNewGain_Button.Click

        Dim NewGain = HearingAidGainData.CreateNewNoGainData
        NewGain.Name = CurrentPatient.ID & "_" & DateTime.Now
        AvailableHaGains.Add(NewGain)
        UpdateGainList()

    End Sub

    Private Sub AddFig6Gain_Button_Click(sender As Object, e As EventArgs) Handles AddFig6Gain_Button.Click

        If SelectedAudiogramData Is Nothing Or SelectedReferenceLevel Is Nothing Then
            MsgBox("Before you can create new Fig6 gain, you must select/supply audiogram data and reference level.", MsgBoxStyle.Information, "Please supply data!")
            Exit Sub
        End If

        Dim NewGain = HearingAidGainData.CreateNewFig6GainData(SelectedAudiogramData, SelectedReferenceLevel)
        NewGain.Name = CurrentPatient.ID & "_" & "Fig6" & "_" & SelectedAudiogramData.Name
        AvailableHaGains.Add(NewGain)
        UpdateGainList()

    End Sub


    ''' <summary>
    ''' Updates the hearing aid gain list and selects the gain data last added 
    ''' </summary>
    Public Sub UpdateGainList()

        HaGainComboBox.Items.Clear()
        HaGainComboBox.Items.AddRange(AvailableHaGains.ToArray)
        If HaGainComboBox.Items.Count > 0 Then
            HaGainComboBox.SelectedIndex = HaGainComboBox.Items.Count - 1
        End If

    End Sub


    Public Sub SelectHearingAidGain(sender As Object, e As EventArgs) Handles HaGainComboBox.SelectedIndexChanged

        'Stores the selected hearing-aid gain type
        SelectedHearingAidGain = HaGainComboBox.SelectedItem

        'Displays it in the Audiogram
        GainDiagram.UpdateGainValues(SelectedHearingAidGain)

        TryCalculatePsychometricFunction()

    End Sub

#End Region

    Public Sub SelectPreset(sender As Object, e As EventArgs) Handles PresetComboBox.SelectedIndexChanged

        'Stores the selected preset
        SelectedPresetName = PresetComboBox.SelectedItem

        TryCalculatePsychometricFunction()

    End Sub

    Public Sub SelectSituation(sender As Object, e As EventArgs) Handles TestSituationComboBox.SelectedIndexChanged

        'Stores the selected preset
        SelectedMediaSet = TestSituationComboBox.SelectedItem

        TryCalculatePsychometricFunction()

    End Sub

    Public Sub SelectTestLength(sender As Object, e As EventArgs) Handles TestLengthComboBox.SelectedIndexChanged

        'Stores the selected preset
        SelectedLengthReduplications = TestLengthComboBox.SelectedItem


        'Setting the test length is the last required step before calculating estimated psychometric function and therefore the last re-calculation step
        'At this stage a new test session should be created

        TryCalculatePsychometricFunction()

    End Sub

#Region "PsychometricFunction"


    Private Sub TryCalculatePsychometricFunction()

        If CurrentPatient Is Nothing Then Exit Sub
        If SelectedAudiogramData Is Nothing Then Exit Sub
        If SelectedReferenceLevel.HasValue = False Then Exit Sub
        If SelectedHearingAidGain Is Nothing Then Exit Sub
        If SelectedPresetName = "" Then Exit Sub
        If SelectedMediaSet Is Nothing Then Exit Sub
        If SelectedLengthReduplications.HasValue = False Then Exit Sub


        'Creates a new test and updates the psychometric function diagram
        CurrentSipTestMeasurement = New SipMeasurement(CurrentPatient, CompleteSpeechMaterial.ParentTestSpecification)
        CurrentSipTestMeasurement.SelectedAudiogramData = SelectedAudiogramData
        CurrentSipTestMeasurement.ReferenceLevel = SelectedReferenceLevel
        CurrentSipTestMeasurement.HearingAidGain = SelectedHearingAidGain
        CurrentSipTestMeasurement.SelectedPresetName = SelectedPresetName
        CurrentSipTestMeasurement.SelectedMediaSetName = SelectedMediaSet.MediaSetName ' TODO: should we use the name or the mediaset in the GUI/Measurment?
        CurrentSipTestMeasurement.TestProcedure.LengthReduplications = SelectedLengthReduplications

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
        DisplayPredictedPsychometricCurve(PNRs, PredictedScores, LowerCriticalBoundary, UpperCriticalBoundary)

        'Initiates the test
        InitiateNewMeasurement()

    End Sub

    Public Sub DisplayPredictedPsychometricCurve(PNRs() As Single, PredictedScores() As Single, LowerCiLimits() As Single, UpperCiLimits() As Single)

        ExpectedScoreDiagram.Lines.Clear()
        ExpectedScoreDiagram.Lines.Add(New PlotBase.Line With {.Color = Color.Black, .Dashed = False, .LineWidth = 3, .XValues = PNRs, .YValues = PredictedScores})

        ExpectedScoreDiagram.Areas.Clear()

        'High-jacking the values in PredictedScores for now
        For n = 0 To PredictedScores.Length - 1
            LowerCiLimits(n) = PredictedScores(n) - 0.1
            UpperCiLimits(n) = PredictedScores(n) + 0.1
        Next

        ExpectedScoreDiagram.Areas.Add(New PlotBase.Area With {.Color = Color.Pink, .XValues = PNRs, .YValuesLower = LowerCiLimits, .YValuesUpper = UpperCiLimits})

        ExpectedScoreDiagram.Invalidate()
        ExpectedScoreDiagram.Update()

    End Sub

#End Region


    Public Sub SelectPNR(sender As Object, e As EventArgs) Handles PnrComboBox.SelectedIndexChanged

        'Stores the selected selected PNR
        SelectedPnr = PnrComboBox.SelectedItem

    End Sub

    Private Sub TestDescriptionTextBox_TextChanged(sender As Object, e As EventArgs) Handles TestDescriptionTextBox.TextChanged

        SelectedTestDescription = TestDescriptionTextBox.Text

    End Sub


    Public Sub InitiateNewMeasurement()

        If CurrentSipTestMeasurement Is Nothing Then
            ShowMessageBox("Inget test är laddat.", "SiP-test")
            Exit Sub
        End If

        Dim GetGuiTableData = CurrentSipTestMeasurement.GetGuiTableData()

        UpdateTestTrialTable(GetGuiTableData.TestWords.ToArray, GetGuiTableData.Responses.ToArray, GetGuiTableData.ResultResponseTypes.ToArray,
                                            GetGuiTableData.UpdateRow, GetGuiTableData.SelectionRow, GetGuiTableData.FirstRowToDisplayInScrollmode)

        'SipGui.UpdateTestProgress(CurrentSipTestMeasurement.TestLength, CurrentSipTestMeasurement.NumberPresented, CurrentSipTestMeasurement.NumberCorrect, CurrentSipTestMeasurement.PercentCorrect)

        EnablePlayButton()


    End Sub


#Region "Active measurement"

    Public Sub StartTest() Handles StartButton.Click

        If SelectedPnr Is Nothing Then
            ShowMessageBox("Please select a PNR value!", "SiP-test")
            Exit Sub
        End If
        If SelectedTestDescription = "" Then
            ShowMessageBox("Please provide a test description (such as 'test 1, with HA')!", "SiP-test")
            Exit Sub
        End If

        'Storing the SelectedPnr and the SelectedTestDescription
        CurrentSipTestMeasurement.SelectedPnr = SelectedPnr
        CurrentSipTestMeasurement.TestDescription = SelectedTestDescription

        'Things seemed to be in order,
        'Starting the test

        LockSettingsPanels()

        'Creates a new randomizer before each test start
        Dim Seed As Integer? = 42 'TODO: remove this seed value, and possible let the user specify one instead
        If Seed.HasValue Then
            SipMeasurementRandomizer = New Random(Seed)
        Else
            SipMeasurementRandomizer = New Random
        End If

        NewTrialTimer.Start()

    End Sub

    Private SipMeasurementRandomizer As Random

    Private CurrentSipTrial As SipTrial
    Private WithEvents NewTrialTimer As New Windows.Forms.Timer With {.Interval = 500} ' TODO: Set this interval to the correct SiP-value!

    Public Sub InitiateNextTrial() Handles NewTrialTimer.Tick
        NewTrialTimer.Stop()

        CurrentSipTrial = CurrentSipTestMeasurement.GetNextTrial(SipMeasurementRandomizer)

        If CurrentSipTrial Is Nothing Then
            TestCompleted()
            Exit Sub
        End If

        MsgBox(CurrentSipTrial.SpeechMaterialComponent.PrimaryStringRepresentation)

        CurrentSipTrial.TrialResult = ResultResponseType.Correct

        NewTrialTimer.Start()

    End Sub




    Public Sub TestCompleted()

        'Display results
        Dim NewMeasurementSummary = CurrentSipTestMeasurement.GetMeasurementSummary
        TestHistorySummary.Measurements.Add(NewMeasurementSummary)
        PopulateTestHistoryTables()


        'Export data here?

        'Resets values to prepare for next measurement
        ResetValuesAfterMeasurement()

    End Sub


    Public Sub ResetValuesAfterMeasurement()

        ClearTestNameBox()

        UnlockSettingsPanels()

        MsgBox("Unlock stuff for new test!")

    End Sub


    Public Sub StopButton_Click() Handles StopButton.Click
        Throw New NotImplementedException()
    End Sub


#End Region









#Region "Active measurement - GUI stuff"

    Public Sub EnablePlayButton()
        StartButton.Enabled = True
        StartButton.BackgroundImage = My.Resources.PlayImage
    End Sub

    Public Sub DisableStopButton()
        StopButton.Enabled = False
        StopButton.BackgroundImage = My.Resources.StopDisabledImage
    End Sub

    Public Sub EnableStopButton()
        StopButton.Enabled = True
        StopButton.BackgroundImage = My.Resources.StopImage
    End Sub

    Public Sub DisablePlayButton()
        StartButton.Enabled = False
        StartButton.BackgroundImage = My.Resources.PlayDisabledImage
    End Sub

    Public Sub TogglePlayButton(PlayMode As Boolean)
        If StartButton.Enabled = True Then
            If PlayMode = True Then
                StartButton.BackgroundImage = My.Resources.PlayImage
            Else
                StartButton.BackgroundImage = My.Resources.PauseImage
            End If
        Else
            If PlayMode = True Then
                StartButton.BackgroundImage = My.Resources.PlayDisabledImage
            Else
                StartButton.BackgroundImage = My.Resources.PauseDisabledImage
            End If
        End If
    End Sub

    Public Sub UpdateTestProgress(Max As Integer, Progress As Integer, Correct As Integer, ProportionCorrect As String)
        MeasurementProgressBar.Minimum = 0
        MeasurementProgressBar.Maximum = Max
        MeasurementProgressBar.Value = Progress

        CorrectCountTextBox.Text = Correct & " / " & Progress
        ProportionCorrectTextBox.Text = ProportionCorrect
    End Sub

    Public Sub UpdateTestTrialTable(ByVal TestWords() As String, ByVal Responses() As String, ByVal ResultResponseTypes() As SipTest.ResultResponseType,
                             Optional ByVal UpdateRow As Integer? = Nothing, Optional SelectionRow As Integer? = Nothing, Optional FirstRowToDisplayInScrollmode As Integer? = Nothing)

        'Checking input arguments
        If TestWords.Length <> Responses.Length Or TestWords.Length <> ResultResponseTypes.Length Then
            Throw New ArgumentException("TestWords, Responses and ResultResponseTypes must all have the same length!")
        End If

        If UpdateRow.HasValue = True Then
            If UpdateRow < 0 Or UpdateRow >= Responses.Length Then Throw New ArgumentException("UpdateRow must be non-negative integer, less than the length of the number of test-list items!")
        End If

        If SelectionRow.HasValue = True Then
            If SelectionRow < 0 Or SelectionRow >= Responses.Length Then Throw New ArgumentException("SelectionRow must be non-negative integer, less than the length of the number of test-list items!")
        End If

        If FirstRowToDisplayInScrollmode.HasValue = True Then
            If FirstRowToDisplayInScrollmode < 0 Or FirstRowToDisplayInScrollmode >= Responses.Length Then Throw New ArgumentException("FirstRowToDisplayInScrollmode must be non-negative integer, less than the length of the number of test-list items!")
        End If


        'Determines if the whole table can be 
        Dim UpdateOnlySpecificRow As Boolean = False

        If UpdateRow.HasValue = True Then
            If Responses.Length = TestTrialDataGridView.Rows.Count Then
                'Allows updating of only a specific row if 
                '-an UpdateIndex is given
                '-the number of rows in the existing table equals the number of test-list items (taken as the length of TestWords). This could happen if the number of test-list items have changed.
                UpdateOnlySpecificRow = True
            End If
        End If

        If UpdateOnlySpecificRow = True Then

            TestTrialDataGridView.Rows(UpdateRow.Value).Cells(0).Value = TestWords(UpdateRow.Value)
            TestTrialDataGridView.Rows(UpdateRow.Value).Cells(1).Value = Responses(UpdateRow.Value)

            Select Case ResultResponseTypes(UpdateRow.Value)
                Case SipTest.ResultResponseType.Correct
                    TestTrialDataGridView.Rows(UpdateRow.Value).Cells(2).Value = My.Resources.CorrectResponseImage

                Case SipTest.ResultResponseType.Incorrect
                    TestTrialDataGridView.Rows(UpdateRow.Value).Cells(2).Value = My.Resources.IncorrectResponseImage

                Case SipTest.ResultResponseType.NotPresented
                    TestTrialDataGridView.Rows(UpdateRow.Value).Cells(2).Value = My.Resources.TrialNotPresentedImage

                Case Else
                    Throw New ArgumentException("Unknown SipTestTrial.ResultResponseType!")
            End Select

        Else

            'Clearing all rows
            TestTrialDataGridView.Rows.Clear()

            'Creating new rows
            TestTrialDataGridView.Rows.Add(TestWords.Length)

            'Adding data to all rows
            For r = 0 To TestWords.Length - 1

                TestTrialDataGridView.Rows(r).Cells(0).Value = TestWords(r)
                TestTrialDataGridView.Rows(r).Cells(1).Value = Responses(r)

                Select Case ResultResponseTypes(r)
                    Case SipTest.ResultResponseType.Correct
                        TestTrialDataGridView.Rows(r).Cells(2).Value = My.Resources.CorrectResponseImage

                    Case SipTest.ResultResponseType.Incorrect
                        TestTrialDataGridView.Rows(r).Cells(2).Value = My.Resources.IncorrectResponseImage

                    Case SipTest.ResultResponseType.NotPresented
                        TestTrialDataGridView.Rows(r).Cells(2).Value = My.Resources.TrialNotPresentedImage

                    Case Else
                        Throw New ArgumentException("Unknown SipTestTrial.ResultResponseType!")
                End Select


            Next

        End If

        'Sets the selection row
        'Clears any selection first
        TestTrialDataGridView.ClearSelection()
        If SelectionRow.HasValue Then
            'Selects the first column of the SelectionRow
            TestTrialDataGridView.Rows(SelectionRow).Cells(0).Selected = True
        End If

        'Scrolls to the indicated FirstRowToDisplayInScrollmode
        If FirstRowToDisplayInScrollmode.HasValue Then
            TestTrialDataGridView.FirstDisplayedScrollingRowIndex = FirstRowToDisplayInScrollmode
        End If

    End Sub

#End Region


    Public Sub ClearTestNameBox()
        TestDescriptionTextBox.Text = ""
    End Sub

    Public Sub LockTestNameBox()
        TestDescriptionTextBox.ReadOnly = True
    End Sub

    Public Sub UnlockTestNameBox()
        TestDescriptionTextBox.ReadOnly = False
    End Sub

    Private Sub LockSettingsPanels()

        TestSettings_TableLayoutPanel.Enabled = False
        LockTestNameBox()

    End Sub

    Private Sub UnlockSettingsPanels()

        TestSettings_TableLayoutPanel.Enabled = True
        UnlockTestNameBox()

    End Sub


#Region "Test-result comparison"

    Public Sub PopulateTestHistoryTables()

        'Clears all rows in the TestHistoryTables
        CurrentSessionResults_DataGridView.Rows.Clear()

        'Adds rows
        CurrentSessionResults_DataGridView.Rows.Add(TestHistorySummary.Measurements.Count)

        'Adds data
        For r = 0 To TestHistorySummary.Measurements.Count - 1
            CurrentSessionResults_DataGridView.Rows(r).Cells(0).Value = TestHistorySummary.Measurements(r).Description
            CurrentSessionResults_DataGridView.Rows(r).Cells(1).Value = TestHistorySummary.Measurements(r).TestLength
            CurrentSessionResults_DataGridView.Rows(r).Cells(2).Value = TestHistorySummary.Measurements(r).PercentCorrect
            CurrentSessionResults_DataGridView.Rows(r).Cells(3).Value = False 'Setting selected value to false by default
        Next

    End Sub

    Public Sub UpdateSignificanceTestResult(Result As String)

        SignificanceTestResultLabel.Text = Result

        'Also showing/hiding the StatAnalysisLabel depending on the information in Result
        If Result = "" Then
            StatAnalysisLabel.Visible = False
        Else
            StatAnalysisLabel.Visible = True
        End If

    End Sub


#End Region


#Region "Test-result comparison: Handling of local events"

    Private TestComparisonHistory As New List(Of String)

    Private Sub CurrentSessionResults_DataGridView_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles CurrentSessionResults_DataGridView.CurrentCellDirtyStateChanged

        'This extra event handler is needed since the CellValueChanged event does not always trigger for DataGridViewCheckBoxCells. See https://stackoverflow.com/questions/11843488/how-to-detect-datagridview-checkbox-event-change for this solution
        Dim Result = TryCast(sender.CurrentCell, DataGridViewCheckBoxCell)
        If Result IsNot Nothing Then
            sender.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If

    End Sub

    Private Sub SessionResults_DataGridView_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles CurrentSessionResults_DataGridView.CellValueChanged

        'Exits sub if invalid indices are sent
        If e.RowIndex < 0 Then Exit Sub
        If e.ColumnIndex < 0 Then Exit Sub
        If e.RowIndex > sender.Rows.count - 1 Then Exit Sub
        If e.ColumnIndex > sender.Columns.count - 1 Then Exit Sub

        'Ignores any calls that do not come from the third column (i.e. the check-box column)
        If e.ColumnIndex <> 3 Then Exit Sub

        'Adding/removing the appropriate test GuiDescriptions to/from the TestComparisonHistory.
        If sender.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = True Then

            'Checks to see whether to add the test to the TestComparisonHistory and perform a significance test

            'Updating the testcomparison history with the description string of checked test
            If Not TestComparisonHistory.Contains(CurrentSessionResults_DataGridView.Rows(e.RowIndex).Cells(0).Value) Then
                TestComparisonHistory.Add(CurrentSessionResults_DataGridView.Rows(e.RowIndex).Cells(0).Value)
            End If

            'Removing anything but the two last items in TestComparisonHistory
            If TestComparisonHistory.Count > 2 Then
                TestComparisonHistory.RemoveRange(0, TestComparisonHistory.Count - 2)
            End If

            'Updating the checkboxes in both the CurrentSessionResults_DataGridView and the PreviousSessionsResults_DataGridView based on the last two selected values
            For r = 0 To CurrentSessionResults_DataGridView.Rows.Count - 1
                If TestComparisonHistory.Contains(CurrentSessionResults_DataGridView.Rows(r).Cells(0).Value) Then
                    CurrentSessionResults_DataGridView.Rows(r).Cells(3).Value = True
                Else
                    CurrentSessionResults_DataGridView.Rows(r).Cells(3).Value = False
                End If
            Next

        Else

            'Removes the test from testcomparison if it is there as the test was unchecked (This need to loop because Remove only removes the first occurence...)
            Do Until TestComparisonHistory.Contains(CurrentSessionResults_DataGridView.Rows(e.RowIndex).Cells(0).Value) = False
                TestComparisonHistory.Remove(CurrentSessionResults_DataGridView.Rows(e.RowIndex).Cells(0).Value)
            Loop

        End If

        'Sending a call for statistical analysis to the backend. 
        CompareTwoSipTestScores(TestComparisonHistory)

    End Sub


#End Region


#Region "BlueToothConnection"

    Public Sub UpdateBluetoothConnectionStatusIndicator(ConnectionOk As Boolean)
        Throw New NotImplementedException()
    End Sub

    Public Sub PopulateBluetoothDevicesList(DeviceDescriptions As List(Of String))
        Throw New NotImplementedException()
    End Sub

    Public Sub EnableBluetoothDevicesList()
        Throw New NotImplementedException()
    End Sub

    Public Sub DisableBluetoothDevicesList()
        Throw New NotImplementedException()
    End Sub

    Public Sub EnableBluetoothSearchButton()
        Throw New NotImplementedException()
    End Sub

    Public Sub DisableBluetoothSearchButton()
        Throw New NotImplementedException()
    End Sub

    Public Sub EnableBluetoothDeviceSelectionButton()
        Throw New NotImplementedException()
    End Sub

    Public Sub DisableBluetoothDeviceSelectionButton()
        Throw New NotImplementedException()
    End Sub

#End Region


#Region "BlueToothConnection: Handling of local events"

#End Region

#Region "SoundDevice"

    Public Sub UpdateSoundDeviceConnectionStatusIndicator(ConnectionOk As Boolean)
        Throw New NotImplementedException()
    End Sub

    Public Sub PopulateSoundDevicesList(DeviceDescriptions As List(Of String))
        Throw New NotImplementedException()
    End Sub

    Public Sub EnableSoundDevicesList()
        Throw New NotImplementedException()
    End Sub

    Public Sub DisableSoundDevicesList()
        Throw New NotImplementedException()
    End Sub

    Public Sub EnableSoundDeviceSearchButton()
        Throw New NotImplementedException()
    End Sub

    Public Sub DisableSoundDeviceSearchButton()
        Throw New NotImplementedException()
    End Sub

    Public Sub EnableSoundDeviceSelectionButton()
        Throw New NotImplementedException()
    End Sub

    Public Sub DisableSoundDeviceSelectionButton()
        Throw New NotImplementedException()
    End Sub

#End Region


#Region "SoundDevice: Handling of local events"

#End Region


#Region "MessageBoxes"


    ''' <summary>
    ''' This method can be called by the backend in order to display a message box message to the user.
    ''' </summary>
    ''' <param name="Message"></param>
    Public Sub ShowMessageBox(Message As String, Optional ByVal Title As String = "SiP-testet")

        MsgBox(Message, MsgBoxStyle.Information, Title)

    End Sub

    Public Function ShowYesNoMessageBox(Question As String, Optional Title As String = "SiP-testet") As Boolean

        Dim Result = MsgBox(Question, MsgBoxStyle.YesNo, Title)

        If Result = MsgBoxResult.Yes Then
            Return True
        Else
            Return False
        End If

    End Function



#End Region




    Private Function LookForCalibrationData() As SortedList(Of Integer, Double)

        MsgBox("Here we should look for calibration data and offer some options...")

        Dim CalibrationData As SortedList(Of Integer, Double) = Nothing

        Return CalibrationData

    End Function

#Region "ImportExport"

    Public Sub SaveFileButtonPressed() Handles SaveButton.Click
        Throw New NotImplementedException()
    End Sub

    Public Sub OpenFileButtonPressed() Handles OpenButton.Click
        Throw New NotImplementedException()
    End Sub

    Public Sub ExportDataButtonPressed() Handles ExportButton.Click
        Throw New NotImplementedException()
    End Sub

#End Region




#Region "Test-result comparison"


    ''' <summary>
    ''' Performs a statistical analysis of the score difference between the SiP-testet refered to in ComparedMeasurementGuiDescriptions by their GuiDescription strings
    ''' </summary>
    ''' <param name="ComparedMeasurementGuiDescriptions"></param>
    Sub CompareTwoSipTestScores(ByRef ComparedMeasurementGuiDescriptions As List(Of String))

        'Clears the Gui significance test result box if not exaclty two measurements descriptions are recieved. And the exits the sub
        If ComparedMeasurementGuiDescriptions.Count = 2 Then

            Dim SummariesToCompare As New List(Of SipTestSummary)
            For Each Summary In TestHistorySummary.Measurements
                If ComparedMeasurementGuiDescriptions.Contains(Summary.Description) Then
                    SummariesToCompare.Add(Summary)
                End If
            Next

            MsgBox("Implement BPAC!")
            Dim Result = CriticalDifferences.IsNotSignificantlyDifferent(SummariesToCompare(0).TestLength, SummariesToCompare(1).TestLength,
                                                                         SummariesToCompare(0).AverageScore, SummariesToCompare(1).AverageScore,
                                                                         0.95, True)


            Dim Result = CriticalDifferences.IsNotSignificantlyDifferent(SummariesToCompare(0).TestLength, SummariesToCompare(1).TestLength,
                                                                         SummariesToCompare(0).AverageScore, SummariesToCompare(1).AverageScore,
                                                                         0.95, True)
            If Result = False Then
                'Significant
                UpdateSignificanceTestResult("The difference is statistically significant (p < 0.05)")
            Else
                'Not significant
                UpdateSignificanceTestResult("The difference is NOT statistically significant (p < 0.05)")
            End If

        End If

    End Sub


#End Region


#Region "BlueToothConnection"


    Public Sub SearchForBluetoothDevices()
        Throw New NotImplementedException()
    End Sub

    Public Sub SelectBluetoothDevice(SelectedBluetoothDeviceDescription As String)
        Throw New NotImplementedException()
    End Sub

#End Region

#Region "SoundDevice"

    Public Sub SearchForSoundDevices()
        Throw New NotImplementedException()
    End Sub

    Public Sub SelectSoundDevice(SelectedSoundDeviceDescription As String)
        Throw New NotImplementedException()
    End Sub

#End Region



#Region "ExperimentalStuff"

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






#End Region





End Class


