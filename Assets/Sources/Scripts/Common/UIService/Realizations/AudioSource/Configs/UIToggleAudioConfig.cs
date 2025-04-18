﻿using Potman.Common.UIService.AudioSource.Handlers;
using Potman.Common.UIService.Data;
using UnityEngine;

namespace Potman.Common.UIService.AudioSource.Configs
{
    [CreateAssetMenu(fileName = nameof(UIToggleAudioHandler), menuName = "UIService/Audio/" + nameof(UIToggleAudioHandler))]
    public class UIToggleAudioConfig : UIAudioBaseConfig
    {
        [SerializeField] protected bool includeDisabled = true;
        [SerializeField] protected bool includeInherited = true;
        [SerializeField] protected UIAudioClipData enableAudio;
        [SerializeField] protected UIAudioClipData disableAudio;

        public bool IncludeDisabled => includeDisabled;
        public bool IncludeInherited => includeInherited;
        public UIAudioClipData EnableAudio => enableAudio;
        public UIAudioClipData DisableAudio => disableAudio;
    }
}