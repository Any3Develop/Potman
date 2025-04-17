using Cysharp.Threading.Tasks;
using Potman.Game.Entities.Player.Data;

namespace Potman.Game.Entities.Player.Abstractions
{
    public interface IPlayerEntityFactory
    {
        UniTask<IPlayerEntity> CreateAsync(PlayerData playerData);
    }
}