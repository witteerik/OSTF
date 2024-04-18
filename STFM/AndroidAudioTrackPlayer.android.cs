using Android.Media;
using STFN.Audio;
using STFN.Audio.Formats;
using STFN.Audio.SoundPlayers;
using STFN.Audio.SoundScene;
using System.Runtime.Versioning;
using STFN;
using Android.Content;
using Android.Bluetooth;

namespace STFM
{

    public class AndroidAudioTrackPlayer : STFN.Audio.SoundPlayers.iSoundPlayer
    {

        private AndroidAudioTrackPlayerSettings AudioSettings = null;

        private DuplexMixer Mixer;
        public DuplexMixer GetMixer()
        {
            return Mixer;
        }


        //Se documentation for AudioTrack at: https://developer.android.com/reference/android/media/AudioTrack

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
        public event iSoundPlayer.FatalPlayerErrorEventHandler FatalPlayerError;

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

        private void SetOverlapDuration(double Duration)
        {
            //Enforcing at least one sample overlap
            OverlapFrameCount = (int)Math.Max(1, (double)SampleRate * Duration);
        }

        public double GetOverlapDuration()
        {
            return _OverlapFrameCount / SampleRate;
        }

        STFN.Audio.Formats.WaveFormat CurrentFormat = null;


        Object audioTrack = null; // This is declared as an Object instead of AudioTrack since it will otherwise register an error in the Visual Studio editor.

        private System.Threading.SpinLock callbackSpinLock = new System.Threading.SpinLock();
        private System.Threading.SpinLock audioCheckSpinLock = new System.Threading.SpinLock();

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
        int NumberOfOutputChannels; // This corresponds to the number higest numbered physical output channel on the selected device.
        int SampleRate;

        volatile bool runBufferLoop = false;
        int buffersSent = 0;

        bool runAudioCheckLoop = false;

        volatile bool runTalkBackLoop = true;

        [SupportedOSPlatform("Android31.0")]
        public AndroidAudioTrackPlayer(ref AndroidAudioTrackPlayerSettings AudioSettings, ref DuplexMixer Mixer)
        {
            this.AudioSettings = AudioSettings;
            this.Mixer = Mixer;
        }


        [SupportedOSPlatform("Android31.0")]
        void StartPlayer()
        {

            if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {

                this.SampleRate = (int)CurrentFormat.SampleRate;
                NumberOfOutputChannels = Mixer.GetHighestOutputChannel();

                SetOverlapDuration(0.05); // This value is default, but should be set by each user of the player by a call to ChangePlayerSettings
                SilentBuffer = new float[AudioSettings.FramesPerBuffer * NumberOfOutputChannels];
                PlaybackBuffer = new float[AudioSettings.FramesPerBuffer * NumberOfOutputChannels];

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

                audioTrackBuilder.SetBufferSizeInBytes(NumberOfOutputChannels * AudioSettings.FramesPerBuffer);

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

                //Setting both sounds to silent sound
                SilentSound = [new STFN.Audio.PortAudioVB.PortAudioBasedSoundPlayer.BufferHolder(NumberOfOutputChannels, AudioSettings.FramesPerBuffer)];
                OutputSoundA = SilentSound;
                OutputSoundB = SilentSound;

                castAudioTrack.SetStartThresholdInFrames(2);

                // Start the AudioTrack
                castAudioTrack.Play();

                // Sets the output device
                CheckAndSetRequiredMainOutputDevice();

                // Starting the loop that supplied samples tothe AudioTrack on a new thread
                Thread newThread = new Thread(new ThreadStart(BufferLoop));
                newThread.Start();

                // Staring the loop that ensures correct player settings
                Thread newThread2 = new Thread(new ThreadStart(AudioSettingsCheckLoop));
                newThread2.Start();

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

        [SupportedOSPlatform("Android31.0")]
        public void ChangePlayerSettings(AudioSettings AudioApiSettings, int? SampleRate, int? BitDepth, WaveFormat.WaveFormatEncodings? Encoding, double? OverlapDuration, DuplexMixer Mixer, iSoundPlayer.SoundDirections? SoundDirection, bool ReOpenStream, bool ReStartStream, bool? ClippingIsActivated)
        {

            // Inactivating any active talkback 
            runTalkBackLoop = false;

            bool WasPlaying = false;

            if (audioTrack != null)
            {
                AudioTrack castAudioTrack = (AudioTrack)audioTrack;
                if (castAudioTrack.PlayState == PlayState.Playing)
                {

                    runBufferLoop = false;
                    runAudioCheckLoop = false;
                    Thread.Sleep(200);

                    castAudioTrack.Stop();
                    castAudioTrack.Release();
                    WasPlaying = true;
                }
            }

            if (AudioApiSettings != null)
            {
                this.AudioSettings = (AndroidAudioTrackPlayerSettings)AudioApiSettings;
            }

            if (SampleRate != null)
            {
                this.SampleRate = SampleRate.Value;
            }

            CheckWaveFormat(BitDepth, Encoding);

            //'Updating values
            //  If PortAudioApiSettings IsNot Nothing Then SetApiAndDevice(PortAudioApiSettings, True)

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
            runBufferLoop = false;
            runAudioCheckLoop = false;

            if (audioTrack != null)
            {
                AudioTrack castAudioTrack = (AudioTrack)audioTrack;
                castAudioTrack.Stop();
            }
        }

        public void Dispose()
        {

            runBufferLoop = false;
            runAudioCheckLoop = false;

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
            NewSound = STFN.Audio.PortAudioVB.PortAudioBasedSoundPlayer.CreateBufferHoldersOnNewThread(ref NewOutputSound, ref Mixer, AudioSettings.FramesPerBuffer, ref NumberOfOutputChannels, BitdepthScaling);

            return true;

        }

        private void BufferLoop()
        {

            AudioTrack castAudioTrack = (AudioTrack)audioTrack;
            int localFramesPerBuffer = AudioSettings.FramesPerBuffer;
            double checkTimesPerBuffer = 5;
            int bufferNeedCheckInterval = (int)(((double)1000 * ((double)localFramesPerBuffer / (double)CurrentFormat.SampleRate)) / checkTimesPerBuffer);
            runBufferLoop = true;

            do
            {

                if (castAudioTrack != null)
                {
                    if (castAudioTrack.State == AudioTrackState.Initialized)
                    {
                        try
                        {
                            int buffersPlayed = (int)System.Math.Floor((double)castAudioTrack.PlaybackHeadPosition / (double)localFramesPerBuffer);
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
                    else
                    {
                        // Stops the loop if castAudioTrack no longer refers to any instance, or is in a non-initialized state
                        if (castAudioTrack.State == AudioTrackState.Initialized)
                        {
                            runBufferLoop = false;
                        }
                    }
                }
                else
                {
                    // Stops the loop if castAudioTrack no longer refers to any instance, or is in a non-initialized state
                    runBufferLoop = false;
                }
                Thread.Sleep(bufferNeedCheckInterval);
            } while (runBufferLoop == true);
        }

        /// <summary>
        /// The current method starts a loop
        /// </summary>
        [SupportedOSPlatform("Android31.0")]
        private void AudioSettingsCheckLoop()
        {

            AudioTrack castAudioTrack = (AudioTrack)audioTrack;
            int checkInterval = 500;
            runAudioCheckLoop = true;

            do
            {
                if (castAudioTrack != null)
                {
                    if (castAudioTrack.State == AudioTrackState.Initialized)
                    {
                        if (CheckAudioSettings(AudioSettings.SelectedOutputDeviceName, AudioSettings.AllowDefaultOutputDevice.Value, Mixer.HostVolumeOutputLevel) == false)
                        {
                            // Playback should stop immediately and 
                            runBufferLoop = false;
                            runAudioCheckLoop = false;
                            castAudioTrack.Stop();
                            castAudioTrack.Release();

                            // Raising event FatalPlayerError
                            FatalPlayerError?.Invoke();
                        }
                    }
                }

                Thread.Sleep(checkInterval);

            } while (runAudioCheckLoop == true);

        }

        [SupportedOSPlatform("Android31.0")]
        public bool CheckAudioSettings(string IntendedOutputDeviceName, bool AllowAnyOutputDevice, int IntendedVolumePercentage)
        {

            // This method checks to ensure that the intended output device is selected, and that the intended android media volume is set as intended, and that all other volume types are set to zero volume.
            // If the current sound setting are not as intended, an attempt is made to to correct them. If not possible to correct the settings, false is returned, otherwise (if all is fine) true is returned.
            // The method should be called regularly (twice a second?) on a background thread, administered by the current SpeechTest.

            // Declaring a spin lock taken variable
            bool spinLockTaken = false;

            // Attempts to enter a spin lock to avoid multiple threads calling before complete
            audioCheckSpinLock.Enter(ref spinLockTaken);

            try
            {

                AudioTrack castAudioTrack = (AudioTrack)audioTrack;

                TrackStatus trackStatus = castAudioTrack.SetVolume((float)1); // This is a linear gain. The AudioTrack .NET implementation seem to lack a method for retrieving the maxVolume (why?). It seems to be set to unity by default.
                //Nontheless, we set it to unity to ensure a gain of zero dB. (N.B. This volume is not the same as the one set for Audiomanager SetStreamVolume, see https://developer.android.com/reference/android/media/AudioTrack#setVolume(float))

                // Checks that the player is alive
                if (castAudioTrack == null)
                {
                    // Attempting to start the device
                    StartPlayer();

                    // Try referencing the new player instance
                    castAudioTrack = (AudioTrack)audioTrack;

                    // Checking if start was possible
                    if (castAudioTrack == null)
                    {
                        // The player could not be started
                        if (spinLockTaken) audioCheckSpinLock.Exit();
                        return false;
                    }
                }

                if (castAudioTrack.PlayState != PlayState.Playing)
                {
                    // Attempting to start the device
                    StartPlayer();

                    // Try referencing the new player instance
                    castAudioTrack = (AudioTrack)audioTrack;

                    // Checking if start was possible
                    if (castAudioTrack == null)
                    {
                        // The player could not be started
                        if (spinLockTaken) audioCheckSpinLock.Exit();
                        return false;
                    }
                }

                if (castAudioTrack.PlayState != PlayState.Playing)
                {
                    // The player still does not play
                    if (spinLockTaken) audioCheckSpinLock.Exit();
                    return false;
                }

                AudioManager audioManager = Android.App.Application.Context.GetSystemService(Context.AudioService) as Android.Media.AudioManager;

                //Checks that the main AudioTrack has the correct output device set
                CheckAndSetRequiredMainOutputDevice(audioManager);

                //Checks that the talkback AudioTrack has the correct output device set
                CheckTalkBackAudioDevice(audioManager);


                // Checks that the intended volume is set
                int? MaxVol = audioManager?.GetStreamMaxVolume(Android.Media.Stream.Music);
                int? MinVol = audioManager?.GetStreamMinVolume(Android.Media.Stream.Music);
                int? currentVolume = audioManager?.GetStreamVolume(Android.Media.Stream.Music);
                //audioManager?.GetStreamVolumeDb(Android.Media.Stream.Music);
                int volumeRange = MaxVol.Value - MinVol.Value;
                int indendedVolume = (int)Math.Clamp((double)((double)IntendedVolumePercentage / (double)100) * (double)volumeRange, (double)MinVol.Value, (double)MaxVol.Value);

                if (indendedVolume != currentVolume)
                {
                    audioManager?.SetStreamVolume(Android.Media.Stream.Music, indendedVolume, VolumeNotificationFlags.RemoveSoundAndVibrate);

                    // Checking that the correct volume was also set
                    currentVolume = audioManager?.GetStreamVolume(Android.Media.Stream.Music);
                    indendedVolume = (int)Math.Clamp((double)((double)IntendedVolumePercentage / (double)100) * (double)volumeRange, (double)MinVol.Value, (double)MaxVol.Value);

                    if (indendedVolume != currentVolume)
                    {
                        // The volume is still incorrect
                        if (spinLockTaken) audioCheckSpinLock.Exit();
                        return false;
                    }
                }

                List<Android.Media.Stream> streamTypesToSilence = new List<Android.Media.Stream> {
                    //Android.Media.Stream.NotificationDefault,
                    Android.Media.Stream.VoiceCall,
                    Android.Media.Stream.System,
                    Android.Media.Stream.Ring,
                    Android.Media.Stream.Alarm,
                    Android.Media.Stream.Notification,
                    Android.Media.Stream.Dtmf,
                    //Android.Media.Stream.Accessibility  // I'm not sure what permission has to be requested to change this Accessibility volume. Leaving it for now.
                };

                for (int i = 0; i < streamTypesToSilence.Count; i++)
                {
                    Android.Media.Stream currentStreamToToSilence = streamTypesToSilence[i];

                    int? TargetMinVol = audioManager?.GetStreamMinVolume(currentStreamToToSilence);
                    int? currentStreamVolume = audioManager?.GetStreamVolume(currentStreamToToSilence);
                    if (currentStreamVolume.HasValue)
                    {
                        if (currentStreamVolume != TargetMinVol)
                        {
                            audioManager?.SetStreamVolume(currentStreamToToSilence, TargetMinVol.Value, VolumeNotificationFlags.RemoveSoundAndVibrate);
                        }
                        // Checking that the volume changed
                        currentStreamVolume = audioManager?.GetStreamVolume(currentStreamToToSilence);
                        if (currentStreamVolume != TargetMinVol.Value)
                        {
                            if (spinLockTaken) audioCheckSpinLock.Exit();
                            return false;
                        }
                    }
                }

                // Everything should be fine
                if (spinLockTaken) audioCheckSpinLock.Exit();
                return true;

            }
            catch (Exception ex)
            {
                Messager.MsgBox("The following error has occured in the CheckAudioSettings method!\n\n" + ex.ToString());
                if (spinLockTaken) audioCheckSpinLock.Exit();
                return false;
            }

        }

        private void NewSoundBuffer(object castAudioTrack_in)
        {

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
                    _OverlapFrameCount = AudioSettings.FramesPerBuffer * (int)Math.Ceiling((double)value / AudioSettings.FramesPerBuffer);

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


        // Some helper functions

        /// <summary>
        /// Checks if an output or input sound device with the ProductName of DeviceProductName exist on the system. 
        /// </summary>
        /// <param name="DeviceProductName"></param>
        /// <returns>Returns true if output exists on the system, or false if not.</returns>
        [SupportedOSPlatform("Android31.0")]
        public static bool CheckIfDeviceExists(string DeviceProductName, bool IsOutput)
        {
            try
            {
                var audioManager = Android.App.Application.Context.GetSystemService(Context.AudioService) as Android.Media.AudioManager;
                AudioDeviceInfo[] devices;
                if (IsOutput == true)
                {
                    devices = audioManager.GetDevices(GetDevicesTargets.Outputs);
                }
                else
                {
                    devices = audioManager.GetDevices(GetDevicesTargets.Inputs);
                }

                foreach (var device in devices)
                {
                    if (device.ProductName != null)
                    {
                        string ProductName = device.ProductName;
                        if (IsOutput == true)
                        {
                            if (ProductName == DeviceProductName)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            string deviceType = device.Type.ToString();
                            string evaluationString = ProductName + "+" + deviceType;
                            if (evaluationString == DeviceProductName)
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Messager.MsgBox(ex.ToString());
                //throw;
                return false;
            }
        }

        /// <summary>
        /// Checks if the IntendedDevice exist on the system and returns the highest output or input channel count that the indicated device supports, or minus 1 if the device does not exist. 
        /// </summary>
        /// <param name="IntendedDevice"></param>
        /// <param name="IsOutput">Set to true for output channels or false for input channels.</param>
        /// <returns>Returns the highest number of channels or -1 if no device exists.</returns>
        [SupportedOSPlatform("Android31.0")]
        public static int GetNumberChannelsOnDevice(string IntendedDevice, bool IsOutput)
        {
            try
            {
                var audioManager = Android.App.Application.Context.GetSystemService(Context.AudioService) as Android.Media.AudioManager;

                GetDevicesTargets devicesTargets = GetDevicesTargets.Inputs;
                if (IsOutput == true)
                {
                    devicesTargets = GetDevicesTargets.Outputs;
                }

                var devices = audioManager.GetDevices(devicesTargets);

                foreach (var device in devices)
                {
                    if (device.ProductName != null)
                    {
                        if (IsOutput == true)
                        {
                            if (device.ProductName == IntendedDevice)
                            {
                                int[] channelCounts = device.GetChannelCounts();
                                List<string> ChannelCountList = new List<string>();
                                foreach (int c in channelCounts)
                                {
                                    ChannelCountList.Add(c.ToString());
                                }
                                return channelCounts.Max();
                            }
                        }
                        else
                        {
                            if (device.ProductName + "+" + device.Type.ToString() == IntendedDevice)
                            {
                                int[] channelCounts = device.GetChannelCounts();
                                List<string> ChannelCountList = new List<string>();
                                foreach (int c in channelCounts)
                                {
                                    ChannelCountList.Add(c.ToString());
                                }
                                return channelCounts.Max();
                            }
                        }
                    }
                }
                return -1;
            }
            catch (Exception ex)
            {
                Messager.MsgBox(ex.ToString());
                //throw;
                return -1;
            }
        }


        /// <summary>
        /// Sets the output device to the first output device with a ProductName of DeviceProductName. 
        /// </summary>
        /// <returns>Returns true if the intended device was set, otherwise false.</returns>
        [SupportedOSPlatform("Android31.0")]
        public bool CheckAndSetRequiredMainOutputDevice(AudioManager audioManager = null)
        {

            try
            {

                AudioTrack castAudioTrack = (AudioTrack)audioTrack;

                // Trying to get the currently selected output device 
                if (audioManager == null) { audioManager = Android.App.Application.Context.GetSystemService(Context.AudioService) as Android.Media.AudioManager; }
                var devices = audioManager.GetDevices(GetDevicesTargets.Outputs);
                AudioDeviceInfo CurrentlySelectedOutputDevice = castAudioTrack.RoutedDevice;

                if (CurrentlySelectedOutputDevice != null)
                {
                    if (AudioSettings.SelectedOutputDeviceName == CurrentlySelectedOutputDevice.ProductName)
                    {
                        return true;
                    }
                }

                // Trying to set the intended output device.
                foreach (var device in devices)
                {
                    if (device.ProductName != null)
                    {
                        string ProductName = device.ProductName;
                        if (ProductName == AudioSettings.SelectedOutputDeviceName)
                        {
                            if (castAudioTrack.SetPreferredDevice(device) == true)
                            {
                                return true;
                            };
                        }
                    }
                }

                if (AudioSettings.AllowDefaultOutputDevice.Value == false)
                {
                    return false;
                }
                else
                {
                    // Leaves the default device (which should have been selected upon audio track creation
                    return true;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [SupportedOSPlatform("Android31.0")]
        public string GetAvaliableOutputDeviceNames()
        {

            var audioManager = Android.App.Application.Context.GetSystemService(Context.AudioService) as Android.Media.AudioManager;
            var devices = audioManager.GetDevices(GetDevicesTargets.Outputs);

            List<string> DeviceList = new List<string>();

            foreach (var device in devices)
            {
                string ProductName = device.ProductName.ToString();
                DeviceList.Add(ProductName);
            }

            return string.Join("\n", DeviceList);
        }

        [SupportedOSPlatform("Android31.0")]
        public static string GetAvaliableDeviceInformation()
        {
            var audioManager = Android.App.Application.Context.GetSystemService(Context.AudioService) as Android.Media.AudioManager;
            var devices = audioManager.GetDevices(GetDevicesTargets.All);

            List<string> DeviceList = new List<string>();

            foreach (var device in devices)
            {
                //DeviceList.Add(device.ToString() + " " + device.ProductNameFormatted + " " + (string)device.ProductName + " " + device.Type.ToString() + " " + (int)device.GetChannelCounts());

                int[] channelCounts = device.GetChannelCounts();
                List<string> ChannelCountList = new List<string>();
                foreach (int c in channelCounts)
                {
                    ChannelCountList.Add(c.ToString());
                }

                string ChannelCountString = string.Join("|", ChannelCountList);

                string DeviceType = device.Type.ToString();

                string ProductNameFormatted = device.ProductNameFormatted.ToString();

                string ProductName = (string)device.ProductName;

                //https://developer.android.com/reference/android/media/AudioDeviceInfo#getEncodings()
                Android.Media.Encoding[] Encodings = device.GetEncodings();
                List<string> EncodingList = new List<string>();
                foreach (Android.Media.Encoding en in Encodings)
                {
                    EncodingList.Add(en.ToString());
                }
                string EncodingString = string.Join("|", EncodingList);

                //DeviceList.Add(device.ToString() + ", " + ProductNameFormatted + ", " + ProductName + ", " + DeviceType + ", IsSink:" + device.IsSink.ToString() + ", IsSource:" + device.IsSource.ToString() + ", Channels " + ChannelCountString + "\n   Encodings: " + EncodingString);

                DeviceList.Add("Device: " + device.ToString() + ", ProductNameFormatted: " + ProductNameFormatted + ", ProductName:" + ProductName + ", DeviceType: " + DeviceType + ", IsSink:" + device.IsSink.ToString() + ", IsSource:" + device.IsSource.ToString() + ", Channels " + ChannelCountString + "\n   Encodings: " + EncodingString);

            }

            return string.Join("\n", DeviceList);

        }



        Object TalkbackAudioTrack = null; // This is declared as an Object instead of AudioTrack since it will otherwise register an error in the Visual Studio editor.
        Object TalkbackAudioRecord = null;
        int TalkbackBufferSize = 1024;


        [SupportedOSPlatform("Android31.0")]
        public void StartTalkback()
        {

            int talkbackSampleRate = 48000;
            if (CurrentFormat != null)
            {
                talkbackSampleRate = (int)CurrentFormat.SampleRate;
            }

            runTalkBackLoop = false;

            // (re-)Initializing audio
            if (TalkbackAudioTrack != null)
            {
                AudioTrack castPreviuosTalkbackAudioTrack = (AudioTrack)TalkbackAudioTrack;
                castPreviuosTalkbackAudioTrack.Stop();
                castPreviuosTalkbackAudioTrack.Release();
                castPreviuosTalkbackAudioTrack.Dispose();
                TalkbackAudioTrack = null;
            }

            if (TalkbackAudioRecord != null)
            {
                AudioRecord castPreviuosTalkbackAudioRecord = (AudioRecord)TalkbackAudioRecord;
                castPreviuosTalkbackAudioRecord.Stop();
                castPreviuosTalkbackAudioRecord.Release();
                castPreviuosTalkbackAudioRecord.Dispose();
                TalkbackAudioRecord = null;
            }

            // Creating an AudioTrack object for talkback
            var TackbackAudioTrackBuilder = new AudioTrack.Builder();

            TackbackAudioTrackBuilder.SetAudioFormat(new AudioFormat.Builder()
                .SetEncoding(Android.Media.Encoding.Pcm32bit)
                .SetSampleRate(talkbackSampleRate)
                .SetChannelMask(ChannelOut.Mono)
                .Build());

            TackbackAudioTrackBuilder.SetBufferSizeInBytes(TalkbackBufferSize);

            TackbackAudioTrackBuilder.SetAudioAttributes(new Android.Media.AudioAttributes.Builder()
                .SetUsage(AudioUsageKind.Media)
                .SetContentType(AudioContentType.Music)
                .Build());

            TackbackAudioTrackBuilder.SetPerformanceMode(AudioTrackPerformanceMode.None);
            TackbackAudioTrackBuilder.SetTransferMode(AudioTrackMode.Stream);

            // Building the audio tracks
            TalkbackAudioTrack = TackbackAudioTrackBuilder.Build();

            AudioTrack castTalkbackAudioTrack = (AudioTrack)TalkbackAudioTrack;

            castTalkbackAudioTrack.SetStartThresholdInFrames(2);

            // Start the AudioTrack
            castTalkbackAudioTrack.Play();

            TalkbackAudioRecord = new AudioRecord(AudioSource.Default, talkbackSampleRate, ChannelIn.Mono, Encoding.Pcm32bit, TalkbackBufferSize);

            AudioRecord castTalkbackAudioRecord = (AudioRecord)TalkbackAudioRecord;

            //var mics = castTalkbackAudioRecord.ActiveMicrophones;
            //castTalkbackAudioRecord.SetPreferredDevice()

            castTalkbackAudioRecord.StartRecording();

            // Starting loop
            Thread newThread = new Thread(new ThreadStart(TalkbackLoop));
            newThread.Start();

        }

        private float talkbackGain = 0;
        public float TalkbackGain
        {
            get
            {
                return talkbackGain;
            }
            set
            {
                talkbackGain = value;
            }
        }

        public bool SupportsTalkBack
        {
            get
            {
                return true;
            }
        }

        [SupportedOSPlatform("Android31.0")]
        private void TalkbackLoop()
        {

            AudioTrack castTalkbackAudioTrack = (AudioTrack)TalkbackAudioTrack;
            AudioRecord castTalkbackAudioRecord = (AudioRecord)TalkbackAudioRecord;
            runTalkBackLoop = true;

            // Checking that we have the right output device
            CheckTalkBackAudioDevice();

            byte[] talkbackBuffer = new byte[TalkbackBufferSize];

            int bytesEachLoop = TalkbackBufferSize / 10;
            int loopsPerBuffer = TalkbackBufferSize / bytesEachLoop;

            Android.Media.Audiofx.LoudnessEnhancer loudnessEnhancer = new Android.Media.Audiofx.LoudnessEnhancer(castTalkbackAudioTrack.AudioSessionId);
            loudnessEnhancer.SetTargetGain((int)(100 * talkbackGain));
            loudnessEnhancer.SetEnabled(true);
            castTalkbackAudioTrack.AttachAuxEffect(loudnessEnhancer.Id);
            castTalkbackAudioTrack.SetAuxEffectSendLevel(1);

            float lastTalkbackGainValue = talkbackGain;

            do
            {
                try
                {
                    //castTalkbackAudioTrack.SetVolume(talkbackGain);

                    if (lastTalkbackGainValue != talkbackGain)
                    {
                        loudnessEnhancer.SetTargetGain((int)(100 * talkbackGain));
                    }

                    for (int i = 0; i < loopsPerBuffer; i++)
                    {
                        int lastBytesRead = castTalkbackAudioRecord.Read(talkbackBuffer, i * bytesEachLoop, bytesEachLoop, 0);
                        int lastBytesWritten = castTalkbackAudioTrack.Write(talkbackBuffer, i * bytesEachLoop, bytesEachLoop, WriteMode.Blocking);
                    }

                }
                catch (Exception)
                {
                    runTalkBackLoop = false;
                }
            } while (runTalkBackLoop == true);
        }

        public void StopTalkback()
        {
            runTalkBackLoop = false;
        }

        [SupportedOSPlatform("Android31.0")]
        bool CheckTalkBackAudioDevice(AudioManager audioManager = null)
        {

            // Output device
            AudioTrack castTalkbackAudioTrack = (AudioTrack)TalkbackAudioTrack;

            if (castTalkbackAudioTrack == null)
            {
                // This means that the talkback is not active. Simply returns true.
                return true;
            }

            // Trying to get the currently selected output device 
            if (audioManager == null) { audioManager = Android.App.Application.Context.GetSystemService(Context.AudioService) as Android.Media.AudioManager; }
            var devices = audioManager.GetDevices(GetDevicesTargets.Outputs);
            AudioDeviceInfo CurrentlySelectedOutputDevice = castTalkbackAudioTrack.RoutedDevice;

            if (CurrentlySelectedOutputDevice != null)
            {
                if (AudioSettings.SelectedOutputDeviceName == CurrentlySelectedOutputDevice.ProductName)
                {
                    return true;
                }
            }

            // Trying to set the intended output device.
            foreach (var device in devices)
            {
                if (device.ProductName != null)
                {
                    string ProductName = device.ProductName;
                    if (ProductName == AudioSettings.SelectedOutputDeviceName)
                    {
                        if (castTalkbackAudioTrack.SetPreferredDevice(device) == true)
                        {
                            return true;
                        };
                    }
                }
            }

            if (AudioSettings.AllowDefaultOutputDevice.Value == false)
            {
                return false;
            }
            //Else, leaves the default device (which should have been selected upon audio track creation

            // Input device
            AudioRecord castTalkbackAudioRecord = (AudioRecord)TalkbackAudioRecord;

            if (castTalkbackAudioRecord == null)
            {
                // This means that the talkback is not active. Simply returns true. (This should not be needed since a null castTalkbackAudioTrack will be caught above)
                return true;
            }

            // Trying to get the currently selected output device 
            var inputDevices = audioManager.GetDevices(GetDevicesTargets.Inputs);
            AudioDeviceInfo CurrentlySelectedInputDevice = castTalkbackAudioRecord.RoutedDevice;

            if (CurrentlySelectedInputDevice != null)
            {
                if (AudioSettings.SelectedInputDeviceName == CurrentlySelectedInputDevice.ProductName + "+" + CurrentlySelectedInputDevice.Type.ToString())
                {
                    return true;
                }
            }

            // Trying to set the intended input device.
            foreach (var device in inputDevices)
            {
                if (device.ProductName != null)
                {
                    if (device.ProductName + "+" + device.Type.ToString() == AudioSettings.SelectedInputDeviceName)
                    {
                        if (castTalkbackAudioRecord.SetPreferredDevice(device) == true)
                        {
                            return true;
                        };
                    }
                }
            }

            if (AudioSettings.AllowDefaultInputDevice.Value == false)
            {
                return false;
            }
            //Else, leaves the default device (which should have been selected upon audio track creation
            
            return true;
        }
    }



    /// <summary>
    /// Represents permission to access notification policy.
    /// </summary>
    public partial class AccessNotificationPolicy : Microsoft.Maui.ApplicationModel.Permissions.BasePermission
    {
        public override async Task<PermissionStatus> CheckStatusAsync()
        {
            var context = Android.App.Application.Context;

            var notificationManager = (Android.App.NotificationManager)context.GetSystemService(Android.Content.Context.NotificationService);

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M
                && notificationManager.IsNotificationPolicyAccessGranted)
            {
                return PermissionStatus.Granted;
            }

            return PermissionStatus.Denied;
        }

        public override async Task<PermissionStatus> RequestAsync()
        {
            var status = await CheckStatusAsync();
            if (status == PermissionStatus.Granted)
            {
                return status;
            }

            //var intent2 = new Android.Content.Intent(Android.Provider.Settings.ExtraDoNotDisturbModeEnabled);
            //var intent3 = new Android.Content.Intent(Android.Provider.Settings.ActionVoiceControlDoNotDisturbMode);

            var intent = new Android.Content.Intent(Android.Provider.Settings.ActionNotificationPolicyAccessSettings);
            intent.AddFlags(Android.Content.ActivityFlags.NewTask);
            Android.App.Application.Context.StartActivity(intent);

            // After returning to the app, you should check the permission status again
            // This could be done by the user manually calling a method to check the status
            // after setting the permission from the settings.
            return PermissionStatus.Denied; // Temporary response, actual check should be done after returning to the app

        }

        public override void EnsureDeclared()
        {
            // This permission is a special case and does not need to be declared in the AndroidManifest.xml
        }

        public override bool ShouldShowRationale()
        {
            // For ACCESS_NOTIFICATION_POLICY, we typically won't show a rationale, as this
            // permission is granted through the system settings and not a standard permission request.
            return false;
        }

    }

}
