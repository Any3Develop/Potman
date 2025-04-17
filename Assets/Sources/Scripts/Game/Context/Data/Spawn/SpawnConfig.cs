using System.Collections.Generic;
using Potman.Game.Common.Data;
using Potman.Game.Entities.Data;
using UnityEngine;

namespace Potman.Game.Context.Data.Spawn
{
    [CreateAssetMenu(fileName = "SpawnConfig", menuName = "Potman/Scenarios/SpawnConfig")]
    public class SpawnConfig : ConfigBase
    {
        [Space, Header("Conditions")]
        [Tooltip("Starts : When units with this count died, then will start to spawn this config.\n\n" +
                 "Ends : When units with this count died, then will end to spawn this config.\n\n" +
                 "Each : When each time units with this count died, then will spawn this config.")]
        public Condition whenUnitsDied;

        [Tooltip("Starts : When game time reached this value in seconds, then will start to spawn this config.\n\n" +
                 "Ends : When game time reached this value in seconds, then will end to spawn this config.\n\n" +
                 "Each : When game time each times reached this value in seconds, then will spawn this config.")]
        public Condition whenGameTime;

        [Tooltip(
            "Starts : When game reached the progress in Percents greater or equal this value, then will start to spawn this config.\n\n" +
            "Ends : When game reached the progress in Percents greater or equal this value, then will end to spawn this config.\n\n" +
            "Each : When each time game reached the progress in Percents equal this value, then will spawn this config.")]
        public Condition whenGameProgress;

        [Tooltip(
            "Starts : When the game reached the level greater or equal this value will start to spawn this config.\n\n" +
            "Ends : When the game reached the level greater or equal this value, then will end to spawn this config.\n\n" +
            "Each : When each time the game reached the level equal this value will spawn this config.")]
        public Condition whenLevelUp;
        
        [Space, Header("Spawn")]
        [Tooltip("Provide how to select from the list of PositionIds a next spawn id.")]
        public FunctionSelector spawnFunction;

        [Tooltip("At what points can units of this config spawn?")]
        public List<SpawnId> spawnIds = new();
        
        [Tooltip("Provide how to select from the list a next entityId.")]
        public FunctionSelector idsOrderFunction;
        
        [Tooltip("Entities for this config, they will spawn when this config conditions will true.")]
        public List<EntityConfig> entities = new();
        
#if UNITY_EDITOR
        private void Reset()
        {
            whenUnitsDied.Reset(-1);
            whenGameTime.Reset(-1);
            whenGameProgress.Reset(-1);
            whenGameProgress.Reset(-1);
            whenLevelUp.Reset(-1);
        }

        private void OnValidate()
        {
            whenUnitsDied.ClampMax(-1);
            whenGameTime.ClampMax(-1);
            whenGameProgress.Clamp(-1, 100);
            whenGameProgress.ClampMax(-1);
            whenLevelUp.ClampMax(-1);
        }
#endif
    }
}