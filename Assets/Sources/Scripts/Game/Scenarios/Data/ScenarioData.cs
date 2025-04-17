using Potman.Lobby.Identity.Abstractions;

namespace Potman.Game.Scenarios.Data
{
    public class ScenarioData : IRedirectionArg
    {
        public ScenarioId Id { get; set; }
        public int Level { get; set; }
    }
}