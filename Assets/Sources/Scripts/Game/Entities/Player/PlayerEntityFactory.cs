using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Potman.Common.DependencyInjection;
using Potman.Common.ResourceManagament;
using Potman.Common.Utilities;
using Potman.Game.Abilities.Abstractions;
using Potman.Game.Context.Abstractions;
using Potman.Game.Entities.Abstractions;
using Potman.Game.Entities.Player.Abstractions;
using Potman.Game.Entities.Player.Data;
using Potman.Game.Stats.Abstractions;
using Potman.Game.Stats.Utils;
using Unity.Cinemachine;
using UnityEngine;
using IServiceProvider = Potman.Common.DependencyInjection.IServiceProvider;
using Object = UnityEngine.Object;

namespace Potman.Game.Entities.Player
{
    public class PlayerEntityFactory : IPlayerEntityFactory, IDisposable
    {
        private const string pathFormat = "Game/PlayerConfigs/PlayerCfg_{0}";
        private readonly IGameContext gameContext;
        private readonly IRuntimePool runtimePool;
        private readonly IAbilityFactory abilityFactory;
        private readonly IAbstractFactory abstractFactory;
        private readonly IServiceProvider serviceProvider;
        private readonly ISpawnPointProvider spawnPointProvider;
        private readonly Dictionary<PlayerId, PlayerConfig> configs;
        private readonly ResourceCache<PlayerEntity> resourceCache;
        private readonly CinemachineCamera camera;

        public PlayerEntityFactory(
            IGameContext gameContext,
            IRuntimePool runtimePool,
            IAbilityFactory abilityFactory,
            IAbstractFactory abstractFactory, 
            IServiceProvider serviceProvider,
            ISpawnPointProvider spawnPointProvider)
        {
            this.gameContext = gameContext;
            this.runtimePool = runtimePool;
            this.abilityFactory = abilityFactory;
            this.abstractFactory = abstractFactory;
            this.serviceProvider = serviceProvider;
            this.spawnPointProvider = spawnPointProvider;
            
            camera = Object.FindAnyObjectByType<CinemachineCamera>(); // TODO Instantiate camera instead
            configs = new Dictionary<PlayerId, PlayerConfig>();
            resourceCache = new ResourceCache<PlayerEntity>();
        }

        public async UniTask<IPlayerEntity> CreateAsync(PlayerData playerData)
        {
            if (!configs.TryGetValue(playerData.Id, out var playerCfg))
                configs[playerData.Id] = playerCfg = await Resources
                    .LoadAsync(string.Format(pathFormat, playerData.Id))
                    .ToUniTask<PlayerConfig>();
            
            if (!playerCfg)
                throw new NullReferenceException($"Player with id : {playerData.Id} doesn't have the config.");
            
            if (runtimePool.TrySpawn(x => x.PoolableId == playerCfg.entityPrefabId, out PlayerEntity player, false))
                return Spawn(player, playerCfg, playerData);
            
            var prefab = await resourceCache.GetAsync(playerCfg.entityPrefabId);
            player = abstractFactory.CreateUnityObject<PlayerEntity>(prefab).Construct(gameContext);

            runtimePool.Add(player, true, false);
            return Spawn(player, playerCfg, playerData);
        }

        private IPlayerEntity Spawn(PlayerEntity player, PlayerConfig playerCfg, PlayerData playerData)
        {
            player.SetPoolableId(playerCfg.entityPrefabId);
            var point = spawnPointProvider.Get(playerCfg.id.ToString(), 0, playerCfg.spawnIds, playerCfg.spawnFunction);
            var abilityCollection = serviceProvider.GetRequiredService<IAbilityCollection>();
            abilityCollection.AddRange(playerData.Abilities.Select(data => abilityFactory.Create(data, player)));
            
            var statsCollection = serviceProvider
                .GetRequiredService<IStatsCollection>()
                .Merge(playerCfg.stats)
                .Merge(playerData.Stats);

            camera.Target.TrackingTarget = player.transform;
            
            player.Init(point.Position, playerCfg, statsCollection, abilityCollection);
            player.Spawn();
            return player;
        }

        public void Dispose()
        {
            resourceCache.Release();
            configs.Values.Each(Resources.UnloadAsset);
            configs.Clear();
        }
    }
}