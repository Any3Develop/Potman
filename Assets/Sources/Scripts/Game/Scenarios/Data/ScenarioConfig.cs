using System.Linq;
using Potman.Common.ResourceManagament.Attributes;
using Potman.Game.Context.Data.Spawn;
using UnityEngine;

namespace Potman.Game.Scenarios.Data
{
    [CreateAssetMenu(fileName = "ScenarioConfig", menuName = "Potman/Scenarios/ScenarioConfig")]
    public class ScenarioConfig : ScriptableObject
    {
        [Header("General")]
        [Tooltip("Which scenario code will execute with this config.")]
        public ScenarioId id;
        
        [Tooltip("Level as uniq id of the scenario config, to chose from many of configs."), Min(0)]
        public int level;

        [Tooltip("Basic configurations of the scenario, the context of the game.")]
        public ContextConfig context = new();

        [Space, Header("Providers")]
        [Tooltip("Available units and their conditions of occurrence for this scenario.")]
        public SpawnConfig[] unitsScenario;

        [Tooltip("Available object and their conditions of occurrence for this scenario.")]
        public SpawnConfig[] objectsScenario;

        [Tooltip("Available effects and their conditions of occurrence for this scenario.")]
        public SpawnConfig[] effectsScenario;

        [Tooltip("Scenario spawn points prefab id.")]
        [SerializeField, ResourceAssetId(typeof(GameObject))] public string spawnPrefabId;
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            var targetName = $"ScenarioConfig_{id}_{level}";
            if (targetName != name)
            {
                var guids = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(ScenarioConfig)}");
                var duplicates = guids
                    .Select(guid => UnityEditor.AssetDatabase.LoadAssetAtPath<ScenarioConfig>(UnityEditor.AssetDatabase.GUIDToAssetPath(guid)))
                    .Count(so => so && so != this && so.name == targetName);

                if (duplicates > 0)
                {
                    Debug.LogError($"{name} : Каждый конфиг сценария должен быть с уникальным названием, выбери {nameof(id)} и {nameof(level)} которых еще нет в прокте, название файла будет автоматически переименовано.");
                }
                else
                {
                    var path = UnityEditor.AssetDatabase.GetAssetPath(this);
                    UnityEditor.AssetDatabase.RenameAsset(path, targetName);
                    UnityEditor.AssetDatabase.SaveAssets();
                    Debug.Log($"Файл переименован : {targetName}");
                }
            }
        }
#endif
    }
}