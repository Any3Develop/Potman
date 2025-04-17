using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Potman.Common.UIService.Abstractions.AnimationSource;
using Potman.Common.UIService.AnimationSource.Configs;
using UnityEngine;

namespace Potman.Common.UIService.AnimationSource
{
    public class UIAnimationsProvider : MonoBehaviour, IUIAnimationConfigsProvider
    {
        [SerializeField] protected List<UIAnimationBaseConfig> configs = new();
        
        public IEnumerable<IUIAnimationConfig> LoadAll()
            => configs;
        
        public Task<IEnumerable<IUIAnimationConfig>> LoadAllAsync() 
            => Task.FromResult<IEnumerable<IUIAnimationConfig>>(configs);

        [ContextMenu("Load All Resources")]
        private void Reset()
        {
            configs = Resources.LoadAll<UIAnimationBaseConfig>(string.Empty).ToList();
        }
    }
}