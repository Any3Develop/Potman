using Potman.Common.Collections;
using Potman.Game.Stats.Data;

namespace Potman.Game.Stats.Abstractions
{
    public interface IStatsCollection : IRuntimeCollection<IRuntimeStat>
    {
        IRuntimeStat AddNew(StatData data);
        bool TryGet(StatType type, out IRuntimeStat result);
        IRuntimeStat Get(StatType type);
        bool Contains(StatType type);
    }
}