using Cysharp.Threading.Tasks;

namespace Potman.Common.Inventory.Abstractions
{
    public interface IInventory
    {
        IInventoryRepository Repository { get; }

        UniTask LoadAsync();
        UniTask SaveAsync();
    }
}