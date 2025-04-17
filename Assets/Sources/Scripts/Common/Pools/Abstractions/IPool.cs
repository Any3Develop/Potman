using System;
using System.Collections.Generic;

namespace Potman.Common.Pools.Abstractions
{
    public interface IPool<T> where T : IPoolable
    {
        IReadOnlyCollection<T> Active { get; }
        IReadOnlyCollection<T> Free { get; }
        
        void AddRage(IEnumerable<T> value, bool spawned = false, bool callSpawn = true);
        void Add(T value, bool asActive = false, bool callSpawn = true);
        void Release(T value);
        void Release(Predicate<T> value);
        void Release<TCastable>(Predicate<TCastable> value) where TCastable : T;
        T Spawn(bool callSpawn = true);
        bool TrySpawn<TCastable>(Predicate<TCastable> predicate, out TCastable result, bool callSpawn = true) where TCastable : T;
        bool TrySpawn<TCastable>(Type concreteType, out TCastable result, bool callSpawn = true) where TCastable : T;
        bool TrySpawn<TCastable>(out TCastable result, bool callSpawn = true) where TCastable : T;
        bool TrySpawn(out T result, bool callSpawn = true);
        void Clear();
    }
}