
public class CorrectionButton : Grid
{

    private Label indicatorLabel;
    private Button repsonseButton;
    private Frame repsonseButtonFrame;
    private bool showAsCorrect = false;

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

    private Color incorrectColor = Colors.Red;
    public Color IncorrectColor
    {
        get { return incorrectColor; }
        set { incorrectColor = value; }
    }

    public CorrectionButton(string text, double textSize)
    {

        this.HorizontalOptions = LayoutOptions.Fill;
        this.VerticalOptions = LayoutOptions.Fill;
        this.BackgroundColor = Color.FromRgb(40, 40, 40);
        this.AddColumnDefinition(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        this.AddRowDefinition(new RowDefinition { Height = new GridLength(0.8, GridUnitType.Star) });
        this.AddRowDefinition(new RowDefinition { Height = new GridLength(1.2, GridUnitType.Star) });

        indicatorLabel = new Label()
        {
            Text = "✗",
            TextColor = Color.FromRgb(40, 40, 40),
            FontSize = textSize * 1.5,
            VerticalTextAlignment = TextAlignment.Center,
            HorizontalTextAlignment = TextAlignment.Center,
        };

        repsonseButton = new Button()
        {
            Text = text,
            BackgroundColor = Color.FromRgb(255, 255, 128),
            Padding = new Thickness(2, 10),
            TextColor = Color.FromRgb(40, 40, 40),
            FontSize = textSize,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        };

        repsonseButtonFrame = new Frame();
        repsonseButtonFrame.BorderColor = Colors.Gray;
        repsonseButtonFrame.CornerRadius = 8;
        repsonseButtonFrame.Padding = 10;
        repsonseButtonFrame.Margin = 4;
        repsonseButtonFrame.Content = repsonseButton;

        this.Add(indicatorLabel, 0, 0);
        this.Add(repsonseButtonFrame, 0, 1);

        repsonseButton.Clicked += reponseButton_Clicked;

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

        // Getting the responsed label

        if (showAsCorrect == true)
        {
            indicatorLabel.Text = "✓";
            indicatorLabel.TextColor = correctColor;

            // Modifies the frame color to mark that it's set as correct
            repsonseButtonFrame.BorderColor = correctColor;
            repsonseButtonFrame.BackgroundColor = correctColor;
        }
        else
        {
            indicatorLabel.Text = "✗";
            indicatorLabel.TextColor = incorrectColor;

            // Modifies the frame color to mark that it's set as incorrect
            repsonseButtonFrame.BorderColor = incorrectColor;
            repsonseButtonFrame.BackgroundColor = incorrectColor;
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

