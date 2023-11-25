
public partial class CalibrationForm
{

    public bool IsStandAlone { get; private set; }

    private int SelectedHardwareOutputChannel; // Left channel if used with sound field simulation, otherwise just the output channel
    private int SelectedHardwareOutputChannel_Right; // Only used with field simulation

    private double SelectedLevel;
    private SortedList<string, string> CalibrationFileDescriptions = new SortedList<string, string>();
    private SpeechTestFramework.OstfBase.AudioSystemSpecification SelectedTransducer = (SpeechTestFramework.OstfBase.AudioSystemSpecification)null;
    private SpeechTestFramework.Utils.Constants.UserTypes UserType;

    private string CalibrationFilesDirectory = "";

    private string NoSimulationString = "No simulation";

    public CalibrationForm() : this(SpeechTestFramework.Utils.Constants.UserTypes.Research, true)
    {
    }

    /// <summary>
    /// Creates a new instance of CalibrationForm.
    /// </summary>
    /// <param name="IsStandAlone">Set to true if called from within another OSTF application, and False if run as a standalone OSTF application. </param>
    public CalibrationForm(SpeechTestFramework.Utils.Constants.UserTypes UserType, bool IsStandAlone)
    {

        // This call is required by the designer.
        this.InitializeComponent();

        // Add any initialization after the InitializeComponent() call.
        this.IsStandAlone = IsStandAlone;

        // Add any initialization after the InitializeComponent() call.
        this.UserType = UserType;

        string UserTypeString = "";
        switch (UserType)
        {
            case SpeechTestFramework.Utils.Constants.UserTypes.Research:
                {
                    UserTypeString = "Research version";
                    break;
                }

            default:
                {
                    UserTypeString = "";
                    break;
                }
        }

        this.Text = "OSTA Sound Level Calibration" + " - " + UserTypeString;

        // Adds frequency weightings
        this.FrequencyWeighting_ComboBox.Items.Add(SpeechTestFramework.Audio.BasicAudioEnums.FrequencyWeightings.Z);
        this.FrequencyWeighting_ComboBox.Items.Add(SpeechTestFramework.Audio.BasicAudioEnums.FrequencyWeightings.C);
        this.FrequencyWeighting_ComboBox.SelectedIndex = 0;

    }


    private void CalibrationForm_Load(object sender, EventArgs e)
    {

        // Adding transducers
        var TempWaveFormat = new SpeechTestFramework.Audio.Formats.WaveFormat(48000, 32, 1, Encoding: SpeechTestFramework.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints);
        SpeechTestFramework.OstfBase.SoundPlayer.ChangePlayerSettings(SampleRate: (int?)TempWaveFormat.SampleRate, BitDepth: TempWaveFormat.BitDepth, Encoding: TempWaveFormat.Encoding, SoundDirection: SpeechTestFramework.Audio.SoundPlayers.iSoundPlayer.SoundDirections.PlaybackOnly, ReOpenStream: false, ReStartStream: false);

        var LocalAvailableTransducers = SpeechTestFramework.OstfBase.AvaliableTransducers;
        if (LocalAvailableTransducers.Count == 0)
        {
            Interaction.MsgBox("Unable to start the application since no sound transducers could be found!", MsgBoxStyle.Critical, "Calibration");
        }

        // Adding transducers to the combobox, and selects the first one
        foreach (var Transducer in LocalAvailableTransducers)
            this.Transducer_ComboBox.Items.Add(Transducer);
        this.Transducer_ComboBox.SelectedIndex = 0;

        // Adding signals
        CalibrationFilesDirectory = System.IO.Path.Combine(SpeechTestFramework.OstfBase.MediaRootDirectory, SpeechTestFramework.OstfBase.CalibrationSignalSubDirectory);
        string[] CalibrationFiles = System.IO.Directory.GetFiles(CalibrationFilesDirectory);

        // Getting calibration file descriptions from the text file SignalDescriptions.txt
        string DescriptionFile = System.IO.Path.Combine(CalibrationFilesDirectory, "SignalDescriptions.txt");
        string[] InputLines = System.IO.File.ReadAllLines(DescriptionFile, System.Text.Encoding.UTF8);
        foreach (var line in InputLines)
        {
            if (string.IsNullOrEmpty(line.Trim()))
                continue;
            if (line.Trim().StartsWith("//"))
                continue;
            string[] LineSplit = line.Trim().Split('=');
            if (LineSplit.Length < 2)
                continue;
            if (string.IsNullOrEmpty(LineSplit[0].Trim()))
                continue;
            if (string.IsNullOrEmpty(LineSplit[1].Trim()))
                continue;
            // Adds the description (filename, description)
            CalibrationFileDescriptions.Add(System.IO.Path.GetFileNameWithoutExtension(LineSplit[0].Trim()), LineSplit[1].Trim());
        }

        // Adding sound files
        var CalibrationSounds = new List<SpeechTestFramework.Audio.Sound>();
        foreach (var File in CalibrationFiles)
        {
            if (System.IO.Path.GetExtension(File) == ".wav")
            {
                var NewCalibrationSound = SpeechTestFramework.Audio.Sound.LoadWaveFile(File);
                NewCalibrationSound.Description = System.IO.Path.GetFileNameWithoutExtension(File).Replace("_", " ");
                if (NewCalibrationSound is not null)
                    CalibrationSounds.Add(NewCalibrationSound);
            }
        }

        // Adding into CalibrationSignal_ComboBox
        foreach (var CalibrationSound in CalibrationSounds)
            this.CalibrationSignal_ComboBox.Items.Add(CalibrationSound);
        if (this.CalibrationSignal_ComboBox.Items.Count > 0)
        {
            this.CalibrationSignal_ComboBox.SelectedIndex = 0;
        }

        // Adding internally generated sounds 
        AddInternallyGeneratedSounds();

        // Adding levels
        for (int Level = 60; Level <= 80; Level += 5)
            this.CalibrationLevel_ComboBox.Items.Add((object)Level);
        this.CalibrationLevel_ComboBox.SelectedItem = (object)70;

    }

    public void AddInternallyGeneratedSounds()
    {

        var TempWaveFormat = new SpeechTestFramework.Audio.Formats.WaveFormat(48000, 32, 1, Encoding: SpeechTestFramework.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints);

        var GeneratedWarble = SpeechTestFramework.Audio.GenerateSound.Signals.CreateFrequencyModulatedSineWave(ref TempWaveFormat, 1, 1000d, 0.5m, 20d, 0.125d, duration: 60d);
        GeneratedWarble.FileName = "Internal1";
        GeneratedWarble.Description = "Warble tone (60 s)";
        this.CalibrationSignal_ComboBox.Items.Add(GeneratedWarble);
        CalibrationFileDescriptions.Add("Internal1", "OSTF generated warble tone. The tone is frequency modulated around 1 kHz by ±12.5 %, with a modulation frequency of 20 Hz. Samplerate 48kHz, duration 60 seconds.");

        var GeneratedSine = SpeechTestFramework.Audio.GenerateSound.Signals.CreateSineWave(ref TempWaveFormat, 1, 1000d, 0.5m, duration: 60d);
        GeneratedSine.FileName = "Internal2";
        GeneratedSine.Description = "Sine";
        this.CalibrationSignal_ComboBox.Items.Add(GeneratedSine);
        CalibrationFileDescriptions.Add("Internal2", "OSTF generated 1kHz sine. Samplerate 48kHz, duration 60 seconds.");

        var GeneratedSweep1 = SpeechTestFramework.Audio.GenerateSound.SignalsExt.CreateLogSineSweep(ref TempWaveFormat, 1, 20d, 20000d, false, 0.5m, TotalDuration: 15d);
        GeneratedSweep1.FileName = "Internal3";
        GeneratedSweep1.Description = "Sweep";
        this.CalibrationSignal_ComboBox.Items.Add(GeneratedSweep1);
        CalibrationFileDescriptions.Add("Internal3", "OSTF generated log-sine sweep, 20Hz - 20kHz. Samplerate 48kHz, duration 15 seconds.");

        var GeneratedSweep2 = SpeechTestFramework.Audio.GenerateSound.SignalsExt.CreateLogSineSweep(ref TempWaveFormat, 1, 20d, 20000d, true, 0.5m, TotalDuration: 15d);
        GeneratedSweep2.FileName = "Internal4";
        GeneratedSweep2.Description = "Sweep (flat)";
        this.CalibrationSignal_ComboBox.Items.Add(GeneratedSweep2);
        CalibrationFileDescriptions.Add("Internal4", "OSTF generated (flat spectrum) log-sine sweep, 20Hz - 20kHz. Samplerate 48kHz, duration 15 seconds.");

    }


    private void Transducer_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {

        this.SoundSystem_RichTextBox.Text = "";

        SelectedTransducer = (SpeechTestFramework.OstfBase.AudioSystemSpecification)this.Transducer_ComboBox.SelectedItem;

        if (SelectedTransducer.CanPlay == true)
        {
            // (At this stage the sound player will be started, if not already done.)
            var argAudioApiSettings = SelectedTransducer.ParentAudioApiSettings;
            var argMixer = SelectedTransducer.Mixer;
            SpeechTestFramework.OstfBase.SoundPlayer.ChangePlayerSettings(ref argAudioApiSettings, OverlapDuration: 0.3d, Mixer: ref argMixer, ReOpenStream: true, ReStartStream: true);
            SelectedTransducer.Mixer = argMixer;
            this.PlaySignal_Button.Enabled = true;
        }

        else
        {
            Interaction.MsgBox("Unable to start the player using the selected transducer (probably the selected output device doesn't have enough output channels?)!", MsgBoxStyle.Exclamation, "Sound player failure");
            this.PlaySignal_Button.Enabled = false;
        }

        // Adding description
        this.SoundSystem_RichTextBox.Text = SelectedTransducer.GetDescriptionString();

        // Adding channels
        this.SelectedHardWareOutputChannel_ComboBox.Items.Clear();
        this.SelectedHardWareOutputChannel_Right_ComboBox.Items.Clear();
        foreach (var c in SelectedTransducer.Mixer.OutputRouting.Keys)
        {
            this.SelectedHardWareOutputChannel_ComboBox.Items.Add((object)c);
            this.SelectedHardWareOutputChannel_Right_ComboBox.Items.Add((object)c);
        }
        if (this.SelectedHardWareOutputChannel_ComboBox.Items.Count > 0)
        {
            this.SelectedHardWareOutputChannel_ComboBox.SelectedIndex = 0;
        }
        if (this.SelectedHardWareOutputChannel_Right_ComboBox.Items.Count > 1)
        {
            this.SelectedHardWareOutputChannel_Right_ComboBox.SelectedIndex = 1;
        }

    }

    private void CalibrationSignal_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {

        if (SelectedTransducer is null)
            return;

        this.CalibrationSignal_RichTextBox.Text = "";

        if (this.CalibrationSignal_ComboBox.SelectedItem is not null)
        {
            SpeechTestFramework.Audio.Sound SelectedCalibrationSound = (SpeechTestFramework.Audio.Sound)this.CalibrationSignal_ComboBox.SelectedItem;
            if (CalibrationFileDescriptions.ContainsKey(SelectedCalibrationSound.FileName))
            {
                this.CalibrationSignal_RichTextBox.Text = CalibrationFileDescriptions[SelectedCalibrationSound.FileName] + Constants.vbCrLf + SelectedCalibrationSound.WaveFormat.ToString();
            }
            else
            {
                this.CalibrationSignal_RichTextBox.Text = "Calibration file without custom description." + Constants.vbCrLf + SelectedCalibrationSound.WaveFormat.ToString();
            }

            // Checks if signal FS is 48 kHz. If not disables the FrequencyWeighting_ComboBox, and sets its selected value to Z-weighting
            if ((long)SelectedCalibrationSound.WaveFormat.SampleRate == 48000L)
            {
                this.FrequencyWeighting_ComboBox.Enabled = true;
            }
            else
            {
                this.FrequencyWeighting_ComboBox.SelectedIndex = 0;
                this.FrequencyWeighting_ComboBox.Enabled = false;
            }

            // Clearing previously added DirectionalSimulationSets
            this.DirectionalSimulationSet_ComboBox.Items.Clear();
            this.SimulatedDistance_ComboBox.Items.Clear();

            // Adding available DirectionalSimulationSets
            var AvailableSets = SpeechTestFramework.OstfBase.DirectionalSimulator.GetAvailableDirectionalSimulationSets(ref SelectedTransducer, (int)SelectedCalibrationSound.WaveFormat.SampleRate);
            AvailableSets.Insert(0, NoSimulationString);
            foreach (var Item in AvailableSets)
                this.DirectionalSimulationSet_ComboBox.Items.Add(Item);
            this.DirectionalSimulationSet_ComboBox.SelectedIndex = 0;

        }

    }

    private void DirectionalSimulationSet_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {

        this.SimulatedDistance_ComboBox.Items.Clear();
        this.SimulatedDistance_ComboBox.ResetText();

        var SelectedItem = this.DirectionalSimulationSet_ComboBox.SelectedItem;
        if (SelectedItem is not null)
        {

            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(SelectedItem, NoSimulationString, false)))
            {
                SpeechTestFramework.OstfBase.DirectionalSimulator.ClearSelectedDirectionalSimulationSet();
                this.SelectedHardWareOutputChannel_Right_ComboBox.Enabled = false;
                this.RightChannel_Label.Enabled = false;
            }
            // SelectedSoundPropagationType = SoundPropagationTypes.PointSpeakers
            else
            {
                // SelectedSoundPropagationType = SoundPropagationTypes.SimulatedSoundField
                SpeechTestFramework.OstfBase.DirectionalSimulator.TrySetSelectedDirectionalSimulationSet(Conversions.ToString(SelectedItem), ref SelectedTransducer);
                this.SelectedHardWareOutputChannel_Right_ComboBox.Enabled = true;
                this.RightChannel_Label.Enabled = true;
            }

            // Adding available simulation distances
            var AvailableDistances = SpeechTestFramework.OstfBase.DirectionalSimulator.GetAvailableDirectionalSimulationSetDistances(Conversions.ToString(SelectedItem));
            foreach (var Distance in AvailableDistances)
                this.SimulatedDistance_ComboBox.Items.Add((object)Distance);
            if (this.SimulatedDistance_ComboBox.Items.Count > 0)
                this.SimulatedDistance_ComboBox.SelectedIndex = 0;
        }

        else
        {
            SpeechTestFramework.OstfBase.DirectionalSimulator.ClearSelectedDirectionalSimulationSet();
        }

    }




    private void CalibrationLevel_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        SelectedLevel = Conversions.ToDouble(this.CalibrationLevel_ComboBox.SelectedItem);
    }

    private void SelectedChannelComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        SelectedHardwareOutputChannel = Conversions.ToInteger(this.SelectedHardWareOutputChannel_ComboBox.SelectedItem);
    }

    private void SelectedRightChannelComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        SelectedHardwareOutputChannel_Right = Conversions.ToInteger(this.SelectedHardWareOutputChannel_Right_ComboBox.SelectedItem);
    }


    private void PlaySignal_Button_Click(object sender, EventArgs e)
    {

        try
        {

            // Silencing any previously started calibration signal
            SilenceCalibrationTone();

            if (SelectedTransducer.CanPlay == true)
            {

                // Sets the SelectedCalibrationSound 
                SpeechTestFramework.Audio.Sound CalibrationSound = (SpeechTestFramework.Audio.Sound)this.CalibrationSignal_ComboBox.SelectedItem;

                if (CalibrationSound is null)
                {
                    Interaction.MsgBox("Please select a calibration signal!", MsgBoxStyle.Exclamation, "Calibration");
                    return;
                }

                // Copies the sound 
                CalibrationSound = CalibrationSound.CreateCopy();

                // Converts 16 bit PCM to 32 float
                if ((int)CalibrationSound.WaveFormat.BitDepth == 16 & CalibrationSound.WaveFormat.Encoding == SpeechTestFramework.Audio.Formats.WaveFormat.WaveFormatEncodings.PCM)
                {
                    CalibrationSound = CalibrationSound.Convert16to32bitSound();
                    // Any other attempted format, except 32 bit IEEE, will be stopped by the player.
                }

                // Updates the wave format of the sound player
                SpeechTestFramework.Audio.SoundScene.DuplexMixer argMixer = null;
                SpeechTestFramework.OstfBase.SoundPlayer.ChangePlayerSettings(SampleRate: (int?)CalibrationSound.WaveFormat.SampleRate, BitDepth: CalibrationSound.WaveFormat.BitDepth, Encoding: CalibrationSound.WaveFormat.Encoding, Mixer: ref argMixer);

                // Setting the signal level
                SpeechTestFramework.Audio.DSP.Transformations.MeasureAndAdjustSectionLevel(ref CalibrationSound, (decimal)SpeechTestFramework.Audio.AudioManagement.Standard_dBSPL_To_dBFS(SelectedLevel), 1, FrequencyWeighting: (SpeechTestFramework.Audio.BasicAudioEnums.FrequencyWeightings)Conversions.ToInteger(this.FrequencyWeighting_ComboBox.SelectedItem));

                // Fading in and out
                SpeechTestFramework.Audio.DSP.Transformations.Fade(ref CalibrationSound, default(double?), 0, 1, 0, (int?)Math.Round(0.02d * (double)CalibrationSound.WaveFormat.SampleRate), SpeechTestFramework.Audio.DSP.Transformations.FadeSlopeType.Smooth);
                SpeechTestFramework.Audio.DSP.Transformations.Fade(ref CalibrationSound, 0, default(double?), 1, (int)Math.Round(-0.02d * (double)CalibrationSound.WaveFormat.SampleRate), default(int?), SpeechTestFramework.Audio.DSP.Transformations.FadeSlopeType.Smooth);

                SpeechTestFramework.Audio.Sound PlaySound = (SpeechTestFramework.Audio.Sound)null;

                if (SpeechTestFramework.OstfBase.DirectionalSimulator.IsActive() == true)
                {

                    double SelectedSimulatedDistance;
                    if (this.SimulatedDistance_ComboBox.SelectedItem is not null)
                    {
                        SelectedSimulatedDistance = Conversions.ToDouble(this.SimulatedDistance_ComboBox.SelectedItem);
                    }
                    else
                    {
                        Interaction.MsgBox("Please select a directional simulation distance!");
                        return;
                    }

                    var SimulationKernel = SpeechTestFramework.OstfBase.DirectionalSimulator.GetStereoKernel(SpeechTestFramework.OstfBase.DirectionalSimulator.SelectedDirectionalSimulationSetName, 0d, 0d, SelectedSimulatedDistance);
                    var CurrentKernel = SimulationKernel.BinauralIR.CreateSoundDataCopy();

                    var StereoCalibrationSound = new SpeechTestFramework.Audio.Sound(new SpeechTestFramework.Audio.Formats.WaveFormat((int)CalibrationSound.WaveFormat.SampleRate, (int)CalibrationSound.WaveFormat.BitDepth, 2, Encoding: CalibrationSound.WaveFormat.Encoding));
                    var LeftChannelSound = CalibrationSound.CreateSoundDataCopy();
                    var RightChannelSound = CalibrationSound.CreateSoundDataCopy();
                    StereoCalibrationSound.WaveData.set_SampleData(1, LeftChannelSound.WaveData.get_SampleData(1));
                    StereoCalibrationSound.WaveData.set_SampleData(2, RightChannelSound.WaveData.get_SampleData(1));

                    // Applies FIR-filtering
                    // StereoCalibrationSound.WriteWaveFile(IO.Path.Combine(Utils.logFilePath, "CalibSound_PreDirFilter"))
                    int argsetAnalysisWindowSize = 1024;
                    int argsetFftWindowSize = -1;
                    int argsetoverlapSize = 0;
                    bool argInActivateWarnings = false;
                    var argfftFormat = new SpeechTestFramework.Audio.Formats.FftFormat(setAnalysisWindowSize: ref argsetAnalysisWindowSize, setFftWindowSize: ref argsetFftWindowSize, setoverlapSize: ref argsetoverlapSize, InActivateWarnings: ref argInActivateWarnings);
                    var FilteredSound = SpeechTestFramework.Audio.DSP.TransformationsExt.FIRFilter(StereoCalibrationSound, CurrentKernel, ref argfftFormat, InActivateWarnings: true);
                    // FilteredSound.WriteWaveFile(IO.Path.Combine(Utils.logFilePath, "CalibSound_PostDirFilter"))

                    // Putting the sound in the intended channels
                    PlaySound = new SpeechTestFramework.Audio.Sound(new SpeechTestFramework.Audio.Formats.WaveFormat((int)FilteredSound.WaveFormat.SampleRate, (int)FilteredSound.WaveFormat.BitDepth, (int)SelectedTransducer.ParentAudioApiSettings.NumberOfOutputChannels(), Encoding: FilteredSound.WaveFormat.Encoding));
                    PlaySound.WaveData.set_SampleData(SelectedTransducer.Mixer.OutputRouting[SelectedHardwareOutputChannel], FilteredSound.WaveData.get_SampleData(1));
                    PlaySound.WaveData.set_SampleData(SelectedTransducer.Mixer.OutputRouting[SelectedHardwareOutputChannel_Right], FilteredSound.WaveData.get_SampleData(2));
                }

                else
                {

                    // Putting the sound in the intended channel
                    PlaySound = new SpeechTestFramework.Audio.Sound(new SpeechTestFramework.Audio.Formats.WaveFormat((int)CalibrationSound.WaveFormat.SampleRate, (int)CalibrationSound.WaveFormat.BitDepth, (int)SelectedTransducer.ParentAudioApiSettings.NumberOfOutputChannels(), Encoding: CalibrationSound.WaveFormat.Encoding));
                    PlaySound.WaveData.set_SampleData(SelectedTransducer.Mixer.OutputRouting[SelectedHardwareOutputChannel], CalibrationSound.WaveData.get_SampleData(1));

                }

                // PlaySound = Audio.DSP.IIRFilter(PlaySound, FrequencyWeightings.C)
                // PlaySound.WriteWaveFile(IO.Path.Combine(Utils.logFilePath, "CalibSound_NS_C"))

                // Plays the sound
                SpeechTestFramework.OstfBase.SoundPlayer.SwapOutputSounds(ref PlaySound);
            }

            else
            {
                Interaction.MsgBox("Unable to start the player using the selected transducer!", MsgBoxStyle.Exclamation, "Sound player failure");
                this.PlaySignal_Button.Enabled = false;
            }
        }

        catch (Exception ex)
        {
            Interaction.MsgBox("An error occurred!", MsgBoxStyle.Exclamation, "Calibration");
        }

    }

    private void StopSignal_Button_Click(object sender, EventArgs e)
    {
        SilenceCalibrationTone();
    }

    private void SilenceCalibrationTone()
    {
        SpeechTestFramework.Audio.Sound argNewOutputSound = (SpeechTestFramework.Audio.Sound)null;
        SpeechTestFramework.OstfBase.SoundPlayer.SwapOutputSounds(ref argNewOutputSound);
    }

    private void Close_Button_Click(object sender, EventArgs e)
    {
        SpeechTestFramework.OstfBase.SoundPlayer.CloseStream();
        this.Close();
    }

    private void Help_Button_Click(object sender, EventArgs e)
    {
        ShowHelp();
    }

    private void ShowHelp()
    {

        var InstructionsForm = new SpeechTestFramework.InfoForm();

        string AudioSystemSpecificationFilePath = System.IO.Path.Combine(SpeechTestFramework.OstfBase.MediaRootDirectory, SpeechTestFramework.OstfBase.AudioSystemSettingsFile);

        string CalibrationInfoString = @"Instructions on how to perform calibration

1. Select the sound system that you want to calibrate.
2. Select the desired calibration signal. (If you want to use your own signal, just locate the folder '" + CalibrationFilesDirectory + @"' and put your signal there, and restart the app. The signal needs to be stored in 32-bit IEEE/float or 16-bit PCM format.)
3. Select a calibration signal level to present.
4. Select a frequency weighting. If set to 'Z' the unit of the selected signal level will be dB SPL, if set to 'C' it will instead be in dB C. The C-weighting option is only available for calibration signals with a sample rate of 48 kHz.
5. Select which output speaker channel you want to calibrate.
6. Use a sound level meter and measure the sound level at the desired measurement point.
7. Calculate the difference between the selected calibration signal level and the measured level.
8. Adjust the calibration value ('CalibrationGain') for the output channel in the file '" + AudioSystemSpecificationFilePath + @"' by the calculated difference. 
9. Do the same for all speaker channels.
10. Restart the app to load the new calibration values and measure the calibration levels again to ensure correct calibration.";

        InstructionsForm.SetInfo(CalibrationInfoString, "How to calibrate");
        InstructionsForm.Show();

    }

    private void CalibrationForm_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
    {
        if (IsStandAlone == true)
            SpeechTestFramework.OstfBase.TerminateOSTF();
    }

    private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
    {

        var AboutBox = new SpeechTestFramework.AboutBox_WithLicenseButton();
        AboutBox.SelectedLicense = SpeechTestFramework.LicenseBox.AvailableLicenses.MIT_X11;
        AboutBox.LicenseAdditions.Add(SpeechTestFramework.LicenseBox.AvailableLicenseAdditions.PortAudio);
        AboutBox.ShowDialog();

    }

    private void ViewAvailableSoundDevicesToolStripMenuItem_Click(object sender, EventArgs e)
    {

        var SoundDevicesForm = new SpeechTestFramework.InfoForm();
        string DeviceInfoString = "Currently available sound devices:" + Constants.vbCrLf + Constants.vbCrLf + SpeechTestFramework.Audio.AudioApiSettings.GetAllAvailableDevices();
        SoundDevicesForm.SetInfo(DeviceInfoString, "Available sound devices");
        SoundDevicesForm.Show();

    }

    private void HelpToolStripMenuItem_Click(object sender, EventArgs e)
    {
        ShowHelp();
    }

    private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
    {
        this.Close();
    }

}
