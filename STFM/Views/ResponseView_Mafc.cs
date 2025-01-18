using Microsoft.Maui.Controls;
using STFN;
using STFN.Audio.SoundScene;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace STFM.Views;

public class ResponseView_Mafc : ResponseView
{

    Grid MainGrid = null;
    Grid ResponseAlternativeGrid = null;
    ProgressBar PtcProgressBar = null;
    private IDispatcherTimer HideAllTimer;
    Frame ProgressBarFrame = null;


    public ResponseView_Mafc()
    {

        // Setting background color
        this.BackgroundColor = Color.FromRgb(40, 40, 40);

        // Creating a hide-all timer
        HideAllTimer = Microsoft.Maui.Controls.Application.Current.Dispatcher.CreateTimer();
        HideAllTimer.Interval = TimeSpan.FromMilliseconds(300);
        HideAllTimer.Tick += ClearMainGrid;
        HideAllTimer.IsRepeating = false;

        // Creating a main grid
        MainGrid = new Grid { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };
        MainGrid.AddRowDefinition(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        MainGrid.AddRowDefinition(new RowDefinition { Height = new GridLength(30, GridUnitType.Absolute) });
        MainGrid.AddColumnDefinition(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        PtcProgressBar = new ProgressBar
        {
            Progress = 0,
            ProgressColor = Color.FromRgb(255, 255, 128),
            Background = Color.FromRgb(47, 79, 79),
            HorizontalOptions = LayoutOptions.Fill,
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
        // Hides the progress-bar frame, until used
        ProgressBarFrame.IsVisible = false;

        // Creating a grid
        ResponseAlternativeGrid = new Grid { 
            HorizontalOptions = LayoutOptions.Fill, 
            VerticalOptions = LayoutOptions.Fill, 
            Padding = new Thickness(15,15,15,15),
            BackgroundColor = Color.FromRgb(40, 40, 40)
        };

        // Adding the ResponseAlternativeGrid to the MainGrid
        MainGrid.Add(ResponseAlternativeGrid, 0, 0);

        // Adding the MainGrid as Content of the form
        Content = MainGrid;

    }

    public override void InitializeNewTrial()
    {
        StopAllTimers();
        
        if (ResponseAlternativeGrid != null) {
            //ResponseAlternativeGrid.IsVisible = false;
            ResponseAlternativeGrid.Clear();
            ResponseAlternativeGrid.RowDefinitions.Clear();
            ResponseAlternativeGrid.ColumnDefinitions.Clear();
        }

    }

    public override void StopAllTimers()
    {
        HideAllTimer.Stop();
    }

    public override void ShowResponseAlternativePositions(List<List<SpeechTestResponseAlternative>> ResponseAlternatives)
    {
        // Not used in this view as response alternatives are always centralized
        //throw new NotImplementedException("ShowResponseAlternativePositions is not implemented");
    }


    public override void ShowResponseAlternatives(List<List<SpeechTestResponseAlternative>> responseAlternatives)
    {

        if (responseAlternatives.Count > 1)
        {
            throw new ArgumentException("ShowResponseAlternatives is not yet implemented for multidimensional sets of response alternatives");
        }

        List<SpeechTestResponseAlternative> localResponseAlternatives = responseAlternatives[0];


        int nItems = localResponseAlternatives.Count;
        int nRows = 0;
        int nCols = 0;
        bool assymetric = false;

        var localReponseAlternativeLayoutType = ReponseAlternativeLayoutType;

        if (localReponseAlternativeLayoutType == ReponseAlternativeLayoutTypes.Flexible)
        {
            if (true)
            {
                if (nItems < 7)
                {
                    localReponseAlternativeLayoutType = ReponseAlternativeLayoutTypes.Matrix;
                }
                else if (nItems < 11)
                {
                    localReponseAlternativeLayoutType = ReponseAlternativeLayoutTypes.TopDown;
                }
                else
                {
                    localReponseAlternativeLayoutType = ReponseAlternativeLayoutTypes.Matrix;
                }
            }
        }

        int LastRowSkipFrames = 0;

        switch (localReponseAlternativeLayoutType)
        {
            case ReponseAlternativeLayoutTypes.TopDown:
                nRows = nItems;
                nCols = 1;
                break;
            case ReponseAlternativeLayoutTypes.LeftToRight:
                nRows = 1;
                nCols = nItems;
                break;
            case ReponseAlternativeLayoutTypes.Matrix:

                var aspectRatio = 4 / 3;
                nCols = (int)Math.Ceiling(Math.Sqrt(nItems) * aspectRatio);
                nRows = (int)Math.Ceiling(nItems / (double)nCols);

                int nLastRowItemCount = nItems - (nCols *(nRows-1)) ;
                if ((nCols % nLastRowItemCount !=0) | ((nLastRowItemCount == 1) & (nCols>1)))
                {
                    assymetric = true;
                    nCols *= 2;
                    LastRowSkipFrames = (nCols - nLastRowItemCount *2) / 2;
                }
                else
                {
                    LastRowSkipFrames = (nCols - nLastRowItemCount) / 2;
                }

                break;
            case ReponseAlternativeLayoutTypes.Flexible:

                // This option is handled before the Switch statement, and the code should not end up here
                throw new NotImplementedException("This is a bug! There is something wrong with the usage of Flexible ReponseAlternativeLayoutType. Please report it to the OSTF team!");
                //break;

            case ReponseAlternativeLayoutTypes.Free:

                throw new NotImplementedException("ReponseAlternativeLayoutType Free is ot yet implemented");
                //break;
            default:
                break;
        }


        // Clearing the ResponseAlternativeGrid 
        ResponseAlternativeGrid.Clear();

        // Setting up row and columns dynamically depending on the number of respoinse alternatives
        for (int i = 0; i < nRows; i++)
        {
            ResponseAlternativeGrid.AddRowDefinition(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        }

        for (int i = 0; i < nCols; i++)
        {
            ResponseAlternativeGrid.AddColumnDefinition(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }

        // Determining suitable text size (TODO: This is a bad method, since it doesn't care for the lengths of any strings.....
        var myHeight = this.Height;
        var textSize = Math.Round(myHeight / (4* nRows));

        // Creating controls and positioning them in the ResponseAlternativeGrid
        int currentRow = -1;
        int currentColumn = 0;
        for (int i = 0; i < localResponseAlternatives.Count; i++)
        {

            var repsonseBtn = new Button()
            {
                Text = localResponseAlternatives[i].Spelling,
                BackgroundColor = Color.FromRgb(255, 255, 128),
                Padding = 10,
                TextColor = Color.FromRgb(40, 40, 40),
                FontSize = textSize,
                CornerRadius = 8,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };

            repsonseBtn.Clicked += ReponseButton_Clicked;

            Frame frame = new Frame
            {
                BorderColor = Color.FromRgb(123, 123, 123),
                Background = Color.FromRgb(47, 79, 79),
                //BorderColor = Colors.Gray,
                CornerRadius = 8,
                ClassId = "TWA",
                Padding = 10,
                Margin = 25,
                Content = repsonseBtn
            };

            if (assymetric == false)
            {
                if ((i % nCols) == 0)
                {
                    currentRow += 1;
                    currentColumn = 0;
                }
                else
                {
                    currentColumn += 1;
                }
            }
            else
            {
                if ((i*2 % nCols ) == 0)
                {
                    currentRow += 1;
                    currentColumn = 0;
                }
                else
                {
                    currentColumn += 2;
                }
            }

            if ((currentColumn == 0) & (currentRow == nRows - 1))
            {
                currentColumn = LastRowSkipFrames;
            }


            ResponseAlternativeGrid.Add(frame, currentColumn, currentRow);

            if (assymetric == true)
            {
                ResponseAlternativeGrid.SetColumnSpan(frame, 2);
            }


        }

        // Shows the ResponseAlternativeGrid
        //ResponseAlternativeGrid.IsVisible = true;

    }

    private void ReponseButton_Clicked(object sender, EventArgs e)
    {

        // Getting the responsed label
        var responseBtn = sender as Button;
        var buttonParentFrame = responseBtn.Parent as Frame;

        // Hides all other labels, fokuses the selected one
        foreach (var child in ResponseAlternativeGrid.Children)
        {
            if (child is Frame)
            {

                // Removing the event handlers
                var currentFrame = (Frame)child;
                if (currentFrame.Content is Button)
                {
                    var button = (Button)buttonParentFrame.Content;
                    button.Clicked -= ReponseButton_Clicked;
                }

                // Hiding all frames (and buttons) except the one clicked
                if (object.ReferenceEquals(currentFrame, buttonParentFrame) == false)
                {
                    if (currentFrame.ClassId == "TWA")
                    {
                        currentFrame.IsVisible = false;
                    }
                }
                else
                {
                    // Modifies the frame color to mark that it's selected
                    // frame.BorderColor = Color.FromRgb(4, 255, 61);
                    // frame.BackgroundColor = Color.FromRgb(4, 255, 61);
                }
            }
        }

        // Sends the linguistic response
        ReportResult(responseBtn.Text);

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
        if (ResponseAlternativeGrid != null) {
            ResponseAlternativeGrid.Clear();
            ResponseAlternativeGrid.RowDefinitions.Clear();
            ResponseAlternativeGrid.ColumnDefinitions.Clear();
        }
    }


    public override void HideVisualCue()
    {
        // Ignored in this view
        //throw new NotImplementedException();
    }

    public override void ResponseTimesOut()
    {

        // Hides all other labels, fokuses the selected one
        foreach (var child in ResponseAlternativeGrid.Children)
        {
            if (child is Frame)
            {
                var frame = (Frame)child;
                // Modifies the frame color to mark that it's missed
                // Modifies the frame border color
                //frame.BorderColor = Colors.LightGray; 
                //frame.BackgroundColor = Colors.LightGray;

                // Modifies the button color
                if (frame.Content is Button)
                {
                    var button = (Button)frame.Content;
                    button.BorderColor = Colors.Red;
                    button.BackgroundColor = Colors.Red;

                    // Also removing the event handler
                    button.Clicked -= ReponseButton_Clicked;
                }
            }
        }

        // Reporting an empty response (indicating missing response)
        ReportResult("");

    }

    public override void ShowMessage(string Message)
    {

        StopAllTimers();    
        //HideAllItems();

        var myHeight = this.Height;
        var textSize = Math.Round(myHeight / (12));

        // Putting the MessageButton in the response view
        ClearMainGrid();
        ResponseAlternativeGrid.AddRowDefinition(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        ResponseAlternativeGrid.AddColumnDefinition(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        var MessageButton = new Button()
        {
            Text = Message,
            BackgroundColor = Color.FromRgb(255, 255, 128),
            Padding = 10,
            TextColor = Color.FromRgb(40, 40, 40),
            FontSize = textSize,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(50,60,50,60)
        };

        ResponseAlternativeGrid.Add(MessageButton, 0, 0);

        // Shows the ResponseAlternativeGrid
        //ResponseAlternativeGrid.IsVisible = true;

    }


    public override void ShowVisualCue()
    {
        // Ignored in this view
        //throw new NotImplementedException();
    }

    public async override void UpdateTestFormProgressbar(int Value, int Maximum, int Minimum)
    {
        if (PtcProgressBar != null)
        {
            ProgressBarFrame.IsVisible = true;
            double range = Maximum - Minimum;
            double progressProp = Value / range;
            await PtcProgressBar.ProgressTo(progressProp, 50, Easing.Linear);
        }
    }

    public override void AddSourceAlternatives(VisualizedSoundSource[] soundSources)
    {
        throw new NotImplementedException();
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

