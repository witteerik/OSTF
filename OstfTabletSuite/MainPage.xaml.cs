using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Layouts;
using STFN;
using System.ComponentModel;
using System.Diagnostics;

namespace OstfTabletSuite
{
    public partial class MainPage : ContentPage, IOstfGui
    {
        //public static STFM.SoundPlayer MainPageSoundPlayer = null;
        //public static MediaElement MainPageMediaElement = null;

        public MainPage()
        {
            InitializeComponent();

            STFN.Messager.OnNewMessage += DisplayMessage;
            STFN.Messager.OnNewAsyncMessage += DisplayMessageAsync;
            STFN.Messager.OnNewQuestion += DisplayBooleanQuestion;
            STFN.Messager.OnGetSaveFilePath += GetSaveFilePath;
            STFN.Messager.OnGetFolder += GetFolder;
            STFN.Messager.OnGetOpenFilePath += GetOpenFilePath;
            STFN.Messager.OnGetOpenFilePaths += GetOpenFilePaths;
            STFN.Messager.OnCloseAppRequest += CloseApp;

        }

        public void WelcomePageAllDone(object sender, EventArgs e)
        {

            this.Content = null;

            var mySpeechTest = new STFM.Views.SpeechTestView
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill
            };

            Content = mySpeechTest;

        }

        public void WelcomePageStartCalibrator(object sender, EventArgs e)
        {

            this.Content = null;

            var mySpeechTest = new STFM.Views.SpeechTestCalibrationView
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill
            };

            Content = mySpeechTest;

        }

        public async void DisplayMessage(string title, string message, string cancelButtonText)
        {
            await DisplayAlert(title, message, cancelButtonText);
        }

        
        public static void CloseApp()
        {
            Application.Current.Quit();
        }


        public async void DisplayMessageAsync(object sender, MessageEventArgs e)
        {
            await DisplayAlert(e.Title, e.Message, e.CancelButtonText);
            // Setting false as response
            e.TaskCompletionSource.SetResult(false);
        }


        public async void DisplayBooleanQuestion(object sender, QuestionEventArgs e)
        {

            //See more at https://learn.microsoft.com/en-us/dotnet/maui/user-interface/pop-ups?view=net-maui-8.0

            bool answer = await DisplayAlert(e.Title, e.Question, e.AcceptButtonText, e.CancelButtonText);

            e.TaskCompletionSource.SetResult(answer);

        }

        public void GetSaveFilePath(object sender, PathEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public void GetFolder(object sender, PathEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public void GetOpenFilePath(object sender, PathEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public void GetOpenFilePaths(object sender, PathsEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}

