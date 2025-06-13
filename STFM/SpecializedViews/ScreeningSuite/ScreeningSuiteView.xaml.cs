using STFN;
using static STFN.ResponseViewEvents;
using System.Reflection;
using Microsoft.Maui.Controls.Internals;
using STFM.Pages;
using STFM.SpecializedViews.SSQ12;
using STFN.Audio.SoundPlayers;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using STFM.Views;
using Microsoft.Maui.Primitives;


namespace STFM.SpecializedViews.ScreeningSuite;

public partial class ScreeningSuiteView : ContentView
{

    int TestStage = 0;
    List<String> TestResultSummary = new List<string>();

    SSQ12_MainView SSQ12View = null;
    UoAudView ScreeningUoAudiometerView = null;

    bool TestIsInitiated;
    ResponseView CurrentResponseView;

    SpeechTest CurrentSpeechTest
    {
        get { return STFN.SharedSpeechTestObjects.CurrentSpeechTest; }
        set { STFN.SharedSpeechTestObjects.CurrentSpeechTest = value; }
    }

    OstfBase.AudioSystemSpecification SelectedTransducer = null;

    View CurrentTestOptionsView = null;

    private List<IDispatcherTimer> testTrialEventTimerList = null;

    public ScreeningSuiteView()
    {
        InitializeComponent();
        Initialize();
    }

    async void Initialize()
    {

        // Initializing STFM if not already done
        if (STFM.StfmBase.IsInitialized == false)
        {
            // Initializing STFM
            await STFM.StfmBase.InitializeSTFM(OstfBase.MediaPlayerTypes.Default);

            // Selecting transducer
            var LocalAvailableTransducers = STFN.OstfBase.AvaliableTransducers;
            if (LocalAvailableTransducers.Count == 0)
            {
                await Messager.MsgBoxAsync("Unable to start the application since no sound transducers could be found!", Messager.MsgBoxStyle.Critical, "No transducers found");
                Messager.RequestCloseApp();
            }

            if (UpdateSoundPlayerSettings() == false)
            {
                await Messager.MsgBoxAsync("Unable to start the player using the selected transducer (probably the selected output device doesn't have enough output channels?)!", Messager.MsgBoxStyle.Exclamation, "Sound player failure");
                Messager.RequestCloseApp();
            }
        }

        OstfBase.LoadAvailableSpeechMaterialSpecifications();
        var OSTF_AvailableSpeechMaterials = OstfBase.AvailableSpeechMaterials;
        if (OSTF_AvailableSpeechMaterials.Count == 0)
        {
            await Messager.MsgBoxAsync("Unable to load any speech materials.\n\n Unable to start the application. Press OK to close.", Messager.MsgBoxStyle.Exclamation);
            Messager.RequestCloseApp();
        }

        var OSTF_AvailableTests = OstfBase.AvailableTests;
        if (OSTF_AvailableTests == null)
        {
            await Messager.MsgBoxAsync("Unable to locate the 'AvailableTests.txt' text file.\n\n Unable to start the application. Press OK to close.", Messager.MsgBoxStyle.Exclamation);
            Messager.RequestCloseApp();
        }

        SetInstructionViewTexts();

        // Adding logotyp if a logotyp file exist
        string LogoTypePath = STFN.OstfBase.GetLogotypePath();
        if (LogoTypePath != "")
        {
            LogoImage.Source = LogoTypePath;
            LogoImage.IsVisible = true;
        }

    }

    private void SetInstructionViewTexts()
    {

        TestReponseGrid.IsVisible = false;

        switch (STFN.SharedSpeechTestObjects.GuiLanguage)
        {
            case STFN.Utils.Constants.Languages.Swedish:

                switch (TestStage)
                {
                    case 0:
                        // Initial instructions, starting with SSQ12
                        InstructionsHeadingLabel.Text = "Testa din hörsel!";
                        
                        InstructionsEditor.Text =
                            "\nHär kan du testa din hörsel på tre olika sätt:\n\n" +
                            " • Frågeformulär\n" +
                            " • Taluppfattningstest\n" +
                            " • Tontest (screening)\n" +
                            "\n" +
                            "Totalt tar testen omkring 5 till 10 minuter." +
                            "\n" +
                            "Klicka på knappen 'STARTA' för att komma igång!\n";

                        InstructionsImage.IsVisible = false;
                        InstructionsContinueButton.Text = "STARTA";
                        break;

                    case 1:
                        // QSiP instructions
                        InstructionsHeadingLabel.Text = "Taluppfattningstest (Quick-SiP)";
                        InstructionsEditor.Text =
                            "\nDetta test går till så här:\n" +
                            " • Du ska lyssna efter enstaviga ord som uttalas i en stadsmiljö.\n" +
                            " • Efter varje ord ska du ange vilket ord du uppfattade från ett antal svarsalternativ på skärmen.\n" +
                            " • Du har maximalt fyra sekunder på dig att svara.\n" +
                            " • Gissa om du är osäker.\n" +
                            " • Om svarsalternativen blinkar i röd färg har du inte svarat i tid.\n" +
                            " • Testet består av totalt 30 ord, som blir svårare och svårare ju längre testet går.\n\n" +
                            "INNAN du startar testet måste du sätta på dig hörlurarna.\n\n" +
                            "Placera hörlurarna med den RÖDA luren på HÖGER sida\n\n" +
                            "Starta sedan testet genom att klicka på knappen 'FORTSÄTT'\n";

                        InstructionsImage.IsVisible = false;
                        InstructionsContinueButton.Text = "FORTSÄTT";
                        break;

                    case 2:
                        // PTA screening instructions
                        InstructionsHeadingLabel.Text = "Tontest (screening)";
                        InstructionsEditor.Text =
                            "\nI detta test kommer du höra toner med varierande styrka och tonhöjd.\n" +
                            "Även tonernas längd varierar mellan en och två sekunder.\n\n" +
                            "Varje gång du hör en ton ska du trycka på en knapp på skärmen och hålla knappen intryckt så länge tonen hörs. Knappen lyser när du trycker på den.\n\n" +
                            "OBS! Släpp inte knappen förrän tonen har tystnat!\n\n" +
                            "Starta testet genom att klicka på knappen 'FORTSÄTT'\n";
                        InstructionsImage.IsVisible = true;
                        InstructionsContinueButton.Text = "FORTSÄTT";
                        break;

                    case 3:
                        // Test results presentation
                        InstructionsHeadingLabel.Text = "Dina resultat";
                        InstructionsEditor.Text = string.Join("\n", TestResultSummary);
                        InstructionsImage.IsVisible = false;
                        InstructionsContinueButton.Text = "ÅTERSTÄLL";
                        break;

                    default:
                        break;
                }


                break;
            default:
                throw new NotImplementedException("Language texts not yet implemented for the selected GUI langauge.");
        }

        // Showing the InstructionsGrid
        InstructionsGrid.IsVisible = true;

    }

    #region Button_clicks


    private void InstructionsContinueButton_Clicked(object sender, EventArgs e)
    {

        // Unsubscribing the event that updates the sound player settings when transducer is changed (if needed)
        if (CurrentSpeechTest != null)
        {
            CurrentSpeechTest.TransducerChanged -= UpdateSoundPlayerSettings;
            // Note that if, in the future, different audio formats will exist within the same SpeechMaterial, a SpeechMaterialChanged Event will also have to be implemented.
        }

        // Remove ResponseGiven handler from the CurrentResponseView (if previously set up)
        if (CurrentResponseView != null)
        {
            CurrentResponseView.ResponseGiven -= HandleResponseView_ResponseGiven;
        }

        // Setting TestIsInitiated to false to allow starting a new test
        TestIsInitiated = false;

        // Clearing the TestReponseGrid and the TestOptionsGrid
        TestReponseGrid.Children.Clear();
        TestOptionsGrid.Children.Clear();

        // Clearing CurrentSpeechTest and CurrentTestOptionsView 
        CurrentSpeechTest = null;
        CurrentTestOptionsView = null;

        // Hiding the InstructionsGrid
        InstructionsGrid.IsVisible = false;

        switch (TestStage)
        {

            case 0:
                // Start SSQ12

                // Setting MinimalVersion to true, in order to skip free text questions
                SSQ12_MainView.MinimalVersion = true;

                SSQ12View = new SSQ12_MainView();

                TestReponseGrid.Children.Add(SSQ12View);

                SSQ12View.Finished -= CurrentTestFinished;
                SSQ12View.Finished += CurrentTestFinished;

                break;

            case 1:
                // Start QSiP

                // Speech test
                //CurrentSpeechTest = new QuickSiP("Swedish SiP-test");
                CurrentSpeechTest = new QuickSiP_StaticSpeechTest("The Swedish SiP-test (Quick-SiP)");

                // Adding the event handlar that listens for transducer changes (but unsubscribing first to avoid multiple subscriptions)
                CurrentSpeechTest.TransducerChanged -= UpdateSoundPlayerSettings;
                CurrentSpeechTest.TransducerChanged += UpdateSoundPlayerSettings;

                var newOptionsQSipView = new OptionsViewAll(CurrentSpeechTest);
                TestOptionsGrid.Children.Add(newOptionsQSipView);
                CurrentTestOptionsView = newOptionsQSipView;

                // Selecting a random start list
                CurrentSpeechTest.StartList = CurrentSpeechTest.AvailableTestListsNames[SpeechTest.Randomizer.Next(0, CurrentSpeechTest.AvailableTestListsNames.Count())];

                // Response view
                if (CurrentSpeechTest.Transducer.IsHeadphones())
                {
                    // Using normal mafc response view when presented in headphones
                    CurrentResponseView = new ResponseView_Mafc();
                }
                else
                {
                    // Using mafc response view with side-panel presentation (for head movements) when presented in sound field
                    CurrentResponseView = new ResponseView_SiP_SF();
                }

                TestReponseGrid.Children.Add(CurrentResponseView);

                CurrentResponseView.ResponseGiven += HandleResponseView_ResponseGiven;

                // Starting the QSiP
                StartTest();

                break;

            case 2:
                // Start PTA Screening

                // Updating settings needed for the loaded test
                OstfBase.SoundPlayer.ChangePlayerSettings(SelectedTransducer.ParentAudioApiSettings, 48000, 32, STFN.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints, 0.1, SelectedTransducer.Mixer, ReOpenStream: true, ReStartStream: true);

                // Instantiating a UoPta with screening settings
                var CurrentScreeningPtaTest = new UoPta(PtaTestProtocol: UoPta.PtaTestProtocols.SAME96_Screening);

                // Setting UoPta to not save results to file
                UoPta.SaveAudiogramDataToFile = false;

                ScreeningUoAudiometerView = new UoAudView(CurrentScreeningPtaTest);
                TestReponseGrid.Children.Add(ScreeningUoAudiometerView);

                ScreeningUoAudiometerView.Finished -= CurrentTestFinished;
                ScreeningUoAudiometerView.Finished += CurrentTestFinished;

                ScreeningUoAudiometerView.StartTest();

                break;

            case 3:
                // Erase all data and reset test

                SSQ12View = null;
                CurrentSpeechTest = null;
                CurrentScreeningPtaTest = null;

                TestResultSummary.Clear();
                TestStage = 0;

                // Showing the instructions view again
                SetInstructionViewTexts();

                // Returns to skip any futher instructions in this method
                return;

            default:
                break;
        }

        // Show and enable the TestReponseGrid
        TestReponseGrid.IsEnabled = true;
        TestReponseGrid.IsVisible = true;

    }


    private void CurrentTestFinished(Object sender, EventArgs e)
    {

        // String the result
        switch (TestStage)
        {
            case 0:
                TestResultSummary.Add("Frågeformulär (SSQ12)\n" + SSQ12View.GetResults() + "\n");
                break;

            case 1:

                var AverageQSiPScore = CurrentSpeechTest.GetAverageScore();

                if (AverageQSiPScore.HasValue)
                {
                    string QuickSiP_ResultString = Math.Round(100 * AverageQSiPScore.Value, 0).ToString() + "% rätt";
                    TestResultSummary.Add("Taluppfattning (Quick-SiP)");
                    TestResultSummary.Add("Resultat = " + QuickSiP_ResultString + "\n");
                }
                else
                {
                    TestResultSummary.Add("Taluppfattning (Quick-SiP)");
                    TestResultSummary.Add("Resultat saknas\n");
                }
                break;

            case 2:
                TestResultSummary.Add("Tontest (screening)");
                TestResultSummary.Add(ScreeningUoAudiometerView.GetResults());
                break;

            default:
                break;
        }

        // Increasing TestStage and shows the instructions for the next test (or results if all tests are done)
        TestStage += 1;
        SetInstructionViewTexts();

    }


    #endregion

    #region Test_setup

    void SelectNextView(string selectedItem)
    {

        if (CurrentSpeechTest != null)
        {
            // Unsubscribing the event that updates the sound player settings when transducer is changed
            CurrentSpeechTest.TransducerChanged -= UpdateSoundPlayerSettings;

            // Note that if, in the future, different audio formats will exist within the same SpeechMaterial, a SpeechMaterialChanged Event will also have to be implemented.
        }


            switch (selectedItem)
            {

                case "SSQ12":

                    TestOptionsGrid.Children.Clear();
                    CurrentSpeechTest = null;
                    CurrentTestOptionsView = null;

                    var SSQ12View = new SSQ12_MainView();
                    TestReponseGrid.Children.Add(SSQ12View);

                    TestReponseGrid.IsEnabled = true;

                    TestOptionsGrid.IsEnabled = false;

                    // Returns right here to skip test-related adjustments
                    return;


                case "User-operated audiometry":

                    TestOptionsGrid.Children.Clear();
                    CurrentSpeechTest = null;
                    CurrentTestOptionsView = null;

                    // Updating settings needed for the loaded test
                    OstfBase.SoundPlayer.ChangePlayerSettings(SelectedTransducer.ParentAudioApiSettings, 48000, 32, STFN.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints, 0.1, SelectedTransducer.Mixer, ReOpenStream: true, ReStartStream: true);

                    // Instantiating a UoPta
                    STFN.UoPta CurrentPtaTest = new UoPta();

                    var UoAudiometerView = new UoAudView(CurrentPtaTest);
                    TestReponseGrid.Children.Add(UoAudiometerView);

                    TestReponseGrid.IsEnabled = true;

                    TestOptionsGrid.IsEnabled = false;

                    // Returns right here to skip test-related adjustments
                    return;

                case "User-operated audiometry (screening)":

                    TestOptionsGrid.Children.Clear();
                    CurrentSpeechTest = null;
                    CurrentTestOptionsView = null;

                    // Updating settings needed for the loaded test
                    OstfBase.SoundPlayer.ChangePlayerSettings(SelectedTransducer.ParentAudioApiSettings, 48000, 32, STFN.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints, 0.1, SelectedTransducer.Mixer, ReOpenStream: true, ReStartStream: true);

                    // Instantiating a UoPta with screening settings
                    STFN.UoPta CurrentScreeningPtaTest = new UoPta(PtaTestProtocol: UoPta.PtaTestProtocols.SAME96_Screening);

                    var ScreeningUoAudiometerView = new UoAudView(CurrentScreeningPtaTest);
                    TestReponseGrid.Children.Add(ScreeningUoAudiometerView);

                    TestReponseGrid.IsEnabled = true;

                    //ScreeningUoAudiometerView.Finished -= GotoNextView;
                    //ScreeningUoAudiometerView.Finished += GotoNextView;

                    TestOptionsGrid.IsEnabled = false;

                    // Returns right here to skip test-related adjustments
                    return;

                case "Screening audiometer (Calibration mode)":

                    TestOptionsGrid.Children.Clear();
                    CurrentSpeechTest = null;
                    CurrentTestOptionsView = null;

                    // Updating settings needed for the loaded test
                    OstfBase.SoundPlayer.ChangePlayerSettings(SelectedTransducer.ParentAudioApiSettings, 48000, 32, STFN.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints, 0.1, SelectedTransducer.Mixer, ReOpenStream: true, ReStartStream: true);

                    var audiometerCalibView = new ScreeningAudiometerView();
                    audiometerCalibView.GotoCalibrationMode();
                    TestReponseGrid.Children.Add(audiometerCalibView);

                    TestReponseGrid.IsEnabled = true;

                    TestOptionsGrid.IsEnabled = false;

                    // Returns right here to skip test-related adjustments
                    return;



                case "Quick SiP":

                    // Speech test
                    CurrentSpeechTest = new QuickSiP("Swedish SiP-test");

                    // Adding the event handlar that listens for transducer changes (but unsubscribing first to avoid multiple subscriptions)
                    CurrentSpeechTest.TransducerChanged -= UpdateSoundPlayerSettings;
                    CurrentSpeechTest.TransducerChanged += UpdateSoundPlayerSettings;

                    TestOptionsGrid.Children.Clear();
                    var newOptionsQSipView = new OptionsViewAll(CurrentSpeechTest);
                    TestOptionsGrid.Children.Add(newOptionsQSipView);
                    CurrentTestOptionsView = newOptionsQSipView;

                //CurrentTestResultsView = new TestResultView_QuickSiP();

                //CurrentTestResultsView.StartedFromTestResultView += StartTestBtn_Clicked;
                //CurrentTestResultsView.StoppedFromTestResultView += StopTestBtn_Clicked;
                //CurrentTestResultsView.PausedFromTestResultView += PauseTestBtn_Clicked;

                //TestResultGrid.Children.Add(CurrentTestResultsView);


                // Response view
                if (CurrentSpeechTest.Transducer.IsHeadphones())
                {
                    // Using normal mafc response view when presented in headphones
                    CurrentResponseView = new ResponseView_Mafc();
                }
                else
                {
                    // Using mafc response view with side-panel presentation (for head movements) when presented in sound field
                    CurrentResponseView = new ResponseView_SiP_SF();
                }

                TestReponseGrid.Children.Add(CurrentResponseView);

                CurrentResponseView.ResponseGiven += HandleResponseView_ResponseGiven;


                break;

                default:
                    TestOptionsGrid.Children.Clear();
                    break;
            }
    }

      

    private void UpdateSoundPlayerSettings(object sender, EventArgs e)
    {
        UpdateSoundPlayerSettings();
    }

    private bool UpdateSoundPlayerSettings()
    {

        // Updating settings needed for the loaded test 
        if (CurrentSpeechTest != null)
        {

            // Getting the selected transducer from the current speech test
            SelectedTransducer = CurrentSpeechTest.Transducer;

            if (SelectedTransducer.CanPlay == true)
            {
                var argAudioApiSettings = SelectedTransducer.ParentAudioApiSettings;
                var argMixer = SelectedTransducer.Mixer;
                var mediaSets = CurrentSpeechTest.AvailableMediasets;
                if (mediaSets.Count > 0)
                {
                    OstfBase.SoundPlayer.ChangePlayerSettings(argAudioApiSettings,
                        mediaSets[0].WaveFileSampleRate, mediaSets[0].WaveFileBitDepth, mediaSets[0].WaveFileEncoding,
                        CurrentSpeechTest.SoundOverlapDuration, Mixer: argMixer, ReOpenStream: true, ReStartStream: true);
                    SelectedTransducer.Mixer = argMixer;
                }
            }
            else
            {
                Messager.MsgBox("The sound player cannot play with the selected speech test and transducer!", Messager.MsgBoxStyle.Critical, "Sound player unable to play");
                return false;
            }
        }
        else
        {

            // Checking that there are any transducers
            var LocalAvailableTransducers = STFN.OstfBase.AvaliableTransducers;
            if (LocalAvailableTransducers.Count == 0)
            {
                Messager.MsgBox("Unable to start the sound player since no sound transducers could be found!", Messager.MsgBoxStyle.Critical, "No transducers found");
                return false;
            }

            // Selecting the the first transducer as default
            SelectedTransducer = LocalAvailableTransducers[0];
            if (SelectedTransducer.CanPlay == true)
            {
                // (At this stage the sound player will be started, if not already done.)
                var argAudioApiSettings = SelectedTransducer.ParentAudioApiSettings;
                var argMixer = SelectedTransducer.Mixer;
                STFN.OstfBase.SoundPlayer.ChangePlayerSettings(argAudioApiSettings, 48000, 32, STFN.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints, 0.1d, argMixer,
                    STFN.Audio.SoundPlayers.iSoundPlayer.SoundDirections.PlaybackOnly, ReOpenStream: true, ReStartStream: true);
                SelectedTransducer.Mixer = argMixer;
            }
            else
            {
                Messager.MsgBox("Unable to start the sound player using the default settings!", Messager.MsgBoxStyle.Exclamation, "Sound player failure");
                return false;
            }
        }

        //if (OstfBase.SoundPlayer.IsPlaying == true)
        //{

        // Starts listening to the FatalPlayerError event (first unsubsribing to avoid multiple subscriptions)
        OstfBase.SoundPlayer.FatalPlayerError -= OnFatalPlayerError;
        OstfBase.SoundPlayer.FatalPlayerError += OnFatalPlayerError;
        return true;
        //}
        //else
        //{
        //    Messager.MsgBox("Unable to start the player using the selected transducer!", Messager.MsgBoxStyle.Exclamation, "Sound player failure");
        //    return false;
        //}

    }



    async Task<bool> InitiateTesting()
    {

        if (TestIsInitiated == true)
        {
            // Exits right away if a test is already initiated. The user needs to pres the new-test button to be able to initiate a new test
            return true;
        }

        if (CurrentSpeechTest != null)
        {

            // Inactivating GUI updates of the TestOptions of the selected test. // N.B. As of now, this is never turned on again, which means that the GUI connection will not work properly after the test has been started. 
            // The reason we need to inactivate the GUI connection is that when the GUI is updated asynchronosly, some objects needed for testing may not have been set before they are needed.
            CurrentSpeechTest.SkipGuiUpdates = true;

            // Initializing the test
            Tuple<bool, string> testInitializationResponse = CurrentSpeechTest.InitializeCurrentTest();
            await HandleTestInitializationResult(testInitializationResponse);
            if (testInitializationResponse.Item1 == false)
            {

                // Initialization was not successful
                if (CurrentResponseView != null)
                {
                    // Removing event handlers
                    CurrentResponseView.ResponseGiven -= HandleResponseView_ResponseGiven;
                    CurrentResponseView.ResponseHistoryUpdated -= ResponseHistoryUpdate;
                    CurrentResponseView.CorrectionButtonClicked -= ResponseViewCorrectionButtonClicked;
                    CurrentResponseView.StartedByTestee -= StartedByTestee;

                    // Removing the response view
                    CurrentResponseView = null;

                }

                // Removing the speech test
                CurrentSpeechTest = null;

                switch (STFN.SharedSpeechTestObjects.GuiLanguage)
                {
                    case STFN.Utils.Constants.Languages.Swedish:
                        Messager.MsgBox("Något gick fel när testet skulle skapas! Välj nytt test och se till att alla nödvändiga val är ifyllda.", Messager.MsgBoxStyle.Information, "Något gick fel!");
                        break;
                    default:
                        Messager.MsgBox("Something went wrong when the test was created! Please try again and make sure that all required settings have been made!", Messager.MsgBoxStyle.Information, "Something went wrong!");
                        break;
                }

                return false;
            }

            // Unsubsribes from the sound player updates from the change of transducers
            CurrentSpeechTest.TransducerChanged -= UpdateSoundPlayerSettings;

            // Setting TestIsInitiated to true to allow starting the test, and block any re-initializations
            TestIsInitiated = true;
            return true;

        }
        else
        {
            switch (STFN.SharedSpeechTestObjects.GuiLanguage)
            {
                case STFN.Utils.Constants.Languages.Swedish:
                    Messager.MsgBox("Inget test kunde skapas!", Messager.MsgBoxStyle.Information, "Något gick fel!");
                    break;
                default:
                    Messager.MsgBox("No test has been created!", Messager.MsgBoxStyle.Information, "Something went wrong!");
                    break;
            }

            return false;
        }
    }

    async Task HandleTestInitializationResult(Tuple<bool, string> testInitializationResult)
    {
        if (testInitializationResult.Item2.Trim() != "")
        {
            string MsgBoxTitle = "";
            switch (STFN.SharedSpeechTestObjects.GuiLanguage)
            {
                case STFN.Utils.Constants.Languages.Swedish:
                    MsgBoxTitle = "Test valda testet säger:";
                    break;
                default:
                    MsgBoxTitle = "The selected test says:";
                    break;
            }
            await Messager.MsgBoxAsync(testInitializationResult.Item2.Trim(), Messager.MsgBoxStyle.Information, MsgBoxTitle);
        }
    }

    #endregion


    //The following methods should be common between all test types, and thus highly general: Basically, start test, loop over test trials, end test.
    #region Running_test 

    async void StartTest()
    {

        bool InitializationResult = await InitiateTesting();
        if (InitializationResult == false)
        {
            FinalizeTest(false);
            TestOptionsGrid.Children.Clear();
            return;
        }

        // Calling NewSpeechTestInput with e as null
        // Making the call on a separate a background thread so that the GUI changes doesn't have to wait for the creation of the initial test stimuli 
        await Task.Run(() => HandleResponseView_ResponseGiven(null, null));

    }


    void StartedByTestee(object sender, EventArgs e)
    {
        // Not used
    }


    void ResponseViewCorrectionButtonClicked(object sender, EventArgs e)
    {
        // Registering timed trial event
        if (CurrentSpeechTest.CurrentTestTrial != null)
        {
            CurrentSpeechTest.CurrentTestTrial.TimedEventsList.Add(new Tuple<TestTrial.TimedTrialEvents, DateTime>(TestTrial.TimedTrialEvents.TestAdministratorCorrectedResponse, DateTime.Now));
        }
    }


    void UpdateTestFormProgressbar(int Value, int Maximum, int Minimum)
    {

        // Directing the call to the main thread if not already on the main thread
        /// if (MainThread.IsMainThread == false) { MethodBase currentMethod = MethodBase.GetCurrentMethod(); MainThread.BeginInvokeOnMainThread(() => { currentMethod.Invoke(this, [Value, Maximum, Minimum]); }); return; }

        if (CurrentResponseView != null)
        {
            CurrentResponseView.UpdateTestFormProgressbar(Value, Maximum, Minimum);
        }

    }

    void HandleResponseView_ResponseGiven(object sender, SpeechTestInputEventArgs e)
    {
        // This method handles the event triggered by a response in the response view, and calls the GetSpeechTestReply method of the CurrentSpeechTest to determine what should happen next.
        // Calls to this method should always be done from a worker thread! See explanation below!

        // Stops all event timers. N.B. This disallows automatic response after the first incoming response
        // Note that this is allways done twice, both before and after the SleepMilliseconds delay described below.
        StopAllTrialEventTimers();

        // Ignores any calls from the resonse GUI if test is not running (i.e. paused or stopped, etc).
        // Note that this is allways done twice, both before and after the SleepMilliseconds delay described below.
        // TODO! Do we need to fix a similar functionality in the Screening Suite, as the line below?
        //if (CurrentGuiLayoutState != GuiLayoutStates.TestIsRunning) { return; }


        int SleepMilliseconds = 300; // N.B. The dalay works fine at 10 ms, but if we need to show a GUI response such as flashing the response
                                     // alternatives in red when no response is given, this delay needs to be longer. Setting it to 300 ms across all tests forces an extra interstimulus interval of 300 ms. 
                                     // Perhaps this should be set specifically by each test...
                                     //int SleepMilliseconds = 10;
                                     // A call to this method should allways be done from a worker thread, in order to allow the GUI to be updated after a response is given.
                                     // Effectively, the delay places the calls made in this method later in the MainThread Que than the GUI update. (Or at least, that's what I think it does...)
                                     // However, to avoid problems associated with multiple threads in the application, the call is directed back to the main thread already at this point,
                                     // after applying a delay to the worker thread of SleepMilliseconds ms, giving the MainThread time to update the GUI. 
                                     // (If this for som ereason should fail, the GUI will not be immediately updated, but the next trial should be loaded just fine.)
                                     // If SleepMilliseconds is not enough for the GUI to get updated, its value should be increased.
                                     // Thus, any calls to this method will cause a SleepMilliseconds ms delay. This is corrected for in the registration of timed events below.
                                     // The follwoing code block directs the call to the main thread (if not already on the main thread)
        if (MainThread.IsMainThread == false)
        {
            Thread.Sleep(SleepMilliseconds);
            MethodBase currentMethod = MethodBase.GetCurrentMethod();
            MainThread.BeginInvokeOnMainThread(() => { currentMethod.Invoke(this, [sender, e]); });
            //await MainThread.InvokeOnMainThreadAsync(() => { currentMethod.Invoke(this, [sender, e]); });
            return;
        }


        // Registering timed trial event
        if (CurrentSpeechTest.CurrentTestTrial != null)
        {
            if (CurrentSpeechTest.IsFreeRecall == true)
            {
                CurrentSpeechTest.CurrentTestTrial.TimedEventsList.Add(new Tuple<TestTrial.TimedTrialEvents, DateTime>(TestTrial.TimedTrialEvents.TestAdministratorPressedNextTrial, DateTime.Now - TimeSpan.FromMilliseconds(SleepMilliseconds)));
            }
            else
            {
                CurrentSpeechTest.CurrentTestTrial.TimedEventsList.Add(new Tuple<TestTrial.TimedTrialEvents, DateTime>(TestTrial.TimedTrialEvents.ParticipantResponded, DateTime.Now - TimeSpan.FromMilliseconds(SleepMilliseconds)));
            }
        }


        var SpeechTestReply = CurrentSpeechTest.GetSpeechTestReply(sender, e);
        switch (SpeechTestReply)
        {

            case SpeechTest.SpeechTestReplies.ContinueTrial:

                // Doing nothing here, but instead waiting for more responses 

                break;

            case SpeechTest.SpeechTestReplies.GotoNextTrial:

                CurrentSpeechTest.SaveTestTrialResults();

                // Starting the trial
                PresentTrial();

                break;

            case SpeechTest.SpeechTestReplies.PauseTestingWithCustomInformation:

                // No test results should be saved when going to pause, as the presented trial should be retaken
                // CurrentSpeechTest.SaveTestTrialResults();

                // Resets the test PauseInformation 

                throw new NotImplementedException("Pausing is not supported in the Screening Suite");

                // PauseTest();
                CurrentSpeechTest.PauseInformation = "";

                break;

            case SpeechTest.SpeechTestReplies.TestIsCompleted:

                FinalizeTest(false);

                //CurrentSpeechTest.SaveTestTrialResults();

                //switch (STFN.SharedSpeechTestObjects.GuiLanguage)
                //{
                //    case STFN.Utils.Constants.Languages.Swedish:
                //        Messager.MsgBox(CurrentSpeechTest.GetTestCompletedGuiMessage(), Messager.MsgBoxStyle.Information, "Klart!", "OK");
                //        break;
                //    default:
                //        Messager.MsgBox(CurrentSpeechTest.GetTestCompletedGuiMessage(), Messager.MsgBoxStyle.Information, "Finished", "OK");
                //        break;
                //}

                CurrentTestFinished(null, null);

                break;

            case SpeechTest.SpeechTestReplies.AbortTest:


                FinalizeTest(true);

                CurrentSpeechTest.SaveTestTrialResults();

                //if (CurrentSpeechTest != null)
                //{
                //    if (CurrentTestResultsView != null) { CurrentTestResultsView.TakeScreenShot(CurrentSpeechTest); }
                //}

                //CurrentTestResultsView.Focus();

                //Thread.Sleep(2000);

                AbortTest(false);

                break;

            default:
                break;

        }

        // Updating progress (if test is not completed)
        if (SpeechTestReply != SpeechTest.SpeechTestReplies.TestIsCompleted)
        {
            STFN.Utils.ProgressInfo CurrentProgress = CurrentSpeechTest.GetProgress();
            if (CurrentProgress != null)
            {
                UpdateTestFormProgressbar(CurrentProgress.Value, CurrentProgress.Maximum, CurrentProgress.Minimum);
            }
        }

        // Showing results if results view is visible
        ShowResults();

    }

    void ResponseHistoryUpdate(object sender, SpeechTestInputEventArgs e)
    {

        // Acctually this should probably be dealt with on a worker thread, so leaves it on the incoming thread
        // Directing the call to the main thread if not already on the main thread
        // if (MainThread.IsMainThread == false) { MethodBase currentMethod = MethodBase.GetCurrentMethod(); MainThread.BeginInvokeOnMainThread(() => { currentMethod.Invoke(this, [sender, e]); }); return; }

        // Registering timed trial event
        if (CurrentSpeechTest.CurrentTestTrial != null)
        {
            CurrentSpeechTest.CurrentTestTrial.TimedEventsList.Add(new Tuple<TestTrial.TimedTrialEvents, DateTime>(TestTrial.TimedTrialEvents.TestAdministratorCorrectedHistoricResponse, DateTime.Now));
        }

        CurrentSpeechTest.UpdateHistoricTrialResults(sender, e);
    }

    void PresentTrial()
    {

        // Directing the call to the main thread if not already on the main thread
        /// if (MainThread.IsMainThread == false) { MethodBase currentMethod = MethodBase.GetCurrentMethod(); MainThread.BeginInvokeOnMainThread(() => { currentMethod.Invoke(this, null); }); return; }


        // Initializing a new trial, this should always stop any timers in the CurrentResponseView that may still be running from the previuos trial 
        CurrentResponseView.InitializeNewTrial();

        // Here we could add a method that starts preparing the output sound, to save some processing time
        // OstfBase.SoundPlayer.PrepareNewOutputSounds(ref CurrentSpeechTest.CurrentTestTrial.Sound);

        testTrialEventTimerList = new List<IDispatcherTimer>();

        foreach (var trialEvent in CurrentSpeechTest.CurrentTestTrial.TrialEventList)
        {

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

        // Registering timed trial event
        if (CurrentSpeechTest.CurrentTestTrial != null)
        {
            CurrentSpeechTest.CurrentTestTrial.TimedEventsList.Add(new Tuple<TestTrial.TimedTrialEvents, DateTime>(TestTrial.TimedTrialEvents.TrialStarted, DateTime.Now));
        }


        // Starting the trial
        foreach (IDispatcherTimer timer in testTrialEventTimerList)
        {
            timer.Start();
        }

    }

    void TrialEventTimer_Tick(object sender, EventArgs e)
    {

        // Directing the call to the main thread if not already on the main thread
        /// if (MainThread.IsMainThread == false) { MethodBase currentMethod = MethodBase.GetCurrentMethod(); MainThread.BeginInvokeOnMainThread(() => { currentMethod.Invoke(this, [sender, e]); }); return; }


        if (sender != null)
        {
            IDispatcherTimer CurrentTimer = (IDispatcherTimer)sender;
            CurrentTimer.Stop();

            // Hiding everything if there was no test, no trial or no TrialEventList
            if (CurrentSpeechTest == null)
            {
                CurrentResponseView.HideAllItems();
                return;
            }
            if (CurrentSpeechTest.CurrentTestTrial == null)
            {
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

                            if (CurrentSpeechTest.CurrentTestTrial != null)
                            {
                                // Registering timed trial events
                                CurrentSpeechTest.CurrentTestTrial.TimedEventsList.Add(new Tuple<TestTrial.TimedTrialEvents, DateTime>(TestTrial.TimedTrialEvents.SoundStartedPlay, DateTime.Now));

                                // Actually deriving the times for the linguistic portion of the sound
                                CurrentSpeechTest.CurrentTestTrial.TimedEventsList.Add(new Tuple<TestTrial.TimedTrialEvents, DateTime>(TestTrial.TimedTrialEvents.LinguisticSoundStarted,
                                    DateTime.Now.AddSeconds(CurrentSpeechTest.CurrentTestTrial.LinguisticSoundStimulusStartTime)));

                                CurrentSpeechTest.CurrentTestTrial.TimedEventsList.Add(new Tuple<TestTrial.TimedTrialEvents, DateTime>(TestTrial.TimedTrialEvents.LinguisticSoundEnded,
                                    DateTime.Now.AddSeconds(CurrentSpeechTest.CurrentTestTrial.LinguisticSoundStimulusStartTime + CurrentSpeechTest.CurrentTestTrial.LinguisticSoundStimulusDuration)));
                            }

                            OstfBase.SoundPlayer.SwapOutputSounds(ref CurrentSpeechTest.CurrentTestTrial.Sound);

                            break;

                        case ResponseViewEvent.ResponseViewEventTypes.StopSound:

                            // Registering timed trial event
                            if (CurrentSpeechTest.CurrentTestTrial != null)
                            {
                                CurrentSpeechTest.CurrentTestTrial.TimedEventsList.Add(new Tuple<TestTrial.TimedTrialEvents, DateTime>(TestTrial.TimedTrialEvents.SoundStopped, DateTime.Now));
                            }

                            OstfBase.SoundPlayer.FadeOutPlayback();
                            break;

                        case ResponseViewEvent.ResponseViewEventTypes.ShowVisualSoundSources:

                            // Registering timed trial event
                            if (CurrentSpeechTest.CurrentTestTrial != null)
                            {
                                CurrentSpeechTest.CurrentTestTrial.TimedEventsList.Add(new Tuple<TestTrial.TimedTrialEvents, DateTime>(TestTrial.TimedTrialEvents.VisualSoundSourcesShown, DateTime.Now));
                            }

                            List<ResponseView.VisualizedSoundSource> soundSources = new List<ResponseView.VisualizedSoundSource>();
                            soundSources.Add(new ResponseView.VisualizedSoundSource { X = 0.3, Y = 0.15, Width = 0.1, Height = 0.1, Rotation = -15, Text = "S1", SourceLocationsName = SourceLocations.Left });
                            soundSources.Add(new ResponseView.VisualizedSoundSource { X = 0.7, Y = 0.15, Width = 0.1, Height = 0.1, Rotation = 15, Text = "S2", SourceLocationsName = SourceLocations.Right });
                            CurrentResponseView.AddSourceAlternatives(soundSources.ToArray());
                            break;

                        case ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternativePositions:

                            // Registering timed trial event
                            if (CurrentSpeechTest.CurrentTestTrial != null)
                            {
                                CurrentSpeechTest.CurrentTestTrial.TimedEventsList.Add(new Tuple<TestTrial.TimedTrialEvents, DateTime>(TestTrial.TimedTrialEvents.ResponseAlternativePositionsShown, DateTime.Now));
                            }

                            CurrentResponseView.ShowResponseAlternativePositions(CurrentSpeechTest.CurrentTestTrial.ResponseAlternativeSpellings);
                            break;

                        case ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives:

                            // Registering timed trial event
                            if (CurrentSpeechTest.CurrentTestTrial != null)
                            {
                                CurrentSpeechTest.CurrentTestTrial.TimedEventsList.Add(new Tuple<TestTrial.TimedTrialEvents, DateTime>(TestTrial.TimedTrialEvents.ResponseAlternativesShown, DateTime.Now));
                            }

                            CurrentResponseView.ShowResponseAlternatives(CurrentSpeechTest.CurrentTestTrial.ResponseAlternativeSpellings);
                            break;

                        case ResponseViewEvent.ResponseViewEventTypes.ShowVisualCue:

                            // Registering timed trial event
                            if (CurrentSpeechTest.CurrentTestTrial != null)
                            {
                                CurrentSpeechTest.CurrentTestTrial.TimedEventsList.Add(new Tuple<TestTrial.TimedTrialEvents, DateTime>(TestTrial.TimedTrialEvents.VisualQueShown, DateTime.Now));
                            }

                            CurrentResponseView.ShowVisualCue();
                            break;

                        case ResponseViewEvent.ResponseViewEventTypes.HideVisualCue:

                            // Registering timed trial event
                            if (CurrentSpeechTest.CurrentTestTrial != null)
                            {
                                CurrentSpeechTest.CurrentTestTrial.TimedEventsList.Add(new Tuple<TestTrial.TimedTrialEvents, DateTime>(TestTrial.TimedTrialEvents.VisualQueHidden, DateTime.Now));
                            }

                            CurrentResponseView.HideVisualCue();
                            break;

                        case ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut:

                            // Registering timed trial event
                            if (CurrentSpeechTest.CurrentTestTrial != null)
                            {
                                CurrentSpeechTest.CurrentTestTrial.TimedEventsList.Add(new Tuple<TestTrial.TimedTrialEvents, DateTime>(TestTrial.TimedTrialEvents.ResponseTimeWasOut, DateTime.Now));
                            }

                            CurrentResponseView.ResponseTimesOut();
                            break;

                        case ResponseViewEvent.ResponseViewEventTypes.ShowMessage:

                            // Registering timed trial event
                            if (CurrentSpeechTest.CurrentTestTrial != null)
                            {
                                CurrentSpeechTest.CurrentTestTrial.TimedEventsList.Add(new Tuple<TestTrial.TimedTrialEvents, DateTime>(TestTrial.TimedTrialEvents.MessageShown, DateTime.Now));
                            }

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
        }
    }

    void StopAllTrialEventTimers()
    {

        // This call is done on whichever thread that calls it, since no GUI object should be updated by this call.
        // Directing the call to the main thread if not already on the main thread
        //if (MainThread.IsMainThread == false) { MethodBase currentMethod = MethodBase.GetCurrentMethod(); MainThread.BeginInvokeOnMainThread(() => { currentMethod.Invoke(this, null); }); return; }

        // Stops all event timers
        if (testTrialEventTimerList != null)
        {
            foreach (IDispatcherTimer timer in testTrialEventTimerList)
            {
                timer.Stop();
            }
        }
    }

    void FinalizeTest(bool wasStoppedBeforeFinished)
    {

        // Directing the call to the main thread if not already on the main thread
        /// if (MainThread.IsMainThread == false) { MethodBase currentMethod = MethodBase.GetCurrentMethod(); MainThread.BeginInvokeOnMainThread(() => { currentMethod.Invoke(this, new object[] { wasStoppedBeforeFinished }); }); return; }

        // Stopping all timers
        StopAllTrialEventTimers();
        if (CurrentResponseView != null)
        {
            CurrentResponseView.HideAllItems();
        }

        // Fading out sound
        if (OstfBase.SoundPlayer != null)
        {
            if (OstfBase.SoundPlayer.IsPlaying == true)
            {
                OstfBase.SoundPlayer.FadeOutPlayback();
            }
        }

        if (CurrentSpeechTest != null)
        {
            // Attempting to finalize the test protocol, if aborted ahead of time
            if (wasStoppedBeforeFinished == true)
            {
                CurrentSpeechTest.FinalizeTestAheadOfTime();
            }

            // Getting test results

            // Displaying test results
            ShowResults();

        }

    }



    async void AbortTest(bool closeApp)
    {

        // TODO: Maybe we shoud not have this in the Screening Suite?

        bool showDefaultInfo = true;
        bool msgBoxResult;

        if (CurrentSpeechTest != null)
        {
            if (CurrentSpeechTest.AbortInformation != "")
            {
                showDefaultInfo = false;
                msgBoxResult = await Messager.MsgBoxAsync(CurrentSpeechTest.AbortInformation, Messager.MsgBoxStyle.Information, CurrentSpeechTest.AbortInformation, "OK");
            }
        }
        if (showDefaultInfo == true)
        {
            switch (STFN.SharedSpeechTestObjects.GuiLanguage)
            {
                case STFN.Utils.Constants.Languages.Swedish:
                    msgBoxResult = await Messager.MsgBoxAsync("Testet har avbrutits.", Messager.MsgBoxStyle.Information, "Avslutat", "OK");
                    break;
                default:
                    msgBoxResult = await Messager.MsgBoxAsync("The test had to be aborted.", Messager.MsgBoxStyle.Information, "Aborted", "OK");
                    break;
            }
        }
        if (closeApp == true)
        {
            Messager.RequestCloseApp();
        }
    }

    void ShowResults()
    {

        if (CurrentSpeechTest != null)
        {

            // TODO. What to do here?

        }
    }

    #endregion

    #region Unexpected_errors

    private void OnFatalPlayerError()
    {
        Dispatcher.Dispatch(() =>
        {
            OnFatalPlayerErrorSafe();
        });
    }

    private void OnFatalPlayerErrorSafe()
    {

        switch (STFN.SharedSpeechTestObjects.GuiLanguage)
        {
            case STFN.Utils.Constants.Languages.Swedish:
                CurrentSpeechTest.AbortInformation = "Ett fel har uppstått med ljuduppspelningen! \n\nHar ljudgivarna kopplats ur?\n\nTestet måste avbrytas och appen stängas!\n\nKlicka OK, se till att rätt ljudgivare är inkopplade och starta sedan om appen.";
                break;
            default:
                CurrentSpeechTest.AbortInformation = "An error occured with the sound playback! \n\nHas the sound device been disconnected?\n\nThe test must be aborted and the app closed.\n\nPlease Click OK, ensure that the sound device is connected and then restart the app!";
                break;
        }

        FinalizeTest(true);

        CurrentSpeechTest.SaveTestTrialResults();

        AbortTest(true);

    }



    #endregion


}



