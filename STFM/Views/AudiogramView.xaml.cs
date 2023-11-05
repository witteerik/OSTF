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

public class PlotBase : IDrawable
{

    public List<STFM.Views.Area> Areas { get; set; } = new List<Area>();
    public List<Line> Lines { get; set; } = new List<Line>();
    public List<PointSerie> PointSeries { get; set; } = new List<PointSerie>();

    public float PlotAreaRelativeMarginLeft = 0.1F;
    public float PlotAreaRelativeMarginRight = 0.1F;
    public float PlotAreaRelativeMarginTop = 0.1F;
    public float PlotAreaRelativeMarginBottom = 0.1F;

    protected float XlimMin = 0;
    protected float XlimMax = 10;

    protected bool Xlog = false;
    protected float XlogBase = 2;

    protected float YlimMin = 0;
    protected float YlimMax = 10;

    protected bool Yreversed = false;
    protected bool Ylog = false;
    protected float YlogBase = 10;

    protected Microsoft.Maui.Graphics. Color PlotAreaBorderColor = Colors.DarkGray;
    protected bool PlotAreaBorder = true;
    protected Microsoft.Maui.Graphics.Color GridLineColor = Colors.Gray;
    protected Microsoft.Maui.Graphics.Color DashedGridLineColor = Colors.Gray;
    protected List<float> XaxisGridLinePositions = new List<float>();
    protected List<float> XaxisDashedGridLinePositions = new List<float>();
    protected bool XaxisDrawTop = false;
    protected bool XaxisDrawBottom = true;
    protected List<float> XaxisTickPositions = new List<float>();
    protected float XaxisTickHeight = 2;
    protected List<float> XaxisTextPositions = new List<float>();
    protected string[] XaxisTextValues;
    protected float XaxisTextSize = 1;

    protected List<float> YaxisGridLinePositions = new List<float>();
    protected List<float> YaxisDashedGridLinePositions = new List<float>();
    protected bool YaxisDrawLeft = true;
    protected bool YaxisDrawRight = false;
    protected List<float> YaxisTickPositions = new List<float>();
    protected float YaxisTickWidth = 2;
    protected List<float> YaxisTextPositions = new List<float>();
    protected string[] YaxisTextValues;
    protected float YaxisTextSize = 1;

    public PlotBase()
    {
        //AddSomeLinesAndPoints();
    }

    private void AddSomeLinesAndPoints()
    {

        Yreversed = false;

        XaxisDrawTop = true; ;
        XaxisDrawBottom = true;
        YaxisDrawLeft = true;
        YaxisDrawRight = true;


        // Adding some gridlines
        //XaxisGridLinePositions = new List<float>() { 0F, 2F, 4F, 6F, 8F, 10F };
        //XaxisDashedGridLinePositions = new List<float>() { 1F, 3F, 5F, 7F, 9F };
        XaxisTickPositions = new List<float>() { 1.5F, 3.5F, 5.5F, 7.5F, 9.5F };

        //YaxisGridLinePositions = new List<float>() { 0F, 2F, 4F, 6F, 8F, 10F };
        //YaxisDashedGridLinePositions = new List<float>() { 1F, 3F, 5F, 7F, 9F };
        YaxisTickPositions = new List<float>() { 1.5F, 3.5F, 5.5F, 7.5F, 9.5F };

        // Adding some PointSeries
        PointSeries.Add(new PointSerie() { Color = Colors.Red, PointSize = 2, Type = PointSerie.PointTypes.Cross, XValues = new[] { 1F, 2F, 3F, 4F }, YValues = new[] { 1F,2F, 3F, 4F } });
        PointSeries.Add(new PointSerie() { Color = Colors.Red, PointSize = 2, Type = PointSerie.PointTypes.FilledCircle, XValues = new[] { 1F, 2F, 3F, 4F }, YValues = new[] { 2F, 3F, 4F, 5F } });
        PointSeries.Add(new PointSerie() { Color = Colors.Red, PointSize = 2, Type = PointSerie.PointTypes.FilledRectangle, XValues = new[] { 1F, 2F, 3F, 4F }, YValues = new[] { 3F, 4F, 5F, 6F } });
        PointSeries.Add(new PointSerie() { Color = Colors.Red, PointSize = 2, Type = PointSerie.PointTypes.OpenCircle, XValues = new[] { 1F, 2F, 3F, 4F }, YValues = new[] { 4F, 5F, 6F, 7F } });
        PointSeries.Add(new PointSerie() { Color = Colors.Red, PointSize = 2, Type = PointSerie.PointTypes.Rectangle, XValues = new[] { 1F, 2F, 3F, 4F }, YValues = new[] { 5F, 6F, 7F, 8F } });

        //Areas.Add(new Area() { Color = Colors.Yellow, XValues = new[] { 1F, 2F, 3F, 4F, 5F, 6F }, YValuesUpper = new[] { 0.42F, 0.83F, 0.44F, 0.43F, 0.42F, 0.41F }, YValuesLower = new[] { 0.42F, 0.57F, 0.51F, 0.41F, 0.41F, 0.4F } });
        //Areas.Add(new Area() { Color = Colors.Blue, XValues = new[] { 1F, 2F, 3F, 4F, 5F, 6F }, YValuesUpper = new[] { 0.32F, 0.53F, 0.64F, 0.63F, 0.62F, 0.61F }, YValuesLower = new[] { 0.42F, 0.57F, 0.51F, 0.71F, 0.71F, 0.7F } });
        Areas.Add(new Area() { Color = Colors.Turquoise, XValues = new[] { 1F, 2F, 3F, 4F }, YValuesLower = new[] { 2F, 2F, 2F, 1F }, YValuesUpper = new[] { 1.5F, 2F, 3F, 4F } });

        Lines.Add(new Line() { Color = Colors.Green, Dashed = true, DashPattern = new[] { 2F, 3F }, LineWidth = 1, XValues = new[] { 1F, 2F, 3F, 4F }, YValues = new[] { 1F, 2F, 3F, 4F } });
        Lines.Add(new Line() { Color = Colors.Green, Dashed = true, DashPattern = new[] { 2F, 3F }, LineWidth = 2, XValues = new[] { 1F, 2F, 3F, 4F }, YValues = new[] { 2F, 3F, 4F, 5F } });
        Lines.Add(new Line() { Color = Colors.Green, Dashed = false, DashPattern = null, LineWidth = 2, XValues = new[] { 1F, 2F, 3F, 4F }, YValues = new[] { 3F, 4F, 5F, 6F } });

        XaxisTextPositions = new List<float> { 1,2,3};
        XaxisTextValues = new[] { "1","2","3"};

        YaxisTextPositions = new List<float> { 1, 2, 3 };
        YaxisTextValues = new[] { "1", "2", "3" };

    }


    private float getBase_n_Log(float value, float n = 2)
    {
        return (float)(Math.Log10(value) / Math.Log10(n));
    }


    public float XValueToCoordinate(float x, float PlotAreaLeft,  float PlotAreaWidth, float Xrange)
    {
            float Output;

            if (Xlog == false)
                Output = PlotAreaLeft + ((x - XlimMin) / Xrange) * PlotAreaWidth;
            else
            {
                // Overriding Xmin to avoid log of non-positive values
                float LimXmin = getBase_n_Log(Math.Max(float.Epsilon, XlimMin), XlogBase);
                float LimXmax = getBase_n_Log(Math.Max(float.Epsilon, XlimMax), XlogBase);
                float LimXrange = LimXmax - LimXmin;

                Output = PlotAreaLeft + ((getBase_n_Log(x, XlogBase) - LimXmin) / LimXrange) * PlotAreaWidth;
            }

            return Output;
        }

    public float CoordinateToXValue(float Coordinate,float PlotAreaLeft, float PlotAreaWidth, float Xrange)
    {
            float x;

            if (Xlog == false)
                x = XlimMin + ((Coordinate - PlotAreaLeft) / PlotAreaWidth) * Xrange;
            else
            {
                // Overriding Xmin to avoid log of non-positive values
                float LimXmin = getBase_n_Log(Math.Max(float.Epsilon, XlimMin), XlogBase);
                float LimXmax = getBase_n_Log(Math.Max(float.Epsilon, XlimMax), XlogBase);
                float LimXrange = LimXmax - LimXmin;

                x =(float)( Math.Pow(XlogBase, (LimXmin + ((Coordinate - PlotAreaLeft) / PlotAreaWidth) * LimXrange)));
            }

            return x;
    }


    public float YValueToCoordinate(float y, float PlotAreaTop, float PlotAreaBottom, float PlotAreaHeight, float Yrange)
    {
            float Output;

            if (Yreversed == true)
            {
                if (Ylog == false)
                    Output = PlotAreaTop + ((y - YlimMin) / Yrange) * PlotAreaHeight;
                else
                {
                    // Overriding Ymin to avoid log of non-positive values
                    float LimYmin = getBase_n_Log(Math.Max(float.Epsilon, YlimMin), YlogBase);
                    float LimYmax = getBase_n_Log(Math.Max(float.Epsilon, YlimMax), YlogBase);
                    float LimYrange = LimYmax - LimYmin;

                    Output = PlotAreaTop + ((getBase_n_Log(y, YlogBase) - LimYmin) / LimYrange) * PlotAreaHeight;
                }
            }
            else if (Ylog == false)
                Output = PlotAreaBottom - ((y - YlimMin) / Yrange) * PlotAreaHeight;
            else
            {
                // Overriding Ymin to avoid log of non-positive values
                float LimYmin = getBase_n_Log(Math.Max(float.Epsilon, YlimMin), YlogBase);
                float LimYmax = getBase_n_Log(Math.Max(float.Epsilon, YlimMax), YlogBase);
                float LimYrange = LimYmax - LimYmin;

                Output = PlotAreaBottom - ((getBase_n_Log(y, YlogBase) - LimYmin) / LimYrange) * PlotAreaHeight;
            }

            return Output;
    }

    public float CoordinateToYValue(float Coordinate, float PlotAreaTop, float PlotAreaBottom, float PlotAreaHeight, float Yrange)
    {
            float y;

            if (Yreversed == true)
            {
                if (Ylog == false)
                    y = YlimMin + ((Coordinate - PlotAreaTop) / PlotAreaHeight) * Yrange;
                else
                {
                    // Overriding Ymin to avoid log of non-positive values
                    float LimYmin = getBase_n_Log(Math.Max(float.Epsilon, YlimMin), YlogBase);
                    float LimYmax = getBase_n_Log(Math.Max(float.Epsilon, YlimMax), YlogBase);
                    float LimYrange = LimYmax - LimYmin;

                    y = (float)(Math.Pow(YlogBase, (LimYmin + ((Coordinate - PlotAreaTop) / PlotAreaHeight) * LimYrange)));
                }
            }
            else if (Ylog == false)
                y = YlimMin + ((PlotAreaBottom - Coordinate) / PlotAreaHeight) * Yrange;
            else
            {
                // Overriding Ymin to avoid log of non-positive values
                float LimYmin = getBase_n_Log(Math.Max(float.Epsilon, YlimMin), YlogBase);
                float LimYmax = getBase_n_Log(Math.Max(float.Epsilon, YlimMax), YlogBase);
                float LimYrange = LimYmax - LimYmin;

                y = (float)(Math.Pow(YlogBase, (LimYmin + ((PlotAreaBottom - Coordinate) / PlotAreaHeight) * LimYrange)));
            }

            return y;
    }



    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
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

        // DrawPlotAreaBorder
        if (PlotAreaBorder == true)
        {
            canvas.StrokeColor = PlotAreaBorderColor;
            canvas.StrokeSize = (float)0.005 * PlotAreaHeight;
            canvas.StrokeDashPattern = null;
            canvas.DrawRectangle(PlotAreaRectangle);
        }

        // DrawVerticalGridLines
        float LineWidth = (float)0.003 * PlotAreaWidth;

        foreach (int l in XaxisGridLinePositions)
        {
            canvas.StrokeColor = GridLineColor;
            canvas.StrokeSize = LineWidth;
            canvas.StrokeDashPattern = null;

            float x = XValueToCoordinate(l,PlotAreaLeft, PlotAreaWidth, Xrange);
            if (x >= PlotAreaLeft && x <= PlotAreaRight)
            {
                canvas.DrawLine(x, PlotAreaBottom, x, PlotAreaTop);
            }
        }

        if (XaxisDashedGridLinePositions.Count > 0)
        {
            canvas.StrokeColor = DashedGridLineColor;
            canvas.StrokeSize = LineWidth;
            canvas.StrokeDashPattern = new[] { (float)LineWidth * 3, (float)LineWidth * 2};

        foreach (int l in XaxisDashedGridLinePositions)
            {
                float x = XValueToCoordinate(l, PlotAreaLeft, PlotAreaWidth, Xrange);
                if (x >= PlotAreaLeft && x <= PlotAreaRight)
                {
                    canvas.DrawLine(x, PlotAreaBottom, x, PlotAreaTop);
                }
            }
        }

        // DrawHorizontalGridLines
        LineWidth = (float)0.003 * PlotAreaHeight ;
        foreach (int l in YaxisGridLinePositions)
        {
            canvas.StrokeColor = GridLineColor;
            canvas.StrokeSize = LineWidth;
            canvas.StrokeDashPattern = null;

            float y = YValueToCoordinate(l,PlotAreaTop,PlotAreaBottom, PlotAreaHeight, Yrange);
            if (y >= PlotAreaTop && y <= PlotAreaBottom)
            {
                canvas.DrawLine(PlotAreaLeft, y, PlotAreaRight, y);
            }
        }

        if (YaxisDashedGridLinePositions.Count > 0)
        {
            canvas.StrokeColor = DashedGridLineColor;
            canvas.StrokeSize = LineWidth;
            canvas.StrokeDashPattern = new[] { (float)LineWidth * 3, (float)LineWidth * 2 };

            foreach (int l in YaxisDashedGridLinePositions)
            {
                float y = YValueToCoordinate(l, PlotAreaTop, PlotAreaBottom, PlotAreaHeight, Yrange);
                if (y >= PlotAreaTop && y <= PlotAreaBottom)
                {
                    canvas.DrawLine(PlotAreaLeft, y, PlotAreaRight, y);
                }
            }
        }

        // DrawXaxisTicks
        float ProportionOfPlotWidth = (float)0.003 * PlotAreaWidth;
        float TickExtension = XaxisTickHeight * ProportionOfPlotWidth;

        if (XaxisDrawTop == true)
        {
            foreach (int l in XaxisTickPositions)
            {
                canvas.StrokeColor = GridLineColor;
                canvas.StrokeSize = ProportionOfPlotWidth;
                canvas.StrokeDashPattern = null;

                float x = XValueToCoordinate(l, PlotAreaLeft, PlotAreaWidth, Xrange);
                if (x >= PlotAreaLeft && x <= PlotAreaRight)
                {
                    canvas.DrawLine(x, PlotAreaTop - TickExtension, x, PlotAreaTop + TickExtension);
                }
            }
        }

        if (XaxisDrawBottom == true)
        {
            foreach (int l in XaxisTickPositions)
            {
                canvas.StrokeColor = GridLineColor;
                canvas.StrokeSize = ProportionOfPlotWidth;
                canvas.StrokeDashPattern = null;

                float x = XValueToCoordinate(l, PlotAreaLeft, PlotAreaWidth, Xrange);
                if (x >= PlotAreaLeft && x <= PlotAreaRight)
                {
                    canvas.DrawLine(x, PlotAreaBottom - TickExtension, x, PlotAreaBottom + TickExtension);
                }
            }
        }

        // DrawYaxisTicks
        float ProportionOfPlotHeight = (float)0.003 * PlotAreaHeight;
        TickExtension = YaxisTickWidth * ProportionOfPlotHeight;

        if (YaxisDrawLeft == true)
        {
            foreach (int l in YaxisTickPositions)
            {
                canvas.StrokeColor = GridLineColor;
                canvas.StrokeSize = ProportionOfPlotHeight;
                canvas.StrokeDashPattern = null;

                float y = YValueToCoordinate(l,PlotAreaTop, PlotAreaBottom, PlotAreaHeight, Yrange);
                if (y >= PlotAreaTop && y <= PlotAreaBottom)
                {
                    canvas.DrawLine(PlotAreaLeft - TickExtension, y, PlotAreaLeft + TickExtension, y);
                }
            }
        }

        if (YaxisDrawRight == true)
        {
            foreach (int l in YaxisTickPositions)
            {
                canvas.StrokeColor = GridLineColor;
                canvas.StrokeSize = ProportionOfPlotHeight;
                canvas.StrokeDashPattern = null;

                float y = YValueToCoordinate(l, PlotAreaTop, PlotAreaBottom, PlotAreaHeight, Yrange);
                if (y >= PlotAreaTop && y <= PlotAreaBottom)
                {
                    canvas.DrawLine(PlotAreaRight - TickExtension, y, PlotAreaRight+ TickExtension, y);
                }
            }
        }

        // DrawXaxisText
        if ((XaxisTextValues is null) == false)
        {
            int DrawCount = Math.Min(XaxisTextPositions.Count, XaxisTextValues.Length);

            // Skipping to set font, 
            // canvas.Font = new Microsoft.Maui.Graphics.Font('Arial');
            //canvas.Font = Microsoft.Maui.Graphics.Font.DefaultBold;
            canvas.Font = Microsoft.Maui.Graphics.Font.Default;
            canvas.FontColor = Colors.Black;
            float currentXaxisTextSize = (float)0.05 * PlotAreaHeight * XaxisTextSize;
            float halfCurrentXaxisTextSize = currentXaxisTextSize / 2;
            canvas.FontSize = currentXaxisTextSize;

            if (XaxisDrawTop == true)
            {
                float y = PlotAreaMarginTop / 2;
                float XaxisTextWidth = dirtyRect.Width / DrawCount;
                float halfXaxisTextWidth = XaxisTextWidth / 2;
                for (int n = 0; n < DrawCount; n++)
                {
                    float x = XValueToCoordinate(XaxisTextPositions[n], PlotAreaLeft, PlotAreaWidth, Xrange);
                    canvas.DrawString(XaxisTextValues[n], x - halfXaxisTextWidth, y - halfCurrentXaxisTextSize, XaxisTextWidth, currentXaxisTextSize, HorizontalAlignment.Center, VerticalAlignment.Center, TextFlow.OverflowBounds);
                }
            }

            if (XaxisDrawBottom == true)
            {
                float y = PlotAreaBottom + PlotAreaMarginBottom / 2;
                for (int n = 0; n < DrawCount; n++)
                {
                    float x = XValueToCoordinate(XaxisTextPositions[n], PlotAreaLeft, PlotAreaWidth, Xrange);
                    canvas.DrawString(XaxisTextValues[n], x - halfCurrentXaxisTextSize, y - halfCurrentXaxisTextSize, currentXaxisTextSize, currentXaxisTextSize, HorizontalAlignment.Center, VerticalAlignment.Center, TextFlow.OverflowBounds);
                }
            }


        }

        // DrawYaxisText
        if ((YaxisTextValues is null) == false)
        {
            int DrawCount = Math.Min(YaxisTextPositions.Count, YaxisTextValues.Length);

            // Skipping to set font, 
            // canvas.Font = new Microsoft.Maui.Graphics.Font('Arial');
            //canvas.Font = Microsoft.Maui.Graphics.Font.DefaultBold;
            canvas.Font = Microsoft.Maui.Graphics.Font.Default;
            canvas.FontColor = Colors.Black;
            float currentYaxisTextSize = (float)0.05 * PlotAreaHeight * YaxisTextSize;
            float halfCurrentYaxisTextSize = currentYaxisTextSize / 2;
            canvas.FontSize = currentYaxisTextSize;

            if (YaxisDrawLeft == true)
            {
                float x = PlotAreaMarginLeft / 2;
                for (int n = 0; n < DrawCount; n++)
                {
                    float y = YValueToCoordinate(YaxisTextPositions[n], PlotAreaTop, PlotAreaBottom, PlotAreaHeight, Yrange);
                    canvas.DrawString(YaxisTextValues[n], x - halfCurrentYaxisTextSize, y - halfCurrentYaxisTextSize, currentYaxisTextSize, currentYaxisTextSize, HorizontalAlignment.Center, VerticalAlignment.Center, TextFlow.OverflowBounds);
                }
            }

            if (YaxisDrawRight == true)
            {
                float x = PlotAreaRight + PlotAreaMarginRight / 2;
                for (int n = 0; n < DrawCount; n++)
                {
                    float y = YValueToCoordinate(YaxisTextPositions[n], PlotAreaTop, PlotAreaBottom, PlotAreaHeight, Yrange);
                    canvas.DrawString(YaxisTextValues[n], x - halfCurrentYaxisTextSize, y - halfCurrentYaxisTextSize, currentYaxisTextSize, currentYaxisTextSize, HorizontalAlignment.Center, VerticalAlignment.Center, TextFlow.OverflowBounds);
                }
            }


        }


        // DrawAreas
        foreach (STFM.Views.Area currentArea in  Areas)
        {

            int DrawLength = Math.Min(currentArea.XValues.Length, Math.Min(currentArea.YValuesUpper.Length, currentArea.YValuesUpper.Length));

            List< Microsoft.Maui.Graphics.Point> NewPoints = new List<Microsoft.Maui.Graphics.Point>();

            // Creating a path around the area
            // Adding the upper limit
            for (int n = 0; n <= DrawLength - 1; n++)
                {
                    if (float.IsNaN(currentArea.YValuesUpper[n]) == true)
                        continue;
                    NewPoints.Add(new Microsoft.Maui.Graphics.Point(
                        XValueToCoordinate(currentArea.XValues[n],PlotAreaLeft, PlotAreaWidth,Xrange), 
                        YValueToCoordinate(currentArea.YValuesUpper[n], PlotAreaTop,PlotAreaBottom,PlotAreaHeight,Yrange)));
                }

                // Adding the lower limit in descending x-axis order
                for (int n = DrawLength - 1; n >= 0; n += -1)
                {
                    if (float.IsNaN(currentArea.YValuesLower[n]) == true)
                        continue;
                    NewPoints.Add(new Microsoft.Maui.Graphics.Point(
                        XValueToCoordinate(currentArea.XValues[n], PlotAreaLeft, PlotAreaWidth, Xrange), 
                        YValueToCoordinate(currentArea.YValuesLower[n], PlotAreaTop, PlotAreaBottom, PlotAreaHeight, Yrange)));
                }

            //Creating the path
            if (NewPoints.Count > 0)
                canvas.FillColor = Microsoft.Maui.Graphics.Color.FromRgba(currentArea.Color.Red, currentArea.Color.Green, currentArea.Color.Blue, currentArea.Alpha);
            {
                PathF pathF = new PathF();
                pathF.MoveTo(NewPoints[0]);
                for (int i = 1; i < NewPoints.Count; i++)
                {
                    pathF.LineTo(NewPoints[i]);
                }
                pathF.Close();
                canvas.FillPath(pathF);
            }

        }

        // DrawPointSeries
        foreach (PointSerie PS in PointSeries)
        {

            float CurrentSize = (float)PS.PointSize * (0.005F * PlotAreaHeight + 0.005F * PlotAreaWidth);
            float HalfCurrentSize = CurrentSize / 2F;
            float DrawLength = Math.Min(PS.XValues.Length, PS.YValues.Length);

            canvas.StrokeColor = PS.Color;
            canvas.StrokeSize = CurrentSize;
            canvas.StrokeDashPattern = null;

            if (Yreversed == false)
            {
                switch (PS.Type)
                {
                    case PointSerie.PointTypes.Cross:
                        for (int n = 0; n < DrawLength; n++)
                        {
                            if (float.IsNaN(PS.YValues[n])){continue;}
                            float CurrentX = XValueToCoordinate(PS.XValues[n], PlotAreaLeft, PlotAreaWidth, Xrange);
                            float CurrentY = YValueToCoordinate(PS.YValues[n], PlotAreaTop, PlotAreaBottom, PlotAreaHeight, Yrange);
                            canvas.DrawLine(CurrentX - HalfCurrentSize, CurrentY - HalfCurrentSize, CurrentX + HalfCurrentSize, CurrentY + HalfCurrentSize);
                            canvas.DrawLine(CurrentX - HalfCurrentSize, CurrentY + HalfCurrentSize, CurrentX + HalfCurrentSize, CurrentY - HalfCurrentSize);
                        }
                        break;

                    case PointSerie.PointTypes.FilledCircle:
                        for (int n = 0; n < DrawLength; n++)
                        {
                            if (float.IsNaN(PS.YValues[n])) { continue; }
                            float CurrentX = XValueToCoordinate(PS.XValues[n] , PlotAreaLeft, PlotAreaWidth, Xrange);
                            float CurrentY = YValueToCoordinate(PS.YValues[n] , PlotAreaTop, PlotAreaBottom, PlotAreaHeight, Yrange);
                            canvas.FillEllipse(CurrentX - HalfCurrentSize, CurrentY - HalfCurrentSize, CurrentSize, CurrentSize);
                        }
                        break;

                    case PointSerie.PointTypes.OpenCircle:
                        for (int n = 0; n < DrawLength; n++)
                        {
                            if (float.IsNaN(PS.YValues[n])) { continue; }
                            float CurrentX = XValueToCoordinate(PS.XValues[n] , PlotAreaLeft, PlotAreaWidth, Xrange);
                            float CurrentY = YValueToCoordinate(PS.YValues[n] , PlotAreaTop, PlotAreaBottom, PlotAreaHeight, Yrange);
                            canvas.DrawArc(CurrentX - HalfCurrentSize, CurrentY - HalfCurrentSize, CurrentSize, CurrentSize,0,360,true,true);
                        }
                        break;

                    case PointSerie.PointTypes.Rectangle:
                        for (int n = 0; n < DrawLength; n++)
                        {
                            if (float.IsNaN(PS.YValues[n])) { continue; }
                            float CurrentX = XValueToCoordinate(PS.XValues[n] , PlotAreaLeft, PlotAreaWidth, Xrange);
                            float CurrentY = YValueToCoordinate(PS.YValues[n] , PlotAreaTop, PlotAreaBottom, PlotAreaHeight, Yrange);
                            canvas.DrawRectangle(CurrentX - HalfCurrentSize, CurrentY - HalfCurrentSize, CurrentSize, CurrentSize);
                        }
                        break;
 
                    case PointSerie.PointTypes.FilledRectangle:
                        for (int n = 0; n < DrawLength; n++)
                        {
                            if (float.IsNaN(PS.YValues[n])) { continue; }
                            float CurrentX = XValueToCoordinate(PS.XValues[n] , PlotAreaLeft, PlotAreaWidth, Xrange);
                            float CurrentY = YValueToCoordinate(PS.YValues[n] , PlotAreaTop, PlotAreaBottom, PlotAreaHeight, Yrange);
                            canvas.FillRectangle(CurrentX - HalfCurrentSize, CurrentY - HalfCurrentSize, CurrentSize, CurrentSize);
                        }
                        break;
 
                    default:
                        break;
                }
            }
            else
            {
                switch (PS.Type)
                {
                    case PointSerie.PointTypes.Cross:
                        for (int n = 0; n < DrawLength; n++)
                        {
                            if (float.IsNaN(PS.YValues[n])) { continue; }
                            float CurrentX = XValueToCoordinate(PS.XValues[n], PlotAreaLeft, PlotAreaWidth, Xrange);
                            float CurrentY = YValueToCoordinate(PS.YValues[n], PlotAreaTop, PlotAreaBottom, PlotAreaHeight, Yrange);
                            canvas.DrawLine(CurrentX - HalfCurrentSize, CurrentY + HalfCurrentSize, CurrentX + HalfCurrentSize, CurrentY - HalfCurrentSize);
                            canvas.DrawLine(CurrentX - HalfCurrentSize, CurrentY - HalfCurrentSize, CurrentX + HalfCurrentSize, CurrentY + HalfCurrentSize);
                        }
                        break;

                    case PointSerie.PointTypes.FilledCircle:
                        for (int n = 0; n < DrawLength; n++)
                        {
                            if (float.IsNaN(PS.YValues[n])) { continue; }
                            float CurrentX = XValueToCoordinate(PS.XValues[n], PlotAreaLeft, PlotAreaWidth, Xrange);
                            float CurrentY = YValueToCoordinate(PS.YValues[n], PlotAreaTop, PlotAreaBottom, PlotAreaHeight, Yrange);
                            canvas.FillEllipse(CurrentX - HalfCurrentSize, CurrentY - HalfCurrentSize, CurrentSize, CurrentSize);
                        }
                        break;

                    case PointSerie.PointTypes.OpenCircle:
                        for (int n = 0; n < DrawLength; n++)
                        {
                            if (float.IsNaN(PS.YValues[n])) { continue; }
                            float CurrentX = XValueToCoordinate(PS.XValues[n], PlotAreaLeft, PlotAreaWidth, Xrange);
                            float CurrentY = YValueToCoordinate(PS.YValues[n], PlotAreaTop, PlotAreaBottom, PlotAreaHeight, Yrange);
                            canvas.DrawArc(CurrentX - HalfCurrentSize, CurrentY - HalfCurrentSize, CurrentSize, CurrentSize, 0, 360, true, true);
                        }
                        break;

                    case PointSerie.PointTypes.Rectangle:
                        for (int n = 0; n < DrawLength; n++)
                        {
                            if (float.IsNaN(PS.YValues[n])) { continue; }
                            float CurrentX = XValueToCoordinate(PS.XValues[n], PlotAreaLeft, PlotAreaWidth, Xrange);
                            float CurrentY = YValueToCoordinate(PS.YValues[n], PlotAreaTop, PlotAreaBottom, PlotAreaHeight, Yrange);
                            canvas.DrawRectangle(CurrentX - HalfCurrentSize, CurrentY - HalfCurrentSize, CurrentSize, CurrentSize);
                        }
                        break;

                    case PointSerie.PointTypes.FilledRectangle:
                        for (int n = 0; n < DrawLength; n++)
                        {
                            if (float.IsNaN(PS.YValues[n])) { continue; }
                            float CurrentX = XValueToCoordinate(PS.XValues[n], PlotAreaLeft, PlotAreaWidth, Xrange);
                            float CurrentY = YValueToCoordinate(PS.YValues[n], PlotAreaTop, PlotAreaBottom, PlotAreaHeight, Yrange);
                            canvas.FillRectangle(CurrentX - HalfCurrentSize, CurrentY - HalfCurrentSize, CurrentSize, CurrentSize);
                        }
                        break;

                    default:
                        break;
                }


            }

        }

        // DrawLines
        foreach (Line line in Lines)
        {

            float CurrentSize = (float)line.LineWidth * (0.001F * PlotAreaHeight + 0.001F * PlotAreaWidth);
            float DrawLength = Math.Min(line.XValues.Length, line.YValues.Length);

            canvas.StrokeColor = line.Color;
            canvas.StrokeSize = CurrentSize;
            canvas.StrokeDashPattern = null;

            if (line.Dashed == true)
            {
                List<float> TempDashPattern = new List<float>();
                if ((line.DashPattern is null) == false)
                {
                    foreach (float p in line.DashPattern)
                    {
                        TempDashPattern.Add(p * CurrentSize);
                    }
                    canvas.StrokeDashPattern = TempDashPattern.ToArray();
                }
            }

            for (int n = 0; n < (DrawLength-1); n++)
            {
                if (float.IsNaN(line.YValues[n])) { continue; }
                canvas.DrawLine(
                    XValueToCoordinate(line.XValues[n], PlotAreaLeft, PlotAreaWidth, Xrange),
                    YValueToCoordinate(line.YValues[n],PlotAreaTop,PlotAreaBottom, PlotAreaHeight,Yrange),
                    XValueToCoordinate(line.XValues[n + 1], PlotAreaLeft, PlotAreaWidth, Xrange),
                    YValueToCoordinate(line.YValues[n + 1], PlotAreaTop, PlotAreaBottom, PlotAreaHeight, Yrange));
            }

        }



    }
}

public class Area
{
    public float[] YValuesUpper { get; set; }
    public float[] YValuesLower { get; set; }
    public float[] XValues { get; set; }
    public Microsoft.Maui.Graphics.Color Color { get; set; } = Colors.Yellow;
    public byte Alpha { get; set; } = 40;
}

public class Line
{
    public float[] YValues { get; set; }
    public float[] XValues { get; set; }
    public float LineWidth { get; set; } = 1;
    public Microsoft.Maui.Graphics.Color Color { get; set; } = Colors.Black;
    public bool Dashed { get; set; } = false;
    public float[] DashPattern { get; set; } = new[] { (float)3, (float)2 };
}

public class PointSerie
{
    public float[] YValues { get; set; }
    public float[] XValues { get; set; }
    public Microsoft.Maui.Graphics.Color Color { get; set; } = Colors.Black;
    public PointTypes Type { get; set; } = PointTypes.FilledCircle;
    public float PointSize { get; set; } = 2;
    public enum PointTypes
    {
        OpenCircle,
        FilledCircle,
        Cross,
        Rectangle,
        FilledRectangle
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


