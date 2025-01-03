
using System.ComponentModel;
using Microsoft.Maui.Controls;
using STFN;
using STFN.Utils;
using STFN.SipTest;
using System.Reflection;
using Microsoft.Maui.Platform;

namespace STFM.Views
{

    [ToolboxItem(true)] // Marks this class as available for the Toolbox
    public partial class ResponseView_AdaptiveSiP2 : ResponseView
    {
        public ResponseView_AdaptiveSiP2()
        {
            // Loading xaml content manually since for some reason does not InitializeComponent exist
            //this.LoadFromXaml(typeof(ResponseView_AdaptiveSiP));
            InitializeComponent();

            //RightSideControl.IsVisible = false;
            LeftSideControl.IsVisible = false;
            MessageButton.IsVisible = false;

            // Assign the custom drawable to the GraphicsView
            ArrowView.Drawable = new ArrowDrawable(ArrowView);

            // Force redraw on size change
            ArrowView.SizeChanged += (s, e) => ArrowView.Invalidate();
        }

        Color LampOnColor = Color.FromArgb("#00FFFA");
        Color LampOnBorderColor = Color.FromArgb("#00A7A3");
        Color LampOffColor = Color.FromArgb("#585858");
        Color LampOffBorderColor = Color.FromArgb("#4E4E4E");
        Color LampRedColor = Color.FromArgb("#FF0000");
        Color LampRedBorderColor = Color.FromArgb("#D95B3D");
        Color RedButtonColor = Color.FromArgb("#FF0000");
        Color DefaultButtonColor = Color.FromArgb("#FFFF80");
        Color Button_DisabledColor = Color.FromArgb("#D0D0D0");
        
        private STFN.Utils.Constants.Sides CurrentSide = STFN.Utils.Constants.Sides.Left;

        private int ResponseCount  = 0;
        private int VisualCueCount = 0;

        private List<string> ReplyList = new List<string>();

        public override void AddSourceAlternatives(STFM.Views.ResponseView.VisualizedSoundSource[] soundSources)
        {
            //throw new NotImplementedException();
        }

        public override void HideAllItems()
        {
            //RightSideControl.IsVisible = false;
            LeftSideControl.IsVisible = false;
            MessageButton.IsVisible = false;
        }

        public override void HideVisualCue()
        {
            //throw new NotImplementedException();
        }


        //public override void InitializeNewTrial()
        //{

        //    if (MainThread.IsMainThread == false)
        //    {
        //        MainThread.BeginInvokeOnMainThread(() =>
        //        {
        //            Inner_InitializeNewTrial();
        //        });
        //        return;
        //    }

        //}

        public override void InitializeNewTrial()
        {

            StopAllTimers();

            //RightSideControl.IsVisible = false;
            LeftSideControl.IsVisible = false;
            MessageButton.IsVisible = false;

            ResponseCount  = 0;
            VisualCueCount = 0;

            Circle1.IsVisible = false;
            Circle2.IsVisible = false;
            Circle3.IsVisible = false;
            Circle4.IsVisible = false;
            Circle5.IsVisible = false;

            LeftButton1_1.Background = DefaultButtonColor;
            LeftButton1_2.Background = DefaultButtonColor;
            LeftButton1_3.Background = DefaultButtonColor;
            //RightButton1_1.Background = DefaultButtonColor;
            //RightButton1_2.Background = DefaultButtonColor;
            //RightButton1_3.Background = DefaultButtonColor;

            LeftSideRow1.IsVisible = true;
            LeftSideRow2.IsVisible = true;
            LeftSideRow3.IsVisible = true;
            LeftSideRow4.IsVisible = true;
            LeftSideRow5.IsVisible = true;

            ReplyList.Clear();

            ArrowDrawable arrowDrawable = (ArrowDrawable)ArrowView.Drawable;
            arrowDrawable.TransitionHeightRatio = 0.86f;

            arrowDrawable.Background = Colors.DarkSlateGray; 

        }

        public override void ResponseTimesOut()
        {

            LeftButton1_1.Background = RedButtonColor;
            LeftButton1_2.Background = RedButtonColor;
            LeftButton1_3.Background = RedButtonColor;
            //RightButton1_1.Background = RedButtonColor;
            //RightButton1_2.Background = RedButtonColor;
            //RightButton1_3.Background = RedButtonColor;

            SendReply();

        }

        public override void ShowMessage(string Message)
        {

            StopAllTimers();
            MessageButton.IsVisible = true;
            MessageButton.Text = Message;
            MessageButton.FontSize = 25;

        }

        public override void ShowResponseAlternativePositions(List<List<SpeechTestResponseAlternative>> ResponseAlternatives)
        {

            // Clearing all texts on the buttons
            //RightButton1_1.Text = "";
            //RightButton1_2.Text = "";
            //RightButton1_3.Text = "";

            LeftButton1_1.Text = "";
            LeftButton1_2.Text = "";
            LeftButton1_3.Text = "";

            LeftButton2_1.Text = "";
            LeftButton2_2.Text = "";
            LeftButton2_3.Text = "";

            LeftButton3_1.Text = "";
            LeftButton3_2.Text = "";
            LeftButton3_3.Text = "";

            LeftButton4_1.Text = "";
            LeftButton4_2.Text = "";
            LeftButton4_3.Text = "";

            LeftButton5_1.Text = "";
            LeftButton5_2.Text = "";
            LeftButton5_3.Text = "";

            // Calling resize on every presentation (could be done only initially)
            ResizeStuff(this.Width, this.Height);

            List<SpeechTestResponseAlternative> localResponseAlternatives = ResponseAlternatives[0];

            // Reading which side to put the response alternatives, based on the first one
            SipTrial parentTestTrial = (SipTrial)localResponseAlternatives[0].ParentTestTrial.SubTrials[0];
            if (parentTestTrial.TargetStimulusLocations[0].HorizontalAzimuth > 0)
            {
                // the sound source is to the right, head turn to the left
                CurrentSide = STFN.Utils.Constants.Sides.Right;

                //RightSideControl.IsVisible = false;
                LeftSideControl.IsVisible = true;

                MainGrid.SetColumn(LeftSideControl, 0);
                MainGrid.SetColumn(LeftOrderGrid, 1);

            }
            else
            {
                // the sound source is to the left, head turn to the right
                CurrentSide = STFN.Utils.Constants.Sides.Left;

                //RightSideControl.IsVisible = true;
                LeftSideControl.IsVisible = true;
                //LeftSideControl.IsVisible = false;

                MainGrid.SetColumn(LeftSideControl, 4);
                MainGrid.SetColumn(LeftOrderGrid, 3);

            }

        }


        public override void ShowResponseAlternatives(List<List<SpeechTestResponseAlternative>> ResponseAlternatives)
        {

            List<SpeechTestResponseAlternative> localResponseAlternatives1 = ResponseAlternatives[0];
            List<SpeechTestResponseAlternative> localResponseAlternatives2 = ResponseAlternatives[1];
            List<SpeechTestResponseAlternative> localResponseAlternatives3 = ResponseAlternatives[2];
            List<SpeechTestResponseAlternative> localResponseAlternatives4 = ResponseAlternatives[3];
            List<SpeechTestResponseAlternative> localResponseAlternatives5 = ResponseAlternatives[4];

            //switch (CurrentSide)
            //{
            //    case STFN.Utils.Constants.Sides.Left:

                    LeftButton1_1.Text = localResponseAlternatives5[0].Spelling;
                    LeftButton1_2.Text = localResponseAlternatives5[1].Spelling;
                    LeftButton1_3.Text = localResponseAlternatives5[2].Spelling;

                    LeftButton2_1.Text = localResponseAlternatives4[0].Spelling;
                    LeftButton2_2.Text = localResponseAlternatives4[1].Spelling;
                    LeftButton2_3.Text = localResponseAlternatives4[2].Spelling;

                    LeftButton3_1.Text = localResponseAlternatives3[0].Spelling;
                    LeftButton3_2.Text = localResponseAlternatives3[1].Spelling;
                    LeftButton3_3.Text = localResponseAlternatives3[2].Spelling;

                    LeftButton4_1.Text = localResponseAlternatives2[0].Spelling;
                    LeftButton4_2.Text = localResponseAlternatives2[1].Spelling;
                    LeftButton4_3.Text = localResponseAlternatives2[2].Spelling;

                    LeftButton5_1.Text = localResponseAlternatives1[0].Spelling;
                    LeftButton5_2.Text = localResponseAlternatives1[1].Spelling;
                    LeftButton5_3.Text = localResponseAlternatives1[2].Spelling;

                    LeftButton1_1.IsEnabled = true;
                    LeftButton1_2.IsEnabled = true;
                    LeftButton1_3.IsEnabled = true;

                    LeftButton2_1.IsEnabled = true;
                    LeftButton2_2.IsEnabled = true;
                    LeftButton2_3.IsEnabled = true;

                    LeftButton3_1.IsEnabled = true;
                    LeftButton3_2.IsEnabled = true;
                    LeftButton3_3.IsEnabled = true;

                    LeftButton4_1.IsEnabled = true;
                    LeftButton4_2.IsEnabled = true;
                    LeftButton4_3.IsEnabled = true;

                    LeftButton5_1.IsEnabled = true;
                    LeftButton5_2.IsEnabled = true;
                    LeftButton5_3.IsEnabled = true;

                    LeftButton1_1.IsVisible = true;
                    LeftButton1_2.IsVisible = true;
                    LeftButton1_3.IsVisible = true;

                    LeftButton2_1.IsVisible = true;
                    LeftButton2_2.IsVisible = true;
                    LeftButton2_3.IsVisible = true;

                    LeftButton3_1.IsVisible = true;
                    LeftButton3_2.IsVisible = true;
                    LeftButton3_3.IsVisible = true;

                    LeftButton4_1.IsVisible = true;
                    LeftButton4_2.IsVisible = true;
                    LeftButton4_3.IsVisible = true;

                    LeftButton5_1.IsVisible = true;
                    LeftButton5_2.IsVisible = true;
                    LeftButton5_3.IsVisible = true;

            //        break;
            //    case STFN.Utils.Constants.Sides.Right:

            //        //RightButton1_1.Text = localResponseAlternatives[0].Spelling;
            //        //RightButton1_2.Text = localResponseAlternatives[1].Spelling;
            //        //RightButton1_3.Text = localResponseAlternatives[2].Spelling;

            //        //RightButton1_1.IsEnabled = true;
            //        //RightButton1_2.IsEnabled = true;
            //        //RightButton1_3.IsEnabled = true;

            //        break;
            //    default:
            //        break;
            //}

        }

        public override void ShowVisualCue()
        {

            VisualCueCount += 1;

            switch (VisualCueCount)
            {
                case 1:
                    Circle1.IsVisible = true;
                    break;
                case 2:
                    Circle2.IsVisible = true;
                    break;
                case 3:
                    Circle3.IsVisible = true;
                    break;
                case 4:
                    Circle4.IsVisible = true;
                    break;
                case 5:
                    Circle5.IsVisible = true;
                    break;
                default:
                    break;
            }

        }

        public override void StopAllTimers()
        {
            //SendReplyTimer.Stop();
        }

        public override void UpdateTestFormProgressbar(int Value, int Maximum, int Minimum)
        {
            //throw new NotImplementedException();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            ResizeStuff(width, height);

        }

        private void ResizeStuff(double width, double height)
        {
            var textSize = System.Math.Round(width / 32);

            if (LeftButton1_1 != null) // This basically checks if the View has been initialized
            {

                LeftButton1_1.FontSize = textSize;
                LeftButton1_2.FontSize = textSize;
                LeftButton1_3.FontSize = textSize;

                LeftButton2_1.FontSize = textSize;
                LeftButton2_2.FontSize = textSize;
                LeftButton2_3.FontSize = textSize;

                LeftButton3_1.FontSize = textSize;
                LeftButton3_2.FontSize = textSize;
                LeftButton3_3.FontSize = textSize;

                LeftButton4_1.FontSize = textSize;
                LeftButton4_2.FontSize = textSize;
                LeftButton4_3.FontSize = textSize;

                LeftButton5_1.FontSize = textSize;
                LeftButton5_2.FontSize = textSize;
                LeftButton5_3.FontSize = textSize;

                //RightButton1_1.FontSize = textSize;
                //RightButton1_2.FontSize = textSize;
                //RightButton1_3.FontSize = textSize;

                foreach (var item in LeftOrderGrid.Children)
                {
                    if (item is Label)
                    {
                        Label label = (Label)item;
                        label.FontSize = textSize;
                    }

                }

            }

        }

        private async void ButtonButton_Clicked(object sender, EventArgs e)
        {

            if (ReplyList.Count >= 5)
            {
                // This should not happen but, if it does, this call is blocked and a reply is sent

                // Sending the reply on on a background thread
                await Task.Run(() => SendReply());

                return;
            }

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
                    //button.IsEnabled = false;
                }
            }

            ResponseCount += 1;
            if (ResponseCount == 5)
            {

                switch (CurrentSide)
                {
                    case STFN.Utils.Constants.Sides.Left:

                        // Adding from bottom to top
                        ReplyList.Add(GetRowResponse(LeftSideGrid5));
                        ReplyList.Add(GetRowResponse(LeftSideGrid4));
                        ReplyList.Add(GetRowResponse(LeftSideGrid3));
                        ReplyList.Add(GetRowResponse(LeftSideGrid2));
                        ReplyList.Add(GetRowResponse(LeftSideGrid1));

                        break;
                    case STFN.Utils.Constants.Sides.Right:

                        // Adding from bottom to top
                        ReplyList.Add(GetRowResponse(LeftSideGrid5));
                        ReplyList.Add(GetRowResponse(LeftSideGrid4));
                        ReplyList.Add(GetRowResponse(LeftSideGrid3));
                        ReplyList.Add(GetRowResponse(LeftSideGrid2));
                        ReplyList.Add(GetRowResponse(LeftSideGrid1));

                        break;
                    default:
                        break;
                }


                // Sending the reply on on a background thread
                await Task.Run(() => SendReply());
            }

        }

        private string GetRowResponse(Grid LeftSideGrid1)
        {
            foreach (var item in LeftSideGrid1.Children)
            {
                int visibleCount = 0;
                string response = "";
                if (item is Button)
                {
                    Button button = (Button)item;
                    if (button.IsVisible == true)
                    {
                        visibleCount += 1;
                        response = button.Text;
                    }
                }

                if (visibleCount == 1) {
                    return response; 
                }
            }

            // Returning an empty response since no reponse was selected on this row.
            return "";
        }

        private void SendReply()
        {

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

            // Raising the Response given event in the base class
            OnResponseGiven(args);

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

