using Potman.Common.Pools.Abstractions;

namespace Potman.Game.Entities.Abstractions
{
    public interface IRuntimeEntityView : IPoolable
    {
        IEntityMapper Mapper { get; }
    }
}