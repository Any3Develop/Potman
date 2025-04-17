using Potman.Common.Pools.Abstractions;
using Potman.Game.Stats.Abstractions;
using Potman.Game.Stats.Data;

namespace Potman.Game.Stats
{
    public class StatFactory : IStatFactory
    {
        private readonly IPool<IRuntimeStat> pool;
        public StatFactory(IPool<IRuntimeStat> pool)
        {
            this.pool = pool;
        }

        public IRuntimeStat Create(StatData value)
        {
            if (pool.TrySpawn(out RuntimeStat runtimeStat))
            {
                runtimeStat.Init(value);
                return runtimeStat;
            }
            
            runtimeStat = new RuntimeStat();
            runtimeStat.Init(value);
            pool.Add(runtimeStat, true);
            return runtimeStat;
        }
    }
}