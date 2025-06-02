namespace STFM.SpecializedViews.SSQ12;

public partial class SSQ12_HaView : ContentView
{
	public SSQ12_HaView()
	{
		InitializeComponent();


        switch (STFN.SharedSpeechTestObjects.GuiLanguage)
        {

            case STFN.Utils.Constants.Languages.Swedish:

                HA_UseQuestion_Label.Text = "Använder du hörapparat?";
                HA_DoUse_RadioButton.Content = "Jag använder hörapparat";
                HA_NotUse_RadioButton.Content = "Jag använder inte hörapparat";

                HA_Use_Label.Text = "Ange ett av alternativen";
                HA_DoUseLeft_RadioButton.Content = "Jag använder hörapparat till vänster öra";
                HA_DoUseRight_RadioButton.Content = "Jag använder hörapparat till höger öra";
                HA_DoUseBoth_RadioButton.Content = "Jag använder hörapparat till båda öronen";

                HA_UseTimeQuestion_Label.Text = "Hur länge har du använt din hörapparat?";
                HA_UseTime_Editor.Text = "";

                break;
            default:
                // Using English as default

                HA_UseQuestion_Label.Text = "";
                HA_DoUse_RadioButton.Content = "";
                HA_NotUse_RadioButton.Content = "";

                HA_Use_Label.Text = "";
                HA_DoUseLeft_RadioButton.Content = "";
                HA_DoUseRight_RadioButton.Content = "";
                HA_DoUseBoth_RadioButton.Content = "";

                HA_UseTimeQuestion_Label.Text = "";
                HA_UseTime_Editor.Text = "";

                break;
        }

    }

    private void HA_DoUse_RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            HA_Details_StackLayout.IsVisible = true;
        }
    }

    private void HA_NotUse_RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            HA_Details_StackLayout.IsVisible = false;
        }
    }

    /// <summary>
    /// Checks if side of hearing aid was selected if hearing aid use is specified. However, everything else is passed through, since this question is not mandatory.
    /// </summary>
    /// <returns></returns>
    public async Task<bool> HasResponse()
    {

        if (HA_DoUse_RadioButton.IsChecked)
        {

            if (HA_DoUseLeft_RadioButton.IsChecked | HA_DoUseRight_RadioButton.IsChecked | HA_DoUseBoth_RadioButton.IsChecked)
            {
                return true;
            }
            else
            {
                // Asks the user to specify left / righ / or both sides
                switch (STFN.SharedSpeechTestObjects.GuiLanguage)
                {
                    case STFN.Utils.Constants.Languages.Swedish:
                        await STFN.Messager.MsgBoxAsync("Vänligen ange om du använder hörapparat på vänster, höger eller båda öronen!", STFN.Messager.MsgBoxStyle.Information, "Ange hörapparatsida!");

                        break;
                    default:
                        // Using English as default
                        await STFN.Messager.MsgBoxAsync("Please specify if you use a hearing aid on left, right or both ears.", STFN.Messager.MsgBoxStyle.Information, "Specify hearing aid side!");

                        break;
                }
                return false;
            }
        }
        else
        {
            return true;
        }

    }

    /// <summary>
    /// Gets a string formatted result of the hearing aid use questions. Note that HasResponse should have been evaluated before calling this function.
    /// </summary>
    /// <returns></returns>
    public string GetResultString()
    {

        List<string> ReturnValueList = new List<string>();


        switch (STFN.SharedSpeechTestObjects.GuiLanguage)
        {
            case STFN.Utils.Constants.Languages.Swedish:

                ReturnValueList.Add("HÖRAPPARATANVÄNDNING");

                // Adding hearing aid use
                if (HA_DoUse_RadioButton.IsChecked)
                {
                    if (HA_DoUseLeft_RadioButton.IsChecked)
                    {
                        ReturnValueList.Add("Använder hörapparat till vänster öra.");
                    }
                    else if (HA_DoUseRight_RadioButton.IsChecked)
                    {
                        ReturnValueList.Add("Använder hörapparat till höger öra.");
                    }
                    else if (HA_DoUseRight_RadioButton.IsChecked)
                    {
                        ReturnValueList.Add("Använder hörapparat till båda öronen.");
                    }

                    // Adding time comment
                    if (HA_UseTime_Editor.Text.Trim() != "")
                    {
                        ReturnValueList.Add("TID: " + HA_UseTime_Editor.Text);
                    }

                }
                else if (HA_NotUse_RadioButton.IsChecked)
                {
                    ReturnValueList.Add("Använder inte hörapparat.");
                }
                else
                {
                    ReturnValueList.Add("Hörapparatanvändning ej angiven.");
                }

                break;
            default:

                ReturnValueList.Add("HEARING AID USE");

                // Adding hearing aid use
                if (HA_DoUse_RadioButton.IsChecked)
                {
                    if (HA_DoUseLeft_RadioButton.IsChecked)
                    {
                        ReturnValueList.Add("Hearing aid on left ear.");
                    }
                    else if (HA_DoUseRight_RadioButton.IsChecked)
                    {
                        ReturnValueList.Add("Hearing aid on right ear.");
                    }
                    else if (HA_DoUseRight_RadioButton.IsChecked)
                    {
                        ReturnValueList.Add("Hearing aids on both ears.");
                    }

                    // Adding time comment
                    if (HA_UseTime_Editor.Text.Trim() != "")
                    {
                        ReturnValueList.Add("TIME: " + HA_UseTime_Editor.Text);
                    }

                }
                else if (HA_NotUse_RadioButton.IsChecked)
                {
                    ReturnValueList.Add("Not using hearing aids.");
                }
                else
                {
                    ReturnValueList.Add("Hörapparatanvändning ej angiven.");
                }

                break;
        }

        return string.Join("\n", ReturnValueList);

    }

    public enum BoolWithNotSet
    {
        Yes,
        No,
        NotSet
    }

    /// <summary>
    /// Gets a BoolWithNotSet specifying the response concerning hearing aid use. Note that HasResponse should have been evaluated before calling this function.
    /// </summary>
    /// <returns></returns>
    public BoolWithNotSet UsingHearingAids()
    {

        // Adding hearing aid use
        if (HA_DoUse_RadioButton.IsChecked)
        {
            return BoolWithNotSet.Yes;
        } else if (HA_NotUse_RadioButton.IsChecked)
        {
            return BoolWithNotSet.No;
        } else
        {
            return BoolWithNotSet.NotSet;
        }

    }

}