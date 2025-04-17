using System;
using Potman.Game.Abilities.Abstractions;
using UnityEngine;

namespace Potman.Game.Abilities.Views
{
    [RequireComponent(typeof(IAbilityMovement))]
    [DisallowMultipleComponent]
    public class AbilityProjectileView : AbilityBaseView
    {
        private Collider collider3D;
        private LayerMask defaultMask;
        private Action<GameObject> callBack;
        public IAbilityMovement Movement { get; private set; }

        protected override void OnConstruct()
        {
            Movement = Mapper.Map<IAbilityMovement>();
            collider3D = Mapper.RecursiveMap<Collider>();
            defaultMask = collider3D.excludeLayers;
        }

        public void ExcludeCollisionLayers(LayerMask value) => collider3D.excludeLayers = defaultMask | value;
        public void SubscribeOnCollision(Action<GameObject> value) => callBack = value;
        
        private void OnCollisionEnter(Collision other) => callBack?.Invoke(other.gameObject);
        
        protected override void OnReleased()
        {
            base.OnReleased();
            Movement.Reset();
            collider3D.excludeLayers = defaultMask;
            callBack = null;
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            callBack = null;
            collider3D = null;
            Movement = null;
        }
    }
}