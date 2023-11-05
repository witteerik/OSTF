namespace STFM.Views;

public partial class SpeechTestView : ContentView, IDrawable
{
	public SpeechTestView()
	{
		InitializeComponent();

        MyAudiogramView.Audiogram.Areas.Add(new Area()
        {
            Color = Colors.Turquoise,
            XValues = new[] { 250F, 1000F, 4000F, 6000F },
            YValuesLower = new[] { 20F, 30F, 35F, 40F },
            YValuesUpper = new[] { 40F, 50F, 60F, 70F }
        });

    }


    void IDrawable.Draw(ICanvas canvas, RectF dirtyRect)
    {

        MyAudiogramView.Audiogram.Draw(canvas, dirtyRect);

    }

}