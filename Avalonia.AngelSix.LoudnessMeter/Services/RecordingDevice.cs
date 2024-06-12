using System;
using System.Collections.Generic;
using ManagedBass;

namespace Avalonia.AngelSix.LoudnessMeter.Services
{
    public partial class AudioCaptureService
    {
        public class RecordingDevice : IDisposable
        {
            public int Index { get; init; }
            public string Name { get; init; }

            /// <summary>
            /// CTOR
            /// </summary>
            /// <param name="index"></param>
            /// <param name="name"></param>
            RecordingDevice(int index, string name)
            {
                Index = index;
                Name = name;
            }

            public static IEnumerable<RecordingDevice> Enumerate()
            {
                for (int i = 0; Bass.RecordGetDeviceInfo(i, out var info); ++i)
                    yield return new RecordingDevice(i, info.Name);
            }

            public void Dispose()
            {
                Bass.CurrentRecordingDevice = Index;
                Bass.RecordFree();
            }

            public override string ToString() => Name;
        }
    }
}
