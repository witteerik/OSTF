using CommunityToolkit.Maui.Media;
using Microsoft.Maui;
using Microsoft.Maui.Devices;

namespace STFM.SpecializedViews.SSQ12;

public static class Ssq12Styling
{

    private static double ScalingFactor = 2;

    private static double AndroidScalingFactor = 2;

    public static Color TextFrameBackcolor
    {
        get
        {
            return Color.FromArgb("#F1F3F2");
        }
    }

    public static Color TextColor
    {
        get
        {
            return Color.FromArgb("#363636");
        }
    }

    public static Color ButtonColor
    {
        get
        {
            return Color.FromArgb("#3A6191");
        }
    }
    

    public static double SuperLargeFontSize
    {
        get
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
                return Math.Round(36 * AndroidScalingFactor);
            else
                return Math.Round(36 * ScalingFactor);
        }
    }

    public static double LargeFontSize
    {
        get
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
                return Math.Round(18 * AndroidScalingFactor);
            else
                return 18 * ScalingFactor;
        }
    }

    public static double MediumFontSize
    {
        get
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
                return Math.Round(12 * AndroidScalingFactor);
            else
                return 12* ScalingFactor;
        }
    }

    public static double SmallFontSize
    {
        get
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
                return Math.Round(11 * AndroidScalingFactor);
            else
                return 11* ScalingFactor;
        }
    }

    public static double TinyFontSize
    {
        get
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
                return Math.Round(6 * AndroidScalingFactor);
            else
                return 6* ScalingFactor;
        }
    }

}

public partial class SSQ12_MainView : ContentView
{

    private string FilePathRepresentation = "SSQ12";

    public event EventHandler<EventArgs> EnterFullScreenMode;
    public event EventHandler<EventArgs> ExitFullScreenMode;

    /// <summary>
    /// The event fires upon the completion of all mandatory questions.
    /// </summary>
    public event EventHandler<EventArgs> Finished;

    private List<SsqQuestion> CurrentSsqQuestions = new List<SsqQuestion>();

    public Button SubmitButton;

    /// <summary>
    /// Static variable determining if SSQ12 uses the minimal version or not. In the minimal version, the user is not asked to respond to free-text questions, and also, the collapsable response-alternative list is hidden.
    /// </summary>
    public static bool MinimalVersion;

    public SSQ12_MainView()
    {
        InitializeComponent();

        // Instantiating the questions views

        for (int i = 1; i < 13; i++)
        {
            SSQ12_QuestionView sSQ12_QuestionView = new SSQ12_QuestionView(i);
            sSQ12_QuestionView.ShowQuestion();
            MainStackLayout.Add(sSQ12_QuestionView);

            // Also referencing the SsqQuestion in CurrentSsqQuestions 
            CurrentSsqQuestions.Add(sSQ12_QuestionView.SsqQuestion);

        }

        SubmitButton = new Button() { BackgroundColor = Ssq12Styling.ButtonColor, FontSize = Ssq12Styling.LargeFontSize, FontAttributes = FontAttributes.Bold };
        SubmitButton.HeightRequest = 200;
        SubmitButton.HorizontalOptions = LayoutOptions.Fill;
        SubmitButton.Clicked += SubmitButton_Clicked;
        MainStackLayout.Add(SubmitButton);

        // Creating and adding a reference box
        Frame ReferenceFrame = new Frame { BackgroundColor = Ssq12Styling.TextFrameBackcolor };
        ReferenceFrame.Margin = new Thickness(100, 50, 100, 50);
        ReferenceFrame.HorizontalOptions = LayoutOptions.Fill;
        MainStackLayout.Add(ReferenceFrame);
        Label ReferenceLabel1 = new Label() { FontSize = Ssq12Styling.MediumFontSize };

        ReferenceLabel1.HorizontalOptions = LayoutOptions.Fill;
        ReferenceLabel1.VerticalOptions = LayoutOptions.Fill;
        Label ReferenceLabel2 = new Label() { FontSize = Ssq12Styling.TinyFontSize };
        ReferenceLabel2.HorizontalOptions = LayoutOptions.Fill;
        ReferenceLabel2.VerticalOptions= LayoutOptions.Fill;
        StackLayout ReferenceStackLayout = new StackLayout();
        ReferenceFrame.Content = ReferenceStackLayout;
        ReferenceStackLayout.Add(ReferenceLabel1);
        ReferenceStackLayout.Add(ReferenceLabel2);


        switch (STFN.SharedSpeechTestObjects.GuiLanguage)
        {

            case STFN.Utils.Constants.Languages.Swedish:
                InstructionsHeadings.Text = "Instruktion";
                InstructionsSubHeadings.Text = "SSQ12-INSTRUKTIONER";
                InstructionsBodyTextP1.Text = "De följande frågorna gäller din förmåga och dina upplevelser i samband med att höra och lyssna i olika situationer. Om du använder hörapparat(er), besvara frågan så som du hör med hörapparat(er).";
                InstructionsBodyTextP2.Text = "Läs igenom frågeformuläret så du känner dig orienterad om vad det handlar om. Välj det svar som du tycker motsvarar dina upplevelser. 10 betyder att du klarar eller upplever det som frågan gäller helt perfekt. 0 betyder att du inte alls klarar eller upplever att du klarar det som frågan gäller.";
                InstructionsBodyTextP3.Text = "Vi tror att du kan känna igen alla situationerna från din vardag, men om en fråga beskriver en situation som du inte alls kan relatera till ombeds du markera 'Vet inte'-rutan och skriva en kort anmärkning om varför du inte kan svara på frågan.";
                InstructionsToggleLabel.Text = "Dölj instruktionen för formuläret";

                MandatoryInfoLabel.Text = "🞲  = Obligatoriskt att fylla i";

                SubmitButton.Text = "SLUTFÖR";

                ReferenceLabel1.Text = "ⓘ";
                ReferenceLabel2.Text = "Noble, W., Jensen, N. S., Naylor, G., Bhullar, N., & Akeroyd, M. A. (2013). A short form of the Speech, Spatial and Qualities of Hearing scale suitable for clinical use: the SSQ12. International Journal of Audiology, 52(6), 409-412. doi:10.3109/14992027.2013.781278\r\nÖversatt till svenska av Docent Öberg, Linköpings universitetssjukhus och hennes kollegor.";

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

                ReferenceLabel1.Text = "ⓘ";
                ReferenceLabel2.Text = "Noble, W., Jensen, N. S., Naylor, G., Bhullar, N., & Akeroyd, M. A. (2013). A short form of the Speech, Spatial and Qualities of Hearing scale suitable for clinical use: the SSQ12. International Journal of Audiology, 52(6), 409-412. doi:10.3109/14992027.2013.781278";

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
        switch (STFN.SharedSpeechTestObjects.GuiLanguage)
        {
            case STFN.Utils.Constants.Languages.Swedish:
                InstructionsToggleLabel.Text = isVisible ? "Visa instruktionen för formuläret" : "Dölj instruktionen för formuläret";
                break;

            default:
                InstructionsToggleLabel.Text = isVisible ? "Visa instruktionen för formuläret" : "Dölj instruktionen för formuläret";
                break;
        }

    }

    private async void SubmitButton_Clicked(object sender, EventArgs e)
    {

        bool HasHaResponse = await sSQ12_HaView.HasResponse();

        if (HasHaResponse == false)
        {
            await MainScrollView.ScrollToAsync(sSQ12_HaView, ScrollToPosition.Start, true);
            return;
        }

        foreach (var child in MainStackLayout.Children)
        {

            if (child is SSQ12_QuestionView)
            {

                SSQ12_QuestionView castChild = (SSQ12_QuestionView)child;
                if (castChild.HasResponse() == false)
                {
                    await MainScrollView.ScrollToAsync(castChild, ScrollToPosition.Start, true);
                    switch (STFN.SharedSpeechTestObjects.GuiLanguage)
                    {
                        case STFN.Utils.Constants.Languages.Swedish:
                            await STFN.Messager.MsgBoxAsync("Vänligen besvara denna fråga innan du går vidare!", STFN.Messager.MsgBoxStyle.Information, "Obesvarad fråga!");
                            break;

                        default:
                            await STFN.Messager.MsgBoxAsync("Please answer this question before you move on!", STFN.Messager.MsgBoxStyle.Information, "Unanswered question!");
                            break;
                    }

                    return;
                }

            }

        }

        // if code gets here, all mandatory questions have been answered.

        // Fires the test competed event.
        EventHandler<EventArgs> handler = Finished;
        EventArgs e2 = new EventArgs();
        if (handler != null)
        {
            handler(this, e2);
        }


    }

    public string GetResults(bool IncludeDetails = false)
    {

        List<string> ResultsList = new List<string>();

        if (IncludeDetails)
        {
            switch (STFN.SharedSpeechTestObjects.GuiLanguage)
            {
                case STFN.Utils.Constants.Languages.Swedish:
                    ResultsList.Add("SSQ12 RESULTAT");
                    break;

                default:
                    ResultsList.Add("SSQ12 RESULTS");
                    break;
            }
        }

        ResultsList.Add(sSQ12_HaView.GetResultString());

        List<double> RatingList = new List<double>();
        foreach (SsqQuestion item in CurrentSsqQuestions)
        {
            if (item.ResponseIndex > -1 & item.ResponseIndex < 11)
            {
                RatingList.Add(item.ResponseIndex);
            }
        }

        //ResultsList.Add("\n");

        if (RatingList.Count > 0)
        {
            // Filling up missing (unanswered) with the average rating
            double MeanRating = RatingList.Average();
            int ValidAnswers = RatingList.Count;
            do
            {
                RatingList.Add(MeanRating);
            } while (RatingList.Count < 12);

            // calculating mean rating
            double FinalMeanRating = RatingList.Average();

            switch (STFN.SharedSpeechTestObjects.GuiLanguage)
            {
                case STFN.Utils.Constants.Languages.Swedish:
                    ResultsList.Add("Resultat = " + Math.Round(FinalMeanRating, 1).ToString());
                    break;

                default:
                    ResultsList.Add("Score = " + Math.Round(FinalMeanRating, 1).ToString());
                    break;
            }

            if (ValidAnswers != 12)
            {
                switch (STFN.SharedSpeechTestObjects.GuiLanguage)
                {
                    case STFN.Utils.Constants.Languages.Swedish:
                        ResultsList.Add("    (Baserat på " + ValidAnswers.ToString() + ") svar.");
                        break;

                    default:
                        ResultsList.Add("    (Based on " + ValidAnswers.ToString() + ") questions.");
                        break;
                }
            }

        }
        else
        {

            switch (STFN.SharedSpeechTestObjects.GuiLanguage)
            {
                case STFN.Utils.Constants.Languages.Swedish:
                    ResultsList.Add("SSQ: För få (" + RatingList.Count.ToString() + ") besvarade frågor.");
                    break;

                default:
                    ResultsList.Add("SSQ: Too few questions (" + RatingList.Count.ToString() + ") answered.");
                    break;
            }

        }

        
        if (IncludeDetails)
        {
            ResultsList.Add("\n");
            foreach (SsqQuestion item in CurrentSsqQuestions)
            {
                ResultsList.Add(item.GetResponseString() + "\n");
            }
        }

        string Output = string.Join("\n", ResultsList);

        return Output;

    }

    public void SaveResults()
    {
        string resultsString = GetResults(true);
        string OutputPath = System.IO.Path.Combine(STFN.SharedSpeechTestObjects.TestResultsRootFolder, FilePathRepresentation);
        STFN.Utils.Logging.SendInfoToLog(resultsString, "SSQ", OutputPath, false, false, false, false, true);
    }


}
