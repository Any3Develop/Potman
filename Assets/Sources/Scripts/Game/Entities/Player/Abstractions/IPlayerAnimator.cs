using UnityEngine;

namespace Potman.Game.Entities.Player.Abstractions
{
    public interface IPlayerAnimator
    {
        bool Enabled { get; }

        void Enable(bool value);
        void SetHands(int type);
        void Recoil();
        void Reload(float time);
        void Action(int type);
        void Move(Vector3 dir);
    }
}