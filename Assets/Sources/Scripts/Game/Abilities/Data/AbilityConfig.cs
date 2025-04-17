using System.Collections.Generic;
using Potman.Common.ResourceManagament.Attributes;
using Potman.Game.Abilities.Views;
using Potman.Game.Stats.Data;
using Potman.Game.Stats.Utils;
using UnityEngine;

namespace Potman.Game.Abilities.Data
{
    [CreateAssetMenu(fileName = "AbilityConfig", menuName = "Potman/Abilities/AbilityConfig")]
    public class AbilityConfig : ScriptableObject
    {
        [Header("General")]
        [SerializeField, ResourceAssetId(typeof(AbilityBaseView))] public List<string> prefabIds = new();
        
        [Tooltip("The localization identifier for the ability title.")]
        public string titleId = "Title Id";
        
        [Tooltip("The localization identifier for the ability description.")]
        public string descriptionId = "Description Id";
        
        [Tooltip("A unique identifier or name, path or link to the image for UI.")]
        public string artId = "Image Id";
        
        [Tooltip("The unique identifier of the ability.")]
        public AbilityId id;
        
        [Tooltip("How the ability will be manipulated by the player and the game's environment.")]
        public AbilityType type;
        
        [Tooltip("How the ability will affect the chosen targets.")]
        public AbilityAffect affect;
        
        [Tooltip("Affects the type of ability life time reduction.")]
        public AbilityExpire expire;
        
        [Tooltip("Controls the possibility of stack abilities.")]
        public AbilityStack stack;
        
        [Tooltip("Basic unique ability characteristics.")]
        public List<StatData> stats;
        

        [Header("Targets")] 
        [Tooltip("The number of targets is the minimum to trigger the ability. 0 disabled."), Min(0)]
        public int minTargets;
        
        [Tooltip("Number of maximum targets allowed for the ability. 0 disabled."), Min(0)]
        public int maxTargets;
        
        [Tooltip("A way of selecting targets.")]
        public AbilityTargeting targeting;
        
        [Tooltip("Determines the final number of targets.")]
        public TargetAoeFilter aoeFilter;
        
        [Tooltip("Filtering based on distance from the ability owner.")]
        public TargetDistanceFilter distanceFilter;
        
        [Tooltip("Filtering depending on the owner of the ability.")]
        public TargetOwnerFilter ownerFilter;

#if UNITY_EDITOR
        [ContextMenu(nameof(ResetStats))]
        private void ResetStats()
        {
            stats = new List<StatData>
            {
                new() {type = StatType.Range, value = 100},
                new() {type = StatType.Countdown, value = 0.25f},
                new() {type = StatType.Damage, value = 50},
                new() {type = StatType.MoveSpeed, value = 100},
                new() {type = StatType.LifeTime, value = -1},
            };
            
            OnValidate();
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        private void OnValidate()
        {
            stats.OnValidateStats();
        }
#endif
    }
}