using System.ComponentModel;
using STFN;
using STFN.SipTest;

namespace STFM.Views
{

    [ToolboxItem(true)] // Marks this class as available for the Toolbox
    public partial class TestResultView_Adaptive : TestResultsView
    {
        public TestResultView_Adaptive()
        {
            InitializeComponent();
            //this.LoadFromXaml(typeof(TestResultView_Adaptive));

            // Setting up diagram 1

            // Assign the custom drawable to the GraphicsView
            SnrView.Drawable = new TestResultsDiagram(SnrView);
            TestResultsDiagram MySnrDiagram = (TestResultsDiagram)SnrView.Drawable;
            MySnrDiagram.SetSizeModificationStrategy(PlotBase.SizeModificationStrategies.Horizontal);
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

                    SpeechLevelNameLabel.Text = "Talnivå:";
                    NoiseLevelNameLabel.Text = "Brusnivå";
                    AdaptiveLevelNameLabel.Text = "Adaptiv nivå:";
                    ContralateralNoiseNameLabel.Text = "Kontralat. maskeringsnivå:";

                    SnrGridLabelY.Text = "PNR (dB)";
                    SnrGridLabelX.Text = "Försök nummer";

                    TargetScoreNameLabel.Text = "Mål, andel korrekt:";
                    TenLastScoreNameLabel.Text = "Andel korrekt, sista 10:";
                    TrialNumberNameLabel.Text = "Försök nummer:";
                    FinalResultNameLabel.Text = "Hörtröskel:";

                    break;
                default:
                    StartButton.Text = "Start";
                    StopButton.Text = "Stop";
                    PauseButton.Text = "Pause";

                    SpeechLevelNameLabel.Text = "Speech level:";
                    NoiseLevelNameLabel.Text = "Noise level";
                    AdaptiveLevelNameLabel.Text = "Adaptive level:";
                    ContralateralNoiseNameLabel.Text = "Contralat. masking level";

                    SnrGridLabelY.Text = "PNR (dB)";
                    SnrGridLabelX.Text = "Test trial";

                    TargetScoreNameLabel.Text = "Target score:";
                    TenLastScoreNameLabel.Text = "Average scores, last 10:";
                    TrialNumberNameLabel.Text = "Trial number:";
                    FinalResultNameLabel.Text = "SRT";

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


            // Speech and noise levels
            SpeechLevelValueLabel.Text = System.Math.Round(speechTest.TargetLevel, 1).ToString() + " " +  speechTest.dBString();
            NoiseLevelValueLabel.Text = System.Math.Round(speechTest.MaskingLevel, 1).ToString() + " " + speechTest.dBString();

            // PNR
            double? CurrentAdaptiveValue = testProtocol.GetCurrentAdaptiveValue();
            if (CurrentAdaptiveValue.HasValue)
            {
                AdaptiveLevelValueLabel.Text = System.Math.Round(CurrentAdaptiveValue.Value,1).ToString() + " dB PNR";
            }
            else
            {
                AdaptiveLevelValueLabel.Text = "";
            }

            // Contralateral level
            ContralateralNoiseLevelValueLabel.Text = System.Math.Round(speechTest.ContralateralMaskingLevel,1).ToString() + " " + speechTest.dBString();


            // Target score
            if (testProtocol.TargetScore.HasValue)
            {
                if (speechTest.IsPractiseTest == false)
                {
                    TargetScoreValueLabel.Text = System.Math.Round(100 * testProtocol.TargetScore.Value, 0).ToString() + "%";
                }
                else
                {
                    TargetScoreValueLabel.Text = "";
                }
            }

            // Ten last trials' score
            double? averageScore = speechTest.GetAverageScore(10);
            if (averageScore.HasValue)
            {
                TenLastScoreValueLabel.Text = System.Math.Round(100 * averageScore.Value, 0).ToString();
            }
            else 
            {
                TenLastScoreValueLabel.Text = "";
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
            double? FinalResult = testProtocol.GetFinalResultValue();
            if (FinalResult.HasValue)
            {
                FinalResultValueLabel.Text = System.Math.Round(FinalResult.Value, 1).ToString() + " dB SNR";
            }
            else
            {
                FinalResultValueLabel.Text = "---";
            }


            // SNR diagram 
            if (ObservedTestTrials.Any())
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


}
