using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Potman.Common.DependencyInjection;
using Potman.Common.ResourceManagament;
using Potman.Common.Utilities;
using Potman.Game.Context.Abstractions;
using Potman.Game.Entities.Abstractions;
using Potman.Game.Entities.Units.Abstractions;
using Potman.Game.Entities.Units.Data;
using UnityEngine;

namespace Potman.Game.Entities.Units
{
    public class UnitEntityFactory : IUnitEntityFactory, IDisposable
    {
        private readonly IGameContext gameContext;
        private readonly IRuntimePool runtimePool;
        private readonly IAbstractFactory abstractFactory;
        private readonly ResourceCache<UnitEntity> resourceCache;

        public UnitEntityFactory(
            IGameContext gameContext,
            IRuntimePool runtimePool,
            IAbstractFactory abstractFactory)
        {
            this.gameContext = gameContext;
            this.runtimePool = runtimePool;
            this.abstractFactory = abstractFactory;
            resourceCache = new ResourceCache<UnitEntity>();
        }
        
        public void Dispose()
        {
            resourceCache.Release();
        }

        public async UniTask<IUnitEntity> CreateAsync(UnitConfig cfg, Vector3 position)
        {
            if (runtimePool.TrySpawn(x => x.PoolableId == cfg.entityPrefabId, out UnitEntity entity, false))
                return Spawn(entity, cfg, position);

            var prefab = await resourceCache.GetAsync(cfg.entityPrefabId);
            entity = abstractFactory.CreateUnityObject<UnitEntity>(prefab);
            entity.SetPoolableId(cfg.entityPrefabId);
            entity.Construct(gameContext);
            runtimePool.Add(entity, true, false);
            
            return Spawn(entity, cfg, position);
        }

        private static IUnitEntity Spawn(UnitEntity entity, UnitConfig cfg, Vector3 position)
        {
            entity.name = $"{cfg.Id}_{cfg.entityPrefabId}"; // TODO для облегчения тестирования
            entity.Init(cfg, position);
            entity.Spawn();
            return entity;
        }
    }
}