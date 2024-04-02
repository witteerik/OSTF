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
//using Java.Lang;


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
        public AndroidAudioTrackPlayer(ref DuplexMixer Mixer, int bufferSize = 512)
        {
            this.bufferSize = bufferSize;
            this.Mixer = Mixer;

        }

        [SupportedOSPlatform("Android31.0")]
        bool StartPlayer()
        {

            if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {

                this.SampleRate = (int)CurrentFormat.SampleRate;
                SetOverlapDuration(0.5);
                NumberOfOutputChannels = Mixer.GetHighestOutputChannel();
                SilentBuffer = new float[FramesPerBuffer * NumberOfOutputChannels];
                PlaybackBuffer = new float[FramesPerBuffer * NumberOfOutputChannels];

                // Cretaing a ChannelIndexMask representing the output channels
                // Define a bit array containing boolean values for channels 1-32 ?
                // https://developer.android.com/reference/android/media/AudioFormat.Builder#setChannelIndexMask(int)

                List<bool> channelInclusionList = new List<bool>();
                for (int c = 0; c < Mixer.GetHighestOutputChannel(); c++)
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
                    .SetEncoding(Android.Media.Encoding.Pcm32bit)
                    .SetSampleRate((int)CurrentFormat.SampleRate)
                    .SetChannelIndexMask(ChannelIndexMask)
                    .Build());

                //audioTrackBuilder.SetAudioFormat(new AudioFormat.Builder()
                //    .SetEncoding(Android.Media.Encoding.Pcm32bit)
                //    .SetSampleRate((int)CurrentFormat.SampleRate)
                //    .SetChannelMask(ChannelOut.Mono)
                //    .Build());

                // Set buffer size here
                audioTrackBuilder.SetBufferSizeInBytes(NumberOfOutputChannels * FramesPerBuffer * 4);

                audioTrackBuilder.SetAudioAttributes(new Android.Media.AudioAttributes.Builder()
                    .SetUsage(AudioUsageKind.Media)
                    .SetContentType(AudioContentType.Music)
                    .Build());

                audioTrackBuilder.SetPerformanceMode(AudioTrackPerformanceMode.LowLatency);

                // Building the audio tracks
                audioTrack = audioTrackBuilder.Build();

                AudioTrack castAudioTrack = (AudioTrack)audioTrack;

                castAudioTrack.SetNotificationMarkerPosition(4);
                castAudioTrack.MarkerReached += MarkerReached;

                //Setting both sounds to silent sound
                SilentSound = [new STFN.Audio.PortAudioVB.PortAudioBasedSoundPlayer.BufferHolder(NumberOfOutputChannels, FramesPerBuffer)];
                OutputSoundA = SilentSound;
                OutputSoundB = SilentSound;

                // Start playback
                castAudioTrack.Play();

                // Writes the first buffer (now empty)
                castAudioTrack.Write(SilentBuffer, 0, SilentBuffer.Length, WriteMode.NonBlocking);

                if (castAudioTrack.PlayState != PlayState.Playing)
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

        private void CheckWaveFormat(int? BitDepth, WaveFormat.WaveFormatEncodings? Encoding) {

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
            //throw new NotImplementedException();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public void FadeOutPlayback()
        {
            //throw new NotImplementedException();
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
                if (StartPlayer() == false)
                {
                    throw new Exception("Unable to start the sound AndroidAudioTrackPlayer");
                };
            }

            CheckWaveFormat(NewOutputSound.WaveFormat.BitDepth, NewOutputSound.WaveFormat.Encoding);

            if (NewOutputSound.WaveFormat.IsEqual(ref CurrentFormat, true, true, true, true) == false)
            {
                CurrentFormat = NewOutputSound.WaveFormat;
                if (StartPlayer() == false)
                {
                    throw new Exception("Unable to start the sound AndroidAudioTrackPlayer");
                };
            }

            return playNewSound_IeeeFloatingPoints(ref NewOutputSound);

        }



        [SupportedOSPlatform("Android31.0")]
        bool playNewSound_IeeeFloatingPoints(ref Sound NewOutputSound)
        {

            //'Setting NewSound to the NewOutputSound to indicate that the output sound should be swapped by the callback
            NewSound = STFN.Audio.PortAudioVB.PortAudioBasedSoundPlayer.CreateBufferHoldersOnNewThread(ref NewOutputSound,ref Mixer,ref FramesPerBuffer, ref NumberOfOutputChannels);

            if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {

                AudioTrack castAudioTrack = (AudioTrack)audioTrack;

                //castAudioTrack.SetVolume();

                //AudioTrack.StreamEventCallback StreamEventCallback()
                //castAudioTrack.AddOnRoutingChangedListener()

                if (castAudioTrack.PlayState != PlayState.Playing)
                {
                    throw new System.Exception("Sound player not running anymore!");
                }

                // This should be async ? or on another thread

                if (NewOutputSound.WaveFormat.IsEqual(ref CurrentFormat, true, true, true, true) == false)
                {
                    throw new System.Exception("The format of the new sound does not agree with the format for which the sound player was instantiated.");
                }


                // Getting the sound samples and writing them to the player
                int channel = 1;

                // Getting a gain factor. This could be the channel dependent speaker calibration value. It could also be multiplied by another "online" gain factor applied to the sound.
                double GainFactor = 1;

                // Limiting BitdepthScaling to Int32.MaxValue // May be unecessary
                double BitdepthScaling;
                switch (NewOutputSound.WaveFormat.BitDepth)
                {
                    case 16:
                        // 16 bit sample data should be scale to 32 bit integer range
                        BitdepthScaling = System.Math.Min(GainFactor * Int32.MaxValue / short.MaxValue, Int32.MaxValue);
                        break;

                    case 32:
                        // +/1 unity range should be scaled to 32 bit integer range
                        BitdepthScaling = System.Math.Min(GainFactor * Int32.MaxValue, Int32.MaxValue);
                        break;

                    default:
                        throw new NotImplementedException("Unsupported bit depth");
                        break;
                }

                // Getting the channel sample array
                float[] sample = NewOutputSound.WaveData.get_SampleData(channel);

                // Getting the number of samples
                int numSamples = sample.Length;

                // Convert samples to byte array
                byte[] generatedSnd = new byte[4 * numSamples];
                int idx = 0;
                foreach (float sampleValue in sample)
                {
                    byte[] val_bytes = BitConverter.GetBytes((Int32)(sampleValue * BitdepthScaling));
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

            }
            else
            {
                return false;
            }

        }

        //public STFN.Audio.PortAudioVB.PortAudioBasedSoundPlayer.BufferHolder[] CreateBufferHoldersOnNewThread(ref Sound inputSound, int buffersOnMainThread = 10)
        //{
        //    int bufferCount = (int)(inputSound.WaveData.LongestChannelSampleCount / FramesPerBuffer) + 1;

        //    STFN.Audio.PortAudioVB.PortAudioBasedSoundPlayer.BufferHolder[] output = new STFN.Audio.PortAudioVB.PortAudioBasedSoundPlayer.BufferHolder[bufferCount];

        //    // Initializing the BufferHolders
        //    for (int b = 0; b < output.Length; b++)
        //    {
        //        output[b] = new STFN.Audio.PortAudioVB.PortAudioBasedSoundPlayer.BufferHolder(NumberOfOutputChannels, FramesPerBuffer);
        //    }

        //    // Creating the BuffersOnMainThread first buffers
        //    // Limiting the number of main thread buffers if the sound is very short
        //    if (output.Length - 1 < buffersOnMainThread)
        //    {
        //        buffersOnMainThread = Math.Max(0, output.Length - 1);
        //    }

        //    int currentChannelInterleavedPosition = 0;
        //    foreach (var outputRouting in Mixer.OutputRouting)
        //    {
        //        if (outputRouting.Value == 0) continue;

        //        if (outputRouting.Value > inputSound.WaveFormat.Channels) continue;

        //        // Skipping if channel contains no data
        //        var ChannelArray = inputSound.WaveData.get_SampleData(outputRouting.Value);
        //        if (ChannelArray.Length == 0) continue;

        //        // Calculates the calibration gain
        //        double calibrationGain = Mixer.get_CalibrationGain(outputRouting.Key);
        //        float calibrationGainFactor = (float)Math.Pow(10, calibrationGain / 20);

        //        currentChannelInterleavedPosition = outputRouting.Key - 1;

        //        // Going through buffer by buffer
        //        for (int bufferIndex = 0; bufferIndex < buffersOnMainThread; bufferIndex++)
        //        {
        //            // Setting start sample and time
        //            output[bufferIndex].StartSample = bufferIndex * FramesPerBuffer;

        //            // Shuffling samples from the input sound to the interleaved array
        //            int currentWriteSampleIndex = 0;
        //            for (int sample = bufferIndex * FramesPerBuffer; sample < (bufferIndex + 1) * FramesPerBuffer; sample++)
        //            {
        //                int x = currentWriteSampleIndex * NumberOfOutputChannels + currentChannelInterleavedPosition;

        //                output[bufferIndex].InterleavedSampleArray[currentWriteSampleIndex * NumberOfOutputChannels + currentChannelInterleavedPosition] = ChannelArray[sample] * calibrationGainFactor;
        //                currentWriteSampleIndex++;
        //            }
        //        }
        //    }

        //    // Fixes the rest of the buffers on a new thread, allowing the new sound to start playing
        //    STFN.Audio.PortAudioVB.PortAudioBasedSoundPlayer.BufferCreaterOnNewThread threadWork = new STFN.Audio.PortAudioVB.PortAudioBasedSoundPlayer.BufferCreaterOnNewThread(ref inputSound, ref output, buffersOnMainThread,
        //                                                                        NumberOfOutputChannels, ref Mixer, (uint)FramesPerBuffer);

        //    return output;
        //}


        private System.Threading.SpinLock callbackSpinLock = new System.Threading.SpinLock();
        private bool PlaybackIsActive = false;

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

        private void MarkerReached(object sender, AudioTrack.MarkerReachedEventArgs e)
        {
            // To be replaced
            //IntPtr input;
            //IntPtr output;
            //uint frameCount;
            //PortAudio.PaStreamCallbackTimeInfo timeInfo;
            //PortAudio.PaStreamCallbackFlags statusFlags;
            //IntPtr userData;

            float[] OutputBuffer = new float[PlaybackBuffer.Length];

            // Sending a buffer tick to the controller
            // Temporarily outcommented, until better solutions are fixed:
            // SendMessageToController(PlayBack.ISoundPlayerControl.MessagesFromSoundPlayer.NewBufferTick);

            // Declaring a spin lock taken variable
            bool spinLockTaken = false;

            try
            {
                // Attempts to enter a spin lock to avoid multiple threads calling before complete
                callbackSpinLock.Enter(ref spinLockTaken);

                // OUTPUT SOUND
                if (PlaybackIsActive == true)
                {
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
                                OutputBuffer = SilentBuffer;
                                //Marshal.Copy(SilentBuffer, 0, output, SilentBuffer.Length);
                            }
                            else
                            {
                                PlaybackBuffer = OutputSoundA[PositionA].InterleavedSampleArray;

                                //Copying the playback buffer to unmanaged memory
                                OutputBuffer = PlaybackBuffer; //Marshal.Copy(PlaybackBuffer, 0, output, FramesPerBuffer * NumberOfOutputChannels);
                                PositionA += 1;
                            }

                            break;

                        case OutputSounds.OutputSoundB:

                            if (PositionB >= OutputSoundB.Length)
                            {
                                OutputBuffer = SilentBuffer; //Marshal.Copy(SilentBuffer, 0, output, SilentBuffer.Length);
                            }
                            else
                            {
                                PlaybackBuffer = OutputSoundB[PositionB].InterleavedSampleArray;

                                //Copying the playback buffer to unmanaged memory
                                OutputBuffer = PlaybackBuffer; //Marshal.Copy(PlaybackBuffer, 0, output, FramesPerBuffer * NumberOfOutputChannels);
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

                                // Copying the playback buffer to unmanaged memory
                                OutputBuffer = PlaybackBuffer; //Marshal.Copy(PlaybackBuffer, 0, output, FramesPerBuffer * NumberOfOutputChannels);
                            }
                            else if (PositionA < OutputSoundA.Length && PositionB >= OutputSoundB.Length)
                            {
                                // Copying only sound A to the buffer
                                for (int j = 0; j < PlaybackBuffer.Length; j++)
                                {
                                    PlaybackBuffer[j] = OutputSoundA[PositionA].InterleavedSampleArray[j] * OverlapFadeInArray[CrossFadeProgress];
                                    CrossFadeProgress += 1;
                                }

                                // Copying the playback buffer to unmanaged memory
                                OutputBuffer = PlaybackBuffer; //Marshal.Copy(PlaybackBuffer, 0, output, FramesPerBuffer * NumberOfOutputChannels);
                            }
                            else if (PositionA >= OutputSoundA.Length && PositionB < OutputSoundB.Length)
                            {
                                // Copying only sound B to the buffer
                                for (int j = 0; j < PlaybackBuffer.Length; j++)
                                {
                                    PlaybackBuffer[j] = OutputSoundB[PositionB].InterleavedSampleArray[j] * OverlapFadeOutArray[CrossFadeProgress];
                                    CrossFadeProgress += 1;
                                }

                                // Copying the playback buffer to unmanaged memory
                                OutputBuffer = PlaybackBuffer; //Marshal.Copy(PlaybackBuffer, 0, output, FramesPerBuffer * NumberOfOutputChannels);
                            }
                            else
                            {
                                // End of both sounds: Copying silence
                                CrossFadeProgress = FadeArrayLength();
                                OutputBuffer = SilentBuffer; //Marshal.Copy(SilentBuffer, 0, output, SilentBuffer.Length);

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

                                // Copying the playback buffer to unmanaged memory
                                OutputBuffer = PlaybackBuffer; //Marshal.Copy(PlaybackBuffer, 0, output, FramesPerBuffer * NumberOfOutputChannels);
                            }
                            else if (PositionA < OutputSoundA.Length && PositionB >= OutputSoundB.Length)
                            {
                                // Copying only sound A to the buffer
                                for (int j = 0; j < PlaybackBuffer.Length; j++)
                                {
                                    PlaybackBuffer[j] = OutputSoundA[PositionA].InterleavedSampleArray[j] * OverlapFadeOutArray[CrossFadeProgress];
                                    CrossFadeProgress += 1;
                                }

                                // Copying the playback buffer to unmanaged memory
                                OutputBuffer = PlaybackBuffer; //Marshal.Copy(PlaybackBuffer, 0, output, FramesPerBuffer * NumberOfOutputChannels);
                            }
                            else if (PositionA >= OutputSoundA.Length && PositionB < OutputSoundB.Length)
                            {
                                // Copying only sound B to the buffer
                                for (int j = 0; j < PlaybackBuffer.Length; j++)
                                {
                                    PlaybackBuffer[j] = OutputSoundB[PositionB].InterleavedSampleArray[j] * OverlapFadeInArray[CrossFadeProgress];
                                    CrossFadeProgress += 1;
                                }

                                // Copying the playback buffer to unmanaged memory
                                OutputBuffer = PlaybackBuffer; //Marshal.Copy(PlaybackBuffer, 0, output, FramesPerBuffer * NumberOfOutputChannels);
                            }
                            else
                            {
                                // End of both sounds: Copying silence
                                CrossFadeProgress = FadeArrayLength();
                                OutputBuffer = SilentBuffer; //Marshal.Copy(SilentBuffer, 0, output, SilentBuffer.Length);

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

                    //if (raisePlaybackBufferTickEvents == true)
                    //{
                    //    PlaybackBufferTick?.Invoke();
                    //}

                }

                // Write the generated audio data to the AudioTrack
                byte[] soundByteArray = new byte[4 * OutputBuffer.Length];
                int idx = 0;
                for (int i = 0; i < OutputBuffer.Length; i++)
                {
                    // Copying to byte
                    Int32 val = (Int32)OutputBuffer[i];
                    byte[] val_bytes = BitConverter.GetBytes(val);
                    val_bytes.CopyTo(soundByteArray, idx);
                    idx += 4;

                }

                AudioTrack castAudioTrack = (AudioTrack)audioTrack;

                castAudioTrack.Write(soundByteArray, 0, soundByteArray.Length, WriteMode.Blocking);

                //currentBufferIndex += 1;

                castAudioTrack.SetNotificationMarkerPosition(castAudioTrack.PlaybackHeadPosition + FramesPerBuffer / 2);

            }
            catch (Exception ex)
            {

                // Playing silence if an exception occurred
                AudioTrack castAudioTrack = (AudioTrack)audioTrack;
                castAudioTrack.Write(SilentBuffer, 0, SilentBuffer.Length, WriteMode.NonBlocking);

                //Marshal.Copy(SilentBuffer, 0, output, FramesPerBuffer * NumberOfOutputChannels);
                //audioTrack.Write(generatedSnd, 0, generatedSnd.Length, WriteMode.Blocking);

            }
            finally
            {
                // Releases any spinlock
                if (spinLockTaken) callbackSpinLock.Exit();
            }
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







