using System;
using Cysharp.Threading.Tasks;
using Potman.Common.Events;
using Potman.Common.Utilities;
using Potman.Game.Context.Abstractions;
using Potman.Game.Context.Events;
using Potman.Game.Entities.Player.Abstractions;
using Potman.Game.Entities.Units.Events;
using Potman.Game.Scenarios.Data;
using Potman.Game.Scenarios.Events;
using R3;
using IServiceProvider = Potman.Common.DependencyInjection.IServiceProvider;

namespace Potman.Game.Context
{
    public class GameContext : IGameContext
    {
        private int maxTime;
        
        public IPlayerEntity Player { get; private set; }
        public IServiceProvider ServiceProvider { get; }
        public int Time { get; private set; }
        public int TimeLeft => maxTime - Time;
        public int StartDelay { get; private set; }
        public int UnitsTotalMax { get; private set; }
        public int UnitsSceneMax { get; private set; }
        public int UnitsAlive { get; private set; }
        public int UnitsSpawned  => UnitsAlive + UnitsDied;
        public int UnitsDied { get; private set; }
        public int UnitsLeft => UnitsTotalMax - UnitsSpawned;
        public bool Initialized { get; private set; }
        private const int TickValueMs = 1;
        private IDisposable subscribes;
        private bool isPaused;

        public GameContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public void Pause(bool value)
        {
            if (isPaused == value)
                return;

            isPaused = value;
        }

        public UniTask StartAsync(ContextConfig cfg, IPlayerEntity player)
        {
            isPaused = false;
            maxTime = cfg.gameTime;

            StartDelay = cfg.delayedStartMs;
            UnitsTotalMax = cfg.maxUnitsTotal <= 0 ? int.MaxValue : cfg.maxUnitsTotal;
            UnitsSceneMax = cfg.maxUnitsScene <= 0 ? int.MaxValue : cfg.maxUnitsScene;
            subscribes?.Dispose();
            using var builder = new DisposableBuilder();
            
            builder.Add(Observable.Interval(TimeSpan.FromSeconds(TickValueMs))
                .Skip(TimeSpan.FromMilliseconds(cfg.delayedStartMs))
                .Subscribe(_ => Update()));
            
            builder.Add(MessageBroker.Receive<SenarioChangedEvent>()
                .Subscribe(evData => Pause(evData.Current.AnyFlags(ScenarioState.Paused))));
            
            builder.Add(MessageBroker.Receive<UnitSpawnedEvent>().Subscribe(_ =>
            {
                ++UnitsAlive;
                MessageBroker.Publish(new GameStatisticsChangedEvent());
            }));
            builder.Add(MessageBroker.Receive<UnitDiedEvent>().Subscribe(_ =>
            {
                --UnitsAlive;
                ++UnitsDied;
                MessageBroker.Publish(new GameStatisticsChangedEvent());
            }));

            subscribes = builder.Build();
            Player = player;
            Initialized = true;
            
            return UniTask.Delay(StartDelay);
        }

        private void Update()
        {
            if (isPaused)
                return;
            
            Time += TickValueMs;
            MessageBroker.Publish(new GameTimeChangedEvent());
            if (maxTime <= 0 || Time < maxTime)
                return;

            Time = maxTime;
            subscribes?.Dispose();
            subscribes = null;
            MessageBroker.Publish(new GameTimeEndedEvent());
        }

        public void End()
        {
            isPaused = false;
            subscribes?.Dispose();
            subscribes = null;
            Time = 0;
            Initialized = false;
        }
    }
}