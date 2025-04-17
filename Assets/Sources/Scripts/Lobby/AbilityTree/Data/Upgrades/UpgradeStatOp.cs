using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Potman.Lobby.AbilityTree.Data.Upgrades
{
    [Flags, JsonConverter(typeof(StringEnumConverter))]
    public enum UpgradeStatOp
    {
        Addition = 2,
        Subtract = 8,
        Multiply = 16,
        Divide = 32,
        Percent = 64,
    }
}