using Potman.Common.Inventory;
using Zenject;

namespace Potman.Infrastructure.Common.Inventory
{
    public class InventoryInstaller : Installer<InventoryInstaller>
    {
        public override void InstallBindings()
        {
            Container
                .BindInterfacesTo<InventoryRepository>()
                .AsSingle();

            Container
                .BindInterfacesTo<Potman.Common.Inventory.Inventory>()
                .AsSingle();
        }
    }
}