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
            SnrView.Drawable = new TestResultsDiagram(SnrView);
            TestResultsDiagram MySnrDiagram = (TestResultsDiagram)SnrView.Drawable;
            MySnrDiagram.SetSizeModificationStrategy(PlotBase.SizeModificationStrategies.Horizontal);
            MySnrDiagram.SetXlim(0.5f, 1.5f);
            MySnrDiagram.SetYlim(-10, 10);
            //MySnrDiagram.TransitionHeightRatio = 0.86f;
            //MySnrDiagram.Background = Colors.DarkSlateGray;
            MySnrDiagram.UpdateLayout();

            // Force redraw on size change
            SnrView.SizeChanged += (s, e) => SnrView.Invalidate();


            switch (STFN.SharedSpeechTestObjects.GuiLanguage)
            {
                case STFN.Utils.Constants.Languages.Swedish:
                    StartButton.Text = "Start";
                    StopButton.Text = "Stop";
                    PauseButton.Text = "Pause";
                    ScreenShotButton.Text = "Spara";

                    TargetScoreNameLabel.Text = "Mål, andel korrekt:";
                    ReferenceLevelNameLabel.Text = "Referensnivå:";
                    AdaptiveLevelNameLabel.Text = "Adaptiv nivå:";
                    TrialNumberNameLabel.Text = "Försök nummer:";
                    FinalResultNameLabel.Text = "Hörtröskel:";

                    SnrGridLabelY.Text = "PNR (dB)";
                    SnrGridLabelX.Text = "Försök nummer";

                    GroupScoreTitleLabel.Text = "Andel korrekt sista 10 försöken";
                    TwgNameLabel1.Text = "Grupp 1";
                    TwgNameLabel2.Text = "Grupp 2";
                    TwgNameLabel3.Text = "Grupp 3";
                    TwgNameLabel4.Text = "Grupp 4";
                    TwgNameLabel5.Text = "Grupp 5";
                    TenLastScoreNameLabel.Text = "Alla grupper";

                    break;
                default:
                    StartButton.Text = "Start";
                    StopButton.Text = "Stop";
                    PauseButton.Text = "Pause";
                    ScreenShotButton.Text = "Save";

                    SnrGridLabelY.Text = "PNR (dB)";
                    SnrGridLabelX.Text = "Test trial";

                    TargetScoreNameLabel.Text = "Target score:";
                    ReferenceLevelNameLabel.Text = "Reference level:";
                    AdaptiveLevelNameLabel.Text = "Adaptive level:";
                    TrialNumberNameLabel.Text = "Trial number:";
                    FinalResultNameLabel.Text = "SRT";

                    GroupScoreTitleLabel.Text = "Average scores in last 10 trials";
                    TwgNameLabel1.Text = "Group 1";
                    TwgNameLabel2.Text = "Group 2";
                    TwgNameLabel3.Text = "Group 3";
                    TwgNameLabel4.Text = "Group 4";
                    TwgNameLabel5.Text = "Group 5";
                    TenLastScoreNameLabel.Text = "Overall score";

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
            TestResultsDiagram MySnrDiagram = (TestResultsDiagram)SnrView.Drawable;
            TestProtocol testProtocol = speechTest.TestProtocol;

            if (speechTest.TestProtocol == null)
            {
                // This works bad without a test protocol. Returning to prevent exceptions
                return;
            }

            var ObservedTestTrials = speechTest.GetObservedTestTrials();

            // Target score
            if (testProtocol.TargetScore.HasValue)
            {
                if (speechTest.IsPractiseTest == false)
                {
                    TargetScoreValueLabel.Text = System.Math.Round(100 * testProtocol.TargetScore.Value, 0).ToString() + "%";
                }
                else
                {
                    TargetScoreValueLabel.Text = "—";
                }
            }

            // Reference level
            ReferenceLevelValueLabel.Text = speechTest.ReferenceLevel.ToString() + " dB SPL";

            // PNR
            double? CurrentAdaptiveValue = testProtocol.GetCurrentAdaptiveValue();
            if (CurrentAdaptiveValue.HasValue)
            {
                AdaptiveLevelValueLabel.Text = System.Math.Round(CurrentAdaptiveValue.Value,1).ToString() + " dB PNR";
            }
            else
            {
                AdaptiveLevelValueLabel.Text = "—";
            }

            // Trial count / progress
            if (ObservedTestTrials.Count() > 0)
            {
                TrialNumberValueLabel.Text = (1 + ObservedTestTrials.Count()).ToString() + " / " + speechTest.GetTotalTrialCount().ToString();
            }
            else
            {
                TrialNumberValueLabel.Text = "1 / " + speechTest.GetTotalTrialCount().ToString();
            }

            // SRT:
            if (speechTest.IsPractiseTest == false)
            {
                double? FinalResult = testProtocol.GetFinalResultValue();
                switch (STFN.SharedSpeechTestObjects.GuiLanguage)
                {
                    case STFN.Utils.Constants.Languages.English:
                        FinalResultNameLabel.Text = "SRT:";
                        break;
                    case STFN.Utils.Constants.Languages.Swedish:
                        FinalResultNameLabel.Text = "Hörtröskel:";
                        break;
                    default:
                        FinalResultNameLabel.Text = "SRT:";
                        break;
                }
                if (FinalResult.HasValue)
                {
                    FinalResultValueLabel.Text = System.Math.Round(FinalResult.Value, 1).ToString() + " dB PNR";
                }
                else
                {
                    FinalResultValueLabel.Text = "—";
                }
            }
            else
            {
                double? PractiseTestScore = speechTest.GetAverageScore();
                switch (STFN.SharedSpeechTestObjects.GuiLanguage)
                {
                    case STFN.Utils.Constants.Languages.English:
                        FinalResultNameLabel.Text = "Score:";
                        break;
                    case STFN.Utils.Constants.Languages.Swedish:
                        FinalResultNameLabel.Text = "Andel korrekt:";
                        break;
                    default:
                        FinalResultNameLabel.Text = "Score:";
                        break;
                }
                if (PractiseTestScore.HasValue)
                {
                    FinalResultValueLabel.Text = System.Math.Round(100 * PractiseTestScore.Value, 0).ToString() + " %";
                }
                else
                {
                    FinalResultValueLabel.Text = "—";
                }
            }


            // SNR diagram (Not updating the SNR diagram in practise tests)
            if (speechTest.IsPractiseTest == false)
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


                MySnrDiagram.SetSizeModificationStrategy(PlotBase.SizeModificationStrategies.Horizontal);
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

                MySnrDiagram.UpdateLayout();

            }


            // Result details
            List<Tuple< string,double>> SubGroupResults = speechTest.GetSubGroupResults();

            if (SubGroupResults != null)
            {

                if (SubGroupResults.Count > 0)
                {
                    TwgNameLabel1.Text = SubGroupResults[0].Item1;
                    TwgProgressBar1.Progress = SubGroupResults[0].Item2;
                    TwgScoreLabel1.Text = System.Math.Round(100* SubGroupResults[0].Item2).ToString() + "%";
                }

                if (SubGroupResults.Count > 1)
                {
                    TwgNameLabel2.Text = SubGroupResults[1].Item1;
                    TwgProgressBar2.Progress = SubGroupResults[1].Item2;
                    TwgScoreLabel2.Text = System.Math.Round(100 * SubGroupResults[1].Item2).ToString() + "%";
                }

                if (SubGroupResults.Count > 2)
                {
                    TwgNameLabel3.Text = SubGroupResults[2].Item1;
                    TwgProgressBar3.Progress = SubGroupResults[2].Item2;
                    TwgScoreLabel3.Text = System.Math.Round(100 * SubGroupResults[2].Item2).ToString() + "%";
                }

                if (SubGroupResults.Count > 3)
                {
                    TwgNameLabel4.Text = SubGroupResults[3].Item1;
                    TwgProgressBar4.Progress = SubGroupResults[3].Item2;
                    TwgScoreLabel4.Text = System.Math.Round(100 * SubGroupResults[3].Item2).ToString() + "%";
                }

                if (SubGroupResults.Count > 4)
                {
                    TwgNameLabel5.Text = SubGroupResults[4].Item1;
                    TwgProgressBar5.Progress = SubGroupResults[4].Item2;
                    TwgScoreLabel5.Text = System.Math.Round(100 * SubGroupResults[4].Item2).ToString() + "%";
                }

                if (SubGroupResults.Count > 5)
                {
                    TenLastScoreNameLabel.Text = SubGroupResults[5].Item1;
                    TenLastScoreProgressBar.Progress = SubGroupResults[5].Item2;
                    TenLastScoreLabel.Text = System.Math.Round(100 * SubGroupResults[5].Item2).ToString() + "%";
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

                    break;
                case SpeechTestView.GuiLayoutStates.TestSelection:
                    StartButton.IsEnabled = false;
                    PauseButton.IsEnabled = false;
                    StopButton.IsEnabled = false;

                    break;
                case SpeechTestView.GuiLayoutStates.SpeechMaterialSelection:
                    StartButton.IsEnabled = false;
                    PauseButton.IsEnabled = false;
                    StopButton.IsEnabled = false;

                    break;
                case SpeechTestView.GuiLayoutStates.TestOptions_StartButton_TestResultsOnForm:
                    StartButton.IsEnabled = true;
                    PauseButton.IsEnabled = false;
                    StopButton.IsEnabled = false;

                    break;
                case SpeechTestView.GuiLayoutStates.TestOptions_StartButton_TestResultsOffForm:
                    StartButton.IsEnabled = true;
                    PauseButton.IsEnabled = false;
                    StopButton.IsEnabled = false;

                    break;
                case SpeechTestView.GuiLayoutStates.TestIsRunning:
                    StartButton.IsEnabled = false;
                    PauseButton.IsEnabled = true;
                    StopButton.IsEnabled = true;

                    break;
                case SpeechTestView.GuiLayoutStates.TestIsPaused:
                    StartButton.IsEnabled = true;
                    PauseButton.IsEnabled = false;
                    StopButton.IsEnabled = true;

                    break;
                case SpeechTestView.GuiLayoutStates.TestIsStopped:
                    StartButton.IsEnabled = false;
                    PauseButton.IsEnabled = false;
                    StopButton.IsEnabled = false;

                    break;
                default:
                    break;
            }


        }
    }

}
