Public Class MediaSetSetupControl


    Private SelectedTestSpecification As TestSpecification = Nothing

    Private SelectedMediaSet As MediaSet = Nothing


    Private Sub MediaSetSetupControl_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'Adding genders
        TalkerGender_ComboBox.Items.AddRange([Enum].GetNames(GetType(MediaSet.Genders)))

        'Adding sound file linguistic levels
        SoundFileLevelComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.List)
        SoundFileLevelComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.Sentence)
        SoundFileLevelComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.Word)
        SoundFileLevelComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.Phoneme)

        'Adding supported bit depths
        WaveFileBitDepth_ComboBox.Items.Add(16)
        WaveFileBitDepth_ComboBox.Items.Add(32)

        'Adding encodings
        WaveFileEncoding_ComboBox.Items.AddRange([Enum].GetNames(GetType(Audio.Formats.WaveFormat.WaveFormatEncodings)))

        'Adding speech level frequency weightings
        SpeechLevelFrequencyWeighting_ComboBox.Items.Add(Audio.FrequencyWeightings.Z)
        SpeechLevelFrequencyWeighting_ComboBox.Items.Add(Audio.FrequencyWeightings.C)
        SpeechLevelFrequencyWeighting_ComboBox.Items.Add(Audio.FrequencyWeightings.K)
        SpeechLevelFrequencyWeighting_ComboBox.Items.Add(Audio.FrequencyWeightings.RLB)

        'Pre-selecting Z weighting
        SpeechLevelFrequencyWeighting_ComboBox.SelectedIndex = 0

        UpdateControlEnabledStatuses()

    End Sub

    Private Sub LoadOstaTestSpecificationControl1_SpeechTestSpecificationSelected() Handles LoadOstaTestSpecificationControl1.SpeechTestSpecificationSelected

        Try

            LoadOstaTestSpecificationControl1.SelectedTestSpecification.LoadSpeechMaterialComponentsFile()

            SelectedTestSpecification = LoadOstaTestSpecificationControl1.SelectedTestSpecification

            If SelectedTestSpecification IsNot Nothing Then
                LoadedSpeechMaterialName_TextBox.Text = SelectedTestSpecification.SpeechMaterial.PrimaryStringRepresentation

                'Also referencing the selected test situation in the LoadOstaMediaSetControl1
                LoadOstaMediaSetControl1.SelectedTestSpecification = SelectedTestSpecification

            Else
                LoadedSpeechMaterialName_TextBox.Text = "No speech material loaded"
                MsgBox("Unable to load the speech material file.", MsgBoxStyle.Information, "File reading error")
            End If
        Catch ex As Exception
            MsgBox("The following error occured: " & vbCrLf & vbCrLf & ex.ToString)
        End Try

        UpdateControlEnabledStatuses()

    End Sub


    Private Sub LoadOstaMediaSetControl1_TestSituationSelected() Handles LoadOstaMediaSetControl1.MediaSetSelected

        If LoadOstaMediaSetControl1.SelectedMediaSet IsNot Nothing Then
            SelectedMediaSet = LoadOstaMediaSetControl1.SelectedMediaSet
        End If

        ViewMediaSetData()

        UpdateControlEnabledStatuses()

    End Sub


    Private Sub UpdateControlEnabledStatuses()

        If SelectedTestSpecification IsNot Nothing Then
            LoadOstaMediaSetControl1.Enabled = True
            NewMediaSet_Button.Enabled = True

            If SelectedMediaSet IsNot Nothing Then
                EditSpecification_TableLayoutPanel.Enabled = True
                SaveMediaSetSpecification_Button.Enabled = True
            End If

        Else
            LoadOstaMediaSetControl1.Enabled = False
            NewMediaSet_Button.Enabled = False
            EditSpecification_TableLayoutPanel.Enabled = False
            SaveMediaSetSpecification_Button.Enabled = False
        End If

    End Sub


    Private Sub NewMediaSet_Button_Click(sender As Object, e As EventArgs) Handles NewMediaSet_Button.Click

        Me.SelectedMediaSet = New MediaSet With {.ParentTestSpecification = SelectedTestSpecification}

        ViewMediaSetData()

        UpdateControlEnabledStatuses()

    End Sub


    Private Sub ViewMediaSetData()

        'Exits if no media set is selected
        If Me.SelectedMediaSet Is Nothing Then
            MsgBox("No media set selected!", MsgBoxStyle.Information, "Viewing media set specification data")
            UpdateControlEnabledStatuses()
            Exit Sub
        End If

        MediaSetName_TextBox.Text = SelectedMediaSet.MediaSetName
        TalkerName_TextBox.Text = SelectedMediaSet.TalkerName

        TalkerGender_ComboBox.SelectedItem = SelectedMediaSet.TalkerGender.ToString

        TalkerAge_IntegerParsingTextBox.Text = SelectedMediaSet.TalkerAge
        TalkerDialect_TextBox.Text = SelectedMediaSet.TalkerDialect
        VoiceType_TextBox.Text = SelectedMediaSet.VoiceType
        MediaAudioItems_IntegerParsingTextBox.Text = SelectedMediaSet.MediaAudioItems
        MaskerAudioItems_IntegerParsingTextBox.Text = SelectedMediaSet.MaskerAudioItems
        MediaImageItems_IntegerParsingTextBox.Text = SelectedMediaSet.MediaImageItems
        MaskerImageItems_IntegerParsingTextBox.Text = SelectedMediaSet.MaskerImageItems
        MediaParentFolder_TextBox.Text = SelectedMediaSet.MediaParentFolder
        MaskerParentFolder_TextBox.Text = SelectedMediaSet.MaskerParentFolder
        BackgroundNonspeechParentFolder_TextBox.Text = SelectedMediaSet.BackgroundNonspeechParentFolder
        BackgroundSpeechParentFolder_TextBox.Text = SelectedMediaSet.BackgroundSpeechParentFolder
        PrototypeMediaParentFolder_TextBox.Text = SelectedMediaSet.PrototypeMediaParentFolder
        MasterPrototypeRecordingPath_TextBox.Text = SelectedMediaSet.MasterPrototypeRecordingPath
        PrototypeRecordingLevel_DoubleParsingTextBox.Text = SelectedMediaSet.PrototypeRecordingLevel
        LombardNoisePath_TextBox.Text = SelectedMediaSet.LombardNoisePath
        LombardNoiseLevel_DoubleParsingTextBox.Text = SelectedMediaSet.LombardNoiseLevel
        WaveFileSampleRate_IntegerParsingTextBox.Text = SelectedMediaSet.WaveFileSampleRate
        WaveFileBitDepth_ComboBox.SelectedItem = SelectedMediaSet.WaveFileBitDepth

        WaveFileEncoding_ComboBox.SelectedItem = SelectedMediaSet.WaveFileEncoding.ToString

    End Sub

    Private Sub SaveMediaSetSpecification_Button_Click(sender As Object, e As EventArgs) Handles SaveMediaSetSpecification_Button.Click

        Dim TempMediaSet = New MediaSet

        'Checking and adding values
        If MediaSetName_TextBox.Text.Trim = "" Then
            MsgBox("Supply a media set name")
            Exit Sub
        Else
            TempMediaSet.MediaSetName = MediaSetName_TextBox.Text.Trim
        End If

        If TalkerName_TextBox.Text.Trim = "" Then
            MsgBox("Supply a talker name")
            Exit Sub
        Else
            TempMediaSet.TalkerName = TalkerName_TextBox.Text.Trim
        End If

        If TalkerGender_ComboBox.SelectedItem IsNot Nothing Then
            TempMediaSet.TalkerGender = [Enum].Parse(GetType(MediaSet.Genders), TalkerGender_ComboBox.SelectedItem.ToString)
        End If

        If TalkerAge_IntegerParsingTextBox.Value IsNot Nothing Then
            TempMediaSet.TalkerAge = TalkerAge_IntegerParsingTextBox.Value
        End If

        TempMediaSet.TalkerDialect = TalkerDialect_TextBox.Text.Trim
        TempMediaSet.VoiceType = VoiceType_TextBox.Text.Trim

        'Parsing the info about which linguistic level sound recording should be used
        If WaveFileEncoding_ComboBox.SelectedItem IsNot Nothing Then
            TempMediaSet.AudioItemLinguisticLevel = [Enum].Parse(GetType(SpeechMaterialComponent.LinguisticLevels), SoundFileLevelComboBox.SelectedItem.ToString)
        Else
            MsgBox("You must select a value for 'Linguistic level of sound files'.", MsgBoxStyle.Information, "Checking input data")
            Exit Sub
        End If

        If MediaAudioItems_IntegerParsingTextBox.Value IsNot Nothing Then
            TempMediaSet.MediaAudioItems = MediaAudioItems_IntegerParsingTextBox.Value
        Else
            MsgBox("Supply a value for number of duplicate audio targets")
            Exit Sub
        End If

        If MaskerAudioItems_IntegerParsingTextBox.Value IsNot Nothing Then
            TempMediaSet.MaskerAudioItems = MaskerAudioItems_IntegerParsingTextBox.Value
        Else
            MsgBox("Supply a value for number of duplicate audio maskers")
            Exit Sub
        End If

        If MediaImageItems_IntegerParsingTextBox.Value IsNot Nothing Then
            TempMediaSet.MediaImageItems = MediaImageItems_IntegerParsingTextBox.Value
        Else
            MsgBox("Supply a value for number of duplicate image targets")
            Exit Sub
        End If

        If MaskerImageItems_IntegerParsingTextBox.Value IsNot Nothing Then
            TempMediaSet.MaskerImageItems = MaskerImageItems_IntegerParsingTextBox.Value
        Else
            MsgBox("Supply a value for number of duplicate image maskers")
            Exit Sub
        End If

        TempMediaSet.MediaParentFolder = MediaParentFolder_TextBox.Text.Trim
        If TempMediaSet.MediaAudioItems + TempMediaSet.MediaImageItems = 0 And TempMediaSet.MediaParentFolder = "" Then
            MsgBox("You must supply a subfolder containing target files")
            Exit Sub
        End If

        TempMediaSet.MaskerParentFolder = MaskerParentFolder_TextBox.Text.Trim
        If TempMediaSet.MaskerAudioItems + TempMediaSet.MaskerImageItems = 0 And TempMediaSet.MaskerParentFolder = "" Then
            MsgBox("You must supply a subfolder containing masker files")
            Exit Sub
        End If

        TempMediaSet.BackgroundNonspeechParentFolder = BackgroundNonspeechParentFolder_TextBox.Text.Trim
        If BackgroundNonspeechRealisticLevel_DoubleParsingTextBox.Value IsNot Nothing Then
            TempMediaSet.BackgroundNonspeechRealisticLevel = BackgroundNonspeechRealisticLevel_DoubleParsingTextBox.Value
        End If

        TempMediaSet.BackgroundSpeechParentFolder = BackgroundSpeechParentFolder_TextBox.Text.Trim

        TempMediaSet.PrototypeMediaParentFolder = PrototypeMediaParentFolder_TextBox.Text.Trim
        TempMediaSet.MasterPrototypeRecordingPath = MasterPrototypeRecordingPath_TextBox.Text.Trim

        If PrototypeRecordingLevel_DoubleParsingTextBox.Value IsNot Nothing Then
            TempMediaSet.PrototypeRecordingLevel = PrototypeRecordingLevel_DoubleParsingTextBox.Value
        End If

        TempMediaSet.LombardNoisePath = LombardNoisePath_TextBox.Text.Trim

        If LombardNoiseLevel_DoubleParsingTextBox.Value IsNot Nothing Then
            TempMediaSet.LombardNoiseLevel = LombardNoiseLevel_DoubleParsingTextBox.Value
        End If

        If WaveFileSampleRate_IntegerParsingTextBox.Value IsNot Nothing Then
            TempMediaSet.WaveFileSampleRate = WaveFileSampleRate_IntegerParsingTextBox.Value
        Else
            MsgBox("You must supply a sample rate (48000 is recommended)")
            Exit Sub
        End If

        If WaveFileBitDepth_ComboBox.SelectedItem IsNot Nothing Then
            TempMediaSet.WaveFileBitDepth = WaveFileBitDepth_ComboBox.SelectedItem
        Else
            MsgBox("You must supply a bit depth (32 is recommended)")
            Exit Sub
        End If

        If WaveFileEncoding_ComboBox.SelectedItem IsNot Nothing Then
            TempMediaSet.WaveFileEncoding = [Enum].Parse(GetType(Audio.Formats.WaveFormat.WaveFormatEncodings), WaveFileEncoding_ComboBox.SelectedItem.ToString)
        Else
            MsgBox("You must supply a wave file encoding (IEEE float is recommended)")
            Exit Sub
        End If

        Me.SelectedMediaSet = TempMediaSet

        Me.SelectedMediaSet.ParentTestSpecification = SelectedTestSpecification

        Me.SelectedMediaSet.WriteToFile()

        UpdateControlEnabledStatuses()

    End Sub

    Private Sub LaunchRecorder_Button_Click(sender As Object, e As EventArgs) Handles LaunchRecorder_Button.Click

        'Checking first that needed folders are specified
        If SelectedMediaSet.MediaParentFolder = "" Then
            MsgBox("The subfolder for target media files must be specified before launching the recording and segmentation tool.", MsgBoxStyle.Exclamation, "Launching the recording and segmentation tool")
        End If

        'Here we could also check for other things such as Lombard recordings folder etc, but skipps this for now...

        'Parsing user input settings
        Dim PrototypeRecordingOptions As SpeechTestFramework.MediaSet.PrototypeRecordingOptions
        If MasterPrototypeRecording_RadioButton.Checked = True Then
            PrototypeRecordingOptions = MediaSet.PrototypeRecordingOptions.MasterPrototypeRecording
        ElseIf SpecificPrototypeRecording_RadioButton.Checked = True Then
            PrototypeRecordingOptions = MediaSet.PrototypeRecordingOptions.PrototypeRecordings
        Else
            PrototypeRecordingOptions = MediaSet.PrototypeRecordingOptions.None
        End If


        SelectedMediaSet.RecordAndEditAudioMediaFiles(SpeechTestFramework.MediaSet.SpeechMaterialRecorderLoadOptions.LoadAllSounds, PrototypeRecordingOptions, RandomOrder_CheckBox.Checked)


    End Sub

    Private Sub TemporalIntegration_CheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles TemporalIntegration_CheckBox.CheckedChanged
        TemporalIntegration_DoubleParsingTextBox.Enabled = TemporalIntegration_CheckBox.Checked
    End Sub

    Private Sub ApplySpeechLevels_Button_Click(sender As Object, e As EventArgs) Handles ApplySpeechLevels_Button.Click

        'Checking input data
        If SpeechLevel_DoubleParsingTextBox.Value Is Nothing Then
            MsgBox("You must specify a speech level.", MsgBoxStyle.Information, "Checking input data")
            Exit Sub
        End If

        Dim SpeechLevelFrequencyWeighting As Audio.FrequencyWeightings = Audio.BasicAudioEnums.FrequencyWeightings.Z
        If SpeechLevelFrequencyWeighting_ComboBox.SelectedItem IsNot Nothing Then
            SpeechLevelFrequencyWeighting = [Enum].Parse(GetType(Audio.FrequencyWeightings), SpeechLevelFrequencyWeighting_ComboBox.SelectedItem.ToString)
        Else
            MsgBox("You must specify a speech level frequency weighting.", MsgBoxStyle.Information, "Checking input data")
            Exit Sub
        End If

        Dim TemporalIntegration As Double? = Nothing
        If TemporalIntegration_CheckBox.Checked Then
            If TemporalIntegration_DoubleParsingTextBox.Value Is Nothing Then
                MsgBox("You must specify a temporal Integration time.", MsgBoxStyle.Information, "Checking input data")
                Exit Sub
            Else
                TemporalIntegration = TemporalIntegration_DoubleParsingTextBox.Value
            End If
        End If

        'Launching sound level adjustment algoritms
        If NaturalLevelsAlgorithm_Checkbox.Checked = True Then
            SelectedMediaSet.SetNaturalLevels(SpeechLevel_DoubleParsingTextBox.Value, SpeechLevelFrequencyWeighting, TemporalIntegration) ' N.B. 'TemporalIntegration is Nothing for long-time average
        Else
            SelectedMediaSet.SetSpeechLevels(SpeechLevel_DoubleParsingTextBox.Value, SpeechLevelFrequencyWeighting, TemporalIntegration)
        End If

        MsgBox("Finished adjusting the speech sound levels.", MsgBoxStyle.Information, "Speech level ")

    End Sub
End Class


'Speech level group box
'Speech level __
'Frequency weighting: C, Z, RLB, K
'(CB) Temporal integration: __
'(CB) Apply 'natural levels' algorithm
'
'Button: Apply levels
'
'
'Create maskers group box
'
'Common maskers at linguistic level: ListCollection, List, Sentence, Word, Phoneme
'
'Spectral match type: Everything, Contrasting unit
'
'Masker source type:
'- (RB) Random Noise
'- (RB) Speech material
'- (RB) Best match of external sound files 
'
'Settings:
'-Fade in duration __
'-Masking duration (*auto, __
'-Fade out duration __
'
'(Settings not for Speech Material)
'Frequency weighting: C, Z
'(CB) Temporal integration: 50 ms
'	
'(Settings for external sound file matching only)
'-(Sound file folder: ...) ____
'-Spectral fine-tuning (max (dB)) __


