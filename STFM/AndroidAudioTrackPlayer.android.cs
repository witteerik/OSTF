using Android.Media;
using STFN.Audio;
using STFN.Audio.Formats;
using STFN.Audio.SoundPlayers;
using STFN.Audio.SoundScene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using AndroidX.Core.App;
using System.Threading;
using Android.Runtime;
using Java.Interop;


namespace STFM
{
    internal class AndroidAudioTrackPlayer : STFN.Audio.SoundPlayers.iSoundPlayer
    {
        bool iSoundPlayer.WideFormatSupport { get { return true; } }

        private bool raisePlaybackBufferTickEvents = false;
        bool iSoundPlayer.RaisePlaybackBufferTickEvents
        {
            get { return raisePlaybackBufferTickEvents; }
            set { raisePlaybackBufferTickEvents = value; }
        }

        private bool equalPowerCrossFade = false;
        bool iSoundPlayer.EqualPowerCrossFade
        {
            get { return equalPowerCrossFade; }
            set { equalPowerCrossFade = value; }
        }

        bool IsPlaying = false;

        public event iSoundPlayer.MessageFromPlayerEventHandler MessageFromPlayer;
        public event iSoundPlayer.StartedSwappingOutputSoundsEventHandler StartedSwappingOutputSounds;
        public event iSoundPlayer.FinishedSwappingOutputSoundsEventHandler FinishedSwappingOutputSounds;

        bool iSoundPlayer.IsPlaying
        {
            get { return IsPlaying; }
        }

        double overlapDuration = 0.1;
        int overlapGranuality = 5;

        void iSoundPlayer.SetOverlapDuration(double Duration)
        {
            overlapDuration = Duration;
        }

        double iSoundPlayer.GetOverlapDuration()
        {
            return overlapDuration;
        }
        void iSoundPlayer.SetOverlapGranuality(int Granuality)
        {
            overlapGranuality = Granuality;
        }

        int iSoundPlayer.GetOverlapGranuality()
        {
            return overlapGranuality;
        }

        int bufferSize;

        STFN.Audio.Formats.WaveFormat CurrentFormat = null;

        DuplexMixer Mixer;

        public DuplexMixer GetMixer()
        {
            return Mixer;
        }

        Object audioTrack = null;

        //AudioTrack audioTrack1 = null;


        [SupportedOSPlatform("Android31.0")]
        public AndroidAudioTrackPlayer( ref DuplexMixer Mixer, int bufferSize = 512)
        {
            this.bufferSize = bufferSize;
            this.Mixer = Mixer;

        }

        [SupportedOSPlatform("Android31.0")]
        bool setupSoundTracks()
        {

            if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {

                // Create AudioTrack with PCM float format using AudioTrack.Builder
                var audioTrackBuilder = new AudioTrack.Builder();

                //if (Mixer.OutputRouting.Count!=2)
                //{
                //    // TODO: Finish this later
                //}

                audioTrackBuilder.SetAudioFormat(new AudioFormat.Builder()
                    .SetEncoding(Android.Media.Encoding.Pcm32bit)
                    .SetSampleRate((int)CurrentFormat.SampleRate)
                    .SetChannelMask(ChannelOut.Mono)
                    .Build());


                // Set buffer size here
                //audioTrackBuilder.SetBufferSizeInBytes(numSamples * 4);
                audioTrackBuilder.SetBufferSizeInBytes(30000 * 4);

                audioTrackBuilder.SetAudioAttributes(new Android.Media.AudioAttributes.Builder()
                    .SetUsage(AudioUsageKind.Media)
                    .SetContentType(AudioContentType.Music)
                    .Build());

                audioTrackBuilder.SetPerformanceMode(AudioTrackPerformanceMode.LowLatency);

                // Building the audio tracks
                audioTrack = audioTrackBuilder.Build();
               
                AudioTrack castAudioTrack1 = (AudioTrack)audioTrack;

                castAudioTrack1.SetNotificationMarkerPosition(4);
                castAudioTrack1.MarkerReached += MarkerReached;

                //castAudioTrack1.SetPlaybackPositionUpdateListener(this);
                
                int markerPosition = 2000;
                castAudioTrack1.SetNotificationMarkerPosition(markerPosition);

                // Start playback
                castAudioTrack1.Play();

                if (castAudioTrack1.PlayState != PlayState.Playing)
                {
                    return false;
                }

                return true;
            }
            else
            {
                return false;
            }

        }

        private void MarkerReached(object sender, AudioTrack.MarkerReachedEventArgs e)
        {

            var x = 1;

        }

        void iSoundPlayer.ChangePlayerSettings(ref AudioApiSettings AudioApiSettings, int? SampleRate, int? BitDepth, WaveFormat.WaveFormatEncodings? Encoding, double? OverlapDuration, ref DuplexMixer Mixer, iSoundPlayer.SoundDirections? SoundDirection, bool ReOpenStream, bool ReStartStream, bool? ClippingIsActivated)
        {
            //Ignores any calls for now.
        }

        void iSoundPlayer.CloseStream()
        {
            //throw new NotImplementedException();
        }

        void iSoundPlayer.Dispose()
        {
            //throw new NotImplementedException();
        }

        void iSoundPlayer.FadeOutPlayback()
        {
            //throw new NotImplementedException();
        }

    

        Sound iSoundPlayer.GetRecordedSound(bool ClearRecordingBuffer)
        {
            throw new NotImplementedException("The AndroidAudioTrackPlayer cannot record sound.");
        }

        [SupportedOSPlatform("Android31.0")]
        bool iSoundPlayer.SwapOutputSounds(ref Sound NewOutputSound, bool Record, bool AppendRecordedSound)
        {
            if (CurrentFormat == null)
            {
                CurrentFormat = NewOutputSound.WaveFormat;
                if (setupSoundTracks() == false)
                {
                    throw new Exception("Unable to start the sound AndroidAudioTrackPlayer");
                };
            }

            if (NewOutputSound.WaveFormat.IsEqual(ref CurrentFormat, true, true, true, true) == false)
            {
                CurrentFormat = NewOutputSound.WaveFormat;
                if (setupSoundTracks() == false)
                {
                    throw new Exception("Unable to start the sound AndroidAudioTrackPlayer");
                };
            }

            return  playNewSound_IeeeFloatingPoints(ref NewOutputSound);

        }



        [SupportedOSPlatform("Android31.0")]
        bool playNewSound_IeeeFloatingPoints(ref Sound NewOutputSound)
        {

            if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {

                AudioTrack castAudioTrack = (AudioTrack)audioTrack;

                //castAudioTrack.SetVolume();

                //AudioTrack.StreamEventCallback StreamEventCallback()
                //castAudioTrack.AddOnRoutingChangedListener()

                if (castAudioTrack.PlayState != PlayState.Playing)
                {
                    throw new Exception("Sound player not running anymore!");
                }

                // This should be async ? or on another thread

                if (NewOutputSound.WaveFormat.IsEqual(ref CurrentFormat, true, true, true, true) == false)
                {
                    throw new Exception("The format of the new sound does not agree with the format for which the sound player was instantiated.");
                }


                // Getting the sound samples and writing them to the player
                int channel = 1;

                // Getting a gain factor. This could be the channel dependent speaker calibration value. It could also be multiplied by another "online" gain factor applied to the sound.
                double GainFactor = 1;

                // Limiting BitdepthScaling to Int32.MaxValue // May be unecessary
                double BitdepthScaling = Math.Min(GainFactor * Int32.MaxValue, Int32.MaxValue);

                // Getting the channel sample array
                float[] sample = NewOutputSound.WaveData.get_SampleData(channel);

                // Getting the number of samples
                int numSamples = sample.Length;

                // Convert samples to byte array
                byte[] generatedSnd = new byte[4 * numSamples];
                int idx = 0;
                foreach (float sampleValue in sample)
                {
                    byte[] val_bytes = BitConverter.GetBytes((Int32)(sampleValue*BitdepthScaling));
                    val_bytes.CopyTo(generatedSnd, idx);
                    idx += 4;
                }

                try
                {

                    // Write the generated audio data to the AudioTrack
                    castAudioTrack.Write(generatedSnd, 0, generatedSnd.Length, WriteMode.Blocking);

                    return true;

                }
                catch (Exception ex)
                {
                    // Handle any exceptions
                    //Console.WriteLine("Error playing audio: " + ex.Message);

                    return false;

                }

            }else{
                return false;
            }

        }  

    }
}





//public class CustomAudioTrack : AudioTrack, AudioTrack.IOnPlaybackPositionUpdateListener
//{
//    public CustomAudioTrack(AudioAttributes attributes, AudioFormat format, int bufferSizeInBytes, AudioTrackMode mode, int sessionId)
//        : base(attributes, format, bufferSizeInBytes, mode, sessionId)
//    {
//        // Set the listener to the current instance
//        SetPlaybackPositionUpdateListener(this);
//    }

//    public event EventHandler Clicked;

//    public event EventHandler MarkerReached(MarkerReachedEventArgs);

//    // Callback when the playback head reaches a marker set by SetNotificationMarkerPosition
//    public void OnMarkerReached(AudioTrack track)
//    {
//        // Implement your callback logic here
//        // This method will be called when the playback head reaches the specified marker
//    }

//    // Callback for periodic updates during playback
//    public void OnPeriodicNotification(AudioTrack track)
//    {
//        // This method can be used for periodic updates if needed
//    }

//    // Example method to start playback and set a marker
//    public void StartPlaybackWithMarker(int markerInFrames)
//    {
//        // Set a marker at a specified frame position
//        SetNotificationMarkerPosition(markerInFrames);
//        Play();
//    }
//}




