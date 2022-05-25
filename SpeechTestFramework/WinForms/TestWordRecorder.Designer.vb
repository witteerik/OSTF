<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class SpeechMaterialRecorder
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
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RecordingSettingsMenu = New System.Windows.Forms.ToolStripMenuItem()
        Me.FontSizeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.IncreaseFontSizeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DecreaseFontSizeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SetFontsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SetFontOfSpellingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SetFontOfPhoneticTranscriptionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AuditoryPrequeingToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StartRecordingAutomaticallyOnNextpreviousToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToggleBackgroundSoundWhileRecordingonoffToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToggleSoundLevelMeteronoffToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SegmentationSettingsMenu = New System.Windows.Forms.ToolStripMenuItem()
        Me.SpectrogramSettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AudioSettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.IOSettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CalibrateOutputLevelToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SelectTransducerTypeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SegmentationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.WordBoundaryDetectionToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AutoDetectBoundariesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BoundaryDetectionSettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.UpdateAllSegmentationsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ValidateAllSegmentationsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FadeAllPaddingSectionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MoveSegmentationsToZeroCrossingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ResetAllSegmentationDataToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.preQueLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel2 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.AutoRecordingStatusLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.BackgroundSoundStatusLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel3 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.SoundFilePathStatusLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.MainTabControl = New System.Windows.Forms.TabControl()
        Me.RecordingTab = New System.Windows.Forms.TabPage()
        Me.RecordingTabMainSplitContainer = New System.Windows.Forms.SplitContainer()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.TopRecordingControlPanel = New System.Windows.Forms.Panel()
        Me.TableLayoutPanel3 = New System.Windows.Forms.TableLayoutPanel()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.RecordingLabel = New System.Windows.Forms.Label()
        Me.TableLayoutPanel4 = New System.Windows.Forms.TableLayoutPanel()
        Me.Rec_PreviousItemButton = New System.Windows.Forms.Button()
        Me.Rec_PreviousNRItemButton = New System.Windows.Forms.Button()
        Me.Rec_NextItemButton = New System.Windows.Forms.Button()
        Me.Rec_NextNRItemButton = New System.Windows.Forms.Button()
        Me.StartRecordingButton = New System.Windows.Forms.Button()
        Me.ListenButton = New System.Windows.Forms.Button()
        Me.StopRecordingButton = New System.Windows.Forms.Button()
        Me.SegmentationTab = New System.Windows.Forms.TabPage()
        Me.SegmentationPanel = New System.Windows.Forms.Panel()
        Me.ItemProgressBar = New System.Windows.Forms.ProgressBar()
        Me.ItemProgressLabel = New System.Windows.Forms.Label()
        Me.ItemComboBox = New System.Windows.Forms.ComboBox()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Top_Id_Label = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Top_Spelling_Label = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Top_Transcription_Label = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Top_PreviousItemButton = New System.Windows.Forms.Button()
        Me.Top_NextItemButton = New System.Windows.Forms.Button()
        Me.MainSplitContainer = New System.Windows.Forms.SplitContainer()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.RecordingSoundLevelMeter = New SpeechTestFramework.Audio.Graphics.SoundLevelMeter()
        Me.AutoHeightTextBox1 = New SpeechTestFramework.AutoHeightTextBox()
        Me.Spelling_AutoHeightTextBox = New SpeechTestFramework.AutoHeightTextBox()
        Me.LoadWaveFileSMAIXMLChunkRequiredToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveWaveFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CloseToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveWaveFileAsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuStrip1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.MainTabControl.SuspendLayout()
        Me.RecordingTab.SuspendLayout()
        CType(Me.RecordingTabMainSplitContainer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.RecordingTabMainSplitContainer.Panel1.SuspendLayout()
        Me.RecordingTabMainSplitContainer.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.TopRecordingControlPanel.SuspendLayout()
        Me.TableLayoutPanel3.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.TableLayoutPanel4.SuspendLayout()
        Me.SegmentationTab.SuspendLayout()
        Me.FlowLayoutPanel1.SuspendLayout()
        CType(Me.MainSplitContainer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MainSplitContainer.Panel1.SuspendLayout()
        Me.MainSplitContainer.Panel2.SuspendLayout()
        Me.MainSplitContainer.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        CType(Me.RecordingSoundLevelMeter, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.RecordingSettingsMenu, Me.SegmentationSettingsMenu, Me.AudioSettingsToolStripMenuItem, Me.SegmentationToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        Me.MenuStrip1.Size = New System.Drawing.Size(1012, 24)
        Me.MenuStrip1.TabIndex = 0
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LoadWaveFileSMAIXMLChunkRequiredToolStripMenuItem, Me.SaveWaveFileAsToolStripMenuItem, Me.SaveWaveFileToolStripMenuItem, Me.CloseToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.FileToolStripMenuItem.Text = "File"
        '
        'RecordingSettingsMenu
        '
        Me.RecordingSettingsMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FontSizeToolStripMenuItem, Me.SetFontsToolStripMenuItem, Me.AuditoryPrequeingToolStripMenuItem, Me.StartRecordingAutomaticallyOnNextpreviousToolStripMenuItem, Me.ToggleBackgroundSoundWhileRecordingonoffToolStripMenuItem, Me.ToggleSoundLevelMeteronoffToolStripMenuItem})
        Me.RecordingSettingsMenu.Name = "RecordingSettingsMenu"
        Me.RecordingSettingsMenu.Size = New System.Drawing.Size(61, 20)
        Me.RecordingSettingsMenu.Text = "Settings"
        '
        'FontSizeToolStripMenuItem
        '
        Me.FontSizeToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.IncreaseFontSizeToolStripMenuItem, Me.DecreaseFontSizeToolStripMenuItem})
        Me.FontSizeToolStripMenuItem.Name = "FontSizeToolStripMenuItem"
        Me.FontSizeToolStripMenuItem.Size = New System.Drawing.Size(342, 22)
        Me.FontSizeToolStripMenuItem.Text = "Font size"
        '
        'IncreaseFontSizeToolStripMenuItem
        '
        Me.IncreaseFontSizeToolStripMenuItem.Name = "IncreaseFontSizeToolStripMenuItem"
        Me.IncreaseFontSizeToolStripMenuItem.Size = New System.Drawing.Size(168, 22)
        Me.IncreaseFontSizeToolStripMenuItem.Text = "Increase font size"
        '
        'DecreaseFontSizeToolStripMenuItem
        '
        Me.DecreaseFontSizeToolStripMenuItem.Name = "DecreaseFontSizeToolStripMenuItem"
        Me.DecreaseFontSizeToolStripMenuItem.Size = New System.Drawing.Size(168, 22)
        Me.DecreaseFontSizeToolStripMenuItem.Text = "Decrease font size"
        '
        'SetFontsToolStripMenuItem
        '
        Me.SetFontsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SetFontOfSpellingsToolStripMenuItem, Me.SetFontOfPhoneticTranscriptionsToolStripMenuItem})
        Me.SetFontsToolStripMenuItem.Name = "SetFontsToolStripMenuItem"
        Me.SetFontsToolStripMenuItem.Size = New System.Drawing.Size(342, 22)
        Me.SetFontsToolStripMenuItem.Text = "Set fonts"
        '
        'SetFontOfSpellingsToolStripMenuItem
        '
        Me.SetFontOfSpellingsToolStripMenuItem.Name = "SetFontOfSpellingsToolStripMenuItem"
        Me.SetFontOfSpellingsToolStripMenuItem.Size = New System.Drawing.Size(258, 22)
        Me.SetFontOfSpellingsToolStripMenuItem.Text = "Set font for spellings"
        '
        'SetFontOfPhoneticTranscriptionsToolStripMenuItem
        '
        Me.SetFontOfPhoneticTranscriptionsToolStripMenuItem.Name = "SetFontOfPhoneticTranscriptionsToolStripMenuItem"
        Me.SetFontOfPhoneticTranscriptionsToolStripMenuItem.Size = New System.Drawing.Size(258, 22)
        Me.SetFontOfPhoneticTranscriptionsToolStripMenuItem.Text = "Set font for phonetic transcriptions"
        '
        'AuditoryPrequeingToolStripMenuItem
        '
        Me.AuditoryPrequeingToolStripMenuItem.Name = "AuditoryPrequeingToolStripMenuItem"
        Me.AuditoryPrequeingToolStripMenuItem.Size = New System.Drawing.Size(342, 22)
        Me.AuditoryPrequeingToolStripMenuItem.Text = "Toggle auditory pre-queing (on/off)"
        '
        'StartRecordingAutomaticallyOnNextpreviousToolStripMenuItem
        '
        Me.StartRecordingAutomaticallyOnNextpreviousToolStripMenuItem.Name = "StartRecordingAutomaticallyOnNextpreviousToolStripMenuItem"
        Me.StartRecordingAutomaticallyOnNextpreviousToolStripMenuItem.Size = New System.Drawing.Size(342, 22)
        Me.StartRecordingAutomaticallyOnNextpreviousToolStripMenuItem.Text = "Toggle auto-recording on next/previous (on/off)"
        '
        'ToggleBackgroundSoundWhileRecordingonoffToolStripMenuItem
        '
        Me.ToggleBackgroundSoundWhileRecordingonoffToolStripMenuItem.Name = "ToggleBackgroundSoundWhileRecordingonoffToolStripMenuItem"
        Me.ToggleBackgroundSoundWhileRecordingonoffToolStripMenuItem.Size = New System.Drawing.Size(342, 22)
        Me.ToggleBackgroundSoundWhileRecordingonoffToolStripMenuItem.Text = "Toggle background sound while recording (on/off)"
        '
        'ToggleSoundLevelMeteronoffToolStripMenuItem
        '
        Me.ToggleSoundLevelMeteronoffToolStripMenuItem.Name = "ToggleSoundLevelMeteronoffToolStripMenuItem"
        Me.ToggleSoundLevelMeteronoffToolStripMenuItem.Size = New System.Drawing.Size(342, 22)
        Me.ToggleSoundLevelMeteronoffToolStripMenuItem.Text = "Toggle sound level meter (on/off)"
        '
        'SegmentationSettingsMenu
        '
        Me.SegmentationSettingsMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SpectrogramSettingsToolStripMenuItem})
        Me.SegmentationSettingsMenu.Name = "SegmentationSettingsMenu"
        Me.SegmentationSettingsMenu.Size = New System.Drawing.Size(61, 20)
        Me.SegmentationSettingsMenu.Text = "Settings"
        Me.SegmentationSettingsMenu.Visible = False
        '
        'SpectrogramSettingsToolStripMenuItem
        '
        Me.SpectrogramSettingsToolStripMenuItem.Name = "SpectrogramSettingsToolStripMenuItem"
        Me.SpectrogramSettingsToolStripMenuItem.Size = New System.Drawing.Size(186, 22)
        Me.SpectrogramSettingsToolStripMenuItem.Text = "Spectrogram settings"
        '
        'AudioSettingsToolStripMenuItem
        '
        Me.AudioSettingsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.IOSettingsToolStripMenuItem, Me.CalibrateOutputLevelToolStripMenuItem, Me.SelectTransducerTypeToolStripMenuItem})
        Me.AudioSettingsToolStripMenuItem.Name = "AudioSettingsToolStripMenuItem"
        Me.AudioSettingsToolStripMenuItem.Size = New System.Drawing.Size(95, 20)
        Me.AudioSettingsToolStripMenuItem.Text = "Audio settings"
        '
        'IOSettingsToolStripMenuItem
        '
        Me.IOSettingsToolStripMenuItem.Name = "IOSettingsToolStripMenuItem"
        Me.IOSettingsToolStripMenuItem.Size = New System.Drawing.Size(190, 22)
        Me.IOSettingsToolStripMenuItem.Text = "I/O settings"
        '
        'CalibrateOutputLevelToolStripMenuItem
        '
        Me.CalibrateOutputLevelToolStripMenuItem.Name = "CalibrateOutputLevelToolStripMenuItem"
        Me.CalibrateOutputLevelToolStripMenuItem.Size = New System.Drawing.Size(190, 22)
        Me.CalibrateOutputLevelToolStripMenuItem.Text = "Calibrate output level"
        '
        'SelectTransducerTypeToolStripMenuItem
        '
        Me.SelectTransducerTypeToolStripMenuItem.Name = "SelectTransducerTypeToolStripMenuItem"
        Me.SelectTransducerTypeToolStripMenuItem.Size = New System.Drawing.Size(190, 22)
        Me.SelectTransducerTypeToolStripMenuItem.Text = "Select transducer type"
        '
        'SegmentationToolStripMenuItem
        '
        Me.SegmentationToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.WordBoundaryDetectionToolStripMenuItem, Me.UpdateAllSegmentationsToolStripMenuItem, Me.ValidateAllSegmentationsToolStripMenuItem, Me.FadeAllPaddingSectionsToolStripMenuItem, Me.MoveSegmentationsToZeroCrossingsToolStripMenuItem, Me.ResetAllSegmentationDataToolStripMenuItem})
        Me.SegmentationToolStripMenuItem.Name = "SegmentationToolStripMenuItem"
        Me.SegmentationToolStripMenuItem.Size = New System.Drawing.Size(93, 20)
        Me.SegmentationToolStripMenuItem.Text = "Segmentation"
        Me.SegmentationToolStripMenuItem.Visible = False
        '
        'WordBoundaryDetectionToolStripMenuItem
        '
        Me.WordBoundaryDetectionToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AutoDetectBoundariesToolStripMenuItem, Me.BoundaryDetectionSettingsToolStripMenuItem})
        Me.WordBoundaryDetectionToolStripMenuItem.Name = "WordBoundaryDetectionToolStripMenuItem"
        Me.WordBoundaryDetectionToolStripMenuItem.Size = New System.Drawing.Size(276, 22)
        Me.WordBoundaryDetectionToolStripMenuItem.Text = "Speech signal boundary detection"
        '
        'AutoDetectBoundariesToolStripMenuItem
        '
        Me.AutoDetectBoundariesToolStripMenuItem.Name = "AutoDetectBoundariesToolStripMenuItem"
        Me.AutoDetectBoundariesToolStripMenuItem.Size = New System.Drawing.Size(222, 22)
        Me.AutoDetectBoundariesToolStripMenuItem.Text = "Auto-detect boundaries"
        '
        'BoundaryDetectionSettingsToolStripMenuItem
        '
        Me.BoundaryDetectionSettingsToolStripMenuItem.Name = "BoundaryDetectionSettingsToolStripMenuItem"
        Me.BoundaryDetectionSettingsToolStripMenuItem.Size = New System.Drawing.Size(222, 22)
        Me.BoundaryDetectionSettingsToolStripMenuItem.Text = "Boundary detection settings"
        '
        'UpdateAllSegmentationsToolStripMenuItem
        '
        Me.UpdateAllSegmentationsToolStripMenuItem.Name = "UpdateAllSegmentationsToolStripMenuItem"
        Me.UpdateAllSegmentationsToolStripMenuItem.Size = New System.Drawing.Size(276, 22)
        Me.UpdateAllSegmentationsToolStripMenuItem.Text = "Update all segmentations"
        '
        'ValidateAllSegmentationsToolStripMenuItem
        '
        Me.ValidateAllSegmentationsToolStripMenuItem.Name = "ValidateAllSegmentationsToolStripMenuItem"
        Me.ValidateAllSegmentationsToolStripMenuItem.Size = New System.Drawing.Size(276, 22)
        Me.ValidateAllSegmentationsToolStripMenuItem.Text = "Validate all segmentations"
        '
        'FadeAllPaddingSectionsToolStripMenuItem
        '
        Me.FadeAllPaddingSectionsToolStripMenuItem.Name = "FadeAllPaddingSectionsToolStripMenuItem"
        Me.FadeAllPaddingSectionsToolStripMenuItem.Size = New System.Drawing.Size(276, 22)
        Me.FadeAllPaddingSectionsToolStripMenuItem.Text = "Fade all padding sections"
        '
        'MoveSegmentationsToZeroCrossingsToolStripMenuItem
        '
        Me.MoveSegmentationsToZeroCrossingsToolStripMenuItem.Name = "MoveSegmentationsToZeroCrossingsToolStripMenuItem"
        Me.MoveSegmentationsToZeroCrossingsToolStripMenuItem.Size = New System.Drawing.Size(276, 22)
        Me.MoveSegmentationsToZeroCrossingsToolStripMenuItem.Text = "Move segmentations to zero crossings"
        '
        'ResetAllSegmentationDataToolStripMenuItem
        '
        Me.ResetAllSegmentationDataToolStripMenuItem.Name = "ResetAllSegmentationDataToolStripMenuItem"
        Me.ResetAllSegmentationDataToolStripMenuItem.Size = New System.Drawing.Size(276, 22)
        Me.ResetAllSegmentationDataToolStripMenuItem.Text = "Reset all segmentation data"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.preQueLabel, Me.ToolStripStatusLabel2, Me.AutoRecordingStatusLabel, Me.ToolStripStatusLabel1, Me.BackgroundSoundStatusLabel, Me.ToolStripStatusLabel3, Me.SoundFilePathStatusLabel})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 479)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(1012, 22)
        Me.StatusStrip1.TabIndex = 1
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'preQueLabel
        '
        Me.preQueLabel.Name = "preQueLabel"
        Me.preQueLabel.Size = New System.Drawing.Size(89, 17)
        Me.preQueLabel.Text = "Pre-queing: Off"
        '
        'ToolStripStatusLabel2
        '
        Me.ToolStripStatusLabel2.Name = "ToolStripStatusLabel2"
        Me.ToolStripStatusLabel2.Size = New System.Drawing.Size(10, 17)
        Me.ToolStripStatusLabel2.Text = "|"
        '
        'AutoRecordingStatusLabel
        '
        Me.AutoRecordingStatusLabel.Name = "AutoRecordingStatusLabel"
        Me.AutoRecordingStatusLabel.Size = New System.Drawing.Size(111, 17)
        Me.AutoRecordingStatusLabel.Text = "Auto-recording: On"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(10, 17)
        Me.ToolStripStatusLabel1.Text = "|"
        '
        'BackgroundSoundStatusLabel
        '
        Me.BackgroundSoundStatusLabel.Name = "BackgroundSoundStatusLabel"
        Me.BackgroundSoundStatusLabel.Size = New System.Drawing.Size(128, 17)
        Me.BackgroundSoundStatusLabel.Text = "Background sound: off"
        '
        'ToolStripStatusLabel3
        '
        Me.ToolStripStatusLabel3.Name = "ToolStripStatusLabel3"
        Me.ToolStripStatusLabel3.Size = New System.Drawing.Size(10, 17)
        Me.ToolStripStatusLabel3.Text = "|"
        '
        'SoundFilePathStatusLabel
        '
        Me.SoundFilePathStatusLabel.Name = "SoundFilePathStatusLabel"
        Me.SoundFilePathStatusLabel.Size = New System.Drawing.Size(66, 17)
        Me.SoundFilePathStatusLabel.Text = "Sound file: "
        '
        'MainTabControl
        '
        Me.MainTabControl.Controls.Add(Me.RecordingTab)
        Me.MainTabControl.Controls.Add(Me.SegmentationTab)
        Me.MainTabControl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MainTabControl.Location = New System.Drawing.Point(0, 0)
        Me.MainTabControl.Name = "MainTabControl"
        Me.MainTabControl.SelectedIndex = 0
        Me.MainTabControl.Size = New System.Drawing.Size(1012, 374)
        Me.MainTabControl.TabIndex = 2
        '
        'RecordingTab
        '
        Me.RecordingTab.Controls.Add(Me.RecordingTabMainSplitContainer)
        Me.RecordingTab.Location = New System.Drawing.Point(4, 22)
        Me.RecordingTab.Name = "RecordingTab"
        Me.RecordingTab.Padding = New System.Windows.Forms.Padding(3)
        Me.RecordingTab.Size = New System.Drawing.Size(1004, 348)
        Me.RecordingTab.TabIndex = 0
        Me.RecordingTab.Text = "Recording"
        Me.RecordingTab.UseVisualStyleBackColor = True
        '
        'RecordingTabMainSplitContainer
        '
        Me.RecordingTabMainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RecordingTabMainSplitContainer.Location = New System.Drawing.Point(3, 3)
        Me.RecordingTabMainSplitContainer.Name = "RecordingTabMainSplitContainer"
        '
        'RecordingTabMainSplitContainer.Panel1
        '
        Me.RecordingTabMainSplitContainer.Panel1.Controls.Add(Me.TableLayoutPanel1)
        Me.RecordingTabMainSplitContainer.Size = New System.Drawing.Size(998, 342)
        Me.RecordingTabMainSplitContainer.SplitterDistance = 648
        Me.RecordingTabMainSplitContainer.TabIndex = 0
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 4
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33332!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 201.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.RecordingSoundLevelMeter, 3, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.TopRecordingControlPanel, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.StartRecordingButton, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.ListenButton, 2, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.StopRecordingButton, 1, 1)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(648, 342)
        Me.TableLayoutPanel1.TabIndex = 3
        '
        'TopRecordingControlPanel
        '
        Me.TopRecordingControlPanel.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.TopRecordingControlPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TableLayoutPanel1.SetColumnSpan(Me.TopRecordingControlPanel, 3)
        Me.TopRecordingControlPanel.Controls.Add(Me.TableLayoutPanel3)
        Me.TopRecordingControlPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TopRecordingControlPanel.Location = New System.Drawing.Point(3, 3)
        Me.TopRecordingControlPanel.Name = "TopRecordingControlPanel"
        Me.TopRecordingControlPanel.Size = New System.Drawing.Size(440, 296)
        Me.TopRecordingControlPanel.TabIndex = 0
        '
        'TableLayoutPanel3
        '
        Me.TableLayoutPanel3.ColumnCount = 1
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel3.Controls.Add(Me.TableLayoutPanel2, 0, 0)
        Me.TableLayoutPanel3.Controls.Add(Me.TableLayoutPanel4, 0, 1)
        Me.TableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel3.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel3.Name = "TableLayoutPanel3"
        Me.TableLayoutPanel3.RowCount = 2
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.72603!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 85.27397!))
        Me.TableLayoutPanel3.Size = New System.Drawing.Size(436, 292)
        Me.TableLayoutPanel3.TabIndex = 15
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.ColumnCount = 3
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel2.Controls.Add(Me.RecordingLabel, 1, 0)
        Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(3, 3)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 1
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(430, 37)
        Me.TableLayoutPanel2.TabIndex = 14
        '
        'RecordingLabel
        '
        Me.RecordingLabel.BackColor = System.Drawing.Color.DarkGray
        Me.RecordingLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.RecordingLabel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RecordingLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RecordingLabel.ForeColor = System.Drawing.Color.Blue
        Me.RecordingLabel.Location = New System.Drawing.Point(146, 4)
        Me.RecordingLabel.Margin = New System.Windows.Forms.Padding(3, 4, 3, 3)
        Me.RecordingLabel.Name = "RecordingLabel"
        Me.RecordingLabel.Size = New System.Drawing.Size(137, 30)
        Me.RecordingLabel.TabIndex = 7
        Me.RecordingLabel.Text = "Not recording"
        Me.RecordingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TableLayoutPanel4
        '
        Me.TableLayoutPanel4.ColumnCount = 3
        Me.TableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90.0!))
        Me.TableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90.0!))
        Me.TableLayoutPanel4.Controls.Add(Me.AutoHeightTextBox1, 1, 1)
        Me.TableLayoutPanel4.Controls.Add(Me.Spelling_AutoHeightTextBox, 1, 0)
        Me.TableLayoutPanel4.Controls.Add(Me.Rec_PreviousItemButton, 0, 0)
        Me.TableLayoutPanel4.Controls.Add(Me.Rec_PreviousNRItemButton, 0, 1)
        Me.TableLayoutPanel4.Controls.Add(Me.Rec_NextItemButton, 2, 0)
        Me.TableLayoutPanel4.Controls.Add(Me.Rec_NextNRItemButton, 2, 1)
        Me.TableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel4.Location = New System.Drawing.Point(3, 46)
        Me.TableLayoutPanel4.Name = "TableLayoutPanel4"
        Me.TableLayoutPanel4.RowCount = 2
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel4.Size = New System.Drawing.Size(430, 243)
        Me.TableLayoutPanel4.TabIndex = 15
        '
        'Rec_PreviousItemButton
        '
        Me.Rec_PreviousItemButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Rec_PreviousItemButton.Location = New System.Drawing.Point(3, 3)
        Me.Rec_PreviousItemButton.Name = "Rec_PreviousItemButton"
        Me.Rec_PreviousItemButton.Size = New System.Drawing.Size(84, 115)
        Me.Rec_PreviousItemButton.TabIndex = 1
        Me.Rec_PreviousItemButton.Text = "Previous item"
        Me.Rec_PreviousItemButton.UseVisualStyleBackColor = True
        '
        'Rec_PreviousNRItemButton
        '
        Me.Rec_PreviousNRItemButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Rec_PreviousNRItemButton.Location = New System.Drawing.Point(3, 124)
        Me.Rec_PreviousNRItemButton.Name = "Rec_PreviousNRItemButton"
        Me.Rec_PreviousNRItemButton.Size = New System.Drawing.Size(84, 116)
        Me.Rec_PreviousNRItemButton.TabIndex = 2
        Me.Rec_PreviousNRItemButton.Text = "Previous non-recorded item"
        Me.Rec_PreviousNRItemButton.UseVisualStyleBackColor = True
        '
        'Rec_NextItemButton
        '
        Me.Rec_NextItemButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Rec_NextItemButton.Location = New System.Drawing.Point(343, 3)
        Me.Rec_NextItemButton.Name = "Rec_NextItemButton"
        Me.Rec_NextItemButton.Size = New System.Drawing.Size(84, 115)
        Me.Rec_NextItemButton.TabIndex = 3
        Me.Rec_NextItemButton.Text = "Next item"
        Me.Rec_NextItemButton.UseVisualStyleBackColor = True
        '
        'Rec_NextNRItemButton
        '
        Me.Rec_NextNRItemButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Rec_NextNRItemButton.Location = New System.Drawing.Point(343, 124)
        Me.Rec_NextNRItemButton.Name = "Rec_NextNRItemButton"
        Me.Rec_NextNRItemButton.Size = New System.Drawing.Size(84, 116)
        Me.Rec_NextNRItemButton.TabIndex = 4
        Me.Rec_NextNRItemButton.Text = "Next non-recorded item"
        Me.Rec_NextNRItemButton.UseVisualStyleBackColor = True
        '
        'StartRecordingButton
        '
        Me.StartRecordingButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.StartRecordingButton.Location = New System.Drawing.Point(3, 305)
        Me.StartRecordingButton.Name = "StartRecordingButton"
        Me.StartRecordingButton.Size = New System.Drawing.Size(142, 34)
        Me.StartRecordingButton.TabIndex = 0
        Me.StartRecordingButton.Text = "Start recording"
        Me.StartRecordingButton.UseVisualStyleBackColor = True
        '
        'ListenButton
        '
        Me.ListenButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListenButton.Location = New System.Drawing.Point(300, 305)
        Me.ListenButton.Name = "ListenButton"
        Me.ListenButton.Size = New System.Drawing.Size(143, 34)
        Me.ListenButton.TabIndex = 1
        Me.ListenButton.Text = "Play"
        Me.ListenButton.UseVisualStyleBackColor = True
        '
        'StopRecordingButton
        '
        Me.StopRecordingButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.StopRecordingButton.Location = New System.Drawing.Point(151, 305)
        Me.StopRecordingButton.Name = "StopRecordingButton"
        Me.StopRecordingButton.Size = New System.Drawing.Size(143, 34)
        Me.StopRecordingButton.TabIndex = 3
        Me.StopRecordingButton.Text = "Stop recording"
        Me.StopRecordingButton.UseVisualStyleBackColor = True
        '
        'SegmentationTab
        '
        Me.SegmentationTab.Controls.Add(Me.SegmentationPanel)
        Me.SegmentationTab.Location = New System.Drawing.Point(4, 22)
        Me.SegmentationTab.Name = "SegmentationTab"
        Me.SegmentationTab.Padding = New System.Windows.Forms.Padding(3)
        Me.SegmentationTab.Size = New System.Drawing.Size(1004, 348)
        Me.SegmentationTab.TabIndex = 1
        Me.SegmentationTab.Text = "Segmentation"
        Me.SegmentationTab.UseVisualStyleBackColor = True
        '
        'SegmentationPanel
        '
        Me.SegmentationPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SegmentationPanel.Location = New System.Drawing.Point(3, 3)
        Me.SegmentationPanel.Name = "SegmentationPanel"
        Me.SegmentationPanel.Size = New System.Drawing.Size(998, 342)
        Me.SegmentationPanel.TabIndex = 0
        '
        'ItemProgressBar
        '
        Me.ItemProgressBar.Location = New System.Drawing.Point(467, 12)
        Me.ItemProgressBar.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.ItemProgressBar.Name = "ItemProgressBar"
        Me.ItemProgressBar.Size = New System.Drawing.Size(100, 17)
        Me.ItemProgressBar.TabIndex = 2
        '
        'ItemProgressLabel
        '
        Me.ItemProgressLabel.AutoSize = True
        Me.ItemProgressLabel.Location = New System.Drawing.Point(573, 14)
        Me.ItemProgressLabel.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.ItemProgressLabel.Name = "ItemProgressLabel"
        Me.ItemProgressLabel.Size = New System.Drawing.Size(59, 13)
        Me.ItemProgressLabel.TabIndex = 3
        Me.ItemProgressLabel.Text = "Item X of Y"
        Me.ItemProgressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ItemComboBox
        '
        Me.ItemComboBox.FormattingEnabled = True
        Me.ItemComboBox.Location = New System.Drawing.Point(638, 12)
        Me.ItemComboBox.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.ItemComboBox.Name = "ItemComboBox"
        Me.ItemComboBox.Size = New System.Drawing.Size(121, 21)
        Me.ItemComboBox.TabIndex = 4
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.BackColor = System.Drawing.Color.Transparent
        Me.FlowLayoutPanel1.Controls.Add(Me.Label1)
        Me.FlowLayoutPanel1.Controls.Add(Me.Top_Id_Label)
        Me.FlowLayoutPanel1.Controls.Add(Me.Label3)
        Me.FlowLayoutPanel1.Controls.Add(Me.Label4)
        Me.FlowLayoutPanel1.Controls.Add(Me.Top_Spelling_Label)
        Me.FlowLayoutPanel1.Controls.Add(Me.Label5)
        Me.FlowLayoutPanel1.Controls.Add(Me.Label6)
        Me.FlowLayoutPanel1.Controls.Add(Me.Top_Transcription_Label)
        Me.FlowLayoutPanel1.Controls.Add(Me.Label2)
        Me.FlowLayoutPanel1.Controls.Add(Me.ItemProgressBar)
        Me.FlowLayoutPanel1.Controls.Add(Me.ItemProgressLabel)
        Me.FlowLayoutPanel1.Controls.Add(Me.ItemComboBox)
        Me.FlowLayoutPanel1.Controls.Add(Me.Top_PreviousItemButton)
        Me.FlowLayoutPanel1.Controls.Add(Me.Top_NextItemButton)
        Me.FlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(3, 16)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Padding = New System.Windows.Forms.Padding(5, 10, 5, 10)
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(1006, 58)
        Me.FlowLayoutPanel1.TabIndex = 3
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(8, 14)
        Me.Label1.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(19, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Id:"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Top_Id_Label
        '
        Me.Top_Id_Label.AutoSize = True
        Me.Top_Id_Label.Location = New System.Drawing.Point(33, 14)
        Me.Top_Id_Label.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Top_Id_Label.Name = "Top_Id_Label"
        Me.Top_Id_Label.Size = New System.Drawing.Size(51, 13)
        Me.Top_Id_Label.TabIndex = 1
        Me.Top_Id_Label.Text = "StimulisId"
        Me.Top_Id_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(90, 14)
        Me.Label3.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(9, 13)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "|"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(105, 14)
        Me.Label4.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(47, 13)
        Me.Label4.TabIndex = 3
        Me.Label4.Text = "Spelling:"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Top_Spelling_Label
        '
        Me.Top_Spelling_Label.AutoSize = True
        Me.Top_Spelling_Label.Location = New System.Drawing.Point(158, 14)
        Me.Top_Spelling_Label.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Top_Spelling_Label.Name = "Top_Spelling_Label"
        Me.Top_Spelling_Label.Size = New System.Drawing.Size(83, 13)
        Me.Top_Spelling_Label.TabIndex = 4
        Me.Top_Spelling_Label.Text = "StimulusSpelling"
        Me.Top_Spelling_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(247, 14)
        Me.Label5.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(9, 13)
        Me.Label5.TabIndex = 5
        Me.Label5.Text = "|"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(262, 14)
        Me.Label6.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(71, 13)
        Me.Label6.TabIndex = 6
        Me.Label6.Text = "Transcription:"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Top_Transcription_Label
        '
        Me.Top_Transcription_Label.AutoSize = True
        Me.Top_Transcription_Label.Location = New System.Drawing.Point(339, 14)
        Me.Top_Transcription_Label.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Top_Transcription_Label.Name = "Top_Transcription_Label"
        Me.Top_Transcription_Label.Size = New System.Drawing.Size(107, 13)
        Me.Top_Transcription_Label.TabIndex = 7
        Me.Top_Transcription_Label.Text = "StimulusTranscription"
        Me.Top_Transcription_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(452, 14)
        Me.Label2.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(9, 13)
        Me.Label2.TabIndex = 8
        Me.Label2.Text = "|"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Top_PreviousItemButton
        '
        Me.Top_PreviousItemButton.Location = New System.Drawing.Point(765, 12)
        Me.Top_PreviousItemButton.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Top_PreviousItemButton.Name = "Top_PreviousItemButton"
        Me.Top_PreviousItemButton.Size = New System.Drawing.Size(86, 23)
        Me.Top_PreviousItemButton.TabIndex = 9
        Me.Top_PreviousItemButton.Text = "Previous item"
        Me.Top_PreviousItemButton.UseVisualStyleBackColor = True
        '
        'Top_NextItemButton
        '
        Me.Top_NextItemButton.Location = New System.Drawing.Point(857, 12)
        Me.Top_NextItemButton.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Top_NextItemButton.Name = "Top_NextItemButton"
        Me.Top_NextItemButton.Size = New System.Drawing.Size(86, 23)
        Me.Top_NextItemButton.TabIndex = 10
        Me.Top_NextItemButton.Text = "Next item"
        Me.Top_NextItemButton.UseVisualStyleBackColor = True
        '
        'MainSplitContainer
        '
        Me.MainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MainSplitContainer.Location = New System.Drawing.Point(0, 24)
        Me.MainSplitContainer.Name = "MainSplitContainer"
        Me.MainSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'MainSplitContainer.Panel1
        '
        Me.MainSplitContainer.Panel1.Controls.Add(Me.GroupBox1)
        '
        'MainSplitContainer.Panel2
        '
        Me.MainSplitContainer.Panel2.Controls.Add(Me.MainTabControl)
        Me.MainSplitContainer.Size = New System.Drawing.Size(1012, 455)
        Me.MainSplitContainer.SplitterDistance = 77
        Me.MainSplitContainer.TabIndex = 5
        '
        'GroupBox1
        '
        Me.GroupBox1.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.GroupBox1.Controls.Add(Me.FlowLayoutPanel1)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(1012, 77)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Current item"
        '
        'RecordingSoundLevelMeter
        '
        Me.RecordingSoundLevelMeter.Activated = False
        Me.RecordingSoundLevelMeter.BackColor = System.Drawing.Color.White
        Me.RecordingSoundLevelMeter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RecordingSoundLevelMeter.FullScaleLevel = 0!
        Me.RecordingSoundLevelMeter.Location = New System.Drawing.Point(449, 3)
        Me.RecordingSoundLevelMeter.maxLevel = 12.0!
        Me.RecordingSoundLevelMeter.minLevel = -100.0!
        Me.RecordingSoundLevelMeter.Name = "RecordingSoundLevelMeter"
        Me.RecordingSoundLevelMeter.Size = New System.Drawing.Size(196, 296)
        Me.RecordingSoundLevelMeter.TabIndex = 0
        Me.RecordingSoundLevelMeter.TabStop = False
        Me.RecordingSoundLevelMeter.WarningLevel = -4.0!
        '
        'AutoHeightTextBox1
        '
        Me.AutoHeightTextBox1.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.AutoHeightTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.AutoHeightTextBox1.Dock = System.Windows.Forms.DockStyle.Top
        Me.AutoHeightTextBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 16.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AutoHeightTextBox1.Location = New System.Drawing.Point(93, 131)
        Me.AutoHeightTextBox1.Margin = New System.Windows.Forms.Padding(3, 10, 3, 3)
        Me.AutoHeightTextBox1.Multiline = True
        Me.AutoHeightTextBox1.Name = "AutoHeightTextBox1"
        Me.AutoHeightTextBox1.ReadOnly = True
        Me.AutoHeightTextBox1.Size = New System.Drawing.Size(244, 26)
        Me.AutoHeightTextBox1.TabIndex = 5
        Me.AutoHeightTextBox1.Text = "Transcription"
        Me.AutoHeightTextBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Spelling_AutoHeightTextBox
        '
        Me.Spelling_AutoHeightTextBox.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.Spelling_AutoHeightTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.Spelling_AutoHeightTextBox.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Spelling_AutoHeightTextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 16.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Spelling_AutoHeightTextBox.Location = New System.Drawing.Point(93, 85)
        Me.Spelling_AutoHeightTextBox.Margin = New System.Windows.Forms.Padding(3, 3, 3, 10)
        Me.Spelling_AutoHeightTextBox.Multiline = True
        Me.Spelling_AutoHeightTextBox.Name = "Spelling_AutoHeightTextBox"
        Me.Spelling_AutoHeightTextBox.ReadOnly = True
        Me.Spelling_AutoHeightTextBox.Size = New System.Drawing.Size(244, 26)
        Me.Spelling_AutoHeightTextBox.TabIndex = 0
        Me.Spelling_AutoHeightTextBox.Text = "Spelling"
        Me.Spelling_AutoHeightTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'LoadWaveFileSMAIXMLChunkRequiredToolStripMenuItem
        '
        Me.LoadWaveFileSMAIXMLChunkRequiredToolStripMenuItem.Name = "LoadWaveFileSMAIXMLChunkRequiredToolStripMenuItem"
        Me.LoadWaveFileSMAIXMLChunkRequiredToolStripMenuItem.Size = New System.Drawing.Size(298, 22)
        Me.LoadWaveFileSMAIXMLChunkRequiredToolStripMenuItem.Text = "Load wave file (SMA iXML chunk required)"
        '
        'SaveWaveFileToolStripMenuItem
        '
        Me.SaveWaveFileToolStripMenuItem.Name = "SaveWaveFileToolStripMenuItem"
        Me.SaveWaveFileToolStripMenuItem.Size = New System.Drawing.Size(298, 22)
        Me.SaveWaveFileToolStripMenuItem.Text = "Save wave file"
        '
        'CloseToolStripMenuItem
        '
        Me.CloseToolStripMenuItem.Name = "CloseToolStripMenuItem"
        Me.CloseToolStripMenuItem.Size = New System.Drawing.Size(298, 22)
        Me.CloseToolStripMenuItem.Text = "Exit"
        '
        'SaveWaveFileAsToolStripMenuItem
        '
        Me.SaveWaveFileAsToolStripMenuItem.Name = "SaveWaveFileAsToolStripMenuItem"
        Me.SaveWaveFileAsToolStripMenuItem.Size = New System.Drawing.Size(298, 22)
        Me.SaveWaveFileAsToolStripMenuItem.Text = "Save wave file as"
        '
        'SpeechMaterialRecorder
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1012, 501)
        Me.Controls.Add(Me.MainSplitContainer)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "SpeechMaterialRecorder"
        Me.Text = "Speech Material Recorder"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.MainTabControl.ResumeLayout(False)
        Me.RecordingTab.ResumeLayout(False)
        Me.RecordingTabMainSplitContainer.Panel1.ResumeLayout(False)
        CType(Me.RecordingTabMainSplitContainer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.RecordingTabMainSplitContainer.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TopRecordingControlPanel.ResumeLayout(False)
        Me.TableLayoutPanel3.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.TableLayoutPanel4.ResumeLayout(False)
        Me.TableLayoutPanel4.PerformLayout()
        Me.SegmentationTab.ResumeLayout(False)
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.FlowLayoutPanel1.PerformLayout()
        Me.MainSplitContainer.Panel1.ResumeLayout(False)
        Me.MainSplitContainer.Panel2.ResumeLayout(False)
        CType(Me.MainSplitContainer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MainSplitContainer.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        CType(Me.RecordingSoundLevelMeter, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents MenuStrip1 As Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents StatusStrip1 As Windows.Forms.StatusStrip
    Friend WithEvents preQueLabel As Windows.Forms.ToolStripStatusLabel
    Friend WithEvents SoundFilePathStatusLabel As Windows.Forms.ToolStripStatusLabel
    Friend WithEvents MainTabControl As Windows.Forms.TabControl
    Friend WithEvents RecordingTab As Windows.Forms.TabPage
    Friend WithEvents SegmentationTab As Windows.Forms.TabPage
    Friend WithEvents RecordingSettingsMenu As Windows.Forms.ToolStripMenuItem
    Friend WithEvents FontSizeToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents IncreaseFontSizeToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents DecreaseFontSizeToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents SetFontsToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents SetFontOfSpellingsToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents SetFontOfPhoneticTranscriptionsToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents AuditoryPrequeingToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents StartRecordingAutomaticallyOnNextpreviousToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents SegmentationSettingsMenu As Windows.Forms.ToolStripMenuItem
    Friend WithEvents AudioSettingsToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents SegmentationToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripStatusLabel2 As Windows.Forms.ToolStripStatusLabel
    Friend WithEvents AutoRecordingStatusLabel As Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusLabel3 As Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToggleBackgroundSoundWhileRecordingonoffToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents BackgroundSoundStatusLabel As Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusLabel1 As Windows.Forms.ToolStripStatusLabel
    Friend WithEvents SpectrogramSettingsToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents IOSettingsToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents CalibrateOutputLevelToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents SelectTransducerTypeToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents WordBoundaryDetectionToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents AutoDetectBoundariesToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents BoundaryDetectionSettingsToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents UpdateAllSegmentationsToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ValidateAllSegmentationsToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents FadeAllPaddingSectionsToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents MoveSegmentationsToZeroCrossingsToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ResetAllSegmentationDataToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents RecordingTabMainSplitContainer As Windows.Forms.SplitContainer
    Friend WithEvents TopRecordingControlPanel As Windows.Forms.Panel
    Friend WithEvents ListenButton As Windows.Forms.Button
    Friend WithEvents StartRecordingButton As Windows.Forms.Button
    Friend WithEvents ItemProgressBar As Windows.Forms.ProgressBar
    Friend WithEvents ItemComboBox As Windows.Forms.ComboBox
    Friend WithEvents ItemProgressLabel As Windows.Forms.Label
    Friend WithEvents RecordingSoundLevelMeter As Audio.Graphics.SoundLevelMeter
    Friend WithEvents ToggleSoundLevelMeteronoffToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents FlowLayoutPanel1 As Windows.Forms.FlowLayoutPanel
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents Top_Id_Label As Windows.Forms.Label
    Friend WithEvents Label3 As Windows.Forms.Label
    Friend WithEvents Label4 As Windows.Forms.Label
    Friend WithEvents Top_Spelling_Label As Windows.Forms.Label
    Friend WithEvents Label5 As Windows.Forms.Label
    Friend WithEvents Label6 As Windows.Forms.Label
    Friend WithEvents Top_Transcription_Label As Windows.Forms.Label
    Friend WithEvents Label2 As Windows.Forms.Label
    Friend WithEvents MainSplitContainer As Windows.Forms.SplitContainer
    Friend WithEvents Top_PreviousItemButton As Windows.Forms.Button
    Friend WithEvents Top_NextItemButton As Windows.Forms.Button
    Friend WithEvents GroupBox1 As Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel1 As Windows.Forms.TableLayoutPanel
    Friend WithEvents Rec_NextNRItemButton As Windows.Forms.Button
    Friend WithEvents Rec_NextItemButton As Windows.Forms.Button
    Friend WithEvents Rec_PreviousNRItemButton As Windows.Forms.Button
    Friend WithEvents Rec_PreviousItemButton As Windows.Forms.Button
    Friend WithEvents RecordingLabel As Windows.Forms.Label
    Friend WithEvents Spelling_AutoHeightTextBox As AutoHeightTextBox
    Friend WithEvents TableLayoutPanel2 As Windows.Forms.TableLayoutPanel
    Friend WithEvents TableLayoutPanel3 As Windows.Forms.TableLayoutPanel
    Friend WithEvents StopRecordingButton As Windows.Forms.Button
    Friend WithEvents TableLayoutPanel4 As Windows.Forms.TableLayoutPanel
    Friend WithEvents AutoHeightTextBox1 As AutoHeightTextBox
    Friend WithEvents SegmentationPanel As Windows.Forms.Panel
    Friend WithEvents LoadWaveFileSMAIXMLChunkRequiredToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents SaveWaveFileToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents CloseToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents SaveWaveFileAsToolStripMenuItem As Windows.Forms.ToolStripMenuItem
End Class
