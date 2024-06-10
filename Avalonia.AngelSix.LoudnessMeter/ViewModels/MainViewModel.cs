using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.AngelSix.LoudnessMeter.DataModels;
using Avalonia.AngelSix.LoudnessMeter.Services;
using Avalonia.Threading;
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
        [ObservableProperty] private bool _isOpenChannelConfigurationList = false;
        [ObservableProperty] private ObservableGroupedCollection<string, ChannelConfigurationItem> _channelConfigurations = default!;

        [ObservableProperty] private double _volumePercentPosition;
        [ObservableProperty] private double _volumeContainerSize;

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

            Initialize();
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

            Initialize();
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



        //#############################################################################################
        #region PRIVATE METHODS

        private void Initialize()
        {
            // Temp code to move volume position 

            var tick = 0;
            var input = 0.0;

            var tempTimer = new DispatcherTimer
            {
                Interval = System.TimeSpan.FromSeconds(1 / 60.0),
            };

            tempTimer.Tick += (s, e) =>
            {
                // Slow down ticks
                input = ++tick / 20f;

                // Scale value
                var scale = VolumeContainerSize / 2f;

                VolumePercentPosition = (Math.Sin(input) + 1) * scale;
            };

            tempTimer.Start();
        }

        #endregion // PRIVATE METHODS
    }
}
