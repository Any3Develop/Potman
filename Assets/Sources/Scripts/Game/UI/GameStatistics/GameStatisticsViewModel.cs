using System;
using System.Collections.Generic;
using Potman.Common.Events;
using Potman.Common.LifecycleService.Abstractions;
using Potman.Common.UIService.Abstractions;
using Potman.Game.Context.Abstractions;
using Potman.Game.Context.Events;
using Potman.Game.Scenarios.Data;
using Potman.Game.Scenarios.Events;
using R3;

namespace Potman.Game.UI.GameStatistics
{
    public class GameStatisticsViewModel : IInitable
    {
        private const string GameTimeId = "Stat1";
        private const string UnitDiedId = "Stat2";
        private const string UnitAliveId = "Stat3";
        
        private readonly IUIService uiService;
        private readonly Dictionary<string, Func<string>> statistics;
        private IDisposable subscriptions;
        
        public GameStatisticsViewModel(
            IUIService uiService, 
            IGameContext gameContext)
        {
            this.uiService = uiService;
            statistics = new Dictionary<string, Func<string>>
            {
                {UnitAliveId,() => $"Units Alive : {PadLeft(gameContext.UnitsAlive, 1)}"},
                {UnitDiedId ,() => $"Units Died : {PadLeft(gameContext.UnitsDied, 2)}"},
                {GameTimeId ,() => $"Game Time : {TimeSpan.FromSeconds(gameContext.Time):c}"},
            };
        }

        private static string PadLeft(int value, int spaceBefore)
        {
            var result = value.ToString();
            return result.Length < 1 + spaceBefore
                ? result.PadLeft(spaceBefore + result.Length, '\u00A0')
                : result;
        }

        public void Initialize()
        {
            MessageBroker.Receive<SenarioChangedEvent>()
                .Where(evData => evData.Current == ScenarioState.Playing)
                .Take(1)
                .Subscribe(_ => Start());
            
            MessageBroker.Receive<SenarioChangedEvent>()
                .Where(evData => evData.Current == ScenarioState.Ended)
                .Take(1)
                .Subscribe(_ => End());
        }

        private void Start()
        {
            subscriptions?.Dispose();
            subscriptions = null;
            
            uiService
                .Begin<StatisticsWindow>()
                .WithInit(InitWindow)
                .Show();
        }
        
        private void End()
        {
            subscriptions?.Dispose();
            subscriptions = null;
            
            uiService
                .Begin<StatisticsWindow>()
                .Hide();
        }
        
        private void InitWindow(StatisticsWindow window)
        {
            foreach (var (id, getInitValue) in statistics) 
                window.AddSatistic(id, getInitValue());

            using var builder = new DisposableBuilder();
            builder.Add(MessageBroker.Receive<GameTimeChangedEvent>()
                .Subscribe(_ => window.UpdateStatistic(GameTimeId, statistics[GameTimeId].Invoke())));
            
            builder.Add(MessageBroker.Receive<GameStatisticsChangedEvent>()
                .Subscribe(_ =>
                {
                    window.UpdateStatistic(UnitAliveId, statistics[UnitAliveId].Invoke());
                    window.UpdateStatistic(UnitDiedId, statistics[UnitDiedId].Invoke());
                }));
            
            subscriptions = builder.Build();
        }
    }
}