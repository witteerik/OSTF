using STFN;

namespace OstfTabletSuite
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();

            //MainPage = new AppShell();

            // Adding an event handler that disposes the sound player (clearing temporary wave files) (TODO: maybe there are better ways to do this...?)
            MainPage.Unloaded += MainPage_Unloaded;

        }

        private void MainPage_Unloaded(object sender, EventArgs e)
        {

            switch (OstfBase.CurrentMediaPlayerType)
            {
                case OstfBase.MediaPlayerTypes.PaBased:
                    OstfBase.TerminateOSTF();
                    break;
                case OstfBase.MediaPlayerTypes.AudioTrackBased:
                    OstfBase.TerminateOSTF();
                    break;
                case OstfBase.MediaPlayerTypes.Default:
                    break;
                default:
                    break;
            }
        }

    }
}