using System.Collections.Generic;
using Potman.Lobby.AbilityTree.Data.Upgrades;
using UnityEngine;

namespace Potman.AbilityTree.Upgrades
{
    [NodeWidth(375)]
    public class UpgradeStatNode : UpgradeNode
    {
        [Tooltip("Choose stats to upgrade or downgrade.")]
        public UpgradeStatModel[] Stats;
        public override IEnumerable<UpgradeModel> Upgrades => Stats;
    }
}