using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using System.ComponentModel;
using System;
using Android.Bluetooth;
using Android.Content;
using Java.Util;
using Java.IO;
using Java.Lang;
using System.Threading;
using Java.Util.Concurrent;
using System.Text;
using System.Xml;
using Android.Graphics.Drawables;

namespace SiPTablet
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.AppCompat.NoActionBar", MainLauncher = true,
        ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape, Immersive = true)]
    public class MainActivity : AppCompatActivity
    {

        LinearLayout linearLayout_main;

        LinearLayout linearLayout_menu;
        LinearLayout linearLayout_test;

        LinearLayout linearLayout_start;
        LinearLayout linearLayout_message;
        LinearLayout linearLayout_presentation;
        LinearLayout linearLayout_testWords;
        Button button_response1;
        Button button_response2;
        Button button_response3;
        LinearLayout linearLayout_circle;
        LinearLayout linearLayout_blank;

        ProgressBar progressBar_SiP;

        Button button_Start;

        TextView textView_Message;

        
        Button MenuButton1;
        Button MenuButton2;
        Button MenuButton3;
        Button MenuButton4;

        private bool IsInTestView = false;

        private Languages currentLanguage = Languages.English;
        private enum Languages
        {
            Swedish,
            English
        }  


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            // Referencing objects
            linearLayout_main = FindViewById<LinearLayout>(Resource.Id.linearLayout_main);
            linearLayout_menu = FindViewById<LinearLayout>(Resource.Id.linearLayout_menu);
            linearLayout_test = FindViewById<LinearLayout>(Resource.Id.linearLayout_test);
            linearLayout_start = FindViewById<LinearLayout>(Resource.Id.linearLayout_start);
            linearLayout_message = FindViewById<LinearLayout>(Resource.Id.linearLayout_message);
            linearLayout_presentation = FindViewById<LinearLayout>(Resource.Id.linearLayout_presentation);
            linearLayout_testWords = FindViewById<LinearLayout>(Resource.Id.linearLayout_testWords);
            linearLayout_circle = FindViewById<LinearLayout>(Resource.Id.linearLayout_circle);
            linearLayout_blank = FindViewById<LinearLayout>(Resource.Id.linearLayout_blank);

            button_response1 = FindViewById<Button>(Resource.Id.button_response1);
            button_response2 = FindViewById<Button>(Resource.Id.button_response2);
            button_response3 = FindViewById<Button>(Resource.Id.button_response3);

            textView_Message = FindViewById<TextView>(Resource.Id.textView_Message);
            progressBar_SiP = FindViewById<ProgressBar>(Resource.Id.progressBar_SiP);

            button_Start = FindViewById<Button>(Resource.Id.button_Start);


            MenuButton1 = FindViewById<Button>(Resource.Id.menuButton1);
            MenuButton2 = FindViewById<Button>(Resource.Id.BTbutton2);
            MenuButton3 = FindViewById<Button>(Resource.Id.BTbutton3);
            MenuButton4 = FindViewById<Button>(Resource.Id.BTbutton4);
            

            // Adding the custom CircleView to linearLayout_circle
            linearLayout_circle.AddView(new CircleView(this));

            // Setting colors
            linearLayout_main.SetBackgroundColor(new Android.Graphics.Color(40, 40, 40));
            button_response1.SetBackgroundColor(new Android.Graphics.Color(255, 255, 128));
            button_response2.SetBackgroundColor(new Android.Graphics.Color(255, 255, 128));
            button_response3.SetBackgroundColor(new Android.Graphics.Color(255, 255, 128));
            button_Start.SetBackgroundColor(new Android.Graphics.Color(255, 255, 128));
            textView_Message.SetBackgroundColor(new Android.Graphics.Color(255, 255, 128));
            button_response1.SetTextColor(new Android.Graphics.Color(0, 0, 0));
            button_response2.SetTextColor(new Android.Graphics.Color(0, 0, 0));
            button_response3.SetTextColor(new Android.Graphics.Color(0, 0, 0));
            button_Start.SetTextColor(new Android.Graphics.Color(0, 0, 0));
            textView_Message.SetTextColor(new Android.Graphics.Color(0, 0, 0));
            //progressBar_SiP.ProgressDrawable.SetColorFilter(new Android.Graphics.Color(255, 255, 128), Android.Graphics.PorterDuff.Mode.Multiply);

            // Setting font
            // Don't know yet how to do this, but perhaps something like this, it needs a font file though...
            //Android.Graphics.Typeface font = Android.Graphics.Typeface.CreateFromAsset(Android.App.Application.Context.Assets, "Roboto-Regular.ttf");  // font name specified here
            //button_response1.Typeface = font;
            // Or?
            //button_response1.Typeface = Android.Graphics.Typeface.Create("Verdana", Android.Graphics.TypefaceStyle.Normal);
            //button_response2.Typeface = Android.Graphics.Typeface.Create("Verdana", Android.Graphics.TypefaceStyle.Normal);

            // Adding event handlers
            button_response1.Click += (sender, e) => ResponseButtonClicked(sender, e);
            button_response2.Click += (sender, e) => ResponseButtonClicked(sender, e);
            button_response3.Click += (sender, e) => ResponseButtonClicked(sender, e);

            MenuButton1.Click += (sender, e) => MenuButton1_Clicked(sender, e);

            MenuButton2.Click += (sender, e) => MenuButton2_Clicked(sender, e);
            // MenuButton3.Click += (sender, e) => MenuButton3_Clicked(sender, e); // This is not needed
            MenuButton4.Click += (sender, e) => MenuButton4_Clicked(sender, e);

            button_Start.Click += (sender, e) => StartButtonClicked(sender, e);

            // Going to a view
            //string[] testwords = { "hej", "haj", "hoj" };
            //ShowResponseAlternatives(testwords);
            //UpdateSipProgressBar(10, 40);

            currentLanguage = Languages.Swedish;

            if (currentLanguage == Languages.Swedish)
            {
                button_Start.Text = "Starta testet";
            }
            else
            {
                button_Start.Text = "Start test";
            }

            PrepareNewConnection();

        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
         }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);

            if (hasFocus)
            {
                // The activity has gained focus and is visible to the user.
                SetTextSizes();
            }
        }

        public void SetTextSizes()
        {
            // Setting appropriate text size
            int myWidth = linearLayout_main.Width;
            int textSize = myWidth / 15;
            button_response1.TextSize = textSize;
            button_response2.TextSize = textSize;
            button_response3.TextSize = textSize;

            // Setting their color as well
            button_response1.SetBackgroundColor(new Android.Graphics.Color(255, 255, 128));
            button_response2.SetBackgroundColor(new Android.Graphics.Color(255, 255, 128));
            button_response3.SetBackgroundColor(new Android.Graphics.Color(255, 255, 128));

            // Setting appropriate text size
            int startButtonTextSize = myWidth / 30;
            button_Start.TextSize = startButtonTextSize;

        }


        private void StartButtonClicked(object sender, System.EventArgs e)
        {

            SendBtMessage("StartTest");

        }


        private void ResponseButtonClicked(object sender, System.EventArgs e)
        {

            var castButton = sender as Button;
            if (castButton != null)
            {

                // This should send the response to the PC
                SendBtMessage("TW|" + castButton.Text);

                if (object.ReferenceEquals(castButton, button_response1) == true)
                {
                    // Hiding buttons 2 and 3
                    button_response2.Visibility = ViewStates.Invisible;
                    button_response3.Visibility = ViewStates.Invisible;

                }

                if (object.ReferenceEquals(castButton, button_response2) == true)
                {
                    // Hiding buttons 1 and 3
                    button_response1.Visibility = ViewStates.Invisible;
                    button_response3.Visibility = ViewStates.Invisible;
                }

                if (object.ReferenceEquals(castButton, button_response3) == true)
                {
                    // Hiding buttons 1 and 2
                    button_response1.Visibility = ViewStates.Invisible;
                    button_response2.Visibility = ViewStates.Invisible;
                }
            }
        }


        public void ShowMenu()
        {

            //this.Window.ClearFlags(Android.Views.WindowManagerFlags.Fullscreen);
            //this.Window.ClearFlags(Android.Views.WindowManagerFlags.KeepScreenOn);
            ////this.Window.ClearFlags(Android.Views.WindowManagerFlags.LayoutInScreen);


            // C.f. for this syntax: https://github.com/xamarin/monodroid-samples/blob/master/BasicImmersiveMode/BasicImmersiveModeFragment.cs
            int uiOptions = (int)this.Window.DecorView.SystemUiVisibility;
            int newUiOptions = uiOptions;
            newUiOptions ^= (int)SystemUiFlags.Visible;
            this.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)newUiOptions;

            // Inactivating meny buttons
            MenuButton2.Enabled = false;
            MenuButton3.Enabled = false;
            MenuButton4.Enabled = false;


            linearLayout_test.Visibility = Android.Views.ViewStates.Gone;
            linearLayout_menu.Visibility = Android.Views.ViewStates.Visible;

            IsInTestView = false;
        }

        public void ShowTest(bool goToStart = false)
        {

            //this.Window.AddFlags(Android.Views.WindowManagerFlags.Fullscreen);
            //this.Window.AddFlags(Android.Views.WindowManagerFlags.KeepScreenOn);
            //this.Window.DecorView.SystemUiVisibility = StatusBarVisibility.Hidden; // Android.Views.SystemUiFlags.ImmersiveSticky;

            // C.f. for this syntax: https://github.com/xamarin/monodroid-samples/blob/master/BasicImmersiveMode/BasicImmersiveModeFragment.cs
            int uiOptions = (int)this.Window.DecorView.SystemUiVisibility;
            int newUiOptions = uiOptions;
            newUiOptions ^= (int)SystemUiFlags.HideNavigation;
            newUiOptions ^= (int)SystemUiFlags.Fullscreen;
            newUiOptions ^= (int)SystemUiFlags.ImmersiveSticky;
            this.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)newUiOptions;

            // This code hides the navigation, but it reappears as soon as the screen is pressed. Perhaps stop the press event

            //this.Window.AddFlags(Android.Views.WindowManagerFlags.LayoutInScreen);

            // We should make sure android menu is hidden!
            //getSupportActionBar().hide();
            //RequestWindowFeature(Window.FEATURE_NO_TITLE);

            //Toolbar mToolbar = (Toolbar)FindViewById(Resource.Id.action_bar);
            //setSupportActionBar(mToolbar);
            //getSupportActionBar().setDisplayShowHomeEnabled(true);

            // Added manually
            //RequestWindowFeature(WindowFeatures.ContextMenu);

            //SupportActionBar.Hide();

            linearLayout_menu.Visibility = Android.Views.ViewStates.Gone;
            linearLayout_test.Visibility = Android.Views.ViewStates.Visible;

            if (goToStart == true)
            {
                linearLayout_start.Visibility = Android.Views.ViewStates.Visible;
                linearLayout_message.Visibility = Android.Views.ViewStates.Gone;
                linearLayout_presentation.Visibility = Android.Views.ViewStates.Gone;
                linearLayout_testWords.Visibility = Android.Views.ViewStates.Gone;
                linearLayout_circle.Visibility = Android.Views.ViewStates.Gone;
                linearLayout_blank.Visibility = Android.Views.ViewStates.Gone;

            }

            IsInTestView = true;

        }

        public void ShowStart()
        {
            if (IsInTestView == false)
            {
                ShowTest();
            }  

            linearLayout_start.Visibility = Android.Views.ViewStates.Visible;
            linearLayout_message.Visibility = Android.Views.ViewStates.Gone;
            linearLayout_presentation.Visibility = Android.Views.ViewStates.Gone;
            linearLayout_testWords.Visibility = Android.Views.ViewStates.Gone;
            linearLayout_circle.Visibility = Android.Views.ViewStates.Gone;
            linearLayout_blank.Visibility = Android.Views.ViewStates.Gone;

        }

        public void ShowResponseAlternatives(string[] responseAlternatives)
        {

            // Putting the response alternatives in the text in 
            button_response1.Text = responseAlternatives[0];
            button_response2.Text = responseAlternatives[1];
            button_response3.Text = responseAlternatives[2];

            // Setting their color as well
            //button_response1.SetBackgroundColor(new Android.Graphics.Color(255, 255, 128));
            //button_response2.SetBackgroundColor(new Android.Graphics.Color(255, 255, 128));
            //button_response3.SetBackgroundColor(new Android.Graphics.Color(255, 255, 128));

            // Create a shape drawable with rounded corners
            GradientDrawable roundedDrawable = new GradientDrawable();
            roundedDrawable.SetShape(ShapeType.Rectangle);
            int myWidth = linearLayout_main.Width;
            float cornerRaduis = myWidth / 45;
            roundedDrawable.SetCornerRadius(cornerRaduis); // Adjust the corner radius as needed
            roundedDrawable.SetColor(new Android.Graphics.Color(255, 255, 128)); // Background color of the button
            //roundedDrawable.SetColor(Android.Graphics.Color.ParseColor("#FF4081")); // Background color of the button

            // Set the custom drawable as the background of the button
            button_response1.Background = roundedDrawable;
            button_response2.Background = roundedDrawable;
            button_response3.Background = roundedDrawable;

            // Making sure buttons are visible
            button_response1.Visibility = ViewStates.Visible;
            button_response2.Visibility = ViewStates.Visible;
            button_response3.Visibility = ViewStates.Visible;

            if (IsInTestView == false)
            {
                ShowTest();
            }

            linearLayout_start.Visibility = Android.Views.ViewStates.Gone;
            linearLayout_message.Visibility = Android.Views.ViewStates.Gone;
            linearLayout_presentation.Visibility = Android.Views.ViewStates.Visible;
            linearLayout_testWords.Visibility = Android.Views.ViewStates.Visible;
            linearLayout_circle.Visibility = Android.Views.ViewStates.Gone;
            linearLayout_blank.Visibility = Android.Views.ViewStates.Gone;


        }

        public void FlashResponseAlternatives()
        {

            // Create a shape drawable with rounded corners
            GradientDrawable roundedDrawable = new GradientDrawable();
            roundedDrawable.SetShape(ShapeType.Rectangle);
            int myWidth = linearLayout_main.Width;
            float cornerRaduis = myWidth / 45;
            roundedDrawable.SetCornerRadius(cornerRaduis); // Adjust the corner radius as needed
            roundedDrawable.SetColor(Android.Graphics.Color.Red); // Background color of the button

            // Set the custom drawable as the background of the button
            button_response1.Background = roundedDrawable;
            button_response2.Background = roundedDrawable;
            button_response3.Background = roundedDrawable;

            //button_response1.SetBackgroundColor(Android.Graphics.Color.Red);
            //button_response2.SetBackgroundColor(Android.Graphics.Color.Red);
            //button_response3.SetBackgroundColor(Android.Graphics.Color.Red);
        }


        public void ShowCircle()
        {
            if (IsInTestView == false)
            {
                ShowTest();
            }

            linearLayout_start.Visibility = Android.Views.ViewStates.Gone;
            linearLayout_message.Visibility = Android.Views.ViewStates.Gone;
            linearLayout_presentation.Visibility = Android.Views.ViewStates.Visible;
            linearLayout_testWords.Visibility = Android.Views.ViewStates.Gone;
            linearLayout_circle.Visibility = Android.Views.ViewStates.Visible;
            linearLayout_blank.Visibility = Android.Views.ViewStates.Gone;
        }

        public void ShowBlank()
        {
            if (IsInTestView == false)
            {
                ShowTest();
            }

            linearLayout_start.Visibility = Android.Views.ViewStates.Gone;
            linearLayout_message.Visibility = Android.Views.ViewStates.Gone;
            linearLayout_presentation.Visibility = Android.Views.ViewStates.Visible;
            linearLayout_testWords.Visibility = Android.Views.ViewStates.Gone;
            linearLayout_circle.Visibility = Android.Views.ViewStates.Gone;
            linearLayout_blank.Visibility = Android.Views.ViewStates.Visible;
        }

        public void ShowMessage(string message)
        {

            // Setting appropriate text size
            int myWidth = linearLayout_main.Width;
            int textSize = myWidth / 40;
            textView_Message.TextSize = textSize;

            textView_Message.Text = message;

            if (IsInTestView == false)
            {
                ShowTest();
            }

            linearLayout_start.Visibility = Android.Views.ViewStates.Gone;
            linearLayout_message.Visibility = Android.Views.ViewStates.Visible;
            linearLayout_presentation.Visibility = Android.Views.ViewStates.Gone;
            linearLayout_testWords.Visibility = Android.Views.ViewStates.Gone;
            linearLayout_circle.Visibility = Android.Views.ViewStates.Gone;
            linearLayout_blank.Visibility = Android.Views.ViewStates.Gone;
        }

        public void UpdateSipProgressBar(int presented, int total)
        {
            if (progressBar_SiP.Activated == false)
            {
                progressBar_SiP.Activated = true;
             }

            progressBar_SiP.Visibility = Android.Views.ViewStates.Visible;
            
            progressBar_SiP.Progress = presented;
            progressBar_SiP.Max = total;
        }

        private void PrepareNewConnection()
        {

            SetDefaultTexts();
            MenuButton1.Enabled = true;
            MenuButton2.Enabled = false;

            // Actually button 4 is not needed, hiding it
            MenuButton4.Visibility = ViewStates.Gone;
            MenuButton3.Enabled = false;
            MenuButton4.Enabled = false;
            ShowMenu();

        }

        private void SetDefaultTexts()
        {

            switch (currentLanguage)
            {
                case Languages.Swedish:
                    MenuButton1.Text = "Steg 1. Aktivera Bluetooth";
                    MenuButton2.Text = "Steg 2. Gör enhet synlig för datorn";
                    MenuButton3.Text = "Steg 3. Sök nu efter denna enhet från datorn";
                    MenuButton4.Text = "Koppla bort från dator";
                    break;
                case Languages.English:
                    MenuButton1.Text = "Step 1. Activate bluetooth";
                    MenuButton2.Text = "Step 2. Make connectable to PC";
                    MenuButton3.Text = "Steg 3. Sök nu efter denna enhet från datorn";
                    MenuButton4.Text = "Close connection to PC";
                    break;
                default:
                    MenuButton1.Text = "Step 1. Activate bluetooth";
                    MenuButton2.Text = "Step 2. Make connectable to PC";
                    MenuButton3.Text = "Step 3. Search for this unit from the computer";
                    MenuButton4.Text = "Close connection to PC";
                    break;
            }

        }

        private void MenuButton1_Clicked(object sender, System.EventArgs e)
        {

            // Getting the bluetooth adapter
            bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            if (bluetoothAdapter == null)
            {
                switch (currentLanguage)
                {
                    case Languages.Swedish:
                        ShowMessage("Bluetooth är inte tillgängligt. Försök starta om appen.");
                        break;
                    case Languages.English:
                        ShowMessage("Bluetooth is not available. Try to restart the tablet app!");
                        break;
                    default:
                        ShowMessage("Bluetooth is not available. Try to restart the tablet app!");
                        break;
                }
                return;
            }

            // Enable bluetooth
            if (bluetoothAdapter.IsEnabled == false)
            {
                Intent enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                int REQUEST_ENABLE_BT = 1;
                StartActivityForResult(enableBtIntent, REQUEST_ENABLE_BT);

                if (REQUEST_ENABLE_BT != 1)
                {
                    // Dropping out of the sub without going on
                    return;
                }

                //// Don't know how to handle the situation where the user selected not to start BT
                //// I guess I can just check again...
                //OnActivityResult(REQUEST_ENABLE_BT, )

                //if (REQUEST_ENABLE_BT == 1)
                //{
                //    menuButton1.Text = "Started bt";
                //}
                //else
                //{
                //    menuButton1.Text = "Started bt???";
                // }
                // Se further at https://developer.android.com/guide/topics/connectivity/bluetooth#java


            }

            // Making discoverable
            Intent discoverableIntent = new Intent(BluetoothAdapter.ActionRequestDiscoverable);
            discoverableIntent.PutExtra(BluetoothAdapter.ExtraDiscoverableDuration, 120);
            StartActivity(discoverableIntent);

            MenuButton1.Enabled = false;
            MenuButton2.Enabled = true;

        }

        private void MenuButton2_Clicked(object sender, System.EventArgs e)
        {

            // Accepting connection of separate thread
            //System.Threading.Thread AcceptConnectionThread = new System.Threading.Thread(new ThreadStart(ServerConnectionThread));
            //AcceptConnectionThread.Start();

            ServerConnectionThread();

            MenuButton2.Enabled = false;
            MenuButton3.Enabled = true;

        }


        //private void MenuButton3_Clicked(object sender, System.EventArgs e)
        //{

        //    // Starting a BT read thread
        //    //System.Threading.Thread BlueToothReadThread = new System.Threading.Thread(new ThreadStart(BtReadThread));
        //    //BlueToothReadThread.Start();
        //    BtReadThread();

        //    MenuButton3.Enabled = false;
        //    MenuButton4.Enabled = true;

        //    // This should be started as a thread right from the start

        //}

        private void MenuButton4_Clicked(object sender, System.EventArgs e)
        {

            // Setting streams to null to stop reading and writing
            inputStream = null;
            outputStream = null;

            // Closing the connection
            // TODO: Needs further implementation?
            bluetoothAdapter.Disable();

        }



        BluetoothAdapter bluetoothAdapter = null;
        System.IO.Stream inputStream = null;
        System.IO.Stream outputStream = null;

        private async void ServerConnectionThread()
        {

            // Server component accepting incoming connections
            // TODO: Should be in a separate thread
            BluetoothServerSocket tempBtServerSocket = null;
            BluetoothSocket serverClientSocket = null;
            try
            {
                string myUUID = "056435e9-cfdd-4fb3-8cc8-9a4eb21c439c";

                tempBtServerSocket = bluetoothAdapter.ListenUsingRfcommWithServiceRecord("SiPTablet", UUID.FromString(myUUID));
                while (serverClientSocket == null)
                {
                    serverClientSocket = await tempBtServerSocket.AcceptAsync();

                }

            }
            catch (IOException e)
            {

                MenuButton1.Text = e.Message;

                //throw;
            }


            // Stop the listener and get the stream
            if (serverClientSocket != null)
            {


                //MenuButton1.Text = "Connected";
                //MenuButton3.Enabled = true;

                tempBtServerSocket.Close();

                var device = serverClientSocket.RemoteDevice;
                try
                {
                    inputStream = serverClientSocket.InputStream;
                    outputStream = serverClientSocket.OutputStream;

                }
                catch (IOException e)
                {

                    switch (currentLanguage)
                    {
                        case Languages.Swedish:
                            ShowMessage(e.Message + "Försök starta om appen");
                            break;
                        case Languages.English:
                            ShowMessage(e.Message + "Try to restart the tablet app");
                            break;
                        default:
                            ShowMessage(e.Message + "Try to restart the tablet app");
                            break;
                    }
                    //throw;
                    // Exits the void if something went wrong
                    return;
                }

                switch (currentLanguage)
                {
                    case Languages.Swedish:
                        ShowMessage("Ansluten till datorn");
                        break;
                    case Languages.English:
                        ShowMessage("Connected to PC");
                        break;
                    default:
                        ShowMessage("Connected to PC");
                        break;
                }

                // Staring to read BT stream
                BtReadThread();

            }
        }

        private async void BtReadThread()
        {

            // Reading bytes
            byte[] bytes = new byte[1024];
            bool continueReading = true;
            while (continueReading)
            {

                if (inputStream != null)
                {


                    try
                    {
                        var size = await inputStream.ReadAsync(bytes, 0, bytes.Length);
                        var stringRead = Encoding.UTF8.GetString(bytes, 0, size);

                        if (stringRead.Trim() != "")
                        {
                            ParseBtMessage(stringRead.Trim());

                            // Clearing the read bytes
                            bytes = new byte[1024];
                        }

                    }
                    catch (IOException e)
                    {

                        if (e.Message == "bt socket closed, read return: -1")
                        {
                            continueReading = false;
                            switch (currentLanguage)
                            {
                                case Languages.Swedish:
                                    ShowMessage("Bluetooth-anslutningen stängdes!");
                                    break;
                                case Languages.English:
                                    ShowMessage("Bluetooth socket closed!");
                                    break;
                                default:
                                    ShowMessage("Bluetooth socket closed!");
                                    break;
                            }

                            PrepareNewConnection();
                            return;
                        }
                        else
                        {
                            ShowMessage("BtReadError" + e.Message);
                        }
                    }
                }
            }


        }

        private async void SendBtMessage(string Message)
        {

            // Converting to binary message
            // Using § as and end-of-message character (This means that the charecters | and § are forbidden within messages)
            var bytes = Encoding.UTF8.GetBytes(Message + "§");
            if (outputStream != null)
            {

                // Writing to the bluetooth stream
                try
                {
                    await outputStream.WriteAsync(bytes, 0, bytes.Length);
                }
                catch (IOException e)
                {
                    ShowMessage("BtWriteError" + e.Message);
                }
            }
        }


        private void ParseBtMessage(string MessageString)
        {

            // Splitting into messages
            string[] messages = MessageString.TrimEnd(char.Parse("§")).Split("§");

            // Going through and sequentially executes all incoming messages
            foreach (string message in messages)
            {

                // Splitting string by vertical bar
                string[] messageParts = message.Split("|");

                switch (messageParts[0])
                {
                    case "ping":
                        // Doing nothing right now, could be used to respond to the PC software
                        break;

                    case "M":
                        ShowMenu();
                        break;

                    case "S":
                        ShowStart();
                        break;

                    case "Msg":                                                                      
                        ShowMessage(messageParts[1]);
                        break;

                    case "TW":
                        string[] responseAlternatives = new string[3];
                        responseAlternatives[0] = messageParts[1];
                        responseAlternatives[1] = messageParts[2];
                        responseAlternatives[2] = messageParts[3];
                        ShowResponseAlternatives(responseAlternatives);
                        break;

                    case "L":

                        // Setting presentation language
                        if (messageParts[1] == "EN")
                        {
                            currentLanguage = Languages.English;
                        }

                        if (messageParts[1] == "SE")
                        {
                            currentLanguage = Languages.Swedish;
                        }

                        SetDefaultTexts();

                        break;

                    case "F":
                        FlashResponseAlternatives();
                        break;

                    case "C":
                        ShowCircle();
                        break;

                    case "B":
                        ShowBlank();
                        break;

                    case "P":

                        int presented;
                        int total;
                        if (int.TryParse(messageParts[1], out presented) == true & int.TryParse(messageParts[2], out total) == true)
                        {
                            UpdateSipProgressBar(presented, total);
                        }
                        break;


                    default:
                        ShowMessage("Failed parsing BT message: " + message);
                        break;
                }


            }


        }

    }






    public class CircleView : Android.Views.View
    {
        public CircleView(Android.Content.Context context)
            : base(context) { }

        protected override void OnDraw(Android.Graphics.Canvas canvas)
        {
            var paint = new Android.Graphics.Paint { Color = new Android.Graphics.Color(255, 255, 128), StrokeWidth = 3 };
            float radius = this.Height / 4;
            float cx = this.Width / 2;
            float cy = this.Height / 2;
            canvas.DrawCircle(cx, cy, radius, paint);
        }
    }

}