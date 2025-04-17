using Potman.Common.Pools.Abstractions;
using UnityEngine.AI;

namespace Potman.Game.Entities.Units.Abstractions.Swarming
{
    public interface IUnitPath : IPoolable
    {
        int Id { get;}
        bool Available { get; }
        NavMeshPath Value { get; }
    }
}