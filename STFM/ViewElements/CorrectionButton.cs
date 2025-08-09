using STFN;

public class CorrectionButton : Grid
{

    private Label indicatorLabel;
    private Button repsonseButton;
    public Frame RepsonseButtonFrame;
    private bool showAsCorrect = false;

    public readonly bool IsScoredItem;

    public bool ShowAsCorrect
    {
        get { return showAsCorrect; }
        set { showAsCorrect = value; }
    }

    private Color correctColor = Color.FromRgb(4, 255, 61);
    public Color CorrectColor
    {
        get { return correctColor; }
        set { correctColor = value; }
    }

    private Color correctColorLight = Color.FromRgb(129, 255, 159);


    private Color incorrectColor = Color.FromRgb(255, 80, 100);
    public Color IncorrectColor
    {
        get { return incorrectColor; }
        set { incorrectColor = value; }
    }

    private Color incorrectColorLight = Color.FromRgb(254, 152, 164);

    private Color notScoredColor = Colors.LightGray;

    public Color NotScoredColor
    {
        get { return notScoredColor; }
        set { notScoredColor = value; }
    }

    public event EventHandler Clicked;

    public CorrectionButton(SpeechTestResponseAlternative responseAlternative, double textSize)
    {

        IsScoredItem = responseAlternative.IsScoredItem;

        this.HorizontalOptions = LayoutOptions.Fill;
        this.VerticalOptions = LayoutOptions.Fill;
        this.BackgroundColor = Color.FromRgb(40, 40, 40);
        this.AddColumnDefinition(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        this.AddRowDefinition(new RowDefinition { Height = new GridLength(0.8, GridUnitType.Star) });
        this.AddRowDefinition(new RowDefinition { Height = new GridLength(1.2, GridUnitType.Star) });

        indicatorLabel = new Label()
        {
            FontSize = textSize * 1.5,
            VerticalTextAlignment = TextAlignment.Center,
            HorizontalTextAlignment = TextAlignment.Center,
        };

        repsonseButton = new Button()
        {
            Text = responseAlternative.Spelling,
            Padding = new Thickness(2, 10),
            TextColor = Color.FromRgb(40, 40, 40),
            FontSize = textSize,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        };

        setButtonAppearence();

        RepsonseButtonFrame = new Frame();
        RepsonseButtonFrame.BorderColor = Colors.LightGray;
        RepsonseButtonFrame.BackgroundColor = Colors.LightGray;
        //RepsonseButtonFrame.BorderColor = inCorrectColorLight;
        //RepsonseButtonFrame.BackgroundColor = inCorrectColorLight;
        RepsonseButtonFrame.CornerRadius = 0;
        RepsonseButtonFrame.Padding = new Thickness(4, 8);
        RepsonseButtonFrame.Margin = 0;
        RepsonseButtonFrame.Content = repsonseButton;

        this.Add(indicatorLabel, 0, 0);
        this.Add(RepsonseButtonFrame, 0, 1);

        // Activating event Clicked handler only is item is scored
        if (IsScoredItem)
        {
            repsonseButton.Clicked += reponseButton_Clicked;
        }

    }

    public string GetValue()
    {
        if (showAsCorrect == true)
        {
            return repsonseButton.Text;
        }
        else
        {
            return "";
        }
    }

    private void reponseButton_Clicked(object sender, EventArgs e)
    {

        //Swapping the value
        showAsCorrect = !showAsCorrect;

        setButtonAppearence();

        //Raising the clicked event of the CorrectionButton (which can be listened to by an external class
        Clicked?.Invoke(sender, e);

    }

    public void setButtonAppearence()
    {

        if (IsScoredItem)
        {

            if (showAsCorrect == true)
            {
                indicatorLabel.Text = "✓";
                indicatorLabel.TextColor = correctColor;

                // Modifies the frame color to mark that it's set as correct
                repsonseButton.BackgroundColor = correctColor;
                repsonseButton.BorderColor = correctColorLight;

            }
            else
            {
                indicatorLabel.Text = "✗";
                indicatorLabel.TextColor = incorrectColor;

                // Modifies the frame color to mark that it's set as incorrect
                repsonseButton.BackgroundColor = incorrectColor;
                repsonseButton.BorderColor = incorrectColorLight;

            }

        }
        else {

                indicatorLabel.Text = "";
                indicatorLabel.TextColor = correctColor;
                repsonseButton.BackgroundColor = notScoredColor;
                repsonseButton.BorderColor = notScoredColor;
        }

    }

    public void RemoveHandler()
    {
        repsonseButton.Clicked -= reponseButton_Clicked;
    }

    public void TurnRed()
    {
        repsonseButton.BorderColor = Colors.Red;
        repsonseButton.BackgroundColor = Colors.Red;

        RemoveHandler();
    }

}

