using Android;
using Android.AdServices.AdIds;
using Android.AdServices.Common;
using Android.AdServices.Topics;
using Android.Bluetooth;
using Android.Content;
using Android.Health.Connect.DataTypes.Units;
using Android.Media;
using Android.Net.Wifi.P2p;
using AndroidX.Core.App;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Org.Apache.Http.Conn.Routing;
using System.Diagnostics;
using System.Runtime.Versioning;
using Xamarin.Google.Crypto.Tink.Prf;
using Xamarin.Google.Crypto.Tink.Shaded.Protobuf;
using static Microsoft.Maui.ApplicationModel.Permissions;
using static Microsoft.Maui.Controls.PlatformConfiguration.Android;

namespace ExoPlayerTestApp
{
    public partial class MainPage : ContentPage
    {

        double volume  = 0.000005;

        public MainPage()
        {
            InitializeComponent();


            VolLabel.Text = volume.ToString();

        }


        private void OnGetDevicesBtn(object sender, EventArgs e)
        {
            var audioManager = Android.App.Application.Context.GetSystemService(Context.AudioService) as Android.Media.AudioManager;
            var devices = audioManager.GetDevices(GetDevicesTargets.All);

            List<string> DeviceList = new List<string>();

            foreach (var device in devices)
            {
                //DeviceList.Add(device.ToString() + " " + device.ProductNameFormatted + " " + (string)device.ProductName + " " + device.Type.ToString() + " " + (int)device.GetChannelCounts());

                int[] channelCounts = device.GetChannelCounts();
                List<string> ChannelCountList = new List<string>();
                foreach (int  c in channelCounts)
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

                DeviceList.Add(device.ToString() + ", " + ProductNameFormatted + ", " + ProductName + ", " + DeviceType  + ", IsSink:" + device.IsSink.ToString() + ", IsSource:" + device.IsSource.ToString() +  ", Channels " + ChannelCountString + "\n   Encodings: " + EncodingString);

                //DeviceList.Add(device.ToString() + " " + (string)device.ProductNameFormatted + " " + (string)device.ProductName + " " + device.Type.ToString() + " " + ChannelCountString);
                
            }

            DeviceLabel.Text = string.Join("\n", DeviceList);

        }

        private void VolUpBtnClicked(object sender, EventArgs e)
        {
            volume = volume * 2;
            VolLabel.Text = volume.ToString();
        }

        private void AVolUpBtnClicked(object sender, EventArgs e)
        {

            var audioManager = Android.App.Application.Context.GetSystemService(Context.AudioService) as Android.Media.AudioManager;

            int? MaxVol = audioManager?.GetStreamMaxVolume(Android.Media.Stream.Music);
            int? MinVol = audioManager?.GetStreamMinVolume(Android.Media.Stream.Music);

            int? currentVolume = audioManager?.GetStreamVolume(Android.Media.Stream.Music);
            //audioManager?.GetStreamVolumeDb(Android.Media.Stream.Music);

            int newVolume = Math.Min((int)MaxVol, (int)currentVolume +1);

            audioManager?.SetStreamVolume(Android.Media.Stream.Music, newVolume, VolumeNotificationFlags.ShowUi);

            //audioManager?.SetStreamVolume(Android.Media.Stream.Alarm, 0, VolumeNotificationFlags.Vibrate);
            //audioManager?.SetStreamVolume(Android.Media.Stream.System, 0, VolumeNotificationFlags.PlaySound);
            //audioManager?.SetStreamVolume(Android.Media.Stream.Accessibility, 0, VolumeNotificationFlags.PlaySound);
            //audioManager?.SetStreamVolume(Android.Media.Stream.VoiceCall, 0, VolumeNotificationFlags.PlaySound);
            //audioManager?.SetStreamVolume(Android.Media.Stream.Dtmf, 0, VolumeNotificationFlags.PlaySound);
            //audioManager?.SetStreamVolume(Android.Media.Stream.Notification, 0, VolumeNotificationFlags.PlaySound);
            //audioManager?.SetStreamVolume(Android.Media.Stream.NotificationDefault, 0, VolumeNotificationFlags.PlaySound);
            //audioManager?.SetStreamVolume(Android.Media.Stream.Ring, 0, VolumeNotificationFlags.PlaySound);


            var pars = audioManager?.GetParameters("");

            var rate = audioManager?.GetProperty(Android.Media.AudioManager.PropertyOutputSampleRate);
            var rate2 = audioManager?.GetProperty(Android.Media.AudioManager.PropertyOutputFramesPerBuffer);
            var rate3 = audioManager?.GetProperty(Android.Media.AudioManager.ActionHeadsetPlug);
            var rate4 = audioManager?.GetProperty(Android.Media.AudioManager.PropertySupportAudioSourceUnprocessed);


        }

        private void VolDwnBtnClicked(object sender, EventArgs e)
        {
            
            volume = volume / 2;
            VolLabel.Text = volume.ToString();
        }

        private void AVolDwnBtnClicked(object sender, EventArgs e)
        {

            var audioManager = Android.App.Application.Context.GetSystemService(Context.AudioService) as Android.Media.AudioManager;

            int? MaxVol = audioManager?.GetStreamMaxVolume(Android.Media.Stream.Music);
            int? MinVol = audioManager?.GetStreamMinVolume(Android.Media.Stream.Music);

            int? currentVolume = audioManager?.GetStreamVolume(Android.Media.Stream.Music);
            //audioManager?.GetStreamVolumeDb(Android.Media.Stream.Music);

            int newVolume = Math.Max((int)MinVol, (int)currentVolume - 1);

            audioManager?.SetStreamVolume(Android.Media.Stream.Music, newVolume, VolumeNotificationFlags.ShowUi);

        }

        private void OnSine3Clicked(object sender, EventArgs e)
        {

            PlaySineWave32I_8Ch();
        }

        private void OnSine2Clicked(object sender, EventArgs e)
        { 

            PlaySineWave16I();
        }

            private  void OnSine1Clicked(object sender, EventArgs e)
        {

            PlaySineWave32I ();

            //PlaySineWave24();

            //PlaySineWave2();
                       return;

            CustomAudioTrack customAudioTrack = new CustomAudioTrack(48000, ChannelOut.Mono, Encoding.Pcm32bit);
            customAudioTrack.Play();
            List<float> audioSamples = new List<float>();
            Random rnd = new Random();
            for (int i = 0; i < 48000; i++)
            {
                audioSamples.Add(rnd.NextSingle()-(float)0.5);
            }


            customAudioTrack.LoadAndPlayWaveSamples(audioSamples.ToArray(),48000,1);

            //await  CheckPermissions();

            //PlaySineWave2();


            //string filePath = "sine.wav";
            //using System.IO.Stream fileStream = await Microsoft.Maui.Storage.FileSystem.Current.OpenAppPackageFileAsync(filePath);
            ////var myStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            //var audioPlayer = new AudioPlayer(fileStream);
            //audioPlayer.Play();




            //// Define your audio parameters
            //var sampleRate = 44100; // Sample rate in Hz
            //var channelConfig = ChannelOut.Mono; // Mono or stereo
            //var audioFormat = Android.Media.Encoding.PcmFloat; // Choose PCM format


            //double Frequency = 1000; // Frequency of the sine wave in Hz
            //double Duration = 3; // Duration of the sine wave in seconds
            //int numSamples = (int)(Duration * sampleRate);
            //double[] samples = new double[numSamples];

            //for (int i = 0; i < numSamples; i++)
            //{
            //    samples[i] = Math.Sin(2 * Math.PI * Frequency * i / sampleRate);
            //}

            ////// Convert samples to byte array
            ////byte[] rawData = new byte[2 * numSamples];
            ////int idx = 0;
            ////foreach (double dVal in samples)
            ////{
            ////    short val = (short)(dVal * 32767);
            ////    rawData[idx++] = (byte)(val & 0x00ff);
            ////    rawData[idx++] = (byte)((val & 0xff00) >> 8);
            ////}

            //// Convert samples to byte array with PCM float encoding
            //byte[] rawData = new byte[4 * numSamples];
            //int idx = 0;
            //foreach (double dVal in samples)
            //{
            //    float val = (float)dVal;
            //    int intVal = Java.Lang.Float.FloatToIntBits(val);
            //    rawData[idx++] = (byte)intVal;
            //    rawData[idx++] = (byte)(intVal >> 8);
            //    rawData[idx++] = (byte)(intVal >> 16);
            //    rawData[idx++] = (byte)(intVal >> 24);
            //}


            //// Calculate the buffer size
            //var bufferSize = AudioTrack.GetMinBufferSize(sampleRate, channelConfig, audioFormat);

            //// Create an AudioTrack instance
            //var audioTrack = new AudioTrack(
            //      Android.Media.Stream.Music,
            //      sampleRate,
            //      Android.Media.ChannelOut.Mono,
            //      Android.Media.Encoding.PcmFloat,
            //      numSamples * 4,
            //      AudioTrackMode.Static);

            //// Write audio data to the AudioTrack buffer (assuming rawData is your audio data)
            //audioTrack.Write(rawData, 0, rawData.Length);

            //// Start playback
            //audioTrack.Play();


            // Stop playback when done
            //audioTrack.Stop();
            //audioTrack.Release();

        }


        [SupportedOSPlatform("Android23.0")]
        public void PlaySineWave32I_OLD()
        {
            int SampleRate = 44100; // Sample rate in Hz
            double Frequency_L = 1000 - 10; // Frequency of the sine wave in Hz (Changed to 1000 Hz)
            double Frequency_R = 1000 + 10; // Frequency of the sine wave in Hz (Changed to 1000 Hz)
            double Duration = 3; // Duration of the sine wave in seconds
            double Amplitude = volume * Int32.MaxValue;

            int numSamples = (int)(Duration * SampleRate);
            double[] sample_L = new double[numSamples];
            for (int i = 0; i < numSamples; i++)
            {
                sample_L[i] = Amplitude * Math.Sin(2 * Math.PI * Frequency_L * i / SampleRate);
            }

            double[] sample_R = new double[numSamples];
            for (int i = 0; i < numSamples; i++)
            {
                sample_R[i] = Amplitude * Math.Sin(2 * Math.PI * Frequency_R * i / SampleRate);
            }


            // Convert samples to byte array with PCM float encoding
            byte[] generatedSnd = new byte[2 * 4 * numSamples];
            int idx = 0;
            for (int i = 0; i < numSamples; i++)
            {

                // Copying to left channel

                Int32 val_L = (Int32)sample_L[i];
                byte[] val_bytes_L = BitConverter.GetBytes(val_L);
                val_bytes_L.CopyTo(generatedSnd, idx);
                idx += 4;

                // Copying to right channel
                Int32 val_R = (Int32)sample_R[i];
                byte[] val_bytes_R = BitConverter.GetBytes(val_R);
                val_bytes_R.CopyTo(generatedSnd, idx);
                idx += 4;

            }

            // Create AudioTrack with PCM float format using AudioTrack.Builder
            var audioTrackBuilder = new AudioTrack.Builder();

            audioTrackBuilder.SetAudioFormat(new AudioFormat.Builder()
                .SetEncoding(Encoding.Pcm32bit)
                .SetSampleRate(SampleRate)
                .SetChannelMask(ChannelOut.Stereo)
                .Build());

            audioTrackBuilder.SetBufferSizeInBytes(2 * numSamples * 4);

            audioTrackBuilder.SetAudioAttributes(new AudioAttributes.Builder()
                .SetUsage(AudioUsageKind.Media)
                .SetContentType(AudioContentType.Music)
                .Build());

            audioTrackBuilder.SetPerformanceMode(AudioTrackPerformanceMode.LowLatency);

            var audioTrack = audioTrackBuilder.Build();

            try
            {
                // Start playback
                audioTrack.Play();

                // Write the generated audio data to the AudioTrack
                audioTrack.Write(generatedSnd, 0, generatedSnd.Length, WriteMode.NonBlocking);

            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine("Error playing audio: " + ex.Message);
            }
        }

        private int currentPosition = 0;
        double[] sample_R;
        double[] sample_L;
        int bufferSizeInSamples = 64*1024;
        int currentBufferIndex = 0;
        AudioTrack audioTrack;

        [SupportedOSPlatform("Android23.0")]
        public void PlaySineWave32I()
        {
            int SampleRate = 44100; // Sample rate in Hz
            double Frequency_L = 1000 - 10; // Frequency of the sine wave in Hz (Changed to 1000 Hz)
            double Frequency_R = 1000 + 10; // Frequency of the sine wave in Hz (Changed to 1000 Hz)
            double Duration = 20; // Duration of the sine wave in seconds
            double Amplitude = volume * Int32.MaxValue;

            int numSamples = (int)(Duration * SampleRate);
            sample_L = new double[numSamples];
            for (int i = 0; i < numSamples; i++)
            {
                sample_L[i] = Amplitude * Math.Sin(2 * Math.PI * Frequency_L * i / SampleRate);
            }

            sample_R = new double[numSamples];
            for (int i = 0; i < numSamples; i++)
            {
                sample_R[i] = Amplitude * Math.Sin(2 * Math.PI * Frequency_R * i / SampleRate);
            }

           
            // Create AudioTrack with PCM float format using AudioTrack.Builder
            var audioTrackBuilder = new AudioTrack.Builder();

            audioTrackBuilder.SetAudioFormat(new AudioFormat.Builder()
                .SetEncoding(Encoding.Pcm32bit)
                .SetSampleRate(SampleRate)
                .SetChannelMask(ChannelOut.Stereo)
                .Build());

            audioTrackBuilder.SetBufferSizeInBytes(2 * bufferSizeInSamples * 4);

            audioTrackBuilder.SetAudioAttributes(new AudioAttributes.Builder()
                .SetUsage(AudioUsageKind.Media)
                .SetContentType(AudioContentType.Music)
                .Build());

            audioTrackBuilder.SetPerformanceMode(AudioTrackPerformanceMode.LowLatency);

            audioTrack = audioTrackBuilder.Build();

            audioTrack.SetNotificationMarkerPosition(4);
            audioTrack.MarkerReached += MarkerReached;

            //audioTrack.SetPositionNotificationPeriod((int)(bufferSizeInSamples*0.8));
            //audioTrack.PeriodicNotification += MarkerReached;
            // Send a first silent buffer
            byte[] generatedSnd = new byte[2 * 4 * bufferSizeInSamples];

            // Start playback
            audioTrack.Play();

            // Writes the first buffer (now empty)
            audioTrack.Write(generatedSnd, 0, generatedSnd.Length, WriteMode.NonBlocking);

        }


        private void MarkerReached(object sender, AudioTrack.MarkerReachedEventArgs e)
        {

            // Convert samples to byte array with PCM float encoding
            byte[] generatedSnd = new byte[2 * 4 * bufferSizeInSamples];
            int idx = 0;
            for (int i = currentBufferIndex * bufferSizeInSamples; i < (currentBufferIndex + 1) * bufferSizeInSamples; i++)
            {

                // Copying to left channel

                Int32 val_L = (Int32)sample_L[i];
                byte[] val_bytes_L = BitConverter.GetBytes(val_L);
                val_bytes_L.CopyTo(generatedSnd, idx);
                idx += 4;

                // Copying to right channel
                Int32 val_R = (Int32)sample_R[i];
                byte[] val_bytes_R = BitConverter.GetBytes(val_R);
                val_bytes_R.CopyTo(generatedSnd, idx);
                idx += 4;

            }

            // Write the generated audio data to the AudioTrack
            audioTrack.Write(generatedSnd, 0, generatedSnd.Length, WriteMode.Blocking);

            currentBufferIndex += 1;
                        
            if (currentBufferIndex >= 5) { currentBufferIndex = 0; }

            audioTrack.SetNotificationMarkerPosition(audioTrack.PlaybackHeadPosition + bufferSizeInSamples/2);

            DeviceLabel.Text = currentBufferIndex.ToString();

        }

        private void PeriodicMarkerReached(object sender, AudioTrack.PeriodicNotificationEventArgs e)
        {

            // Convert samples to byte array with PCM float encoding
            byte[] generatedSnd = new byte[2 * 4 * bufferSizeInSamples];
            int idx = 0;
            for (int i = currentBufferIndex*bufferSizeInSamples; i < (currentBufferIndex +1) * bufferSizeInSamples; i++)
            {

                // Copying to left channel

                Int32 val_L = (Int32)sample_L[i];
                byte[] val_bytes_L = BitConverter.GetBytes(val_L);
                val_bytes_L.CopyTo(generatedSnd, idx);
                idx += 4;

                // Copying to right channel
                Int32 val_R = (Int32)sample_R[i];
                byte[] val_bytes_R = BitConverter.GetBytes(val_R);
                val_bytes_R.CopyTo(generatedSnd, idx);
                idx += 4;

            }

            // Write the generated audio data to the AudioTrack
            audioTrack.Write(generatedSnd, 0, generatedSnd.Length, WriteMode.Blocking);

            currentBufferIndex += 1;

        }


        [SupportedOSPlatform("Android23.0")]
        public void PlaySineWave32I_8Ch()
        {
            int SampleRate = 44100; // Sample rate in Hz
            double Duration = 3; // Duration of the sine wave in seconds
            double Amplitude = volume * Int32.MaxValue;

            int numSamples = (int)(Duration * SampleRate);

            int Channels = 8;

            List<double[]> channelArrays = new List<double[]>();
            List<double> frequencies = new List<double>() { 500,700,900,1100,1300,1500,1700,1900};

            for (int c = 0; c < Channels; c++)
            {
                double Frequency = frequencies[c];
                double[] samples = new double[numSamples];
                for (int i = 0; i < numSamples; i++)
                {
                    samples[i] = Amplitude * Math.Sin(2 * Math.PI * Frequency * i / SampleRate);
                }

                channelArrays.Add(samples);
            }


            // Convert samples to byte array with PCM float encoding
            byte[] generatedSnd = new byte[Channels * 4 * numSamples];
            int idx = 0;
            for (int i = 0; i < numSamples; i++)
            {

                // Copying channel data to buffer
                for (int c = 0; c < Channels; c++)
                {

                    Int32 val_L = (Int32)channelArrays[c][i];
                    byte[] val_bytes_L = BitConverter.GetBytes(val_L);
                    val_bytes_L.CopyTo(generatedSnd, idx);
                    idx += 4;

                }
            }

            // Create AudioTrack with PCM float format using AudioTrack.Builder
            var audioTrackBuilder = new AudioTrack.Builder();
           

            // https://developer.android.com/reference/android/media/AudioFormat.Builder#setChannelIndexMask(int)
            // Define a bit array containing boolean values for channels 1-32 ?
            System.Collections.BitArray bitArray = new System.Collections.BitArray(new bool[] { true, true, true, true , true, true , true, true }); // channel 1, channel 2, ...

            // Create a byte array with length of 4 (32 bits)
            byte[] bytes = new byte[4];

            // Copy bits from the BitArray to the byte array
            bitArray.CopyTo(bytes, 0);

            // Convert byte array to integer
            int result = BitConverter.ToInt32(bytes, 0);

    //        audioTrackBuilder.SetAudioFormat(new AudioFormat.Builder()
    //.SetEncoding(Encoding.Pcm32bit)
    //.SetSampleRate(SampleRate)
    //.SetChannelMask(ChannelOut.SevenPointOne)
    //.Build());


            audioTrackBuilder.SetAudioFormat(new AudioFormat.Builder()
                .SetEncoding(Encoding.Pcm32bit)
                .SetSampleRate(SampleRate)
                .SetChannelIndexMask(result)
                .Build());

            audioTrackBuilder.SetBufferSizeInBytes(Channels * numSamples * 4);

            audioTrackBuilder.SetAudioAttributes(new AudioAttributes.Builder()
                .SetUsage(AudioUsageKind.Media)
                .SetContentType(AudioContentType.Music)
                .Build());

            audioTrackBuilder.SetPerformanceMode(AudioTrackPerformanceMode.LowLatency);

            var audioTrack = audioTrackBuilder.Build();

            try
            {
                // Start playback
                audioTrack.Play();

                // Write the generated audio data to the AudioTrack
                audioTrack.Write(generatedSnd, 0, generatedSnd.Length, WriteMode.Blocking);

            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine("Error playing audio: " + ex.Message);
            }
        }


        [SupportedOSPlatform("Android23.0")]
        public void PlaySineWave16I()
        {
            int SampleRate = 44100; // Sample rate in Hz
            double Frequency = 1000; // Frequency of the sine wave in Hz (Changed to 1000 Hz)
            double Duration = 3; // Duration of the sine wave in seconds
            double Amplitude = volume * Int16.MaxValue;

            int numSamples = (int)(Duration * SampleRate);
            double[] sample = new double[numSamples];

            for (int i = 0; i < numSamples; i++)
            {
                sample[i] = Amplitude * Math.Sin(2 * Math.PI * Frequency * i / SampleRate);
            }

            // Convert samples to byte array with PCM float encoding
            byte[] generatedSnd = new byte[2 * numSamples];
            int idx = 0;
            foreach (double dVal in sample)
            {
                Int16 val = (Int16)dVal;

                byte[] val_bytes = BitConverter.GetBytes(val);
                //if (BitConverter.IsLittleEndian)
                //{
                //    Array.Reverse(val_bytes);
                //}
                val_bytes.CopyTo(generatedSnd, idx);
                idx += 2;

                //int intVal = Java.Lang.Float.FloatToIntBits(val);

                //generatedSnd[idx++] = (byte)intVal;
                //generatedSnd[idx++] = (byte)(intVal >> 8);
                //generatedSnd[idx++] = (byte)(intVal >> 16);
                //generatedSnd[idx++] = (byte)(intVal >> 24);
            }

            // Create AudioTrack with PCM float format using AudioTrack.Builder
            var audioTrackBuilder = new AudioTrack.Builder();

            audioTrackBuilder.SetAudioFormat(new AudioFormat.Builder()
                .SetEncoding(Encoding.Pcm16bit)
                .SetSampleRate(SampleRate)
                .SetChannelMask(ChannelOut.Default)
                .Build());

            audioTrackBuilder.SetBufferSizeInBytes(numSamples * 2);

            audioTrackBuilder.SetAudioAttributes(new AudioAttributes.Builder()
                .SetUsage(AudioUsageKind.Media)
                .SetContentType(AudioContentType.Music)
                .Build());

            audioTrackBuilder.SetPerformanceMode(AudioTrackPerformanceMode.LowLatency);

            var audioTrack = audioTrackBuilder.Build();

            try
            {
                // Start playback
                audioTrack.Play();

                // Write the generated audio data to the AudioTrack
                audioTrack.Write(generatedSnd, 0, generatedSnd.Length, WriteMode.Blocking);

            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine("Error playing audio: " + ex.Message);
            }
        }


        [SupportedOSPlatform("Android23.0")]
        public void PlaySineWave2()
        {
            int SampleRate = 44100; // Sample rate in Hz
            float Frequency = 1000; // Frequency of the sine wave in Hz (Changed to 1000 Hz)
            double Duration = 3; // Duration of the sine wave in seconds
            float Amplitude = (float)0.001;

            int numSamples = (int)(Duration * SampleRate);
            short[] sample = new short[numSamples];

            double twopi = 2 * Math.PI;
            double Phase = 0;
            float PositiveFullScale = short.MaxValue;

            for (int i = 0; i < numSamples; i++)
            {
                sample[i] = (short)((Amplitude * PositiveFullScale) * Math.Sin(Phase + twopi * (Frequency / SampleRate) * i));
            }
            
            // Create AudioTrack with PCM float format using AudioTrack.Builder
            var audioTrackBuilder = new AudioTrack.Builder();

            audioTrackBuilder.SetAudioFormat(new AudioFormat.Builder()
                .SetEncoding(Encoding.Pcm16bit)
                .SetSampleRate(SampleRate)
                .SetChannelMask(ChannelOut.Mono)
                .Build());

            //audioTrackBuilder.SetBufferSizeInBytes(numSamples * 4);
            audioTrackBuilder.SetBufferSizeInBytes(numSamples);


                    audioTrackBuilder.SetAudioAttributes(new AudioAttributes.Builder()
            .SetUsage(AudioUsageKind.Media)
            .SetContentType(AudioContentType.Music)
            .SetLegacyStreamType(Android.Media.Stream.Music)
            .Build());


            audioTrackBuilder.SetPerformanceMode(AudioTrackPerformanceMode.LowLatency);

            audioTrackBuilder.SetTransferMode(AudioTrackMode.Stream);

            var audioTrack = audioTrackBuilder.Build();


            try
            {
                // Start playback
                audioTrack.Play();

                // Write the generated audio data to the AudioTrack
                audioTrack.Write(sample, 0, sample.Length, WriteMode.Blocking);

            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine("Error playing audio: " + ex.Message);
            }
        }

        [SupportedOSPlatform("Android23.0")]
        public void PlaySineWave24()
        {
            int SampleRate = 44100; // Sample rate in Hz
            float Frequency = 600; // Frequency of the sine wave in Hz (Changed to 1000 Hz)
            double Duration = 3; // Duration of the sine wave in seconds
            float Amplitude = (float)1;

            int numSamples = (int)(Duration * SampleRate);
            float[] sample = new float[numSamples];

            double twopi = 2 * Math.PI;
            double Phase = 0;
            float PositiveFullScale = (float)Math.Pow(2, 23) - 2; //short.MaxValue;

            for (int i = 0; i < numSamples; i++)
            {
                sample[i] = (short)((Amplitude * PositiveFullScale) * Math.Sin(Phase + twopi * (Frequency / SampleRate) * i));
            }

            // Create AudioTrack with PCM float format using AudioTrack.Builder
            var audioTrackBuilder = new AudioTrack.Builder();

            audioTrackBuilder.SetAudioFormat(new AudioFormat.Builder()
                .SetEncoding(Encoding.Pcm24bitPacked)
                .SetSampleRate(SampleRate)
                .SetChannelMask(ChannelOut.Mono)
                .Build());

            //audioTrackBuilder.SetBufferSizeInBytes(numSamples * 4);
            audioTrackBuilder.SetBufferSizeInBytes(numSamples);


            audioTrackBuilder.SetAudioAttributes(new AudioAttributes.Builder()
    .SetUsage(AudioUsageKind.Media)
    .SetContentType(AudioContentType.Music)
    .SetLegacyStreamType(Android.Media.Stream.Music)
    .Build());


            audioTrackBuilder.SetPerformanceMode(AudioTrackPerformanceMode.LowLatency);

            audioTrackBuilder.SetTransferMode(AudioTrackMode.Stream);

            var audioTrack = audioTrackBuilder.Build();


            try
            {
                // Start playback
                audioTrack.Play();

                // Write the generated audio data to the AudioTrack
                audioTrack.Write(sample, 0, sample.Length, WriteMode.Blocking);

            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine("Error playing audio: " + ex.Message);
            }
        }


        async static Task CheckPermissions()
        {

            bool hasStorageReadPermission = await Permissions.CheckStatusAsync<Permissions.StorageRead>() == PermissionStatus.Granted;
            bool hasStorageWritePermission = await Permissions.CheckStatusAsync<Permissions.StorageWrite>() == PermissionStatus.Granted;
            bool hasMediaPermission = await Permissions.CheckStatusAsync<Permissions.Media>() == PermissionStatus.Granted;
            bool hasPhotoPermission = await Permissions.CheckStatusAsync<Permissions.Photos>() == PermissionStatus.Granted;

            bool hasAudioPermission = await Permissions.CheckStatusAsync<Permissions.Microphone>() == PermissionStatus.Granted;

            if (hasStorageReadPermission == false)
            {
                var status = await Permissions.RequestAsync<Permissions.StorageRead>();
            }

            if (hasStorageWritePermission == false)
            {
                var status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            }

            if (hasMediaPermission == false)
            {
                var status = await Permissions.RequestAsync<Permissions.Media>();
            }

            if (hasPhotoPermission == false)
            {
                var status = await Permissions.RequestAsync<Permissions.Photos>();
            }

            if (hasAudioPermission == false)
            {
                var status = await Permissions.RequestAsync<Permissions.Microphone>();
            }


            bool hasAudioPermission2 = await Permissions.CheckStatusAsync<ModifyAudioSettings>() == PermissionStatus.Granted;

            if (hasAudioPermission2 == false)
            {
                var status = await RequestAsync();
            }

        }

        public partial class ModifyAudioSettings : BasePlatformPermission
        {
            public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
                new (string, bool)[] { (Manifest.Permission.ReadCalendar, true) };
        }


        async static Task<PermissionStatus> RequestAsync()
        {
            // Check status before requesting first
            if (await Permissions.CheckStatusAsync<ModifyAudioSettings>() == PermissionStatus.Granted)
                return PermissionStatus.Granted;

            List<string> req = new List<string>();
            req.Add(Manifest.Permission.ReadCalendar);

            ActivityCompat.RequestPermissions(ActivityStateManager.Default.GetCurrentActivity(), req.ToArray(), 2);

            //ActivityCompat.ShouldShowRequestPermissionRationale(ActivityStateManager.Default.GetCurrentActivity(), Manifest.Permission.CaptureAudioOutput.ToString());

            return PermissionStatus.Granted;
        }


    }

    public class CustomAudioTrack : IDisposable
    {
        private AudioTrack audioTrack;
        private int bufferSize;
        private byte[] buffer;

        public CustomAudioTrack(int sampleRate, ChannelOut channelConfig, Encoding audioFormat)
        {
            bufferSize = AudioTrack.GetMinBufferSize(sampleRate, channelConfig, audioFormat);
            audioTrack = new AudioTrack(Android.Media.Stream.Music, sampleRate, channelConfig, audioFormat, bufferSize, AudioTrackMode.Stream);
            buffer = new byte[bufferSize];
        }

        public void Play()
        {
            audioTrack.Play();
        }

        public void Write(byte[] data, int offset, int count)
        {
            audioTrack.Write(data, offset, count);
        }

        public void LoadAndPlayWaveSamples(float[] waveSamples, int sampleRate, int channels)
        {
            var waveProvider = new ReadOnlyWaveProvider(waveSamples, sampleRate, channels);

            int bytesRead;
            do
            {
                bytesRead = waveProvider.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    audioTrack.Write(buffer, 0, bytesRead);
                }
            } while (bytesRead > 0);
        }

        public void Dispose()
        {
            if (audioTrack != null)
            {
                audioTrack.Stop();
                audioTrack.Release();
                audioTrack = null;
            }
        }

    }

    // Custom class to convert wave samples to byte array
    public class ReadOnlyWaveProvider 
    {
        private readonly float[] waveSamples;
        private int position;
        private readonly int sampleRate;
        private readonly int channels;

        public ReadOnlyWaveProvider(float[] waveSamples, int sampleRate, int channels)
        {
            this.waveSamples = waveSamples;
            this.position = 0;
            this.sampleRate = sampleRate;
            this.channels = channels;
        }

        //public int Read(byte[] buffer, int offset, int count)
        //{
        //    int sampleCount = count / 2; // 16-bit samples
        //    for (int i = 0; i < sampleCount; i++)
        //    {
        //        short sampleValue = (short)(waveSamples[position] * short.MaxValue);
        //        buffer[offset + i * 2] = (byte)(sampleValue & 0xFF);
        //        buffer[offset + i * 2 + 1] = (byte)((sampleValue >> 8) & 0xFF);
        //        position++;
        //        if (position >= waveSamples.Length)
        //        {
        //            return i * 2;
        //        }
        //    }
        //    return count;
        //}


        public int Read(byte[] buffer, int offset, int count)
        {
            int sampleCount = count / 4; // 32-bit samples

            //byte[] byteArray = new byte[waveSamples.Length * sizeof(float)];
            Buffer.BlockCopy(waveSamples, offset, buffer, 0, buffer.Length);

            return count;
        }

        //public int Read(byte[] buffer, int offset, int count)
        //{
        //    int sampleCount = count / 4; // 32-bit samples
        //    for (int i = 0; i < sampleCount; i++)
        //    {
        //        float sampleValue = waveSamples[position];
        //        //float floatValue = BitConverter.ToInt32(BitConverter.GetBytes(sampleValue), 0);
        //        float floatValue = BitConverter.ToSingle(BitConverter.GetBytes(sampleValue), 0);
        //        Buffer.BlockCopy(BitConverter.GetBytes(sampleValue), 0, buffer, offset + i * 4, 4);
        //        position++;
        //        if (position >= waveSamples.Length)
        //        {
        //            return i * 4;
        //        }
        //    }
        //    return count;
        //}


    }

}
