using Potman.Common.Inventory.Abstractions;

namespace Potman.Lobby.AbilityTree.Data
{
    public class AbilityProgress : IInventoryItem
    {
        public string Id { get; set; }
        public string GraphId { get; set; }
    }
}