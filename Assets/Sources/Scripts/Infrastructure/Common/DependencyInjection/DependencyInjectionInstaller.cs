using Zenject;

namespace Potman.Infrastructure.Common.DependencyInjection
{
    public class DependencyInjectionInstaller : Installer<DependencyInjectionInstaller>
    {
        public override void InstallBindings()
        {
            Container
                .BindInterfacesTo<ZenjectServiceProvider>()
                .AsSingle()
                .CopyIntoAllSubContainers();

            Container
                .BindInterfacesTo<ZenjectAbstractFactory>()
                .AsSingle()
                .CopyIntoAllSubContainers();
        }
    }
}