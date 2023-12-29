
using STFN;

namespace STFM.Views;

public partial class OptionsViewAll : ContentView
{
    public OptionsViewAll()
	{
		InitializeComponent();

		if (SelectedPreset_Picker.Items.Count > 0) {SelectedPreset_Picker.SelectedIndex = 0;}
        if (SelectedPreset_Picker.Items.Count == 1) { SelectedPreset_Picker.IsVisible = false; }

        if (StartList_Picker.Items.Count > 0) { StartList_Picker.SelectedIndex = 0; }
        if (StartList_Picker.Items.Count == 1) { StartList_Picker.IsVisible = false; }

        if (SelectedMediaSet_Picker.Items.Count > 0) { SelectedMediaSet_Picker.SelectedIndex = 0; }
        if (SelectedMediaSet_Picker.Items.Count ==1) { SelectedMediaSet_Picker.IsVisible = false; }

        ReferenceLevelControl.IsVisible = SharedSpeechTestObjects.CurrentSpeechTest.AllowsReferenceLevelControl;
        SpeechLevelControl.IsVisible = SharedSpeechTestObjects.CurrentSpeechTest.CanHaveTargets;
        MaskerLevelControl.IsVisible = SharedSpeechTestObjects.CurrentSpeechTest.CanHaveMaskers;
        BackgroundLevelControl.IsVisible = SharedSpeechTestObjects.CurrentSpeechTest.CanHaveBackgroundNonSpeech;

        if (AvailableTestModes_Picker.Items.Count > 0) { AvailableTestModes_Picker.SelectedIndex = 0; }
        if (AvailableTestModes_Picker.Items.Count == 1) { AvailableTestModes_Picker.IsVisible= false; }

        if (AvailableTestProtocols_Picker.Items.Count > 0) { AvailableTestProtocols_Picker.SelectedIndex = 0; }
        if (AvailableTestProtocols_Picker.Items.Count ==1) { AvailableTestProtocols_Picker.IsVisible= false; }


        switch (SharedSpeechTestObjects.CurrentSpeechTest.UseKeyWordScoring)
        {
            case STFN.Utils.Constants.TriState.True:
                KeyWords_Switch.IsToggled = true;
                KeyWordScoringControl.IsVisible = false;
                break;
            case STFN.Utils.Constants.TriState.False:
                KeyWords_Switch.IsToggled = false;
                KeyWordScoringControl.IsVisible = false;
                break;
            default:
                break;
        }

        switch (SharedSpeechTestObjects.CurrentSpeechTest.UseListOrderRandomization)
        {
            case STFN.Utils.Constants.TriState.True:
                ListOrderRandomization_Switch.IsToggled = true;
                ListOrderRandomizationControl.IsVisible = false;
                break;
            case STFN.Utils.Constants.TriState.False:
                ListOrderRandomization_Switch.IsToggled = false;
                ListOrderRandomizationControl.IsVisible = false;
                break;
            default:
                break;
        }


        switch (SharedSpeechTestObjects.CurrentSpeechTest.UseWithinListRandomization)
        {
            case STFN.Utils.Constants.TriState.True:
                WithinListRandomization_Switch.IsToggled = true;
                WithinListRandomizationControl.IsVisible = false;
                break;
            case STFN.Utils.Constants.TriState.False:
                WithinListRandomization_Switch.IsToggled = false;
                WithinListRandomizationControl.IsVisible = false;
                break;
            default:
                break;
        }

        switch (SharedSpeechTestObjects.CurrentSpeechTest.UseAcrossListRandomization)
        {
            case STFN.Utils.Constants.TriState.True:
                AcrossListRandomization_Switch.IsToggled = true;
                AcrossListRandomizationControl.IsVisible = false;
                break;
            case STFN.Utils.Constants.TriState.False:
                AcrossListRandomization_Switch.IsToggled = false;
                AcrossListRandomizationControl.IsVisible = false;
                break;
            default:
                break;
        }

        switch (SharedSpeechTestObjects.CurrentSpeechTest.UseFreeRecall)
        {
            case STFN.Utils.Constants.TriState.True:
                UseFreeRecall_Switch.IsToggled = true;
                UseFreeRecallControl.IsVisible = false;
                break;
            case STFN.Utils.Constants.TriState.False:
                UseFreeRecall_Switch.IsToggled = false;
                UseFreeRecallControl.IsVisible = false;
                break;
            default:
                break;
        }

        switch (SharedSpeechTestObjects.CurrentSpeechTest.UseDidNotHearAlternative)
        {
            case STFN.Utils.Constants.TriState.True:
                UseDidNotHearAlternative_Switch.IsToggled = true;
                UseDidNotHearAlternativeControl.IsVisible = false;
                break;
            case STFN.Utils.Constants.TriState.False:
                UseDidNotHearAlternative_Switch.IsToggled = false;
                UseDidNotHearAlternativeControl.IsVisible = false;
                break;
            default:
                break;
        }

        // Selecting the third alternative, if possible, otherwise the second of first
        if (AvailableFixedResponseAlternativeCounts_Picker.Items.Count > 0) { AvailableFixedResponseAlternativeCounts_Picker.SelectedIndex = 0; }
        if (AvailableFixedResponseAlternativeCounts_Picker.Items.Count > 1) { AvailableFixedResponseAlternativeCounts_Picker.SelectedIndex = 1; }
        if (AvailableFixedResponseAlternativeCounts_Picker.Items.Count > 2) { AvailableFixedResponseAlternativeCounts_Picker.SelectedIndex = 2; }
        if (AvailableFixedResponseAlternativeCounts_Picker.Items.Count < 2) { AvailableFixedResponseAlternativeCounts_Picker.IsVisible= false; }

        if (SelectedTransducer_Picker.Items.Count > 0) { SelectedTransducer_Picker.SelectedIndex = 0; }
        if (SelectedTransducer_Picker.Items.Count ==1) { SelectedTransducer_Picker.IsVisible=false; }

        UseSimulatedSoundFieldControl.IsVisible = OstfBase.AllowDirectionalSimulation;
        SelectedIrSet_Picker.IsVisible = false;

        SpeechSoundSourceView.IsVisible = SharedSpeechTestObjects.CurrentSpeechTest.CanHaveTargets;
        MaskerSoundSourceView.IsVisible = SharedSpeechTestObjects.CurrentSpeechTest.CanHaveMaskers;
        BackgroundNonSpeechSoundSourceView.IsVisible = SharedSpeechTestObjects.CurrentSpeechTest.CanHaveBackgroundNonSpeech;
        BackgroundSpeechSoundSourceView.IsVisible = SharedSpeechTestObjects.CurrentSpeechTest.CanHaveBackgroundSpeech;

        switch (SharedSpeechTestObjects.CurrentSpeechTest.UseContralateralMasking)
        {
            case STFN.Utils.Constants.TriState.True:
                UseContralateralMasking_Switch.IsToggled = true;
                UseContralateralMaskingControl.IsVisible = false;
                break;
            case STFN.Utils.Constants.TriState.False:
                UseContralateralMasking_Switch.IsToggled = false;
                UseContralateralMaskingControl.IsVisible = false;
                break;
            default:
                break;
        }


        switch (SharedSpeechTestObjects.CurrentSpeechTest.UsePhaseAudiometry)
        {
            case STFN.Utils.Constants.TriState.True:
                UsePhaseAudiometry_Switch.IsToggled = true;
                UsePhaseAudiometryControl.IsVisible = false;
                break;
            case STFN.Utils.Constants.TriState.False:
                UsePhaseAudiometry_Switch.IsToggled = false;
                UsePhaseAudiometryControl.IsVisible = false;
                break;
            default:
                break;
        }

        if (AvailablePhaseAudiometryTypes_Picker.Items.Count > 0) { AvailablePhaseAudiometryTypes_Picker.SelectedIndex = 0; }
        AvailablePhaseAudiometryTypes_Picker.IsVisible = false;

    }

    private void SelectedTransducer_Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateSoundSourceLocations();

        UpdateContralteralNoiseIsVisible();

    }

    private void UseSimulatedSoundField_Switch_Toggled(object sender, ToggledEventArgs e)
    {

        SelectedIrSet_Picker.IsEnabled = e.Value;
        SelectedIrSet_Picker.IsVisible = e.Value;

        UpdateSoundSourceLocations();

        UpdateContralteralNoiseIsVisible();

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

    private void UsePhaseAudiometry_Switch_Toggled(object sender, ToggledEventArgs e)
    {
        AvailablePhaseAudiometryTypes_Picker.IsVisible = e.Value;
    }

    private void UseFreeRecall_Switch_Toggled(object sender, ToggledEventArgs e)
    {
        AvailableFixedResponseAlternativeCounts_Picker.IsVisible = e.Value;
    }

    private void AvailableTestModes_Picker_SelectedIndexChanged(object sender, EventArgs e)
    {

        SpeechTest.TestModes castItem = (SpeechTest.TestModes)AvailableTestModes_Picker.SelectedItem;

        // Resetting default values
        SpeechLevelControl.IsVisible = SharedSpeechTestObjects.CurrentSpeechTest.CanHaveTargets;
        MaskerLevelControl.IsVisible = SharedSpeechTestObjects.CurrentSpeechTest.CanHaveMaskers;
        BackgroundLevelControl.IsVisible = SharedSpeechTestObjects.CurrentSpeechTest.CanHaveBackgroundNonSpeech;

        // Then hiding controls not to be used
        if (castItem == SpeechTest.TestModes.AdaptiveSpeech)
        {
            SpeechLevelControl.IsVisible = false;
        }
        if (castItem == SpeechTest.TestModes.AdaptiveNoise)
        {
            MaskerLevelControl.IsVisible = false;
            BackgroundLevelControl.IsVisible = false;
        }

    }

    private void UpdateContralteralNoiseIsVisible() { 

        // Hiding the UseContralateralMaskingControl in cases where it cannot be used, and deselecting UseContralateralMasking
        if (CurrentTransducerIsHeadPhones() == true & SharedSpeechTestObjects.CurrentSpeechTest.UseContralateralMasking == STFN.Utils.Constants.TriState.Optional & UseSimulatedSoundField_Switch.IsToggled == false)
        {
            UseContralateralMaskingControl.IsVisible = true;
        }
        else 
        {
            UseContralateralMasking_Switch.IsToggled = false;
            UseContralateralMaskingControl.IsVisible = false;
        }
    }

    private bool CurrentTransducerIsHeadPhones()
    {

        if (BindingContext is CustomizableTestOptions customizableTestOptions)
        {
            if (customizableTestOptions != null)
            {
                if (customizableTestOptions.SelectedTransducer != null)
                {
                    return customizableTestOptions.SelectedTransducer.IsHeadphones(1, 2);
                }
            }
        }
        return false;
    }

}