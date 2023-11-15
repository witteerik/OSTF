using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using STFN.Audio;
using STFN.Audio.Formats;
using STFN.Audio.SoundPlayers;
using STFN.Audio.SoundScene;

namespace STFM
{
    public class SoundPlayer : iSoundPlayer
    {

        public MediaElement mediaElement1 = null;
        public MediaElement mediaElement2 = null;
        int lastStartedMediaPlayer = 2;
        double overlapDuration = 1;
        double overlapGranuality = 0.05;
        string InfoText = "";

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

        public event iSoundPlayer.StartedSwappingOutputSoundsEventHandler StartedSwappingOutputSounds;
        public event iSoundPlayer.FinishedSwappingOutputSoundsEventHandler FinishedSwappingOutputSounds;

        bool iSoundPlayer.IsPlaying
        {
            get { return IsPlaying; }
        }

        public SoundPlayer(Microsoft.Maui.Controls.VerticalStackLayout ParentContainer)
        {
            mediaElement1 = new MediaElement { ShouldShowPlaybackControls = false };
            mediaElement2 = new MediaElement { ShouldShowPlaybackControls = false };
            mediaElement1.IsVisible = false;
            mediaElement2.IsVisible = false;

            mediaElement1.MediaFailed += MediaFail_Handler;
            mediaElement1.MediaOpened += MediaOpened_Handler;
            mediaElement1.MediaEnded += MediaEnded_Handler;
            mediaElement1.StateChanged += MediaStateChanged_Handler;

            mediaElement2.MediaFailed += MediaFail_Handler;
            mediaElement2.MediaOpened += MediaOpened_Handler;
            mediaElement2.MediaEnded += MediaEnded_Handler;
            mediaElement2.StateChanged += MediaStateChanged_Handler;

            mediaElement1.HeightRequest = 200;
            mediaElement1.WidthRequest = 400;
            mediaElement2.HeightRequest = 200;
            mediaElement2.WidthRequest = 400;

            ParentContainer.Children.Add(mediaElement1);
            ParentContainer.Children.Add(mediaElement2);

            ParentContainer.BackgroundColor = Colors.Beige;

        }


        void iSoundPlayer.SetOverlapDuration(double Duration)
        {
            overlapDuration = Duration;
        }

        double iSoundPlayer.GetOverlapDuration()
        {
            return overlapDuration;
        }
        void iSoundPlayer.SetOverlapGranuality(double Granuality)
        {
            overlapGranuality = Granuality;
        }

        double iSoundPlayer.GetOverlapGranuality()
        {
            return overlapGranuality;
        }

        public void PlaySound( string soundFilePath)
        {

            try
            {

                //string CurrentSoundFileName = TestItems[CurrentTrialIndex].SoundFile;
                //string cacheDir = Microsoft.Maui.Storage.FileSystem.Current.CacheDirectory;
                //string TempSoundPath = Path.Combine(cacheDir, CurrentSoundFileName);

                //if (System.IO.File.Exists(TempSoundPath) == false)
                //{
                //    await CopySoundFile(Path.Combine("sounds", CurrentSoundFileName), TempSoundPath);
                //}

                if (lastStartedMediaPlayer == 1)
                {

                    // Create a temporary file to store the audio data
                    //string tempFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "temp_audio.mp3");

                    // Write the audio data to the temporary file
                    //File.WriteAllBytes(tempFilePath, audioBuffer);

                    // Create a Stream from the audio buffer and set it as the MediaElement source.
                    //Stream stream = new MemoryStream(audioBuffer);
                    ////mediaElement2.Source = new FileMediaSource { File = "dummy.mp3" };
                    //mediaElement2.Source = MediaSource.FromStream(() => stream);

                    mediaElement2.Source = new Uri(soundFilePath);
                }
                else
                {
                    mediaElement1.Source = new Uri(soundFilePath);
                }

                Crossfade();

            }
            catch (Exception ex)
            {

                InfoText = InfoText + "\n" + ex.Message;
                UpdateInfoText_Safe();

            }


        }


        // Crossfade from mediaElement1 to mediaElement2 over 1 seconds
        async void Crossfade()
        {
            double fadeDuration = overlapDuration; // using a local variable here so that fadeDuration never gets changed in the middle of a crossfade loop
            double step = overlapGranuality; // using a local variable here so that overlapGranuality never gets changed in the middle of a crossfade loop // Adjust step for smoother or faster crossfade

            // TODO: Here, the StartedSwappingOutputSounds event should be raised

            if (lastStartedMediaPlayer == 1)
            {

                mediaElement2.Play();

                lastStartedMediaPlayer = 2;

                for (double t = 0; t < 1; t += step)
                {
                    mediaElement1.Volume = 1 - t;
                    mediaElement2.Volume = t;
                    await Task.Delay((int)(overlapDuration * 1000 * step));
                }

                mediaElement1.Stop();
                mediaElement2.Volume = 1; // Ensure volume is max for mediaElement2

            }
            else
            {

                mediaElement1.Play();

                lastStartedMediaPlayer = 1;

                for (double t = 0; t < 1; t += step)
                {
                    mediaElement2.Volume = 1 - t;
                    mediaElement1.Volume = t;
                    await Task.Delay((int)(fadeDuration * 1000 * step));
                }

                mediaElement2.Stop();
                mediaElement1.Volume = 1; // Ensure volume is max for mediaElement2

            }

            // TODO: Here, the FinishedSwappingOutputSounds event should be raised

        }


        bool iSoundPlayer.SwapOutputSounds(ref Sound NewOutputSound, bool Record, bool AppendRecordedSound)
        {

            if (NewOutputSound != null)
            {

                string CurrentTempSoundFileName;

                if (lastStartedMediaPlayer == 1)
                {
                    CurrentTempSoundFileName = "TempSound2.wav";
                }
                else
                {
                    CurrentTempSoundFileName = "TempSound1.wav";
                }

                string cacheDir = Microsoft.Maui.Storage.FileSystem.Current.CacheDirectory;
                string TempSoundPath = Path.Combine(cacheDir, CurrentTempSoundFileName);

                // Removing the iXML Chunk (Apparently the MediaElement player cannot handle it...)
                NewOutputSound.SMA = null;

                // Saving the file to cache memory
                NewOutputSound.WriteWaveFile(ref TempSoundPath);

                PlaySound(TempSoundPath);
                return true;
            }
            else
            {
                return false;
            }

        }

        void iSoundPlayer.FadeOutPlayback()
        {
            throw new NotImplementedException();
        }

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

        void MediaFail_Handler(object sender, MediaFailedEventArgs e)
        {
            InfoText = InfoText + "\n" + e.ErrorMessage;
            UpdateInfoText_Safe();
        }

        void MediaOpened_Handler(object sender, EventArgs e)
        {
            InfoText = InfoText + "\n" + "Loaded sound";
            UpdateInfoText_Safe();
        }

        void MediaEnded_Handler(object sender, EventArgs e)
        {
            InfoText = InfoText + "\n" + "Media ended";
            UpdateInfoText_Safe();
        }

        void MediaStateChanged_Handler(object sender, MediaStateChangedEventArgs e)
        {
            InfoText = InfoText + "\n" + "New state: " + e.NewState.ToString();
            UpdateInfoText_Safe();
        }

        void UpdateInfoText_Safe()
        {
            MainThread.BeginInvokeOnMainThread(UpdateInfoText);
        }

        void UpdateInfoText()
        {
            //InfoLabel.Text = InfoText;
        }

        Sound iSoundPlayer.GetRecordedSound(bool ClearRecordingBuffer)
        {
            throw new NotImplementedException("The STFM Sound player cannot yet record sound!");
        }

        void iSoundPlayer.ChangePlayerSettings(ref AudioApiSettings AudioApiSettings, int? SampleRate, int? BitDepth, WaveFormat.WaveFormatEncodings? Encoding, double? OverlapDuration, ref DuplexMixer Mixer, iSoundPlayer.SoundDirections? SoundDirection, bool ReOpenStream, bool ReStartStream, bool? ClippingIsActivated)
        {
            // Ignored!
        }

        void iSoundPlayer.CloseStream()
        {
            // No need to close the stream, but stops the sound, if called
            if (mediaElement1 != null)
            {
                mediaElement1.Stop();
            }
            if (mediaElement2 != null)
            {
                mediaElement2.Stop();
            }
        }

        void iSoundPlayer.Dispose()
        {
            // Ignored, but stops the sound, if called
            if (mediaElement1 != null)
            {
                mediaElement1.Stop();
            }
            if (mediaElement2 != null)
            {
                mediaElement2.Stop();
            }
        }
    }

}
