using STFN;
using STFN.Audio.SoundScene;

namespace STFM.Views;

public abstract class ResponseView : ContentView
{
    public ResponseView()
    {
        // Here we could make room for a test progress bar, info labels and start button etc. Instead of having the derived classeds filling up the content directly, they could fill a central cell in a grid.
        //Content = new VerticalStackLayout
        //{
        //    Children = {
        //        new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, Text = "Welcome to .NET MAUI!"
        //        }
        //    }
        //};
    }

    public ReponseAlternativeLayoutTypes ReponseAlternativeLayoutType = ReponseAlternativeLayoutTypes.Flexible;

    public enum ReponseAlternativeLayoutTypes
    {
        TopDown,
        LeftToRight,
        Matrix,
        Flexible,
        Free
    }



    public event EventHandler StartedByTestee;

    protected virtual void OnStartedByTestee(EventArgs e)
    {
        EventHandler handler = StartedByTestee;
        if (handler != null)
        {
            handler(this, e);
        }
    }

    public abstract void InitializeNewTrial();

    public abstract void StopAllTimers();


    public event EventHandler<SpeechTestInputEventArgs> ResponseGiven;

    protected virtual void OnResponseGiven(SpeechTestInputEventArgs e)
    {
        EventHandler<SpeechTestInputEventArgs> handler = ResponseGiven;
        if (handler != null)
        {
            handler(this, e);
        }
    }



    public event EventHandler<SpeechTestInputEventArgs> ResponseHistoryUpdated;

    protected virtual void OnResponseHistoryUpdated(SpeechTestInputEventArgs e)
    {
        EventHandler<SpeechTestInputEventArgs> handler = ResponseHistoryUpdated;
        if (handler != null)
        {
            handler(this, e);
        }
    }

    public event EventHandler<EventArgs> CorrectionButtonClicked;

    protected virtual void OnCorrectionButtonClicked(EventArgs e)
    {
        EventHandler<EventArgs> handler = CorrectionButtonClicked;
        if (handler != null)
        {
            handler(this, e);
        }
    }


    public abstract void AddSourceAlternatives(VisualizedSoundSource[] soundSources);

    
    /// <summary>
    /// Should contain a list of lists of response alternatives, order column wise
    /// </summary>
    /// <param name="ResponseAlternatives"></param>
    public abstract void ShowResponseAlternativePositions(List<List<SpeechTestResponseAlternative>> ResponseAlternatives);

    /// <summary>
    /// Should contain a list of lists of response alternatives, order column wise
    /// </summary>
    /// <param name="ResponseAlternatives"></param>
    public abstract void ShowResponseAlternatives(List<List<SpeechTestResponseAlternative>> ResponseAlternatives);

    public abstract void ShowVisualCue();

    public abstract void HideVisualCue();

    public abstract void ResponseTimesOut();

    public abstract void HideAllItems();

    public abstract void UpdateTestFormProgressbar(int Value, int Maximum, int Minimum);

    public abstract void ShowMessage(string Message);


    public class VisualizedSoundSource
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


}





