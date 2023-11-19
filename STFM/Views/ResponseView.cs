using STFN;
using STFN.Audio.SoundScene;

namespace STFM.Views;

public abstract class ResponseView : ContentView
{
	public ResponseView()
	{
        //Content = new VerticalStackLayout
        //{
        //    Children = {
        //        new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, Text = "Welcome to .NET MAUI!"
        //        }
        //    }
        //};
    }


    public abstract event IResposeView.StartedByTesteeEventHandler StartedByTestee;
    //{
    //    add
    //    {
    //        throw new NotImplementedException();
    //    }

    //    remove
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public abstract event IResposeView.ResponseGivenEventHandler ResponseGiven;
    //{
    //    add
    //    {
    //        throw new NotImplementedException();
    //    }

    //    remove
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public abstract void AddResponseAlternatives(string[] text);

    public abstract void AddDefaultSources();

    public abstract void ShowResponseAlternatives(List<string> ResponseAlternatives);

    public abstract void ShowResponseAlternatives(List<Tuple<string, SoundSourceLocation>> ResponseAlternatives);

    public abstract void ShowVisualQue();

    public abstract void HideVisualQue();

    public abstract void ResponseTimesOut();

    public abstract void ResetTestItemPanel();

    public abstract void UpdateTestFormProgressbar(int Value, int Maximum, int Minimum);

    public abstract void ShowMessage(string Message);
}





