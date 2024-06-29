using System.Threading;
using Avalonia.AngelSix.LoudnessMeter.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;

namespace Avalonia.AngelSix.LoudnessMeter.Views
{
    public partial class MainView : UserControl
    {

        private readonly Control _mainGrid;
        private readonly Control _channelConfigPopup;
        private readonly Control _channelConfigButton;
        private readonly Control _volumeContainer;
        private readonly Control _volumeBar;

        private readonly Timer _sizingTimer;                                    // - The timeout timer to detect when auto sizing has finished firing
        

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
            _volumeBar = this.FindControl<Control>("VolumeBar")
                ?? throw new System.Exception("Cannot find Channel Volume Bar by name");
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


        /// <summary>
        /// Run on-load initialization code
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnLoaded(RoutedEventArgs e)
        {
            await _MainViewModel.LoadCommand.ExecuteAsync(null);

            base.OnLoaded(e);
        }


        private void Border_PointerPressed(object? sender, Input.PointerPressedEventArgs e)
            => _MainViewModel.ChannelConfigurationButtonPressedCommand.Execute(null);


        /// <summary>
        /// Update the application window/control sizes dynamically
        /// </summary>
        private void UpdateSizes()
        {
            _MainViewModel.VolumeContainerHeight = _volumeContainer.Bounds.Height;
            _MainViewModel.VolumeBarHeight = _volumeBar.Bounds.Height;
        }
    }
}