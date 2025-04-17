using Potman.Common.Collections;
using Potman.Common.Pools;
using Potman.Game.Abilities;
using Potman.Game.Abilities.Abstractions;
using Potman.Game.Abilities.Data.Upgrades;
using Potman.Game.Abilities.Upgrades;
using Potman.Game.Context;
using Potman.Game.Context.Spawn;
using Potman.Game.Entities;
using Potman.Game.Entities.Abstractions;
using Potman.Game.Entities.Player;
using Potman.Game.Entities.Units;
using Potman.Game.Scenarios;
using Potman.Game.Stats;
using Potman.Game.Stats.Abstractions;
using Potman.Game.UI;
using Potman.Game.UI.AbilityList;
using Potman.Game.UI.GameStatistics;
using Potman.Infrastructure.Common.Inventory;
using Zenject;

namespace Potman.Infrastructure.Game
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            InstallUI();
            InstallFactories();
            InstallPools();
            InstallContext();
            InstallEntryPoint();
        }

        private void InstallFactories()
        {
            Container
                .BindInterfacesTo<ScenarioFactory>()
                .AsSingle()
                .NonLazy();

            Container
                .BindInterfacesTo<AbilityFactory>()
                .AsSingle()
                .NonLazy();

            Container
                .BindInterfacesTo<AbilityViewFactory>()
                .AsSingle();
            
            Container
                .BindInterfacesTo<UnitEntityFactory>()
                .AsSingle();

            Container
                .BindInterfacesTo<StatFactory>()
                .AsSingle();

            Container
                .BindInterfacesTo<PlayerEntityFactory>()
                .AsSingle();
        }

        private void InstallPools()
        {
            Container
                .BindInterfacesTo<RuntimePool<IRuntimeStat>>()
                .AsSingle();
            
            Container
                .BindInterfacesTo<RuntimePool<IRuntimeEntityView>>()
                .AsSingle();
            
            Container
                .BindInterfacesTo<RuntimePool<IAbilityView>>()
                .AsSingle();
            
            Container
                .BindInterfacesTo<RuntimeEntityPool>()
                .AsSingle();
        }

        private void InstallContext()
        {
            Container
                .BindInterfacesTo<StatsCollection>()
                .AsTransient();
            
            Container
                .BindInterfacesTo<AbilityCollection>()
                .AsTransient();
            
            Container
                .BindInterfacesTo<PositionProvider>()
                .AsSingle();
            
            Container
                .BindInterfacesTo<SpawnPointProvider>()
                .AsSingle();
            
            Container
                .BindInterfacesTo<GameContext>()
                .AsSingle();
            
            Container
                .BindInterfacesTo<EntityMapperMock>()
                .AsSingle();
            
            Container
                .BindInterfacesTo<UpgradeSystem>()
                .AsSingle();
            
            Container
                .Bind<IRuntimeCollection<UpgradeConfig>>()
                .To<RuntimeCollection<UpgradeConfig>>()
                .AsSingle();
            
            InventoryInstaller.Install(Container);
        }
        
        private void InstallUI()
        {
            Container
                .BindInterfacesTo<SetupGameUIGroup>()
                .AsSingle()
                .NonLazy();
            
            Container
                .BindInterfacesTo<GameStatisticsViewModel>()
                .AsSingle()
                .NonLazy();
            
            Container
                .BindInterfacesTo<AbilityListViewModel>()
                .AsSingle()
                .NonLazy();
        }
        
        private void InstallEntryPoint()
        {
            Container
                .BindInterfacesTo<ScenarioProcessor>()
                .AsSingle()
                .NonLazy();
        }
    }
}