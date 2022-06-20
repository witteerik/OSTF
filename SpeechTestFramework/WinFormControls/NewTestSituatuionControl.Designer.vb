<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EditTestSituationControl
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
        Me.Edit_TableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.TalkerName_TextBox = New System.Windows.Forms.TextBox()
        Me.WaveFileEncoding_ComboBox = New System.Windows.Forms.ComboBox()
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
        Me.TalkerAge_IntegerParsingTextBox = New SpeechTestFramework.IntegerParsingTextBox()
        Me.MediaAudioItems_IntegerParsingTextBox = New SpeechTestFramework.IntegerParsingTextBox()
        Me.MaskerAudioItems_IntegerParsingTextBox = New SpeechTestFramework.IntegerParsingTextBox()
        Me.MediaImageItems_IntegerParsingTextBox = New SpeechTestFramework.IntegerParsingTextBox()
        Me.MaskerImageItems_IntegerParsingTextBox = New SpeechTestFramework.IntegerParsingTextBox()
        Me.WaveFileSampleRate_IntegerParsingTextBox = New SpeechTestFramework.IntegerParsingTextBox()
        Me.TestSituationName_TextBox = New System.Windows.Forms.TextBox()
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
        Me.BackgroundNonspeechRealisticLevel_DoubleParsingTextBox = New SpeechTestFramework.DoubleParsingTextBox()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.PrototypeRecordingLevel_DoubleParsingTextBox = New SpeechTestFramework.DoubleParsingTextBox()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.LombardNoiseLevel_DoubleParsingTextBox = New SpeechTestFramework.DoubleParsingTextBox()
        Me.LombardNoisePath_TextBox = New System.Windows.Forms.TextBox()
        Me.Save_Button = New System.Windows.Forms.Button()
        Me.NewTestSituation_Button = New System.Windows.Forms.Button()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.LoadOstaTestSituationsControl1 = New SpeechTestFramework.LoadOstaTestSituationsControl()
        Me.Edit_TableLayoutPanel.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'Edit_TableLayoutPanel
        '
        Me.Edit_TableLayoutPanel.AutoScroll = True
        Me.Edit_TableLayoutPanel.AutoSize = True
        Me.Edit_TableLayoutPanel.ColumnCount = 3
        Me.TableLayoutPanel2.SetColumnSpan(Me.Edit_TableLayoutPanel, 2)
        Me.Edit_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.Edit_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.Edit_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 91.0!))
        Me.Edit_TableLayoutPanel.Controls.Add(Me.TalkerName_TextBox, 1, 2)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.WaveFileEncoding_ComboBox, 1, 23)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label1, 0, 0)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label2, 0, 1)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label3, 0, 2)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label4, 0, 3)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label13, 0, 11)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label12, 0, 10)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label11, 0, 9)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label10, 0, 8)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label9, 0, 7)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label5, 0, 4)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label6, 0, 5)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label7, 0, 6)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label14, 0, 12)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label15, 0, 13)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label16, 0, 15)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label17, 0, 16)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label18, 0, 17)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label19, 0, 21)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label20, 0, 22)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label21, 0, 23)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.TalkerAge_IntegerParsingTextBox, 1, 4)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.MediaAudioItems_IntegerParsingTextBox, 1, 7)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.MaskerAudioItems_IntegerParsingTextBox, 1, 8)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.MediaImageItems_IntegerParsingTextBox, 1, 9)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.MaskerImageItems_IntegerParsingTextBox, 1, 10)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.WaveFileSampleRate_IntegerParsingTextBox, 1, 21)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.TestSituationName_TextBox, 1, 1)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.TalkerDialect_TextBox, 1, 5)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.VoiceType_TextBox, 1, 6)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.MediaParentFolder_TextBox, 1, 11)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.MaskerParentFolder_TextBox, 1, 12)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.BackgroundNonspeechParentFolder_TextBox, 1, 13)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.BackgroundSpeechParentFolder_TextBox, 1, 15)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.PrototypeMediaParentFolder_TextBox, 1, 16)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.MasterPrototypeRecordingPath_TextBox, 1, 17)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.TalkerGender_ComboBox, 1, 3)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.WaveFileBitDepth_ComboBox, 1, 22)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label8, 0, 14)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.BackgroundNonspeechRealisticLevel_DoubleParsingTextBox, 1, 14)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label23, 0, 18)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.PrototypeRecordingLevel_DoubleParsingTextBox, 1, 18)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label22, 0, 19)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.Label24, 0, 20)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.LombardNoiseLevel_DoubleParsingTextBox, 1, 20)
        Me.Edit_TableLayoutPanel.Controls.Add(Me.LombardNoisePath_TextBox, 1, 19)
        Me.Edit_TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Edit_TableLayoutPanel.Location = New System.Drawing.Point(3, 63)
        Me.Edit_TableLayoutPanel.Name = "Edit_TableLayoutPanel"
        Me.Edit_TableLayoutPanel.RowCount = 24
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
        Me.Edit_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.Edit_TableLayoutPanel.Size = New System.Drawing.Size(631, 591)
        Me.Edit_TableLayoutPanel.TabIndex = 0
        '
        'TalkerName_TextBox
        '
        Me.Edit_TableLayoutPanel.SetColumnSpan(Me.TalkerName_TextBox, 2)
        Me.TalkerName_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TalkerName_TextBox.Location = New System.Drawing.Point(273, 51)
        Me.TalkerName_TextBox.Name = "TalkerName_TextBox"
        Me.TalkerName_TextBox.Size = New System.Drawing.Size(355, 20)
        Me.TalkerName_TextBox.TabIndex = 42
        '
        'WaveFileEncoding_ComboBox
        '
        Me.Edit_TableLayoutPanel.SetColumnSpan(Me.WaveFileEncoding_ComboBox, 2)
        Me.WaveFileEncoding_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WaveFileEncoding_ComboBox.FormattingEnabled = True
        Me.WaveFileEncoding_ComboBox.Location = New System.Drawing.Point(273, 576)
        Me.WaveFileEncoding_ComboBox.Name = "WaveFileEncoding_ComboBox"
        Me.WaveFileEncoding_ComboBox.Size = New System.Drawing.Size(355, 21)
        Me.WaveFileEncoding_ComboBox.TabIndex = 41
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Edit_TableLayoutPanel.SetColumnSpan(Me.Label1, 3)
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Location = New System.Drawing.Point(3, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(625, 23)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Edit the fields below to modify the test situation, and press 'Save changes' to s" &
    "ave your changes"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label2
        '
        Me.Label2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label2.Location = New System.Drawing.Point(3, 23)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(264, 25)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Test situation name"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label3
        '
        Me.Label3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label3.Location = New System.Drawing.Point(3, 48)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(264, 25)
        Me.Label3.TabIndex = 3
        Me.Label3.Text = "Talker name"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label4
        '
        Me.Label4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label4.Location = New System.Drawing.Point(3, 73)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(264, 25)
        Me.Label4.TabIndex = 4
        Me.Label4.Text = "Talker gender"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label13
        '
        Me.Label13.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label13.Location = New System.Drawing.Point(3, 273)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(264, 25)
        Me.Label13.TabIndex = 13
        Me.Label13.Text = "Subfolder containing target files"
        Me.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label12
        '
        Me.Label12.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label12.Location = New System.Drawing.Point(3, 248)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(264, 25)
        Me.Label12.TabIndex = 12
        Me.Label12.Text = "Number of duplicate image maskers"
        Me.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label11
        '
        Me.Label11.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label11.Location = New System.Drawing.Point(3, 223)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(264, 25)
        Me.Label11.TabIndex = 11
        Me.Label11.Text = "Number of duplicate image targets"
        Me.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label10
        '
        Me.Label10.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label10.Location = New System.Drawing.Point(3, 198)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(264, 25)
        Me.Label10.TabIndex = 10
        Me.Label10.Text = "Number of duplicate audio maskers"
        Me.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label9
        '
        Me.Label9.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label9.Location = New System.Drawing.Point(3, 173)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(264, 25)
        Me.Label9.TabIndex = 9
        Me.Label9.Text = "Number of duplicate audio targets"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label5
        '
        Me.Label5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label5.Location = New System.Drawing.Point(3, 98)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(264, 25)
        Me.Label5.TabIndex = 5
        Me.Label5.Text = "Talker age (years)"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label6
        '
        Me.Label6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label6.Location = New System.Drawing.Point(3, 123)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(264, 25)
        Me.Label6.TabIndex = 6
        Me.Label6.Text = "Talker dialect"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label7
        '
        Me.Label7.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label7.Location = New System.Drawing.Point(3, 148)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(264, 25)
        Me.Label7.TabIndex = 7
        Me.Label7.Text = "Voice type"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label14
        '
        Me.Label14.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label14.Location = New System.Drawing.Point(3, 298)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(264, 25)
        Me.Label14.TabIndex = 14
        Me.Label14.Text = "Subfolder containing masker files"
        Me.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label15
        '
        Me.Label15.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label15.Location = New System.Drawing.Point(3, 323)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(264, 25)
        Me.Label15.TabIndex = 15
        Me.Label15.Text = "Subfolder containing background non-speech files"
        Me.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label16
        '
        Me.Label16.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label16.Location = New System.Drawing.Point(3, 373)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(264, 25)
        Me.Label16.TabIndex = 16
        Me.Label16.Text = "Subfolder containing background speech files"
        Me.Label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label17
        '
        Me.Label17.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label17.Location = New System.Drawing.Point(3, 398)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(264, 25)
        Me.Label17.TabIndex = 17
        Me.Label17.Text = "Subfolder containing target prototype recordings"
        Me.Label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label18
        '
        Me.Label18.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label18.Location = New System.Drawing.Point(3, 423)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(264, 25)
        Me.Label18.TabIndex = 18
        Me.Label18.Text = "Subpath to the master prototype recording"
        Me.Label18.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label19.Location = New System.Drawing.Point(3, 523)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(264, 25)
        Me.Label19.TabIndex = 19
        Me.Label19.Text = "Wave file sample rate"
        Me.Label19.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label20
        '
        Me.Label20.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label20.Location = New System.Drawing.Point(3, 548)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(264, 25)
        Me.Label20.TabIndex = 20
        Me.Label20.Text = "Wave file bit depth"
        Me.Label20.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label21
        '
        Me.Label21.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label21.Location = New System.Drawing.Point(3, 573)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(264, 25)
        Me.Label21.TabIndex = 21
        Me.Label21.Text = "Wave file encoding"
        Me.Label21.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TalkerAge_IntegerParsingTextBox
        '
        Me.Edit_TableLayoutPanel.SetColumnSpan(Me.TalkerAge_IntegerParsingTextBox, 2)
        Me.TalkerAge_IntegerParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TalkerAge_IntegerParsingTextBox.ForeColor = System.Drawing.SystemColors.WindowText
        Me.TalkerAge_IntegerParsingTextBox.Location = New System.Drawing.Point(273, 101)
        Me.TalkerAge_IntegerParsingTextBox.Name = "TalkerAge_IntegerParsingTextBox"
        Me.TalkerAge_IntegerParsingTextBox.Size = New System.Drawing.Size(355, 20)
        Me.TalkerAge_IntegerParsingTextBox.TabIndex = 22
        Me.TalkerAge_IntegerParsingTextBox.Text = "-1"
        '
        'MediaAudioItems_IntegerParsingTextBox
        '
        Me.Edit_TableLayoutPanel.SetColumnSpan(Me.MediaAudioItems_IntegerParsingTextBox, 2)
        Me.MediaAudioItems_IntegerParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MediaAudioItems_IntegerParsingTextBox.ForeColor = System.Drawing.SystemColors.WindowText
        Me.MediaAudioItems_IntegerParsingTextBox.Location = New System.Drawing.Point(273, 176)
        Me.MediaAudioItems_IntegerParsingTextBox.Name = "MediaAudioItems_IntegerParsingTextBox"
        Me.MediaAudioItems_IntegerParsingTextBox.Size = New System.Drawing.Size(355, 20)
        Me.MediaAudioItems_IntegerParsingTextBox.TabIndex = 24
        Me.MediaAudioItems_IntegerParsingTextBox.Text = "5"
        '
        'MaskerAudioItems_IntegerParsingTextBox
        '
        Me.Edit_TableLayoutPanel.SetColumnSpan(Me.MaskerAudioItems_IntegerParsingTextBox, 2)
        Me.MaskerAudioItems_IntegerParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MaskerAudioItems_IntegerParsingTextBox.ForeColor = System.Drawing.SystemColors.WindowText
        Me.MaskerAudioItems_IntegerParsingTextBox.Location = New System.Drawing.Point(273, 201)
        Me.MaskerAudioItems_IntegerParsingTextBox.Name = "MaskerAudioItems_IntegerParsingTextBox"
        Me.MaskerAudioItems_IntegerParsingTextBox.Size = New System.Drawing.Size(355, 20)
        Me.MaskerAudioItems_IntegerParsingTextBox.TabIndex = 25
        Me.MaskerAudioItems_IntegerParsingTextBox.Text = "5"
        '
        'MediaImageItems_IntegerParsingTextBox
        '
        Me.Edit_TableLayoutPanel.SetColumnSpan(Me.MediaImageItems_IntegerParsingTextBox, 2)
        Me.MediaImageItems_IntegerParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MediaImageItems_IntegerParsingTextBox.ForeColor = System.Drawing.SystemColors.WindowText
        Me.MediaImageItems_IntegerParsingTextBox.Location = New System.Drawing.Point(273, 226)
        Me.MediaImageItems_IntegerParsingTextBox.Name = "MediaImageItems_IntegerParsingTextBox"
        Me.MediaImageItems_IntegerParsingTextBox.Size = New System.Drawing.Size(355, 20)
        Me.MediaImageItems_IntegerParsingTextBox.TabIndex = 26
        Me.MediaImageItems_IntegerParsingTextBox.Text = "0"
        '
        'MaskerImageItems_IntegerParsingTextBox
        '
        Me.Edit_TableLayoutPanel.SetColumnSpan(Me.MaskerImageItems_IntegerParsingTextBox, 2)
        Me.MaskerImageItems_IntegerParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MaskerImageItems_IntegerParsingTextBox.ForeColor = System.Drawing.SystemColors.WindowText
        Me.MaskerImageItems_IntegerParsingTextBox.Location = New System.Drawing.Point(273, 251)
        Me.MaskerImageItems_IntegerParsingTextBox.Name = "MaskerImageItems_IntegerParsingTextBox"
        Me.MaskerImageItems_IntegerParsingTextBox.Size = New System.Drawing.Size(355, 20)
        Me.MaskerImageItems_IntegerParsingTextBox.TabIndex = 27
        Me.MaskerImageItems_IntegerParsingTextBox.Text = "0"
        '
        'WaveFileSampleRate_IntegerParsingTextBox
        '
        Me.Edit_TableLayoutPanel.SetColumnSpan(Me.WaveFileSampleRate_IntegerParsingTextBox, 2)
        Me.WaveFileSampleRate_IntegerParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WaveFileSampleRate_IntegerParsingTextBox.ForeColor = System.Drawing.Color.Red
        Me.WaveFileSampleRate_IntegerParsingTextBox.Location = New System.Drawing.Point(273, 526)
        Me.WaveFileSampleRate_IntegerParsingTextBox.Name = "WaveFileSampleRate_IntegerParsingTextBox"
        Me.WaveFileSampleRate_IntegerParsingTextBox.Size = New System.Drawing.Size(355, 20)
        Me.WaveFileSampleRate_IntegerParsingTextBox.TabIndex = 28
        '
        'TestSituationName_TextBox
        '
        Me.Edit_TableLayoutPanel.SetColumnSpan(Me.TestSituationName_TextBox, 2)
        Me.TestSituationName_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TestSituationName_TextBox.Location = New System.Drawing.Point(273, 26)
        Me.TestSituationName_TextBox.Name = "TestSituationName_TextBox"
        Me.TestSituationName_TextBox.Size = New System.Drawing.Size(355, 20)
        Me.TestSituationName_TextBox.TabIndex = 29
        '
        'TalkerDialect_TextBox
        '
        Me.Edit_TableLayoutPanel.SetColumnSpan(Me.TalkerDialect_TextBox, 2)
        Me.TalkerDialect_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TalkerDialect_TextBox.Location = New System.Drawing.Point(273, 126)
        Me.TalkerDialect_TextBox.Name = "TalkerDialect_TextBox"
        Me.TalkerDialect_TextBox.Size = New System.Drawing.Size(355, 20)
        Me.TalkerDialect_TextBox.TabIndex = 31
        '
        'VoiceType_TextBox
        '
        Me.Edit_TableLayoutPanel.SetColumnSpan(Me.VoiceType_TextBox, 2)
        Me.VoiceType_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.VoiceType_TextBox.Location = New System.Drawing.Point(273, 151)
        Me.VoiceType_TextBox.Name = "VoiceType_TextBox"
        Me.VoiceType_TextBox.Size = New System.Drawing.Size(355, 20)
        Me.VoiceType_TextBox.TabIndex = 32
        '
        'MediaParentFolder_TextBox
        '
        Me.Edit_TableLayoutPanel.SetColumnSpan(Me.MediaParentFolder_TextBox, 2)
        Me.MediaParentFolder_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MediaParentFolder_TextBox.Location = New System.Drawing.Point(273, 276)
        Me.MediaParentFolder_TextBox.Name = "MediaParentFolder_TextBox"
        Me.MediaParentFolder_TextBox.Size = New System.Drawing.Size(355, 20)
        Me.MediaParentFolder_TextBox.TabIndex = 33
        Me.MediaParentFolder_TextBox.Text = "Media\Unechoic-Talker1-RVE\TestWordRecordings"
        '
        'MaskerParentFolder_TextBox
        '
        Me.Edit_TableLayoutPanel.SetColumnSpan(Me.MaskerParentFolder_TextBox, 2)
        Me.MaskerParentFolder_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MaskerParentFolder_TextBox.Location = New System.Drawing.Point(273, 301)
        Me.MaskerParentFolder_TextBox.Name = "MaskerParentFolder_TextBox"
        Me.MaskerParentFolder_TextBox.Size = New System.Drawing.Size(355, 20)
        Me.MaskerParentFolder_TextBox.TabIndex = 34
        '
        'BackgroundNonspeechParentFolder_TextBox
        '
        Me.Edit_TableLayoutPanel.SetColumnSpan(Me.BackgroundNonspeechParentFolder_TextBox, 2)
        Me.BackgroundNonspeechParentFolder_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BackgroundNonspeechParentFolder_TextBox.Location = New System.Drawing.Point(273, 326)
        Me.BackgroundNonspeechParentFolder_TextBox.Name = "BackgroundNonspeechParentFolder_TextBox"
        Me.BackgroundNonspeechParentFolder_TextBox.Size = New System.Drawing.Size(355, 20)
        Me.BackgroundNonspeechParentFolder_TextBox.TabIndex = 35
        '
        'BackgroundSpeechParentFolder_TextBox
        '
        Me.Edit_TableLayoutPanel.SetColumnSpan(Me.BackgroundSpeechParentFolder_TextBox, 2)
        Me.BackgroundSpeechParentFolder_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BackgroundSpeechParentFolder_TextBox.Location = New System.Drawing.Point(273, 376)
        Me.BackgroundSpeechParentFolder_TextBox.Name = "BackgroundSpeechParentFolder_TextBox"
        Me.BackgroundSpeechParentFolder_TextBox.Size = New System.Drawing.Size(355, 20)
        Me.BackgroundSpeechParentFolder_TextBox.TabIndex = 36
        '
        'PrototypeMediaParentFolder_TextBox
        '
        Me.Edit_TableLayoutPanel.SetColumnSpan(Me.PrototypeMediaParentFolder_TextBox, 2)
        Me.PrototypeMediaParentFolder_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PrototypeMediaParentFolder_TextBox.Location = New System.Drawing.Point(273, 401)
        Me.PrototypeMediaParentFolder_TextBox.Name = "PrototypeMediaParentFolder_TextBox"
        Me.PrototypeMediaParentFolder_TextBox.Size = New System.Drawing.Size(355, 20)
        Me.PrototypeMediaParentFolder_TextBox.TabIndex = 37
        '
        'MasterPrototypeRecordingPath_TextBox
        '
        Me.Edit_TableLayoutPanel.SetColumnSpan(Me.MasterPrototypeRecordingPath_TextBox, 2)
        Me.MasterPrototypeRecordingPath_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MasterPrototypeRecordingPath_TextBox.Location = New System.Drawing.Point(273, 426)
        Me.MasterPrototypeRecordingPath_TextBox.Name = "MasterPrototypeRecordingPath_TextBox"
        Me.MasterPrototypeRecordingPath_TextBox.Size = New System.Drawing.Size(355, 20)
        Me.MasterPrototypeRecordingPath_TextBox.TabIndex = 38
        '
        'TalkerGender_ComboBox
        '
        Me.Edit_TableLayoutPanel.SetColumnSpan(Me.TalkerGender_ComboBox, 2)
        Me.TalkerGender_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TalkerGender_ComboBox.FormattingEnabled = True
        Me.TalkerGender_ComboBox.Location = New System.Drawing.Point(273, 76)
        Me.TalkerGender_ComboBox.Name = "TalkerGender_ComboBox"
        Me.TalkerGender_ComboBox.Size = New System.Drawing.Size(355, 21)
        Me.TalkerGender_ComboBox.TabIndex = 39
        '
        'WaveFileBitDepth_ComboBox
        '
        Me.Edit_TableLayoutPanel.SetColumnSpan(Me.WaveFileBitDepth_ComboBox, 2)
        Me.WaveFileBitDepth_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WaveFileBitDepth_ComboBox.FormattingEnabled = True
        Me.WaveFileBitDepth_ComboBox.Location = New System.Drawing.Point(273, 551)
        Me.WaveFileBitDepth_ComboBox.Name = "WaveFileBitDepth_ComboBox"
        Me.WaveFileBitDepth_ComboBox.Size = New System.Drawing.Size(355, 21)
        Me.WaveFileBitDepth_ComboBox.TabIndex = 40
        '
        'Label8
        '
        Me.Label8.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label8.Location = New System.Drawing.Point(3, 348)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(264, 25)
        Me.Label8.TabIndex = 8
        Me.Label8.Text = "Background (non-speech) realistic sound level (dB SPL)"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BackgroundNonspeechRealisticLevel_DoubleParsingTextBox
        '
        Me.Edit_TableLayoutPanel.SetColumnSpan(Me.BackgroundNonspeechRealisticLevel_DoubleParsingTextBox, 2)
        Me.BackgroundNonspeechRealisticLevel_DoubleParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BackgroundNonspeechRealisticLevel_DoubleParsingTextBox.ForeColor = System.Drawing.Color.Red
        Me.BackgroundNonspeechRealisticLevel_DoubleParsingTextBox.Location = New System.Drawing.Point(273, 351)
        Me.BackgroundNonspeechRealisticLevel_DoubleParsingTextBox.Name = "BackgroundNonspeechRealisticLevel_DoubleParsingTextBox"
        Me.BackgroundNonspeechRealisticLevel_DoubleParsingTextBox.Size = New System.Drawing.Size(355, 20)
        Me.BackgroundNonspeechRealisticLevel_DoubleParsingTextBox.TabIndex = 23
        '
        'Label23
        '
        Me.Label23.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label23.Location = New System.Drawing.Point(3, 448)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(264, 25)
        Me.Label23.TabIndex = 44
        Me.Label23.Text = "Prototype recording level (dBC, 50 ms)"
        Me.Label23.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'PrototypeRecordingLevel_DoubleParsingTextBox
        '
        Me.Edit_TableLayoutPanel.SetColumnSpan(Me.PrototypeRecordingLevel_DoubleParsingTextBox, 2)
        Me.PrototypeRecordingLevel_DoubleParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PrototypeRecordingLevel_DoubleParsingTextBox.ForeColor = System.Drawing.SystemColors.WindowText
        Me.PrototypeRecordingLevel_DoubleParsingTextBox.Location = New System.Drawing.Point(273, 451)
        Me.PrototypeRecordingLevel_DoubleParsingTextBox.Name = "PrototypeRecordingLevel_DoubleParsingTextBox"
        Me.PrototypeRecordingLevel_DoubleParsingTextBox.Size = New System.Drawing.Size(355, 20)
        Me.PrototypeRecordingLevel_DoubleParsingTextBox.TabIndex = 45
        Me.PrototypeRecordingLevel_DoubleParsingTextBox.Text = "65"
        '
        'Label22
        '
        Me.Label22.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label22.Location = New System.Drawing.Point(3, 473)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(264, 25)
        Me.Label22.TabIndex = 46
        Me.Label22.Text = "Subpath to 'Lombard' (recording) noise"
        Me.Label22.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label24
        '
        Me.Label24.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label24.Location = New System.Drawing.Point(3, 498)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(264, 25)
        Me.Label24.TabIndex = 47
        Me.Label24.Text = "'Lombard' (recording) noise level (dBC, 50 ms)"
        Me.Label24.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LombardNoiseLevel_DoubleParsingTextBox
        '
        Me.Edit_TableLayoutPanel.SetColumnSpan(Me.LombardNoiseLevel_DoubleParsingTextBox, 2)
        Me.LombardNoiseLevel_DoubleParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LombardNoiseLevel_DoubleParsingTextBox.ForeColor = System.Drawing.SystemColors.WindowText
        Me.LombardNoiseLevel_DoubleParsingTextBox.Location = New System.Drawing.Point(273, 501)
        Me.LombardNoiseLevel_DoubleParsingTextBox.Name = "LombardNoiseLevel_DoubleParsingTextBox"
        Me.LombardNoiseLevel_DoubleParsingTextBox.Size = New System.Drawing.Size(355, 20)
        Me.LombardNoiseLevel_DoubleParsingTextBox.TabIndex = 48
        Me.LombardNoiseLevel_DoubleParsingTextBox.Text = "65"
        '
        'LombardNoisePath_TextBox
        '
        Me.Edit_TableLayoutPanel.SetColumnSpan(Me.LombardNoisePath_TextBox, 2)
        Me.LombardNoisePath_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LombardNoisePath_TextBox.Location = New System.Drawing.Point(273, 476)
        Me.LombardNoisePath_TextBox.Name = "LombardNoisePath_TextBox"
        Me.LombardNoisePath_TextBox.Size = New System.Drawing.Size(355, 20)
        Me.LombardNoisePath_TextBox.TabIndex = 49
        '
        'Save_Button
        '
        Me.TableLayoutPanel2.SetColumnSpan(Me.Save_Button, 2)
        Me.Save_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Save_Button.Location = New System.Drawing.Point(3, 660)
        Me.Save_Button.Name = "Save_Button"
        Me.Save_Button.Size = New System.Drawing.Size(631, 24)
        Me.Save_Button.TabIndex = 0
        Me.Save_Button.Text = "Save changes"
        Me.Save_Button.UseVisualStyleBackColor = True
        '
        'NewTestSituation_Button
        '
        Me.NewTestSituation_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.NewTestSituation_Button.Location = New System.Drawing.Point(556, 3)
        Me.NewTestSituation_Button.Name = "NewTestSituation_Button"
        Me.NewTestSituation_Button.Size = New System.Drawing.Size(78, 54)
        Me.NewTestSituation_Button.TabIndex = 50
        Me.NewTestSituation_Button.Text = "Create new test situation"
        Me.NewTestSituation_Button.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.ColumnCount = 2
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 84.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.Edit_TableLayoutPanel, 0, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.LoadOstaTestSituationsControl1, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.NewTestSituation_Button, 1, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.Save_Button, 0, 2)
        Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 3
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(637, 687)
        Me.TableLayoutPanel2.TabIndex = 52
        '
        'LoadOstaTestSituationsControl1
        '
        Me.LoadOstaTestSituationsControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LoadOstaTestSituationsControl1.Location = New System.Drawing.Point(3, 3)
        Me.LoadOstaTestSituationsControl1.Name = "LoadOstaTestSituationsControl1"
        Me.LoadOstaTestSituationsControl1.SelectedTestSituation = Nothing
        Me.LoadOstaTestSituationsControl1.SelectedTestSpecification = Nothing
        Me.LoadOstaTestSituationsControl1.Size = New System.Drawing.Size(547, 54)
        Me.LoadOstaTestSituationsControl1.TabIndex = 51
        '
        'EditTestSituationControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.Controls.Add(Me.TableLayoutPanel2)
        Me.Name = "EditTestSituationControl"
        Me.Size = New System.Drawing.Size(637, 687)
        Me.Edit_TableLayoutPanel.ResumeLayout(False)
        Me.Edit_TableLayoutPanel.PerformLayout()
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.TableLayoutPanel2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Edit_TableLayoutPanel As Windows.Forms.TableLayoutPanel
    Friend WithEvents Save_Button As Windows.Forms.Button
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents Label2 As Windows.Forms.Label
    Friend WithEvents Label3 As Windows.Forms.Label
    Friend WithEvents Label4 As Windows.Forms.Label
    Friend WithEvents Label13 As Windows.Forms.Label
    Friend WithEvents Label12 As Windows.Forms.Label
    Friend WithEvents Label11 As Windows.Forms.Label
    Friend WithEvents Label10 As Windows.Forms.Label
    Friend WithEvents Label9 As Windows.Forms.Label
    Friend WithEvents Label8 As Windows.Forms.Label
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
    Friend WithEvents WaveFileEncoding_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents TalkerAge_IntegerParsingTextBox As IntegerParsingTextBox
    Friend WithEvents BackgroundNonspeechRealisticLevel_DoubleParsingTextBox As DoubleParsingTextBox
    Friend WithEvents MediaAudioItems_IntegerParsingTextBox As IntegerParsingTextBox
    Friend WithEvents MaskerAudioItems_IntegerParsingTextBox As IntegerParsingTextBox
    Friend WithEvents MediaImageItems_IntegerParsingTextBox As IntegerParsingTextBox
    Friend WithEvents MaskerImageItems_IntegerParsingTextBox As IntegerParsingTextBox
    Friend WithEvents WaveFileSampleRate_IntegerParsingTextBox As IntegerParsingTextBox
    Friend WithEvents TestSituationName_TextBox As Windows.Forms.TextBox
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
    Friend WithEvents TalkerName_TextBox As Windows.Forms.TextBox
    Friend WithEvents Label23 As Windows.Forms.Label
    Friend WithEvents PrototypeRecordingLevel_DoubleParsingTextBox As DoubleParsingTextBox
    Friend WithEvents Label22 As Windows.Forms.Label
    Friend WithEvents Label24 As Windows.Forms.Label
    Friend WithEvents LombardNoiseLevel_DoubleParsingTextBox As DoubleParsingTextBox
    Friend WithEvents LombardNoisePath_TextBox As Windows.Forms.TextBox
    Friend WithEvents NewTestSituation_Button As Windows.Forms.Button
    Friend WithEvents LoadOstaTestSituationsControl1 As LoadOstaTestSituationsControl
    Friend WithEvents TableLayoutPanel2 As Windows.Forms.TableLayoutPanel
End Class
