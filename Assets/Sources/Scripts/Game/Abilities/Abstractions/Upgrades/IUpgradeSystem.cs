using Cysharp.Threading.Tasks;

namespace Potman.Game.Abilities.Abstractions.Upgrades
{
    public interface IUpgradeSystem
    {
        UniTask StartAsync();
        void End();
    }
}