using Potman.Common.Inventory.Abstractions;

namespace Potman.Game.Abilities.Data.Upgrades
{
    public class UpgradeProgress : IInventoryItem
    {
        public string Id { get; set; }
        public string RuntimeId { get; set; }
    }
}