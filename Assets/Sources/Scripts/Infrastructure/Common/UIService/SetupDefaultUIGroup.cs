using Potman.Common.Audio;
using Potman.Common.UIService;
using Potman.Common.UIService.Abstractions;
using Zenject;

namespace Potman.Infrastructure.Common.UIService
{
	public class SetupDefaultUIGroup : IInitializable
	{
		private readonly IUIService uiService;
		private readonly IUIAudioListener audioListener;

		public SetupDefaultUIGroup(IUIService uiService, IUIAudioListener audioListener)
		{
			this.uiService = uiService;
			this.audioListener = audioListener;
		}

		public void Initialize()
		{
			audioListener.Subscribe(uiService.CreateAll(UILayer.DefaultUIGroup));
			// uiService.Begin<BackgroundWindow>()
			// 	.WithMove(uiService.UIRoot.ButtomContainer, 0)
			// 	.WithInit(window => window.SetDefaultImage())
			// 	.Show();
		}
	}
}