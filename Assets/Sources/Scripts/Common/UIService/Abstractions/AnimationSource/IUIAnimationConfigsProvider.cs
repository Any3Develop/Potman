using System.Collections.Generic;
using System.Threading.Tasks;

namespace Potman.Common.UIService.Abstractions.AnimationSource
{
    public interface IUIAnimationConfigsProvider
    {
        IEnumerable<IUIAnimationConfig> LoadAll();
        Task<IEnumerable<IUIAnimationConfig>> LoadAllAsync();
    }
}