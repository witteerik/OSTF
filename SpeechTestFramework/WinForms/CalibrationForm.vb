Public Class CalibrationForm

    Private SelectedChannel As Integer
    Private SelectedLevel As Double
    Private CalibrationFileDescriptions As New SortedList(Of String, String)
    Private SelectedTransducer As AudioSystemSpecification = Nothing
    Private DisposeSoundPlayerOnClose As Boolean
    Private UserType As Utils.UserTypes

    Public Sub New()
        MyClass.New(Utils.Constants.UserTypes.Research, True)
    End Sub

    ''' <summary>
    ''' Creates a new instance of CalibrationForm.
    ''' </summary>
    ''' <param name="DisposeSoundPlayerOnClose">Set to True if the calibration form is started as a standalone application. This will dispose the SoundPlayer on when the calibration form is closed. 
    ''' If the calibration form is launched from another OSTF application that uses the SoundPlayer, that application is instead responsible for disposing the SoundPlayer when closed.</param>
    Public Sub New(ByVal UserType As Utils.UserTypes, ByVal DisposeSoundPlayerOnClose As Boolean)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.DisposeSoundPlayerOnClose = DisposeSoundPlayerOnClose

        ' Add any initialization after the InitializeComponent() call.
        Me.UserType = UserType

        Dim UserTypeString As String = ""
        Select Case UserType
            Case Utils.Constants.UserTypes.Research
                UserTypeString = "Research version"
            Case Else
                UserTypeString = ""
        End Select

        Me.Text = "OSTA Sound Level Calibration" & " - " & UserTypeString

    End Sub


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
        GeneratedWarble.Description = "Warble tone (60 s)"
        CalibrationSignal_ComboBox.Items.Add(GeneratedWarble)
        CalibrationFileDescriptions.Add("Internal1", "OSTF generated warble tone. The tone is frequency modulated around 1 kHz by ±12.5 %, with a modulation frequency of 20 Hz. Samplerate 48kHz, duration 60 seconds.")

        Dim GeneratedSine = Audio.GenerateSound.CreateSineWave(TempWaveFormat, 1, 1000, 0.5, , 60)
        GeneratedSine.FileName = "Internal2"
        GeneratedSine.Description = "Sine"
        CalibrationSignal_ComboBox.Items.Add(GeneratedSine)
        CalibrationFileDescriptions.Add("Internal2", "OSTF generated 1kHz sine. Samplerate 48kHz, duration 60 seconds.")

        Dim GeneratedSweep1 = Audio.GenerateSound.CreateLogSineSweep(TempWaveFormat, 1, 20, 20000, False, 0.5,, 15)
        GeneratedSweep1.FileName = "Internal3"
        GeneratedSweep1.Description = "Sweep"
        CalibrationSignal_ComboBox.Items.Add(GeneratedSweep1)
        CalibrationFileDescriptions.Add("Internal3", "OSTF generated log-sine sweep, 20Hz - 20kHz. Samplerate 48kHz, duration 15 seconds.")

        Dim GeneratedSweep2 = Audio.GenerateSound.CreateLogSineSweep(TempWaveFormat, 1, 20, 20000, True, 0.5,, 15)
        GeneratedSweep2.FileName = "Internal4"
        GeneratedSweep2.Description = "Sweep (flat)"
        CalibrationSignal_ComboBox.Items.Add(GeneratedSweep2)
        CalibrationFileDescriptions.Add("Internal4", "OSTF generated (flat spectrum) log-sine sweep, 20Hz - 20kHz. Samplerate 48kHz, duration 15 seconds.")

    End Sub

    Private Sub Transducer_ComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Transducer_ComboBox.SelectedIndexChanged

        SoundSystem_RichTextBox.Text = ""

        SelectedTransducer = Transducer_ComboBox.SelectedItem

        If SelectedTransducer.CanPlay = True Then
            '(At this stage the sound player will be started, if not already done.)
            OstfBase.SoundPlayer.ChangePlayerSettings(SelectedTransducer.ParentAudioApiSettings, , 0.1, SelectedTransducer.Mixer,, True, True)
            PlaySignal_Button.Enabled = True
        Else
            MsgBox("Unable to start the player using the selected transducer (probably the selected output device doesn't have enough output channels?)!", MsgBoxStyle.Exclamation, "Sound player failure")
            PlaySignal_Button.Enabled = False
        End If

        'Adding description
        SoundSystem_RichTextBox.Text = SelectedTransducer.GetDescriptionString

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

        Try

            'Silencing any previously started calibration signal
            SilenceCalibrationTone()

            If SelectedTransducer.CanPlay = True Then

                'Sets the SelectedCalibrationSound 
                Dim CalibrationSound As Audio.Sound = CalibrationSignal_ComboBox.SelectedItem

                If CalibrationSound Is Nothing Then
                    MsgBox("Please select a calibration signal!", MsgBoxStyle.Exclamation, "Calibration")
                    Exit Sub
                End If

                'Copies the sound 
                CalibrationSound = CalibrationSound.CreateCopy

                'Converts 16 bit PCM to 32 float
                If CalibrationSound.WaveFormat.BitDepth = 16 And CalibrationSound.WaveFormat.Encoding = Audio.Formats.WaveFormat.WaveFormatEncodings.PCM Then
                    CalibrationSound = CalibrationSound.Convert16to32bitSound
                    'Any other attempted format, except 32 bit IEEE, will be stopped by the player.
                End If

                'Updates the wave format of the sound player
                OstfBase.SoundPlayer.ChangePlayerSettings(, CalibrationSound.WaveFormat)

                'Setting the signal level
                Audio.DSP.MeasureAndAdjustSectionLevel(CalibrationSound, Audio.PortAudioVB.DuplexMixer.Simulated_dBSPL_To_dBFS(SelectedLevel), 1)

                'Fading in and out
                Audio.DSP.Fade(CalibrationSound, Nothing, 0, 1, 0, 0.02 * CalibrationSound.WaveFormat.SampleRate, Audio.DSP.Transformations.FadeSlopeType.Smooth)
                Audio.DSP.Fade(CalibrationSound, 0, Nothing, 1, -0.02 * CalibrationSound.WaveFormat.SampleRate, Nothing, Audio.DSP.FadeSlopeType.Smooth)

                'Putting the sound in the intended channel
                Dim PlaySound = New Audio.Sound(New Audio.Formats.WaveFormat(CalibrationSound.WaveFormat.SampleRate, CalibrationSound.WaveFormat.BitDepth, SelectedTransducer.ParentAudioApiSettings.NumberOfOutputChannels,, CalibrationSound.WaveFormat.Encoding))
                PlaySound.WaveData.SampleData(SelectedChannel) = CalibrationSound.WaveData.SampleData(1)
                'PlaySound.WaveData.EnforceEqualChannelLength()

                'Applying calibration gain
                Dim CalibrationGain = SelectedTransducer.Mixer.GetCalibrationGain(SelectedChannel)
                Audio.DSP.AmplifySection(PlaySound, CalibrationGain, SelectedChannel)

                'Plays the sound
                SoundPlayer.SwapOutputSounds(PlaySound)

            Else


            End If

        Catch ex As Exception
            MsgBox("An error occurred!", MsgBoxStyle.Exclamation, "Calibration")
        End Try

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

    Private Sub Help_Button_Click(sender As Object, e As EventArgs) Handles Help_Button.Click
        ShowHelp()
    End Sub

    Private Sub ShowHelp()

        Dim InstructionsForm As New InfoForm

        Dim CalibrationInfoString As String = "Instructions on how to perform calibration

1. Select the sound system that you want to calibrate.
2. Select the desired calibration signal. (If you want to use your own signal, just locate the folder 'OSTF\CalibrationSignals\' and put your signal there, and restart the app. The signal needs to be stored in 32-bit IEEE/float or 16-bit PCM format.)
3. Select a calibration signal level (in dB SPL) to present.
4. Select which output speaker channel you want to calibrate.
5. Use a sound level meter and measure the sound level at the desired measurement point.
6. Calculate the difference between the selected calibration signal level and the measured level.
7. Adjust the calibration value ('Calibration_FsToSpl') for the output channel in the file OSTF\AudioSystem\AudioSystemSpecification.txt by the calculated difference. 
8. Do the same for all speaker channels.
9. Restart the app to load the new calibration values and measure the calibration levels again to ensure correct calibration."

        InstructionsForm.SetInfo(CalibrationInfoString, "How to calibrate")
        InstructionsForm.Show()

    End Sub

    Private Sub CalibrationForm_FormClosing(sender As Object, e As Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        'Disposing the sound player. Note that this means that 
        If DisposeSoundPlayerOnClose = True Then If SoundPlayer IsNot Nothing Then SoundPlayer.Dispose()
    End Sub

    Private Sub AboutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem.Click

        Dim AboutBox = New AboutBox_WithLicenseButton
        AboutBox.SelectedLicense = LicenseBox.AvailableLicenses.MIT_X11
        AboutBox.LicenseAdditions.Add(LicenseBox.AvailableLicenseAdditions.PortAudio)
        AboutBox.ShowDialog()

    End Sub

    Private Sub ViewAvailableSoundDevicesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewAvailableSoundDevicesToolStripMenuItem.Click

        Dim SoundDevicesForm As New InfoForm
        Dim DeviceInfoString As String = "Currently available sound devices:" & vbCrLf & vbCrLf & Audio.AudioApiSettings.GetAllAvailableDevices
        SoundDevicesForm.SetInfo(DeviceInfoString, "Available sound devices")
        SoundDevicesForm.Show()

    End Sub

    Private Sub HelpToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HelpToolStripMenuItem.Click
        ShowHelp()
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()
    End Sub

End Class