using System;
using Potman.Common.SerializeService.Abstractions;

namespace Potman.Common.Collections
{
    public class SerializedTypeCollection<TKey, TAttribute> : TypeCollection<TKey, TAttribute> where TAttribute : Attribute
    {
        public SerializedTypeCollection(
            ISerializeService serializeService,
            Func<TAttribute, TKey> selector, 
            Type baseType = null)
#if UNITY_EDITOR
            : base(selector, baseType) {}
#else
        {
            var key = $"SerializedTypeCollection({typeof(TKey).Name},{typeof(TAttribute).Name})";
            var hashPath = System.IO.Path.Combine(key, "Hash");
            var dataPath = System.IO.Path.Combine(key, "Data");
            if (!serializeService.TryGet(dataPath, out var jsonContent)
                || !serializeService.TryGet(hashPath, out var hash)
                || hash != jsonContent.GetHashCode().ToString())
            {
                cachedTypes = RegisterTypes(selector, baseType);
                jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(cachedTypes);
                serializeService.Patch(dataPath, jsonContent);
                serializeService.Patch(hashPath, jsonContent.GetHashCode().ToString());
                serializeService.Save();
                return;
            }
            
            cachedTypes = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.Dictionary<TKey, Type>>(jsonContent);
        }
#endif
    }
}