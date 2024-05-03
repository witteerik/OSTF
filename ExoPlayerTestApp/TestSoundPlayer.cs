//// This code is a drastically stripped and modified version of the Plugin.Maui.Audio only for use with Android, above verison 22.

//using System.Runtime.Versioning;
//using Android.Content;
//using Android.Content.Res;
//using Android.Media;
//using Stream = System.IO.Stream;
//using Uri = Android.Net.Uri;

//namespace ExoPlayerTestApp;

//[SupportedOSPlatform("Android23.0")]
//public class AudioPlayer 
//{
//    public event EventHandler? PlaybackEnded;

//    readonly MediaPlayer player;
//    double volume = 0.5;
//    double balance = 0;
//    readonly MemoryStream? stream;
//    bool isDisposed = false;

//    public double Duration => player.Duration <= -1 ? -1 : player.Duration / 1000.0;

//    public double CurrentPosition => player.CurrentPosition / 1000.0;

//    public double Volume
//    {
//        get => volume;
//        set => SetVolume(volume = value, Balance);
//    }

//    public double Balance
//    {
//        get => balance;
//        set => SetVolume(Volume, balance = value);
//    }

//    public double Speed
//    {
//        get => player.PlaybackParams.Speed;
//        set
//        {
//                // Speed on Android can be between 0 and 6
//                var speedValue = Math.Clamp((float)value, 0.0f, 6.0f);

//                if (float.IsNaN(speedValue))
//                {
//                    speedValue = 1.0f;
//                }

//                player.PlaybackParams = player.PlaybackParams.SetSpeed(speedValue) ?? player.PlaybackParams;
//        }
//    }

//    public double MinimumSpeed => 0;

//    public double MaximumSpeed => 6;

//    public bool IsPlaying => player.IsPlaying;

//    public bool Loop
//    {
//        get => player.Looping;
//        set => player.Looping = value;
//    }

//    public bool CanSeek => true;

//    internal AudioPlayer(Stream audioStream)
//    {
//        player = new MediaPlayer();

//        player.SetAudioAttributes(new AudioAttributes.Builder().SetFlags(AudioFlags.None).SetUsage(AudioUsageKind.Media).SetLegacyStreamType( Android.Media.Stream.Music).Build());
                
//        var audioManager = Android.App.Application.Context.GetSystemService(Context.AudioService) as Android.Media.AudioManager;
//        audioManager?.SetStreamVolume(Android.Media.Stream.Music, 1, VolumeNotificationFlags.PlaySound);
//        var pars = audioManager?.GetParameters("");

//        var rate = audioManager?.GetProperty(Android.Media.AudioManager.PropertyOutputSampleRate);
//        var rate2 = audioManager?.GetProperty(Android.Media.AudioManager.PropertyOutputFramesPerBuffer);
//        var rate3 = audioManager?.GetProperty(Android.Media.AudioManager.ActionHeadsetPlug);
//        var rate4 = audioManager?.GetProperty(Android.Media.AudioManager.PropertySupportAudioSourceUnprocessed);

//        //Android.Media.AudioDeviceType.

//        //var apc = audioManager?.ActivePlaybackConfigurations;

//        //var devInf = new Android.Media.AudioDeviceInfo(player,);

//        //player.SetPreferredDevice(  )

//         //player.SetAudioAttributes(new AudioFormat.Builder().SetEncoding(Encoding.PcmFloat).Build());

//        //audioManager.ClearPreferredMixerAttributes()

//        //player.SetAudioAttributes(new AudioAttributes.Builder().SetFlags(AudioFlags.AudibilityEnforced).SetLegacyStreamType(Android.Media.Stream.Music).SetUsage(AudioUsageKind.Game).SetContentType(AudioContentType.Music).Build());
//        // player.Reset();

//        player.Completion += OnPlaybackEnded;

//            stream = new MemoryStream();
//            audioStream.CopyTo(stream);
//            var mediaDataSource = new StreamMediaDataSource(stream);

//        // Create AudioFormat
//        AudioFormat format = new AudioFormat.Builder().SetEncoding(Encoding.PcmFloat).Build();

//        player.SetDataSource(mediaDataSource);
//        player.Prepare();


//    }

//    public void Play()
//    {
//        if (IsPlaying)
//        {
//            Pause();
//            Seek(0);
//        }
//        player.Start();
//    }

//    public void Stop()
//    {
//        if (!IsPlaying)
//        {
//            return;
//        }

//        Pause();
//        Seek(0);
//        PlaybackEnded?.Invoke(this, EventArgs.Empty);
//    }

//    public void Pause()
//    {
//        player.Pause();
//    }

//    public void Seek(double position)
//    {
//        player.SeekTo((int)(position * 1000D));
//    }

//    void SetVolume(double volume, double balance)
//    {
//        volume = Math.Clamp(volume, 0, 1);

//        balance = Math.Clamp(balance, -1, 1);

//        // Using the "constant power pan rule." See: http://www.rs-met.com/documents/tutorials/PanRules.pdf
//        var left = Math.Cos((Math.PI * (balance + 1)) / 4) * volume;
//        var right = Math.Sin((Math.PI * (balance + 1)) / 4) * volume;

//        player.SetVolume((float)left, (float)right);
//    }

//    void OnPlaybackEnded(object? sender, EventArgs e)
//    {
//        PlaybackEnded?.Invoke(this, e);

//        //this improves stability on older devices but has minor performance impact
//        // We need to check whether the player is null or not as the user might have dipsosed it in an event handler to PlaybackEnded above.
//        if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.M)
//        {
//            player.SeekTo(0);
//            player.Stop();
//            player.Prepare();
//        }
//    }

//    protected virtual void Dispose(bool disposing)
//    {
//        if (isDisposed)
//        {
//            return;
//        }

//        if (disposing)
//        {
//            player.Completion -= OnPlaybackEnded;
//            player.Release();
//            player.Dispose();
//            stream?.Dispose();
//        }

//        isDisposed = true;
//    }

//    public void Dispose()
//    {

//        Dispose(true);

//        GC.SuppressFinalize(this);

//    }

//}


//class StreamMediaDataSource : Android.Media.MediaDataSource
//{
//    Stream data;

//    public StreamMediaDataSource(Stream data)
//    {
//        this.data = data;
//    }

//    public override long Size => data.Length;

//    public override int ReadAt(long position, byte[]? buffer, int offset, int size)
//    {
//        ArgumentNullException.ThrowIfNull(buffer);

//        if (data.CanSeek)
//        {
//            data.Seek(position, SeekOrigin.Begin);
//        }

//        return data.Read(buffer, offset, size);
//    }

//    public override void Close()
//    {
//        data.Dispose();
//        data = Stream.Null;
//    }

//    protected override void Dispose(bool disposing)
//    {
//        base.Dispose(disposing);

//        data.Dispose();
//        data = Stream.Null;
//    }
//}


////#if ANDROID
////using Android.Content;
////using Android.Views;
////using Android.Runtime;

////#elif IOS
////using UIKit;
////#endif


////using Microsoft.Maui.Controls.PlatformConfiguration;
////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Text;
////using System.Threading.Tasks;
////using ExoPlayerTestApp;
////using Stream = System.IO.Stream;


////namespace MauiPlayground.Services
////{
////    internal class PlayAudioService
////    {
////        protected MediaPlayer player;
////        public void StartPlayer(Stream stream)
////        {
////            if (player == null)
////            {
////                player = new MediaPlayer();
////                player.SetAudioAttributes(new AudioAttributes.Builder().SetFlags(AudioFlags.AudibilityEnforced)
////                    .SetLegacyStreamType(Android.Media.Stream.Music).SetUsage(AudioUsageKind.Game).SetContentType(AudioContentType.Music).Build());
////                player.Reset();
////                player.SetDataSource(new MyMediaDataSource(stream));
////                player.Prepare();
////                player.Start();
////            }
////        }

////        public void Pause()
////        {
////            player.Pause();
////        }

////        public void Stop()
////        {
////            player.Stop();
////        }
////    }

////    class MyMediaDataSource : MediaDataSource
////    {
////        Stream AudioStream;
////        public MyMediaDataSource(Stream stream)
////        {
////            AudioStream = stream;
////        }

////        public override long Size => AudioStream != null ? AudioStream.Length : 0;

////        public override void Close()
////        {
////            AudioStream?.Close();
////        }

////        public override int ReadAt(long position, byte[] buffer, int offset, int size)
////        {
////            if (position >= AudioStream.Length) return -1; // -1 indicates EOF 

////            AudioStream.Position = position;
////            return AudioStream.Read(buffer, offset, size);
////        }
////    }
////}



