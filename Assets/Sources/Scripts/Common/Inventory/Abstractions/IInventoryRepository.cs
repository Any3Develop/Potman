using System.Collections.Generic;

namespace Potman.Common.Inventory.Abstractions
{
    public interface IInventoryRepository
    {
        void Add(IInventoryItem value);
        bool Remove(IInventoryItem value);
        bool Remove(string id);
        
        T Get<T>(string id) where T : IInventoryItem;
        IInventoryItem Get(string id);
        IEnumerable<IInventoryItem> GetAll(bool asQuery = false);
        IEnumerable<T> GetAll<T>(bool asQuery = false) where T : IInventoryItem;
        bool HasItem(string id);
        bool HasItem<T>() where T : IInventoryItem;
    }
}