using Potman.Common.Collections;
using Potman.Game.Abilities.Data;

namespace Potman.Game.Abilities.Abstractions
{
    public interface IAbilityCollection : IRuntimeCollection<IAbility>
    {
        bool TryGet(AbilityId id, out IAbility result);
        IAbility Get(AbilityId id);
        bool Contains(AbilityId id);
    }
}