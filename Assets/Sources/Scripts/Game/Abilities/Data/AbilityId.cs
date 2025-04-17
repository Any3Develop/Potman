using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Potman.Game.Abilities.Data
{
    /// <summary>
    /// The unique identifier of the ability controller.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AbilityId
    {
        BattleAbility0 = 0,
        BattleAbility1 = 1,
        BattleAbility2 = 2,
    }
}