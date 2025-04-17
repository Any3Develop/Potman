using System;
using UnityEngine;

namespace Potman.Game.Common.Data
{
    [Serializable]
    public struct Condition
    {
        [Tooltip("Include this condition.")]
        public bool enabled;
        
        [Tooltip("Control how much times this condition can execute, when reached this value it won't execute anymore. -1 is always.")]
        public int repeat;

        [Tooltip("When the condition reached this value it will allow to execute. -1 is disabled.")]
        public int starts;

        [Tooltip("When the condition reached this value it will disallow to execute. -1 is disabled.")]
        public int ends;

        [Tooltip("When each time the condition reached this value then it will allow to execute once. -1 is disabled.")]
        public int each;
        
        [HideInInspector] public int passedLast;
        [HideInInspector] public int repeatLeft;

        public bool IsConditionTrue(int value)
        {
            if (repeatLeft == 0)
                return false;
            
            if (ends != -1 && value >= ends)
                return false;
            
            if (starts != -1 && value < starts)
                return false;

            if (value == passedLast)
                return false;
            
            return each <= 0 || value >= passedLast + each || value % each == 0;
        }
        
        public void Passed(int value)
        {
            if (!enabled)
                return;

            if (repeatLeft > 0) 
                repeatLeft--;
            
            passedLast = value;
        }

        public void Default()
        {
            passedLast = -1;
            repeatLeft = repeat;
        }
        
#if UNITY_EDITOR
        internal void Reset(int to, bool includeRepeat = false)
        {
            if (includeRepeat)
                repeat = to;
            
            starts = to;
            ends = to;
            each = to;
        }
        internal void ClampMax(int value)
        {
            repeat = Math.Max(value, repeat);
            starts = Math.Max(value, starts);
            ends = Math.Max(value, ends);
            each = Math.Max(value, each);
        }

        internal void Clamp(int min, int max, bool includeRepeat = false)
        {
            if (includeRepeat)
                repeat = Math.Clamp(repeat, min, max);

            starts = Math.Clamp(starts, min, max);
            ends = Math.Clamp(ends, min, max);
            each = Math.Clamp(each, min, max);
        }
#endif
    }
}