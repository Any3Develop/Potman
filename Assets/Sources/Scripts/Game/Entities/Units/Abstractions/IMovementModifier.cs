using UnityEngine;

namespace Potman.Game.Entities.Units.Abstractions
{
    public interface IMovementModifier
    {
        IMovementModifier SetPosition(Vector3 value);
        IMovementModifier Enable(bool value);
    }
}