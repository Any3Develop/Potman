using System;
using System.Collections.Generic;
using System.Linq;
using Potman.Common.Pools.Abstractions;
using UnityEngine;

namespace Potman.Common.Pools
{
    public class RuntimePool<T> : IPool<T> where T : IPoolable
    {
        protected readonly List<T> FreeList = new();
        protected readonly List<T> ActiveList = new();

        public IReadOnlyCollection<T> Active => ActiveList;
        public IReadOnlyCollection<T> Free => FreeList;

        public void AddRage(IEnumerable<T> value, bool spawned = false, bool callSpawn = true)
        {
            foreach (var poolable in value)
                Add(poolable, spawned, callSpawn);
        }

        public void Add(T value, bool asActive = false, bool callSpawn = true)
        {
            if (asActive)
            {
                SpawnInternal(value, callSpawn);
                return;
            }

            ReleaseInternal(value);
        }

        public void Release(T value)
        {
            if (!ActiveList.Remove(value))
            {
                Debug.LogError($"Can't release {nameof(IPoolable)} who's never been in the pool : [{value}]");
                return;
            }

            ReleaseInternal(value);
        }

        public void Release(Predicate<T> value)
        {
            var release = Active.Where(value.Invoke).ToArray();
            foreach (var item in release)
                Release(item);
        }

        public void Release<TCastable>(Predicate<TCastable> value) where TCastable : T
        {
            var release = Active.OfType<TCastable>().Where(value.Invoke).ToArray();
            foreach (var item in release)
                Release(item);
        }

        public T Spawn(bool callSpawn = true)
        {
            TrySpawn(out var result, callSpawn);
            return result;
        }

        public bool TrySpawn<TCastable>(Predicate<TCastable> predicate, out TCastable result, bool callSpawn = true) where TCastable : T
        {
            result = (TCastable)FreeList.FirstOrDefault( x => x is TCastable castable && predicate.Invoke(castable));
            if (!FreeList.Remove(result))
                return false;

            SpawnInternal(result, callSpawn);
            return true;
        }

        private readonly Type destinationType = typeof(T);
        public bool TrySpawn<TCastable>(Type concreteType, out TCastable result, bool callSpawn = true) where TCastable : T
        {
            result = default;
            if (!destinationType.IsAssignableFrom(concreteType))
                return false;

            result = (TCastable)FreeList.LastOrDefault(x => x.GetType() == concreteType);
            if (!FreeList.Remove(result))
                return false;

            SpawnInternal(result, callSpawn);
            return true;
        }

        public bool TrySpawn<TCastable>(out TCastable result, bool callSpawn = true) where TCastable : T
        {
            result = FreeList.OfType<TCastable>().FirstOrDefault();
            if (!FreeList.Remove(result))
                return false;

            SpawnInternal(result, callSpawn);
            return true;
        }

        public bool TrySpawn(out T result, bool callSpawn = true)
        {
            result = FreeList.FirstOrDefault();
            if (!FreeList.Remove(result))
                return false;

            SpawnInternal(result, callSpawn);
            return TrySpawn<T>(out result, callSpawn);
        }

        public void Clear()
        {
            foreach (var poolable in FreeList.Concat(ActiveList).OfType<IDisposable>())
                poolable.Dispose();

            FreeList.Clear();
            ActiveList.Clear();
        }

        private void SpawnInternal<TPollable>(TPollable tObj, bool spawn = true) where TPollable : T
        {
            if (ActiveList.Contains(tObj))
            {
                Debug.LogError($"Can't add as spawned {nameof(IPoolable)} twice : [{tObj}]");
                return;
            }
                
            ActiveList.Add(tObj);
            
            if (spawn && tObj is ISpwanPoolable spwanPoolable)
                spwanPoolable.Spawn();
        }

        private void ReleaseInternal<TPollable>(TPollable tObj) where TPollable : T
        {
            if (FreeList.Contains(tObj))
            {
                Debug.LogError($"Can't add as free {nameof(IPoolable)} twice : [{tObj}]");
                return;
            }
                
            tObj.Release();
            FreeList.Add(tObj);
        }
    }
}