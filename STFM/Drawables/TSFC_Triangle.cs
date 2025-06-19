
public class TSFC_Triangle : IDrawable
{

    private GraphicsView parentView; // Reference to the parent GraphicsView


    public Color background = Color.FromArgb("#2F2F2F");

    public Color Background
    {
        get { return background; }
        set
        {
            background = value;
            parentView?.Invalidate(); // Trigger a redraw of the GraphicsView
        }
    }

    public TSFC_Triangle(GraphicsView view)
    {
        parentView = view;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // Set up the drawing properties
        canvas.StrokeColor = Colors.Black;
        canvas.StrokeSize = 2;

        // Dynamically calculate triangle dimensions based on the dirtyRect size
        // Using the minium of width and height as the triangle height, scale to 98% of the GraphicsView
        float triangleHeight = Math.Min(dirtyRect.Width, dirtyRect.Height) * 0.98f;

        // Calculating the triangle width
        float triangleWidth = (2F * triangleHeight) / (float)Math.Sqrt(3F);

        // Gettting the centre point
        float centerX = dirtyRect.Width / 2;
        float centerY = dirtyRect.Height / 2;
        
        // Start drawing the arrow path
        PathF arrowPath = new PathF();

        PointF point1 = new PointF(centerX, centerY - triangleHeight / 2);
        PointF point2 = new PointF(centerX - triangleWidth / 2, centerY + triangleHeight / 2);
        PointF point3 = new PointF(centerX + triangleWidth / 2, centerY + triangleHeight / 2);

        arrowPath.MoveTo(point1);
        arrowPath.LineTo(point2);
        arrowPath.LineTo(point3);

        arrowPath.Close();

        // Fill the arrow with color
        canvas.StrokeSize = 0;
        canvas.StrokeLineJoin = LineJoin.Round;

        canvas.FillColor = background;
        canvas.FillPath(arrowPath);
        canvas.SetShadow(new SizeF(0, 0), 10, Colors.Grey);

        // Optional: Add an outline
        canvas.StrokeColor = background;
        canvas.DrawPath(arrowPath);
    }
}