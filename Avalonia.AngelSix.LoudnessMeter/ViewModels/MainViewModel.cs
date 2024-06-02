using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Avalonia.AngelSix.LoudnessMeter.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _boldTitle = "AVALONIA";

        [ObservableProperty]
        private string _regularTitle = "LOUDNESS METER";

        [ObservableProperty]
        private bool _isOpenChannelConfigurationList = false;

        [RelayCommand]
        private void ChannelConfigurationButtonPressed()
            => IsOpenChannelConfigurationList ^= true;
    }
}
