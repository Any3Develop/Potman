using System;
using Potman.Common.Pools.Abstractions;
using Potman.Game.Stats.Data;

namespace Potman.Game.Stats.Abstractions
{
    public interface IRuntimeStat : IPoolable, IDisposable
    {
        event Action<IRuntimeStat> OnChanged;
        
        float Current { get; }
        float Min { get; }
        float Max { get; }
        StatType Type { get; }
        
        void Set(float value, bool notify = true);
        void SetMax(float value, bool notify = true);
        void SetMin(float value, bool notify = true);
        void Add(float value, bool notify = true);
        void Subtract(float value, bool notify = true);
        void SetToMax(bool notify = true);
    }
}