using CommunityToolkit.Maui.Core;
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
        STFN.Audio.Formats.WaveFormat LastPlayedWaveFormat = new WaveFormat(48000, 32, 2, "", WaveFormat.WaveFormatEncodings.IeeeFloatingPoints); // Creating a default LastPlayedWaveFormat

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

        bool iSoundPlayer.WideFormatSupport { get { return true; } }

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


        bool iSoundPlayer.SwapOutputSounds(ref Sound NewOutputSound, bool Record, bool AppendRecordedSound)
        {
            return SwapOutputSounds(ref NewOutputSound, Record, AppendRecordedSound);
        }

        static int writtenSounds = 0;
        static List<string> writtenSoundsList = new List<string>();

        bool SwapOutputSounds(ref Sound NewOutputSound, bool Record, bool AppendRecordedSound)
        {

            if (NewOutputSound != null)
            {
                // Incrementing writtenSounds 
                writtenSounds += 1;
                string currentTempSoundFileName = "TempSound" + writtenSounds.ToString("000000") + ".wav";
                string cacheDirectory = Microsoft.Maui.Storage.FileSystem.Current.CacheDirectory;
                string currentTempSoundPath = Path.Combine(cacheDirectory, currentTempSoundFileName);

                // Removing the iXML Chunk (Apparently the MediaElement player cannot handle it...)
                NewOutputSound.SMA = null;

                // Storing the wave format, for use in fade out
                LastPlayedWaveFormat = NewOutputSound.WaveFormat;

                // Saving the file to cache memory
                NewOutputSound.WriteWaveFile(ref currentTempSoundPath);

                // Saving the file path for later removal
                writtenSoundsList.Add(currentTempSoundPath);

                Crossfade(currentTempSoundPath);
                return true;
            }
            else
            {
                FadeOutPlayback();
                return true;
            }

        }


        //private void PlaySound( string newSoundFilePath)
        //{

        //    try
        //    {

        //        //string CurrentSoundFileName = TestItems[CurrentTrialIndex].SoundFile;
        //        //string cacheDirectory = Microsoft.Maui.Storage.FileSystem.Current.CacheDirectory;
        //        //string currentTempSoundPath = Path.Combine(cacheDirectory, CurrentSoundFileName);

        //        //if (System.IO.File.Exists(currentTempSoundPath) == false)
        //        //{
        //        //    await CopySoundFile(Path.Combine("sounds", CurrentSoundFileName), currentTempSoundPath);
        //        //}

        //        if (lastStartedMediaPlayer == 1)
        //        {

        //            // Create a temporary file to store the audio data
        //            //string tempFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "temp_audio.mp3");

        //            // Write the audio data to the temporary file
        //            //File.WriteAllBytes(tempFilePath, audioBuffer);

        //            // Create a Stream from the audio buffer and set it as the MediaElement source.
        //            //Stream stream = new MemoryStream(audioBuffer);
        //            ////mediaElement2.Source = new FileMediaSource { File = "dummy.mp3" };
        //            //mediaElement2.Source = MediaSource.FromStream(() => stream);

        //            mediaElement2.Source = new Uri(newSoundFilePath);
        //        }
        //        else
        //        {
        //            mediaElement1.Source = new Uri(newSoundFilePath);
        //        }

        //        Crossfade(newSoundFilePath);

        //    }
        //    catch (Exception ex)
        //    {

        //        InfoText = InfoText + "\n" + ex.Message;
        //        UpdateInfoText_Safe();

        //    }


        //}


        // Crossfade from mediaElement1 to mediaElement2 over 1 seconds

        async void Crossfade(string newSoundFilePath)
        {

            double fadeDuration = overlapDuration; // using a local variable here so that fadeDuration never gets changed in the middle of a crossfade loop
            double step = overlapGranuality; // using a local variable here so that overlapGranuality never gets changed in the middle of a crossfade loop // Adjust step for smoother or faster crossfade

            // TODO: Here, the StartedSwappingOutputSounds event should be raised

            if (lastStartedMediaPlayer == 2)
            {
                // Fading in player 1
                // And fading out player 2
                mediaElement1.Source = new Uri(newSoundFilePath);

                //mediaElement1.Speed = 1.5; // Funny detail! The speed parameter sound very good!!

                mediaElement1.Volume = 0;
                mediaElement1.Play();

                lastStartedMediaPlayer = 1;

                if (fadeDuration > 0)
                {
                    // Cross-fading
                    for (double t = 0; t < 1; t += step)
                    {
                        mediaElement1.Volume = t;
                        mediaElement2.Volume = 1 - t;
                        await Task.Delay((int)(fadeDuration * 1000 * step));
                    }
                }

                // Ensure volume is final values
                mediaElement1.Volume = 1;
                mediaElement2.Volume = 0;

                mediaElement2.Stop();
                // Setting the Source to null, so that the file can be overwritten on the next swap
                mediaElement2.Source = null;

            }
            else
            {

                // Fading in player 2
                // And fading out player 1

                mediaElement2.Source = new Uri(newSoundFilePath);

                //mediaElement2.Speed = 0.5; // Funny detail! The speed parameter sound very good!!
                mediaElement2.Volume = 0;
                mediaElement2.Play();

                lastStartedMediaPlayer = 2;

                if (fadeDuration > 0)
                {
                    // Cross-fading
                    for (double t = 0; t < 1; t += step)
                    {
                        mediaElement2.Volume = t;
                        mediaElement1.Volume = 1 - t;
                        await Task.Delay((int)(overlapDuration * 1000 * step));
                    }
                }

                // Ensure volume is final values
                mediaElement1.Volume = 0;
                mediaElement2.Volume = 1;

                mediaElement1.Stop();
                // Setting the Source to null, so that the file can be overwritten on the next swap
                mediaElement1.Source = null;

            }




            // TODO: Here, the FinishedSwappingOutputSounds event should be raised

        }

        void iSoundPlayer.FadeOutPlayback()
        {
            FadeOutPlayback();
        }


        void FadeOutPlayback()
        {
            Sound silentSound = STFN.Audio.GenerateSound.Signals.CreateSilence(ref LastPlayedWaveFormat, null, Math.Max(0.1, overlapDuration * 2));
            SwapOutputSounds(ref silentSound, false, false);
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

            // Disconnecting source
            mediaElement1.Source = null;
            mediaElement2.Source = null;

            // Clearing temporary files
            foreach (string file  in writtenSoundsList)
            {
                try
                {
                    System.IO.File.Delete(file);
                }
                catch (Exception)
                {
                    // Just ignoring if delet was not possible
                }
            }

        }
    }

}
