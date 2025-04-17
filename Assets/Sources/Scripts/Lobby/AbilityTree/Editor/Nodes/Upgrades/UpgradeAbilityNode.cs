using System.Collections.Generic;
using Potman.Lobby.AbilityTree.Data.Upgrades;
using UnityEngine;

namespace Potman.AbilityTree.Upgrades
{
    [NodeWidth(375)]
    public class UpgradeAbilityNode : UpgradeNode
    {
        [Tooltip("Choose abilities to add new one or upgrade or downgrade.")]
        public UpgradeAbilityModel[] Abilities;
        public override IEnumerable<UpgradeModel> Upgrades => Abilities;
    }
}