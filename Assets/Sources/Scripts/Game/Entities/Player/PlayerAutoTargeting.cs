using System.Linq;
using Potman.Game.Entities.Abstractions;
using Potman.Game.Entities.Player.Abstractions;
using Potman.Game.Entities.Player.Data;
using Potman.Game.Entities.Units.Abstractions;
using UnityEngine;

namespace Potman.Game.Entities.Player
{
    [RequireComponent(typeof(IPlayerEntity))]
    [DisallowMultipleComponent]
    public class PlayerAutoTargeting : MonoBehaviour, IAutoTargeting, IEntityComponent
    {
        [SerializeField] private float testRange = 50f;
        [SerializeField] private float refreshTime = 1.5f;
        private IPlayerEntity entity;
        private IRuntimePool runtimePool;
        private IRuntimeEntity target;
        private float lastUpdate;
        private Transform root;
        
        public bool Enabled => enabled;
        
        public void Init()
        {
            entity = GetComponent<IPlayerEntity>();
            runtimePool = entity.GameContext.ServiceProvider.GetRequiredService<IRuntimePool>();
            root = entity.Root;
        }

        public void Dispose()
        {
            if (entity == null)
                return;
            
            entity.AimPositioning.SetTarget(null);
            entity = null;
        }
        
        public void Enable(bool value)
        {
            if (Enabled == value)
                return;

            enabled = value;
        }

        private void LateUpdate()
        {
            if (!Enabled || entity.AimPositioning.IsControlledByUser)
                return;
            
            if (target is {IsAlive: true} && Time.time < lastUpdate)
            {
                if (entity.AimPositioning.IsAutoControll)
                    entity.Input.Get(PlayerActions.Attack).Perform(1f);
                return;
            }
            
            lastUpdate = Time.time + refreshTime;
            var currPos = root.position;
            var lastBest = float.MaxValue;
            foreach (var unitEntity in runtimePool.Active.OfType<IUnitEntity>())
            {
                var curr = Vector3.Distance(unitEntity.Root.position, currPos);
                if (curr < lastBest && curr <= testRange)
                {
                    lastBest = curr;
                    target = unitEntity;
                }
            }
            entity.AimPositioning.SetTarget(target);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.RadiusHandle(transform.rotation, transform.position, testRange);
        }
#endif
    }
}