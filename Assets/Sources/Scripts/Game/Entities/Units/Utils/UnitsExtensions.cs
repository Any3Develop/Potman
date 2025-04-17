using System;
using System.Collections.Generic;
using System.Linq;
using Potman.Game.Entities.Units.Data;
using UnityEngine;
using UnityEngine.AI;

namespace Potman.Game.Entities.Units.Utils
{
    public static class UnitsExtensions
    {
        public static int AsAgentId(this UnitMovementType type) => AgentTypeMapCached[type];
        private static Dictionary<UnitMovementType, int> AgentTypeMapCached { get; } = GetAgentTypeMap();

        private static Dictionary<UnitMovementType, int> GetAgentTypeMap()
        {
            var movementTypes = Enum.GetValues(typeof(UnitMovementType)).OfType<UnitMovementType>().ToArray();
            var agentBuildSettings = Enumerable.Range(0, NavMesh.GetSettingsCount())
                .Select(i => (NavMesh.GetSettingsByIndex(i),
                    NavMesh.GetSettingsNameFromID(NavMesh.GetSettingsByIndex(i).agentTypeID)))
                .ToArray();

            var agentTypeIdMap = new Dictionary<UnitMovementType, int>();
            foreach (var (setting, name) in agentBuildSettings)
            {
                var momementType =
                    movementTypes.FirstOrDefault(x => x.ToString().Equals(name, StringComparison.OrdinalIgnoreCase));
                if (momementType == UnitMovementType.Static)
                    continue;

                if (!agentTypeIdMap.TryAdd(momementType, setting.agentTypeID))
                    Debug.LogWarning($"You have duplicate names in NamMesh.Settings : {momementType}");
            }

            return agentTypeIdMap;
        }
    }
}