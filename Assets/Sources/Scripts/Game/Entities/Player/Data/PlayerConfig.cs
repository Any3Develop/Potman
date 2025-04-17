using System.Collections.Generic;
using Potman.Common.ResourceManagament.Attributes;
using Potman.Game.Context.Data.Spawn;
using Potman.Game.Entities.Data;
using Potman.Game.Stats.Data;
using Potman.Game.Stats.Utils;
using UnityEngine;

namespace Potman.Game.Entities.Player.Data
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Potman/Entities/PlayerConfig")]
    public class PlayerConfig : EntityConfig
    {
        [Header("General")]
        [Tooltip("Which scenario code will execute with this config.")]
        public PlayerId id;
        
        [Tooltip("Player entity prefab id.")]
        [SerializeField, ResourceAssetId(typeof(PlayerEntity))] public string entityPrefabId;
        
        [Tooltip("Player base stats.")]
        public List<StatData> stats;
        
        [Space, Header("Spawn")]
        [Tooltip("Provide how to select from the list of PositionIds a next spawn id.")]
        public FunctionSelector spawnFunction;

        [Tooltip("At what points can player of this config spawn?")]
        public List<SpawnId> spawnIds = new();
        
#if UNITY_EDITOR
        [ContextMenu(nameof(ResetStats))]
        private void ResetStats()
        {
            stats = new List<StatData>
            {
                new() {type = StatType.Level, value = 1, useMin = true, min = 1},
                new() {type = StatType.Heath, value = 100, useMin = true, min = 0},
                new() {type = StatType.MoveSpeed, value = 250/10f},
                new() {type = StatType.MoveTurnSpeed, value = 500/10f},
                new() {type = StatType.MoveAcceleration, value = 1000/10f},
                new() {type = StatType.MoveDumpingFactor, value = 95/10f},
            };
            
            OnValidate();
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        protected void Reset()
        {
            ResetStats();
        }

        protected void OnValidate()
        {
            stats.OnValidateStats();
        }
#endif
    }
}