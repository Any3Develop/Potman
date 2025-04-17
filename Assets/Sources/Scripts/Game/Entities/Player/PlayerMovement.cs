using System;
using Potman.Common.InputSystem.Abstractions;
using Potman.Game.Entities.Abstractions;
using Potman.Game.Entities.Player.Abstractions;
using Potman.Game.Entities.Player.Data;
using Potman.Game.Stats.Data;
using Potman.Game.Stats.Utils;
using R3;
using UnityEngine;

namespace Potman.Game.Entities.Player
{
    [RequireComponent(typeof(IPlayerEntity))]
    [DisallowMultipleComponent]
    public class PlayerMovement : MonoBehaviour, IPlayerMovement, IEntityComponent
    {
        private CharacterController target;
        private Transform targetTransform;
        private IDisposable subscriptions;
        private IInputAction moveAction;
        private IPlayerEntity entity;
        private Vector3 move;
        private bool isPaused;

        public bool Enabled => !IsDisposed && enabled;
        public bool IsDisposed { get; private set; }
        public float Speed { get; private set; }
        public float Acceleration { get; private set; }
        public float DumpingFactor { get; private set; }
        public float TurnSpeed { get; private set; }

        public void Init()
        {
            if (entity != null)
                return;
            
            entity = GetComponent<IPlayerEntity>();
            target = entity.Mapper.Map<CharacterController>();
            moveAction = entity.Input.Get(PlayerActions.Move);
            targetTransform = target.transform;
            Enable(false);
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;
            subscriptions?.Dispose();
            subscriptions = null;
            targetTransform = null;
            moveAction = null;
            entity = null;
            target = null;
        }

        private void OnDestroy() => Dispose();

        public void SetPosition(Vector3 value)
        {
            if (!Enabled)
                return;

            targetTransform.position = value;
        }

        public void SetRotation(Quaternion value)
        {
            if (!Enabled)
                return;

            targetTransform.rotation = value;
        }

        public void Enable(bool value)
        {
            if (Enabled == value)
                return;

            enabled = value && target;
            
            if (Enabled)
            {
                using var builder = Disposable.CreateBuilder();
                var stats = entity.StatsCollection;
                builder.Add(stats.SubscribeStat(StatType.MoveSpeed, stat => Speed = stat.Current));
                builder.Add(stats.SubscribeStat(StatType.MoveAcceleration, stat => Acceleration = stat.Current));
                builder.Add(stats.SubscribeStat(StatType.MoveDumpingFactor, stat => DumpingFactor = stat.Current));
                builder.Add(stats.SubscribeStat(StatType.MoveTurnSpeed, stat => TurnSpeed = stat.Current));
                subscriptions = builder.Build();
                return;
            }

            subscriptions?.Dispose();
            subscriptions = null;
        }

        private void Update()
        {
            if (!Enabled)
                return;
            
            var input = moveAction.ReadValue<Vector2>();
            var hasInput = input.sqrMagnitude > 0;
            
            if (hasInput)
            {
                move = new Vector3(input.x, 0, input.y);
                targetTransform.rotation = Quaternion.Slerp(targetTransform.rotation, Quaternion.LookRotation(move), TurnSpeed * Time.deltaTime);
            }
            else
                move *= DumpingFactor;

            if (!target.isGrounded)
                move.y = -9.81f;

            if (move.sqrMagnitude > 0)
            {
                target.Move(move * Speed * Time.deltaTime);
                entity.Animator.Move(target.velocity);
            }
        }
    }
}