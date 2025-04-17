using Potman.Game.Abilities.Data;
using Potman.Game.Entities.Abstractions;

namespace Potman.Game.Abilities.Abstractions
{
    public interface IAbilityFactory
    {
        IAbility Create(AbilityId id, IRuntimeEntity owner);
        IAbility Create(AbilityData data, IRuntimeEntity owner);
    }
}