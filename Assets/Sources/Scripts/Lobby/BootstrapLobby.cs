using Potman.Common.Inventory.Abstractions;
using Potman.Common.LifecycleService.Abstractions;
using Potman.Lobby.AbilityTree.Abstractions;
using Potman.Lobby.UI.GameModes;

namespace Potman.Lobby
{
    public class BootstrapLobby : IInitable
    {
        private readonly IAbilityTreeApplication abilityTreeApplication;
        private readonly IGameModesViewModel gameModesViewModel;
        private readonly IInventory inventory;

        public BootstrapLobby(
            IAbilityTreeApplication abilityTreeApplication, 
            IGameModesViewModel gameModesViewModel,
            IInventory inventory)
        {
            this.abilityTreeApplication = abilityTreeApplication;
            this.gameModesViewModel = gameModesViewModel;
            this.inventory = inventory;
        }

        public async void Initialize()
        {
            await inventory.LoadAsync();
            await abilityTreeApplication.LoadAsync();
            await gameModesViewModel.LoadAsync();
        }
    }
}