
using CommunityToolkit.Maui.Media;
using Microsoft.VisualBasic;
using STFN;

namespace STFM.Views;

public partial class OptionsViewAll : ContentView
{

    private CustomizableTestOptions CurrentBindingContext
    {
        get
        {
            if (BindingContext is CustomizableTestOptions customizableTestOptions)
            {
                if (customizableTestOptions != null)
                {
                    return customizableTestOptions;
                }
            }
            return null;
        }
        set
        {
            BindingContext = value;
        }
    }

    public OptionsViewAll()
    {
        InitializeComponent();

        if (SharedSpeechTestObjects.CurrentSpeechTest.TesterInstructions.Trim() != "")
        {
            ShowTesterInstructionsButton.IsVisible = true;
        }
        else
        {
            ShowTesterInstructionsButton.IsVisible = false;
        }

        if (SharedSpeechTestObjects.CurrentSpeechTest.ParticipantInstructions.Trim() != "")
        {
            ShowParticipantInstructionsButton.IsVisible = true;
        }
        else
        {
            ShowParticipantInstructionsButton.IsVisible = false;
        }

        PractiseTestControl.IsVisible = SharedSpeechTestObjects.CurrentSpeechTest.HasOptionalPractiseTest;

        if (SharedSpeechTestObjects.CurrentSpeechTest.AllowsManualPreSetSelection == true)
        {
            if (SelectedPreset_Picker.Items.Count > 0) { SelectedPreset_Picker.SelectedIndex = 0; }
            if (SelectedPreset_Picker.Items.Count < 2) { SelectedPreset_Picker.IsVisible = false; }
        }
        else { SelectedPreset_Picker.IsVisible = false; }

        if (SharedSpeechTestObjects.CurrentSpeechTest.AllowsManualStartListSelection == true)
        {
            if (StartList_Picker.Items.Count > 0) { StartList_Picker.SelectedIndex = 0; }
            if (StartList_Picker.Items.Count < 2) { StartList_Picker.IsVisible = false; }
        }
        else { StartList_Picker.IsVisible = false; }


        if (SharedSpeechTestObjects.CurrentSpeechTest.AllowsManualMediaSetSelection == true)
        {
            if (SelectedMediaSet_Picker.Items.Count > 0) { SelectedMediaSet_Picker.SelectedIndex = 0; }
            if (SelectedMediaSet_Picker.Items.Count < 2) { SelectedMediaSet_Picker.IsVisible = false; }
        }
        else { SelectedMediaSet_Picker.IsVisible = false; }

        ReferenceLevelControl.IsVisible = SharedSpeechTestObjects.CurrentSpeechTest.AllowsManualReferenceLevelSelection;
        SpeechLevelControl.IsVisible = SharedSpeechTestObjects.CurrentSpeechTest.AllowsManualSpeechLevelSelection;
        MaskerLevelControl.IsVisible = SharedSpeechTestObjects.CurrentSpeechTest.AllowsManualMaskingLevelSelection;
        BackgroundLevelControl.IsVisible = SharedSpeechTestObjects.CurrentSpeechTest.AllowsManualBackgroundLevelSelection;
        SetSoundFieldSimulationVisibility();

        // Outcommented 2024-11-02 to allow for showing speech level slider without the speech level direction control. May break other protocols?
        //if (SharedSpeechTestObjects.CurrentSpeechTest.AllowsManualSpeechLevelSelection)
        //{
        //    SpeechLevelControl.IsVisible = SharedSpeechTestObjects.CurrentSpeechTest.CanHaveTargets;
        //}

        if (SharedSpeechTestObjects.CurrentSpeechTest.AllowsManualMaskingLevelSelection)
        {
            MaskerLevelControl.IsVisible = SharedSpeechTestObjects.CurrentSpeechTest.CanHaveMaskers;
        }

        if (SharedSpeechTestObjects.CurrentSpeechTest.AllowsManualBackgroundLevelSelection)
        {
            BackgroundLevelControl.IsVisible = SharedSpeechTestObjects.CurrentSpeechTest.CanHaveBackgroundNonSpeech;
        }

        if (AvailableTestModes_Picker.Items.Count > 0) { AvailableTestModes_Picker.SelectedIndex = 0; }
        if (AvailableTestModes_Picker.Items.Count < 2) { AvailableTestModes_Picker.IsVisible = false; }

        if (AvailableTestProtocols_Picker.Items.Count > 0) { AvailableTestProtocols_Picker.SelectedIndex = 0; }
        if (AvailableTestProtocols_Picker.Items.Count < 2) { AvailableTestProtocols_Picker.IsVisible = false; }

        UseRetsplCorrection_Switch.IsToggled = false;
        UseRetsplCorrectionControl.IsVisible = false;

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
        if (AvailableFixedResponseAlternativeCounts_Picker.Items.Count < 2) { AvailableFixedResponseAlternativeCounts_Picker.IsVisible = false; }

        if (SelectedTransducer_Picker.Items.Count > 0) { SelectedTransducer_Picker.SelectedIndex = 0; }
        if (SelectedTransducer_Picker.Items.Count < 2) { SelectedTransducer_Picker.IsVisible = false; }

        SetSoundFieldSimulationVisibility();

        UpdateSoundSourceViewsIsVisible();

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

        PreListenControl.IsVisible = SharedSpeechTestObjects.CurrentSpeechTest.SupportsPrelistening;

    }

    private void SetSoundFieldSimulationVisibility()
    {

        if (OstfBase.AllowDirectionalSimulation == true & CurrentTransducerIsHeadPhones() == true & SharedSpeechTestObjects.CurrentSpeechTest.UseSoundFieldSimulation == STFN.Utils.Constants.TriState.Optional)
        {
            UseSimulatedSoundFieldControl.IsVisible = true;
        }
        else
        {
            if (SharedSpeechTestObjects.CurrentSpeechTest.UseSoundFieldSimulation == STFN.Utils.Constants.TriState.True)
            {
                UseSimulatedSoundField_Switch.IsToggled = true;
            }
            else
            {
                UseSimulatedSoundField_Switch.IsToggled = false;
            }
            UseSimulatedSoundFieldControl.IsVisible = false;
            SelectedIrSet_Picker.IsVisible = false;
        }
    }

    private void UseFreeRecall_Switch_Toggled(object sender, ToggledEventArgs e)
    {
        AvailableFixedResponseAlternativeCounts_Picker.IsVisible = !e.Value;
        if (AvailableFixedResponseAlternativeCounts_Picker.Items.Count < 2) { AvailableFixedResponseAlternativeCounts_Picker.IsVisible = false; }
    }

    private void AvailableTestModes_Picker_SelectedIndexChanged(object sender, EventArgs e)
    {

        //SpeechTest.TestModes castItem = (SpeechTest.TestModes)AvailableTestModes_Picker.SelectedItem;

        //// Resetting default values
        //SpeechLevelControl.IsVisible = SharedSpeechTestObjects.CurrentSpeechTest.CanHaveTargets;
        //MaskerLevelControl.IsVisible = SharedSpeechTestObjects.CurrentSpeechTest.CanHaveMaskers;
        //BackgroundLevelControl.IsVisible = SharedSpeechTestObjects.CurrentSpeechTest.CanHaveBackgroundNonSpeech;

        //// Then hiding controls not to be used
        //if (castItem == SpeechTest.TestModes.AdaptiveSpeech)
        //{
        //    SpeechLevelControl.IsVisible = false;
        //}
        //if (castItem == SpeechTest.TestModes.AdaptiveNoise)
        //{
        //    MaskerLevelControl.IsVisible = false;
        //    BackgroundLevelControl.IsVisible = false;
        //}

    }

    private void SelectedTransducer_Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateSoundSourceLocations();
    }

    private void UseSimulatedSoundField_Switch_Toggled(object sender, ToggledEventArgs e)
    {
        SelectedIrSet_Picker.IsVisible = e.Value;

        if (e.Value == true)
        {

            if (SelectedIrSet_Picker.Items.Count > 0)
            {
                SelectedIrSet_Picker.SelectedIndex = 0;
            }
            else
            {
                Messager.MsgBox("No HRIR for sound field simulation has been loaded! Sound field simulation will be disabled!", Messager.MsgBoxStyle.Information, "Cannot locate needed resources!");
                UseSimulatedSoundField_Switch.IsEnabled = false;
                SelectedIrSet_Picker.IsEnabled = false;
                UseSimulatedSoundField_Switch.IsVisible = false;
                SelectedIrSet_Picker.IsVisible = false;
                return;
            }

            CurrentBindingContext.UseContralateralMasking = false;
        }

        UpdateSoundSourceLocations();
    }

    private void SelectedIrSet_Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateSoundSourceLocations();
    }

    private void UsePhaseAudiometry_Switch_Toggled(object sender, ToggledEventArgs e)
    {
        AvailablePhaseAudiometryTypes_Picker.IsVisible = e.Value;

        UpdateSoundSourceLocations();
    }


    private void UpdateSoundSourceLocations()
    {

        UpdateSoundSourceViewsIsVisible();
        UpdateContralteralNoiseIsVisible();

        // Clearing first
        SpeechSoundSourceView.SoundSources = new List<STFN.Audio.SoundScene.VisualSoundSourceLocation>();
        MaskerSoundSourceView.SoundSources = new List<STFN.Audio.SoundScene.VisualSoundSourceLocation>();
        BackgroundNonSpeechSoundSourceView.SoundSources = new List<STFN.Audio.SoundScene.VisualSoundSourceLocation>();
        BackgroundSpeechSoundSourceView.SoundSources = new List<STFN.Audio.SoundScene.VisualSoundSourceLocation>();

        if (CurrentBindingContext != null)
        {
            if (CurrentBindingContext.SelectedTransducer != null)
            {
                if (CurrentBindingContext.UseSimulatedSoundField == false)
                {
                    SpeechSoundSourceView.SoundSources = CurrentBindingContext.SelectedTransducer.GetVisualSoundSourceLocations();
                    MaskerSoundSourceView.SoundSources = CurrentBindingContext.SelectedTransducer.GetVisualSoundSourceLocations();
                    BackgroundNonSpeechSoundSourceView.SoundSources = CurrentBindingContext.SelectedTransducer.GetVisualSoundSourceLocations();
                    BackgroundSpeechSoundSourceView.SoundSources = CurrentBindingContext.SelectedTransducer.GetVisualSoundSourceLocations();
                }
                else
                {
                    // Adding simulated sound filed locations
                    if (CurrentBindingContext.SelectedIrSet != null)
                    {
                        SpeechSoundSourceView.SoundSources = CurrentBindingContext.SelectedIrSet.GetVisualSoundSourceLocations();
                        MaskerSoundSourceView.SoundSources = CurrentBindingContext.SelectedIrSet.GetVisualSoundSourceLocations();
                        BackgroundNonSpeechSoundSourceView.SoundSources = CurrentBindingContext.SelectedIrSet.GetVisualSoundSourceLocations();
                        BackgroundSpeechSoundSourceView.SoundSources = CurrentBindingContext.SelectedIrSet.GetVisualSoundSourceLocations();
                    }
                }


                if (CurrentBindingContext.SelectedTransducer.IsHeadphones())
                {

                    if (CurrentBindingContext.UseSimulatedSoundField == false)
                    {
                        // If the transducer is headphones, dB HL should be used, as long as the headphones do not present a simulated sound field
                        UseRetsplCorrection_Switch.IsToggled = true;

                        if (SharedSpeechTestObjects.CurrentSpeechTest.AllowsUseRetsplChoice)
                        {
                            // Allowing the user to swap values by showing the switch
                            UseRetsplCorrectionControl.IsVisible = true;
                        }
                        else
                        {
                            UseRetsplCorrectionControl.IsVisible = false;
                        }
                    }
                    else
                    {
                        // It's a simulated sound field, dB SPL should be used
                        UseRetsplCorrection_Switch.IsToggled = false;
                        if (SharedSpeechTestObjects.CurrentSpeechTest.AllowsUseRetsplChoice)
                        {
                            // Allowing the user to swap values by showing the switch
                            UseRetsplCorrectionControl.IsVisible = true;
                        }
                        else
                        {
                            UseRetsplCorrectionControl.IsVisible = false;
                        }
                    }
                }
                else
                {
                    // If the transducer is not headphones, dB HL shold not be used
                    UseRetsplCorrection_Switch.IsToggled = false;
                    UseRetsplCorrectionControl.IsVisible = false;

                    // We can not use simulated sound field when headphones are not used
                    UseSimulatedSoundField_Switch.IsToggled = false;
                }

                SetSoundFieldSimulationVisibility();

            }
        }
    }



    private void UpdateSoundSourceViewsIsVisible()
    {

        SpeechSoundSourceView.IsVisible = false;
        MaskerSoundSourceView.IsVisible = false;
        BackgroundNonSpeechSoundSourceView.IsVisible = false;
        BackgroundSpeechSoundSourceView.IsVisible = false;

        if (CurrentBindingContext.UsePhaseAudiometry == false)
        {
            SpeechSoundSourceView.IsVisible = SharedSpeechTestObjects.CurrentSpeechTest.CanHaveTargets;
            BackgroundSpeechSoundSourceView.IsVisible = SharedSpeechTestObjects.CurrentSpeechTest.CanHaveBackgroundSpeech;
            MaskerSoundSourceView.IsVisible = SharedSpeechTestObjects.CurrentSpeechTest.CanHaveMaskers;
            BackgroundNonSpeechSoundSourceView.IsVisible = SharedSpeechTestObjects.CurrentSpeechTest.CanHaveBackgroundNonSpeech;
        }
    }

    private void UpdateContralteralNoiseIsVisible()
    {

        // Hiding the UseContralateralMaskingControl in cases where it cannot be used, and deselecting UseContralateralMasking
        if (CurrentTransducerIsHeadPhones() == true &
            SharedSpeechTestObjects.CurrentSpeechTest.UseContralateralMasking == STFN.Utils.Constants.TriState.Optional &
            CurrentBindingContext.UseSimulatedSoundField == false &
            CurrentBindingContext.UsePhaseAudiometry == false)
        {
            UseContralateralMaskingControl.IsVisible = true;
        }
        else
        {
            CurrentBindingContext.UseContralateralMasking = false;
            UseContralateralMaskingControl.IsVisible = false;
        }

        UpdateContralteralNoiseLevelIsVisible();
    }

    private void UpdateContralteralNoiseLevelIsVisible()
    {
        if (UseContralateralMaskingControl.IsVisible && UseContralateralMasking_Switch.IsToggled)
        {
            ContralateralMaskingLevelControl.IsVisible = true;
            LockSpeechLevelToContralateralMaskingControl.IsVisible = true;
        }
        else
        {
            ContralateralMaskingLevelControl.IsVisible = false;
            LockSpeechLevelToContralateralMaskingControl.IsVisible = false;
        }
    }

    private void UseContralateralMaskingControl_Switch_Toggled(object sender, ToggledEventArgs e)
    {
        UpdateContralteralNoiseLevelIsVisible();
    }

    private bool CurrentTransducerIsHeadPhones()
    {
        if (CurrentBindingContext != null)
        {
            if (CurrentBindingContext.SelectedTransducer != null)
            {
                return CurrentBindingContext.SelectedTransducer.IsHeadphones();
            }
        }

        return false;
    }

    private void PreListenPlayButton_Clicked(object sender, EventArgs e)
    {

        var PreTestStimulus = SharedSpeechTestObjects.CurrentSpeechTest.CreatePreTestStimulus();
        STFN.Audio.Sound PreTestStimulusSound = PreTestStimulus.Item1;
        string PreTestStimulusSpelling = PreTestStimulus.Item2;
        PreListenSpellingLabel.Text = PreTestStimulusSpelling;
        OstfBase.SoundPlayer.SwapOutputSounds(ref PreTestStimulusSound);

    }

    private void PreListenStopButton_Clicked(object sender, EventArgs e)
    {
        OstfBase.SoundPlayer.FadeOutPlayback();
    }

    //private void PreListenLouderButton_Clicked(object sender, EventArgs e)
    //{
    //    CurrentBindingContext.SpeechLevel += 5;
    //    CurrentBindingContext.ContralateralMaskingLevel += 5;
    //}

    //private void PreListenSofterButton_Clicked(object sender, EventArgs e)
    //{
    //    CurrentBindingContext.SpeechLevel -= 5;
    //    CurrentBindingContext.ContralateralMaskingLevel -= 5;
    //}

    private void SpeechLevelSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (LockSpeechLevelToContralateralMasking_Switch != null)
        {
            if (LockSpeechLevelToContralateralMasking_Switch.IsToggled == true)
            {
                // Adjusting the contralateral masking by the new level difference
                double differenceValue = e.NewValue - e.OldValue;
                CurrentBindingContext.ContralateralMaskingLevel += differenceValue;
            }
        }
    }

    private async void ShowTesterInstructionsButton_Clicked(object sender, EventArgs e)
    {
        await Messager.MsgBoxAsync(SharedSpeechTestObjects.CurrentSpeechTest.TesterInstructions, Messager.MsgBoxStyle.Information, CurrentBindingContext.TesterInstructionsButtonText);
    }

    private async void ShowParticipantInstructionsButton_Clicked(object sender, EventArgs e)
    {
        await Messager.MsgBoxAsync(SharedSpeechTestObjects.CurrentSpeechTest.ParticipantInstructions, Messager.MsgBoxStyle.Information, CurrentBindingContext.ParticipantInstructionsButtonText);
    }
}