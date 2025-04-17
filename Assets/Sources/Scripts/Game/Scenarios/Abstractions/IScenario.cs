using System;
using System.Collections.Generic;
using Potman.Game.Scenarios.Data;

namespace Potman.Game.Scenarios.Abstractions
{
    public interface IScenario : IDisposable
    {
        ScenarioState State { get; }
        IEnumerable<IScenario> Nested { get; }
        
        void Start();
        void Pause(bool value);
        void End();
    }
}