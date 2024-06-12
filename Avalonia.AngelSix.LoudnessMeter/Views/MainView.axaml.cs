using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.AngelSix.LoudnessMeter.Services;
using Avalonia.AngelSix.LoudnessMeter.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using ManagedBass;
using static Avalonia.AngelSix.LoudnessMeter.Services.AudioCaptureService;

namespace Avalonia.AngelSix.LoudnessMeter.Views
{
    public partial class MainView : UserControl
    {
        private Control _mainGrid;
        private Control _channelConfigPopup;
        private Control _channelConfigButton;
        private Control _volumeContainer;

        private Timer _sizingTimer;                                         // - The timeout timer to detect when auto sizing has finished firing

        /// <summary>
        /// The main view model of this view
        /// </summary>
        private MainViewModel _mainViewModel => (MainViewModel)DataContext!;


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
        /// ÏÅĞÅĞÈÑÎÂÊÀ ÑÒÀĞÒÎÂÎÃÎ ÏÎËÎÆÅÍÈß POPUP ÄËß ÊÍÎÏÊÈ ÏĞÈ ÈÇÌÅÍÅÍÈÈ ĞÀÇÌÅĞÀ ÎÊÍÀ.
        /// Â ïîçäíèõ âåğñèÿõ Àâàëîíèè ğàáîòàåò áåç ıòîãî
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
            await _mainViewModel.LoadSettingsCommand.ExecuteAsync(null);

            Task.Run(async () =>
            {
                // Output all devices, then select one
                var deviceList = RecordingDevice.Enumerate();
                foreach (var device in deviceList)
                {
                    System.Diagnostics.Debug.WriteLine($"{device?.Index}: {device?.Name}");
                }

                var outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Audio\\");
                Directory.CreateDirectory(outputPath);
                var filePath = Path.Combine(outputPath, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "wav");
                using var writer = new WaveFileWriter(new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read), new WaveFormat());

                using var captureDevice = new AudioCaptureService(0);
                captureDevice.DataAvailable += (buffer, length) =>
                {
                    writer.Write(buffer, length);
                    //System.Diagnostics.Debug.WriteLine(BitConverter.ToString(buffer));
                };
                captureDevice.Start();

                await Task.Delay(5000);

                captureDevice.Stop();

                await Task.Delay(100);

            });

            base.OnLoaded(e);
        }


        private void Border_PointerPressed(object? sender, Input.PointerPressedEventArgs e)
            => _mainViewModel.ChannelConfigurationButtonPressedCommand.Execute(null);


        private void UpdateSizes()
        {
            ((MainViewModel)DataContext).VolumeContainerSize = _volumeContainer.Bounds.Height;
        }
    }
}