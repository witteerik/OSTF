using System.ComponentModel;
using STFN;

namespace STFM.Views
{

    [ToolboxItem(true)] // Marks this class as available for the Toolbox
    public partial class TestResultView_ConstantStimuli : TestResultsView
    {
        public TestResultView_ConstantStimuli()
        {
            InitializeComponent();
            //this.LoadFromXaml(typeof(TestResultView_Adaptive));

            // Setting up the diagram

            // Assign the custom drawable to the GraphicsView
            DiagramView.Drawable = new TestResultsDiagram(DiagramView);
            TestResultsDiagram MyDiagram = (TestResultsDiagram)DiagramView.Drawable;
            MyDiagram.SetSizeModificationStrategy(PlotBase.SizeModificationStrategies.Horizontal);

            // Setting up default scales in the diagram
            MyDiagram.SetYlim(0f,1f);
            MyDiagram.SetXlim(0, 105);

            List<float> XaxisLinePositions = new List<float>();
            List<float> XaxisTextPositions = new List<float>();
            List<string> XaxisTextValues = new List<string>();
            for (int i = 0; i < 100 + 10; i += 20)
            {
                XaxisTextPositions.Add((float)i);
                XaxisTextValues.Add(i.ToString());
                if (i != 0) { XaxisLinePositions.Add((float)i); }
            }

            MyDiagram.SetTickTextsX(XaxisTextPositions, XaxisTextValues.ToArray());
            MyDiagram.SetTickTextsY(new[] { 0.5f }.ToList(), new[] {""}.ToArray());

            MyDiagram.SetXaxisDashedGridLinePositions(XaxisLinePositions);

            MyDiagram.SetSizeModificationStrategy(PlotBase.SizeModificationStrategies.Horizontal);
            MyDiagram.SetTextSizeAxisX(0.8f);
            MyDiagram.SetTextSizeAxisY(0.8f);

            MyDiagram.PlotAreaRelativeMarginLeft = 0.05f;
            MyDiagram.PlotAreaRelativeMarginRight = 0.05f;
            MyDiagram.PlotAreaRelativeMarginTop = 0.05f;
            MyDiagram.PlotAreaRelativeMarginBottom = 0.15f;

            MyDiagram.UpdateLayout();


            // Force redraw on size change
            DiagramView.SizeChanged += (s, e) => DiagramView.Invalidate();


            switch (STFN.SharedSpeechTestObjects.GuiLanguage)
            {
                case STFN.Utils.Constants.Languages.Swedish:
                    StartButton.Text = "Start";
                    StopButton.Text = "Stop";
                    PauseButton.Text = "Pause";
                    ScreenShotButton.Text = "Spara";

                    SpeechLevelNameLabel.Text = "Talnivå:";
                    NoiseLevelNameLabel.Text = "Brusnivå";
                    SnrNameLabel.Text = "SNR:";
                    ContralateralNoiseNameLabel.Text = "Kontralat. mask.:";
                    TrialNumberNameLabel.Text = "Försök nummer:";
                    FinalResultNameLabel.Text = "Resultat, TP:";

                    SnrGridLabelX.Text = "Resultat, TP (%)";

                    break;
                default:
                    StartButton.Text = "Start";
                    StopButton.Text = "Stop";
                    PauseButton.Text = "Pause";
                    ScreenShotButton.Text = "Save";

                    SpeechLevelNameLabel.Text = "Speech level:";
                    NoiseLevelNameLabel.Text = "Noise level";
                    SnrNameLabel.Text = "SNR:";
                    ContralateralNoiseNameLabel.Text = "Contralat. mask.:";
                    TrialNumberNameLabel.Text = "Trial number:";
                    FinalResultNameLabel.Text = "Result, SRS";

                    SnrGridLabelX.Text = "Score (%)";

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
            // Referencing the diagram locally
            TestResultsDiagram MyDiagram = (TestResultsDiagram)DiagramView.Drawable;
            TestProtocol testProtocol = speechTest.TestProtocol;

            if (speechTest.TestProtocol == null)
            {
                // This works bad without a test protocol. Returning to prevent exceptions
                return;
            }

            // Getting the observed test trials
            var ObservedTestTrials = speechTest.GetObservedTestTrials();


            // Speech and noise levels
            SpeechLevelValueLabel.Text = System.Math.Round(speechTest.TargetLevel, 0).ToString() + " " + speechTest.dBString();
            if (speechTest.MaskerLocations.Any())
            {
                NoiseLevelValueLabel.Text = System.Math.Round(speechTest.MaskingLevel, 0).ToString() + " " + speechTest.dBString();
            }
            else
            {
                NoiseLevelValueLabel.Text = "—";
            }

            // SNR
            if (speechTest.MaskerLocations.Any()) // Showing SNR only if there is a masker
            {
                SnrValueLabel.Text = System.Math.Round(speechTest.CurrentSNR,0).ToString() + " dB";
            }
            else
            {
                SnrValueLabel.Text = "—";
            }

            // Contralateral level
            if (speechTest.ContralateralMasking)
            {
                ContralateralNoiseLevelValueLabel.Text = System.Math.Round(speechTest.ContralateralMaskingLevel, 0).ToString() + " " + speechTest.dBString();
            }
            else
            {
                ContralateralNoiseLevelValueLabel.Text = "—";
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


            if (ObservedTestTrials.Any())
            {

                // Getting the score so far
                double? averageScore = speechTest.GetAverageScore();
                // Diagram 
                if (averageScore.HasValue)
                {

                FinalResultValueLabel.Text = System.Math.Round(100 * averageScore.Value, 0).ToString() + "%";

                MyDiagram.Areas.Clear();

                    MyDiagram.Areas.Add(new Area() { Color = Color.FromRgb(4, 255, 61), Alpha = 0.4f, XValues = new[] { 0f, (float)(100 * averageScore) }, YValuesLower = [0.2f, 0.2f], YValuesUpper = [0.8f, 0.8f] });

                MyDiagram.UpdateLayout();

                }
                else
                {
                    FinalResultValueLabel.Text = "—";
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
