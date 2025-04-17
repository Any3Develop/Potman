using System.Collections.Generic;
using UnityEngine;

namespace Potman.Common.Utilities
{
    public static class ComponentExtensions
    {
        /// <summary>
        /// Gets or adds a component to a GameObject.
        /// </summary>
        /// <param name="gameObject"> the gameObject targeted</param>
        /// <typeparam name="T"> The type of the component required</typeparam>
        /// <returns>The requested component type, either already on the GameObject or created and added within this method.</returns>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (!gameObject)
                return null;

            return gameObject.TryGetComponent(out T result) ? result : gameObject.AddComponent<T>();
        }       
        
        public static IEnumerable<GameObject> GetChilds(this GameObject root)
        {
            if (!root) 
                yield break;

            foreach (Transform child in root.transform)
            {
                yield return child.gameObject;

                foreach (var otherChild in GetChilds(child.gameObject))
                    yield return otherChild;
            }
        }
        
        public static IEnumerable<Transform> GetChilds(this Transform root)
        {
            if (!root) 
                yield break;

            foreach (Transform child in root.transform)
            {
                yield return child;

                foreach (var otherChild in GetChilds(child))
                    yield return otherChild;
            }
        }

        public static void Set(this ref Vector3 source, float? x = null, float? y = null, float? z = null)
        {
            source.x = x ?? source.x;
            source.y = y ?? source.y;
            source.z = z ?? source.z;
        }
    }
}