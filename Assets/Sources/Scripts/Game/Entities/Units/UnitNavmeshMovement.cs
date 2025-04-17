using System;
using System.Collections.Generic;
using Potman.Common.Utilities;
using Potman.Game.Entities.Abstractions;
using Potman.Game.Entities.Units.Abstractions;
using Potman.Game.Entities.Units.Utils;
using Potman.Game.Stats.Data;
using Potman.Game.Stats.Utils;
using R3;
using UnityEngine;
using UnityEngine.AI;

namespace Potman.Game.Entities.Units
{
    [RequireComponent(typeof(IUnitEntity))]
    [DisallowMultipleComponent]
    public class UnitNavmeshMovement : MonoBehaviour, IUnitMovement, IEntityComponent
    {
        private List<IMovementModifier> modifiers = new();
        private IUnitEntity entity;
        private IDisposable steering;
        private IDisposable subscriptions;
        private NavMeshPath localTestPath;
        private NavMeshAgent navMeshAgent;
        private Transform navTransform;
        private Transform steeringTarget;
        private Vector3 destination;

        #region Properties
        public Vector3 Position => navTransform.position;
        public Quaternion Rotation => navTransform.rotation;
        public bool Enabled => enabled && navMeshAgent && navMeshAgent.enabled;
        public bool IsStopped => !Enabled || navMeshAgent.isStopped;
        public float RemainingDistance => Enabled ? Vector3.Distance(Position, destination) : float.MaxValue;
        
        public float StopDistance
        {
            get => navMeshAgent.stoppingDistance;
            set => navMeshAgent.stoppingDistance = value;
        }
        public float Speed
        {
            get => navMeshAgent.speed;
            set => navMeshAgent.speed = value;
        }
        public float TurnSpeed
        {
            get => navMeshAgent.angularSpeed;
            set => navMeshAgent.angularSpeed = value;
        }
        public float Acceleration
        {
            get => navMeshAgent.acceleration;
            set => navMeshAgent.acceleration = value;
        }
        public int Priority
        {
            get => navMeshAgent.avoidancePriority;
            set => navMeshAgent.avoidancePriority = value;
        }
#endregion

        public void Init()
        {
            if (entity != null)
                return;
            
            entity = GetComponent<IUnitEntity>();
            navMeshAgent = entity.Mapper.Map<NavMeshAgent>();
            navTransform = navMeshAgent.transform;
            destination = Vector3.positiveInfinity;
            Enable(false);
        }

        public void Dispose()
        {
            if (entity == null)
                return;
            
            modifiers = null;
            steering?.Dispose();
            subscriptions?.Dispose();
            subscriptions = null;
            localTestPath = null;
            steeringTarget = null;
            navTransform = null;
            navMeshAgent = null;
            steering = null;
            entity = null;
        }

        private void OnDestroy() => Dispose();

        public void AddModifier(IMovementModifier value) => modifiers.Add(value);
        public void RemoveModifier(IMovementModifier value) => modifiers.Remove(value);

        public void Enable(bool value)
        {
            if (Enabled == value)
                return;

            navMeshAgent.enabled = enabled = value;
            
            if (Enabled)
            {
                OnEnabled();
                Stop();
                return;
            }

            Stop();
            OnDisabled();
        }

        public void SetSteering(Transform target)
        {
            if (!Enabled)
                return;
            
            Stop();
            
            if (!target)
                return;
            
            steeringTarget = target;
            destination = Vector3.positiveInfinity;
            steering = Observable.Interval(TimeSpan.FromSeconds(1/60f*10)).Subscribe(_ =>
            {
                if (steeringTarget)
                    MoveAuto(steeringTarget.position);
            });
        }
        
        public void MoveAuto(Vector3 worldPoint)
        {
            if (!Enabled)
                return;
            
            destination = worldPoint;
            localTestPath ??= new NavMeshPath();
            if (!navMeshAgent.CalculatePath(worldPoint, localTestPath))
                Debug.LogError($"Path is not calculated : {localTestPath.status}");
            
            OnPathChanged(localTestPath);
        }

        public void MoveRelative(Vector3 worldPoint)
        {
            if (!Enabled)
                return;

            destination = worldPoint;
            navMeshAgent.Move(worldPoint - Position);
        }

        public void Move(Vector3 worldPoint)
        {
            if (!Enabled)
                return;

            navMeshAgent.Warp(worldPoint);
        }

        public void Stop()
        {
            steering?.Dispose();
            steering = null;
            
            if (!navMeshAgent.isOnNavMesh)
                return;

            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
        }

        private void OnPathChanged(NavMeshPath unitPath)
        {
            if (!Enabled || unitPath is not {status: NavMeshPathStatus.PathComplete})
            {
                Debug.Log($"OnPathChanged cancelled : Enabled:{Enabled}, Path is not available");
                return;
            }
            
            navMeshAgent.SetPath(unitPath);
            navMeshAgent.isStopped = false;
        }

        private void OnEnabled()
        {
            var config = entity.Config;
            var stats = entity.StatsCollection;

            navMeshAgent.autoRepath = false;
            navMeshAgent.agentTypeID = config.movementType.AsAgentId();
            navMeshAgent.areaMask = config.walkableAreas;
            destination = Vector3.positiveInfinity;
            
            using var eventBuilder = Disposable.CreateBuilder();
            eventBuilder.Add(stats.SubscribeStat(StatType.MoveSpeed, stat => Speed = stat.Current));
            eventBuilder.Add(stats.SubscribeStat(StatType.MoveTurnSpeed, stat => TurnSpeed = stat.Current));
            eventBuilder.Add(stats.SubscribeStat(StatType.MoveAcceleration, stat => Acceleration = stat.Current));
            eventBuilder.Add(stats.SubscribeStat(StatType.Priority, stat => Priority = (int)stat.Current));
            subscriptions = eventBuilder.Build();
            modifiers.Each(x => x.Enable(true).SetPosition(Position));
        }
        
        private void OnDisabled()
        {
            modifiers.Each(x => x.Enable(false));
            steering?.Dispose();
            steering = null;
            subscriptions?.Dispose();
            subscriptions = null;
        }
    }
}