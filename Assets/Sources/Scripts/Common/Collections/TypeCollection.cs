using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Potman.Common.Collections
{
    public class TypeCollection<TKey, TAttribute> where TAttribute : Attribute
    {
        protected Dictionary<TKey, Type> cachedTypes;

        public TypeCollection()
        {
            cachedTypes = new Dictionary<TKey, Type>();
        }
        
        public TypeCollection(Func<TAttribute, TKey> selector, Type baseType = null)
        {
            cachedTypes = RegisterTypes(selector, baseType);
        }
		
        protected static Dictionary<TKey, Type> RegisterTypes(Func<TAttribute, TKey> selector, Type baseType)
        {
            var registred = new Dictionary<TKey, Type>();
            var subclassTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly =>
                {
                    try
                    {
                        return assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException e)
                    {
                        Debug.LogError(e);
                        return e.Types.Where(type => type != null);
                    }
                })
                .Where(x => x.IsClass && !x.IsAbstract && (baseType == null || baseType.IsAssignableFrom(x)));

            foreach (var type in subclassTypes)
            {
                foreach (var attribute in type.GetCustomAttributes<TAttribute>(false))
                {
                    var key = selector(attribute);
                
                    if (registred.TryGetValue(key, out var existingType))
                    {
                        Debug.LogError($"Key: {key} already exists in collection. Existing Type: {existingType}, New Type: {type}");
                        continue;
                    }

                    registred.Add(key, type);
                }
            }

            return registred;
        }
		
        public Type Get(TKey key)
        {
            return cachedTypes[key];
        }

        public bool TryGet(TKey key, out Type result)
        {
            return cachedTypes.TryGetValue(key, out result);
        }
    }
}