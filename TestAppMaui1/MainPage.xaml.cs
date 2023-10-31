namespace TestAppMaui1
{
    public partial class MainPage : ContentPage
    {
        

        //STFM.SoundPlayer SoundPlayer = null;

        public MainPage()
        {
            InitializeComponent();

            STFM.StfmBase.InitiateSTFM(SoundPlayerAbsoluteLayout, "C:\\EriksDokument\\source\\repos\\OSTF\\OSTFMedia");

            //SoundPlayer = new STFM.SoundPlayer(SoundPlayerAbsoluteLayout);

        }

        private void OnSound1Clicked(object sender, EventArgs e)
        {
            //SoundPlayer.PlaySound("C:\\EriksDokument\\source\\repos\\QuickSiP\\QuickSiP\\Resources\\Raw\\sounds\\L17S01_City-Talker1-RVE_Az_15_PNR_5.wav");
        }

        private void OnSound2Clicked(object sender, EventArgs e)
        {
            //SoundPlayer.PlaySound("C:\\EriksDokument\\source\\repos\\QuickSiP\\QuickSiP\\Resources\\Raw\\sounds\\L21S03_City-Talker2-RVE_Az_15_PNR_5.wav");
        }

        private void KickstartOstfBtnClicked(object sender, EventArgs e)
        {

            STFM.StfmBase.SetupTest();

        }

        

    }
}