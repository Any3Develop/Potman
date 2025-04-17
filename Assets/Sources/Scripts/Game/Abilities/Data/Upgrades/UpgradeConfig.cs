using System.Collections.Generic;
using Potman.Game.Common.Data;
using Potman.Game.Stats.Data;
using Potman.Game.Stats.Utils;
using Potman.Lobby.AbilityTree.Data.Upgrades;
using UnityEngine;

namespace Potman.Game.Abilities.Data.Upgrades
{
    [CreateAssetMenu(fileName = "UpgradeConfig", menuName = "Potman/Abilities/UpgradeConfig")]
    public class UpgradeConfig : ConfigBase
    {
        [Header("General")] 
        [Tooltip("What tier does this upgrade take? The higher the tier, the more it increases the odds depending on the player's luck.")]
        public UpgradeTier tier;
        
        [Tooltip("The localization id for the upgrade title.")]
        public string titleId = "Title Id";
        
        [Tooltip("The localization id for the upgrade description.")]
        public string descriptionId = "Description Id";
        
        [Tooltip("A unique id or name, path or link to the image for UI.")]
        public string artId = "Image Id";
        
        [Space, Header("Balance")] 
        [Tooltip("What's the chance of getting this upgrade? Where 0 is never, where 100 is always."), Range(0,100)]
        public float chance;
        
        [Tooltip("The number of points to purchase this upgrade."), Min(0)]
        public int buyPrice;
        
        [Tooltip("The number of points will be earned if you sell this upgrade."), Min(0)]
        public int sellPrice;

        [Space, Header("Upgrade")] 
        [Tooltip("You will receive this ability or its upgrade upon purchase.")]
        public AbilityId abilityId;

        [Tooltip("You will receive stats upgrade for the selected ability upon purchase. Optional.")]
        public List<StatData> upgradeStats = new();

        [Space, Header("Conditions")] 
        [Tooltip("Operation of upgrade:\n" +
                 "Add - a new ability, must be in play,\n" +
                 "Remove - exists ability, must be in play\n" +
                 "Union - add or update exists ability.")]
        public UpgradeAbilityOp upgradeFunction = UpgradeAbilityOp.Union; 
        
        [Tooltip("Starts : When the game reached the level greater or equal this value will start to be available this config.\n\n" +
                 "Ends : When the game reached the level greater or equal this value, then won't be available this config.\n\n" +
                 "Each : When each time the game reached the level equal this value will be available this config.")]
        public Condition whenLevelUp;
        
#if UNITY_EDITOR
        protected void Reset()
        {
            validateTier = (UpgradeTier) (-1);
            OnValidate();
        }

        [HideInInspector, SerializeField] private UpgradeTier validateTier;
        protected void OnValidate()
        {
            whenLevelUp.ClampMax(-1);
            if (validateTier != tier)
            {
                validateTier = tier;
                chance = 100 - (int) tier;
            }
            upgradeStats.OnValidateStats();
        }
#endif
    }
}