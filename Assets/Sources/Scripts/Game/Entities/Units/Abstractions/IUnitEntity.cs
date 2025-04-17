using Potman.Game.Entities.Abstractions;
using Potman.Game.Entities.Units.Data;
using UnityEngine;

namespace Potman.Game.Entities.Units.Abstractions
{
    public interface IUnitEntity : IRuntimeEntity
    {
        Transform Root { get; } // TODO temp
        UnitConfig Config { get; }
        IUnitMovement Movement { get; }
    }
}