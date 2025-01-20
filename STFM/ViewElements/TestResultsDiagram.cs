
namespace STFM.Views
{

    public class TestResultsDiagram : PlotBase, IDrawable
    {

        public TestResultsDiagram(GraphicsView view)
        {
            parentView = view;
            SetupDiagram();
        }

        private void SetupDiagram()
        {

            //Setting up audiogram properties
            PlotAreaRelativeMarginLeft = 0.2F;
            PlotAreaRelativeMarginRight = 0.05F;
            PlotAreaRelativeMarginTop = 0.05F;
            PlotAreaRelativeMarginBottom = 0.1F;

            PlotAreaBorderColor = Colors.DarkGray;
            PlotAreaBorder = true;
            GridLineColor = Colors.Gray;

            //XaxisGridLinePositions = new List<float>() { 1, 2, 3, 4, 5, 6 };
            //XaxisDashedGridLinePositions = new List<float>() { 750, 1500, 3000, 6000 };
            XaxisDrawBottom = true;
            XaxisTickPositions = new List<float>();
            XaxisTickHeight = 2;
            XaxisTextPositions = new List<float>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            XaxisTextValues = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
            XaxisTextSize = 1;

            //YaxisGridLinePositions = new List<float>() { -10, -5, 0, 5, 10 };
            //YaxisDashedGridLinePositions = new List<float>() { -5, 5, 15, 25, 35, 45, 55, 65, 75, 85, 95, 105 };
            YaxisDrawLeft = true;
            YaxisTickPositions = new List<float>();
            YaxisTickWidth = 2;
            YaxisTextPositions = new List<float>() { -10, -5, 0, 5, 10 };
            YaxisTextValues = new string[] { "-10", "-5", "0", "5", "10" };
            YaxisTextSize = 1;

        }


        void IDrawable.Draw(ICanvas canvas, RectF dirtyRect)
        {
            // Calls the base class drawing first
            base.Draw(canvas, dirtyRect);

            //// Continues drawing audiogram objects
            //float PlotAreaMarginLeft = PlotAreaRelativeMarginLeft * dirtyRect.Width;
            //float PlotAreaMarginRight = PlotAreaRelativeMarginRight * dirtyRect.Width;
            //float PlotAreaMarginTop = PlotAreaRelativeMarginTop * dirtyRect.Height;
            //float PlotAreaMarginBottom = PlotAreaRelativeMarginBottom * dirtyRect.Height;
            //float PlotAreaLeft = PlotAreaMarginLeft;
            //float PlotAreaRight = dirtyRect.Width - PlotAreaMarginRight;
            //float PlotAreaBottom = dirtyRect.Height - PlotAreaMarginBottom;
            //float PlotAreaTop = PlotAreaMarginTop;
            //float PlotAreaWidth = dirtyRect.Width - PlotAreaMarginLeft - PlotAreaMarginRight;
            //float PlotAreaHeight = dirtyRect.Height - PlotAreaMarginTop - PlotAreaMarginBottom;

            //RectF PlotAreaRectangle = new RectF(PlotAreaLeft, PlotAreaTop, PlotAreaWidth, PlotAreaHeight);

            //float Xrange = XlimMax - XlimMin;
            //float Yrange = YlimMax - YlimMin;

            //canvas.StrokeColor = Colors.Coral;
            //canvas.StrokeSize = (float)0.005 * PlotAreaHeight;
            //canvas.StrokeDashPattern = null;
            //canvas.DrawLine(PlotAreaLeft, PlotAreaTop, PlotAreaWidth, PlotAreaHeight);

        }
    }
}