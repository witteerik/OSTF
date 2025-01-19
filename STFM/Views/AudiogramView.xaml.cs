using Microsoft.Maui;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;

namespace STFM.Views;

public partial class AudiogramView : ContentView
{
	public AudiogramView()
	{
		InitializeComponent();

        //MyDrawable.PlotAreaRelativeMarginLeft = 0;

    }

    public Audiogram Audiogram { 
        get
        {
            return MyAudiogram;
        }
        set
        {
            Audiogram = value;
        } 
    }

}


public class Audiogram : PlotBase, IDrawable
{

    public Audiogram()
    {
        SetupAudiogram();
    }

    private void SetupAudiogram()
    {

        //Setting up audiogram properties
        PlotAreaRelativeMarginLeft = 0.1F;
        PlotAreaRelativeMarginRight = 0.1F;
        PlotAreaRelativeMarginTop = 0.1F;
        PlotAreaRelativeMarginBottom = 0.05F;

        XlimMin = 125;
        XlimMax = 8000;

        Xlog = true;
        XlogBase = 2;

        YlimMin = -10;
        YlimMax = 110;
        Yreversed = true;
        Ylog = false;
        YlogBase = 10;

        PlotAreaBorderColor = Colors.DarkGray;
        PlotAreaBorder = true;
        GridLineColor = Colors.Gray;

        XaxisGridLinePositions = new List<float>() { 125, 250, 500, 1000, 2000, 4000 };
        XaxisDashedGridLinePositions = new List<float>() { 750, 1500, 3000, 6000 };
        XaxisDrawTop = true;
        XaxisDrawBottom = false;
        XaxisTickPositions = new List<float>();
        XaxisTickHeight = 2;
        XaxisTextPositions = new List<float>() { 125, 250, 500, 1000, 2000, 4000, 8000 };
        XaxisTextValues = new string[] { "125", "250", "500", "1k", "2k", "4k", "8k" };
        XaxisTextSize = 1;

        YaxisGridLinePositions = new List<float>() { 0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
        YaxisDashedGridLinePositions = new List<float>() { -5, 5, 15, 25, 35, 45, 55, 65, 75, 85, 95, 105 };
        YaxisDrawLeft = true;
        YaxisDrawRight = true;
        YaxisTickPositions = new List<float>();
        YaxisTickWidth = 2;
        YaxisTextPositions = new List<float>() { 0, 20, 40, 60, 80, 100 };
        YaxisTextValues = new string[] { "0", "20", "40", "60", "80", "100" };
        YaxisTextSize = 1;

    }


    void IDrawable.Draw(ICanvas canvas, RectF dirtyRect)
    {
        // Calls the base class drawing first
        base.Draw(canvas, dirtyRect);

        // Continues drawing audiogram objects
        float PlotAreaMarginLeft = PlotAreaRelativeMarginLeft * dirtyRect.Width;
        float PlotAreaMarginRight = PlotAreaRelativeMarginRight * dirtyRect.Width;
        float PlotAreaMarginTop = PlotAreaRelativeMarginTop * dirtyRect.Height;
        float PlotAreaMarginBottom = PlotAreaRelativeMarginBottom * dirtyRect.Height;
        float PlotAreaLeft = PlotAreaMarginLeft;
        float PlotAreaRight = dirtyRect.Width - PlotAreaMarginRight;
        float PlotAreaBottom = dirtyRect.Height - PlotAreaMarginBottom;
        float PlotAreaTop = PlotAreaMarginTop;
        float PlotAreaWidth = dirtyRect.Width - PlotAreaMarginLeft - PlotAreaMarginRight;
        float PlotAreaHeight = dirtyRect.Height - PlotAreaMarginTop - PlotAreaMarginBottom;

        RectF PlotAreaRectangle = new RectF(PlotAreaLeft, PlotAreaTop, PlotAreaWidth, PlotAreaHeight);

        float Xrange = XlimMax - XlimMin;
        float Yrange = YlimMax - YlimMin;

        canvas.StrokeColor =  Colors.Coral;
        canvas.StrokeSize = (float)0.005 * PlotAreaHeight;
        canvas.StrokeDashPattern = null;
        canvas.DrawLine(PlotAreaLeft, PlotAreaTop, PlotAreaWidth, PlotAreaHeight);

    }
}


