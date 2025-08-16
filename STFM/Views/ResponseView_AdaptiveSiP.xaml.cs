
using System.ComponentModel;
using STFN.Core;
using STFN.Core.SipTest;


namespace STFM.Views
{

    [ToolboxItem(true)] // Marks this class as available for the Toolbox
    public partial class ResponseView_AdaptiveSiP : ResponseView
    {
        public ResponseView_AdaptiveSiP()
        {
            // Loading xaml content manually since for some reason does not InitializeComponent exist
            //this.LoadFromXaml(typeof(ResponseView_AdaptiveSiP));
            InitializeComponent();

            TestWordMatrix.IsVisible = false;
            OrderGrid.IsVisible = false;
            MessageButton.IsVisible = false;

            // Hides the ProgressFrame until used
            ProgressFrame.IsVisible = false;

            // Assign the custom drawable to the GraphicsView
            ArrowView.Drawable = new ArrowDrawable(ArrowView);
            ArrowDrawable arrowDrawable = (ArrowDrawable)ArrowView.Drawable;
            arrowDrawable.TransitionHeightRatio = 0.86f;
            arrowDrawable.Background = Colors.DarkSlateGray;

            // Force redraw on size change
            ArrowView.SizeChanged += (s, e) => ArrowView.Invalidate();

            // Creating a hide-all timer
            ResetGuiTimer = Microsoft.Maui.Controls.Application.Current.Dispatcher.CreateTimer();
            ResetGuiTimer.Interval = TimeSpan.FromMilliseconds(400);
            ResetGuiTimer.Tick += ResetGui_TimerTick;
            ResetGuiTimer.IsRepeating = false;

        }

        private IDispatcherTimer ResetGuiTimer;

        Color RedButtonColor = Color.FromArgb("#FF0000");
        Color DefaultButtonColor = Color.FromArgb("#FFFF80");
        
        private int ResponseCount  = 0;
        private int VisualCueCount = 0;

        private List<string> ReplyList = new List<string>();

        public override void AddSourceAlternatives(STFM.Views.ResponseView.VisualizedSoundSource[] soundSources)
        {
            //throw new NotImplementedException();
        }



        public override void HideAllItems()
        {
            TestWordMatrix.IsVisible = false;
            MessageButton.IsVisible = false;
            OrderGrid.IsVisible = false;
            ProgressFrame.IsVisible = false;    
        }

        private void ResetGui_TimerTick(object sender, EventArgs e)
        {
            //ResetGuiToInitialState();
        }



        public override void HideVisualCue()
        {
            //Not used
        }


        public override void InitializeNewTrial()
        {

            StopAllTimers();

            ResponseCount  = 0;
            VisualCueCount = 0;

            ResetGuiToInitialState();

            ReplyList.Clear();

        }

        public override void ResponseTimesOut()
        {
            if (GetRowResponse(TestWordGrid1) == "")
            {
                TestWordButton1_1.Background = RedButtonColor;
                TestWordButton1_2.Background = RedButtonColor;
                TestWordButton1_3.Background = RedButtonColor;
            }

            if (GetRowResponse(TestWordGrid2) == "")
            {
                TestWordButton2_1.Background = RedButtonColor;
                TestWordButton2_2.Background = RedButtonColor;
                TestWordButton2_3.Background = RedButtonColor;
            }

            if (GetRowResponse(TestWordGrid3) == "")
            {
                TestWordButton3_1.Background = RedButtonColor;
                TestWordButton3_2.Background = RedButtonColor;
                TestWordButton3_3.Background = RedButtonColor;
            }

            if (GetRowResponse(TestWordGrid4) == "")
            {
                TestWordButton4_1.Background = RedButtonColor;
                TestWordButton4_2.Background = RedButtonColor;
                TestWordButton4_3.Background = RedButtonColor;
            }

            if (GetRowResponse(TestWordGrid5) == "")
            {
                TestWordButton5_1.Background = RedButtonColor;
                TestWordButton5_2.Background = RedButtonColor;
                TestWordButton5_3.Background = RedButtonColor;
            }

            // Sending the reply
            SendReply();

        }

        public override void ShowMessage(string Message)
        {

            StopAllTimers();

            TestWordMatrix.IsVisible = false;
            OrderGrid.IsVisible = false; 

            MessageButton.IsVisible = true;
            MessageButton.Text = Message;
            MessageButton.FontSize = 25;

        }

        public void ResetGuiToInitialState()
        {

            // Clearing all texts on the buttons
            TestWordButton1_1.Text = "";
            TestWordButton1_2.Text = "";
            TestWordButton1_3.Text = "";

            TestWordButton2_1.Text = "";
            TestWordButton2_2.Text = "";
            TestWordButton2_3.Text = "";

            TestWordButton3_1.Text = "";
            TestWordButton3_2.Text = "";
            TestWordButton3_3.Text = "";

            TestWordButton4_1.Text = "";
            TestWordButton4_2.Text = "";
            TestWordButton4_3.Text = "";

            TestWordButton5_1.Text = "";
            TestWordButton5_2.Text = "";
            TestWordButton5_3.Text = "";

            TestWordButton1_1.Background = DefaultButtonColor;
            TestWordButton1_2.Background = DefaultButtonColor;
            TestWordButton1_3.Background = DefaultButtonColor;

            TestWordButton2_1.Background = DefaultButtonColor;
            TestWordButton2_2.Background = DefaultButtonColor;
            TestWordButton2_3.Background = DefaultButtonColor;

            TestWordButton3_1.Background = DefaultButtonColor;
            TestWordButton3_2.Background = DefaultButtonColor;
            TestWordButton3_3.Background = DefaultButtonColor;

            TestWordButton4_1.Background = DefaultButtonColor;
            TestWordButton4_2.Background = DefaultButtonColor;
            TestWordButton4_3.Background = DefaultButtonColor;

            TestWordButton5_1.Background = DefaultButtonColor;
            TestWordButton5_2.Background = DefaultButtonColor;
            TestWordButton5_3.Background = DefaultButtonColor;

            Circle1.IsVisible = false;
            Circle2.IsVisible = false;
            Circle3.IsVisible = false;
            Circle4.IsVisible = false;
            Circle5.IsVisible = false;

            TestWordRow1.IsVisible = true;
            TestWordRow2.IsVisible = true;
            TestWordRow3.IsVisible = true;
            TestWordRow4.IsVisible = true;
            TestWordRow5.IsVisible = true;

            TestWordMatrix.IsVisible = true;
            TestWordGrid.IsVisible = true;
            MessageButton.IsVisible = false;
            OrderGrid.IsVisible = true;

        }

        public override void ShowResponseAlternativePositions(List<List<SpeechTestResponseAlternative>> ResponseAlternatives)
        {

            // Calling resize on every presentation (could be done only initially)
            ResizeStuff(this.Width, this.Height);

            List<SpeechTestResponseAlternative> localResponseAlternatives = ResponseAlternatives[0];

            // Reading which side to put the response alternatives, based on the first one
            SipTrial parentTestTrial = (SipTrial)localResponseAlternatives[0].ParentTestTrial.SubTrials[0];
            if (parentTestTrial.TargetStimulusLocations[0].HorizontalAzimuth > 0)
            {
                // the sound source is to the right, head turn to the left
                MainGrid.SetColumn(TestWordMatrix, 0);
                MainGrid.SetColumn(OrderGrid, 1);

            }
            else
            {
                // the sound source is to the left, head turn to the right
                MainGrid.SetColumn(TestWordMatrix, 4);
                MainGrid.SetColumn(OrderGrid, 3);

            }

            TestWordGrid.IsVisible = true;
            TestWordMatrix.IsVisible = true;

        }


        public override void ShowResponseAlternatives(List<List<SpeechTestResponseAlternative>> ResponseAlternatives)
        {

            List<SpeechTestResponseAlternative> localResponseAlternatives1 = ResponseAlternatives[0];
            List<SpeechTestResponseAlternative> localResponseAlternatives2 = ResponseAlternatives[1];
            List<SpeechTestResponseAlternative> localResponseAlternatives3 = ResponseAlternatives[2];
            List<SpeechTestResponseAlternative> localResponseAlternatives4 = ResponseAlternatives[3];
            List<SpeechTestResponseAlternative> localResponseAlternatives5 = ResponseAlternatives[4];

            TestWordButton1_1.Text = localResponseAlternatives5[0].Spelling;
            TestWordButton1_2.Text = localResponseAlternatives5[1].Spelling;
            TestWordButton1_3.Text = localResponseAlternatives5[2].Spelling;

            TestWordButton2_1.Text = localResponseAlternatives4[0].Spelling;
            TestWordButton2_2.Text = localResponseAlternatives4[1].Spelling;
            TestWordButton2_3.Text = localResponseAlternatives4[2].Spelling;

            TestWordButton3_1.Text = localResponseAlternatives3[0].Spelling;
            TestWordButton3_2.Text = localResponseAlternatives3[1].Spelling;
            TestWordButton3_3.Text = localResponseAlternatives3[2].Spelling;

            TestWordButton4_1.Text = localResponseAlternatives2[0].Spelling;
            TestWordButton4_2.Text = localResponseAlternatives2[1].Spelling;
            TestWordButton4_3.Text = localResponseAlternatives2[2].Spelling;

            TestWordButton5_1.Text = localResponseAlternatives1[0].Spelling;
            TestWordButton5_2.Text = localResponseAlternatives1[1].Spelling;
            TestWordButton5_3.Text = localResponseAlternatives1[2].Spelling;

            TestWordButton1_1.IsEnabled = false;
            TestWordButton1_2.IsEnabled = false;
            TestWordButton1_3.IsEnabled = false;

            TestWordButton2_1.IsEnabled = false;
            TestWordButton2_2.IsEnabled = false;
            TestWordButton2_3.IsEnabled = false;

            TestWordButton3_1.IsEnabled = false;
            TestWordButton3_2.IsEnabled = false;
            TestWordButton3_3.IsEnabled = false;

            TestWordButton4_1.IsEnabled = false;
            TestWordButton4_2.IsEnabled = false;
            TestWordButton4_3.IsEnabled = false;

            TestWordButton5_1.IsEnabled = false;
            TestWordButton5_2.IsEnabled = false;
            TestWordButton5_3.IsEnabled = false;

            TestWordButton1_1.IsVisible = true;
            TestWordButton1_2.IsVisible = true;
            TestWordButton1_3.IsVisible = true;

            TestWordButton2_1.IsVisible = true;
            TestWordButton2_2.IsVisible = true;
            TestWordButton2_3.IsVisible = true;

            TestWordButton3_1.IsVisible = true;
            TestWordButton3_2.IsVisible = true;
            TestWordButton3_3.IsVisible = true;

            TestWordButton4_1.IsVisible = true;
            TestWordButton4_2.IsVisible = true;
            TestWordButton4_3.IsVisible = true;

            TestWordButton5_1.IsVisible = true;
            TestWordButton5_2.IsVisible = true;
            TestWordButton5_3.IsVisible = true;

        }

        public override void ShowVisualCue()
        {

            // Using this method also to unlock the testword response buttons when each test word is presented (so that they cannot be clicked before the test word is presented)

            VisualCueCount += 1;

            switch (VisualCueCount)
            {
                case 1:
                    TestWordButton5_1.IsEnabled = true;
                    TestWordButton5_2.IsEnabled = true;
                    TestWordButton5_3.IsEnabled = true;
                    Circle1.IsVisible = true;
                    break;
                case 2:
                    TestWordButton4_1.IsEnabled = true;
                    TestWordButton4_2.IsEnabled = true;
                    TestWordButton4_3.IsEnabled = true;
                    Circle2.IsVisible = true;
                    break;
                case 3:
                    TestWordButton3_1.IsEnabled = true;
                    TestWordButton3_2.IsEnabled = true;
                    TestWordButton3_3.IsEnabled = true;
                    Circle3.IsVisible = true;
                    break;
                case 4:
                    TestWordButton2_1.IsEnabled = true;
                    TestWordButton2_2.IsEnabled = true;
                    TestWordButton2_3.IsEnabled = true;
                    Circle4.IsVisible = true;
                    break;
                case 5:
                    TestWordButton1_1.IsEnabled = true;
                    TestWordButton1_2.IsEnabled = true;
                    TestWordButton1_3.IsEnabled = true;
                    Circle5.IsVisible = true;

                    // Setting ResponseIsGiven as late as here to ensure that no responses is sent before this time (could likely also be done right as the start of the trial)
                    ResponseIsGiven = false;

                    break;
                default:
                    break;
            }

        }

        public override void StopAllTimers()
        {
            ResetGuiTimer.Stop();
        }

        public async override void UpdateTestFormProgressbar(int Value, int Maximum, int Minimum)
        {
            ProgressFrame.IsVisible = true;
            double range = Maximum - Minimum;
            double progressProp = Value / range;
            await PtcProgressBar.ProgressTo(progressProp, 50, Easing.Linear);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            ResizeStuff(width, height);

        }

        private void ResizeStuff(double width, double height)
        {

            if (TestWordButton1_1 != null) // This basically checks if the View has been initialized
            {

                var textSize = System.Math.Round(width / 32);

                TestWordButton1_1.FontSize = textSize;
                TestWordButton1_2.FontSize = textSize;
                TestWordButton1_3.FontSize = textSize;

                TestWordButton2_1.FontSize = textSize;
                TestWordButton2_2.FontSize = textSize;
                TestWordButton2_3.FontSize = textSize;

                TestWordButton3_1.FontSize = textSize;
                TestWordButton3_2.FontSize = textSize;
                TestWordButton3_3.FontSize = textSize;

                TestWordButton4_1.FontSize = textSize;
                TestWordButton4_2.FontSize = textSize;
                TestWordButton4_3.FontSize = textSize;

                TestWordButton5_1.FontSize = textSize;
                TestWordButton5_2.FontSize = textSize;
                TestWordButton5_3.FontSize = textSize;

                float CircleScale = 1.2f;

                Circle1.WidthRequest = textSize * CircleScale;
                Circle1.HeightRequest = textSize * CircleScale;

                Circle2.WidthRequest = textSize * CircleScale;
                Circle2.HeightRequest = textSize * CircleScale;

                Circle3.WidthRequest = textSize * CircleScale;
                Circle3.HeightRequest = textSize * CircleScale;

                Circle4.WidthRequest = textSize * CircleScale;
                Circle4.HeightRequest = textSize * CircleScale;

                Circle5.WidthRequest = textSize * CircleScale;
                Circle5.HeightRequest = textSize * CircleScale;

                foreach (var item in OrderGrid.Children)
                {
                    if (item is Label)
                    {
                        Label label = (Label)item;
                        label.FontSize = textSize;
                    }
                }
            }
        }

        private void ButtonButton_Clicked(object sender, EventArgs e)
        {

            Button clickedButton = (Button)sender;

            // hides the control in which the button lays
            Grid ParentGrid = (Grid)clickedButton.Parent;

            foreach (Button button in ParentGrid.Children)
            {
                if (ReferenceEquals(button, clickedButton) == false)
                {
                    button.IsVisible = false;
                }
                else
                {
                    button.IsEnabled = false;
                }
            }

            ResponseCount += 1;
            if (ResponseCount == 5)
            {

                // Sending the reply
                SendReply();

            }

        }

        private string GetRowResponse(Grid TestWordGrid)
        {
            int visibleCount = 0;
            string response = "";
            foreach (var item in TestWordGrid.Children)
            {
                if (item is Button)
                {
                    Button button = (Button)item;
                    if (button.IsVisible == true)
                    {
                        visibleCount += 1;
                        response = button.Text;
                    }
                }
            }

            if (visibleCount == 1)
            {
                return response;
            }
            else
            {
                // Returning an empty response since no reponse was selected on this row.
                return "";
            }
        }

        private void SendReply()
        {

            //Messager.MsgBoxAsync("New Replies" + GetRowResponse(TestWordGrid1));

                //Messager.MsgBoxAsync("In Lock" + GetRowResponse(TestWordGrid1));

                if (ResponseIsGiven == true)
                {
                    return;
                }
                ResponseIsGiven = true;

                // Collecting responses (including missing), adding from bottom to top
                ReplyList.Add(GetRowResponse(TestWordGrid5));
                ReplyList.Add(GetRowResponse(TestWordGrid4));
                ReplyList.Add(GetRowResponse(TestWordGrid3));
                ReplyList.Add(GetRowResponse(TestWordGrid2));
                ReplyList.Add(GetRowResponse(TestWordGrid1));

                // Copies the responses to a new list (so that the reply has its own instance of the list)
                List<string> ReplyListCopy = new List<string>();
                for (int i = 0; i < ReplyList.Count; i++)
                {
                    ReplyListCopy.Add(ReplyList[i]);
                }

                // Storing the raw response
                SpeechTestInputEventArgs args = new SpeechTestInputEventArgs();
                args.LinguisticResponses = ReplyListCopy;
                args.LinguisticResponseTime = DateTime.Now;

            // Raising the ResponseGiven event in the base class.
            // Note that this is done on a background thread that returns to the main thread after a short delay to allow the GUI to be updated.
            OnResponseGiven(args);

            // starting timer that hides everything
            ResetGuiTimer.Start();

        }               

    }

    public class ArrowDrawable : IDrawable
    {

        private GraphicsView parentView; // Reference to the parent GraphicsView

        private float transitionHeightRatio = 0.9f; // 

        /// <summary>
        /// Get or set the ratio of the shaft height to the total arrow height
        /// </summary>
        public float TransitionHeightRatio
        {
            get { return transitionHeightRatio; }   
            set {
                if (transitionHeightRatio != value)
                {
                    transitionHeightRatio = value;
                    parentView?.Invalidate(); // Trigger a redraw of the GraphicsView
                }
            }
        }

        public Color background = Color.FromArgb("#FFFF80");

        public Color Background
        {
            get { return background; }
            set
            {
                background = value;
                parentView?.Invalidate(); // Trigger a redraw of the GraphicsView
            }
        }

        public ArrowDrawable(GraphicsView view)
        {
            parentView = view;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // Set up the drawing properties
            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 2;

            // Dynamically calculate arrow dimensions based on the dirtyRect size
            float centerX = dirtyRect.Width / 2;
            float centerY = dirtyRect.Height / 2;

            // Arrow dimensions relative to the GraphicsView
            float arrowWidth = dirtyRect.Width * 0.90f;        // 100% of the width
            float arrowHeight = dirtyRect.Height * 0.96f;      // 100% of the height
            float shaftWidth = arrowWidth * 0.5f;           // 50% of arrow width for the shaft
            float centerShiftY = (TransitionHeightRatio - 0.5f) * arrowHeight; // Adjusts arrowhead/shaft transition

            // Start drawing the arrow path
            PathF arrowPath = new PathF();

            PointF point1 = new PointF(centerX, centerY - arrowHeight / 2);
            PointF point2 = new PointF(centerX - arrowWidth / 2, centerY - centerShiftY);
            PointF point3 = new PointF(centerX - shaftWidth / 2, centerY - centerShiftY);
            PointF point4 = new PointF(centerX - shaftWidth / 2, centerY + arrowHeight / 2);
            PointF point5 = new PointF(centerX + shaftWidth / 2, centerY + arrowHeight / 2);
            PointF point6 = new PointF(centerX + shaftWidth / 2, centerY - centerShiftY);
            PointF point7 = new PointF(centerX + arrowWidth / 2, centerY - centerShiftY);

            arrowPath.MoveTo(point1);
            arrowPath.LineTo(point2);
            arrowPath.LineTo(point3);
            arrowPath.LineTo(point4);
            arrowPath.LineTo(point5);
            arrowPath.LineTo(point6);
            arrowPath.LineTo(point7);

            arrowPath.Close();


            // Fill the arrow with color
            canvas.StrokeSize = 8;
            canvas.StrokeLineJoin = LineJoin.Round;

            canvas.FillColor = background;
            canvas.FillPath(arrowPath);
            canvas.SetShadow(new SizeF(0, 0), 10, Colors.Grey);

            // Optional: Add an outline
            canvas.StrokeColor = background;
            canvas.DrawPath(arrowPath);
        }
    }

}

