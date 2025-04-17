using System;
using Potman.Game.Abilities.Abstractions;
using Potman.Game.Entities.Abstractions;
using UnityEngine;

namespace Potman.Game.Abilities.Views
{
    [RequireComponent(typeof(IAbilityView))]
    [DisallowMultipleComponent]
    public class AbilityMovement : MonoBehaviour, IAbilityMovement, IEntityComponent
    {
        private bool hasSteering;
        private bool hasDirection;
        private bool hasDestination;
        private Vector3 initPosition;
        private Action onRangeLimit;
        private Transform root;

        [field: SerializeField] protected float StopDistance { get; private set; } = 0.1f; // TODO move to stats
        protected IAbilityView View { get; private set; }
        protected float Speed { get; private set; }
        protected float? RangeLimit { get; private set; }
        protected float RangePassed { get; private set; }
        protected float DistanceLeft { get; private set; }
        protected Vector3 Direction { get; private set; }
        protected Vector3 Destination { get; private set; }
        protected Transform Target { get; private set; }
        protected bool Enabled => !IsDisposed && enabled;
        protected bool IsDisposed { get; private set; }
        
        public void Init()
        {
            View = GetComponent<IAbilityView>();
            root = View.Mapper.Map<Transform>("Root");
            Enable(false);
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;
            
            Reset();
            IsDisposed = true;
        }

        private void OnDestroy() => Dispose();

        public void Apply(float speed)
        {
            if (!hasSteering && !hasDirection && !hasDestination)
                throw new InvalidOperationException($"Select one of the following fluid interfaces before {nameof(Apply)}: {nameof(SetSteering)}, {nameof(SetDirection)}, {nameof(SetDestination)}.");

            if (!hasSteering)
                Target = null;

            if (!hasDestination)
                Destination = Vector3.zero;

            if (!hasDirection)
                Direction = Vector3.zero;
            
            Speed = speed;
            Enable(true);
        }

        public void Reset()
        {
            if (IsDisposed)
                return;

            enabled = false;
            Target = null;
            Direction = root.forward;
            Destination = Vector3.zero;
            Speed = RangePassed = 0;
            onRangeLimit = null;
            RangeLimit = null;
            hasSteering = false;
            hasDirection = false;
            hasDestination = false;
        }

        public void Enable(bool value)
        {
            if (Enabled == value)
                return;

            enabled = value && (hasDirection || hasDestination || hasSteering);
        }

        public IAbilityMovement SetPosition(Vector3 value)
        {
            root.position = value;
            return this;
        }

        public IAbilityMovement SetRotation(Quaternion value)
        {
            root.rotation = value;
            return this;
        }

        public IAbilityMovement SetSteering(Transform value)
        {
            enabled = false;
            Target = value;
            hasSteering = true;
            return this;
        }

        public IAbilityMovement SetDirection(Vector3 value)
        {
            enabled = false;
            Direction = value.normalized;
            hasDirection = true;
            return this;
        }

        public IAbilityMovement SetDestination(Vector3 value)
        {
            enabled = false;
            Destination = value;
            hasDestination = true;
            return this;
        }

        public IAbilityMovement SetRangeLimit(float rangeLimit, Action onComplete)
        {
            RangeLimit = rangeLimit;
            onRangeLimit = onComplete;
            return this;
        }

        protected void LateUpdate() // TODO добавить влияние паузы
        {
            if (!Enabled)
                return;

            if (hasDirection)
            {
                root.position += Direction * (Speed * Time.deltaTime);
                ProcessPassedPath();
                return;
            }

            if (hasSteering && Target)
                Destination = Target.position;

            root.position = Vector3.MoveTowards(root.position, Destination, Speed * Time.deltaTime);
            ProcessPassedPath();
        }

        private void ProcessPassedPath()
        {
            var pos = root.position;
            RangePassed = Vector3.Distance(pos, initPosition);
            DistanceLeft = hasDestination ? Vector3.Distance(pos, Destination) : float.PositiveInfinity;
            if (!RangeLimit.HasValue || RangePassed < RangeLimit || DistanceLeft > StopDistance)
                return;

            Enable(false);
            onRangeLimit();
        }
    }
}