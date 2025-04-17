using System;
using Potman.Game.Abilities.Data;
using Potman.Game.Context.Abstractions;
using Potman.Game.Entities.Abstractions;
using Potman.Game.Stats.Abstractions;

namespace Potman.Game.Abilities.Abstractions
{
    public interface IAbility : IDisposable
    {
        string Id { get; }
        bool Enabled { get; }
        AbilityType Type { get; }
        AbilityConfig Config { get; }
        IRuntimeEntity Owner { get; }
        IStatsCollection StatsCollection { get; }
        IGameContext GameContext { get;}

        void Enable(bool value);
        bool CanExecute();
        void Execute();
        bool TryStack(IAbility value, bool intergrate);
        bool TryExpire(AbilityExpire value);
    }
}