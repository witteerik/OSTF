using STFN;
using static STFN.ResponseViewEvents;

namespace STFM.Views;

public partial class SpeechTestView : ContentView, IDrawable
{

    ResponseView CurrentResponseView;
    TestResultsView CurrentTestResultsView;
    
    SpeechTest CurrentSpeechTest
    {
        get { return STFN.SharedSpeechTestObjects.CurrentSpeechTest; }
        set { STFN.SharedSpeechTestObjects.CurrentSpeechTest = value; }
    }

    

    OstfBase.AudioSystemSpecification SelectedTransducer = null;

    private string[] availableTests = new string[] {"Svenska HINT", "Hagermans meningar (Matrix)", "Hörtröskel för tal (HTT)", "PB50", "Quick SiP", "SiP-testet"};

    RowDefinition originalBottomPanelHeight = null;
    ColumnDefinition originalLeftPanelWidth = null;
    View CurrentTestOptionsView = null;


    private List<IDispatcherTimer> testTrialEventTimerList = null;

    public SpeechTestView()
	{
		InitializeComponent();

        //MyAudiogramView.Audiogram.Areas.Add(new Area()
        //{
        //    Color = Colors.Turquoise,
        //    XValues = new[] { 250F, 1000F, 4000F, 6000F },
        //    YValuesLower = new[] { 20F, 30F, 35F, 40F },
        //    YValuesUpper = new[] { 40F, 50F, 60F, 70F }
        //});

        // Set start IsEnabled values of controls
        NewTestBtn.IsEnabled = true;   
        SpeechTestPicker.IsEnabled = false;
        TestOptionsGrid.IsEnabled = false;
        if (CurrentTestOptionsView != null) { CurrentTestOptionsView.IsEnabled = false; }
        //HideSettingsPanelSwitch.IsEnabled = false;
        //HideResultsPanelSwitch.IsEnabled = false;
        StartTestBtn.IsEnabled = false;
        PauseTestBtn.IsEnabled = false;
        StopTestBtn.IsEnabled = false;
        TestReponseGrid.IsEnabled = false;
        TestResultGrid.IsEnabled = false;

        foreach (string test in availableTests)
        {
            SpeechTestPicker.Items.Add(test);
        }


    }


    void IDrawable.Draw(ICanvas canvas, RectF dirtyRect)
    {

        //MyAudiogramView.Audiogram.Draw(canvas, dirtyRect);

    }

    private void SetBottomPanelShow(bool show)
    {

        // Storing the original height specified in the xaml code, so that it can be reused.
        if (originalBottomPanelHeight == null)
        {
            originalBottomPanelHeight = RightSideGrid.RowDefinitions[1];
        }

        if (show)
        {
            TestResultGrid.IsVisible = true;
            RightSideGrid.RowDefinitions[1] = originalBottomPanelHeight;
        }
        else
        {
            TestResultGrid.IsVisible = false;
            RightSideGrid.RowDefinitions[1] = new RowDefinition(0);
        }

    }

    private void SetLeftPanelShow(bool show)
    {

        // Storing the original width specified in the xaml code, so that it can be reused.
        if (originalLeftPanelWidth == null)
        {
            originalLeftPanelWidth = MainSpeechTestGrid.ColumnDefinitions[0];
        }

        if (show)
        {
            TestSettingsGrid.IsVisible = true;
            MainSpeechTestGrid.ColumnDefinitions[0] = originalLeftPanelWidth;
        }
        else
        {
            TestSettingsGrid.IsVisible = false;
            MainSpeechTestGrid.ColumnDefinitions[0] = new ColumnDefinition(0);
        }
    }

    private async void NewTestBtn_Clicked(object sender, EventArgs e)
    {

        // Ititializing STFM if not already done
        if (STFM.StfmBase.IsInitialized == false)
        {
            // Initializing STFM
            await STFM.StfmBase.InitializeSTFM(SoundPlayerLayout, OstfBase.MediaPlayerTypes.Default);

            // Selecting transducer
            var LocalAvailableTransducers = STFN.OstfBase.AvaliableTransducers;
            if (LocalAvailableTransducers.Count == 0)
            {
                Messager.MsgBox("Unable to start the application since no sound transducers could be found!", Messager.MsgBoxStyle.Critical, "No transducers found");
            }
            
            // Always using the first transducer
            SelectedTransducer = LocalAvailableTransducers[0];
            if (SelectedTransducer.CanPlay == true)
            {
                // (At this stage the sound player will be started, if not already done.)
                var argAudioApiSettings = SelectedTransducer.ParentAudioApiSettings;
                var argMixer = SelectedTransducer.Mixer;
                STFN.OstfBase.SoundPlayer.ChangePlayerSettings(ref argAudioApiSettings, 48000, 32, STFN.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints, 0.1d, ref argMixer, 
                    STFN.Audio.SoundPlayers.iSoundPlayer.SoundDirections.PlaybackOnly, ReOpenStream: true, ReStartStream: true);
                SelectedTransducer.Mixer = argMixer;
            }
            else
            {
                Messager.MsgBox("Unable to start the player using the selected transducer (probably the selected output device doesn't have enough output channels?)!", Messager.MsgBoxStyle.Exclamation, "Sound player failure");
            }

        }

        // Resets the text on the start button, as this may have been changed if test was paused.
        StartTestBtn.Text = "Start";

        TestOptionsGrid.Children.Clear();

        // Set IsEnabled values of controls
        NewTestBtn.IsEnabled = false;
        SpeechTestPicker.IsEnabled = true;
        TestOptionsGrid.IsEnabled = false;
        if (CurrentTestOptionsView != null) { CurrentTestOptionsView.IsEnabled = false; }
        //HideSettingsPanelSwitch.IsEnabled = false;
        //HideResultsPanelSwitch.IsEnabled = false;
        StartTestBtn.IsEnabled = false;
        PauseTestBtn.IsEnabled = false;
        StopTestBtn.IsEnabled = false;
        TestReponseGrid.IsEnabled = false;
        TestResultGrid.IsEnabled = false;

    }


    void OnSpeechTestPickerSelectedItemChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        var selectedItem = picker.SelectedItem;

        if (selectedItem != null)
        {

            bool success = true;

            selectedSpeechTestName = (string)selectedItem;

            switch (selectedItem)
            {



                case "Svenska HINT":

                    // Speech test
                    CurrentSpeechTest = new HintSpeechTest("Swedish HINT");

                    // Testoptions
                    TestOptionsGrid.Children.Clear();
                    var newOptionsHintTestView = new OptionsViewAll();
                    TestOptionsGrid.Children.Add(newOptionsHintTestView);
                    CurrentTestOptionsView = newOptionsHintTestView;

                    break;


                case "Hagermans meningar (Matrix)":


                    // Speech test
                    CurrentSpeechTest = new MatrixSpeechTest("Swedish Matrix Test (Hagerman)");

                    // Testoptions
                    TestOptionsGrid.Children.Clear();
                    var newOptionsMatrixTestView = new OptionsViewAll();
                    TestOptionsGrid.Children.Add(newOptionsMatrixTestView);
                    CurrentTestOptionsView = newOptionsMatrixTestView;

                    break;


                case "Hörtröskel för tal (HTT)":

                    // Speech test
                    CurrentSpeechTest = new SrtSpeechTest("Swedish Spondees 23");

                    // Testoptions
                    TestOptionsGrid.Children.Clear();
                    var newOptionsSrtTestView = new OptionsViewAll();
                    TestOptionsGrid.Children.Add(newOptionsSrtTestView);
                    CurrentTestOptionsView = newOptionsSrtTestView;

                    break;

                case "SiP-testet":

                    // Speech test
                    CurrentSpeechTest = new SrtSpeechTest("Swedish SiP-test");

                    TestOptionsGrid.Children.Clear();
                    var newOptionsSipTestView2 = new OptionsViewAll();
                    TestOptionsGrid.Children.Add(newOptionsSipTestView2);
                    CurrentTestOptionsView = newOptionsSipTestView2;

                    break;

                case "Quick SiP":

                    // Speech test
                    CurrentSpeechTest = new SipSpeechTest("Swedish SiP-test");

                    TestOptionsGrid.Children.Clear();
                    var newOptionsSipTestView = new OptionsViewAll();
                    TestOptionsGrid.Children.Add(newOptionsSipTestView);
                    CurrentTestOptionsView = newOptionsSipTestView;

                    break;

                default:
                    TestOptionsGrid.Children.Clear();
                    success = false;
                    break;
            }

            if (success)
            {
                // Set IsEnabled values of controls
                NewTestBtn.IsEnabled = false;
                SpeechTestPicker.IsEnabled = false;
                TestOptionsGrid.IsEnabled = true;
                if (CurrentTestOptionsView != null) { CurrentTestOptionsView.IsEnabled = true; }
                //HideSettingsPanelSwitch.IsEnabled = true;
                //HideResultsPanelSwitch.IsEnabled = true;
                StartTestBtn.IsEnabled = true;
                PauseTestBtn.IsEnabled = false;
                StopTestBtn.IsEnabled = false;
                TestReponseGrid.IsEnabled = false;
                TestResultGrid.IsEnabled = false;
            }
        }
    }

    string selectedSpeechTestName = string.Empty;

    void InitiateTesting()
    {               

        if (CurrentSpeechTest != null)
        {

            switch (selectedSpeechTestName)
            {


                case "Svenska HINT":


                    CurrentSpeechTest.InitializeCurrentTest();

                    // Response view
                    CurrentResponseView = new ResponseView_FreeRecall();

                    CurrentResponseView.ResponseGiven += NewSpeechTestInput;
                    //TestResponseView.StartedByTestee += StartedByTestee;

                    TestReponseGrid.Children.Add(CurrentResponseView);

                    // TODO: Setting sound overlap duration, maybe better somewhere else
                    CurrentSpeechTest.SoundOverlapDuration = 0.1;

                    break;


                case "Hagermans meningar (Matrix)":


                    CurrentSpeechTest.InitializeCurrentTest();

                    // Response view
                    if (CurrentSpeechTest.CustomizableTestOptions.IsFreeRecall)
                    {
                        CurrentResponseView = new ResponseView_FreeRecall();
                    }
                    else
                    {
                        CurrentResponseView = new ResponseView_Matrix();
                    }

                    CurrentResponseView.ResponseGiven += NewSpeechTestInput;
                    //TestResponseView.StartedByTestee += StartedByTestee;

                    TestReponseGrid.Children.Add(CurrentResponseView);

                    // TODO: Setting sound overlap duration, maybe better somewhere else
                    CurrentSpeechTest.SoundOverlapDuration = 0;

                    break;


                case "Hörtröskel för tal (HTT)":


                    CurrentSpeechTest.InitializeCurrentTest();

                    // Response view
                    if (CurrentSpeechTest.CustomizableTestOptions.IsFreeRecall)
                    {
                        CurrentResponseView = new ResponseView_FreeRecall();
                    }
                    else
                    {
                        CurrentResponseView = new ResponseView_Mafc();
                    }
                    CurrentResponseView.ResponseGiven += NewSpeechTestInput;
                    //TestResponseView.StartedByTestee += StartedByTestee;

                    TestReponseGrid.Children.Add(CurrentResponseView);

                    // TODO: Setting sound overlap duration, maybe better somewhere else
                    CurrentSpeechTest.SoundOverlapDuration = 0;

                    break;

                case "SiP-testet":

                    CurrentResponseView = new ResponseView_Mafc();
                    TestReponseGrid.Children.Add(CurrentResponseView);

                    // TODO: Setting sound overlap duration, maybe better somewhere else
                    CurrentSpeechTest.SoundOverlapDuration = 0.5;

                    break;

                case "Quick SiP":


                    CurrentResponseView = new ResponseView_MafcDragDrop();
                    TestReponseGrid.Children.Add(CurrentResponseView);

                    //CurrentResponseView.AddDefaultSources();
                    //CurrentResponseView.AddResponseAlternatives(testStringArray);

                    // TODO: Setting sound overlap duration, maybe better somewhere else
                    CurrentSpeechTest.SoundOverlapDuration = 0.1;

                    break;

                default:
                    TestOptionsGrid.Children.Clear();
                    break;
            }

            // Updating sound player settings for PaBased player
            if (OstfBase.CurrentMediaPlayerType == OstfBase.MediaPlayerTypes.PaBased)
            {
                // Updating settings needed for the loaded test
                // (At this stage the sound player will be started, if not already done.)
                var argAudioApiSettings = SelectedTransducer.ParentAudioApiSettings;
                var argMixer = SelectedTransducer.Mixer;
                if (CurrentSpeechTest != null)
                {
                    var mediaSets = CurrentSpeechTest.AvailableMediasets;
                    if (mediaSets.Count > 0)
                    {
                        OstfBase.SoundPlayer.ChangePlayerSettings(ref argAudioApiSettings,
                            mediaSets[0].WaveFileSampleRate, mediaSets[0].WaveFileBitDepth, mediaSets[0].WaveFileEncoding,
                            CurrentSpeechTest.SoundOverlapDuration, Mixer: ref argMixer, ReOpenStream: true, ReStartStream: true);
                        SelectedTransducer.Mixer = argMixer;
                    }
                }
            }
            else
            {
                OstfBase.SoundPlayer.SetOverlapDuration(CurrentSpeechTest.SoundOverlapDuration);
            }
        }
    }



    private void StartTestBtn_Clicked(object sender, EventArgs e)
    {

        InitiateTesting();

        bool testIsReady = true; // This should call a fuction that check is the selected test is ready to be started
        bool testSupportPause = true; // This should call a function that determies if the test supports pausing

        if (testIsReady) {

            // Set IsEnabled values of controls
            NewTestBtn.IsEnabled = false;
            SpeechTestPicker.IsEnabled = false;
            TestOptionsGrid.IsEnabled = false;
            if (CurrentTestOptionsView != null) { CurrentTestOptionsView.IsEnabled = false; }
            //HideSettingsPanelSwitch.IsEnabled = false;
            //HideResultsPanelSwitch.IsEnabled = false;
            StartTestBtn.IsEnabled = false;
            if (testSupportPause) {PauseTestBtn.IsEnabled = true; }
            StopTestBtn.IsEnabled = true;
            TestReponseGrid.IsEnabled = true;
            TestResultGrid.IsEnabled = true;

            // Showing / hiding panels during test
            SetBottomPanelShow(CurrentSpeechTest.CustomizableTestOptions.IsFreeRecall);
            SetLeftPanelShow(CurrentSpeechTest.CustomizableTestOptions.IsFreeRecall);

            // Starting the test
            StartTest();

        }
        else
        {

            // Here we should inform the user that something is missing / not ready, and of course what...
        }

    }

    private void PauseTestBtn_Clicked(object sender, EventArgs e)
    {

        StartTestBtn.Text = "Fortsätt";

        // Set IsEnabled values of controls
        NewTestBtn.IsEnabled = true;
        SpeechTestPicker.IsEnabled = false;
        TestOptionsGrid.IsEnabled = false;
        if (CurrentTestOptionsView != null) { CurrentTestOptionsView.IsEnabled = false; }
        //HideSettingsPanelSwitch.IsEnabled = false;
        //HideResultsPanelSwitch.IsEnabled = false;
        StartTestBtn.IsEnabled = true;
        PauseTestBtn.IsEnabled = false;
        StopTestBtn.IsEnabled = true;
        TestReponseGrid.IsEnabled = false;
        TestResultGrid.IsEnabled = true;

        // Pause testing
        // ...

    }


    private void StopTestBtn_Clicked(object sender, EventArgs e)
    {

        // Showing panels again
        SetBottomPanelShow(true);
        SetLeftPanelShow(true);

        StartTestBtn.Text = "Start";

        // Set IsEnabled values of controls
        NewTestBtn.IsEnabled = true;
        SpeechTestPicker.IsEnabled = false;
        TestOptionsGrid.IsEnabled = false;
        if (CurrentTestOptionsView != null){ CurrentTestOptionsView.IsEnabled = false; } 
        //HideSettingsPanelSwitch.IsEnabled = false;
        //HideResultsPanelSwitch.IsEnabled = false;
        StartTestBtn.IsEnabled = false;
        PauseTestBtn.IsEnabled = false;
        StopTestBtn.IsEnabled = false;
        TestReponseGrid.IsEnabled = false;
        TestResultGrid.IsEnabled = true;

        // Stopping the test
        // ...

    }

    // Region run test. These methods should be common between all test types, and thus highly general: Basically, start test, loop over test trials, end test.

    void StartedByTestee(object sender, EventArgs e)
    {
        StartTest();
    }


    void StartTest()
    {

        // Calling NewSpeechTestInput with e as null
        NewSpeechTestInput(null, null);
    }

 


    void NewSpeechTestInput(object sender, SpeechTestInputEventArgs e)
    {

        switch (CurrentSpeechTest.GetSpeechTestReply(sender, e))
        {

            case SpeechTest.SpeechTestReplies.ContinueTrial:

                // Doing nothing here, but instead waiting for more responses 

                break;

            case SpeechTest.SpeechTestReplies.GotoNextTrial:

                // Stops all event timers
                StopAllTrialEventTimers();

                // Starting the trial
                PresentTrial();

                break;

            case SpeechTest.SpeechTestReplies.TestIsCompleted:

                FinalizeTest();

                break;

            case SpeechTest.SpeechTestReplies.AbortTest:

                AbortTest();

                break;

            default:
                break;

        }

        // Showing results if results view is visible
        if (TestResultGrid.IsVisible == true)
        {
            var CurrentResults = CurrentSpeechTest.GetResults();
            ShowResults(CurrentResults);
        }

    }


    void PresentTrial() {

        // Initializing a new trial, this should always stop any timers in the CurrentResponseView that may still be running from the previuos trial 
        CurrentResponseView.InitializeNewTrial();

        // Here we could add a method that starts preparing the output sound, to save some processing time
        // OstfBase.SoundPlayer.PrepareNewOutputSounds(ref CurrentSpeechTest.CurrentTestTrial.Sound);

        testTrialEventTimerList = new List<IDispatcherTimer>();

        foreach (var trialEvent in CurrentSpeechTest.CurrentTestTrial.TrialEventList) {

            //IDispatcherProvider trialProvider = null;

            // Create and setup timer
            IDispatcherTimer trialEventTimer;     
            trialEventTimer = Application.Current.Dispatcher.CreateTimer();
            trialEventTimer.Interval = TimeSpan.FromMilliseconds(trialEvent.TickTime);
            trialEventTimer.Tick += TrialEventTimer_Tick;
            trialEventTimer.IsRepeating = false;
            testTrialEventTimerList.Add(trialEventTimer);

            // Storing the timer here to be able to comparit later. Bad idea I know! But find no better now...
            trialEvent.Box = trialEventTimer;
        }

        // Starting the trial
        foreach (IDispatcherTimer timer  in testTrialEventTimerList)
        {
            timer.Start();
        }

    }


    void TrialEventTimer_Tick(object sender, EventArgs e)
    {
        if (sender != null)
        {
            IDispatcherTimer CurrentTimer = (IDispatcherTimer)sender;
            CurrentTimer.Stop();

            // Hiding everything if there was no test, no trial or no TrialEventList
            if (CurrentSpeechTest == null) {
                CurrentResponseView.HideAllItems();
                return;
            }
            if (CurrentSpeechTest.CurrentTestTrial == null) { 
                CurrentResponseView.HideAllItems();
                return;
            }
            if (CurrentSpeechTest.CurrentTestTrial.TrialEventList == null)
            {
                CurrentResponseView.HideAllItems();
                return;
            }

            // Triggering the next trial event
            foreach (var trialEvent in CurrentSpeechTest.CurrentTestTrial.TrialEventList)
            {
                if (CurrentTimer == trialEvent.Box)
                {

                    // Issuing the current trial event

                    switch (trialEvent.Type)
                    {
                        case ResponseViewEvent.ResponseViewEventTypes.PlaySound:
                            OstfBase.SoundPlayer.SwapOutputSounds(ref CurrentSpeechTest.CurrentTestTrial.Sound);
                            break;

                        case ResponseViewEvent.ResponseViewEventTypes.StopSound:
                            OstfBase.SoundPlayer.FadeOutPlayback();
                            break;

                        case ResponseViewEvent.ResponseViewEventTypes.ShowVisualSoundSources:
                            List<ResponseView. VisualizedSoundSource> soundSources = new List<ResponseView.VisualizedSoundSource>();
                            soundSources.Add(new ResponseView.VisualizedSoundSource { X = 0.3, Y = 0.15, Width = 0.1, Height = 0.1, Rotation = -15, Text = "S1", SourceLocationsName = SourceLocations.Left });
                            soundSources.Add(new ResponseView.VisualizedSoundSource { X = 0.7, Y = 0.15, Width = 0.1, Height = 0.1, Rotation = 15, Text = "S2", SourceLocationsName = SourceLocations.Right });
                            CurrentResponseView.AddSourceAlternatives(soundSources.ToArray());
                            break;

                        case ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives:
                            CurrentResponseView.ShowResponseAlternatives(CurrentSpeechTest.CurrentTestTrial.ResponseAlternativeSpellings);
                            break;

                        case ResponseViewEvent.ResponseViewEventTypes.ShowVisualCue:
                            CurrentResponseView.ShowVisualCue();
                            break;

                        case ResponseViewEvent.ResponseViewEventTypes.HideVisualCue:
                            CurrentResponseView.HideVisualCue();
                            break;

                        case ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut:
                            CurrentResponseView.ResponseTimesOut();
                            break;

                        case ResponseViewEvent.ResponseViewEventTypes.ShowMessage:
                            string tempMessage = "This is a temporary message";
                                CurrentResponseView.ShowMessage(tempMessage);
                            break;

                        case ResponseViewEvent.ResponseViewEventTypes.HideAll:
                            CurrentResponseView.HideAllItems(); 
                            break;

                        default:
                            break;
                    }

                    break;
                }
            }



            // Calls PresentNextTrial on the main thread
            //MainThread.BeginInvokeOnMainThread(PresentNextTrial);
        }
    }

    void StopAllTrialEventTimers()
    {
        // Stops all event timers
        if (testTrialEventTimerList != null)
        {
            foreach (IDispatcherTimer timer in testTrialEventTimerList)
            {
                timer.Stop();
            }
        }
    }

    void FinalizeTest()
    {

        // Stopping all timers
        StopAllTrialEventTimers();

        CurrentResponseView.HideAllItems();

        TestResults CurrentResults = CurrentSpeechTest.GetResults();

        CurrentSpeechTest.SaveResults(CurrentResults);

        ShowResults(CurrentResults);

        // Simulating a click on the stop button to show the correct things in the GUI
        StopTestBtn_Clicked(null, null);

        Messager.MsgBox("The test is finished", Messager.MsgBoxStyle.Information, "Finished", "OK");
        //CurrentResponseView.ShowMessage("Test is finished!");

    }

   void  AbortTest()
    {

        // Stopping all timers
        StopAllTrialEventTimers();

        CurrentResponseView.HideAllItems();

        TestResults CurrentResults = CurrentSpeechTest.GetResults();

        CurrentSpeechTest.SaveResults(CurrentResults);

        ShowResults(CurrentResults);

        // Simulating a click on the stop button to show the correct things in the GUI
        StopTestBtn_Clicked(null, null);

        Messager.MsgBox("The test had to be aborted", Messager.MsgBoxStyle.Information, "Aborted", "OK");
        //CurrentResponseView.ShowMessage("Test is finished!");

    }

    void ShowResults(TestResults results)
    {
        // Showing result panel
        SetBottomPanelShow(true);

        // TODO: Present results in some way
        // Use "results" to generate some kind of results view

        // Testing with a basic text only view
        TestResultGrid.Children.Clear();
        CurrentTestResultsView = new TestResultsView_Text();
        TestResultGrid.Children.Add(CurrentTestResultsView);
        CurrentTestResultsView.ShowTestResults(results);

        // Also shows the settings panel
        SetLeftPanelShow(true);

    }



}



