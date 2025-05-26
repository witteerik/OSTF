using System.Runtime.CompilerServices;

namespace STFM.SpecializedViews.SSQ12;

public partial class SSQ12_QuestionView : ContentView
{

	int CurrentQuestionIndex = -1;

    public List<SsqQuestion> SsqQuestions;


    public SSQ12_QuestionView()
	{
		InitializeComponent();

        AddSsqQuestions();

    }


    private void AddSsqQuestions()
    {

        SsqQuestions = new List<SsqQuestion>();

        // Adding questions with language dependent strings
        switch (STFN.SharedSpeechTestObjects.GuiLanguage)
        {
            case STFN.Utils.Constants.Languages.Swedish:

                SsqQuestions.Add(new SsqQuestion { Question = "1. Du talar med en person och en TV är på i samma rum. Kan du följa med i vad den andra personen säger, utan att sänka TV:n?" });
                SsqQuestions.Add(new SsqQuestion { Question = "2. Du lyssnar på en person som talar med dig, samtidigt som du försöker att följa nyheterna på TV. Kan du följa med i vad båda personerna säger?" });
                SsqQuestions.Add(new SsqQuestion { Question = "3. Du samtalar med en person i ett rum där det finns flera andra personer som talar. Kan du följa med i vad den personen som du samtalar med säger?" });
                SsqQuestions.Add(new SsqQuestion { Question = "4. Du är i en grupp med cirka fem personer på en välbesökt restaurang. Du kan se alla de andra i gruppen. Kan du uppfatta samtalet?" });
                SsqQuestions.Add(new SsqQuestion { Question = "5. Du är i en grupp där samtalet skiftar från en person till en annan. Kan du lätt följa med i samtalet utan att missa början av vad varje ny talare säger?" });
                SsqQuestions.Add(new SsqQuestion { Question = "6. Du är utomhus. En hund skäller högt. Kan du omedelbart avgöra var den befinner sig utan att se den?" });
                SsqQuestions.Add(new SsqQuestion { Question = "7. Kan du med hjälp av ljudet avgöra hur långt bort en buss eller en lastbil befinner sig?" });
                SsqQuestions.Add(new SsqQuestion { Question = "8. Kan du med hjälp av ljudet avgöra om en buss eller lastbil kommer mot dig eller färdas ifrån dig?" });
                SsqQuestions.Add(new SsqQuestion { Question = "9. När du hör mer än ett ljud i taget har du då intrycket av att det verkar som en enda sammanblandning av ljud?" });
                SsqQuestions.Add(new SsqQuestion { Question = "10. När du lyssnar på musik, kan du urskilja vilka instrument som spelas?" });
                SsqQuestions.Add(new SsqQuestion { Question = "11. Ljud som finns i din vardag som du lätt kan höra, låter dessa klart (inte otydligt)?" });
                SsqQuestions.Add(new SsqQuestion { Question = "12. Måste du koncentrera dig väldigt mycket när du lyssnar på någon eller någonting?" });

                break;
            default:
                // Using English as default

                SsqQuestions.Add(new SsqQuestion { Question = "1. Add question here ..." });
                SsqQuestions.Add(new SsqQuestion { Question = "2. Add question here ..." });
                SsqQuestions.Add(new SsqQuestion { Question = "3. Add question here ..." });
                SsqQuestions.Add(new SsqQuestion { Question = "4. Add question here ..." });
                SsqQuestions.Add(new SsqQuestion { Question = "5. Add question here ..." });
                SsqQuestions.Add(new SsqQuestion { Question = "6. Add question here ..." });
                SsqQuestions.Add(new SsqQuestion { Question = "7. Add question here ..." });
                SsqQuestions.Add(new SsqQuestion { Question = "8. Add question here ..." });
                SsqQuestions.Add(new SsqQuestion { Question = "9. Add question here ..." });
                SsqQuestions.Add(new SsqQuestion { Question = "10. Add question here ..." });
                SsqQuestions.Add(new SsqQuestion { Question = "11. Add question here ..." });
                SsqQuestions.Add(new SsqQuestion { Question = "12. Add question here ..." });

                break;
        }

    }

    bool IsSwappingQuestin = false;

    public void ShowQuestion(int currentQuestionIndex)
	{

        IsSwappingQuestin = true;

        // Updating the current question index and updatin gthe GUI data
        this.CurrentQuestionIndex = currentQuestionIndex;

        QuestionLabel.Text = SsqQuestions[CurrentQuestionIndex].Question;
        CommentEditor.Text = SsqQuestions[CurrentQuestionIndex].Comment;
        CommentLabel.Text = SsqQuestions[CurrentQuestionIndex].CommentLabel;

        ResponsePicker.Items.Clear();
        foreach (string ResponseString in SsqQuestions[CurrentQuestionIndex].SsqQuestionValue.Values)
        {
            ResponsePicker.Items.Add(ResponseString);
        }
        ResponsePicker.Title = SsqQuestions[CurrentQuestionIndex].ResponseTitle;

        ResponsePicker.SelectedIndex = SsqQuestions[CurrentQuestionIndex].ResponseIndex;

        // Showing the CommentEditor if the user selected index 11
        CommentEditor.IsVisible = (SsqQuestions[CurrentQuestionIndex].ResponseIndex == 11);
        CommentLabel.IsVisible = (SsqQuestions[CurrentQuestionIndex].ResponseIndex == 11);

        IsSwappingQuestin = false;

    }

    private void ResponsePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (IsSwappingQuestin == false)
        {
            SsqQuestions[CurrentQuestionIndex].ResponseIndex = ResponsePicker.SelectedIndex;

            // Showing the CommentEditor if the user selected index 11
            CommentEditor.IsVisible = (SsqQuestions[CurrentQuestionIndex].ResponseIndex == 11);
            CommentLabel.IsVisible = (SsqQuestions[CurrentQuestionIndex].ResponseIndex == 11);
        }

    }

    private void CommentEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (IsSwappingQuestin == false)
        {
            SsqQuestions[CurrentQuestionIndex].Comment = e.NewTextValue;
        }

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

    public string Question = "";
    public int ResponseIndex = -1;
    public string Comment = "";
    public string ResponseTitle = "";
    public string CommentLabel = "";

    public SortedList<int, string> SsqQuestionValue = new SortedList<int, string>();

    public SsqQuestion()
    {

        // Adding responses
        for (int i = 0; i < 11; i++)
        {
            SsqQuestionValue.Add(i, i.ToString());
        }

        // Creating language dependent strings representing the "Do not know" response
        switch (STFN.SharedSpeechTestObjects.GuiLanguage)
        {
            case STFN.Utils.Constants.Languages.Swedish:
                SsqQuestionValue.Add(11, "Vet inte");
                ResponseTitle = "- Välj -";
                CommentLabel = "Vänligen ange varför du inte vet svaret";
                break;
            default:
                // Using English as default
                SsqQuestionValue.Add(11, "I don't know"); // TDOD: Check how this is worded in English
                ResponseTitle = "- Select -";
                CommentLabel = "Please explain why you don't know the answer";
                break;
        }

    }

    public string GetResponse()
    {
        if (ResponseIndex > -1 & ResponseIndex < SsqQuestionValue.Keys.Count)
        {
            return SsqQuestionValue[ResponseIndex];
        }
        else
        {
            return "";
        }
    }


}
