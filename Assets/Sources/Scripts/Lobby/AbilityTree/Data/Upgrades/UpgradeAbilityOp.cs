using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Potman.Lobby.AbilityTree.Data.Upgrades
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum UpgradeAbilityOp
    {
        Union = 0,
        Remove,
        Add,
    }
}