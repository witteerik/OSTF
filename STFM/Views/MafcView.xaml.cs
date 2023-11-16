using Microsoft.Maui.Controls;

namespace STFM.Views;

public partial class MafcView : ContentView
{

    Grid responseAlternativeGrid = null;

    public MafcView()
	{
		InitializeComponent();

        MainMafcGrid.BackgroundColor = Color.FromRgb(40, 40, 40);

    }

    private void ReportResult(string RespondedSpelling)
    {
        clearMainGrid();
        //clearResponseAlternatives();

        AddResponseAlternatives(new string[] { "1", "2", "3" });

        // Storing the raw response

        // Correcting the response

    }


    public void AddResponseAlternatives(string[] text)
    {

        responseAlternativeGrid = new Grid { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill};

        //responsAlternativeGrid.AddRowDefinition(new RowDefinition(new GridLength(0.5, GridUnitType.Star)));
        responseAlternativeGrid.AddRowDefinition(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        responseAlternativeGrid.AddColumnDefinition(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        responseAlternativeGrid.AddColumnDefinition(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        responseAlternativeGrid.AddColumnDefinition(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        responseAlternativeGrid.BackgroundColor = Color.FromRgb(40, 40, 40);


        for (int i = 0; i < text.Length; i++)
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

            //responseAlternativeGrid.Add(repsonseBtn);

            //responseAlternativeGrid.Add(repsonseBtn, i, 0);

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
        var responseBtn= sender as Button;
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

    public void clearResponseAlternatives()
    {
        responseAlternativeGrid.Clear();
    }

    public void clearMainGrid()
    {
        MainMafcGrid.Clear();
    }

    

}