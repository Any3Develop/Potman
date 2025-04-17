using System;
using Potman.Common.SerializableDictionary;
using Unity.Behavior;
using UnityEngine;

namespace Potman.Game.Entities.Units.Data
{
    [CreateAssetMenu(fileName = "UnitBehaviourSet", menuName = "Potman/Entities/AI/UnitBehaviourSet")]
    public class UnitBehaviourSet : ScriptableObject
    {
        [Serializable]
        private class BehaviorsMap : SerializableDictionary<UnitBehaviorType, BehaviorGraph> {}

        [SerializeField] private BehaviorsMap behavioursSet = new();

        public BehaviorGraph Get(UnitBehaviorType value) => behavioursSet[value];
    }
}