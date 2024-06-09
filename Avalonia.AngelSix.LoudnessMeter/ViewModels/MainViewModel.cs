using System.Linq;
using System.Threading.Tasks;
using Avalonia.AngelSix.LoudnessMeter.DataModels;
using Avalonia.AngelSix.LoudnessMeter.Services;
using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Avalonia.AngelSix.LoudnessMeter.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private IAudioInterfaceService _audioInterfaceService;


        //#############################################################################################
        #region ObservableProperties

        [ObservableProperty] private string _boldTitle = "AVALONIA";
        [ObservableProperty] private string _regularTitle = "LOUDNESS METER";
        [ObservableProperty] private bool _isOpenChannelConfigurationList = true;
        [ObservableProperty] private ObservableGroupedCollection<string, ChannelConfigurationItem> _channelConfigurations = default!;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ChannelConfigurationButtonText))]          // - уведомлять указанное свойство при изменении
        private ChannelConfigurationItem? _selectedChannelConfiguration;

        #endregion // ObservableProperties


        public string ChannelConfigurationButtonText => SelectedChannelConfiguration?.ShortText ?? "Select Channel";



        //#############################################################################################
        #region CTOR

        /// <summary>
        /// Design time constructor
        /// </summary>
        public MainViewModel()
        {
            _audioInterfaceService = new DummyAudioInterfaceService();
        }


        /// <summary>
        /// Default CTOR
        /// </summary>
        /// <param name="audioInterfaceService">The audio interface service</param>
        public MainViewModel(IAudioInterfaceService audioInterfaceService)
        {
            //if (Controls.Design.IsDesignMode)
            //{
            //}

            _audioInterfaceService = audioInterfaceService;
        }

        #endregion



        //#############################################################################################
        #region Commands

        [RelayCommand]
        private void ChannelConfigurationButtonPressed()
            => IsOpenChannelConfigurationList ^= true;


        [RelayCommand]
        private void ChannelConfigurationItemPressed(ChannelConfigurationItem item)
        {
            // Update the selected item
            SelectedChannelConfiguration = item;

            // Close the menu
            IsOpenChannelConfigurationList = false;
        }

        [RelayCommand]
        private async Task LoadSettingsAsync()
        {
            // Get the channel configuration data
            var channelConfigurations = await _audioInterfaceService.GetChannelConfigurationsAsync();

            // Create a grouping from the flat data
            ChannelConfigurations
                = new ObservableGroupedCollection<string, ChannelConfigurationItem>(channelConfigurations.GroupBy(item => item.Group));
        }

        #endregion // Commands
    }
}
