using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Layouts;
using System.ComponentModel;
using System.Diagnostics;

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

            STFN.Messager.NewMessage += DisplayMessage;
            STFN.Messager.NewQuestion += DisplayBooleanQuestion;
            
        }
        private async void DisplayMessage(string title, string message, string cancelButtonText)
        {
            await DisplayAlert(title, message, cancelButtonText);
        }

        private async Task<bool> DisplayBooleanQuestion(string title, string question, string acceptButtonText, string cancelButtonText)
        {

            //See more at https://learn.microsoft.com/en-us/dotnet/maui/user-interface/pop-ups?view=net-maui-8.0

            bool answer = await DisplayAlert(title, question, acceptButtonText, cancelButtonText);
            return  answer;

        }

    }
}

