Imports System.Globalization

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

            ' Items for the SpeechLevel_TargetLinguisticlevel_ComboBox
            SpeechLevel_TargetLinguisticlevel_ComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.ListCollection)
            SpeechLevel_TargetLinguisticlevel_ComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.List)
            SpeechLevel_TargetLinguisticlevel_ComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.Sentence)
            SpeechLevel_TargetLinguisticlevel_ComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.Word)
            SpeechLevel_TargetLinguisticlevel_ComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.Phoneme)

            'Items for the GenerateSnrRangeStimuli_NoiseType_ComboBox
            GenerateSnrRangeStimuli_NoiseType_ComboBox.Items.Add(GenerateSnrRangeStimuli_NoiseTypes.White)
            GenerateSnrRangeStimuli_NoiseType_ComboBox.Items.Add(GenerateSnrRangeStimuli_NoiseTypes.SpeechWeighted)
            GenerateSnrRangeStimuli_NoiseType_ComboBox.Items.Add(GenerateSnrRangeStimuli_NoiseTypes.ThresholdSimulating)
            GenerateSnrRangeStimuli_NoiseType_ComboBox.SelectedIndex = 0

            GenerateSnrRangeStimuli_NoiseFW_ComboBox.Items.Add(Audio.FrequencyWeightings.Z)
            GenerateSnrRangeStimuli_NoiseFW_ComboBox.Items.Add(Audio.FrequencyWeightings.C)
            GenerateSnrRangeStimuli_NoiseFW_ComboBox.Items.Add(Audio.FrequencyWeightings.K)
            GenerateSnrRangeStimuli_NoiseFW_ComboBox.Items.Add(Audio.FrequencyWeightings.RLB)
            GenerateSnrRangeStimuli_NoiseFW_ComboBox.SelectedIndex = 0

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
            'TODO: SetVpNormalizedLevels do not use TargetLinguisticLevel but is fixed to the linguistic levels used in the SiP-test. Change in the future?
            SelectedMediaSet.SetVpNormalizedLevels(CDbl(SpeechLevelSPL_DoubleParsingTextBox.Value), SpeechLevelFrequencyWeighting, TemporalIntegration) ' N.B. 'TemporalIntegration is zero for long-time average
        Else

            'Getting the target linguistic level
            Dim TargetLinguisticLevel As SpeechMaterialComponent.LinguisticLevels
            If SpeechLevel_TargetLinguisticlevel_ComboBox.SelectedItem Is Nothing Then
                MsgBox("You must select the linguistic level for which you want to set the sound level!", MsgBoxStyle.Exclamation, "Missing linguistic level")
                Exit Sub
            End If
            TargetLinguisticLevel = SpeechLevel_TargetLinguisticlevel_ComboBox.SelectedItem

            If OnlyNominalLevel_CheckBox.Checked = False Then
                SelectedMediaSet.SetSpeechLevels(SpeechLevelFS_DoubleParsingTextBox.Value, SpeechLevelFrequencyWeighting, TemporalIntegration, TargetLinguisticLevel, NominalLevel_CheckBox.Checked)
            Else
                'Only setting nominal level in the corresponding SMA chunks
                SelectedMediaSet.StoreNominalLevel(SpeechLevelFS_DoubleParsingTextBox.Value, TargetLinguisticLevel)
            End If
        End If



        'Creating a calibration signal with the SpeechLevelFS_DoubleParsingTextBox.Value value
        If CreateCalibrationSignal_CheckBox.Checked = True Then

            'Asks the user where to store the calibration signal
            Dim CalibrationSignalPath = Utils.GetSaveFilePath(SelectedMediaSet.GetFullMediaParentFolder, "CalibrationSignal_" & SpeechLevelFS_DoubleParsingTextBox.Value.ToString().Replace(",", ".") & "dB")

            If CalibrationSignalPath <> "" Then
                'Creates a standard calibration signal (frequency modulated sine wave)
                Dim CarrierFrequency As Double = 1000
                Dim ModulationFrequency As Double = 20
                Dim ModulationDepth As Double = 0.125
                Dim Duration As Double = 60

                Dim GeneratedWarble = Audio.GenerateSound.CreateFrequencyModulatedSineWave(SelectedMediaSet.CreateCalibrationSoundWaveFormat, , CarrierFrequency, 0.5, ModulationFrequency, ModulationDepth,, Duration)

                'Sets its level using Z-weighting even if some other weighting was used for the speech material 
                Audio.DSP.MeasureAndAdjustSectionLevel(GeneratedWarble, SpeechLevelFS_DoubleParsingTextBox.Value,,,,)

                'Stores the calibration signal
                GeneratedWarble.WriteWaveFile(CalibrationSignalPath)

                'Shows and stores information about the calibration signal
                Dim CalibrationSignalDescription = "The calibration signal (frequency modulated sine wave) in " & CalibrationSignalPath & " is frequency modulated around " & CarrierFrequency & " Hz by ±" & (ModulationDepth * 100).ToString & " %, with a modulation frequency of " & ModulationFrequency & " Hz. Samplerate: " & GeneratedWarble.WaveFormat.SampleRate & " Hz, duration: " & Duration & " seconds."
                Utils.SendInfoToLog(CalibrationSignalDescription, "Calibration signal info", IO.Path.GetDirectoryName(CalibrationSignalPath))
                MsgBox(CalibrationSignalDescription, MsgBoxStyle.Information, "Calibration signal info.")

            Else
                MsgBox("Unable to store the calibration signal. No path was supplied!", MsgBoxStyle.Exclamation, "No calibration signal path supplied!")
            End If
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

    Private Sub NominalLevel_CheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles NominalLevel_CheckBox.CheckedChanged
        'Infering the check value of NominalLevel_CheckBox also to CreateCalibrationSignal_CheckBox 
        CreateCalibrationSignal_CheckBox.Checked = NominalLevel_CheckBox.Checked

        OnlyNominalLevel_CheckBox.Enabled = NominalLevel_CheckBox.Checked
        If NominalLevel_CheckBox.Checked = False Then OnlyNominalLevel_CheckBox.Checked = False

    End Sub

    Private Sub VpNormalization_Checkbox_CheckedChanged(sender As Object, e As EventArgs) Handles VpNormalization_Checkbox.CheckedChanged

        'Unchecks the NominalLevel_CheckBox if VpNormalization_Checkbox is checked, because nominal level is not (yet) set by the VpNormalization function. When VpNormalization is used, nominal levels have to be set in a separate step.
        If VpNormalization_Checkbox.Checked = True Then NominalLevel_CheckBox.Checked = False

    End Sub

    Private Sub ApplyCustomSpeechGain_Button_Click(sender As Object, e As EventArgs) Handles ApplyCustomSpeechGain_Button.Click

        Dim AllComponents = SelectedMediaSet.ParentTestSpecification.SpeechMaterial.GetAllRelatives
        Dim AllComponentsLookup = New SortedList(Of String, SpeechMaterialComponent)
        For Each Component In AllComponents
            AllComponentsLookup.Add(Component.Id, Component)
        Next

        Dim InputData = CustomSpeechGain_TextBox.Lines
        'Checking lines first
        Dim ComponentsToAdjust As New List(Of Tuple(Of SpeechMaterialComponent, Integer, Double)) 'ID, SoundIndex, Gain
        For i = 0 To InputData.Length - 1

            Dim Line = InputData(i)

            If Line.Trim = "" Then Continue For

            Dim LineSplit = Line.Split("|")

            If LineSplit.Length <> 3 Then
                MsgBox("Incorrect number of items on line " & i + 1 & " ( " & Line & " )")
                Exit Sub
            End If

            Dim ID As String = LineSplit(0).Trim
            If ID = "" Then
                MsgBox("Invalid SpeechComponentID detected on line " & i + 1 & " ( " & Line & " )")
                Exit Sub
            End If
            If AllComponentsLookup.ContainsKey(ID) = False Then
                MsgBox("The current speech material does not contain a Speech Material Component with the ID " & ID & " as specified on line " & i + 1 & " ( " & Line & " )")
                Exit Sub
            End If
            Dim Component = AllComponentsLookup(ID)

            Dim SoundIndex As Integer
            If Integer.TryParse(LineSplit(1).Trim.Replace(",", "."), SoundIndex) = False Then
                MsgBox("Unable to parse the sound index value on line " & i + 1 & " ( " & Line & " )")
                Exit Sub
            End If

            Dim Gain As Double

            If Double.TryParse(LineSplit(2).Trim.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, Gain) = False Then
                MsgBox("Unable to parse the gain value on line " & i + 1 & " ( " & Line & " )")
                Exit Sub
            End If
            If Double.IsInfinity(Gain) Or Double.IsNaN(Gain) Then
                MsgBox("The gain value on line " & i + 1 & " was parsed as NaN or Infinity, cannot proceed. ( " & Line & " )")
                Exit Sub
            End If

            'Adding the component to adjust
            ComponentsToAdjust.Add(New Tuple(Of SpeechMaterialComponent, Integer, Double)(Component, SoundIndex, Gain))

        Next

        'Clears previously loaded sounds
        SelectedMediaSet.ParentTestSpecification.SpeechMaterial.ClearAllLoadedSounds()

        'Applies the specified gains
        For i = 0 To ComponentsToAdjust.Count - 1

            Dim CurrentComponent = ComponentsToAdjust(i).Item1
            Dim CurrentSoundIndex = ComponentsToAdjust(i).Item2
            Dim CurrentGain = ComponentsToAdjust(i).Item3

            Dim CorrespondingSmaComponents = CurrentComponent.GetCorrespondingSmaComponent(SelectedMediaSet, CurrentSoundIndex, 1, True, False)

            For j = 0 To CorrespondingSmaComponents.Count - 1
                CorrespondingSmaComponents(j).ApplyGain(CurrentGain)
            Next
        Next

        'And save the modified sounds back to file
        SelectedMediaSet.ParentTestSpecification.SpeechMaterial.SaveAllLoadedSounds(True)

        MsgBox("Adjusted the levels of " & ComponentsToAdjust.Count & " speech material components in " & SelectedMediaSet.ParentTestSpecification.SpeechMaterial.GetNumberOfLoadedSounds & " sound files.")

    End Sub

    Private Sub GenerateSnrRangeStimuli_NoiseType_ComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles GenerateSnrRangeStimuli_NoiseType_ComboBox.SelectedIndexChanged

        If GenerateSnrRangeStimuli_NoiseType_ComboBox.SelectedItem = GenerateSnrRangeStimuli_NoiseTypes.ThresholdSimulating Then
            GenerateSnrRangeStimuli_PresentationLevel_DoubleParsingTextBox.Enabled = True
        Else
            GenerateSnrRangeStimuli_PresentationLevel_DoubleParsingTextBox.Enabled = False
        End If

        If GenerateSnrRangeStimuli_NoiseType_ComboBox.SelectedItem = GenerateSnrRangeStimuli_NoiseTypes.SpeechWeighted Then
            GenerateSnrRangeStimuli_Overlays_IntegerParsingTextBox.Enabled = True
        Else
            GenerateSnrRangeStimuli_Overlays_IntegerParsingTextBox.Enabled = False
        End If

    End Sub

    Public Enum GenerateSnrRangeStimuli_NoiseTypes
        White
        SpeechWeighted
        ThresholdSimulating
    End Enum

    Private Sub GenerateSnrRangeStimuli_Button_Click(sender As Object, e As EventArgs) Handles GenerateSnrRangeStimuli_Button.Click

        If GenerateSnrRangeStimuli_UpperLimit_IntegerParsingTextBox.Value Is Nothing Then
            MsgBox("You must supply a value for the upper SNR limit!")
            Exit Sub
        End If

        If GenerateSnrRangeStimuli_LowerLimit_IntegerParsingTextBox.Value Is Nothing Then
            MsgBox("You must supply a value for the lower SNR limit!")
            Exit Sub
        End If
        If GenerateSnrRangeStimuli_StepSize_DoubleParsingTextBox.Value Is Nothing Then
            MsgBox("You must supply a value for SNR step size!")
            Exit Sub
        End If

        'Getting values
        Dim UpperSNRLimit As Double = GenerateSnrRangeStimuli_UpperLimit_IntegerParsingTextBox.Value
        Dim LowerSNRLimit As Double = GenerateSnrRangeStimuli_LowerLimit_IntegerParsingTextBox.Value
        Dim SnrStepSize As Double = GenerateSnrRangeStimuli_StepSize_DoubleParsingTextBox.Value
        If SnrStepSize > 0 Then
            'ok
        Else
            MsgBox("The SNR step size must be a positive value!")
            Exit Sub
        End If
        Dim NoiseType As GenerateSnrRangeStimuli_NoiseTypes = GenerateSnrRangeStimuli_NoiseType_ComboBox.SelectedItem
        If NoiseType = GenerateSnrRangeStimuli_NoiseTypes.ThresholdSimulating Then
            If GenerateSnrRangeStimuli_PresentationLevel_DoubleParsingTextBox.Value Is Nothing Then
                MsgBox("You must supply a value for the intended presentation level!")
                Exit Sub
            End If
        End If

        Dim OverlayCount As Integer = 0
        If NoiseType = GenerateSnrRangeStimuli_NoiseTypes.SpeechWeighted Then
            If GenerateSnrRangeStimuli_Overlays_IntegerParsingTextBox.Value Is Nothing Then
                MsgBox("You must supply a value for number of speech overlays!")
                Exit Sub
            Else
                OverlayCount = GenerateSnrRangeStimuli_Overlays_IntegerParsingTextBox.Value
            End If
        End If


        Dim NoiseFrequencyWeighting As Audio.FrequencyWeightings = GenerateSnrRangeStimuli_NoiseFW_ComboBox.SelectedItem

        Dim IntendedPresentationLevel As Double = GenerateSnrRangeStimuli_PresentationLevel_DoubleParsingTextBox.Value

        Dim PostProcessingOutputGain As Double? = Nothing
        If GenerateSnrRangeStimuli_OutputGain_DoubleParsingTextBox.Value IsNot Nothing Then
            PostProcessingOutputGain = GenerateSnrRangeStimuli_OutputGain_DoubleParsingTextBox.Value
        End If

        Dim ExportConcatenatedSounds As Boolean = GenerateSnrRangeStimuli_ConcatenatedSound_CheckBox.Checked
        Dim InsertConcatenationSilence As Boolean = GenerateSnrRangeStimuli_InsertConcatenationSilence_CheckBox.Checked

        'Asks the user for an output path.
        Dim OutputFolder As String = SelectedMediaSet.GetFullMediaParentFolder
        Dim fbd = New Windows.Forms.FolderBrowserDialog
        fbd.SelectedPath = OutputFolder
        fbd.Description = "Where do you want to save you files?"
        Dim result = fbd.ShowDialog
        If result = Windows.Forms.DialogResult.OK Then
            OutputFolder = fbd.SelectedPath
        Else
            Exit Sub
        End If

        'All arguments ok, now start processing

        'Clears previously loaded sounds
        SelectedMediaSet.ParentTestSpecification.SpeechMaterial.ClearAllLoadedSounds()

        'Loading All needed sound files
        Dim AllComponents = SelectedMediaSet.ParentTestSpecification.SpeechMaterial.GetAllRelativesAtLevel(SelectedMediaSet.AudioFileLinguisticLevel)
        For SoundIndex = 0 To SelectedMediaSet.MediaAudioItems - 1
            For Each Component In AllComponents
                Dim CurrentSound = Component.GetSound(SelectedMediaSet, SoundIndex, 1)
            Next
        Next

        'Referencing the loaded sounds locally
        Dim LoadedSoundFiles = SelectedMediaSet.ParentTestSpecification.SpeechMaterial.GetAllLoadedSounds

        'Getting and checking the nominal level
        Dim NominalLevel As Double? = Nothing
        Dim LongestSoundFileLength As Integer = 0
        Dim WaveFormat As Audio.Formats.WaveFormat = Nothing
        For Each LoadedSoundFile In LoadedSoundFiles
            If NominalLevel.HasValue = False Then
                If LoadedSoundFile.Value.SMA.NominalLevel.HasValue Then
                    NominalLevel = LoadedSoundFile.Value.SMA.NominalLevel
                Else
                    MsgBox("All speech sounds in the current media set needs to have nominal levels specified in their SMA chunk!")
                    Exit Sub
                End If
            Else
                If NominalLevel <> LoadedSoundFile.Value.SMA.NominalLevel Then
                    MsgBox("Unequal nominal levels (as specified in the corresponding SMA chunk) detected among the speech sounds in the current media set!" & vbCrLf & " First deviation sound file detected is: " & LoadedSoundFile.Key)
                    Exit Sub
                End If
            End If

            'Storing the longest sound file length
            LongestSoundFileLength = Math.Max(LongestSoundFileLength, LoadedSoundFile.Value.WaveData.LongestChannelSampleCount)

            If WaveFormat Is Nothing Then
                WaveFormat = LoadedSoundFile.Value.WaveFormat
            End If
        Next

        'Creating noise of lengt LongestSoundFileLength 
        Dim Noise As Audio.Sound = Nothing
        Select Case NoiseType
            Case GenerateSnrRangeStimuli_NoiseTypes.White
                Noise = Audio.GenerateSound.CreateWhiteNoise(WaveFormat, 1, 1, Math.Ceiling(LongestSoundFileLength), Audio.BasicAudioEnums.TimeUnits.samples)

                'Setting the noise level to the Nominal Level, using indicated frequency weighting
                Audio.DSP.MeasureAndAdjustSectionLevel(Noise, NominalLevel, 1,,, NoiseFrequencyWeighting)

            Case GenerateSnrRangeStimuli_NoiseTypes.SpeechWeighted
                Dim AllSoundsConcatenated = Audio.DSP.ConcatenateSounds(LoadedSoundFiles.Values.ToList,,,, False,)
                Noise = Audio.GenerateSound.CreateOverlayNoise(AllSoundsConcatenated, OverlayCount, (LongestSoundFileLength / WaveFormat.SampleRate) + 1)

                'Setting the noise level to the Nominal Level, using indicated frequency weighting
                Audio.DSP.MeasureAndAdjustSectionLevel(Noise, NominalLevel, 1,,, NoiseFrequencyWeighting)

            'Case GenerateSnrRangeStimuli_NoiseTypes.ThresholdSimulating

            '    'Creating TSN
            '    Noise = CreateThresholdSimulatingNoise(WaveFormat, IntendedPresentationLevel, Math.Ceiling(LongestSoundFileLength))

            '    'Creating a temporary white noise (perhaps it would be better to use a pink noise here...)
            '    Dim TempWhiteNoise = Audio.GenerateSound.CreateWhiteNoise(WaveFormat, 1, 1, Math.Ceiling(LongestSoundFileLength), Audio.BasicAudioEnums.TimeUnits.samples)

            '    'Setting the level of the temporary white noise to the Nominal Level, using indicated frequency weighting
            '    Audio.DSP.MeasureAndAdjustSectionLevel(Noise, NominalLevel, 1,,, NoiseFrequencyWeighting)

            '    'Creating a bank band
            '    Dim BandBank = Audio.DSP.BandBank.GetSiiCriticalRatioBandBank

            '    'Calculating the band levels 
            '    Dim BandLevels_TSN = Audio.DSP.Measurements.CalculateBandLevels(TempWhiteNoise, 1, BandBank)
            '    Dim BandLevels_WN = Audio.DSP.Measurements.CalculateBandLevels(Noise, 1, BandBank)

            '    'Getting the index if the 1 kHz band
            '    Dim CentreFrequencies = BandBank.GetCentreFrequencies().ToList
            '    Dim IndexOf1kHz = CentreFrequencies.IndexOf(1000)

            '    'Getting the level of the 1 kHz band
            '    Dim NoiseLevelAt1kHz = BandLevels_TSN(IndexOf1kHz)
            '    Dim WN_LevelAt1kHz = BandLevels_WN(IndexOf1kHz)

            '    'Equalizing the levels in the 1 kHz band, by changing the TSN
            '    Dim NeededGain As Double = NoiseLevelAt1kHz - WN_LevelAt1kHz

            '    'Applying the needed gain
            '    Audio.DSP.AmplifySection(Noise, NeededGain)

            Case GenerateSnrRangeStimuli_NoiseTypes.ThresholdSimulating

                'Creating TSN
                Noise = CreateThresholdSimulatingNoise(WaveFormat, IntendedPresentationLevel, Math.Ceiling(LongestSoundFileLength))

                'Setting the noise level to the Nominal Level, using indicated frequency weighting
                Audio.DSP.MeasureAndAdjustSectionLevel(Noise, NominalLevel, 1,,, NoiseFrequencyWeighting)

        End Select

        'Creating a 1 second silence to use if needed for the concatenation
        Dim SilentSound = Audio.GenerateSound.CreateSilence(WaveFormat,, 1)

        Dim MainOutputPath = IO.Path.Combine(OutputFolder, "SnrRange")

        'Generating files 
        Dim NumberOfGeneratedSoundFiles As Integer = 0
        For CurrentSNR As Double = LowerSNRLimit To UpperSNRLimit Step SnrStepSize

            Dim CurrentSnrAllSoundsList As New List(Of Audio.Sound)

            Dim CurrentOutputPath = IO.Path.Combine(MainOutputPath, "SNR_" & CurrentSNR.ToString)

            For Each SpeechSound In LoadedSoundFiles

                Dim CurrentMixedSoundOutputPath = IO.Path.Combine(CurrentOutputPath, IO.Path.GetDirectoryName(SpeechSound.Key).Split(IO.Path.DirectorySeparatorChar).Last.Trim & "-" & IO.Path.GetFileNameWithoutExtension(SpeechSound.Key))

                'And a speech copy
                Dim SpeechCopy = SpeechSound.Value.CreateCopy

                'Amplifying the speech sound by CurrentSNR  to attain the CurrentSNR 
                Audio.DSP.AmplifySection(SpeechCopy, CurrentSNR)

                'Mixing the sounds
                Dim MixedSound = Audio.DSP.SuperpositionSounds({SpeechCopy, Noise}.ToList)

                'Applies postprocessing gain, if needed
                If PostProcessingOutputGain.HasValue Then
                    If PostProcessingOutputGain.Value <> 0 Then
                        Audio.DSP.AmplifySection(MixedSound, PostProcessingOutputGain.Value)
                    End If
                End If

                'Updates the nominal level
                MixedSound.SMA.NominalLevel = NominalLevel + PostProcessingOutputGain
                MixedSound.SMA.InferNominalLevelToAllDescendants()

                'Saves the mixed sound to file
                MixedSound.WriteWaveFile(CurrentMixedSoundOutputPath)

                'Counts the number of generated sound files
                NumberOfGeneratedSoundFiles += 1

                If ExportConcatenatedSounds = True Then
                    'Adds the current mixed sound
                    CurrentSnrAllSoundsList.Add(MixedSound)

                    'Adds silence
                    If InsertConcatenationSilence = True Then
                        CurrentSnrAllSoundsList.Add(SilentSound)
                    End If
                End If

            Next

            If ExportConcatenatedSounds = True Then

                'Exports the concatenated sound of all sounds a the current SNR
                Dim CurrentSnrConcatenatedSound = Audio.DSP.ConcatenateSounds(CurrentSnrAllSoundsList,,,, False)
                Dim CurrentConcatSoundOutputPath = IO.Path.Combine(MainOutputPath, "Concatenated", "ConcatenatedSound_SNR_" & CurrentSNR.ToString)

                ''Updating the nominal level
                CurrentSnrConcatenatedSound.SMA.NominalLevel = NominalLevel + PostProcessingOutputGain
                CurrentSnrConcatenatedSound.SMA.InferNominalLevelToAllDescendants()

                CurrentSnrConcatenatedSound.WriteWaveFile(CurrentConcatSoundOutputPath)

                'Counts the number of generated sound files
                NumberOfGeneratedSoundFiles += 1
            End If

        Next

        MsgBox("Finished creating " & NumberOfGeneratedSoundFiles & " sound files.")

    End Sub

    Public Function CreateThresholdSimulatingNoise(ByVal WaveFormat As Audio.Formats.WaveFormat, ByVal ReferenceLoudness As Double, ByVal TotalLength As Integer)

        'Getting the noise kernel
        Dim NoiseKernel = GetNoiseKernel(WaveFormat, ReferenceLoudness)

        'Creates white noise
        Dim InternalNoise = Audio.GenerateSound.CreateWhiteNoise(WaveFormat, 1, , TotalLength, Audio.BasicAudioEnums.TimeUnits.samples)

        'Runs convolution with the kernel
        Dim Noise = SpeechTestFramework.Audio.DSP.FIRFilter(InternalNoise, NoiseKernel, New SpeechTestFramework.Audio.Formats.FftFormat, ,,,,, True)

        Return Noise

    End Function

    Public Function GetNoiseKernel(ByVal WaveFormat As Audio.Formats.WaveFormat, ByVal ReferenceLoudness As Double) As Audio.Sound

        Dim SpectrumLevels As New List(Of Tuple(Of Double, Double, Double))

        Dim BandBank = Audio.DSP.BandBank.GetSiiCriticalRatioBandBank

        Dim CentreFrequencies = BandBank.GetCentreFrequencies.ToList
        Dim BandWidths = BandBank.GetBandWidths.ToList

        Dim Filter = New Audio.DSP.IsoPhonFilter(CentreFrequencies)

        Dim FrequencyRepsonse As New List(Of Tuple(Of Single, Single))
        For i = 0 To CentreFrequencies.Count - 1
            Dim SpectrumLevel = Filter.GetPhonToSpl(ReferenceLoudness, i)
            Dim BandLevel = Audio.DSP.SpectrumLevel2BandLevel(SpectrumLevel, BandWidths(i))
            FrequencyRepsonse.Add(New Tuple(Of Single, Single)(CentreFrequencies(i), BandLevel))
        Next

        Dim FirKernelLength As Integer = WaveFormat.SampleRate / 24 '?? very arbibrarily!

        Dim NoiseKernel = Audio.GenerateSound.CreateCustumImpulseResponse(FrequencyRepsonse, Nothing, WaveFormat, New Audio.Formats.FftFormat(,,,, True), FirKernelLength)

        Return NoiseKernel

    End Function

    Private Sub GenerateSnrRangeStimuli_ConcatenatedSound_CheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles GenerateSnrRangeStimuli_ConcatenatedSound_CheckBox.CheckedChanged
        GenerateSnrRangeStimuli_InsertConcatenationSilence_CheckBox.Enabled = GenerateSnrRangeStimuli_ConcatenatedSound_CheckBox.Checked
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


