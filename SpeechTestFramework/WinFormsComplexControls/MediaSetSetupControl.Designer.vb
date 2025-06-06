﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MediaSetSetupControl
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MediaSetSetupControl))
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.LoadOstaMediaSetControl1 = New SpeechTestFramework.LoadOstaMediaSetControl()
        Me.LoadOstaTestSpecificationControl1 = New SpeechTestFramework.LoadOstaTestSpecificationControl()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.LoadedSpeechMaterialName_TextBox = New System.Windows.Forms.TextBox()
        Me.Splitter1 = New System.Windows.Forms.Splitter()
        Me.Splitter2 = New System.Windows.Forms.Splitter()
        Me.NewMediaSet_Button = New System.Windows.Forms.Button()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.EditSpecification_TableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
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
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.MediaImageItems_IntegerParsingTextBox = New SpeechTestFramework.IntegerParsingTextBox()
        Me.MaskerImageItems_IntegerParsingTextBox = New SpeechTestFramework.IntegerParsingTextBox()
        Me.WaveFileSampleRate_IntegerParsingTextBox = New SpeechTestFramework.IntegerParsingTextBox()
        Me.MediaSetName_TextBox = New System.Windows.Forms.TextBox()
        Me.MediaParentFolder_TextBox = New System.Windows.Forms.TextBox()
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
        Me.Label26 = New System.Windows.Forms.Label()
        Me.AudioFileLinguisticLevelComboBox = New System.Windows.Forms.ComboBox()
        Me.SharedMaskersLevelComboBox = New System.Windows.Forms.ComboBox()
        Me.Label30 = New System.Windows.Forms.Label()
        Me.CustomVariablesFolder_TextBox = New System.Windows.Forms.TextBox()
        Me.Label31 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TalkerAge_IntegerParsingTextBox = New SpeechTestFramework.IntegerParsingTextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.TalkerDialect_TextBox = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.VoiceType_TextBox = New System.Windows.Forms.TextBox()
        Me.Label51 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.MediaAudioItems_IntegerParsingTextBox = New SpeechTestFramework.IntegerParsingTextBox()
        Me.MaskerAudioItems_IntegerParsingTextBox = New SpeechTestFramework.IntegerParsingTextBox()
        Me.Label52 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.MaskerParentFolder_TextBox = New System.Windows.Forms.TextBox()
        Me.Label53 = New System.Windows.Forms.Label()
        Me.Label54 = New System.Windows.Forms.Label()
        Me.SharedContralateralMaskersLevel_ComboBox = New System.Windows.Forms.ComboBox()
        Me.ContralateralMaskerAudioItems_IntegerParsingTextBox = New SpeechTestFramework.IntegerParsingTextBox()
        Me.ContralateralMaskerParentFolder_TextBox = New System.Windows.Forms.TextBox()
        Me.EffectiveContralateralMaskingGain_DoubleParsingTextBox = New SpeechTestFramework.DoubleParsingTextBox()
        Me.EditSoundFile_TabControl = New System.Windows.Forms.TabControl()
        Me.StartRecorder_TabPage = New System.Windows.Forms.TabPage()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.LaunchRecorder_Button = New System.Windows.Forms.Button()
        Me.RandomOrder_CheckBox = New System.Windows.Forms.CheckBox()
        Me.SpecificPrototypeRecording_RadioButton = New System.Windows.Forms.RadioButton()
        Me.NoPrototypeRecording_RadioButton = New System.Windows.Forms.RadioButton()
        Me.MasterPrototypeRecording_RadioButton = New System.Windows.Forms.RadioButton()
        Me.SpeechLevels_TabPage = New System.Windows.Forms.TabPage()
        Me.TableLayoutPanel5 = New System.Windows.Forms.TableLayoutPanel()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel3 = New System.Windows.Forms.TableLayoutPanel()
        Me.SpeechLevelFS_DoubleParsingTextBox = New SpeechTestFramework.DoubleParsingTextBox()
        Me.Label29 = New System.Windows.Forms.Label()
        Me.ApplySpeechLevels_Button = New System.Windows.Forms.Button()
        Me.SpeechLevelSPL_Label = New System.Windows.Forms.Label()
        Me.Label28 = New System.Windows.Forms.Label()
        Me.SpeechLevelTemporalIntegration_CheckBox = New System.Windows.Forms.CheckBox()
        Me.OnlyNominalLevel_CheckBox = New System.Windows.Forms.CheckBox()
        Me.VpNormalization_Checkbox = New System.Windows.Forms.CheckBox()
        Me.SpeechLevelSPL_DoubleParsingTextBox = New SpeechTestFramework.DoubleParsingTextBox()
        Me.SpeechLevelFrequencyWeighting_ComboBox = New System.Windows.Forms.ComboBox()
        Me.SpeechLevelTemporalIntegration_DoubleParsingTextBox = New SpeechTestFramework.DoubleParsingTextBox()
        Me.Label40 = New System.Windows.Forms.Label()
        Me.SpeechLevel_TargetLinguisticlevel_ComboBox = New System.Windows.Forms.ComboBox()
        Me.NominalLevel_CheckBox = New System.Windows.Forms.CheckBox()
        Me.CreateCalibrationSignal_CheckBox = New System.Windows.Forms.CheckBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel6 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.MeasureSmaLevels_Button = New System.Windows.Forms.Button()
        Me.SmaTemporalIntegration_CheckBox = New System.Windows.Forms.CheckBox()
        Me.SmaFrequencyWeighting_ComboBox = New System.Windows.Forms.ComboBox()
        Me.SmaTemporalIntegration_DoubleParsingTextBox = New SpeechTestFramework.DoubleParsingTextBox()
        Me.IncludeCriticalBandLevels_CheckBox = New System.Windows.Forms.CheckBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel7 = New System.Windows.Forms.TableLayoutPanel()
        Me.SipTestLevels_Button = New System.Windows.Forms.Button()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel9 = New System.Windows.Forms.TableLayoutPanel()
        Me.CustomSpeechGain_TextBox = New System.Windows.Forms.TextBox()
        Me.Label32 = New System.Windows.Forms.Label()
        Me.Label41 = New System.Windows.Forms.Label()
        Me.TextBox2 = New System.Windows.Forms.TextBox()
        Me.ApplyCustomSpeechGain_Button = New System.Windows.Forms.Button()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel10 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label42 = New System.Windows.Forms.Label()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.Label43 = New System.Windows.Forms.Label()
        Me.Label44 = New System.Windows.Forms.Label()
        Me.Label45 = New System.Windows.Forms.Label()
        Me.Label46 = New System.Windows.Forms.Label()
        Me.Label47 = New System.Windows.Forms.Label()
        Me.GenerateSnrRangeStimuli_UpperLimit_IntegerParsingTextBox = New SpeechTestFramework.IntegerParsingTextBox()
        Me.GenerateSnrRangeStimuli_LowerLimit_IntegerParsingTextBox = New SpeechTestFramework.IntegerParsingTextBox()
        Me.GenerateSnrRangeStimuli_StepSize_DoubleParsingTextBox = New SpeechTestFramework.DoubleParsingTextBox()
        Me.GenerateSnrRangeStimuli_PresentationLevel_DoubleParsingTextBox = New SpeechTestFramework.DoubleParsingTextBox()
        Me.GenerateSnrRangeStimuli_NoiseType_ComboBox = New System.Windows.Forms.ComboBox()
        Me.GenerateSnrRangeStimuli_Button = New System.Windows.Forms.Button()
        Me.GenerateSnrRangeStimuli_NoiseFW_ComboBox = New System.Windows.Forms.ComboBox()
        Me.Label48 = New System.Windows.Forms.Label()
        Me.Label49 = New System.Windows.Forms.Label()
        Me.GenerateSnrRangeStimuli_OutputGain_DoubleParsingTextBox = New SpeechTestFramework.DoubleParsingTextBox()
        Me.Label50 = New System.Windows.Forms.Label()
        Me.GenerateSnrRangeStimuli_Overlays_IntegerParsingTextBox = New SpeechTestFramework.IntegerParsingTextBox()
        Me.GenerateSnrRangeStimuli_ConcatenatedSound_CheckBox = New System.Windows.Forms.CheckBox()
        Me.GenerateSnrRangeStimuli_InsertConcatenationSilence_CheckBox = New System.Windows.Forms.CheckBox()
        Me.ListMixer_TabPage = New System.Windows.Forms.TabPage()
        Me.ListRearrangerControl1 = New SpeechTestFramework.ListRearrangerControl()
        Me.CreateMaskers_TabPage = New System.Windows.Forms.TabPage()
        Me.CreateMaskersControl1 = New SpeechTestFramework.CreateMaskersControl()
        Me.SoundfileLinguisticLevels_TabPage = New System.Windows.Forms.TabPage()
        Me.TableLayoutPanel8 = New System.Windows.Forms.TableLayoutPanel()
        Me.RandomizeOrder_CheckBox = New System.Windows.Forms.CheckBox()
        Me.NewSoundFile_InterStimulusInterval_IntegerParsingTextBox = New SpeechTestFramework.IntegerParsingTextBox()
        Me.Label36 = New System.Windows.Forms.Label()
        Me.RichTextBox1 = New System.Windows.Forms.RichTextBox()
        Me.Label33 = New System.Windows.Forms.Label()
        Me.Label34 = New System.Windows.Forms.Label()
        Me.Label35 = New System.Windows.Forms.Label()
        Me.ModifiedMediaSetName_TextBox = New System.Windows.Forms.TextBox()
        Me.CurrentMediaSetLinguisticLevel_TextBox = New System.Windows.Forms.TextBox()
        Me.ModifiedMediaSetLinguisticLevel_ComboBox = New System.Windows.Forms.ComboBox()
        Me.Label37 = New System.Windows.Forms.Label()
        Me.NewSoundFilePadding_IntegerParsingTextBox = New SpeechTestFramework.IntegerParsingTextBox()
        Me.CreateModifiedMediaSet_Button = New System.Windows.Forms.Button()
        Me.IncludePractiseItems_CheckBox = New System.Windows.Forms.CheckBox()
        Me.IncludeTestItems_CheckBox = New System.Windows.Forms.CheckBox()
        Me.Label38 = New System.Windows.Forms.Label()
        Me.CrossfadeDuration_IntegerParsingTextBox = New SpeechTestFramework.IntegerParsingTextBox()
        Me.Label39 = New System.Windows.Forms.Label()
        Me.RandomSeed_IntegerParsingTextBox = New SpeechTestFramework.IntegerParsingTextBox()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.TableLayoutPanel4 = New System.Windows.Forms.TableLayoutPanel()
        Me.CreateSipTestMaskersButton = New System.Windows.Forms.Button()
        Me.CalculateSipTestMaskerSpectrumLevels_Button = New System.Windows.Forms.Button()
        Me.TempButton = New System.Windows.Forms.Button()
        Me.ExportSpelledSoundFiles_Button = New System.Windows.Forms.Button()
        Me.TableLayoutPanel2.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.EditSpecification_TableLayoutPanel.SuspendLayout()
        Me.EditSoundFile_TabControl.SuspendLayout()
        Me.StartRecorder_TabPage.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SpeechLevels_TabPage.SuspendLayout()
        Me.TableLayoutPanel5.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.TableLayoutPanel3.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.TableLayoutPanel6.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.TableLayoutPanel7.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.TableLayoutPanel9.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.TableLayoutPanel10.SuspendLayout()
        Me.ListMixer_TabPage.SuspendLayout()
        Me.CreateMaskers_TabPage.SuspendLayout()
        Me.SoundfileLinguisticLevels_TabPage.SuspendLayout()
        Me.TableLayoutPanel8.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TableLayoutPanel4.SuspendLayout()
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
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(1314, 839)
        Me.TableLayoutPanel2.TabIndex = 53
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
        Me.LoadOstaMediaSetControl1.Size = New System.Drawing.Size(1158, 54)
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
        Me.LoadOstaTestSpecificationControl1.Size = New System.Drawing.Size(1308, 54)
        Me.LoadOstaTestSpecificationControl1.TabIndex = 52
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
        Me.LoadedSpeechMaterialName_TextBox.Size = New System.Drawing.Size(1158, 20)
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
        Me.Splitter1.Size = New System.Drawing.Size(1308, 2)
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
        Me.Splitter2.Size = New System.Drawing.Size(1308, 2)
        Me.Splitter2.TabIndex = 57
        Me.Splitter2.TabStop = False
        '
        'NewMediaSet_Button
        '
        Me.NewMediaSet_Button.Location = New System.Drawing.Point(1167, 101)
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
        Me.SplitContainer1.Panel1.Controls.Add(Me.EditSpecification_TableLayoutPanel)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.EditSoundFile_TabControl)
        Me.SplitContainer1.Size = New System.Drawing.Size(1308, 637)
        Me.SplitContainer1.SplitterDistance = 734
        Me.SplitContainer1.TabIndex = 58
        '
        'EditSpecification_TableLayoutPanel
        '
        Me.EditSpecification_TableLayoutPanel.AutoScroll = True
        Me.EditSpecification_TableLayoutPanel.AutoSize = True
        Me.EditSpecification_TableLayoutPanel.ColumnCount = 2
        Me.EditSpecification_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 300.0!))
        Me.EditSpecification_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.TalkerName_TextBox, 1, 2)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.WaveFileEncoding_ComboBox, 1, 30)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.SaveMediaSetSpecification_Button, 0, 31)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label1, 0, 0)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label2, 0, 1)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label3, 0, 2)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label4, 0, 3)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label13, 0, 16)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label12, 0, 14)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label11, 0, 13)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label15, 0, 20)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label16, 0, 22)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label17, 0, 23)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label18, 0, 24)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label19, 0, 28)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label20, 0, 29)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label21, 0, 30)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.MediaImageItems_IntegerParsingTextBox, 1, 13)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.MaskerImageItems_IntegerParsingTextBox, 1, 14)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.WaveFileSampleRate_IntegerParsingTextBox, 1, 28)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.MediaSetName_TextBox, 1, 1)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.MediaParentFolder_TextBox, 1, 16)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.BackgroundNonspeechParentFolder_TextBox, 1, 20)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.BackgroundSpeechParentFolder_TextBox, 1, 22)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.PrototypeMediaParentFolder_TextBox, 1, 23)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.MasterPrototypeRecordingPath_TextBox, 1, 24)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.TalkerGender_ComboBox, 1, 3)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.WaveFileBitDepth_ComboBox, 1, 29)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label8, 0, 21)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.BackgroundNonspeechRealisticLevel_DoubleParsingTextBox, 1, 21)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label23, 0, 25)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.PrototypeRecordingLevel_DoubleParsingTextBox, 1, 25)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label22, 0, 26)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label24, 0, 27)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.LombardNoiseLevel_DoubleParsingTextBox, 1, 27)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.LombardNoisePath_TextBox, 1, 26)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label26, 0, 7)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.AudioFileLinguisticLevelComboBox, 1, 7)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.SharedMaskersLevelComboBox, 1, 8)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label30, 0, 8)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.CustomVariablesFolder_TextBox, 1, 15)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label31, 0, 15)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label5, 0, 4)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.TalkerAge_IntegerParsingTextBox, 1, 4)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label6, 0, 5)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.TalkerDialect_TextBox, 1, 5)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label7, 0, 6)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.VoiceType_TextBox, 1, 6)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label51, 0, 9)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label9, 0, 10)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label10, 0, 11)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.MediaAudioItems_IntegerParsingTextBox, 1, 10)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.MaskerAudioItems_IntegerParsingTextBox, 1, 11)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label52, 0, 12)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label14, 0, 17)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.MaskerParentFolder_TextBox, 1, 17)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label53, 0, 18)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.Label54, 0, 19)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.SharedContralateralMaskersLevel_ComboBox, 1, 9)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.ContralateralMaskerAudioItems_IntegerParsingTextBox, 1, 12)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.ContralateralMaskerParentFolder_TextBox, 1, 18)
        Me.EditSpecification_TableLayoutPanel.Controls.Add(Me.EffectiveContralateralMaskingGain_DoubleParsingTextBox, 1, 19)
        Me.EditSpecification_TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.EditSpecification_TableLayoutPanel.Location = New System.Drawing.Point(0, 0)
        Me.EditSpecification_TableLayoutPanel.Name = "EditSpecification_TableLayoutPanel"
        Me.EditSpecification_TableLayoutPanel.RowCount = 32
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.EditSpecification_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
        Me.EditSpecification_TableLayoutPanel.Size = New System.Drawing.Size(734, 637)
        Me.EditSpecification_TableLayoutPanel.TabIndex = 0
        '
        'TalkerName_TextBox
        '
        Me.TalkerName_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TalkerName_TextBox.Location = New System.Drawing.Point(303, 51)
        Me.TalkerName_TextBox.Name = "TalkerName_TextBox"
        Me.TalkerName_TextBox.Size = New System.Drawing.Size(428, 20)
        Me.TalkerName_TextBox.TabIndex = 42
        '
        'WaveFileEncoding_ComboBox
        '
        Me.WaveFileEncoding_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WaveFileEncoding_ComboBox.FormattingEnabled = True
        Me.WaveFileEncoding_ComboBox.Location = New System.Drawing.Point(303, 751)
        Me.WaveFileEncoding_ComboBox.Name = "WaveFileEncoding_ComboBox"
        Me.WaveFileEncoding_ComboBox.Size = New System.Drawing.Size(428, 21)
        Me.WaveFileEncoding_ComboBox.TabIndex = 41
        '
        'SaveMediaSetSpecification_Button
        '
        Me.EditSpecification_TableLayoutPanel.SetColumnSpan(Me.SaveMediaSetSpecification_Button, 2)
        Me.SaveMediaSetSpecification_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SaveMediaSetSpecification_Button.Location = New System.Drawing.Point(3, 776)
        Me.SaveMediaSetSpecification_Button.Name = "SaveMediaSetSpecification_Button"
        Me.SaveMediaSetSpecification_Button.Size = New System.Drawing.Size(728, 34)
        Me.SaveMediaSetSpecification_Button.TabIndex = 0
        Me.SaveMediaSetSpecification_Button.Text = "Save changes"
        Me.SaveMediaSetSpecification_Button.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.EditSpecification_TableLayoutPanel.SetColumnSpan(Me.Label1, 2)
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Location = New System.Drawing.Point(3, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(728, 23)
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
        Me.Label13.Location = New System.Drawing.Point(3, 398)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(294, 25)
        Me.Label13.TabIndex = 13
        Me.Label13.Text = "Subfolder containing target files"
        Me.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label12
        '
        Me.Label12.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label12.Location = New System.Drawing.Point(3, 348)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(294, 25)
        Me.Label12.TabIndex = 12
        Me.Label12.Text = "Number of duplicate image maskers"
        Me.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label11
        '
        Me.Label11.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label11.Location = New System.Drawing.Point(3, 323)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(294, 25)
        Me.Label11.TabIndex = 11
        Me.Label11.Text = "Number of duplicate image targets"
        Me.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label15
        '
        Me.Label15.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label15.Location = New System.Drawing.Point(3, 498)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(294, 25)
        Me.Label15.TabIndex = 15
        Me.Label15.Text = "Subfolder containing background non-speech files"
        Me.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label16
        '
        Me.Label16.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label16.Location = New System.Drawing.Point(3, 548)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(294, 25)
        Me.Label16.TabIndex = 16
        Me.Label16.Text = "Subfolder containing background speech files"
        Me.Label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label17
        '
        Me.Label17.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label17.Location = New System.Drawing.Point(3, 573)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(294, 25)
        Me.Label17.TabIndex = 17
        Me.Label17.Text = "Subfolder containing target prototype recordings"
        Me.Label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label18
        '
        Me.Label18.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label18.Location = New System.Drawing.Point(3, 598)
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
        Me.Label19.Location = New System.Drawing.Point(3, 698)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(294, 25)
        Me.Label19.TabIndex = 19
        Me.Label19.Text = "Wave file sample rate"
        Me.Label19.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label20
        '
        Me.Label20.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label20.Location = New System.Drawing.Point(3, 723)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(294, 25)
        Me.Label20.TabIndex = 20
        Me.Label20.Text = "Wave file bit depth"
        Me.Label20.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label21
        '
        Me.Label21.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label21.Location = New System.Drawing.Point(3, 748)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(294, 25)
        Me.Label21.TabIndex = 21
        Me.Label21.Text = "Wave file encoding"
        Me.Label21.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'MediaImageItems_IntegerParsingTextBox
        '
        Me.MediaImageItems_IntegerParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MediaImageItems_IntegerParsingTextBox.ForeColor = System.Drawing.Color.Red
        Me.MediaImageItems_IntegerParsingTextBox.Location = New System.Drawing.Point(303, 326)
        Me.MediaImageItems_IntegerParsingTextBox.Name = "MediaImageItems_IntegerParsingTextBox"
        Me.MediaImageItems_IntegerParsingTextBox.Size = New System.Drawing.Size(428, 20)
        Me.MediaImageItems_IntegerParsingTextBox.TabIndex = 26
        '
        'MaskerImageItems_IntegerParsingTextBox
        '
        Me.MaskerImageItems_IntegerParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MaskerImageItems_IntegerParsingTextBox.ForeColor = System.Drawing.Color.Red
        Me.MaskerImageItems_IntegerParsingTextBox.Location = New System.Drawing.Point(303, 351)
        Me.MaskerImageItems_IntegerParsingTextBox.Name = "MaskerImageItems_IntegerParsingTextBox"
        Me.MaskerImageItems_IntegerParsingTextBox.Size = New System.Drawing.Size(428, 20)
        Me.MaskerImageItems_IntegerParsingTextBox.TabIndex = 27
        '
        'WaveFileSampleRate_IntegerParsingTextBox
        '
        Me.WaveFileSampleRate_IntegerParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WaveFileSampleRate_IntegerParsingTextBox.ForeColor = System.Drawing.Color.Red
        Me.WaveFileSampleRate_IntegerParsingTextBox.Location = New System.Drawing.Point(303, 701)
        Me.WaveFileSampleRate_IntegerParsingTextBox.Name = "WaveFileSampleRate_IntegerParsingTextBox"
        Me.WaveFileSampleRate_IntegerParsingTextBox.Size = New System.Drawing.Size(428, 20)
        Me.WaveFileSampleRate_IntegerParsingTextBox.TabIndex = 28
        '
        'MediaSetName_TextBox
        '
        Me.MediaSetName_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MediaSetName_TextBox.Location = New System.Drawing.Point(303, 26)
        Me.MediaSetName_TextBox.Name = "MediaSetName_TextBox"
        Me.MediaSetName_TextBox.Size = New System.Drawing.Size(428, 20)
        Me.MediaSetName_TextBox.TabIndex = 29
        '
        'MediaParentFolder_TextBox
        '
        Me.MediaParentFolder_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MediaParentFolder_TextBox.Location = New System.Drawing.Point(303, 401)
        Me.MediaParentFolder_TextBox.Name = "MediaParentFolder_TextBox"
        Me.MediaParentFolder_TextBox.Size = New System.Drawing.Size(428, 20)
        Me.MediaParentFolder_TextBox.TabIndex = 33
        '
        'BackgroundNonspeechParentFolder_TextBox
        '
        Me.BackgroundNonspeechParentFolder_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BackgroundNonspeechParentFolder_TextBox.Location = New System.Drawing.Point(303, 501)
        Me.BackgroundNonspeechParentFolder_TextBox.Name = "BackgroundNonspeechParentFolder_TextBox"
        Me.BackgroundNonspeechParentFolder_TextBox.Size = New System.Drawing.Size(428, 20)
        Me.BackgroundNonspeechParentFolder_TextBox.TabIndex = 35
        '
        'BackgroundSpeechParentFolder_TextBox
        '
        Me.BackgroundSpeechParentFolder_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BackgroundSpeechParentFolder_TextBox.Location = New System.Drawing.Point(303, 551)
        Me.BackgroundSpeechParentFolder_TextBox.Name = "BackgroundSpeechParentFolder_TextBox"
        Me.BackgroundSpeechParentFolder_TextBox.Size = New System.Drawing.Size(428, 20)
        Me.BackgroundSpeechParentFolder_TextBox.TabIndex = 36
        '
        'PrototypeMediaParentFolder_TextBox
        '
        Me.PrototypeMediaParentFolder_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PrototypeMediaParentFolder_TextBox.Location = New System.Drawing.Point(303, 576)
        Me.PrototypeMediaParentFolder_TextBox.Name = "PrototypeMediaParentFolder_TextBox"
        Me.PrototypeMediaParentFolder_TextBox.Size = New System.Drawing.Size(428, 20)
        Me.PrototypeMediaParentFolder_TextBox.TabIndex = 37
        '
        'MasterPrototypeRecordingPath_TextBox
        '
        Me.MasterPrototypeRecordingPath_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MasterPrototypeRecordingPath_TextBox.Location = New System.Drawing.Point(303, 601)
        Me.MasterPrototypeRecordingPath_TextBox.Name = "MasterPrototypeRecordingPath_TextBox"
        Me.MasterPrototypeRecordingPath_TextBox.Size = New System.Drawing.Size(428, 20)
        Me.MasterPrototypeRecordingPath_TextBox.TabIndex = 38
        '
        'TalkerGender_ComboBox
        '
        Me.TalkerGender_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TalkerGender_ComboBox.FormattingEnabled = True
        Me.TalkerGender_ComboBox.Location = New System.Drawing.Point(303, 76)
        Me.TalkerGender_ComboBox.Name = "TalkerGender_ComboBox"
        Me.TalkerGender_ComboBox.Size = New System.Drawing.Size(428, 21)
        Me.TalkerGender_ComboBox.TabIndex = 39
        '
        'WaveFileBitDepth_ComboBox
        '
        Me.WaveFileBitDepth_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WaveFileBitDepth_ComboBox.FormattingEnabled = True
        Me.WaveFileBitDepth_ComboBox.Location = New System.Drawing.Point(303, 726)
        Me.WaveFileBitDepth_ComboBox.Name = "WaveFileBitDepth_ComboBox"
        Me.WaveFileBitDepth_ComboBox.Size = New System.Drawing.Size(428, 21)
        Me.WaveFileBitDepth_ComboBox.TabIndex = 40
        '
        'Label8
        '
        Me.Label8.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label8.Location = New System.Drawing.Point(3, 523)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(294, 25)
        Me.Label8.TabIndex = 8
        Me.Label8.Text = "Background (non-speech) realistic sound level (dB SPL)"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BackgroundNonspeechRealisticLevel_DoubleParsingTextBox
        '
        Me.BackgroundNonspeechRealisticLevel_DoubleParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BackgroundNonspeechRealisticLevel_DoubleParsingTextBox.ForeColor = System.Drawing.Color.Red
        Me.BackgroundNonspeechRealisticLevel_DoubleParsingTextBox.Location = New System.Drawing.Point(303, 526)
        Me.BackgroundNonspeechRealisticLevel_DoubleParsingTextBox.Name = "BackgroundNonspeechRealisticLevel_DoubleParsingTextBox"
        Me.BackgroundNonspeechRealisticLevel_DoubleParsingTextBox.Size = New System.Drawing.Size(428, 20)
        Me.BackgroundNonspeechRealisticLevel_DoubleParsingTextBox.TabIndex = 23
        '
        'Label23
        '
        Me.Label23.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label23.Location = New System.Drawing.Point(3, 623)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(294, 25)
        Me.Label23.TabIndex = 44
        Me.Label23.Text = "Prototype recording level (dBC, 50 ms)"
        Me.Label23.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'PrototypeRecordingLevel_DoubleParsingTextBox
        '
        Me.PrototypeRecordingLevel_DoubleParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PrototypeRecordingLevel_DoubleParsingTextBox.ForeColor = System.Drawing.Color.Red
        Me.PrototypeRecordingLevel_DoubleParsingTextBox.Location = New System.Drawing.Point(303, 626)
        Me.PrototypeRecordingLevel_DoubleParsingTextBox.Name = "PrototypeRecordingLevel_DoubleParsingTextBox"
        Me.PrototypeRecordingLevel_DoubleParsingTextBox.Size = New System.Drawing.Size(428, 20)
        Me.PrototypeRecordingLevel_DoubleParsingTextBox.TabIndex = 45
        '
        'Label22
        '
        Me.Label22.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label22.Location = New System.Drawing.Point(3, 648)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(294, 25)
        Me.Label22.TabIndex = 46
        Me.Label22.Text = "Subpath to 'Lombard' (recording) noise"
        Me.Label22.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label24
        '
        Me.Label24.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label24.Location = New System.Drawing.Point(3, 673)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(294, 25)
        Me.Label24.TabIndex = 47
        Me.Label24.Text = "'Lombard' (recording) noise level (dBC, 50 ms)"
        Me.Label24.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LombardNoiseLevel_DoubleParsingTextBox
        '
        Me.LombardNoiseLevel_DoubleParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LombardNoiseLevel_DoubleParsingTextBox.ForeColor = System.Drawing.Color.Red
        Me.LombardNoiseLevel_DoubleParsingTextBox.Location = New System.Drawing.Point(303, 676)
        Me.LombardNoiseLevel_DoubleParsingTextBox.Name = "LombardNoiseLevel_DoubleParsingTextBox"
        Me.LombardNoiseLevel_DoubleParsingTextBox.Size = New System.Drawing.Size(428, 20)
        Me.LombardNoiseLevel_DoubleParsingTextBox.TabIndex = 48
        '
        'LombardNoisePath_TextBox
        '
        Me.LombardNoisePath_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LombardNoisePath_TextBox.Location = New System.Drawing.Point(303, 651)
        Me.LombardNoisePath_TextBox.Name = "LombardNoisePath_TextBox"
        Me.LombardNoisePath_TextBox.Size = New System.Drawing.Size(428, 20)
        Me.LombardNoisePath_TextBox.TabIndex = 49
        '
        'Label26
        '
        Me.Label26.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label26.Location = New System.Drawing.Point(3, 173)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(294, 25)
        Me.Label26.TabIndex = 50
        Me.Label26.Text = "Linguistic level of sound files"
        Me.Label26.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'AudioFileLinguisticLevelComboBox
        '
        Me.AudioFileLinguisticLevelComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AudioFileLinguisticLevelComboBox.FormattingEnabled = True
        Me.AudioFileLinguisticLevelComboBox.Location = New System.Drawing.Point(303, 176)
        Me.AudioFileLinguisticLevelComboBox.Name = "AudioFileLinguisticLevelComboBox"
        Me.AudioFileLinguisticLevelComboBox.Size = New System.Drawing.Size(428, 21)
        Me.AudioFileLinguisticLevelComboBox.TabIndex = 51
        '
        'SharedMaskersLevelComboBox
        '
        Me.SharedMaskersLevelComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SharedMaskersLevelComboBox.FormattingEnabled = True
        Me.SharedMaskersLevelComboBox.Location = New System.Drawing.Point(303, 201)
        Me.SharedMaskersLevelComboBox.Name = "SharedMaskersLevelComboBox"
        Me.SharedMaskersLevelComboBox.Size = New System.Drawing.Size(428, 21)
        Me.SharedMaskersLevelComboBox.TabIndex = 52
        '
        'Label30
        '
        Me.Label30.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label30.Location = New System.Drawing.Point(3, 198)
        Me.Label30.Name = "Label30"
        Me.Label30.Size = New System.Drawing.Size(294, 25)
        Me.Label30.TabIndex = 53
        Me.Label30.Text = "Linguistic level at which to share maskers"
        Me.Label30.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CustomVariablesFolder_TextBox
        '
        Me.CustomVariablesFolder_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CustomVariablesFolder_TextBox.Location = New System.Drawing.Point(303, 376)
        Me.CustomVariablesFolder_TextBox.Name = "CustomVariablesFolder_TextBox"
        Me.CustomVariablesFolder_TextBox.Size = New System.Drawing.Size(428, 20)
        Me.CustomVariablesFolder_TextBox.TabIndex = 54
        '
        'Label31
        '
        Me.Label31.AutoSize = True
        Me.Label31.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label31.Location = New System.Drawing.Point(3, 373)
        Me.Label31.Name = "Label31"
        Me.Label31.Size = New System.Drawing.Size(294, 25)
        Me.Label31.TabIndex = 55
        Me.Label31.Text = "Subfolder to store custom variables"
        Me.Label31.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
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
        'TalkerAge_IntegerParsingTextBox
        '
        Me.TalkerAge_IntegerParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TalkerAge_IntegerParsingTextBox.ForeColor = System.Drawing.Color.Red
        Me.TalkerAge_IntegerParsingTextBox.Location = New System.Drawing.Point(303, 101)
        Me.TalkerAge_IntegerParsingTextBox.Name = "TalkerAge_IntegerParsingTextBox"
        Me.TalkerAge_IntegerParsingTextBox.Size = New System.Drawing.Size(428, 20)
        Me.TalkerAge_IntegerParsingTextBox.TabIndex = 22
        '
        'Label6
        '
        Me.Label6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label6.Location = New System.Drawing.Point(3, 123)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(294, 25)
        Me.Label6.TabIndex = 6
        Me.Label6.Text = "Talker dialect"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TalkerDialect_TextBox
        '
        Me.TalkerDialect_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TalkerDialect_TextBox.Location = New System.Drawing.Point(303, 126)
        Me.TalkerDialect_TextBox.Name = "TalkerDialect_TextBox"
        Me.TalkerDialect_TextBox.Size = New System.Drawing.Size(428, 20)
        Me.TalkerDialect_TextBox.TabIndex = 31
        '
        'Label7
        '
        Me.Label7.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label7.Location = New System.Drawing.Point(3, 148)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(294, 25)
        Me.Label7.TabIndex = 7
        Me.Label7.Text = "Voice type"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'VoiceType_TextBox
        '
        Me.VoiceType_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.VoiceType_TextBox.Location = New System.Drawing.Point(303, 151)
        Me.VoiceType_TextBox.Name = "VoiceType_TextBox"
        Me.VoiceType_TextBox.Size = New System.Drawing.Size(428, 20)
        Me.VoiceType_TextBox.TabIndex = 32
        '
        'Label51
        '
        Me.Label51.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label51.Location = New System.Drawing.Point(3, 223)
        Me.Label51.Name = "Label51"
        Me.Label51.Size = New System.Drawing.Size(294, 25)
        Me.Label51.TabIndex = 56
        Me.Label51.Text = "Linguistic level at which to share contralateral maskers"
        Me.Label51.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label9
        '
        Me.Label9.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label9.Location = New System.Drawing.Point(3, 248)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(294, 25)
        Me.Label9.TabIndex = 9
        Me.Label9.Text = "Number of duplicate audio targets"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label10
        '
        Me.Label10.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label10.Location = New System.Drawing.Point(3, 273)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(294, 25)
        Me.Label10.TabIndex = 10
        Me.Label10.Text = "Number of duplicate audio maskers"
        Me.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'MediaAudioItems_IntegerParsingTextBox
        '
        Me.MediaAudioItems_IntegerParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MediaAudioItems_IntegerParsingTextBox.ForeColor = System.Drawing.Color.Red
        Me.MediaAudioItems_IntegerParsingTextBox.Location = New System.Drawing.Point(303, 251)
        Me.MediaAudioItems_IntegerParsingTextBox.Name = "MediaAudioItems_IntegerParsingTextBox"
        Me.MediaAudioItems_IntegerParsingTextBox.Size = New System.Drawing.Size(428, 20)
        Me.MediaAudioItems_IntegerParsingTextBox.TabIndex = 24
        '
        'MaskerAudioItems_IntegerParsingTextBox
        '
        Me.MaskerAudioItems_IntegerParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MaskerAudioItems_IntegerParsingTextBox.ForeColor = System.Drawing.Color.Red
        Me.MaskerAudioItems_IntegerParsingTextBox.Location = New System.Drawing.Point(303, 276)
        Me.MaskerAudioItems_IntegerParsingTextBox.Name = "MaskerAudioItems_IntegerParsingTextBox"
        Me.MaskerAudioItems_IntegerParsingTextBox.Size = New System.Drawing.Size(428, 20)
        Me.MaskerAudioItems_IntegerParsingTextBox.TabIndex = 25
        '
        'Label52
        '
        Me.Label52.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label52.Location = New System.Drawing.Point(3, 298)
        Me.Label52.Name = "Label52"
        Me.Label52.Size = New System.Drawing.Size(294, 25)
        Me.Label52.TabIndex = 57
        Me.Label52.Text = "Number of duplicate contralateral audio maskers"
        Me.Label52.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label14
        '
        Me.Label14.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label14.Location = New System.Drawing.Point(3, 423)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(294, 25)
        Me.Label14.TabIndex = 14
        Me.Label14.Text = "Subfolder containing masker files"
        Me.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'MaskerParentFolder_TextBox
        '
        Me.MaskerParentFolder_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MaskerParentFolder_TextBox.Location = New System.Drawing.Point(303, 426)
        Me.MaskerParentFolder_TextBox.Name = "MaskerParentFolder_TextBox"
        Me.MaskerParentFolder_TextBox.Size = New System.Drawing.Size(428, 20)
        Me.MaskerParentFolder_TextBox.TabIndex = 34
        '
        'Label53
        '
        Me.Label53.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label53.Location = New System.Drawing.Point(3, 448)
        Me.Label53.Name = "Label53"
        Me.Label53.Size = New System.Drawing.Size(294, 25)
        Me.Label53.TabIndex = 58
        Me.Label53.Text = "Subfolder containing contralateral masker files"
        Me.Label53.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label54
        '
        Me.Label54.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label54.Location = New System.Drawing.Point(3, 473)
        Me.Label54.Name = "Label54"
        Me.Label54.Size = New System.Drawing.Size(294, 25)
        Me.Label54.TabIndex = 59
        Me.Label54.Text = "Effective contralateral masking gain (dB)"
        Me.Label54.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'SharedContralateralMaskersLevel_ComboBox
        '
        Me.SharedContralateralMaskersLevel_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SharedContralateralMaskersLevel_ComboBox.FormattingEnabled = True
        Me.SharedContralateralMaskersLevel_ComboBox.Location = New System.Drawing.Point(303, 226)
        Me.SharedContralateralMaskersLevel_ComboBox.Name = "SharedContralateralMaskersLevel_ComboBox"
        Me.SharedContralateralMaskersLevel_ComboBox.Size = New System.Drawing.Size(428, 21)
        Me.SharedContralateralMaskersLevel_ComboBox.TabIndex = 60
        '
        'ContralateralMaskerAudioItems_IntegerParsingTextBox
        '
        Me.ContralateralMaskerAudioItems_IntegerParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ContralateralMaskerAudioItems_IntegerParsingTextBox.ForeColor = System.Drawing.Color.Red
        Me.ContralateralMaskerAudioItems_IntegerParsingTextBox.Location = New System.Drawing.Point(303, 301)
        Me.ContralateralMaskerAudioItems_IntegerParsingTextBox.Name = "ContralateralMaskerAudioItems_IntegerParsingTextBox"
        Me.ContralateralMaskerAudioItems_IntegerParsingTextBox.Size = New System.Drawing.Size(428, 20)
        Me.ContralateralMaskerAudioItems_IntegerParsingTextBox.TabIndex = 61
        '
        'ContralateralMaskerParentFolder_TextBox
        '
        Me.ContralateralMaskerParentFolder_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ContralateralMaskerParentFolder_TextBox.Location = New System.Drawing.Point(303, 451)
        Me.ContralateralMaskerParentFolder_TextBox.Name = "ContralateralMaskerParentFolder_TextBox"
        Me.ContralateralMaskerParentFolder_TextBox.Size = New System.Drawing.Size(428, 20)
        Me.ContralateralMaskerParentFolder_TextBox.TabIndex = 62
        '
        'EffectiveContralateralMaskingGain_DoubleParsingTextBox
        '
        Me.EffectiveContralateralMaskingGain_DoubleParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.EffectiveContralateralMaskingGain_DoubleParsingTextBox.ForeColor = System.Drawing.Color.Red
        Me.EffectiveContralateralMaskingGain_DoubleParsingTextBox.Location = New System.Drawing.Point(303, 476)
        Me.EffectiveContralateralMaskingGain_DoubleParsingTextBox.Name = "EffectiveContralateralMaskingGain_DoubleParsingTextBox"
        Me.EffectiveContralateralMaskingGain_DoubleParsingTextBox.Size = New System.Drawing.Size(428, 20)
        Me.EffectiveContralateralMaskingGain_DoubleParsingTextBox.TabIndex = 63
        '
        'EditSoundFile_TabControl
        '
        Me.EditSoundFile_TabControl.Controls.Add(Me.StartRecorder_TabPage)
        Me.EditSoundFile_TabControl.Controls.Add(Me.SpeechLevels_TabPage)
        Me.EditSoundFile_TabControl.Controls.Add(Me.ListMixer_TabPage)
        Me.EditSoundFile_TabControl.Controls.Add(Me.CreateMaskers_TabPage)
        Me.EditSoundFile_TabControl.Controls.Add(Me.SoundfileLinguisticLevels_TabPage)
        Me.EditSoundFile_TabControl.Controls.Add(Me.TabPage1)
        Me.EditSoundFile_TabControl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.EditSoundFile_TabControl.Enabled = False
        Me.EditSoundFile_TabControl.Location = New System.Drawing.Point(0, 0)
        Me.EditSoundFile_TabControl.Name = "EditSoundFile_TabControl"
        Me.EditSoundFile_TabControl.SelectedIndex = 0
        Me.EditSoundFile_TabControl.Size = New System.Drawing.Size(570, 637)
        Me.EditSoundFile_TabControl.TabIndex = 3
        '
        'StartRecorder_TabPage
        '
        Me.StartRecorder_TabPage.AutoScroll = True
        Me.StartRecorder_TabPage.BackColor = System.Drawing.Color.PaleTurquoise
        Me.StartRecorder_TabPage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.StartRecorder_TabPage.Controls.Add(Me.TableLayoutPanel1)
        Me.StartRecorder_TabPage.Location = New System.Drawing.Point(4, 22)
        Me.StartRecorder_TabPage.Name = "StartRecorder_TabPage"
        Me.StartRecorder_TabPage.Padding = New System.Windows.Forms.Padding(3)
        Me.StartRecorder_TabPage.Size = New System.Drawing.Size(562, 611)
        Me.StartRecorder_TabPage.TabIndex = 0
        Me.StartRecorder_TabPage.Text = "Recording and segmentation tool"
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.AutoSize = True
        Me.TableLayoutPanel1.BackColor = System.Drawing.SystemColors.Control
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.LaunchRecorder_Button, 0, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.RandomOrder_CheckBox, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.SpecificPrototypeRecording_RadioButton, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.NoPrototypeRecording_RadioButton, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.MasterPrototypeRecording_RadioButton, 0, 1)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(3, 3)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 6
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(554, 150)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'LaunchRecorder_Button
        '
        Me.LaunchRecorder_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LaunchRecorder_Button.Location = New System.Drawing.Point(3, 103)
        Me.LaunchRecorder_Button.Name = "LaunchRecorder_Button"
        Me.LaunchRecorder_Button.Size = New System.Drawing.Size(548, 44)
        Me.LaunchRecorder_Button.TabIndex = 0
        Me.LaunchRecorder_Button.Text = "Launch recording and segmentation tool"
        Me.LaunchRecorder_Button.UseVisualStyleBackColor = True
        '
        'RandomOrder_CheckBox
        '
        Me.RandomOrder_CheckBox.AutoSize = True
        Me.RandomOrder_CheckBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RandomOrder_CheckBox.Location = New System.Drawing.Point(3, 3)
        Me.RandomOrder_CheckBox.Name = "RandomOrder_CheckBox"
        Me.RandomOrder_CheckBox.Size = New System.Drawing.Size(548, 19)
        Me.RandomOrder_CheckBox.TabIndex = 1
        Me.RandomOrder_CheckBox.Text = "Randomize order"
        Me.RandomOrder_CheckBox.UseVisualStyleBackColor = True
        '
        'SpecificPrototypeRecording_RadioButton
        '
        Me.SpecificPrototypeRecording_RadioButton.AutoSize = True
        Me.SpecificPrototypeRecording_RadioButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SpecificPrototypeRecording_RadioButton.Location = New System.Drawing.Point(3, 53)
        Me.SpecificPrototypeRecording_RadioButton.Name = "SpecificPrototypeRecording_RadioButton"
        Me.SpecificPrototypeRecording_RadioButton.Size = New System.Drawing.Size(548, 19)
        Me.SpecificPrototypeRecording_RadioButton.TabIndex = 2
        Me.SpecificPrototypeRecording_RadioButton.TabStop = True
        Me.SpecificPrototypeRecording_RadioButton.Text = "Use prototype recordings"
        Me.SpecificPrototypeRecording_RadioButton.UseVisualStyleBackColor = True
        '
        'NoPrototypeRecording_RadioButton
        '
        Me.NoPrototypeRecording_RadioButton.AutoSize = True
        Me.NoPrototypeRecording_RadioButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.NoPrototypeRecording_RadioButton.Location = New System.Drawing.Point(3, 78)
        Me.NoPrototypeRecording_RadioButton.Name = "NoPrototypeRecording_RadioButton"
        Me.NoPrototypeRecording_RadioButton.Size = New System.Drawing.Size(548, 19)
        Me.NoPrototypeRecording_RadioButton.TabIndex = 3
        Me.NoPrototypeRecording_RadioButton.TabStop = True
        Me.NoPrototypeRecording_RadioButton.Text = "No prototype recordings"
        Me.NoPrototypeRecording_RadioButton.UseVisualStyleBackColor = True
        '
        'MasterPrototypeRecording_RadioButton
        '
        Me.MasterPrototypeRecording_RadioButton.AutoSize = True
        Me.MasterPrototypeRecording_RadioButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MasterPrototypeRecording_RadioButton.Location = New System.Drawing.Point(3, 28)
        Me.MasterPrototypeRecording_RadioButton.Name = "MasterPrototypeRecording_RadioButton"
        Me.MasterPrototypeRecording_RadioButton.Size = New System.Drawing.Size(548, 19)
        Me.MasterPrototypeRecording_RadioButton.TabIndex = 4
        Me.MasterPrototypeRecording_RadioButton.TabStop = True
        Me.MasterPrototypeRecording_RadioButton.Text = "Use master prototype recording"
        Me.MasterPrototypeRecording_RadioButton.UseVisualStyleBackColor = True
        '
        'SpeechLevels_TabPage
        '
        Me.SpeechLevels_TabPage.AutoScroll = True
        Me.SpeechLevels_TabPage.BackColor = System.Drawing.Color.PaleGreen
        Me.SpeechLevels_TabPage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.SpeechLevels_TabPage.Controls.Add(Me.TableLayoutPanel5)
        Me.SpeechLevels_TabPage.Location = New System.Drawing.Point(4, 22)
        Me.SpeechLevels_TabPage.Name = "SpeechLevels_TabPage"
        Me.SpeechLevels_TabPage.Padding = New System.Windows.Forms.Padding(3)
        Me.SpeechLevels_TabPage.Size = New System.Drawing.Size(562, 611)
        Me.SpeechLevels_TabPage.TabIndex = 1
        Me.SpeechLevels_TabPage.Text = "Speech levels"
        '
        'TableLayoutPanel5
        '
        Me.TableLayoutPanel5.AutoSize = True
        Me.TableLayoutPanel5.BackColor = System.Drawing.SystemColors.Control
        Me.TableLayoutPanel5.ColumnCount = 1
        Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel5.Controls.Add(Me.GroupBox1, 0, 0)
        Me.TableLayoutPanel5.Controls.Add(Me.GroupBox2, 0, 3)
        Me.TableLayoutPanel5.Controls.Add(Me.GroupBox3, 0, 4)
        Me.TableLayoutPanel5.Controls.Add(Me.GroupBox4, 0, 2)
        Me.TableLayoutPanel5.Controls.Add(Me.GroupBox5, 0, 1)
        Me.TableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Top
        Me.TableLayoutPanel5.Location = New System.Drawing.Point(3, 3)
        Me.TableLayoutPanel5.Name = "TableLayoutPanel5"
        Me.TableLayoutPanel5.RowCount = 6
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 273.0!))
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 521.0!))
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 300.0!))
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200.0!))
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200.0!))
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel5.Size = New System.Drawing.Size(537, 1494)
        Me.TableLayoutPanel5.TabIndex = 1
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.TableLayoutPanel3)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox1.Location = New System.Drawing.Point(3, 3)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(531, 267)
        Me.GroupBox1.TabIndex = 12
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Set speech level"
        '
        'TableLayoutPanel3
        '
        Me.TableLayoutPanel3.BackColor = System.Drawing.SystemColors.Control
        Me.TableLayoutPanel3.ColumnCount = 2
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel3.Controls.Add(Me.SpeechLevelFS_DoubleParsingTextBox, 1, 2)
        Me.TableLayoutPanel3.Controls.Add(Me.Label29, 0, 2)
        Me.TableLayoutPanel3.Controls.Add(Me.ApplySpeechLevels_Button, 0, 8)
        Me.TableLayoutPanel3.Controls.Add(Me.SpeechLevelSPL_Label, 0, 1)
        Me.TableLayoutPanel3.Controls.Add(Me.Label28, 0, 3)
        Me.TableLayoutPanel3.Controls.Add(Me.SpeechLevelTemporalIntegration_CheckBox, 0, 4)
        Me.TableLayoutPanel3.Controls.Add(Me.OnlyNominalLevel_CheckBox, 1, 6)
        Me.TableLayoutPanel3.Controls.Add(Me.VpNormalization_Checkbox, 0, 5)
        Me.TableLayoutPanel3.Controls.Add(Me.SpeechLevelSPL_DoubleParsingTextBox, 1, 1)
        Me.TableLayoutPanel3.Controls.Add(Me.SpeechLevelFrequencyWeighting_ComboBox, 1, 3)
        Me.TableLayoutPanel3.Controls.Add(Me.SpeechLevelTemporalIntegration_DoubleParsingTextBox, 1, 4)
        Me.TableLayoutPanel3.Controls.Add(Me.Label40, 0, 0)
        Me.TableLayoutPanel3.Controls.Add(Me.SpeechLevel_TargetLinguisticlevel_ComboBox, 1, 0)
        Me.TableLayoutPanel3.Controls.Add(Me.NominalLevel_CheckBox, 0, 6)
        Me.TableLayoutPanel3.Controls.Add(Me.CreateCalibrationSignal_CheckBox, 0, 7)
        Me.TableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel3.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel3.Name = "TableLayoutPanel3"
        Me.TableLayoutPanel3.RowCount = 10
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel3.Size = New System.Drawing.Size(525, 248)
        Me.TableLayoutPanel3.TabIndex = 0
        '
        'SpeechLevelFS_DoubleParsingTextBox
        '
        Me.SpeechLevelFS_DoubleParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SpeechLevelFS_DoubleParsingTextBox.ForeColor = System.Drawing.Color.Red
        Me.SpeechLevelFS_DoubleParsingTextBox.Location = New System.Drawing.Point(265, 53)
        Me.SpeechLevelFS_DoubleParsingTextBox.Name = "SpeechLevelFS_DoubleParsingTextBox"
        Me.SpeechLevelFS_DoubleParsingTextBox.Size = New System.Drawing.Size(257, 20)
        Me.SpeechLevelFS_DoubleParsingTextBox.TabIndex = 10
        '
        'Label29
        '
        Me.Label29.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label29.Location = New System.Drawing.Point(3, 50)
        Me.Label29.Name = "Label29"
        Me.Label29.Size = New System.Drawing.Size(256, 25)
        Me.Label29.TabIndex = 9
        Me.Label29.Text = "Speech level (dB FS)"
        Me.Label29.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ApplySpeechLevels_Button
        '
        Me.TableLayoutPanel3.SetColumnSpan(Me.ApplySpeechLevels_Button, 2)
        Me.ApplySpeechLevels_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ApplySpeechLevels_Button.Location = New System.Drawing.Point(3, 203)
        Me.ApplySpeechLevels_Button.Name = "ApplySpeechLevels_Button"
        Me.ApplySpeechLevels_Button.Size = New System.Drawing.Size(519, 29)
        Me.ApplySpeechLevels_Button.TabIndex = 0
        Me.ApplySpeechLevels_Button.Text = "Apply speech levels"
        Me.ApplySpeechLevels_Button.UseVisualStyleBackColor = True
        '
        'SpeechLevelSPL_Label
        '
        Me.SpeechLevelSPL_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SpeechLevelSPL_Label.Location = New System.Drawing.Point(3, 25)
        Me.SpeechLevelSPL_Label.Name = "SpeechLevelSPL_Label"
        Me.SpeechLevelSPL_Label.Size = New System.Drawing.Size(256, 25)
        Me.SpeechLevelSPL_Label.TabIndex = 1
        Me.SpeechLevelSPL_Label.Text = "Speech level (dB SPL)"
        Me.SpeechLevelSPL_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label28
        '
        Me.Label28.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label28.Location = New System.Drawing.Point(3, 75)
        Me.Label28.Name = "Label28"
        Me.Label28.Size = New System.Drawing.Size(256, 25)
        Me.Label28.TabIndex = 2
        Me.Label28.Text = "Frequency weighting"
        Me.Label28.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'SpeechLevelTemporalIntegration_CheckBox
        '
        Me.SpeechLevelTemporalIntegration_CheckBox.AutoSize = True
        Me.SpeechLevelTemporalIntegration_CheckBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SpeechLevelTemporalIntegration_CheckBox.Location = New System.Drawing.Point(3, 103)
        Me.SpeechLevelTemporalIntegration_CheckBox.Name = "SpeechLevelTemporalIntegration_CheckBox"
        Me.SpeechLevelTemporalIntegration_CheckBox.Size = New System.Drawing.Size(256, 19)
        Me.SpeechLevelTemporalIntegration_CheckBox.TabIndex = 3
        Me.SpeechLevelTemporalIntegration_CheckBox.Text = "Temporal integration (ms)"
        Me.SpeechLevelTemporalIntegration_CheckBox.UseVisualStyleBackColor = True
        '
        'OnlyNominalLevel_CheckBox
        '
        Me.OnlyNominalLevel_CheckBox.AutoSize = True
        Me.OnlyNominalLevel_CheckBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.OnlyNominalLevel_CheckBox.Location = New System.Drawing.Point(265, 153)
        Me.OnlyNominalLevel_CheckBox.Name = "OnlyNominalLevel_CheckBox"
        Me.OnlyNominalLevel_CheckBox.Size = New System.Drawing.Size(257, 19)
        Me.OnlyNominalLevel_CheckBox.TabIndex = 13
        Me.OnlyNominalLevel_CheckBox.Text = "Do not modify sound (i.e. only set nominal level)"
        Me.OnlyNominalLevel_CheckBox.UseVisualStyleBackColor = True
        '
        'VpNormalization_Checkbox
        '
        Me.VpNormalization_Checkbox.AutoSize = True
        Me.TableLayoutPanel3.SetColumnSpan(Me.VpNormalization_Checkbox, 2)
        Me.VpNormalization_Checkbox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.VpNormalization_Checkbox.Location = New System.Drawing.Point(3, 128)
        Me.VpNormalization_Checkbox.Name = "VpNormalization_Checkbox"
        Me.VpNormalization_Checkbox.Size = New System.Drawing.Size(519, 19)
        Me.VpNormalization_Checkbox.TabIndex = 4
        Me.VpNormalization_Checkbox.Text = "Use variation-preserving (VP) sound-level normalization"
        Me.VpNormalization_Checkbox.UseVisualStyleBackColor = True
        '
        'SpeechLevelSPL_DoubleParsingTextBox
        '
        Me.SpeechLevelSPL_DoubleParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SpeechLevelSPL_DoubleParsingTextBox.ForeColor = System.Drawing.SystemColors.WindowText
        Me.SpeechLevelSPL_DoubleParsingTextBox.Location = New System.Drawing.Point(265, 28)
        Me.SpeechLevelSPL_DoubleParsingTextBox.Name = "SpeechLevelSPL_DoubleParsingTextBox"
        Me.SpeechLevelSPL_DoubleParsingTextBox.Size = New System.Drawing.Size(257, 20)
        Me.SpeechLevelSPL_DoubleParsingTextBox.TabIndex = 5
        Me.SpeechLevelSPL_DoubleParsingTextBox.Text = "65"
        '
        'SpeechLevelFrequencyWeighting_ComboBox
        '
        Me.SpeechLevelFrequencyWeighting_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SpeechLevelFrequencyWeighting_ComboBox.FormattingEnabled = True
        Me.SpeechLevelFrequencyWeighting_ComboBox.Location = New System.Drawing.Point(265, 78)
        Me.SpeechLevelFrequencyWeighting_ComboBox.Name = "SpeechLevelFrequencyWeighting_ComboBox"
        Me.SpeechLevelFrequencyWeighting_ComboBox.Size = New System.Drawing.Size(257, 21)
        Me.SpeechLevelFrequencyWeighting_ComboBox.TabIndex = 7
        '
        'SpeechLevelTemporalIntegration_DoubleParsingTextBox
        '
        Me.SpeechLevelTemporalIntegration_DoubleParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SpeechLevelTemporalIntegration_DoubleParsingTextBox.Enabled = False
        Me.SpeechLevelTemporalIntegration_DoubleParsingTextBox.ForeColor = System.Drawing.Color.Red
        Me.SpeechLevelTemporalIntegration_DoubleParsingTextBox.Location = New System.Drawing.Point(265, 103)
        Me.SpeechLevelTemporalIntegration_DoubleParsingTextBox.Name = "SpeechLevelTemporalIntegration_DoubleParsingTextBox"
        Me.SpeechLevelTemporalIntegration_DoubleParsingTextBox.Size = New System.Drawing.Size(257, 20)
        Me.SpeechLevelTemporalIntegration_DoubleParsingTextBox.TabIndex = 8
        '
        'Label40
        '
        Me.Label40.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label40.Location = New System.Drawing.Point(3, 0)
        Me.Label40.Name = "Label40"
        Me.Label40.Size = New System.Drawing.Size(256, 25)
        Me.Label40.TabIndex = 13
        Me.Label40.Text = "Target linguistic level:"
        Me.Label40.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'SpeechLevel_TargetLinguisticlevel_ComboBox
        '
        Me.SpeechLevel_TargetLinguisticlevel_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SpeechLevel_TargetLinguisticlevel_ComboBox.FormattingEnabled = True
        Me.SpeechLevel_TargetLinguisticlevel_ComboBox.Location = New System.Drawing.Point(265, 3)
        Me.SpeechLevel_TargetLinguisticlevel_ComboBox.Name = "SpeechLevel_TargetLinguisticlevel_ComboBox"
        Me.SpeechLevel_TargetLinguisticlevel_ComboBox.Size = New System.Drawing.Size(257, 21)
        Me.SpeechLevel_TargetLinguisticlevel_ComboBox.TabIndex = 14
        '
        'NominalLevel_CheckBox
        '
        Me.NominalLevel_CheckBox.AutoSize = True
        Me.NominalLevel_CheckBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.NominalLevel_CheckBox.Location = New System.Drawing.Point(3, 153)
        Me.NominalLevel_CheckBox.Name = "NominalLevel_CheckBox"
        Me.NominalLevel_CheckBox.Size = New System.Drawing.Size(256, 19)
        Me.NominalLevel_CheckBox.TabIndex = 15
        Me.NominalLevel_CheckBox.Text = "Set as nominal level (i.e. calibration level)"
        Me.NominalLevel_CheckBox.UseVisualStyleBackColor = True
        '
        'CreateCalibrationSignal_CheckBox
        '
        Me.CreateCalibrationSignal_CheckBox.AutoSize = True
        Me.TableLayoutPanel3.SetColumnSpan(Me.CreateCalibrationSignal_CheckBox, 2)
        Me.CreateCalibrationSignal_CheckBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CreateCalibrationSignal_CheckBox.Location = New System.Drawing.Point(3, 178)
        Me.CreateCalibrationSignal_CheckBox.Name = "CreateCalibrationSignal_CheckBox"
        Me.CreateCalibrationSignal_CheckBox.Size = New System.Drawing.Size(519, 19)
        Me.CreateCalibrationSignal_CheckBox.TabIndex = 16
        Me.CreateCalibrationSignal_CheckBox.Text = "Create calibration signal"
        Me.CreateCalibrationSignal_CheckBox.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.TableLayoutPanel6)
        Me.GroupBox2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox2.Location = New System.Drawing.Point(3, 1097)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(531, 194)
        Me.GroupBox2.TabIndex = 13
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Measure speech material annotation (SMA) levels "
        '
        'TableLayoutPanel6
        '
        Me.TableLayoutPanel6.ColumnCount = 2
        Me.TableLayoutPanel6.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel6.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel6.Controls.Add(Me.Label27, 0, 0)
        Me.TableLayoutPanel6.Controls.Add(Me.MeasureSmaLevels_Button, 0, 3)
        Me.TableLayoutPanel6.Controls.Add(Me.SmaTemporalIntegration_CheckBox, 0, 1)
        Me.TableLayoutPanel6.Controls.Add(Me.SmaFrequencyWeighting_ComboBox, 1, 0)
        Me.TableLayoutPanel6.Controls.Add(Me.SmaTemporalIntegration_DoubleParsingTextBox, 1, 1)
        Me.TableLayoutPanel6.Controls.Add(Me.IncludeCriticalBandLevels_CheckBox, 0, 2)
        Me.TableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel6.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel6.Name = "TableLayoutPanel6"
        Me.TableLayoutPanel6.RowCount = 5
        Me.TableLayoutPanel6.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel6.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel6.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel6.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35.0!))
        Me.TableLayoutPanel6.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel6.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel6.Size = New System.Drawing.Size(525, 175)
        Me.TableLayoutPanel6.TabIndex = 0
        '
        'Label27
        '
        Me.Label27.Location = New System.Drawing.Point(3, 0)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(178, 25)
        Me.Label27.TabIndex = 3
        Me.Label27.Text = "Frequency weighting"
        Me.Label27.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'MeasureSmaLevels_Button
        '
        Me.TableLayoutPanel6.SetColumnSpan(Me.MeasureSmaLevels_Button, 2)
        Me.MeasureSmaLevels_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MeasureSmaLevels_Button.Location = New System.Drawing.Point(3, 78)
        Me.MeasureSmaLevels_Button.Name = "MeasureSmaLevels_Button"
        Me.MeasureSmaLevels_Button.Size = New System.Drawing.Size(519, 29)
        Me.MeasureSmaLevels_Button.TabIndex = 11
        Me.MeasureSmaLevels_Button.Text = "Measure and store SMA component sound levels"
        Me.MeasureSmaLevels_Button.UseVisualStyleBackColor = True
        '
        'SmaTemporalIntegration_CheckBox
        '
        Me.SmaTemporalIntegration_CheckBox.AutoSize = True
        Me.SmaTemporalIntegration_CheckBox.Location = New System.Drawing.Point(3, 28)
        Me.SmaTemporalIntegration_CheckBox.Name = "SmaTemporalIntegration_CheckBox"
        Me.SmaTemporalIntegration_CheckBox.Size = New System.Drawing.Size(144, 17)
        Me.SmaTemporalIntegration_CheckBox.TabIndex = 9
        Me.SmaTemporalIntegration_CheckBox.Text = "Temporal integration (ms)"
        Me.SmaTemporalIntegration_CheckBox.UseVisualStyleBackColor = True
        '
        'SmaFrequencyWeighting_ComboBox
        '
        Me.SmaFrequencyWeighting_ComboBox.FormattingEnabled = True
        Me.SmaFrequencyWeighting_ComboBox.Location = New System.Drawing.Point(265, 3)
        Me.SmaFrequencyWeighting_ComboBox.Name = "SmaFrequencyWeighting_ComboBox"
        Me.SmaFrequencyWeighting_ComboBox.Size = New System.Drawing.Size(178, 21)
        Me.SmaFrequencyWeighting_ComboBox.TabIndex = 8
        '
        'SmaTemporalIntegration_DoubleParsingTextBox
        '
        Me.SmaTemporalIntegration_DoubleParsingTextBox.Enabled = False
        Me.SmaTemporalIntegration_DoubleParsingTextBox.ForeColor = System.Drawing.Color.Red
        Me.SmaTemporalIntegration_DoubleParsingTextBox.Location = New System.Drawing.Point(265, 28)
        Me.SmaTemporalIntegration_DoubleParsingTextBox.Name = "SmaTemporalIntegration_DoubleParsingTextBox"
        Me.SmaTemporalIntegration_DoubleParsingTextBox.Size = New System.Drawing.Size(178, 20)
        Me.SmaTemporalIntegration_DoubleParsingTextBox.TabIndex = 10
        '
        'IncludeCriticalBandLevels_CheckBox
        '
        Me.IncludeCriticalBandLevels_CheckBox.AutoSize = True
        Me.IncludeCriticalBandLevels_CheckBox.Location = New System.Drawing.Point(3, 53)
        Me.IncludeCriticalBandLevels_CheckBox.Name = "IncludeCriticalBandLevels_CheckBox"
        Me.IncludeCriticalBandLevels_CheckBox.Size = New System.Drawing.Size(151, 17)
        Me.IncludeCriticalBandLevels_CheckBox.TabIndex = 12
        Me.IncludeCriticalBandLevels_CheckBox.Text = "Include critical band levels"
        Me.IncludeCriticalBandLevels_CheckBox.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        Me.GroupBox3.BackColor = System.Drawing.SystemColors.Control
        Me.GroupBox3.Controls.Add(Me.TableLayoutPanel7)
        Me.GroupBox3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox3.Location = New System.Drawing.Point(3, 1297)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(531, 194)
        Me.GroupBox3.TabIndex = 14
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "CustomId functions"
        '
        'TableLayoutPanel7
        '
        Me.TableLayoutPanel7.BackColor = System.Drawing.SystemColors.Control
        Me.TableLayoutPanel7.ColumnCount = 1
        Me.TableLayoutPanel7.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel7.Controls.Add(Me.SipTestLevels_Button, 0, 0)
        Me.TableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel7.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel7.Name = "TableLayoutPanel7"
        Me.TableLayoutPanel7.RowCount = 2
        Me.TableLayoutPanel7.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel7.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel7.Size = New System.Drawing.Size(525, 175)
        Me.TableLayoutPanel7.TabIndex = 0
        '
        'SipTestLevels_Button
        '
        Me.SipTestLevels_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SipTestLevels_Button.Location = New System.Drawing.Point(3, 3)
        Me.SipTestLevels_Button.Name = "SipTestLevels_Button"
        Me.SipTestLevels_Button.Size = New System.Drawing.Size(519, 81)
        Me.SipTestLevels_Button.TabIndex = 0
        Me.SipTestLevels_Button.Text = "Calculate SiP-test sounds levels"
        Me.SipTestLevels_Button.UseVisualStyleBackColor = True
        '
        'GroupBox4
        '
        Me.GroupBox4.BackColor = System.Drawing.SystemColors.Control
        Me.GroupBox4.Controls.Add(Me.TableLayoutPanel9)
        Me.GroupBox4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox4.Location = New System.Drawing.Point(3, 797)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(531, 294)
        Me.GroupBox4.TabIndex = 15
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "CustomId speech level adjustment"
        '
        'TableLayoutPanel9
        '
        Me.TableLayoutPanel9.BackColor = System.Drawing.SystemColors.Control
        Me.TableLayoutPanel9.ColumnCount = 2
        Me.TableLayoutPanel9.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.61345!))
        Me.TableLayoutPanel9.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.38655!))
        Me.TableLayoutPanel9.Controls.Add(Me.CustomSpeechGain_TextBox, 1, 1)
        Me.TableLayoutPanel9.Controls.Add(Me.Label32, 1, 0)
        Me.TableLayoutPanel9.Controls.Add(Me.Label41, 0, 0)
        Me.TableLayoutPanel9.Controls.Add(Me.TextBox2, 0, 1)
        Me.TableLayoutPanel9.Controls.Add(Me.ApplyCustomSpeechGain_Button, 0, 2)
        Me.TableLayoutPanel9.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel9.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel9.Name = "TableLayoutPanel9"
        Me.TableLayoutPanel9.RowCount = 3
        Me.TableLayoutPanel9.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel9.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel9.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35.0!))
        Me.TableLayoutPanel9.Size = New System.Drawing.Size(525, 275)
        Me.TableLayoutPanel9.TabIndex = 0
        '
        'CustomSpeechGain_TextBox
        '
        Me.CustomSpeechGain_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CustomSpeechGain_TextBox.Location = New System.Drawing.Point(179, 29)
        Me.CustomSpeechGain_TextBox.Multiline = True
        Me.CustomSpeechGain_TextBox.Name = "CustomSpeechGain_TextBox"
        Me.TableLayoutPanel9.SetRowSpan(Me.CustomSpeechGain_TextBox, 2)
        Me.CustomSpeechGain_TextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.CustomSpeechGain_TextBox.Size = New System.Drawing.Size(343, 243)
        Me.CustomSpeechGain_TextBox.TabIndex = 0
        '
        'Label32
        '
        Me.Label32.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label32.Location = New System.Drawing.Point(179, 0)
        Me.Label32.Name = "Label32"
        Me.Label32.Size = New System.Drawing.Size(343, 26)
        Me.Label32.TabIndex = 1
        Me.Label32.Text = "Component gain (dB)"
        Me.Label32.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label41
        '
        Me.Label41.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label41.Location = New System.Drawing.Point(3, 0)
        Me.Label41.Name = "Label41"
        Me.Label41.Size = New System.Drawing.Size(170, 26)
        Me.Label41.TabIndex = 2
        Me.Label41.Text = "Instructions"
        Me.Label41.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TextBox2
        '
        Me.TextBox2.Cursor = System.Windows.Forms.Cursors.Default
        Me.TextBox2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TextBox2.Location = New System.Drawing.Point(3, 29)
        Me.TextBox2.Multiline = True
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.ReadOnly = True
        Me.TextBox2.Size = New System.Drawing.Size(170, 208)
        Me.TextBox2.TabIndex = 3
        Me.TextBox2.Text = resources.GetString("TextBox2.Text")
        '
        'ApplyCustomSpeechGain_Button
        '
        Me.ApplyCustomSpeechGain_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ApplyCustomSpeechGain_Button.Location = New System.Drawing.Point(3, 243)
        Me.ApplyCustomSpeechGain_Button.Name = "ApplyCustomSpeechGain_Button"
        Me.ApplyCustomSpeechGain_Button.Size = New System.Drawing.Size(170, 29)
        Me.ApplyCustomSpeechGain_Button.TabIndex = 4
        Me.ApplyCustomSpeechGain_Button.Text = "Apply gain"
        Me.ApplyCustomSpeechGain_Button.UseVisualStyleBackColor = True
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.TableLayoutPanel10)
        Me.GroupBox5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox5.Location = New System.Drawing.Point(3, 276)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(531, 515)
        Me.GroupBox5.TabIndex = 16
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "Export SNR range stimuli"
        '
        'TableLayoutPanel10
        '
        Me.TableLayoutPanel10.ColumnCount = 2
        Me.TableLayoutPanel10.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel10.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel10.Controls.Add(Me.Label42, 0, 0)
        Me.TableLayoutPanel10.Controls.Add(Me.TextBox1, 0, 1)
        Me.TableLayoutPanel10.Controls.Add(Me.Label43, 0, 2)
        Me.TableLayoutPanel10.Controls.Add(Me.Label44, 0, 3)
        Me.TableLayoutPanel10.Controls.Add(Me.Label45, 0, 4)
        Me.TableLayoutPanel10.Controls.Add(Me.Label46, 0, 5)
        Me.TableLayoutPanel10.Controls.Add(Me.Label47, 0, 8)
        Me.TableLayoutPanel10.Controls.Add(Me.GenerateSnrRangeStimuli_UpperLimit_IntegerParsingTextBox, 1, 2)
        Me.TableLayoutPanel10.Controls.Add(Me.GenerateSnrRangeStimuli_LowerLimit_IntegerParsingTextBox, 1, 3)
        Me.TableLayoutPanel10.Controls.Add(Me.GenerateSnrRangeStimuli_StepSize_DoubleParsingTextBox, 1, 4)
        Me.TableLayoutPanel10.Controls.Add(Me.GenerateSnrRangeStimuli_PresentationLevel_DoubleParsingTextBox, 1, 8)
        Me.TableLayoutPanel10.Controls.Add(Me.GenerateSnrRangeStimuli_NoiseType_ComboBox, 1, 5)
        Me.TableLayoutPanel10.Controls.Add(Me.GenerateSnrRangeStimuli_Button, 0, 11)
        Me.TableLayoutPanel10.Controls.Add(Me.GenerateSnrRangeStimuli_NoiseFW_ComboBox, 1, 6)
        Me.TableLayoutPanel10.Controls.Add(Me.Label48, 0, 6)
        Me.TableLayoutPanel10.Controls.Add(Me.Label49, 0, 9)
        Me.TableLayoutPanel10.Controls.Add(Me.GenerateSnrRangeStimuli_OutputGain_DoubleParsingTextBox, 1, 9)
        Me.TableLayoutPanel10.Controls.Add(Me.Label50, 0, 7)
        Me.TableLayoutPanel10.Controls.Add(Me.GenerateSnrRangeStimuli_Overlays_IntegerParsingTextBox, 1, 7)
        Me.TableLayoutPanel10.Controls.Add(Me.GenerateSnrRangeStimuli_ConcatenatedSound_CheckBox, 0, 10)
        Me.TableLayoutPanel10.Controls.Add(Me.GenerateSnrRangeStimuli_InsertConcatenationSilence_CheckBox, 1, 10)
        Me.TableLayoutPanel10.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel10.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel10.Name = "TableLayoutPanel10"
        Me.TableLayoutPanel10.RowCount = 12
        Me.TableLayoutPanel10.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel10.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel10.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel10.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel10.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel10.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel10.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel10.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel10.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel10.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel10.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel10.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35.0!))
        Me.TableLayoutPanel10.Size = New System.Drawing.Size(525, 496)
        Me.TableLayoutPanel10.TabIndex = 0
        '
        'Label42
        '
        Me.TableLayoutPanel10.SetColumnSpan(Me.Label42, 2)
        Me.Label42.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label42.Location = New System.Drawing.Point(3, 0)
        Me.Label42.Name = "Label42"
        Me.Label42.Size = New System.Drawing.Size(519, 26)
        Me.Label42.TabIndex = 3
        Me.Label42.Text = "Instructions"
        Me.Label42.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TextBox1
        '
        Me.TableLayoutPanel10.SetColumnSpan(Me.TextBox1, 2)
        Me.TextBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TextBox1.Location = New System.Drawing.Point(3, 29)
        Me.TextBox1.Multiline = True
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(519, 195)
        Me.TextBox1.TabIndex = 0
        Me.TextBox1.Text = resources.GetString("TextBox1.Text")
        '
        'Label43
        '
        Me.Label43.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label43.Location = New System.Drawing.Point(3, 227)
        Me.Label43.Name = "Label43"
        Me.Label43.Size = New System.Drawing.Size(256, 26)
        Me.Label43.TabIndex = 4
        Me.Label43.Text = "Upper SNR limit"
        Me.Label43.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label44
        '
        Me.Label44.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label44.Location = New System.Drawing.Point(3, 253)
        Me.Label44.Name = "Label44"
        Me.Label44.Size = New System.Drawing.Size(256, 26)
        Me.Label44.TabIndex = 5
        Me.Label44.Text = "Lower SNR limit"
        Me.Label44.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label45
        '
        Me.Label45.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label45.Location = New System.Drawing.Point(3, 279)
        Me.Label45.Name = "Label45"
        Me.Label45.Size = New System.Drawing.Size(256, 26)
        Me.Label45.TabIndex = 6
        Me.Label45.Text = "SNR step size"
        Me.Label45.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label46
        '
        Me.Label46.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label46.Location = New System.Drawing.Point(3, 305)
        Me.Label46.Name = "Label46"
        Me.Label46.Size = New System.Drawing.Size(256, 26)
        Me.Label46.TabIndex = 7
        Me.Label46.Text = "Masking noise type"
        Me.Label46.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label47
        '
        Me.Label47.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label47.Location = New System.Drawing.Point(3, 383)
        Me.Label47.Name = "Label47"
        Me.Label47.Size = New System.Drawing.Size(256, 26)
        Me.Label47.TabIndex = 8
        Me.Label47.Text = "Intended loudness level (Phon)"
        Me.Label47.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'GenerateSnrRangeStimuli_UpperLimit_IntegerParsingTextBox
        '
        Me.GenerateSnrRangeStimuli_UpperLimit_IntegerParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GenerateSnrRangeStimuli_UpperLimit_IntegerParsingTextBox.ForeColor = System.Drawing.SystemColors.WindowText
        Me.GenerateSnrRangeStimuli_UpperLimit_IntegerParsingTextBox.Location = New System.Drawing.Point(265, 230)
        Me.GenerateSnrRangeStimuli_UpperLimit_IntegerParsingTextBox.Name = "GenerateSnrRangeStimuli_UpperLimit_IntegerParsingTextBox"
        Me.GenerateSnrRangeStimuli_UpperLimit_IntegerParsingTextBox.Size = New System.Drawing.Size(257, 20)
        Me.GenerateSnrRangeStimuli_UpperLimit_IntegerParsingTextBox.TabIndex = 9
        Me.GenerateSnrRangeStimuli_UpperLimit_IntegerParsingTextBox.Text = "20"
        '
        'GenerateSnrRangeStimuli_LowerLimit_IntegerParsingTextBox
        '
        Me.GenerateSnrRangeStimuli_LowerLimit_IntegerParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GenerateSnrRangeStimuli_LowerLimit_IntegerParsingTextBox.ForeColor = System.Drawing.SystemColors.WindowText
        Me.GenerateSnrRangeStimuli_LowerLimit_IntegerParsingTextBox.Location = New System.Drawing.Point(265, 256)
        Me.GenerateSnrRangeStimuli_LowerLimit_IntegerParsingTextBox.Name = "GenerateSnrRangeStimuli_LowerLimit_IntegerParsingTextBox"
        Me.GenerateSnrRangeStimuli_LowerLimit_IntegerParsingTextBox.Size = New System.Drawing.Size(257, 20)
        Me.GenerateSnrRangeStimuli_LowerLimit_IntegerParsingTextBox.TabIndex = 10
        Me.GenerateSnrRangeStimuli_LowerLimit_IntegerParsingTextBox.Text = "-20"
        '
        'GenerateSnrRangeStimuli_StepSize_DoubleParsingTextBox
        '
        Me.GenerateSnrRangeStimuli_StepSize_DoubleParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GenerateSnrRangeStimuli_StepSize_DoubleParsingTextBox.ForeColor = System.Drawing.SystemColors.WindowText
        Me.GenerateSnrRangeStimuli_StepSize_DoubleParsingTextBox.Location = New System.Drawing.Point(265, 282)
        Me.GenerateSnrRangeStimuli_StepSize_DoubleParsingTextBox.Name = "GenerateSnrRangeStimuli_StepSize_DoubleParsingTextBox"
        Me.GenerateSnrRangeStimuli_StepSize_DoubleParsingTextBox.Size = New System.Drawing.Size(257, 20)
        Me.GenerateSnrRangeStimuli_StepSize_DoubleParsingTextBox.TabIndex = 11
        Me.GenerateSnrRangeStimuli_StepSize_DoubleParsingTextBox.Text = "1"
        '
        'GenerateSnrRangeStimuli_PresentationLevel_DoubleParsingTextBox
        '
        Me.GenerateSnrRangeStimuli_PresentationLevel_DoubleParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GenerateSnrRangeStimuli_PresentationLevel_DoubleParsingTextBox.ForeColor = System.Drawing.SystemColors.WindowText
        Me.GenerateSnrRangeStimuli_PresentationLevel_DoubleParsingTextBox.Location = New System.Drawing.Point(265, 386)
        Me.GenerateSnrRangeStimuli_PresentationLevel_DoubleParsingTextBox.Name = "GenerateSnrRangeStimuli_PresentationLevel_DoubleParsingTextBox"
        Me.GenerateSnrRangeStimuli_PresentationLevel_DoubleParsingTextBox.Size = New System.Drawing.Size(257, 20)
        Me.GenerateSnrRangeStimuli_PresentationLevel_DoubleParsingTextBox.TabIndex = 12
        Me.GenerateSnrRangeStimuli_PresentationLevel_DoubleParsingTextBox.Text = "65"
        '
        'GenerateSnrRangeStimuli_NoiseType_ComboBox
        '
        Me.GenerateSnrRangeStimuli_NoiseType_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GenerateSnrRangeStimuli_NoiseType_ComboBox.FormattingEnabled = True
        Me.GenerateSnrRangeStimuli_NoiseType_ComboBox.Location = New System.Drawing.Point(265, 308)
        Me.GenerateSnrRangeStimuli_NoiseType_ComboBox.Name = "GenerateSnrRangeStimuli_NoiseType_ComboBox"
        Me.GenerateSnrRangeStimuli_NoiseType_ComboBox.Size = New System.Drawing.Size(257, 21)
        Me.GenerateSnrRangeStimuli_NoiseType_ComboBox.TabIndex = 13
        '
        'GenerateSnrRangeStimuli_Button
        '
        Me.TableLayoutPanel10.SetColumnSpan(Me.GenerateSnrRangeStimuli_Button, 2)
        Me.GenerateSnrRangeStimuli_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GenerateSnrRangeStimuli_Button.Location = New System.Drawing.Point(3, 464)
        Me.GenerateSnrRangeStimuli_Button.Name = "GenerateSnrRangeStimuli_Button"
        Me.GenerateSnrRangeStimuli_Button.Size = New System.Drawing.Size(519, 29)
        Me.GenerateSnrRangeStimuli_Button.TabIndex = 14
        Me.GenerateSnrRangeStimuli_Button.Text = "Create sound files"
        Me.GenerateSnrRangeStimuli_Button.UseVisualStyleBackColor = True
        '
        'GenerateSnrRangeStimuli_NoiseFW_ComboBox
        '
        Me.GenerateSnrRangeStimuli_NoiseFW_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GenerateSnrRangeStimuli_NoiseFW_ComboBox.FormattingEnabled = True
        Me.GenerateSnrRangeStimuli_NoiseFW_ComboBox.Location = New System.Drawing.Point(265, 334)
        Me.GenerateSnrRangeStimuli_NoiseFW_ComboBox.Name = "GenerateSnrRangeStimuli_NoiseFW_ComboBox"
        Me.GenerateSnrRangeStimuli_NoiseFW_ComboBox.Size = New System.Drawing.Size(257, 21)
        Me.GenerateSnrRangeStimuli_NoiseFW_ComboBox.TabIndex = 15
        '
        'Label48
        '
        Me.Label48.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label48.Location = New System.Drawing.Point(3, 331)
        Me.Label48.Name = "Label48"
        Me.Label48.Size = New System.Drawing.Size(256, 26)
        Me.Label48.TabIndex = 16
        Me.Label48.Text = "Noise level frequency weighting"
        Me.Label48.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label49
        '
        Me.Label49.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label49.Location = New System.Drawing.Point(3, 409)
        Me.Label49.Name = "Label49"
        Me.Label49.Size = New System.Drawing.Size(256, 26)
        Me.Label49.TabIndex = 17
        Me.Label49.Text = "Post-processing output gain (dB)"
        Me.Label49.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'GenerateSnrRangeStimuli_OutputGain_DoubleParsingTextBox
        '
        Me.GenerateSnrRangeStimuli_OutputGain_DoubleParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GenerateSnrRangeStimuli_OutputGain_DoubleParsingTextBox.ForeColor = System.Drawing.SystemColors.WindowText
        Me.GenerateSnrRangeStimuli_OutputGain_DoubleParsingTextBox.Location = New System.Drawing.Point(265, 412)
        Me.GenerateSnrRangeStimuli_OutputGain_DoubleParsingTextBox.Name = "GenerateSnrRangeStimuli_OutputGain_DoubleParsingTextBox"
        Me.GenerateSnrRangeStimuli_OutputGain_DoubleParsingTextBox.Size = New System.Drawing.Size(257, 20)
        Me.GenerateSnrRangeStimuli_OutputGain_DoubleParsingTextBox.TabIndex = 18
        Me.GenerateSnrRangeStimuli_OutputGain_DoubleParsingTextBox.Text = "0"
        '
        'Label50
        '
        Me.Label50.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label50.Location = New System.Drawing.Point(3, 357)
        Me.Label50.Name = "Label50"
        Me.Label50.Size = New System.Drawing.Size(256, 26)
        Me.Label50.TabIndex = 19
        Me.Label50.Text = "Number of overlays (for generation of SW noise)"
        Me.Label50.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'GenerateSnrRangeStimuli_Overlays_IntegerParsingTextBox
        '
        Me.GenerateSnrRangeStimuli_Overlays_IntegerParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GenerateSnrRangeStimuli_Overlays_IntegerParsingTextBox.ForeColor = System.Drawing.SystemColors.WindowText
        Me.GenerateSnrRangeStimuli_Overlays_IntegerParsingTextBox.Location = New System.Drawing.Point(265, 360)
        Me.GenerateSnrRangeStimuli_Overlays_IntegerParsingTextBox.Name = "GenerateSnrRangeStimuli_Overlays_IntegerParsingTextBox"
        Me.GenerateSnrRangeStimuli_Overlays_IntegerParsingTextBox.Size = New System.Drawing.Size(257, 20)
        Me.GenerateSnrRangeStimuli_Overlays_IntegerParsingTextBox.TabIndex = 20
        Me.GenerateSnrRangeStimuli_Overlays_IntegerParsingTextBox.Text = "1000"
        '
        'GenerateSnrRangeStimuli_ConcatenatedSound_CheckBox
        '
        Me.GenerateSnrRangeStimuli_ConcatenatedSound_CheckBox.AutoSize = True
        Me.GenerateSnrRangeStimuli_ConcatenatedSound_CheckBox.Checked = True
        Me.GenerateSnrRangeStimuli_ConcatenatedSound_CheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.GenerateSnrRangeStimuli_ConcatenatedSound_CheckBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GenerateSnrRangeStimuli_ConcatenatedSound_CheckBox.Location = New System.Drawing.Point(3, 438)
        Me.GenerateSnrRangeStimuli_ConcatenatedSound_CheckBox.Name = "GenerateSnrRangeStimuli_ConcatenatedSound_CheckBox"
        Me.GenerateSnrRangeStimuli_ConcatenatedSound_CheckBox.Size = New System.Drawing.Size(256, 20)
        Me.GenerateSnrRangeStimuli_ConcatenatedSound_CheckBox.TabIndex = 21
        Me.GenerateSnrRangeStimuli_ConcatenatedSound_CheckBox.Text = "Also export concatenated sounds"
        Me.GenerateSnrRangeStimuli_ConcatenatedSound_CheckBox.UseVisualStyleBackColor = True
        '
        'GenerateSnrRangeStimuli_InsertConcatenationSilence_CheckBox
        '
        Me.GenerateSnrRangeStimuli_InsertConcatenationSilence_CheckBox.AutoSize = True
        Me.GenerateSnrRangeStimuli_InsertConcatenationSilence_CheckBox.Checked = True
        Me.GenerateSnrRangeStimuli_InsertConcatenationSilence_CheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.GenerateSnrRangeStimuli_InsertConcatenationSilence_CheckBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GenerateSnrRangeStimuli_InsertConcatenationSilence_CheckBox.Location = New System.Drawing.Point(265, 438)
        Me.GenerateSnrRangeStimuli_InsertConcatenationSilence_CheckBox.Name = "GenerateSnrRangeStimuli_InsertConcatenationSilence_CheckBox"
        Me.GenerateSnrRangeStimuli_InsertConcatenationSilence_CheckBox.Size = New System.Drawing.Size(257, 20)
        Me.GenerateSnrRangeStimuli_InsertConcatenationSilence_CheckBox.TabIndex = 22
        Me.GenerateSnrRangeStimuli_InsertConcatenationSilence_CheckBox.Text = "Insert silence (1 sec) between concatenated sounds"
        Me.GenerateSnrRangeStimuli_InsertConcatenationSilence_CheckBox.UseVisualStyleBackColor = True
        '
        'ListMixer_TabPage
        '
        Me.ListMixer_TabPage.BackColor = System.Drawing.Color.Firebrick
        Me.ListMixer_TabPage.Controls.Add(Me.ListRearrangerControl1)
        Me.ListMixer_TabPage.Location = New System.Drawing.Point(4, 22)
        Me.ListMixer_TabPage.Name = "ListMixer_TabPage"
        Me.ListMixer_TabPage.Padding = New System.Windows.Forms.Padding(3)
        Me.ListMixer_TabPage.Size = New System.Drawing.Size(562, 611)
        Me.ListMixer_TabPage.TabIndex = 4
        Me.ListMixer_TabPage.Text = "Re-arrange lists"
        '
        'ListRearrangerControl1
        '
        Me.ListRearrangerControl1.AutoScroll = True
        Me.ListRearrangerControl1.BackColor = System.Drawing.SystemColors.Control
        Me.ListRearrangerControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListRearrangerControl1.Enabled = False
        Me.ListRearrangerControl1.Location = New System.Drawing.Point(3, 3)
        Me.ListRearrangerControl1.Name = "ListRearrangerControl1"
        Me.ListRearrangerControl1.Size = New System.Drawing.Size(556, 605)
        Me.ListRearrangerControl1.TabIndex = 0
        '
        'CreateMaskers_TabPage
        '
        Me.CreateMaskers_TabPage.AutoScroll = True
        Me.CreateMaskers_TabPage.BackColor = System.Drawing.Color.LightPink
        Me.CreateMaskers_TabPage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.CreateMaskers_TabPage.Controls.Add(Me.CreateMaskersControl1)
        Me.CreateMaskers_TabPage.Location = New System.Drawing.Point(4, 22)
        Me.CreateMaskers_TabPage.Name = "CreateMaskers_TabPage"
        Me.CreateMaskers_TabPage.Padding = New System.Windows.Forms.Padding(3)
        Me.CreateMaskers_TabPage.Size = New System.Drawing.Size(562, 611)
        Me.CreateMaskers_TabPage.TabIndex = 2
        Me.CreateMaskers_TabPage.Text = "Create maskers"
        '
        'CreateMaskersControl1
        '
        Me.CreateMaskersControl1.BackColor = System.Drawing.SystemColors.Control
        Me.CreateMaskersControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CreateMaskersControl1.Enabled = False
        Me.CreateMaskersControl1.Location = New System.Drawing.Point(3, 3)
        Me.CreateMaskersControl1.Name = "CreateMaskersControl1"
        Me.CreateMaskersControl1.Size = New System.Drawing.Size(554, 603)
        Me.CreateMaskersControl1.TabIndex = 0
        '
        'SoundfileLinguisticLevels_TabPage
        '
        Me.SoundfileLinguisticLevels_TabPage.AutoScroll = True
        Me.SoundfileLinguisticLevels_TabPage.BackColor = System.Drawing.Color.Gold
        Me.SoundfileLinguisticLevels_TabPage.Controls.Add(Me.TableLayoutPanel8)
        Me.SoundfileLinguisticLevels_TabPage.Location = New System.Drawing.Point(4, 22)
        Me.SoundfileLinguisticLevels_TabPage.Name = "SoundfileLinguisticLevels_TabPage"
        Me.SoundfileLinguisticLevels_TabPage.Padding = New System.Windows.Forms.Padding(3)
        Me.SoundfileLinguisticLevels_TabPage.Size = New System.Drawing.Size(562, 611)
        Me.SoundfileLinguisticLevels_TabPage.TabIndex = 3
        Me.SoundfileLinguisticLevels_TabPage.Text = "Sound file linguistic levels"
        '
        'TableLayoutPanel8
        '
        Me.TableLayoutPanel8.AutoSize = True
        Me.TableLayoutPanel8.BackColor = System.Drawing.SystemColors.Control
        Me.TableLayoutPanel8.ColumnCount = 2
        Me.TableLayoutPanel8.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 59.60591!))
        Me.TableLayoutPanel8.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40.39409!))
        Me.TableLayoutPanel8.Controls.Add(Me.RandomizeOrder_CheckBox, 1, 9)
        Me.TableLayoutPanel8.Controls.Add(Me.NewSoundFile_InterStimulusInterval_IntegerParsingTextBox, 1, 5)
        Me.TableLayoutPanel8.Controls.Add(Me.Label36, 0, 4)
        Me.TableLayoutPanel8.Controls.Add(Me.RichTextBox1, 0, 0)
        Me.TableLayoutPanel8.Controls.Add(Me.Label33, 0, 1)
        Me.TableLayoutPanel8.Controls.Add(Me.Label34, 0, 2)
        Me.TableLayoutPanel8.Controls.Add(Me.Label35, 0, 3)
        Me.TableLayoutPanel8.Controls.Add(Me.ModifiedMediaSetName_TextBox, 1, 3)
        Me.TableLayoutPanel8.Controls.Add(Me.CurrentMediaSetLinguisticLevel_TextBox, 1, 1)
        Me.TableLayoutPanel8.Controls.Add(Me.ModifiedMediaSetLinguisticLevel_ComboBox, 1, 2)
        Me.TableLayoutPanel8.Controls.Add(Me.Label37, 0, 5)
        Me.TableLayoutPanel8.Controls.Add(Me.NewSoundFilePadding_IntegerParsingTextBox, 1, 4)
        Me.TableLayoutPanel8.Controls.Add(Me.CreateModifiedMediaSet_Button, 0, 11)
        Me.TableLayoutPanel8.Controls.Add(Me.IncludePractiseItems_CheckBox, 1, 8)
        Me.TableLayoutPanel8.Controls.Add(Me.IncludeTestItems_CheckBox, 1, 7)
        Me.TableLayoutPanel8.Controls.Add(Me.Label38, 0, 6)
        Me.TableLayoutPanel8.Controls.Add(Me.CrossfadeDuration_IntegerParsingTextBox, 1, 6)
        Me.TableLayoutPanel8.Controls.Add(Me.Label39, 0, 10)
        Me.TableLayoutPanel8.Controls.Add(Me.RandomSeed_IntegerParsingTextBox, 1, 10)
        Me.TableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Top
        Me.TableLayoutPanel8.Location = New System.Drawing.Point(3, 3)
        Me.TableLayoutPanel8.Name = "TableLayoutPanel8"
        Me.TableLayoutPanel8.RowCount = 13
        Me.TableLayoutPanel8.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80.0!))
        Me.TableLayoutPanel8.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel8.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel8.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel8.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel8.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel8.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel8.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel8.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel8.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel8.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel8.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35.0!))
        Me.TableLayoutPanel8.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel8.Size = New System.Drawing.Size(556, 365)
        Me.TableLayoutPanel8.TabIndex = 0
        '
        'RandomizeOrder_CheckBox
        '
        Me.RandomizeOrder_CheckBox.AutoSize = True
        Me.RandomizeOrder_CheckBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RandomizeOrder_CheckBox.Location = New System.Drawing.Point(334, 283)
        Me.RandomizeOrder_CheckBox.Name = "RandomizeOrder_CheckBox"
        Me.RandomizeOrder_CheckBox.Size = New System.Drawing.Size(219, 19)
        Me.RandomizeOrder_CheckBox.TabIndex = 17
        Me.RandomizeOrder_CheckBox.Text = "Randomize order"
        Me.RandomizeOrder_CheckBox.UseVisualStyleBackColor = True
        '
        'NewSoundFile_InterStimulusInterval_IntegerParsingTextBox
        '
        Me.NewSoundFile_InterStimulusInterval_IntegerParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.NewSoundFile_InterStimulusInterval_IntegerParsingTextBox.ForeColor = System.Drawing.SystemColors.WindowText
        Me.NewSoundFile_InterStimulusInterval_IntegerParsingTextBox.Location = New System.Drawing.Point(334, 183)
        Me.NewSoundFile_InterStimulusInterval_IntegerParsingTextBox.Name = "NewSoundFile_InterStimulusInterval_IntegerParsingTextBox"
        Me.NewSoundFile_InterStimulusInterval_IntegerParsingTextBox.Size = New System.Drawing.Size(219, 20)
        Me.NewSoundFile_InterStimulusInterval_IntegerParsingTextBox.TabIndex = 10
        Me.NewSoundFile_InterStimulusInterval_IntegerParsingTextBox.Text = "4000"
        Me.NewSoundFile_InterStimulusInterval_IntegerParsingTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label36
        '
        Me.Label36.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label36.Location = New System.Drawing.Point(3, 155)
        Me.Label36.Name = "Label36"
        Me.Label36.Size = New System.Drawing.Size(325, 25)
        Me.Label36.TabIndex = 7
        Me.Label36.Text = "New sound file padding (ms)"
        Me.Label36.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'RichTextBox1
        '
        Me.TableLayoutPanel8.SetColumnSpan(Me.RichTextBox1, 2)
        Me.RichTextBox1.Cursor = System.Windows.Forms.Cursors.Default
        Me.RichTextBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RichTextBox1.Location = New System.Drawing.Point(3, 3)
        Me.RichTextBox1.Name = "RichTextBox1"
        Me.RichTextBox1.ReadOnly = True
        Me.RichTextBox1.Size = New System.Drawing.Size(550, 74)
        Me.RichTextBox1.TabIndex = 0
        Me.RichTextBox1.Text = resources.GetString("RichTextBox1.Text")
        '
        'Label33
        '
        Me.Label33.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label33.Location = New System.Drawing.Point(3, 80)
        Me.Label33.Name = "Label33"
        Me.Label33.Size = New System.Drawing.Size(325, 25)
        Me.Label33.TabIndex = 1
        Me.Label33.Text = "Current linguistic level of sound files"
        Me.Label33.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label34
        '
        Me.Label34.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label34.Location = New System.Drawing.Point(3, 105)
        Me.Label34.Name = "Label34"
        Me.Label34.Size = New System.Drawing.Size(325, 25)
        Me.Label34.TabIndex = 2
        Me.Label34.Text = "New linguistic level of sound files"
        Me.Label34.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label35
        '
        Me.Label35.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label35.Location = New System.Drawing.Point(3, 130)
        Me.Label35.Name = "Label35"
        Me.Label35.Size = New System.Drawing.Size(325, 25)
        Me.Label35.TabIndex = 3
        Me.Label35.Text = "Name of new media set"
        Me.Label35.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ModifiedMediaSetName_TextBox
        '
        Me.ModifiedMediaSetName_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ModifiedMediaSetName_TextBox.Location = New System.Drawing.Point(334, 133)
        Me.ModifiedMediaSetName_TextBox.Name = "ModifiedMediaSetName_TextBox"
        Me.ModifiedMediaSetName_TextBox.Size = New System.Drawing.Size(219, 20)
        Me.ModifiedMediaSetName_TextBox.TabIndex = 4
        '
        'CurrentMediaSetLinguisticLevel_TextBox
        '
        Me.CurrentMediaSetLinguisticLevel_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CurrentMediaSetLinguisticLevel_TextBox.Location = New System.Drawing.Point(334, 83)
        Me.CurrentMediaSetLinguisticLevel_TextBox.Name = "CurrentMediaSetLinguisticLevel_TextBox"
        Me.CurrentMediaSetLinguisticLevel_TextBox.ReadOnly = True
        Me.CurrentMediaSetLinguisticLevel_TextBox.Size = New System.Drawing.Size(219, 20)
        Me.CurrentMediaSetLinguisticLevel_TextBox.TabIndex = 5
        '
        'ModifiedMediaSetLinguisticLevel_ComboBox
        '
        Me.ModifiedMediaSetLinguisticLevel_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ModifiedMediaSetLinguisticLevel_ComboBox.FormattingEnabled = True
        Me.ModifiedMediaSetLinguisticLevel_ComboBox.Location = New System.Drawing.Point(334, 108)
        Me.ModifiedMediaSetLinguisticLevel_ComboBox.Name = "ModifiedMediaSetLinguisticLevel_ComboBox"
        Me.ModifiedMediaSetLinguisticLevel_ComboBox.Size = New System.Drawing.Size(219, 21)
        Me.ModifiedMediaSetLinguisticLevel_ComboBox.TabIndex = 6
        '
        'Label37
        '
        Me.Label37.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label37.Location = New System.Drawing.Point(3, 180)
        Me.Label37.Name = "Label37"
        Me.Label37.Size = New System.Drawing.Size(325, 25)
        Me.Label37.TabIndex = 8
        Me.Label37.Text = "New sound file inter-stimulus interval (ms)"
        Me.Label37.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'NewSoundFilePadding_IntegerParsingTextBox
        '
        Me.NewSoundFilePadding_IntegerParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.NewSoundFilePadding_IntegerParsingTextBox.ForeColor = System.Drawing.SystemColors.WindowText
        Me.NewSoundFilePadding_IntegerParsingTextBox.Location = New System.Drawing.Point(334, 158)
        Me.NewSoundFilePadding_IntegerParsingTextBox.Name = "NewSoundFilePadding_IntegerParsingTextBox"
        Me.NewSoundFilePadding_IntegerParsingTextBox.Size = New System.Drawing.Size(219, 20)
        Me.NewSoundFilePadding_IntegerParsingTextBox.TabIndex = 9
        Me.NewSoundFilePadding_IntegerParsingTextBox.Text = "0"
        Me.NewSoundFilePadding_IntegerParsingTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'CreateModifiedMediaSet_Button
        '
        Me.TableLayoutPanel8.SetColumnSpan(Me.CreateModifiedMediaSet_Button, 2)
        Me.CreateModifiedMediaSet_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CreateModifiedMediaSet_Button.Location = New System.Drawing.Point(3, 333)
        Me.CreateModifiedMediaSet_Button.Name = "CreateModifiedMediaSet_Button"
        Me.CreateModifiedMediaSet_Button.Size = New System.Drawing.Size(550, 29)
        Me.CreateModifiedMediaSet_Button.TabIndex = 11
        Me.CreateModifiedMediaSet_Button.Text = "Create modified media set"
        Me.CreateModifiedMediaSet_Button.UseVisualStyleBackColor = True
        '
        'IncludePractiseItems_CheckBox
        '
        Me.IncludePractiseItems_CheckBox.AutoSize = True
        Me.IncludePractiseItems_CheckBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.IncludePractiseItems_CheckBox.Location = New System.Drawing.Point(334, 258)
        Me.IncludePractiseItems_CheckBox.Name = "IncludePractiseItems_CheckBox"
        Me.IncludePractiseItems_CheckBox.Size = New System.Drawing.Size(219, 19)
        Me.IncludePractiseItems_CheckBox.TabIndex = 16
        Me.IncludePractiseItems_CheckBox.Text = "Include practise items"
        Me.IncludePractiseItems_CheckBox.UseVisualStyleBackColor = True
        '
        'IncludeTestItems_CheckBox
        '
        Me.IncludeTestItems_CheckBox.AutoSize = True
        Me.IncludeTestItems_CheckBox.Checked = True
        Me.IncludeTestItems_CheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.IncludeTestItems_CheckBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.IncludeTestItems_CheckBox.Location = New System.Drawing.Point(334, 233)
        Me.IncludeTestItems_CheckBox.Name = "IncludeTestItems_CheckBox"
        Me.IncludeTestItems_CheckBox.Size = New System.Drawing.Size(219, 19)
        Me.IncludeTestItems_CheckBox.TabIndex = 15
        Me.IncludeTestItems_CheckBox.Text = "Include test items"
        Me.IncludeTestItems_CheckBox.UseVisualStyleBackColor = True
        '
        'Label38
        '
        Me.Label38.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label38.Location = New System.Drawing.Point(3, 205)
        Me.Label38.Name = "Label38"
        Me.Label38.Size = New System.Drawing.Size(325, 25)
        Me.Label38.TabIndex = 18
        Me.Label38.Text = "Crossfade duration (ms)"
        Me.Label38.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CrossfadeDuration_IntegerParsingTextBox
        '
        Me.CrossfadeDuration_IntegerParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CrossfadeDuration_IntegerParsingTextBox.ForeColor = System.Drawing.SystemColors.WindowText
        Me.CrossfadeDuration_IntegerParsingTextBox.Location = New System.Drawing.Point(334, 208)
        Me.CrossfadeDuration_IntegerParsingTextBox.Name = "CrossfadeDuration_IntegerParsingTextBox"
        Me.CrossfadeDuration_IntegerParsingTextBox.Size = New System.Drawing.Size(219, 20)
        Me.CrossfadeDuration_IntegerParsingTextBox.TabIndex = 19
        Me.CrossfadeDuration_IntegerParsingTextBox.Text = "0"
        Me.CrossfadeDuration_IntegerParsingTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label39
        '
        Me.Label39.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label39.Location = New System.Drawing.Point(3, 305)
        Me.Label39.Name = "Label39"
        Me.Label39.Size = New System.Drawing.Size(325, 25)
        Me.Label39.TabIndex = 20
        Me.Label39.Text = "(Optional) Random seed, to re-use same random order (can be empty)"
        Me.Label39.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'RandomSeed_IntegerParsingTextBox
        '
        Me.RandomSeed_IntegerParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RandomSeed_IntegerParsingTextBox.ForeColor = System.Drawing.Color.Red
        Me.RandomSeed_IntegerParsingTextBox.Location = New System.Drawing.Point(334, 308)
        Me.RandomSeed_IntegerParsingTextBox.Name = "RandomSeed_IntegerParsingTextBox"
        Me.RandomSeed_IntegerParsingTextBox.Size = New System.Drawing.Size(219, 20)
        Me.RandomSeed_IntegerParsingTextBox.TabIndex = 21
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.TableLayoutPanel4)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(562, 611)
        Me.TabPage1.TabIndex = 5
        Me.TabPage1.Text = "Temp page"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel4
        '
        Me.TableLayoutPanel4.AutoSize = True
        Me.TableLayoutPanel4.BackColor = System.Drawing.SystemColors.Control
        Me.TableLayoutPanel4.ColumnCount = 2
        Me.TableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel4.Controls.Add(Me.CreateSipTestMaskersButton, 0, 0)
        Me.TableLayoutPanel4.Controls.Add(Me.CalculateSipTestMaskerSpectrumLevels_Button, 1, 0)
        Me.TableLayoutPanel4.Controls.Add(Me.TempButton, 0, 1)
        Me.TableLayoutPanel4.Controls.Add(Me.ExportSpelledSoundFiles_Button, 1, 1)
        Me.TableLayoutPanel4.Location = New System.Drawing.Point(5, 51)
        Me.TableLayoutPanel4.Name = "TableLayoutPanel4"
        Me.TableLayoutPanel4.RowCount = 2
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel4.Size = New System.Drawing.Size(551, 297)
        Me.TableLayoutPanel4.TabIndex = 1
        '
        'CreateSipTestMaskersButton
        '
        Me.CreateSipTestMaskersButton.Location = New System.Drawing.Point(3, 3)
        Me.CreateSipTestMaskersButton.Name = "CreateSipTestMaskersButton"
        Me.CreateSipTestMaskersButton.Size = New System.Drawing.Size(180, 23)
        Me.CreateSipTestMaskersButton.TabIndex = 0
        Me.CreateSipTestMaskersButton.Text = "Create SiP-test masker sounds"
        Me.CreateSipTestMaskersButton.UseVisualStyleBackColor = True
        '
        'CalculateSipTestMaskerSpectrumLevels_Button
        '
        Me.CalculateSipTestMaskerSpectrumLevels_Button.Location = New System.Drawing.Point(278, 3)
        Me.CalculateSipTestMaskerSpectrumLevels_Button.Name = "CalculateSipTestMaskerSpectrumLevels_Button"
        Me.CalculateSipTestMaskerSpectrumLevels_Button.Size = New System.Drawing.Size(184, 44)
        Me.CalculateSipTestMaskerSpectrumLevels_Button.TabIndex = 1
        Me.CalculateSipTestMaskerSpectrumLevels_Button.Text = "Calculate SiP-test masker spectrum levels (SLm)"
        Me.CalculateSipTestMaskerSpectrumLevels_Button.UseVisualStyleBackColor = True
        '
        'TempButton
        '
        Me.TempButton.Location = New System.Drawing.Point(3, 151)
        Me.TempButton.Name = "TempButton"
        Me.TempButton.Size = New System.Drawing.Size(153, 23)
        Me.TempButton.TabIndex = 2
        Me.TempButton.Text = "Temporary Button"
        Me.TempButton.UseVisualStyleBackColor = True
        '
        'ExportSpelledSoundFiles_Button
        '
        Me.ExportSpelledSoundFiles_Button.Location = New System.Drawing.Point(278, 151)
        Me.ExportSpelledSoundFiles_Button.Name = "ExportSpelledSoundFiles_Button"
        Me.ExportSpelledSoundFiles_Button.Size = New System.Drawing.Size(234, 49)
        Me.ExportSpelledSoundFiles_Button.TabIndex = 3
        Me.ExportSpelledSoundFiles_Button.Text = "Export sound files with spelling as names"
        Me.ExportSpelledSoundFiles_Button.UseVisualStyleBackColor = True
        '
        'MediaSetSetupControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.TableLayoutPanel2)
        Me.Name = "MediaSetSetupControl"
        Me.Size = New System.Drawing.Size(1314, 839)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.TableLayoutPanel2.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.PerformLayout()
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.EditSpecification_TableLayoutPanel.ResumeLayout(False)
        Me.EditSpecification_TableLayoutPanel.PerformLayout()
        Me.EditSoundFile_TabControl.ResumeLayout(False)
        Me.StartRecorder_TabPage.ResumeLayout(False)
        Me.StartRecorder_TabPage.PerformLayout()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.SpeechLevels_TabPage.ResumeLayout(False)
        Me.SpeechLevels_TabPage.PerformLayout()
        Me.TableLayoutPanel5.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.TableLayoutPanel3.ResumeLayout(False)
        Me.TableLayoutPanel3.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.TableLayoutPanel6.ResumeLayout(False)
        Me.TableLayoutPanel6.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.TableLayoutPanel7.ResumeLayout(False)
        Me.GroupBox4.ResumeLayout(False)
        Me.TableLayoutPanel9.ResumeLayout(False)
        Me.TableLayoutPanel9.PerformLayout()
        Me.GroupBox5.ResumeLayout(False)
        Me.TableLayoutPanel10.ResumeLayout(False)
        Me.TableLayoutPanel10.PerformLayout()
        Me.ListMixer_TabPage.ResumeLayout(False)
        Me.CreateMaskers_TabPage.ResumeLayout(False)
        Me.SoundfileLinguisticLevels_TabPage.ResumeLayout(False)
        Me.SoundfileLinguisticLevels_TabPage.PerformLayout()
        Me.TableLayoutPanel8.ResumeLayout(False)
        Me.TableLayoutPanel8.PerformLayout()
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        Me.TableLayoutPanel4.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TableLayoutPanel2 As Windows.Forms.TableLayoutPanel
    Friend WithEvents EditSpecification_TableLayoutPanel As Windows.Forms.TableLayoutPanel
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
    Friend WithEvents TableLayoutPanel1 As Windows.Forms.TableLayoutPanel
    Friend WithEvents LaunchRecorder_Button As Windows.Forms.Button
    Friend WithEvents RandomOrder_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents SpecificPrototypeRecording_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents NoPrototypeRecording_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents MasterPrototypeRecording_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents Label26 As Windows.Forms.Label
    Friend WithEvents AudioFileLinguisticLevelComboBox As Windows.Forms.ComboBox
    Friend WithEvents EditSoundFile_TabControl As Windows.Forms.TabControl
    Friend WithEvents StartRecorder_TabPage As Windows.Forms.TabPage
    Friend WithEvents SpeechLevels_TabPage As Windows.Forms.TabPage
    Friend WithEvents CreateMaskers_TabPage As Windows.Forms.TabPage
    Friend WithEvents TableLayoutPanel3 As Windows.Forms.TableLayoutPanel
    Friend WithEvents ApplySpeechLevels_Button As Windows.Forms.Button
    Friend WithEvents SpeechLevelSPL_Label As Windows.Forms.Label
    Friend WithEvents Label28 As Windows.Forms.Label
    Friend WithEvents SpeechLevelTemporalIntegration_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents VpNormalization_Checkbox As Windows.Forms.CheckBox
    Friend WithEvents SpeechLevelSPL_DoubleParsingTextBox As DoubleParsingTextBox
    Friend WithEvents SpeechLevelFrequencyWeighting_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents SpeechLevelTemporalIntegration_DoubleParsingTextBox As DoubleParsingTextBox
    Friend WithEvents SpeechLevelFS_DoubleParsingTextBox As DoubleParsingTextBox
    Friend WithEvents Label29 As Windows.Forms.Label
    Friend WithEvents MeasureSmaLevels_Button As Windows.Forms.Button
    Friend WithEvents TableLayoutPanel5 As Windows.Forms.TableLayoutPanel
    Friend WithEvents GroupBox1 As Windows.Forms.GroupBox
    Friend WithEvents GroupBox2 As Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel6 As Windows.Forms.TableLayoutPanel
    Friend WithEvents SmaTemporalIntegration_DoubleParsingTextBox As DoubleParsingTextBox
    Friend WithEvents Label27 As Windows.Forms.Label
    Friend WithEvents SmaFrequencyWeighting_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents SmaTemporalIntegration_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents IncludeCriticalBandLevels_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents GroupBox3 As Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel7 As Windows.Forms.TableLayoutPanel
    Friend WithEvents SipTestLevels_Button As Windows.Forms.Button
    Friend WithEvents SharedMaskersLevelComboBox As Windows.Forms.ComboBox
    Friend WithEvents Label30 As Windows.Forms.Label
    Friend WithEvents CustomVariablesFolder_TextBox As Windows.Forms.TextBox
    Friend WithEvents Label31 As Windows.Forms.Label
    Friend WithEvents SoundfileLinguisticLevels_TabPage As Windows.Forms.TabPage
    Friend WithEvents TableLayoutPanel8 As Windows.Forms.TableLayoutPanel
    Friend WithEvents RichTextBox1 As Windows.Forms.RichTextBox
    Friend WithEvents Label33 As Windows.Forms.Label
    Friend WithEvents Label34 As Windows.Forms.Label
    Friend WithEvents Label35 As Windows.Forms.Label
    Friend WithEvents ModifiedMediaSetName_TextBox As Windows.Forms.TextBox
    Friend WithEvents CurrentMediaSetLinguisticLevel_TextBox As Windows.Forms.TextBox
    Friend WithEvents ModifiedMediaSetLinguisticLevel_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents NewSoundFile_InterStimulusInterval_IntegerParsingTextBox As IntegerParsingTextBox
    Friend WithEvents Label36 As Windows.Forms.Label
    Friend WithEvents Label37 As Windows.Forms.Label
    Friend WithEvents NewSoundFilePadding_IntegerParsingTextBox As IntegerParsingTextBox
    Friend WithEvents CreateModifiedMediaSet_Button As Windows.Forms.Button
    Friend WithEvents RandomizeOrder_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents IncludeTestItems_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents IncludePractiseItems_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents Label38 As Windows.Forms.Label
    Friend WithEvents CrossfadeDuration_IntegerParsingTextBox As IntegerParsingTextBox
    Friend WithEvents Label39 As Windows.Forms.Label
    Friend WithEvents RandomSeed_IntegerParsingTextBox As IntegerParsingTextBox
    Friend WithEvents Label40 As Windows.Forms.Label
    Friend WithEvents SpeechLevel_TargetLinguisticlevel_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents NominalLevel_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents CreateCalibrationSignal_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents OnlyNominalLevel_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents GroupBox4 As Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel9 As Windows.Forms.TableLayoutPanel
    Friend WithEvents CustomSpeechGain_TextBox As Windows.Forms.TextBox
    Friend WithEvents Label32 As Windows.Forms.Label
    Friend WithEvents Label41 As Windows.Forms.Label
    Friend WithEvents TextBox2 As Windows.Forms.TextBox
    Friend WithEvents ApplyCustomSpeechGain_Button As Windows.Forms.Button
    Friend WithEvents GroupBox5 As Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel10 As Windows.Forms.TableLayoutPanel
    Friend WithEvents TextBox1 As Windows.Forms.TextBox
    Friend WithEvents Label42 As Windows.Forms.Label
    Friend WithEvents Label43 As Windows.Forms.Label
    Friend WithEvents Label44 As Windows.Forms.Label
    Friend WithEvents Label45 As Windows.Forms.Label
    Friend WithEvents Label46 As Windows.Forms.Label
    Friend WithEvents Label47 As Windows.Forms.Label
    Friend WithEvents GenerateSnrRangeStimuli_UpperLimit_IntegerParsingTextBox As IntegerParsingTextBox
    Friend WithEvents GenerateSnrRangeStimuli_LowerLimit_IntegerParsingTextBox As IntegerParsingTextBox
    Friend WithEvents GenerateSnrRangeStimuli_StepSize_DoubleParsingTextBox As DoubleParsingTextBox
    Friend WithEvents GenerateSnrRangeStimuli_PresentationLevel_DoubleParsingTextBox As DoubleParsingTextBox
    Friend WithEvents GenerateSnrRangeStimuli_NoiseType_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents GenerateSnrRangeStimuli_Button As Windows.Forms.Button
    Friend WithEvents GenerateSnrRangeStimuli_NoiseFW_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents Label48 As Windows.Forms.Label
    Friend WithEvents Label49 As Windows.Forms.Label
    Friend WithEvents GenerateSnrRangeStimuli_OutputGain_DoubleParsingTextBox As DoubleParsingTextBox
    Friend WithEvents Label50 As Windows.Forms.Label
    Friend WithEvents GenerateSnrRangeStimuli_Overlays_IntegerParsingTextBox As IntegerParsingTextBox
    Friend WithEvents GenerateSnrRangeStimuli_ConcatenatedSound_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents GenerateSnrRangeStimuli_InsertConcatenationSilence_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents ListMixer_TabPage As Windows.Forms.TabPage
    Friend WithEvents ListRearrangerControl1 As ListRearrangerControl
    Friend WithEvents TabPage1 As Windows.Forms.TabPage
    Friend WithEvents TableLayoutPanel4 As Windows.Forms.TableLayoutPanel
    Friend WithEvents CreateSipTestMaskersButton As Windows.Forms.Button
    Friend WithEvents CalculateSipTestMaskerSpectrumLevels_Button As Windows.Forms.Button
    Friend WithEvents TempButton As Windows.Forms.Button
    Friend WithEvents CreateMaskersControl1 As CreateMaskersControl
    Friend WithEvents Label51 As Windows.Forms.Label
    Friend WithEvents Label52 As Windows.Forms.Label
    Friend WithEvents Label53 As Windows.Forms.Label
    Friend WithEvents Label54 As Windows.Forms.Label
    Friend WithEvents SharedContralateralMaskersLevel_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents ContralateralMaskerAudioItems_IntegerParsingTextBox As IntegerParsingTextBox
    Friend WithEvents ContralateralMaskerParentFolder_TextBox As Windows.Forms.TextBox
    Friend WithEvents EffectiveContralateralMaskingGain_DoubleParsingTextBox As DoubleParsingTextBox
    Friend WithEvents ExportSpelledSoundFiles_Button As Windows.Forms.Button
End Class
