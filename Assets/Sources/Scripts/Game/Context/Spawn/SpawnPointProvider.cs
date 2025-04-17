using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Potman.Common.DependencyInjection;
using Potman.Common.ResourceManagament;
using Potman.Game.Context.Abstractions;
using Potman.Game.Context.Data.Spawn;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Potman.Game.Context.Spawn
{
    public class SpawnPointProvider : IDisposable, ISpawnPointProvider
    {
        private readonly IAbstractFactory abstractFactory;
        private readonly IPositionProvider positionProvider;
        private readonly ResourceCache<GameObject> resourceCache;
        private Dictionary<SpawnId, SpawnPoint> spawnMap;
        private Transform spawnPointsRoot;

        public SpawnPointProvider(
            IAbstractFactory abstractFactory, 
            IPositionProvider positionProvider)
        {
            this.abstractFactory = abstractFactory;
            this.positionProvider = positionProvider;
            resourceCache = new ResourceCache<GameObject>();
        }

        public async UniTask StartAsync(string spawnId)
        {
            positionProvider.ResetAll();
            if (spawnPointsRoot)
                Object.Destroy(spawnPointsRoot.gameObject);

            var prefab = await resourceCache.GetAsync(spawnId);
            spawnMap?.Clear();
            spawnPointsRoot = abstractFactory.CreateUnityObject(prefab).transform;
            spawnMap = spawnPointsRoot
                .GetComponentsInChildren<SpawnPoint>(true)
                .ToDictionary(key => key.Id);
        }

        public void End()
        {
            positionProvider.ResetAll();
        }

        public void Add(SpawnPoint point, bool defaultParent = true)
        {
            if (point != null && spawnMap.TryAdd(point.Id, point) && defaultParent)
                point.SetParent(spawnPointsRoot);
        }

        public void Remove(SpawnPoint point)
        {
            if (point)
                spawnMap.Remove(point.Id);
        }

        public void Remove(SpawnId id)
        {
            spawnMap.Remove(id);
        }

        public SpawnPoint Get(SpawnId id) => spawnMap[id];
        public SpawnPoint[] GetAll() => spawnMap.Values.ToArray();

        public SpawnPoint Get(string groupId, int index, List<SpawnId> ids, FunctionSelector selector)
            => Get(positionProvider.Apply(groupId, index, ids, selector));

        public void Dispose()
        {
            resourceCache.Release();
            spawnMap?.Clear();
            spawnMap = null;
        }
    }
}