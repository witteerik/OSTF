using STFN.Core;

public class SelectionTextButton : Frame
{

    private Button repsonseButton;
    private bool showAsSelected = false;
    public bool ShowAsSelected
    {
        get { return showAsSelected; }
        set { 
            showAsSelected = value;
            UpdateButtonColor();
        }
    }

    private Color selectionColor = Color.FromRgb(4, 255, 61);
    public Color SelectionColor
    {
        get { return selectionColor; }
        set { selectionColor = value; }
    }

    private Color unSelectedColor = Colors.Gray;
    public Color UnSelectedColor
    {
        get { return unSelectedColor; }
        set { unSelectedColor = value; }
    }

    public event EventHandler Clicked;


    public SelectionTextButton(SpeechTestResponseAlternative responseAlternative, double textSize)
        : this(responseAlternative, textSize, Color.FromRgb(4, 255, 61), Color.FromRgb(40, 40, 40)) { }

    public SelectionTextButton(SpeechTestResponseAlternative responseAlternative, double textSize, Color selectionColor, Color unSelectedColor)
    {

        this.selectionColor = selectionColor;
        this.unSelectedColor = unSelectedColor;

        this.HorizontalOptions = LayoutOptions.Fill;
        this.VerticalOptions = LayoutOptions.Fill;
        this.CornerRadius = 6;
        this.Padding = new Thickness(22,2);
        this.Margin = new Thickness(4,0);
        this.Shadow = new Shadow();


        repsonseButton = new Button()
        {
            Text = responseAlternative.Spelling,
            BackgroundColor = Color.FromRgb(255, 255, 128),
            Padding = new Thickness(0, 0),
            TextColor = Color.FromRgb(40, 40, 40),
            FontSize = textSize,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };


        if (responseAlternative.IsScoredItem == true)
        {
            repsonseButton.FontAttributes = FontAttributes.Bold;
        }
        else
        {
            repsonseButton.FontSize = textSize * 0.7;
        }


        this.Content = repsonseButton;

        repsonseButton.Clicked += reponseButton_Clicked;

        UpdateButtonColor();

    }

    /// <summary>
    /// Checks to see if the supplied button is a reference to the internal button which triggers the Clicked event.
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    public bool CheckButtonReferenceEquality(Button button)
    {
        return ReferenceEquals(button, repsonseButton);
    }

    public string GetValue()
    {
        if (showAsSelected == true)
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
        showAsSelected = !showAsSelected;

        UpdateButtonColor();

        //Raising the clicked event of the SelectionTextButton (which can be listened to by an external class
        Clicked?.Invoke(sender, e);

    }

    private void UpdateButtonColor()
    {
        if (showAsSelected == true)
        {
            // Modifies the frame color to mark that it's selected
            this.BorderColor = selectionColor;
            this.BackgroundColor = selectionColor;
        }
        else
        {
            // Modifies the frame color to mark that it's not selected
            this.BorderColor = unSelectedColor;
            this.BackgroundColor = unSelectedColor;
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