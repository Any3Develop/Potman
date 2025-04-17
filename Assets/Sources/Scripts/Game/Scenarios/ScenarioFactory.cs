using System;
using Potman.Common.Collections;
using Potman.Common.DependencyInjection;
using Potman.Common.SerializeService.Abstractions;
using Potman.Game.Scenarios.Abstractions;
using Potman.Game.Scenarios.Data;

namespace Potman.Game.Scenarios
{
    public class ScenarioFactory : IScenarioFactory
    {
        private readonly IAbstractFactory abstractFactory;
        private readonly TypeCollection<ScenarioId, ScenarioAttribute> typeCollection;

        public ScenarioFactory(IAbstractFactory abstractFactory, ISerializeService serializeService)
        {
            this.abstractFactory = abstractFactory;
            typeCollection = new SerializedTypeCollection<ScenarioId, ScenarioAttribute>(serializeService, att => att.Id, typeof(IScenario));
        }
        
        public IScenario Create(ScenarioId id, params object[] args)
        {
            if (!typeCollection.TryGet(id, out var type))
                throw new NullReferenceException($"Can't create a {nameof(IScenario)} with id : {id}, because it's not registered.");

            return (IScenario)abstractFactory.Create(type, args);
        }
    }
}