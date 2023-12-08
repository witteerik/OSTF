using Microsoft.Maui.Controls;
//using Microsoft.UI.Xaml.Controls;
using STFN;
using STFN.Audio.SoundScene;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace STFM.Views;

public class ResponseView_FreeRecall : ResponseView
{

    Grid responseAlternativeGrid = null;
    private IDispatcherTimer HideAllTimer;


    public ResponseView_FreeRecall()
    {

        // Setting background color
        this.BackgroundColor = Color.FromRgb(40, 40, 40);
        //MainMafcGrid.BackgroundColor = Color.FromRgb(40, 40, 40);

        // Creating a hide-all timer
        HideAllTimer = Application.Current.Dispatcher.CreateTimer();
        HideAllTimer.Interval = TimeSpan.FromMilliseconds(300);
        HideAllTimer.Tick += HideAllItems;
        HideAllTimer.IsRepeating = false;
    }

    public override void InitializeNewTrial()
    {
        StopAllTimers();
    }

    public override void StopAllTimers()
    {
        HideAllTimer.Stop();
    }



    public override void ShowResponseAlternatives(List<Tuple<string, SoundSourceLocation>> ResponseAlternatives)
    {
        throw new NotImplementedException();
    }


    public override void ShowResponseAlternatives(List<List<string>> ResponseAlternatives)
    {

        if (ResponseAlternatives.Count > 1)
        {
            throw new ArgumentException("ShowResponseAlternatives is not yet implemented for multidimensional sets of response alternatives");
        }

        List<string> localResponseAlternatives = ResponseAlternatives[0];

        int nItems = localResponseAlternatives.Count;
        int nRows = 3;
        int nCols = nItems;

        // Creating a grid
        responseAlternativeGrid = new Grid { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };
        responseAlternativeGrid.BackgroundColor = Color.FromRgb(40, 40, 40);

        // Setting up rows and columns
        for (int i = 0; i < nRows; i++)
        {
            responseAlternativeGrid.AddRowDefinition(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        }

        for (int i = 0; i < nCols; i++)
        {
            responseAlternativeGrid.AddColumnDefinition(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }

        // Determining suitable text size (TODO: This is a bad method, since it doesn't care for the lengths of any strings.....
        var textSize = Math.Round(this.Width / 25);

        // Adding info on the top row
        Grid infoGrid = new Grid { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };
        infoGrid.BackgroundColor = Color.FromRgb(40, 40, 40);
        infoGrid.AddRowDefinition(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        infoGrid.AddColumnDefinition(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        infoGrid.AddColumnDefinition(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        infoGrid.AddColumnDefinition(new ColumnDefinition { Width = new GridLength(6, GridUnitType.Star) });

        var yesLabel = new Label()
        {
            Text = "✓ = rätt",
            //BackgroundColor = Color.FromRgb(255, 255, 128),
            HorizontalTextAlignment = TextAlignment.Start,
            VerticalTextAlignment = TextAlignment.Start,
            Padding = 5,
            TextColor = Color.FromRgb(4, 255, 61),
            FontSize = textSize / 2,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };

        var noLabel = new Label()
        {
            Text = "✗ = fel",
            //BackgroundColor = Color.FromRgb(255, 255, 128),
            HorizontalTextAlignment = TextAlignment.Start,
            VerticalTextAlignment = TextAlignment.Start,
            Padding = 5,
            TextColor = Colors.Red,
            FontSize = textSize / 2,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };

        infoGrid.Add(yesLabel, 0, 0);
        infoGrid.Add(noLabel, 1, 0);

        responseAlternativeGrid.Add(infoGrid, 0, 0);
        responseAlternativeGrid.SetColumnSpan(infoGrid, nCols);

        // Creating controls and positioning them in the responseAlternativeGrid
        for (int i = 0; i < localResponseAlternatives.Count; i++)
        {
            CorrectionButton correctionButton = new CorrectionButton(localResponseAlternatives[i], textSize);
            responseAlternativeGrid.Add(correctionButton, i, 1);
        }

        // Creating controls and positioning them in the responseAlternativeGrid
        int controlButtons = 1;
        for (int i = 0; i < controlButtons; i++)
        {

            var controlButton = new Button()
            {
                Text = "Nästa",
                BackgroundColor = Colors.LightBlue,
                Padding = 10,
                TextColor = Color.FromRgb(40, 40, 40),
                FontSize = textSize,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
            };

            controlButton.Clicked += controlButton_Clicked;

            Frame controlButtonFrame = new Frame
            {
                BorderColor = Colors.Gray,
                CornerRadius = 8,
                ClassId = "NextButton",
                Padding = 10,
                Margin = 4,
                Content = controlButton
            };

            responseAlternativeGrid.Add(controlButtonFrame, 0, 2);
            responseAlternativeGrid.SetColumnSpan(controlButtonFrame, nCols);

        }

        Content = responseAlternativeGrid;

    }


    private void wrapUpTrial()
    {

        List<string> CorrectResponses = new List<string>();

        // Hides all other labels, fokuses the selected one
        foreach (var child in responseAlternativeGrid.Children)
        {
            if (child is CorrectionButton)
            {
                var correctionButton = (CorrectionButton)child;
                CorrectResponses.Add(correctionButton.GetValue());
            }

            if (child is Frame)
            {
                var currentFrame = (Frame)child;
                if (currentFrame.ClassId == "NextButton")
                {
                    if (currentFrame.Content is Button)
                    {
                        // Removing the event handler
                        var button = (Button)currentFrame.Content;
                        button.Clicked -= controlButton_Clicked;
                    }
                }
            }
        }

        clearMainGrid();

        // Sends the linguistic response
        ReportResult(CorrectResponses);

    }

    private void controlButton_Clicked(object sender, EventArgs e)
    {

        // Getting the responsed label
        var controlButton = sender as Button;
        var controlButtonParentFrame = controlButton.Parent as Frame;

        if (controlButtonParentFrame.ClassId == "NextButton")
        {
            wrapUpTrial();
        }


    }

    private void ReportResult(List<string> CorrectResponses)
    {

        // Storing the raw response
        SpeechTestInputEventArgs args = new SpeechTestInputEventArgs();
        args.LinguisticResponses = CorrectResponses;
        args.LinguisticResponseTime = DateTime.Now;

        // Raising the Response given event in the base class
        OnResponseGiven(args);

        //HideAllTimer.Start();

    }


    private void StartedByTestee_ButtonClicked()
    {
        OnStartedByTestee(new EventArgs());
    }

    public void clearMainGrid()
    {

        Content = null;

        //MainMafcGrid.Clear();
    }



    public override void HideVisualCue()
    {
        throw new NotImplementedException();
    }

    public override void ResponseTimesOut()
    {
        foreach (var child in responseAlternativeGrid.Children)
        {
            if (child is CorrectionButton)
            {
                // Removing the event handler and changs the color
                var correctionButton = (CorrectionButton)child;
                correctionButton.RemoveHandler();
                correctionButton.TurnRed();
            }
        }

        // Auto wrapping up the trial
        wrapUpTrial();
    }

    public override void ShowMessage(string Message)
    {

        StopAllTimers();
        HideAllItems();

        var myHeight = this.Height;
        var textSize = Math.Round(myHeight / (12));

        var messageBtn = new Button()
        {
            Text = Message,
            BackgroundColor = Color.FromRgb(255, 255, 128),
            Padding = 10,
            TextColor = Color.FromRgb(40, 40, 40),
            FontSize = textSize,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };

        Content = messageBtn;
        //MainMafcGrid.Add(messageBtn, 0, 0);

    }


    public override void ShowVisualCue()
    {
        throw new NotImplementedException();
    }

    public override void UpdateTestFormProgressbar(int Value, int Maximum, int Minimum)
    {
        throw new NotImplementedException();
    }

    public override void AddSourceAlternatives(VisualizedSoundSource[] soundSources)
    {
        throw new NotImplementedException();
    }

    private void HideAllItems(object sender, EventArgs e)
    {
        HideAllItems();
    }

    public override void HideAllItems()
    {
        clearMainGrid();
    }

}

public class CorrectionButton : Grid
{

    private Label indicatorLabel;
    private Button repsonseButton;
    private Frame repsonseButtonFrame;
    private bool isMarkedCorrect = false;

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
            FontSize = textSize*1.5,
            VerticalTextAlignment = TextAlignment.Center,
            HorizontalTextAlignment = TextAlignment.Center,
        };

        repsonseButton = new Button()
        {
            Text = text,
            BackgroundColor = Color.FromRgb(255, 255, 128),
            Padding = new Thickness(2,10),
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
            if (isMarkedCorrect == true)
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
        isMarkedCorrect = !isMarkedCorrect;

        // Getting the responsed label

        if (isMarkedCorrect == true)
        {
            indicatorLabel.Text = "✓";
            indicatorLabel.TextColor = Color.FromRgb(4, 255, 61);

            // Modifies the frame color to mark that it's set as correct
            repsonseButtonFrame.BorderColor = Color.FromRgb(4, 255, 61);
            repsonseButtonFrame.BackgroundColor = Color.FromRgb(4, 255, 61);
        }
        else
        {
            indicatorLabel.Text = "✗";
            indicatorLabel.TextColor = Colors.Red;

            // Modifies the frame color to mark that it's set as incorrect
            repsonseButtonFrame.BorderColor = Colors.Red;
            repsonseButtonFrame.BackgroundColor = Colors.Red;
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
    }

}

 

