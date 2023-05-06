using Android.App;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using Android.Widget;
using Android.Views;
using Android.Bluetooth;
using Android.Content;
using Java.Util;
using Java.IO;
using System.Text;


namespace OSTF_Android_TelePromter
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.AppCompat.NoActionBar", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape, Immersive = true)]
    public class MainActivity : AppCompatActivity
    {

        LinearLayout BT_linearLayout;
        Button BT1_Button;
        Button BT2_Button;
        Button BT3_Button;

        LinearLayout Promter_linearLayout;
        TextView Promter_TextView;
        Button Mirror_Button;
        Button Demirror_Button;

        LinearLayout RecCircle_LinearLayout;
        CircleView RecCircle;


        BluetoothAdapter bluetoothAdapter = null;
        System.IO.Stream inputStream = null;
        System.IO.Stream outputStream = null;

        private bool IsInPromterView = false;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);


            BT_linearLayout = FindViewById<LinearLayout>(Resource.Id.BT_linearLayout);
            BT1_Button = FindViewById<Button>(Resource.Id.BT1_Button);
            BT2_Button = FindViewById<Button>(Resource.Id.BT2_Button);
            BT3_Button = FindViewById<Button>(Resource.Id.BT3_Button);

            Promter_linearLayout = FindViewById<LinearLayout>(Resource.Id.Promter_linearLayout);
            RecCircle_LinearLayout = FindViewById<LinearLayout>(Resource.Id.RecCircle_LinearLayout);
            Promter_TextView = FindViewById<TextView>(Resource.Id.Promter_TextView);
            Mirror_Button = FindViewById<Button>(Resource.Id.Mirror_Button);
            Demirror_Button = FindViewById<Button>(Resource.Id.Demirror_Button);

            BT1_Button.Click += (sender, e) => BT1_Button_Clicked(sender, e);
            BT2_Button.Click += (sender, e) => BT2_Button_Clicked(sender, e);

            Mirror_Button.Click += (sender, e) => Mirror_Button_Clicked(sender, e);
            Demirror_Button.Click += (sender, e) => Demirror_Button_Clicked(sender, e);

            // Adding the custom CircleView RecCircle to RecCircle_LinearLayout
            RecCircle = new CircleView(this);
            RecCircle_LinearLayout.AddView(RecCircle);

            Promter_TextView.KeepScreenOn = true;

            BT2_Button.Enabled = false;
            BT3_Button.Enabled = false;

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }



        private void BT1_Button_Clicked(object sender, System.EventArgs e)
        {

            // Getting the bluetooth adapter
            bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            if (bluetoothAdapter == null)
            {
                ShowPromterText("Bluetooth is not available. Try to restart the tablet app!");
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

            BT1_Button.Enabled = false;
            BT2_Button.Enabled = true;

        }

        private void BT2_Button_Clicked(object sender, System.EventArgs e)
        {

            // Accepting connection of separate thread
            //System.Threading.Thread AcceptConnectionThread = new System.Threading.Thread(new ThreadStart(ServerConnectionThread));
            //AcceptConnectionThread.Start();

            ServerConnectionThread();

            BT2_Button.Enabled = false;
            BT3_Button.Enabled = true;

        }

        private void Mirror_Button_Clicked(object sender, System.EventArgs e)
        {
            Promter_TextView.RotationX = 180;
        }

        private void Demirror_Button_Clicked(object sender, System.EventArgs e)
        {
            Promter_TextView.RotationX = 0;
        }


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

                BT1_Button.Text = e.Message;

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

                    ShowPromterText(e.Message + "Try to restart the tablet app");

                    //throw;
                    // Exits the void if something went wrong
                    return;
                }

                ShowPromterText("Connected to PC");

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
                            ShowPromterText("Bluetooth socket closed!");

                            PrepareNewConnection();
                            return;
                        }
                        else
                        {
                            ShowPromterText("BtReadError" + e.Message);
                        }
                    }
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

                    case "Msg":
                        ShowPromterText(messageParts[1]);
                        break;

                    case "R":
                        ShowCircle();
                        break;

                    case "NR":
                        HideCircle();
                        break;

                    case "L":
                        // This is a language options string, ignoring it.


                        break;

                    default:
                        ShowPromterText("Failed parsing BT message: " + message);
                        break;
                }


            }


        }

        public void ShowCircle()
        {
            RecCircle.Visibility = ViewStates.Visible;
        }

        public void HideCircle()
        {
            RecCircle.Visibility = ViewStates.Invisible;
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
            BT2_Button.Enabled = false;
            BT3_Button.Enabled = false;

            Promter_linearLayout.Visibility = Android.Views.ViewStates.Gone;
            BT_linearLayout.Visibility = Android.Views.ViewStates.Visible;

            IsInPromterView = false;
        }

        public void ShowPromterView()
        {

            // C.f. for this syntax: https://github.com/xamarin/monodroid-samples/blob/master/BasicImmersiveMode/BasicImmersiveModeFragment.cs
            int uiOptions = (int)this.Window.DecorView.SystemUiVisibility;
            int newUiOptions = uiOptions;
            newUiOptions ^= (int)SystemUiFlags.HideNavigation;
            newUiOptions ^= (int)SystemUiFlags.Fullscreen;
            newUiOptions ^= (int)SystemUiFlags.ImmersiveSticky;
            this.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)newUiOptions;

            BT_linearLayout.Visibility = Android.Views.ViewStates.Gone;
            Promter_linearLayout.Visibility = Android.Views.ViewStates.Visible;

            IsInPromterView = true;

        }

        public void ShowPromterText(string message)
        {

            Promter_TextView.Text = message;
            Promter_TextView.Selected = false;

            if (IsInPromterView == false)
            {
                ShowPromterView();
            }
        }

        private void PrepareNewConnection()
        {

            SetDefaultTexts();
            BT1_Button.Enabled = true;
            BT2_Button.Enabled = false;
            BT3_Button.Enabled = false;
            ShowMenu();

        }

        private void SetDefaultTexts()
        {
            BT1_Button.Text = "Step 1. Activate bluetooth";
            BT2_Button.Text = "Step 2. Make connectable to PC";
            BT3_Button.Text = "Step 3. Search for this unit from the PC";
        }



    }

    public class CircleView : Android.Views.View
    {
        public CircleView(Android.Content.Context context)
            : base(context) { }

        protected override void OnDraw(Android.Graphics.Canvas canvas)
        {
            var paint = new Android.Graphics.Paint { Color = new Android.Graphics.Color(255, 0, 0), StrokeWidth = 3 };
            float radius = this.Height / 4;
            float cx = this.Width / 2;
            float cy = this.Height / 2;
            canvas.DrawCircle(cx, cy, radius, paint);
        }
    }

}