using STFN;

namespace STFM.Views;

public partial class WelcomePageR : ContentView
{

    public event EventHandler<EventArgs> AllDone;
    
    public static List<STFN.Utils.Constants.Languages> AvailableGuiLanguages = new List<STFN.Utils.Constants.Languages> { STFN.Utils.Constants.Languages.English, STFN.Utils.Constants.Languages.Swedish };

    
    public WelcomePageR()
	{
		InitializeComponent();

        SelectedLanguage_Picker.ItemsSource = AvailableGuiLanguages;
        SelectedLanguage_Picker.SelectedIndex = 0;

	}

    private void UseSoundFieldSimulation_Switch_Toggled(object sender, ToggledEventArgs e)
    {
        OstfBase.AllowDirectionalSimulation = e.Value;
    }

    private void SelectedLanguage_Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        SharedSpeechTestObjects.GuiLanguage = (STFN.Utils.Constants.Languages)SelectedLanguage_Picker.SelectedItem;
        UpdateLanguageStrings();
    }

    private void UpdateLanguageStrings()
    {

        switch (SharedSpeechTestObjects.GuiLanguage)
        {
            case STFN.Utils.Constants.Languages.English:

                Welcome_Label.Text = "OSTF Suite";
                SelectLangage_Label.Text = "Select language";
                SelectedLanguage_Picker.Title = "";
                //SelectedLanguage_Picker.Title = "Select language";
                ParticipantCode_Label.Text = "Enter participant code";
                SoundFieldSimulation_Label.Text = "Allow sound field simulation in headphones (may slow down processing)";
                Submit_Button.Text = "Continue";
                break;
            case STFN.Utils.Constants.Languages.Swedish:

                Welcome_Label.Text = "OSTF Suite";
                SelectLangage_Label.Text = "Select language";
                SelectedLanguage_Picker.Title = "";
                ParticipantCode_Label.Text = "Fyll i deltagarkod";
                SoundFieldSimulation_Label.Text = "Tillåt ljudfältssimulering i hörlurar (kan göra appen långsam)";
                Submit_Button.Text = "Fortsätt";
                break;
            default:

                Welcome_Label.Text = "OSTF Suite";
                SelectLangage_Label.Text = "Select language";
                SelectedLanguage_Picker.Title = "";
                ParticipantCode_Label.Text = "Enter participant code";
                SoundFieldSimulation_Label.Text = "Allow sound field simulation in headphones (may slow down processing)";
                Submit_Button.Text = "Continue";
                break;
        }


    }

    private void Submit_Button_Clicked(object sender, EventArgs e)
    {
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

        if (codeOk == true)
        {
            // All ok, raising the 
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
                case STFN.Utils.Constants.Languages.English:
                    Messager.MsgBox("Invalid participant code! The code must consist of two letters followed by four digits.", Messager.MsgBoxStyle.Information, "Invalid participant code!");
                    break;
                case STFN.Utils.Constants.Languages.Swedish:
                    Messager.MsgBox("Ogiltig deltagarkod! Koden måste bestå av två bokstäver följda av fyra siffor.", Messager.MsgBoxStyle.Information, "Ogiltig deltagarkod!");
                    break;
                default:
                    Messager.MsgBox("Invalid participant code! The code must consist of two letters followed by four digits.", Messager.MsgBoxStyle.Information, "Invalid participant code!");
                    break;
            }
        }

    }
}