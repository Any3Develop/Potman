using System;
using System.Collections.Generic;
using System.Linq;
using Potman.Common.SerializableDictionary;
using Potman.Common.Utilities;
using Potman.Game.Entities.Abstractions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Potman.Game.Entities
{
    [DisallowMultipleComponent]
    public class UnityObjectMapper : MonoBehaviour, IEntityMapper
    {
        [Serializable]
        private class ComponentMap : SerializableDictionary<string, Object> {}

        [SerializeField] private GameObject container;
        [SerializeField] private List<string> sharedMap = new();
        [SerializeField] private ComponentMap componentMap = new();

        private void Awake()
        {
            if (sharedMap.Count == 0)
                return;
            
            var shared = sharedMap.Select(id => (id, Map<Object>(id))).ToArray();
            if (shared.Length == 0)
                return;
            
            foreach (var mapper in RecursiveMapAll<IEntityMapper>())
            {
                foreach (var (id, obj) in shared)
                    mapper.AddMap(id, obj);
            }
        }

        public T Find<T>(bool recursive = false) where T : class
            => TryFind(out T obj, recursive) ? obj : default;

        public T[] FindAll<T>(bool recursive = false) where T : class
            => recursive
                ? container.GetComponentsInChildren<T>()
                : container.GetComponents<T>();

        public bool TryFind<T>(out T result, bool recursive = false) where T : class
            => recursive 
                ? (result = container.GetComponentInChildren<T>()) != null
                : container.TryGetComponent(out result);

        public void AddMap<TMap>(string id, TMap obj) where TMap : class
        {
            if (obj is not Object unityObject)
                throw new InvalidOperationException("Expected a class with base type : UnityEngine.Object");
            
            if (!componentMap.TryAdd(id, unityObject))
                Debug.LogError($"{typeof(TMap).Name} with id {id} has already mapped.");
        }

        public TMap Map<TMap>(string id = null) where TMap : class
        {
            if (string.IsNullOrEmpty(id))
            {
                if (componentMap.Values.FirstOrDefault(x => x is TMap) is TMap tObj)
                    return tObj;
            }
            else if (componentMap.TryGetValue(id, out var obj))
            {
                if (obj is TMap tObj)
                    return tObj;

                Debug.LogError($"{typeof(TMap).Name} with Id : {id} has mapped with another type : {obj.GetType().Name}.");
                return default;
            }

            Debug.LogError($"{typeof(TMap).Name} with Id : {id} haven't mapped yet.");
            return default;
        }

        public TMap[] MapAll<TMap>() where TMap : class 
            => componentMap.Values.OfType<TMap>().ToArray();

        public bool TryMap<TMap>(out TMap result, string id = null) where TMap : class
        {
            if (string.IsNullOrEmpty(id))
                return (result = componentMap.Values.FirstOrDefault(x => x is TMap) as TMap) != null;

            if (componentMap.TryGetValue(id, out var obj))
                return (result = obj as TMap) != null;

            result = default;
            return false;
        }

        public TMap RecursiveMap<TMap>(string id = null, int depth = 0) where TMap : class
        {
            TMap result;
            
            if (!string.IsNullOrEmpty(id))
            {
                if (componentMap.TryGetValue(id, out var obj) && obj is not TMap)
                    Debug.LogError($"{typeof(TMap).Name} with Id : {id} has mapped with another type : {obj.GetType().Name}.");
                
                result = obj as TMap;
            }
            else
                result = componentMap.Values.FirstOrDefault(x => x is TMap) as TMap;

            if (result != null)
                return result;

            foreach (var child in componentMap.Values.OfType<IEntityMapper>().Where(x => !ReferenceEquals(x, this)))
            {
                result = child.RecursiveMap<TMap>(id, depth + 1);

                if (result != null)
                    return result;
            }

            if (depth > 0)
                return null;

            var logVar = string.IsNullOrEmpty(id) ? "" : $" with Id : {id}";
            Debug.LogError($"{typeof(TMap).Name}{logVar} hasn't mapped anywhere.");
            return null;
        }

        public TMap[] RecursiveMapAll<TMap>(string id = null, int depth = 0) where TMap : class
        {
            var current = Enumerable.Empty<TMap>();
            if (!string.IsNullOrEmpty(id))
            {
                if (componentMap.TryGetValue(id, out var obj))
                {
                    if (obj is TMap tObj)
                        current = Enumerable.Empty<TMap>().Append(tObj);
                    else
                        Debug.LogError($"{typeof(TMap).Name} with Id : {id} has mapped with another type : {obj.GetType().Name}.");
                }
            }
            else
            {
                current = componentMap.Values.OfType<TMap>();
            }
            
            var result = current.Concat(componentMap.Values
                .OfType<IEntityMapper>()
                .Where(x => !ReferenceEquals(x, this))
                .SelectMany(x => x.RecursiveMapAll<TMap>(id, depth + 1))).ToArray();

            if (depth > 0)
                return result;

            if (result.Length == 0)
                Debug.LogError($"{typeof(TMap).Name} hasn't mapped anywhere.");

            return result;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!container)
                container = gameObject;
        }

        private void Reset()
        {
            OnValidate();
            componentMap.Add("Root", transform);
            UnityEditor.EditorUtility.SetDirty(this);
        }

        [ContextMenu("AutoFill: Preset for Entities")]
        private void AutoFillEntity()
        {
            AutoFillComponentsRecursively();
            AutoFillNormalize();
        }


        [ContextMenu("AutoFill: Normalize Id")]
        private void AutoFillNormalize()
        {
            var indexMap = new Dictionary<string, int>();
            foreach (var path in componentMap.Keys.Where(key => key.StartsWith("AutoFill")).ToArray())
            {
                var obj = componentMap[path];
                var newId = GerenateObjectId(obj);
                AppendIndex(ref newId, indexMap);
                componentMap.Remove(path);
                componentMap[newId] = obj;
            }

            UnityEditor.EditorUtility.SetDirty(this);
        }

        [ContextMenu("AutoFill: Components Only Recursively")]
        private void AutoFillComponentsRecursively()
        {
            var indexMap = new Dictionary<string, int>();
            var skip = new List<Transform>();
            var objects = container.GetComponentsInChildren<Component>(true).Where(x => x is not Transform);
            
            foreach (var obj in objects)
            {
                var objTrans = obj.transform;
                if (obj is IEntityMapper mapper)
                {
                    if (ReferenceEquals(mapper, this))
                        continue;

                    skip.Add(objTrans);
                }                
                else if (skip.Any(x => objTrans.IsChildOf(x)))
                    continue;
                
                var path = GerenateObjectPath(obj, objTrans.parent) + GerenateObjectId(obj);
                AppendIndex(ref path, indexMap);
                componentMap[path] = obj;
            }

            UnityEditor.EditorUtility.SetDirty(this);
        }

        [ContextMenu("AutoFill: All Recursively")]
        private void AutoFillAllRecursively()
        {
            var indexMap = new Dictionary<string, int>();
            var skip = new List<Transform>();
            var objects = container.GetComponentsInChildren<Component>(true).Concat<Object>(container.GetChilds()).Distinct();

            foreach (var obj in objects)
            {
                var objTrans = obj switch
                {
                    GameObject go => go.transform,
                    Component comp => comp.transform,
                    _ => null
                };

                if (obj is IEntityMapper mapper)
                {
                    if (ReferenceEquals(mapper, this))
                        continue;

                    skip.Add(objTrans);
                }
                else if (objTrans && skip.Any(x => objTrans.IsChildOf(x)))
                    continue;

                var path = GerenateObjectPath(obj, objTrans ? objTrans.parent : default) + GerenateObjectId(obj);
                AppendIndex(ref path, indexMap);
                componentMap[path] = obj;
            }
            
            UnityEditor.EditorUtility.SetDirty(this);
        }

        private static string GerenateObjectId(Object obj)
        {
            return obj switch
            {
                GameObject => $"{obj.name}_{nameof(GameObject)}",
                Transform => $"{obj.name}_{nameof(Transform)}",
                IEntityMapper => $"Mapper:{obj.name}",
                _ => $"{obj.GetType().Name}"
            };
        }
        
        private static string GerenateObjectPath(Object obj, Transform parent)
        {
            return parent
                ? $"AutoFill/.../{parent.name}/{obj.name}/"
                : $"AutoFill/{obj.name}/";
        }

        private static void AppendIndex(ref string id, IDictionary<string, int> indexMap)
        {
            if (!indexMap.TryGetValue(id, out var index))
                indexMap.Add(id, index = 0);
            else
                indexMap[id] = index += 1;

            if (index >= 1)
                id += $"_{index}";
        }

        [ContextMenu("AutoFill: Clear")]
        private void AutoFillClear()
        {
            foreach (var key in componentMap.Where(x => x.Key.StartsWith("AutoFill")).Select(x => x.Key).ToArray())
                componentMap.Remove(key);

            UnityEditor.EditorUtility.SetDirty(this);
        }

        [ContextMenu("AutoFill: Copy Ids to SharedMap")]
        private void AutoFillSharedMap()
        {
            sharedMap = componentMap.Keys.ToList();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}