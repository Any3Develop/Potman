using System;
using Potman.Game.Entities.Abstractions;

namespace Potman.Game.Entities
{
    public class EntityMapperMock : IEntityMapper
    {
        public T Find<T>(bool recursive = false) where T : class
            => default;

        public T[] FindAll<T>(bool recursive = false) where T : class
            => Array.Empty<T>();

        public bool TryFind<T>(out T result, bool recursive = false) where T : class
            => (result = default) != null;

        public void AddMap<TMap>(string id, TMap obj) where TMap : class {}

        public TMap Map<TMap>(string id) where TMap : class
            => default;

        public TMap[] MapAll<TMap>() where TMap : class
            => Array.Empty<TMap>();

        public bool TryMap<TMap>(out TMap result, string id = null) where TMap : class
            => (result = default) != null;

        public TMap RecursiveMap<TMap>(string id = null, int depth = 0) where TMap : class
            => default;

        public TMap[] RecursiveMapAll<TMap>(string id = null, int depth = 0) where TMap : class
            => Array.Empty<TMap>();
    }
}