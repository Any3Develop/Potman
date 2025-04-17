using Cysharp.Threading.Tasks;

namespace Potman.Game.Abilities.Abstractions
{
    public interface IAbilityViewFactory
    {
        UniTask<IAbilityView> CreateAsync(IAbility ability, int index = 0);
        UniTask<TView> CreateAsync<TView>(IAbility ability, int index = 0) where TView : IAbilityView;
    }
}