Public Class EditTestSituationControl


    Public Property SelectedTestSituation As MediaSet = Nothing

    Private Sub NewTestSituatuionControl_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'Adding genders
        TalkerGender_ComboBox.Items.AddRange([Enum].GetNames(GetType(MediaSet.Genders)))

        'Adding supported bit depths
        WaveFileBitDepth_ComboBox.Items.Add(16)
        WaveFileBitDepth_ComboBox.Items.Add(32)

        'Adding encodings
        WaveFileEncoding_ComboBox.Items.AddRange([Enum].GetNames(GetType(Audio.Formats.WaveFormat.WaveFormatEncodings)))

        UpdateControlEnabledStatuses()

    End Sub

    Public Sub SetTestSpecification(ByRef SelectedTestSpecification As TestSpecification)
        Me.LoadOstaTestSituationsControl1.SelectedTestSpecification = SelectedTestSpecification
    End Sub

    Private Sub LoadOstaTestSituationsControl1_TestSituationSelected() Handles LoadOstaTestSituationsControl1.TestSituationSelected

        If LoadOstaTestSituationsControl1.SelectedTestSituation IsNot Nothing Then
            SelectedTestSituation = LoadOstaTestSituationsControl1.SelectedTestSituation
        End If

        ViewTestSituationData()

        UpdateControlEnabledStatuses()

    End Sub


    Private Sub UpdateControlEnabledStatuses()

        If SelectedTestSituation IsNot Nothing Then
            Edit_TableLayoutPanel.Enabled = True
            Save_Button.Enabled = True
        Else
            Edit_TableLayoutPanel.Enabled = False
            Save_Button.Enabled = False
        End If

    End Sub

    Private Sub NewTestSituation_Button_Click(sender As Object, e As EventArgs) Handles NewTestSituation_Button.Click

        Me.SelectedTestSituation = New MediaSet With {.ParentTestSpecification = LoadOstaTestSituationsControl1.SelectedTestSpecification}

        ViewTestSituationData()

        UpdateControlEnabledStatuses()

    End Sub


    Private Sub ViewTestSituationData()

        'Exits if no test situation is selected
        If Me.SelectedTestSituation Is Nothing Then
            MsgBox("No test situation selected!", MsgBoxStyle.Information, "Viewing test situation data")
            UpdateControlEnabledStatuses()
            Exit Sub
        End If

        TestSituationName_TextBox.Text = SelectedTestSituation.TestSituationName
        TalkerName_TextBox.Text = SelectedTestSituation.TalkerName
        TalkerGender_ComboBox.SelectedItem = SelectedTestSituation.TalkerGender
        TalkerAge_IntegerParsingTextBox.Text = SelectedTestSituation.TalkerAge
        TalkerDialect_TextBox.Text = SelectedTestSituation.TalkerDialect
        VoiceType_TextBox.Text = SelectedTestSituation.VoiceType
        MediaAudioItems_IntegerParsingTextBox.Text = SelectedTestSituation.MediaAudioItems
        MaskerAudioItems_IntegerParsingTextBox.Text = SelectedTestSituation.MaskerAudioItems
        MediaImageItems_IntegerParsingTextBox.Text = SelectedTestSituation.MediaImageItems
        MaskerImageItems_IntegerParsingTextBox.Text = SelectedTestSituation.MaskerImageItems
        MediaParentFolder_TextBox.Text = SelectedTestSituation.MediaParentFolder
        MaskerParentFolder_TextBox.Text = SelectedTestSituation.MaskerParentFolder
        BackgroundNonspeechParentFolder_TextBox.Text = SelectedTestSituation.BackgroundNonspeechParentFolder
        BackgroundSpeechParentFolder_TextBox.Text = SelectedTestSituation.BackgroundSpeechParentFolder
        PrototypeMediaParentFolder_TextBox.Text = SelectedTestSituation.PrototypeMediaParentFolder
        MasterPrototypeRecordingPath_TextBox.Text = SelectedTestSituation.MasterPrototypeRecordingPath
        PrototypeRecordingLevel_DoubleParsingTextBox.Text = SelectedTestSituation.PrototypeRecordingLevel
        LombardNoisePath_TextBox.Text = SelectedTestSituation.LombardNoisePath
        LombardNoiseLevel_DoubleParsingTextBox.Text = SelectedTestSituation.LombardNoiseLevel
        WaveFileSampleRate_IntegerParsingTextBox.Text = SelectedTestSituation.WaveFileSampleRate
        WaveFileBitDepth_ComboBox.SelectedItem = SelectedTestSituation.WaveFileBitDepth
        WaveFileEncoding_ComboBox.SelectedItem = SelectedTestSituation.WaveFileEncoding

    End Sub

    Private Sub SaveTestSituation(sender As Object, e As EventArgs) Handles Save_Button.Click

        Dim TempTestSituation = New MediaSet

        'Checking and adding values
        If TestSituationName_TextBox.Text.Trim = "" Then
            MsgBox("Supply a test situation name")
            Exit Sub
        Else
            TempTestSituation.TestSituationName = TestSituationName_TextBox.Text.Trim
        End If

        If TalkerName_TextBox.Text.Trim = "" Then
            MsgBox("Supply a talker name")
            Exit Sub
        Else
            TempTestSituation.TalkerName = TalkerName_TextBox.Text.Trim
        End If

        If TalkerGender_ComboBox.SelectedItem IsNot Nothing Then
            TempTestSituation.TalkerGender = [Enum].Parse(GetType(MediaSet.Genders), TalkerGender_ComboBox.SelectedItem.ToString)
        End If

        If TalkerAge_IntegerParsingTextBox.Value IsNot Nothing Then
            TempTestSituation.TalkerAge = TalkerAge_IntegerParsingTextBox.Value
        End If

        TempTestSituation.TalkerDialect = TalkerDialect_TextBox.Text.Trim
        TempTestSituation.VoiceType = VoiceType_TextBox.Text.Trim

        If MediaAudioItems_IntegerParsingTextBox.Value IsNot Nothing Then
            TempTestSituation.MediaAudioItems = MediaAudioItems_IntegerParsingTextBox.Value
        Else
            MsgBox("Supply a value for number of duplicate audio targets")
            Exit Sub
        End If

        If MaskerAudioItems_IntegerParsingTextBox.Value IsNot Nothing Then
            TempTestSituation.MaskerAudioItems = MaskerAudioItems_IntegerParsingTextBox.Value
        Else
            MsgBox("Supply a value for number of duplicate audio maskers")
            Exit Sub
        End If

        If MediaImageItems_IntegerParsingTextBox.Value IsNot Nothing Then
            TempTestSituation.MediaImageItems = MediaImageItems_IntegerParsingTextBox.Value
        Else
            MsgBox("Supply a value for number of duplicate image targets")
            Exit Sub
        End If

        If MaskerImageItems_IntegerParsingTextBox.Value IsNot Nothing Then
            TempTestSituation.MaskerImageItems = MaskerImageItems_IntegerParsingTextBox.Value
        Else
            MsgBox("Supply a value for number of duplicate image maskers")
            Exit Sub
        End If

        TempTestSituation.MediaParentFolder = MediaParentFolder_TextBox.Text.Trim
        If TempTestSituation.MediaAudioItems + TempTestSituation.MediaImageItems = 0 And TempTestSituation.MediaParentFolder = "" Then
            MsgBox("You must supply a subfolder containing target files")
            Exit Sub
        End If

        TempTestSituation.MaskerParentFolder = MaskerParentFolder_TextBox.Text.Trim
        If TempTestSituation.MaskerAudioItems + TempTestSituation.MaskerImageItems = 0 And TempTestSituation.MaskerParentFolder = "" Then
            MsgBox("You must supply a subfolder containing masker files")
            Exit Sub
        End If

        TempTestSituation.BackgroundNonspeechParentFolder = BackgroundNonspeechParentFolder_TextBox.Text.Trim
        If BackgroundNonspeechRealisticLevel_DoubleParsingTextBox.Value IsNot Nothing Then
            TempTestSituation.BackgroundNonspeechRealisticLevel = BackgroundNonspeechRealisticLevel_DoubleParsingTextBox.Value
        End If

        TempTestSituation.BackgroundSpeechParentFolder = BackgroundSpeechParentFolder_TextBox.Text.Trim

        TempTestSituation.PrototypeMediaParentFolder = PrototypeMediaParentFolder_TextBox.Text.Trim
        TempTestSituation.MasterPrototypeRecordingPath = MasterPrototypeRecordingPath_TextBox.Text.Trim

        If PrototypeRecordingLevel_DoubleParsingTextBox.Value IsNot Nothing Then
            TempTestSituation.PrototypeRecordingLevel = PrototypeRecordingLevel_DoubleParsingTextBox.Value
        End If

        TempTestSituation.LombardNoisePath = LombardNoisePath_TextBox.Text.Trim

        If LombardNoiseLevel_DoubleParsingTextBox.Value IsNot Nothing Then
            TempTestSituation.LombardNoiseLevel = LombardNoiseLevel_DoubleParsingTextBox.Value
        End If

        If WaveFileSampleRate_IntegerParsingTextBox.Value IsNot Nothing Then
            TempTestSituation.WaveFileSampleRate = WaveFileSampleRate_IntegerParsingTextBox.Value
        Else
            MsgBox("You must supply a sample rate (48000 is recommended)")
            Exit Sub
        End If

        If WaveFileBitDepth_ComboBox.SelectedItem IsNot Nothing Then
            TempTestSituation.WaveFileBitDepth = WaveFileBitDepth_ComboBox.SelectedItem
        Else
            MsgBox("You must supply a bit depth (32 is recommended)")
            Exit Sub
        End If

        If WaveFileEncoding_ComboBox.SelectedItem IsNot Nothing Then
            TempTestSituation.WaveFileEncoding = [Enum].Parse(GetType(Audio.Formats.WaveFormat.WaveFormatEncodings), WaveFileEncoding_ComboBox.SelectedItem.ToString)
        Else
            MsgBox("You must supply a wave file encoding (IEEE float is recommended)")
            Exit Sub
        End If

        Me.SelectedTestSituation = TempTestSituation

        Me.SelectedTestSituation.WriteToFile()

        UpdateControlEnabledStatuses()

    End Sub

End Class
