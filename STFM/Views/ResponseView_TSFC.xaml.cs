using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics.Platform;
using STFN;

namespace STFM.Views;

public partial class ResponseView_TSFC : ResponseView
{

    TSFC_Triangle CurrentTSFC_Triangle = null;

    public ResponseView_TSFC()
	{
		InitializeComponent();

        // Assign the custom drawable to the GraphicsView
        TSFC_TriangleView.Drawable = new TSFC_Triangle(TSFC_TriangleView);
        
        // Referencing the TSFC_Triangle so that it gets directly accessible in code
        CurrentTSFC_Triangle = (TSFC_Triangle)TSFC_TriangleView.Drawable;
        //CurrentTSFC_Triangle.Background = Color.FromArgb("#2F2F2F");

        TSFC_TriangleView.StartInteraction += OnStartInteraction;
        TSFC_TriangleView.DragInteraction += OnDragInteraction;
        TSFC_TriangleView.EndInteraction += OnEndInteraction;

        // Cascades loaded and any resizing to the CurrentTSFC_Triangle
        TSFC_TriangleView.Loaded += CascadeResizeToTriangle;
        TSFC_TriangleView.SizeChanged += CascadeResizeToTriangle;


    }


    private void CascadeResizeToTriangle(object sender, EventArgs e)
    {
        // Updating the cached size of the CurrentTSFC_Triangle
        var width = TSFC_TriangleView.Width;
        var height = TSFC_TriangleView.Height;
        CurrentTSFC_Triangle.ViewportSize = new SizeF((float)width, (float)height);

        // Forcíng redraw on size change
        TSFC_TriangleView.Invalidate();
    }


    private void OnStartInteraction(object sender, TouchEventArgs e)
    {
        // User has started touching
        PointF touchPoint = new PointF(e.Touches.First().X, e.Touches.First().Y);

        if (CurrentTSFC_Triangle != null)
        {
            CurrentTSFC_Triangle.StartInteraction(touchPoint);
        }

    }

    private void OnDragInteraction(object sender, TouchEventArgs e)
    {
        // Tracking the movement
        PointF touchPoint= new PointF( e.Touches.First().X, e.Touches.First().Y);

        if (CurrentTSFC_Triangle != null)
        {
            CurrentTSFC_Triangle.DragInteraction(touchPoint);
        }

    }

    private void OnEndInteraction(object sender, TouchEventArgs e)
    {
        // Touch finished

        PointF touchPoint = new PointF(e.Touches.First().X, e.Touches.First().Y);

        if (CurrentTSFC_Triangle != null)
        {
            CurrentTSFC_Triangle.EndInteraction(touchPoint);
        }

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