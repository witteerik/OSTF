namespace STFM.Views;

public partial class MafcDragDropView : ContentView
{

    bool CurrentLinguisticResponseGiven = false;
    bool CurrentDirectionResponseGiven = false;

    public enum SourceLocations
    {
        None,
        Left,
        Right
    }

    public MafcDragDropView()
	{
		InitializeComponent();

        ResponseAbsoluteLayout.BackgroundColor = Color.FromRgb(40, 40, 40);

    }

    public void AddDefaultSources()
    {
        List<SoundSource> soundSources = new List<SoundSource>();
        soundSources.Add(new SoundSource { X = 0.3, Y = 0.15, Width = 0.1, Height = 0.1, Rotation = -15, Text = "S1", SourceLocationsName = SourceLocations.Left });
        soundSources.Add(new SoundSource { X = 0.7, Y = 0.15, Width = 0.1, Height = 0.1, Rotation = 15, Text = "S2", SourceLocationsName = SourceLocations.Right });

        AddSourceAlternatives(soundSources.ToArray());

    }

    private void ReportLingusticResult(string RespondedSpelling)
    {
        // Storing the raw response

        // Correcting the response

    }

    private void ReportDirectionResult(SourceLocations RespondedSourceLocation)
    {
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
        public SourceLocations SourceLocationsName;
    }

    private List<SoundSource> CurrentSoundSources = new List<SoundSource>();

    public void AddSourceAlternatives(SoundSource[] soundSources)
    {

        CurrentSoundSources.Clear();
        foreach (SoundSource source in soundSources)
        {
            CurrentSoundSources.Add(source);
        }


        for (int i = 0; i < soundSources.Length; i++)
        {

            var label = new Label()
            {
                Text = soundSources[i].Text,
                BackgroundColor = Colors.WhiteSmoke,
                Padding = 10
            };

            label.TextColor = Color.FromRgb(40, 40, 40);
            label.HorizontalTextAlignment = TextAlignment.Center;
            label.VerticalTextAlignment = TextAlignment.Center;
            label.FontSize = 20;
            label.FontAutoScalingEnabled = true;

            label.Rotation = soundSources[i].Rotation;
            ResponseAbsoluteLayout.Children.Add(label);

            ResponseAbsoluteLayout.SetLayoutBounds(label, new Rect(soundSources[i].X, soundSources[i].Y, soundSources[i].Width, soundSources[i].Height));
            ResponseAbsoluteLayout.SetLayoutFlags(label, Microsoft.Maui.Layouts.AbsoluteLayoutFlags.All);

            // Storing a reference to the visual object
            soundSources[i].VisualObject = label;

        }

    }


    public void AddResponseAlternatives(string[] text)
    {

        for (int i = 0; i < text.Length; i++)
        {

            var label = new Label()
            {
                Text = text[i],
                BackgroundColor = Color.FromRgb(255, 255, 128),
                Padding = 10
            };

            label.TextColor = Color.FromRgb(40, 40, 40);

            label.HorizontalTextAlignment = TextAlignment.Center;
            label.VerticalTextAlignment = TextAlignment.Center;
            label.FontSize = 16;

            //var panGesture = new PanGestureRecognizer();
            //panGesture.PanUpdated += PanGestureRecognizer_PanUpdated;
            //label.GestureRecognizers.Add(panGesture);


            Frame frame = new Frame
            {
                BorderColor = Colors.Gray,
                CornerRadius = 8,
                ClassId = "TWA",
                Padding = 8,
                Content = label
            };


            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += PanGestureRecognizer_PanUpdated;
            frame.GestureRecognizers.Add(panGesture);

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


    double tempx = 0;
    double tempy = 0;

    private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
    {

        // Getting the responsed label
        var frame = sender as Frame;
        var label = frame.Content as Label;

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
        if (CurrentLinguisticResponseGiven == false)
        {
            CurrentLinguisticResponseGiven = true;
            ReportLingusticResult(label.Text);
        }

        switch (e.StatusType)
        {
            case GestureStatus.Started:

                // See https://stackoverflow.com/questions/71402699/drag-and-drop-net-maui
                // This hasn't yet been tested on iOS
                if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
                {
                    tempx = frame.TranslationX;
                    tempy = frame.TranslationY;
                }

                break;

            case GestureStatus.Running:

                if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
                {

                    // See https://stackoverflow.com/questions/71402699/drag-and-drop-net-maui
                    // This hasn't yet been tested on iOS

                    frame.TranslationX = e.TotalX + tempx;
                    frame.TranslationY = e.TotalY + tempy;
                }
                else if (DeviceInfo.Current.Platform == DevicePlatform.Android)
                {
                    frame.TranslationX += e.TotalX;
                    frame.TranslationY += e.TotalY;
                }
                else
                {
                    frame.TranslationX = e.TotalX;
                    frame.TranslationY = e.TotalY;
                }


                break;

            case GestureStatus.Completed:

                //// Move item to the transitioned location
                //ParentLayout.SetLayoutBounds(label, new Rect(label.X + label.TranslationX, label.Y + label.TranslationY, 800, label.Height));
                //label.TranslationX = 0;
                //label.TranslationY = 0;

                // Transitions back to the start location
                frame.TranslateTo(e.TotalX, e.TotalY, 100);
                break;

            default:

                break;

        }

        // Checks for overlaps, only if direction response has not been given
        if (CurrentDirectionResponseGiven == false)
        {
            SourceLocations OverlappedSourceLocation = OverlapsSoundSource(new Rect(frame.X + frame.TranslationX, frame.Y + frame.TranslationY, frame.Width, frame.Height));

            if (OverlappedSourceLocation != SourceLocations.None)
            {
                //Hides the label
                frame.IsVisible = false;
                ReportDirectionResult(OverlappedSourceLocation);

                CurrentDirectionResponseGiven = true;

            }
        }

    }


    private SourceLocations OverlapsSoundSource(Rect responseRectangle)
    {
        foreach (var soundSource in CurrentSoundSources)
        {
            if (responseRectangle.IntersectsWith(new Rect(soundSource.VisualObject.X, soundSource.VisualObject.Y, soundSource.VisualObject.Width, soundSource.VisualObject.Height)) == true)
            {

                // Selects the sound source by chancing it's color
                soundSource.VisualObject.BackgroundColor = Color.FromRgb(4, 255, 61);

                return soundSource.SourceLocationsName;
            }
        }
        return SourceLocations.None;
    }

}