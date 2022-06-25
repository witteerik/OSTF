<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MediaSetSetupControl
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.LoadedSpeechMaterialName_TextBox = New System.Windows.Forms.TextBox()
        Me.Splitter1 = New System.Windows.Forms.Splitter()
        Me.Splitter2 = New System.Windows.Forms.Splitter()
        Me.NewMediaSet_Button = New System.Windows.Forms.Button()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.Edit_TableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.TalkerName_TextBox = New System.Windows.Forms.TextBox()
        Me.WaveFileEncoding_ComboBox = New System.Windows.Forms.ComboBox()
        Me.SaveMediaSetSpecification_Button = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.MediaSetName_TextBox = New System.Windows.Forms.TextBox()
        Me.TalkerDialect_TextBox = New System.Windows.Forms.TextBox()
        Me.VoiceType_TextBox = New System.Windows.Forms.TextBox()
        Me.MediaParentFolder_TextBox = New System.Windows.Forms.TextBox()
        Me.MaskerParentFolder_TextBox = New System.Windows.Forms.TextBox()
        Me.BackgroundNonspeechParentFolder_TextBox = New System.Windows.Forms.TextBox()
        Me.BackgroundSpeechParentFolder_TextBox = New System.Windows.Forms.TextBox()
        Me.PrototypeMediaParentFolder_TextBox = New System.Windows.Forms.TextBox()
        Me.MasterPrototypeRecordingPath_TextBox = New System.Windows.Forms.TextBox()
        Me.TalkerGender_ComboBox = New System.Windows.Forms.ComboBox()
        Me.WaveFileBitDepth_ComboBox = New System.Windows.Forms.ComboBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.LombardNoisePath_TextBox = New System.Windows.Forms.TextBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.LaunchRecorder_Button = New System.Windows.Forms.Button()
        Me.RandomOrder_CheckBox = New System.Windows.Forms.CheckBox()
        Me.SpecificPrototypeRecording_RadioButton = New System.Windows.Forms.RadioButton()
        Me.NoPrototypeRecording_RadioButton = New System.Windows.Forms.RadioButton()
        Me.MasterPrototypeRecording_RadioButton = New System.Windows.Forms.RadioButton()
        Me.LoadOstaMediaSetControl1 = New SpeechTestFramework.LoadOstaMediaSetControl()
        Me.LoadOstaTestSpecificationControl1 = New SpeechTestFramework.LoadOstaTestSpecificationControl()
        Me.TalkerAge_IntegerParsingTextBox = New SpeechTestFramework.IntegerParsingTextBox()
        Me.MediaAudioItems_IntegerParsingTextBox = New SpeechTestFramework.IntegerParsingTextBox()
        Me.MaskerAudioItems_IntegerParsingTextBox = New SpeechTestFramework.IntegerParsingTextBox()
        Me.MediaImageItems_IntegerParsingTextBox = New SpeechTestFramework.IntegerParsingTextBox()
        Me.MaskerImageItems_IntegerParsingTextBox = New SpeechTestFramework.IntegerParsingTextBox()
        Me.WaveFileSampleRate_IntegerParsingTextBox = New SpeechTestFramework.IntegerParsingTextBox()
        Me.BackgroundNonspeechRealisticLevel_DoubleParsingTextBox = New SpeechTestFramework.DoubleParsingTextBox()
        Me.PrototypeRecordingLevel_DoubleParsingTextBox = New SpeechTestFramework.DoubleParsingTextBox()
        Me.LombardNoiseLevel_DoubleParsingTextBox = New SpeechTestFramework.DoubleParsingTextBox()
        Me.Label26 = New System.Windows.Forms.Label()
        Me.SoundFileLevelComboBox = New System.Windows.Forms.ComboBox()
        Me.TableLayoutPanel2.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.Edit_TableLayoutPanel.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.ColumnCount = 3
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.LoadOstaMediaSetControl1, 0, 3)
        Me.TableLayoutPanel2.Controls.Add(Me.LoadOstaTestSpecificationControl1, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.Label25, 0, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.LoadedSpeechMaterialName_TextBox, 1, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.Splitter1, 0, 2)
        Me.TableLayoutPanel2.Controls.Add(Me.Splitter2, 0, 4)
        Me.TableLayoutPanel2.Controls.Add(Me.NewMediaSet_Button, 2, 3)
        Me.TableLayoutPanel2.Controls.Add(Me.SplitContainer1, 0, 5)
        Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 7
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(841, 815)
        Me.TableLayoutPanel2.TabIndex = 53
        '
        'Label25
        '
        Me.Label25.BackColor = System.Drawing.SystemColors.Control
        Me.Label25.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label25.Location = New System.Drawing.Point(3, 60)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(144, 30)
        Me.Label25.TabIndex = 53
        Me.Label25.Text = "Loaded speech material:"
        Me.Label25.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LoadedSpeechMaterialName_TextBox
        '
        Me.LoadedSpeechMaterialName_TextBox.BackColor = System.Drawing.SystemColors.Control
        Me.TableLayoutPanel2.SetColumnSpan(Me.LoadedSpeechMaterialName_TextBox, 2)
        Me.LoadedSpeechMaterialName_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LoadedSpeechMaterialName_TextBox.Location = New System.Drawing.Point(153, 63)
        Me.LoadedSpeechMaterialName_TextBox.Name = "LoadedSpeechMaterialName_TextBox"
        Me.LoadedSpeechMaterialName_TextBox.ReadOnly = True
        Me.LoadedSpeechMaterialName_TextBox.Size = New System.Drawing.Size(685, 20)
        Me.LoadedSpeechMaterialName_TextBox.TabIndex = 54
        Me.LoadedSpeechMaterialName_TextBox.Text = "No speech material loaded"
        '
        'Splitter1
        '
        Me.Splitter1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TableLayoutPanel2.SetColumnSpan(Me.Splitter1, 3)
        Me.Splitter1.Cursor = System.Windows.Forms.Cursors.HSplit
        Me.Splitter1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Splitter1.Location = New System.Drawing.Point(3, 93)
        Me.Splitter1.Name = "Splitter1"
        Me.Splitter1.Size = New System.Drawing.Size(835, 2)
        Me.Splitter1.TabIndex = 56
        Me.Splitter1.TabStop = False
        '
        'Splitter2
        '
        Me.Splitter2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TableLayoutPanel2.SetColumnSpan(Me.Splitter2, 3)
        Me.Splitter2.Dock = System.Windows.Forms.DockStyle.Top
        Me.Splitter2.Location = New System.Drawing.Point(3, 161)
        Me.Splitter2.Name = "Splitter2"
        Me.Splitter2.Size = New System.Drawing.Size(835, 2)
        Me.Splitter2.TabIndex = 57
        Me.Splitter2.TabStop = False
        '
        'NewMediaSet_Button
        '
        Me.NewMediaSet_Button.Location = New System.Drawing.Point(694, 101)
        Me.NewMediaSet_Button.Name = "NewMediaSet_Button"
        Me.NewMediaSet_Button.Size = New System.Drawing.Size(143, 54)
        Me.NewMediaSet_Button.TabIndex = 55
        Me.NewMediaSet_Button.Text = "Create new media set"
        Me.NewMediaSet_Button.UseVisualStyleBackColor = True
        '
        'SplitContainer1
        '
        Me.TableLayoutPanel2.SetColumnSpan(Me.SplitContainer1, 3)
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(3, 169)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.Edit_TableLayoutPanel)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.GroupBox1)
        Me.SplitContainer1.Size = New System.Drawing.Size(835, 613)
        Me.SplitContainer1.SplitterDistance = 435
        Me.SplitContainer1.TabIndex = 58
        '
        'Edit_TableLayoutPanel
        '
        Me.Edit_TableLayoutPanel.AutoScroll = True
        Me.Edit_TableLayoutPanel.AutoSize = True
        Me.Edit_TableLayoutPanel.ColumnCount = 2
        Me.Edit_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 300.0!))
        Me.Edit_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.Edit_TableLayoutPanel.Controls.Add(Me.TalkerName_TextBox, 1, 2)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.WaveFileEncoding_ComboBox, 1, 24)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.SaveMediaSetSpecification_Button, 0, 25)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label1, 0, 0)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label2, 0, 1)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label3, 0, 2)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label4, 0, 3)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label13, 0, 12)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label12, 0, 11)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label11, 0, 10)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label10, 0, 9)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label9, 0, 8)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label5, 0, 4)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label6, 0, 6)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label7, 0, 7)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label14, 0, 13)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label15, 0, 14)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label16, 0, 16)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label17, 0, 17)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label18, 0, 18)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label19, 0, 22)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label20, 0, 23)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label21, 0, 24)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.TalkerAge_IntegerParsingTextBox, 1, 4)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.MediaAudioItems_IntegerParsingTextBox, 1, 8)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.MaskerAudioItems_IntegerParsingTextBox, 1, 9)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.MediaImageItems_IntegerParsingTextBox, 1, 10)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.MaskerImageItems_IntegerParsingTextBox, 1, 11)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.WaveFileSampleRate_IntegerParsingTextBox, 1, 22)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.MediaSetName_TextBox, 1, 1)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.TalkerDialect_TextBox, 1, 6)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.VoiceType_TextBox, 1, 7)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.MediaParentFolder_TextBox, 1, 12)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.MaskerParentFolder_TextBox, 1, 13)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.BackgroundNonspeechParentFolder_TextBox, 1, 14)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.BackgroundSpeechParentFolder_TextBox, 1, 16)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.PrototypeMediaParentFolder_TextBox, 1, 17)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.MasterPrototypeRecordingPath_TextBox, 1, 18)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.TalkerGender_ComboBox, 1, 3)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.WaveFileBitDepth_ComboBox, 1, 23)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label8, 0, 15)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.BackgroundNonspeechRealisticLevel_DoubleParsingTextBox, 1, 15)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label23, 0, 19)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.PrototypeRecordingLevel_DoubleParsingTextBox, 1, 19)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label22, 0, 20)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label24, 0, 21)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.LombardNoiseLevel_DoubleParsingTextBox, 1, 21)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.LombardNoisePath_TextBox, 1, 20)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label26, 0, 5)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.SoundFileLevelComboBox, 1, 5)
        Me.Edit_TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Edit_TableLayoutPanel.Location = New System.Drawing.Point(0, 0)
        Me.Edit_TableLayoutPanel.Name = "Edit_TableLayoutPanel"
        Me.Edit_TableLayoutPanel.RowCount = 26
        Me.Edit_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23.0!))
        Me.Edit_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.Edit_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.Edit_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.Edit_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.Edit_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.Edit_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.Edit_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.Edit_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.Edit_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.Edit_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.Edit_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.Edit_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.Edit_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.Edit_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.Edit_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.Edit_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.Edit_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.Edit_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.Edit_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.Edit_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.Edit_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.Edit_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.Edit_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.Edit_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.Edit_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
        Me.Edit_TableLayoutPanel.Size = New System.Drawing.Size(435, 613)
        Me.Edit_TableLayoutPanel.TabIndex = 0
        '
        'TalkerName_TextBox
        '
        Me.TalkerName_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TalkerName_TextBox.Location = New System.Drawing.Point(303, 51)
        Me.TalkerName_TextBox.Name = "TalkerName_TextBox"
        Me.TalkerName_TextBox.Size = New System.Drawing.Size(129, 20)
        Me.TalkerName_TextBox.TabIndex = 42
        '
        'WaveFileEncoding_ComboBox
        '
        Me.WaveFileEncoding_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WaveFileEncoding_ComboBox.FormattingEnabled = True
        Me.WaveFileEncoding_ComboBox.Location = New System.Drawing.Point(303, 601)
        Me.WaveFileEncoding_ComboBox.Name = "WaveFileEncoding_ComboBox"
        Me.WaveFileEncoding_ComboBox.Size = New System.Drawing.Size(129, 21)
        Me.WaveFileEncoding_ComboBox.TabIndex = 41
        '
        'SaveMediaSetSpecification_Button
        '
        Me.Edit_TableLayoutPanel.SetColumnSpan(Me.SaveMediaSetSpecification_Button, 2)
        Me.SaveMediaSetSpecification_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SaveMediaSetSpecification_Button.Location = New System.Drawing.Point(3, 626)
        Me.SaveMediaSetSpecification_Button.Name = "SaveMediaSetSpecification_Button"
        Me.SaveMediaSetSpecification_Button.Size = New System.Drawing.Size(429, 34)
        Me.SaveMediaSetSpecification_Button.TabIndex = 0
        Me.SaveMediaSetSpecification_Button.Text = "Save changes"
        Me.SaveMediaSetSpecification_Button.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Edit_TableLayoutPanel.SetColumnSpan(Me.Label1, 2)
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Location = New System.Drawing.Point(3, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(429, 23)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Edit the fields below to modify the media set, and press 'Save changes' to save y" &
    "our changes"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label2
        '
        Me.Label2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label2.Location = New System.Drawing.Point(3, 23)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(294, 25)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Media set name"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label3
        '
        Me.Label3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label3.Location = New System.Drawing.Point(3, 48)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(294, 25)
        Me.Label3.TabIndex = 3
        Me.Label3.Text = "Talker name"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label4
        '
        Me.Label4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label4.Location = New System.Drawing.Point(3, 73)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(294, 25)
        Me.Label4.TabIndex = 4
        Me.Label4.Text = "Talker gender"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label13
        '
        Me.Label13.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label13.Location = New System.Drawing.Point(3, 298)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(294, 25)
        Me.Label13.TabIndex = 13
        Me.Label13.Text = "Subfolder containing target files"
        Me.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label12
        '
        Me.Label12.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label12.Location = New System.Drawing.Point(3, 273)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(294, 25)
        Me.Label12.TabIndex = 12
        Me.Label12.Text = "Number of duplicate image maskers"
        Me.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label11
        '
        Me.Label11.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label11.Location = New System.Drawing.Point(3, 248)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(294, 25)
        Me.Label11.TabIndex = 11
        Me.Label11.Text = "Number of duplicate image targets"
        Me.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label10
        '
        Me.Label10.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label10.Location = New System.Drawing.Point(3, 223)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(294, 25)
        Me.Label10.TabIndex = 10
        Me.Label10.Text = "Number of duplicate audio maskers"
        Me.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label9
        '
        Me.Label9.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label9.Location = New System.Drawing.Point(3, 198)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(294, 25)
        Me.Label9.TabIndex = 9
        Me.Label9.Text = "Number of duplicate audio targets"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label5
        '
        Me.Label5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label5.Location = New System.Drawing.Point(3, 98)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(294, 25)
        Me.Label5.TabIndex = 5
        Me.Label5.Text = "Talker age (years)"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label6
        '
        Me.Label6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label6.Location = New System.Drawing.Point(3, 148)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(294, 25)
        Me.Label6.TabIndex = 6
        Me.Label6.Text = "Talker dialect"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label7
        '
        Me.Label7.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label7.Location = New System.Drawing.Point(3, 173)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(294, 25)
        Me.Label7.TabIndex = 7
        Me.Label7.Text = "Voice type"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label14
        '
        Me.Label14.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label14.Location = New System.Drawing.Point(3, 323)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(294, 25)
        Me.Label14.TabIndex = 14
        Me.Label14.Text = "Subfolder containing masker files"
        Me.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label15
        '
        Me.Label15.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label15.Location = New System.Drawing.Point(3, 348)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(294, 25)
        Me.Label15.TabIndex = 15
        Me.Label15.Text = "Subfolder containing background non-speech files"
        Me.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label16
        '
        Me.Label16.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label16.Location = New System.Drawing.Point(3, 398)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(294, 25)
        Me.Label16.TabIndex = 16
        Me.Label16.Text = "Subfolder containing background speech files"
        Me.Label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label17
        '
        Me.Label17.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label17.Location = New System.Drawing.Point(3, 423)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(294, 25)
        Me.Label17.TabIndex = 17
        Me.Label17.Text = "Subfolder containing target prototype recordings"
        Me.Label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label18
        '
        Me.Label18.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label18.Location = New System.Drawing.Point(3, 448)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(294, 25)
        Me.Label18.TabIndex = 18
        Me.Label18.Text = "Subpath to the master prototype recording"
        Me.Label18.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label19.Location = New System.Drawing.Point(3, 548)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(294, 25)
        Me.Label19.TabIndex = 19
        Me.Label19.Text = "Wave file sample rate"
        Me.Label19.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label20
        '
        Me.Label20.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label20.Location = New System.Drawing.Point(3, 573)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(294, 25)
        Me.Label20.TabIndex = 20
        Me.Label20.Text = "Wave file bit depth"
        Me.Label20.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label21
        '
        Me.Label21.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label21.Location = New System.Drawing.Point(3, 598)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(294, 25)
        Me.Label21.TabIndex = 21
        Me.Label21.Text = "Wave file encoding"
        Me.Label21.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'MediaSetName_TextBox
        '
        Me.MediaSetName_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MediaSetName_TextBox.Location = New System.Drawing.Point(303, 26)
        Me.MediaSetName_TextBox.Name = "MediaSetName_TextBox"
        Me.MediaSetName_TextBox.Size = New System.Drawing.Size(129, 20)
        Me.MediaSetName_TextBox.TabIndex = 29
        '
        'TalkerDialect_TextBox
        '
        Me.TalkerDialect_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TalkerDialect_TextBox.Location = New System.Drawing.Point(303, 151)
        Me.TalkerDialect_TextBox.Name = "TalkerDialect_TextBox"
        Me.TalkerDialect_TextBox.Size = New System.Drawing.Size(129, 20)
        Me.TalkerDialect_TextBox.TabIndex = 31
        '
        'VoiceType_TextBox
        '
        Me.VoiceType_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.VoiceType_TextBox.Location = New System.Drawing.Point(303, 176)
        Me.VoiceType_TextBox.Name = "VoiceType_TextBox"
        Me.VoiceType_TextBox.Size = New System.Drawing.Size(129, 20)
        Me.VoiceType_TextBox.TabIndex = 32
        '
        'MediaParentFolder_TextBox
        '
        Me.MediaParentFolder_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MediaParentFolder_TextBox.Location = New System.Drawing.Point(303, 301)
        Me.MediaParentFolder_TextBox.Name = "MediaParentFolder_TextBox"
        Me.MediaParentFolder_TextBox.Size = New System.Drawing.Size(129, 20)
        Me.MediaParentFolder_TextBox.TabIndex = 33
        '
        'MaskerParentFolder_TextBox
        '
        Me.MaskerParentFolder_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MaskerParentFolder_TextBox.Location = New System.Drawing.Point(303, 326)
        Me.MaskerParentFolder_TextBox.Name = "MaskerParentFolder_TextBox"
        Me.MaskerParentFolder_TextBox.Size = New System.Drawing.Size(129, 20)
        Me.MaskerParentFolder_TextBox.TabIndex = 34
        '
        'BackgroundNonspeechParentFolder_TextBox
        '
        Me.BackgroundNonspeechParentFolder_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BackgroundNonspeechParentFolder_TextBox.Location = New System.Drawing.Point(303, 351)
        Me.BackgroundNonspeechParentFolder_TextBox.Name = "BackgroundNonspeechParentFolder_TextBox"
        Me.BackgroundNonspeechParentFolder_TextBox.Size = New System.Drawing.Size(129, 20)
        Me.BackgroundNonspeechParentFolder_TextBox.TabIndex = 35
        '
        'BackgroundSpeechParentFolder_TextBox
        '
        Me.BackgroundSpeechParentFolder_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BackgroundSpeechParentFolder_TextBox.Location = New System.Drawing.Point(303, 401)
        Me.BackgroundSpeechParentFolder_TextBox.Name = "BackgroundSpeechParentFolder_TextBox"
        Me.BackgroundSpeechParentFolder_TextBox.Size = New System.Drawing.Size(129, 20)
        Me.BackgroundSpeechParentFolder_TextBox.TabIndex = 36
        '
        'PrototypeMediaParentFolder_TextBox
        '
        Me.PrototypeMediaParentFolder_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PrototypeMediaParentFolder_TextBox.Location = New System.Drawing.Point(303, 426)
        Me.PrototypeMediaParentFolder_TextBox.Name = "PrototypeMediaParentFolder_TextBox"
        Me.PrototypeMediaParentFolder_TextBox.Size = New System.Drawing.Size(129, 20)
        Me.PrototypeMediaParentFolder_TextBox.TabIndex = 37
        '
        'MasterPrototypeRecordingPath_TextBox
        '
        Me.MasterPrototypeRecordingPath_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MasterPrototypeRecordingPath_TextBox.Location = New System.Drawing.Point(303, 451)
        Me.MasterPrototypeRecordingPath_TextBox.Name = "MasterPrototypeRecordingPath_TextBox"
        Me.MasterPrototypeRecordingPath_TextBox.Size = New System.Drawing.Size(129, 20)
        Me.MasterPrototypeRecordingPath_TextBox.TabIndex = 38
        '
        'TalkerGender_ComboBox
        '
        Me.TalkerGender_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TalkerGender_ComboBox.FormattingEnabled = True
        Me.TalkerGender_ComboBox.Location = New System.Drawing.Point(303, 76)
        Me.TalkerGender_ComboBox.Name = "TalkerGender_ComboBox"
        Me.TalkerGender_ComboBox.Size = New System.Drawing.Size(129, 21)
        Me.TalkerGender_ComboBox.TabIndex = 39
        '
        'WaveFileBitDepth_ComboBox
        '
        Me.WaveFileBitDepth_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WaveFileBitDepth_ComboBox.FormattingEnabled = True
        Me.WaveFileBitDepth_ComboBox.Location = New System.Drawing.Point(303, 576)
        Me.WaveFileBitDepth_ComboBox.Name = "WaveFileBitDepth_ComboBox"
        Me.WaveFileBitDepth_ComboBox.Size = New System.Drawing.Size(129, 21)
        Me.WaveFileBitDepth_ComboBox.TabIndex = 40
        '
        'Label8
        '
        Me.Label8.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label8.Location = New System.Drawing.Point(3, 373)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(294, 25)
        Me.Label8.TabIndex = 8
        Me.Label8.Text = "Background (non-speech) realistic sound level (dB SPL)"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label23
        '
        Me.Label23.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label23.Location = New System.Drawing.Point(3, 473)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(294, 25)
        Me.Label23.TabIndex = 44
        Me.Label23.Text = "Prototype recording level (dBC, 50 ms)"
        Me.Label23.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label22
        '
        Me.Label22.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label22.Location = New System.Drawing.Point(3, 498)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(294, 25)
        Me.Label22.TabIndex = 46
        Me.Label22.Text = "Subpath to 'Lombard' (recording) noise"
        Me.Label22.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label24
        '
        Me.Label24.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label24.Location = New System.Drawing.Point(3, 523)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(294, 25)
        Me.Label24.TabIndex = 47
        Me.Label24.Text = "'Lombard' (recording) noise level (dBC, 50 ms)"
        Me.Label24.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LombardNoisePath_TextBox
        '
        Me.LombardNoisePath_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LombardNoisePath_TextBox.Location = New System.Drawing.Point(303, 501)
        Me.LombardNoisePath_TextBox.Name = "LombardNoisePath_TextBox"
        Me.LombardNoisePath_TextBox.Size = New System.Drawing.Size(129, 20)
        Me.LombardNoisePath_TextBox.TabIndex = 49
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.TableLayoutPanel1)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(396, 613)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Recording and segmentation tool"
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.LaunchRecorder_Button, 0, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.RandomOrder_CheckBox, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.SpecificPrototypeRecording_RadioButton, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.NoPrototypeRecording_RadioButton, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.MasterPrototypeRecording_RadioButton, 0, 1)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 6
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(390, 594)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'LaunchRecorder_Button
        '
        Me.LaunchRecorder_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LaunchRecorder_Button.Location = New System.Drawing.Point(3, 103)
        Me.LaunchRecorder_Button.Name = "LaunchRecorder_Button"
        Me.LaunchRecorder_Button.Size = New System.Drawing.Size(384, 24)
        Me.LaunchRecorder_Button.TabIndex = 0
        Me.LaunchRecorder_Button.Text = "Launch recording and segmentation tool"
        Me.LaunchRecorder_Button.UseVisualStyleBackColor = True
        '
        'RandomOrder_CheckBox
        '
        Me.RandomOrder_CheckBox.AutoSize = True
        Me.RandomOrder_CheckBox.Checked = True
        Me.RandomOrder_CheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.RandomOrder_CheckBox.Location = New System.Drawing.Point(3, 3)
        Me.RandomOrder_CheckBox.Name = "RandomOrder_CheckBox"
        Me.RandomOrder_CheckBox.Size = New System.Drawing.Size(106, 17)
        Me.RandomOrder_CheckBox.TabIndex = 1
        Me.RandomOrder_CheckBox.Text = "Randomize order"
        Me.RandomOrder_CheckBox.UseVisualStyleBackColor = True
        '
        'SpecificPrototypeRecording_RadioButton
        '
        Me.SpecificPrototypeRecording_RadioButton.AutoSize = True
        Me.SpecificPrototypeRecording_RadioButton.Location = New System.Drawing.Point(3, 53)
        Me.SpecificPrototypeRecording_RadioButton.Name = "SpecificPrototypeRecording_RadioButton"
        Me.SpecificPrototypeRecording_RadioButton.Size = New System.Drawing.Size(143, 17)
        Me.SpecificPrototypeRecording_RadioButton.TabIndex = 2
        Me.SpecificPrototypeRecording_RadioButton.TabStop = True
        Me.SpecificPrototypeRecording_RadioButton.Text = "Use prototype recordings"
        Me.SpecificPrototypeRecording_RadioButton.UseVisualStyleBackColor = True
        '
        'NoPrototypeRecording_RadioButton
        '
        Me.NoPrototypeRecording_RadioButton.AutoSize = True
        Me.NoPrototypeRecording_RadioButton.Location = New System.Drawing.Point(3, 78)
        Me.NoPrototypeRecording_RadioButton.Name = "NoPrototypeRecording_RadioButton"
        Me.NoPrototypeRecording_RadioButton.Size = New System.Drawing.Size(138, 17)
        Me.NoPrototypeRecording_RadioButton.TabIndex = 3
        Me.NoPrototypeRecording_RadioButton.TabStop = True
        Me.NoPrototypeRecording_RadioButton.Text = "No prototype recordings"
        Me.NoPrototypeRecording_RadioButton.UseVisualStyleBackColor = True
        '
        'MasterPrototypeRecording_RadioButton
        '
        Me.MasterPrototypeRecording_RadioButton.AutoSize = True
        Me.MasterPrototypeRecording_RadioButton.Location = New System.Drawing.Point(3, 28)
        Me.MasterPrototypeRecording_RadioButton.Name = "MasterPrototypeRecording_RadioButton"
        Me.MasterPrototypeRecording_RadioButton.Size = New System.Drawing.Size(172, 17)
        Me.MasterPrototypeRecording_RadioButton.TabIndex = 4
        Me.MasterPrototypeRecording_RadioButton.TabStop = True
        Me.MasterPrototypeRecording_RadioButton.Text = "Use master prototype recording"
        Me.MasterPrototypeRecording_RadioButton.UseVisualStyleBackColor = True
        '
        'LoadOstaMediaSetControl1
        '
        Me.LoadOstaMediaSetControl1.BackColor = System.Drawing.SystemColors.Control
        Me.TableLayoutPanel2.SetColumnSpan(Me.LoadOstaMediaSetControl1, 2)
        Me.LoadOstaMediaSetControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LoadOstaMediaSetControl1.Location = New System.Drawing.Point(3, 101)
        Me.LoadOstaMediaSetControl1.Name = "LoadOstaMediaSetControl1"
        Me.LoadOstaMediaSetControl1.SelectedMediaSet = Nothing
        Me.LoadOstaMediaSetControl1.SelectedTestSpecification = Nothing
        Me.LoadOstaMediaSetControl1.Size = New System.Drawing.Size(685, 54)
        Me.LoadOstaMediaSetControl1.TabIndex = 51
        '
        'LoadOstaTestSpecificationControl1
        '
        Me.LoadOstaTestSpecificationControl1.BackColor = System.Drawing.SystemColors.Control
        Me.TableLayoutPanel2.SetColumnSpan(Me.LoadOstaTestSpecificationControl1, 3)
        Me.LoadOstaTestSpecificationControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LoadOstaTestSpecificationControl1.Location = New System.Drawing.Point(3, 3)
        Me.LoadOstaTestSpecificationControl1.Name = "LoadOstaTestSpecificationControl1"
        Me.LoadOstaTestSpecificationControl1.SelectedTestSpecification = Nothing
        Me.LoadOstaTestSpecificationControl1.Size = New System.Drawing.Size(835, 54)
        Me.LoadOstaTestSpecificationControl1.TabIndex = 52
        '
        'TalkerAge_IntegerParsingTextBox
        '
        Me.TalkerAge_IntegerParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TalkerAge_IntegerParsingTextBox.ForeColor = System.Drawing.Color.Red
        Me.TalkerAge_IntegerParsingTextBox.Location = New System.Drawing.Point(303, 101)
        Me.TalkerAge_IntegerParsingTextBox.Name = "TalkerAge_IntegerParsingTextBox"
        Me.TalkerAge_IntegerParsingTextBox.Size = New System.Drawing.Size(129, 20)
        Me.TalkerAge_IntegerParsingTextBox.TabIndex = 22
        '
        'MediaAudioItems_IntegerParsingTextBox
        '
        Me.MediaAudioItems_IntegerParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MediaAudioItems_IntegerParsingTextBox.ForeColor = System.Drawing.Color.Red
        Me.MediaAudioItems_IntegerParsingTextBox.Location = New System.Drawing.Point(303, 201)
        Me.MediaAudioItems_IntegerParsingTextBox.Name = "MediaAudioItems_IntegerParsingTextBox"
        Me.MediaAudioItems_IntegerParsingTextBox.Size = New System.Drawing.Size(129, 20)
        Me.MediaAudioItems_IntegerParsingTextBox.TabIndex = 24
        '
        'MaskerAudioItems_IntegerParsingTextBox
        '
        Me.MaskerAudioItems_IntegerParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MaskerAudioItems_IntegerParsingTextBox.ForeColor = System.Drawing.Color.Red
        Me.MaskerAudioItems_IntegerParsingTextBox.Location = New System.Drawing.Point(303, 226)
        Me.MaskerAudioItems_IntegerParsingTextBox.Name = "MaskerAudioItems_IntegerParsingTextBox"
        Me.MaskerAudioItems_IntegerParsingTextBox.Size = New System.Drawing.Size(129, 20)
        Me.MaskerAudioItems_IntegerParsingTextBox.TabIndex = 25
        '
        'MediaImageItems_IntegerParsingTextBox
        '
        Me.MediaImageItems_IntegerParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MediaImageItems_IntegerParsingTextBox.ForeColor = System.Drawing.Color.Red
        Me.MediaImageItems_IntegerParsingTextBox.Location = New System.Drawing.Point(303, 251)
        Me.MediaImageItems_IntegerParsingTextBox.Name = "MediaImageItems_IntegerParsingTextBox"
        Me.MediaImageItems_IntegerParsingTextBox.Size = New System.Drawing.Size(129, 20)
        Me.MediaImageItems_IntegerParsingTextBox.TabIndex = 26
        '
        'MaskerImageItems_IntegerParsingTextBox
        '
        Me.MaskerImageItems_IntegerParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MaskerImageItems_IntegerParsingTextBox.ForeColor = System.Drawing.Color.Red
        Me.MaskerImageItems_IntegerParsingTextBox.Location = New System.Drawing.Point(303, 276)
        Me.MaskerImageItems_IntegerParsingTextBox.Name = "MaskerImageItems_IntegerParsingTextBox"
        Me.MaskerImageItems_IntegerParsingTextBox.Size = New System.Drawing.Size(129, 20)
        Me.MaskerImageItems_IntegerParsingTextBox.TabIndex = 27
        '
        'WaveFileSampleRate_IntegerParsingTextBox
        '
        Me.WaveFileSampleRate_IntegerParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WaveFileSampleRate_IntegerParsingTextBox.ForeColor = System.Drawing.Color.Red
        Me.WaveFileSampleRate_IntegerParsingTextBox.Location = New System.Drawing.Point(303, 551)
        Me.WaveFileSampleRate_IntegerParsingTextBox.Name = "WaveFileSampleRate_IntegerParsingTextBox"
        Me.WaveFileSampleRate_IntegerParsingTextBox.Size = New System.Drawing.Size(129, 20)
        Me.WaveFileSampleRate_IntegerParsingTextBox.TabIndex = 28
        '
        'BackgroundNonspeechRealisticLevel_DoubleParsingTextBox
        '
        Me.BackgroundNonspeechRealisticLevel_DoubleParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BackgroundNonspeechRealisticLevel_DoubleParsingTextBox.ForeColor = System.Drawing.Color.Red
        Me.BackgroundNonspeechRealisticLevel_DoubleParsingTextBox.Location = New System.Drawing.Point(303, 376)
        Me.BackgroundNonspeechRealisticLevel_DoubleParsingTextBox.Name = "BackgroundNonspeechRealisticLevel_DoubleParsingTextBox"
        Me.BackgroundNonspeechRealisticLevel_DoubleParsingTextBox.Size = New System.Drawing.Size(129, 20)
        Me.BackgroundNonspeechRealisticLevel_DoubleParsingTextBox.TabIndex = 23
        '
        'PrototypeRecordingLevel_DoubleParsingTextBox
        '
        Me.PrototypeRecordingLevel_DoubleParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PrototypeRecordingLevel_DoubleParsingTextBox.ForeColor = System.Drawing.Color.Red
        Me.PrototypeRecordingLevel_DoubleParsingTextBox.Location = New System.Drawing.Point(303, 476)
        Me.PrototypeRecordingLevel_DoubleParsingTextBox.Name = "PrototypeRecordingLevel_DoubleParsingTextBox"
        Me.PrototypeRecordingLevel_DoubleParsingTextBox.Size = New System.Drawing.Size(129, 20)
        Me.PrototypeRecordingLevel_DoubleParsingTextBox.TabIndex = 45
        '
        'LombardNoiseLevel_DoubleParsingTextBox
        '
        Me.LombardNoiseLevel_DoubleParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LombardNoiseLevel_DoubleParsingTextBox.ForeColor = System.Drawing.Color.Red
        Me.LombardNoiseLevel_DoubleParsingTextBox.Location = New System.Drawing.Point(303, 526)
        Me.LombardNoiseLevel_DoubleParsingTextBox.Name = "LombardNoiseLevel_DoubleParsingTextBox"
        Me.LombardNoiseLevel_DoubleParsingTextBox.Size = New System.Drawing.Size(129, 20)
        Me.LombardNoiseLevel_DoubleParsingTextBox.TabIndex = 48
        '
        'Label26
        '
        Me.Label26.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label26.Location = New System.Drawing.Point(3, 123)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(294, 25)
        Me.Label26.TabIndex = 50
        Me.Label26.Text = "Linguistic level of sound files"
        Me.Label26.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'SoundFileLevelComboBox
        '
        Me.SoundFileLevelComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SoundFileLevelComboBox.FormattingEnabled = True
        Me.SoundFileLevelComboBox.Location = New System.Drawing.Point(303, 126)
        Me.SoundFileLevelComboBox.Name = "SoundFileLevelComboBox"
        Me.SoundFileLevelComboBox.Size = New System.Drawing.Size(129, 21)
        Me.SoundFileLevelComboBox.TabIndex = 51
        '
        'MediaSetSetupControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.TableLayoutPanel2)
        Me.Name = "MediaSetSetupControl"
        Me.Size = New System.Drawing.Size(841, 815)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.TableLayoutPanel2.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.PerformLayout()
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.Edit_TableLayoutPanel.ResumeLayout(False)
        Me.Edit_TableLayoutPanel.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TableLayoutPanel2 As Windows.Forms.TableLayoutPanel
    Friend WithEvents Edit_TableLayoutPanel As Windows.Forms.TableLayoutPanel
    Friend WithEvents TalkerName_TextBox As Windows.Forms.TextBox
    Friend WithEvents WaveFileEncoding_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents Label2 As Windows.Forms.Label
    Friend WithEvents Label3 As Windows.Forms.Label
    Friend WithEvents Label4 As Windows.Forms.Label
    Friend WithEvents Label13 As Windows.Forms.Label
    Friend WithEvents Label12 As Windows.Forms.Label
    Friend WithEvents Label11 As Windows.Forms.Label
    Friend WithEvents Label10 As Windows.Forms.Label
    Friend WithEvents Label9 As Windows.Forms.Label
    Friend WithEvents Label5 As Windows.Forms.Label
    Friend WithEvents Label6 As Windows.Forms.Label
    Friend WithEvents Label7 As Windows.Forms.Label
    Friend WithEvents Label14 As Windows.Forms.Label
    Friend WithEvents Label15 As Windows.Forms.Label
    Friend WithEvents Label16 As Windows.Forms.Label
    Friend WithEvents Label17 As Windows.Forms.Label
    Friend WithEvents Label18 As Windows.Forms.Label
    Friend WithEvents Label19 As Windows.Forms.Label
    Friend WithEvents Label20 As Windows.Forms.Label
    Friend WithEvents Label21 As Windows.Forms.Label
    Friend WithEvents TalkerAge_IntegerParsingTextBox As IntegerParsingTextBox
    Friend WithEvents MediaAudioItems_IntegerParsingTextBox As IntegerParsingTextBox
    Friend WithEvents MaskerAudioItems_IntegerParsingTextBox As IntegerParsingTextBox
    Friend WithEvents MediaImageItems_IntegerParsingTextBox As IntegerParsingTextBox
    Friend WithEvents MaskerImageItems_IntegerParsingTextBox As IntegerParsingTextBox
    Friend WithEvents WaveFileSampleRate_IntegerParsingTextBox As IntegerParsingTextBox
    Friend WithEvents MediaSetName_TextBox As Windows.Forms.TextBox
    Friend WithEvents TalkerDialect_TextBox As Windows.Forms.TextBox
    Friend WithEvents VoiceType_TextBox As Windows.Forms.TextBox
    Friend WithEvents MediaParentFolder_TextBox As Windows.Forms.TextBox
    Friend WithEvents MaskerParentFolder_TextBox As Windows.Forms.TextBox
    Friend WithEvents BackgroundNonspeechParentFolder_TextBox As Windows.Forms.TextBox
    Friend WithEvents BackgroundSpeechParentFolder_TextBox As Windows.Forms.TextBox
    Friend WithEvents PrototypeMediaParentFolder_TextBox As Windows.Forms.TextBox
    Friend WithEvents MasterPrototypeRecordingPath_TextBox As Windows.Forms.TextBox
    Friend WithEvents TalkerGender_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents WaveFileBitDepth_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents Label8 As Windows.Forms.Label
    Friend WithEvents BackgroundNonspeechRealisticLevel_DoubleParsingTextBox As DoubleParsingTextBox
    Friend WithEvents Label23 As Windows.Forms.Label
    Friend WithEvents PrototypeRecordingLevel_DoubleParsingTextBox As DoubleParsingTextBox
    Friend WithEvents Label22 As Windows.Forms.Label
    Friend WithEvents Label24 As Windows.Forms.Label
    Friend WithEvents LombardNoiseLevel_DoubleParsingTextBox As DoubleParsingTextBox
    Friend WithEvents LombardNoisePath_TextBox As Windows.Forms.TextBox
    Friend WithEvents LoadOstaMediaSetControl1 As LoadOstaMediaSetControl
    Friend WithEvents SaveMediaSetSpecification_Button As Windows.Forms.Button
    Friend WithEvents LoadOstaTestSpecificationControl1 As LoadOstaTestSpecificationControl
    Friend WithEvents Label25 As Windows.Forms.Label
    Friend WithEvents LoadedSpeechMaterialName_TextBox As Windows.Forms.TextBox
    Friend WithEvents NewMediaSet_Button As Windows.Forms.Button
    Friend WithEvents Splitter1 As Windows.Forms.Splitter
    Friend WithEvents Splitter2 As Windows.Forms.Splitter
    Friend WithEvents SplitContainer1 As Windows.Forms.SplitContainer
    Friend WithEvents GroupBox1 As Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel1 As Windows.Forms.TableLayoutPanel
    Friend WithEvents LaunchRecorder_Button As Windows.Forms.Button
    Friend WithEvents RandomOrder_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents SpecificPrototypeRecording_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents NoPrototypeRecording_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents MasterPrototypeRecording_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents Label26 As Windows.Forms.Label
    Friend WithEvents SoundFileLevelComboBox As Windows.Forms.ComboBox
End Class
