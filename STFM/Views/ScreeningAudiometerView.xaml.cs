using STFN;
using System.Reflection.Metadata;
using System.Security.AccessControl;

namespace STFM.Views;

public partial class ScreeningAudiometerView : ContentView
{

    public event EventHandler<EventArgs> EnterFullScreenMode;
    public event EventHandler<EventArgs> ExitFullScreenMode;

    private SortedList<SignalSides, SortedList<double, SortedList<double, STFN.Audio.Sound>>> Sines;
    private List<int> Frequencies;
    private List<double> Levels;
    private STFN.Audio.Formats.WaveFormat WaveFormat;

    private SortedList<int, double> RetSplList;
    private SortedList<int, double> PureToneCalibrationList = new SortedList<int, double>();

    private STFN.Audio.Sound silentSound = null;

    private enum SignalSides
    {
        None,
        Left,
        Right
    }

    private SignalSides Sides;

    private SignalSides CurrentSide = SignalSides.None;

    private int CurrentLevelIndex;
    private int CurrentFrequencyIndex;

    private Color originalButtonColor;

    private double MaxSoundDuration = 3;
    private bool IsInCalibrationMode = false;

    public ScreeningAudiometerView()
	{
		InitializeComponent();

        WaveFormat = new STFN.Audio.Formats.WaveFormat(48000,32, 2,"", STFN.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints );

        silentSound = STFN.Audio.GenerateSound.Signals.CreateSilence(ref this.WaveFormat, null, 3);

        Frequencies = new List<int>() { 125, 250, 500, 750, 1000, 1500, 2000, 3000, 4000, 6000, 8000 };
        Levels = new List<double>() { 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70 };
        Sines = new SortedList<SignalSides, SortedList<double, SortedList<double, STFN.Audio.Sound>>>();
        List<SignalSides> possibleSides = new List<SignalSides>() { SignalSides.Left, SignalSides.Right };

        // Filling up frequencies and levels structure with nulls
        for (int s = 0; s < possibleSides .Count; s++)
        {

            Sines.Add(possibleSides[s], new SortedList<double, SortedList<double, STFN.Audio.Sound>>());

            for (int i = 0; i < Frequencies.Count; i++)
            {
                Sines[possibleSides[s]].Add(Frequencies[i], new SortedList<double, STFN.Audio.Sound>());
            }
        }

        originalButtonColor = Channel1InputAirButton.BackgroundColor;

        // Setting initial values
        CurrentSide = SignalSides.Right;
        CurrentFrequencyIndex = 3;
        CurrentLevelIndex = 8;

        UpdateGUI();

        // RETSPL values for DD65v2
        RetSplList = new SortedList<int, double>();
        RetSplList.Add(125, 30.5);
        RetSplList.Add(160, 25.5);
        RetSplList.Add(200, 21.5);
        RetSplList.Add(250, 17);
        RetSplList.Add(315, 14);
        RetSplList.Add(400, 10.5);
        RetSplList.Add(500, 8);
        RetSplList.Add(630, 6.5);
        RetSplList.Add(750, 5.5);
        RetSplList.Add(800, 5);
        RetSplList.Add(1000, 4.5);
        RetSplList.Add(1250, 3.5);
        RetSplList.Add(1500, 2.5);
        RetSplList.Add(1600, 2.5);
        RetSplList.Add(2000, 2.5);
        RetSplList.Add(2500, 2);
        RetSplList.Add(3000, 2);
        RetSplList.Add(3150, 3);
        RetSplList.Add(4000, 9.5);
        RetSplList.Add(5000, 15.5);
        RetSplList.Add(6000, 21);
        RetSplList.Add(6300, 21);
        RetSplList.Add(8000, 21);


        // RETSPL values for DD450
        //RetSplList.Add(125, 30.5);
        //RetSplList.Add(160, 26);
        //RetSplList.Add(200, 22);
        //RetSplList.Add(250, 18);
        //RetSplList.Add(315, 15.5);
        //RetSplList.Add(400, 13.5);
        //RetSplList.Add(500, 11);
        //RetSplList.Add(630, 8);
        //RetSplList.Add(750, 6);
        //RetSplList.Add(800, 6);
        //RetSplList.Add(1000, 5.5);
        //RetSplList.Add(1250, 6);
        //RetSplList.Add(1500, 5.5);
        //RetSplList.Add(1600, 5.5);
        //RetSplList.Add(2000, 4.5);
        //RetSplList.Add(2500, 3);
        //RetSplList.Add(3000, 2.5);
        //RetSplList.Add(3150, 4);
        //RetSplList.Add(4000, 9.5);
        //RetSplList.Add(5000, 14);
        //RetSplList.Add(6000, 17);
        //RetSplList.Add(6300, 17.5);
        //RetSplList.Add(8000, 17.5);
        //RetSplList.Add(9000, 19);
        //RetSplList.Add(10000, 22);
        //RetSplList.Add(11200, 23);
        //RetSplList.Add(12500, 27.5);
        //RetSplList.Add(14000, 35);
        //RetSplList.Add(16000, 56);

        var CurrentMixer = OstfBase.SoundPlayer.GetMixer();
        RightInfoLabel2.Text = CurrentMixer.GetParentTransducerSpecification.Name;
        RightInfoLabel3.Text = CurrentMixer.GetParentTransducerSpecification.ParentAudioApiSettings.GetSelectedOutputDeviceName();

        if (CurrentMixer.GetParentTransducerSpecification.PtaCalibrationGain != null)
        {
            PureToneCalibrationList = CurrentMixer.GetParentTransducerSpecification.PtaCalibrationGain;
        }

    }

    public void GotoCalibrationMode() {

        // Clearing sounds
        foreach (var side in Sines.Keys)
        {
            foreach (var frequency in Sines[side].Keys)
            {
                foreach (var level in Sines[side][frequency].Keys)
                {
                    Sines[side][frequency][level] = null;
                }
            }
        }

        // Making sounds 1 minute long
        MaxSoundDuration = 60;

        IsInCalibrationMode = true;

    }



    private STFN.Audio.Sound GetAudiometerSine(SignalSides side, int frequency, double level)
    {

        if (side == SignalSides.None)
        {
            return null;
        }

        double RetSplCorrectedLevel = level + RetSplList[frequency];
        double CalibratedRetSplCorrectedLevel = RetSplCorrectedLevel + PureToneCalibrationList[frequency];
        double RetSplCorrectedLevel_FS = STFN.Audio.AudioManagement.Standard_dBSPL_To_dBFS(CalibratedRetSplCorrectedLevel);

        // Here, to make sure the tone does not get in distorted integer sound formats, we should also add the output channel specific general calibration gain added by the sound player and which we can get from:
        // var CurrentMixer = OstfBase.SoundPlayer.GetMixer();
        // CurrentMixer.GetParentTransducerSpecification.CalibrationGain();
        // Skipping this for now

        if (RetSplCorrectedLevel_FS  > -3)
        {
            Messager.MsgBox("This level (" + level.ToString() + " dB HL) exceeds the maximum level of the output device for the frequency " + frequency.ToString() + " Hz.\n\nThe tone will not be played.", Messager.MsgBoxStyle.Information, "Maximum output level reached!");
            return null;
        }

        // Adding the level value to the dictionary, if needed
        if (Sines[side][frequency].ContainsKey(RetSplCorrectedLevel_FS) == false)
        {
            Sines[side][frequency].Add(RetSplCorrectedLevel_FS, null);
        }

        // Creating the sine if it doesn't exist
        if (Sines[side][frequency][RetSplCorrectedLevel_FS] == null)
        {

            //Checking that there is a RETSPL correction value available
            if (RetSplList.ContainsKey(frequency) == false)
            {
                Messager.MsgBox("Missing RETSPL value for frequency " + frequency.ToString());
                return null;
            }

            //Checking that there is a calibration value available
            if (PureToneCalibrationList.ContainsKey(frequency) == false)
            {
                Messager.MsgBox("Missing pure tone calibration value for frequency " + frequency.ToString());
                return null;
            }

            STFN.Audio.Sound newSine = null;
            switch (side)
            {
                case SignalSides.Left:
                    newSine = STFN.Audio.GenerateSound.Signals.CreateSineWave(ref this.WaveFormat,1, frequency, (decimal)RetSplCorrectedLevel_FS, STFN.Audio.AudioManagement.SoundDataUnit.dB, MaxSoundDuration);
                    STFN.Audio.DSP.Transformations.Fade(ref newSine, null, 0, 1, 0, (int)(WaveFormat.SampleRate * 0.1), STFN.Audio.DSP.Transformations.FadeSlopeType.Linear);
                    STFN.Audio.DSP.Transformations.Fade(ref newSine, 0, null, 1, (int)(-WaveFormat.SampleRate * 0.1),null, STFN.Audio.DSP.Transformations.FadeSlopeType.Linear);
                    break;
                case SignalSides.Right:
                    newSine = STFN.Audio.GenerateSound.Signals.CreateSineWave(ref this.WaveFormat,2, frequency, (decimal)RetSplCorrectedLevel_FS, STFN.Audio.AudioManagement.SoundDataUnit.dB, MaxSoundDuration);
                    STFN.Audio.DSP.Transformations.Fade(ref newSine, null, 0, 2, 0, (int)(WaveFormat.SampleRate * 0.1), STFN.Audio.DSP.Transformations.FadeSlopeType.Linear);
                    STFN.Audio.DSP.Transformations.Fade(ref newSine, 0, null, 2, (int)(-WaveFormat.SampleRate * 0.1),null, STFN.Audio.DSP.Transformations.FadeSlopeType.Linear);
                    break;
                default:
                    throw new Exception("Invalid value for sides");
                    //break;
            }



            Sines[side][frequency][RetSplCorrectedLevel_FS] = newSine;
        }
        // Returning the sine
        return Sines[side][frequency][RetSplCorrectedLevel_FS];
    }

    private void Channel1StimulusButtonPressed(object sender, EventArgs e) {

        var currentSine = GetAudiometerSine(CurrentSide, Frequencies[CurrentFrequencyIndex], Levels[CurrentLevelIndex]);

        if (currentSine != null)
        {
            OstfBase.SoundPlayer.SwapOutputSounds(ref currentSine);

            // Light on
            Channel1StimulusButton.BackgroundColor = Colors.Yellow;
            // Updating GUI to reflect any other changes that may have occurred
            UpdateGUI();
        }
    }

    private void Channel1StimulusButtonReleased(object sender, EventArgs e) {

        // Ignored button release in calibration mode, to keep the tone on.
        if (IsInCalibrationMode == true)
        {
            return;
        }

        
        OstfBase.SoundPlayer.SwapOutputSounds(ref silentSound);

        //OstfBase.SoundPlayer.FadeOutPlayback();

        // Light off
        Channel1StimulusButton.BackgroundColor = originalButtonColor;
        // Updating GUI to reflect any other changes that may have occurred
        UpdateGUI();

    }

    private void OnLeftUpClicked(object sender, EventArgs e) {
        CurrentLevelIndex = System.Math.Min(CurrentLevelIndex + 1, Levels.Count-1);
        UpdateGUI();
    }
    private void LeftDownBtnClicked(object sender, EventArgs e) {
        CurrentLevelIndex = System.Math.Max(CurrentLevelIndex - 1, 0);
        UpdateGUI();
    }
    private void IncreaseFrequencyButtonClicked(object sender, EventArgs e) {
        CurrentFrequencyIndex = System.Math.Min(CurrentFrequencyIndex + 1, Frequencies.Count - 1);
        UpdateGUI();
    }
    private void DecreaseFrequencyButtonClicked(object sender, EventArgs e)
    {
        CurrentFrequencyIndex = System.Math.Max(CurrentFrequencyIndex - 1, 0);
        UpdateGUI();
    }

    private void Channel1InputAirButtonClicked(object sender, EventArgs e) {

        // Rotating value
        switch (CurrentSide)
        {
            case SignalSides.None:
                CurrentSide = SignalSides.Right;
                break;

            case SignalSides.Right:
                CurrentSide = SignalSides.Left;
                break;

            case SignalSides.Left:
                CurrentSide = SignalSides.None;
                break;

            default:
                break;
        }

        UpdateGUI();
    }

    private void UpdateGUI()
    {

        // Showing in GUI
        RightLevelLabel.Text = Levels[CurrentLevelIndex].ToString() + " dB HL";
        RightInfoLabel1.Text = Frequencies[CurrentFrequencyIndex].ToString() + " Hz";

        if (CurrentLevelIndex < Levels.Count-1)
        {
            LeftUpBtn.IsEnabled = true;
        }
        else
        {
            LeftUpBtn.IsEnabled = false;
        }

        if (CurrentLevelIndex >0)
        {
            LeftDownBtn.IsEnabled = true;
        }
        else
        {
            LeftDownBtn.IsEnabled = false;
        }

        if (CurrentFrequencyIndex< Frequencies.Count -1)
        {
            IncreaseFrequencyButton.IsEnabled = true;
        }
        else
        {
            IncreaseFrequencyButton.IsEnabled = false;
        }

        if (CurrentFrequencyIndex > 0 )
        {
            DecreaseFrequencyButton.IsEnabled = true;
        }
        else
        {
            DecreaseFrequencyButton.IsEnabled = false;
        }

        var CurrentMixer = OstfBase.SoundPlayer.GetMixer();
        RightInfoLabel2.Text = CurrentMixer.GetParentTransducerSpecification.Name;
        RightInfoLabel3.Text = CurrentMixer.GetParentTransducerSpecification.ParentAudioApiSettings.GetSelectedOutputDeviceName();

        // Rotating value
        switch (CurrentSide)
        {
            case SignalSides.None:
                Channel1InputAirButtonLightR.BackgroundColor = Colors.Grey;
                Channel1InputAirButtonLightL.BackgroundColor = Colors.Grey;
                Channel1StimulusButton.IsEnabled = false;
                break;

            case SignalSides.Right:
                Channel1InputAirButtonLightR.BackgroundColor = Colors.Red;
                Channel1InputAirButtonLightL.BackgroundColor = Colors.Grey;
                Channel1StimulusButton.IsEnabled = true;
                break;

            case SignalSides.Left:
                Channel1InputAirButtonLightR.BackgroundColor = Colors.Grey;
                Channel1InputAirButtonLightL.BackgroundColor = Colors.Blue;
                Channel1StimulusButton.IsEnabled = true;
                break;

            default:
                break;
        }


    }


    bool IsInFullScreenMode = false;

    private void FullScreenSwapBtn_Clicked(object sender, EventArgs e)
    {

        if (IsInFullScreenMode == false)
        {
            EventHandler<EventArgs> handler = EnterFullScreenMode;
            if (handler != null)
            {
                handler(this, e);
                FullScreenSwapBtn.Text = "Exit fullscreen";
                IsInFullScreenMode = true;
            }
        }
        else
        {
            EventHandler<EventArgs> handler = ExitFullScreenMode;
            if (handler != null)
            {
                handler(this, e);
                FullScreenSwapBtn.Text = "Fullscreen";
                IsInFullScreenMode = false;
            }
        }
    }
}