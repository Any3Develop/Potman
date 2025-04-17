using System;
using System.Linq;
using Potman.Common.Collections;
using Potman.Common.Pools.Abstractions;
using Potman.Game.Stats.Abstractions;
using Potman.Game.Stats.Data;

namespace Potman.Game.Stats
{
    public class StatsCollection : RuntimeCollection<IRuntimeStat>, IStatsCollection
    {
        private readonly IPool<IRuntimeStat> pool;
        private readonly IStatFactory statFactory;

        public StatsCollection(IPool<IRuntimeStat> pool, IStatFactory statFactory)
        {
            this.pool = pool;
            this.statFactory = statFactory;
        }

        public IRuntimeStat AddNew(StatData data)
        {
            var stat = statFactory.Create(data);
            Add(stat);
            return stat;
        }

        public bool TryGet(StatType type, out IRuntimeStat result) => base.TryGet(x => x.Type == type, out result);
        public IRuntimeStat Get(StatType type) => Find(x => x.Type == type);
        public bool Contains(StatType type) => this.Any(x => x.Type == type);

        public override void RemoveAt(int index) => Remove(this.ElementAt(index));

        public override int RemoveAll(Predicate<IRuntimeStat> predicate)
        {
            var toRemove = this.Where(predicate.Invoke).ToArray();
            foreach (var runtimeStat in toRemove)
                Remove(runtimeStat);
            
            return toRemove.Length;
        }

        public override bool Remove(IRuntimeStat value)
        {
            if (value == null)
                return base.Remove(null);
                    
            pool.Release(value);
            return base.Remove(value);
        }

        public override void Clear()
        {
            ForEach(pool.Release);
            base.Clear();
        }
    }
}