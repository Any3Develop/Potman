using Potman.Lobby.AbilityTree.Data.Upgrades;

namespace Potman.Lobby.AbilityTree.Data
{
    public class AbilityNode
    {
        /// <summary>
        /// Uniq identifier of the node.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// A graph identifier that specifies which graph this node belongs to.
        /// </summary>
        public string GraphId { get; set; }

        /// <summary>
        /// Path or URL of the sprite or texture for this node.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Should be opened nodes before will open this one.
        /// </summary>
        public AbilityLinkNode[] Depends { get; set; }

        /// <summary>
        /// Costs of a node to open it.
        /// </summary>
        public CostModel[] Costs { get; set; }

        /// <summary>
        /// Upgradable models will be given to the player as a reward.
        /// </summary>
        public UpgradeModel[] Upgrades { get; set; }
    }
}