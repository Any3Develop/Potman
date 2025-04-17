using System;
using Potman.Game.Stats.Abstractions;
using Potman.Game.Stats.Data;

namespace Potman.Game.Stats
{
    public class RuntimeStat : IRuntimeStat
    {
        private StatData stat;
        private ModifiableFloat current = new();
        private ModifiableFloat min = new();
        private ModifiableFloat max = new();
        
        public event Action<IRuntimeStat> OnChanged;
        public StatType Type => stat.type;
        public float Current => current;
        public float Min => min;
        public float Max => max;

        internal void Init(StatData value)
        {
            stat = value;
            current.Override(CalmpMinMax(stat.value));
            min.Override(stat.min);
            max.Override(stat.max);
        }

        public void Set(float value, bool notify = true)
        {
            var newModifier = CalmpMinMax(value - current.Value);
            if (Math.Abs(newModifier - current.Modifier) < 0.01f)
                return;
            
            current.Set(newModifier);
            OnChangedInvoke(notify);
        }

        public void SetMax(float value, bool notify = true)
        {
            max.Override(value);
            OnChangedInvoke(notify);
        }

        public void SetMin(float value, bool notify = true)
        {
            min.Override(value);
            OnChangedInvoke(notify);
        }

        public void Add(float value, bool notify = true)
        {
            var virtualTotal = CalmpMinMax(current.Total + value); // clamp a new virtual total
            var newValue = virtualTotal - current.Total; // find how much we will add
            if (newValue == 0) // if any changes won't come
                return;
            
            current.Add(newValue); // a new modifier
            OnChangedInvoke(notify);
        }

        public void Subtract(float value, bool notify = true) => Add(-value, notify);

        public void SetToMax(bool notify = true)
        {
            if (current.Modifier == 0)
                return;
            
            current.Set(0);
            OnChangedInvoke(notify);
        }

        public void Release()
        {
            OnChanged = null;
            current?.Set(0);
            min?.Set(0);
            max?.Set(0);
        }
        
        public void Dispose()
        {
            OnChanged = null;
            current = null;
            min = null;
            max = null;
        }

        private void OnChangedInvoke(bool notify)
        {
            if (notify)
                OnChanged?.Invoke(this);
        }

        private float CalmpMinMax(float curr) 
            => Math.Clamp(curr, stat.useMin ? min : float.MinValue, stat.useMax ? max : float.MaxValue);
    }
}