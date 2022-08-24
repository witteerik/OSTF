Imports SpeechTestFramework
Imports SpeechTestFramework.SipTest
Imports SpeechTestFramework.WinFormControls
Imports System.Windows.Forms
Imports System.Drawing

Public Class SipTestGui
    Implements ISipGui

    Private Audiogram As Audiogram
    Private GainDiagram As GainDiagram
    Private ExpectedScoreDiagram As PsychometricFunctionDiagram

    Private _SipTestBackend As New ModuleBackend(Me)

    Property SipTestBackend As IModuleBackend Implements ISipGui.SipTestBackend
        Get
            Return _SipTestBackend
        End Get
        Set(value As IModuleBackend)
            _SipTestBackend = value
        End Set
    End Property


    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub SipTestGui_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        SipTestBackend.InitializeModuleBackendComponents()

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


    End Sub


    Private Sub Test(sender As Object, e As EventArgs) Handles Label1.Click

        Dim t = New TestHistoryListData
        t.CurrentTestSessionData.Add(New TestHistoryListData.SipTestMeasurementGuiDescription("Test1", 30, "76%"))
        t.CurrentTestSessionData.Add(New TestHistoryListData.SipTestMeasurementGuiDescription("Test2", 40, "77%"))
        t.CurrentTestSessionData.Add(New TestHistoryListData.SipTestMeasurementGuiDescription("Test3", 50, "78%"))

        t.PreviousTestSessionData.Add(New TestHistoryListData.SipTestMeasurementGuiDescription("Test4", 20, "26%"))
        t.PreviousTestSessionData.Add(New TestHistoryListData.SipTestMeasurementGuiDescription("Test5", 10, "27%"))
        t.PreviousTestSessionData.Add(New TestHistoryListData.SipTestMeasurementGuiDescription("Test6", 3, "23%"))

        PopulateTestHistoryTables(t)

        'Exit Sub


        Dim t1 As New List(Of String)
        For n = 0 To 49
            t1.Add("ord" & n + 1)
        Next

        Dim t2(t1.Count - 1) As String
        Dim t3(t1.Count - 1) As SipTest.ResultResponseType

        For n = 0 To 49
            If n < 5 Then
                t2(n) = "ord" & n + 1
            End If
        Next

        t3(0) = SipTest.ResultResponseType.Correct
        t3(1) = SipTest.ResultResponseType.Correct
        t3(2) = SipTest.ResultResponseType.Incorrect
        t2(2) = "ordX"
        t3(3) = SipTest.ResultResponseType.Correct
        t3(4) = SipTest.ResultResponseType.Correct


        UpdateTestTrialTable(t1.ToArray, t2, t3, , 5)

        'UpdateTestTrialTable({"Hej", "Ord2", "Ord3"}, {"Då", "Ord2", ""}, {SipTestTrial.ResultResponseType.Correct, SipTestTrial.ResultResponseType.Incorrect, SipTestTrial.ResultResponseType.NotPresented})
        'UpdateTestTrialTable({"Hej", "Ord2", "Ord3"}, {"Då", "Ord2", ""}, {SipTestTrial.ResultResponseType.Correct, SipTestTrial.ResultResponseType.Incorrect, SipTestTrial.ResultResponseType.NotPresented})

        Me.Update()

        'Threading.Thread.Sleep(2000)

        'UpdateTestTrialTable(t1.ToArray, t2, t3, , 25, 20)


        'UpdateTestTrialTable({"Hej", "Ord2", "Ord3"}, {"Då", "Ord2", ""}, {SipTestTrial.ResultResponseType.Correct, SipTestTrial.ResultResponseType.Incorrect, SipTestTrial.ResultResponseType.NotPresented})
        'Me.Update()

        'Threading.Thread.Sleep(2000)

        'UpdateTestTrialTable({"Hej", "Ord6", "Ord3"}, {"Då", "Ord2", ""}, {SipTestTrial.ResultResponseType.Correct, SipTestTrial.ResultResponseType.Incorrect, SipTestTrial.ResultResponseType.NotPresented}, 1)

        'Me.Update()

        'Threading.Thread.Sleep(2000)

        'UpdateTestTrialTable({"Hej", "Ord6", "Ord3"}, {"Då", "Ord2", ""}, {SipTestTrial.ResultResponseType.Correct, SipTestTrial.ResultResponseType.Incorrect, SipTestTrial.ResultResponseType.NotPresented}, 1, 0)

        'Me.Update()

        'Threading.Thread.Sleep(2000)

        'UpdateTestTrialTable({"Hej", "Ord6", "Ord3"}, {"Då", "Ord2", ""}, {SipTestTrial.ResultResponseType.Correct, SipTestTrial.ResultResponseType.Incorrect, SipTestTrial.ResultResponseType.NotPresented}, 1, 2)

    End Sub


#Region "ImportExport: ISipGui implementations"

    Public Sub LockPatientDetails(SocialSecurityNumber As String, FirstName As String, LastName As String) Implements ISipGui.LockPatientDetails

        SSNumber1TextBox.ReadOnly = True
        SSNumber2TextBox.ReadOnly = True

        SSNumber1TextBox.Text = SocialSecurityNumber.Substring(0, 8)
        SSNumber2TextBox.Text = SocialSecurityNumber.Substring(8, 4)

        PatientSearchButton.Enabled = False

        FirstNameTextBox.Text = FirstName
        LastNameTextBox.Text = LastName

    End Sub

    Public Sub EnableSaveButton() Implements ISipGui.EnableSaveButton
        Throw New NotImplementedException()
    End Sub

    Public Sub DisableSaveButton() Implements ISipGui.DisableSaveButton
        Throw New NotImplementedException()
    End Sub

    Public Sub EnableOpenButton() Implements ISipGui.EnableOpenButton
        Throw New NotImplementedException()
    End Sub

    Public Sub DisableOpenButton() Implements ISipGui.DisableOpenButton
        Throw New NotImplementedException()
    End Sub

    Public Sub EnableExportButton() Implements ISipGui.EnableExportButton
        Throw New NotImplementedException()
    End Sub

    Public Sub DisableExportButton() Implements ISipGui.DisableExportButton
        Throw New NotImplementedException()
    End Sub

    Public Function GetOpenFileDialogResult(Title As String, Optional Extension As String = "") As Object Implements ISipGui.GetOpenFileDialogResult
        Throw New NotImplementedException()
    End Function

    Public Function GetSaveFileDialogResult(Title As String, Optional Extension As String = "") As String Implements ISipGui.GetSaveFileDialogResult
        Throw New NotImplementedException()
    End Function



#End Region

#Region "ImportExport: Handling of local events"
    Private Sub PatientSearchButton_Click(sender As Object, e As EventArgs) Handles PatientSearchButton.Click
        SipTestBackend.SearchPatient(SSNumber1TextBox.Text & SSNumber2TextBox.Text)
    End Sub


#End Region

#Region "Measurement settings: ISipGui implementations"

    Public Sub PopulateAudiogramList(ByRef Audiograms As List(Of AudiogramData), Optional SelectedIndex As Integer = -1) Implements ISipGui.PopulateAudiogramList
        AudiogramComboBox.Items.Clear()
        AudiogramComboBox.Items.AddRange(Audiograms.ToArray)
        If SelectedIndex <> -1 Then
            AudiogramComboBox.SelectedIndex = SelectedIndex
        End If
    End Sub

    Public Sub DisplaySelectedAudiogram(ByRef AudiogramData As AudiogramData) Implements ISipGui.DisplaySelectedAudiogram

        Audiogram.AudiogramData = AudiogramData

    End Sub

    Public Sub PopulateReferenceLevelList(AvailableReferenceLevels As List(Of Double), SelectedIndex As Integer) Implements ISipGui.PopulateReferenceLevelList

        ReferenceLevelComboBox.Items.Clear()
        For Each RefLevel In AvailableReferenceLevels
            ReferenceLevelComboBox.Items.Add(RefLevel)
        Next
        ReferenceLevelComboBox.SelectedIndex = SelectedIndex

    End Sub

    Public Sub PopulateHearingAidGainTypeList(AvailableGainTypes As List(Of HearingAidGainData.GainTypes), SelectedIndex As Integer) Implements ISipGui.PopulateHearingAidGainTypeList

        GainTypeComboBox.Items.Clear()
        For Each GainType In AvailableGainTypes
            GainTypeComboBox.Items.Add(GainType)
        Next
        GainTypeComboBox.SelectedIndex = SelectedIndex

    End Sub

    Public Sub DisplayHearingAidGain(ByRef HearingAidGain As HearingAidGainData) Implements ISipGui.DisplayHearingAidGain

        GainDiagram.UpdateGainValues(HearingAidGain)

    End Sub

    Public Sub PopulatePresetList(AvailablePresets As List(Of String), SelectedIndex As Integer) Implements ISipGui.PopulatePresetList

        PresetComboBox.Items.Clear()
        For Each Preset In AvailablePresets
            PresetComboBox.Items.Add(Preset)
        Next
        PresetComboBox.SelectedIndex = SelectedIndex

    End Sub

    Public Sub PopulateTestSituationList(AvailableSituations As List(Of String), SelectedIndex As Integer?) Implements ISipGui.PopulateTestSituationList

        TestSituationComboBox.Items.Clear()
        For Each Value In AvailableSituations
            TestSituationComboBox.Items.Add(Value)
        Next
        If SelectedIndex.HasValue Then
            TestSituationComboBox.SelectedIndex = SelectedIndex
        End If

    End Sub

    Public Sub PopulateTestLengthList(AvailableTestLengths As List(Of Integer), SelectedIndex As Integer) Implements ISipGui.PopulateTestLengthList

        TestLengthComboBox.Items.Clear()
        For Each TestLength In AvailableTestLengths
            TestLengthComboBox.Items.Add(TestLength)
        Next
        TestLengthComboBox.SelectedIndex = SelectedIndex

    End Sub

    Public Sub DisplayPredictedPsychometricCurve(PNRs() As Single, PredictedScores() As Single, LowerCiLimits() As Single, UpperCiLimits() As Single) Implements ISipGui.DisplayPredictedPsychometricCurve

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

    Public Sub PopulatePnrList(AvailablePNRs As List(Of Double), SelectedIndex As Integer) Implements ISipGui.PopulatePnrList

        PnrComboBox.Items.Clear()
        For Each Pnr In AvailablePNRs
            PnrComboBox.Items.Add(Pnr)
        Next
        PnrComboBox.SelectedIndex = SelectedIndex

    End Sub

    Public Sub ClearTestNameBox() Implements ISipGui.ClearTestNameBox
        TestDescriptionTextBox.Text = ""
    End Sub

    Public Sub LockTestNameBox() Implements ISipGui.LockTestNameBox
        TestDescriptionTextBox.ReadOnly = True
    End Sub

    Public Sub UnlockTestNameBox() Implements ISipGui.UnlockTestNameBox
        TestDescriptionTextBox.ReadOnly = False
    End Sub

#End Region

#Region "Measurement settings: Handling of local events"

    Private Sub AudiogramComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles AudiogramComboBox.SelectedIndexChanged

        SipTestBackend.SelectAudiogram(AudiogramComboBox.SelectedItem)
    End Sub

    Private Sub ReferenceLevelComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ReferenceLevelComboBox.SelectedIndexChanged

        SipTestBackend.SelectReferenceLevel(ReferenceLevelComboBox.SelectedItem)

    End Sub

    Private Sub GainTypeComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles GainTypeComboBox.SelectedIndexChanged

        SipTestBackend.SelectHearingAidGainType(GainTypeComboBox.SelectedItem)

    End Sub

    Private Sub PresetComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles PresetComboBox.SelectedIndexChanged

        SipTestBackend.SelectPreset(PresetComboBox.SelectedItem)

    End Sub

    Private Sub VoiceComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TestSituationComboBox.SelectedIndexChanged

        SipTestBackend.SelectSituation(TestSituationComboBox.SelectedItem)

    End Sub

    Private Sub TestLengthComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TestLengthComboBox.SelectedIndexChanged

        SipTestBackend.SelectTestLength(TestLengthComboBox.SelectedItem)

    End Sub

    Private Sub PnrComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles PnrComboBox.SelectedIndexChanged

        SipTestBackend.SelectPNR(PnrComboBox.SelectedItem)

    End Sub


#End Region

#Region "Active measurement: ISipGui implementations"


    Public Sub SubEnablePlayButton() Implements ISipGui.SubEnablePlayButton
        StartButton.Enabled = True
        StartButton.BackgroundImage = My.Resources.PlayImage
    End Sub

    Public Sub SubDisableStopButton() Implements ISipGui.SubDisableStopButton
        StopButton.Enabled = False
        StopButton.BackgroundImage = My.Resources.StopDisabledImage
    End Sub

    Public Sub SubEnableStopButton() Implements ISipGui.SubEnableStopButton
        StopButton.Enabled = True
        StopButton.BackgroundImage = My.Resources.StopImage
    End Sub

    Public Sub SubDisablePlayButton() Implements ISipGui.SubDisablePlayButton
        StartButton.Enabled = False
        StartButton.BackgroundImage = My.Resources.PlayDisabledImage
    End Sub

    Public Sub TogglePlayButton(PlayMode As Boolean) Implements ISipGui.TogglePlayButton
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

    Public Sub UpdateTestProgress(Max As Integer, Progress As Integer, Correct As Integer, ProportionCorrect As String) Implements ISipGui.UpdateTestProgress
        MeasurementProgressBar.Minimum = 0
        MeasurementProgressBar.Maximum = Max
        MeasurementProgressBar.Value = Progress

        CorrectCountTextBox.Text = Correct & " / " & Progress
        ProportionCorrectTextBox.Text = ProportionCorrect
    End Sub

    Public Sub UpdateTestTrialTable(ByVal TestWords() As String, ByVal Responses() As String, ByVal ResultResponseTypes() As SipTest.ResultResponseType,
                             Optional ByVal UpdateRow As Integer? = Nothing, Optional SelectionRow As Integer? = Nothing, Optional FirstRowToDisplayInScrollmode As Integer? = Nothing) Implements ISipGui.UpdateTestTrialTable

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

#Region "Active measurement: Handling of local events"
    Private Sub StartButton_Click(sender As Object, e As EventArgs) Handles StartButton.Click
        SipTestBackend.StartButtonPressed()
    End Sub

    Private Sub StopButton_Click(sender As Object, e As EventArgs) Handles StopButton.Click
        SipTestBackend.StopButtonPressed()
    End Sub


#End Region


#Region "Test-result comparison: ISipGui implementations"

    Public Sub PopulateTestHistoryTables(ByRef TestHistoryListData As TestHistoryListData) Implements ISipGui.PopulateTestHistoryTables

        'Clears all rows in the TestHistoryTables
        CurrentSessionResults_DataGridView.Rows.Clear()
        PreviousSessionsResults_DataGridView.Rows.Clear()

        'Adds rows
        CurrentSessionResults_DataGridView.Rows.Add(TestHistoryListData.CurrentTestSessionData.Count)
        PreviousSessionsResults_DataGridView.Rows.Add(TestHistoryListData.PreviousTestSessionData.Count)

        'Adds data
        For r = 0 To TestHistoryListData.CurrentTestSessionData.Count - 1
            CurrentSessionResults_DataGridView.Rows(r).Cells(0).Value = TestHistoryListData.CurrentTestSessionData(r).GuiDescription
            CurrentSessionResults_DataGridView.Rows(r).Cells(1).Value = TestHistoryListData.CurrentTestSessionData(r).TestLength
            CurrentSessionResults_DataGridView.Rows(r).Cells(2).Value = TestHistoryListData.CurrentTestSessionData(r).PercentCorrect
            CurrentSessionResults_DataGridView.Rows(r).Cells(3).Value = False 'Setting selected value to false by default
        Next

        For r = 0 To TestHistoryListData.PreviousTestSessionData.Count - 1
            PreviousSessionsResults_DataGridView.Rows(r).Cells(0).Value = TestHistoryListData.PreviousTestSessionData(r).GuiDescription
            PreviousSessionsResults_DataGridView.Rows(r).Cells(1).Value = TestHistoryListData.PreviousTestSessionData(r).TestLength
            PreviousSessionsResults_DataGridView.Rows(r).Cells(2).Value = TestHistoryListData.PreviousTestSessionData(r).PercentCorrect
            PreviousSessionsResults_DataGridView.Rows(r).Cells(3).Value = False 'Setting selected value to false by default
        Next

    End Sub

    Public Sub UpdateSignificanceTestResult(Result As String) Implements ISipGui.UpdateSignificanceTestResult

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

    Private Sub CurrentSessionResults_DataGridView_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles CurrentSessionResults_DataGridView.CurrentCellDirtyStateChanged, PreviousSessionsResults_DataGridView.CurrentCellDirtyStateChanged

        'This extra event handler is needed since the CellValueChanged event does not always trigger for DataGridViewCheckBoxCells. See https://stackoverflow.com/questions/11843488/how-to-detect-datagridview-checkbox-event-change for this solution
        Dim Result = TryCast(sender.CurrentCell, DataGridViewCheckBoxCell)
        If Result IsNot Nothing Then
            sender.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If

    End Sub

    Private Sub SessionResults_DataGridView_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles CurrentSessionResults_DataGridView.CellValueChanged, PreviousSessionsResults_DataGridView.CellValueChanged

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

            For r = 0 To PreviousSessionsResults_DataGridView.Rows.Count - 1
                If TestComparisonHistory.Contains(PreviousSessionsResults_DataGridView.Rows(r).Cells(0).Value) Then
                    PreviousSessionsResults_DataGridView.Rows(r).Cells(3).Value = True
                Else
                    PreviousSessionsResults_DataGridView.Rows(r).Cells(3).Value = False
                End If
            Next

        Else

            'Removes the test from testcomparison if it is there as the test was unchecked (This need to loop because Remove only removes the first occurence...)
            Do Until TestComparisonHistory.Contains(CurrentSessionResults_DataGridView.Rows(e.RowIndex).Cells(0).Value) = False
                TestComparisonHistory.Remove(CurrentSessionResults_DataGridView.Rows(e.RowIndex).Cells(0).Value)
            Loop

        End If

        'Sending a call for statistical analysis to the backend. 
        SipTestBackend.CompareTwoSipTestScores(TestComparisonHistory)

    End Sub


#End Region


#Region "BlueToothConnection: ISipGui implementations"

    Public Sub UpdateBluetoothConnectionStatusIndicator(ConnectionOk As Boolean) Implements ISipGui.UpdateBluetoothConnectionStatusIndicator
        Throw New NotImplementedException()
    End Sub

    Public Sub PopulateBluetoothDevicesList(DeviceDescriptions As List(Of String)) Implements ISipGui.PopulateBluetoothDevicesList
        Throw New NotImplementedException()
    End Sub

    Public Sub EnableBluetoothDevicesList() Implements ISipGui.EnableBluetoothDevicesList
        Throw New NotImplementedException()
    End Sub

    Public Sub DisableBluetoothDevicesList() Implements ISipGui.DisableBluetoothDevicesList
        Throw New NotImplementedException()
    End Sub

    Public Sub EnableBluetoothSearchButton() Implements ISipGui.EnableBluetoothSearchButton
        Throw New NotImplementedException()
    End Sub

    Public Sub DisableBluetoothSearchButton() Implements ISipGui.DisableBluetoothSearchButton
        Throw New NotImplementedException()
    End Sub

    Public Sub EnableBluetoothDeviceSelectionButton() Implements ISipGui.EnableBluetoothDeviceSelectionButton
        Throw New NotImplementedException()
    End Sub

    Public Sub DisableBluetoothDeviceSelectionButton() Implements ISipGui.DisableBluetoothDeviceSelectionButton
        Throw New NotImplementedException()
    End Sub

#End Region


#Region "BlueToothConnection: Handling of local events"

#End Region

#Region "SoundDevice: ISipGui implementations"

    Public Sub UpdateSoundDeviceConnectionStatusIndicator(ConnectionOk As Boolean) Implements ISipGui.UpdateSoundDeviceConnectionStatusIndicator
        Throw New NotImplementedException()
    End Sub

    Public Sub PopulateSoundDevicesList(DeviceDescriptions As List(Of String)) Implements ISipGui.PopulateSoundDevicesList
        Throw New NotImplementedException()
    End Sub

    Public Sub EnableSoundDevicesList() Implements ISipGui.EnableSoundDevicesList
        Throw New NotImplementedException()
    End Sub

    Public Sub DisableSoundDevicesList() Implements ISipGui.DisableSoundDevicesList
        Throw New NotImplementedException()
    End Sub

    Public Sub EnableSoundDeviceSearchButton() Implements ISipGui.EnableSoundDeviceSearchButton
        Throw New NotImplementedException()
    End Sub

    Public Sub DisableSoundDeviceSearchButton() Implements ISipGui.DisableSoundDeviceSearchButton
        Throw New NotImplementedException()
    End Sub

    Public Sub EnableSoundDeviceSelectionButton() Implements ISipGui.EnableSoundDeviceSelectionButton
        Throw New NotImplementedException()
    End Sub

    Public Sub DisableSoundDeviceSelectionButton() Implements ISipGui.DisableSoundDeviceSelectionButton
        Throw New NotImplementedException()
    End Sub

#End Region


#Region "SoundDevice: Handling of local events"

#End Region


#Region "MessageBoxes: ISipGui implementations"


    ''' <summary>
    ''' This method can be called by the backend in order to display a message box message to the user.
    ''' </summary>
    ''' <param name="Message"></param>
    Public Sub ShowMessageBox(Message As String, Optional ByVal Title As String = "SiP-testet") Implements ISipGui.ShowMessageBox

        MsgBox(Message, MsgBoxStyle.Information, Title)

    End Sub

    Public Function ShowYesNoMessageBox(Question As String, Optional Title As String = "SiP-testet") As Boolean Implements ISipGui.ShowYesNoMessageBox

        Dim Result = MsgBox(Question, MsgBoxStyle.YesNo, Title)

        If Result = MsgBoxResult.Yes Then
            Return True
        Else
            Return False
        End If

    End Function













#End Region


End Class


