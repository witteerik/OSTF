// N.B. This is an AndroidAudioTrackPlayer class file, never used but needed in order to compile the real Android verison of the player along with non-androd code.

using STFN.Audio;
using STFN.Audio.Formats;
using STFN.Audio.SoundPlayers;
using STFN.Audio.SoundScene;

using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace STFM
{

    public class MyBackgroundService : IHostedService, IDisposable
    {
        private Timer _timer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Start your background task
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            // Perform your background work here
            Console.WriteLine("Background task is running...");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop your background task
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
    public class AndroidAudioTrackPlayer : STFN.Audio.SoundPlayers.iSoundPlayer, IHostedService, IDisposable
    {
        private bool disposedValue;

        public AndroidAudioTrackPlayer()
        {
            throw new Exception("This class should not be used!");
        }

        bool iSoundPlayer.WideFormatSupport => throw new NotImplementedException();

        bool iSoundPlayer.RaisePlaybackBufferTickEvents { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        bool iSoundPlayer.EqualPowerCrossFade { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        bool iSoundPlayer.IsPlaying => throw new NotImplementedException();

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

        void iSoundPlayer.ChangePlayerSettings(ref AudioApiSettings AudioApiSettings, int? SampleRate, int? BitDepth, WaveFormat.WaveFormatEncodings? Encoding, double? OverlapDuration, ref DuplexMixer Mixer, iSoundPlayer.SoundDirections? SoundDirection, bool ReOpenStream, bool ReStartStream, bool? ClippingIsActivated)
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

        int iSoundPlayer.GetOverlapGranuality()
        {
            throw new NotImplementedException();
        }

        Sound iSoundPlayer.GetRecordedSound(bool ClearRecordingBuffer)
        {
            throw new NotImplementedException();
        }

        void iSoundPlayer.SetOverlapDuration(double Duration)
        {
            throw new NotImplementedException();
        }

        void iSoundPlayer.SetOverlapGranuality(int Granuality)
        {
            throw new NotImplementedException();
        }

        Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
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

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

}