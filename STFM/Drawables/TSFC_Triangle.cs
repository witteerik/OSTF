
using Microsoft.Maui;
using static System.Net.Mime.MediaTypeNames;

public class TSFC_Triangle : IDrawable
{

    private GraphicsView parentView; // Reference to the parent GraphicsView

    private SizeF viewportSize = new(1, 1);

    public SizeF ViewportSize
    {
        get { return viewportSize; }
        set
        {
            viewportSize = value;

            UpdateTriangularVertices();

            parentView?.Invalidate(); // Trigger a redraw of the GraphicsView
        }
    }

    private float marginTriangleHeight = 1;
    private float marginTriangleWidth = 1;
    private float visibleTriangleHeight = 1;
    private float visibleTriangleWidth = 1;
    private float centerX = 1;
    private float centerY = 1;
    private float circleRadius = 1;

    private PointF VisibleTriangleVertex_TopLeft;
    private PointF VisibleTriangleVertex_TopRight;
    private PointF VisibleTriangleVertex_BottomMid;

    private PointF MarginTriangleVertex_TopLeft;
    private PointF MarginTriangleVertex_TopRight;
    private PointF MarginTriangleVertex_BottomMid;

    private Color background = Color.FromArgb("#2F2F2F");

    public Color Background
    {
        get { return background; }
        set
        {
            background = value;
            parentView?.Invalidate(); // Trigger a redraw of the GraphicsView
        }
    }


    private bool drawMarginTriangle = false;

    public bool DrawMarginTriangle
    {
        get { return drawMarginTriangle; }
        set
        {
            drawMarginTriangle = value;
            parentView?.Invalidate(); // Trigger a redraw of the GraphicsView
        }
    }

    private float circleSize = 0.09f;


    public float CircleSize
    {
        get { return circleSize; }
        set
        {
            circleSize = value;
            parentView?.Invalidate(); // Trigger a redraw of the GraphicsView
        }
    }

    private PointF circleLocation;

    public PointF CircleLocation
    {
        get { return CircleLocation; }
        set
        {

            PointF TempCircleLocation = value;

            // Checking if the user pointed within the outer (margin) triangle, if not skips changing the location
            if (IsPointInTriangle(TempCircleLocation, MarginTriangleVertex_TopLeft, MarginTriangleVertex_TopRight, MarginTriangleVertex_BottomMid) == false)
            {
                // Exits without updating the circle location
                return;
            }

            // Limit to inner (visible) triangle range 
            var (u, v, w) = GetBarycentricCoordinates(TempCircleLocation, VisibleTriangleVertex_TopLeft, VisibleTriangleVertex_TopRight, VisibleTriangleVertex_BottomMid);

            // -by checking if u or v is below zero and in such cases use only height, placed alow the triangle border (X value determined from the triangle height)
            if (u < 0)
            {
                TempCircleLocation.X = centerX + ((visibleTriangleWidth / 2f) / visibleTriangleHeight) * (centerY + 2 * visibleTriangleHeight / 3 - TempCircleLocation.Y);
            }

            if (v < 0)
            {
                TempCircleLocation.X = centerX - ((visibleTriangleWidth / 2f) / visibleTriangleHeight) * (centerY + 2* visibleTriangleHeight / 3 - TempCircleLocation.Y);
            }

            // -by checking if w is below zero and in such cases limit the Y value to the upper triangle border and uses the unmodified X value
            if (w < 0)
            {
                TempCircleLocation.X = TempCircleLocation.X;
                TempCircleLocation.Y = VisibleTriangleVertex_TopRight.Y;
            }

            // -by checking that no value (u, v or w) is above 1 and in such cases uses the values of the corresponding triangle corner
            if (u > 1)
            {
                TempCircleLocation.X = VisibleTriangleVertex_TopLeft.X;
                TempCircleLocation.Y = VisibleTriangleVertex_TopLeft.Y;
            }

            if (v > 1)
            {
                TempCircleLocation.X = VisibleTriangleVertex_TopRight.X;
                TempCircleLocation.Y = VisibleTriangleVertex_TopRight.Y;
            }

            if (w > 1)
            {
                TempCircleLocation.X = VisibleTriangleVertex_BottomMid.X;
                TempCircleLocation.Y = VisibleTriangleVertex_BottomMid.Y;
            }

            circleLocation = TempCircleLocation;

            parentView?.Invalidate(); // Trigger a redraw of the GraphicsView
        }
    }


    private bool circleIsVisible = false;

    public bool CircleIsVisible
    {
        get { return circleIsVisible; }
        set
        {
            circleIsVisible = value;
            parentView?.Invalidate(); // Trigger a redraw of the GraphicsView
        }
    }

    public TSFC_Triangle(GraphicsView view)
    {
        parentView = view;
    }


    public void StartInteraction(PointF touchPoint)
    {

        CircleLocation = touchPoint;
        CircleIsVisible = true;

    }

    public void DragInteraction(PointF touchPoint)
    {

        CircleLocation = touchPoint;
        //CircleIsVisible = true;

    }


    public void EndInteraction(PointF touchPoint)
    {

        CircleLocation = touchPoint;
        //CircleIsVisible = true;

    }

    public void UpdateTriangularVertices()
    {

        // Dynamically calculate triangle dimensions based on the ViewportSize size
        // Using the minium of width and height as the triangle height
        marginTriangleHeight = Math.Min(ViewportSize.Width, ViewportSize.Height);

        // Calculating the triangle width
        marginTriangleWidth = (2F * marginTriangleHeight) / (float)Math.Sqrt(3F);

        // Calculating the actual circle size based on the CircleSize factor
        circleRadius = marginTriangleHeight * circleSize;

        // Calculating the inner (visible) triangle size
        visibleTriangleHeight = marginTriangleHeight - 3.5f * circleRadius; // making the inner triangle smaller by 3.5 times the radius of the circle (3 would be enough if the circle shadow is not taken into account) 
        visibleTriangleWidth = (2F * visibleTriangleHeight) / (float)Math.Sqrt(3F);

        // Gettting the centre point
        centerX = ViewportSize.Width / 2;
        centerY = ViewportSize.Height / 3; // This is the "center-of-mass" midpoint of the triangles

        // Setting up triangle vertices
        MarginTriangleVertex_TopLeft = new PointF(centerX - marginTriangleWidth / 2, centerY - marginTriangleHeight / 3);
        MarginTriangleVertex_TopRight = new PointF(centerX + marginTriangleWidth / 2, centerY - marginTriangleHeight / 3);
        MarginTriangleVertex_BottomMid = new PointF(centerX, centerY + 2 * marginTriangleHeight / 3);

        VisibleTriangleVertex_TopLeft = new PointF(centerX - visibleTriangleWidth / 2, centerY - visibleTriangleHeight / 3);
        VisibleTriangleVertex_TopRight = new PointF(centerX + visibleTriangleWidth / 2, centerY - visibleTriangleHeight / 3);
        VisibleTriangleVertex_BottomMid = new PointF(centerX, centerY + 2 * visibleTriangleHeight / 3);

    }


    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // Set up the drawing properties
        canvas.StrokeColor = Colors.Black;
        canvas.StrokeSize = 2;

        // Start drawing the arrow path
        PathF trianglePath = new PathF();

        trianglePath.MoveTo(VisibleTriangleVertex_TopLeft);
        trianglePath.LineTo(VisibleTriangleVertex_TopRight);
        trianglePath.LineTo(VisibleTriangleVertex_BottomMid);

        trianglePath.Close();

        // Fill the arrow with color
        canvas.StrokeSize = 0;
        canvas.StrokeLineJoin = LineJoin.Round;

        canvas.FillColor = background;
        canvas.FillPath(trianglePath);
        canvas.SetShadow(new SizeF(0, 0), 10, Colors.Grey);

        // Optional: Add an outline
        canvas.StrokeColor = background;
        canvas.DrawPath(trianglePath);

        if (drawMarginTriangle)
        {
            // Start drawing the arrow path
            PathF marginTrianglePath = new PathF();
            marginTrianglePath.MoveTo(MarginTriangleVertex_TopLeft);
            marginTrianglePath.LineTo(MarginTriangleVertex_TopRight);
            marginTrianglePath.LineTo(MarginTriangleVertex_BottomMid);
            marginTrianglePath.Close();
            canvas.StrokeColor = Colors.Red;
            canvas.StrokeSize = 2;
            canvas.DrawPath(marginTrianglePath);

        }

        // Updating the barycentric coordinates
        var (u, v, w) = GetBarycentricCoordinates(circleLocation, VisibleTriangleVertex_TopLeft, VisibleTriangleVertex_TopRight, VisibleTriangleVertex_BottomMid);

        // Drawing the circle
        if (circleIsVisible)
        {
            float CircleShadowRadius = circleRadius / 3;
            canvas.FillColor = Color.FromArgb("#FFFFAC");
            canvas.StrokeColor = Colors.Yellow;
            canvas.StrokeSize = 2;
            canvas.SetShadow(new SizeF(0, 0), CircleShadowRadius, Colors.Yellow);
            canvas.FillCircle(circleLocation, circleRadius);
            canvas.DrawCircle(circleLocation, circleRadius);

            // Resetting the shadow
            canvas.SetShadow(new SizeF(0, 0), 0, Colors.Transparent);

        }

        // Temporarily drawing also the barycentric coordinates
        string u_v_w_string = "u = " + Math.Round(u, 5).ToString() + "\nv = " + Math.Round(v, 5).ToString() + "\nw = " + Math.Round(w, 5).ToString();
        canvas.FontSize = 14;
        canvas.FontColor = Colors.Black;
        canvas.DrawString(u_v_w_string, centerX * 2f - 100, centerY * 2f - 100, 100, 100, HorizontalAlignment.Left, VerticalAlignment.Top, TextFlow.OverflowBounds);

    }

    /// <summary>
    /// Checks if the barycentric coordinates are within the circle (by testing if all are non-negative)
    /// </summary>
    /// <param name="u"></param>
    /// <param name="v"></param>
    /// <param name="w"></param>
    /// <returns></returns>
    public static bool IsPointInTriangle(double u, double v, double w)
    {
        return (u >= 0) && (v >= 0) && (w >= 0);
    }

    public static bool IsPointInTriangle(PointF p, PointF a, PointF b, PointF c)
    {

        var (u, v, w) = GetBarycentricCoordinates(p, a, b, c);

        return (u >= 0) && (v >= 0) && (w >= 0);
    }

    public static (double u, double v, double w) GetBarycentricCoordinates(PointF p, PointF a, PointF b, PointF c)
    {
        double denominator = ((b.Y - c.Y) * (a.X - c.X) + (c.X - b.X) * (a.Y - c.Y));

        double u = ((b.Y - c.Y) * (p.X - c.X) + (c.X - b.X) * (p.Y - c.Y)) / denominator;
        double v = ((c.Y - a.Y) * (p.X - c.X) + (a.X - c.X) * (p.Y - c.Y)) / denominator;
        double w = 1 - u - v;

        return (u, v, w);
    }

}