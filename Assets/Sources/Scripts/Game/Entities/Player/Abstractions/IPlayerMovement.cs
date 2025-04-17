using UnityEngine;

namespace Potman.Game.Entities.Player.Abstractions
{
    public interface IPlayerMovement
    {
        bool Enabled { get; }
        
        void SetPosition(Vector3 value);
        void SetRotation(Quaternion value);
        void Enable(bool value);
    }
}