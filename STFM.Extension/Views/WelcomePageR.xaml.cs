using STFN; 
using STFN.Core;

namespace STFM.Extension.Views;

public partial class WelcomePageR : ContentView
{

    public event EventHandler<EventArgs> AllDone;

    public event EventHandler<EventArgs> StartCalibrator;

    // Using only Swedish for now, as English is not implemented everywhere
    public static List<STFN.Core.Utils.EnumCollection.Languages> AvailableGuiLanguages = new List<STFN.Core.Utils.EnumCollection.Languages> { STFN.Core.Utils.EnumCollection.Languages.Swedish };
    //public static List<STFN.Utils.Constants.Languages> AvailableGuiLanguages = new List<STFN.Utils.Constants.Languages> { STFN.Utils.Constants.Languages.English, STFN.Utils.Constants.Languages.Swedish };

    public bool RunScreeningTest = false;

    public WelcomePageR()
	{
		InitializeComponent();

        SelectedLanguage_Picker.ItemsSource = AvailableGuiLanguages;
        if (AvailableGuiLanguages.Count>0)
        {
            // Picking the second language, which is Swedish when this is written
            SelectedLanguage_Picker.SelectedIndex = 1;
        }
        else
        {
            SelectedLanguage_Picker.SelectedIndex = 0;
        }

        //SoundFieldSimulation_Label.IsVisible = false;
        //UseSoundFieldSimulation_Switch.IsVisible = false;

    }

    private void UseSoundFieldSimulation_Switch_Toggled(object sender, ToggledEventArgs e)
    {
        OstfBase.AllowDirectionalSimulation = e.Value;
    }

    private void SelectedLanguage_Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        SharedSpeechTestObjects.GuiLanguage = (STFN.Core.Utils.EnumCollection.Languages)SelectedLanguage_Picker.SelectedItem;
        UpdateLanguageStrings();
    }

    private void UpdateLanguageStrings()
    {

        switch (SharedSpeechTestObjects.GuiLanguage)
        {
            case STFN.Core.Utils.EnumCollection.Languages.English:

                Welcome_Label.Text = "OSTF SUITE";
                SelectLangage_Label.Text = "Select language";
                SelectedLanguage_Picker.Title = "";
                //SelectedLanguage_Picker.Title = "Select language";
                ParticipantCode_Label.Text = "Enter participant code";
                DemoCode_Label.Text = "(use " + SharedSpeechTestObjects.NoTestId + " for demo mode)";
                //SoundFieldSimulation_Label.Text = "Allow sound field simulation in headphones (may slow down processing)";
                ScreeningTest_Checkbox_Label.Text = "Run screening test";
                Submit_Button.Text = "Continue";
                Calibrator_Button.Text = "Calibration";
                UseCalibrationCheck_Label.Text = "Show calibration check in test settings";
                ExportAllSounds_Label.Text = "Export all played sounds";
                break;
            case STFN.Core.Utils.EnumCollection.Languages.Swedish:

                Welcome_Label.Text = "OSTF SUITE";
                SelectLangage_Label.Text = "Select language";
                SelectedLanguage_Picker.Title = "";
                ParticipantCode_Label.Text = "Fyll i deltagarkod";
                DemoCode_Label.Text = "(använd " + SharedSpeechTestObjects.NoTestId + " för demoläge)";
                //SoundFieldSimulation_Label.Text = "Tillåt ljudfältssimulering i hörlurar (kan göra appen långsam)";
                ScreeningTest_Checkbox_Label.Text = "Kör screeningtest";
                Submit_Button.Text = "Fortsätt";
                Calibrator_Button.Text = "Kalibrering";
                UseCalibrationCheck_Label.Text = "Visa kalibreringskontroll i testinställningar";
                ExportAllSounds_Label.Text = "Exportera alla uppspelade ljud";
                break;
            default:

                Welcome_Label.Text = "OSTF SUITE";
                SelectLangage_Label.Text = "Select language";
                SelectedLanguage_Picker.Title = "";
                ParticipantCode_Label.Text = "Enter participant code";
                DemoCode_Label.Text = "(use " + SharedSpeechTestObjects.NoTestId + " for demo mode)";
                //SoundFieldSimulation_Label.Text = "Allow sound field simulation in headphones (may slow down processing)";
                Submit_Button.Text = "Continue";
                Calibrator_Button.Text = "Calibration";
                UseCalibrationCheck_Label.Text = "Show calibration check in test settings";
                ExportAllSounds_Label.Text = "Export all played sounds";
                break;
        }


    }

    private void Submit_Button_Clicked(object sender, EventArgs e)
    {
        TryStartSpeechTestView();
    }

    //private void ParticipantCode_Editor_Completed(object sender, EventArgs e)
    //{
    //    TryStartSpeechTestView();
    //}


    private async void TryStartSpeechTestView()
    {

        // Notes if the screening test shuld be run
        RunScreeningTest = ScreeningTest_Checkbox.IsChecked;

        bool codeOk = true;
        string ptcCode = "";
        if (ParticipantCode_Editor.Text == null)
        {
            codeOk = false;
        }
        else
        {
            ptcCode = ParticipantCode_Editor.Text.Trim();
        }

        if (codeOk == true)
        {
            if (ptcCode.Length != 6)
            {
                codeOk = false;
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    if (Char.IsLetter(ptcCode[i]) == false) { codeOk = false; }
                }
                for (int i = 2; i < 6; i++)
                {
                    if (Char.IsDigit(ptcCode[i]) == false) { codeOk = false; }
                }
            }
        }

        if (ptcCode == SharedSpeechTestObjects.NoTestId)
        {
           bool demoModeQuestionResult = await Messager.MsgBoxAcceptQuestion("You have entered the code for demo mode.\n\n TEST RESULTS WILL NOT BE SAVED! \n\n Use this for demonstration purpose only!","Warning! Starting demo mode", "OK", "Abort");
           if (demoModeQuestionResult == false)
           {
                ParticipantCode_Editor.Text = "";
                return;
           }
        }


        if (codeOk == true)
        {

            // Storing the current userID
            SharedSpeechTestObjects.CurrentParticipantID = ptcCode;

            // All ok, raising the AllDone handler
            EventHandler<EventArgs> handler = AllDone;
            // Check if there are any subscribers (null check)
            if (handler != null)
            {
                // Create EventArgs (or use EventArgs.Empty if no additional data is needed)
                EventArgs args = new EventArgs();

                // Raise the event by invoking all subscribers
                handler(this, args);
            }
        }
        else
        {
            switch (SharedSpeechTestObjects.GuiLanguage)
            {
                case STFN.Core.Utils.EnumCollection.Languages.English:
                    Messager.MsgBox("Invalid participant code! The code must consist of two letters followed by four digits.", Messager.MsgBoxStyle.Information, "Invalid participant code!");
                    break;
                case STFN.Core.Utils.EnumCollection.Languages.Swedish:
                    Messager.MsgBox("Ogiltig deltagarkod! Koden måste bestå av två bokstäver följda av fyra siffor.", Messager.MsgBoxStyle.Information, "Ogiltig deltagarkod!");
                    break;
                default:
                    Messager.MsgBox("Invalid participant code! The code must consist of two letters followed by four digits.", Messager.MsgBoxStyle.Information, "Invalid participant code!");
                    break;
            }
        }

    }


    private void Calibrator_Button_Clicked(object sender, EventArgs e)
    {

        switch (SharedSpeechTestObjects.GuiLanguage)
        {
            case STFN.Core.Utils.EnumCollection.Languages.English:
                Messager.MsgBox("Loading calibration view. It may take some time before it becomes responsive! Hang tight!", Messager.MsgBoxStyle.Information, "Loading calibration view!");
                break;
            case STFN.Core.Utils.EnumCollection.Languages.Swedish:
                Messager.MsgBox("Laddar kalibreringsvyn. Det kan dröja en stund innan vyn är redo!", Messager.MsgBoxStyle.Information, "Laddar kalibreringsvyn!");
                break;
            default:
                Messager.MsgBox("Loading calibration view. It may take some time before it becomes responsive! Hang tight!", Messager.MsgBoxStyle.Information, "Loading calibration view!");
                break;
        }
       

        EventHandler<EventArgs> handler = StartCalibrator;
        // Check if there are any subscribers (null check)
        if (handler != null)
        {
            // Create EventArgs (or use EventArgs.Empty if no additional data is needed)
            EventArgs args = new EventArgs();

            // Raise the event by invoking all subscribers
            handler(this, args);
        }

    }

    private void UseCalibrationCheck_Switch_Toggled(object sender, ToggledEventArgs e)
    {
        OstfBase.ShowCalibrationCheck = e.Value;
    }

    private void ExportAllSounds_Switch_Toggled(object sender, ToggledEventArgs e)
    {
        OstfBase.LogAllPlayedSoundFiles = e.Value;
    }
}