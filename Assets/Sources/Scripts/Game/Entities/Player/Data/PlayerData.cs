using System.Collections.Generic;
using Potman.Game.Abilities.Data;
using Potman.Game.Stats.Data;
using Potman.Lobby.Identity.Abstractions;

namespace Potman.Game.Entities.Player.Data
{
    public class PlayerData : IRedirectionArg
    {
        public PlayerId Id { get; set; }
        public List<StatData> Stats { get; set; } = new();
        public List<AbilityData> Abilities { get; set; } = new();
    }
}