using System.Collections.Generic;
using Potman.Common.ResourceManagament.Attributes;
using Potman.Game.Common.Attributes;
using Potman.Game.Entities.Data;
using Potman.Game.Stats.Data;
using Potman.Game.Stats.Utils;
using UnityEngine;

namespace Potman.Game.Entities.Units.Data
{
    [CreateAssetMenu(fileName = "UnitConfig", menuName = "Potman/Entities/AI/UnitConfig")]
    public class UnitConfig : EntityConfig
    {
        [Header("General")]
        [Tooltip("Player entity prefab id.")]
        [ResourceAssetId(typeof(UnitEntity))]
        public string entityPrefabId;
        
        [Tooltip("Unit base stats.")]
        public List<StatData> stats;
        
        [Space, Header("Behaviours")]
        public UnitClassType classType;
        public UnitBehaviorType behaviorType;
        public UnitMovementType movementType;
        [NavMeshAreaMask] public int walkableAreas;
        public UnitBehaviourSet behavioursSet;

#if UNITY_EDITOR
        [ContextMenu(nameof(ResetStatsForInfantry))]
        private void ResetStatsForInfantry()
        {
            stats = new List<StatData>
            {
                new() {type = StatType.Level, value = 1},
                new() {type = StatType.Heath, value = 100},
                
                new() {type = StatType.MoveSpeed, value = 250/10f},
                new() {type = StatType.MoveTurnSpeed, value = 2500/10f},
                new() {type = StatType.MoveAcceleration, value = 1000/10f},
                
                new() {type = StatType.Priority, value = 100}
            };
            
            OnValidate();
            UnityEditor.EditorUtility.SetDirty(this);
        }
        [ContextMenu(nameof(ResetStatsForAviation))]
        private void ResetStatsForAviation()
        {
            stats = new List<StatData>
            {
                new() {type = StatType.Level, value = 1},
                new() {type = StatType.Heath, value = 100},
                
                new() {type = StatType.MoveSpeed, value = 250/10f},
                new() {type = StatType.MoveTurnSpeed, value = 2500/10f},
                new() {type = StatType.MoveAcceleration, value = 1000/10f},
                new() {type = StatType.FlyAltitude, value = 30},
                new() {type = StatType.FlyDumping, value = 10/10f},
                new() {type = StatType.FlyTrunDumping, value = 10/10f},
                
                new() {type = StatType.Priority, value = 20}
            };
            
            OnValidate();
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        protected void Reset()
        {
            ResetStatsForInfantry();
            OnValidate();
        }

        protected void OnValidate()
        {
            stats.OnValidateStats();
        }
#endif
    }
}