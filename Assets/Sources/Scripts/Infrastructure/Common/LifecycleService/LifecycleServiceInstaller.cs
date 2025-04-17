using Zenject;

namespace Potman.Infrastructure.Common.LifecycleService
{
    public class LifecycleServiceInstaller : Installer<LifecycleServiceInstaller>
    {
        public override void InstallBindings()
        {
            Container
                .BindInterfacesTo<ZenjectLifecycleSystem>()
                .AsSingle()
                .CopyIntoAllSubContainers();
        }
    }
}