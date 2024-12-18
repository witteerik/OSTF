﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class SipTestGui_2023
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
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SipTestGui_2023))
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle8 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.Top_TableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.SoundSettings_TableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.SelectTransducer_Label = New System.Windows.Forms.Label()
        Me.Transducer_ComboBox = New System.Windows.Forms.ComboBox()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.ParticipantIdTextBox = New System.Windows.Forms.TextBox()
        Me.ParticipantLock_Button = New System.Windows.Forms.Button()
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
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExportResultsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Operation_ProgressBarWithText = New SpeechTestFramework.ProgressBarWithText()
        Me.Test_TableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
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
        Me.ScrollToPresented_CheckBox = New System.Windows.Forms.CheckBox()
        Me.PauseOnNextNonTrial_CheckBox = New System.Windows.Forms.CheckBox()
        Me.TestSettings_TableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.ReferenceLevel_Label = New System.Windows.Forms.Label()
        Me.Preset_Label = New System.Windows.Forms.Label()
        Me.PresetComboBox = New System.Windows.Forms.ComboBox()
        Me.ReferenceLevelComboBox = New System.Windows.Forms.ComboBox()
        Me.PNR_Label = New System.Windows.Forms.Label()
        Me.Situation_Label = New System.Windows.Forms.Label()
        Me.PlannedTestLength_TextBox = New System.Windows.Forms.TextBox()
        Me.TestLength_Label = New System.Windows.Forms.Label()
        Me.Testparadigm_Label = New System.Windows.Forms.Label()
        Me.LengthReduplications_Label = New System.Windows.Forms.Label()
        Me.Testparadigm_ComboBox = New System.Windows.Forms.ComboBox()
        Me.TestLengthComboBox = New System.Windows.Forms.ComboBox()
        Me.Situations_FlowLayoutPanel = New System.Windows.Forms.FlowLayoutPanel()
        Me.PNRs_FlowLayoutPanel = New System.Windows.Forms.FlowLayoutPanel()
        Me.TestMode_TabControl = New System.Windows.Forms.TabControl()
        Me.BinauralSettings_TabPage = New System.Windows.Forms.TabPage()
        Me.TableLayoutPanel7 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.DirectionalSimulationSet_C1_ComboBox = New System.Windows.Forms.ComboBox()
        Me.Custom_SNC_TextBox = New System.Windows.Forms.TextBox()
        Me.TextBox2 = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.SimulatedDistance_C1_ComboBox = New System.Windows.Forms.ComboBox()
        Me.DirectionalModeTabPage = New System.Windows.Forms.TabPage()
        Me.TableLayoutPanel3 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.SpeechAzimuth_FlowLayoutPanel = New System.Windows.Forms.FlowLayoutPanel()
        Me.MaskerAzimuth_FlowLayoutPanel = New System.Windows.Forms.FlowLayoutPanel()
        Me.BackgroundAzimuth_FlowLayoutPanel = New System.Windows.Forms.FlowLayoutPanel()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.SimultaneousMaskersCount_ComboBox = New System.Windows.Forms.ComboBox()
        Me.DirectionalSimulationSet_Label = New System.Windows.Forms.Label()
        Me.DirectionalSimulationSet_ComboBox = New System.Windows.Forms.ComboBox()
        Me.SimulatedDistance_Label = New System.Windows.Forms.Label()
        Me.SimulatedDistance_ComboBox = New System.Windows.Forms.ComboBox()
        Me.BmldModeTabPage = New System.Windows.Forms.TabPage()
        Me.TableLayoutPanel6 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.BmldSignalMode_ComboBox = New System.Windows.Forms.ComboBox()
        Me.BmldNoiseMode_ComboBox = New System.Windows.Forms.ComboBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.BmldMode_RichTextBox = New System.Windows.Forms.RichTextBox()
        Me.ExportTrialSounds_CheckBox = New System.Windows.Forms.CheckBox()
        Me.TableLayoutPanel5 = New System.Windows.Forms.TableLayoutPanel()
        Me.CompletedTests_Label = New System.Windows.Forms.Label()
        Me.CurrentSessionResults_DataGridView = New System.Windows.Forms.DataGridView()
        Me.TestDescriptionColumnSession = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TestLengthColumnSession = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ResultColumnSession = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.CompareColumnSession = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.Top_TableLayoutPanel.SuspendLayout()
        Me.SoundSettings_TableLayoutPanel.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.Screen_TableLayoutPanel.SuspendLayout()
        Me.PcScreen_TableLayoutPanel.SuspendLayout()
        Me.BtScreen_TableLayoutPanel.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.Test_TableLayoutPanel.SuspendLayout()
        Me.TableLayoutPanel4.SuspendLayout()
        CType(Me.TestTrialDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.KeyboardShortcutContainer_Panel.SuspendLayout()
        Me.KeybordShortcut_TableLayoutPanel.SuspendLayout()
        Me.TestSettings_TableLayoutPanel.SuspendLayout()
        Me.TestMode_TabControl.SuspendLayout()
        Me.BinauralSettings_TabPage.SuspendLayout()
        Me.TableLayoutPanel7.SuspendLayout()
        Me.DirectionalModeTabPage.SuspendLayout()
        Me.TableLayoutPanel3.SuspendLayout()
        Me.BmldModeTabPage.SuspendLayout()
        Me.TableLayoutPanel6.SuspendLayout()
        Me.TableLayoutPanel5.SuspendLayout()
        CType(Me.CurrentSessionResults_DataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Top_TableLayoutPanel
        '
        Me.Top_TableLayoutPanel.ColumnCount = 3
        Me.Top_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 261.0!))
        Me.Top_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40.0!))
        Me.Top_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60.0!))
        Me.Top_TableLayoutPanel.Controls.Add(Me.SoundSettings_TableLayoutPanel, 1, 0)
        Me.Top_TableLayoutPanel.Controls.Add(Me.TableLayoutPanel2, 0, 0)
        Me.Top_TableLayoutPanel.Controls.Add(Me.Screen_TableLayoutPanel, 2, 0)
        Me.Top_TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Top_TableLayoutPanel.Location = New System.Drawing.Point(3, 3)
        Me.Top_TableLayoutPanel.Name = "Top_TableLayoutPanel"
        Me.Top_TableLayoutPanel.RowCount = 1
        Me.Top_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.Top_TableLayoutPanel.Size = New System.Drawing.Size(1833, 56)
        Me.Top_TableLayoutPanel.TabIndex = 2
        '
        'SoundSettings_TableLayoutPanel
        '
        Me.SoundSettings_TableLayoutPanel.ColumnCount = 1
        Me.SoundSettings_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.SoundSettings_TableLayoutPanel.Controls.Add(Me.SelectTransducer_Label, 0, 0)
        Me.SoundSettings_TableLayoutPanel.Controls.Add(Me.Transducer_ComboBox, 0, 1)
        Me.SoundSettings_TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SoundSettings_TableLayoutPanel.Enabled = False
        Me.SoundSettings_TableLayoutPanel.Location = New System.Drawing.Point(264, 3)
        Me.SoundSettings_TableLayoutPanel.Name = "SoundSettings_TableLayoutPanel"
        Me.SoundSettings_TableLayoutPanel.RowCount = 2
        Me.SoundSettings_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24.0!))
        Me.SoundSettings_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.SoundSettings_TableLayoutPanel.Size = New System.Drawing.Size(622, 50)
        Me.SoundSettings_TableLayoutPanel.TabIndex = 0
        '
        'SelectTransducer_Label
        '
        Me.SelectTransducer_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SelectTransducer_Label.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.SelectTransducer_Label.Location = New System.Drawing.Point(3, 0)
        Me.SelectTransducer_Label.Name = "SelectTransducer_Label"
        Me.SelectTransducer_Label.Size = New System.Drawing.Size(616, 24)
        Me.SelectTransducer_Label.TabIndex = 0
        Me.SelectTransducer_Label.Text = "Select transducer"
        Me.SelectTransducer_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Transducer_ComboBox
        '
        Me.Transducer_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Transducer_ComboBox.FormattingEnabled = True
        Me.Transducer_ComboBox.Location = New System.Drawing.Point(3, 27)
        Me.Transducer_ComboBox.Name = "Transducer_ComboBox"
        Me.Transducer_ComboBox.Size = New System.Drawing.Size(616, 21)
        Me.Transducer_ComboBox.TabIndex = 1
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.ColumnCount = 2
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65.4902!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34.5098!))
        Me.TableLayoutPanel2.Controls.Add(Me.Label2, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.ParticipantIdTextBox, 0, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.ParticipantLock_Button, 1, 1)
        Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(3, 3)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 2
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(255, 50)
        Me.TableLayoutPanel2.TabIndex = 3
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.TableLayoutPanel2.SetColumnSpan(Me.Label2, 2)
        Me.Label2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label2.Location = New System.Drawing.Point(3, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(249, 24)
        Me.Label2.TabIndex = 9
        Me.Label2.Text = "Add participant code"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ParticipantIdTextBox
        '
        Me.ParticipantIdTextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ParticipantIdTextBox.Location = New System.Drawing.Point(3, 27)
        Me.ParticipantIdTextBox.MaxLength = 8
        Me.ParticipantIdTextBox.Name = "ParticipantIdTextBox"
        Me.ParticipantIdTextBox.Size = New System.Drawing.Size(161, 20)
        Me.ParticipantIdTextBox.TabIndex = 0
        Me.ParticipantIdTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'ParticipantLock_Button
        '
        Me.ParticipantLock_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ParticipantLock_Button.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ParticipantLock_Button.Location = New System.Drawing.Point(170, 27)
        Me.ParticipantLock_Button.Name = "ParticipantLock_Button"
        Me.ParticipantLock_Button.Size = New System.Drawing.Size(82, 20)
        Me.ParticipantLock_Button.TabIndex = 8
        Me.ParticipantLock_Button.Text = "Lock"
        Me.ParticipantLock_Button.UseVisualStyleBackColor = True
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
        Me.Screen_TableLayoutPanel.Location = New System.Drawing.Point(892, 3)
        Me.Screen_TableLayoutPanel.Name = "Screen_TableLayoutPanel"
        Me.Screen_TableLayoutPanel.RowCount = 2
        Me.Screen_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.Screen_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.Screen_TableLayoutPanel.Size = New System.Drawing.Size(938, 50)
        Me.Screen_TableLayoutPanel.TabIndex = 4
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
        Me.PcScreen_TableLayoutPanel.Size = New System.Drawing.Size(886, 21)
        Me.PcScreen_TableLayoutPanel.TabIndex = 2
        '
        'PcScreen_ComboBox
        '
        Me.PcScreen_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PcScreen_ComboBox.FormattingEnabled = True
        Me.PcScreen_ComboBox.Location = New System.Drawing.Point(80, 0)
        Me.PcScreen_ComboBox.Margin = New System.Windows.Forms.Padding(0)
        Me.PcScreen_ComboBox.Name = "PcScreen_ComboBox"
        Me.PcScreen_ComboBox.Size = New System.Drawing.Size(806, 21)
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
        Me.BtScreen_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 54.0!))
        Me.BtScreen_TableLayoutPanel.Controls.Add(Me.ConnectBluetoothScreen_Button, 0, 0)
        Me.BtScreen_TableLayoutPanel.Controls.Add(Me.DisconnectBtScreen_Button, 1, 0)
        Me.BtScreen_TableLayoutPanel.Controls.Add(Me.BtLamp, 2, 0)
        Me.BtScreen_TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BtScreen_TableLayoutPanel.Location = New System.Drawing.Point(50, 26)
        Me.BtScreen_TableLayoutPanel.Margin = New System.Windows.Forms.Padding(1)
        Me.BtScreen_TableLayoutPanel.Name = "BtScreen_TableLayoutPanel"
        Me.BtScreen_TableLayoutPanel.RowCount = 1
        Me.BtScreen_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.BtScreen_TableLayoutPanel.Size = New System.Drawing.Size(886, 22)
        Me.BtScreen_TableLayoutPanel.TabIndex = 3
        '
        'ConnectBluetoothScreen_Button
        '
        Me.ConnectBluetoothScreen_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ConnectBluetoothScreen_Button.Location = New System.Drawing.Point(0, 0)
        Me.ConnectBluetoothScreen_Button.Margin = New System.Windows.Forms.Padding(0)
        Me.ConnectBluetoothScreen_Button.Name = "ConnectBluetoothScreen_Button"
        Me.ConnectBluetoothScreen_Button.Size = New System.Drawing.Size(416, 22)
        Me.ConnectBluetoothScreen_Button.TabIndex = 1
        Me.ConnectBluetoothScreen_Button.Text = "Anslut BT-skärm"
        Me.ConnectBluetoothScreen_Button.UseVisualStyleBackColor = True
        '
        'DisconnectBtScreen_Button
        '
        Me.DisconnectBtScreen_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DisconnectBtScreen_Button.Location = New System.Drawing.Point(416, 0)
        Me.DisconnectBtScreen_Button.Margin = New System.Windows.Forms.Padding(0)
        Me.DisconnectBtScreen_Button.Name = "DisconnectBtScreen_Button"
        Me.DisconnectBtScreen_Button.Size = New System.Drawing.Size(416, 22)
        Me.DisconnectBtScreen_Button.TabIndex = 2
        Me.DisconnectBtScreen_Button.Text = "Koppla från BT-skärm"
        Me.DisconnectBtScreen_Button.UseVisualStyleBackColor = True
        '
        'BtLamp
        '
        Me.BtLamp.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BtLamp.Location = New System.Drawing.Point(833, 1)
        Me.BtLamp.Margin = New System.Windows.Forms.Padding(1)
        Me.BtLamp.Name = "BtLamp"
        Me.BtLamp.Shape = SpeechTestFramework.Lamp.Shapes.Circle
        Me.BtLamp.ShapeSize = 0.8!
        Me.BtLamp.Size = New System.Drawing.Size(52, 20)
        Me.BtLamp.State = SpeechTestFramework.Lamp.States.Disabled
        Me.BtLamp.TabIndex = 3
        Me.BtLamp.Text = "Lamp1"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.AboutToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(1839, 24)
        Me.MenuStrip1.TabIndex = 3
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ExportResultsToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.FileToolStripMenuItem.Text = "File"
        '
        'ExportResultsToolStripMenuItem
        '
        Me.ExportResultsToolStripMenuItem.Name = "ExportResultsToolStripMenuItem"
        Me.ExportResultsToolStripMenuItem.Size = New System.Drawing.Size(145, 22)
        Me.ExportResultsToolStripMenuItem.Text = "Export results"
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(52, 20)
        Me.AboutToolStripMenuItem.Text = "About"
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.Panel1, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Top_TableLayoutPanel, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Operation_ProgressBarWithText, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.Test_TableLayoutPanel, 0, 2)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 24)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 4
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 11.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(1839, 858)
        Me.TableLayoutPanel1.TabIndex = 4
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.Silver
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(3, 65)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1833, 5)
        Me.Panel1.TabIndex = 3
        '
        'Operation_ProgressBarWithText
        '
        Me.Operation_ProgressBarWithText.CustomText = ""
        Me.Operation_ProgressBarWithText.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Operation_ProgressBarWithText.Location = New System.Drawing.Point(3, 832)
        Me.Operation_ProgressBarWithText.Name = "Operation_ProgressBarWithText"
        Me.Operation_ProgressBarWithText.Size = New System.Drawing.Size(1833, 23)
        Me.Operation_ProgressBarWithText.Step = 1
        Me.Operation_ProgressBarWithText.TabIndex = 4
        Me.Operation_ProgressBarWithText.TextFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
        Me.Operation_ProgressBarWithText.TextMode = SpeechTestFramework.ProgressBarWithText.TextModes.CustomText
        '
        'Test_TableLayoutPanel
        '
        Me.Test_TableLayoutPanel.ColumnCount = 3
        Me.Test_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 37.37374!))
        Me.Test_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.30303!))
        Me.Test_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.32323!))
        Me.Test_TableLayoutPanel.Controls.Add(Me.TableLayoutPanel4, 0, 0)
        Me.Test_TableLayoutPanel.Controls.Add(Me.TestSettings_TableLayoutPanel, 0, 0)
        Me.Test_TableLayoutPanel.Controls.Add(Me.TableLayoutPanel5, 2, 0)
        Me.Test_TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Test_TableLayoutPanel.Enabled = False
        Me.Test_TableLayoutPanel.Location = New System.Drawing.Point(3, 76)
        Me.Test_TableLayoutPanel.Name = "Test_TableLayoutPanel"
        Me.Test_TableLayoutPanel.RowCount = 1
        Me.Test_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.Test_TableLayoutPanel.Size = New System.Drawing.Size(1833, 750)
        Me.Test_TableLayoutPanel.TabIndex = 5
        '
        'TableLayoutPanel4
        '
        Me.TableLayoutPanel4.ColumnCount = 4
        Me.TableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100.0!))
        Me.TableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 103.0!))
        Me.TableLayoutPanel4.Controls.Add(Me.ProportionCorrectTextBox, 2, 5)
        Me.TableLayoutPanel4.Controls.Add(Me.MeasurementProgressBar, 0, 3)
        Me.TableLayoutPanel4.Controls.Add(Me.TestTrialDataGridView, 0, 7)
        Me.TableLayoutPanel4.Controls.Add(Me.CorrectCount_Label, 0, 4)
        Me.TableLayoutPanel4.Controls.Add(Me.ProportionCorrect_Label, 2, 4)
        Me.TableLayoutPanel4.Controls.Add(Me.Start_AudioButton, 0, 2)
        Me.TableLayoutPanel4.Controls.Add(Me.CorrectCountTextBox, 0, 5)
        Me.TableLayoutPanel4.Controls.Add(Me.TestDescriptionTextBox, 1, 0)
        Me.TableLayoutPanel4.Controls.Add(Me.TestDescription_Label, 0, 0)
        Me.TableLayoutPanel4.Controls.Add(Me.RandomSeed_Label, 0, 1)
        Me.TableLayoutPanel4.Controls.Add(Me.RandomSeed_IntegerParsingTextBox, 1, 1)
        Me.TableLayoutPanel4.Controls.Add(Me.Stop_AudioButton, 2, 2)
        Me.TableLayoutPanel4.Controls.Add(Me.KeyboardShortcutContainer_Panel, 0, 8)
        Me.TableLayoutPanel4.Controls.Add(Me.ScrollToPresented_CheckBox, 0, 6)
        Me.TableLayoutPanel4.Controls.Add(Me.PauseOnNextNonTrial_CheckBox, 2, 6)
        Me.TableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel4.Location = New System.Drawing.Point(688, 3)
        Me.TableLayoutPanel4.Name = "TableLayoutPanel4"
        Me.TableLayoutPanel4.RowCount = 9
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45.0!))
        Me.TableLayoutPanel4.Size = New System.Drawing.Size(549, 744)
        Me.TableLayoutPanel4.TabIndex = 2
        '
        'ProportionCorrectTextBox
        '
        Me.TableLayoutPanel4.SetColumnSpan(Me.ProportionCorrectTextBox, 2)
        Me.ProportionCorrectTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ProportionCorrectTextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ProportionCorrectTextBox.Location = New System.Drawing.Point(273, 124)
        Me.ProportionCorrectTextBox.Margin = New System.Windows.Forms.Padding(0)
        Me.ProportionCorrectTextBox.Name = "ProportionCorrectTextBox"
        Me.ProportionCorrectTextBox.ReadOnly = True
        Me.ProportionCorrectTextBox.Size = New System.Drawing.Size(276, 24)
        Me.ProportionCorrectTextBox.TabIndex = 4
        Me.ProportionCorrectTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'MeasurementProgressBar
        '
        Me.TableLayoutPanel4.SetColumnSpan(Me.MeasurementProgressBar, 4)
        Me.MeasurementProgressBar.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MeasurementProgressBar.Location = New System.Drawing.Point(3, 85)
        Me.MeasurementProgressBar.Name = "MeasurementProgressBar"
        Me.MeasurementProgressBar.Size = New System.Drawing.Size(543, 15)
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
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.TestTrialDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.TestTrialDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.TestTrialDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.TestWordColumn, Me.ResponseColumn, Me.ResultColumn})
        Me.TableLayoutPanel4.SetColumnSpan(Me.TestTrialDataGridView, 4)
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.TestTrialDataGridView.DefaultCellStyle = DataGridViewCellStyle4
        Me.TestTrialDataGridView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TestTrialDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.TestTrialDataGridView.Location = New System.Drawing.Point(3, 178)
        Me.TestTrialDataGridView.Name = "TestTrialDataGridView"
        Me.TestTrialDataGridView.ReadOnly = True
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.TestTrialDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle5
        Me.TestTrialDataGridView.RowHeadersVisible = False
        Me.TestTrialDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TestTrialDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.TestTrialDataGridView.ShowCellToolTips = False
        Me.TestTrialDataGridView.Size = New System.Drawing.Size(543, 518)
        Me.TestTrialDataGridView.TabIndex = 6
        '
        'TestWordColumn
        '
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TestWordColumn.DefaultCellStyle = DataGridViewCellStyle2
        Me.TestWordColumn.HeaderText = "Testord"
        Me.TestWordColumn.Name = "TestWordColumn"
        Me.TestWordColumn.ReadOnly = True
        Me.TestWordColumn.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.TestWordColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'ResponseColumn
        '
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ResponseColumn.DefaultCellStyle = DataGridViewCellStyle3
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
        Me.CorrectCount_Label.Size = New System.Drawing.Size(267, 21)
        Me.CorrectCount_Label.TabIndex = 0
        Me.CorrectCount_Label.Text = "Number correct"
        Me.CorrectCount_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ProportionCorrect_Label
        '
        Me.ProportionCorrect_Label.AutoSize = True
        Me.TableLayoutPanel4.SetColumnSpan(Me.ProportionCorrect_Label, 2)
        Me.ProportionCorrect_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ProportionCorrect_Label.Location = New System.Drawing.Point(276, 103)
        Me.ProportionCorrect_Label.Name = "ProportionCorrect_Label"
        Me.ProportionCorrect_Label.Size = New System.Drawing.Size(270, 21)
        Me.ProportionCorrect_Label.TabIndex = 1
        Me.ProportionCorrect_Label.Text = "Proportion correct"
        Me.ProportionCorrect_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Start_AudioButton
        '
        Me.TableLayoutPanel4.SetColumnSpan(Me.Start_AudioButton, 2)
        Me.Start_AudioButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Start_AudioButton.Enabled = False
        Me.Start_AudioButton.Location = New System.Drawing.Point(3, 45)
        Me.Start_AudioButton.Name = "Start_AudioButton"
        Me.Start_AudioButton.Size = New System.Drawing.Size(267, 34)
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
        Me.CorrectCountTextBox.Size = New System.Drawing.Size(273, 24)
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
        Me.TestDescriptionTextBox.Size = New System.Drawing.Size(449, 20)
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
        Me.RandomSeed_IntegerParsingTextBox.Size = New System.Drawing.Size(449, 20)
        Me.RandomSeed_IntegerParsingTextBox.TabIndex = 8
        '
        'Stop_AudioButton
        '
        Me.TableLayoutPanel4.SetColumnSpan(Me.Stop_AudioButton, 2)
        Me.Stop_AudioButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Stop_AudioButton.Enabled = False
        Me.Stop_AudioButton.Location = New System.Drawing.Point(276, 45)
        Me.Stop_AudioButton.Name = "Stop_AudioButton"
        Me.Stop_AudioButton.Size = New System.Drawing.Size(270, 34)
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
        Me.KeyboardShortcutContainer_Panel.Location = New System.Drawing.Point(3, 699)
        Me.KeyboardShortcutContainer_Panel.Margin = New System.Windows.Forms.Padding(3, 0, 3, 0)
        Me.KeyboardShortcutContainer_Panel.Name = "KeyboardShortcutContainer_Panel"
        Me.KeyboardShortcutContainer_Panel.Size = New System.Drawing.Size(543, 45)
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
        Me.KeybordShortcut_TableLayoutPanel.Size = New System.Drawing.Size(541, 43)
        Me.KeybordShortcut_TableLayoutPanel.TabIndex = 16
        '
        'Pause_Label
        '
        Me.Pause_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Pause_Label.Location = New System.Drawing.Point(3, 23)
        Me.Pause_Label.Name = "Pause_Label"
        Me.Pause_Label.Size = New System.Drawing.Size(174, 20)
        Me.Pause_Label.TabIndex = 13
        Me.Pause_Label.Text = "Pause = P"
        Me.Pause_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Resume_Label
        '
        Me.Resume_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Resume_Label.Location = New System.Drawing.Point(183, 23)
        Me.Resume_Label.Name = "Resume_Label"
        Me.Resume_Label.Size = New System.Drawing.Size(174, 20)
        Me.Resume_Label.TabIndex = 14
        Me.Resume_Label.Text = "Resume = R"
        Me.Resume_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Stop_Label
        '
        Me.Stop_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Stop_Label.Location = New System.Drawing.Point(363, 23)
        Me.Stop_Label.Name = "Stop_Label"
        Me.Stop_Label.Size = New System.Drawing.Size(175, 20)
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
        Me.KeyBoardShortcut_Label.Size = New System.Drawing.Size(535, 23)
        Me.KeyBoardShortcut_Label.TabIndex = 12
        Me.KeyBoardShortcut_Label.Text = "KeyBoardShortcut_Label"
        Me.KeyBoardShortcut_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ScrollToPresented_CheckBox
        '
        Me.ScrollToPresented_CheckBox.Checked = True
        Me.ScrollToPresented_CheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.TableLayoutPanel4.SetColumnSpan(Me.ScrollToPresented_CheckBox, 2)
        Me.ScrollToPresented_CheckBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ScrollToPresented_CheckBox.Location = New System.Drawing.Point(3, 155)
        Me.ScrollToPresented_CheckBox.Margin = New System.Windows.Forms.Padding(3, 1, 3, 1)
        Me.ScrollToPresented_CheckBox.Name = "ScrollToPresented_CheckBox"
        Me.ScrollToPresented_CheckBox.Size = New System.Drawing.Size(267, 19)
        Me.ScrollToPresented_CheckBox.TabIndex = 18
        Me.ScrollToPresented_CheckBox.Text = "Scroll to presented test word"
        Me.ScrollToPresented_CheckBox.UseVisualStyleBackColor = True
        '
        'PauseOnNextNonTrial_CheckBox
        '
        Me.TableLayoutPanel4.SetColumnSpan(Me.PauseOnNextNonTrial_CheckBox, 2)
        Me.PauseOnNextNonTrial_CheckBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PauseOnNextNonTrial_CheckBox.Location = New System.Drawing.Point(276, 155)
        Me.PauseOnNextNonTrial_CheckBox.Margin = New System.Windows.Forms.Padding(3, 1, 3, 1)
        Me.PauseOnNextNonTrial_CheckBox.Name = "PauseOnNextNonTrial_CheckBox"
        Me.PauseOnNextNonTrial_CheckBox.Size = New System.Drawing.Size(270, 19)
        Me.PauseOnNextNonTrial_CheckBox.TabIndex = 19
        Me.PauseOnNextNonTrial_CheckBox.Text = "Pause on next non-test trial"
        Me.PauseOnNextNonTrial_CheckBox.UseVisualStyleBackColor = True
        '
        'TestSettings_TableLayoutPanel
        '
        Me.TestSettings_TableLayoutPanel.ColumnCount = 2
        Me.TestSettings_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.08223!))
        Me.TestSettings_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75.91777!))
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.ReferenceLevel_Label, 0, 2)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.Preset_Label, 0, 1)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.PresetComboBox, 1, 1)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.ReferenceLevelComboBox, 1, 2)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.PNR_Label, 0, 3)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.Situation_Label, 0, 4)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.PlannedTestLength_TextBox, 1, 7)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.TestLength_Label, 0, 7)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.Testparadigm_Label, 0, 6)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.LengthReduplications_Label, 0, 5)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.Testparadigm_ComboBox, 1, 6)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.TestLengthComboBox, 1, 5)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.Situations_FlowLayoutPanel, 1, 4)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.PNRs_FlowLayoutPanel, 1, 3)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.TestMode_TabControl, 0, 0)
        Me.TestSettings_TableLayoutPanel.Controls.Add(Me.ExportTrialSounds_CheckBox, 1, 8)
        Me.TestSettings_TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TestSettings_TableLayoutPanel.Location = New System.Drawing.Point(3, 3)
        Me.TestSettings_TableLayoutPanel.Name = "TestSettings_TableLayoutPanel"
        Me.TestSettings_TableLayoutPanel.RowCount = 9
        Me.TestSettings_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70.0!))
        Me.TestSettings_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TestSettings_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TestSettings_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30.0!))
        Me.TestSettings_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 47.0!))
        Me.TestSettings_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TestSettings_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TestSettings_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TestSettings_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22.0!))
        Me.TestSettings_TableLayoutPanel.Size = New System.Drawing.Size(679, 744)
        Me.TestSettings_TableLayoutPanel.TabIndex = 1
        '
        'ReferenceLevel_Label
        '
        Me.ReferenceLevel_Label.Location = New System.Drawing.Point(3, 420)
        Me.ReferenceLevel_Label.Name = "ReferenceLevel_Label"
        Me.ReferenceLevel_Label.Size = New System.Drawing.Size(92, 21)
        Me.ReferenceLevel_Label.TabIndex = 0
        Me.ReferenceLevel_Label.Text = "Reference level (dB)"
        Me.ReferenceLevel_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Preset_Label
        '
        Me.Preset_Label.Location = New System.Drawing.Point(3, 399)
        Me.Preset_Label.Name = "Preset_Label"
        Me.Preset_Label.Size = New System.Drawing.Size(92, 21)
        Me.Preset_Label.TabIndex = 0
        Me.Preset_Label.Text = "Test (preset)"
        Me.Preset_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'PresetComboBox
        '
        Me.PresetComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PresetComboBox.FormattingEnabled = True
        Me.PresetComboBox.Location = New System.Drawing.Point(163, 399)
        Me.PresetComboBox.Margin = New System.Windows.Forms.Padding(0)
        Me.PresetComboBox.Name = "PresetComboBox"
        Me.PresetComboBox.Size = New System.Drawing.Size(516, 21)
        Me.PresetComboBox.TabIndex = 4
        '
        'ReferenceLevelComboBox
        '
        Me.ReferenceLevelComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ReferenceLevelComboBox.FormattingEnabled = True
        Me.ReferenceLevelComboBox.Location = New System.Drawing.Point(163, 420)
        Me.ReferenceLevelComboBox.Margin = New System.Windows.Forms.Padding(0)
        Me.ReferenceLevelComboBox.Name = "ReferenceLevelComboBox"
        Me.ReferenceLevelComboBox.Size = New System.Drawing.Size(516, 21)
        Me.ReferenceLevelComboBox.TabIndex = 2
        '
        'PNR_Label
        '
        Me.PNR_Label.Location = New System.Drawing.Point(3, 441)
        Me.PNR_Label.Name = "PNR_Label"
        Me.PNR_Label.Size = New System.Drawing.Size(92, 21)
        Me.PNR_Label.TabIndex = 0
        Me.PNR_Label.Text = "PNRs (dB)"
        Me.PNR_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Situation_Label
        '
        Me.Situation_Label.Location = New System.Drawing.Point(3, 612)
        Me.Situation_Label.Name = "Situation_Label"
        Me.Situation_Label.Size = New System.Drawing.Size(92, 21)
        Me.Situation_Label.TabIndex = 0
        Me.Situation_Label.Text = "Situations"
        Me.Situation_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'PlannedTestLength_TextBox
        '
        Me.PlannedTestLength_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PlannedTestLength_TextBox.Location = New System.Drawing.Point(163, 701)
        Me.PlannedTestLength_TextBox.Margin = New System.Windows.Forms.Padding(0)
        Me.PlannedTestLength_TextBox.Name = "PlannedTestLength_TextBox"
        Me.PlannedTestLength_TextBox.ReadOnly = True
        Me.PlannedTestLength_TextBox.Size = New System.Drawing.Size(516, 20)
        Me.PlannedTestLength_TextBox.TabIndex = 16
        '
        'TestLength_Label
        '
        Me.TestLength_Label.Location = New System.Drawing.Point(3, 701)
        Me.TestLength_Label.Name = "TestLength_Label"
        Me.TestLength_Label.Size = New System.Drawing.Size(92, 21)
        Me.TestLength_Label.TabIndex = 15
        Me.TestLength_Label.Text = "Trial count (n)"
        Me.TestLength_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Testparadigm_Label
        '
        Me.Testparadigm_Label.Location = New System.Drawing.Point(3, 680)
        Me.Testparadigm_Label.Name = "Testparadigm_Label"
        Me.Testparadigm_Label.Size = New System.Drawing.Size(100, 21)
        Me.Testparadigm_Label.TabIndex = 22
        Me.Testparadigm_Label.Text = "Test paradigm"
        Me.Testparadigm_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LengthReduplications_Label
        '
        Me.LengthReduplications_Label.Location = New System.Drawing.Point(3, 659)
        Me.LengthReduplications_Label.Name = "LengthReduplications_Label"
        Me.LengthReduplications_Label.Size = New System.Drawing.Size(92, 21)
        Me.LengthReduplications_Label.TabIndex = 0
        Me.LengthReduplications_Label.Text = "Repetitions (n)"
        Me.LengthReduplications_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Testparadigm_ComboBox
        '
        Me.Testparadigm_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Testparadigm_ComboBox.FormattingEnabled = True
        Me.Testparadigm_ComboBox.Location = New System.Drawing.Point(163, 680)
        Me.Testparadigm_ComboBox.Margin = New System.Windows.Forms.Padding(0)
        Me.Testparadigm_ComboBox.Name = "Testparadigm_ComboBox"
        Me.Testparadigm_ComboBox.Size = New System.Drawing.Size(516, 21)
        Me.Testparadigm_ComboBox.TabIndex = 21
        '
        'TestLengthComboBox
        '
        Me.TestLengthComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TestLengthComboBox.FormattingEnabled = True
        Me.TestLengthComboBox.Location = New System.Drawing.Point(163, 659)
        Me.TestLengthComboBox.Margin = New System.Windows.Forms.Padding(0)
        Me.TestLengthComboBox.Name = "TestLengthComboBox"
        Me.TestLengthComboBox.Size = New System.Drawing.Size(516, 21)
        Me.TestLengthComboBox.TabIndex = 6
        '
        'Situations_FlowLayoutPanel
        '
        Me.Situations_FlowLayoutPanel.AutoScroll = True
        Me.Situations_FlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Situations_FlowLayoutPanel.Location = New System.Drawing.Point(166, 615)
        Me.Situations_FlowLayoutPanel.Name = "Situations_FlowLayoutPanel"
        Me.Situations_FlowLayoutPanel.Size = New System.Drawing.Size(510, 41)
        Me.Situations_FlowLayoutPanel.TabIndex = 27
        '
        'PNRs_FlowLayoutPanel
        '
        Me.PNRs_FlowLayoutPanel.AutoScroll = True
        Me.PNRs_FlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PNRs_FlowLayoutPanel.Location = New System.Drawing.Point(166, 444)
        Me.PNRs_FlowLayoutPanel.Name = "PNRs_FlowLayoutPanel"
        Me.PNRs_FlowLayoutPanel.Size = New System.Drawing.Size(510, 165)
        Me.PNRs_FlowLayoutPanel.TabIndex = 28
        '
        'TestMode_TabControl
        '
        Me.TestSettings_TableLayoutPanel.SetColumnSpan(Me.TestMode_TabControl, 2)
        Me.TestMode_TabControl.Controls.Add(Me.BinauralSettings_TabPage)
        Me.TestMode_TabControl.Controls.Add(Me.DirectionalModeTabPage)
        Me.TestMode_TabControl.Controls.Add(Me.BmldModeTabPage)
        Me.TestMode_TabControl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TestMode_TabControl.Location = New System.Drawing.Point(3, 3)
        Me.TestMode_TabControl.Name = "TestMode_TabControl"
        Me.TestMode_TabControl.SelectedIndex = 0
        Me.TestMode_TabControl.Size = New System.Drawing.Size(673, 393)
        Me.TestMode_TabControl.TabIndex = 29
        '
        'BinauralSettings_TabPage
        '
        Me.BinauralSettings_TabPage.Controls.Add(Me.TableLayoutPanel7)
        Me.BinauralSettings_TabPage.Location = New System.Drawing.Point(4, 22)
        Me.BinauralSettings_TabPage.Name = "BinauralSettings_TabPage"
        Me.BinauralSettings_TabPage.Size = New System.Drawing.Size(665, 367)
        Me.BinauralSettings_TabPage.TabIndex = 2
        Me.BinauralSettings_TabPage.Text = "Binaural settings"
        Me.BinauralSettings_TabPage.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel7
        '
        Me.TableLayoutPanel7.ColumnCount = 2
        Me.TableLayoutPanel7.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 58.23223!))
        Me.TableLayoutPanel7.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 41.76777!))
        Me.TableLayoutPanel7.Controls.Add(Me.Label9, 0, 0)
        Me.TableLayoutPanel7.Controls.Add(Me.DirectionalSimulationSet_C1_ComboBox, 1, 0)
        Me.TableLayoutPanel7.Controls.Add(Me.Custom_SNC_TextBox, 1, 2)
        Me.TableLayoutPanel7.Controls.Add(Me.TextBox2, 0, 2)
        Me.TableLayoutPanel7.Controls.Add(Me.Label10, 0, 1)
        Me.TableLayoutPanel7.Controls.Add(Me.SimulatedDistance_C1_ComboBox, 1, 1)
        Me.TableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel7.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel7.Name = "TableLayoutPanel7"
        Me.TableLayoutPanel7.RowCount = 3
        Me.TableLayoutPanel7.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel7.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel7.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel7.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel7.Size = New System.Drawing.Size(665, 367)
        Me.TableLayoutPanel7.TabIndex = 0
        '
        'Label9
        '
        Me.Label9.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label9.Location = New System.Drawing.Point(3, 0)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(381, 21)
        Me.Label9.TabIndex = 31
        Me.Label9.Text = "Directional simulation set"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'DirectionalSimulationSet_C1_ComboBox
        '
        Me.DirectionalSimulationSet_C1_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DirectionalSimulationSet_C1_ComboBox.FormattingEnabled = True
        Me.DirectionalSimulationSet_C1_ComboBox.Location = New System.Drawing.Point(387, 0)
        Me.DirectionalSimulationSet_C1_ComboBox.Margin = New System.Windows.Forms.Padding(0)
        Me.DirectionalSimulationSet_C1_ComboBox.Name = "DirectionalSimulationSet_C1_ComboBox"
        Me.DirectionalSimulationSet_C1_ComboBox.Size = New System.Drawing.Size(278, 21)
        Me.DirectionalSimulationSet_C1_ComboBox.TabIndex = 32
        '
        'Custom_SNC_TextBox
        '
        Me.Custom_SNC_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Custom_SNC_TextBox.Location = New System.Drawing.Point(390, 45)
        Me.Custom_SNC_TextBox.Multiline = True
        Me.Custom_SNC_TextBox.Name = "Custom_SNC_TextBox"
        Me.Custom_SNC_TextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.Custom_SNC_TextBox.Size = New System.Drawing.Size(272, 319)
        Me.Custom_SNC_TextBox.TabIndex = 34
        Me.Custom_SNC_TextBox.Text = "Z | Z" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "P | Z" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "0 | 0" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "90 | 0" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "60 | -60" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "-30 | 60"
        '
        'TextBox2
        '
        Me.TextBox2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TextBox2.Location = New System.Drawing.Point(3, 45)
        Me.TextBox2.Multiline = True
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.ReadOnly = True
        Me.TextBox2.Size = New System.Drawing.Size(381, 319)
        Me.TextBox2.TabIndex = 35
        Me.TextBox2.Text = resources.GetString("TextBox2.Text")
        '
        'Label10
        '
        Me.Label10.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label10.Location = New System.Drawing.Point(3, 21)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(381, 21)
        Me.Label10.TabIndex = 36
        Me.Label10.Text = "Simulated sound source distance"
        Me.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'SimulatedDistance_C1_ComboBox
        '
        Me.SimulatedDistance_C1_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SimulatedDistance_C1_ComboBox.FormattingEnabled = True
        Me.SimulatedDistance_C1_ComboBox.Location = New System.Drawing.Point(387, 21)
        Me.SimulatedDistance_C1_ComboBox.Margin = New System.Windows.Forms.Padding(0)
        Me.SimulatedDistance_C1_ComboBox.Name = "SimulatedDistance_C1_ComboBox"
        Me.SimulatedDistance_C1_ComboBox.Size = New System.Drawing.Size(278, 21)
        Me.SimulatedDistance_C1_ComboBox.TabIndex = 37
        '
        'DirectionalModeTabPage
        '
        Me.DirectionalModeTabPage.Controls.Add(Me.TableLayoutPanel3)
        Me.DirectionalModeTabPage.Location = New System.Drawing.Point(4, 22)
        Me.DirectionalModeTabPage.Name = "DirectionalModeTabPage"
        Me.DirectionalModeTabPage.Padding = New System.Windows.Forms.Padding(3)
        Me.DirectionalModeTabPage.Size = New System.Drawing.Size(665, 367)
        Me.DirectionalModeTabPage.TabIndex = 0
        Me.DirectionalModeTabPage.Text = "Directional mode"
        Me.DirectionalModeTabPage.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel3
        '
        Me.TableLayoutPanel3.ColumnCount = 2
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.82312!))
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 69.17688!))
        Me.TableLayoutPanel3.Controls.Add(Me.Label3, 0, 2)
        Me.TableLayoutPanel3.Controls.Add(Me.Label4, 0, 3)
        Me.TableLayoutPanel3.Controls.Add(Me.Label1, 0, 4)
        Me.TableLayoutPanel3.Controls.Add(Me.SpeechAzimuth_FlowLayoutPanel, 1, 2)
        Me.TableLayoutPanel3.Controls.Add(Me.MaskerAzimuth_FlowLayoutPanel, 1, 3)
        Me.TableLayoutPanel3.Controls.Add(Me.BackgroundAzimuth_FlowLayoutPanel, 1, 4)
        Me.TableLayoutPanel3.Controls.Add(Me.Label5, 0, 5)
        Me.TableLayoutPanel3.Controls.Add(Me.SimultaneousMaskersCount_ComboBox, 1, 5)
        Me.TableLayoutPanel3.Controls.Add(Me.DirectionalSimulationSet_Label, 0, 0)
        Me.TableLayoutPanel3.Controls.Add(Me.DirectionalSimulationSet_ComboBox, 1, 0)
        Me.TableLayoutPanel3.Controls.Add(Me.SimulatedDistance_Label, 0, 1)
        Me.TableLayoutPanel3.Controls.Add(Me.SimulatedDistance_ComboBox, 1, 1)
        Me.TableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel3.Location = New System.Drawing.Point(3, 3)
        Me.TableLayoutPanel3.Name = "TableLayoutPanel3"
        Me.TableLayoutPanel3.RowCount = 6
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel3.Size = New System.Drawing.Size(659, 361)
        Me.TableLayoutPanel3.TabIndex = 0
        '
        'Label3
        '
        Me.Label3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label3.Location = New System.Drawing.Point(3, 42)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(197, 99)
        Me.Label3.TabIndex = 17
        Me.Label3.Text = "Speech azimuths (degrees)"
        '
        'Label4
        '
        Me.Label4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label4.Location = New System.Drawing.Point(3, 141)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(197, 99)
        Me.Label4.TabIndex = 18
        Me.Label4.Text = "Masker azimuths (degrees)"
        '
        'Label1
        '
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Location = New System.Drawing.Point(3, 240)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(197, 99)
        Me.Label1.TabIndex = 23
        Me.Label1.Text = "Background azimuths"
        '
        'SpeechAzimuth_FlowLayoutPanel
        '
        Me.SpeechAzimuth_FlowLayoutPanel.AutoScroll = True
        Me.SpeechAzimuth_FlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SpeechAzimuth_FlowLayoutPanel.Location = New System.Drawing.Point(206, 45)
        Me.SpeechAzimuth_FlowLayoutPanel.Name = "SpeechAzimuth_FlowLayoutPanel"
        Me.SpeechAzimuth_FlowLayoutPanel.Size = New System.Drawing.Size(450, 93)
        Me.SpeechAzimuth_FlowLayoutPanel.TabIndex = 24
        '
        'MaskerAzimuth_FlowLayoutPanel
        '
        Me.MaskerAzimuth_FlowLayoutPanel.AutoScroll = True
        Me.MaskerAzimuth_FlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MaskerAzimuth_FlowLayoutPanel.Location = New System.Drawing.Point(206, 144)
        Me.MaskerAzimuth_FlowLayoutPanel.Name = "MaskerAzimuth_FlowLayoutPanel"
        Me.MaskerAzimuth_FlowLayoutPanel.Size = New System.Drawing.Size(450, 93)
        Me.MaskerAzimuth_FlowLayoutPanel.TabIndex = 25
        '
        'BackgroundAzimuth_FlowLayoutPanel
        '
        Me.BackgroundAzimuth_FlowLayoutPanel.AutoScroll = True
        Me.BackgroundAzimuth_FlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BackgroundAzimuth_FlowLayoutPanel.Location = New System.Drawing.Point(206, 243)
        Me.BackgroundAzimuth_FlowLayoutPanel.Name = "BackgroundAzimuth_FlowLayoutPanel"
        Me.BackgroundAzimuth_FlowLayoutPanel.Size = New System.Drawing.Size(450, 93)
        Me.BackgroundAzimuth_FlowLayoutPanel.TabIndex = 26
        '
        'Label5
        '
        Me.Label5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label5.Location = New System.Drawing.Point(3, 339)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(197, 22)
        Me.Label5.TabIndex = 19
        Me.Label5.Text = "Simultaneous maskers (n)"
        '
        'SimultaneousMaskersCount_ComboBox
        '
        Me.SimultaneousMaskersCount_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SimultaneousMaskersCount_ComboBox.FormattingEnabled = True
        Me.SimultaneousMaskersCount_ComboBox.Location = New System.Drawing.Point(203, 339)
        Me.SimultaneousMaskersCount_ComboBox.Margin = New System.Windows.Forms.Padding(0)
        Me.SimultaneousMaskersCount_ComboBox.Name = "SimultaneousMaskersCount_ComboBox"
        Me.SimultaneousMaskersCount_ComboBox.Size = New System.Drawing.Size(456, 21)
        Me.SimultaneousMaskersCount_ComboBox.TabIndex = 20
        '
        'DirectionalSimulationSet_Label
        '
        Me.DirectionalSimulationSet_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DirectionalSimulationSet_Label.Location = New System.Drawing.Point(3, 0)
        Me.DirectionalSimulationSet_Label.Name = "DirectionalSimulationSet_Label"
        Me.DirectionalSimulationSet_Label.Size = New System.Drawing.Size(197, 21)
        Me.DirectionalSimulationSet_Label.TabIndex = 27
        Me.DirectionalSimulationSet_Label.Text = "Directional simulation set"
        '
        'DirectionalSimulationSet_ComboBox
        '
        Me.DirectionalSimulationSet_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DirectionalSimulationSet_ComboBox.FormattingEnabled = True
        Me.DirectionalSimulationSet_ComboBox.Location = New System.Drawing.Point(203, 0)
        Me.DirectionalSimulationSet_ComboBox.Margin = New System.Windows.Forms.Padding(0)
        Me.DirectionalSimulationSet_ComboBox.Name = "DirectionalSimulationSet_ComboBox"
        Me.DirectionalSimulationSet_ComboBox.Size = New System.Drawing.Size(456, 21)
        Me.DirectionalSimulationSet_ComboBox.TabIndex = 28
        '
        'SimulatedDistance_Label
        '
        Me.SimulatedDistance_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SimulatedDistance_Label.Location = New System.Drawing.Point(3, 21)
        Me.SimulatedDistance_Label.Name = "SimulatedDistance_Label"
        Me.SimulatedDistance_Label.Size = New System.Drawing.Size(197, 21)
        Me.SimulatedDistance_Label.TabIndex = 29
        Me.SimulatedDistance_Label.Text = "Simulated sound source distance"
        '
        'SimulatedDistance_ComboBox
        '
        Me.SimulatedDistance_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SimulatedDistance_ComboBox.FormattingEnabled = True
        Me.SimulatedDistance_ComboBox.Location = New System.Drawing.Point(203, 21)
        Me.SimulatedDistance_ComboBox.Margin = New System.Windows.Forms.Padding(0)
        Me.SimulatedDistance_ComboBox.Name = "SimulatedDistance_ComboBox"
        Me.SimulatedDistance_ComboBox.Size = New System.Drawing.Size(456, 21)
        Me.SimulatedDistance_ComboBox.TabIndex = 30
        '
        'BmldModeTabPage
        '
        Me.BmldModeTabPage.Controls.Add(Me.TableLayoutPanel6)
        Me.BmldModeTabPage.Location = New System.Drawing.Point(4, 22)
        Me.BmldModeTabPage.Name = "BmldModeTabPage"
        Me.BmldModeTabPage.Padding = New System.Windows.Forms.Padding(3)
        Me.BmldModeTabPage.Size = New System.Drawing.Size(665, 367)
        Me.BmldModeTabPage.TabIndex = 1
        Me.BmldModeTabPage.Text = "BMLD mode"
        Me.BmldModeTabPage.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel6
        '
        Me.TableLayoutPanel6.ColumnCount = 2
        Me.TableLayoutPanel6.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.82312!))
        Me.TableLayoutPanel6.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 69.17688!))
        Me.TableLayoutPanel6.Controls.Add(Me.Label6, 0, 0)
        Me.TableLayoutPanel6.Controls.Add(Me.Label7, 0, 1)
        Me.TableLayoutPanel6.Controls.Add(Me.BmldSignalMode_ComboBox, 1, 0)
        Me.TableLayoutPanel6.Controls.Add(Me.BmldNoiseMode_ComboBox, 1, 1)
        Me.TableLayoutPanel6.Controls.Add(Me.Label8, 0, 2)
        Me.TableLayoutPanel6.Controls.Add(Me.BmldMode_RichTextBox, 1, 2)
        Me.TableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel6.Location = New System.Drawing.Point(3, 3)
        Me.TableLayoutPanel6.Name = "TableLayoutPanel6"
        Me.TableLayoutPanel6.RowCount = 4
        Me.TableLayoutPanel6.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel6.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel6.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
        Me.TableLayoutPanel6.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel6.Size = New System.Drawing.Size(659, 361)
        Me.TableLayoutPanel6.TabIndex = 0
        '
        'Label6
        '
        Me.Label6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label6.Location = New System.Drawing.Point(3, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(197, 21)
        Me.Label6.TabIndex = 0
        Me.Label6.Text = "Signal mode"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label7
        '
        Me.Label7.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label7.Location = New System.Drawing.Point(3, 21)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(197, 21)
        Me.Label7.TabIndex = 1
        Me.Label7.Text = "Noise mode"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BmldSignalMode_ComboBox
        '
        Me.BmldSignalMode_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BmldSignalMode_ComboBox.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BmldSignalMode_ComboBox.FormattingEnabled = True
        Me.BmldSignalMode_ComboBox.Location = New System.Drawing.Point(203, 0)
        Me.BmldSignalMode_ComboBox.Margin = New System.Windows.Forms.Padding(0)
        Me.BmldSignalMode_ComboBox.Name = "BmldSignalMode_ComboBox"
        Me.BmldSignalMode_ComboBox.Size = New System.Drawing.Size(456, 22)
        Me.BmldSignalMode_ComboBox.TabIndex = 2
        '
        'BmldNoiseMode_ComboBox
        '
        Me.BmldNoiseMode_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BmldNoiseMode_ComboBox.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BmldNoiseMode_ComboBox.FormattingEnabled = True
        Me.BmldNoiseMode_ComboBox.Location = New System.Drawing.Point(203, 21)
        Me.BmldNoiseMode_ComboBox.Margin = New System.Windows.Forms.Padding(0)
        Me.BmldNoiseMode_ComboBox.Name = "BmldNoiseMode_ComboBox"
        Me.BmldNoiseMode_ComboBox.Size = New System.Drawing.Size(456, 22)
        Me.BmldNoiseMode_ComboBox.TabIndex = 3
        '
        'Label8
        '
        Me.Label8.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label8.Location = New System.Drawing.Point(3, 42)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(197, 40)
        Me.Label8.TabIndex = 4
        Me.Label8.Text = "Selected BMLD mode"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'BmldMode_RichTextBox
        '
        Me.BmldMode_RichTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BmldMode_RichTextBox.Location = New System.Drawing.Point(206, 45)
        Me.BmldMode_RichTextBox.Name = "BmldMode_RichTextBox"
        Me.BmldMode_RichTextBox.Size = New System.Drawing.Size(450, 34)
        Me.BmldMode_RichTextBox.TabIndex = 5
        Me.BmldMode_RichTextBox.Text = ""
        '
        'ExportTrialSounds_CheckBox
        '
        Me.ExportTrialSounds_CheckBox.AutoSize = True
        Me.ExportTrialSounds_CheckBox.Checked = True
        Me.ExportTrialSounds_CheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ExportTrialSounds_CheckBox.Location = New System.Drawing.Point(166, 725)
        Me.ExportTrialSounds_CheckBox.Name = "ExportTrialSounds_CheckBox"
        Me.ExportTrialSounds_CheckBox.Size = New System.Drawing.Size(132, 16)
        Me.ExportTrialSounds_CheckBox.TabIndex = 30
        Me.ExportTrialSounds_CheckBox.Text = "Export test trial sounds"
        Me.ExportTrialSounds_CheckBox.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel5
        '
        Me.TableLayoutPanel5.ColumnCount = 1
        Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel5.Controls.Add(Me.CompletedTests_Label, 0, 0)
        Me.TableLayoutPanel5.Controls.Add(Me.CurrentSessionResults_DataGridView, 0, 1)
        Me.TableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel5.Location = New System.Drawing.Point(1243, 3)
        Me.TableLayoutPanel5.Name = "TableLayoutPanel5"
        Me.TableLayoutPanel5.RowCount = 2
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel5.Size = New System.Drawing.Size(587, 744)
        Me.TableLayoutPanel5.TabIndex = 3
        '
        'CompletedTests_Label
        '
        Me.CompletedTests_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CompletedTests_Label.Location = New System.Drawing.Point(3, 0)
        Me.CompletedTests_Label.Name = "CompletedTests_Label"
        Me.CompletedTests_Label.Size = New System.Drawing.Size(581, 21)
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
        DataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.CurrentSessionResults_DataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle6
        Me.CurrentSessionResults_DataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.CurrentSessionResults_DataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.TestDescriptionColumnSession, Me.TestLengthColumnSession, Me.ResultColumnSession, Me.CompareColumnSession})
        DataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle7.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.CurrentSessionResults_DataGridView.DefaultCellStyle = DataGridViewCellStyle7
        Me.CurrentSessionResults_DataGridView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CurrentSessionResults_DataGridView.Location = New System.Drawing.Point(3, 24)
        Me.CurrentSessionResults_DataGridView.MultiSelect = False
        Me.CurrentSessionResults_DataGridView.Name = "CurrentSessionResults_DataGridView"
        DataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle8.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.CurrentSessionResults_DataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle8
        Me.CurrentSessionResults_DataGridView.RowHeadersVisible = False
        Me.CurrentSessionResults_DataGridView.Size = New System.Drawing.Size(581, 717)
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
        'SipTestGui_2023
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1839, 882)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "SipTestGui_2023"
        Me.Text = "SipTestGui_2023"
        Me.Top_TableLayoutPanel.ResumeLayout(False)
        Me.SoundSettings_TableLayoutPanel.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.TableLayoutPanel2.PerformLayout()
        Me.Screen_TableLayoutPanel.ResumeLayout(False)
        Me.Screen_TableLayoutPanel.PerformLayout()
        Me.PcScreen_TableLayoutPanel.ResumeLayout(False)
        Me.PcScreen_TableLayoutPanel.PerformLayout()
        Me.BtScreen_TableLayoutPanel.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.Test_TableLayoutPanel.ResumeLayout(False)
        Me.TableLayoutPanel4.ResumeLayout(False)
        Me.TableLayoutPanel4.PerformLayout()
        CType(Me.TestTrialDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.KeyboardShortcutContainer_Panel.ResumeLayout(False)
        Me.KeybordShortcut_TableLayoutPanel.ResumeLayout(False)
        Me.TestSettings_TableLayoutPanel.ResumeLayout(False)
        Me.TestSettings_TableLayoutPanel.PerformLayout()
        Me.TestMode_TabControl.ResumeLayout(False)
        Me.BinauralSettings_TabPage.ResumeLayout(False)
        Me.TableLayoutPanel7.ResumeLayout(False)
        Me.TableLayoutPanel7.PerformLayout()
        Me.DirectionalModeTabPage.ResumeLayout(False)
        Me.TableLayoutPanel3.ResumeLayout(False)
        Me.BmldModeTabPage.ResumeLayout(False)
        Me.TableLayoutPanel6.ResumeLayout(False)
        Me.TableLayoutPanel5.ResumeLayout(False)
        CType(Me.CurrentSessionResults_DataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Top_TableLayoutPanel As Windows.Forms.TableLayoutPanel
    Friend WithEvents ParticipantIdTextBox As Windows.Forms.TextBox
    Friend WithEvents ParticipantLock_Button As Windows.Forms.Button
    Friend WithEvents SoundSettings_TableLayoutPanel As Windows.Forms.TableLayoutPanel
    Friend WithEvents SelectTransducer_Label As Windows.Forms.Label
    Friend WithEvents Transducer_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents MenuStrip1 As Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents TableLayoutPanel1 As Windows.Forms.TableLayoutPanel
    Friend WithEvents TableLayoutPanel2 As Windows.Forms.TableLayoutPanel
    Friend WithEvents Label2 As Windows.Forms.Label
    Friend WithEvents Panel1 As Windows.Forms.Panel
    Friend WithEvents Operation_ProgressBarWithText As ProgressBarWithText
    Friend WithEvents Test_TableLayoutPanel As Windows.Forms.TableLayoutPanel
    Friend WithEvents TestSettings_TableLayoutPanel As Windows.Forms.TableLayoutPanel
    Friend WithEvents TestLength_Label As Windows.Forms.Label
    Friend WithEvents LengthReduplications_Label As Windows.Forms.Label
    Friend WithEvents ReferenceLevel_Label As Windows.Forms.Label
    Friend WithEvents ReferenceLevelComboBox As Windows.Forms.ComboBox
    Friend WithEvents PNR_Label As Windows.Forms.Label
    Friend WithEvents Preset_Label As Windows.Forms.Label
    Friend WithEvents PresetComboBox As Windows.Forms.ComboBox
    Friend WithEvents Situation_Label As Windows.Forms.Label
    Friend WithEvents PlannedTestLength_TextBox As Windows.Forms.TextBox
    Friend WithEvents TestLengthComboBox As Windows.Forms.ComboBox
    Friend WithEvents TableLayoutPanel4 As Windows.Forms.TableLayoutPanel
    Friend WithEvents ProportionCorrectTextBox As Windows.Forms.TextBox
    Friend WithEvents MeasurementProgressBar As Windows.Forms.ProgressBar
    Friend WithEvents TestTrialDataGridView As Windows.Forms.DataGridView
    Friend WithEvents TestWordColumn As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ResponseColumn As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ResultColumn As Windows.Forms.DataGridViewImageColumn
    Friend WithEvents CorrectCount_Label As Windows.Forms.Label
    Friend WithEvents ProportionCorrect_Label As Windows.Forms.Label
    Friend WithEvents Start_AudioButton As WinFormControls.AudioButton
    Friend WithEvents CorrectCountTextBox As Windows.Forms.TextBox
    Friend WithEvents TestDescriptionTextBox As Windows.Forms.TextBox
    Friend WithEvents TestDescription_Label As Windows.Forms.Label
    Friend WithEvents RandomSeed_Label As Windows.Forms.Label
    Friend WithEvents RandomSeed_IntegerParsingTextBox As IntegerParsingTextBox
    Friend WithEvents Stop_AudioButton As WinFormControls.AudioButton
    Friend WithEvents KeyboardShortcutContainer_Panel As Windows.Forms.Panel
    Friend WithEvents KeybordShortcut_TableLayoutPanel As Windows.Forms.TableLayoutPanel
    Friend WithEvents Pause_Label As Windows.Forms.Label
    Friend WithEvents Resume_Label As Windows.Forms.Label
    Friend WithEvents Stop_Label As Windows.Forms.Label
    Friend WithEvents KeyBoardShortcut_Label As Windows.Forms.Label
    Friend WithEvents Label3 As Windows.Forms.Label
    Friend WithEvents Label4 As Windows.Forms.Label
    Friend WithEvents Label5 As Windows.Forms.Label
    Friend WithEvents SimultaneousMaskersCount_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents Screen_TableLayoutPanel As Windows.Forms.TableLayoutPanel
    Friend WithEvents PcScreen_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents BtScreen_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents PcScreen_TableLayoutPanel As Windows.Forms.TableLayoutPanel
    Friend WithEvents PcScreen_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents PcTouch_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents BtScreen_TableLayoutPanel As Windows.Forms.TableLayoutPanel
    Friend WithEvents ConnectBluetoothScreen_Button As Windows.Forms.Button
    Friend WithEvents DisconnectBtScreen_Button As Windows.Forms.Button
    Friend WithEvents BtLamp As Lamp
    Friend WithEvents Testparadigm_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents Testparadigm_Label As Windows.Forms.Label
    Friend WithEvents AboutToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExportResultsToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents TableLayoutPanel5 As Windows.Forms.TableLayoutPanel
    Friend WithEvents CompletedTests_Label As Windows.Forms.Label
    Friend WithEvents CurrentSessionResults_DataGridView As Windows.Forms.DataGridView
    Friend WithEvents TestDescriptionColumnSession As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents TestLengthColumnSession As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ResultColumnSession As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents CompareColumnSession As Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents SpeechAzimuth_FlowLayoutPanel As Windows.Forms.FlowLayoutPanel
    Friend WithEvents MaskerAzimuth_FlowLayoutPanel As Windows.Forms.FlowLayoutPanel
    Friend WithEvents BackgroundAzimuth_FlowLayoutPanel As Windows.Forms.FlowLayoutPanel
    Friend WithEvents Situations_FlowLayoutPanel As Windows.Forms.FlowLayoutPanel
    Friend WithEvents PNRs_FlowLayoutPanel As Windows.Forms.FlowLayoutPanel
    Friend WithEvents TestMode_TabControl As Windows.Forms.TabControl
    Friend WithEvents DirectionalModeTabPage As Windows.Forms.TabPage
    Friend WithEvents BmldModeTabPage As Windows.Forms.TabPage
    Friend WithEvents TableLayoutPanel3 As Windows.Forms.TableLayoutPanel
    Friend WithEvents TableLayoutPanel6 As Windows.Forms.TableLayoutPanel
    Friend WithEvents Label6 As Windows.Forms.Label
    Friend WithEvents Label7 As Windows.Forms.Label
    Friend WithEvents BmldSignalMode_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents BmldNoiseMode_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents Label8 As Windows.Forms.Label
    Friend WithEvents BmldMode_RichTextBox As Windows.Forms.RichTextBox
    Friend WithEvents DirectionalSimulationSet_Label As Windows.Forms.Label
    Friend WithEvents DirectionalSimulationSet_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents SimulatedDistance_Label As Windows.Forms.Label
    Friend WithEvents SimulatedDistance_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents BinauralSettings_TabPage As Windows.Forms.TabPage
    Friend WithEvents TableLayoutPanel7 As Windows.Forms.TableLayoutPanel
    Friend WithEvents Label9 As Windows.Forms.Label
    Friend WithEvents DirectionalSimulationSet_C1_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents Custom_SNC_TextBox As Windows.Forms.TextBox
    Friend WithEvents TextBox2 As Windows.Forms.TextBox
    Friend WithEvents ExportTrialSounds_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents ScrollToPresented_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents PauseOnNextNonTrial_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents Label10 As Windows.Forms.Label
    Friend WithEvents SimulatedDistance_C1_ComboBox As Windows.Forms.ComboBox
End Class
