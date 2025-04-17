using System.Collections.Generic;
using Potman.Common.UIService.Abstractions;
using Potman.Common.UIService.Data;
using Potman.Common.Utilities;
using UnityEngine;

namespace Potman.Common.Audio
{
    public class UIAudioListener : IUIAudioListener
    {
        private readonly AudioSource audioSource;

        public UIAudioListener()
        {
            audioSource = new GameObject(nameof(AudioSource)).AddComponent<AudioSource>();
            audioSource.volume = 1f;
        }

        public void Subscribe(IEnumerable<IUIWindow> windows)
        {
            windows.Each(x => x.AudioSource.OnPlayAudio += OnPlayerAudio);
        }

        private void OnPlayerAudio(UIAudioClipData data)
        {
            if (data.clip)
                audioSource.PlayOneShot(data.clip);
        }
    }
}