using System;
using Potman.Game.Stats.Data;
using UnityEngine;

namespace Potman.Lobby.AbilityTree.Data.Upgrades
{
    [Serializable]
    public class UpgradeStatModel : UpgradeModel
    {
        [Tooltip("Operation of upgrade:\n" +
                 "Addition - total + value,\n"+
                 "Subtract - total - value,\n"+
                 "Multiply - total * value,\n" +
                 "Divide - total / value,\n" +
                 "Percent - total + ((total / 100) * value),\n" +
                 "You can select multiple values.")]
        public UpgradeStatOp func = UpgradeStatOp.Addition;
        [Tooltip("Target stat, identifier.")]
        public StatType stat;
        [Tooltip("Stat value to upgrade or downgrade.")]
        public float value;
    }
}