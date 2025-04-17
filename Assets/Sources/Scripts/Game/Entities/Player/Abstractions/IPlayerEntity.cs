using Potman.Common.InputSystem.Abstractions;
using Potman.Game.Entities.Abstractions;
using Potman.Game.Entities.Player.Data;
using UnityEngine;

namespace Potman.Game.Entities.Player.Abstractions
{
    public interface IPlayerEntity : IRuntimeEntity
    {
        Transform Root { get; }
        PlayerConfig Config { get; }
        IPlayerMovement Movement { get; }
        IPlayerAnimator Animator { get; }
        IAimPositioning AimPositioning { get; }
        IInputController<PlayerActions> Input { get; }
    }
}