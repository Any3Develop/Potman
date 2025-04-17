using Potman.Common.UIService.Abstractions.AudioSource;
using UnityEngine;

namespace Potman.Common.UIService.AudioSource.Configs
{
    public abstract class UIAudioBaseConfig : ScriptableObject, IUIAudioConfig
    {
        [SerializeField] private bool enabledByDefault = true;
        [SerializeField] private bool reInitWhenModified;
        
        public bool EnabledByDefault => enabledByDefault;
        public bool ReInitWhenModified => reInitWhenModified;
    }
}