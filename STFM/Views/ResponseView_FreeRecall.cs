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

    public override void ShowResponseAlternativePositions(List<List<SpeechTestResponseAlternative>> ResponseAlternatives)
    {
        throw new NotImplementedException("ShowResponseAlternativePositions is not implemented");
    }        

    public override void ShowResponseAlternatives(List<List<SpeechTestResponseAlternative>> ResponseAlternatives)
    {

        if (ResponseAlternatives.Count > 1)
        {
            throw new ArgumentException("ShowResponseAlternatives is not yet implemented for multidimensional sets of response alternatives");
        }

        List<SpeechTestResponseAlternative> localResponseAlternatives = ResponseAlternatives[0];

        int nItems = localResponseAlternatives.Count;
        int nCols = nItems;

        // Creating a grid
        responseAlternativeGrid = new Grid { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };
        responseAlternativeGrid.BackgroundColor = Color.FromRgb(40, 40, 40);

        // Setting up rows and columns
        responseAlternativeGrid.AddRowDefinition(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        responseAlternativeGrid.AddRowDefinition(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
        responseAlternativeGrid.AddRowDefinition(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });

        for (int i = 0; i < nCols; i++)
        {
            responseAlternativeGrid.AddColumnDefinition(new ColumnDefinition { Width = new GridLength(3 + localResponseAlternatives[i].Spelling.Length, GridUnitType.Star) });
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
            correctionButton.Clicked += CorrectionButton_Clicked;
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
                Margin = new Thickness(200, 60),
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

    private void CorrectionButton_Clicked(object sender, EventArgs e)
    {
        OnCorrectionButtonClicked(e);
    }

    private void ReportResult(List<string> CorrectResponses)
    {

        // Storing the raw response
        SpeechTestInputEventArgs args = new SpeechTestInputEventArgs();
        args.LinguisticResponses = CorrectResponses;
        args.LinguisticResponseTime = DateTime.Now;

        // Raising the ResponseGiven event in the base class.
        // Note that this is done on a background thread that returns to the main thread after a short delay to allow the GUI to be updated.
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
        responseAlternativeGrid.Clear();
        //HideAllItems();

        var myHeight = this.Height;
        var textSize = Math.Round(myHeight / (20));

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

        responseAlternativeGrid = new Grid { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };
        responseAlternativeGrid.BackgroundColor = Color.FromRgb(40, 40, 40);

        responseAlternativeGrid.AddRowDefinition(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        responseAlternativeGrid.AddRowDefinition(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        responseAlternativeGrid.AddRowDefinition(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        responseAlternativeGrid.AddColumnDefinition(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        responseAlternativeGrid.AddColumnDefinition(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) });
        responseAlternativeGrid.AddColumnDefinition(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        //Content = messageBtn;
        responseAlternativeGrid.Add(messageBtn, 1, 1);

        Content = responseAlternativeGrid;

    }


    public override void ShowVisualCue()
    {
        throw new NotImplementedException();
    }

    public override void UpdateTestFormProgressbar(int Value, int Maximum, int Minimum)
    {
        //throw new NotImplementedException();
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


