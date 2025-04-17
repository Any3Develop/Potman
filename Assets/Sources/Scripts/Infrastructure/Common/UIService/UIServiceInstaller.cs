using Potman.Common.Audio;
using Potman.Common.UIService;
using Potman.Common.UIService.AnimationSource.Factories;
using Potman.Common.UIService.AudioSource.Factories;
using Potman.Common.UIService.FullFade;
using Potman.Common.UIService.Options;
using Zenject;

namespace Potman.Infrastructure.Common.UIService
{
	public class UIServiceInstaller : Installer<UIServiceInstaller>
	{
		public override void InstallBindings()
		{
			Container
				.BindInterfacesTo<UIRoot>()
				.FromComponentInNewPrefabResource("UIService/UIRoot")
				.AsSingle()
				.NonLazy();

			Container
				.BindInterfacesTo<UIDefaultService>()
				.AsSingle()
				.WithArguments(UILayer.DefaultUIGroup);

			Container
				.BindInterfacesTo<UIWindowPrototypeProvider>()
				.AsSingle()
				.WithArguments("UIService");

			Container
				.BindInterfacesTo<UIWindowFactory>()
				.AsSingle();

			Container
				.BindInterfacesTo<UIAudioSourceFactory>()
				.AsSingle();

			Container
				.BindInterfacesTo<UIAudioHandlersFactory>()
				.AsSingle();

			Container
				.BindInterfacesTo<UIAnimationSourceFactory>()
				.AsSingle();

			Container
				.BindInterfacesTo<UIAnimationsFactory>()
				.AsSingle();

			Container
				.BindInterfacesTo<UIOptionsFactory>()
				.AsSingle();

			Container
				.BindInterfacesTo<UIServiceRepository>()
				.AsSingle();

			Container
				.BindInterfacesTo<UIFullFadePresenter>()
				.AsSingle();

			Container
				.BindInterfacesTo<UIAudioListener>()
				.AsSingle();

			Container
				.BindInterfacesTo<SetupDefaultUIGroup>()
				.AsSingle()
				.NonLazy();
		}
	}
}