using STFN;

namespace OstfTabletSuiteCalibrator
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

            // Adding an event handler that disposes the sound player (clearing temporary wave files) (TODO: maybe there are better ways to do this...?)
            MainPage.Unloaded += MainPage_Unloaded;

        }

        private void MainPage_Unloaded(object sender, EventArgs e)
        {
            if (OstfBase.SoundPlayer != null)
            {
                OstfBase.SoundPlayer.Dispose();
            }
        }

    }
}