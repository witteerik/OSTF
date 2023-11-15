using Microsoft.Maui.Controls;

namespace STFM.Views;

public partial class MafcView : ContentView
{


    public MafcView()
	{
		InitializeComponent();

        ResponseAbsoluteLayout.BackgroundColor = Color.FromRgb(40, 40, 40);

    }

    private void ReportLingusticResult(string RespondedSpelling)
    {

        Random rnd = new Random();
        string[] stringArray2 = new string[] { rnd.Next(1, 100).ToString(), rnd.Next(1, 100).ToString(), rnd.Next(1, 100).ToString() };

        clearResponseAlternatives();

        
        AddResponseAlternatives(stringArray2);

        // Storing the raw response

        // Correcting the response

    }


    public class SoundSource
    {
        public string Text = "";
        public Image SourceImage = null;
        public double X = 0;
        public double Y = 0;
        public double Width = 0.1;
        public double Height = 0.1;
        public double Rotation = 0;
        public Label VisualObject = null;
    }

    public void AddResponseAlternatives(string[] text)
    {

        for (int i = 0; i < text.Length; i++)
        {

            var repsonseBtn = new Button()
            {
                Text = text[i],
                BackgroundColor = Color.FromRgb(255, 255, 128),
                Padding = 10
            };

            repsonseBtn.TextColor = Color.FromRgb(40, 40, 40);
            repsonseBtn.FontSize = 16;

            repsonseBtn.Clicked += reponseButton_Clicked;

            Frame frame = new Frame
            {
                BorderColor = Colors.Gray,
                CornerRadius = 8,
                ClassId = "TWA",
                Padding = 8,
                Content = repsonseBtn
            };
            
            ResponseAbsoluteLayout.Children.Add(frame);

            var labelAreaWidth = 0.7;
            var labelSectionMarginProportion = 0.15;
            var labelHeight = 0.3;
            var labelAreaVerticalLocation = 0.75;

            var labelSectionWidth = labelAreaWidth / text.Length;
            var labelSectionMargin = labelSectionWidth * labelSectionMarginProportion;
            var labelWidth = labelSectionWidth - 2 * labelSectionMargin;
            var labelX = 0.5 - labelAreaWidth / 2 + i * labelSectionWidth + labelSectionMargin + labelWidth / 2;
            var labelY = labelAreaVerticalLocation + labelHeight / 2;

            ResponseAbsoluteLayout.SetLayoutBounds(frame, new Rect(labelX, labelY, labelWidth, labelHeight));
            ResponseAbsoluteLayout.SetLayoutFlags(frame, Microsoft.Maui.Layouts.AbsoluteLayoutFlags.All);


        }
    }

    private void reponseButton_Clicked(object sender, EventArgs e)
    {

        // Getting the responsed label
        var responseBtn= sender as Button;
        var frame = responseBtn.Parent as Frame;

        // Hides all other labels, fokuses the selected one
        foreach (var child in ResponseAbsoluteLayout.Children)
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
            ReportLingusticResult(responseBtn.Text);

    }

    public void clearResponseAlternatives()
    {
        ResponseAbsoluteLayout.Children.Clear();
    }

}