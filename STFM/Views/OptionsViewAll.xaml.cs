
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

        //Updating possible source lications
        List<STFN.Audio.SoundScene.VisualSoundSourceLocation> SignalLocations = new List<STFN.Audio.SoundScene.VisualSoundSourceLocation>();
        SignalLocations.Add(new STFN.Audio.SoundScene.VisualSoundSourceLocation(new STFN.Audio.SoundScene.SoundSourceLocation { Distance = 0.8, Elevation = 0, HorizontalAzimuth = 0 }));
        SignalLocations.Add(new STFN.Audio.SoundScene.VisualSoundSourceLocation(new STFN.Audio.SoundScene.SoundSourceLocation { Distance = 0.5, Elevation = 0, HorizontalAzimuth = 30 }));
        SignalLocations.Add(new STFN.Audio.SoundScene.VisualSoundSourceLocation(new STFN.Audio.SoundScene.SoundSourceLocation { Distance = 0.5, Elevation = 0, HorizontalAzimuth = -30 }));
        //SignalLocations.Add(new STFN.Audio.SoundScene.VisualSoundSourceLocation(new STFN.Audio.SoundScene.SoundSourceLocation { Distance = 0.5, Elevation = 0, HorizontalAzimuth = 90 }));
        SignalLocations.Add(new STFN.Audio.SoundScene.VisualSoundSourceLocation(new STFN.Audio.SoundScene.SoundSourceLocation { Distance = 0.5, Elevation = 0, HorizontalAzimuth = -90 }));
        SignalLocations.Add(new STFN.Audio.SoundScene.VisualSoundSourceLocation(new STFN.Audio.SoundScene.SoundSourceLocation { Distance = 0.5, Elevation = 0, HorizontalAzimuth = 150 }));
        SignalLocations.Add(new STFN.Audio.SoundScene.VisualSoundSourceLocation(new STFN.Audio.SoundScene.SoundSourceLocation { Distance = 0.5, Elevation = 0, HorizontalAzimuth = -150 }));
        //SignalLocations.Add(new STFN.Audio.SoundScene.VisualSoundSourceLocation(new STFN.Audio.SoundScene.SoundSourceLocation { Distance = 0.5, Elevation = 0, HorizontalAzimuth = 180 }));
        SpeechSoundSourceView.SoundSources = SignalLocations;

    }
}