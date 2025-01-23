using Microsoft.Maui.Controls.Internals;
using STFM.Pages;
using STFN;
using STFN.Audio.SoundPlayers;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using static STFN.ResponseViewEvents;

namespace STFM.Views;

public partial class SpeechTestView : ContentView, IDrawable
{

    bool TestIsInitiated;
    string selectedSpeechTestName = string.Empty;

    GuiLayoutStates currentGuiLayoutState = GuiLayoutStates.InitialState;
    GuiLayoutStates CurrentGuiLayoutState
    {
        get
        {
            return currentGuiLayoutState;
        }
        set 
        {

            currentGuiLayoutState = value;

            //Updating GUI controls that depend on this calue
            // Set IsEnabled values of controls

            switch (currentGuiLayoutState) 
            {
                case GuiLayoutStates.InitialState:

                    NewTestBtn.IsEnabled = true;
                    SpeechTestPicker.IsEnabled = false;
                    SpeechMaterialPicker.IsEnabled = false;
                    TestOptionsGrid.IsEnabled = false;
                    if (CurrentTestOptionsView != null) { CurrentTestOptionsView.IsEnabled = false; }
                    StartTestBtn.IsEnabled = false;
                    PauseTestBtn.IsEnabled = false;
                    StopTestBtn.IsEnabled = false;
                    TestReponseGrid.IsEnabled = false;
                    TestResultGrid.IsEnabled = false;

                    // Also clearing selected test and speech material
                    SpeechTestPicker.SelectedItem = null;
                    SpeechMaterialPicker.SelectedItem = null;

                    SetLayoutConfiguration(LayoutConfiguration.Settings);

                    break;

                case GuiLayoutStates.TestSelection:

                    NewTestBtn.IsEnabled = true;
                    SpeechTestPicker.IsEnabled = true;
                    SpeechMaterialPicker.IsEnabled = false;
                    TestOptionsGrid.IsEnabled = false;
                    if (CurrentTestOptionsView != null) { CurrentTestOptionsView.IsEnabled = false; }
                    StartTestBtn.IsEnabled = false;
                    PauseTestBtn.IsEnabled = false;
                    StopTestBtn.IsEnabled = false;
                    TestReponseGrid.IsEnabled = false;
                    TestResultGrid.IsEnabled = false;

                    SetLayoutConfiguration(LayoutConfiguration.Settings);

                    break;

                case GuiLayoutStates.SpeechMaterialSelection:

                    NewTestBtn.IsEnabled = false;
                    SpeechTestPicker.IsEnabled = true;
                    SpeechMaterialPicker.IsEnabled = true;
                    TestOptionsGrid.IsEnabled = false;
                    if (CurrentTestOptionsView != null) { CurrentTestOptionsView.IsEnabled = false; }
                    StartTestBtn.IsEnabled = false;
                    PauseTestBtn.IsEnabled = false;
                    StopTestBtn.IsEnabled = false;
                    TestReponseGrid.IsEnabled = false;
                    TestResultGrid.IsEnabled = false;

                    SetLayoutConfiguration(LayoutConfiguration.Settings);

                    break;


                case GuiLayoutStates.TestOptions_StartButton_TestResultsOnForm:

                    NewTestBtn.IsEnabled = false;
                    SpeechTestPicker.IsEnabled = true;
                    //SpeechMaterialPicker.IsEnabled = true; // Leaving this unchanged as it might be enabled or not depending on if the speech material was selected manually
                    //SpeechMaterialPicker.IsEnabled = false;
                    TestOptionsGrid.IsEnabled = true;
                    if (CurrentTestOptionsView != null) { CurrentTestOptionsView.IsEnabled = true; }
                    StartTestBtn.IsEnabled = true;
                    PauseTestBtn.IsEnabled = false;
                    StopTestBtn.IsEnabled = false;
                    TestReponseGrid.IsEnabled = false;
                    TestResultGrid.IsEnabled = true;

                    SetLayoutConfiguration(LayoutConfiguration.Settings_Result_Response);

                    break;


                case GuiLayoutStates.TestOptions_StartButton_TestResultsOffForm:

                    NewTestBtn.IsEnabled = false;
                    SpeechTestPicker.IsEnabled = true;
                    //SpeechMaterialPicker.IsEnabled = true; // Leaving this unchanged as it might be enabled or not depending on if the speech material was selected manually
                    //SpeechMaterialPicker.IsEnabled = false;
                    TestOptionsGrid.IsEnabled = true;
                    if (CurrentTestOptionsView != null) { CurrentTestOptionsView.IsEnabled = true; }
                    StartTestBtn.IsEnabled = true;
                    PauseTestBtn.IsEnabled = false;
                    StopTestBtn.IsEnabled = false;
                    TestReponseGrid.IsEnabled = false;
                    TestResultGrid.IsEnabled = false;

                    SetLayoutConfiguration(LayoutConfiguration.Settings_Response);

                    break;
                case GuiLayoutStates.TestIsRunning:

                    NewTestBtn.IsEnabled = false;
                    SpeechTestPicker.IsEnabled = false;
                    SpeechMaterialPicker.IsEnabled = false;
                    TestOptionsGrid.IsEnabled = false;
                    if (CurrentTestOptionsView != null) { CurrentTestOptionsView.IsEnabled = false; }
                    StartTestBtn.IsEnabled = false;
                    if (CurrentSpeechTest.SupportsManualPausing) { PauseTestBtn.IsEnabled = true; }
                    StopTestBtn.IsEnabled = true;
                    TestReponseGrid.IsEnabled = true;
                    TestResultGrid.IsEnabled = true;

                    if (CurrentSpeechTest != null)
                    {
                        if (CurrentSpeechTest.IsFreeRecall)
                        {
                            if (HasExternalResultsView)
                            {
                                SetLayoutConfiguration(LayoutConfiguration.Settings_Response);
                            }
                            else
                            {
                                SetLayoutConfiguration(LayoutConfiguration.Settings_Result_Response);
                            }
                        }
                        else
                        {
                            SetLayoutConfiguration(LayoutConfiguration.Response);
                        }
                    }

                    break;
                case GuiLayoutStates.TestIsPaused:

                    NewTestBtn.IsEnabled = false;
                    SpeechTestPicker.IsEnabled = false;
                    SpeechMaterialPicker.IsEnabled = false;
                    TestOptionsGrid.IsEnabled = false;
                    if (CurrentTestOptionsView != null) { CurrentTestOptionsView.IsEnabled = false; }
                    StartTestBtn.IsEnabled = true;
                    PauseTestBtn.IsEnabled = false;
                    StopTestBtn.IsEnabled = true;
                    TestReponseGrid.IsEnabled = true;
                    TestResultGrid.IsEnabled = true;

                    // Not changing LayoutConfiguration here

                    break;
                case GuiLayoutStates.TestIsStopped:

                    // Set IsEnabled values of controls
                    NewTestBtn.IsEnabled = true;
                    SpeechTestPicker.IsEnabled = false;
                    SpeechMaterialPicker.IsEnabled = false;
                    TestOptionsGrid.IsEnabled = false;
                    if (CurrentTestOptionsView != null) { CurrentTestOptionsView.IsEnabled = false; }
                    StartTestBtn.IsEnabled = false;
                    PauseTestBtn.IsEnabled = false;
                    StopTestBtn.IsEnabled = false;
                    TestReponseGrid.IsEnabled = false;
                    TestResultGrid.IsEnabled = true;

                    if (CurrentSpeechTest != null)
                    {
                        if (CurrentSpeechTest.IsFreeRecall)
                        {
                            if (HasExternalResultsView)
                            {
                                SetLayoutConfiguration(LayoutConfiguration.Settings_Response);
                            }
                            else
                            {
                                SetLayoutConfiguration(LayoutConfiguration.Settings_Result_Response);
                            }
                        }
                        else
                        {
                            SetLayoutConfiguration(LayoutConfiguration.Settings_Result_Response);
                        }
                    }
                    else
                    {
                        SetLayoutConfiguration(LayoutConfiguration.Settings_Result);
                    }

                    break;
                default:
                    break;
            }


            // Also updates any other dependent controls
            if (CurrentTestResultsView != null)
            {
                CurrentTestResultsView.SetGuiLayoutState(currentGuiLayoutState);
            }

    }
}

    public enum GuiLayoutStates {
        InitialState,
        TestSelection,
        SpeechMaterialSelection,
        TestOptions_StartButton_TestResultsOnForm,
        TestOptions_StartButton_TestResultsOffForm,
        TestIsRunning,
        TestIsPaused,
        TestIsStopped
    }

    ResponseView CurrentResponseView;
    TestResultsView CurrentTestResultsView;
    Window CurrentExternalTestResultWindow;

    string SelectedSpeechMaterialName = "";

    SpeechTest CurrentSpeechTest
    {
        get { return STFN.SharedSpeechTestObjects.CurrentSpeechTest; }
        set { STFN.SharedSpeechTestObjects.CurrentSpeechTest = value; }
    }

    OstfBase.AudioSystemSpecification SelectedTransducer = null;

    private string[] availableTests;

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

        //TestResultGrid.BackgroundColor = Color.FromRgb(40, 40, 40);

        Initialize();

    }

    async void Initialize()
    {

        // Ititializing STFM if not already done
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
        foreach (SpeechMaterialSpecification speechMaterial in OSTF_AvailableSpeechMaterials)
        {
            SpeechMaterialPicker.Items.Add(speechMaterial.Name);
        }


        var OSTF_AvailableTests = OstfBase.AvailableTests;
        if (OSTF_AvailableTests == null)
        {
            await Messager.MsgBoxAsync("Unable to locate the 'AvailableTests.txt' text file.\n\n Unable to start the application. Press OK to close.", Messager.MsgBoxStyle.Exclamation);
            Messager.RequestCloseApp();
        }
        else
        {

            availableTests = OSTF_AvailableTests.ToArray();
        }

        CurrentGuiLayoutState = GuiLayoutStates.InitialState;


        foreach (string test in availableTests)
        {
            SpeechTestPicker.Items.Add(test);
        }

        SetShowTalkbackPanel();

        switch (STFN.SharedSpeechTestObjects.GuiLanguage)
        {
            case STFN.Utils.Constants.Languages.Swedish:
                TalkbackGainTitle_Span.Text = "Talkback-nivå: ";
                NewTestBtn.Text = "Nytt test";
                SpeechTestPicker.Title = "Välj ett test";
                StartTestBtn.Text = "Start";
                PauseTestBtn.Text = "Paus";
                StopTestBtn.Text = "Avbryt test";
                break;
            default:
                TalkbackGainTitle_Span.Text = "Talkback level: ";
                NewTestBtn.Text = "New test";
                SpeechTestPicker.Title = "Select a test";
                StartTestBtn.Text = "Start";
                PauseTestBtn.Text = "Pause";
                StopTestBtn.Text = "Abort test";
                break;
        }

        // Setting a default talkback gain 
        TalkbackGain = 0;

        // Views the settings view
        //SetLayoutConfiguration(LayoutConfiguration.Settings);

    }

    #region Region_panels_show_hide

    private void SetShowTalkbackPanel()
    {

        // Directing the call to the main thread if not already on the main thread
        ///  if (MainThread.IsMainThread == false) { MethodBase currentMethod = MethodBase.GetCurrentMethod(); MainThread.BeginInvokeOnMainThread(() => { currentMethod.Invoke(this, null); }); return; }

        if (OstfBase.SoundPlayer.SupportsTalkBack == true)
        {
            TalkbackControl.IsVisible = true;

            // Correcting for the presence of the talkback control
            TestSettingsGrid.SetRowSpan(TestOptionsGrid, 1);
        }
        else
        {
            TalkbackControl.IsVisible = false;

            // Correcting for the absence of the talkback control
            TestSettingsGrid.SetRowSpan(TestOptionsGrid, 2);
        }
    }


    private bool HasExternalResultsView = false;

    private enum LayoutConfiguration
    {
        Settings,
        Response,
        Settings_Response,
        Settings_Result,
        Response_Result,
        Settings_Result_Response,
    }


    LayoutConfiguration currentLayoutConfiguration = LayoutConfiguration.Settings;

    private void SetLayoutConfiguration(LayoutConfiguration layoutConfiguration)
    {

        MainSpeechTestGrid.Clear();

        switch (layoutConfiguration)
        {
            case LayoutConfiguration.Settings:

                MainSpeechTestGrid.Add(TestSettingsGrid, 0, 0);
                MainSpeechTestGrid.SetColumnSpan(TestSettingsGrid, 2);
                MainSpeechTestGrid.SetRowSpan(TestSettingsGrid, 2);

                break;
            case LayoutConfiguration.Response:

                MainSpeechTestGrid.Add(TestReponseGrid, 0, 0);
                MainSpeechTestGrid.SetColumnSpan(TestReponseGrid, 2);
                MainSpeechTestGrid.SetRowSpan(TestReponseGrid, 2);

                break;
            case LayoutConfiguration.Settings_Response:

                MainSpeechTestGrid.Add(TestSettingsGrid, 0, 0);
                MainSpeechTestGrid.SetColumnSpan(TestSettingsGrid, 1);
                MainSpeechTestGrid.SetRowSpan(TestSettingsGrid, 2);

                MainSpeechTestGrid.Add(TestReponseGrid, 1, 0);
                MainSpeechTestGrid.SetColumnSpan(TestReponseGrid, 1);
                MainSpeechTestGrid.SetRowSpan(TestReponseGrid, 2);

                break;
            case LayoutConfiguration.Settings_Result:

                MainSpeechTestGrid.Add(TestSettingsGrid, 0, 0);
                MainSpeechTestGrid.SetColumnSpan(TestSettingsGrid, 1);
                MainSpeechTestGrid.SetRowSpan(TestSettingsGrid, 2);

                MainSpeechTestGrid.Add(TestResultGrid, 1, 0);
                MainSpeechTestGrid.SetColumnSpan(TestResultGrid, 1);
                MainSpeechTestGrid.SetRowSpan(TestResultGrid, 2);

                break;
            case LayoutConfiguration.Response_Result:

                MainSpeechTestGrid.Add(TestResultGrid, 0, 0);
                MainSpeechTestGrid.SetColumnSpan(TestResultGrid, 2);
                MainSpeechTestGrid.SetRowSpan(TestResultGrid, 1);

                MainSpeechTestGrid.Add(TestReponseGrid, 0, 1);
                MainSpeechTestGrid.SetColumnSpan(TestReponseGrid, 2);
                MainSpeechTestGrid.SetRowSpan(TestReponseGrid, 1);

                break;
            case LayoutConfiguration.Settings_Result_Response:

                MainSpeechTestGrid.Add(TestSettingsGrid, 0, 0);
                MainSpeechTestGrid.SetColumnSpan(TestSettingsGrid, 1);
                MainSpeechTestGrid.SetRowSpan(TestSettingsGrid, 2);

                MainSpeechTestGrid.Add(TestResultGrid, 1, 0);
                MainSpeechTestGrid.SetColumnSpan(TestResultGrid, 1);
                MainSpeechTestGrid.SetRowSpan(TestResultGrid, 1);

                MainSpeechTestGrid.Add(TestReponseGrid, 1, 1);
                MainSpeechTestGrid.SetColumnSpan(TestReponseGrid, 1);
                MainSpeechTestGrid.SetRowSpan(TestReponseGrid, 1);

                break;
            
           
            default:
                break;
        }

        currentLayoutConfiguration = layoutConfiguration;

    }


    public void OnEnterFullScreenMode(Object sender, EventArgs e)
    {
        SetLayoutConfiguration(LayoutConfiguration.Response);
    }

    public void OnExitFullScreenMode(Object sender, EventArgs e)
    {
        SetLayoutConfiguration(LayoutConfiguration.Settings_Response);
    }

    #endregion

    #region Button_clicks

    private void NewTestBtn_Clicked(object sender, EventArgs e)
    {
        NewTest();
    }

    private void StartTestBtn_Clicked(object sender, EventArgs e)
    {

        //IHearProtocolB4SpeechTest_II TempObject = (IHearProtocolB4SpeechTest_II)CurrentSpeechTest;
        //TempObject.TestListCombinations();

        StartTest();
    }

    private void PauseTestBtn_Clicked(object sender, EventArgs e)
    {
        PauseTest();
    }

    private void StopTestBtn_Clicked(object sender, EventArgs e)
    {
       FinalizeTest(true);
    }

    #endregion

    #region Test_setup

    private void CloseExtraWindows()
    {
        // Closing all extra windows (such as testresult windows) except this first one
        var windows = Application.Current.Windows.ToList();
        foreach (Window window in windows)
        {
            if (window != this.Window)
            {
                Application.Current.CloseWindow(window);
            }
        }
    }

    private void NewTest()
    {

        // Directing the call to the main thread if not already on the main thread
        /// if (MainThread.IsMainThread == false) { MethodBase currentMethod = MethodBase.GetCurrentMethod(); MainThread.BeginInvokeOnMainThread(() => { currentMethod.Invoke(this, null); }); return; }

        // Inactivates tackback
        InactivateTalkback();

        // Setting TestIsInitiated to false to allow starting a new test
        TestIsInitiated = false;

        // Restting HasExternalResultsView 
        HasExternalResultsView = false;

        // Clearing the test-results view
        TestResultGrid.Children.Clear();
        if (CurrentTestResultsView != null)
        {
            CurrentTestResultsView = null;
        }

        //// Closing any previous external test-results window
        //if (CurrentExternalTestResultWindow != null)
        //{
        //    try
        //    {
        //        Application.Current.CloseWindow(CurrentExternalTestResultWindow);
        //    }
        //    catch (Exception)
        //    {
        //        // Ignores any error here for now
        //    }
        //}


        // Closing all extra windows (such as testresult windows) except this first one
        CloseExtraWindows();

        // Setting layout
        //SetLayoutConfiguration(LayoutConfiguration.Settings);

        // Resets the text on the start button, as this may have been changed if test was paused.
        switch (STFN.SharedSpeechTestObjects.GuiLanguage)
        {
            case STFN.Utils.Constants.Languages.Swedish:
                StartTestBtn.Text = "Start";
                break;
            default:
                StartTestBtn.Text = "Start";
                break;
        }

        TestOptionsGrid.Children.Clear();

        // Set IsEnabled values of controls
        TestReponseGrid.Children.Clear();
        //TestOptionsGrid.IsVisible = false;

        CurrentGuiLayoutState = GuiLayoutStates.TestSelection;

        // Deselecting previous test
        SpeechTestPicker.SelectedIndex = -1;

    }


    void OnSpeechTestPickerSelectedItemChanged(object sender, EventArgs e)
    {

        if (CurrentSpeechTest != null)
        {
            // Unsubscribing the event that updates the sound player settings when transducer is changed
            CurrentSpeechTest.TransducerChanged -= UpdateSoundPlayerSettings;

            // Note that if, in the future, different audio formats will exist within the same SpeechMaterial, a SpeechMaterialChanged Event will also have to be implemented.
        }

        // Closing all extra windows (such as testresult windows) except this first one
        CloseExtraWindows();

        // Restting HasExternalResultsView 
        HasExternalResultsView = false;

        // Inactivates tackback
        InactivateTalkback();

        // Clearing the test-results view
        TestResultGrid.Children.Clear();
        if (CurrentTestResultsView != null)
        {
            CurrentTestResultsView = null;
        }

        var picker = (Picker)sender;
        var selectedItem = picker.SelectedItem;

        if (selectedItem != null)
        {

            bool success = true;

            selectedSpeechTestName = (string)selectedItem;

            switch (selectedItem)
            {

                case "Screening audiometer":

                    TestOptionsGrid.Children.Clear();
                    CurrentSpeechTest = null;
                    CurrentTestOptionsView = null;

                    // Updating settings needed for the loaded test
                    OstfBase.SoundPlayer.ChangePlayerSettings(SelectedTransducer.ParentAudioApiSettings, 48000, 32, STFN.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints, 0.1, SelectedTransducer.Mixer, ReOpenStream: true, ReStartStream: true);

                    SetLayoutConfiguration(LayoutConfiguration.Settings_Response);
                    StartTestBtn.IsEnabled = false;

                    var audioMeterView = new ScreeningAudiometerView();
                    TestReponseGrid.Children.Add(audioMeterView);

                    TestReponseGrid.IsEnabled = true;

                    audioMeterView.EnterFullScreenMode -= OnEnterFullScreenMode;
                    audioMeterView.ExitFullScreenMode -= OnExitFullScreenMode;
                    audioMeterView.EnterFullScreenMode += OnEnterFullScreenMode;
                    audioMeterView.ExitFullScreenMode += OnExitFullScreenMode;

                    NewTestBtn.IsEnabled = true;
                    SpeechTestPicker.IsEnabled = false;
                    TestOptionsGrid.IsEnabled = false;

                    // Returns right here to skip test-related adjustments
                    return;

                //break;

                case "Screening audiometer (Calibration mode)":

                    TestOptionsGrid.Children.Clear();
                    CurrentSpeechTest = null;
                    CurrentTestOptionsView = null;

                    // Updating settings needed for the loaded test
                    OstfBase.SoundPlayer.ChangePlayerSettings(SelectedTransducer.ParentAudioApiSettings, 48000, 32, STFN.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints, 0.1, SelectedTransducer.Mixer, ReOpenStream: true, ReStartStream: true);

                    SetLayoutConfiguration(LayoutConfiguration.Settings_Response);

                    StartTestBtn.IsEnabled = false;

                    var audiometerCalibView = new ScreeningAudiometerView();
                    audiometerCalibView.GotoCalibrationMode();
                    TestReponseGrid.Children.Add(audiometerCalibView);

                    TestReponseGrid.IsEnabled = true;

                    audiometerCalibView.EnterFullScreenMode -= OnEnterFullScreenMode;
                    audiometerCalibView.ExitFullScreenMode -= OnExitFullScreenMode;
                    audiometerCalibView.EnterFullScreenMode += OnEnterFullScreenMode;
                    audiometerCalibView.ExitFullScreenMode += OnExitFullScreenMode;

                    NewTestBtn.IsEnabled = true;
                    SpeechTestPicker.IsEnabled = false;
                    TestOptionsGrid.IsEnabled = false;

                    // Returns right here to skip test-related adjustments
                    return;

                case "Talaudiometri":

                    CurrentGuiLayoutState = GuiLayoutStates.SpeechMaterialSelection;

                    return;


                case "Svenska HINT":

                    SpeechMaterialPicker.SelectedItem = "Swedish HINT";

                    // Speech test
                    CurrentSpeechTest = new HintSpeechTest("Swedish HINT");

                    // Adding the event handlar that listens for transducer changes (but unsubscribing first to avoid multiple subscriptions)
                    CurrentSpeechTest.TransducerChanged -= UpdateSoundPlayerSettings;
                    CurrentSpeechTest.TransducerChanged += UpdateSoundPlayerSettings;

                    // Testoptions
                    TestOptionsGrid.Children.Clear();
                    var newOptionsHintTestView = new OptionsViewAll(CurrentSpeechTest);
                    TestOptionsGrid.Children.Add(newOptionsHintTestView);
                    CurrentTestOptionsView = newOptionsHintTestView;

                    // Creating test result view, on form (HINT is always free recall)
                    TestResultGrid.Children.Clear();
                    CurrentTestResultsView = new TestResultView_Adaptive();

                    CurrentTestResultsView.StartedFromTestResultView += StartTestBtn_Clicked;
                    CurrentTestResultsView.StoppedFromTestResultView += StopTestBtn_Clicked;
                    CurrentTestResultsView.PausedFromTestResultView += PauseTestBtn_Clicked;

                    TestResultGrid.Children.Add(CurrentTestResultsView);

                    // Setting CurrentTestPlayState 
                    CurrentGuiLayoutState = GuiLayoutStates.TestOptions_StartButton_TestResultsOnForm;

                    break;


                case "Hagermans meningar (Matrix)":

                    SpeechMaterialPicker.SelectedItem = "Swedish Matrix Test (Hagerman)";

                    // Speech test
                    CurrentSpeechTest = new MatrixSpeechTest("Swedish Matrix Test (Hagerman)");

                    // Adding the event handlar that listens for transducer changes (but unsubscribing first to avoid multiple subscriptions)
                    CurrentSpeechTest.TransducerChanged -= UpdateSoundPlayerSettings;
                    CurrentSpeechTest.TransducerChanged += UpdateSoundPlayerSettings;

                    // Testoptions
                    TestOptionsGrid.Children.Clear();
                    var newOptionsMatrixTestView = new OptionsViewAll(CurrentSpeechTest);
                    TestOptionsGrid.Children.Add(newOptionsMatrixTestView);
                    CurrentTestOptionsView = newOptionsMatrixTestView;

                    // Creating test result view
                    // TODO: If the matrix test is to be run in MAFC-mode, such a test should be separate from this, available from the test selection list.
                    // Such a test should create an control window for the test administrator. Here we use only an on-form test results control
                    TestResultGrid.Children.Clear();
                    CurrentTestResultsView = new TestResultView_Adaptive();

                    CurrentTestResultsView.StartedFromTestResultView += StartTestBtn_Clicked;
                    CurrentTestResultsView.StoppedFromTestResultView += StopTestBtn_Clicked;
                    CurrentTestResultsView.PausedFromTestResultView += PauseTestBtn_Clicked;

                    TestResultGrid.Children.Add(CurrentTestResultsView);

                    // Setting CurrentTestPlayState 
                    CurrentGuiLayoutState = GuiLayoutStates.TestOptions_StartButton_TestResultsOnForm;

                    break;


                case "Hörtröskel för tal (HTT)":

                    SpeechMaterialPicker.SelectedItem = "Swedish Spondees 23";

                    // Speech test
                    CurrentSpeechTest = new HTT23SpeechTest("Swedish Spondees 23");

                    // Adding the event handlar that listens for transducer changes (but unsubscribing first to avoid multiple subscriptions)
                    CurrentSpeechTest.TransducerChanged -= UpdateSoundPlayerSettings;
                    CurrentSpeechTest.TransducerChanged += UpdateSoundPlayerSettings;

                    // Testoptions
                    TestOptionsGrid.Children.Clear();
                    var newOptionsSrtTestView = new OptionsViewAll(CurrentSpeechTest);
                    TestOptionsGrid.Children.Add(newOptionsSrtTestView);
                    CurrentTestOptionsView = newOptionsSrtTestView;

                    // Creating test result view. This is always on form, as it is always a free-recall test
                    TestResultGrid.Children.Clear();
                    CurrentTestResultsView = new TestResultView_Adaptive();

                    CurrentTestResultsView.StartedFromTestResultView += StartTestBtn_Clicked;
                    CurrentTestResultsView.StoppedFromTestResultView += StopTestBtn_Clicked;
                    CurrentTestResultsView.PausedFromTestResultView += PauseTestBtn_Clicked;

                    TestResultGrid.Children.Add(CurrentTestResultsView);

                    // Setting CurrentTestPlayState 
                    CurrentGuiLayoutState = GuiLayoutStates.TestOptions_StartButton_TestResultsOnForm;

                    break;

                case "Manuell TP i brus":

                    SpeechMaterialPicker.SelectedItem = "SwedishTP50";

                    // Speech test
                    //CurrentSpeechTest = new IHearProtocolB2SpeechTest("SwedishMonosyllablesTP800"); // This line was used during I HeAR data collection. The speech material name was changed after Protocol B1, but as this line was hard coded in the tablets a temporary speech material named SwedishMonosyllablesTP800 but located in the "SwedishTP50" folder was used. 
                    CurrentSpeechTest = new TP50SpeechTest("SwedishTP50"); // This line is new from 2024-11-02 (but not used in the data collection for protocol B2.

                    // Adding the event handlar that listens for transducer changes (but unsubscribing first to avoid multiple subscriptions)
                    CurrentSpeechTest.TransducerChanged -= UpdateSoundPlayerSettings;
                    CurrentSpeechTest.TransducerChanged += UpdateSoundPlayerSettings;

                    // Testoptions
                    TestOptionsGrid.Children.Clear();
                    var newOptionsPB2TestView = new OptionsViewAll(CurrentSpeechTest);
                    TestOptionsGrid.Children.Add(newOptionsPB2TestView);
                    CurrentTestOptionsView = newOptionsPB2TestView;

                    // Creating test result view. This is always on form, as it is always a free-recall test
                    TestResultGrid.Children.Clear();
                    CurrentTestResultsView = new TestResultView_ConstantStimuli();

                    CurrentTestResultsView.StartedFromTestResultView += StartTestBtn_Clicked;
                    CurrentTestResultsView.StoppedFromTestResultView += StopTestBtn_Clicked;
                    CurrentTestResultsView.PausedFromTestResultView += PauseTestBtn_Clicked;

                    TestResultGrid.Children.Add(CurrentTestResultsView);

                    // Setting CurrentTestPlayState 
                    CurrentGuiLayoutState = GuiLayoutStates.TestOptions_StartButton_TestResultsOnForm;

                    break;


                case "Quick SiP":

                    SpeechMaterialPicker.SelectedItem = "Swedish SiP-test";

                    // Speech test
                    CurrentSpeechTest = new QuickSiP("Swedish SiP-test");

                    // Adding the event handlar that listens for transducer changes (but unsubscribing first to avoid multiple subscriptions)
                    CurrentSpeechTest.TransducerChanged -= UpdateSoundPlayerSettings;
                    CurrentSpeechTest.TransducerChanged += UpdateSoundPlayerSettings;

                    TestOptionsGrid.Children.Clear();
                    var newOptionsQSipView = new OptionsViewAll(CurrentSpeechTest);
                    TestOptionsGrid.Children.Add(newOptionsQSipView);
                    CurrentTestOptionsView = newOptionsQSipView;

                    // Creating test result view. Using an external test-results window on PC and on-form on other platforms
                    if (OstfBase.CurrentPlatForm == OstfBase.Platforms.WinUI & OstfBase.UseExtraWindows == true)
                    {

                        HasExternalResultsView = true;
                        CurrentTestResultsView = new TestResultView_QuickSiP();

                        CurrentTestResultsView.StartedFromTestResultView += StartTestBtn_Clicked;
                        CurrentTestResultsView.StoppedFromTestResultView += StopTestBtn_Clicked;
                        CurrentTestResultsView.PausedFromTestResultView += PauseTestBtn_Clicked;

                        TestResultPage NewTestResultPage = new TestResultPage(ref CurrentTestResultsView);
                        CurrentExternalTestResultWindow = new Window(NewTestResultPage);
                        CurrentExternalTestResultWindow.Title = "OSTF Tablet Suite - Test results window - Quick SiP";
                        CurrentExternalTestResultWindow.Height = 240;
                        CurrentExternalTestResultWindow.Width = 1200;
                        Application.Current.OpenWindow(CurrentExternalTestResultWindow);

                        // Setting CurrentTestPlayState 
                        CurrentGuiLayoutState = GuiLayoutStates.TestOptions_StartButton_TestResultsOffForm;

                    }
                    else
                    {

                        TestResultGrid.Children.Clear();
                        CurrentTestResultsView = new TestResultView_QuickSiP();

                        CurrentTestResultsView.StartedFromTestResultView += StartTestBtn_Clicked;
                        CurrentTestResultsView.StoppedFromTestResultView += StopTestBtn_Clicked;
                        CurrentTestResultsView.PausedFromTestResultView += PauseTestBtn_Clicked;

                        TestResultGrid.Children.Add(CurrentTestResultsView);

                        // Setting CurrentTestPlayState 
                        CurrentGuiLayoutState = GuiLayoutStates.TestOptions_StartButton_TestResultsOnForm;

                    }

                    break;

                case "SiP-testet (Adaptivt)":

                    SpeechMaterialPicker.SelectedItem = "Swedish SiP-test";

                    // Speech test
                    CurrentSpeechTest = new AdaptiveSiP("Swedish SiP-test");

                    // Adding the event handlar that listens for transducer changes (but unsubscribing first to avoid multiple subscriptions)
                    CurrentSpeechTest.TransducerChanged -= UpdateSoundPlayerSettings;
                    CurrentSpeechTest.TransducerChanged += UpdateSoundPlayerSettings;

                    TestOptionsGrid.Children.Clear();
                    var newOptionsASipTestView = new OptionsViewAll(CurrentSpeechTest);
                    TestOptionsGrid.Children.Add(newOptionsASipTestView);
                    CurrentTestOptionsView = newOptionsASipTestView;


                    // Creating test result view. Using an external test-results window on PC and on-form on other platforms
                    if (OstfBase.CurrentPlatForm == OstfBase.Platforms.WinUI & OstfBase.UseExtraWindows == true)
                    {

                        HasExternalResultsView = true;
                        CurrentTestResultsView = new TestResultView_AdaptiveSiP();

                        CurrentTestResultsView.StartedFromTestResultView += StartTestBtn_Clicked;
                        CurrentTestResultsView.StoppedFromTestResultView += StopTestBtn_Clicked;
                        CurrentTestResultsView.PausedFromTestResultView += PauseTestBtn_Clicked;

                        TestResultPage NewTestResultPage = new TestResultPage(ref CurrentTestResultsView);
                        CurrentExternalTestResultWindow = new Window(NewTestResultPage);
                        CurrentExternalTestResultWindow.Title = "OSTF Tablet Suite - Test results window - Adaptive SiP-test";
                        CurrentExternalTestResultWindow.Height = 240;
                        CurrentExternalTestResultWindow.Width = 1200;
                        Application.Current.OpenWindow(CurrentExternalTestResultWindow);

                        // Setting CurrentTestPlayState 
                        CurrentGuiLayoutState = GuiLayoutStates.TestOptions_StartButton_TestResultsOffForm;
                    }
                    else
                    {

                        TestResultGrid.Children.Clear();
                        CurrentTestResultsView = new TestResultView_Adaptive();

                        CurrentTestResultsView.StartedFromTestResultView += StartTestBtn_Clicked;
                        CurrentTestResultsView.StoppedFromTestResultView += StopTestBtn_Clicked;
                        CurrentTestResultsView.PausedFromTestResultView += PauseTestBtn_Clicked;

                        TestResultGrid.Children.Add(CurrentTestResultsView);

                        // Setting CurrentTestPlayState 
                        CurrentGuiLayoutState = GuiLayoutStates.TestOptions_StartButton_TestResultsOnForm;

                    }


                    break;

                case "SiP-testet (Adaptivt) - Övning":

                    SpeechMaterialPicker.SelectedItem = "Swedish SiP-test";

                    // Speech test
                    CurrentSpeechTest = new AdaptiveSiP("Swedish SiP-test");
                    CurrentSpeechTest.IsPractiseTest = true;

                    // Adding the event handlar that listens for transducer changes (but unsubscribing first to avoid multiple subscriptions)
                    CurrentSpeechTest.TransducerChanged -= UpdateSoundPlayerSettings;
                    CurrentSpeechTest.TransducerChanged += UpdateSoundPlayerSettings;

                    TestOptionsGrid.Children.Clear();
                    var newOptionsPASipTestView = new OptionsViewAll(CurrentSpeechTest);
                    TestOptionsGrid.Children.Add(newOptionsPASipTestView);
                    CurrentTestOptionsView = newOptionsPASipTestView;


                    // Creating test result view. Using an external test-results window on PC and on-form on other platforms
                    if (OstfBase.CurrentPlatForm == OstfBase.Platforms.WinUI & OstfBase.UseExtraWindows == true)
                    {

                        HasExternalResultsView = true;
                        CurrentTestResultsView = new TestResultView_AdaptiveSiP();

                        CurrentTestResultsView.StartedFromTestResultView += StartTestBtn_Clicked;
                        CurrentTestResultsView.StoppedFromTestResultView += StopTestBtn_Clicked;
                        CurrentTestResultsView.PausedFromTestResultView += PauseTestBtn_Clicked;

                        TestResultPage NewTestResultPage = new TestResultPage(ref CurrentTestResultsView);
                        CurrentExternalTestResultWindow = new Window(NewTestResultPage);
                        CurrentExternalTestResultWindow.Title = "OSTF Tablet Suite - Test results window - Adaptive SiP-test (practise)";
                        CurrentExternalTestResultWindow.Height = 240;
                        CurrentExternalTestResultWindow.Width = 1200;
                        Application.Current.OpenWindow(CurrentExternalTestResultWindow);

                        // Setting CurrentTestPlayState 
                        CurrentGuiLayoutState = GuiLayoutStates.TestOptions_StartButton_TestResultsOffForm;

                    }
                    else
                    {

                        TestResultGrid.Children.Clear();
                        CurrentTestResultsView = new TestResultView_Adaptive();

                        CurrentTestResultsView.StartedFromTestResultView += StartTestBtn_Clicked;
                        CurrentTestResultsView.StoppedFromTestResultView += StopTestBtn_Clicked;
                        CurrentTestResultsView.PausedFromTestResultView += PauseTestBtn_Clicked;

                        TestResultGrid.Children.Add(CurrentTestResultsView);

                        // Setting CurrentTestPlayState 
                        CurrentGuiLayoutState = GuiLayoutStates.TestOptions_StartButton_TestResultsOnForm;

                    }

                    break;


                default:
                    TestOptionsGrid.Children.Clear();
                    success = false;
                    break;
            }
        }
    }


    void SpeechMaterial_Picker_SelectedIndexChanged(object sender, EventArgs e)
    {

        // Inactivates tackback
        InactivateTalkback();

        var picker = (Picker)sender;
        var selectedItem = picker.SelectedItem;

        SelectedSpeechMaterialName = (string)selectedItem;

        bool success = true;

        // Here we should have options depending on which test is selected

        switch (selectedSpeechTestName)
        {

            //case "Talaudiometri":

            //    // Assigning a new SpeechTest to the options
            //    CurrentSpeechTest = new SpeechAudiometryTest(SelectedSpeechMaterialName);

            //    // Adding the event handlar that listens for transducer changes (but unsubscribing first to avoid multiple subscriptions)
            //            CurrentSpeechTest.TransducerChanged -= UpdateSoundPlayerSettings;
            //CurrentSpeechTest.TransducerChanged += UpdateSoundPlayerSettings;

            //    // Testoptions
            //    TestOptionsGrid.Children.Clear();
            //    var newOptionsSpeechAudiometryTestView = new OptionsViewAll(CurrentSpeechTest);
            //    TestOptionsGrid.Children.Add(newOptionsSpeechAudiometryTestView);
            //    CurrentTestOptionsView = newOptionsSpeechAudiometryTestView;

            //    break;

            default:
                TestOptionsGrid.Children.Clear();
                success = false;
                break;
        }

        // This is not necessary since UpdateSoundPlayerSettings is called on an event chain that starts from selecting transcuder in the SpeechTest
        //if (UpdateSoundPlayerSettings() == false)
        //{
        //    success = false;
        //}


        if (success)
        {

            CurrentGuiLayoutState = GuiLayoutStates.TestOptions_StartButton_TestResultsOnForm;

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

    private bool CreateTestGui()
    {

        if (CurrentSpeechTest != null)
        {

            try
            {

                switch (selectedSpeechTestName)
                {

                    case "Talaudiometri":

                        // Pick appropriate response view
                        if (CurrentSpeechTest.IsFreeRecall)
                        {
                            CurrentResponseView = new ResponseView_FreeRecall();
                        }
                        else
                        {
                            CurrentResponseView = new ResponseView_Mafc();

                            // We have to choose between:
                            //CurrentResponseView = new ResponseView_Matrix();

                            //CurrentResponseView = new ResponseView_FreeRecallWithHistory(TestReponseGrid.Width, TestReponseGrid.Height, CurrentSpeechTest.HistoricTrialCount);
                            //Which also requires:
                            // CurrentResponseView.ResponseHistoryUpdated += ResponseHistoryUpdate;

                            //CurrentResponseView = new ResponseView_SiP_SF();

                        }

                        CurrentResponseView.ResponseGiven += HandleResponseView_ResponseGiven;
                        CurrentResponseView.CorrectionButtonClicked += ResponseViewCorrectionButtonClicked;
                        //TestResponseView.StartedByTestee += StartedByTestee;

                        // Add the response view to TestReponseGrid (or put it in a separate window, code not finished there)
                        if (true)
                        {
                            TestReponseGrid.Children.Add(CurrentResponseView);
                        }
                        else
                        {
                            ResponsePage responsePage = new ResponsePage(ref CurrentResponseView);
                            Window secondWindow = new Window(responsePage);
                            secondWindow.Title = "";
                            Application.Current.OpenWindow(secondWindow);
                        }

                        // Testing with a basic text only view
                        TestResultGrid.Children.Clear();
                        if (CurrentTestResultsView != null)
                        {
                            CurrentTestResultsView = new TestResultsView_Text();
                            TestResultGrid.Children.Add(CurrentTestResultsView);
                        }

                        break;

                    case "Svenska HINT":

                        // Response view
                        CurrentResponseView = new ResponseView_FreeRecall();

                        // Select separate response window here, code not finished
                        if (false)
                        {
                            ResponsePage responsePage = new ResponsePage(ref CurrentResponseView);
                            Window secondWindow = new Window(responsePage);
                            secondWindow.Title = "";
                            Application.Current.OpenWindow(secondWindow);

                        }
                        else
                        {
                            TestReponseGrid.Children.Add(CurrentResponseView);
                        }

                        //TestResponseView.StartedByTestee += StartedByTestee;
                        CurrentResponseView.ResponseGiven += HandleResponseView_ResponseGiven;
                        CurrentResponseView.CorrectionButtonClicked += ResponseViewCorrectionButtonClicked;

                        break;


                    case "Hagermans meningar (Matrix)":

                        // Response view
                        if (CurrentSpeechTest.IsFreeRecall)
                        {
                            CurrentResponseView = new ResponseView_FreeRecall();
                        }
                        else
                        {
                            CurrentResponseView = new ResponseView_Matrix();
                        }

                        CurrentResponseView.ResponseGiven += HandleResponseView_ResponseGiven;
                        //TestResponseView.StartedByTestee += StartedByTestee;
                        CurrentResponseView.CorrectionButtonClicked += ResponseViewCorrectionButtonClicked;

                        TestReponseGrid.Children.Add(CurrentResponseView);

                        break;


                    case "Hörtröskel för tal (HTT)":

                        // Response view
                        if (CurrentSpeechTest.IsFreeRecall)
                        {
                            CurrentResponseView = new ResponseView_FreeRecall();
                        }
                        else
                        {
                            CurrentResponseView = new ResponseView_Mafc();
                        }
                        CurrentResponseView.ResponseGiven += HandleResponseView_ResponseGiven;
                        //TestResponseView.StartedByTestee += StartedByTestee;
                        CurrentResponseView.CorrectionButtonClicked += ResponseViewCorrectionButtonClicked;

                        TestReponseGrid.Children.Add(CurrentResponseView);


                        break;

                    //case "SiP-testet":

                    //    CurrentResponseView = new ResponseView_Mafc();
                    //    CurrentResponseView.ResponseGiven += NewSpeechTestInput;

                    //    TestReponseGrid.Children.Add(CurrentResponseView);

                    //    break;


                    case "Manuell TP i brus":

                        // Response view
                        CurrentResponseView = new ResponseView_FreeRecallWithHistory(TestReponseGrid.Width, TestReponseGrid.Height, CurrentSpeechTest.HistoricTrialCount);

                        bool openInSeparateWindow = false;
                        if (openInSeparateWindow)
                        {
                            ResponsePage PB2responsePage = new ResponsePage(ref CurrentResponseView);
                            Window PB2secondWindow = new Window(PB2responsePage);
                            PB2secondWindow.Title = "";
                            Application.Current.OpenWindow(PB2secondWindow);
                        }
                        else
                        {
                            TestReponseGrid.Children.Add(CurrentResponseView);
                        }

                        CurrentResponseView.ResponseGiven += HandleResponseView_ResponseGiven;
                        CurrentResponseView.ResponseHistoryUpdated += ResponseHistoryUpdate;
                        CurrentResponseView.CorrectionButtonClicked += ResponseViewCorrectionButtonClicked;

                        //// Testing with a basic text only view
                        //TestResultGrid.Children.Clear();
                        //CurrentTestResultsView = new TestResultsView_Text();
                        //TestResultGrid.Children.Add(CurrentTestResultsView);

                        break;


                    case "Quick SiP":


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

                        //// Testing with a basic text only view
                        //TestResultGrid.Children.Clear();
                        //CurrentTestResultsView = new TestResultsView_Text();
                        //TestResultGrid.Children.Add(CurrentTestResultsView);

                        break;

                    //case "SiP-testet":

                    //    CurrentResponseView = new ResponseView_Mafc();
                    //    TestReponseGrid.Children.Add(CurrentResponseView);

                    //    CurrentResponseView.ResponseGiven += NewSpeechTestInput;

                    //    break;

                    case "TP50 - Ljudfält":

                        // Response view
                        CurrentResponseView = new ResponseView_FreeRecallWithHistory(TestReponseGrid.Width, TestReponseGrid.Height, CurrentSpeechTest.HistoricTrialCount);

                        bool openTP50InSeparateWindow = false;
                        if (openTP50InSeparateWindow)
                        {
                            ResponsePage PB2responsePage = new ResponsePage(ref CurrentResponseView);
                            Window PB2secondWindow = new Window(PB2responsePage);
                            PB2secondWindow.Title = "";
                            Application.Current.OpenWindow(PB2secondWindow);
                        }
                        else
                        {
                            TestReponseGrid.Children.Add(CurrentResponseView);
                        }

                        CurrentResponseView.ResponseGiven += HandleResponseView_ResponseGiven;
                        CurrentResponseView.ResponseHistoryUpdated += ResponseHistoryUpdate;
                        CurrentResponseView.CorrectionButtonClicked += ResponseViewCorrectionButtonClicked;

                        break;

                    case "SiP-testet (Adaptivt)":

                        CurrentResponseView = new ResponseView_AdaptiveSiP();
                        TestReponseGrid.Children.Add(CurrentResponseView);
                        CurrentResponseView.ResponseGiven += HandleResponseView_ResponseGiven;

                        break;

                    case "SiP-testet (Adaptivt) - Övning":

                        CurrentResponseView = new ResponseView_AdaptiveSiP();
                        TestReponseGrid.Children.Add(CurrentResponseView);
                        CurrentResponseView.ResponseGiven += HandleResponseView_ResponseGiven;

                        break;


                    default:
                        Messager.MsgBox("No participant response GUI available for the selected test! This is must likely a bug which should not occur!");
                        return false;
                }
                return true;
            }
            catch (Exception)
            {

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
        }
        else
        {
            return false;
        }

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

            // Inactivates tackback
            InactivateTalkback();

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

        // Directing the call to the main thread if not already on the main thread
        /// if (MainThread.IsMainThread == false) { MethodBase currentMethod = MethodBase.GetCurrentMethod(); MainThread.BeginInvokeOnMainThread(() => { currentMethod.Invoke(this, null); }); return; }

        bool TestGuiCreationResult = CreateTestGui();
        if (TestGuiCreationResult == false)
        {
            FinalizeTest(false);
            TestOptionsGrid.Children.Clear();
            CurrentGuiLayoutState = GuiLayoutStates.InitialState;
            return;
        }


        bool InitializationResult = await InitiateTesting();
        if (InitializationResult == false)
        {
            FinalizeTest(false);
            TestOptionsGrid.Children.Clear();
            CurrentGuiLayoutState = GuiLayoutStates.InitialState;
            return;
        }

        CurrentGuiLayoutState = GuiLayoutStates.TestIsRunning;

        // Calling NewSpeechTestInput with e as null
        // Making the call on a separate a background thread so that the GUI changes doesn't have to wait for the creation of the initial test stimuli 
        await Task.Run(() => HandleResponseView_ResponseGiven(null, null));

    }

    private void PauseTest()
    {

        // Directing the call to the main thread if not already on the main thread
        /// if (MainThread.IsMainThread == false) { MethodBase currentMethod = MethodBase.GetCurrentMethod(); MainThread.BeginInvokeOnMainThread(() => { currentMethod.Invoke(this, null); }); return; }

        if (CurrentSpeechTest == null) { return; }

        // Just ignores this call if the current test does not support pausing
        if (CurrentSpeechTest.SupportsManualPausing == false) { return; }

        CurrentGuiLayoutState = GuiLayoutStates.TestIsPaused;

        OstfBase.SoundPlayer.FadeOutPlayback();

        // Pause testing
        StopAllTrialEventTimers();

        // The test administrator must resume the test
        switch (STFN.SharedSpeechTestObjects.GuiLanguage)
        {
            case STFN.Utils.Constants.Languages.Swedish:
                StartTestBtn.Text = "Fortsätt";

                if (CurrentTestResultsView != null)
                {
                    CurrentTestResultsView.UpdateStartButtonText("Fortsätt");
                }

                break;
            default:
                StartTestBtn.Text = "Continue";

                if (CurrentTestResultsView != null)
                {
                    CurrentTestResultsView.UpdateStartButtonText("Continue");
                }

                break;
        }

        ShowResults();

        if (CurrentSpeechTest.PauseInformation == "")
        {
            switch (STFN.SharedSpeechTestObjects.GuiLanguage)
            {
                case STFN.Utils.Constants.Languages.Swedish:
                    CurrentSpeechTest.PauseInformation = "Testet är pausat";
                    break;
                default:
                    CurrentSpeechTest.PauseInformation = "The test has been paused";
                    break;
            }
        }

        // Registering timed trial event
        if (CurrentSpeechTest.CurrentTestTrial != null)
        {
            CurrentSpeechTest.CurrentTestTrial.TimedEventsList.Add(new Tuple<TestTrial.TimedTrialEvents, DateTime>(TestTrial.TimedTrialEvents.PauseMessageShown, DateTime.Now));
        }

        // Reduplicating the message also in the caption to get at larger font size... (lazy idea, TOSO: improve in future versions)
        //await Messager.MsgBoxAsync(CurrentSpeechTest.PauseInformation, Messager.MsgBoxStyle.Information, CurrentSpeechTest.PauseInformation, "OK");

        CurrentResponseView.ShowMessage(CurrentSpeechTest.PauseInformation);

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
        if (CurrentGuiLayoutState != GuiLayoutStates.TestIsRunning) { return; }


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
                PauseTest();
                CurrentSpeechTest.PauseInformation = "";

                break;

            case SpeechTest.SpeechTestReplies.TestIsCompleted:

                FinalizeTest(false);

                CurrentSpeechTest.SaveTestTrialResults();

                //if (CurrentSpeechTest != null)
                //{
                //    if (CurrentTestResultsView != null) {

                //        // Closing all extra windows (such as testresult windows) except this first one
                //        var windows = Application.Current.Windows.ToList();
                //        foreach (Window window in windows)
                //        {
                //            if (window != this.Window)
                //            {

                //            }
                //        }

                //        CurrentTestResultsView.TakeScreenShot(CurrentSpeechTest);

                //        Thread.Sleep(2000);

                //    }
                //}

                switch (STFN.SharedSpeechTestObjects.GuiLanguage)
                {
                    case STFN.Utils.Constants.Languages.Swedish:
                        Messager.MsgBox(CurrentSpeechTest.GetTestCompletedGuiMessage(), Messager.MsgBoxStyle.Information, "Klart!", "OK");
                        break;
                    default:
                        Messager.MsgBox(CurrentSpeechTest.GetTestCompletedGuiMessage(), Messager.MsgBoxStyle.Information, "Finished", "OK");
                        break;
                }

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
        if (currentLayoutConfiguration == LayoutConfiguration.Settings_Result |
            currentLayoutConfiguration == LayoutConfiguration.Settings_Result_Response | 
            HasExternalResultsView == true )
        {
        ShowResults();
        }

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

        //// Showing panels again
        //if (HasExternalResultsView)
        //{
        //    SetLayoutConfiguration(LayoutConfiguration.Settings);
        //}
        //else
        //{
        //    SetLayoutConfiguration(LayoutConfiguration.Settings_Result);
        //}

        // Restting start button text
        switch (STFN.SharedSpeechTestObjects.GuiLanguage)
        {
            case STFN.Utils.Constants.Languages.Swedish:
                StartTestBtn.Text = "Start";
                break;
            default:
                StartTestBtn.Text = "Start";
                break;
        }

        CurrentGuiLayoutState = GuiLayoutStates.TestIsStopped;

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

            // Saving test results to file
            // CurrentSpeechTest.SaveTableFormatedTestResults();

        }

    }




    async void AbortTest(bool closeApp)
    {

        // Directing the call to the main thread if not already on the main thread
        /// if (MainThread.IsMainThread == false) { MethodBase currentMethod = MethodBase.GetCurrentMethod(); MainThread.BeginInvokeOnMainThread(() => { currentMethod.Invoke(this, new object[] { closeApp }); }); return; }

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
            switch (CurrentSpeechTest.GuiResultType)
            {
                case SpeechTest.GuiResultTypes.StringResults:

                    CurrentTestResultsView.ShowTestResults(CurrentSpeechTest.GetResultStringForGui());

                    break;
                case SpeechTest.GuiResultTypes.VisualResults:

                    CurrentTestResultsView.ShowTestResults(CurrentSpeechTest);

                    break;
                default:
                    break;
            }
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

    #region Talkback_control

    private void TalkbackVolumeSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        TalkbackGain = (float)e.NewValue;
    }

    float talkbackGain = 0;

    public float TalkbackGain
    {
        get { return talkbackGain; }
        set
        {
            talkbackGain = (float)Math.Round((double)value);
            TalkbackGainlevel_Span.Text = talkbackGain.ToString();
            OstfBase.SoundPlayer.TalkbackGain = talkbackGain;
        }
    }

    bool TalkbackOn = false;

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (STFN.OstfBase.SoundPlayer.SupportsTalkBack == true)
        {
            if (TalkbackOn == true)
            {
                InactivateTalkback();
            }
            else
            {
                ActivateTalkback();
            }
        }
    }

    void InactivateTalkback()
    {

        // Directing the call to the main thread if not already on the main thread
        /// if (MainThread.IsMainThread == false) { MethodBase currentMethod = MethodBase.GetCurrentMethod(); MainThread.BeginInvokeOnMainThread(() => { currentMethod.Invoke(this, null); }); return; }

        TalkbackButton.BackgroundColor = Colors.Gray;
        TalkbackButton.BorderColor = Colors.LightGrey;

        // Inactivates tackback
        if (STFN.OstfBase.SoundPlayerIsInitialized())
        {
            if (STFN.OstfBase.SoundPlayer.SupportsTalkBack == true)
            {
                STFN.OstfBase.SoundPlayer.StopTalkback();
            }
        }

        TalkbackOn = false;
    }

    void ActivateTalkback()
    {

        // Directing the call to the main thread if not already on the main thread
        /// if (MainThread.IsMainThread == false) { MethodBase currentMethod = MethodBase.GetCurrentMethod(); MainThread.BeginInvokeOnMainThread(() => { currentMethod.Invoke(this, null); }); return; }

        if (STFN.OstfBase.SoundPlayerIsInitialized())
        {
            if (STFN.OstfBase.SoundPlayer.SupportsTalkBack == true)
            {
                if (STFN.OstfBase.SoundPlayer.IsPlaying == false)
                {
                    if (UpdateSoundPlayerSettings() == false)
                    {
                        Messager.MsgBox("Unable to start the player! Try selecting a test first.", Messager.MsgBoxStyle.Exclamation, "Sound player failure");
                        return;
                    }
                }

                // Activates tackback
                STFN.OstfBase.SoundPlayer.StartTalkback();
                TalkbackButton.BackgroundColor = Colors.GreenYellow;
                TalkbackButton.BorderColor = Colors.YellowGreen;

                TalkbackOn = true;
            }
        }
    }

    #endregion


    #region Audiograms_related_stuff_not_yet_implemented

    // This region should contain audiogram related stuff, not yet implemented

    void IDrawable.Draw(ICanvas canvas, RectF dirtyRect)
    {

        //MyAudiogramView.Audiogram.Draw(canvas, dirtyRect);

    }

    #endregion

}



