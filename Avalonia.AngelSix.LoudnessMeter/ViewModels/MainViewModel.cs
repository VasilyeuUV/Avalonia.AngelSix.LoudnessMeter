using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia.AngelSix.LoudnessMeter.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _boldTitle = "AVALONIA";


        [ObservableProperty]
        private string _regularTitle = "LOUDNESS METER";

    }
}
