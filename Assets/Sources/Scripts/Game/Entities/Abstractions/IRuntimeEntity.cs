using System;
using Potman.Common.Pools.Abstractions;
using Potman.Game.Abilities.Abstractions;
using Potman.Game.Context.Abstractions;
using Potman.Game.Stats.Abstractions;

namespace Potman.Game.Entities.Abstractions
{
    public interface IRuntimeEntity : IPoolable, IDisposable
    {
        bool IsAlive { get; }
        IEntityMapper Mapper { get; }
        IStatsCollection StatsCollection { get; }
        IAbilityCollection AbilityCollection { get; }
        IGameContext GameContext { get; }

        // TODO We can remove this and use some global system to handle incoming damage and stack effects.
        // TODO As a separate logic to relieve entities of this responsibility and lighten their code.
        bool ApplyDamage(float damage);
    }
}