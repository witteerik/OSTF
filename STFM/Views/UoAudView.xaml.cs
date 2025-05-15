using STFN;

namespace STFM.Views;

public partial class UoAudView : ContentView
{

    public event EventHandler<EventArgs> EnterFullScreenMode;
    public event EventHandler<EventArgs> ExitFullScreenMode;

    private Brush NonPressedBrush = Colors.LightGray;
    private Brush PressedBrush = Colors.Yellow;

    private STFN.UoPta CurrentTest;

    STFN.UoPta.PtaTrial CurrentTrial;

    private STFN.Audio.Formats.WaveFormat WaveFormat;

    private SortedList<int, double> RetSplList;
    private SortedList<int, double> PureToneCalibrationList = new SortedList<int, double>();

    private STFN.Audio.Sound silentSound = null;

    IDispatcherTimer TrialEndTimer;

    public UoAudView()
    {
        InitializeComponent();

        // Setting default texts
        switch (STFN.SharedSpeechTestObjects.GuiLanguage)
        {
            case STFN.Utils.Constants.Languages.English:

                MessageButton.Text = "Click here to start!";
                MessageLabel.Text = "Press when you hear a tone.\nRelease when the tone disappears.";

                LeftResponseButton.Text = "Left\n ear";
                MidResponseButton.Text = "Middle";
                RightResponseButton.Text = "Right\n ear";

                ResultView.Text = "";

                break;

            case STFN.Utils.Constants.Languages.Swedish:

                MessageButton.Text = "Tryck här för att starta!";
                MessageLabel.Text = "Tryck på knappen nedanför när du hör en ton.\nSläpp knappen när tonen tystnar.";

                MidResponseButton.Text = "Tryck här!";

                LeftResponseButton.Text = "Vänster\n   öra";
                //MidResponseButton.Text = "Mitten";
                RightResponseButton.Text = "Höger\n  öra";

                ResultView.Text = "";

                break;
            default:
                // Using English as default

                MessageButton.Text = "Click here to start!";
                MessageLabel.Text = "Press when you hear a tone.\nRelease when the tone disappears.";

                LeftResponseButton.Text = "Left\n ear";
                MidResponseButton.Text = "Middle";
                RightResponseButton.Text = "Right\n ear";

                ResultView.Text = "";

                break;
        }

        // Setting default colors
        LeftResponseButton.Background = NonPressedBrush;
        MidResponseButton.Background = NonPressedBrush;
        RightResponseButton.Background = NonPressedBrush;

        LeftResponseButton.TextColor = Colors.DarkGrey;
        MidResponseButton.TextColor = Colors.DarkGrey;
        RightResponseButton.TextColor = Colors.DarkGrey;

        // Setting default visibility
        SetVisibility(VisibilityTypes.MessageMode);

        // Initializing the CurrentTest
        CurrentTest = new STFN.UoPta();

        // Creating a default wave format and silent sound
        WaveFormat = new STFN.Audio.Formats.WaveFormat(48000, 32, 2, "", STFN.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints);

        silentSound = STFN.Audio.GenerateSound.Signals.CreateSilence(ref this.WaveFormat, null, 3);

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

        // Adding PTA calibration gain values
        var CurrentMixer = OstfBase.SoundPlayer.GetMixer();
        //RightInfoLabel2.Text = CurrentMixer.GetParentTransducerSpecification.Name;
        //RightInfoLabel3.Text = CurrentMixer.GetParentTransducerSpecification.ParentAudioApiSettings.GetSelectedOutputDeviceName();

        if (CurrentMixer.GetParentTransducerSpecification.PtaCalibrationGain != null)
        {
            PureToneCalibrationList = CurrentMixer.GetParentTransducerSpecification.PtaCalibrationGain;
        }

    }

    private enum VisibilityTypes
    {
        MessageMode,
        TestMode,
        ResultMode
    }

    private void SetVisibility(VisibilityTypes visibilityType)
    {
        switch (visibilityType)
        {
            case VisibilityTypes.MessageMode:
                MessageLabel.IsVisible = false;
                LeftResponseButton.IsVisible = false;
                MidResponseButton.IsVisible = false;
                RightResponseButton.IsVisible = false;
                MessageButton.IsVisible = true;
                ResultView.IsVisible = false;
                break;
            case VisibilityTypes.TestMode:
                MessageLabel.IsVisible = true;
                
                //LeftResponseButton.IsVisible = true;
                LeftResponseButton.IsVisible = false;

                MidResponseButton.IsVisible = true;
                
                //RightResponseButton.IsVisible = true;
                RightResponseButton.IsVisible = false;

                MessageButton.IsVisible = false;

                ResultView.IsVisible = false;
                break;

            case VisibilityTypes.ResultMode:
                MessageLabel.IsVisible = false;
                LeftResponseButton.IsVisible = false;
                MidResponseButton.IsVisible = false;
                RightResponseButton.IsVisible = false;
                MessageButton.IsVisible = true;
                ResultView.IsVisible = true;
                break;

            default:
                throw new NotImplementedException("Unknown VisibilityType");
        }

    }

    private void MessageButton_Clicked(object sender, EventArgs e)
    {

        if (CurrentTest.IsCompleted() == false)
        {
            // Goes into test mode and starts the next trial
            SetVisibility(VisibilityTypes.TestMode);

            // And into fullscreen mode
            SetFullScreenMode(true);

            // Starting the testing loop
            StartNextTrial();
            StartTimer();
        }
        else
        {
            // Stopping the testing loop
            TrialEndTimer.Stop();

            // Leaves the fullscreen mode
            SetFullScreenMode(false);

            // Shows test results
            SetVisibility(VisibilityTypes.ResultMode);
            ResultView.Text = CurrentTest.ResultSummary;

        }
    }

    private void StartNextTrial()
    {

        CurrentTrial = CurrentTest.GetNextTrial();

        // Checking if the test is completed, and messaging the user
        if (CurrentTrial == null)
        {
            if (CurrentTest.IsCompleted() == true)
            {
                // The test is completed
                SetVisibility(VisibilityTypes.MessageMode);

                // Stopping the trial end timer
                TrialEndTimer.Stop();

                // Saving test result
                if (STFN.UoPta.SaveAudiogramDataToFile == true)
                {
                    CurrentTest.ExportAudiogramData();
                }

                switch (STFN.SharedSpeechTestObjects.GuiLanguage)
                {
                    case STFN.Utils.Constants.Languages.English:
                        MessageButton.Text = "This hearing test is now completed.\nClick here to continue!";
                        break;
                    case STFN.Utils.Constants.Languages.Swedish:
                        MessageButton.Text = "Detta test är nu klart.\nTryck här för att gå vidare!";
                        break;
                    default:
                        // Using English as default
                        MessageButton.Text = "This hearing test is now completed.\nClick here to continue!";
                        break;
                }
            }
            else
            {
                // Something else has caused the CurrentTrial to be null. Why? Message the user...
            }
            // Returning from the method early
            return;
        }

        // Mixing the sound
        double RetSplCorrectedLevel = CurrentTrial.ToneLevel + RetSplList[CurrentTrial.ParentSubTest.Frequency];
        double CalibratedRetSplCorrectedLevel = RetSplCorrectedLevel + PureToneCalibrationList[CurrentTrial.ParentSubTest.Frequency];
        double RetSplCorrectedLevel_FS = STFN.Audio.AudioManagement.Standard_dBSPL_To_dBFS(CalibratedRetSplCorrectedLevel);

        // Here, to make sure the tone does not get in distorted integer sound formats, we should also add the output channel specific general calibration gain added by the sound player and which we can get from:
        // var CurrentMixer = OstfBase.SoundPlayer.GetMixer();
        // CurrentMixer.GetParentTransducerSpecification.CalibrationGain();
        // Skipping this for now

        //if (RetSplCorrectedLevel_FS > -3)
        //{
        //    Messager.MsgBox("This level (" + CurrentTrial.ToneLevel.ToString() + " dB HL) exceeds the maximum level of the output device for the frequency " + CurrentTrial.ParentSubTest.Frequency.ToString() + " Hz.\n\nThe tone will not be played.", Messager.MsgBoxStyle.Information, "Maximum output level reached!");
        //    return;
        //}


        // Creating the sine 

        //Checking that there is a RETSPL correction value available
        if (RetSplList.ContainsKey(CurrentTrial.ParentSubTest.Frequency) == false)
        {
            Messager.MsgBox("Missing RETSPL value for frequency " + CurrentTrial.ParentSubTest.Frequency.ToString());
            return;
        }

        //Checking that there is a calibration value available
        if (PureToneCalibrationList.ContainsKey(CurrentTrial.ParentSubTest.Frequency) == false)
        {
            Messager.MsgBox("Missing pure tone calibration value for frequency " + CurrentTrial.ParentSubTest.Frequency.ToString());
            return;
        }

        // Creating the sine
        STFN.Audio.Sound SineSound = STFN.Audio.GenerateSound.Signals.CreateSineWave(ref this.WaveFormat, 1, CurrentTrial.ParentSubTest.Frequency, (decimal)RetSplCorrectedLevel_FS, STFN.Audio.AudioManagement.SoundDataUnit.dB, 
            CurrentTrial.ToneDuration.TotalSeconds, STFN.Audio.BasicAudioEnums.TimeUnits.seconds, 0, true);
        STFN.Audio.DSP.Transformations.Fade(ref SineSound, null, 0, 1, 0, (int)(WaveFormat.SampleRate * 0.1), STFN.Audio.DSP.Transformations.FadeSlopeType.Linear);
        STFN.Audio.DSP.Transformations.Fade(ref SineSound, 0, null, 1, (int)(-WaveFormat.SampleRate * 0.1), null, STFN.Audio.DSP.Transformations.FadeSlopeType.Linear);

        // Padding the sine with the initial silence 
        SineSound.ZeroPad(CurrentTrial.ToneOnsetTime.TotalSeconds, null, false);

        // Creating a stereo sound
        STFN.Audio.Sound TrialSound = new STFN.Audio.Sound(WaveFormat);

        switch (CurrentTrial.ParentSubTest.Side)
        {
            case STFN.Utils.Constants.Sides.Left:

                TrialSound.WaveData.set_SampleData(1, SineSound.WaveData.get_SampleData(1)); 
                // Leaving the other channel empty

                break;
            case STFN.Utils.Constants.Sides.Right:

                TrialSound.WaveData.set_SampleData(2, SineSound.WaveData.get_SampleData(1));
                // Leaving the other channel empty

                break;
            default:
                throw new Exception("Invalid value for sides");
                //break;
        }

        // Storing the sound in the trial (probabilty unecessary)
        CurrentTrial.MixedSound = TrialSound;

        // Storing the trial start time
        CurrentTrial.TrialStartTime = DateTime.Now;

        // Starting the playback
        OstfBase.SoundPlayer.SwapOutputSounds(ref TrialSound);

    }



    private void OnLeftResponseButtonPressed(object sender, EventArgs e)
    {
        LeftResponseButton.Background = PressedBrush;

        if (CurrentTrial != null)
        {
            CurrentTrial.ResponseOnsetTime = DateTime.Now - CurrentTrial.TrialStartTime;
        }

    }

    private void OnMidResponseButtonPressed(object sender, EventArgs e)
    {
        MidResponseButton.Background = PressedBrush;

        if (CurrentTrial != null)
        {
            CurrentTrial.ResponseOnsetTime = DateTime.Now - CurrentTrial.TrialStartTime;
        }

    }


    private void OnRightResponseButtonPressed(object sender, EventArgs e)
    {
        RightResponseButton.Background = PressedBrush;

        if (CurrentTrial != null)
        {
            CurrentTrial.ResponseOnsetTime = DateTime.Now - CurrentTrial.TrialStartTime;
        }

    }





    private void OnLeftResponseButtonReleased(object sender, EventArgs e)
    {
        LeftResponseButton.Background = NonPressedBrush;

        if (CurrentTrial != null)
        {
            CurrentTrial.ResponseOffsetTime = DateTime.Now - CurrentTrial.TrialStartTime;
        }

    }

    private void OnMidResponseButtonReleased(object sender, EventArgs e)
    {
        MidResponseButton.Background = NonPressedBrush;

        if (CurrentTrial != null)
        {
            CurrentTrial.ResponseOffsetTime = DateTime.Now - CurrentTrial.TrialStartTime;
        }

    }

    private void OnRightResponseButtonReleased(object sender, EventArgs e)
    {
        RightResponseButton.Background = NonPressedBrush;

        if (CurrentTrial != null)
        {
            CurrentTrial.ResponseOffsetTime = DateTime.Now - CurrentTrial.TrialStartTime;
        }

    }



    private void StartTimer()
    {

        // Create and setup timer
        TrialEndTimer = Application.Current.Dispatcher.CreateTimer();
        TrialEndTimer.Interval = TimeSpan.FromMilliseconds(50); // Checking if trial has ended every 50th ms
        TrialEndTimer.Tick += TrialEndTimer_Tick;
        TrialEndTimer.IsRepeating = true;
        TrialEndTimer.Start();

    }

    void TrialEndTimer_Tick(object sender, EventArgs e)
    {

        if (CurrentTrial != null)
        {
            if (CurrentTrial.Result == UoPta.PtaResults.TruePositive)
            {
                // We should wait PostTruePositiveResponseDuration seconds after a true positive response before next trial is started
                if (DateTime.Now > CurrentTrial.TrialStartTime + CurrentTrial.ResponseOffsetTime + TimeSpan.FromSeconds(STFN.UoPta.PostTruePositiveResponseDuration))
                {
                    //Saving trial data to file
                    if (STFN.UoPta.LogTrialData == true) { CurrentTrial.ExportPtaTrialData(); }

                    // Setting CurrentTrial to null and starting next trial
                    CurrentTrial = null;
                    StartNextTrial();
                }
            }
            else
            {
                // If we don't have a true positive response, we should wait PostToneDuration seconds after the end of the signal before next trial is started
                if (DateTime.Now > CurrentTrial.TrialStartTime + CurrentTrial.ToneOffsetTime + TimeSpan.FromSeconds(STFN.UoPta.PostToneDuration))
                {
                    //Saving trial data to file
                    if (STFN.UoPta.LogTrialData == true) { CurrentTrial.ExportPtaTrialData(); }

                    // Setting CurrentTrial to null and starting next trial
                    CurrentTrial = null;
                    StartNextTrial();
                }
            }
        }

    }

    private void SetFullScreenMode(bool Fullscreen)
    {

        if (Fullscreen == true)
        {
            EventHandler<EventArgs> handler = EnterFullScreenMode;
            EventArgs e = new EventArgs();
            if (handler != null)
            {
                handler(this, e);
            }
        }
        else
        {
            EventHandler<EventArgs> handler = ExitFullScreenMode;
            EventArgs e = new EventArgs();
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }


}