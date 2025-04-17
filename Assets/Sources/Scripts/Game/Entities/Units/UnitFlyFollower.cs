using System;
using Potman.Game.Entities.Abstractions;
using Potman.Game.Entities.Units.Abstractions;
using Potman.Game.Stats.Abstractions;
using Potman.Game.Stats.Data;
using Potman.Game.Stats.Utils;
using R3;
using UnityEngine;

namespace Potman.Game.Entities.Units
{
    [RequireComponent(typeof(IUnitEntity))]
    [DisallowMultipleComponent]
    public class UnitFlyFollower : MonoBehaviour, IMovementModifier, IEntityComponent
    {
        private IUnitEntity entity;
        private IDisposable subscriptions;
        private Transform origin;
        private Transform steering;
        private Transform root;

        public bool Enabled => !IsDisposed && enabled;
        public bool IsDisposed { get; private set; }
        [field: SerializeField] public float SpeedDumping { get; private set; } = 50f;
        [field: SerializeField] public float TurnDumping { get; private set; } = 50f;
        [field: SerializeField] public float Altitude { get; private set; } = 10f;

        public void Init()
        {
            if (entity != null)
                return;
            
            entity = GetComponent<IUnitEntity>();
            root = entity.Mapper.RecursiveMap<IRuntimeEntityView>().Mapper.Map<Transform>("Root");
            steering = entity.Mapper.Map<Transform>("Root");
            origin = root.parent;
            entity.Movement.AddModifier(this);
            Enable(false);
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;
            entity.Movement?.RemoveModifier(this);
            subscriptions?.Dispose();
            subscriptions = null;
            steering = null;
            entity = null;
            root = null;
        }

        private void OnDestroy() => Dispose();
        
        public IMovementModifier SetPosition(Vector3 value)
        {
            if (!Enabled)
                return this;

            root.position = value;
            return this;
        }
        
        public IMovementModifier Enable(bool value)
        {
            if (IsDisposed || Enabled == value)
                return this;

            enabled = value;
            if (Enabled)
            {
                root.SetParent(null);
                using var builder = new DisposableBuilder();
                var stats = entity.StatsCollection;
                builder.Add(stats.SubscribeStat(StatType.FlyDumping, stat => SpeedDumping = CalcDumping(stat, StatType.MoveSpeed)));
                builder.Add(stats.SubscribeStat(StatType.FlyAltitude, stat => Altitude = stat.Current));
                builder.Add(stats.SubscribeStat(StatType.FlyTrunDumping, stat => TurnDumping = CalcDumping(stat, StatType.MoveTurnSpeed)));
                subscriptions = builder.Build();
                return this;
            }

            root.SetParent(origin);
            root.localPosition = Vector3.zero;
            subscriptions?.Dispose();
            subscriptions = null;
            return this;
        }
        
        private float CalcDumping(IRuntimeStat dumpingStat, StatType baseType)
        {
            if (!Enabled || dumpingStat == null || !entity.StatsCollection.TryGet(baseType, out var baseStat))
                return 0;

            return baseStat.Current * dumpingStat.Current;
        }

        private void LateUpdate()
        {
            if (!Enabled)
                return;

            var targetPos = steering.position;
            targetPos.y = Altitude;
            var delta = Time.deltaTime;
            var targetRot = Quaternion.LookRotation(steering.forward);
            var currRot = root.rotation;

            var maxAngleDifference = Quaternion.Angle(currRot, targetRot);
            var t = Mathf.Min(1f, TurnDumping * Time.deltaTime / maxAngleDifference);
            root.position = Vector3.Lerp(root.position, targetPos, SpeedDumping * delta);
            root.rotation = Quaternion.Slerp(currRot, targetRot, t);
        }
    }
}