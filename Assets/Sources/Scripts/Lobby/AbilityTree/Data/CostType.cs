using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Potman.Lobby.AbilityTree.Data
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CostType
    {
        Common = 0,
        Rare,
        Mythic,
    }
}