using System;
using Potman.Game.Abilities.Data;
using UnityEngine;

namespace Potman.Lobby.AbilityTree.Data.Upgrades
{
    [Serializable]
    public class UpgradeAbilityModel : UpgradeModel
    {
        [Tooltip("Target ability identifier to upgrade.")]
        public AbilityId id;
        [Tooltip("Operation of upgrade:\n" +
                 "Add - a new ability,\n" +
                 "Remove - exists ability,\n" +
                 "Union - add or update exists ability.")]
        public UpgradeAbilityOp func;
        public UpgradeStatModel[] stats;
    }
}