using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Avalonia.AngelSix.LoudnessMeter.Services;
using Avalonia.AngelSix.LoudnessMeter.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using NWaves.Signals;
using NWaves.Utils;

namespace Avalonia.AngelSix.LoudnessMeter.Views
{
    public partial class MainView : UserControl
    {
        private int _captureFrequency = 44100;

        private Control _mainGrid;
        private Control _channelConfigPopup;
        private Control _channelConfigButton;
        private Control _volumeContainer;

        private Timer _sizingTimer;                                         // - The timeout timer to detect when auto sizing has finished firing
        private AudioCaptureService _captureDevice;

        private Queue<double> _lufs = new();


        /// <summary>
        /// The main view model of this view
        /// </summary>
        private MainViewModel _MainViewModel => (MainViewModel)DataContext!;


        /// <summary>
        /// CTOR
        /// </summary>
        /// <exception cref="System.Exception"></exception>
        public MainView()
        {
            InitializeComponent();

            _sizingTimer = new Timer(t =>
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    // Update desired size
                    UpdateSizes();
                });
            });

            _mainGrid = this.FindControl<Control>("MainGrid")
                ?? throw new System.Exception("Cannot find Main Grid by name");
            _channelConfigButton = this.FindControl<Control>("ChannelConfigurationButton")
                ?? throw new System.Exception("Cannot find Channel Configuration Button by name");
            _channelConfigPopup = this.FindControl<Control>("ChannelConfigurationPopup")
                ?? throw new System.Exception("Cannot find Channel Configuration Popup by name");
            _volumeContainer = this.FindControl<Control>("VolumeContainer")
                ?? throw new System.Exception("Cannot find Channel Volume Container by name");
        }


        /// <summary>
        /// œ≈–≈–»—Œ¬ ¿ —“¿–“Œ¬Œ√Œ œŒÀŒ∆≈Õ»ﬂ POPUP ƒÀﬂ  ÕŒœ » œ–» »«Ã≈Õ≈Õ»» –¿«Ã≈–¿ Œ Õ¿.
        /// ¬ ÔÓÁ‰ÌËı ‚ÂÒËˇı ¿‚‡ÎÓÌËË ‡·ÓÚ‡ÂÚ ·ÂÁ ˝ÚÓ„Ó
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="System.Exception"></exception>
        public override void Render(DrawingContext context)
        {
            base.Render(context);

            _sizingTimer.Change(100, int.MaxValue);

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                // Get relative position of button, in relation to main grid
                var position = _channelConfigButton.TranslatePoint(new Point(), _mainGrid)
                    ?? throw new System.Exception("Cannot get TranslatePoint from Configuration Button");

                // Set margin of popup so it appears bottom left of button
                Dispatcher.UIThread.Post(() =>
                {
                    _channelConfigPopup.Margin = new Thickness(
                        position.X,
                        0,
                        0,
                        _mainGrid.Bounds.Height - position.Y - _channelConfigButton.Bounds.Height);
                });
            });
        }


        protected override async void OnLoaded(RoutedEventArgs e)
        {
            await _MainViewModel.LoadSettingsCommand.ExecuteAsync(null);

            StartCapture(1);

            base.OnLoaded(e);
        }


        private void StartCapture(int deviceId)
        {
            _captureDevice = new AudioCaptureService(deviceId, _captureFrequency);
            _captureDevice.DataAvailable += (buffer, length) =>
            {
                CalculateValues(buffer);
            };
            _captureDevice.Start();
        }


        private void CalculateValues(byte[] buffer)
        {
            //System.Diagnostics.Debug.WriteLine(BitConverter.ToString(buffer));

            // Get total PCM16 samples in this buffer (16 bits per sample)
            var sampleCount = buffer.Length / 2;

            // Create our Discrete Signal ready to the filled with information
            var signal = new DiscreteSignal(_captureFrequency, sampleCount);

            // Loop all bytes and extract the 16 bits into signal floats
            using var reader = new BinaryReader(new MemoryStream(buffer));

            for (int i = 0; i < sampleCount; i++)
            {
                signal[i] = reader.ReadInt16() / 32768f;
            }

            // Calcilate te LUFS
            var lufs = Scale.ToDecibel(signal.Rms());
            _lufs.Enqueue(lufs);

            if (_lufs.Count > 15)
            {
                _lufs.Dequeue();
            }

            var averageLufs = _lufs.Average();

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                _MainViewModel.ShortTermLoudness = $"{averageLufs:0.0} LUFS";
            });
        }


        private void Border_PointerPressed(object? sender, Input.PointerPressedEventArgs e)
            => _MainViewModel.ChannelConfigurationButtonPressedCommand.Execute(null);


        private void UpdateSizes()
        {
            _MainViewModel.VolumeContainerSize = _volumeContainer.Bounds.Height;
        }
    }
}