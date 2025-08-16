using STFN.Core;
using STFN;
using System.Diagnostics;

namespace OstfTabletSuite
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();

            //MainPage = new AppShell();

            // Adding an event handler that disposes the sound player 
            MainPage.Unloaded += MainPage_Unloaded;

        }

        private void MainPage_Unloaded(object sender, EventArgs e)
        {
            OstfBase.TerminateOSTF();
        }



    }
}