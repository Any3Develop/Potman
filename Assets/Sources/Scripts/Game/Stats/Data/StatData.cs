using System;
using UnityEngine;

namespace Potman.Game.Stats.Data
{
    [Serializable]
    public struct StatData
    {
#if UNITY_EDITOR
        /// <summary>
        /// It's for visual in UnityEditor.Inspector
        /// </summary>
        [HideInInspector] public string name;        
#endif
        
        [Tooltip("Unique identifier of the stat.")]
        public StatType type;
        
        [Tooltip("Value : This value will be the Current component at runtime.")]
        public float value;
        
        [Tooltip("Min : The current component will be clamped to this value.")]
        public float min;
        
        [Tooltip("Max : The current component will be clamped to this value.")]
        public float max;
                
        [Tooltip("Enable to use the Min component.")]
        public bool useMin;
        
        [Tooltip("Enable to use the Max component.")]
        public bool useMax;
    }
}