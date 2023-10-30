Public Class MediaSetSetupControl


    Private SelectedTestSpecification As SpeechMaterialSpecification = Nothing

    Private SelectedMediaSet As MediaSet = Nothing


    Private Sub MediaSetSetupControl_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Try

            'Adding genders
            TalkerGender_ComboBox.Items.AddRange([Enum].GetNames(GetType(MediaSet.Genders)))

            'Adding sound file linguistic levels
            SoundFileLevelComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.List)
            SoundFileLevelComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.Sentence)
            SoundFileLevelComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.Word)
            SoundFileLevelComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.Phoneme)

            'Adding common masker sound file linguistic levels
            SharedMaskersLevelComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.ListCollection)
            SharedMaskersLevelComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.List)
            SharedMaskersLevelComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.Sentence)
            SharedMaskersLevelComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.Word)
            SharedMaskersLevelComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.Phoneme)

            'Adding linguistic levels for the speech level adjustment function
            SpeechLevelLinguisticLevel_ComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.ListCollection)
            SpeechLevelLinguisticLevel_ComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.List)
            SpeechLevelLinguisticLevel_ComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.Sentence)
            SpeechLevelLinguisticLevel_ComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.Word)
            SpeechLevelLinguisticLevel_ComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.Phoneme)
            'Auto-selects the sentence level as default
            SpeechLevelLinguisticLevel_ComboBox.SelectedIndex = 2

            'Adding linguistic levels for the modified linguistic level of sound files function
            'ModifiedMediaSetLinguisticLevel_ComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.ListCollection) ' TODO: It's not possible to convert to ListCollection lince a ListCollection cannot fit within the SMA (v1.1) specification.
            ModifiedMediaSetLinguisticLevel_ComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.List)
            ModifiedMediaSetLinguisticLevel_ComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.Sentence)
            ModifiedMediaSetLinguisticLevel_ComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.Word)
            ModifiedMediaSetLinguisticLevel_ComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.Phoneme)

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

            'Adding SMA level frequency weightings 
            SmaFrequencyWeighting_ComboBox.Items.Add(Audio.FrequencyWeightings.Z)
            SmaFrequencyWeighting_ComboBox.Items.Add(Audio.FrequencyWeightings.C)
            SmaFrequencyWeighting_ComboBox.Items.Add(Audio.FrequencyWeightings.K)
            SmaFrequencyWeighting_ComboBox.Items.Add(Audio.FrequencyWeightings.RLB)

            'Pre-selecting Z weighting
            SmaFrequencyWeighting_ComboBox.SelectedIndex = 0


            'Showing the value of Standard_dBFS_dBSPL_Difference in the Speech level SPL lable

            SpeechLevelSPL_Label.Text = "Speech level (dB SPL, [SPL - FS = " & Audio.Standard_dBFS_dBSPL_Difference & " dB])"

            UpdateControlEnabledStatuses()

        Catch ex As Exception
            MsgBox("The following error occured: " & vbCrLf & vbCrLf & ex.ToString)
        End Try

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
                EditSoundFile_TabControl.Enabled = True
            Else
                EditSpecification_TableLayoutPanel.Enabled = False
                SaveMediaSetSpecification_Button.Enabled = False
                EditSoundFile_TabControl.Enabled = False
            End If

        Else
            LoadOstaMediaSetControl1.Enabled = False
            NewMediaSet_Button.Enabled = False
            EditSpecification_TableLayoutPanel.Enabled = False
            SaveMediaSetSpecification_Button.Enabled = False
        End If

        UpdatePrototypeRecordingOptionsInGUI()

    End Sub


    Public Sub UpdatePrototypeRecordingOptionsInGUI()

        If SelectedTestSpecification IsNot Nothing Then
            If SelectedMediaSet IsNot Nothing Then
                Dim LocalUsePrototypeRecordings As Boolean = False
                If SelectedMediaSet.MasterPrototypeRecordingPath.Trim <> "" Then
                    LocalUsePrototypeRecordings = True
                    MasterPrototypeRecording_RadioButton.Enabled = True
                Else
                    MasterPrototypeRecording_RadioButton.Enabled = False
                End If

                If SelectedMediaSet.PrototypeMediaParentFolder.Trim <> "" Then
                    LocalUsePrototypeRecordings = True
                    SpecificPrototypeRecording_RadioButton.Enabled = True
                Else
                    SpecificPrototypeRecording_RadioButton.Enabled = False
                End If

                'Checks the NoPrototypeRecording_RadioButton if no prototype recording path has been specified, and unchecks it otherwise
                If LocalUsePrototypeRecordings = False Then
                    NoPrototypeRecording_RadioButton.Checked = True
                Else
                    NoPrototypeRecording_RadioButton.Checked = False
                End If
            End If
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
        SoundFileLevelComboBox.Text = SelectedMediaSet.AudioFileLinguisticLevel.ToString
        SharedMaskersLevelComboBox.Text = SelectedMediaSet.SharedMaskersLevel.ToString
        TalkerDialect_TextBox.Text = SelectedMediaSet.TalkerDialect
        VoiceType_TextBox.Text = SelectedMediaSet.VoiceType
        MediaAudioItems_IntegerParsingTextBox.Text = SelectedMediaSet.MediaAudioItems
        MaskerAudioItems_IntegerParsingTextBox.Text = SelectedMediaSet.MaskerAudioItems
        MediaImageItems_IntegerParsingTextBox.Text = SelectedMediaSet.MediaImageItems
        MaskerImageItems_IntegerParsingTextBox.Text = SelectedMediaSet.MaskerImageItems
        CustomVariablesFolder_TextBox.Text = SelectedMediaSet.CustomVariablesFolder
        MediaParentFolder_TextBox.Text = SelectedMediaSet.MediaParentFolder
        MaskerParentFolder_TextBox.Text = SelectedMediaSet.MaskerParentFolder
        BackgroundNonspeechRealisticLevel_DoubleParsingTextBox.Text = SelectedMediaSet.BackgroundNonspeechRealisticLevel
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

        'And also adding to other controls
        CurrentMediaSetLinguisticLevel_TextBox.Text = SelectedMediaSet.AudioFileLinguisticLevel.ToString

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
        If SoundFileLevelComboBox.SelectedItem IsNot Nothing Then
            TempMediaSet.AudioFileLinguisticLevel = [Enum].Parse(GetType(SpeechMaterialComponent.LinguisticLevels), SoundFileLevelComboBox.SelectedItem.ToString)
        Else
            MsgBox("You must select a value for 'Linguistic level of sound files'.", MsgBoxStyle.Information, "Checking input data")
            Exit Sub
        End If

        If SharedMaskersLevelComboBox.SelectedItem IsNot Nothing Then
            TempMediaSet.SharedMaskersLevel = [Enum].Parse(GetType(SpeechMaterialComponent.LinguisticLevels), SharedMaskersLevelComboBox.SelectedItem.ToString)
        Else
            MsgBox("You must select a value for 'Linguistic level on which to share maskers sound files'.", MsgBoxStyle.Information, "Checking input data")
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

        TempMediaSet.CustomVariablesFolder = CustomVariablesFolder_TextBox.Text.Trim
        If TempMediaSet.CustomVariablesFolder = "" Then
            MsgBox("You must supply a subfolder for custom media set variable files")
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
            Exit Sub
        End If

        'Parsing user input settings
        Dim PrototypeRecordingOptions As SpeechTestFramework.MediaSet.PrototypeRecordingOptions
        If MasterPrototypeRecording_RadioButton.Checked = True Then
            PrototypeRecordingOptions = MediaSet.PrototypeRecordingOptions.MasterPrototypeRecording
        ElseIf SpecificPrototypeRecording_RadioButton.Checked = True Then
            PrototypeRecordingOptions = MediaSet.PrototypeRecordingOptions.PrototypeRecordings
        Else
            PrototypeRecordingOptions = MediaSet.PrototypeRecordingOptions.None
        End If


        SelectedMediaSet.RecordAndEditAudioMediaFiles(SpeechTestFramework.MediaSet.SpeechMaterialRecorderLoadOptions.LoadAllSounds, RandomOrder_CheckBox.Checked, PrototypeRecordingOptions)


    End Sub

    Private Sub TemporalIntegration_CheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles SpeechLevelTemporalIntegration_CheckBox.CheckedChanged
        SpeechLevelTemporalIntegration_DoubleParsingTextBox.Enabled = SpeechLevelTemporalIntegration_CheckBox.Checked
    End Sub

    Private Sub ApplySpeechLevels_Button_Click(sender As Object, e As EventArgs) Handles ApplySpeechLevels_Button.Click

        'Checking input data
        If SpeechLevelSPL_DoubleParsingTextBox.Value Is Nothing Then
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

        Dim TemporalIntegration As Double = 0
        If SpeechLevelTemporalIntegration_CheckBox.Checked Then
            If SpeechLevelTemporalIntegration_DoubleParsingTextBox.Value Is Nothing Then
                MsgBox("You must specify a temporal Integration time.", MsgBoxStyle.Information, "Checking input data")
                Exit Sub
            Else
                TemporalIntegration = SpeechLevelTemporalIntegration_DoubleParsingTextBox.Value
            End If
        End If

        If TemporalIntegration < 0 Then
            MsgBox("Temporal Integration time cannot be below zero.", MsgBoxStyle.Information, "Checking input data")
            Exit Sub
        End If

        'Launching sound level adjustment algoritms
        If VpNormalization_Checkbox.Checked = True Then
            SelectedMediaSet.SetVpNormalizedLevels(CDbl(SpeechLevelSPL_DoubleParsingTextBox.Value), SpeechLevelFrequencyWeighting, TemporalIntegration) ' N.B. 'TemporalIntegration is zero for long-time average
        Else
            Dim TargetLinguisticLevel As SpeechMaterialComponent.LinguisticLevels
            If SpeechLevelLinguisticLevel_ComboBox.SelectedItem Is Nothing Then
                MsgBox("You must select the linguistic level for which you want to set the sound level!", MsgBoxStyle.Exclamation, "Missing linguistic level")
                Exit Sub
            End If
            TargetLinguisticLevel = SpeechLevelLinguisticLevel_ComboBox.SelectedItem
            SelectedMediaSet.SetSpeechLevels(SpeechLevelFS_DoubleParsingTextBox.Value, SpeechLevelFrequencyWeighting, TemporalIntegration, TargetLinguisticLevel)
        End If

        MsgBox("Finished adjusting the speech sound levels.", MsgBoxStyle.Information, "Speech level ")

    End Sub

    Private Sub SpeechLevelSPL_DoubleParsingTextBox_ValueUpdated() Handles SpeechLevelSPL_DoubleParsingTextBox.ValueUpdated

        If SpeechLevelSPL_DoubleParsingTextBox.Value IsNot Nothing Then
            SpeechLevelFS_DoubleParsingTextBox.Text = Audio.Standard_dBSPL_To_dBFS(SpeechLevelSPL_DoubleParsingTextBox.Value)
        Else
            SpeechLevelFS_DoubleParsingTextBox.Text = ""
        End If

    End Sub



    Private Sub SpeechLevelFS_DoubleParsingTextBox_ValueUpdated() Handles SpeechLevelFS_DoubleParsingTextBox.ValueUpdated

        If SpeechLevelFS_DoubleParsingTextBox.Value IsNot Nothing Then
            SpeechLevelSPL_DoubleParsingTextBox.Text = Audio.Standard_dBFS_To_dBSPL(SpeechLevelFS_DoubleParsingTextBox.Value)
        Else
            SpeechLevelSPL_DoubleParsingTextBox.Text = ""
        End If

    End Sub

    Private Sub MeasureSmaLevels_Button_Click(sender As Object, e As EventArgs) Handles MeasureSmaLevels_Button.Click

        Dim SmaFrequencyWeighting As Audio.FrequencyWeightings = Audio.BasicAudioEnums.FrequencyWeightings.Z
        If SmaFrequencyWeighting_ComboBox.SelectedItem IsNot Nothing Then
            SmaFrequencyWeighting = [Enum].Parse(GetType(Audio.FrequencyWeightings), SmaFrequencyWeighting_ComboBox.SelectedItem.ToString)
        Else
            MsgBox("You must specify a SMA frequency weighting.", MsgBoxStyle.Information, "Checking input data")
            Exit Sub
        End If

        Dim SmaTemporalIntegration As Double = 0
        If SmaTemporalIntegration_CheckBox.Checked Then
            If SmaTemporalIntegration_DoubleParsingTextBox.Value Is Nothing Then
                MsgBox("You must specify a SMA temporal Integration time.", MsgBoxStyle.Information, "Checking input data")
                Exit Sub
            Else
                SmaTemporalIntegration = SmaTemporalIntegration_DoubleParsingTextBox.Value
            End If
        End If

        If SmaTemporalIntegration < 0 Then
            MsgBox("Temporal Integration time cannot be below zero.", MsgBoxStyle.Information, "Checking input data")
            Exit Sub
        End If

        Dim IncludeCriticalBandLevels As Boolean = IncludeCriticalBandLevels_CheckBox.Checked

        SelectedMediaSet.MeasureSmaObjectSoundLevels(IncludeCriticalBandLevels, SmaFrequencyWeighting, SmaTemporalIntegration)

    End Sub

    Private Sub SmaTemporalIntegration_CheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles SmaTemporalIntegration_CheckBox.CheckedChanged
        SmaTemporalIntegration_DoubleParsingTextBox.Enabled = SmaTemporalIntegration_CheckBox.Checked
    End Sub

    Private Sub SipTestLevels_Button_Click(sender As Object, e As EventArgs) Handles SipTestLevels_Button.Click

        SelectedMediaSet.CalculateConcatenatedComponentSpectrumLevels(SpeechMaterialComponent.LinguisticLevels.Sentence, SpeechMaterialComponent.LinguisticLevels.Phoneme, True, 1, False)
        'N.B. This can instead be called (and stored) on the phoneme level using:
        'SelectedMediaSet.CalculateSpectrumlevels(SpeechMaterialComponent.LinguisticLevels.Phoneme, 1, True, False,)

        SelectedMediaSet.CalculateAverageMaxLevelOfCousinComponents(SpeechMaterialComponent.LinguisticLevels.List, SpeechMaterialComponent.LinguisticLevels.Phoneme, 1, False)

        SelectedMediaSet.CalculateComponentLevel(SpeechMaterialComponent.LinguisticLevels.ListCollection, 1, ,,,, True)

        SelectedMediaSet.CalculateComponentLevel(SpeechMaterialComponent.LinguisticLevels.Sentence, 1)

        SelectedMediaSet.CalculateAverageDurationOfContrastingComponents(SpeechMaterialComponent.LinguisticLevels.Sentence, SpeechMaterialComponent.LinguisticLevels.Phoneme, 1)

        'This can be used to get spectrum levels for any linguistic level
        'SelectedMediaSet.CalculateSpectrumlevels(SpeechMaterialComponent.LinguisticLevels.Phoneme, 1, False, False)

    End Sub

    Private Sub CreateSipTestMaskersButton_Click(sender As Object, e As EventArgs) Handles CreateSipTestMaskersButton.Click

        'Creating SiP-test type masker sounds
        SelectedMediaSet.CreateNewTestSituationMaskers(MediaSet.MaskerSourceTypes.ExternalSoundFilesBestMatch,
                                                        SpeechMaterialComponent.LinguisticLevels.Sentence,
                                                          SpeechMaterialComponent.LinguisticLevels.Word, True, , False,,,,, 3,, 1, 10, SipTest.Common.SipTestReferenceMaskerLevel_FS)

    End Sub

    Private Sub CalculateSipTestMaskerSpectrumLevels_Button_Click(sender As Object, e As EventArgs) Handles CalculateSipTestMaskerSpectrumLevels_Button.Click

        SelectedMediaSet.CalculateMaskerSpectrumLevels()

    End Sub

    Private Sub TempButton_Click(sender As Object, e As EventArgs) Handles TempButton.Click

        'SelectedMediaSet.TemporaryFunction_RenameMaskerFolder()

    End Sub

    Private Sub CreateModifiedMediaSet_Button_Click(sender As Object, e As EventArgs) Handles CreateModifiedMediaSet_Button.Click

        'Getting values
        If ModifiedMediaSetLinguisticLevel_ComboBox.SelectedItem Is Nothing Then
            MsgBox("Please indicate a linguistic level for the sound files to create!", MsgBoxStyle.Exclamation, "Sound file linguistic levels")
            Exit Sub
        End If

        If ModifiedMediaSetName_TextBox.Text = "" Then
            MsgBox("Please enter a name for the modified media set!", MsgBoxStyle.Exclamation, "Sound file linguistic levels")
            Exit Sub
        End If

        If NewSoundFilePadding_IntegerParsingTextBox.Value Is Nothing Then
            MsgBox("Please indicate padding duration (silence before and after the speech components) for the new sound files!", MsgBoxStyle.Exclamation, "Sound file linguistic levels")
            Exit Sub
        End If

        If NewSoundFile_InterStimulusInterval_IntegerParsingTextBox.Value Is Nothing Then
            MsgBox("Please indicate an inter-stimulus interval (i.e. silence between the speech components) for the new sound files!", MsgBoxStyle.Exclamation, "Sound file linguistic levels")
            Exit Sub
        End If

        If CrossfadeDuration_IntegerParsingTextBox.Value Is Nothing Then
            MsgBox("Please indicate a crossfade duration (can be zero, if you don't want any crossfading) for the new sound files!", MsgBoxStyle.Exclamation, "Sound file linguistic levels")
            Exit Sub
        End If

        If IncludePractiseItems_CheckBox.Checked = False And IncludeTestItems_CheckBox.Checked = False Then
            MsgBox("Please include either test or practise items, or both!", MsgBoxStyle.Exclamation, "Sound file linguistic levels")
            Exit Sub
        End If

        'Creating a deep copy of the current media set
        Dim NewMediaSet = SelectedMediaSet.CreateCopy

        'Setting changed values
        NewMediaSet.MediaSetName = ModifiedMediaSetName_TextBox.Text
        NewMediaSet.AudioFileLinguisticLevel = ModifiedMediaSetLinguisticLevel_ComboBox.SelectedItem

        'Inferring other values
        If NewMediaSet.BackgroundNonspeechParentFolder <> "" Then NewMediaSet.BackgroundNonspeechParentFolder = IO.Path.Combine("Media", NewMediaSet.MediaSetName, "BackgroundNonspeech")
        If NewMediaSet.BackgroundSpeechParentFolder <> "" Then NewMediaSet.BackgroundSpeechParentFolder = IO.Path.Combine("Media", NewMediaSet.MediaSetName, "BackgroundSpeech")
        NewMediaSet.CustomVariablesFolder = IO.Path.Combine("Media", NewMediaSet.MediaSetName, "CustomVariables")
        NewMediaSet.MaskerParentFolder = IO.Path.Combine("Media", NewMediaSet.MediaSetName, "Maskers")
        NewMediaSet.MediaParentFolder = IO.Path.Combine("Media", NewMediaSet.MediaSetName, "TestWordRecordings")
        If NewMediaSet.PrototypeMediaParentFolder <> "" Then NewMediaSet.PrototypeMediaParentFolder = IO.Path.Combine("Media", NewMediaSet.MediaSetName, "PrototypeRecordings")

        'Copying sound files
        Dim SoundChannel As Integer = 1

        Dim Padding As Integer = Math.Floor((NewSoundFilePadding_IntegerParsingTextBox.Value.Value / 1000) * SelectedMediaSet.WaveFileSampleRate)
        Dim InterStimulusIntervalLength As Integer = Math.Floor((NewSoundFile_InterStimulusInterval_IntegerParsingTextBox.Value.Value / 1000) * SelectedMediaSet.WaveFileSampleRate)
        Dim CrossfadeLength As Integer = Math.Floor((CrossfadeDuration_IntegerParsingTextBox.Value.Value / 1000) * SelectedMediaSet.WaveFileSampleRate)

        Dim RandomSeed As Integer? = Nothing
        If RandomSeed_IntegerParsingTextBox.Value IsNot Nothing Then
            RandomSeed = RandomSeed_IntegerParsingTextBox.Value.Value
        End If

        SelectedMediaSet.CopySoundsToNewMediaSet(NewMediaSet, SoundChannel, Padding, InterStimulusIntervalLength, CrossfadeLength,
                                                 IncludeTestItems_CheckBox.Checked, IncludePractiseItems_CheckBox.Checked, RandomizeOrder_CheckBox.Checked, RandomSeed, True)

        NewMediaSet.WriteCustomVariables()

        NewMediaSet.WriteToFile()

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


