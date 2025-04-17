using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Potman.Common.Utilities;
using Potman.Game.Context.Abstractions;
using Potman.Game.Context.Data.Spawn;
using Potman.Game.Entities.Player.Abstractions;
using Potman.Game.Entities.Player.Data;
using Potman.Game.Entities.Units.Abstractions;
using Potman.Game.Entities.Units.Data;
using Potman.Game.Scenarios.Data;
using Potman.Game.Stats.Data;
using UnityEngine;

namespace Potman.Game.Scenarios
{
    [Scenario(ScenarioId.Default)]
    [Scenario(ScenarioId.Graveyard)]
    public class DefaultScenario : ScenarioBase
    {
        private const string pathFormat = "Game/ScenarioConfigs/ScenarioCfg_{0}_{1}";
        private readonly IGameContext context;
        private readonly IUnitEntityFactory unitEntityFactory;
        private readonly IPlayerEntityFactory playerEntityFactory;
        private readonly ISpawnPointProvider spawnPointProvider;
        private readonly IPositionProvider positionProvider;
        private readonly ScenarioData scenarioData;
        private readonly PlayerData playerData;
        private ScenarioConfig scenarioCfg;

        private readonly CancellationTokenSource lifetime;
        private readonly CancellationToken token;

        public DefaultScenario(
            IGameContext context,
            IUnitEntityFactory unitEntityFactory,
            IPlayerEntityFactory playerEntityFactory,
            ISpawnPointProvider spawnPointProvider,
            IPositionProvider positionProvider,
            ScenarioData scenarioData,
            PlayerData playerData)
        {
            this.context = context;
            this.unitEntityFactory = unitEntityFactory;
            this.playerEntityFactory = playerEntityFactory;
            this.spawnPointProvider = spawnPointProvider;
            this.positionProvider = positionProvider;
            this.scenarioData = scenarioData;
            this.playerData = playerData;
            lifetime = new CancellationTokenSource();
            token = lifetime.Token;
        }

        protected override async UniTask OnStartAsync()
        {
            try
            {
                // TODO добавить прогресс в UI
                
                scenarioCfg = await Resources
                    .LoadAsync(string.Format(pathFormat, scenarioData.Id, scenarioData.Level))
                    .ToUniTask<ScenarioConfig>();
                
                await spawnPointProvider.StartAsync(scenarioCfg.spawnPrefabId);
                
                var player = await playerEntityFactory.CreateAsync(playerData);
                var spawnConfigs = scenarioCfg.unitsScenario
                    .Concat(scenarioCfg.objectsScenario)
                    .Concat(scenarioCfg.effectsScenario);
                
                foreach (var cfg in spawnConfigs)
                    SetDefault(cfg);
                
                await context.StartAsync(scenarioCfg.context, player);
                ExecuteLoopAsync().Forget();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                End();
            }
        }

        protected override void OnEnded()
        {
            spawnPointProvider?.End();
            context?.End();
            if (!lifetime.IsCancellationRequested)
            {
                lifetime?.Cancel();
                lifetime?.Dispose();
            }
        }

        protected override void OnDisposed()
        {
            lifetime.Cancel();
            lifetime.Dispose();
            
            if (!scenarioCfg) 
                return;
            
            Resources.UnloadAsset(scenarioCfg);
            scenarioCfg = null;
        }

        private async UniTask ExecuteLoopAsync()
        {
            try
            {
                var hasUnitsCfg = scenarioCfg.unitsScenario.Length > 0;
                var hasObjectsCfg = scenarioCfg.objectsScenario.Length > 0;
                var hasEffectsCfg = scenarioCfg.effectsScenario.Length > 0;
                var levelStat = context.Player.StatsCollection.Get(StatType.Level);
                var hpStat = context.Player.StatsCollection.Get(StatType.Heath);
                var tickDelay = TimeSpan.FromMilliseconds(1000);

                while (Application.isPlaying && !token.IsCancellationRequested)
                {
                    var diedCount = context.UnitsDied;
                    var progress = (diedCount / context.UnitsTotalMax) * 100; // TODO Improve game progress
                    var level = (int) levelStat.Current;
                    var time = context.Time;

                    if (hasUnitsCfg)
                        ExecuteScenario(scenarioCfg.unitsScenario, level, diedCount, progress, time, SpawnUnits);

                    if (hasObjectsCfg)
                        ExecuteScenario(scenarioCfg.objectsScenario, level, diedCount, progress, time, SpawnObjects);

                    if (hasEffectsCfg)
                        ExecuteScenario(scenarioCfg.effectsScenario, level, diedCount, progress, time, SpawnEffects);

                    await UniTask.Delay(tickDelay, cancellationToken: token);

                    if (CheckScenarioEnded(hpStat.Current))
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                End();
            }
        }

        private void ExecuteScenario<T>(
            IEnumerable<T> configs,
            int level,
            int diedCount,
            int progress,
            int time,
            Func<T, bool> execute) where T : SpawnConfig
        {
            foreach (var cfg in configs.Where(cfg => ProcessConditions(cfg, level, diedCount, progress, time) && execute(cfg)))
                PassConditions(cfg, level, diedCount, progress, time);
        }
        
        private bool SpawnUnits(SpawnConfig cfg)
        {
            var available = context.UnitsSceneMax - context.UnitsAlive;
            if (available <= 0)
                return false;
            
            available = Math.Min(available, cfg.entities.Count);
            for (var index = 0; index < available; index++)
            {
                var unitCfg = positionProvider.Apply(cfg.Id + 1, index, cfg.entities, cfg.idsOrderFunction);
                var point = spawnPointProvider.Get(cfg.Id, index, cfg.spawnIds, cfg.spawnFunction);
                unitEntityFactory.CreateAsync((UnitConfig)unitCfg, point.Position).Forget();
            }
            return true;
        }

        private bool SpawnObjects(SpawnConfig cfg) => true; // TODO implement 
        private bool SpawnEffects(SpawnConfig cfg) => true; // TODO implement 

        protected virtual bool CheckScenarioEnded(float playerHealth) // TODO implement more specific conditions
            => playerHealth <= 0 || (context.UnitsLeft <= 0 && context.UnitsAlive <= 0);

        private static bool ProcessConditions(
            SpawnConfig cfg,
            int level,
            int diedCount,
            int progress,
            int time)
        {
            return (!cfg.whenUnitsDied.enabled || cfg.whenUnitsDied.IsConditionTrue(diedCount))
                   && (!cfg.whenGameProgress.enabled || cfg.whenGameProgress.IsConditionTrue(progress))
                   && (!cfg.whenLevelUp.enabled || cfg.whenLevelUp.IsConditionTrue(level))
                   && (!cfg.whenGameTime.enabled || cfg.whenGameTime.IsConditionTrue(time));
        }

        private static void PassConditions(
            SpawnConfig cfg,
            int level,
            int diedCount,
            int progress,
            int time)
        {
            cfg.whenUnitsDied.Passed(diedCount);
            cfg.whenGameProgress.Passed(progress);
            cfg.whenLevelUp.Passed(level);
            cfg.whenGameTime.Passed(time);
        }

        private static void SetDefault(SpawnConfig cfg)
        {
            cfg.whenUnitsDied.Default();
            cfg.whenGameProgress.Default();
            cfg.whenLevelUp.Default();
            cfg.whenGameTime.Default();
        }
    }
}