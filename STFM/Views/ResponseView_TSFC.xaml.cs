using STFN;

namespace STFM.Views;

public partial class ResponseView_TSFC : ResponseView
{
	public ResponseView_TSFC()
	{
		InitializeComponent();

        // Assign the custom drawable to the GraphicsView
        TSFC_TriangleView.Drawable = new TSFC_Triangle(TSFC_TriangleView);
        TSFC_Triangle arrowDrawable = (TSFC_Triangle)TSFC_TriangleView.Drawable;
        //TSFC_TriangleView.Background = Color.FromArgb("#2F2F2F");

        // Force redraw on size change
        TSFC_TriangleView.SizeChanged += (s, e) => TSFC_TriangleView.Invalidate();

    }

    public override void AddSourceAlternatives(VisualizedSoundSource[] soundSources)
    {
        //throw new NotImplementedException();
    }

    public override void HideAllItems()
    {
        // throw new NotImplementedException();
    }

    public override void HideVisualCue()
    {
        // throw new NotImplementedException();
    }

    public override void InitializeNewTrial()
    {
        // throw new NotImplementedException();
    }

    public override void ResponseTimesOut()
    {
        // throw new NotImplementedException();
    }

    public override void ShowMessage(string Message)
    {
        // throw new NotImplementedException();
    }

    public override void ShowResponseAlternativePositions(List<List<SpeechTestResponseAlternative>> ResponseAlternatives)
    {
        //  throw new NotImplementedException();
    }

    public override void ShowResponseAlternatives(List<List<SpeechTestResponseAlternative>> ResponseAlternatives)
    {
        //  throw new NotImplementedException();
    }

    public override void ShowVisualCue()
    {
        //  throw new NotImplementedException();
    }

    public override void StopAllTimers()
    {
        //  throw new NotImplementedException();
    }

    public override void UpdateTestFormProgressbar(int Value, int Maximum, int Minimum)
    {
        //  throw new NotImplementedException();
    }
}