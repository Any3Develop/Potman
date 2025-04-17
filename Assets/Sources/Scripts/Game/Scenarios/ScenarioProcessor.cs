using System;
using Potman.Common.Events;
using Potman.Common.LifecycleService.Abstractions;
using Potman.Common.Utilities;
using Potman.Game.Entities.Player.Data;
using Potman.Game.Scenarios.Abstractions;
using Potman.Game.Scenarios.Data;
using Potman.Game.Scenarios.Events;
using Potman.Lobby.Identity;
using R3;
using UnityEngine;

namespace Potman.Game.Scenarios
{
    public class ScenarioProcessor : IScenarioProcessor, IDisposable, IInitable
    {
        private readonly IScenarioFactory scenarioFactory;
        private IDisposable subscribes;
        public IScenario Scenario { get; private set; }

        public ScenarioProcessor(IScenarioFactory scenarioFactory)
        {
            this.scenarioFactory = scenarioFactory;
        }

        public void Dispose()
        {
            subscribes?.Dispose();
            subscribes = null;
            Scenario?.Dispose();
            Scenario = null;
        }
        
        public void Initialize()
        {
            try
            {
                var levelData = UserIdentity.Redirections.GetArg<ScenarioData>();
                var playerData = UserIdentity.Redirections.GetArg<PlayerData>();
                using var builder = new DisposableBuilder();
            
                builder.Add(MessageBroker.Receive<SenarioChangedEvent>()
                    .Where(x => x.Scenario == Scenario)
                    .Subscribe(OnScenarioChanged));
            
                subscribes = builder.Build();
                Scenario = scenarioFactory.Create(levelData.Id, levelData, playerData);
                Scenario.Start();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        private void OnScenarioChanged(SenarioChangedEvent evData)
        {
            if (evData.Current.AnyFlags(ScenarioState.Ended | ScenarioState.None))
                Debug.Log($"Scenario : {evData.Scenario} ended!");
        }
    }
}