using Potman.Common.SceneService;
using Potman.Game.Scenarios.Data;

namespace Potman.Lobby.UI.GameModes
{
    public readonly struct GameModeData
    {
        public ScenarioId Id { get; }
        public SceneId Scene { get; }
        public string Name { get; }

        public GameModeData(SceneId scene, ScenarioId id, string name)
        {
            Id = id;
            Scene = scene;
            Name = name;
        }
    }
}