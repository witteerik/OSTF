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
        Me.LoadWaveFileSMAIXMLChunkRequiredToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveWaveFileAsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveWaveFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CloseToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RecordingSettingsMenu = New System.Windows.Forms.ToolStripMenuItem()
        Me.FontSizeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.IncreaseFontSizeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DecreaseFontSizeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SetFontsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SetFontOfSpellingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SetFontOfPhoneticTranscriptionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowSpellingToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowTranscriptionToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.UsePrototypeRecordingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StartRecordingAutomaticallyOnNextpreviousToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToggleBackgroundSoundWhileRecordingonoffToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToggleSoundLevelMeteronoffToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RecordingStopDelayToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RecordingStopDelay_ToolStripComboBox = New System.Windows.Forms.ToolStripComboBox()
        Me.NextpreviousNonrecordedOnPageUpdownToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AudioSettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PresentationLevelToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PresentationLevel_ToolStripComboBox = New System.Windows.Forms.ToolStripComboBox()
        Me.PresentationSoundLevelTypeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BackgroundSoundLevelToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BackgroundSoundLevel_ToolStripComboBox = New System.Windows.Forms.ToolStripComboBox()
        Me.BackgroundSoundLevelTypeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SegmentationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MoveSegmentationsToZeroCrossingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DrawNormalizedWaveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowSpectrogramToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SpectrogramSettingsToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.SetPaddingTimesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PaddingTimeComboBox = New System.Windows.Forms.ToolStripComboBox()
        Me.IntersentenceTimesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.InterSentenceTimeComboBox = New System.Windows.Forms.ToolStripComboBox()
        Me.ExportToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AllSoundsFiles_ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AllSoundsFiles_AllSegmentations_ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AllSoundsFiles_SentenceTimes_ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CurrentFile_ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CurrentFile_AllSegmentations_ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CurrentFile_SentenceTimes_ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ValidationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EnforceValidationInLoadedSoundToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ValidateAllLinguisticLevels_ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ValidateSentenceLevel_ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ValidateWordLevel_ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ValidatePhonemeLevel_ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.InvalidateEverythingInLoadedSoundToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.InvalidateAllLinguisticLevels_ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.InvalidateSentenceLevel_ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.InvalidateWordLevel_ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.InvalidatePhonemeLevel_ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TelepromterBTToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ConnectToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DisconnectToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStrip_BT_StatusLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel6 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.PrototypeRecordingLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel2 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.AutoRecordingStatusLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.BackgroundSoundStatusLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel5 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.PresentationLevelToolStripStatusLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel4 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.BackgroundLevel_ToolStripStatusLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel3 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.SoundFilePathStatusLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.MainTabControl = New System.Windows.Forms.TabControl()
        Me.RecordingTab = New System.Windows.Forms.TabPage()
        Me.RecordingTabMainSplitContainer = New System.Windows.Forms.SplitContainer()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.RecordingSoundLevelMeter = New SpeechTestFramework.Audio.Graphics.SoundLevelMeter()
        Me.TopRecordingControlPanel = New System.Windows.Forms.Panel()
        Me.TableLayoutPanel3 = New System.Windows.Forms.TableLayoutPanel()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.RecordingLabel = New System.Windows.Forms.Label()
        Me.TableLayoutPanel4 = New System.Windows.Forms.TableLayoutPanel()
        Me.Transcription_AutoHeightTextBox = New SpeechTestFramework.AutoHeightTextBox()
        Me.Spelling_AutoHeightTextBox = New SpeechTestFramework.AutoHeightTextBox()
        Me.Rec_PreviousItemButton = New System.Windows.Forms.Button()
        Me.Rec_PreviousNRItemButton = New System.Windows.Forms.Button()
        Me.Rec_NextItemButton = New System.Windows.Forms.Button()
        Me.Rec_NextNRItemButton = New System.Windows.Forms.Button()
        Me.StartRecordingButton = New System.Windows.Forms.Button()
        Me.ListenButton = New System.Windows.Forms.Button()
        Me.StopRecordingButton = New System.Windows.Forms.Button()
        Me.StopPlayback_Button = New System.Windows.Forms.Button()
        Me.SegmentationTab = New System.Windows.Forms.TabPage()
        Me.SegmentationPanel = New System.Windows.Forms.Panel()
        Me.ItemProgressBar = New System.Windows.Forms.ProgressBar()
        Me.ItemProgressLabel = New System.Windows.Forms.Label()
        Me.FileComboBox = New System.Windows.Forms.ComboBox()
        Me.SoundFileSelection_FlowLayoutPanel = New System.Windows.Forms.FlowLayoutPanel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.FileName_Label = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Top_PreviousFileButton = New System.Windows.Forms.Button()
        Me.Top_NextFileButton = New System.Windows.Forms.Button()
        Me.MainSplitContainer = New System.Windows.Forms.SplitContainer()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel5 = New System.Windows.Forms.TableLayoutPanel()
        Me.SoundSettings_TableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.SelectTransducer_Label = New System.Windows.Forms.Label()
        Me.Transducer_ComboBox = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.InputChannel_ComboBox = New System.Windows.Forms.ComboBox()
        Me.Unlock_Button = New System.Windows.Forms.Button()
        Me.StopRunningTimersToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuStrip1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.MainTabControl.SuspendLayout()
        Me.RecordingTab.SuspendLayout()
        CType(Me.RecordingTabMainSplitContainer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.RecordingTabMainSplitContainer.Panel1.SuspendLayout()
        Me.RecordingTabMainSplitContainer.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        CType(Me.RecordingSoundLevelMeter, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TopRecordingControlPanel.SuspendLayout()
        Me.TableLayoutPanel3.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.TableLayoutPanel4.SuspendLayout()
        Me.SegmentationTab.SuspendLayout()
        Me.SoundFileSelection_FlowLayoutPanel.SuspendLayout()
        CType(Me.MainSplitContainer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MainSplitContainer.Panel1.SuspendLayout()
        Me.MainSplitContainer.Panel2.SuspendLayout()
        Me.MainSplitContainer.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.TableLayoutPanel5.SuspendLayout()
        Me.SoundSettings_TableLayoutPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.RecordingSettingsMenu, Me.AudioSettingsToolStripMenuItem, Me.SegmentationToolStripMenuItem, Me.TelepromterBTToolStripMenuItem, Me.StopRunningTimersToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        Me.MenuStrip1.Size = New System.Drawing.Size(1107, 24)
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
        'LoadWaveFileSMAIXMLChunkRequiredToolStripMenuItem
        '
        Me.LoadWaveFileSMAIXMLChunkRequiredToolStripMenuItem.Name = "LoadWaveFileSMAIXMLChunkRequiredToolStripMenuItem"
        Me.LoadWaveFileSMAIXMLChunkRequiredToolStripMenuItem.Size = New System.Drawing.Size(298, 22)
        Me.LoadWaveFileSMAIXMLChunkRequiredToolStripMenuItem.Text = "Load wave file (SMA iXML chunk required)"
        Me.LoadWaveFileSMAIXMLChunkRequiredToolStripMenuItem.Visible = False
        '
        'SaveWaveFileAsToolStripMenuItem
        '
        Me.SaveWaveFileAsToolStripMenuItem.Name = "SaveWaveFileAsToolStripMenuItem"
        Me.SaveWaveFileAsToolStripMenuItem.Size = New System.Drawing.Size(298, 22)
        Me.SaveWaveFileAsToolStripMenuItem.Text = "Save wave file as"
        '
        'SaveWaveFileToolStripMenuItem
        '
        Me.SaveWaveFileToolStripMenuItem.Name = "SaveWaveFileToolStripMenuItem"
        Me.SaveWaveFileToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.S), System.Windows.Forms.Keys)
        Me.SaveWaveFileToolStripMenuItem.Size = New System.Drawing.Size(298, 22)
        Me.SaveWaveFileToolStripMenuItem.Text = "Save wave file"
        '
        'CloseToolStripMenuItem
        '
        Me.CloseToolStripMenuItem.Name = "CloseToolStripMenuItem"
        Me.CloseToolStripMenuItem.Size = New System.Drawing.Size(298, 22)
        Me.CloseToolStripMenuItem.Text = "Exit"
        '
        'RecordingSettingsMenu
        '
        Me.RecordingSettingsMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FontSizeToolStripMenuItem, Me.SetFontsToolStripMenuItem, Me.ShowSpellingToolStripMenuItem, Me.ShowTranscriptionToolStripMenuItem, Me.UsePrototypeRecordingsToolStripMenuItem, Me.StartRecordingAutomaticallyOnNextpreviousToolStripMenuItem, Me.ToggleBackgroundSoundWhileRecordingonoffToolStripMenuItem, Me.ToggleSoundLevelMeteronoffToolStripMenuItem, Me.RecordingStopDelayToolStripMenuItem, Me.NextpreviousNonrecordedOnPageUpdownToolStripMenuItem})
        Me.RecordingSettingsMenu.Name = "RecordingSettingsMenu"
        Me.RecordingSettingsMenu.Size = New System.Drawing.Size(61, 20)
        Me.RecordingSettingsMenu.Text = "Settings"
        '
        'FontSizeToolStripMenuItem
        '
        Me.FontSizeToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.IncreaseFontSizeToolStripMenuItem, Me.DecreaseFontSizeToolStripMenuItem})
        Me.FontSizeToolStripMenuItem.Name = "FontSizeToolStripMenuItem"
        Me.FontSizeToolStripMenuItem.Size = New System.Drawing.Size(340, 22)
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
        Me.SetFontsToolStripMenuItem.Size = New System.Drawing.Size(340, 22)
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
        'ShowSpellingToolStripMenuItem
        '
        Me.ShowSpellingToolStripMenuItem.Name = "ShowSpellingToolStripMenuItem"
        Me.ShowSpellingToolStripMenuItem.Size = New System.Drawing.Size(340, 22)
        Me.ShowSpellingToolStripMenuItem.Text = "Show spelling"
        '
        'ShowTranscriptionToolStripMenuItem
        '
        Me.ShowTranscriptionToolStripMenuItem.Name = "ShowTranscriptionToolStripMenuItem"
        Me.ShowTranscriptionToolStripMenuItem.Size = New System.Drawing.Size(340, 22)
        Me.ShowTranscriptionToolStripMenuItem.Text = "Show transcription"
        '
        'UsePrototypeRecordingsToolStripMenuItem
        '
        Me.UsePrototypeRecordingsToolStripMenuItem.Name = "UsePrototypeRecordingsToolStripMenuItem"
        Me.UsePrototypeRecordingsToolStripMenuItem.Size = New System.Drawing.Size(340, 22)
        Me.UsePrototypeRecordingsToolStripMenuItem.Text = "Use prototype recordings"
        '
        'StartRecordingAutomaticallyOnNextpreviousToolStripMenuItem
        '
        Me.StartRecordingAutomaticallyOnNextpreviousToolStripMenuItem.Name = "StartRecordingAutomaticallyOnNextpreviousToolStripMenuItem"
        Me.StartRecordingAutomaticallyOnNextpreviousToolStripMenuItem.Size = New System.Drawing.Size(340, 22)
        Me.StartRecordingAutomaticallyOnNextpreviousToolStripMenuItem.Text = "Use auto-recording on next/previous"
        '
        'ToggleBackgroundSoundWhileRecordingonoffToolStripMenuItem
        '
        Me.ToggleBackgroundSoundWhileRecordingonoffToolStripMenuItem.Name = "ToggleBackgroundSoundWhileRecordingonoffToolStripMenuItem"
        Me.ToggleBackgroundSoundWhileRecordingonoffToolStripMenuItem.Size = New System.Drawing.Size(340, 22)
        Me.ToggleBackgroundSoundWhileRecordingonoffToolStripMenuItem.Text = "Use background (Lombard) sound while recording"
        '
        'ToggleSoundLevelMeteronoffToolStripMenuItem
        '
        Me.ToggleSoundLevelMeteronoffToolStripMenuItem.Name = "ToggleSoundLevelMeteronoffToolStripMenuItem"
        Me.ToggleSoundLevelMeteronoffToolStripMenuItem.Size = New System.Drawing.Size(340, 22)
        Me.ToggleSoundLevelMeteronoffToolStripMenuItem.Text = "Show sound level meter"
        '
        'RecordingStopDelayToolStripMenuItem
        '
        Me.RecordingStopDelayToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.RecordingStopDelay_ToolStripComboBox})
        Me.RecordingStopDelayToolStripMenuItem.Name = "RecordingStopDelayToolStripMenuItem"
        Me.RecordingStopDelayToolStripMenuItem.Size = New System.Drawing.Size(340, 22)
        Me.RecordingStopDelayToolStripMenuItem.Text = "Recording stop delay"
        '
        'RecordingStopDelay_ToolStripComboBox
        '
        Me.RecordingStopDelay_ToolStripComboBox.Name = "RecordingStopDelay_ToolStripComboBox"
        Me.RecordingStopDelay_ToolStripComboBox.Size = New System.Drawing.Size(121, 23)
        '
        'NextpreviousNonrecordedOnPageUpdownToolStripMenuItem
        '
        Me.NextpreviousNonrecordedOnPageUpdownToolStripMenuItem.CheckOnClick = True
        Me.NextpreviousNonrecordedOnPageUpdownToolStripMenuItem.Name = "NextpreviousNonrecordedOnPageUpdownToolStripMenuItem"
        Me.NextpreviousNonrecordedOnPageUpdownToolStripMenuItem.Size = New System.Drawing.Size(340, 22)
        Me.NextpreviousNonrecordedOnPageUpdownToolStripMenuItem.Text = "Next/previous non-recorded on page up/down"
        '
        'AudioSettingsToolStripMenuItem
        '
        Me.AudioSettingsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.PresentationLevelToolStripMenuItem, Me.PresentationSoundLevelTypeToolStripMenuItem, Me.BackgroundSoundLevelToolStripMenuItem, Me.BackgroundSoundLevelTypeToolStripMenuItem})
        Me.AudioSettingsToolStripMenuItem.Name = "AudioSettingsToolStripMenuItem"
        Me.AudioSettingsToolStripMenuItem.Size = New System.Drawing.Size(95, 20)
        Me.AudioSettingsToolStripMenuItem.Text = "Audio settings"
        '
        'PresentationLevelToolStripMenuItem
        '
        Me.PresentationLevelToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.PresentationLevel_ToolStripComboBox})
        Me.PresentationLevelToolStripMenuItem.Name = "PresentationLevelToolStripMenuItem"
        Me.PresentationLevelToolStripMenuItem.Size = New System.Drawing.Size(248, 22)
        Me.PresentationLevelToolStripMenuItem.Text = "Presentation level (dB SPL)"
        '
        'PresentationLevel_ToolStripComboBox
        '
        Me.PresentationLevel_ToolStripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.PresentationLevel_ToolStripComboBox.Name = "PresentationLevel_ToolStripComboBox"
        Me.PresentationLevel_ToolStripComboBox.Size = New System.Drawing.Size(121, 23)
        '
        'PresentationSoundLevelTypeToolStripMenuItem
        '
        Me.PresentationSoundLevelTypeToolStripMenuItem.Name = "PresentationSoundLevelTypeToolStripMenuItem"
        Me.PresentationSoundLevelTypeToolStripMenuItem.Size = New System.Drawing.Size(248, 22)
        Me.PresentationSoundLevelTypeToolStripMenuItem.Text = "Presentation sound level type"
        '
        'BackgroundSoundLevelToolStripMenuItem
        '
        Me.BackgroundSoundLevelToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BackgroundSoundLevel_ToolStripComboBox})
        Me.BackgroundSoundLevelToolStripMenuItem.Name = "BackgroundSoundLevelToolStripMenuItem"
        Me.BackgroundSoundLevelToolStripMenuItem.Size = New System.Drawing.Size(248, 22)
        Me.BackgroundSoundLevelToolStripMenuItem.Text = "Background sound level (dB SPL)"
        '
        'BackgroundSoundLevel_ToolStripComboBox
        '
        Me.BackgroundSoundLevel_ToolStripComboBox.Name = "BackgroundSoundLevel_ToolStripComboBox"
        Me.BackgroundSoundLevel_ToolStripComboBox.Size = New System.Drawing.Size(121, 23)
        '
        'BackgroundSoundLevelTypeToolStripMenuItem
        '
        Me.BackgroundSoundLevelTypeToolStripMenuItem.Name = "BackgroundSoundLevelTypeToolStripMenuItem"
        Me.BackgroundSoundLevelTypeToolStripMenuItem.Size = New System.Drawing.Size(248, 22)
        Me.BackgroundSoundLevelTypeToolStripMenuItem.Text = "Background sound level type"
        '
        'SegmentationToolStripMenuItem
        '
        Me.SegmentationToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MoveSegmentationsToZeroCrossingsToolStripMenuItem, Me.DrawNormalizedWaveToolStripMenuItem, Me.ShowSpectrogramToolStripMenuItem, Me.SpectrogramSettingsToolStripMenuItem1, Me.SetPaddingTimesToolStripMenuItem, Me.IntersentenceTimesToolStripMenuItem, Me.ExportToolStripMenuItem, Me.ValidationToolStripMenuItem})
        Me.SegmentationToolStripMenuItem.Name = "SegmentationToolStripMenuItem"
        Me.SegmentationToolStripMenuItem.Size = New System.Drawing.Size(93, 20)
        Me.SegmentationToolStripMenuItem.Text = "Segmentation"
        Me.SegmentationToolStripMenuItem.Visible = False
        '
        'MoveSegmentationsToZeroCrossingsToolStripMenuItem
        '
        Me.MoveSegmentationsToZeroCrossingsToolStripMenuItem.Checked = True
        Me.MoveSegmentationsToZeroCrossingsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.MoveSegmentationsToZeroCrossingsToolStripMenuItem.Name = "MoveSegmentationsToZeroCrossingsToolStripMenuItem"
        Me.MoveSegmentationsToZeroCrossingsToolStripMenuItem.Size = New System.Drawing.Size(248, 22)
        Me.MoveSegmentationsToZeroCrossingsToolStripMenuItem.Text = "Place segments at zero-crossings"
        '
        'DrawNormalizedWaveToolStripMenuItem
        '
        Me.DrawNormalizedWaveToolStripMenuItem.Checked = True
        Me.DrawNormalizedWaveToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.DrawNormalizedWaveToolStripMenuItem.Name = "DrawNormalizedWaveToolStripMenuItem"
        Me.DrawNormalizedWaveToolStripMenuItem.Size = New System.Drawing.Size(248, 22)
        Me.DrawNormalizedWaveToolStripMenuItem.Text = "Draw normalized wave"
        '
        'ShowSpectrogramToolStripMenuItem
        '
        Me.ShowSpectrogramToolStripMenuItem.Checked = True
        Me.ShowSpectrogramToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ShowSpectrogramToolStripMenuItem.Name = "ShowSpectrogramToolStripMenuItem"
        Me.ShowSpectrogramToolStripMenuItem.Size = New System.Drawing.Size(248, 22)
        Me.ShowSpectrogramToolStripMenuItem.Text = "Show spectrogram"
        '
        'SpectrogramSettingsToolStripMenuItem1
        '
        Me.SpectrogramSettingsToolStripMenuItem1.Name = "SpectrogramSettingsToolStripMenuItem1"
        Me.SpectrogramSettingsToolStripMenuItem1.Size = New System.Drawing.Size(248, 22)
        Me.SpectrogramSettingsToolStripMenuItem1.Text = "Spectrogram settings"
        '
        'SetPaddingTimesToolStripMenuItem
        '
        Me.SetPaddingTimesToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.PaddingTimeComboBox})
        Me.SetPaddingTimesToolStripMenuItem.Name = "SetPaddingTimesToolStripMenuItem"
        Me.SetPaddingTimesToolStripMenuItem.Size = New System.Drawing.Size(248, 22)
        Me.SetPaddingTimesToolStripMenuItem.Text = "Padding time (s)"
        '
        'PaddingTimeComboBox
        '
        Me.PaddingTimeComboBox.Name = "PaddingTimeComboBox"
        Me.PaddingTimeComboBox.Size = New System.Drawing.Size(121, 23)
        '
        'IntersentenceTimesToolStripMenuItem
        '
        Me.IntersentenceTimesToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.InterSentenceTimeComboBox})
        Me.IntersentenceTimesToolStripMenuItem.Name = "IntersentenceTimesToolStripMenuItem"
        Me.IntersentenceTimesToolStripMenuItem.Size = New System.Drawing.Size(248, 22)
        Me.IntersentenceTimesToolStripMenuItem.Text = "Inter-sentence time (s)"
        '
        'InterSentenceTimeComboBox
        '
        Me.InterSentenceTimeComboBox.Name = "InterSentenceTimeComboBox"
        Me.InterSentenceTimeComboBox.Size = New System.Drawing.Size(121, 23)
        '
        'ExportToolStripMenuItem
        '
        Me.ExportToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AllSoundsFiles_ToolStripMenuItem, Me.CurrentFile_ToolStripMenuItem})
        Me.ExportToolStripMenuItem.Name = "ExportToolStripMenuItem"
        Me.ExportToolStripMenuItem.Size = New System.Drawing.Size(248, 22)
        Me.ExportToolStripMenuItem.Text = "Export"
        '
        'AllSoundsFiles_ToolStripMenuItem
        '
        Me.AllSoundsFiles_ToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AllSoundsFiles_AllSegmentations_ToolStripMenuItem, Me.AllSoundsFiles_SentenceTimes_ToolStripMenuItem})
        Me.AllSoundsFiles_ToolStripMenuItem.Name = "AllSoundsFiles_ToolStripMenuItem"
        Me.AllSoundsFiles_ToolStripMenuItem.Size = New System.Drawing.Size(148, 22)
        Me.AllSoundsFiles_ToolStripMenuItem.Text = "All sound files"
        '
        'AllSoundsFiles_AllSegmentations_ToolStripMenuItem
        '
        Me.AllSoundsFiles_AllSegmentations_ToolStripMenuItem.Name = "AllSoundsFiles_AllSegmentations_ToolStripMenuItem"
        Me.AllSoundsFiles_AllSegmentations_ToolStripMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.AllSoundsFiles_AllSegmentations_ToolStripMenuItem.Text = "All segmentations"
        '
        'AllSoundsFiles_SentenceTimes_ToolStripMenuItem
        '
        Me.AllSoundsFiles_SentenceTimes_ToolStripMenuItem.Name = "AllSoundsFiles_SentenceTimes_ToolStripMenuItem"
        Me.AllSoundsFiles_SentenceTimes_ToolStripMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.AllSoundsFiles_SentenceTimes_ToolStripMenuItem.Text = "Sentence times"
        '
        'CurrentFile_ToolStripMenuItem
        '
        Me.CurrentFile_ToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CurrentFile_AllSegmentations_ToolStripMenuItem, Me.CurrentFile_SentenceTimes_ToolStripMenuItem})
        Me.CurrentFile_ToolStripMenuItem.Name = "CurrentFile_ToolStripMenuItem"
        Me.CurrentFile_ToolStripMenuItem.Size = New System.Drawing.Size(148, 22)
        Me.CurrentFile_ToolStripMenuItem.Text = "Current file"
        '
        'CurrentFile_AllSegmentations_ToolStripMenuItem
        '
        Me.CurrentFile_AllSegmentations_ToolStripMenuItem.Name = "CurrentFile_AllSegmentations_ToolStripMenuItem"
        Me.CurrentFile_AllSegmentations_ToolStripMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.CurrentFile_AllSegmentations_ToolStripMenuItem.Text = "All segmentations"
        '
        'CurrentFile_SentenceTimes_ToolStripMenuItem
        '
        Me.CurrentFile_SentenceTimes_ToolStripMenuItem.Name = "CurrentFile_SentenceTimes_ToolStripMenuItem"
        Me.CurrentFile_SentenceTimes_ToolStripMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.CurrentFile_SentenceTimes_ToolStripMenuItem.Text = "Sentence times"
        '
        'ValidationToolStripMenuItem
        '
        Me.ValidationToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.EnforceValidationInLoadedSoundToolStripMenuItem, Me.InvalidateEverythingInLoadedSoundToolStripMenuItem})
        Me.ValidationToolStripMenuItem.Name = "ValidationToolStripMenuItem"
        Me.ValidationToolStripMenuItem.Size = New System.Drawing.Size(248, 22)
        Me.ValidationToolStripMenuItem.Text = "Validation"
        '
        'EnforceValidationInLoadedSoundToolStripMenuItem
        '
        Me.EnforceValidationInLoadedSoundToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ValidateAllLinguisticLevels_ToolStripMenuItem, Me.ValidateSentenceLevel_ToolStripMenuItem, Me.ValidateWordLevel_ToolStripMenuItem, Me.ValidatePhonemeLevel_ToolStripMenuItem})
        Me.EnforceValidationInLoadedSoundToolStripMenuItem.Name = "EnforceValidationInLoadedSoundToolStripMenuItem"
        Me.EnforceValidationInLoadedSoundToolStripMenuItem.Size = New System.Drawing.Size(333, 22)
        Me.EnforceValidationInLoadedSoundToolStripMenuItem.Text = "Enforce segmentation validation in loaded sound"
        '
        'ValidateAllLinguisticLevels_ToolStripMenuItem
        '
        Me.ValidateAllLinguisticLevels_ToolStripMenuItem.Name = "ValidateAllLinguisticLevels_ToolStripMenuItem"
        Me.ValidateAllLinguisticLevels_ToolStripMenuItem.Size = New System.Drawing.Size(196, 22)
        Me.ValidateAllLinguisticLevels_ToolStripMenuItem.Text = "All linguistic levels"
        '
        'ValidateSentenceLevel_ToolStripMenuItem
        '
        Me.ValidateSentenceLevel_ToolStripMenuItem.Name = "ValidateSentenceLevel_ToolStripMenuItem"
        Me.ValidateSentenceLevel_ToolStripMenuItem.Size = New System.Drawing.Size(196, 22)
        Me.ValidateSentenceLevel_ToolStripMenuItem.Text = "Down to sentence level"
        '
        'ValidateWordLevel_ToolStripMenuItem
        '
        Me.ValidateWordLevel_ToolStripMenuItem.Name = "ValidateWordLevel_ToolStripMenuItem"
        Me.ValidateWordLevel_ToolStripMenuItem.Size = New System.Drawing.Size(196, 22)
        Me.ValidateWordLevel_ToolStripMenuItem.Text = "Down to word level"
        '
        'ValidatePhonemeLevel_ToolStripMenuItem
        '
        Me.ValidatePhonemeLevel_ToolStripMenuItem.Name = "ValidatePhonemeLevel_ToolStripMenuItem"
        Me.ValidatePhonemeLevel_ToolStripMenuItem.Size = New System.Drawing.Size(196, 22)
        Me.ValidatePhonemeLevel_ToolStripMenuItem.Text = "Down to phone level"
        '
        'InvalidateEverythingInLoadedSoundToolStripMenuItem
        '
        Me.InvalidateEverythingInLoadedSoundToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.InvalidateAllLinguisticLevels_ToolStripMenuItem, Me.InvalidateSentenceLevel_ToolStripMenuItem, Me.InvalidateWordLevel_ToolStripMenuItem, Me.InvalidatePhonemeLevel_ToolStripMenuItem})
        Me.InvalidateEverythingInLoadedSoundToolStripMenuItem.Name = "InvalidateEverythingInLoadedSoundToolStripMenuItem"
        Me.InvalidateEverythingInLoadedSoundToolStripMenuItem.Size = New System.Drawing.Size(333, 22)
        Me.InvalidateEverythingInLoadedSoundToolStripMenuItem.Text = "Invalidate segmentation in the loaded sound"
        '
        'InvalidateAllLinguisticLevels_ToolStripMenuItem
        '
        Me.InvalidateAllLinguisticLevels_ToolStripMenuItem.Name = "InvalidateAllLinguisticLevels_ToolStripMenuItem"
        Me.InvalidateAllLinguisticLevels_ToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.InvalidateAllLinguisticLevels_ToolStripMenuItem.Text = "All linguistic levels"
        '
        'InvalidateSentenceLevel_ToolStripMenuItem
        '
        Me.InvalidateSentenceLevel_ToolStripMenuItem.Name = "InvalidateSentenceLevel_ToolStripMenuItem"
        Me.InvalidateSentenceLevel_ToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.InvalidateSentenceLevel_ToolStripMenuItem.Text = "Up to sentence level"
        '
        'InvalidateWordLevel_ToolStripMenuItem
        '
        Me.InvalidateWordLevel_ToolStripMenuItem.Name = "InvalidateWordLevel_ToolStripMenuItem"
        Me.InvalidateWordLevel_ToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.InvalidateWordLevel_ToolStripMenuItem.Text = "Up to word level"
        '
        'InvalidatePhonemeLevel_ToolStripMenuItem
        '
        Me.InvalidatePhonemeLevel_ToolStripMenuItem.Name = "InvalidatePhonemeLevel_ToolStripMenuItem"
        Me.InvalidatePhonemeLevel_ToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.InvalidatePhonemeLevel_ToolStripMenuItem.Text = "On phone level"
        '
        'TelepromterBTToolStripMenuItem
        '
        Me.TelepromterBTToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ConnectToolStripMenuItem, Me.DisconnectToolStripMenuItem})
        Me.TelepromterBTToolStripMenuItem.Name = "TelepromterBTToolStripMenuItem"
        Me.TelepromterBTToolStripMenuItem.Size = New System.Drawing.Size(105, 20)
        Me.TelepromterBTToolStripMenuItem.Text = "Telepromter (BT)"
        '
        'ConnectToolStripMenuItem
        '
        Me.ConnectToolStripMenuItem.Name = "ConnectToolStripMenuItem"
        Me.ConnectToolStripMenuItem.Size = New System.Drawing.Size(133, 22)
        Me.ConnectToolStripMenuItem.Text = "Connect"
        '
        'DisconnectToolStripMenuItem
        '
        Me.DisconnectToolStripMenuItem.Name = "DisconnectToolStripMenuItem"
        Me.DisconnectToolStripMenuItem.Size = New System.Drawing.Size(133, 22)
        Me.DisconnectToolStripMenuItem.Text = "Disconnect"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.BackColor = System.Drawing.SystemColors.Control
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStrip_BT_StatusLabel, Me.ToolStripStatusLabel6, Me.PrototypeRecordingLabel, Me.ToolStripStatusLabel2, Me.AutoRecordingStatusLabel, Me.ToolStripStatusLabel1, Me.BackgroundSoundStatusLabel, Me.ToolStripStatusLabel5, Me.PresentationLevelToolStripStatusLabel, Me.ToolStripStatusLabel4, Me.BackgroundLevel_ToolStripStatusLabel, Me.ToolStripStatusLabel3, Me.SoundFilePathStatusLabel})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 490)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(1107, 22)
        Me.StatusStrip1.TabIndex = 1
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStrip_BT_StatusLabel
        '
        Me.ToolStrip_BT_StatusLabel.Name = "ToolStrip_BT_StatusLabel"
        Me.ToolStrip_BT_StatusLabel.Size = New System.Drawing.Size(178, 17)
        Me.ToolStrip_BT_StatusLabel.Text = "Telepromter (BT): Not connected"
        '
        'ToolStripStatusLabel6
        '
        Me.ToolStripStatusLabel6.Name = "ToolStripStatusLabel6"
        Me.ToolStripStatusLabel6.Size = New System.Drawing.Size(10, 17)
        Me.ToolStripStatusLabel6.Text = "|"
        '
        'PrototypeRecordingLabel
        '
        Me.PrototypeRecordingLabel.Name = "PrototypeRecordingLabel"
        Me.PrototypeRecordingLabel.Size = New System.Drawing.Size(136, 17)
        Me.PrototypeRecordingLabel.Text = "Prototype recording: Off"
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
        Me.AutoRecordingStatusLabel.Size = New System.Drawing.Size(112, 17)
        Me.AutoRecordingStatusLabel.Text = "Auto-recording: Off"
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
        Me.BackgroundSoundStatusLabel.Size = New System.Drawing.Size(130, 17)
        Me.BackgroundSoundStatusLabel.Text = "Background sound: Off"
        '
        'ToolStripStatusLabel5
        '
        Me.ToolStripStatusLabel5.Name = "ToolStripStatusLabel5"
        Me.ToolStripStatusLabel5.Size = New System.Drawing.Size(10, 17)
        Me.ToolStripStatusLabel5.Text = "|"
        '
        'PresentationLevelToolStripStatusLabel
        '
        Me.PresentationLevelToolStripStatusLabel.Name = "PresentationLevelToolStripStatusLabel"
        Me.PresentationLevelToolStripStatusLabel.Size = New System.Drawing.Size(103, 17)
        Me.PresentationLevelToolStripStatusLabel.Text = "Presentation level:"
        '
        'ToolStripStatusLabel4
        '
        Me.ToolStripStatusLabel4.Name = "ToolStripStatusLabel4"
        Me.ToolStripStatusLabel4.Size = New System.Drawing.Size(10, 17)
        Me.ToolStripStatusLabel4.Text = "|"
        '
        'BackgroundLevel_ToolStripStatusLabel
        '
        Me.BackgroundLevel_ToolStripStatusLabel.Name = "BackgroundLevel_ToolStripStatusLabel"
        Me.BackgroundLevel_ToolStripStatusLabel.Size = New System.Drawing.Size(104, 17)
        Me.BackgroundLevel_ToolStripStatusLabel.Text = "Background level: "
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
        Me.MainTabControl.Enabled = False
        Me.MainTabControl.Location = New System.Drawing.Point(0, 0)
        Me.MainTabControl.Name = "MainTabControl"
        Me.MainTabControl.SelectedIndex = 0
        Me.MainTabControl.Size = New System.Drawing.Size(1107, 379)
        Me.MainTabControl.TabIndex = 16
        '
        'RecordingTab
        '
        Me.RecordingTab.Controls.Add(Me.RecordingTabMainSplitContainer)
        Me.RecordingTab.Location = New System.Drawing.Point(4, 22)
        Me.RecordingTab.Name = "RecordingTab"
        Me.RecordingTab.Padding = New System.Windows.Forms.Padding(3)
        Me.RecordingTab.Size = New System.Drawing.Size(1099, 353)
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
        Me.RecordingTabMainSplitContainer.Size = New System.Drawing.Size(1093, 347)
        Me.RecordingTabMainSplitContainer.SplitterDistance = 681
        Me.RecordingTabMainSplitContainer.TabIndex = 0
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 5
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.RecordingSoundLevelMeter, 4, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.TopRecordingControlPanel, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.StartRecordingButton, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.ListenButton, 2, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.StopRecordingButton, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.StopPlayback_Button, 3, 1)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(681, 347)
        Me.TableLayoutPanel1.TabIndex = 3
        '
        'RecordingSoundLevelMeter
        '
        Me.RecordingSoundLevelMeter.Activated = False
        Me.RecordingSoundLevelMeter.BackColor = System.Drawing.Color.White
        Me.RecordingSoundLevelMeter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RecordingSoundLevelMeter.FullScaleLevel = 0!
        Me.RecordingSoundLevelMeter.Location = New System.Drawing.Point(603, 3)
        Me.RecordingSoundLevelMeter.MaxLevel = 12.0!
        Me.RecordingSoundLevelMeter.MinLevel = -100.0!
        Me.RecordingSoundLevelMeter.Name = "RecordingSoundLevelMeter"
        Me.RecordingSoundLevelMeter.Size = New System.Drawing.Size(75, 301)
        Me.RecordingSoundLevelMeter.TabIndex = 0
        Me.RecordingSoundLevelMeter.TabStop = False
        Me.RecordingSoundLevelMeter.WarningLevel = -4.0!
        '
        'TopRecordingControlPanel
        '
        Me.TopRecordingControlPanel.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.TopRecordingControlPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TableLayoutPanel1.SetColumnSpan(Me.TopRecordingControlPanel, 4)
        Me.TopRecordingControlPanel.Controls.Add(Me.TableLayoutPanel3)
        Me.TopRecordingControlPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TopRecordingControlPanel.Location = New System.Drawing.Point(3, 3)
        Me.TopRecordingControlPanel.Name = "TopRecordingControlPanel"
        Me.TopRecordingControlPanel.Size = New System.Drawing.Size(594, 301)
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
        Me.TableLayoutPanel3.Size = New System.Drawing.Size(590, 297)
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
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(584, 37)
        Me.TableLayoutPanel2.TabIndex = 14
        '
        'RecordingLabel
        '
        Me.RecordingLabel.BackColor = System.Drawing.Color.DarkGray
        Me.RecordingLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.RecordingLabel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RecordingLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RecordingLabel.ForeColor = System.Drawing.Color.Blue
        Me.RecordingLabel.Location = New System.Drawing.Point(197, 4)
        Me.RecordingLabel.Margin = New System.Windows.Forms.Padding(3, 4, 3, 3)
        Me.RecordingLabel.Name = "RecordingLabel"
        Me.RecordingLabel.Size = New System.Drawing.Size(188, 30)
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
        Me.TableLayoutPanel4.Controls.Add(Me.Transcription_AutoHeightTextBox, 1, 1)
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
        Me.TableLayoutPanel4.Size = New System.Drawing.Size(584, 248)
        Me.TableLayoutPanel4.TabIndex = 15
        '
        'Transcription_AutoHeightTextBox
        '
        Me.Transcription_AutoHeightTextBox.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.Transcription_AutoHeightTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.Transcription_AutoHeightTextBox.Dock = System.Windows.Forms.DockStyle.Top
        Me.Transcription_AutoHeightTextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 16.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Transcription_AutoHeightTextBox.Location = New System.Drawing.Point(93, 134)
        Me.Transcription_AutoHeightTextBox.Margin = New System.Windows.Forms.Padding(3, 10, 3, 3)
        Me.Transcription_AutoHeightTextBox.Multiline = True
        Me.Transcription_AutoHeightTextBox.Name = "Transcription_AutoHeightTextBox"
        Me.Transcription_AutoHeightTextBox.ReadOnly = True
        Me.Transcription_AutoHeightTextBox.Size = New System.Drawing.Size(398, 26)
        Me.Transcription_AutoHeightTextBox.TabIndex = 5
        Me.Transcription_AutoHeightTextBox.Text = "Transcription"
        Me.Transcription_AutoHeightTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Spelling_AutoHeightTextBox
        '
        Me.Spelling_AutoHeightTextBox.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.Spelling_AutoHeightTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.Spelling_AutoHeightTextBox.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Spelling_AutoHeightTextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 16.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Spelling_AutoHeightTextBox.Location = New System.Drawing.Point(93, 88)
        Me.Spelling_AutoHeightTextBox.Margin = New System.Windows.Forms.Padding(3, 3, 3, 10)
        Me.Spelling_AutoHeightTextBox.Multiline = True
        Me.Spelling_AutoHeightTextBox.Name = "Spelling_AutoHeightTextBox"
        Me.Spelling_AutoHeightTextBox.ReadOnly = True
        Me.Spelling_AutoHeightTextBox.Size = New System.Drawing.Size(398, 26)
        Me.Spelling_AutoHeightTextBox.TabIndex = 0
        Me.Spelling_AutoHeightTextBox.Text = "Spelling"
        Me.Spelling_AutoHeightTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Rec_PreviousItemButton
        '
        Me.Rec_PreviousItemButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Rec_PreviousItemButton.Location = New System.Drawing.Point(3, 3)
        Me.Rec_PreviousItemButton.Name = "Rec_PreviousItemButton"
        Me.Rec_PreviousItemButton.Size = New System.Drawing.Size(84, 118)
        Me.Rec_PreviousItemButton.TabIndex = 9
        Me.Rec_PreviousItemButton.Text = "Previous item"
        Me.Rec_PreviousItemButton.UseVisualStyleBackColor = True
        '
        'Rec_PreviousNRItemButton
        '
        Me.Rec_PreviousNRItemButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Rec_PreviousNRItemButton.Location = New System.Drawing.Point(3, 127)
        Me.Rec_PreviousNRItemButton.Name = "Rec_PreviousNRItemButton"
        Me.Rec_PreviousNRItemButton.Size = New System.Drawing.Size(84, 118)
        Me.Rec_PreviousNRItemButton.TabIndex = 11
        Me.Rec_PreviousNRItemButton.Text = "Previous non-recorded item"
        Me.Rec_PreviousNRItemButton.UseVisualStyleBackColor = True
        '
        'Rec_NextItemButton
        '
        Me.Rec_NextItemButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Rec_NextItemButton.Location = New System.Drawing.Point(497, 3)
        Me.Rec_NextItemButton.Name = "Rec_NextItemButton"
        Me.Rec_NextItemButton.Size = New System.Drawing.Size(84, 118)
        Me.Rec_NextItemButton.TabIndex = 8
        Me.Rec_NextItemButton.Text = "Next item"
        Me.Rec_NextItemButton.UseVisualStyleBackColor = True
        '
        'Rec_NextNRItemButton
        '
        Me.Rec_NextNRItemButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Rec_NextNRItemButton.Location = New System.Drawing.Point(497, 127)
        Me.Rec_NextNRItemButton.Name = "Rec_NextNRItemButton"
        Me.Rec_NextNRItemButton.Size = New System.Drawing.Size(84, 118)
        Me.Rec_NextNRItemButton.TabIndex = 10
        Me.Rec_NextNRItemButton.Text = "Next non-recorded item"
        Me.Rec_NextNRItemButton.UseVisualStyleBackColor = True
        '
        'StartRecordingButton
        '
        Me.StartRecordingButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.StartRecordingButton.Location = New System.Drawing.Point(3, 310)
        Me.StartRecordingButton.Name = "StartRecordingButton"
        Me.StartRecordingButton.Size = New System.Drawing.Size(144, 34)
        Me.StartRecordingButton.TabIndex = 7
        Me.StartRecordingButton.Text = "Start recording"
        Me.StartRecordingButton.UseVisualStyleBackColor = True
        '
        'ListenButton
        '
        Me.ListenButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListenButton.Location = New System.Drawing.Point(303, 310)
        Me.ListenButton.Name = "ListenButton"
        Me.ListenButton.Size = New System.Drawing.Size(144, 34)
        Me.ListenButton.TabIndex = 13
        Me.ListenButton.Text = "Play"
        Me.ListenButton.UseVisualStyleBackColor = True
        '
        'StopRecordingButton
        '
        Me.StopRecordingButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.StopRecordingButton.Location = New System.Drawing.Point(153, 310)
        Me.StopRecordingButton.Name = "StopRecordingButton"
        Me.StopRecordingButton.Size = New System.Drawing.Size(144, 34)
        Me.StopRecordingButton.TabIndex = 12
        Me.StopRecordingButton.Text = "Stop recording"
        Me.StopRecordingButton.UseVisualStyleBackColor = True
        '
        'StopPlayback_Button
        '
        Me.StopPlayback_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.StopPlayback_Button.Enabled = False
        Me.StopPlayback_Button.Location = New System.Drawing.Point(453, 310)
        Me.StopPlayback_Button.Name = "StopPlayback_Button"
        Me.StopPlayback_Button.Size = New System.Drawing.Size(144, 34)
        Me.StopPlayback_Button.TabIndex = 14
        Me.StopPlayback_Button.Text = "Stop playback"
        Me.StopPlayback_Button.UseVisualStyleBackColor = True
        '
        'SegmentationTab
        '
        Me.SegmentationTab.Controls.Add(Me.SegmentationPanel)
        Me.SegmentationTab.Location = New System.Drawing.Point(4, 22)
        Me.SegmentationTab.Name = "SegmentationTab"
        Me.SegmentationTab.Padding = New System.Windows.Forms.Padding(3)
        Me.SegmentationTab.Size = New System.Drawing.Size(1099, 353)
        Me.SegmentationTab.TabIndex = 1
        Me.SegmentationTab.Text = "Segmentation"
        Me.SegmentationTab.UseVisualStyleBackColor = True
        '
        'SegmentationPanel
        '
        Me.SegmentationPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SegmentationPanel.Location = New System.Drawing.Point(3, 3)
        Me.SegmentationPanel.Name = "SegmentationPanel"
        Me.SegmentationPanel.Size = New System.Drawing.Size(1093, 347)
        Me.SegmentationPanel.TabIndex = 0
        '
        'ItemProgressBar
        '
        Me.ItemProgressBar.Location = New System.Drawing.Point(139, 7)
        Me.ItemProgressBar.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.ItemProgressBar.Name = "ItemProgressBar"
        Me.ItemProgressBar.Size = New System.Drawing.Size(100, 21)
        Me.ItemProgressBar.TabIndex = 2
        '
        'ItemProgressLabel
        '
        Me.ItemProgressLabel.AutoSize = True
        Me.ItemProgressLabel.Location = New System.Drawing.Point(245, 10)
        Me.ItemProgressLabel.Margin = New System.Windows.Forms.Padding(3, 5, 3, 5)
        Me.ItemProgressLabel.Name = "ItemProgressLabel"
        Me.ItemProgressLabel.Size = New System.Drawing.Size(59, 13)
        Me.ItemProgressLabel.TabIndex = 3
        Me.ItemProgressLabel.Text = "Item X of Y"
        Me.ItemProgressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'FileComboBox
        '
        Me.FileComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.FileComboBox.FormattingEnabled = True
        Me.FileComboBox.Location = New System.Drawing.Point(310, 7)
        Me.FileComboBox.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.FileComboBox.Name = "FileComboBox"
        Me.FileComboBox.Size = New System.Drawing.Size(121, 21)
        Me.FileComboBox.TabIndex = 1
        '
        'SoundFileSelection_FlowLayoutPanel
        '
        Me.SoundFileSelection_FlowLayoutPanel.AutoScroll = True
        Me.SoundFileSelection_FlowLayoutPanel.BackColor = System.Drawing.Color.Transparent
        Me.SoundFileSelection_FlowLayoutPanel.Controls.Add(Me.Label1)
        Me.SoundFileSelection_FlowLayoutPanel.Controls.Add(Me.FileName_Label)
        Me.SoundFileSelection_FlowLayoutPanel.Controls.Add(Me.Label2)
        Me.SoundFileSelection_FlowLayoutPanel.Controls.Add(Me.ItemProgressBar)
        Me.SoundFileSelection_FlowLayoutPanel.Controls.Add(Me.ItemProgressLabel)
        Me.SoundFileSelection_FlowLayoutPanel.Controls.Add(Me.FileComboBox)
        Me.SoundFileSelection_FlowLayoutPanel.Controls.Add(Me.Top_PreviousFileButton)
        Me.SoundFileSelection_FlowLayoutPanel.Controls.Add(Me.Top_NextFileButton)
        Me.SoundFileSelection_FlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SoundFileSelection_FlowLayoutPanel.Location = New System.Drawing.Point(3, 3)
        Me.SoundFileSelection_FlowLayoutPanel.Name = "SoundFileSelection_FlowLayoutPanel"
        Me.SoundFileSelection_FlowLayoutPanel.Padding = New System.Windows.Forms.Padding(5)
        Me.TableLayoutPanel5.SetRowSpan(Me.SoundFileSelection_FlowLayoutPanel, 2)
        Me.SoundFileSelection_FlowLayoutPanel.Size = New System.Drawing.Size(567, 58)
        Me.SoundFileSelection_FlowLayoutPanel.TabIndex = 3
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(8, 8)
        Me.Label1.Margin = New System.Windows.Forms.Padding(3)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(52, 17)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Filename:"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'FileName_Label
        '
        Me.FileName_Label.AutoSize = True
        Me.FileName_Label.Location = New System.Drawing.Point(66, 10)
        Me.FileName_Label.Margin = New System.Windows.Forms.Padding(3, 5, 3, 5)
        Me.FileName_Label.Name = "FileName_Label"
        Me.FileName_Label.Size = New System.Drawing.Size(52, 13)
        Me.FileName_Label.TabIndex = 1
        Me.FileName_Label.Text = "File name"
        Me.FileName_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(124, 5)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(9, 21)
        Me.Label2.TabIndex = 8
        Me.Label2.Text = "|"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Top_PreviousFileButton
        '
        Me.Top_PreviousFileButton.Location = New System.Drawing.Point(437, 6)
        Me.Top_PreviousFileButton.Margin = New System.Windows.Forms.Padding(3, 1, 3, 1)
        Me.Top_PreviousFileButton.Name = "Top_PreviousFileButton"
        Me.Top_PreviousFileButton.Size = New System.Drawing.Size(86, 23)
        Me.Top_PreviousFileButton.TabIndex = 2
        Me.Top_PreviousFileButton.Text = "Previous file"
        Me.Top_PreviousFileButton.UseVisualStyleBackColor = True
        '
        'Top_NextFileButton
        '
        Me.Top_NextFileButton.Location = New System.Drawing.Point(8, 31)
        Me.Top_NextFileButton.Margin = New System.Windows.Forms.Padding(3, 1, 3, 1)
        Me.Top_NextFileButton.Name = "Top_NextFileButton"
        Me.Top_NextFileButton.Size = New System.Drawing.Size(86, 23)
        Me.Top_NextFileButton.TabIndex = 3
        Me.Top_NextFileButton.Text = "Next file"
        Me.Top_NextFileButton.UseVisualStyleBackColor = True
        '
        'MainSplitContainer
        '
        Me.MainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MainSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
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
        Me.MainSplitContainer.Size = New System.Drawing.Size(1107, 466)
        Me.MainSplitContainer.SplitterDistance = 83
        Me.MainSplitContainer.TabIndex = 5
        '
        'GroupBox1
        '
        Me.GroupBox1.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.GroupBox1.Controls.Add(Me.TableLayoutPanel5)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(1107, 83)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Current item"
        '
        'TableLayoutPanel5
        '
        Me.TableLayoutPanel5.ColumnCount = 3
        Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 313.0!))
        Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 215.0!))
        Me.TableLayoutPanel5.Controls.Add(Me.SoundSettings_TableLayoutPanel, 1, 0)
        Me.TableLayoutPanel5.Controls.Add(Me.SoundFileSelection_FlowLayoutPanel, 0, 0)
        Me.TableLayoutPanel5.Controls.Add(Me.Unlock_Button, 2, 0)
        Me.TableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel5.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel5.Name = "TableLayoutPanel5"
        Me.TableLayoutPanel5.RowCount = 2
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel5.Size = New System.Drawing.Size(1101, 64)
        Me.TableLayoutPanel5.TabIndex = 4
        '
        'SoundSettings_TableLayoutPanel
        '
        Me.SoundSettings_TableLayoutPanel.ColumnCount = 2
        Me.SoundSettings_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.SoundSettings_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 118.0!))
        Me.SoundSettings_TableLayoutPanel.Controls.Add(Me.SelectTransducer_Label, 0, 0)
        Me.SoundSettings_TableLayoutPanel.Controls.Add(Me.Transducer_ComboBox, 0, 1)
        Me.SoundSettings_TableLayoutPanel.Controls.Add(Me.Label3, 1, 0)
        Me.SoundSettings_TableLayoutPanel.Controls.Add(Me.InputChannel_ComboBox, 1, 1)
        Me.SoundSettings_TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SoundSettings_TableLayoutPanel.Location = New System.Drawing.Point(576, 3)
        Me.SoundSettings_TableLayoutPanel.Name = "SoundSettings_TableLayoutPanel"
        Me.SoundSettings_TableLayoutPanel.RowCount = 2
        Me.TableLayoutPanel5.SetRowSpan(Me.SoundSettings_TableLayoutPanel, 2)
        Me.SoundSettings_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.SoundSettings_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.SoundSettings_TableLayoutPanel.Size = New System.Drawing.Size(307, 58)
        Me.SoundSettings_TableLayoutPanel.TabIndex = 11
        '
        'SelectTransducer_Label
        '
        Me.SelectTransducer_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SelectTransducer_Label.Location = New System.Drawing.Point(3, 0)
        Me.SelectTransducer_Label.Name = "SelectTransducer_Label"
        Me.SelectTransducer_Label.Size = New System.Drawing.Size(183, 29)
        Me.SelectTransducer_Label.TabIndex = 0
        Me.SelectTransducer_Label.Text = "Select audio transducer"
        Me.SelectTransducer_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Transducer_ComboBox
        '
        Me.Transducer_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Transducer_ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.Transducer_ComboBox.FormattingEnabled = True
        Me.Transducer_ComboBox.ItemHeight = 13
        Me.Transducer_ComboBox.Location = New System.Drawing.Point(3, 32)
        Me.Transducer_ComboBox.Name = "Transducer_ComboBox"
        Me.Transducer_ComboBox.Size = New System.Drawing.Size(183, 21)
        Me.Transducer_ComboBox.TabIndex = 4
        '
        'Label3
        '
        Me.Label3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label3.Location = New System.Drawing.Point(192, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(112, 29)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "Select input channel"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'InputChannel_ComboBox
        '
        Me.InputChannel_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.InputChannel_ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.InputChannel_ComboBox.FormattingEnabled = True
        Me.InputChannel_ComboBox.Location = New System.Drawing.Point(192, 32)
        Me.InputChannel_ComboBox.Name = "InputChannel_ComboBox"
        Me.InputChannel_ComboBox.Size = New System.Drawing.Size(112, 21)
        Me.InputChannel_ComboBox.TabIndex = 5
        '
        'Unlock_Button
        '
        Me.Unlock_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Unlock_Button.Enabled = False
        Me.Unlock_Button.Location = New System.Drawing.Point(889, 3)
        Me.Unlock_Button.Name = "Unlock_Button"
        Me.TableLayoutPanel5.SetRowSpan(Me.Unlock_Button, 2)
        Me.Unlock_Button.Size = New System.Drawing.Size(209, 58)
        Me.Unlock_Button.TabIndex = 6
        Me.Unlock_Button.Text = "Unlock panel"
        Me.Unlock_Button.UseVisualStyleBackColor = True
        '
        'StopRunningTimersToolStripMenuItem
        '
        Me.StopRunningTimersToolStripMenuItem.Name = "StopRunningTimersToolStripMenuItem"
        Me.StopRunningTimersToolStripMenuItem.Size = New System.Drawing.Size(124, 20)
        Me.StopRunningTimersToolStripMenuItem.Text = "Stop running timers"
        '
        'SpeechMaterialRecorder
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1107, 512)
        Me.Controls.Add(Me.MainSplitContainer)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.KeyPreview = True
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
        CType(Me.RecordingSoundLevelMeter, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TopRecordingControlPanel.ResumeLayout(False)
        Me.TableLayoutPanel3.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.TableLayoutPanel4.ResumeLayout(False)
        Me.TableLayoutPanel4.PerformLayout()
        Me.SegmentationTab.ResumeLayout(False)
        Me.SoundFileSelection_FlowLayoutPanel.ResumeLayout(False)
        Me.SoundFileSelection_FlowLayoutPanel.PerformLayout()
        Me.MainSplitContainer.Panel1.ResumeLayout(False)
        Me.MainSplitContainer.Panel2.ResumeLayout(False)
        CType(Me.MainSplitContainer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MainSplitContainer.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.TableLayoutPanel5.ResumeLayout(False)
        Me.SoundSettings_TableLayoutPanel.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents MenuStrip1 As Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents StatusStrip1 As Windows.Forms.StatusStrip
    Friend WithEvents PrototypeRecordingLabel As Windows.Forms.ToolStripStatusLabel
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
    Friend WithEvents StartRecordingAutomaticallyOnNextpreviousToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents AudioSettingsToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents SegmentationToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripStatusLabel2 As Windows.Forms.ToolStripStatusLabel
    Friend WithEvents AutoRecordingStatusLabel As Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusLabel3 As Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToggleBackgroundSoundWhileRecordingonoffToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents BackgroundSoundStatusLabel As Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusLabel1 As Windows.Forms.ToolStripStatusLabel
    Friend WithEvents MoveSegmentationsToZeroCrossingsToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents RecordingTabMainSplitContainer As Windows.Forms.SplitContainer
    Friend WithEvents TopRecordingControlPanel As Windows.Forms.Panel
    Friend WithEvents ListenButton As Windows.Forms.Button
    Friend WithEvents StartRecordingButton As Windows.Forms.Button
    Friend WithEvents ItemProgressBar As Windows.Forms.ProgressBar
    Friend WithEvents FileComboBox As Windows.Forms.ComboBox
    Friend WithEvents ItemProgressLabel As Windows.Forms.Label
    Friend WithEvents RecordingSoundLevelMeter As Audio.Graphics.SoundLevelMeter
    Friend WithEvents ToggleSoundLevelMeteronoffToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents SoundFileSelection_FlowLayoutPanel As Windows.Forms.FlowLayoutPanel
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents FileName_Label As Windows.Forms.Label
    Friend WithEvents Label2 As Windows.Forms.Label
    Friend WithEvents MainSplitContainer As Windows.Forms.SplitContainer
    Friend WithEvents Top_PreviousFileButton As Windows.Forms.Button
    Friend WithEvents Top_NextFileButton As Windows.Forms.Button
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
    Friend WithEvents Transcription_AutoHeightTextBox As AutoHeightTextBox
    Friend WithEvents SegmentationPanel As Windows.Forms.Panel
    Friend WithEvents LoadWaveFileSMAIXMLChunkRequiredToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents SaveWaveFileToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents CloseToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents SaveWaveFileAsToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents SpectrogramSettingsToolStripMenuItem1 As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ShowSpectrogramToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents DrawNormalizedWaveToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents SetPaddingTimesToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents IntersentenceTimesToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents PaddingTimeComboBox As Windows.Forms.ToolStripComboBox
    Friend WithEvents InterSentenceTimeComboBox As Windows.Forms.ToolStripComboBox
    Friend WithEvents PresentationLevelToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents PresentationLevel_ToolStripComboBox As Windows.Forms.ToolStripComboBox
    Friend WithEvents BackgroundSoundLevelToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents BackgroundSoundLevel_ToolStripComboBox As Windows.Forms.ToolStripComboBox
    Friend WithEvents ToolStripStatusLabel5 As Windows.Forms.ToolStripStatusLabel
    Friend WithEvents PresentationLevelToolStripStatusLabel As Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusLabel4 As Windows.Forms.ToolStripStatusLabel
    Friend WithEvents BackgroundLevel_ToolStripStatusLabel As Windows.Forms.ToolStripStatusLabel
    Friend WithEvents PresentationSoundLevelTypeToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents BackgroundSoundLevelTypeToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents TableLayoutPanel5 As Windows.Forms.TableLayoutPanel
    Friend WithEvents SoundSettings_TableLayoutPanel As Windows.Forms.TableLayoutPanel
    Friend WithEvents SelectTransducer_Label As Windows.Forms.Label
    Friend WithEvents Transducer_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents ExportToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents AllSoundsFiles_ToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents AllSoundsFiles_AllSegmentations_ToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents AllSoundsFiles_SentenceTimes_ToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents CurrentFile_ToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents CurrentFile_AllSegmentations_ToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents CurrentFile_SentenceTimes_ToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents RecordingStopDelayToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents RecordingStopDelay_ToolStripComboBox As Windows.Forms.ToolStripComboBox
    Friend WithEvents ShowSpellingToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ShowTranscriptionToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents TelepromterBTToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ConnectToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents DisconnectToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents Unlock_Button As Windows.Forms.Button
    Friend WithEvents StopPlayback_Button As Windows.Forms.Button
    Friend WithEvents ToolStripStatusLabel6 As Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStrip_BT_StatusLabel As Windows.Forms.ToolStripStatusLabel
    Friend WithEvents Label3 As Windows.Forms.Label
    Friend WithEvents InputChannel_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents ValidationToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents EnforceValidationInLoadedSoundToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents InvalidateEverythingInLoadedSoundToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents UsePrototypeRecordingsToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ValidateAllLinguisticLevels_ToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ValidateSentenceLevel_ToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ValidateWordLevel_ToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ValidatePhonemeLevel_ToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents InvalidateAllLinguisticLevels_ToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents InvalidateSentenceLevel_ToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents InvalidateWordLevel_ToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents InvalidatePhonemeLevel_ToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents NextpreviousNonrecordedOnPageUpdownToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents StopRunningTimersToolStripMenuItem As Windows.Forms.ToolStripMenuItem
End Class
