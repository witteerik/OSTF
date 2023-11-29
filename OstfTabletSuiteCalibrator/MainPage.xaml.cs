using STFN;

namespace OstfTabletSuiteCalibrator
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();

            Messager.NewMessage += DisplayMessage;

        }


        private async void DisplayMessage(string title, string message, string cancelButtonText)
        {
            await DisplayAlert(title, message, cancelButtonText);
        }


    }
}