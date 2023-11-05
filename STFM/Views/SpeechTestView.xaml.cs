namespace STFM.Views;

public partial class SpeechTestView : ContentView, IDrawable
{

    RowDefinition originalBottomPanelHeight = null;
    ColumnDefinition originalLeftPanelWidth = null;
    View CurrentTestOptionsView = null;

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
        HideSettingsPanelSwitch.IsEnabled = false;
        HideResultsPanelSwitch.IsEnabled = false;
        StartTestBtn.IsEnabled = false;
        PauseTestBtn.IsEnabled = false;
        StopTestBtn.IsEnabled = false;
        TestReponseGrid.IsEnabled = false;
        TestResultGrid.IsEnabled = false;

    }


    void IDrawable.Draw(ICanvas canvas, RectF dirtyRect)
    {

        //MyAudiogramView.Audiogram.Draw(canvas, dirtyRect);

    }

    private void SetBottomPanelHide(bool hide)
    {

        // Storing the original height specified in the xaml code, so that it can be reused.
        if (originalBottomPanelHeight == null)
        {
            originalBottomPanelHeight = RightSideGrid.RowDefinitions[1];
        }

        if (hide)
        {
            TestResultGrid.IsVisible = false;
            RightSideGrid.RowDefinitions[1] = new RowDefinition(0);
        }
        else
        {
            TestResultGrid.IsVisible = true;
            RightSideGrid.RowDefinitions[1] = originalBottomPanelHeight;
        }

    }

    private void SetLeftPanelHide(bool hide)
    {

        // Storing the original width specified in the xaml code, so that it can be reused.
        if (originalLeftPanelWidth == null)
        {
            originalLeftPanelWidth = MainSpeechTestGrid.ColumnDefinitions[0];
        }

        if (hide)
        {
            TestSettingsGrid.IsVisible = false;
            MainSpeechTestGrid.ColumnDefinitions[0] = new ColumnDefinition(0);
        }
        else
        {
            TestSettingsGrid.IsVisible = true;
            MainSpeechTestGrid.ColumnDefinitions[0] = originalLeftPanelWidth;
        }

    }


    private void NewTestBtn_Clicked(object sender, EventArgs e)
    {
        // Resets the text on the start button, as this may have been changed if test was paused.
        StartTestBtn.Text = "Start";

        TestOptionsGrid.Children.Clear();

        // Set IsEnabled values of controls
        NewTestBtn.IsEnabled = false;
        SpeechTestPicker.IsEnabled = true;
        TestOptionsGrid.IsEnabled = false;
        if (CurrentTestOptionsView != null) { CurrentTestOptionsView.IsEnabled = false; }
        HideSettingsPanelSwitch.IsEnabled = false;
        HideResultsPanelSwitch.IsEnabled = false;
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

            switch (selectedItem)
            {
                case "SiP-testet":

                    TestOptionsGrid.Children.Clear();
                    var newOptionsSipTestView = new OptionsSipTestView();
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
                HideSettingsPanelSwitch.IsEnabled = true;
                HideResultsPanelSwitch.IsEnabled = true;
                StartTestBtn.IsEnabled = true;
                PauseTestBtn.IsEnabled = false;
                StopTestBtn.IsEnabled = false;
                TestReponseGrid.IsEnabled = false;
                TestResultGrid.IsEnabled = false;
            }
        }
    }

    private void StartTestBtn_Clicked(object sender, EventArgs e)
    {

        bool testIsReady = true; // This should call a fuction that check is the selected test is ready to be started
        bool testSupportPause = true; // This should call a function that determies if the test supports pausing

        if (testIsReady) {

            // Set IsEnabled values of controls
            NewTestBtn.IsEnabled = false;
            SpeechTestPicker.IsEnabled = false;
            TestOptionsGrid.IsEnabled = false;
            if (CurrentTestOptionsView != null) { CurrentTestOptionsView.IsEnabled = false; }
            HideSettingsPanelSwitch.IsEnabled = false;
            HideResultsPanelSwitch.IsEnabled = false;
            StartTestBtn.IsEnabled = false;
            if (testSupportPause) {PauseTestBtn.IsEnabled = true; }
            StopTestBtn.IsEnabled = true;
            TestReponseGrid.IsEnabled = true;
            TestResultGrid.IsEnabled = true;

            // Showing / hiding panels during test
            SetBottomPanelHide(HideResultsPanelSwitch.IsToggled);
            SetLeftPanelHide(HideSettingsPanelSwitch.IsToggled);

            // Starting the test
            // ...

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
        HideSettingsPanelSwitch.IsEnabled = false;
        HideResultsPanelSwitch.IsEnabled = false;
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
        SetBottomPanelHide(false);
        SetLeftPanelHide(false);

        StartTestBtn.Text = "Start";

        // Set IsEnabled values of controls
        NewTestBtn.IsEnabled = true;
        SpeechTestPicker.IsEnabled = false;
        TestOptionsGrid.IsEnabled = false;
        if (CurrentTestOptionsView != null){ CurrentTestOptionsView.IsEnabled = false; } 
        HideSettingsPanelSwitch.IsEnabled = false;
        HideResultsPanelSwitch.IsEnabled = false;
        StartTestBtn.IsEnabled = false;
        PauseTestBtn.IsEnabled = false;
        StopTestBtn.IsEnabled = false;
        TestReponseGrid.IsEnabled = false;
        TestResultGrid.IsEnabled = true;

        // Stopping the test
        // ...

    }

}