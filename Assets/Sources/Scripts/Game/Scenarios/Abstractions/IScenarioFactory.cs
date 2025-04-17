using Potman.Game.Scenarios.Data;

namespace Potman.Game.Scenarios.Abstractions
{
    public interface IScenarioFactory
    {
        IScenario Create(ScenarioId id, params object[] args);
    }
}