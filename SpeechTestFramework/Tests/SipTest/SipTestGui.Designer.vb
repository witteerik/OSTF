<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class SipTestGui
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle8 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SipTestGui))
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.Test_TableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.TableLayoutPanel5 = New System.Windows.Forms.TableLayoutPanel()
        Me.TableLayoutPanel8 = New System.Windows.Forms.TableLayoutPanel()
        Me.StatAnalysisLabel = New System.Windows.Forms.Label()
        Me.SignificanceTestResult_RichTextBox = New System.Windows.Forms.RichTextBox()
        Me.TableLayoutPanel9 = New System.Windows.Forms.TableLayoutPanel()
        Me.ExportData_Button = New System.Windows.Forms.Button()
        Me.ImportData_Button = New System.Windows.Forms.Button()
        Me.CompletedTests_Label = New System.Windows.Forms.Label()
        Me.CurrentSessionResults_DataGridView = New System.Windows.Forms.DataGridView()
        Me.TestDescriptionColumnSession = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TestLengthColumnSession = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ResultColumnSession = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.CompareColumnSession = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.TestSettings_TableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.TestLength_Label = New System.Windows.Forms.Label()
        Me.Gain_Label = New System.Windows.Forms.Label()
        Me.SelectAudiogram_Label = New System.Windows.Forms.Label()
        Me.ExpectedScorePanel = New System.Windows.Forms.Panel()
        Me.GainPanel = New System.Windows.Forms.Panel()
        Me.AudiogramPanel = New System.Windows.Forms.Panel()
        Me.LengthReduplications_Label = New System.Windows.Forms.Label()
        Me.PsychmetricFunction_VerticalLabel = New SpeechTestFramework.WinFormControls.VerticalLabel()
        Me.Audiogram_VerticalLabel = New SpeechTestFramework.WinFormControls.VerticalLabel()
        Me.Gain_VerticalLabel = New SpeechTestFramework.WinFormControls.VerticalLabel()
        Me.AudiogramComboBox = New System.Windows.Forms.ComboBox()
        Me.HaGainComboBox = New System.Windows.Forms.ComboBox()
        Me.ReferenceLevel_Label = New System.Windows.Forms.Label()
        Me.ReferenceLevelComboBox = New System.Windows.Forms.ComboBox()
        Me.TableLayoutPanel10 = New System.Windows.Forms.TableLayoutPanel()
        Me.AddTypicalAudiograms_Button = New System.Windows.Forms.Button()
        Me.NewAudiogram_Button = New System.Windows.Forms.Button()
        Me.TableLayoutPanel11 = New System.Windows.Forms.TableLayoutPanel()
        Me.CreateNewGain_Button = New System.Windows.Forms.Button()
        Me.AddFig6Gain_Button = New System.Windows.Forms.Button()
        Me.PNR_Label = New System.Windows.Forms.Label()
        Me.PnrComboBox = New System.Windows.Forms.ComboBox()
        Me.Preset_Label = New System.Windows.Forms.Label()
        Me.PresetComboBox = New System.Windows.Forms.ComboBox()
        Me.Situation_Label = New System.Windows.Forms.Label()
        Me.TestSituationComboBox = New System.Windows.Forms.ComboBox()
        Me.MostDifficultItems_Button = New System.Windows.Forms.Button()
        Me.PlannedTestLength_TextBox = New System.Windows.Forms.TextBox()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.Testparadigm_ComboBox = New System.Windows.Forms.ComboBox()
        Me.Testparadigm_Label = New System.Windows.Forms.Label()
        Me.TestLengthComboBox = New System.Windows.Forms.ComboBox()
        Me.TableLayoutPanel4 = New System.Windows.Forms.TableLayoutPanel()
        Me.ProportionCorrectTextBox = New System.Windows.Forms.TextBox()
        Me.MeasurementProgressBar = New System.Windows.Forms.ProgressBar()
        Me.TestTrialDataGridView = New System.Windows.Forms.DataGridView()
        Me.TestWordColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ResponseColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ResultColumn = New System.Windows.Forms.DataGridViewImageColumn()
        Me.CorrectCount_Label = New System.Windows.Forms.Label()
        Me.ProportionCorrect_Label = New System.Windows.Forms.Label()
        Me.Start_AudioButton = New SpeechTestFramework.WinFormControls.AudioButton()
        Me.CorrectCountTextBox = New System.Windows.Forms.TextBox()
        Me.TestDescriptionTextBox = New System.Windows.Forms.TextBox()
        Me.TestDescription_Label = New System.Windows.Forms.Label()
        Me.RandomSeed_Label = New System.Windows.Forms.Label()
        Me.RandomSeed_IntegerParsingTextBox = New SpeechTestFramework.IntegerParsingTextBox()
        Me.Stop_AudioButton = New SpeechTestFramework.WinFormControls.AudioButton()
        Me.KeyboardShortcutContainer_Panel = New System.Windows.Forms.Panel()
        Me.KeybordShortcut_TableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.Pause_Label = New System.Windows.Forms.Label()
        Me.Resume_Label = New System.Windows.Forms.Label()
        Me.Stop_Label = New System.Windows.Forms.Label()
        Me.KeyBoardShortcut_Label = New System.Windows.Forms.Label()
        Me.TableLayoutPanel16 = New System.Windows.Forms.TableLayoutPanel()
        Me.ParticipantID_Label = New System.Windows.Forms.Label()
        Me.ParticipantIdTextBox = New System.Windows.Forms.TextBox()
        Me.Screen_TableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.PcScreen_RadioButton = New System.Windows.Forms.RadioButton()
        Me.BtScreen_RadioButton = New System.Windows.Forms.RadioButton()
        Me.PcScreen_TableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.PcScreen_ComboBox = New System.Windows.Forms.ComboBox()
        Me.PcTouch_CheckBox = New System.Windows.Forms.CheckBox()
        Me.BtScreen_TableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.ConnectBluetoothScreen_Button = New System.Windows.Forms.Button()
        Me.DisconnectBtScreen_Button = New System.Windows.Forms.Button()
        Me.BtLamp = New SpeechTestFramework.Lamp()
        Me.ParticipantLock_Button = New System.Windows.Forms.Button()
        Me.SoundDevice_Panel = New System.Windows.Forms.Panel()
        Me.SoundSettings_TableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.SelectTransducer_Label = New System.Windows.Forms.Label()
        Me.Transducer_ComboBox = New System.Windows.Forms.ComboBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Operation_ProgressBarWithText = New SpeechTestFramework.ProgressBarWithText()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.Test_TableLayoutPanel.SuspendLayout()
        Me.TableLayoutPanel5.SuspendLayout()
        Me.TableLayoutPanel8.SuspendLayout()
        Me.TableLayoutPanel9.SuspendLayout()
        CType(Me.CurrentSessionResults_DataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TestSettings_TableLayoutPanel.SuspendLayout()
        Me.TableLayoutPanel10.SuspendLayout()
        Me.TableLayoutPanel11.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.TableLayoutPanel4.SuspendLayout()
        CType(Me.TestTrialDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.KeyboardShortcutContainer_Panel.SuspendLayout()
        Me.KeybordShortcut_TableLayoutPanel.SuspendLayout()
        Me.TableLayoutPanel16.SuspendLayout()
        Me.Screen_TableLayoutPanel.SuspendLayout()
        Me.PcScreen_TableLayoutPanel.SuspendLayout()
        Me.BtScreen_TableLayoutPanel.SuspendLayout()
        Me.SoundDevice_Panel.SuspendLayout()
        Me.SoundSettings_TableLayoutPanel.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 1324.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.Test_TableLayoutPanel, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.TableLayoutPanel16, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Panel1, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Operation_ProgressBarWithText, 0, 3)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 24)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 4
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 11.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(1324, 616)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'Test_TableLayoutPanel
        '
        Me.Test_TableLayoutPanel.ColumnCount = 3
        Me.Test_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 37.1517!))
        Me.Test_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.41796!))
        Me.Test_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.35294!))
        Me.Test_TableLayoutPanel.Controls.Add(Me.TableLayoutPanel5, 2, 0)
        Me.Test_TableLayoutPanel.Controls.Add(Me.TestSettings_TableLayoutPanel, 0, 0)
        Me.Test_TableLayoutPanel.Controls.Add(Me.TableLayoutPanel4, 1, 0)
        Me.Test_TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Test_TableLayoutPanel.Enabled = False
        Me.Test_TableLayoutPanel.Location = New System.Drawing.Point(3, 76)
        Me.Test_TableLayoutPanel.Name = "Test_TableLayoutPanel"
        Me.Test_TableLayoutPanel.RowCount = 1
        Me.Test_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.Test_TableLayoutPanel.Size = New System.Drawing.Size(1318, 511)
        Me.Test_TableLayoutPanel.TabIndex = 0
        '
        'TableLayoutPanel5
        '
        Me.TableLayoutPanel5.ColumnCount = 1
        Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel5.Controls.Add(Me.TableLayoutPanel8, 0, 2)
        Me.TableLayoutPanel5.Controls.Add(Me.TableLayoutPanel9, 0, 3)
        Me.TableLayoutPanel5.Controls.Add(Me.CompletedTests_Label, 0, 0)
        Me.TableLayoutPanel5.Controls.Add(Me.CurrentSessionResults_DataGridView, 0, 1)
        Me.TableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel5.Location = New System.Drawing.Point(894, 3)
        Me.TableLayoutPanel5.Name = "TableLayoutPanel5"
        Me.TableLayoutPanel5.RowCount = 4
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 137.0!))
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50.0!))
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel5.Size = New System.Drawing.Size(421, 505)
        Me.TableLayoutPanel5.TabIndex = 2
        '
        'TableLayoutPanel8
        '
        Me.TableLayoutPanel8.ColumnCount = 1
        Me.TableLayoutPanel8.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.71253!))
        Me.TableLayoutPanel8.Controls.Add(Me.StatAnalysisLabel, 0, 0)
        Me.TableLayoutPanel8.Controls.Add(Me.SignificanceTestResult_RichTextBox, 0, 1)
        Me.TableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel8.Location = New System.Drawing.Point(3, 321)
        Me.TableLayoutPanel8.Name = "TableLayoutPanel8"
        Me.TableLayoutPanel8.RowCount = 2
        Me.TableLayoutPanel8.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel8.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel8.Size = New System.Drawing.Size(415, 131)
        Me.TableLayoutPanel8.TabIndex = 2
        '
        'StatAnalysisLabel
        '
        Me.StatAnalysisLabel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.StatAnalysisLabel.Location = New System.Drawing.Point(3, 0)
        Me.StatAnalysisLabel.Name = "StatAnalysisLabel"
        Me.StatAnalysisLabel.Size = New System.Drawing.Size(409, 20)
        Me.StatAnalysisLabel.TabIndex = 0
        Me.StatAnalysisLabel.Text = "Statistik analys:"
        Me.StatAnalysisLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'SignificanceTestResult_RichTextBox
        '
        Me.SignificanceTestResult_RichTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SignificanceTestResult_RichTextBox.Location = New System.Drawing.Point(3, 23)
        Me.SignificanceTestResult_RichTextBox.Name = "SignificanceTestResult_RichTextBox"
        Me.SignificanceTestResult_RichTextBox.ReadOnly = True
        Me.SignificanceTestResult_RichTextBox.Size = New System.Drawing.Size(409, 105)
        Me.SignificanceTestResult_RichTextBox.TabIndex = 1
        Me.SignificanceTestResult_RichTextBox.Text = ""
        '
        'TableLayoutPanel9
        '
        Me.TableLayoutPanel9.ColumnCount = 3
        Me.TableLayoutPanel9.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel9.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel9.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel9.Controls.Add(Me.ExportData_Button, 0, 0)
        Me.TableLayoutPanel9.Controls.Add(Me.ImportData_Button, 1, 0)
        Me.TableLayoutPanel9.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel9.Location = New System.Drawing.Point(3, 458)
        Me.TableLayoutPanel9.Name = "TableLayoutPanel9"
        Me.TableLayoutPanel9.RowCount = 1
        Me.TableLayoutPanel9.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel9.Size = New System.Drawing.Size(415, 44)
        Me.TableLayoutPanel9.TabIndex = 3
        '
        'ExportData_Button
        '
        Me.ExportData_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ExportData_Button.Location = New System.Drawing.Point(9, 9)
        Me.ExportData_Button.Margin = New System.Windows.Forms.Padding(9)
        Me.ExportData_Button.Name = "ExportData_Button"
        Me.ExportData_Button.Size = New System.Drawing.Size(120, 26)
        Me.ExportData_Button.TabIndex = 0
        Me.ExportData_Button.Text = "Export data"
        Me.ExportData_Button.UseVisualStyleBackColor = True
        '
        'ImportData_Button
        '
        Me.ImportData_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ImportData_Button.Location = New System.Drawing.Point(147, 9)
        Me.ImportData_Button.Margin = New System.Windows.Forms.Padding(9)
        Me.ImportData_Button.Name = "ImportData_Button"
        Me.ImportData_Button.Size = New System.Drawing.Size(120, 26)
        Me.ImportData_Button.TabIndex = 1
        Me.ImportData_Button.Text = "Import data"
        Me.ImportData_Button.UseVisualStyleBackColor = True
        '
        'CompletedTests_Label
        '
        Me.CompletedTests_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CompletedTests_Label.Location = New System.Drawing.Point(3, 0)
        Me.CompletedTests_Label.Name = "CompletedTests_Label"
        Me.CompletedTests_Label.Size = New System.Drawing.Size(415, 21)
        Me.CompletedTests_Label.TabIndex = 4
        Me.CompletedTests_Label.Text = "Genomförda test"
        Me.CompletedTests_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CurrentSessionResults_DataGridView
        '
        Me.CurrentSessionResults_DataGridView.AllowUserToAddRows = False
        Me.CurrentSessionResults_DataGridView.AllowUserToDeleteRows = False
        Me.CurrentSessionResults_DataGridView.AllowUserToResizeRows = False
        Me.CurrentSessionResults_DataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.CurrentSessionResults_DataGridView.BackgroundColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.CurrentSessionResults_DataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.CurrentSessionResults_DataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.CurrentSessionResults_DataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.TestDescriptionColumnSession, Me.TestLengthColumnSession, Me.ResultColumnSession, Me.CompareColumnSession})
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.CurrentSessionResults_DataGridView.DefaultCellStyle = DataGridViewCellStyle2
        Me.CurrentSessionResults_DataGridView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CurrentSessionResults_DataGridView.Location = New System.Drawing.Point(3, 24)
        Me.CurrentSessionResults_DataGridView.MultiSelect = False
        Me.CurrentSessionResults_DataGridView.Name = "CurrentSessionResults_DataGridView"
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.CurrentSessionResults_DataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle3
        Me.CurrentSessionResults_DataGridView.RowHeadersVisible = False
        Me.CurrentSessionResults_DataGridView.Size = New System.Drawing.Size(415, 291)
        Me.CurrentSessionResults_DataGridView.TabIndex = 6
        '
        'TestDescriptionColumnSession
        '
        Me.TestDescriptionColumnSession.HeaderText = "Test"
        Me.TestDescriptionColumnSession.Name = "TestDescriptionColumnSession"
        Me.TestDescriptionColumnSession.ReadOnly = True
        '
        'TestLengthColumnSession
        '
        Me.TestLengthColumnSession.HeaderText = "Längd"
        Me.TestLengthColumnSession.Name = "TestLengthColumnSession"
        Me.TestLengthColumnSession.ReadOnly = True
        '
        'ResultColumnSession
        '
        Me.ResultColumnSession.HeaderText = "Resultat (%)"
        Me.ResultColumnSession.Name = "ResultColumnSession"
        Me.ResultColumnSession.ReadOnly = True
        '
        'CompareColumnSession
        '
        Me.CompareColumnSession.HeaderText = "Jämför"
        Me.CompareColumnSession.Name = "CompareColumnSession"
        Me.CompareColumnSession.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        '
        'TestSettings_TableLayoutPanel
        '
        Me.TestSettings_TableLayoutPanel.ColumnCount = 4
        Me.TestSettings_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TestSettings_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 73.0!))
        Me.TestSettings_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TestSettings_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 58.0!))
        Me.TestSettings_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.TestLength_Label, 0, 10)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.Gain_Label, 0, 3)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.SelectAudiogram_Label, 0, 0)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.ExpectedScorePanel, 1, 9)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.GainPanel, 1, 4)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.AudiogramPanel, 1, 1)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.LengthReduplications_Label, 0, 8)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.PsychmetricFunction_VerticalLabel, 0, 9)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.Audiogram_VerticalLabel, 0, 1)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.Gain_VerticalLabel, 0, 4)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.AudiogramComboBox, 2, 0)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.HaGainComboBox, 2, 3)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.ReferenceLevel_Label, 0, 2)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.ReferenceLevelComboBox, 2, 2)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.TableLayoutPanel10, 3, 1)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.TableLayoutPanel11, 3, 4)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.PNR_Label, 0, 5)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.PnrComboBox, 2, 5)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.Preset_Label, 0, 7)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.PresetComboBox, 2, 7)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.Situation_Label, 0, 6)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.TestSituationComboBox, 2, 6)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.MostDifficultItems_Button, 3, 7)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.PlannedTestLength_TextBox, 2, 10)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.TableLayoutPanel2, 2, 8)
        Me.TestSettings_TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TestSettings_TableLayoutPanel.Location = New System.Drawing.Point(3, 3)
        Me.TestSettings_TableLayoutPanel.Name = "TestSettings_TableLayoutPanel"
        Me.TestSettings_TableLayoutPanel.RowCount = 11
        Me.TestSettings_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TestSettings_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TestSettings_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TestSettings_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TestSettings_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TestSettings_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TestSettings_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TestSettings_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TestSettings_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TestSettings_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TestSettings_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TestSettings_TableLayoutPanel.Size = New System.Drawing.Size(484, 505)
        Me.TestSettings_TableLayoutPanel.TabIndex = 0
        '
        'TestLength_Label
        '
        Me.TestSettings_TableLayoutPanel.SetColumnSpan(Me.TestLength_Label, 2)
        Me.TestLength_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TestLength_Label.Location = New System.Drawing.Point(3, 483)
        Me.TestLength_Label.Name = "TestLength_Label"
        Me.TestLength_Label.Size = New System.Drawing.Size(92, 22)
        Me.TestLength_Label.TabIndex = 15
        Me.TestLength_Label.Text = "Testlängd"
        Me.TestLength_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Gain_Label
        '
        Me.TestSettings_TableLayoutPanel.SetColumnSpan(Me.Gain_Label, 2)
        Me.Gain_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Gain_Label.Location = New System.Drawing.Point(3, 154)
        Me.Gain_Label.Name = "Gain_Label"
        Me.Gain_Label.Size = New System.Drawing.Size(92, 21)
        Me.Gain_Label.TabIndex = 9
        Me.Gain_Label.Text = "Förstärkning"
        Me.Gain_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'SelectAudiogram_Label
        '
        Me.TestSettings_TableLayoutPanel.SetColumnSpan(Me.SelectAudiogram_Label, 2)
        Me.SelectAudiogram_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SelectAudiogram_Label.Location = New System.Drawing.Point(3, 0)
        Me.SelectAudiogram_Label.Name = "SelectAudiogram_Label"
        Me.SelectAudiogram_Label.Size = New System.Drawing.Size(92, 21)
        Me.SelectAudiogram_Label.TabIndex = 8
        Me.SelectAudiogram_Label.Text = "Välj audiogram"
        Me.SelectAudiogram_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ExpectedScorePanel
        '
        Me.TestSettings_TableLayoutPanel.SetColumnSpan(Me.ExpectedScorePanel, 2)
        Me.ExpectedScorePanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ExpectedScorePanel.Location = New System.Drawing.Point(25, 374)
        Me.ExpectedScorePanel.Margin = New System.Windows.Forms.Padding(0, 3, 0, 3)
        Me.ExpectedScorePanel.Name = "ExpectedScorePanel"
        Me.ExpectedScorePanel.Size = New System.Drawing.Size(401, 106)
        Me.ExpectedScorePanel.TabIndex = 1
        '
        'GainPanel
        '
        Me.TestSettings_TableLayoutPanel.SetColumnSpan(Me.GainPanel, 2)
        Me.GainPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GainPanel.Location = New System.Drawing.Point(25, 178)
        Me.GainPanel.Margin = New System.Windows.Forms.Padding(0, 3, 0, 3)
        Me.GainPanel.Name = "GainPanel"
        Me.GainPanel.Size = New System.Drawing.Size(401, 106)
        Me.GainPanel.TabIndex = 0
        '
        'AudiogramPanel
        '
        Me.TestSettings_TableLayoutPanel.SetColumnSpan(Me.AudiogramPanel, 2)
        Me.AudiogramPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AudiogramPanel.Location = New System.Drawing.Point(25, 24)
        Me.AudiogramPanel.Margin = New System.Windows.Forms.Padding(0, 3, 0, 3)
        Me.AudiogramPanel.Name = "AudiogramPanel"
        Me.AudiogramPanel.Size = New System.Drawing.Size(401, 106)
        Me.AudiogramPanel.TabIndex = 1
        '
        'LengthReduplications_Label
        '
        Me.TestSettings_TableLayoutPanel.SetColumnSpan(Me.LengthReduplications_Label, 2)
        Me.LengthReduplications_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LengthReduplications_Label.Location = New System.Drawing.Point(3, 350)
        Me.LengthReduplications_Label.Name = "LengthReduplications_Label"
        Me.LengthReduplications_Label.Size = New System.Drawing.Size(92, 21)
        Me.LengthReduplications_Label.TabIndex = 0
        Me.LengthReduplications_Label.Text = "Repetitioner"
        Me.LengthReduplications_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'PsychmetricFunction_VerticalLabel
        '
        Me.PsychmetricFunction_VerticalLabel.AutoSize = True
        Me.PsychmetricFunction_VerticalLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PsychmetricFunction_VerticalLabel.Location = New System.Drawing.Point(3, 371)
        Me.PsychmetricFunction_VerticalLabel.Name = "PsychmetricFunction_VerticalLabel"
        Me.PsychmetricFunction_VerticalLabel.Size = New System.Drawing.Size(19, 112)
        Me.PsychmetricFunction_VerticalLabel.TabIndex = 0
        Me.PsychmetricFunction_VerticalLabel.Text = "FÖRV. RESULTAT (%)"
        '
        'Audiogram_VerticalLabel
        '
        Me.Audiogram_VerticalLabel.AutoSize = True
        Me.Audiogram_VerticalLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Audiogram_VerticalLabel.Location = New System.Drawing.Point(3, 21)
        Me.Audiogram_VerticalLabel.Name = "Audiogram_VerticalLabel"
        Me.Audiogram_VerticalLabel.Size = New System.Drawing.Size(19, 112)
        Me.Audiogram_VerticalLabel.TabIndex = 0
        Me.Audiogram_VerticalLabel.Text = "AUDIOGRAM (dB HL)"
        '
        'Gain_VerticalLabel
        '
        Me.Gain_VerticalLabel.AutoSize = True
        Me.Gain_VerticalLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Gain_VerticalLabel.Location = New System.Drawing.Point(3, 175)
        Me.Gain_VerticalLabel.Name = "Gain_VerticalLabel"
        Me.Gain_VerticalLabel.Size = New System.Drawing.Size(19, 112)
        Me.Gain_VerticalLabel.TabIndex = 1
        Me.Gain_VerticalLabel.Text = "FÖRSTÄRKNING (dB)"
        '
        'AudiogramComboBox
        '
        Me.TestSettings_TableLayoutPanel.SetColumnSpan(Me.AudiogramComboBox, 2)
        Me.AudiogramComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AudiogramComboBox.FormattingEnabled = True
        Me.AudiogramComboBox.Location = New System.Drawing.Point(98, 0)
        Me.AudiogramComboBox.Margin = New System.Windows.Forms.Padding(0)
        Me.AudiogramComboBox.Name = "AudiogramComboBox"
        Me.AudiogramComboBox.Size = New System.Drawing.Size(386, 21)
        Me.AudiogramComboBox.TabIndex = 1
        '
        'HaGainComboBox
        '
        Me.TestSettings_TableLayoutPanel.SetColumnSpan(Me.HaGainComboBox, 2)
        Me.HaGainComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.HaGainComboBox.FormattingEnabled = True
        Me.HaGainComboBox.Location = New System.Drawing.Point(98, 154)
        Me.HaGainComboBox.Margin = New System.Windows.Forms.Padding(0)
        Me.HaGainComboBox.Name = "HaGainComboBox"
        Me.HaGainComboBox.Size = New System.Drawing.Size(386, 21)
        Me.HaGainComboBox.TabIndex = 3
        '
        'ReferenceLevel_Label
        '
        Me.TestSettings_TableLayoutPanel.SetColumnSpan(Me.ReferenceLevel_Label, 2)
        Me.ReferenceLevel_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ReferenceLevel_Label.Location = New System.Drawing.Point(3, 133)
        Me.ReferenceLevel_Label.Name = "ReferenceLevel_Label"
        Me.ReferenceLevel_Label.Size = New System.Drawing.Size(92, 21)
        Me.ReferenceLevel_Label.TabIndex = 0
        Me.ReferenceLevel_Label.Text = "Referensnivå (dB)"
        Me.ReferenceLevel_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ReferenceLevelComboBox
        '
        Me.TestSettings_TableLayoutPanel.SetColumnSpan(Me.ReferenceLevelComboBox, 2)
        Me.ReferenceLevelComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ReferenceLevelComboBox.FormattingEnabled = True
        Me.ReferenceLevelComboBox.Location = New System.Drawing.Point(98, 133)
        Me.ReferenceLevelComboBox.Margin = New System.Windows.Forms.Padding(0)
        Me.ReferenceLevelComboBox.Name = "ReferenceLevelComboBox"
        Me.ReferenceLevelComboBox.Size = New System.Drawing.Size(386, 21)
        Me.ReferenceLevelComboBox.TabIndex = 2
        '
        'TableLayoutPanel10
        '
        Me.TableLayoutPanel10.ColumnCount = 1
        Me.TableLayoutPanel10.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel10.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel10.Controls.Add(Me.AddTypicalAudiograms_Button, 0, 1)
        Me.TableLayoutPanel10.Controls.Add(Me.NewAudiogram_Button, 0, 0)
        Me.TableLayoutPanel10.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel10.Location = New System.Drawing.Point(429, 24)
        Me.TableLayoutPanel10.Name = "TableLayoutPanel10"
        Me.TableLayoutPanel10.RowCount = 2
        Me.TableLayoutPanel10.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel10.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel10.Size = New System.Drawing.Size(52, 106)
        Me.TableLayoutPanel10.TabIndex = 12
        '
        'AddTypicalAudiograms_Button
        '
        Me.AddTypicalAudiograms_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AddTypicalAudiograms_Button.Location = New System.Drawing.Point(0, 53)
        Me.AddTypicalAudiograms_Button.Margin = New System.Windows.Forms.Padding(0)
        Me.AddTypicalAudiograms_Button.Name = "AddTypicalAudiograms_Button"
        Me.AddTypicalAudiograms_Button.Size = New System.Drawing.Size(52, 53)
        Me.AddTypicalAudiograms_Button.TabIndex = 11
        Me.AddTypicalAudiograms_Button.Text = "Add typical"
        Me.AddTypicalAudiograms_Button.UseVisualStyleBackColor = True
        '
        'NewAudiogram_Button
        '
        Me.NewAudiogram_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.NewAudiogram_Button.Location = New System.Drawing.Point(0, 0)
        Me.NewAudiogram_Button.Margin = New System.Windows.Forms.Padding(0)
        Me.NewAudiogram_Button.Name = "NewAudiogram_Button"
        Me.NewAudiogram_Button.Size = New System.Drawing.Size(52, 53)
        Me.NewAudiogram_Button.TabIndex = 10
        Me.NewAudiogram_Button.Text = "Skapa nytt"
        Me.NewAudiogram_Button.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel11
        '
        Me.TableLayoutPanel11.ColumnCount = 1
        Me.TableLayoutPanel11.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel11.Controls.Add(Me.CreateNewGain_Button, 0, 0)
        Me.TableLayoutPanel11.Controls.Add(Me.AddFig6Gain_Button, 0, 1)
        Me.TableLayoutPanel11.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel11.Location = New System.Drawing.Point(429, 178)
        Me.TableLayoutPanel11.Name = "TableLayoutPanel11"
        Me.TableLayoutPanel11.RowCount = 2
        Me.TableLayoutPanel11.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel11.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel11.Size = New System.Drawing.Size(52, 106)
        Me.TableLayoutPanel11.TabIndex = 13
        '
        'CreateNewGain_Button
        '
        Me.CreateNewGain_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CreateNewGain_Button.Location = New System.Drawing.Point(0, 0)
        Me.CreateNewGain_Button.Margin = New System.Windows.Forms.Padding(0)
        Me.CreateNewGain_Button.Name = "CreateNewGain_Button"
        Me.CreateNewGain_Button.Size = New System.Drawing.Size(52, 53)
        Me.CreateNewGain_Button.TabIndex = 0
        Me.CreateNewGain_Button.Text = "Create new"
        Me.CreateNewGain_Button.UseVisualStyleBackColor = True
        '
        'AddFig6Gain_Button
        '
        Me.AddFig6Gain_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AddFig6Gain_Button.Location = New System.Drawing.Point(0, 53)
        Me.AddFig6Gain_Button.Margin = New System.Windows.Forms.Padding(0)
        Me.AddFig6Gain_Button.Name = "AddFig6Gain_Button"
        Me.AddFig6Gain_Button.Size = New System.Drawing.Size(52, 53)
        Me.AddFig6Gain_Button.TabIndex = 1
        Me.AddFig6Gain_Button.Text = "Fig6"
        Me.AddFig6Gain_Button.UseVisualStyleBackColor = True
        '
        'PNR_Label
        '
        Me.TestSettings_TableLayoutPanel.SetColumnSpan(Me.PNR_Label, 2)
        Me.PNR_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PNR_Label.Location = New System.Drawing.Point(3, 287)
        Me.PNR_Label.Name = "PNR_Label"
        Me.PNR_Label.Size = New System.Drawing.Size(92, 21)
        Me.PNR_Label.TabIndex = 0
        Me.PNR_Label.Text = "PNR (dB)"
        Me.PNR_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'PnrComboBox
        '
        Me.TestSettings_TableLayoutPanel.SetColumnSpan(Me.PnrComboBox, 2)
        Me.PnrComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PnrComboBox.FormattingEnabled = True
        Me.PnrComboBox.Location = New System.Drawing.Point(98, 287)
        Me.PnrComboBox.Margin = New System.Windows.Forms.Padding(0)
        Me.PnrComboBox.Name = "PnrComboBox"
        Me.PnrComboBox.Size = New System.Drawing.Size(386, 21)
        Me.PnrComboBox.TabIndex = 7
        '
        'Preset_Label
        '
        Me.TestSettings_TableLayoutPanel.SetColumnSpan(Me.Preset_Label, 2)
        Me.Preset_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Preset_Label.Location = New System.Drawing.Point(3, 329)
        Me.Preset_Label.Name = "Preset_Label"
        Me.Preset_Label.Size = New System.Drawing.Size(92, 21)
        Me.Preset_Label.TabIndex = 0
        Me.Preset_Label.Text = "Test"
        Me.Preset_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'PresetComboBox
        '
        Me.PresetComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PresetComboBox.FormattingEnabled = True
        Me.PresetComboBox.Location = New System.Drawing.Point(98, 329)
        Me.PresetComboBox.Margin = New System.Windows.Forms.Padding(0)
        Me.PresetComboBox.Name = "PresetComboBox"
        Me.PresetComboBox.Size = New System.Drawing.Size(328, 21)
        Me.PresetComboBox.TabIndex = 4
        '
        'Situation_Label
        '
        Me.TestSettings_TableLayoutPanel.SetColumnSpan(Me.Situation_Label, 2)
        Me.Situation_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Situation_Label.Location = New System.Drawing.Point(3, 308)
        Me.Situation_Label.Name = "Situation_Label"
        Me.Situation_Label.Size = New System.Drawing.Size(92, 21)
        Me.Situation_Label.TabIndex = 0
        Me.Situation_Label.Text = "Situation"
        Me.Situation_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TestSituationComboBox
        '
        Me.TestSettings_TableLayoutPanel.SetColumnSpan(Me.TestSituationComboBox, 2)
        Me.TestSituationComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TestSituationComboBox.FormattingEnabled = True
        Me.TestSituationComboBox.Location = New System.Drawing.Point(98, 308)
        Me.TestSituationComboBox.Margin = New System.Windows.Forms.Padding(0)
        Me.TestSituationComboBox.Name = "TestSituationComboBox"
        Me.TestSituationComboBox.Size = New System.Drawing.Size(386, 21)
        Me.TestSituationComboBox.TabIndex = 5
        '
        'MostDifficultItems_Button
        '
        Me.MostDifficultItems_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MostDifficultItems_Button.Location = New System.Drawing.Point(426, 329)
        Me.MostDifficultItems_Button.Margin = New System.Windows.Forms.Padding(0)
        Me.MostDifficultItems_Button.Name = "MostDifficultItems_Button"
        Me.MostDifficultItems_Button.Size = New System.Drawing.Size(58, 21)
        Me.MostDifficultItems_Button.TabIndex = 14
        Me.MostDifficultItems_Button.Text = "Custom"
        Me.MostDifficultItems_Button.UseVisualStyleBackColor = True
        '
        'PlannedTestLength_TextBox
        '
        Me.TestSettings_TableLayoutPanel.SetColumnSpan(Me.PlannedTestLength_TextBox, 2)
        Me.PlannedTestLength_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PlannedTestLength_TextBox.Location = New System.Drawing.Point(98, 483)
        Me.PlannedTestLength_TextBox.Margin = New System.Windows.Forms.Padding(0)
        Me.PlannedTestLength_TextBox.Name = "PlannedTestLength_TextBox"
        Me.PlannedTestLength_TextBox.Size = New System.Drawing.Size(386, 20)
        Me.PlannedTestLength_TextBox.TabIndex = 16
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.ColumnCount = 3
        Me.TestSettings_TableLayoutPanel.SetColumnSpan(Me.TableLayoutPanel2, 2)
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 92.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.Testparadigm_ComboBox, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.Testparadigm_Label, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.TestLengthComboBox, 0, 0)
        Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(98, 350)
        Me.TableLayoutPanel2.Margin = New System.Windows.Forms.Padding(0)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 1
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(386, 21)
        Me.TableLayoutPanel2.TabIndex = 17
        '
        'Testparadigm_ComboBox
        '
        Me.Testparadigm_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Testparadigm_ComboBox.FormattingEnabled = True
        Me.Testparadigm_ComboBox.Location = New System.Drawing.Point(239, 0)
        Me.Testparadigm_ComboBox.Margin = New System.Windows.Forms.Padding(0)
        Me.Testparadigm_ComboBox.Name = "Testparadigm_ComboBox"
        Me.Testparadigm_ComboBox.Size = New System.Drawing.Size(147, 21)
        Me.Testparadigm_ComboBox.TabIndex = 8
        '
        'Testparadigm_Label
        '
        Me.Testparadigm_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Testparadigm_Label.Location = New System.Drawing.Point(150, 0)
        Me.Testparadigm_Label.Name = "Testparadigm_Label"
        Me.Testparadigm_Label.Size = New System.Drawing.Size(86, 21)
        Me.Testparadigm_Label.TabIndex = 7
        Me.Testparadigm_Label.Text = "Paradigm"
        Me.Testparadigm_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TestLengthComboBox
        '
        Me.TestLengthComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TestLengthComboBox.FormattingEnabled = True
        Me.TestLengthComboBox.Location = New System.Drawing.Point(0, 0)
        Me.TestLengthComboBox.Margin = New System.Windows.Forms.Padding(0)
        Me.TestLengthComboBox.Name = "TestLengthComboBox"
        Me.TestLengthComboBox.Size = New System.Drawing.Size(147, 21)
        Me.TestLengthComboBox.TabIndex = 6
        '
        'TableLayoutPanel4
        '
        Me.TableLayoutPanel4.ColumnCount = 4
        Me.TableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100.0!))
        Me.TableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 101.0!))
        Me.TableLayoutPanel4.Controls.Add(Me.ProportionCorrectTextBox, 2, 5)
        Me.TableLayoutPanel4.Controls.Add(Me.MeasurementProgressBar, 0, 3)
        Me.TableLayoutPanel4.Controls.Add(Me.TestTrialDataGridView, 0, 6)
        Me.TableLayoutPanel4.Controls.Add(Me.CorrectCount_Label, 0, 4)
        Me.TableLayoutPanel4.Controls.Add(Me.ProportionCorrect_Label, 2, 4)
        Me.TableLayoutPanel4.Controls.Add(Me.Start_AudioButton, 0, 2)
        Me.TableLayoutPanel4.Controls.Add(Me.CorrectCountTextBox, 0, 5)
        Me.TableLayoutPanel4.Controls.Add(Me.TestDescriptionTextBox, 1, 0)
        Me.TableLayoutPanel4.Controls.Add(Me.TestDescription_Label, 0, 0)
        Me.TableLayoutPanel4.Controls.Add(Me.RandomSeed_Label, 0, 1)
        Me.TableLayoutPanel4.Controls.Add(Me.RandomSeed_IntegerParsingTextBox, 1, 1)
        Me.TableLayoutPanel4.Controls.Add(Me.Stop_AudioButton, 2, 2)
        Me.TableLayoutPanel4.Controls.Add(Me.KeyboardShortcutContainer_Panel, 0, 7)
        Me.TableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel4.Location = New System.Drawing.Point(493, 3)
        Me.TableLayoutPanel4.Name = "TableLayoutPanel4"
        Me.TableLayoutPanel4.RowCount = 8
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45.0!))
        Me.TableLayoutPanel4.Size = New System.Drawing.Size(395, 505)
        Me.TableLayoutPanel4.TabIndex = 1
        '
        'ProportionCorrectTextBox
        '
        Me.TableLayoutPanel4.SetColumnSpan(Me.ProportionCorrectTextBox, 2)
        Me.ProportionCorrectTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ProportionCorrectTextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ProportionCorrectTextBox.Location = New System.Drawing.Point(197, 124)
        Me.ProportionCorrectTextBox.Margin = New System.Windows.Forms.Padding(0)
        Me.ProportionCorrectTextBox.Name = "ProportionCorrectTextBox"
        Me.ProportionCorrectTextBox.ReadOnly = True
        Me.ProportionCorrectTextBox.Size = New System.Drawing.Size(198, 24)
        Me.ProportionCorrectTextBox.TabIndex = 4
        Me.ProportionCorrectTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'MeasurementProgressBar
        '
        Me.TableLayoutPanel4.SetColumnSpan(Me.MeasurementProgressBar, 4)
        Me.MeasurementProgressBar.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MeasurementProgressBar.Location = New System.Drawing.Point(3, 85)
        Me.MeasurementProgressBar.Name = "MeasurementProgressBar"
        Me.MeasurementProgressBar.Size = New System.Drawing.Size(389, 15)
        Me.MeasurementProgressBar.TabIndex = 5
        '
        'TestTrialDataGridView
        '
        Me.TestTrialDataGridView.AllowUserToAddRows = False
        Me.TestTrialDataGridView.AllowUserToDeleteRows = False
        Me.TestTrialDataGridView.AllowUserToResizeColumns = False
        Me.TestTrialDataGridView.AllowUserToResizeRows = False
        Me.TestTrialDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.TestTrialDataGridView.BackgroundColor = System.Drawing.SystemColors.Window
        Me.TestTrialDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TestTrialDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.TestTrialDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.TestTrialDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.TestTrialDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.TestWordColumn, Me.ResponseColumn, Me.ResultColumn})
        Me.TableLayoutPanel4.SetColumnSpan(Me.TestTrialDataGridView, 4)
        DataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle7.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption
        DataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.TestTrialDataGridView.DefaultCellStyle = DataGridViewCellStyle7
        Me.TestTrialDataGridView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TestTrialDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.TestTrialDataGridView.Location = New System.Drawing.Point(3, 157)
        Me.TestTrialDataGridView.Name = "TestTrialDataGridView"
        Me.TestTrialDataGridView.ReadOnly = True
        DataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle8.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.TestTrialDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle8
        Me.TestTrialDataGridView.RowHeadersVisible = False
        Me.TestTrialDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TestTrialDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.TestTrialDataGridView.ShowCellToolTips = False
        Me.TestTrialDataGridView.Size = New System.Drawing.Size(389, 300)
        Me.TestTrialDataGridView.TabIndex = 6
        '
        'TestWordColumn
        '
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TestWordColumn.DefaultCellStyle = DataGridViewCellStyle5
        Me.TestWordColumn.HeaderText = "Testord"
        Me.TestWordColumn.Name = "TestWordColumn"
        Me.TestWordColumn.ReadOnly = True
        Me.TestWordColumn.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.TestWordColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'ResponseColumn
        '
        DataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle6.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ResponseColumn.DefaultCellStyle = DataGridViewCellStyle6
        Me.ResponseColumn.HeaderText = "Svar"
        Me.ResponseColumn.Name = "ResponseColumn"
        Me.ResponseColumn.ReadOnly = True
        Me.ResponseColumn.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.ResponseColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'ResultColumn
        '
        Me.ResultColumn.HeaderText = "Resultat"
        Me.ResultColumn.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom
        Me.ResultColumn.Name = "ResultColumn"
        Me.ResultColumn.ReadOnly = True
        Me.ResultColumn.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        '
        'CorrectCount_Label
        '
        Me.CorrectCount_Label.AutoSize = True
        Me.TableLayoutPanel4.SetColumnSpan(Me.CorrectCount_Label, 2)
        Me.CorrectCount_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CorrectCount_Label.Location = New System.Drawing.Point(3, 103)
        Me.CorrectCount_Label.Name = "CorrectCount_Label"
        Me.CorrectCount_Label.Size = New System.Drawing.Size(191, 21)
        Me.CorrectCount_Label.TabIndex = 0
        Me.CorrectCount_Label.Text = "Antal rätt"
        Me.CorrectCount_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ProportionCorrect_Label
        '
        Me.ProportionCorrect_Label.AutoSize = True
        Me.TableLayoutPanel4.SetColumnSpan(Me.ProportionCorrect_Label, 2)
        Me.ProportionCorrect_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ProportionCorrect_Label.Location = New System.Drawing.Point(200, 103)
        Me.ProportionCorrect_Label.Name = "ProportionCorrect_Label"
        Me.ProportionCorrect_Label.Size = New System.Drawing.Size(192, 21)
        Me.ProportionCorrect_Label.TabIndex = 1
        Me.ProportionCorrect_Label.Text = "Andel rätt"
        Me.ProportionCorrect_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Start_AudioButton
        '
        Me.TableLayoutPanel4.SetColumnSpan(Me.Start_AudioButton, 2)
        Me.Start_AudioButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Start_AudioButton.Enabled = False
        Me.Start_AudioButton.Location = New System.Drawing.Point(3, 45)
        Me.Start_AudioButton.Name = "Start_AudioButton"
        Me.Start_AudioButton.Size = New System.Drawing.Size(191, 34)
        Me.Start_AudioButton.TabIndex = 10
        Me.Start_AudioButton.UseVisualStyleBackColor = True
        Me.Start_AudioButton.ViewMode = SpeechTestFramework.WinFormControls.AudioButton.ViewModes.Play
        '
        'CorrectCountTextBox
        '
        Me.TableLayoutPanel4.SetColumnSpan(Me.CorrectCountTextBox, 2)
        Me.CorrectCountTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CorrectCountTextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CorrectCountTextBox.Location = New System.Drawing.Point(0, 124)
        Me.CorrectCountTextBox.Margin = New System.Windows.Forms.Padding(0)
        Me.CorrectCountTextBox.Name = "CorrectCountTextBox"
        Me.CorrectCountTextBox.ReadOnly = True
        Me.CorrectCountTextBox.Size = New System.Drawing.Size(197, 24)
        Me.CorrectCountTextBox.TabIndex = 3
        Me.CorrectCountTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'TestDescriptionTextBox
        '
        Me.TableLayoutPanel4.SetColumnSpan(Me.TestDescriptionTextBox, 3)
        Me.TestDescriptionTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TestDescriptionTextBox.Location = New System.Drawing.Point(100, 0)
        Me.TestDescriptionTextBox.Margin = New System.Windows.Forms.Padding(0)
        Me.TestDescriptionTextBox.Name = "TestDescriptionTextBox"
        Me.TestDescriptionTextBox.Size = New System.Drawing.Size(295, 20)
        Me.TestDescriptionTextBox.TabIndex = 1
        '
        'TestDescription_Label
        '
        Me.TestDescription_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TestDescription_Label.Location = New System.Drawing.Point(3, 3)
        Me.TestDescription_Label.Margin = New System.Windows.Forms.Padding(3)
        Me.TestDescription_Label.Name = "TestDescription_Label"
        Me.TestDescription_Label.Size = New System.Drawing.Size(94, 15)
        Me.TestDescription_Label.TabIndex = 0
        Me.TestDescription_Label.Text = "Test description:"
        Me.TestDescription_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'RandomSeed_Label
        '
        Me.RandomSeed_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RandomSeed_Label.Location = New System.Drawing.Point(3, 24)
        Me.RandomSeed_Label.Margin = New System.Windows.Forms.Padding(3)
        Me.RandomSeed_Label.Name = "RandomSeed_Label"
        Me.RandomSeed_Label.Size = New System.Drawing.Size(94, 15)
        Me.RandomSeed_Label.TabIndex = 7
        Me.RandomSeed_Label.Text = "Random seed (optional):"
        Me.RandomSeed_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'RandomSeed_IntegerParsingTextBox
        '
        Me.TableLayoutPanel4.SetColumnSpan(Me.RandomSeed_IntegerParsingTextBox, 3)
        Me.RandomSeed_IntegerParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RandomSeed_IntegerParsingTextBox.ForeColor = System.Drawing.Color.Red
        Me.RandomSeed_IntegerParsingTextBox.Location = New System.Drawing.Point(100, 21)
        Me.RandomSeed_IntegerParsingTextBox.Margin = New System.Windows.Forms.Padding(0)
        Me.RandomSeed_IntegerParsingTextBox.Name = "RandomSeed_IntegerParsingTextBox"
        Me.RandomSeed_IntegerParsingTextBox.Size = New System.Drawing.Size(295, 20)
        Me.RandomSeed_IntegerParsingTextBox.TabIndex = 8
        '
        'Stop_AudioButton
        '
        Me.TableLayoutPanel4.SetColumnSpan(Me.Stop_AudioButton, 2)
        Me.Stop_AudioButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Stop_AudioButton.Enabled = False
        Me.Stop_AudioButton.Location = New System.Drawing.Point(200, 45)
        Me.Stop_AudioButton.Name = "Stop_AudioButton"
        Me.Stop_AudioButton.Size = New System.Drawing.Size(192, 34)
        Me.Stop_AudioButton.TabIndex = 11
        Me.Stop_AudioButton.UseVisualStyleBackColor = True
        Me.Stop_AudioButton.ViewMode = SpeechTestFramework.WinFormControls.AudioButton.ViewModes.[Stop]
        '
        'KeyboardShortcutContainer_Panel
        '
        Me.KeyboardShortcutContainer_Panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TableLayoutPanel4.SetColumnSpan(Me.KeyboardShortcutContainer_Panel, 4)
        Me.KeyboardShortcutContainer_Panel.Controls.Add(Me.KeybordShortcut_TableLayoutPanel)
        Me.KeyboardShortcutContainer_Panel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.KeyboardShortcutContainer_Panel.Location = New System.Drawing.Point(3, 460)
        Me.KeyboardShortcutContainer_Panel.Margin = New System.Windows.Forms.Padding(3, 0, 3, 0)
        Me.KeyboardShortcutContainer_Panel.Name = "KeyboardShortcutContainer_Panel"
        Me.KeyboardShortcutContainer_Panel.Size = New System.Drawing.Size(389, 45)
        Me.KeyboardShortcutContainer_Panel.TabIndex = 17
        '
        'KeybordShortcut_TableLayoutPanel
        '
        Me.KeybordShortcut_TableLayoutPanel.ColumnCount = 3
        Me.KeybordShortcut_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.KeybordShortcut_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.KeybordShortcut_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.KeybordShortcut_TableLayoutPanel.Controls.Add(Me.Pause_Label, 0, 1)
        Me.KeybordShortcut_TableLayoutPanel.Controls.Add(Me.Resume_Label, 1, 1)
        Me.KeybordShortcut_TableLayoutPanel.Controls.Add(Me.Stop_Label, 2, 1)
        Me.KeybordShortcut_TableLayoutPanel.Controls.Add(Me.KeyBoardShortcut_Label, 0, 0)
        Me.KeybordShortcut_TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.KeybordShortcut_TableLayoutPanel.Location = New System.Drawing.Point(0, 0)
        Me.KeybordShortcut_TableLayoutPanel.Name = "KeybordShortcut_TableLayoutPanel"
        Me.KeybordShortcut_TableLayoutPanel.RowCount = 2
        Me.KeybordShortcut_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.KeybordShortcut_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.KeybordShortcut_TableLayoutPanel.Size = New System.Drawing.Size(387, 43)
        Me.KeybordShortcut_TableLayoutPanel.TabIndex = 16
        '
        'Pause_Label
        '
        Me.Pause_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Pause_Label.Location = New System.Drawing.Point(3, 23)
        Me.Pause_Label.Name = "Pause_Label"
        Me.Pause_Label.Size = New System.Drawing.Size(123, 20)
        Me.Pause_Label.TabIndex = 13
        Me.Pause_Label.Text = "Pause = P"
        Me.Pause_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Resume_Label
        '
        Me.Resume_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Resume_Label.Location = New System.Drawing.Point(132, 23)
        Me.Resume_Label.Name = "Resume_Label"
        Me.Resume_Label.Size = New System.Drawing.Size(123, 20)
        Me.Resume_Label.TabIndex = 14
        Me.Resume_Label.Text = "Resume = R"
        Me.Resume_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Stop_Label
        '
        Me.Stop_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Stop_Label.Location = New System.Drawing.Point(261, 23)
        Me.Stop_Label.Name = "Stop_Label"
        Me.Stop_Label.Size = New System.Drawing.Size(123, 20)
        Me.Stop_Label.TabIndex = 15
        Me.Stop_Label.Text = "Stop = S"
        Me.Stop_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'KeyBoardShortcut_Label
        '
        Me.KeybordShortcut_TableLayoutPanel.SetColumnSpan(Me.KeyBoardShortcut_Label, 3)
        Me.KeyBoardShortcut_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.KeyBoardShortcut_Label.Location = New System.Drawing.Point(3, 0)
        Me.KeyBoardShortcut_Label.Name = "KeyBoardShortcut_Label"
        Me.KeyBoardShortcut_Label.Size = New System.Drawing.Size(381, 23)
        Me.KeyBoardShortcut_Label.TabIndex = 12
        Me.KeyBoardShortcut_Label.Text = "KeyBoardShortcut_Label"
        Me.KeyBoardShortcut_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TableLayoutPanel16
        '
        Me.TableLayoutPanel16.ColumnCount = 6
        Me.TableLayoutPanel16.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 64.0!))
        Me.TableLayoutPanel16.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 196.0!))
        Me.TableLayoutPanel16.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100.0!))
        Me.TableLayoutPanel16.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel16.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 366.0!))
        Me.TableLayoutPanel16.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 325.0!))
        Me.TableLayoutPanel16.Controls.Add(Me.ParticipantID_Label, 0, 0)
        Me.TableLayoutPanel16.Controls.Add(Me.ParticipantIdTextBox, 1, 0)
        Me.TableLayoutPanel16.Controls.Add(Me.Screen_TableLayoutPanel, 4, 0)
        Me.TableLayoutPanel16.Controls.Add(Me.ParticipantLock_Button, 2, 0)
        Me.TableLayoutPanel16.Controls.Add(Me.SoundDevice_Panel, 5, 0)
        Me.TableLayoutPanel16.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel16.Location = New System.Drawing.Point(3, 3)
        Me.TableLayoutPanel16.Name = "TableLayoutPanel16"
        Me.TableLayoutPanel16.RowCount = 1
        Me.TableLayoutPanel16.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel16.Size = New System.Drawing.Size(1318, 56)
        Me.TableLayoutPanel16.TabIndex = 1
        '
        'ParticipantID_Label
        '
        Me.ParticipantID_Label.AutoSize = True
        Me.ParticipantID_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ParticipantID_Label.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ParticipantID_Label.Location = New System.Drawing.Point(3, 0)
        Me.ParticipantID_Label.Name = "ParticipantID_Label"
        Me.ParticipantID_Label.Size = New System.Drawing.Size(58, 56)
        Me.ParticipantID_Label.TabIndex = 0
        Me.ParticipantID_Label.Text = "P.Id."
        Me.ParticipantID_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ParticipantIdTextBox
        '
        Me.ParticipantIdTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ParticipantIdTextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ParticipantIdTextBox.Location = New System.Drawing.Point(67, 11)
        Me.ParticipantIdTextBox.Margin = New System.Windows.Forms.Padding(3, 11, 3, 3)
        Me.ParticipantIdTextBox.MaxLength = 8
        Me.ParticipantIdTextBox.Name = "ParticipantIdTextBox"
        Me.ParticipantIdTextBox.Size = New System.Drawing.Size(190, 29)
        Me.ParticipantIdTextBox.TabIndex = 0
        Me.ParticipantIdTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Screen_TableLayoutPanel
        '
        Me.Screen_TableLayoutPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.[Single]
        Me.Screen_TableLayoutPanel.ColumnCount = 2
        Me.Screen_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 47.0!))
        Me.Screen_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.Screen_TableLayoutPanel.Controls.Add(Me.PcScreen_RadioButton, 0, 0)
        Me.Screen_TableLayoutPanel.Controls.Add(Me.BtScreen_RadioButton, 0, 1)
        Me.Screen_TableLayoutPanel.Controls.Add(Me.PcScreen_TableLayoutPanel, 1, 0)
        Me.Screen_TableLayoutPanel.Controls.Add(Me.BtScreen_TableLayoutPanel, 1, 1)
        Me.Screen_TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Screen_TableLayoutPanel.Enabled = False
        Me.Screen_TableLayoutPanel.Location = New System.Drawing.Point(630, 3)
        Me.Screen_TableLayoutPanel.Name = "Screen_TableLayoutPanel"
        Me.Screen_TableLayoutPanel.RowCount = 2
        Me.Screen_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.Screen_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.Screen_TableLayoutPanel.Size = New System.Drawing.Size(360, 50)
        Me.Screen_TableLayoutPanel.TabIndex = 1
        '
        'PcScreen_RadioButton
        '
        Me.PcScreen_RadioButton.AutoSize = True
        Me.PcScreen_RadioButton.Checked = True
        Me.PcScreen_RadioButton.Location = New System.Drawing.Point(4, 4)
        Me.PcScreen_RadioButton.Name = "PcScreen_RadioButton"
        Me.PcScreen_RadioButton.Size = New System.Drawing.Size(41, 17)
        Me.PcScreen_RadioButton.TabIndex = 0
        Me.PcScreen_RadioButton.TabStop = True
        Me.PcScreen_RadioButton.Text = "PC screen"
        Me.PcScreen_RadioButton.UseVisualStyleBackColor = True
        '
        'BtScreen_RadioButton
        '
        Me.BtScreen_RadioButton.AutoSize = True
        Me.BtScreen_RadioButton.Location = New System.Drawing.Point(4, 28)
        Me.BtScreen_RadioButton.Name = "BtScreen_RadioButton"
        Me.BtScreen_RadioButton.Size = New System.Drawing.Size(41, 17)
        Me.BtScreen_RadioButton.TabIndex = 1
        Me.BtScreen_RadioButton.TabStop = True
        Me.BtScreen_RadioButton.Text = "BT screen"
        Me.BtScreen_RadioButton.UseVisualStyleBackColor = True
        '
        'PcScreen_TableLayoutPanel
        '
        Me.PcScreen_TableLayoutPanel.ColumnCount = 2
        Me.PcScreen_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80.0!))
        Me.PcScreen_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.PcScreen_TableLayoutPanel.Controls.Add(Me.PcScreen_ComboBox, 1, 0)
        Me.PcScreen_TableLayoutPanel.Controls.Add(Me.PcTouch_CheckBox, 0, 0)
        Me.PcScreen_TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PcScreen_TableLayoutPanel.Location = New System.Drawing.Point(50, 2)
        Me.PcScreen_TableLayoutPanel.Margin = New System.Windows.Forms.Padding(1)
        Me.PcScreen_TableLayoutPanel.Name = "PcScreen_TableLayoutPanel"
        Me.PcScreen_TableLayoutPanel.RowCount = 1
        Me.PcScreen_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.PcScreen_TableLayoutPanel.Size = New System.Drawing.Size(308, 21)
        Me.PcScreen_TableLayoutPanel.TabIndex = 2
        '
        'PcScreen_ComboBox
        '
        Me.PcScreen_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PcScreen_ComboBox.FormattingEnabled = True
        Me.PcScreen_ComboBox.Location = New System.Drawing.Point(80, 0)
        Me.PcScreen_ComboBox.Margin = New System.Windows.Forms.Padding(0)
        Me.PcScreen_ComboBox.Name = "PcScreen_ComboBox"
        Me.PcScreen_ComboBox.Size = New System.Drawing.Size(228, 21)
        Me.PcScreen_ComboBox.TabIndex = 1
        '
        'PcTouch_CheckBox
        '
        Me.PcTouch_CheckBox.AutoSize = True
        Me.PcTouch_CheckBox.Checked = True
        Me.PcTouch_CheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.PcTouch_CheckBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PcTouch_CheckBox.Location = New System.Drawing.Point(3, 3)
        Me.PcTouch_CheckBox.Name = "PcTouch_CheckBox"
        Me.PcTouch_CheckBox.Size = New System.Drawing.Size(74, 15)
        Me.PcTouch_CheckBox.TabIndex = 2
        Me.PcTouch_CheckBox.Text = "Touch"
        Me.PcTouch_CheckBox.UseVisualStyleBackColor = True
        '
        'BtScreen_TableLayoutPanel
        '
        Me.BtScreen_TableLayoutPanel.ColumnCount = 3
        Me.BtScreen_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.BtScreen_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.BtScreen_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 52.0!))
        Me.BtScreen_TableLayoutPanel.Controls.Add(Me.ConnectBluetoothScreen_Button, 0, 0)
        Me.BtScreen_TableLayoutPanel.Controls.Add(Me.DisconnectBtScreen_Button, 1, 0)
        Me.BtScreen_TableLayoutPanel.Controls.Add(Me.BtLamp, 2, 0)
        Me.BtScreen_TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BtScreen_TableLayoutPanel.Location = New System.Drawing.Point(50, 26)
        Me.BtScreen_TableLayoutPanel.Margin = New System.Windows.Forms.Padding(1)
        Me.BtScreen_TableLayoutPanel.Name = "BtScreen_TableLayoutPanel"
        Me.BtScreen_TableLayoutPanel.RowCount = 1
        Me.BtScreen_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.BtScreen_TableLayoutPanel.Size = New System.Drawing.Size(308, 22)
        Me.BtScreen_TableLayoutPanel.TabIndex = 3
        '
        'ConnectBluetoothScreen_Button
        '
        Me.ConnectBluetoothScreen_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ConnectBluetoothScreen_Button.Location = New System.Drawing.Point(0, 0)
        Me.ConnectBluetoothScreen_Button.Margin = New System.Windows.Forms.Padding(0)
        Me.ConnectBluetoothScreen_Button.Name = "ConnectBluetoothScreen_Button"
        Me.ConnectBluetoothScreen_Button.Size = New System.Drawing.Size(128, 22)
        Me.ConnectBluetoothScreen_Button.TabIndex = 1
        Me.ConnectBluetoothScreen_Button.Text = "Anslut BT-skärm"
        Me.ConnectBluetoothScreen_Button.UseVisualStyleBackColor = True
        '
        'DisconnectBtScreen_Button
        '
        Me.DisconnectBtScreen_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DisconnectBtScreen_Button.Location = New System.Drawing.Point(128, 0)
        Me.DisconnectBtScreen_Button.Margin = New System.Windows.Forms.Padding(0)
        Me.DisconnectBtScreen_Button.Name = "DisconnectBtScreen_Button"
        Me.DisconnectBtScreen_Button.Size = New System.Drawing.Size(128, 22)
        Me.DisconnectBtScreen_Button.TabIndex = 2
        Me.DisconnectBtScreen_Button.Text = "Koppla från BT-skärm"
        Me.DisconnectBtScreen_Button.UseVisualStyleBackColor = True
        '
        'BtLamp
        '
        Me.BtLamp.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BtLamp.Location = New System.Drawing.Point(257, 1)
        Me.BtLamp.Margin = New System.Windows.Forms.Padding(1)
        Me.BtLamp.Name = "BtLamp"
        Me.BtLamp.Shape = SpeechTestFramework.Lamp.Shapes.Circle
        Me.BtLamp.ShapeSize = 0.8!
        Me.BtLamp.Size = New System.Drawing.Size(50, 20)
        Me.BtLamp.State = SpeechTestFramework.Lamp.States.Disabled
        Me.BtLamp.TabIndex = 3
        Me.BtLamp.Text = "Lamp1"
        '
        'ParticipantLock_Button
        '
        Me.ParticipantLock_Button.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ParticipantLock_Button.Location = New System.Drawing.Point(263, 10)
        Me.ParticipantLock_Button.Margin = New System.Windows.Forms.Padding(3, 10, 3, 10)
        Me.ParticipantLock_Button.Name = "ParticipantLock_Button"
        Me.ParticipantLock_Button.Size = New System.Drawing.Size(94, 31)
        Me.ParticipantLock_Button.TabIndex = 8
        Me.ParticipantLock_Button.Text = "Lås"
        Me.ParticipantLock_Button.UseVisualStyleBackColor = True
        '
        'SoundDevice_Panel
        '
        Me.SoundDevice_Panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.SoundDevice_Panel.Controls.Add(Me.SoundSettings_TableLayoutPanel)
        Me.SoundDevice_Panel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SoundDevice_Panel.Location = New System.Drawing.Point(993, 0)
        Me.SoundDevice_Panel.Margin = New System.Windows.Forms.Padding(0)
        Me.SoundDevice_Panel.Name = "SoundDevice_Panel"
        Me.SoundDevice_Panel.Size = New System.Drawing.Size(325, 56)
        Me.SoundDevice_Panel.TabIndex = 9
        '
        'SoundSettings_TableLayoutPanel
        '
        Me.SoundSettings_TableLayoutPanel.ColumnCount = 3
        Me.SoundSettings_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 95.0!))
        Me.SoundSettings_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.SoundSettings_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.SoundSettings_TableLayoutPanel.Controls.Add(Me.SelectTransducer_Label, 0, 0)
        Me.SoundSettings_TableLayoutPanel.Controls.Add(Me.Transducer_ComboBox, 0, 1)
        Me.SoundSettings_TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SoundSettings_TableLayoutPanel.Enabled = False
        Me.SoundSettings_TableLayoutPanel.Location = New System.Drawing.Point(0, 0)
        Me.SoundSettings_TableLayoutPanel.Name = "SoundSettings_TableLayoutPanel"
        Me.SoundSettings_TableLayoutPanel.RowCount = 2
        Me.SoundSettings_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.SoundSettings_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.SoundSettings_TableLayoutPanel.Size = New System.Drawing.Size(323, 54)
        Me.SoundSettings_TableLayoutPanel.TabIndex = 0
        '
        'SelectTransducer_Label
        '
        Me.SoundSettings_TableLayoutPanel.SetColumnSpan(Me.SelectTransducer_Label, 3)
        Me.SelectTransducer_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SelectTransducer_Label.Location = New System.Drawing.Point(3, 0)
        Me.SelectTransducer_Label.Name = "SelectTransducer_Label"
        Me.SelectTransducer_Label.Size = New System.Drawing.Size(317, 27)
        Me.SelectTransducer_Label.TabIndex = 0
        Me.SelectTransducer_Label.Text = "Select transducer"
        Me.SelectTransducer_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Transducer_ComboBox
        '
        Me.SoundSettings_TableLayoutPanel.SetColumnSpan(Me.Transducer_ComboBox, 3)
        Me.Transducer_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Transducer_ComboBox.FormattingEnabled = True
        Me.Transducer_ComboBox.Location = New System.Drawing.Point(3, 30)
        Me.Transducer_ComboBox.Name = "Transducer_ComboBox"
        Me.Transducer_ComboBox.Size = New System.Drawing.Size(317, 21)
        Me.Transducer_ComboBox.TabIndex = 1
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.Silver
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(3, 65)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1318, 5)
        Me.Panel1.TabIndex = 2
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AboutToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.MenuStrip1.Size = New System.Drawing.Size(1324, 24)
        Me.MenuStrip1.TabIndex = 1
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(52, 20)
        Me.AboutToolStripMenuItem.Text = "About"
        '
        'Operation_ProgressBarWithText
        '
        Me.Operation_ProgressBarWithText.CustomText = ""
        Me.Operation_ProgressBarWithText.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Operation_ProgressBarWithText.Location = New System.Drawing.Point(3, 593)
        Me.Operation_ProgressBarWithText.Name = "Operation_ProgressBarWithText"
        Me.Operation_ProgressBarWithText.Size = New System.Drawing.Size(1318, 20)
        Me.Operation_ProgressBarWithText.Step = 1
        Me.Operation_ProgressBarWithText.TabIndex = 3
        Me.Operation_ProgressBarWithText.TextFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
        Me.Operation_ProgressBarWithText.TextMode = SpeechTestFramework.ProgressBarWithText.TextModes.CustomText
        '
        'SipTestGui
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1324, 640)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "SipTestGui"
        Me.Text = "SipTestGui"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.Test_TableLayoutPanel.ResumeLayout(False)
        Me.TableLayoutPanel5.ResumeLayout(False)
        Me.TableLayoutPanel8.ResumeLayout(False)
        Me.TableLayoutPanel9.ResumeLayout(False)
        CType(Me.CurrentSessionResults_DataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TestSettings_TableLayoutPanel.ResumeLayout(False)
        Me.TestSettings_TableLayoutPanel.PerformLayout()
        Me.TableLayoutPanel10.ResumeLayout(False)
        Me.TableLayoutPanel11.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.TableLayoutPanel4.ResumeLayout(False)
        Me.TableLayoutPanel4.PerformLayout()
        CType(Me.TestTrialDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.KeyboardShortcutContainer_Panel.ResumeLayout(False)
        Me.KeybordShortcut_TableLayoutPanel.ResumeLayout(False)
        Me.TableLayoutPanel16.ResumeLayout(False)
        Me.TableLayoutPanel16.PerformLayout()
        Me.Screen_TableLayoutPanel.ResumeLayout(False)
        Me.Screen_TableLayoutPanel.PerformLayout()
        Me.PcScreen_TableLayoutPanel.ResumeLayout(False)
        Me.PcScreen_TableLayoutPanel.PerformLayout()
        Me.BtScreen_TableLayoutPanel.ResumeLayout(False)
        Me.SoundDevice_Panel.ResumeLayout(False)
        Me.SoundSettings_TableLayoutPanel.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents TableLayoutPanel1 As Windows.Forms.TableLayoutPanel
    Friend WithEvents Test_TableLayoutPanel As Windows.Forms.TableLayoutPanel
    Friend WithEvents TestSettings_TableLayoutPanel As Windows.Forms.TableLayoutPanel
    Friend WithEvents TableLayoutPanel4 As Windows.Forms.TableLayoutPanel
    Friend WithEvents TableLayoutPanel5 As Windows.Forms.TableLayoutPanel
    Friend WithEvents TableLayoutPanel16 As Windows.Forms.TableLayoutPanel
    Friend WithEvents ParticipantID_Label As Windows.Forms.Label
    Friend WithEvents ParticipantIdTextBox As Windows.Forms.TextBox
    Friend WithEvents ParticipantLock_Button As Windows.Forms.Button
    Friend WithEvents SoundSettings_TableLayoutPanel As Windows.Forms.TableLayoutPanel
    Friend WithEvents TableLayoutPanel8 As Windows.Forms.TableLayoutPanel
    Friend WithEvents Screen_TableLayoutPanel As Windows.Forms.TableLayoutPanel
    Friend WithEvents TableLayoutPanel9 As Windows.Forms.TableLayoutPanel
    Friend WithEvents ExportData_Button As Windows.Forms.Button
    Friend WithEvents ImportData_Button As Windows.Forms.Button
    Friend WithEvents StatAnalysisLabel As Windows.Forms.Label
    Friend WithEvents CompletedTests_Label As Windows.Forms.Label
    Friend WithEvents PNR_Label As Windows.Forms.Label
    Friend WithEvents LengthReduplications_Label As Windows.Forms.Label
    Friend WithEvents Situation_Label As Windows.Forms.Label
    Friend WithEvents Preset_Label As Windows.Forms.Label
    Friend WithEvents ReferenceLevel_Label As Windows.Forms.Label
    Friend WithEvents CorrectCount_Label As Windows.Forms.Label
    Friend WithEvents ProportionCorrect_Label As Windows.Forms.Label
    Friend WithEvents PnrComboBox As Windows.Forms.ComboBox
    Friend WithEvents TestLengthComboBox As Windows.Forms.ComboBox
    Friend WithEvents TestSituationComboBox As Windows.Forms.ComboBox
    Friend WithEvents PresetComboBox As Windows.Forms.ComboBox
    Friend WithEvents ReferenceLevelComboBox As Windows.Forms.ComboBox
    Friend WithEvents AudiogramComboBox As Windows.Forms.ComboBox
    Friend WithEvents HaGainComboBox As Windows.Forms.ComboBox
    Friend WithEvents ProportionCorrectTextBox As Windows.Forms.TextBox
    Friend WithEvents CorrectCountTextBox As Windows.Forms.TextBox
    Friend WithEvents MeasurementProgressBar As Windows.Forms.ProgressBar
    Friend WithEvents ConnectBluetoothScreen_Button As Windows.Forms.Button
    Friend WithEvents PcScreen_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents VerticalLabel2 As SpeechTestFramework.WinFormControls.VerticalLabel
    Friend WithEvents VerticalLabel1 As SpeechTestFramework.WinFormControls.VerticalLabel
    Friend WithEvents VerticalLabel3 As SpeechTestFramework.WinFormControls.VerticalLabel
    Friend WithEvents TestTrialDataGridView As Windows.Forms.DataGridView
    Friend WithEvents CurrentSessionResults_DataGridView As Windows.Forms.DataGridView
    Friend WithEvents TestDescription_Label As Windows.Forms.Label
    Friend WithEvents TestDescriptionTextBox As Windows.Forms.TextBox
    Friend WithEvents GainPanel As Windows.Forms.Panel
    Friend WithEvents Gain_VerticalLabel As SpeechTestFramework.WinFormControls.VerticalLabel
    Friend WithEvents Audiogram_VerticalLabel As SpeechTestFramework.WinFormControls.VerticalLabel
    Friend WithEvents PsychmetricFunction_VerticalLabel As SpeechTestFramework.WinFormControls.VerticalLabel
    Friend WithEvents AudiogramPanel As Windows.Forms.Panel
    Friend WithEvents ExpectedScorePanel As Windows.Forms.Panel
    Friend WithEvents TestWordColumn As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ResponseColumn As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ResultColumn As Windows.Forms.DataGridViewImageColumn
    Friend WithEvents Gain_Label As Windows.Forms.Label
    Friend WithEvents SelectAudiogram_Label As Windows.Forms.Label
    Friend WithEvents NewAudiogram_Button As Windows.Forms.Button
    Friend WithEvents Panel1 As Windows.Forms.Panel
    Friend WithEvents AddTypicalAudiograms_Button As Windows.Forms.Button
    Friend WithEvents TableLayoutPanel10 As Windows.Forms.TableLayoutPanel
    Friend WithEvents TableLayoutPanel11 As Windows.Forms.TableLayoutPanel
    Friend WithEvents CreateNewGain_Button As Windows.Forms.Button
    Friend WithEvents AddFig6Gain_Button As Windows.Forms.Button
    Friend WithEvents SignificanceTestResult_RichTextBox As Windows.Forms.RichTextBox
    Friend WithEvents TestDescriptionColumnSession As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents TestLengthColumnSession As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ResultColumnSession As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents CompareColumnSession As Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents MostDifficultItems_Button As Windows.Forms.Button
    Friend WithEvents TestLength_Label As Windows.Forms.Label
    Friend WithEvents PlannedTestLength_TextBox As Windows.Forms.TextBox
    Friend WithEvents PcScreen_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents BtScreen_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents PcTouch_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents DisconnectBtScreen_Button As Windows.Forms.Button
    Friend WithEvents PcScreen_TableLayoutPanel As Windows.Forms.TableLayoutPanel
    Friend WithEvents BtScreen_TableLayoutPanel As Windows.Forms.TableLayoutPanel
    Friend WithEvents BtLamp As Lamp
    Friend WithEvents SoundDevice_Panel As Windows.Forms.Panel
    Friend WithEvents SelectTransducer_Label As Windows.Forms.Label
    Friend WithEvents Transducer_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents MenuStrip1 As Windows.Forms.MenuStrip
    Friend WithEvents AboutToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents RandomSeed_Label As Windows.Forms.Label
    Friend WithEvents RandomSeed_IntegerParsingTextBox As IntegerParsingTextBox
    Friend WithEvents Start_AudioButton As WinFormControls.AudioButton
    Friend WithEvents Stop_AudioButton As WinFormControls.AudioButton
    Friend WithEvents TableLayoutPanel2 As Windows.Forms.TableLayoutPanel
    Friend WithEvents Testparadigm_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents Testparadigm_Label As Windows.Forms.Label
    Friend WithEvents KeyBoardShortcut_Label As Windows.Forms.Label
    Friend WithEvents KeybordShortcut_TableLayoutPanel As Windows.Forms.TableLayoutPanel
    Friend WithEvents Stop_Label As Windows.Forms.Label
    Friend WithEvents Pause_Label As Windows.Forms.Label
    Friend WithEvents Resume_Label As Windows.Forms.Label
    Friend WithEvents KeyboardShortcutContainer_Panel As Windows.Forms.Panel
    Friend WithEvents Operation_ProgressBarWithText As ProgressBarWithText
End Class
