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
        SubmitButton.HeightRequest = 200;
        SubmitButton.HorizontalOptions = LayoutOptions.Fill;
        SubmitButton.Clicked += SubmitButton_Clicked;
        MainStackLayout.Add(SubmitButton);

        // Creating and adding a reference box
        Frame ReferenceFrame = new Frame { BackgroundColor = Colors.LightGray };
        ReferenceFrame.Margin = new Thickness(50, 50, 50, 50);
        ReferenceFrame.HorizontalOptions = LayoutOptions.Fill;
        MainStackLayout.Add(ReferenceFrame);
        Label ReferenceLabel = new Label ();
        ReferenceLabel.HorizontalOptions = LayoutOptions.Fill;
        ReferenceLabel.VerticalOptions= LayoutOptions.Fill;
        ReferenceFrame.Content = ReferenceLabel;

        switch (STFN.SharedSpeechTestObjects.GuiLanguage)
        {

            case STFN.Utils.Constants.Languages.Swedish:
                InstructionsHeadings.Text = "Instruktion";
                InstructionsSubHeadings.Text = "SSQ12-INSTRUKTIONER";
                InstructionsBodyTextP1.Text = "De följande frågorna gäller din förmåga och dina upplevelser i samband med att höra och lyssna i olika situationer. Om du använder hörapparat(er), besvara frågan så som du hör med hörapparat(er).";
                InstructionsBodyTextP2.Text = "Läs igenom frågeformuläret så du känner dig orienterad om vad det handlar om. Välj det svar som du tycker motsvarar dina upplevelser. 10 betyder att du klarar eller upplever det som frågan gäller helt perfekt. 0 betyder att du inte alls klarar eller upplever att du klarar det som frågan gäller.";
                InstructionsBodyTextP3.Text = "Vi tror att du kan känna igen alla situationerna från din vardag, men om en fråga beskriver en situation som du inte alls kan relatera till ombeds du markera ”Vet inte”-rutan och skriva en kort anmärkning om varför du inte kan svara på frågan.";
                InstructionsToggleLabel.Text = "Dölj instruktionen för formuläret";

                MandatoryInfoLabel.Text = "🞲  = Obligatoriskt att fylla i";

                SubmitButton.Text = "SLUTFÖR";

                ReferenceLabel.Text = "ⓘ\r\n\r\nNoble, W., Jensen, N. S., Naylor, G., Bhullar, N., & Akeroyd, M. A. (2013). A short form of the Speech, Spatial and Qualities of Hearing scale suitable for clinical use: the SSQ12. International Journal of Audiology, 52(6), 409-412. doi:10.3109/14992027.2013.781278\r\nÖversatt till svenska av Docent Öberg, Linköpings universitetssjukhus och hennes kollegor.";

                break;
            default:
                // Using English as default

                InstructionsHeadings.Text = "";
                InstructionsSubHeadings.Text = "";
                InstructionsBodyTextP1.Text = "";
                InstructionsBodyTextP2.Text = "";
                InstructionsBodyTextP3.Text = "";
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
