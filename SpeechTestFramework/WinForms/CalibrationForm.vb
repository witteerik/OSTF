Imports SpeechTestFramework.Audio
Imports SpeechTestFramework.Audio.Formats

Public Class CalibrationForm

    Public ReadOnly Property IsStandAlone As Boolean

    Private SelectedHardwareOutputChannel As Integer ' Left channel if used with sound field simulation, otherwise just the output channel
    Private SelectedHardwareOutputChannel_Right As Integer ' Only used with field simulation

    Private SelectedLevel As Double
    Private CalibrationFileDescriptions As New SortedList(Of String, String)
    Private SelectedTransducer As AudioSystemSpecification = Nothing
    Private UserType As Utils.UserTypes

    Private CalibrationFilesDirectory As String = ""

    Private NoSimulationString As String = "No simulation"

    Public Sub New()
        MyClass.New(Utils.Constants.UserTypes.Research, True)
    End Sub

    ''' <summary>
    ''' Creates a new instance of CalibrationForm.
    ''' </summary>
    ''' <param name="IsStandAlone">Set to true if called from within another OSTF application, and False if run as a standalone OSTF application. </param>
    Public Sub New(ByVal UserType As Utils.UserTypes, ByVal IsStandAlone As Boolean)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.IsStandAlone = IsStandAlone

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

        'Adds frequency weightings
        FrequencyWeighting_ComboBox.Items.Add(Audio.FrequencyWeightings.Z)
        FrequencyWeighting_ComboBox.Items.Add(Audio.FrequencyWeightings.C)
        FrequencyWeighting_ComboBox.SelectedIndex = 0

    End Sub


    Private Sub CalibrationForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'Adding transducers
        Dim TempWaveFormat As New Audio.Formats.WaveFormat(48000, 32, 1,, Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints)
        OstfBase.SoundPlayer.ChangePlayerSettings(, TempWaveFormat.SampleRate, TempWaveFormat.BitDepth, TempWaveFormat.Encoding,,, Audio.SoundPlayers.iSoundPlayer.SoundDirections.PlaybackOnly, False, False)

        Dim LocalAvailableTransducers = OstfBase.AvaliableTransducers
        If LocalAvailableTransducers.Count = 0 Then
            MsgBox("Unable to start the application since no sound transducers could be found!", MsgBoxStyle.Critical, "Calibration")
        End If

        'Adding transducers to the combobox, and selects the first one
        For Each Transducer In LocalAvailableTransducers
            Transducer_ComboBox.Items.Add(Transducer)
        Next
        Transducer_ComboBox.SelectedIndex = 0

        'Adding signals
        CalibrationFilesDirectory = IO.Path.Combine(OstfBase.MediaRootDirectory, OstfBase.CalibrationSignalSubDirectory)
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
            OstfBase.SoundPlayer.ChangePlayerSettings(SelectedTransducer.ParentAudioApiSettings,,, , 0.3, SelectedTransducer.Mixer,, True, True)
            PlaySignal_Button.Enabled = True

        Else
            MsgBox("Unable to start the player using the selected transducer (probably the selected output device doesn't have enough output channels?)!", MsgBoxStyle.Exclamation, "Sound player failure")
            PlaySignal_Button.Enabled = False
        End If

        'Adding description
        SoundSystem_RichTextBox.Text = SelectedTransducer.GetDescriptionString

        'Adding channels
        SelectedHardWareOutputChannel_ComboBox.Items.Clear()
        SelectedHardWareOutputChannel_Right_ComboBox.Items.Clear()
        For Each c In SelectedTransducer.Mixer.OutputRouting.Keys
            SelectedHardWareOutputChannel_ComboBox.Items.Add(c)
            SelectedHardWareOutputChannel_Right_ComboBox.Items.Add(c)
        Next
        If SelectedHardWareOutputChannel_ComboBox.Items.Count > 0 Then
            SelectedHardWareOutputChannel_ComboBox.SelectedIndex = 0
        End If
        If SelectedHardWareOutputChannel_Right_ComboBox.Items.Count > 1 Then
            SelectedHardWareOutputChannel_Right_ComboBox.SelectedIndex = 1
        End If

    End Sub

    Private Sub CalibrationSignal_ComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CalibrationSignal_ComboBox.SelectedIndexChanged

        If SelectedTransducer Is Nothing Then Exit Sub

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

            'Checks if signal FS is 48 kHz. If not disables the FrequencyWeighting_ComboBox, and sets its selected value to Z-weighting
            If SelectedCalibrationSound.WaveFormat.SampleRate = 48000 Then
                FrequencyWeighting_ComboBox.Enabled = True
            Else
                FrequencyWeighting_ComboBox.SelectedIndex = 0
                FrequencyWeighting_ComboBox.Enabled = False
            End If

            'Clearing previously added DirectionalSimulationSets
            DirectionalSimulationSet_ComboBox.Items.Clear()
            SimulatedDistance_ComboBox.Items.Clear()

            'Adding available DirectionalSimulationSets
            Dim AvailableSets = DirectionalSimulator.GetAvailableDirectionalSimulationSets(SelectedTransducer, SelectedCalibrationSound.WaveFormat.SampleRate)
            AvailableSets.Insert(0, NoSimulationString)
            For Each Item In AvailableSets
                DirectionalSimulationSet_ComboBox.Items.Add(Item)
            Next
            DirectionalSimulationSet_ComboBox.SelectedIndex = 0

        End If

    End Sub

    Private Sub DirectionalSimulationSet_ComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DirectionalSimulationSet_ComboBox.SelectedIndexChanged

        SimulatedDistance_ComboBox.Items.Clear()
        SimulatedDistance_ComboBox.ResetText()

        Dim SelectedItem = DirectionalSimulationSet_ComboBox.SelectedItem
        If SelectedItem IsNot Nothing Then

            If SelectedItem = NoSimulationString Then
                DirectionalSimulator.ClearSelectedDirectionalSimulationSet()
                SelectedHardWareOutputChannel_Right_ComboBox.Enabled = False
                RightChannel_Label.Enabled = False
                'SelectedSoundPropagationType = SoundPropagationTypes.PointSpeakers
            Else
                'SelectedSoundPropagationType = SoundPropagationTypes.SimulatedSoundField
                DirectionalSimulator.TrySetSelectedDirectionalSimulationSet(SelectedItem, SelectedTransducer, False)
                SelectedHardWareOutputChannel_Right_ComboBox.Enabled = True
                RightChannel_Label.Enabled = True
            End If

            'Adding available simulation distances
            Dim AvailableDistances = DirectionalSimulator.GetAvailableDirectionalSimulationSetDistances(SelectedItem)
            For Each Distance In AvailableDistances
                SimulatedDistance_ComboBox.Items.Add(Distance)
            Next
            If SimulatedDistance_ComboBox.Items.Count > 0 Then SimulatedDistance_ComboBox.SelectedIndex = 0

        Else
            DirectionalSimulator.ClearSelectedDirectionalSimulationSet()
        End If

    End Sub




    Private Sub CalibrationLevel_ComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CalibrationLevel_ComboBox.SelectedIndexChanged
        SelectedLevel = CalibrationLevel_ComboBox.SelectedItem
    End Sub

    Private Sub SelectedChannelComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles SelectedHardWareOutputChannel_ComboBox.SelectedIndexChanged
        SelectedHardwareOutputChannel = SelectedHardWareOutputChannel_ComboBox.SelectedItem
    End Sub

    Private Sub SelectedRightChannelComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles SelectedHardWareOutputChannel_Right_ComboBox.SelectedIndexChanged
        SelectedHardwareOutputChannel_Right = SelectedHardWareOutputChannel_Right_ComboBox.SelectedItem
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
                OstfBase.SoundPlayer.ChangePlayerSettings(, CalibrationSound.WaveFormat.SampleRate, CalibrationSound.WaveFormat.BitDepth, CalibrationSound.WaveFormat.Encoding)

                'Setting the signal level
                Audio.DSP.MeasureAndAdjustSectionLevel(CalibrationSound, Audio.Standard_dBSPL_To_dBFS(SelectedLevel), 1,,, FrequencyWeighting_ComboBox.SelectedItem)

                'Fading in and out
                Audio.DSP.Fade(CalibrationSound, Nothing, 0, 1, 0, 0.02 * CalibrationSound.WaveFormat.SampleRate, Audio.DSP.Transformations.FadeSlopeType.Smooth)
                Audio.DSP.Fade(CalibrationSound, 0, Nothing, 1, -0.02 * CalibrationSound.WaveFormat.SampleRate, Nothing, Audio.DSP.FadeSlopeType.Smooth)

                Dim PlaySound As Audio.Sound = Nothing

                If DirectionalSimulator.IsActive = True Then

                    Dim SelectedSimulatedDistance As Double
                    If SimulatedDistance_ComboBox.SelectedItem IsNot Nothing Then
                        SelectedSimulatedDistance = SimulatedDistance_ComboBox.SelectedItem
                    Else
                        MsgBox("Please select a directional simulation distance!")
                        Exit Sub
                    End If

                    Dim SimulationKernel = DirectionalSimulator.GetStereoKernel(DirectionalSimulator.SelectedDirectionalSimulationSetName, 0, 0, SelectedSimulatedDistance)
                    Dim CurrentKernel = SimulationKernel.BinauralIR.CreateSoundDataCopy

                    Dim StereoCalibrationSound = New Audio.Sound(New WaveFormat(CalibrationSound.WaveFormat.SampleRate, CalibrationSound.WaveFormat.BitDepth, 2,, CalibrationSound.WaveFormat.Encoding))
                    Dim LeftChannelSound = CalibrationSound.CreateSoundDataCopy()
                    Dim RightChannelSound = CalibrationSound.CreateSoundDataCopy()
                    StereoCalibrationSound.WaveData.SampleData(1) = LeftChannelSound.WaveData.SampleData(1)
                    StereoCalibrationSound.WaveData.SampleData(2) = RightChannelSound.WaveData.SampleData(1)

                    'Applies FIR-filtering
                    'StereoCalibrationSound.WriteWaveFile(IO.Path.Combine(Utils.logFilePath, "CalibSound_PreDirFilter"))
                    Dim FilteredSound = Audio.DSP.FIRFilter(StereoCalibrationSound, CurrentKernel, New Formats.FftFormat,,,,,, True)
                    'FilteredSound.WriteWaveFile(IO.Path.Combine(Utils.logFilePath, "CalibSound_PostDirFilter"))

                    'Putting the sound in the intended channels
                    PlaySound = New Audio.Sound(New Audio.Formats.WaveFormat(FilteredSound.WaveFormat.SampleRate, FilteredSound.WaveFormat.BitDepth, SelectedTransducer.NumberOfApiOutputChannels,, FilteredSound.WaveFormat.Encoding))
                    PlaySound.WaveData.SampleData(SelectedTransducer.Mixer.OutputRouting(SelectedHardwareOutputChannel)) = FilteredSound.WaveData.SampleData(1)
                    PlaySound.WaveData.SampleData(SelectedTransducer.Mixer.OutputRouting(SelectedHardwareOutputChannel_Right)) = FilteredSound.WaveData.SampleData(2)

                Else

                    'Putting the sound in the intended channel
                    PlaySound = New Audio.Sound(New Audio.Formats.WaveFormat(CalibrationSound.WaveFormat.SampleRate, CalibrationSound.WaveFormat.BitDepth, SelectedTransducer.NumberOfApiOutputChannels,, CalibrationSound.WaveFormat.Encoding))
                    PlaySound.WaveData.SampleData(SelectedTransducer.Mixer.OutputRouting(SelectedHardwareOutputChannel)) = CalibrationSound.WaveData.SampleData(1)

                End If

                'PlaySound = Audio.DSP.IIRFilter(PlaySound, FrequencyWeightings.C)
                'PlaySound.WriteWaveFile(IO.Path.Combine(Utils.logFilePath, "CalibSound_NS_C"))

                'Plays the sound
                SoundPlayer.SwapOutputSounds(PlaySound)

            Else
                MsgBox("Unable to start the player using the selected transducer!", MsgBoxStyle.Exclamation, "Sound player failure")
                PlaySignal_Button.Enabled = False
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

        Dim AudioSystemSpecificationFilePath = IO.Path.Combine(OstfBase.MediaRootDirectory, OstfBase.AudioSystemSettingsFile)

        Dim CalibrationInfoString As String = "Instructions on how to perform calibration

1. Select the sound system that you want to calibrate.
2. Select the desired calibration signal. (If you want to use your own signal, just locate the folder '" & CalibrationFilesDirectory & "' and put your signal there, and restart the app. The signal needs to be stored in 32-bit IEEE/float or 16-bit PCM format.)
3. Select a calibration signal level to present.
4. Select a frequency weighting. If set to 'Z' the unit of the selected signal level will be dB SPL, if set to 'C' it will instead be in dB C. The C-weighting option is only available for calibration signals with a sample rate of 48 kHz.
5. Select which output speaker channel you want to calibrate.
6. Use a sound level meter and measure the sound level at the desired measurement point.
7. Calculate the difference between the selected calibration signal level and the measured level.
8. Adjust the calibration value ('CalibrationGain') for the output channel in the file '" & AudioSystemSpecificationFilePath & "' by the calculated difference. 
9. Do the same for all speaker channels.
10. Restart the app to load the new calibration values and measure the calibration levels again to ensure correct calibration."

        InstructionsForm.SetInfo(CalibrationInfoString, "How to calibrate")
        InstructionsForm.Show()

    End Sub

    Private Sub CalibrationForm_FormClosing(sender As Object, e As Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        If IsStandAlone = True Then OstfBase.TerminateOSTF()
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