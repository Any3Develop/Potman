using System;

namespace Potman.Game.Scenarios.Data
{
    [Flags]
    public enum ScenarioState
    {
        None = 0,
        Playing = 2,
        Paused = 4,
        Ended = 8,
        Cinematic = 16,
    }
}