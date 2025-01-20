using System.ComponentModel;
using STFN;
using STFN.SipTest;

namespace STFM.Views
{

    [ToolboxItem(true)] // Marks this class as available for the Toolbox
    public partial class TestResultView_AdaptiveSiP : TestResultsView
    {
        public TestResultView_AdaptiveSiP()
        {
            InitializeComponent();
            //this.LoadFromXaml(typeof(TestResultView_Adaptive));


            // Assign the custom drawable to the GraphicsView
            SnrView.Drawable = new SnrDiagram(SnrView);
            SnrDiagram MySnrDiagram = (SnrDiagram)SnrView.Drawable;
            MySnrDiagram.SetAxisTextSizeModificationStrategy(PlotBase.AxisTextSizeModificationStrategy.Horizontal);
            MySnrDiagram.SetXlim(0.5f, 1.5f);
            MySnrDiagram.SetYlim(-10, 10);
            //MySnrDiagram.TransitionHeightRatio = 0.86f;
            //MySnrDiagram.Background = Colors.DarkSlateGray;

            // Force redraw on size change
            SnrView.SizeChanged += (s, e) => SnrView.Invalidate();


            switch (STFN.SharedSpeechTestObjects.GuiLanguage)
            {
                case STFN.Utils.Constants.Languages.Swedish:
                    StartButton.Text = "Start";
                    StopButton.Text = "Stop";
                    PauseButton.Text = "Pause";
                    break;
                default:
                    StartButton.Text = "Start";
                    StopButton.Text = "Stop";
                    PauseButton.Text = "Pause";
                    break;
            }

        }

        public override void UpdateStartButtonText(string text)
        {
            StartButton.Text = text;
        }

        public override void ShowTestResults(string results)
        {
            // Not used
        }

        public override void ShowTestResults(SpeechTest speechTest)
        {
            // Referencing the SnrDiagram locally
            SnrDiagram MySnrDiagram = (SnrDiagram)SnrView.Drawable;

            // Using specificelly Adaptive SiP-test
            AdaptiveSiP MyAdaptiveSipTest = (AdaptiveSiP)speechTest;
            TestProtocol testProtocol = MyAdaptiveSipTest.TestProtocol;
            var ObservedTestTrials = MyAdaptiveSipTest.GetObservedTestTrials();

            // Target score
            TargetScoreNameLabel.Text = "Target score:";
            if (testProtocol.TargetScore.HasValue)
            {
                if (MyAdaptiveSipTest.IsPractiseTest == false)
                {
                    TargetScoreValueLabel.Text = System.Math.Round(100 * testProtocol.TargetScore.Value, 0).ToString() + "%";
                }
                else
                {
                    TargetScoreValueLabel.Text = "";
                }
            }
            else
            {
                TargetScoreValueLabel.Text = "";
            }


            // Reference level
            ReferenceLevelNameLabel.Text = "Reference level:";
            ReferenceLevelValueLabel.Text = MyAdaptiveSipTest.ReferenceLevel.ToString() + " dB SPL";

            // PNR
            AdaptiveLevelNameLabel.Text = "Adaptive level:";

            double? CurrentAdaptiveValue = testProtocol.GetCurrentAdaptiveValue();
            if (CurrentAdaptiveValue.HasValue)
            {
                AdaptiveLevelValueLabel.Text = System.Math.Round(CurrentAdaptiveValue.Value,1).ToString() + " dB PNR";
            }
            else
            {
                AdaptiveLevelValueLabel.Text = "";
            }

            // Trial count / progress
            TrialNumberNameLabel.Text = "Trial number:";
            if (ObservedTestTrials.Count() > 0)
            {
                TrialNumberValueLabel.Text = (1 + ObservedTestTrials.Count()).ToString() + " / " + testProtocol.TotalTrialCount().ToString();
            }
            else
            {
                TrialNumberValueLabel.Text = "1 / " + testProtocol.TotalTrialCount().ToString();
            }

            // SRT:
            double? FinalResult = testProtocol.GetFinalResultValue();
            if (MyAdaptiveSipTest.IsPractiseTest == false)
            {
                FinalResultNameLabel.Text = "SRT:";
                if (FinalResult.HasValue)
                {
                    FinalResultValueLabel.Text = System.Math.Round(FinalResult.Value, 1).ToString() + " dB PNR";
                }
                else
                {
                    FinalResultValueLabel.Text = "---";
                }
            }
            else
            {
                FinalResultNameLabel.Text = "Score:";
                if (FinalResult.HasValue)
                {
                    FinalResultValueLabel.Text = System.Math.Round(100 * FinalResult.Value, 0).ToString() + " %";
                }
                else
                {
                    FinalResultValueLabel.Text = "---";
                }
            }


            // SNR diagram (Not updating the SNR diagram in practise tests)
            if (MyAdaptiveSipTest.IsPractiseTest == false)
            {

                List<float> PresentedPnrs = new List<float>();
                List<float> PresentedTrials = new List<float>();

                float presentedTrialIndex = 1;
                foreach (TestTrial trial in ObservedTestTrials)
                {
                    PresentedPnrs.Add((float)trial.AdaptiveProtocolValue);
                    PresentedTrials.Add(presentedTrialIndex);
                    presentedTrialIndex += 1;
                }

                // Adding also the current adaptive value (which has not yet been stored)
                if (CurrentAdaptiveValue != null)
                {
                    PresentedPnrs.Add((float)CurrentAdaptiveValue.Value);
                    PresentedTrials.Add(presentedTrialIndex);
                }


                MySnrDiagram.SetAxisTextSizeModificationStrategy(PlotBase.AxisTextSizeModificationStrategy.Horizontal);
                MySnrDiagram.SetTextSizeAxisX(0.8f);
                MySnrDiagram.SetTextSizeAxisY(0.8f);

                MySnrDiagram.SetXlim(0.5f, PresentedPnrs.Count + 0.5f);

                float Ymin = PresentedPnrs.Min() - 5f;
                float Ymax = PresentedPnrs.Max() + 5f;
                MySnrDiagram.SetYlim(Ymin, Ymax);


                List<float> YaxisTextPositions = new List<float>();
                List<string> YaxisTextValues = new List<string>();

                int Steps = 6;
                int StepSize = (int)((Ymax - Ymin) / Steps);
                for (int i = 0; i < Steps +10; i ++)
                {
                    YaxisTextPositions.Add((float)System.Math.Round( Ymin ,0) + (i * StepSize));
                    YaxisTextValues.Add(((float)System.Math.Round(Ymin, 0) + (i * StepSize)).ToString());
                }

                List<float> XaxisTextPositions = new List<float>();
                List<string> XaxisTextValues = new List<string>();

                int TrialSteps = 1+ (int)(PresentedTrials.Count / 15);
                for (int i = 0; i < PresentedTrials.Count; i+= TrialSteps)
                {
                    XaxisTextPositions.Add(PresentedTrials[i]);
                    XaxisTextValues.Add(PresentedTrials[i].ToString());
                }

                MySnrDiagram.SetTickTextsY(YaxisTextPositions, YaxisTextValues.ToArray());
                MySnrDiagram.SetTickTextsX(XaxisTextPositions, XaxisTextValues.ToArray());

                MySnrDiagram.PointSeries.Clear();
                MySnrDiagram.Lines.Clear();

                MySnrDiagram.PointSeries.Add(new PointSerie() { Color = Colors.Red, PointSize = 1, Type = PointSerie.PointTypes.Cross, XValues = PresentedTrials.ToArray(), YValues = PresentedPnrs.ToArray() });
                MySnrDiagram.Lines.Add(new Line() { Color = Colors.Blue, Dashed = false, LineWidth = 2, XValues = PresentedTrials.ToArray(), YValues = PresentedPnrs.ToArray() });

            }


            //TestLabel.Text = results;

            SortedList<string,double> SubGroupResults = MyAdaptiveSipTest.GetSubGroupResults();

            if (SubGroupResults != null)
            {

                if (SubGroupResults.Keys.Count > 0)
                {
                    TwgNameLabel1.Text = SubGroupResults.Keys[0];
                    TwgProgressBar1.Progress = SubGroupResults.Values[0];
                    TwgScoreLabel1.Text = System.Math.Round(100* SubGroupResults.Values[0]).ToString() + "%";
                }

                if (SubGroupResults.Keys.Count > 1)
                {
                    TwgNameLabel2.Text = SubGroupResults.Keys[1];
                    TwgProgressBar2.Progress = SubGroupResults.Values[1];
                    TwgScoreLabel2.Text = System.Math.Round(100 * SubGroupResults.Values[1]).ToString() + "%";
                }

                if (SubGroupResults.Keys.Count > 2)
                {
                    TwgNameLabel3.Text = SubGroupResults.Keys[2];
                    TwgProgressBar3.Progress = SubGroupResults.Values[2];
                    TwgScoreLabel3.Text = System.Math.Round(100 * SubGroupResults.Values[2]).ToString() + "%";
                }

                if (SubGroupResults.Keys.Count > 3)
                {
                    TwgNameLabel4.Text = SubGroupResults.Keys[3];
                    TwgProgressBar4.Progress = SubGroupResults.Values[3];
                    TwgScoreLabel4.Text = System.Math.Round(100 * SubGroupResults.Values[3]).ToString() + "%";
                }

                if (SubGroupResults.Keys.Count > 4)
                {
                    TwgNameLabel5.Text = SubGroupResults.Keys[4];
                    TwgProgressBar5.Progress = SubGroupResults.Values[4];
                    TwgScoreLabel5.Text = System.Math.Round(100 * SubGroupResults.Values[4]).ToString() + "%";
                }

                if (SubGroupResults.Keys.Count > 5)
                {
                    TenLastScoreNameLabel.Text = SubGroupResults.Keys[5];
                    TenLastScoreProgressBar.Progress = SubGroupResults.Values[5];
                    TenLastScoreLabel.Text = System.Math.Round(100 * SubGroupResults.Values[5]).ToString() + "%";
                }

            }
        }

        private void StartButton_Clicked(object sender, EventArgs e)
        {
            OnStartedFromTestResultView(new EventArgs());
        }

        private void PauseButton_Clicked(object sender, EventArgs e)
        {
            OnPausedFromTestResultView(new EventArgs());
        }
        private void StopButton_Clicked(object sender, EventArgs e)
        {
            OnStoppedFromTestResultView(new EventArgs());
        }


        public override void SetPlayState(SpeechTestView.TestPlayStates currentTestPlayState)
        {

            switch (currentTestPlayState)
            {
                case SpeechTestView.TestPlayStates.InitialState:
                    StartButton.IsEnabled = false;
                    PauseButton.IsEnabled = false;
                    StopButton.IsEnabled = false;

                    break;

                case SpeechTestView.TestPlayStates.ShowTestSelection:
                    StartButton.IsEnabled = false;
                    PauseButton.IsEnabled = false;
                    StopButton.IsEnabled = false;

                    break;
                case SpeechTestView.TestPlayStates.ShowSpeechMaterialSelection:
                    StartButton.IsEnabled = false;
                    PauseButton.IsEnabled = false;
                    StopButton.IsEnabled = false;

                    break;
                case SpeechTestView.TestPlayStates.ShowTestOptionsAndStartButton:
                    StartButton.IsEnabled = true;
                    PauseButton.IsEnabled = false;
                    StopButton.IsEnabled = false;

                    break;
                case SpeechTestView.TestPlayStates.TestIsRunning:
                    StartButton.IsEnabled = false;
                    PauseButton.IsEnabled = true;
                    StopButton.IsEnabled = true;

                    break;
                case SpeechTestView.TestPlayStates.TestIsPaused:
                    StartButton.IsEnabled = true;
                    PauseButton.IsEnabled = false;
                    StopButton.IsEnabled = true;

                    break;
                case SpeechTestView.TestPlayStates.TestIsStopped:
                    StartButton.IsEnabled = false;
                    PauseButton.IsEnabled = false;
                    StopButton.IsEnabled = false;

                    break;
                default:
                    break;
            }

        }
    }

    public class SnrDiagram : PlotBase, IDrawable
    {


        public SnrDiagram(GraphicsView view)
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
