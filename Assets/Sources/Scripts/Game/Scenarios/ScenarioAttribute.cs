using System;
using Potman.Game.Scenarios.Data;

namespace Potman.Game.Scenarios
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ScenarioAttribute : Attribute
    {
        public ScenarioId Id { get; }
        public ScenarioAttribute(ScenarioId id) => Id = id;
    }
}