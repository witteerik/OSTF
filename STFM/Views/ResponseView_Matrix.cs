using Microsoft.Maui.Controls;
using STFN;
using STFN.Audio.SoundScene;
using System.Linq;

namespace STFM.Views;

public class ResponseView_Matrix : ResponseView
{

    Grid responseAlternativeGrid = null;
    private IDispatcherTimer HideAllTimer;
    private SortedList<int, Tuple<bool, string>> givenResponsesSet = new SortedList<int, Tuple<bool, string>>();


    public ResponseView_Matrix()
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

        // resetting givenResponsesSet
        givenResponsesSet.Clear();

        // Creating a new item in givenResponsesSet
        for (int i = 0; i < ResponseAlternatives.Count; i++)
        {
            givenResponsesSet.Add(i, new Tuple<bool, string>(false, ""));

        }

        int nCols = ResponseAlternatives.Count;
        int nRows = 1;

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
        var myWidth = this.Width;
        var textSize = Math.Round(myWidth / (12 * nCols));

        for (int col = 0; col < ResponseAlternatives.Count; col++)
        {
            var column = new SelectionButtonSet(ResponseAlternatives[col], textSize, SelectionButtonSet.Orientation.Vertical, col);
            column.NewSelection += Column_NewSelection;
            responseAlternativeGrid.Add(column, col, 0);
        }

        Content = responseAlternativeGrid;

    }


    private async void Column_NewSelection(SelectionButtonSet.SelectionTextButtonEventArgs e)
    {

        if (e != null) { 

            givenResponsesSet[e.SetOrder] = new Tuple<bool, string>(true,e.SelectedText);

            // Checks if all responses are in
            foreach (var row in givenResponsesSet)
            {
                if  (row.Value.Item1 == false) { return; }
            }

            // If so, sends the response
            // Run the long-running method on a background thread
            await Task.Run(() => ReportResult());

        }
    }


    private void ReportResult()
    {

        InactivateEventHandlers();

        // Storing the raw response
        SpeechTestInputEventArgs args = new SpeechTestInputEventArgs();
        foreach (var row in givenResponsesSet)
        {
            args.LinguisticResponses.Add(row.Value.Item2);
        }

        args.LinguisticResponseTime = DateTime.Now;

        // Raising the Response given event in the base class
        OnResponseGiven(args);

        //HideAllTimer.Start();

    }

    private void InactivateEventHandlers()
    {

        foreach (SelectionButtonSet selectionButtonSet in responseAlternativeGrid.Children)
        {
            selectionButtonSet.NewSelection -= Column_NewSelection;
        }
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

    public async override void ResponseTimesOut()
    {
        InactivateEventHandlers();

        foreach (SelectionButtonSet selectionButtonSet in responseAlternativeGrid.Children)
        {
            selectionButtonSet.TurnRed();
        }

        // Reporting the result so far
        // Run the long-running method on a background thread
        await Task.Run(() => ReportResult());

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

