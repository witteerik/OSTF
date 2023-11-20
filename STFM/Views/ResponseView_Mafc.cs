using STFN;
using STFN.Audio.SoundScene;

namespace STFM.Views;

public class ResponseView_Mafc : ResponseView
{

    Grid MainMafcGrid;
    Grid responseAlternativeGrid = null;


    public ResponseView_Mafc()
    {

        MainMafcGrid = new Grid
        {
            RowDefinitions = { new RowDefinition { Height = new GridLength(1, GridUnitType.Star) } },
            ColumnDefinitions = { new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) } }
        };

        Content = MainMafcGrid;

    }


    public override void ShowResponseAlternatives(List<string> text)
    {

        responseAlternativeGrid = new Grid { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };

        responseAlternativeGrid.AddRowDefinition(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        responseAlternativeGrid.AddColumnDefinition(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        responseAlternativeGrid.AddColumnDefinition(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        responseAlternativeGrid.AddColumnDefinition(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        responseAlternativeGrid.BackgroundColor = Color.FromRgb(40, 40, 40);


        for (int i = 0; i < text.Count; i++)
        {

            var repsonseBtn = new Button()
            {
                Text = text[i],
                BackgroundColor = Color.FromRgb(255, 255, 128),
                Padding = 10,
                TextColor = Color.FromRgb(40, 40, 40),
                FontSize = 16,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };

            repsonseBtn.Clicked += reponseButton_Clicked;

            Frame frame = new Frame
            {
                BorderColor = Colors.Gray,
                CornerRadius = 8,
                ClassId = "TWA",
                Padding = 8,
                Content = repsonseBtn
            };

            responseAlternativeGrid.Add(frame, i, 0);

        }

        MainMafcGrid.Add(responseAlternativeGrid, 0, 0);

    }

    private void reponseButton_Clicked(object sender, EventArgs e)
    {

        // Getting the responsed label
        var responseBtn = sender as Button;
        var frame = responseBtn.Parent as Frame;

        // Hides all other labels, fokuses the selected one
        foreach (var child in responseAlternativeGrid.Children)
        {
            if (child is Frame)
            {
                if (object.ReferenceEquals(child, frame) == false)
                {
                    var nonSelectedAlternative = child as Frame;

                    if (nonSelectedAlternative.ClassId == "TWA")
                    {
                        nonSelectedAlternative.IsVisible = false;
                    }
                }
                else
                {
                    // Modifies the frame color to mark that it's selected
                    frame.BorderColor = Color.FromRgb(4, 255, 61);
                    frame.BackgroundColor = Color.FromRgb(4, 255, 61);
                }
            }
        }

        // Sends the linguistic response
        ReportResult(responseBtn.Text);

    }

    private void ReportResult(string RespondedSpelling)
    {
        clearMainGrid();

        // Storing the raw response
        ResponseGivenEventArgs args = new ResponseGivenEventArgs();
        args.LinguisticResponse = RespondedSpelling;
        args.TimeResponded = DateTime.Now;

        // Raising the Response given event in the base class
        OnResponseGiven(args);

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
        throw new NotImplementedException();
    }

    public override void ShowMessage(string Message)
    {
        throw new NotImplementedException();
    }

    public override void ShowResponseAlternatives(List<Tuple<string, SoundSourceLocation>> ResponseAlternatives)
    {
        throw new NotImplementedException();
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

    public override void HideAllItems()
    {
        clearMainGrid();
    }
}

