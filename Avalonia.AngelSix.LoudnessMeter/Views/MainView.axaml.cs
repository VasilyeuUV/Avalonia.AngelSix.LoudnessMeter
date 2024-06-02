using Avalonia.AngelSix.LoudnessMeter.ViewModels;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace Avalonia.AngelSix.LoudnessMeter.Views
{
    public partial class MainView : UserControl
    {
        private Control _mainGrid;
        private Control _channelConfigPopup;
        private Control _channelConfigButton;


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
        }

        private void Border_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
            => ((MainViewModel)DataContext).ChannelConfigurationButtonPressedCommand.Execute(null);
    }
}