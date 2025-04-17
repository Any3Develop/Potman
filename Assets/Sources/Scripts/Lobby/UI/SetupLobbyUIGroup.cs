using System;
using Potman.Common.Audio;
using Potman.Common.LifecycleService.Abstractions;
using Potman.Common.UIService;
using Potman.Common.UIService.Abstractions;

namespace Potman.Lobby.UI
{
    public class SetupLobbyUIGroup : IInitable, IDisposable
    {
        private readonly IUIService uiService;
        private readonly IUIAudioListener audioListener;

        public SetupLobbyUIGroup(IUIService uiService, IUIAudioListener audioListener)
        {
            this.uiService = uiService;
            this.audioListener = audioListener;
        }

        public void Initialize()
        {
            audioListener.Subscribe(uiService.CreateAll(UILayer.LobbyUIGroup));
        }

        public void Dispose()
        {
            uiService.DestroyAll(UILayer.LobbyUIGroup);
        }
    }
}