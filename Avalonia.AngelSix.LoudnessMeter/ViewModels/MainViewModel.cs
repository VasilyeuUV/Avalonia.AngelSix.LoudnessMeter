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
        private IAudioCaptureService _audioCaptureService;                          // - the audio capture service


        //#############################################################################################
        #region ObservableProperties

        [ObservableProperty] private string _boldTitle = "AVALONIA";
        [ObservableProperty] private string _regularTitle = "LOUDNESS METER";
        [ObservableProperty] private string _shortTermLoudness = "0 LUFS";
        [ObservableProperty] private string _integratedLoudness = "0 LUFS";
        [ObservableProperty] private string _loudnessRange = "0 LU";
        [ObservableProperty] private string _realTimeDynamics = "0 LU";
        [ObservableProperty] private string _averageDynamics = "0 LU";
        [ObservableProperty] private string _momentaryMaxLoudness = "0 LUFS";
        [ObservableProperty] private string _shortTermMaxLoudness = "0 LUFS";
        [ObservableProperty] private string _truePeakMax = "0 dB";
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
        /// Default CTOR for design mode
        /// </summary>
        /// <param name="audioInterfaceService">The audio interface service</param>
        public MainViewModel(IAudioCaptureService audioInterfaceService)
        {
            //if (Controls.Design.IsDesignMode)
            //{
            //}

            _audioCaptureService = audioInterfaceService;

            Initialize();
        }


        /// <summary>
        /// Design time constructor
        /// </summary>
        public MainViewModel()
        {
            _audioCaptureService = new BassAudioCaptureService();

            Initialize();
        }
        #endregion // CTOR



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


        /// <summary>
        /// Do initial loading of dataand settings up services
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task LoadAsync()
        {
            // Get the channel configuration data
            var channelConfigurations = await _audioCaptureService.GetChannelConfigurationsAsync();

            // Create a grouping from the flat data
            ChannelConfigurations
                = new ObservableGroupedCollection<string, ChannelConfigurationItem>(channelConfigurations.GroupBy(item => item.Group));

            StartCapture(1);
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


        /// <summary>
        /// Start capturing audio from the specified device
        /// </summary>
        /// <param name="deviceId">The device ID</param>
        private void StartCapture(int deviceId)
        {
            _audioCaptureService = new BassAudioCaptureService(deviceId);

            // Listen out for chunk of information
            _audioCaptureService.AudioChunkAvailable += audioChunkData =>
            {
                ShortTermLoudness = $"{audioChunkData.ShortTermLufs:0.0} LUFS";
                IntegratedLoudness = $"{audioChunkData.IntegratedLufs:0.0} LUFS";
                LoudnessRange = $"{audioChunkData.LoudnessRange:0.0} LU";
                RealTimeDynamics = $"{audioChunkData.RealTimeDynamics:0.0} LU";
                AverageDynamics = $"{audioChunkData.AverageRealTimeDynamics:0.0} LU";
                MomentaryMaxLoudness = $"{audioChunkData.MomentaryMaxLufs:0.0} LUFS";
                ShortTermMaxLoudness = $"{audioChunkData.ShortTermMaxLufs:0.0} LUFS";
                TruePeakMax = $"{audioChunkData.TruePeakMax:0.0} dB";
            };

            // Start capturing
            _audioCaptureService.Start();
        }

        #endregion // PRIVATE METHODS
    }
}
