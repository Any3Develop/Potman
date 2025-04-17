using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Potman.Common.ResourceManagament
{
    public class ResourceCache<T> where T : Object
    {
        private readonly List<(string, T, Object)> cache = new();
        
        public bool Preloaded { get; private set; }
        public T this[int i] => cache[i].Item2;
        public int Count => cache.Count;

        public async UniTask PreloadAsync(IEnumerable<string> ids)
        {
            if (Preloaded)
                return;

            Preloaded = true;
            var preloadIds = ids.Except(cache.Select(x => x.Item1)).ToArray();
            if (preloadIds.Length == 0)
                return;

            await UniTask.WhenAll(preloadIds.Select(GetAsync));
        }

        public async UniTask PreloadAsync(params string[] ids)
        {
            var preloadIds = ids.Except(cache.Select(x => x.Item1)).ToArray();
            if (preloadIds.Length == 0)
                return;

            await UniTask.WhenAll(preloadIds.Select(GetAsync));
        }
        
        public bool TryGet(string id, out T result)
        {
            if (string.IsNullOrEmpty(id))
            {
                result = default;
                return false;
            }
            
            result = cache.FirstOrDefault(x => x.Item1 == id).Item2;
            return result != null;
        }

        public async UniTask<T> GetAsync(string id)
        {
            if (TryGet(id, out var result))
                return result;

            Object source;
            var tType = typeof(T);
            if (typeof(Component).IsAssignableFrom(tType))
            {
                var obj = await ResourceServiceAdapter.Instance.GetAsync<GameObject>(id);
                source = obj;
                result = obj.GetComponent<T>();
            }
            else if (typeof(GameObject) == tType)
                source = result = await ResourceServiceAdapter.Instance.GetAsync<GameObject>(id) as T;
            else
                source = result = await ResourceServiceAdapter.Instance.GetAsync<T>(id);

            AddInternal(id, result, source);
            return result;
        }

        public void Release()
        {
            foreach (var value in cache) 
                ResourceServiceAdapter.Instance.Release(value.Item3);
            
            cache.Clear();
            Preloaded = false;
        }

        private void AddInternal(string id, T obj, Object source)
        {
            cache.Add((id, obj, source));
        }
    }
}