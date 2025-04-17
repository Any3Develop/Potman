using Potman.Common.Pools.Abstractions;
using Potman.Game.Entities.Abstractions;

namespace Potman.Game.Abilities.Abstractions
{
    public interface IAbilityView : IPoolable
    { 
        string Id { get; }
        string AbilityId { get; }
        IEntityMapper Mapper { get; }
    }
}