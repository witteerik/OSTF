
using CommunityToolkit.Maui.Media;
using STFN;
using System.Collections.ObjectModel;

namespace STFM.Views;

public partial class OptionsViewAll : ContentView
{

    private SpeechTest CurrentSpeechTest
    {
        get
        {
            if (BindingContext is SpeechTest speechTest)
            {
                if (speechTest != null)
                {
                    return speechTest;
                }
            }
            return null;
        }
        set
        {
            BindingContext = value;
        }
    }




    public OptionsViewAll(SpeechTest currentSpeechTest)
    {
        InitializeComponent();

        // Assign the custom instance of SpeechTest
        BindingContext = currentSpeechTest;

        CalibrationCheckControl.IsVisible = OstfBase.ShowCalibrationCheck;

        if (CurrentSpeechTest.TesterInstructions.Trim() != "")
        {
            ShowTesterInstructionsButton.IsVisible = true;
        }
        else
        {
            ShowTesterInstructionsButton.IsVisible = false;
        }

        if (CurrentSpeechTest.ParticipantInstructions.Trim() != "")
        {
            ShowParticipantInstructionsButton.IsVisible = true;
        }
        else
        {
            ShowParticipantInstructionsButton.IsVisible = false;
        }

        PractiseTestControl.IsVisible = CurrentSpeechTest.ShowGuiChoice_PractiseTest;

        if (CurrentSpeechTest.ShowGuiChoice_PreSet == true)
        {
            SelectedPreset_Picker.IsVisible = true;
            // Autoselects the first available PreSet, or hides the control if none is available
            if (SelectedPreset_Picker.Items.Count > 0) { SelectedPreset_Picker.SelectedIndex = 0; }
            if (SelectedPreset_Picker.Items.Count < 2) { SelectedPreset_Picker.IsVisible = false; }
        }
        else { SelectedPreset_Picker.IsVisible = false; }

        if (CurrentSpeechTest.AvailableExperimentNumbers.Length > 0)
        {
            if (ExperimentNumber_Picker.Items.Count > 0) { ExperimentNumber_Picker.SelectedIndex = 0; }
            if (ExperimentNumber_Picker.Items.Count < 2) { ExperimentNumberControl.IsVisible = false; }
        }
        else { ExperimentNumberControl.IsVisible = false; }

        if (CurrentSpeechTest.ShowGuiChoice_StartList == true)
        {
            if (StartList_Picker.Items.Count > 0) { StartList_Picker.SelectedIndex = 0; }
            if (StartList_Picker.Items.Count < 2) { StartList_Picker.IsVisible = false; }
        }
        else { StartList_Picker.IsVisible = false; }


        if (CurrentSpeechTest.ShowGuiChoice_MediaSet == true)
        {
            if (SelectedMediaSet_Picker.Items.Count > 0) { SelectedMediaSet_Picker.SelectedIndex = 0; }
            if (SelectedMediaSet_Picker.Items.Count < 2) { SelectedMediaSet_Picker.IsVisible = false; }
        }
        else { SelectedMediaSet_Picker.IsVisible = false; }

        ReferenceLevelControl.IsVisible = CurrentSpeechTest.ShowGuiChoice_ReferenceLevel;
        TargetSNRControl.IsVisible = CurrentSpeechTest.ShowGuiChoice_TargetSNRLevel;
        SpeechLevelControl.IsVisible = CurrentSpeechTest.ShowGuiChoice_TargetLevel;
        MaskerLevelControl.IsVisible = CurrentSpeechTest.ShowGuiChoice_MaskingLevel;
        BackgroundLevelControl.IsVisible = CurrentSpeechTest.ShowGuiChoice_BackgroundLevel;
        SetSoundFieldSimulationVisibility();

        if (CurrentSpeechTest.ShowGuiChoice_MaskingLevel)
        {
            MaskerLevelControl.IsVisible = CurrentSpeechTest.CanHaveMaskers();
        }

        if (CurrentSpeechTest.ShowGuiChoice_BackgroundLevel)
        {
            BackgroundLevelControl.IsVisible = CurrentSpeechTest.CanHaveBackgroundNonSpeech();
        }

        if (AvailableTestModes_Picker.Items.Count > 0) { AvailableTestModes_Picker.SelectedIndex = 0; }
        if (AvailableTestModes_Picker.Items.Count < 2) { AvailableTestModes_Picker.IsVisible = false; }

        if (AvailableTestProtocols_Picker.Items.Count > 0) { AvailableTestProtocols_Picker.SelectedIndex = 0; }
        if (AvailableTestProtocols_Picker.Items.Count < 2) { AvailableTestProtocols_Picker.IsVisible = false; }

        UseRetsplCorrectionControl.IsVisible = CurrentSpeechTest.ShowGuiChoice_dBHL;

        KeyWordScoringControl.IsVisible = CurrentSpeechTest.ShowGuiChoice_KeyWordScoring;
        ListOrderRandomizationControl.IsVisible = CurrentSpeechTest.ShowGuiChoice_ListOrderRandomization;
        WithinListRandomizationControl.IsVisible = CurrentSpeechTest.ShowGuiChoice_WithinListRandomization;
        AcrossListRandomizationControl.IsVisible = CurrentSpeechTest.ShowGuiChoice_AcrossListRandomization;
        UseFreeRecallControl.IsVisible = CurrentSpeechTest.ShowGuiChoice_FreeRecall;
        UseDidNotHearAlternativeControl.IsVisible = CurrentSpeechTest.ShowGuiChoice_DidNotHearAlternative;

        // Selecting the third alternative, if possible, otherwise the second of first
        if (AvailableFixedResponseAlternativeCounts_Picker.Items.Count > 0) { AvailableFixedResponseAlternativeCounts_Picker.SelectedIndex = 0; }
        if (AvailableFixedResponseAlternativeCounts_Picker.Items.Count > 1) { AvailableFixedResponseAlternativeCounts_Picker.SelectedIndex = 1; }
        if (AvailableFixedResponseAlternativeCounts_Picker.Items.Count > 2) { AvailableFixedResponseAlternativeCounts_Picker.SelectedIndex = 2; }
        if (AvailableFixedResponseAlternativeCounts_Picker.Items.Count < 2) { AvailableFixedResponseAlternativeCounts_Picker.IsVisible = false; }

        if (SelectedTransducer_Picker.Items.Count > 0) { SelectedTransducer_Picker.SelectedIndex = 0; }
        if (SelectedTransducer_Picker.Items.Count < 2) { SelectedTransducer_Picker.IsVisible = false; }

        SetSoundFieldSimulationVisibility();

        UpdateSoundSourceViewsIsVisible();

        UsePhaseAudiometryControl.IsVisible = CurrentSpeechTest.ShowGuiChoice_PhaseAudiometry;

        if (AvailablePhaseAudiometryTypes_Picker.Items.Count > 0) { AvailablePhaseAudiometryTypes_Picker.SelectedIndex = 0; }
        AvailablePhaseAudiometryTypes_Picker.IsVisible = false;

        PreListenControl.IsVisible = CurrentSpeechTest.SupportsPrelistening;

    }



    private void SetSoundFieldSimulationVisibility()
    {

        if (OstfBase.AllowDirectionalSimulation == true & CurrentTransducerIsHeadPhones() == true & CurrentSpeechTest.ShowGuiChoice_SoundFieldSimulation == true)
        {
            UseSimulatedSoundFieldControl.IsVisible = true;
        }
        else
        {
            if (CurrentSpeechTest.SimulatedSoundField == true)
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
        //SpeechLevelControl.IsVisible = CurrentSpeechTest.CanHaveTargets;
        //MaskerLevelControl.IsVisible = CurrentSpeechTest.CanHaveMaskers;
        //BackgroundLevelControl.IsVisible = CurrentSpeechTest.CanHaveBackgroundNonSpeech;

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
            // SelectedIrSet_Picker.Items.Count > 0
            if (CurrentSpeechTest.CurrentlySupportedIrSets.Count > 0)
            {
                SelectedIrSet_Picker.SelectedIndex = 0;
            }
            else
            {
                Messager.MsgBox("No HRIR for sound field simulation has been loaded! Sound field simulation will be disabled!", Messager.MsgBoxStyle.Information, "Cannot locate needed resources!");
                UseSimulatedSoundField_Switch.IsToggled = false;
                //CurrentSpeechTest.UseSoundFieldSimulation = STFN.Utils.Constants.TriState.False; 'Cannot be done, as this is readonly!
                // Hiding the controls instead
                UseSimulatedSoundFieldControl.IsEnabled = false;
                UseSimulatedSoundFieldControl.IsVisible = false;
                UseSimulatedSoundField_Label.IsEnabled = false;
                UseSimulatedSoundField_Label.IsVisible = false;
                UseSimulatedSoundField_Switch.IsEnabled = false;
                UseSimulatedSoundField_Switch.IsVisible = false;
                SelectedIrSet_Picker.IsVisible = false;
                SelectedIrSet_Picker.IsEnabled = false;
                return;
            }

            CurrentSpeechTest.ContralateralMasking = false;
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

        if (CurrentSpeechTest != null)
        {
            if (CurrentSpeechTest.Transducer != null)
            {
                if (CurrentSpeechTest.SimulatedSoundField == false)
                {
                    SpeechSoundSourceView.SoundSources = CurrentSpeechTest.Transducer.GetVisualSoundSourceLocations();
                    MaskerSoundSourceView.SoundSources = CurrentSpeechTest.Transducer.GetVisualSoundSourceLocations();
                    BackgroundNonSpeechSoundSourceView.SoundSources = CurrentSpeechTest.Transducer.GetVisualSoundSourceLocations();
                    BackgroundSpeechSoundSourceView.SoundSources = CurrentSpeechTest.Transducer.GetVisualSoundSourceLocations();
                }
                else
                {
                    // Adding simulated sound filed locations
                    if (CurrentSpeechTest.IrSet != null)
                    {
                        SpeechSoundSourceView.SoundSources = CurrentSpeechTest.IrSet.GetVisualSoundSourceLocations();
                        MaskerSoundSourceView.SoundSources = CurrentSpeechTest.IrSet.GetVisualSoundSourceLocations();
                        BackgroundNonSpeechSoundSourceView.SoundSources = CurrentSpeechTest.IrSet.GetVisualSoundSourceLocations();
                        BackgroundSpeechSoundSourceView.SoundSources = CurrentSpeechTest.IrSet.GetVisualSoundSourceLocations();
                    }
                }


                if (CurrentSpeechTest.Transducer.IsHeadphones())
                {

                    if (CurrentSpeechTest.SimulatedSoundField == false)
                    {
                        // If the transducer is headphones, dB HL should be used, as long as the headphones do not present a simulated sound field
                        UseRetsplCorrection_Switch.IsToggled = true;
                        UseRetsplCorrectionControl.IsVisible = CurrentSpeechTest.ShowGuiChoice_dBHL;

                    }
                    else
                    {
                        // It's a simulated sound field, dB SPL should be used
                        UseRetsplCorrection_Switch.IsToggled = false;
                        if (CurrentSpeechTest.ShowGuiChoice_dBHL)
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

        if (CurrentSpeechTest.PhaseAudiometry == false)
        {
            SpeechSoundSourceView.IsVisible = CurrentSpeechTest.ShowGuiChoice_TargetLocations;
            BackgroundSpeechSoundSourceView.IsVisible = CurrentSpeechTest.ShowGuiChoice_BackgroundSpeechLocations;
            MaskerSoundSourceView.IsVisible = CurrentSpeechTest.ShowGuiChoice_MaskerLocations;
            BackgroundNonSpeechSoundSourceView.IsVisible = CurrentSpeechTest.ShowGuiChoice_BackgroundNonSpeechLocations;
        }
    }

    private void UpdateContralteralNoiseIsVisible()
    {

        bool ShowGuiChoice_ContralateralMasking = CurrentSpeechTest.ShowGuiChoice_ContralateralMasking;

        UseContralateralMaskingControl.IsVisible = ShowGuiChoice_ContralateralMasking;
        UseContralateralMasking_Switch.IsVisible = ShowGuiChoice_ContralateralMasking;

        UpdateContralateralNoiseLevelIsVisible();
    }

    private void UseContralateralMaskingControl_Switch_Toggled(object sender, ToggledEventArgs e)
    {
        UpdateContralateralNoiseLevelIsVisible();
    }

    private void UpdateContralateralNoiseLevelIsVisible()
    {

        bool ShowGuiChoice_ContralateralMaskingLevel = CurrentSpeechTest.ShowGuiChoice_ContralateralMaskingLevel;

        UseContralateralMaskingControl.IsVisible = ShowGuiChoice_ContralateralMaskingLevel;

        if (CurrentSpeechTest.ContralateralMasking == true)
        {
            ContralateralMaskingLevelControl.IsVisible = ShowGuiChoice_ContralateralMaskingLevel;
            LockSpeechLevelToContralateralMaskingControl.IsVisible = ShowGuiChoice_ContralateralMaskingLevel;
        }
        else
        {
            ContralateralMaskingLevelControl.IsVisible = false;
            LockSpeechLevelToContralateralMaskingControl.IsVisible = false;
        }
    }


    private bool CurrentTransducerIsHeadPhones()
    {
        if (CurrentSpeechTest != null)
        {
            if (CurrentSpeechTest.Transducer != null)
            {
                return CurrentSpeechTest.Transducer.IsHeadphones();
            }
        }

        return false;
    }

    private void PreListenPlayButton_Clicked(object sender, EventArgs e)
    {

        var PreTestStimulus = CurrentSpeechTest.CreatePreTestStimulus();
        STFN.Audio.Sound PreTestStimulusSound = PreTestStimulus.Item1;
        string PreTestStimulusSpelling = PreTestStimulus.Item2;
        PreListenSpellingLabel.Text = PreTestStimulusSpelling;
        OstfBase.SoundPlayer.SwapOutputSounds(ref PreTestStimulusSound);

    }

    private void PreListenStopButton_Clicked(object sender, EventArgs e)
    {
        OstfBase.SoundPlayer.FadeOutPlayback();
    }

    private void CalibrationCheckPlayButton_Clicked(object sender, EventArgs e)
    {
        var CalibrationStimulus = CurrentSpeechTest.CreateCalibrationCheckSignal();
        if (CalibrationStimulus != null)
        {
            STFN.Audio.Sound CalibrationSignal = CalibrationStimulus.Item1;
            OstfBase.SoundPlayer.SwapOutputSounds(ref CalibrationSignal);
            CalibrationCheckInfoLabel.Text = CalibrationStimulus.Item2;
        }
    }

    private void CalibrationCheckStopButton_Clicked(object sender, EventArgs e)
    {
        OstfBase.SoundPlayer.FadeOutPlayback();
        CalibrationCheckInfoLabel.Text = "";
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

    //private void SpeechLevelSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    //{
    //    if (LockSpeechLevelToContralateralMasking_Switch != null)
    //    {
    //        if (LockSpeechLevelToContralateralMasking_Switch.IsToggled == true)
    //        {
    //            // Adjusting the contralateral masking by the new level difference
    //            double differenceValue = e.NewValue - e.OldValue;
    //            CurrentSpeechTest.ContralateralMaskingLevel += differenceValue;
    //        }
    //    }
    //}


    private async void ShowTesterInstructionsButton_Clicked(object sender, EventArgs e)
    {
        await Messager.MsgBoxAsync(CurrentSpeechTest.TesterInstructions, Messager.MsgBoxStyle.Information, CurrentSpeechTest.TesterInstructionsButtonText);
    }

    private async void ShowParticipantInstructionsButton_Clicked(object sender, EventArgs e)
    {
        await Messager.MsgBoxAsync(CurrentSpeechTest.ParticipantInstructions, Messager.MsgBoxStyle.Information, CurrentSpeechTest.ParticipantInstructionsButtonText);
    }

}