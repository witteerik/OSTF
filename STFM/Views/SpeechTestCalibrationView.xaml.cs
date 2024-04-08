using Microsoft.VisualBasic;
using STFN;
using STFN.Audio;

namespace STFM.Views;

public partial class SpeechTestCalibrationView : ContentView
{

    public bool IsStandAlone { get; private set; }

    private int SelectedHardwareOutputChannel; // Left channel if used with sound field simulation, otherwise just the output channel
    private int SelectedHardwareOutputChannel_Right; // Only used with field simulation

    private double SelectedLevel;
    private SortedList<string, string> CalibrationFileDescriptions = new SortedList<string, string>();
    private OstfBase.AudioSystemSpecification SelectedTransducer = (OstfBase.AudioSystemSpecification)null;
    private STFN.Utils.Constants.UserTypes UserType;

    private string CalibrationFilesDirectory = "";

    private string NoSimulationString = "No simulation";

    public SpeechTestCalibrationView()
	{
		InitializeComponent();

        // Create and setup init timer // TODO: There should be a better solution for this... The reason it's used it to first show the GUI then call functions that need the GUI handle
        IDispatcherTimer trialEventTimer = Application.Current.Dispatcher.CreateTimer();
        trialEventTimer.Interval = TimeSpan.FromMilliseconds(1000);
        trialEventTimer.Tick += InitByTimer;
        trialEventTimer.IsRepeating = false;
        trialEventTimer.Start();
    }


    private async void InitByTimer(object sender, EventArgs e)
    {

        await InitializeSTFM();

        // Edding events
        Transducer_ComboBox.SelectedIndexChanged += Transducer_ComboBox_SelectedIndexChanged;
        CalibrationSignal_ComboBox.SelectedIndexChanged += CalibrationSignal_ComboBox_SelectedIndexChanged;
        DirectionalSimulationSet_ComboBox.SelectedIndexChanged += DirectionalSimulationSet_ComboBox_SelectedIndexChanged;
        CalibrationLevel_ComboBox.SelectedIndexChanged += CalibrationLevel_ComboBox_SelectedIndexChanged;
        SelectedHardWareOutputChannel_ComboBox.SelectedIndexChanged += SelectedChannelComboBox_SelectedIndexChanged;
        SelectedHardWareOutputChannel_Right_ComboBox.SelectedIndexChanged += SelectedRightChannelComboBox_SelectedIndexChanged;

        PlaySignal_Button.Clicked += PlaySignal_Button_Click;
        StopSignal_Button.Clicked += StopSignal_Button_Click;
        Close_Button.Clicked += Close_Button_Click;
        Help_Button.Clicked += Help_Button_Click;
        Close_Button.Clicked += Close_Button_Click;

        // Add any initialization after the InitializeComponent() call.
        this.IsStandAlone = true;

        // Adds frequency weightings
        this.FrequencyWeighting_ComboBox.ItemsSource = new List<STFN.Audio.BasicAudioEnums.FrequencyWeightings>() { 
            BasicAudioEnums.FrequencyWeightings.Z, BasicAudioEnums.FrequencyWeightings.C };
        this.FrequencyWeighting_ComboBox.SelectedIndex = 0;

        // Adding transducers
        var TempWaveFormat = new STFN.Audio.Formats.WaveFormat(48000, 32, 1, Encoding: STFN.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints);
        STFN.OstfBase.SoundPlayer.ChangePlayerSettings(null, (int?)TempWaveFormat.SampleRate, TempWaveFormat.BitDepth, TempWaveFormat.Encoding, null, null, STFN.Audio.SoundPlayers.iSoundPlayer.SoundDirections.PlaybackOnly, false, false);

        var LocalAvailableTransducers = STFN.OstfBase.AvaliableTransducers;
        if (LocalAvailableTransducers.Count == 0)
        {
            Messager.MsgBox("Unable to start the application since no sound transducers could be found!", Messager.MsgBoxStyle.Critical, "Calibration");
        }

        // Adding transducers to the combobox, and selects the first one
        Transducer_ComboBox.ItemsSource = LocalAvailableTransducers;
        this.Transducer_ComboBox.SelectedIndex = 0;

        // Adding signals
        CalibrationFilesDirectory = System.IO.Path.Combine(STFN.OstfBase.MediaRootDirectory, STFN.OstfBase.CalibrationSignalSubDirectory);
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
        List<Sound> CalibrationSounds = new List<Sound>();

        foreach (var File in CalibrationFiles)
        {
            if (System.IO.Path.GetExtension(File) == ".wav")
            {
                var NewCalibrationSound = STFN.Audio.Sound.LoadWaveFile(File);
                NewCalibrationSound.Description = System.IO.Path.GetFileNameWithoutExtension(File).Replace("_", " ");
                if (NewCalibrationSound is not null)
                    CalibrationSounds.Add(NewCalibrationSound);
            }
        }
        // Adding the internally generated sounds and descriptions
        var InternallyGeneratedSounds = GetInternallyGeneratedSounds();
        CalibrationSounds.AddRange(InternallyGeneratedSounds.Item1);
        foreach (var SoundDescription in InternallyGeneratedSounds.Item2)
        {
            CalibrationFileDescriptions.Add(SoundDescription.Key, SoundDescription.Value);
        }


        // Adding into CalibrationSignal_ComboBox
        this.CalibrationSignal_ComboBox.ItemsSource = CalibrationSounds;
        if (this.CalibrationSignal_ComboBox.Items.Count > 0)
        {
            this.CalibrationSignal_ComboBox.SelectedIndex = 0;
        }

        // Adding levels
        List<double> LevelList = new List<double>();   
        for (int Level = 0; Level <= 130; Level += 5)
            LevelList.Add(Level);

        this.CalibrationLevel_ComboBox.ItemsSource = LevelList;
        this.CalibrationLevel_ComboBox.SelectedIndex = (int)Math.Floor((double)(CalibrationLevel_ComboBox.ItemsSource.Count/2));

    }

    private async Task InitializeSTFM()
    {

        // Ititializing STFM if not already done
        if (STFM.StfmBase.IsInitialized == false)
        {
            // Initializing STFM
            await STFM.StfmBase.InitializeSTFM(SoundPlayerLayout);
        }

    }

    public Tuple<List<Sound>, SortedList<string, string>> GetInternallyGeneratedSounds() 
    {

        List<Sound> soundsList= new List<Sound>();
        SortedList<string, string> descriptionsList= new SortedList<string, string>();

        var TempWaveFormat = new STFN.Audio.Formats.WaveFormat(48000, 32, 1, Encoding: STFN.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints);

        var GeneratedWarble = STFN.Audio.GenerateSound.Signals.CreateFrequencyModulatedSineWave(ref TempWaveFormat, 1, 1000d, 0.5m, 20d, 0.125d, duration: 60d);
        GeneratedWarble.FileName = "Internal1";
        GeneratedWarble.Description = "Warble tone (60 s)";
        soundsList.Add(GeneratedWarble);
        descriptionsList.Add("Internal1", "OSTF generated warble tone. The tone is frequency modulated around 1 kHz by ±12.5 %, with a modulation frequency of 20 Hz. Samplerate 48kHz, duration 60 seconds.");

        var GeneratedSine = STFN.Audio.GenerateSound.Signals.CreateSineWave(ref TempWaveFormat, 1, 1000d, 0.5m, duration: 60d);
        GeneratedSine.FileName = "Internal2";
        GeneratedSine.Description = "Sine";
        soundsList.Add(GeneratedSine);
        descriptionsList.Add("Internal2", "OSTF generated 1kHz sine. Samplerate 48kHz, duration 60 seconds.");

        var GeneratedSweep1 = STFN.Audio.GenerateSound.SignalsExt.CreateLogSineSweep(ref TempWaveFormat, 1, 20d, 20000d, false, 0.5m, TotalDuration: 15d);
        GeneratedSweep1.FileName = "Internal3";
        GeneratedSweep1.Description = "Sweep";
        soundsList.Add(GeneratedSweep1);
        descriptionsList.Add("Internal3", "OSTF generated log-sine sweep, 20Hz - 20kHz. Samplerate 48kHz, duration 15 seconds.");

        var GeneratedSweep2 = STFN.Audio.GenerateSound.SignalsExt.CreateLogSineSweep(ref TempWaveFormat, 1, 20d, 20000d, true, 0.5m, TotalDuration: 15d);
        GeneratedSweep2.FileName = "Internal4";
        GeneratedSweep2.Description = "Sweep (flat)";
        soundsList.Add(GeneratedSweep2);
        descriptionsList.Add("Internal4", "OSTF generated (flat spectrum) log-sine sweep, 20Hz - 20kHz. Samplerate 48kHz, duration 15 seconds.");

        Random random = new Random();
        var WhiteNoise = STFN.Audio.GenerateSound.Signals.CreateWhiteNoise(ref TempWaveFormat, 1, 1, 15, BasicAudioEnums.TimeUnits.seconds, ref random);
        WhiteNoise.FileName = "Internal5";
        WhiteNoise.Description = "White noise";
        soundsList.Add(WhiteNoise);
        descriptionsList.Add("Internal5", "OSTF generated white noise. Samplerate 48kHz, duration 15 seconds.");

        return new Tuple<List<Sound>, SortedList<string, string>>(soundsList, descriptionsList);  

    }

    private void Transducer_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {

        this.SoundSystem_RichTextBox.Text = "";

        SelectedTransducer = (STFN.OstfBase.AudioSystemSpecification)this.Transducer_ComboBox.SelectedItem;

        if (SelectedTransducer.CanPlay == true)
        {
            // (At this stage the sound player will be started, if not already done.)
            var argAudioApiSettings = SelectedTransducer.ParentAudioApiSettings;
            var argMixer = SelectedTransducer.Mixer;
            STFN.OstfBase.SoundPlayer.ChangePlayerSettings(argAudioApiSettings, OverlapDuration: 0.3d, Mixer: argMixer, ReOpenStream: true, ReStartStream: true);
            SelectedTransducer.Mixer = argMixer;
            this.PlaySignal_Button.IsEnabled = true;
        }
        else
        {
            Messager.MsgBox("Unable to start the player using the selected transducer (probably the selected output device doesn't have enough output channels?)!", Messager.MsgBoxStyle.Exclamation, "Sound player failure");
            this.PlaySignal_Button.IsEnabled = false;
        }

        // Adding description
        this.SoundSystem_RichTextBox.Text = SelectedTransducer.GetDescriptionString();

        // Adding channels
        //this.SelectedHardWareOutputChannel_ComboBox.Items.Clear();
        //this.SelectedHardWareOutputChannel_Right_ComboBox.Items.Clear();
        List<int> SelectedHardWareOutputChannels = new List<int>();
        List<int> SelectedHardWareOutputChannels_Right = new List<int>();
        foreach (var c in SelectedTransducer.Mixer.OutputRouting.Keys)
        {
            SelectedHardWareOutputChannels.Add(c);
            SelectedHardWareOutputChannels_Right.Add(c);
        }

        this.SelectedHardWareOutputChannel_ComboBox.ItemsSource = SelectedHardWareOutputChannels;
        this.SelectedHardWareOutputChannel_Right_ComboBox.ItemsSource = SelectedHardWareOutputChannels_Right;


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
            STFN.Audio.Sound SelectedCalibrationSound = (STFN.Audio.Sound)this.CalibrationSignal_ComboBox.SelectedItem;
            if (CalibrationFileDescriptions.ContainsKey(SelectedCalibrationSound.FileName))
            {
                this.CalibrationSignal_RichTextBox.Text = CalibrationFileDescriptions[SelectedCalibrationSound.FileName] + "\n" + SelectedCalibrationSound.WaveFormat.ToString();
            }
            else
            {
                this.CalibrationSignal_RichTextBox.Text = "Calibration file without custom description." + "\n"  + SelectedCalibrationSound.WaveFormat.ToString();
            }

            // Checks if signal FS is 48 kHz. If not disables the FrequencyWeighting_ComboBox, and sets its selected value to Z-weighting
            if ((long)SelectedCalibrationSound.WaveFormat.SampleRate == 48000L)
            {
                this.FrequencyWeighting_ComboBox.IsEnabled = true;
            }
            else
            {
                this.FrequencyWeighting_ComboBox.SelectedIndex = 0;
                this.FrequencyWeighting_ComboBox.IsEnabled = false;
            }

            // Clearing previously added SimulatedDistance_ComboBox
            // this.SimulatedDistance_ComboBox.Items.Clear();

            // Adding available DirectionalSimulationSets

            List<string> AvailableSets = new List<string>();
            if (OstfBase.AllowDirectionalSimulation == true)
            {
                AvailableSets = STFN.OstfBase.DirectionalSimulator.GetAvailableDirectionalSimulationSets(ref SelectedTransducer, (int)SelectedCalibrationSound.WaveFormat.SampleRate);
            }
            AvailableSets.Insert(0, NoSimulationString);
            foreach (var Item in AvailableSets)
                this.DirectionalSimulationSet_ComboBox.ItemsSource = AvailableSets;
            if (this.DirectionalSimulationSet_ComboBox.Items.Count > 0 )
            {
                this.DirectionalSimulationSet_ComboBox.SelectedIndex = 0;
            }
        }
    }

    private void DirectionalSimulationSet_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {

        if (OstfBase.AllowDirectionalSimulation == false){return;}

        //this.SimulatedDistance_ComboBox.Items.Clear();
        //this.SimulatedDistance_ComboBox.ResetText();

        var SelectedItem = this.DirectionalSimulationSet_ComboBox.SelectedItem;
        if (SelectedItem is not null)
        {

            if ((string)SelectedItem == NoSimulationString)
            {
                STFN.OstfBase.DirectionalSimulator.ClearSelectedDirectionalSimulationSet();
                this.SelectedHardWareOutputChannel_Right_ComboBox.IsEnabled = false;
                this.RightChannel_Label.IsEnabled = false;
            }
            // SelectedSoundPropagationType = SoundPropagationTypes.PointSpeakers
            else
            {
                // SelectedSoundPropagationType = SoundPropagationTypes.SimulatedSoundField
                STFN.OstfBase.DirectionalSimulator.TrySetSelectedDirectionalSimulationSet((string)SelectedItem, ref SelectedTransducer);
                this.SelectedHardWareOutputChannel_Right_ComboBox.IsEnabled = true;
                this.RightChannel_Label.IsEnabled = true;
            }

            // Adding available simulation distances
            var AvailableDistances = STFN.OstfBase.DirectionalSimulator.GetAvailableDirectionalSimulationSetDistances((string)SelectedItem);
            this.SimulatedDistance_ComboBox.ItemsSource = AvailableDistances.ToList();
            if (this.SimulatedDistance_ComboBox.Items.Count > 0)
                this.SimulatedDistance_ComboBox.SelectedIndex = 0;

        }

        else
        {
            STFN.OstfBase.DirectionalSimulator.ClearSelectedDirectionalSimulationSet();
        }

    }

    private void CalibrationLevel_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        SelectedLevel = (double)CalibrationLevel_ComboBox.SelectedItem;
    }

    private void SelectedChannelComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SelectedHardWareOutputChannel_ComboBox.SelectedItem != null)
        {
            SelectedHardwareOutputChannel = (int)SelectedHardWareOutputChannel_ComboBox.SelectedItem;
        }
    }

    private void SelectedRightChannelComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SelectedHardWareOutputChannel_Right_ComboBox.SelectedItem !=null)
        {
            SelectedHardwareOutputChannel_Right = (int)SelectedHardWareOutputChannel_Right_ComboBox.SelectedItem;
        }
    }


    private void PlaySignal_Button_Click(object sender, EventArgs e)
    {

        try
        {

            // Silencing any previously started calibration signal
            //SilenceCalibrationTone();

            if (SelectedTransducer.CanPlay == true)
            {

                // Sets the SelectedCalibrationSound 
                STFN.Audio.Sound CalibrationSound = (STFN.Audio.Sound)this.CalibrationSignal_ComboBox.SelectedItem;

                if (CalibrationSound is null)
                {
                    Messager.MsgBox("Please select a calibration signal!", Messager.MsgBoxStyle.Exclamation, "Calibration");
                    return;
                }

                // Copies the sound 
                CalibrationSound = CalibrationSound.CreateSoundDataCopy();

                // Converts 16 bit PCM to 32 float
                if (STFN.OstfBase.SoundPlayer.WideFormatSupport == false)
                {
                    if ((int)CalibrationSound.WaveFormat.BitDepth == 16 & CalibrationSound.WaveFormat.Encoding == STFN.Audio.Formats.WaveFormat.WaveFormatEncodings.PCM)
                    {
                        CalibrationSound = CalibrationSound.Convert16to32bitSound();
                        // Any other attempted format, except 32 bit IEEE, will be stopped by the player.
                    }
                }

                // Updates the wave format of the sound player
                STFN.Audio.SoundScene.DuplexMixer argMixer = SelectedTransducer.Mixer;
                AudioSettings argAudioApiSettings = SelectedTransducer.ParentAudioApiSettings;
                STFN.OstfBase.SoundPlayer.ChangePlayerSettings(argAudioApiSettings, SampleRate: (int?)CalibrationSound.WaveFormat.SampleRate, BitDepth: CalibrationSound.WaveFormat.BitDepth, Encoding: CalibrationSound.WaveFormat.Encoding, Mixer: argMixer);

                // Setting the signal level
                STFN.Audio.DSP.Transformations.MeasureAndAdjustSectionLevel(ref CalibrationSound, (decimal)STFN.Audio.AudioManagement.Standard_dBSPL_To_dBFS(SelectedLevel), 1, FrequencyWeighting: (STFN.Audio.BasicAudioEnums.FrequencyWeightings) (int)FrequencyWeighting_ComboBox.SelectedItem);

                // Fading in and out
                STFN.Audio.DSP.Transformations.Fade(ref CalibrationSound, default(double?), 0, 1, 0, (int?)Math.Round(0.02d * (double)CalibrationSound.WaveFormat.SampleRate), STFN.Audio.DSP.Transformations.FadeSlopeType.Smooth);
                STFN.Audio.DSP.Transformations.Fade(ref CalibrationSound, 0, default(double?), 1, (int)Math.Round(-0.02d * (double)CalibrationSound.WaveFormat.SampleRate), default(int?), STFN.Audio.DSP.Transformations.FadeSlopeType.Smooth);

                STFN.Audio.Sound PlaySound = (STFN.Audio.Sound)null;

                bool useDirectionalSimulation = false;
                if (OstfBase.AllowDirectionalSimulation == true) {
                    if (STFN.OstfBase.DirectionalSimulator.IsActive() == true)
                    {
                        useDirectionalSimulation = true;
                    }
                }

                if (useDirectionalSimulation == true)
                {

                    double SelectedSimulatedDistance;
                    if (this.SimulatedDistance_ComboBox.SelectedItem is not null)
                    {
                        SelectedSimulatedDistance = (double)SimulatedDistance_ComboBox.SelectedItem;
                    }
                    else
                    {
                        Messager.MsgBox("Please select a directional simulation distance!");
                        return;
                    }

                    var SimulationKernel = STFN.OstfBase.DirectionalSimulator.GetStereoKernel(STFN.OstfBase.DirectionalSimulator.SelectedDirectionalSimulationSetName, 0d, 0d, SelectedSimulatedDistance);
                    var CurrentKernel = SimulationKernel.BinauralIR.CreateSoundDataCopy();

                    var StereoCalibrationSound = new STFN.Audio.Sound(new STFN.Audio.Formats.WaveFormat((int)CalibrationSound.WaveFormat.SampleRate, (int)CalibrationSound.WaveFormat.BitDepth, 2, Encoding: CalibrationSound.WaveFormat.Encoding));
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
                    var argfftFormat = new STFN.Audio.Formats.FftFormat(setAnalysisWindowSize: ref argsetAnalysisWindowSize, setFftWindowSize: ref argsetFftWindowSize, setoverlapSize: ref argsetoverlapSize, InActivateWarnings: ref argInActivateWarnings);
                    var FilteredSound = STFN.Audio.DSP.TransformationsExt.FIRFilter(StereoCalibrationSound, CurrentKernel, ref argfftFormat, InActivateWarnings: true);
                    // FilteredSound.WriteWaveFile(IO.Path.Combine(Utils.logFilePath, "CalibSound_PostDirFilter"))

                    // Putting the sound in the intended channels
                    PlaySound = new STFN.Audio.Sound(new STFN.Audio.Formats.WaveFormat((int)FilteredSound.WaveFormat.SampleRate, (int)FilteredSound.WaveFormat.BitDepth, (int)SelectedTransducer.NumberOfApiOutputChannels(), Encoding: FilteredSound.WaveFormat.Encoding));
                    PlaySound.WaveData.set_SampleData(SelectedTransducer.Mixer.OutputRouting[SelectedHardwareOutputChannel], FilteredSound.WaveData.get_SampleData(1));
                    PlaySound.WaveData.set_SampleData(SelectedTransducer.Mixer.OutputRouting[SelectedHardwareOutputChannel_Right], FilteredSound.WaveData.get_SampleData(2));
                }
                else
                {

                    // Putting the sound in the intended channel
                    PlaySound = new STFN.Audio.Sound(new STFN.Audio.Formats.WaveFormat((int)CalibrationSound.WaveFormat.SampleRate, (int)CalibrationSound.WaveFormat.BitDepth, (int)SelectedTransducer.NumberOfApiOutputChannels(), Encoding: CalibrationSound.WaveFormat.Encoding));
                    PlaySound.WaveData.set_SampleData(SelectedTransducer.Mixer.OutputRouting[SelectedHardwareOutputChannel], CalibrationSound.WaveData.get_SampleData(1));

                    //for (int c = 1; c <= SelectedTransducer.Mixer.GetHighestOutputChannel(); c++)
                    //{
                    //    if (c == SelectedTransducer.Mixer.OutputRouting[SelectedHardwareOutputChannel])
                    //    {
                    //        PlaySound.WaveData.set_SampleData(c, CalibrationSound.WaveData.get_SampleData(1));
                    //    }
                    //    else
                    //    {
                    //        float[] silentArray = new float[CalibrationSound.WaveData.get_SampleData(1).Length];
                    //        PlaySound.WaveData.set_SampleData(c, silentArray);
                    //    }
                    //}
                }

                // PlaySound = Audio.DSP.IIRFilter(PlaySound, FrequencyWeightings.C)
                // PlaySound.WriteWaveFile(IO.Path.Combine(Utils.logFilePath, "CalibSound_NS_C"))

                // Plays the sound
                STFN.OstfBase.SoundPlayer.SwapOutputSounds(ref PlaySound);
            }

            else
            {
                Messager.MsgBox("Unable to start the player using the selected transducer!", Messager.MsgBoxStyle.Exclamation, "Sound player failure");
                this.PlaySignal_Button.IsEnabled = false;
            }
        }

        catch (Exception ex)
        {
            Messager.MsgBox("The following error occurred:\n\n" + ex.ToString(), Messager.MsgBoxStyle.Exclamation, "Calibration");
        }

    }

    private void StopSignal_Button_Click(object sender, EventArgs e)
    {
        SilenceCalibrationTone();
    }

    private void SilenceCalibrationTone()
    {
        STFN.OstfBase.SoundPlayer.FadeOutPlayback();
    }

    private void Close_Button_Click(object sender, EventArgs e)
    {

        Application.Current.Quit(); 

    }

    private void Help_Button_Click(object sender, EventArgs e)
    {
        ShowHelp();
    }

    private void ShowHelp()
    {

        //var InstructionsForm = new STFN.InfoForm();

        string AudioSystemSpecificationFilePath = System.IO.Path.Combine(STFN.OstfBase.MediaRootDirectory, STFN.OstfBase.AudioSystemSettingsFile);

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

        Messager.MsgBox(CalibrationInfoString, Messager.MsgBoxStyle.Information, "How to calibrate", "Close");

    }

    //private void CalibrationForm_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
    //{
    //    if (IsStandAlone == true)
    //        //STFN.OstfBase.TerminateOSTF();
    //}

    //private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
    //{

    //    var AboutBox = new STFN.AboutBox_WithLicenseButton();
    //    AboutBox.SelectedLicense = STFN.LicenseBox.AvailableLicenses.MIT_X11;
    //    AboutBox.LicenseAdditions.Add(STFN.LicenseBox.AvailableLicenseAdditions.PortAudio);
    //    AboutBox.ShowDialog();

    //}

    //private void ViewAvailableSoundDevicesToolStripMenuItem_Click(object sender, EventArgs e)
    //{

    //    var SoundDevicesForm = new STFN.InfoForm();
    //    string DeviceInfoString = "Currently available sound devices:" + "\n\n" + STFN.Audio.PortAudioApiSettings.GetAllAvailableDevices();
    //    SoundDevicesForm.SetInfo(DeviceInfoString, "Available sound devices");
    //    SoundDevicesForm.Show();

    //}

    private void HelpToolStripMenuItem_Click(object sender, EventArgs e)
    {
        ShowHelp();
    }

    private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
    {
        //this.Close();
    }

}