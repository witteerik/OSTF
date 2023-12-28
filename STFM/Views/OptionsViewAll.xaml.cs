
using STFN;

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
        if (SelectedTransducer_Picker.Items.Count > 0) { SelectedTransducer_Picker.SelectedIndex = 0; }
        //if (SelectedPresentationMode_Picker.Items.Count > 0) { SelectedPresentationMode_Picker.SelectedIndex = 0; }
        if (StartList_AvailablePhaseAudiometryTypes.Items.Count > 0) { StartList_AvailablePhaseAudiometryTypes.SelectedIndex = 0; }


    }

    private void SelectedTransducer_Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateSoundSourceLocations();
    }

    private void UseSimulatedSoundField_Switch_Toggled(object sender, ToggledEventArgs e)
    {

        SelectedIrSet_Picker.IsEnabled = e.Value;

        UpdateSoundSourceLocations();
    }

    private void SelectedIrSet_Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateSoundSourceLocations();
    }
        

    private void UpdateSoundSourceLocations()
    {

        // Clearing first
        SpeechSoundSourceView.SoundSources = new List<STFN.Audio.SoundScene.VisualSoundSourceLocation>();
        MaskerSoundSourceView.SoundSources = new List<STFN.Audio.SoundScene.VisualSoundSourceLocation>();
        BackgroundNonSpeechSoundSourceView.SoundSources = new List<STFN.Audio.SoundScene.VisualSoundSourceLocation>();
        BackgroundSpeechSoundSourceView.SoundSources = new List<STFN.Audio.SoundScene.VisualSoundSourceLocation>();


        // Access the BindingContext
        if (BindingContext is CustomizableTestOptions customizableTestOptions)
        {
            if (customizableTestOptions != null)
            {
                if (customizableTestOptions.SelectedTransducer != null)
                {

                    if (customizableTestOptions.UseSimulatedSoundField == false)
                    {
                            SpeechSoundSourceView.SoundSources =customizableTestOptions.SelectedTransducer.GetVisualSoundSourceLocations();
                            MaskerSoundSourceView.SoundSources =customizableTestOptions.SelectedTransducer.GetVisualSoundSourceLocations();
                            BackgroundNonSpeechSoundSourceView.SoundSources = customizableTestOptions.SelectedTransducer.GetVisualSoundSourceLocations();
                            BackgroundSpeechSoundSourceView.SoundSources = customizableTestOptions.SelectedTransducer.GetVisualSoundSourceLocations();
                    }
                    else
                    {
                        // Adding simulated sound filed locations
                        if (customizableTestOptions.SelectedIrSet != null)
                        {
                            SpeechSoundSourceView.SoundSources = customizableTestOptions.SelectedIrSet.GetVisualSoundSourceLocations();
                            MaskerSoundSourceView.SoundSources = customizableTestOptions.SelectedIrSet.GetVisualSoundSourceLocations();
                            BackgroundNonSpeechSoundSourceView.SoundSources = customizableTestOptions.SelectedIrSet.GetVisualSoundSourceLocations();
                            BackgroundSpeechSoundSourceView.SoundSources = customizableTestOptions.SelectedIrSet.GetVisualSoundSourceLocations();
                        }
                    }
                }
            }
        }
    }

}