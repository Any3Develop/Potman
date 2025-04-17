using Potman.Common.CameraProvider;
using Potman.Common.InputSystem.Abstractions;
using Potman.Common.Utilities;
using Potman.Game.Entities.Abstractions;
using Potman.Game.Entities.Player.Abstractions;
using Potman.Game.Entities.Player.Data;
using UnityEngine;

namespace Potman.Game.Entities.Player
{
    [RequireComponent(typeof(IPlayerEntity))]
    [DisallowMultipleComponent]
    public class PlayerAimPositioning : MonoBehaviour, IAimPositioning, IEntityComponent
    {
        private IPlayerEntity entity;
        private IInputAction lookAction;
        private IInputAction lookActivation;
        private ICameraProvider cameraProvider;
        private Quaternion currentRotation;
        private IRuntimeEntity targetEntity;
        private Transform target;
        private Transform turret;
        
        public float AimSpeed { get; private set; } = 10f;
        public bool IsAutoControll => !IsControlledByUser && targetEntity?.IsAlive == true;
        public bool IsControlledByUser { get; private set; }
        public bool IsHomeRunning { get; private set; }
        public bool Enabled => !IsDisposed && enabled;
        public bool IsDisposed { get; private set; }

        public void Init()
        {
            if (entity != null)
                return;

            entity = GetComponent<IPlayerEntity>();
            turret = entity.Mapper.RecursiveMap<Transform>("Spine");
            cameraProvider = entity.GameContext.ServiceProvider.GetRequiredService<ICameraProvider>();
            lookAction = entity.Input.Get(PlayerActions.Look);
            lookActivation = entity.Input.Get(PlayerActions.LookActivation);
            Enable(false);
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;
            entity = null;
            lookAction = null;
            lookActivation = null;
            cameraProvider = null;
            turret = null;
        }

        private void OnDestroy() => Dispose();

        public void Enable(bool value)
        {
            if (Enabled == value)
                return;

            enabled = value && turret;

            if (value)
            {
                lookAction ??= entity.Input.Get(PlayerActions.Look);
                lookActivation ??= entity.Input.Get(PlayerActions.LookActivation);
            }
        }

        public void SetTarget(IRuntimeEntity value)
        {
            targetEntity = value;
            target = value != null 
                ? targetEntity.Mapper.RecursiveMap<Transform>("LookTarget") 
                : null;
        }

        private void LateUpdate()
        {
            if (!Enabled)
                return;
            
            if (IsControlledByUser = TryGetInput(out var direction))
            {
                RotateTo(Quaternion.LookRotation(direction));
                return;
            }

            if (IsAutoControll)
            {
                RotateTo(Quaternion.LookRotation(target.position - turret.position));
                return;
            }

            if (IsHomeRunning)
            {
                var defaultRot = turret.rotation;
                RotateTo(defaultRot);
                if (Quaternion.Dot(defaultRot, currentRotation) >= 0.99f)
                    IsHomeRunning = false;
            }
        }

        private void RotateTo(Quaternion value)
        {
            if (!IsHomeRunning)
            {
                currentRotation = turret.rotation;
                IsHomeRunning = true;
            }

            turret.rotation = currentRotation = Quaternion.Slerp(currentRotation, value, AimSpeed * Time.deltaTime);
        }

        private bool TryGetInput(out Vector3 dir)
        {
            if (!Enabled || lookActivation is not {Enabled: true} || lookActivation.GetControlMagnitude() <= 0f)
            {
                dir = Vector3.zero;
                return false;
            }

            dir = lookAction.ReadValue<Vector2>();
            if (dir.sqrMagnitude <= 0)
                return false;

#if UNITY_EDITOR || UNITY_STANDALONE // IMPORTANT: platforms with a pointer or cursor return a point on the screen. For mobile/joysticks input return the direction vector by default.
            var scrAim = cameraProvider.Camera.WorldToScreenPoint(turret.position); // get origin screen point.
            dir = (dir - scrAim).normalized; // calculate the direction between origin and point on the screen.
#endif
            // Swap Y and Z components to convert screen (X[left/right],Y[up/down]) to (X[left/right],Z[forward/backward]) world direction, and except Y vertical direction.
            dir.Set(y: 0, z: dir.y);
            return true;
        }
    }
}