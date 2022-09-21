Public Class CalibrationForm

    Private SelectedChannel As Integer
    Private SelectedLevel As Double
    Private CalibrationFileDescriptions As New SortedList(Of String, String)
    Private SelectedTransducer As AudioSystemSpecification = Nothing

    Private Sub CalibrationForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'Adding signals
        Dim CalibrationFilesDirectory = IO.Path.Combine(OstfBase.RootDirectory, OstfBase.CalibrationSignalSubDirectory)
        Dim CalibrationFiles = IO.Directory.GetFiles(CalibrationFilesDirectory)

        'Getting calibration file descriptions from the text file SignalDescriptions.txt
        Dim DescriptionFile = IO.Path.Combine(CalibrationFilesDirectory, "SignalDescriptions.txt")
        Dim InputLines() As String = System.IO.File.ReadAllLines(DescriptionFile, System.Text.Encoding.UTF8)
        For Each line In InputLines
            If line.Trim = "" Then Continue For
            If line.Trim.StartsWith("//") Then Continue For
            Dim LineSplit = line.Trim.Split("=")
            If LineSplit.Length < 2 Then Continue For
            If LineSplit(0).Trim = "" Then Continue For
            If LineSplit(1).Trim = "" Then Continue For
            'Adds the description (filename, description)
            CalibrationFileDescriptions.Add(IO.Path.GetFileNameWithoutExtension(LineSplit(0).Trim), LineSplit(1).Trim)
        Next

        'Adding sound files
        Dim CalibrationSounds As New List(Of Audio.Sound)
        For Each File In CalibrationFiles
            If IO.Path.GetExtension(File) = ".wav" Then
                Dim NewCalibrationSound = Audio.Sound.LoadWaveFile(File)
                NewCalibrationSound.Description = IO.Path.GetFileNameWithoutExtension(File).Replace("_", " ")
                If NewCalibrationSound IsNot Nothing Then CalibrationSounds.Add(NewCalibrationSound)
            End If
        Next

        'Adding into CalibrationSignal_ComboBox
        For Each CalibrationSound In CalibrationSounds
            CalibrationSignal_ComboBox.Items.Add(CalibrationSound)
        Next
        If CalibrationSignal_ComboBox.Items.Count > 0 Then
            CalibrationSignal_ComboBox.SelectedIndex = 0
        End If

        'Adding internally generated sounds 
        AddInternallyGeneratedSounds()

        'Adding levels
        For Level = 60 To 80 Step 5
            CalibrationLevel_ComboBox.Items.Add(Level)
        Next
        CalibrationLevel_ComboBox.SelectedItem = 70

        'Adding transducers
        'Initiating the sound player if not already done
        If OstfBase.SoundPlayer Is Nothing Then OstfBase.SoundPlayer = New Audio.PortAudioVB.OverlappingSoundPlayer(False, False, False, False)

        Dim TempWaveFromat As New Audio.Formats.WaveFormat(48000, 32, 1,, Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints)
        OstfBase.SoundPlayer.ChangePlayerSettings(, TempWaveFromat,,, Audio.PortAudioVB.OverlappingSoundPlayer.SoundDirections.PlaybackOnly, False, False)

        Dim LocalAvailableTransducers = OstfBase.AvaliableTransducers
        If LocalAvailableTransducers.Count = 0 Then
            MsgBox("Unable to start the application since no sound transducers could be found!", MsgBoxStyle.Critical, "Calibration")
        End If

        'Adding transducers to the combobox, and selects the first one
        For Each Transducer In LocalAvailableTransducers
            Transducer_ComboBox.Items.Add(Transducer)
        Next
        Transducer_ComboBox.SelectedIndex = 0

    End Sub

    Public Sub AddInternallyGeneratedSounds()

        Dim TempWaveFormat As New Audio.Formats.WaveFormat(48000, 32, 1,, Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints)

        Dim GeneratedWarble = Audio.GenerateSound.CreateFrequencyModulatedSineWave(TempWaveFormat, 1, 1000, 0.5, 20, 0.125,, 60)
        GeneratedWarble.FileName = "Internal1"
        CalibrationSignal_ComboBox.Items.Add(GeneratedWarble)
        CalibrationFileDescriptions.Add("Internal1", "Internally generated warble tone. The tone is frequency modulated around 1 kHz by ±12.5 %, with a modulation frequency of 20 Hz. Samplerate 48kHz, duration 60 seconds.")

        Dim GeneratedSine = Audio.GenerateSound.CreateSineWave(TempWaveFormat, 1, 1000, 0.5, , 60)
        GeneratedWarble.FileName = "Internal2"
        CalibrationSignal_ComboBox.Items.Add(GeneratedWarble)
        CalibrationFileDescriptions.Add("Internal2", "Internally generated 1kHz sine. Samplerate 48kHz, duration 60 seconds.")

        Dim GeneratedSweep2 = Audio.GenerateSound.CreateLogSineSweep(TempWaveFormat, 1, 20, 20000, False, 0.5,, 15)
        GeneratedWarble.FileName = "Internal4"
        CalibrationSignal_ComboBox.Items.Add(GeneratedWarble)
        CalibrationFileDescriptions.Add("Internal4", "Internally generated log-sine sweep, 20Hz - 20kHz. Samplerate 48kHz, duration 15 seconds.")

        Dim GeneratedSweep = Audio.GenerateSound.CreateLogSineSweep(TempWaveFormat, 1, 20, 20000, True, 0.5,, 15)
        GeneratedWarble.FileName = "Internal3"
        CalibrationSignal_ComboBox.Items.Add(GeneratedWarble)
        CalibrationFileDescriptions.Add("Internal3", "Internally generated (flat spectrum) log-sine sweep, 20Hz - 20kHz. Samplerate 48kHz, duration 15 seconds.")

    End Sub

    Private Sub Transducer_ComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Transducer_ComboBox.SelectedIndexChanged

        SelectedTransducer = Transducer_ComboBox.SelectedItem

        '(At this stage the sound player will be started, if not already done.)
        OstfBase.SoundPlayer.ChangePlayerSettings(SelectedTransducer.ParentAudioApiSettings, , 1, SelectedTransducer.Mixer,, True, True)

        'Adding channels
        SelectedChannel_ComboBox.Items.Clear()
        For c = 1 To SelectedTransducer.ParentAudioApiSettings.NumberOfOutputChannels
            SelectedChannel_ComboBox.Items.Add(c)
        Next
        If SelectedChannel_ComboBox.Items.Count > 0 Then SelectedChannel_ComboBox.SelectedIndex = 0

    End Sub

    Private Sub CalibrationSignal_ComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CalibrationSignal_ComboBox.SelectedIndexChanged

        CalibrationSignal_RichTextBox.Text = ""

        If CalibrationSignal_ComboBox.SelectedItem IsNot Nothing Then
            Dim SelectedCalibrationSound As Audio.Sound = CalibrationSignal_ComboBox.SelectedItem
            If CalibrationFileDescriptions.ContainsKey(SelectedCalibrationSound.FileName) Then
                CalibrationSignal_RichTextBox.Text = CalibrationFileDescriptions(SelectedCalibrationSound.FileName) & vbCrLf &
                    SelectedCalibrationSound.WaveFormat.ToString
            Else
                CalibrationSignal_RichTextBox.Text = "Calibration file without custom description." & vbCrLf &
                    SelectedCalibrationSound.WaveFormat.ToString
            End If
        End If

    End Sub

    Private Sub CalibrationLevel_ComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CalibrationLevel_ComboBox.SelectedIndexChanged
        SelectedLevel = CalibrationLevel_ComboBox.SelectedItem
    End Sub

    Private Sub SelectedChannelComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles SelectedChannel_ComboBox.SelectedIndexChanged
        SelectedChannel = SelectedChannel_ComboBox.SelectedItem
    End Sub


    Private Sub PlaySignal_Button_Click(sender As Object, e As EventArgs) Handles PlaySignal_Button.Click


        'Silencing any previously started calibration signal
        SilenceCalibrationTone()

        'Sets the SelectedCalibrationSound 
        Dim CalibrationSound As Audio.Sound = CalibrationSignal_ComboBox.SelectedItem

        If CalibrationSound Is Nothing Then
            MsgBox("Please select a calibration signal!", MsgBoxStyle.Exclamation, "Calibration")
            Exit Sub
        End If

        'Copies the sound 
        CalibrationSound = CalibrationSound.CreateCopy

        'Updates the wave format of the sound player
        OstfBase.SoundPlayer.ChangePlayerSettings(, CalibrationSound.WaveFormat)

        'Setting the signal level
        Audio.DSP.MeasureAndAdjustSectionLevel(CalibrationSound, Audio.PortAudioVB.DuplexMixer.Simulated_dBSPL_To_dBFS(SelectedLevel), 1)

        'Fading in and out
        Audio.DSP.Fade(CalibrationSound, Nothing, 0, 1, 0, 0.05 * CalibrationSound.WaveFormat.SampleRate, Audio.DSP.Transformations.FadeSlopeType.Smooth)
        Audio.DSP.Fade(CalibrationSound, 0, Nothing, 1, -0.05 * CalibrationSound.WaveFormat.SampleRate, Nothing, Audio.DSP.FadeSlopeType.Smooth)

        'Putting the sound in the intend channel
        Dim PlaySound = New Audio.Sound(New Audio.Formats.WaveFormat(CalibrationSound.WaveFormat.SampleRate, CalibrationSound.WaveFormat.BitDepth, SelectedTransducer.ParentAudioApiSettings.NumberOfOutputChannels,, CalibrationSound.WaveFormat.Encoding))
        PlaySound.WaveData.SampleData(SelectedChannel) = CalibrationSound.WaveData.SampleData(1)

        'Applying calibration gain
        Dim CalibrationGain = SelectedTransducer.Mixer.GetCalibrationGain(SelectedChannel)
        Audio.DSP.AmplifySection(CalibrationSound, CalibrationGain, SelectedChannel)

        'Plays the sound
        SoundPlayer.SwapOutputSounds(CalibrationSound)

    End Sub

    Private Sub StopSignal_Button_Click(sender As Object, e As EventArgs) Handles StopSignal_Button.Click
        SilenceCalibrationTone()
    End Sub

    Private Sub SilenceCalibrationTone()
        SoundPlayer.SwapOutputSounds(Nothing)
    End Sub

    Private Sub Close_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Close_Button.Click
        SoundPlayer.CloseStream()
        Me.Close()
    End Sub

End Class