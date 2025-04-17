using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Potman.Game.Abilities.Data.Upgrades
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum UpgradeTier
    {
        Low = 0,
        Middle = 25,
        High = 50,
        Top = 75,
    }
}