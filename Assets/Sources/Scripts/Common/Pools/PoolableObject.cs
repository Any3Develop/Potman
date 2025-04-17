using System;
using Potman.Common.Pools.Abstractions;
using UnityEngine;

namespace Potman.Common.Pools
{
    public abstract class PoolableObject : MonoBehaviour, ISpwanPoolable, IDisposable
    {
        [field: SerializeField] public string PoolableId { get; private set; }
        [field: SerializeField] public Transform Root { get; private set; }
        [field: SerializeField] public GameObject Container { get; private set; }
        [field: SerializeField] public bool ActiveOnSpawn { get; private set; } = true;
        [field: SerializeField] public bool ActiveOnRelease { get; private set; }
        [field: SerializeField] public bool IsSpawned { get; private set; }
        [field: SerializeField] public bool IsDisposed { get; private set; }
        
        public void Release()
        {
            if (DisposedLog())
                return;

            if (!IsSpawned)
            {
                Debug.LogWarning($"Release called [{GetType().Name}] twice.");
                return;
            }

            AssertComponents();
            Container.SetActive(ActiveOnRelease);
            IsSpawned = false;
            OnReleased();
        }

        public void Spawn()
        {
            if (DisposedLog())
                return;

            if (IsSpawned)
            {
                Debug.LogWarning($"Spawn called [{GetType().Name}] twice.");
                return;
            }

            AssertComponents();
            Container.SetActive(ActiveOnSpawn);
            IsSpawned = true;
            OnSpawned();
        }

        public void Dispose()
        {
            OnDestroy();
            Destroy(gameObject);
        }

        protected void OnDestroy()
        {
            if (DisposedLog())
                return;

            IsSpawned = false;
            IsDisposed = true;
            OnDisposed();
        }

        protected virtual void OnSpawned() {}
        protected virtual void OnReleased() {}
        protected virtual void OnDisposed() {}
        
        protected virtual void OnValidate()
        {
            if (!Container)
                Container = gameObject;

            if (!Root)
                Root = transform;

            SetPoolableId(Guid.NewGuid().ToString());
        }

        protected bool DisposedLog()
        {
            if (!IsDisposed)
                return false;

            Debug.LogError($"You are trying to use disposed {GetType().Name}.");
            return true;
        }

        private void AssertComponents()
        {
            if (!Container)
                throw new NullReferenceException($"{nameof(Container)} : {nameof(GameObject)} Not set in inspector or was missed while spawn.");

            if (!Root)
                throw new NullReferenceException($"{nameof(Root)} : {nameof(GameObject)} Not set in inspector or was missed while spawn.");
        }

        public void SetPoolableId(string value)
        {
            PoolableId = value;
        }
    }
}