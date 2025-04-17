using Potman.Game.Stats.Data;

namespace Potman.Game.Stats.Abstractions
{
    public interface IStatFactory
    {
        IRuntimeStat Create(StatData value);
    }
}