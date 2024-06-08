using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.AngelSix.LoudnessMeter.DataModels;

namespace Avalonia.AngelSix.LoudnessMeter.Services;

public class DummyAudioInterfaceService : IAudioInterfaceService
{
    public Task<List<ChannelConfigurationItem>> GetChannelConfigurationsAsync()
        => Task.FromResult(new List<ChannelConfigurationItem>(new[]
        {
            new ChannelConfigurationItem(Group: "Momo Stereo Configuration", LongText: "Mono", ShortText: "Mono"),
            new ChannelConfigurationItem(Group: "Momo Stereo Configuration", LongText: "Stereo", ShortText: "Stereo"),
            new ChannelConfigurationItem(Group: "5.1 Surround", LongText: "5.1 DTS - (L, R, Ls, Rs, C, LFE)", ShortText: "5.1 DTS"),
            new ChannelConfigurationItem(Group: "5.1 Surround", LongText: "5.1 DTS - (L, R, C, LFE, Ls, Rs)", ShortText: "5.1 ITU"),
            new ChannelConfigurationItem(Group: "5.1 Surround", LongText: "5.1 DTS - (L, R, Ls, Rs, C, LFE)", ShortText: "5.1 FILM"),
        }));
}
