using CommunityToolkit.Maui.Media;
using Microsoft.Maui;


namespace STFM.SpecializedViews.SSQ12;

public partial class SSQ12_MainView : ContentView
{

    public event EventHandler<EventArgs> EnterFullScreenMode;
    public event EventHandler<EventArgs> ExitFullScreenMode;

    public SSQ12_IntroView sSQ12_IntroView;
    public Button SubmitButton;

    public SSQ12_MainView()
    {
        InitializeComponent();

        // Instantiating the views
        sSQ12_IntroView = new SSQ12_IntroView();

        MainStackLayout.Add(sSQ12_IntroView);

        for (int i = 0; i < 12; i++)
        {
            SSQ12_QuestionView sSQ12_QuestionView = new SSQ12_QuestionView();
            sSQ12_QuestionView.ShowQuestion(i);
            MainStackLayout.Add(sSQ12_QuestionView);

        }

        SubmitButton = new Button();
        SubmitButton.Clicked += SubmitButton_Clicked;

        switch (STFN.SharedSpeechTestObjects.GuiLanguage)
        {

            case STFN.Utils.Constants.Languages.Swedish:
                InstructionsHeadings.Text = "Instruktion";
                InstructionsSubHeadings.Text = "SSQ12-INSTRUKTIONER";
                InstructionsBodyText.Text = "De följande frågorna gäller din förmåga...";
                MandatoryInfoLabel.Text = "Obligatoriskt att fylla i";

                SubmitButton.Text = "SLUTFÖR";

                break;
            default:
                // Using English as default

                InstructionsHeadings.Text = "";
                InstructionsSubHeadings.Text = "";
                InstructionsBodyText.Text = "";
                MandatoryInfoLabel.Text = "";

                SubmitButton.Text = "SUBMIT";
                break;
        }

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += OnToggleTapped;

        InstructionsToggleHeader.GestureRecognizers.Add(tapGesture);

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

    private void OnToggleTapped(object sender, EventArgs e)
    {
        bool isVisible = InstructionsCollapsableStackLayout.IsVisible;
        InstructionsCollapsableStackLayout.IsVisible = !isVisible;

        InstructionsToggleSymbol.Text = isVisible ? "+" : "-";
        InstructionsToggleLabel.Text = isVisible ? "Visa instruktionen för formuläret" : "Dölj instruktionen för formuläret";

    }

    private void SubmitButton_Clicked(object sender, EventArgs e)
    {

            if (sSQ12_IntroView.HasResponse() == true)
            {
            STFN.Messager.MsgBox("Vänligen besvara frågan innan du går vidare!", STFN.Messager.MsgBoxStyle.Information, "Frågan är inte besvarad!");
            return;
            }

        foreach (var child in MainStackLayout.Children)
        {

            if (child is SSQ12_QuestionView)
            {

                SSQ12_QuestionView castChild = (SSQ12_QuestionView)child;
                if (castChild.HasResponse() == false)
                {
                    STFN.Messager.MsgBox("Vänligen besvara frågan innan du går vidare!", STFN.Messager.MsgBoxStyle.Information, "Frågan är inte besvarad!");
                    return;
                }

            }

        }

    }

   

   
}
