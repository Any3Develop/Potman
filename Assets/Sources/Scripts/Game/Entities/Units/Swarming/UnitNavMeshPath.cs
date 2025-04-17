using Potman.Game.Entities.Units.Abstractions.Swarming;
using UnityEngine.AI;

namespace Potman.Game.Entities.Units.Swarming
{
    public class UnitNavMeshPath : IUnitPath
    {
        public int Id { get; private set; } = -1;
        public bool Available { get; private set; }
        public NavMeshPath Value { get; private set; } = new();

        public void Init(int id)
        {
            Id = id;
            Available = true;
        }
        
        public void Release()
        {
            Available = false;
            Value?.ClearCorners();
            Id = -1;
        }

        public void Dispose()
        {
            Release();
            Value = null;
        }
    }
}