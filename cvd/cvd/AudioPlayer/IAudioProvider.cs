using System;
using System.Threading.Tasks;
using System.Windows.Input;
using cvd.Spectrum;

namespace cvd.AudioPlayer
{
    public interface IAudioProvider : ISpectrumProvider
    {
        TimeSpan Duration { get; set; }

        TimeSpan Position { get; set; }

        double PlaybackSpeed { get; set; }

        double Volume { get; set; }

        float CurrentVolumePeek { get; set; }

        ICommand PlayCommand { get; }

        ICommand PauseCommand { get; }

        string CurrentPlayingFile { get; set; }

        void Stop();

        void Pause();

        Task Play();

        bool IsPlaying { get; set; }

    }
}
