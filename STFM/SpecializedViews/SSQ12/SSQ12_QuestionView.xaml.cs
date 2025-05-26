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

                SsqQuestions.Add(new SsqQuestion { Question = "1. Du talar med en person och en TV �r p� i samma rum. Kan du f�lja med i vad den andra personen s�ger, utan att s�nka TV:n?" });
                SsqQuestions.Add(new SsqQuestion { Question = "2. Du lyssnar p� en person som talar med dig, samtidigt som du f�rs�ker att f�lja nyheterna p� TV. Kan du f�lja med i vad b�da personerna s�ger?" });
                SsqQuestions.Add(new SsqQuestion { Question = "3. Du samtalar med en person i ett rum d�r det finns flera andra personer som talar. Kan du f�lja med i vad den personen som du samtalar med s�ger?" });
                SsqQuestions.Add(new SsqQuestion { Question = "4. Du �r i en grupp med cirka fem personer p� en v�lbes�kt restaurang. Du kan se alla de andra i gruppen. Kan du uppfatta samtalet?" });
                SsqQuestions.Add(new SsqQuestion { Question = "5. Du �r i en grupp d�r samtalet skiftar fr�n en person till en annan. Kan du l�tt f�lja med i samtalet utan att missa b�rjan av vad varje ny talare s�ger?" });
                SsqQuestions.Add(new SsqQuestion { Question = "6. Du �r utomhus. En hund sk�ller h�gt. Kan du omedelbart avg�ra var den befinner sig utan att se den?" });
                SsqQuestions.Add(new SsqQuestion { Question = "7. Kan du med hj�lp av ljudet avg�ra hur l�ngt bort en buss eller en lastbil befinner sig?" });
                SsqQuestions.Add(new SsqQuestion { Question = "8. Kan du med hj�lp av ljudet avg�ra om en buss eller lastbil kommer mot dig eller f�rdas ifr�n dig?" });
                SsqQuestions.Add(new SsqQuestion { Question = "9. N�r du h�r mer �n ett ljud i taget har du d� intrycket av att det verkar som en enda sammanblandning av ljud?" });
                SsqQuestions.Add(new SsqQuestion { Question = "10. N�r du lyssnar p� musik, kan du urskilja vilka instrument som spelas?" });
                SsqQuestions.Add(new SsqQuestion { Question = "11. Ljud som finns i din vardag som du l�tt kan h�ra, l�ter dessa klart (inte otydligt)?" });
                SsqQuestions.Add(new SsqQuestion { Question = "12. M�ste du koncentrera dig v�ldigt mycket n�r du lyssnar p� n�gon eller n�gonting?" });

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
                ResponseTitle = "- V�lj -";
                CommentLabel = "V�nligen ange varf�r du inte vet svaret";
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
