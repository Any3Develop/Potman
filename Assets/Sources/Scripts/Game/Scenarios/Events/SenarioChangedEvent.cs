using Potman.Game.Scenarios.Abstractions;
using Potman.Game.Scenarios.Data;

namespace Potman.Game.Scenarios.Events
{
    public readonly struct SenarioChangedEvent
    {
        public ScenarioState Current { get; }
        public ScenarioState Previous { get; }
        public IScenario Scenario { get; }

        public SenarioChangedEvent(ScenarioState current, ScenarioState previous, IScenario scenario)
            => (Current, Previous, Scenario) = (current, previous, scenario);
    }
}