using System.ComponentModel;
using STFN.Core;
using STFN.Core.SipTest;

namespace STFM.Views
{

    [ToolboxItem(true)] // Marks this class as available for the Toolbox
    public partial class TestResultView_QuickSiP : TestResultsView
    {
        public TestResultView_QuickSiP()
        {
            InitializeComponent();
            //this.LoadFromXaml(typeof(TestResultView_Adaptive));


            // Assign the custom drawable to the GraphicsView
            ScoreSnrView.Drawable = new TestResultsDiagram(ScoreSnrView);
            TestResultsDiagram MyScoreSnrDiagram = (TestResultsDiagram)ScoreSnrView.Drawable;
            MyScoreSnrDiagram.SetSizeModificationStrategy(PlotBase.SizeModificationStrategies.Horizontal);

            // Setting up default scales in the diagram
            List<float> X_PnrArray = new List<float>();
            List<double> TestPNRs = STFN.Core.QuickSiP.GetTestPnrs();
            foreach (double pnr in TestPNRs)
            {
                X_PnrArray.Add((float)pnr);
            }

            MyScoreSnrDiagram.SetXlim(X_PnrArray.Min() - 2.5f, X_PnrArray.Max() + 2.5f);
            MyScoreSnrDiagram.SetYlim(0, 105);

            List<float> YaxisLinePositions = new List<float>();
            List<float> YaxisTextPositions = new List<float>();
            List<string> YaxisTextValues = new List<string>();
            for (int i = 0; i < 100 + 10; i += 20)
            {
                YaxisTextPositions.Add((float)i);
                YaxisTextValues.Add(i.ToString());
                if (i != 0) { YaxisLinePositions.Add((float)i); }
            }

            List<float> XaxisTextPositions = new List<float>();
            List<string> XaxisTextValues = new List<string>();
            for (int i = 0; i < X_PnrArray.Count; i++)
            {
                XaxisTextPositions.Add(X_PnrArray[i]);
                XaxisTextValues.Add(X_PnrArray[i].ToString());
            }

            MyScoreSnrDiagram.SetTickTextsY(YaxisTextPositions, YaxisTextValues.ToArray());
            MyScoreSnrDiagram.SetTickTextsX(XaxisTextPositions, XaxisTextValues.ToArray());

            MyScoreSnrDiagram.SetYaxisDashedGridLinePositions(YaxisLinePositions);

            MyScoreSnrDiagram.SetSizeModificationStrategy(PlotBase.SizeModificationStrategies.Horizontal);
            MyScoreSnrDiagram.SetTextSizeAxisX(0.8f);
            MyScoreSnrDiagram.SetTextSizeAxisY(0.8f);
            MyScoreSnrDiagram.UpdateLayout();

            //MySnrDiagram.TransitionHeightRatio = 0.86f;
            //MySnrDiagram.Background = Colors.DarkSlateGray;

            // Force redraw on size change
            ScoreSnrView.SizeChanged += (s, e) => ScoreSnrView.Invalidate();


            switch (SharedSpeechTestObjects.GuiLanguage)
            {
                case STFN.Core.Utils.Constants.Languages.Swedish:
                    StartButton.Text = "Start";
                    StopButton.Text = "Stop";
                    PauseButton.Text = "Pause";
                    ScreenShotButton.Text = "Spara";

                    ReferenceLevelNameLabel.Text = "Referensnivå:";
                    PnrNameLabel.Text = "PNR:";
                    TrialNumberNameLabel.Text = "Presenterade ord:";
                    FinalResultNameLabel.Text = "Resultat, TP:";
                    SnrGridLabelY.Text = "TP (%)";
                    SnrGridLabelX.Text = "PNR (dB)";

                    GroupNameLabel1.Text = "Grupp 1";
                    GroupNameLabel2.Text = "Grupp 2";
                    GroupNameLabel3.Text = "Grupp 3";
                    GroupNameLabel4.Text = "Grupp 4";
                    GroupNameLabel5.Text = "Grupp 5";
                    GroupNameLabel6.Text = "Grupp 6";

                    break;
                default:
                    StartButton.Text = "Start";
                    StopButton.Text = "Stop";
                    PauseButton.Text = "Pause";
                    ScreenShotButton.Text = "Save";

                    ReferenceLevelNameLabel.Text = "Reference level:";
                    PnrNameLabel.Text = "PNR:";
                    TrialNumberNameLabel.Text = "Presented trials:";
                    FinalResultNameLabel.Text = "Final score:";
                    SnrGridLabelY.Text = "Score (%)";
                    SnrGridLabelX.Text = "PNR (dB)";

                    GroupNameLabel1.Text = "Group 1";
                    GroupNameLabel2.Text = "Group 2";
                    GroupNameLabel3.Text = "Group 3";
                    GroupNameLabel4.Text = "Group 4";
                    GroupNameLabel5.Text = "Group 5";
                    GroupNameLabel6.Text = "Group 6";

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
            // Referencing the ScoreSnrDiagram locally
            TestResultsDiagram MyScoreSnrDiagram = (TestResultsDiagram)ScoreSnrView.Drawable;
            var ObservedTestTrials = speechTest.GetObservedTestTrials();

          
            // Reference level
            ReferenceLevelValueLabel.Text = speechTest.ReferenceLevel.ToString() + " dB SPL";

            // PNR
            if (ObservedTestTrials.Any())
            {
                SipTrial lastSipTrial = (SipTrial)(ObservedTestTrials.Last());
                PnrValueLabel.Text = System.Math.Round(lastSipTrial.PNR, 1).ToString() + " dB";
            }
            else
            {
                PnrValueLabel.Text = "—";
            }

            // Trial count / progress
            if (ObservedTestTrials.Any())
            {
                TrialNumberValueLabel.Text = (ObservedTestTrials.Count()).ToString() + " / " + speechTest.GetTotalTrialCount().ToString();
            }
            else
            {
                TrialNumberValueLabel.Text = "1 / " + speechTest.GetTotalTrialCount().ToString();
            }

            // SRT:
            double? FinalResult = speechTest.GetAverageScore();
            if (FinalResult.HasValue)
            {
                FinalResultValueLabel.Text = System.Math.Round(100*FinalResult.Value, 0).ToString() + "%";
            }
            else
            {
                FinalResultValueLabel.Text = "—";
            }


            // Score-PNR diagram 
            List<float> Y_ScoreArray = new List<float>();
            List<float> X_PnrArray = new List<float>();

            // Getting and adding values
            Tuple<string,SortedList<double,double>> PnrScores = speechTest.GetScorePerLevel();

            if (PnrScores != null)
            {
                if (PnrScores.Item2.Keys.Count > 0)
                {

                    // Setting X-axis title
                    SnrGridLabelX.Text = PnrScores.Item1;

                    foreach (var kvp in PnrScores.Item2)
                    {
                        X_PnrArray.Add((float)kvp.Key);
                        Y_ScoreArray.Add((float)(100 * kvp.Value));
                    }

                    MyScoreSnrDiagram.PointSeries.Clear();
                    MyScoreSnrDiagram.Lines.Clear();

                    MyScoreSnrDiagram.PointSeries.Add(new PointSerie() { Color = Colors.Red, PointSize = 1, Type = PointSerie.PointTypes.Cross, XValues = X_PnrArray.ToArray(), YValues = Y_ScoreArray.ToArray() });
                    MyScoreSnrDiagram.Lines.Add(new Line() { Color = Colors.Blue, Dashed = false, LineWidth = 2, XValues = X_PnrArray.ToArray(), YValues = Y_ScoreArray.ToArray() });

                    MyScoreSnrDiagram.UpdateLayout();
                }
            }


            // Details frame
            List<Tuple<string, double>> SubGroupResults = speechTest.GetSubGroupResults();

            if (SubGroupResults != null)
            {

                if (SubGroupResults.Count > 0)
                {
                    GroupNameLabel1.Text = SubGroupResults[0].Item1;
                    GroupProgressBar1.Progress = SubGroupResults[0].Item2;
                    GroupScoreLabel1.Text = System.Math.Round(100* SubGroupResults[0].Item2).ToString() + "%";
                }

                if (SubGroupResults.Count > 1)
                {
                    GroupNameLabel2.Text = SubGroupResults[1].Item1;
                    GroupProgressBar2.Progress = SubGroupResults[1].Item2;
                    GroupScoreLabel2.Text = System.Math.Round(100 * SubGroupResults[1].Item2).ToString() + "%";
                }
                if (SubGroupResults.Count > 2)
                {
                    GroupNameLabel3.Text = SubGroupResults[2].Item1;
                    GroupProgressBar3.Progress = SubGroupResults[2].Item2;
                    GroupScoreLabel3.Text = System.Math.Round(100 * SubGroupResults[2].Item2).ToString() + "%";
                }
                if (SubGroupResults.Count > 3)
                {
                    GroupNameLabel4.Text = SubGroupResults[3].Item1;
                    GroupProgressBar4.Progress = SubGroupResults[3].Item2;
                    GroupScoreLabel4.Text = System.Math.Round(100 * SubGroupResults[3].Item2).ToString() + "%";
                }
                if (SubGroupResults.Count > 4)
                {
                    GroupNameLabel5.Text = SubGroupResults[4].Item1;
                    GroupProgressBar5.Progress = SubGroupResults[4].Item2;
                    GroupScoreLabel5.Text = System.Math.Round(100 * SubGroupResults[4].Item2).ToString() + "%";
                }
                if (SubGroupResults.Count > 5)
                {
                    GroupNameLabel6.Text = SubGroupResults[5].Item1;
                    GroupProgressBar6.Progress = SubGroupResults[5].Item2;
                    GroupScoreLabel6.Text = System.Math.Round(100 * SubGroupResults[5].Item2).ToString() + "%";
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

        private void ScreenShotButton_Clicked(object sender, EventArgs e)
        {
            TakeScreenShot();
        }

        public override void SetGuiLayoutState(SpeechTestView.GuiLayoutStates currentTestPlayState)
        {


            switch (currentTestPlayState)
            {
                case SpeechTestView.GuiLayoutStates.InitialState:
                    StartButton.IsEnabled = false;
                    PauseButton.IsEnabled = false;
                    StopButton.IsEnabled = false;
                    ScreenShotButton.IsEnabled = false;

                    break;
                case SpeechTestView.GuiLayoutStates.TestSelection:
                    StartButton.IsEnabled = false;
                    PauseButton.IsEnabled = false;
                    StopButton.IsEnabled = false;
                    ScreenShotButton.IsEnabled = false;

                    break;
                case SpeechTestView.GuiLayoutStates.SpeechMaterialSelection:
                    StartButton.IsEnabled = false;
                    PauseButton.IsEnabled = false;
                    StopButton.IsEnabled = false;
                    ScreenShotButton.IsEnabled = false;

                    break;
                case SpeechTestView.GuiLayoutStates.TestOptions_StartButton_TestResultsOnForm:
                    StartButton.IsEnabled = true;
                    PauseButton.IsEnabled = false;
                    StopButton.IsEnabled = false;
                    ScreenShotButton.IsEnabled = false;

                    break;
                case SpeechTestView.GuiLayoutStates.TestOptions_StartButton_TestResultsOffForm:
                    StartButton.IsEnabled = true;
                    PauseButton.IsEnabled = false;
                    StopButton.IsEnabled = false;
                    ScreenShotButton.IsEnabled = false;

                    break;
                case SpeechTestView.GuiLayoutStates.TestIsRunning:
                    StartButton.IsEnabled = false;
                    PauseButton.IsEnabled = true;
                    StopButton.IsEnabled = true;
                    ScreenShotButton.IsEnabled = false;

                    break;
                case SpeechTestView.GuiLayoutStates.TestIsPaused:
                    StartButton.IsEnabled = true;
                    PauseButton.IsEnabled = false;
                    StopButton.IsEnabled = true;
                    ScreenShotButton.IsEnabled = true;

                    break;
                case SpeechTestView.GuiLayoutStates.TestIsStopped:
                    StartButton.IsEnabled = false;
                    PauseButton.IsEnabled = false;
                    StopButton.IsEnabled = false;
                    ScreenShotButton.IsEnabled = true;

                    break;
                default:
                    break;
            }


        }
    }

}
