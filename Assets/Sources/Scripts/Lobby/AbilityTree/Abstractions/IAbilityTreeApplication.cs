using Cysharp.Threading.Tasks;

namespace Potman.Lobby.AbilityTree.Abstractions
{
    public interface IAbilityTreeApplication
    {
        UniTask LoadAsync();
    }
}