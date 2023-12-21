namespace STFM.Views;

public partial class OptionsViewAll : ContentView
{
	public OptionsViewAll()
	{
		InitializeComponent();

		if (SelectedPreset_Picker.Items.Count > 0) {SelectedPreset_Picker.SelectedIndex = 0;}
        if (StartList_Picker.Items.Count > 0) { StartList_Picker.SelectedIndex = 0; }
        if (SelectedMediaSet_Picker.Items.Count > 0) { SelectedMediaSet_Picker.SelectedIndex = 0; }
        if (AvailableTestModes_Picker.Items.Count > 0) { AvailableTestModes_Picker.SelectedIndex = 0; }
        if (AvailableTestProtocols_Picker.Items.Count > 0) { AvailableTestProtocols_Picker.SelectedIndex = 0; }
        //if (AvailableFixedResponseAlternativeCounts_Picker.Items.Count > 0) { AvailableFixedResponseAlternativeCounts_Picker.SelectedIndex = 0; }
        if (AvailablePresentationModes_Picker.Items.Count > 0) { AvailablePresentationModes_Picker.SelectedIndex = 0; }
        if (StartList_AvailablePhaseAudiometryTypes.Items.Count > 0) { StartList_AvailablePhaseAudiometryTypes.SelectedIndex = 0; }

    }
}