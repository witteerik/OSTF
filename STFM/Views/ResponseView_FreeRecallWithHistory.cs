using Microsoft.Maui.Controls;
//using Microsoft.UI.Xaml.Controls;
using STFN;
using STFN.Audio.SoundScene;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace STFM.Views;

public class ResponseView_FreeRecallWithHistory : ResponseView
{

    Grid responseAlternativeGrid = null;
    private IDispatcherTimer HideAllTimer;

     private int HistoryLength;
    private double InitialWidth;
    private double InitialHeight;

    public ResponseView_FreeRecallWithHistory(double InitialWidth, double  InitialHeight , int HistoryLength =3)
    {

        // Setting background color
        this.BackgroundColor = Color.FromRgb(40, 40, 40);
        //MainMafcGrid.BackgroundColor = Color.FromRgb(40, 40, 40);

        // Creating a hide-all timer
        HideAllTimer = Application.Current.Dispatcher.CreateTimer();
        HideAllTimer.Interval = TimeSpan.FromMilliseconds(300);
        HideAllTimer.Tick += HideAllItems;
        HideAllTimer.IsRepeating = false;

        this.HistoryLength = HistoryLength;
        this.InitialWidth = InitialWidth;
        this.InitialHeight = InitialHeight;
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

        bool CalledEmpty = false;
        if (ResponseAlternatives.Count == 0)
        {
            CalledEmpty = true;
        }

        if (ResponseAlternatives.Count > 1)
        {
            throw new ArgumentException("ShowResponseAlternatives is not yet implemented for multidimensional sets of response alternatives");
        }

        List<SpeechTestResponseAlternative> localResponseAlternatives;

        if (CalledEmpty == false)
        {
            localResponseAlternatives = ResponseAlternatives[0];
        }
        else
        {
            localResponseAlternatives = new List<SpeechTestResponseAlternative>();
            localResponseAlternatives.Add(new SpeechTestResponseAlternative() { TrialPresentationIndex = -1 });
        }

        int CurrentTrialPresentationIndex = localResponseAlternatives.Last().TrialPresentationIndex;

        int nItems = localResponseAlternatives.Count;
        //int nRows = 3;
        int nCols = HistoryLength+1;

        // filling up with invisible SpeechTestResponseAlternative
        int MissingCount = (HistoryLength +1)- localResponseAlternatives.Count;
        for (int i = 0; i < MissingCount; i++)
        {
            localResponseAlternatives.Insert(0, new SpeechTestResponseAlternative() { IsVisible = false, Spelling = "XXXX", TrialPresentationIndex = CurrentTrialPresentationIndex - localResponseAlternatives.Count });
        }

        // Creating a grid
        responseAlternativeGrid = new Grid { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };
        responseAlternativeGrid.BackgroundColor = Color.FromRgb(40, 40, 40);

        // Setting up rows and columns
        responseAlternativeGrid.AddRowDefinition(new RowDefinition { Height = new GridLength(0.5, GridUnitType.Star) });
        responseAlternativeGrid.AddRowDefinition(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
        responseAlternativeGrid.AddRowDefinition(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        List<Double> historySizeFactor = new List<double>();
        double historySizeChange = 1.4 / (double)nCols;
        for (int i = 0; i < nCols; i++)
        {
            historySizeFactor.Insert(0,(double)1 - historySizeChange * (double)(localResponseAlternatives[i].TrialPresentationIndex- CurrentTrialPresentationIndex));
        }

        for (int i = 0; i < nCols; i++)
        {
            responseAlternativeGrid.AddColumnDefinition(new ColumnDefinition { Width = new GridLength(3.0 + (double)localResponseAlternatives[i].Spelling.Length * historySizeFactor[i], GridUnitType.Star) });
        }
        responseAlternativeGrid.AddColumnDefinition(new ColumnDefinition { Width = new GridLength( 50, GridUnitType.Absolute)});

        // Determining suitable text size (TODO: This is a bad method, since it doesn't care for the lengths of any strings.....
        var textSize = Math.Round(this.Width / 35);

        // A hack to solve the case when the view has not yet been sized
        if (this.Width == -1)
        {
            textSize = InitialWidth / 35;
        }

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
            FontSize = textSize,
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
            FontSize = textSize,
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
            CorrectionButton correctionButton = new CorrectionButton(localResponseAlternatives[i], textSize * historySizeFactor[i]);

            correctionButton.RepsonseButtonFrame.CornerRadius = 10;

            if (localResponseAlternatives[i].ParentTestTrial != null)
            {
                correctionButton.ShowAsCorrect = localResponseAlternatives[i].ParentTestTrial.IsCorrect;
                correctionButton.setButtonAppearence();
            }

            double ViewHeight = this.Height;
            if (ViewHeight == -1)
            {
                ViewHeight = InitialHeight;
            }

            correctionButton.Margin = new Thickness(3);
            correctionButton.HeightRequest = (ViewHeight/ 5) * historySizeFactor[i];//responseAlternativeGrid.Height * historySizeFactor[i];

            correctionButton.IsVisible = localResponseAlternatives[i].IsVisible;

            if (localResponseAlternatives[i].TrialPresentationIndex >= 0)
            {
                if (i < localResponseAlternatives.Count - 1)
                {
                    correctionButton.Clicked += HistoricTrial_Clicked;
                    // The last button is the current test trial and does not trigger any event.
                }
                else
                {
                    // This is the current trial correctionButton, adding the CorrectionButton_Clicked event
                    correctionButton.Clicked += CorrectionButton_Clicked;
                }
            }
            else
            {
                correctionButton.IsVisible = false;
            }

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
                Margin = new Thickness(200, 30),
                Content = controlButton
            };

            if (CalledEmpty == true)
            {
                controlButton.IsVisible = false;
                controlButtonFrame.IsVisible = false;
            }

            responseAlternativeGrid.Add(controlButtonFrame, 0, 2);
            responseAlternativeGrid.SetColumnSpan(controlButtonFrame, nCols);

        }

        Content = responseAlternativeGrid;

    }


    private async void wrapUpTrial()
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

        // Using only the last reponse alternative (the current trial)
        List<string> CorrectResponse = new List<string>();
        CorrectResponse.Add(CorrectResponses.Last());

        clearMainGrid();

        // Sends the linguistic response
        // Run the long-running method on a background thread
        await Task.Run(() => ReportResult(CorrectResponse));
        //ReportResult(CorrectResponse);

    }

    private void HistoricTrial_Clicked(object sender, EventArgs e)
    {

        List<string> CorrectResponses = new List<string>();

        // Gets only historic responses
        for (int i = 0; i < responseAlternativeGrid.Children.Count-2; i++)
        {
        var child = responseAlternativeGrid.Children[i];
            if (child is CorrectionButton)
            {
                var correctionButton = (CorrectionButton)child;
                CorrectResponses.Add(correctionButton.GetValue());
            }
        }

        // Storing the updated responses
        SpeechTestInputEventArgs args = new SpeechTestInputEventArgs();
        args.LinguisticResponses = CorrectResponses;
        args.LinguisticResponseTime = DateTime.Now;

        // Raising the Response given event in the base class
        OnResponseHistoryUpdated(args);

    }

    private void CorrectionButton_Clicked(object sender, EventArgs e)
    {
        OnCorrectionButtonClicked(e);
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
        responseAlternativeGrid.Clear();
        //HideAllItems();

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

        //Content = messageBtn;
        responseAlternativeGrid.Add(messageBtn, 0, 0);

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


