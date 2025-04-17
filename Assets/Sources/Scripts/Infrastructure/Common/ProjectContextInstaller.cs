using Potman.Common.CameraProvider;
using Potman.Common.ResourceManagament;
using Potman.Common.SceneService;
using Potman.Common.SceneService.Abstractions;
using Potman.Common.SerializeService;
using Potman.Common.StateMachine;
using Potman.Infrastructure.Common.DependencyInjection;
using Potman.Infrastructure.Common.InputSystem;
using Potman.Infrastructure.Common.LifecycleService;
using Potman.Infrastructure.Common.UIService;
using Potman.Lobby.Identity;
using UnityEngine;
using Zenject;

namespace Potman.Infrastructure.Common
{
    public class ProjectContextInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            DependencyInjectionInstaller.Install(Container);
            LifecycleServiceInstaller.Install(Container);
            InputSystemInstaller.Install(Container);
            UIServiceInstaller.Install(Container);

            Container
                .BindInterfacesTo<ResourceServiceAdapter>()
                .AsSingle()
                .NonLazy();

            Container
                .BindInterfacesTo<AddressableResourceService>()
                .AsSingle();

            Container
                .BindInterfacesTo<GlobalCameraProvider>()
                .AsSingle()
                .NonLazy();

            Container
                .BindInterfacesTo<SceneService>()
                .AsSingle();

            Container
                .BindInterfacesTo<StateMachine>()
                .AsTransient();

            Container
                .BindInterfacesTo<LocalSerilizeService>()
                .AsSingle()
                .NonLazy();

            Container
                .Bind<SerializeHelperAdapter>()
                .AsSingle()
                .NonLazy();

            Container
                .Bind<UserIdentity>()
                .AsSingle()
                .NonLazy();
        }

        public override void Start()
        {
            base.Start();
            Container.Resolve<ISceneService>().OnSceneLoaded += _ => InstallSettings();
        }

        private static void InstallSettings()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
        }
    }
}