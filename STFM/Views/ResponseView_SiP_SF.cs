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
    Grid mainGrid = null;
    Grid responseAlternativeGrid = null;
    Frame responseAlternativeFrame = null;
    private IDispatcherTimer HideAllTimer;


    public ResponseView_SiP_SF()
    {

        // Setting background color
        this.BackgroundColor = Color.FromRgba("#000000");

        // Creating a hide-all timer
        HideAllTimer = Microsoft.Maui.Controls.Application.Current.Dispatcher.CreateTimer();
        HideAllTimer.Interval = TimeSpan.FromMilliseconds(200);
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


    public override void ShowResponseAlternativePositions(List<List<SpeechTestResponseAlternative>> responseAlternatives)
    {

        if (responseAlternatives.Count > 1)
        {
            throw new ArgumentException("ShowResponseAlternatives is not yet implemented for multidimensional sets of response alternatives");
        }

        List<SpeechTestResponseAlternative> localResponseAlternatives = responseAlternatives[0];

        int nItems = localResponseAlternatives.Count;
        int nRows = nItems;
        int nCols = 3;

        // Creating a main grid
        mainGrid = new Grid { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };
        mainGrid.AddRowDefinition(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        for (int i = 0; i < nCols; i++)
        {
            mainGrid.AddColumnDefinition(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }

        // Creating a frame
        responseAlternativeFrame = new Frame { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, CornerRadius = 10, 
            BorderColor = Color.FromRgb(123,123,123), Background = Color.FromRgb(47,79,79), Margin = new Thickness(15), Padding = new Thickness(10,10,10,10)};

        // Putting hte frame in column 0 or 2 depending on which side to put the response alternatives (based on the first one)
        SipTrial parentTestTrial = (SipTrial)localResponseAlternatives[0].ParentTestTrial;
        if (parentTestTrial.TargetStimulusLocations[0].HorizontalAzimuth > 0)
        {
            // the sound source is to the right, head turn to the left
            mainGrid.Add(responseAlternativeFrame, 0, 0);
        }
        else
        {
            // the sound source is to the left, head turn to the right
            mainGrid.Add(responseAlternativeFrame, 2, 0);
        }

        // Creating a grid
        responseAlternativeGrid = new Grid { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, Background = Color.FromRgb(47, 79, 79)};

        // Putting the responseAlternativeGrid in the responseAlternativeFrame
        responseAlternativeFrame.Content = responseAlternativeGrid;

        // Setting up row and columns
        for (int i = 0; i < nRows; i++)
        {
            responseAlternativeGrid.AddRowDefinition(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        }
        responseAlternativeGrid.AddColumnDefinition(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        //// Determining suitable text size (TODO: This is a bad method, since it doesn't care for the lengths of any strings.....
        //var myHeight = this.Height;
        //var textSize = Math.Round(myHeight / (4 * nRows));

        // Creating controls and positioning them in the responseAlternativeGrid
        int currentRow = -1;
        int currentColumn = 0;


        for (int i = 0; i < localResponseAlternatives.Count; i++)
        {

            //var repsonseBtn = new Button()
            //{
            //    //Text = localResponseAlternatives[i].Spelling,
            //    BackgroundColor = Color.FromRgb(255, 255, 128),
            //    Padding = 10,
            //    TextColor = Color.FromRgb(40, 40, 40),
            //    FontSize = textSize,
            //    HorizontalOptions = LayoutOptions.Fill,
            //    VerticalOptions = LayoutOptions.Fill
            //};

            //repsonseBtn.Clicked += reponseButton_Clicked;

            Frame frame = new Frame
            {
                BorderColor = Colors.Gray,
                CornerRadius = 8,
                ClassId = "TWA",
                Padding = 10,
                Margin = new Thickness(10,10,10,10)
                //Content = repsonseBtn
                //Background = Color.FromRgb(47, 79, 79)
                //BackgroundColor = Color.FromRgb(47, 79, 79)
            };

            currentRow += 1;

            responseAlternativeGrid.Add(frame, currentColumn, currentRow);

        }
                
        Content = mainGrid;

    }

    public override void ShowResponseAlternatives(List<List<SpeechTestResponseAlternative>> responseAlternatives)
    {

        if (responseAlternatives.Count > 1)
        {
            throw new ArgumentException("ShowResponseAlternatives is not yet implemented for multidimensional sets of response alternatives");
        }

        List<SpeechTestResponseAlternative> localResponseAlternatives = responseAlternatives[0];

        // Determining suitable text size (TODO: This is a bad method, since it doesn't care for the lengths of any strings.....
        var myHeight = this.Height;
        var textSize = Math.Round(myHeight / (4 * localResponseAlternatives.Count));

        // Adds the response text to the buttons and adds response handlers to activate them
        int AddedTexts = 0;
        foreach (var child in responseAlternativeGrid.Children)
        {
            if (child is Frame)
            {

                var currentFrame = (Frame)child;
                if (currentFrame.ClassId == "TWA")
                {

                    var repsonseBtn = new Button()
                    {
                        Text = localResponseAlternatives[AddedTexts].Spelling,
                        BackgroundColor = Color.FromRgb(255, 255, 128),
                        Padding = 10,
                        TextColor = Color.FromRgb(40, 40, 40),
                        FontSize = textSize,
                        HorizontalOptions = LayoutOptions.Fill,
                        VerticalOptions = LayoutOptions.Fill
                    };

                    AddedTexts += 1;

                    currentFrame.Content = repsonseBtn;

                    repsonseBtn.Clicked += reponseButton_Clicked;

                }
            }
        }
    }


    //public override void ShowResponseAlternativePositions(List<List<SpeechTestResponseAlternative>> responseAlternatives)
    //{

    //    if (responseAlternatives.Count > 1)
    //    {
    //        throw new ArgumentException("ShowResponseAlternatives is not yet implemented for multidimensional sets of response alternatives");
    //    }

    //    List<SpeechTestResponseAlternative> localResponseAlternatives = responseAlternatives[0];


    //    int nItems = localResponseAlternatives.Count;
    //    int nRows = nItems;
    //    int nCols = 3;

    //    // Creating a grid
    //    responseAlternativeGrid = new Grid { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };
    //    responseAlternativeGrid.BackgroundColor = Color.FromRgb(40, 40, 40);

    //    // Setting up row and columns
    //    for (int i = 0; i < nRows; i++)
    //    {
    //        responseAlternativeGrid.AddRowDefinition(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
    //    }

    //    for (int i = 0; i < nCols; i++)
    //    {
    //        responseAlternativeGrid.AddColumnDefinition(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
    //    }

    //    // Determining suitable text size (TODO: This is a bad method, since it doesn't care for the lengths of any strings.....
    //    var myHeight = this.Height;
    //    var textSize = Math.Round(myHeight / (4 * nRows));

    //    // Creating controls and positioning them in the responseAlternativeGrid
    //    int currentRow = -1;
    //    int currentColumn = 0;

    //    // Reading which side to put the response alternatives, based on the first one
    //    SipTrial parentTestTrial = (SipTrial)localResponseAlternatives[0].ParentTestTrial;
    //    if (parentTestTrial.TargetStimulusLocations[0].HorizontalAzimuth < 0)
    //    {
    //        currentColumn = 0;
    //    }
    //    else
    //    {
    //        currentColumn = 2;
    //    }


    //    for (int i = 0; i < localResponseAlternatives.Count; i++)
    //    {

    //        var repsonseBtn = new Button()
    //        {
    //            //Text = localResponseAlternatives[i].Spelling,
    //            BackgroundColor = Color.FromRgb(255, 255, 128),
    //            Padding = 10,
    //            TextColor = Color.FromRgb(40, 40, 40),
    //            FontSize = textSize,
    //            HorizontalOptions = LayoutOptions.Fill,
    //            VerticalOptions = LayoutOptions.Fill
    //        };

    //        //repsonseBtn.Clicked += reponseButton_Clicked;

    //        Frame frame = new Frame
    //        {
    //            BorderColor = Colors.Gray,
    //            CornerRadius = 8,
    //            ClassId = "TWA",
    //            Padding = 10,
    //            Margin = 4,
    //            Content = repsonseBtn
    //        };

    //        currentRow += 1;

    //        responseAlternativeGrid.Add(frame, currentColumn, currentRow);

    //    }

    //    Content = responseAlternativeGrid;


    //}

    //public override void ShowResponseAlternatives(List<List<SpeechTestResponseAlternative>> responseAlternatives)
    //{

    //    if (responseAlternatives.Count > 1)
    //    {
    //        throw new ArgumentException("ShowResponseAlternatives is not yet implemented for multidimensional sets of response alternatives");
    //    }

    //    List<SpeechTestResponseAlternative> localResponseAlternatives = responseAlternatives[0];

    //    // Adds the response text to the buttons and adds response handlers to activate them
    //    int AddedTexts = 0;
    //    foreach (var child in responseAlternativeGrid.Children)
    //    {
    //        if (child is Frame)
    //        {

    //            var currentFrame = (Frame)child;
    //                if (currentFrame.ClassId == "TWA")
    //                {
    //                    if (currentFrame.Content is Button)
    //                    {
    //                        var repsonseBtn = (Button)currentFrame.Content;

    //                        repsonseBtn.Text = localResponseAlternatives[AddedTexts].Spelling;
    //                        AddedTexts += 1;

    //                        repsonseBtn.Clicked += reponseButton_Clicked;

    //                    }
    //            }
    //        }
    //    }
    //}



    //public override void ShowResponseAlternatives(List<List<SpeechTestResponseAlternative>> responseAlternatives)
    //{

    //    if (responseAlternatives.Count > 1)
    //    {
    //        throw new ArgumentException("ShowResponseAlternatives is not yet implemented for multidimensional sets of response alternatives");
    //    }

    //    List<SpeechTestResponseAlternative> localResponseAlternatives = responseAlternatives[0];


    //    int nItems = localResponseAlternatives.Count;
    //    int nRows = nItems;
    //    int nCols = 3;

    //    // Creating a grid
    //    responseAlternativeGrid = new Grid { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };
    //    responseAlternativeGrid.BackgroundColor = Color.FromRgb(40, 40, 40);

    //    // Setting up row and columns
    //    for (int i = 0; i < nRows; i++)
    //    {
    //        responseAlternativeGrid.AddRowDefinition(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
    //    }

    //    for (int i = 0; i < nCols; i++)
    //    {
    //        responseAlternativeGrid.AddColumnDefinition(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
    //    }

    //    // Determining suitable text size (TODO: This is a bad method, since it doesn't care for the lengths of any strings.....
    //    var myHeight = this.Height;
    //    var textSize = Math.Round(myHeight / (4* nRows));

    //    // Creating controls and positioning them in the responseAlternativeGrid
    //    int currentRow = -1;
    //    int currentColumn = 0;

    //    // Reading which side to put the response alternatives, based on the first one
    //    SipTrial parentTestTrial = (SipTrial)localResponseAlternatives[0].ParentTestTrial;
    //    if (parentTestTrial.TargetStimulusLocations[0].HorizontalAzimuth < 0)
    //    {
    //        currentColumn = 0;
    //    }else
    //    {
    //        currentColumn = 2;
    //    }


    //    for (int i = 0; i < localResponseAlternatives.Count; i++)
    //    {

    //        var repsonseBtn = new Button()
    //        {
    //            Text = localResponseAlternatives[i].Spelling,
    //            BackgroundColor = Color.FromRgb(255, 255, 128),
    //            Padding = 10,
    //            TextColor = Color.FromRgb(40, 40, 40),
    //            FontSize = textSize,
    //            HorizontalOptions = LayoutOptions.Fill,
    //            VerticalOptions = LayoutOptions.Fill
    //        };

    //        repsonseBtn.Clicked += reponseButton_Clicked;

    //        Frame frame = new Frame
    //        {
    //            BorderColor = Colors.Gray,
    //            CornerRadius = 8,
    //            ClassId = "TWA",
    //            Padding = 10,
    //            Margin = 4,
    //            Content = repsonseBtn
    //        };

    //        currentRow += 1;

    //        responseAlternativeGrid.Add(frame, currentColumn, currentRow);

    //        //if (assymetric == true)
    //        //{
    //        //    responseAlternativeGrid.SetColumnSpan(frame, 2);
    //        //}


    //    }

    //    Content = responseAlternativeGrid;

    //}

    private async void reponseButton_Clicked(object sender, EventArgs e)
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
                    button.Clicked -= reponseButton_Clicked;
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
                    currentFrame.BorderColor = Color.FromRgb(4, 255, 61);
                    currentFrame.BackgroundColor = Color.FromRgb(4, 255, 61);
                }
            }
        }

        // Run the long-running method on a background thread
        await Task.Run(() => ReportResult(responseBtn.Text));

        HideAllTimer.Start();

    }

    private void ReportResult(string RespondedSpelling)
    {

        // Storing the raw response
        SpeechTestInputEventArgs args = new SpeechTestInputEventArgs();
        args.LinguisticResponses.Add(RespondedSpelling);
        args.LinguisticResponseTime = DateTime.Now;

        // Raising the Response given event in the base class
        OnResponseGiven(args);

    }


    private void StartedByTestee_ButtonClicked()
    {
        OnStartedByTestee(new EventArgs());
    }

    public void clearMainGrid()
    {
        //Content = null;
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

        Content = messageBtn;

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

