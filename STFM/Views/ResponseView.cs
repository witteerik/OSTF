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


    public event EventHandler StartedByTestee;

    protected virtual void OnStartedByTestee(EventArgs e)
    {
        EventHandler  handler = StartedByTestee;
        if (handler != null)
        {
            handler(this, e);
        }
    }


    public event EventHandler<ResponseGivenEventArgs> ResponseGiven;

    protected virtual void OnResponseGiven(ResponseGivenEventArgs e)
    {
        EventHandler<ResponseGivenEventArgs> handler = ResponseGiven;
        if (handler != null)
        {
            handler(this, e);
        }
    }



    public abstract void AddResponseAlternatives(string[] text);

    public abstract void AddDefaultSources();

    public abstract void AddSourceAlternatives(VisualizedSoundSource[] soundSources);


    public abstract void ShowResponseAlternatives(List<string> ResponseAlternatives);

    public abstract void ShowResponseAlternatives(List<Tuple<string, SoundSourceLocation>> ResponseAlternatives);

    public abstract void ShowVisualQue();

    public abstract void HideVisualQue();

    public abstract void ResponseTimesOut();

    public abstract void ResetTestItemPanel();

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





