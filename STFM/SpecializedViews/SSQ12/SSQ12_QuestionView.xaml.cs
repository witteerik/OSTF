
using System.Runtime.CompilerServices;

namespace STFM.SpecializedViews.SSQ12;

public partial class SSQ12_QuestionView : ContentView
{

    public SsqQuestion SsqQuestion;

    public SSQ12_QuestionView(int QuestionNumber)
	{
		InitializeComponent();

        this.SsqQuestion = new  SsqQuestion(QuestionNumber);

        switch (STFN.SharedSpeechTestObjects.GuiLanguage)
        {
            case STFN.Utils.Constants.Languages.Swedish:
                ShowResponseAlternativeToggleLabel.Text = "Visa svarsalternativ";
                break;

            default:
                ShowResponseAlternativeToggleLabel.Text = "Show response alternatives";
                break;
        }

    }

    public void ShowQuestion()
	{

        QuestionLabel.Text = SsqQuestion.Question;
        CommentLabel.Text = SsqQuestion.CommentLabel;

        ResponsePicker.Items.Clear();
        foreach (string ResponseString in SsqQuestion.AvailableResponses.Values)
        {
            ResponsePicker.Items.Add(ResponseString);
            CollapsibleShowResponse_StackLayout.Add(new Label { Text = " • " + ResponseString, FontSize = Ssq12Styling.SmallFontSize, TextColor = Color.FromArgb("#363636") });
        }

        // Showing the CommentLabel and the CommentEditorBorder (and implicitly the CommentEditor) if the user selected index 11
        CommentLabel.IsVisible = this.SsqQuestion.ResponseIndex == 11;
        CommentEditorBorder.IsVisible = this.SsqQuestion.ResponseIndex == 11;

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += OnToggleTapped;

        ShowResponseAlternativeToggleHeader.GestureRecognizers.Add(tapGesture);

    }


    private void OnToggleTapped(object sender, EventArgs e)
    {
        bool isVisible = CollapsibleShowResponse_StackLayout.IsVisible;
        CollapsibleShowResponse_StackLayout.IsVisible = !isVisible;

        ShowResponseAlternativeToggleSymbol.Text = isVisible ? "+" : "-";

        switch (STFN.SharedSpeechTestObjects.GuiLanguage)
        {
            case STFN.Utils.Constants.Languages.Swedish:
                ShowResponseAlternativeToggleLabel.Text = isVisible ? "Visa svarsalternativ" : "Dölj svarsalternativ";
                break;

            default:
                ShowResponseAlternativeToggleLabel.Text = isVisible ? "Show response alternatives" : "Hide response alternatives";
                break;
        }

    }


    private void ResponsePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
            SsqQuestion.ResponseIndex = ResponsePicker.SelectedIndex;

            // Showing the CommentEditor if the user selected index 11
            CommentEditorBorder.IsVisible = SsqQuestion.ResponseIndex == 11;
            CommentLabel.IsVisible = SsqQuestion.ResponseIndex == 11;
    }

    private void CommentEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
            this.SsqQuestion.Comment = e.NewTextValue;
    }

    public bool HasResponse()
    {

        if (ResponsePicker.SelectedIndex > -1)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    }


public class SsqQuestion
{

    public int QuestionNumber;
    public string Question = "";
    public string CommentLabel = "";

    public int ResponseIndex = -1;
    public string Comment = "";
    public SortedList<int, string> AvailableResponses = new SortedList<int, string>();

    public SsqQuestion(int QuestionNumber)
    {

        this.QuestionNumber = QuestionNumber;
        this.Question = SsqQuestions.GetQuestion(QuestionNumber, STFN.SharedSpeechTestObjects.GuiLanguage);
        this.CommentLabel = SsqQuestions.GetCommentLabelText(QuestionNumber, STFN.SharedSpeechTestObjects.GuiLanguage);
        this.AvailableResponses = SsqQuestions.GetAvailableResponses(QuestionNumber, STFN.SharedSpeechTestObjects.GuiLanguage);

    }

    public string GetResponse()
    {
        if (ResponseIndex > -1 & ResponseIndex < AvailableResponses.Keys.Count)
        {
            return AvailableResponses[ResponseIndex];
        }
        else
        {
            return "";
        }
    }

    public string GetResponseString()
    {

        List<string> outputList = new List<string>();

        switch (STFN.SharedSpeechTestObjects.GuiLanguage)
        {
            case STFN.Utils.Constants.Languages.Swedish:

                outputList.Add("FRÅGA: " + this.Question.ToString());
                outputList.Add("SVAR: " + this.GetResponse()    );

                if (this.ResponseIndex > -1 & this.ResponseIndex < 11)
                {
                    outputList.Add("POÄNG: " + this.ResponseIndex);
                }

                if (this.ResponseIndex == 11)
                {
                    outputList.Add("KOMMENTAR: " + this.Comment);
                }
                else
                {
                    outputList.Add("KOMMENTAR: ");
                }

                break;

            default:

                outputList.Add("QUESTION: " + this.Question.ToString());
                outputList.Add("ANSWER: " + this.GetResponse());

                if (this.ResponseIndex > -1 & this.ResponseIndex < 11)
                {
                    outputList.Add("POINTS: " + this.ResponseIndex);
                }

                if (this.ResponseIndex == 11)
                {
                    outputList.Add("COMMENTS: " + this.Comment);
                }
                else
                {
                    outputList.Add("COMMENTS: ");
                }

                break;
        }

        return string.Join("\n", outputList);

    }

}

static class SsqQuestions
{

    public static string GetQuestion(int QuestionNumber, STFN.Utils.Constants.Languages Language)
    {

        List<string>  questions = new List<string>();

        // Adding questions with language dependent strings
        switch (Language)
        {
            case STFN.Utils.Constants.Languages.Swedish:

                questions.Add("1. Du talar med en person och en TV är på i samma rum. Kan du följa med i vad den andra personen säger, utan att sänka TV:n? (0=Inte alls, 10=Helt och hållet)  🞲" );
                questions.Add("2. Du lyssnar på en person som talar med dig, samtidigt som du försöker att följa nyheterna på TV. Kan du följa med i vad båda personerna säger? (0=Inte alls, 10=Helt och hållet)   🞲" );
                questions.Add("3. Du samtalar med en person i ett rum där det finns flera andra personer som talar. Kan du följa med i vad den personen som du samtalar med säger? (0=Stor koncentration, 10=Inget behov av koncentration)  🞲" );
                questions.Add("4. Du är i en grupp med cirka fem personer på en välbesökt restaurang. Du kan se alla de andra i gruppen. Kan du uppfatta samtalet? (0=XXX, 10=XXX)  🞲" );
                questions.Add("5. Du är i en grupp där samtalet skiftar från en person till en annan. Kan du lätt följa med i samtalet utan att missa början av vad varje ny talare säger? (0=XXX, 10=XXX)  🞲" );
                questions.Add("6. Du är utomhus. En hund skäller högt. Kan du omedelbart avgöra var den befinner sig utan att se den? (0=XXX, 10=XXX)  🞲" );
                questions.Add("7. Kan du med hjälp av ljudet avgöra hur långt bort en buss eller en lastbil befinner sig? (0=XXX, 10=XXX)  🞲" );
                questions.Add("8. Kan du med hjälp av ljudet avgöra om en buss eller lastbil kommer mot dig eller färdas ifrån dig? (0=XXX, 10=XXX)  🞲" );
                questions.Add("9. När du hör mer än ett ljud i taget har du då intrycket av att det verkar som en enda sammanblandning av ljud? (0=XXX, 10=XXX)  🞲" );
                questions.Add("10. När du lyssnar på musik, kan du urskilja vilka instrument som spelas? (0=XXX, 10=XXX)  🞲" );
                questions.Add("11. Ljud som finns i din vardag som du lätt kan höra, låter dessa klart (inte otydligt)? (0=XXX, 10=XXX)  🞲" );
                questions.Add("12. Måste du koncentrera dig väldigt mycket när du lyssnar på någon eller någonting? (0=Stor koncentration, 10=Inget behov av koncentration)  🞲");

                break;
            default:
                // Using English as default

                questions.Add("1. Add question here ..." );
                questions.Add("2. Add question here ...");
                questions.Add("3. Add question here ...");
                questions.Add("4. Add question here ...");
                questions.Add("5. Add question here ...");
                questions.Add("6. Add question here ...");
                questions.Add("7. Add question here ...");
                questions.Add("8. Add question here ...");
                questions.Add("9. Add question here ...");
                questions.Add("10. Add question here ...");
                questions.Add("11. Add question here ...");
                questions.Add("12. Add question here ...");

                break;
        }

        return questions[QuestionNumber - 1];

    }

    public static SortedList<int, string> GetAvailableResponses(int QuestionNumber, STFN.Utils.Constants.Languages Language)
    {

        SortedList<int, List<string>> QuestionResponses = new SortedList<int, List<string>>();
        
        // Adding possible responses with language dependent strings
        switch (Language)
        {
            case STFN.Utils.Constants.Languages.Swedish:

                QuestionResponses.Add(1,new List<string> { "0 = Inte alls", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10 = Helt och hållet", "Vet inte"});
                QuestionResponses.Add(2, new List<string> { "0 = Inte alls", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10 = Helt och hållet", "Vet inte" });
                QuestionResponses.Add(3, new List<string> { "0 = Stor koncentration", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10 = Inget behov av koncentration", "Vet inte" });
                QuestionResponses.Add(4, new List<string> { "0 = Inte alls", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10 = Helt och hållet", "Vet inte" });
                QuestionResponses.Add(5, new List<string> { "0 = Inte alls", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10 = Helt och hållet", "Vet inte" });
                QuestionResponses.Add(6, new List<string> { "0 = Inte alls", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10 = Helt och hållet", "Vet inte" });
                QuestionResponses.Add(7, new List<string> { "0 = Inte alls", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10 = Helt och hållet", "Vet inte" });
                QuestionResponses.Add(8, new List<string> { "0 = Inte alls", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10 = Helt och hållet", "Vet inte" });
                QuestionResponses.Add(9, new List<string> { "0 = Inte alls", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10 = Helt och hållet", "Vet inte" });
                QuestionResponses.Add(10, new List<string> { "0 = Inte alls", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10 = Helt och hållet", "Vet inte" });
                QuestionResponses.Add(11, new List<string> { "0 = Inte alls", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10 = Helt och hållet", "Vet inte" });
                QuestionResponses.Add(12, new List<string> { "0 = Stor koncentration", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10 = Inget behov av koncentration", "Vet inte" });

                        break;
            default:

                // Using English as default

                QuestionResponses.Add(1, new List<string> { "0 = Inte alls", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10 = Helt och hållet", "Vet inte" });
                QuestionResponses.Add(2, new List<string> { "0 = Inte alls", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10 = Helt och hållet", "Vet inte" });
                QuestionResponses.Add(3, new List<string> { "0 = Inte alls", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10 = Helt och hållet", "Vet inte" });
                QuestionResponses.Add(4, new List<string> { "0 = Inte alls", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10 = Helt och hållet", "Vet inte" });
                QuestionResponses.Add(5, new List<string> { "0 = Inte alls", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10 = Helt och hållet", "Vet inte" });
                QuestionResponses.Add(6, new List<string> { "0 = Inte alls", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10 = Helt och hållet", "Vet inte" });
                QuestionResponses.Add(7, new List<string> { "0 = Inte alls", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10 = Helt och hållet", "Vet inte" });
                QuestionResponses.Add(8, new List<string> { "0 = Inte alls", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10 = Helt och hållet", "Vet inte" });
                QuestionResponses.Add(9, new List<string> { "0 = Inte alls", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10 = Helt och hållet", "Vet inte" });
                QuestionResponses.Add(10, new List<string> { "0 = Inte alls", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10 = Helt och hållet", "Vet inte" });
                QuestionResponses.Add(11, new List<string> { "0 = Inte alls", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10 = Helt och hållet", "Vet inte" });
                QuestionResponses.Add(12, new List<string> { "0 = Inte alls", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10 = Helt och hållet", "Vet inte" });

                break;
        }

        SortedList<int, SortedList<int, string>> outputList = new SortedList<int, SortedList<int, string>>();
        for (int i = 1; i < QuestionResponses.Count+1; i++)
        {
            outputList.Add(i, new SortedList<int, string>());

            for (int j = 0; j < QuestionResponses[i].Count; j++)
            {
                outputList[i].Add(j, QuestionResponses[i][j]);
            }
        }
         
        return outputList[QuestionNumber];

    }


    public static string GetCommentLabelText (int QuestionNumber, STFN.Utils.Constants.Languages Language)
    {
        switch (Language)
        {
            case STFN.Utils.Constants.Languages.Swedish:
            return "Om du på fråga " + (QuestionNumber).ToString() + " valde Vet ej, vänligen ange varför du inte vet svaret.";
        default:
                // Using English as default
            return  "If you answered 'I don't know' on question " + (QuestionNumber).ToString() + ", please explain why you don't know the answer";
        }

}


}

