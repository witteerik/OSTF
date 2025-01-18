using System.ComponentModel;
using STFN;

namespace STFM.Views
{

    [ToolboxItem(true)] // Marks this class as available for the Toolbox
    public partial class TestResultView_Adaptive : TestResultsView
    {
        public TestResultView_Adaptive()
        {
            InitializeComponent();
            //this.LoadFromXaml(typeof(TestResultView_Adaptive));


            // Assign the custom drawable to the GraphicsView
            ArrowView.Drawable = new ArrowDrawable(ArrowView);
            ArrowDrawable arrowDrawable = (ArrowDrawable)ArrowView.Drawable;
            arrowDrawable.TransitionHeightRatio = 0.86f;
            arrowDrawable.Background = Colors.DarkSlateGray;

            // Force redraw on size change
            ArrowView.SizeChanged += (s, e) => ArrowView.Invalidate();

        }

        int testInt = 0;

        public override void ShowTestResults(string results)
        {

            if (testInt % 2 == 0 )
            {
                ArrowView.BackgroundColor = Colors.Red;
            }
            else
            {
                ArrowView.Background = Colors.Red;
            }

            ArrowView.Invalidate();

            testInt += 1;

            TestLabel.Text = results;

        }
    }
}