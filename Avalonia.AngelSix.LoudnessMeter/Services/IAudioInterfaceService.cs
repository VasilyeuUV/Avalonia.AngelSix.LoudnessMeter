using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.AngelSix.LoudnessMeter.DataModels;

namespace Avalonia.AngelSix.LoudnessMeter.Services
{
    public interface IAudioInterfaceService
    {
        /// <summary>
        /// Fetch the channel configuarations
        /// </summary>
        /// <returns></returns>
        Task<List<ChannelConfigurationItem>> GetChannelConfigurationsAsync();
    }
}
