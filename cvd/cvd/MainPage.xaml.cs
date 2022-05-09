using Plugin.AudioRecorder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using cvd.AudioPlayer;
using cvd.Dsp;
using cvd.Spectrum;
//using Plugin.FilePicker;
//using Plugin.FilePicker.Abstractions;
using SkiaSharp.Views.Forms;
//using Plugin.FilePicker;
//using Plugin.FilePicker.Abstractions;
//using SkiaSharp.Views.Forms;

namespace cvd
{
    public partial class MainPage : ContentPage
    {
        private LineSpectrum _lineSpectrum;

        private IAudioProvider _audioProvider;

        private readonly FftSize fftSize = FftSize.Fft4096;
        AudioRecorderService recorder;
       

        public MainPage()
        {
          
            InitializeComponent();
            recorder = new AudioRecorderService
            {
                StopRecordingAfterTimeout = true,
                TotalAudioTimeout = TimeSpan.FromSeconds(15),
                AudioSilenceTimeout = TimeSpan.FromSeconds(2)
            };
            
            _audioProvider = new BassAudioPlayer();

            Device.StartTimer(TimeSpan.FromSeconds(1f / 60), () =>
            {
                canvasView.InvalidateSurface();
                return true;
            });
        }

      
        async void Record_Clicked(object sender, EventArgs e)
        {
            await RecordAudio();
        }


       
        private void canvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            if (_lineSpectrum != null && _audioProvider.IsPlaying)
            {
                var size = new Size(e.Info.Width, e.Info.Height);
                _lineSpectrum.CreateSpectrumLine(e.Surface, size);
            }
        }




        async Task RecordAudio()
        {
            try
            {
                if (!recorder.IsRecording) //Record button clicked
                {
                    recorder.StopRecordingOnSilence = TimeoutSwitch.IsToggled;

                    RecordButton.IsEnabled = false;
                    PlayButton.IsEnabled = false;

                    //start recording audio
                    var audioRecordTask = await recorder.StartRecording();

                    RecordButton.Text = "Stop Recording";
                    RecordButton.IsEnabled = true;

                    await audioRecordTask;

                    RecordButton.Text = "Record";
                    PlayButton.IsEnabled = true;
                }
                else //Stop button clicked
                {
                    RecordButton.IsEnabled = false;

                    //stop the recording...
                    await recorder.StopRecording();

                    RecordButton.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                //blow up the app!
                throw ex;
            }
        }

        void Play_Clicked(object sender, EventArgs e)
        {
            PlayAudio();
        }

        void PlayAudio()
        {
            try
            {
                var filePath = recorder.GetAudioFilePath();

                if (filePath != null)
                {
                    PlayButton.IsEnabled = false;
                    RecordButton.IsEnabled = false;

                    _audioProvider.Play();
                    _lineSpectrum = new LineSpectrum(fftSize)
                    {
                        SpectrumProvider = _audioProvider,
                        UseAverage = true,
                        BarCount = 200,
                        BarSpacing = 1,
                        IsXLogScale = false,
                        ScalingStrategy = ScalingStrategy.Sqrt,
                        MinimumFrequency = 20,
                        MaximumFrequency = 20000
                    };
                }

            }
            catch (Exception ex)
            {
                //blow up the app!
                throw ex;
            }
        }

        void Player_FinishedPlaying(object sender, EventArgs e)
        {
            PlayButton.IsEnabled = true;
            RecordButton.IsEnabled = true;
        }

    }
}
