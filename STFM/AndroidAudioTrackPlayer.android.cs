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
using System.Runtime.InteropServices;
using STFN;
using Android.Bluetooth;
using System.Security.AccessControl;
//using STFM.PlatForms.Android;
//using Platforms.Android;
//using Java.Lang;

using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Google.ErrorProne.Annotations.Concurrent;


namespace STFM
{

    public class AndroidAudioTrackPlayer : STFN.Audio.SoundPlayers.iSoundPlayer
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

        public event iSoundPlayer.MessageFromPlayerEventHandler MessageFromPlayer;
        public event iSoundPlayer.StartedSwappingOutputSoundsEventHandler StartedSwappingOutputSounds;
        public event iSoundPlayer.FinishedSwappingOutputSoundsEventHandler FinishedSwappingOutputSounds;

        public bool IsPlaying
        {
            get
            {
                if (audioTrack != null)
                {
                    AudioTrack castAudioTrack = (AudioTrack)audioTrack;
                    if (castAudioTrack.PlayState == PlayState.Playing)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public void SetOverlapDuration(double Duration)
        {
            //Enforcing at least one sample overlap
            OverlapFrameCount = (int)Math.Max(1, (double)SampleRate * Duration);
        }

        public double GetOverlapDuration()
        {
            return _OverlapFrameCount / SampleRate;
        }
        public void SetOverlapGranuality(int Granuality)
        {
            //Ignored by the in this sound player, sinse it's using samplewise granuality by default
        }

        public int GetOverlapGranuality()
        {
            return _OverlapFrameCount;
        }

        STFN.Audio.Formats.WaveFormat CurrentFormat = null;

        DuplexMixer Mixer;

        public DuplexMixer GetMixer()
        {
            return Mixer;
        }

        Object audioTrack = null; // This is declared as an Object instead of AudioTrack since it will otherwise register an error in the Visual Studio editor.

        private System.Threading.SpinLock callbackSpinLock = new System.Threading.SpinLock();

        private STFN.Audio.PortAudioVB.PortAudioBasedSoundPlayer.BufferHolder[] OutputSoundA;
        private STFN.Audio.PortAudioVB.PortAudioBasedSoundPlayer.BufferHolder[] OutputSoundB;
        private STFN.Audio.PortAudioVB.PortAudioBasedSoundPlayer.BufferHolder[] NewSound;
        private STFN.Audio.PortAudioVB.PortAudioBasedSoundPlayer.BufferHolder[] SilentSound;

        private float[] OverlapFadeInArray;
        private float[] OverlapFadeOutArray;

        private int PositionA;
        private int PositionB;
        private int CrossFadeProgress;

        private OutputSounds CurrentOutputSound = OutputSounds.OutputSoundA;
        private enum OutputSounds
        {
            OutputSoundA,
            OutputSoundB,
            FadingToB,
            FadingToA,
        }

        private float[] SilentBuffer = new float[512];
        private float[] PlaybackBuffer = new float[512];
        int FramesPerBuffer;
        int NumberOfOutputChannels; // This corresponds to the number higest numbered physical output channel on the selected device.
        int SampleRate;

        volatile bool runBufferLoop = true;
        int buffersSent = 0;

        [SupportedOSPlatform("Android31.0")]
        public AndroidAudioTrackPlayer()
        {
            this.FramesPerBuffer = 2 * 2048;

            STFN.Audio.SoundScene.DuplexMixer Mixer = new STFN.Audio.SoundScene.DuplexMixer();
            int[] OutputChannels = new int[] { 1, 2 };
            Mixer.DirectMonoSoundToOutputChannels(ref OutputChannels);

            this.Mixer = Mixer;
        }


        //[SupportedOSPlatform("Android31.0")]
        //public AndroidAudioTrackPlayer(ref DuplexMixer Mixer, int bufferSize = 4*2048)
        //{
        //    this.FramesPerBuffer = bufferSize;
        //    this.Mixer = Mixer;
        //}



        [SupportedOSPlatform("Android31.0")]
        void StartPlayer()
        {

            if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {

                this.SampleRate = (int)CurrentFormat.SampleRate;
                NumberOfOutputChannels = Mixer.GetHighestOutputChannel();
                SetOverlapDuration(0.05);
                SilentBuffer = new float[FramesPerBuffer * NumberOfOutputChannels];
                PlaybackBuffer = new float[FramesPerBuffer * NumberOfOutputChannels];

                // Cretaing a ChannelIndexMask representing the output channels
                // Define a bit array containing boolean values for channels 1-32 ?
                // https://developer.android.com/reference/android/media/AudioFormat.Builder#setChannelIndexMask(int)

                List<bool> channelInclusionList = new List<bool>();
                for (int c = 1; c <= Mixer.GetHighestOutputChannel(); c++)
                {
                    if (Mixer.OutputRouting.ContainsKey(c))
                    {
                        channelInclusionList.Add(true);
                    }
                    else
                    {
                        // Maybe this will also require true, and that silent buffers are sent to these channels. Probably it will work with false, but then the Buffer stucture may need to be adjusted 
                        channelInclusionList.Add(false);
                    }
                }

                System.Collections.BitArray bitArray = new System.Collections.BitArray(channelInclusionList.ToArray()); // channel 1, channel 2, ...

                // This is how a hard coded eight channel system channel mask would look
                //System.Collections.BitArray bitArray_NotUsed = new System.Collections.BitArray(new bool[] { true, true, true, true, true, true, true, true }); // channel 1, channel 2, ...

                // Create a byte array with length of 4 (32 bits)
                byte[] bytes = new byte[4];

                // Copy bits from the BitArray to the byte array
                bitArray.CopyTo(bytes, 0);

                // Convert byte array to integer
                int ChannelIndexMask = BitConverter.ToInt32(bytes, 0);

                // Create AudioTrack with PCM float format using AudioTrack.Builder
                var audioTrackBuilder = new AudioTrack.Builder();

                audioTrackBuilder.SetAudioFormat(new AudioFormat.Builder()
                    .SetEncoding(Android.Media.Encoding.PcmFloat)
                    .SetSampleRate((int)CurrentFormat.SampleRate)
                    .SetChannelIndexMask(ChannelIndexMask)
                    .Build());

                audioTrackBuilder.SetBufferSizeInBytes(NumberOfOutputChannels * FramesPerBuffer);

                audioTrackBuilder.SetAudioAttributes(new Android.Media.AudioAttributes.Builder()
                    .SetUsage(AudioUsageKind.Media)
                    .SetContentType(AudioContentType.Music)
                    .Build());

                audioTrackBuilder.SetPerformanceMode(AudioTrackPerformanceMode.None);
                audioTrackBuilder.SetTransferMode(AudioTrackMode.Stream);

                // Building the audio tracks
                audioTrack = audioTrackBuilder.Build();

                buffersSent = 0;

                AudioTrack castAudioTrack = (AudioTrack)audioTrack;

                //castAudioTrack.SetNotificationMarkerPosition(2);
                //castAudioTrack.MarkerReached += MarkerReached;

                //int bufferTimerInterval = (int)(1000 * ((double)FramesPerBuffer / (double)CurrentFormat.SampleRate));

                //bufferTimer = new Timer(NewSoundBuffer, castAudioTrack, 0, bufferTimerInterval);

                //Setting both sounds to silent sound
                SilentSound = [new STFN.Audio.PortAudioVB.PortAudioBasedSoundPlayer.BufferHolder(NumberOfOutputChannels, FramesPerBuffer)];
                OutputSoundA = SilentSound;
                OutputSoundB = SilentSound;

                castAudioTrack.SetStartThresholdInFrames(2);
                //castAudioTrack.SetStartThresholdInFrames(FramesPerBuffer);
                //var x = castAudioTrack.StartThresholdInFrames;

                // Start playback
                castAudioTrack.Play();

                // Calls MarkerReached to initiate play loop
                // MarkerReached(null, null);

                // Starting loop on a new thread
                Thread newThread = new Thread(new ThreadStart(BufferLoop));
                newThread.Start();

            }

        }

        private void CheckWaveFormat(int? BitDepth, WaveFormat.WaveFormatEncodings? Encoding)
        {

            List<WaveFormat.WaveFormatEncodings> SupportedWaveFormatEncodings = new List<WaveFormat.WaveFormatEncodings> { WaveFormat.WaveFormatEncodings.PCM, WaveFormat.WaveFormatEncodings.IeeeFloatingPoints };
            List<int> SupportedSoundBitDepths = new List<int> { 16, 32 };

            if (Encoding != null)
            {
                if (SupportedWaveFormatEncodings.Contains(Encoding.Value) == false)
                {
                    throw new Exception("Unable to start the sound AndroidAudioTrackPlayer. Unsupported audio encoding.");
                }
            }
            if (BitDepth != null)
            {
                if (SupportedSoundBitDepths.Contains(BitDepth.Value) == false)
                {
                    throw new Exception("Unable to start the sound AndroidAudioTrackPlayer. Unsupported audio bit depth.");
                }
            }
        }


        public void ChangePlayerSettings(ref AudioApiSettings AudioApiSettings, int? SampleRate, int? BitDepth, WaveFormat.WaveFormatEncodings? Encoding, double? OverlapDuration, ref DuplexMixer Mixer, iSoundPlayer.SoundDirections? SoundDirection, bool ReOpenStream, bool ReStartStream, bool? ClippingIsActivated)
        {

            bool WasPlaying = false;

            if (audioTrack != null)
            {
                AudioTrack castAudioTrack = (AudioTrack)audioTrack;
                if (castAudioTrack.PlayState == PlayState.Playing)
                {

                    runBufferLoop = false;
                    Thread.Sleep(200);

                    castAudioTrack.Stop();
                    castAudioTrack.Release();
                    WasPlaying = true;
                }
            }

            if (SampleRate != null)
            {
                this.SampleRate = SampleRate.Value;
            }

            CheckWaveFormat(BitDepth, Encoding);

            //'Updating values
            //  If AudioApiSettings IsNot Nothing Then SetApiAndDevice(AudioApiSettings, True)

            if (OverlapDuration != null)
            {
                SetOverlapDuration(OverlapDuration.Value);
            }

            if (Mixer != null)
            {
                this.Mixer = Mixer;
            }

            // SoundDirection is ignored, since this class is only PlaybackOnly

            // ClippingIsActivated is ignored for now (This player should clip all data at Int32.Max and Min values)

            if (WasPlaying == true)
            {
                StartPlayer();
            }

        }

        public void CloseStream()
        {
            if (audioTrack != null)
            {
                AudioTrack castAudioTrack = (AudioTrack)audioTrack;
                castAudioTrack.Stop();
            }
        }

        public void Dispose()
        {
            if (audioTrack != null)
            {
                AudioTrack castAudioTrack = (AudioTrack)audioTrack;
                castAudioTrack.Stop();
                castAudioTrack.Release();
                castAudioTrack.Dispose();
                audioTrack = null;
            }

        }

        public void FadeOutPlayback()
        {
            //Doing fade out by swapping to SilentSound
            NewSound = SilentSound;
        }

        public Sound GetRecordedSound(bool ClearRecordingBuffer)
        {
            throw new NotImplementedException("The AndroidAudioTrackPlayer cannot record sound.");
        }

        [SupportedOSPlatform("Android31.0")]
        public bool SwapOutputSounds(ref Sound NewOutputSound, bool Record, bool AppendRecordedSound)
        {

            if (NewOutputSound == null)
            {
                return false;
            }

            if (NewOutputSound.WaveData.LongestChannelSampleCount == 0)
            {
                return false;
            }

            if (CurrentFormat == null)
            {
                CurrentFormat = NewOutputSound.WaveFormat;
                StartPlayer();
            }

            CheckWaveFormat(NewOutputSound.WaveFormat.BitDepth, NewOutputSound.WaveFormat.Encoding);

            if (NewOutputSound.WaveFormat.IsEqual(ref CurrentFormat, true, true, true, true) == false)
            {
                CurrentFormat = NewOutputSound.WaveFormat;
                StartPlayer();
            }

            if (IsPlaying == false)
            {
                throw new Exception("The AndroidAudioTrackPlayer is no longer running, and was unable to restart!");
            }

            double BitdepthScaling;
            switch (NewOutputSound.WaveFormat.Encoding)
            {
                case WaveFormat.WaveFormatEncodings.PCM:
                    switch (NewOutputSound.WaveFormat.BitDepth)
                    {
                        case 16:
                            // Dividing by short.MaxValue to get the range +/- unity
                            BitdepthScaling = 1 / short.MaxValue;
                            break;

                        default:
                            throw new NotImplementedException("Unsupported bit depth");
                            break;
                    }
                    break;

                case WaveFormat.WaveFormatEncodings.IeeeFloatingPoints:
                    switch (NewOutputSound.WaveFormat.BitDepth)
                    {
                        case 32:
                            // No scaling, already in 32 bit floats
                            BitdepthScaling = 1;
                            break;

                        default:
                            throw new NotImplementedException("Unsupported bit depth");
                            break;
                    }
                    break;

                default:
                    throw new NotImplementedException("Unsupported bit encoding");
                    break;
            }

            //'Setting NewSound to the NewOutputSound to indicate that the output sound should be swapped by the callback
            NewSound = STFN.Audio.PortAudioVB.PortAudioBasedSoundPlayer.CreateBufferHoldersOnNewThread(ref NewOutputSound, ref Mixer, ref FramesPerBuffer, ref NumberOfOutputChannels, BitdepthScaling);

            //playNewSound_IeeeFloatingPoints(ref NewOutputSound);

            return true;

        }

        private void BufferLoop()
        {
                       
            AudioTrack castAudioTrack = (AudioTrack)audioTrack;
            double checkTimesPerBuffer = 5;
            int bufferNeedCheckInterval = (int)(((double)1000 * ((double)FramesPerBuffer / (double)CurrentFormat.SampleRate))/ checkTimesPerBuffer );
            runBufferLoop = true;

            do
            {

                if (castAudioTrack != null)
                {
                    if (castAudioTrack.State == AudioTrackState.Initialized)
                    {

                        try
                        {
                            int buffersPlayed = (int)System.Math.Floor((double)castAudioTrack.PlaybackHeadPosition / (double)FramesPerBuffer);
                            int buffersAheadOfPlayback = 2;

                            if ((buffersSent - buffersAheadOfPlayback) < buffersPlayed)
                            {
                                NewSoundBuffer(castAudioTrack);
                            };
                        }
                        catch (Exception)
                        {
                            //throw;
                        }

                    }
                }

                Thread.Sleep(bufferNeedCheckInterval);

            } while (runBufferLoop == true);

        }


        private void NewSoundBuffer(object castAudioTrack_in)
        {

            //}

            ///// <summary>
            ///// This method is responsible for playing audio buffers. It needs to be triggered once when the AudioTrack is started. After that, it is triggered by the Marker position, which is updated to trigger in the beginning of the playing of the next buffer. 
            ///// </summary>
            //[SupportedOSPlatform("Android31.0")]
            //private void MarkerReached(object sender, AudioTrack.MarkerReachedEventArgs e)
            //{

            // Sending a buffer tick to the controller
            // Temporarily outcommented, until better solutions are fixed:
            // SendMessageToController(PlayBack.ISoundPlayerControl.MessagesFromSoundPlayer.NewBufferTick);

            // Declaring a spin lock taken variable
            bool spinLockTaken = false;

            bool playSilence = false;

            // Attempts to enter a spin lock to avoid multiple threads calling before complete
            callbackSpinLock.Enter(ref spinLockTaken);

            // Checking if the current sound should be swapped (if there is a new sound in NewSound)
            if (NewSound != null)
            {
                // Swapping sound
                switch (CurrentOutputSound)
                {
                    case OutputSounds.OutputSoundA:
                    case OutputSounds.FadingToA:
                        OutputSoundB = NewSound;
                        NewSound = null;
                        CurrentOutputSound = OutputSounds.FadingToB;
                        PositionB = 0;
                        break;

                    case OutputSounds.OutputSoundB:
                    case OutputSounds.FadingToB:
                        OutputSoundA = NewSound;
                        NewSound = null;
                        CurrentOutputSound = OutputSounds.FadingToA;
                        PositionA = 0;
                        break;
                }

                // Setting CrossFadeProgress to 0 since a new fade period has begun
                CrossFadeProgress = 0;

                // Raising event StartedSwappingOutputSounds
                StartedSwappingOutputSounds?.Invoke();
            }

            // Ignoring Checking current positions to see if an EndOfBufferAlert should be sent

            //Copying buffers 
            switch (CurrentOutputSound)
            {
                case OutputSounds.OutputSoundA:
                    if (PositionA >= OutputSoundA.Length)
                    {
                        playSilence = true;
                    }
                    else
                    {
                        PlaybackBuffer = OutputSoundA[PositionA].InterleavedSampleArray;
                        PositionA += 1;
                    }
                    break;

                case OutputSounds.OutputSoundB:

                    if (PositionB >= OutputSoundB.Length)
                    {
                        playSilence = true;
                    }
                    else
                    {
                        PlaybackBuffer = OutputSoundB[PositionB].InterleavedSampleArray;
                        PositionB += 1;
                    }
                    break;

                case OutputSounds.FadingToA:

                    if (PositionA < OutputSoundA.Length && PositionB < OutputSoundB.Length)
                    {
                        // Mixing sound A and B to the buffer
                        for (int j = 0; j < PlaybackBuffer.Length; j++)
                        {
                            PlaybackBuffer[j] = OutputSoundB[PositionB].InterleavedSampleArray[j] * OverlapFadeOutArray[CrossFadeProgress] + OutputSoundA[PositionA].InterleavedSampleArray[j] * OverlapFadeInArray[CrossFadeProgress];
                            CrossFadeProgress += 1;
                        }
                    }
                    else if (PositionA < OutputSoundA.Length && PositionB >= OutputSoundB.Length)
                    {
                        // Copying only sound A to the buffer
                        for (int j = 0; j < PlaybackBuffer.Length; j++)
                        {
                            PlaybackBuffer[j] = OutputSoundA[PositionA].InterleavedSampleArray[j] * OverlapFadeInArray[CrossFadeProgress];
                            CrossFadeProgress += 1;
                        }
                    }
                    else if (PositionA >= OutputSoundA.Length && PositionB < OutputSoundB.Length)
                    {
                        // Copying only sound B to the buffer
                        for (int j = 0; j < PlaybackBuffer.Length; j++)
                        {
                            PlaybackBuffer[j] = OutputSoundB[PositionB].InterleavedSampleArray[j] * OverlapFadeOutArray[CrossFadeProgress];
                            CrossFadeProgress += 1;
                        }
                    }
                    else
                    {
                        // End of both sounds: Copying silence
                        CrossFadeProgress = FadeArrayLength();
                        playSilence = true;
                    }

                    PositionA += 1;
                    PositionB += 1;

                    if (CrossFadeProgress >= FadeArrayLength() - 1)
                    {
                        CurrentOutputSound = OutputSounds.OutputSoundA;
                        CrossFadeProgress = 0;

                        // Raising event FinishedSwappingOutputSounds
                        FinishedSwappingOutputSounds?.Invoke();

                    }
                    break;

                case OutputSounds.FadingToB:

                    if (PositionA < OutputSoundA.Length && PositionB < OutputSoundB.Length)
                    {
                        // Mixing sound A and B to the buffer
                        for (int j = 0; j < PlaybackBuffer.Length; j++)
                        {
                            PlaybackBuffer[j] = OutputSoundB[PositionB].InterleavedSampleArray[j] * OverlapFadeInArray[CrossFadeProgress] + OutputSoundA[PositionA].InterleavedSampleArray[j] * OverlapFadeOutArray[CrossFadeProgress];
                            CrossFadeProgress += 1;
                        }
                    }
                    else if (PositionA < OutputSoundA.Length && PositionB >= OutputSoundB.Length)
                    {
                        // Copying only sound A to the buffer
                        for (int j = 0; j < PlaybackBuffer.Length; j++)
                        {
                            PlaybackBuffer[j] = OutputSoundA[PositionA].InterleavedSampleArray[j] * OverlapFadeOutArray[CrossFadeProgress];
                            CrossFadeProgress += 1;
                        }
                    }
                    else if (PositionA >= OutputSoundA.Length && PositionB < OutputSoundB.Length)
                    {
                        // Copying only sound B to the buffer
                        for (int j = 0; j < PlaybackBuffer.Length; j++)
                        {
                            PlaybackBuffer[j] = OutputSoundB[PositionB].InterleavedSampleArray[j] * OverlapFadeInArray[CrossFadeProgress];
                            CrossFadeProgress += 1;
                        }
                    }
                    else
                    {
                        // End of both sounds: Copying silence
                        CrossFadeProgress = FadeArrayLength();
                        playSilence = true;
                    }

                    PositionA += 1;
                    PositionB += 1;

                    if (CrossFadeProgress >= FadeArrayLength() - 1)
                    {
                        CurrentOutputSound = OutputSounds.OutputSoundB;
                        CrossFadeProgress = 0;

                        // Raising event FinishedSwappingOutputSounds
                        FinishedSwappingOutputSounds?.Invoke();
                    }
                    break;
            }

            AudioTrack castAudioTrack = (AudioTrack)castAudioTrack_in;

            if (castAudioTrack != null)
            {
                if (castAudioTrack.State == AudioTrackState.Initialized)
                {

                    //castAudioTrack.SetNotificationMarkerPosition(castAudioTrack.PlaybackHeadPosition + 2);
                    if (playSilence == false)
                    {
                        try
                        {

                            int retVal = castAudioTrack.Write(PlaybackBuffer, 0, PlaybackBuffer.Length, WriteMode.Blocking);
                            buffersSent += 1;

                            //Console.WriteLine(SilentBuffer.Length.ToString());
                            //Console.WriteLine(buffersSent + " " + retVal.ToString());

                            //if (retVal != PlaybackBuffer.Length)
                            //{

                            //    switch (retVal)
                            //    {
                            //        case -3: //AudioTrack.ErrorInvalidOperation
                            //            break;

                            //        case -2: //AudioTrack.ErrorBadValue
                            //            break;

                            //        case -6: //AudioTrack.ErrorDeadObject
                            //            break;

                            //        case -1: //AudioTrack.Error
                            //            break;

                            //        default:
                            //            break;
                            //    }
                            //}
                        }
                        catch (Exception)
                        {
                            //throw;
                        }
                    }
                    else
                    {
                        try
                        {

                            int retVal = castAudioTrack.Write(SilentBuffer, 0, SilentBuffer.Length, WriteMode.Blocking);
                            buffersSent += 1;

                            //Console.WriteLine(SilentBuffer.Length.ToString());
                            //Console.WriteLine(buffersSent + " " + retVal.ToString());

                            //if (retVal != SilentBuffer.Length)
                            //{

                            //    switch (retVal)
                            //    {
                            //        case -3: //AudioTrack.ErrorInvalidOperation
                            //            break;

                            //        case -2: //AudioTrack.ErrorBadValue
                            //            break;

                            //        case -6: //AudioTrack.ErrorDeadObject
                            //            break;

                            //        case -1: //AudioTrack.Error
                            //            break;

                            //        default:
                            //            break;
                            //    }
                            //}
                        }
                        catch (Exception)
                        {
                            //throw;
                        }
                    }
                }
                else
                {
                    //throw;
                }
            }
            else
            {
                //throw;
            }

            if (spinLockTaken) callbackSpinLock.Exit();

        }


        private int FadeArrayLength()
        {
            return NumberOfOutputChannels * _OverlapFrameCount;
        }


        private int _OverlapFrameCount;

        /// <summary>
        /// A value that holds the number of overlapping frames between two sounds. Setting this value automatically creates overlap fade arrays (OverlapFadeInArray and OverlapFadeOutArray).
        /// </summary>
        private int OverlapFrameCount
        {
            get
            {
                return _OverlapFrameCount;
            }
            set
            {
                try
                {
                    // Enforcing overlap fade length to be a multiple of FramesPerBuffer
                    _OverlapFrameCount = FramesPerBuffer * (int)Math.Ceiling((double)value / FramesPerBuffer);

                    int fadeArrayLength = NumberOfOutputChannels * (int)_OverlapFrameCount;

                    // Linear fading
                    // fade in array
                    OverlapFadeInArray = new float[fadeArrayLength];
                    for (int n = 0; n < _OverlapFrameCount; n++)
                    {
                        for (int c = 0; c < NumberOfOutputChannels; c++)
                        {
                            OverlapFadeInArray[n * NumberOfOutputChannels + c] = n / (_OverlapFrameCount - 1);
                        }
                    }

                    // fade out array
                    OverlapFadeOutArray = new float[fadeArrayLength];
                    for (int n = 0; n < _OverlapFrameCount; n++)
                    {
                        for (int c = 0; c < NumberOfOutputChannels; c++)
                        {
                            OverlapFadeOutArray[n * NumberOfOutputChannels + c] = 1 - (n / (_OverlapFrameCount - 1));
                        }
                    }

                    // Adjusting to equal power fades
                    if (equalPowerCrossFade)
                    {
                        for (int n = 0; n < fadeArrayLength; n++)
                        {
                            OverlapFadeInArray[n] = (float)Math.Sqrt(OverlapFadeInArray[n]);
                        }
                        for (int n = 0; n < fadeArrayLength; n++)
                        {
                            OverlapFadeOutArray[n] = (float)Math.Sqrt(OverlapFadeOutArray[n]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Messager.MsgBox(ex.ToString());
                }
            }
        }


    }




}







