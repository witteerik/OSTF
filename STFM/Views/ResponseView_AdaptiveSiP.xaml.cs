
using System.ComponentModel;
using Microsoft.Maui.Controls;
using STFN;
using STFN.Utils;
using STFN.SipTest;

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

            RightSideControl.IsVisible = false;
            LeftSideControl.IsVisible = false;
            MessageButton.IsVisible = false;

            // Creating a timer for sending responses
            SendReplyTimer = Microsoft.Maui.Controls.Application.Current.Dispatcher.CreateTimer();
            SendReplyTimer.Interval = TimeSpan.FromMilliseconds(200);
            SendReplyTimer.Tick += SendReply;
            SendReplyTimer.IsRepeating = false;

        }

        private IDispatcherTimer SendReplyTimer;

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

        private int ButtonClicks_Left_1 = 0;
        private int ButtonClicks_Left_2 = 0;
        private int ButtonClicks_Left_3 = 0;
        private int ButtonClicks_Right_1 = 0;
        private int ButtonClicks_Right_2 = 0;
        private int ButtonClicks_Right_3 = 0;

        private int RightSideLampsOn = 0;
        private int LeftSideLampsOn = 0;

        private List<string> ReplyList = new List<string>();

        public override void AddSourceAlternatives(STFM.Views.ResponseView.VisualizedSoundSource[] soundSources)
        {
            //throw new NotImplementedException();
        }

        public override void HideAllItems()
        {
            RightSideControl.IsVisible = false;
            LeftSideControl.IsVisible = false;
            MessageButton.IsVisible = false;
        }

        public override void HideVisualCue()
        {
            //throw new NotImplementedException();
        }

        public override void InitializeNewTrial()
        {
            StopAllTimers();

            TurnOffLamps();

            RightSideControl.IsVisible = false;
            LeftSideControl.IsVisible = false;
            MessageButton.IsVisible = false;

            RightSideLampsOn = 0;
            LeftSideLampsOn = 0;

            ButtonClicks_Left_1 = 0;
            ButtonClicks_Left_2 = 0;
            ButtonClicks_Left_3 = 0;
            ButtonClicks_Right_1 = 0;
            ButtonClicks_Right_2 = 0;
            ButtonClicks_Right_3 = 0;

            LeftButton1.Background = DefaultButtonColor;
            LeftButton2.Background = DefaultButtonColor;
            LeftButton3.Background = DefaultButtonColor;
            RightButton1.Background = DefaultButtonColor;
            RightButton2.Background = DefaultButtonColor;
            RightButton3.Background = DefaultButtonColor;

            TurnOffLamps();

            ReplyList.Clear();

        }

        public override void ResponseTimesOut()
        {

            TurnLampsRed();

            LeftButton1.Background = RedButtonColor;
            LeftButton2.Background = RedButtonColor;
            LeftButton3.Background = RedButtonColor;
            RightButton1.Background = RedButtonColor;
            RightButton2.Background = RedButtonColor;
            RightButton3.Background = RedButtonColor;

            SendReplyTimer.Start();

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
            RightButton1.Text = "";
            RightButton2.Text = "";
            RightButton3.Text = "";

            LeftButton1.Text = "";
            LeftButton2.Text = "";
            LeftButton3.Text = "";

            // Calling resize on every presentation (could be done only initially)
            ResizeStuff(this.Width, this.Height);

            List<SpeechTestResponseAlternative> localResponseAlternatives = ResponseAlternatives[0];

            // Reading which side to put the response alternatives, based on the first one
            SipTrial parentTestTrial = (SipTrial)localResponseAlternatives[0].ParentTestTrial;
            if (parentTestTrial.TargetStimulusLocations[0].HorizontalAzimuth > 0)
            {
                // the sound source is to the right, head turn to the left
                CurrentSide = STFN.Utils.Constants.Sides.Left;

                RightSideControl.IsVisible = false;
                LeftSideControl.IsVisible = true;
            }
            else
            {
                // the sound source is to the left, head turn to the right
                CurrentSide = STFN.Utils.Constants.Sides.Right;

                RightSideControl.IsVisible = true;
                LeftSideControl.IsVisible = false;
            }

        }


        public override void ShowResponseAlternatives(List<List<SpeechTestResponseAlternative>> ResponseAlternatives)
        {

            List<SpeechTestResponseAlternative> localResponseAlternatives = ResponseAlternatives[0];

            switch (CurrentSide)
            {
                case STFN.Utils.Constants.Sides.Left:

                    LeftButton1.Text = localResponseAlternatives[0].Spelling;
                    LeftButton2.Text = localResponseAlternatives[1].Spelling;
                    LeftButton3.Text = localResponseAlternatives[2].Spelling;

                    LeftButton1.IsEnabled = true;
                    LeftButton2.IsEnabled = true;
                    LeftButton3.IsEnabled = true;

                    break;
                case STFN.Utils.Constants.Sides.Right:

                    RightButton1.Text = localResponseAlternatives[0].Spelling;
                    RightButton2.Text = localResponseAlternatives[1].Spelling;
                    RightButton3.Text = localResponseAlternatives[2].Spelling;

                    RightButton1.IsEnabled = true;
                    RightButton2.IsEnabled = true;
                    RightButton3.IsEnabled = true;

                    break;
                default:
                    break;
            }

        }

        public override void ShowVisualCue()
        {
            //throw new NotImplementedException();
        }

        public override void StopAllTimers()
        {
            SendReplyTimer.Stop();
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
            var textSize = System.Math.Round(height / 12);

            if (LeftButton1 != null)
            {


                LeftButton1.FontSize = textSize;
                LeftButton2.FontSize = textSize;
                LeftButton3.FontSize = textSize;

                RightButton1.FontSize = textSize;
                RightButton2.FontSize = textSize;
                RightButton3.FontSize = textSize;

                var lampsize = System.Math.Round(height / 20);
                float halfLampSize = (float)(lampsize / 2);

                // Updating size
                LeftLamp1.HeightRequest = lampsize;
                LeftLamp1.WidthRequest = lampsize;
                LeftLamp2.HeightRequest = lampsize;
                LeftLamp2.WidthRequest = lampsize;
                LeftLamp3.HeightRequest = lampsize;
                LeftLamp3.WidthRequest = lampsize;
                LeftLamp4.HeightRequest = lampsize;
                LeftLamp4.WidthRequest = lampsize;

                RightLamp1.HeightRequest = lampsize;
                RightLamp1.WidthRequest = lampsize;
                RightLamp2.HeightRequest = lampsize;
                RightLamp2.WidthRequest = lampsize;
                RightLamp3.HeightRequest = lampsize;
                RightLamp3.WidthRequest = lampsize;
                RightLamp4.HeightRequest = lampsize;
                RightLamp4.WidthRequest = lampsize;

                // Updating corner radius
                LeftLamp1.CornerRadius = halfLampSize;
                LeftLamp2.CornerRadius = halfLampSize;
                LeftLamp3.CornerRadius = halfLampSize;
                LeftLamp4.CornerRadius = halfLampSize;
                RightLamp1.CornerRadius = halfLampSize;
                RightLamp2.CornerRadius = halfLampSize;
                RightLamp3.CornerRadius = halfLampSize;
                RightLamp4.CornerRadius = halfLampSize;


            }

        }

        private void ButtonButton_Clicked(object sender, EventArgs e)
        {

            Button clickedButton = (Button)sender;

            if (ReplyList.Count >= 4)
            {
                // This should not happen but, if it does, this call is blocked and a reply is sent
                SendReplyTimer.Start();
                return;
            }

            //Adding the clicked response
            ReplyList.Add(clickedButton.Text);

            if (clickedButton == LeftButton1) { ButtonClicks_Left_1 += 1; }
            if (clickedButton == LeftButton2) { ButtonClicks_Left_2 += 1; }
            if (clickedButton == LeftButton3) { ButtonClicks_Left_3 += 1; }

            if (clickedButton == RightButton1) { ButtonClicks_Right_1 += 1; }
            if (clickedButton == RightButton2) { ButtonClicks_Right_2 += 1; }
            if (clickedButton == RightButton3) { ButtonClicks_Right_3 += 1; }

            // Disables the buttons after 2 clicks
            if (ButtonClicks_Left_1 == 2) { LeftButton1.IsEnabled = false;  }
            if (ButtonClicks_Left_2 == 2) { LeftButton2.IsEnabled = false;  }
            if (ButtonClicks_Left_3 == 2) { LeftButton3.IsEnabled = false;  }
            if (ButtonClicks_Right_1 == 2) { RightButton1.IsEnabled = false; }
            if (ButtonClicks_Right_2 == 2) { RightButton2.IsEnabled = false; }
            if (ButtonClicks_Right_3 == 2) { RightButton3.IsEnabled = false; }

            // Emphasizes that the button should not be clicked by making it gray after three clicks
            if (ButtonClicks_Left_1 == 3) { LeftButton1.Background = Button_DisabledColor; }
            if (ButtonClicks_Left_2 == 3) { LeftButton2.Background = Button_DisabledColor; }
            if (ButtonClicks_Left_3 == 3) { LeftButton3.Background = Button_DisabledColor; }
            if (ButtonClicks_Right_1 == 3) { RightButton1.Background = Button_DisabledColor; }
            if (ButtonClicks_Right_2 == 3) { RightButton2.Background = Button_DisabledColor; }
            if (ButtonClicks_Right_3 == 3) { RightButton3.Background = Button_DisabledColor; }

            switch (CurrentSide)
            {
                case STFN.Utils.Constants.Sides.Left:
                    LeftSideLampsOn += 1;
                    UpdateLampsOn();
                    if (LeftSideLampsOn == 4) {SendReplyTimer.Start(); }
                    break;
                case STFN.Utils.Constants.Sides.Right:
                    RightSideLampsOn += 1;
                    UpdateLampsOn();
                    if (RightSideLampsOn == 4) { SendReplyTimer.Start(); }
                    break;
                default:
                    break;
            }

        }

        private void UpdateLampsOn()
        {

            switch (CurrentSide)
            {
                case STFN.Utils.Constants.Sides.Left:

                    if (LeftSideLampsOn > 0) { LeftLamp1.Background = LampOnColor; LeftLamp1.BorderColor = LampOnBorderColor; }
                    if (LeftSideLampsOn > 1) { LeftLamp2.Background = LampOnColor; LeftLamp2.BorderColor = LampOnBorderColor; }
                    if (LeftSideLampsOn > 2) { LeftLamp3.Background = LampOnColor; LeftLamp3.BorderColor = LampOnBorderColor; }
                    if (LeftSideLampsOn > 3) { LeftLamp4.Background = LampOnColor; LeftLamp4.BorderColor = LampOnBorderColor; }

                    break;
                case STFN.Utils.Constants.Sides.Right:

                    if (RightSideLampsOn > 0) { RightLamp1.Background = LampOnColor; RightLamp1.BorderColor = LampOnBorderColor; }
                    if (RightSideLampsOn > 1) { RightLamp2.Background = LampOnColor; RightLamp2.BorderColor = LampOnBorderColor; }
                    if (RightSideLampsOn > 2) { RightLamp3.Background = LampOnColor; RightLamp3.BorderColor = LampOnBorderColor; }
                    if (RightSideLampsOn > 3) { RightLamp4.Background = LampOnColor; RightLamp4.BorderColor = LampOnBorderColor; }

                    break;
                default:
                    break;
            }

        }

        private void TurnOffLamps()
        {

            LeftLamp1.Background = LampOffColor; LeftLamp1.BorderColor = LampOffBorderColor;
            LeftLamp2.Background = LampOffColor; LeftLamp2.BorderColor = LampOffBorderColor;
            LeftLamp3.Background = LampOffColor; LeftLamp3.BorderColor = LampOffBorderColor;
            LeftLamp4.Background = LampOffColor; LeftLamp4.BorderColor = LampOffBorderColor;
            RightLamp1.Background = LampOffColor; RightLamp1.BorderColor = LampOffBorderColor;
            RightLamp2.Background = LampOffColor; RightLamp2.BorderColor = LampOffBorderColor;
            RightLamp3.Background = LampOffColor; RightLamp3.BorderColor = LampOffBorderColor;
            RightLamp4.Background = LampOffColor; RightLamp4.BorderColor = LampOffBorderColor;

        }

        private void TurnLampsRed()
        {

            LeftLamp1.Background = LampRedColor; LeftLamp1.BorderColor = LampRedBorderColor;
            LeftLamp2.Background = LampRedColor; LeftLamp2.BorderColor = LampRedBorderColor;
            LeftLamp3.Background = LampRedColor; LeftLamp3.BorderColor = LampRedBorderColor;
            LeftLamp4.Background = LampRedColor; LeftLamp4.BorderColor = LampRedBorderColor;
            RightLamp1.Background = LampRedColor; RightLamp1.BorderColor = LampRedBorderColor;
            RightLamp2.Background = LampRedColor; RightLamp2.BorderColor = LampRedBorderColor;
            RightLamp3.Background = LampRedColor; RightLamp3.BorderColor = LampRedBorderColor;
            RightLamp4.Background = LampRedColor; RightLamp4.BorderColor = LampRedBorderColor;

        }

        private void SendReply(object sender, EventArgs e)
        {
                       

            // Filling up the response list with empty responses. This happens when the response time has ended before all responses has been given.
            if (ReplyList.Count < 4)
            {
                for (int i = 0; i < (4 - ReplyList.Count); i++)
                {
                    ReplyList.Add("");
                }
            }

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
}
