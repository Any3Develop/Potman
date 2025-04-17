using System.Collections.Generic;
using Potman.Common.SerializeService.Abstractions;

namespace Potman.Common.SerializeService
{
    public class MemorySerializeService : ISerializeService
    {
        private readonly Dictionary<string, string> storage = new();

        public void Patch(string key, string value)
            => storage[key] = value;

        public string Get(string key) 
            => HasKey(key) ? storage[key] : "";

        public void Delete(string key) 
            => storage.Remove(key);

        public bool HasKey(string key) 
            => storage.ContainsKey(key);

        public void Save() {}
        
        public bool TryGet(string key, out string result)
            => storage.TryGetValue(key, out result);
    }
}