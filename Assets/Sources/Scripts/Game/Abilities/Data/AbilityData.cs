using System.Collections.Generic;
using Potman.Game.Stats.Data;

namespace Potman.Game.Abilities.Data
{
    public class AbilityData
    {
        public AbilityId Id { get; set; }
        public List<StatData> Stats { get; set; } = new();
    }
}