using Android.App;
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

            // Initializing the sound player

            //MainPageSoundPlayer = new STFM.SoundPlayer(STF_VerticalStackLayout);
            //MainPageSoundPlayer = new STFM.SoundPlayer();

            //MainPageMediaElement = new MediaElement();
            //MainPageMediaElement.HeightRequest = 400;
            //MainPageMediaElement.WidthRequest= 400;

            //STF_VerticalStackLayout.Children.Add(MainPageMediaElement);
            //STF_VerticalStackLayout.Children.Add(MainPageSoundPlayer.mediaElement1);
            //STF_VerticalStackLayout.Children.Add(MainPageSoundPlayer.mediaElement2);

            //ParentContainer.BackgroundColor = Colors.Beige;
            //try
            //{
            //    Uri myUri = new Uri("/data/user/0/com.companyname.ostftabletsuite/cache/TempSound1.wav");

            //    var picked = await FilePicker.PickAsync();
                
            //    MainPageMediaElement.Source = new Uri(picked.FullPath);
            //    MainPageMediaElement.Play();

            //}
            //catch (Exception)
            //{
            //    var c = 1;
            //    throw;
            //}
            //MainPageSoundPlayer.mediaElement1.Source = new Uri("/data/user/0/com.companyname.ostftabletsuite/cache/TempSound1.wav");
            //MainPageSoundPlayer.mediaElement2.Source = new Uri("/data/user/0/com.companyname.ostftabletsuite/cache/TempSound2.wav");

            //MainPageSoundPlayer.mediaElement1.Play();

            //return;


            await STFM.StfmBase.InitiateSTFM(STF_VerticalStackLayout);

            //STFM.StfmBase.SoundPlayer = MainPageSoundPlayer;

            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
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