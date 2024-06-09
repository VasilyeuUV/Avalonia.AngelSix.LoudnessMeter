using Avalonia.AngelSix.LoudnessMeter.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;

namespace Avalonia.AngelSix.LoudnessMeter.Views
{
    public partial class MainView : UserControl
    {
        private Control _mainGrid;
        private Control _channelConfigPopup;
        private Control _channelConfigButton;

        /// <summary>
        /// The main view model of this view
        /// </summary>
        private MainViewModel _mainViewModel => (MainViewModel)DataContext!;


        public MainView()
        {
            InitializeComponent();

            _mainGrid = this.FindControl<Control>("MainGrid")
                ?? throw new System.Exception("Cannot find Main Grid by name");
            _channelConfigButton = this.FindControl<Control>("ChannelConfigurationButton")
                ?? throw new System.Exception("Cannot find Channel Configuration Button by name");
            _channelConfigPopup = this.FindControl<Control>("ChannelConfigurationPopup")
                ?? throw new System.Exception("Cannot find Channel Configuration Popup by name");
        }

        /// <summary>
        /// ПЕРЕРИСОВКА СТАРТОВОГО ПОЛОЖЕНИЯ POPUP ДЛЯ КНОПКИ ПРИ ИЗМЕНЕНИИ РАЗМЕРА ОКНА.
        /// В поздних версиях Авалонии работает без этого
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="System.Exception"></exception>
        public override void Render(DrawingContext context)
        {
            base.Render(context);

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

            base.OnLoaded(e);
        }


        private void Border_PointerPressed(object? sender, Input.PointerPressedEventArgs e)
            => _mainViewModel.ChannelConfigurationButtonPressedCommand.Execute(null);
    }
}