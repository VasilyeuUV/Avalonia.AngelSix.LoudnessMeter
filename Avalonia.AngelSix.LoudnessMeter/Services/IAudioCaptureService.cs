using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.AngelSix.LoudnessMeter.DataModels;

namespace Avalonia.AngelSix.LoudnessMeter.Services
{
    public interface IAudioCaptureService
    {
        /// <summary>
        /// A callback for when the next chunk of audio data is available
        /// </summary>
        event Action<AudioChunkData> AudioChunkAvailable;

        /// <summary>
        /// Fetch the channel configuarations
        /// </summary>
        /// <returns></returns>
        Task<List<ChannelConfigurationItem>> GetChannelConfigurationsAsync();

        /// <summary>
        /// Start capturing audio
        /// </summary>
        void Start();

        /// <summary>
        /// Stop capturing audio
        /// </summary>
        void Stop();

        /// <summary>
        /// Initialises capturing of audio from a specific device
        /// </summary>
        /// <param name="deviceId">The device Id</param>
        /// <param name="frequency">The frequency</param>
        void InitCapture(int deviceId = 0, int frequency = 44100);
    }
}
