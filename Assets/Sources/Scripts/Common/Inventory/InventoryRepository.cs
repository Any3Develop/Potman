using System;
using System.Collections.Generic;
using System.Linq;
using Potman.Common.Inventory.Abstractions;

namespace Potman.Common.Inventory
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly Dictionary<string, IInventoryItem> storage = new();

        public void Add(IInventoryItem value) 
            => storage[AssertId(value?.Id)] = value;

        public bool Remove(IInventoryItem value)
            => Remove(value?.Id);

        public bool Remove(string id) 
            => storage.Remove(id);

        public T Get<T>(string id) where T : IInventoryItem 
            => storage.TryGetValue(AssertId(id), out var item) ? (T)item : default;

        public IInventoryItem Get(string id) 
            => storage[AssertId(id)];

        public IEnumerable<IInventoryItem> GetAll(bool asQuery = false) 
            => asQuery ? storage.Values : storage.Values.ToArray();

        public IEnumerable<T> GetAll<T>(bool asQuery = false) where T : IInventoryItem 
            => asQuery ? storage.Values.OfType<T>() : storage.Values.OfType<T>().ToArray();

        public bool HasItem(string id)
            => storage.ContainsKey(AssertId(id));
        
        public bool HasItem<T>()  where T : IInventoryItem 
            => storage.Values.Any(x => x is T);

        private static string AssertId(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Id must not be null or empty.");

            return value;
        }
    }
}