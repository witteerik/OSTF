Public Class CalibrationForm

    Private ParentMixer As Audio.PortAudioVB.DuplexMixer
    Private SelectedChannel As Integer
    Private CalibrationFileDescriptions As New SortedList(Of String, String)

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Public Sub New(ByRef ParentMixer As Audio.PortAudioVB.DuplexMixer)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.ParentMixer = ParentMixer

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

        'Adding levels
        For Level = 60 To 80 Step 5
            CalibrationLevel_ComboBox.Items.Add(Level)
        Next
        CalibrationLevel_ComboBox.SelectedItem = 70

        'Adding output channels
        For c = 1 To ParentMixer.AvailableOutputChannels
            SelectedChannel_ComboBox.Items.Add(c)
        Next

    End Sub


    Private Sub Close_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Close_Button.Click
        Me.Close()
    End Sub


    Private Sub SelectedChannelComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles SelectedChannel_ComboBox.SelectedIndexChanged

        SelectedChannel = SelectedChannel_ComboBox.SelectedItem

        If SelectedChannel > -1 Then
            Close_Button.Enabled = True
        End If

    End Sub

    Private Sub CalibrationSignal_ComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CalibrationSignal_ComboBox.SelectedIndexChanged

        CalibrationSignal_RichTextBox.Text = ""

        If CalibrationSignal_ComboBox.SelectedItem IsNot Nothing Then

            Dim CurrentCalibrationSound As Audio.Sound = CalibrationSignal_ComboBox.SelectedItem
            If CalibrationFileDescriptions.ContainsKey(CurrentCalibrationSound.FileName) Then
                CalibrationSignal_RichTextBox.Text = CalibrationFileDescriptions(CurrentCalibrationSound.FileName) & vbCrLf &
                    CurrentCalibrationSound.WaveFormat.ToString
            Else
                CalibrationSignal_RichTextBox.Text = "Calibration file without custom description." & vbCrLf &
                    CurrentCalibrationSound.WaveFormat.ToString
            End If

        End If

    End Sub

    Private Enum CalibrationSoundType
        WarbleTone
        PinkNoise
    End Enum


    Private Sub PlayCalibration(ByVal CalibrationSoundType As CalibrationSoundType)

        Dim TargetCalibrationSignalLevel As Double?

        'Silencing any previously started calibration signal
        SilenceCalibrationTone()

        ''Creating a temporary TestSetup. This is needed since it calculates the calibration gain for the current environment (the same TestSoundMixerSettings file (and thus calibration) is used for all environments)
        'Dim TempTestSetup = New TestSetup(MySpeechTestControl.CurrentSpeechMaterialName)

        ''Creating a temporary Testsession, as this holds the sound player
        'TempTestSession = New ForcedChoiceTestSession(MySpeechTestControl, TempTestSetup, New TestSessionDescription(New PatientDetails With {.ID = "Calibration"}))

        ''Creating / loading a calibration signal
        Dim WaveFormat
        'Dim WaveFormat As New Audio.Formats.WaveFormat(TempTestSession.SoundPlayer.GetSampleRate,
        '                                       TempTestSession.SoundPlayer.BitDepth,
        '                                       TempTestSession.SoundPlayer.NumberOfOutputChannels)

        'Ask the user for a channel in which to play the calibration signal
        Dim CalibrationChannel As Integer
        Dim ChannelSelectionDialog As New CalibrationForm() 'TempTestSession.SoundPlayer.NumberOfOutputChannels)
        Dim DialogResult = ChannelSelectionDialog.ShowDialog
        If DialogResult = DialogResult.OK Then
            CalibrationChannel = ChannelSelectionDialog.SelectedChannel
        Else
            SilenceCalibrationTone()
            Exit Sub
        End If


        Dim CalibrationSound As Audio.Sound = Nothing

        Select Case CalibrationSoundType
            Case CalibrationSoundType.WarbleTone
                CalibrationSound = Audio.GenerateSound.CreateFrequencyModulatedSineWave(WaveFormat, CalibrationChannel, 1000, 0.5, 20, 0.125,, 30)

            Case CalibrationSoundType.PinkNoise
                Dim PinkNoiseSound = Audio.AudioIOs.ReadWaveFile("C:\SwedishSiBTest\SoundFiles\Calibration\Pink_Noise_Audacity_60_s.wav")
                CalibrationSound = New Audio.Sound(WaveFormat)
                CalibrationSound.WaveData.SampleData(CalibrationChannel) = PinkNoiseSound.WaveData.SampleData(1)

            Case Else
                Throw New NotImplementedException("Calibration sound type not implemented!")
        End Select

        'Setting the signal level
        Audio.DSP.MeasureAndAdjustSectionLevel(CalibrationSound, Audio.PortAudioVB.DuplexMixer.Simulated_dBSPL_To_dBFS(TargetCalibrationSignalLevel.Value), CalibrationChannel)

        'Fading in and out
        Audio.DSP.Fade(CalibrationSound, Nothing, 0, CalibrationChannel, 0, 0.05 * CalibrationSound.WaveFormat.SampleRate, Audio.DSP.Transformations.FadeSlopeType.Smooth)
        Audio.DSP.Fade(CalibrationSound, 0, Nothing, CalibrationChannel, CalibrationSound.WaveData.SampleData(CalibrationChannel).Length - 1 - 0.05 * CalibrationSound.WaveFormat.SampleRate, Nothing, Audio.DSP.FadeSlopeType.Smooth)

        'Applying calibration gain
        If CalibrationSound.WaveFormat.Channels < 4 Then

            Dim GainList As New List(Of Double)

            'Getting the gain values
            For c = 1 To CalibrationSound.WaveFormat.Channels
                GainList.Add(ParentMixer.GetCalibrationGain(c))
            Next

            'Skipping calibration gain if none is needed.
            Dim CalibrationIsNeeded As Boolean = False
            For Each GainValue In GainList
                If GainValue <> 0 Then CalibrationIsNeeded = True
            Next

            If CalibrationIsNeeded = True Then
                Audio.DSP.AmplifySection(CalibrationSound, GainList)
            End If

        Else

            'Using the slower overload of AmplifySection, as the faster one is not implemented for more than 3 channels
            For c = 1 To CalibrationSound.WaveFormat.Channels
                Dim CalibrationGain = ParentMixer.GetCalibrationGain(c)
                If CalibrationGain <> 0 Then
                    'Skipping calibration gain if none is needed.
                    Audio.DSP.AmplifySection(CalibrationSound, CalibrationGain, c)
                End If
            Next

        End If

        'Plays the sound
        'TempTestSession.SoundPlayer.SwapOutputSounds(CalibrationSound)


    End Sub


    Private Sub SilenceCalibrationTone()
        'If TempTestSession IsNot Nothing Then

        '    If TempTestSession.SoundPlayer IsNot Nothing Then
        '        'Immediately stops any output sound from the TempTestSession.SoundPlayer
        '        TempTestSession.SoundPlayer.Stop(False)
        '        TempTestSession.SoundPlayer.CloseStream()
        '        TempTestSession.SoundPlayer.Dispose()
        '    End If

        '    'Clears the TempTestSession 
        '    TempTestSession = Nothing
        'End If

    End Sub


End Class