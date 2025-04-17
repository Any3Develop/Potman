using Cysharp.Threading.Tasks;
using Potman.Game.Entities.Units.Data;
using UnityEngine;

namespace Potman.Game.Entities.Units.Abstractions
{
    public interface IUnitEntityFactory
    {
        UniTask<IUnitEntity> CreateAsync(UnitConfig cfg, Vector3 position);
    }
}