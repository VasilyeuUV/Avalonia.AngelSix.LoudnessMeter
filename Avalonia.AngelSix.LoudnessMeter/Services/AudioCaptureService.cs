using System;
using System.IO;
using System.Runtime.InteropServices;
using HarfBuzzSharp;
using ManagedBass;

namespace Avalonia.AngelSix.LoudnessMeter.Services
{

    public delegate void DataAvailableHandler(byte[] buffer, int length);

    /// <summary>
    /// Audio capture service
    /// </summary>
    public partial class AudioCaptureService : IDisposable
    {
        private int _device, _handle;
        private byte[] _buffer;

        public event DataAvailableHandler DataAvailable;


        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="device"></param>
        public AudioCaptureService(int deviceId, int frequency = 44100)
        {
            _device = deviceId;

            Bass.Init();
            Bass.RecordInit(_device);

            _handle = Bass.RecordStart(frequency, 2, BassFlags.RecordPause, Procedure);

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


        public void Start() => Bass.ChannelPlay(_handle);
        public void Stop() => Bass.ChannelStop(_handle);


        public void Dispose()
        {
            Bass.CurrentRecordingDevice = _device;
            Bass.RecordFree();
        }


        private bool Procedure(int handle, IntPtr buffer, int length, IntPtr user)
        {
            if (_buffer == null || _buffer.Length < length)
                _buffer = new byte[length];

            Marshal.Copy(buffer, _buffer, 0, length);
            DataAvailable?.Invoke(_buffer, length);

            return true;
        }
















        /// <summary>
        /// ########################################################################################################
        /// </summary>
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



        // Callback function for recording
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

    }
}
