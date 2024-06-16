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
        private int _upgradeCoubter = 3;                                            // - A slow tick counter to upgrade the text slower then the graphs and bars

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
        [ObservableProperty] private double _volumeContainerHeight;
        [ObservableProperty] private double _volumeBarHeight;
        [ObservableProperty] private double _volumeBarMaskHeight;


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


            //// Temp code to move volume position 

            //var tick = 0;
            //var input = 0.0;

            //var tempTimer = new DispatcherTimer
            //{
            //    Interval = System.TimeSpan.FromSeconds(1 / 60.0),
            //};

            //tempTimer.Tick += (s, e) =>
            //{
            //    // Slow down ticks
            //    input = ++tick / 20f;

            //    // Scale value
            //    var scale = VolumeContainerHeight / 2f;

            //    VolumePercentPosition = (Math.Sin(input) + 1) * scale;
            //};

            //tempTimer.Start();
        }


        /// <summary>
        /// Start capturing audio from the specified device
        /// </summary>
        /// <param name="deviceId">The device ID</param>
        private void StartCapture(int deviceId)
        {
            _audioCaptureService = new BassAudioCaptureService(/*deviceId*/);

            // Listen out for chunk of information
            _audioCaptureService.AudioChunkAvailable += audioChunkData =>
            {
                ProcessAudioChunk(audioChunkData);
            };

            // Start capturing
            _audioCaptureService.Start();
        }

        private void ProcessAudioChunk(AudioChunkData audioChunkData)
        {
            // Counter between 0-1-2
            _upgradeCoubter = (_upgradeCoubter + 1) % 3;

            // Every time counter is at 0...
            if (_upgradeCoubter == 0)
            {
                ShortTermLoudness = $"{Math.Max(-60, audioChunkData.ShortTermLufs):0.0} LUFS";
                IntegratedLoudness = $"{Math.Max(-60, audioChunkData.IntegratedLufs):0.0} LUFS";
                LoudnessRange = $"{Math.Max(-60, audioChunkData.LoudnessRange):0.0} LU";
                RealTimeDynamics = $"{Math.Max(-60, audioChunkData.RealTimeDynamics):0.0} LU";
                AverageDynamics = $"{Math.Max(-60, audioChunkData.AverageRealTimeDynamics):0.0} LU";
                MomentaryMaxLoudness = $"{Math.Max(-60, audioChunkData.MomentaryMaxLufs):0.0} LUFS";
                ShortTermMaxLoudness = $"{Math.Max(-60, audioChunkData.ShortTermMaxLufs):0.0} LUFS";
                TruePeakMax = $"{Math.Max(-60, audioChunkData.TruePeakMax):0.0} dB";
            }

            // Set volume bar height
            VolumeBarMaskHeight = Math.Min(_volumeBarHeight, _volumeBarHeight / 60 * -audioChunkData.ShortTermLufs);

            // Set Volume Arrow height
            VolumePercentPosition = Math.Min(_volumeContainerHeight, _volumeContainerHeight / 60 * -audioChunkData.IntegratedLufs);
        }

        #endregion // PRIVATE METHODS
    }
}
