using System.Collections.Generic;
using System.Threading.Tasks;

namespace Potman.Common.UIService.Abstractions.AudioSource
{
    public interface IUIAudioConfigsProvider
    {
        IEnumerable<IUIAudioConfig> LoadAll();
        Task<IEnumerable<IUIAudioConfig>> LoadAllAsync();
    }
}