using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.AngelSix.LoudnessMeter.DataModels;
using Avalonia.Threading;
using ManagedBass;
using NWaves.Signals;
using NWaves.Utils;

namespace Avalonia.AngelSix.LoudnessMeter.Services
{
    /// <summary>
    /// Audio capture service
    /// </summary>
    public partial class BassAudioCaptureService : IAudioCaptureService, IDisposable
    {
        private int _device;                                    // - The device Id we want to capture
        private int _handle;                                    // - The handle to the device we want to capture
        private int _captureFrequency = 44100;                  // - The frequency to capture at 
        private byte[] _buffer;                                 // - The buffer for a short capture of microphone audio

        private Queue<double> _lufs = new();                    // - The last few sets of captureed audio bytes, converted to LUFS


        //public event DataAvailableHandler DataAvailable;


        /// <summary>
        /// CTOR (Initialize the audio capture service and starts capturing the specified device ID)
        /// </summary>
        /// <param name="device"></param>
        public BassAudioCaptureService(int deviceId = 0, int frequency = 44100)
        {
            _device = deviceId;

            // Initialize and start
            Bass.Init();
            Bass.RecordInit(_device);

            // Start recording (but in a paused state)
            _handle = Bass.RecordStart(frequency, 2, BassFlags.RecordPause, AudioChunkCaptured);

            //// Output all devices, then select one
            //var deviceList = RecordingDevice.Enumerate();
            //foreach (var device in deviceList)
            //{
            //    System.Diagnostics.Debug.WriteLine($"{device?.Index}: {device?.Name}");
            //}

            //var outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Audio\\");
            //Directory.CreateDirectory(outputPath);
            //var filePath = Path.Combine(outputPath, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "wav");
            //using var writer = new WaveFileWriter(new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read), new WaveFormat());
        }


        public void CaptureDefaultInput()
        {
        }


        #region Channel Configuration Methods

        #endregion // Channel Configuration Methods



        //###################################################################################################
        #region IAudioCaptureService 

        public event Action<AudioChunkData> AudioChunkAvailable;

        public Task<List<ChannelConfigurationItem>> GetChannelConfigurationsAsync()
            => Task.FromResult(new List<ChannelConfigurationItem>(new[]
            {
            new ChannelConfigurationItem("Mono Stereo Configuration", "Mono", "Mono"),
            new ChannelConfigurationItem("Mono Stereo Configuration", "Stereo", "Stereo"),
            new ChannelConfigurationItem("5.1 Surround", "5.1 DTS - (L, R, Ls, Rs, C, LFE)", "5.1 DTS"),
            new ChannelConfigurationItem("5.1 Surround", "5.1 DTS - (L, R, C, LFE, Ls, Rs)", "5.1 ITU"),
            new ChannelConfigurationItem("5.1 Surround", "5.1 DTS - (L, C, R, Ls, Rs, LFE)", "5.1 FILM"),
            }));


        public void Start()
            => Bass.ChannelPlay(_handle);


        public void Stop()
            => Bass.ChannelStop(_handle);

        #endregion // IAudioCaptureService



        //###################################################################################################
        #region ChatGptCode

        public void CaptureDefaultInputChatGpt()
        {
            // Initialize BASS
            if (!Bass.Init(-1, 44100, DeviceInitFlags.Default))
            {
                Console.WriteLine("Failed to initialize BASS");
                return;
            }

            // Initialize the recording device
            if (!Bass.RecordInit(-1))
            {
                Console.WriteLine("Failed to initialize recording device");
                Bass.Free();
                return;
            }

            // Set up the recording callback
            RecordProcedure recordProc = new RecordProcedure(RecordingCallback);

            // Start recording
            int recordingHandle = Bass.RecordStart(44100, 2, BassFlags.Default, recordProc, IntPtr.Zero);
            if (recordingHandle == 0)
            {
                Console.WriteLine("Failed to start recording");
                Bass.RecordFree();
                Bass.Free();
                return;
            }

            // Inform the user that recording has started
            Console.WriteLine("Recording started. Press Enter to stop...");
            Console.ReadLine();

            // Free the recording channel
            Bass.ChannelStop(recordingHandle);
            Bass.StreamFree(recordingHandle);

            // Free BASS resources
            Bass.RecordFree();
            Bass.Free();
        }


        /// <summary>
        /// Callback function for recording
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private static bool RecordingCallback(int handle, IntPtr buffer, int length, IntPtr user)
        {
            // Process the audio data here
            byte[] data = new byte[length];
            Marshal.Copy(buffer, data, 0, length);

            // For example, print the first 10 bytes of the buffer
            Console.WriteLine("Audio Data: " + BitConverter.ToString(data, 0, Math.Min(10, data.Length)));

            return true; // Continue recording
        }

        #endregion // ChatGptCode



        //##################################################################################################
        #region IDisposable 
        public void Dispose()
        {
            Bass.CurrentRecordingDevice = _device;
            Bass.RecordFree();
        }

        #endregion // IDisposable


        /// <summary>
        /// Call back from the audio recording, to process each chunk of audio data
        /// </summary>
        /// <param name="handle">The device handle</param>
        /// <param name="buffer">The chunk of audio data</param>
        /// <param name="length">The length of the audio data chunk</param>
        /// <param name="user"></param>
        /// <returns></returns>
        private bool AudioChunkCaptured(int handle, IntPtr buffer, int length, IntPtr user)
        {
            if (_buffer == null || _buffer.Length < length)
                _buffer = new byte[length];

            Marshal.Copy(buffer, _buffer, 0, length);

            // Calculate usefull information from this chunk
            CalculateValues(_buffer);

            return true;
        }


        /// <summary>
        /// Calculate usable information from an audio chunk
        /// </summary>
        /// <param name="buffer">The audio buffer</param>
        private void CalculateValues(byte[] buffer)
        {
            //System.Diagnostics.Debug.WriteLine(BitConverter.ToString(buffer));

            // Get total PCM16 samples in this buffer (16 bits per sample)
            var sampleCount = buffer.Length / 2;

            // Create our Discrete Signal ready to the filled with information
            var signal = new DiscreteSignal(_captureFrequency, sampleCount);

            // Loop all bytes and extract the 16 bits into signal floats
            using var reader = new BinaryReader(new MemoryStream(buffer));

            for (int i = 0; i < sampleCount; i++)
            {
                signal[i] = reader.ReadInt16() / 32768f;
            }

            // Calcilate te LUFS
            var lufs = Scale.ToDecibel(signal.Rms());
            _lufs.Enqueue(lufs);

            if (_lufs.Count > 10)
            {
                _lufs.Dequeue();
            }

            // Calculate the average
            var averageLufs = _lufs.Average();

            // Fire off this chunk of information to listeners
            AudioChunkAvailable?.Invoke(new AudioChunkData(
                // TODO: Make this calculation correct
                ShortTermLufs: averageLufs,
                Loudness: lufs,
                LoudnessRange: lufs + 0.9,
                RealTimeDynamics: lufs + 0.8,
                AverageRealTimeDynamics: lufs + 0.7,
                TruePeakMax: lufs + 0.6,
                IntegratedLufs: lufs + 0.5,
                MomentaryMaxLufs: lufs + 0.4,
                ShortTermMaxLufs: lufs + 0.3
                ));
        }
    }
}
