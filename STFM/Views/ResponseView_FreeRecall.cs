using Microsoft.Maui.Controls;
using STFN;
using STFN.Audio.SoundScene;
using System.Linq;

namespace STFM.Views;

public class ResponseView_FreeRecall : ResponseView
{

    Grid MainMafcGrid;
    Grid responseAlternativeGrid = null;
    private IDispatcherTimer HideAllTimer;


    public ResponseView_FreeRecall()
    {

        // Creating content
        MainMafcGrid = new Grid
        {
            RowDefinitions = { new RowDefinition { Height = new GridLength(1, GridUnitType.Star) } },
            ColumnDefinitions = { new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) } }
        };
        Content = MainMafcGrid;

        // Setting background color
        MainMafcGrid.BackgroundColor = Color.FromRgb(40, 40, 40);

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

        int widestPoint = 0;
        for (int i = 0; i < ResponseAlternatives.Count; i++)
        {
            widestPoint = Math.Max(widestPoint, ResponseAlternatives[i].Count);
        }

        int nCols = widestPoint;
        int nRows = ResponseAlternatives.Count;

        // Creating a grid
        responseAlternativeGrid = new Grid { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };
        responseAlternativeGrid.BackgroundColor = Color.FromRgb(40, 40, 40);

        // Setting up row and columns
        for (int i = 0; i < nRows; i++)
        {
            responseAlternativeGrid.AddRowDefinition(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        }

        for (int i = 0; i < nCols; i++)
        {
            responseAlternativeGrid.AddColumnDefinition(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }

        // Determining suitable text size (TODO: This is a bad method, since it doesn't care for the lengths of any strings.....
        var myHeight = this.Height;
        var textSize = Math.Round(myHeight / (2.3 * nRows));

        // Creating controls and positioning them in the responseAlternativeGrid
        for (int row = 0; row < nRows; row++)
        {

            var rowList = ResponseAlternatives[row];

            for (int col = 0; col < rowList.Count; col++)
            {

                var repsonseBtn = new Button()
                {
                    Text = rowList[col],
                    BackgroundColor = Color.FromRgb(255, 255, 128),
                    Padding = 1,
                    TextColor = Color.FromRgb(40, 40, 40),
                    FontSize = textSize,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill
                };

                repsonseBtn.Clicked += reponseButton_Clicked;

                Frame frame = new Frame
                {
                    BorderColor = Colors.Gray,
                    CornerRadius = 1,
                    ClassId = "TWA",
                    Padding = 10,
                    Margin = 1,
                    Content = repsonseBtn
                };

                responseAlternativeGrid.Add(frame, col, row);

            }
        }

        MainMafcGrid.Add(responseAlternativeGrid, 0, 0);

    }

    private void reponseButton_Clicked(object sender, EventArgs e)
    {

        // Getting the responsed label
        var responseBtn = sender as Button;
        var buttonParentFrame = responseBtn.Parent as Frame;

        // Hides all other labels, fokuses the selected one
        foreach (var child in responseAlternativeGrid.Children)
        {
            if (child is Frame)
            {

                // Removing the event handlers
                var currentFrame = (Frame)child;
                if (currentFrame.Content is Button)
                {
                    var button = (Button)buttonParentFrame.Content;
                    //button.Clicked -= reponseButton_Clicked;
                }

                // Hiding all frames (and buttons) except the one clicked
                if (object.ReferenceEquals(currentFrame, buttonParentFrame) == false)
                {
                    if (currentFrame.ClassId == "TWA")
                    {
                        //currentFrame.IsVisible = false;
                    }
                }
                else
                {
                    // Modifies the frame color to mark that it's selected
                    currentFrame.BorderColor = Color.FromRgb(4, 255, 61);
                    currentFrame.BackgroundColor = Color.FromRgb(4, 255, 61);
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
        args.LinguisticResponse = RespondedSpelling;
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
        MainMafcGrid.Clear();
    }



    public override void HideVisualCue()
    {
        throw new NotImplementedException();
    }

    public override void ResponseTimesOut()
    {

        // Hides all other labels, fokuses the selected one
        foreach (var child in responseAlternativeGrid.Children)
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
                    button.Clicked -= reponseButton_Clicked;
                }
            }
        }

        // Reporting an empty response (indicating missing response)
        ReportResult("");

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

        MainMafcGrid.Add(messageBtn, 0, 0);

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

