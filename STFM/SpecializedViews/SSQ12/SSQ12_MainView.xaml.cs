namespace STFM.SpecializedViews.SSQ12;

public partial class SSQ12_MainView : ContentView
{

    public event EventHandler<EventArgs> EnterFullScreenMode;
    public event EventHandler<EventArgs> ExitFullScreenMode;

    public SSQ12_MainView()
	{
		InitializeComponent();

        switch (STFN.SharedSpeechTestObjects.GuiLanguage)
        {
            case STFN.Utils.Constants.Languages.English:
                // Using English as default
                NextButton.Text = "NEXT";

                break;

            case STFN.Utils.Constants.Languages.Swedish:

                NextButton.Text = "NÄSTA";

                break;
            default:
                // Using English as default
                NextButton.Text = "NEXT";

                break;
        }

        // Adding the intro view
        SSQ12_IntroView sSQ12_IntroView = new SSQ12_IntroView();
        ContentFrame.Content = sSQ12_IntroView;


    }

    public void SetFullScreenMode(bool Fullscreen)
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

    private void NextButton_Clicked(object sender, EventArgs e)
    {

    }
}