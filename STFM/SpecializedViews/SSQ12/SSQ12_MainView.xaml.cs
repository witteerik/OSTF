using CommunityToolkit.Maui.Media;


namespace STFM.SpecializedViews.SSQ12;

public partial class SSQ12_MainView : ContentView
{

    public event EventHandler<EventArgs> EnterFullScreenMode;
    public event EventHandler<EventArgs> ExitFullScreenMode;

    public SSQ12_IntroView sSQ12_IntroView;
    public SSQ12_QuestionView sSQ12_QuestionView;


    public int CurrentQuestion = -1;

    public SSQ12_MainView()
	{
		InitializeComponent();

        switch (STFN.SharedSpeechTestObjects.GuiLanguage)
        {

            case STFN.Utils.Constants.Languages.Swedish:
                PreviousButton.Text = "FÖREGÅENDE";
                NextButton.Text = "NÄSTA";

                break;
            default:
                // Using English as default
                PreviousButton.Text = "PREVIOUS";
                NextButton.Text = "NEXT";
                break;
        }

        // Adding the intro view
        sSQ12_IntroView = new SSQ12_IntroView();
        ContentFrame.Content = sSQ12_IntroView;
        NextButton.IsEnabled = true;

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

        if (sSQ12_QuestionView == null)
        {

            if (sSQ12_IntroView.HasResponse() == true)
            {
                sSQ12_QuestionView = new SSQ12_QuestionView();
                CurrentQuestion = 0;
                ContentFrame.Content = sSQ12_QuestionView;
            }else{
                STFN.Messager.MsgBox("Vänligen besvara frågan innan du går vidare!", STFN.Messager.MsgBoxStyle.Information, "Frågan är inte besvarad!");
                return;
            }

        }
        else
        {

            if (sSQ12_QuestionView.HasResponse() == true)
            {
                if (CurrentQuestion == sSQ12_QuestionView.SsqQuestions.Count - 1)
                {
                    // The test should be finished here. Sum up

                }
                else
                {
                    CurrentQuestion += 1;
                }
            }
            else
            {
                STFN.Messager.MsgBox("Vänligen besvara frågan innan du går vidare!", STFN.Messager.MsgBoxStyle.Information, "Frågan är inte besvarad!");
                return;
            }

        }

        SetButtonAppearance();

        sSQ12_QuestionView.ShowQuestion(CurrentQuestion);

    }

    private void PreviousButton_Clicked(object sender, EventArgs e)
    {

        if (CurrentQuestion > 0)
        {

            CurrentQuestion -= 1;

            SetButtonAppearance();

            sSQ12_QuestionView.ShowQuestion(CurrentQuestion);

        }

    }

    private void SetButtonAppearance() {


        switch (STFN.SharedSpeechTestObjects.GuiLanguage)
        {
            case STFN.Utils.Constants.Languages.Swedish:
                NextButton.Text = "NÄSTA";
                break;
            default:
                // Using English as default
                NextButton.Text = "NEXT";
                break;
        }

        // Enabling and disabling buttons
        PreviousButton.IsEnabled = false;
        NextButton.IsEnabled = false;
        if (CurrentQuestion < 1)
        {
            NextButton.IsEnabled = true;

        }
        else if (CurrentQuestion < sSQ12_QuestionView.SsqQuestions.Count -1)
        {
            PreviousButton.IsEnabled = true;
            NextButton.IsEnabled = true;
        }
        else
        {
            PreviousButton.IsEnabled = true;
            NextButton.IsEnabled = true;
            // The test should finish on the next press on NextButton

            switch (STFN.SharedSpeechTestObjects.GuiLanguage)
            {
                case STFN.Utils.Constants.Languages.Swedish:
                    NextButton.Text = "SLUTFÖR";
                    break;
                default:
                    // Using English as default
                    NextButton.Text = "SUBMIT";
                    break;
            }
        }
    }


}
