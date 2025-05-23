﻿using Potman.Common.UIService.AudioSource.Handlers;
using Potman.Common.UIService.Data;
using UnityEngine;

namespace Potman.Common.UIService.AudioSource.Configs
{
    [CreateAssetMenu(fileName = nameof(UISlidersAudioHandler), menuName = "UIService/Audio/" + nameof(UISlidersAudioHandler))]
    public class UISlidersAudioConfig : UIAudioBaseConfig
    {
        [SerializeField] private bool includeDisabled = true;
        [SerializeField] private bool includeInherited = true;
        [SerializeField, Range(0, 1f)] private float gap = 0.1f;
        [SerializeField] private UIAudioClipData scrollAudio;
        
        public bool IncludeDisabled => includeDisabled;
        public bool IncludeInherited => includeInherited;
        public float Gap => gap;
        public UIAudioClipData ScrollAudio => scrollAudio;
    }
}