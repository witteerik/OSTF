using CommunityToolkit.Maui.Media;
using Microsoft.Maui.Controls;
using STFN;
using STFN.Audio.SoundScene;
using STFN.SipTest;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace STFM.Views;

public class ResponseView_SiP_SF : ResponseView
{
    Grid MainGrid = null;
    Grid ResponseAlternativeGrid = null;
    Frame ResponseAlternativeFrame = null;
    ProgressBar PtcProgressBar = null;
    IDispatcherTimer HideAllTimer = null;
    Button MessageButton = null;
    Frame ProgressBarFrame = null;


    public ResponseView_SiP_SF()
    {

        // Setting background color
        this.BackgroundColor = Color.FromRgba("#000000");

        // Creating a hide-all timer
        HideAllTimer = Microsoft.Maui.Controls.Application.Current.Dispatcher.CreateTimer();
        HideAllTimer.Interval = TimeSpan.FromMilliseconds(200);
        HideAllTimer.Tick += ClearMainGrid;
        HideAllTimer.IsRepeating = false;

        // Creating a main grid
        MainGrid = new Grid { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };
        MainGrid.AddRowDefinition(new RowDefinition { Height = new GridLength(25, GridUnitType.Star) });
        MainGrid.AddRowDefinition(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        for (int i = 0; i < 3; i++)
        {
            MainGrid.AddColumnDefinition(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }

        PtcProgressBar = new ProgressBar
        {
            Progress = 0,
            ProgressColor = Color.FromRgb(255, 255, 128),
            Background = Color.FromRgb(47, 79, 79),
            HorizontalOptions= LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            ScaleY = 2
        };

        ProgressBarFrame = new Frame
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            CornerRadius = 10,
            BorderColor = Color.FromRgb(123, 123, 123),
            Background = Color.FromRgb(47, 79, 79),
            Margin = new Thickness(15, 0, 15, 15),
            Padding = new Thickness(5, 0, 5, 0)
        };

        ProgressBarFrame.Content = PtcProgressBar;
        MainGrid.Add(ProgressBarFrame, 0, 1);
        MainGrid.SetColumnSpan(ProgressBarFrame, 3);


        ResponseAlternativeFrame = new Frame
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            CornerRadius = 10,
            BorderColor = Color.FromRgb(123, 123, 123),
            Background = Color.FromRgb(47, 79, 79),
            Margin = new Thickness(15, 15, 15, 15),
            Padding = new Thickness(10, 10, 10, 10)
        };

        // Adding it to the left at start
        MainGrid.Add(ResponseAlternativeFrame, 0, 0);
        ResponseAlternativeFrame.IsVisible = false;

        // Adding the MainGrid as Content of the form
        Content = MainGrid;

    }

    public override void InitializeNewTrial()
    {
        StopAllTimers();

        if (ResponseAlternativeGrid != null) { ResponseAlternativeGrid.Clear(); }

        ProgressBarFrame.IsVisible = true;
    }

    public override void StopAllTimers()
    {
        HideAllTimer.Stop();
    }


    public override void ShowResponseAlternativePositions(List<List<SpeechTestResponseAlternative>> responseAlternatives)
    {

        if (responseAlternatives.Count > 1)
        {
            throw new ArgumentException("ShowResponseAlternatives is not yet implemented for multidimensional sets of response alternatives");
        }

        List<SpeechTestResponseAlternative> localResponseAlternatives = responseAlternatives[0];

        // Putting hte frame in column 0 or 2 depending on which side to put the response alternatives (based on the first one)
        SipTrial parentTestTrial = (SipTrial)localResponseAlternatives[0].ParentTestTrial;
        if (parentTestTrial.TargetStimulusLocations[0].HorizontalAzimuth > 0)
        {
            // the sound source is to the right, head turn to the left
            MainGrid.SetColumn(ResponseAlternativeFrame, 0);
        }
        else
        {
            // the sound source is to the left, head turn to the right
            MainGrid.SetColumn(ResponseAlternativeFrame, 2);
        }

        ResponseAlternativeFrame.IsVisible = true;

    }

    public override void ShowResponseAlternatives(List<List<SpeechTestResponseAlternative>> responseAlternatives)
    {

        List<SpeechTestResponseAlternative> localResponseAlternatives = responseAlternatives[0];

        int nRows = localResponseAlternatives.Count;

        // Creating a grid
        ResponseAlternativeGrid = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Background = Color.FromRgb(47, 79, 79),
            Padding = new Thickness(20,0,20,0)
        };

        // Setting up row and columns
        for (int i = 0; i < nRows; i++)
        {
            ResponseAlternativeGrid.AddRowDefinition(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        }
        ResponseAlternativeGrid.AddColumnDefinition(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        // Determining suitable text size (TODO: This is a bad method, since it doesn't care for the lengths of any strings.....
        var myHeight = this.Height;
        var textSize = Math.Round(myHeight / (4 * nRows));

        // Creating controls and positioning them in the responseAlternativeGrid
        int currentRow = -1;

        for (int i = 0; i < localResponseAlternatives.Count; i++)
        {

            var repsonseBtn = new Button()
            {
                Text = localResponseAlternatives[i].Spelling,
                BackgroundColor = Color.FromRgb(255, 255, 128),
                BorderColor = Color.FromRgb(255, 255, 128),
                ClassId = "TWA",
                CornerRadius = 8,
                Padding = 0,
                Margin = new Thickness(0, 15, 0, 15),
                TextColor = Color.FromRgb(40, 40, 40),
                FontSize = textSize,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };

            repsonseBtn.Clicked += reponseButton_Clicked;
            currentRow += 1;
            ResponseAlternativeGrid.Add(repsonseBtn, 0, currentRow);

        }

        // Putting the responseAlternativeGrid in the responseAlternativeFrame
        ResponseAlternativeFrame.Content = ResponseAlternativeGrid;

    }


    private void reponseButton_Clicked(object sender, EventArgs e)
    {

        // Getting the responsed label
        var responseBtn = sender as Button;

        if (ResponseAlternativeGrid != null)
        {

            // Hides all other labels, fokuses the selected one
            foreach (var child in ResponseAlternativeGrid.Children)
            {
                if (child is Button)
                {

                    var button = (Button)child;
                    // Removing the response handlers
                    button.Clicked -= reponseButton_Clicked;

                    // Hiding all buttons except the one clicked
                    if (object.ReferenceEquals(button, responseBtn) == false)
                    {
                        if (button.ClassId == "TWA")
                        {
                            button.IsVisible = false;
                        }
                    }
                }
            }

            ReportResult(responseBtn.Text);

        }
    }

    private void ReportResult(string RespondedSpelling)
    {
        // Storing the raw response
        SpeechTestInputEventArgs args = new SpeechTestInputEventArgs();
        args.LinguisticResponses.Add(RespondedSpelling);
        args.LinguisticResponseTime = DateTime.Now;

        // Raising the ResponseGiven event in the base class.
        // Note that this is done on a background thread that returns to the main thread after a short delay to allow the GUI to be updated.
        OnResponseGiven(args);

        HideAllTimer.Start();

    }

    private void StartedByTestee_ButtonClicked()
    {
        OnStartedByTestee(new EventArgs());
    }

    public void ClearMainGrid()
    {
        ResponseAlternativeFrame.IsVisible = false;
        if (ResponseAlternativeGrid != null) { ResponseAlternativeGrid.Clear(); }
        MainGrid.Remove(MessageButton);
    }

    public override void HideVisualCue()
    {
        //throw new NotImplementedException();
    }

    public override void ResponseTimesOut()
    {

        if (ResponseAlternativeGrid != null)
        {

            // Hides all other labels, fokuses the selected one
            foreach (var child in ResponseAlternativeGrid.Children)
            {
                if (child is Button)
                {
                        var button = (Button)child;
                        button.BorderColor = Colors.Red;
                        button.BackgroundColor = Colors.Red;

                        // Also removing the event handler
                        button.Clicked -= reponseButton_Clicked;
                }
            }

            // Reporting an empty response (indicating missing response)
            ReportResult("");
        }

    }

    public override void ShowMessage(string Message)
    {

        StopAllTimers();
        //HideAllItems();

        var myHeight = this.Height;
        var textSize = Math.Round(myHeight / (12));

        MessageButton = new Button()
        {
            Text = Message,
            BackgroundColor = Color.FromRgb(255, 255, 128),
            Padding = 10,
            TextColor = Color.FromRgb(40, 40, 40),
            FontSize = textSize,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };

        MainGrid.Add(MessageButton, 1, 1);
    }


    public override void ShowVisualCue()
    {
        //throw new NotImplementedException();
    }

    public async override void UpdateTestFormProgressbar(int Value, int Maximum, int Minimum)
    {
        if (PtcProgressBar != null)
        {
            double range = Maximum - Minimum;
            double progressProp = Value / range;
            await PtcProgressBar.ProgressTo(progressProp, 50, Easing.Linear);
        }
    }

    public override void AddSourceAlternatives(VisualizedSoundSource[] soundSources)
    {
        //throw new NotImplementedException();
    }

    private void ClearMainGrid(object sender, EventArgs e)
    {
        ClearMainGrid();
    }


    public override void HideAllItems()
    {
        ClearMainGrid();
        ProgressBarFrame.IsVisible = false;
    }

}

