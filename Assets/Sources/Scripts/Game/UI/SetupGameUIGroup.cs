using System;
using Potman.Common.Audio;
using Potman.Common.LifecycleService.Abstractions;
using Potman.Common.UIService;
using Potman.Common.UIService.Abstractions;
using UnityEngine;

namespace Potman.Game.UI
{
    public class SetupGameUIGroup : IInitable, IDisposable
    {
        private readonly IUIService uiService;
        private readonly IUIAudioListener audioListener;

        public SetupGameUIGroup(IUIService uiService, IUIAudioListener audioListener)
        {
            this.uiService = uiService;
            this.audioListener = audioListener;
        }

        public void Initialize()
        {
            audioListener.Subscribe(uiService.CreateAll(UILayer.GameUIGroup));
            if (Application.isMobilePlatform)
                uiService.Begin<MobileInputWindow>().Show();
        }

        public void Dispose()
        {
            uiService.DestroyAll(UILayer.GameUIGroup);
        }
    }
}