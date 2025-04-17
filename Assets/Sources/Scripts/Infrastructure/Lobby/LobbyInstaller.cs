using Potman.Common.SceneService;
using Potman.Game.Scenarios.Data;
using Potman.Infrastructure.Common.Inventory;
using Potman.Lobby;
using Potman.Lobby.AbilityTree;
using Potman.Lobby.UI;
using Potman.Lobby.UI.GameModes;
using Zenject;

namespace Potman.Infrastructure.Lobby
{
    public class LobbyInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .BindInterfacesTo<SetupLobbyUIGroup>()
                .AsSingle()
                .NonLazy();

            Container
                .BindInterfacesTo<AbilityTreeCollection>()
                .AsSingle();

            Container
                .BindInterfacesTo<AbilityTreeApplication>()
                .AsSingle();

            Container
                .BindInterfacesTo<GameModesViewModel>()
                .AsSingle()
                .WithArguments(new GameModeData[]
                {
                    new(SceneId.Graveyard, ScenarioId.Default, "Default"),
                    new(SceneId.Graveyard, ScenarioId.Graveyard, "Graveyard"),
                });
            
            Container
                .BindInterfacesTo<BootstrapLobby>()
                .AsSingle()
                .NonLazy();

            InventoryInstaller.Install(Container);
        }
    }
}