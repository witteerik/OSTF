using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Layouts;

namespace OstfTabletSuite
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        //public static STFM.SoundPlayer MainPageSoundPlayer = null;
        //public static MediaElement MainPageMediaElement = null;

        public MainPage()
        {
            InitializeComponent();

        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            
            //await STFM.StfmBase.InitiateSTFM(STF_VerticalStackLayout);



            //count++;
            //if (count == 1)
            //    CounterBtn.Text = $"Clicked {count} time";
            //else
            //    CounterBtn.Text = $"Clicked {count} times";
            //SemanticScreenReader.Announce(CounterBtn.Text);
        }

        private void OnCounterClicked2(object sender, EventArgs e)
        {
            STFM.StfmBase.SetupTest();

        }

        private void playAgainClicked2(object sender, EventArgs e)
        {

            //MainPageSoundPlayer.mediaElement1.Source = new Uri("/data/user/0/com.companyname.ostftabletsuite/cache/TempSound1.wav");
            //MainPageSoundPlayer.mediaElement2.Source = new Uri("/data/user/0/com.companyname.ostftabletsuite/cache/TempSound2.wav");

            //MainPageSoundPlayer.mediaElement1.Volume = 1;

            //MainPageMediaElement.Source = new Uri("/data/user/0/com.companyname.ostftabletsuite/cache/TempSound2.wav");
            //MainPageMediaElement.ShouldLoopPlayback = true;
            //MainPageMediaElement.Play();

            //MainPageSoundPlayer.mediaElement1.Play();


            STFM.StfmBase.playAgain();

        }
        

    }
}