// N.B. This is an AndroidAudioTrackPlayer class file, never used but needed in order to compile the real Android verison of the player along with non-androd code.

using STFN.Audio;
using STFN.Audio.Formats;
using STFN.Audio.SoundPlayers;
using STFN.Audio.SoundScene;

namespace STFM
{
    public class AndroidAudioTrackPlayer : STFN.Audio.SoundPlayers.iSoundPlayer
    {
        private bool disposedValue;

        public AndroidAudioTrackPlayer(ref AndroidAudioTrackPlayerSettings AudioSettings, ref DuplexMixer Mixer)
        {
            throw new Exception("This class should not be used!");
        }

        bool iSoundPlayer.WideFormatSupport => throw new NotImplementedException();

        bool iSoundPlayer.RaisePlaybackBufferTickEvents { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        bool iSoundPlayer.EqualPowerCrossFade { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        bool iSoundPlayer.IsPlaying => throw new NotImplementedException();

        public double TalkbackGain { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool SupportsTalkBack => throw new NotImplementedException();

        float iSoundPlayer.TalkbackGain { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        event iSoundPlayer.MessageFromPlayerEventHandler iSoundPlayer.MessageFromPlayer
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event iSoundPlayer.StartedSwappingOutputSoundsEventHandler iSoundPlayer.StartedSwappingOutputSounds
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event iSoundPlayer.FinishedSwappingOutputSoundsEventHandler iSoundPlayer.FinishedSwappingOutputSounds
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event iSoundPlayer.FatalPlayerErrorEventHandler iSoundPlayer.FatalPlayerError
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        void iSoundPlayer.ChangePlayerSettings( AudioSettings AudioApiSettings, int? SampleRate, int? BitDepth, WaveFormat.WaveFormatEncodings? Encoding, double? OverlapDuration,  DuplexMixer Mixer, iSoundPlayer.SoundDirections? SoundDirection, bool ReOpenStream, bool ReStartStream, bool? ClippingIsActivated)
        {
            throw new NotImplementedException();
        }

        void iSoundPlayer.CloseStream()
        {
            throw new NotImplementedException();
        }

        void iSoundPlayer.Dispose()
        {
            throw new NotImplementedException();
        }

        void iSoundPlayer.FadeOutPlayback()
        {
            throw new NotImplementedException();
        }

        double iSoundPlayer.GetOverlapDuration()
        {
            throw new NotImplementedException();
        }

        Sound iSoundPlayer.GetRecordedSound(bool ClearRecordingBuffer)
        {
            throw new NotImplementedException();
        }

        bool iSoundPlayer.SwapOutputSounds(ref Sound NewOutputSound, bool Record, bool AppendRecordedSound)
        {
            throw new NotImplementedException();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~AndroidAudioTrackPlayer()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public static bool CheckIfDeviceExists(string DeviceProductName, bool IsOutput) { throw new Exception(); }

        public string GetAvaliableOutputDeviceNames(){throw new Exception();}
        public static int GetNumberChannelsOnDevice(string DeviceProductName, bool IsOutput) { throw new Exception(); }

        public bool SetOutputDevice(string DeviceProductName){throw new Exception();}

        public void StartTalkback()
        {
            throw new NotImplementedException();
        }

        public void StopTalkback()
        {
            throw new NotImplementedException();
        }

        public DuplexMixer GetMixer()
        {
            throw new NotImplementedException();
        }

        public static string GetAvaliableDeviceInformation()
        {
            throw new Exception();
        }

    }


    public partial class AccessNotificationPolicy : Microsoft.Maui.ApplicationModel.Permissions.BasePermission
    {
        public AccessNotificationPolicy() { }

        public override Task<PermissionStatus> CheckStatusAsync()
        {
            throw new NotImplementedException();
        }

        public override void EnsureDeclared()
        {
            throw new NotImplementedException();
        }

        public override Task<PermissionStatus> RequestAsync()
        {
            throw new NotImplementedException();
        }

        public override bool ShouldShowRationale()
        {
            throw new NotImplementedException();
        }

    }

}

