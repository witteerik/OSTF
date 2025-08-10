using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Layouts;
using STFN;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace OstfTabletSuite
{
    public partial class MainPage : ContentPage, IOstfGui
    {
        //public static STFM.SoundPlayer MainPageSoundPlayer = null;
        //public static MediaElement MainPageMediaElement = null;

        public MainPage()
        {
            InitializeComponent();

            STFN.Messager.OnNewMessage += (title, message, cancelButtonText) =>
            {
                // Fire and forget the async call
                _ = DisplayMessage(title, message, cancelButtonText);
            };

            // If this should cause problems, a try- ctach block could be added to the lambda (and those below), like this
            //STFN.Messager.OnNewMessage += (title, message, cancelButtonText) =>
            //{
            //    _ = Task.Run(async () =>
            //    {
            //        try
            //        {
            //            await DisplayMessage(title, message, cancelButtonText);
            //        }
            //        catch (Exception ex)
            //        {
            //            // Log or handle exceptions here
            //            Console.WriteLine($"Error: {ex.Message}");
            //        }
            //    });
            //};


            STFN.Messager.OnNewAsyncMessage += (sender, e) =>
            {
                // Fire and forget the async call
                _ = DisplayMessageAsync(sender, e);
            };

            STFN.Messager.OnNewQuestion += (sender, e) =>
            {
                // Fire and forget the async call
                _ = DisplayBooleanQuestion(sender, e);
            };

            //STFN.Messager.OnNewMessage += DisplayMessage;
            //STFN.Messager.OnNewAsyncMessage += DisplayMessageAsync;
            //STFN.Messager.OnNewQuestion += DisplayBooleanQuestion;

            STFN.Messager.OnGetSaveFilePath += GetSaveFilePath;
            STFN.Messager.OnGetFolder += GetFolder;
            STFN.Messager.OnGetOpenFilePath += GetOpenFilePath;
            STFN.Messager.OnGetOpenFilePaths += GetOpenFilePaths;
            STFN.Messager.OnCloseAppRequest += CloseApp;

        }

        public void WelcomePageAllDone(object sender, EventArgs e)
        {

            this.Content = null;

            if (MyWelcomeView.RunScreeningTest)
            {
                var mySpeechTest = new STFM.SpecializedViews.ScreeningSuite.ScreeningSuiteView
                {
                    VerticalOptions = LayoutOptions.Fill,
                    HorizontalOptions = LayoutOptions.Fill
                };
                Content = mySpeechTest;
            }
            else
            {
                //Setting up the STFM.Extension speechTestProvider 
                STFM.SpeechTestProvider speechTestProvider = new STFM.Extension.SpeechTestProvider();

                var mySpeechTest = new STFM.Views.SpeechTestView(speechTestProvider)
                {
                    VerticalOptions = LayoutOptions.Fill,
                    HorizontalOptions = LayoutOptions.Fill
                };
                Content = mySpeechTest;
            }

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

        public async Task DisplayMessage(string title, string message, string cancelButtonText)
        {
            // Directing the call to the main thread if not already on the main thread
            if (MainThread.IsMainThread == false)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await DisplayMessage(title, message, cancelButtonText); // Directly call the method
                });
            }
            else
            {
                await DisplayAlert(title, message, cancelButtonText);
            }
        }

        //public async void DisplayMessage(string title, string message, string cancelButtonText)
        //{
        //    // Directing the call to the main thread if not already on the main thread
        //    if (MainThread.IsMainThread == false) { MethodBase currentMethod = MethodBase.GetCurrentMethod(); MainThread.BeginInvokeOnMainThread(() => { currentMethod.Invoke(this, new object[] { title, message, cancelButtonText }); }); return; }

        //    await DisplayAlert(title, message, cancelButtonText);
        //}


        public static void CloseApp()
        {
            Application.Current.Quit();
        }

        public async Task DisplayMessageAsync(object sender, MessageEventArgs e)
        {

            // Directing the call to the main thread if not already on the main thread
            if (MainThread.IsMainThread == false)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await DisplayMessageAsync(sender, e); // Directly call the method
                });
            }
            else
            {
                await DisplayAlert(e.Title, e.Message, e.CancelButtonText);
                // Setting false as response
                e.TaskCompletionSource.SetResult(false);
            }
        }

        //public async void DisplayMessageAsync(object sender, MessageEventArgs e)
        //{
        //    // Directing the call to the main thread if not already on the main thread
        //    if (MainThread.IsMainThread == false) { MethodBase currentMethod = MethodBase.GetCurrentMethod(); MainThread.BeginInvokeOnMainThread(() => { currentMethod.Invoke(this, new object[] { sender, e }); }); return; }

        //    await DisplayAlert(e.Title, e.Message, e.CancelButtonText);
        //    // Setting false as response
        //    e.TaskCompletionSource.SetResult(false);
        //}


        public async Task DisplayBooleanQuestion(object sender, QuestionEventArgs e)
        {

            // Directing the call to the main thread if not already on the main thread
            if (MainThread.IsMainThread == false)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await DisplayBooleanQuestion(sender, e); // Directly call the method
                });
                return;
            }
            else {

                //See more at https://learn.microsoft.com/en-us/dotnet/maui/user-interface/pop-ups?view=net-maui-8.0

                bool answer = await DisplayAlert(e.Title, e.Question, e.AcceptButtonText, e.CancelButtonText);

                e.TaskCompletionSource.SetResult(answer);
            }
        }


        //public async void DisplayBooleanQuestion(object sender, QuestionEventArgs e)
        //{

        //    // Directing the call to the main thread if not already on the main thread
        //    if (MainThread.IsMainThread == false) { MethodBase currentMethod = MethodBase.GetCurrentMethod(); MainThread.BeginInvokeOnMainThread(() => { currentMethod.Invoke(this, new object[] { sender, e }); }); return; }

        //    //See more at https://learn.microsoft.com/en-us/dotnet/maui/user-interface/pop-ups?view=net-maui-8.0

        //    bool answer = await DisplayAlert(e.Title, e.Question, e.AcceptButtonText, e.CancelButtonText);

        //    e.TaskCompletionSource.SetResult(answer);

        //}

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

