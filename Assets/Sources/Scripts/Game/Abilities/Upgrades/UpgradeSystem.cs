using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Potman.Common.Collections;
using Potman.Common.ResourceManagament;
using Potman.Game.Abilities.Abstractions.Upgrades;
using Potman.Game.Abilities.Data.Upgrades;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Potman.Game.Abilities.Upgrades
{
    public class UpgradeSystem : IUpgradeSystem
    {
        private readonly IRuntimeCollection<UpgradeConfig> repository;
        private readonly IResourceService resourceService;

        public UpgradeSystem(
            IRuntimeCollection<UpgradeConfig> repository, 
            IResourceService resourceService)
        {
            this.repository = repository;
            this.resourceService = resourceService;
        }

        public async UniTask StartAsync()
        {
            try
            {
                var configs = await resourceService.GetAllAsync<UpgradeConfig>("UpgradeConfig");
                repository.AddRange(configs);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void End()
        {
            resourceService.Release(repository.ToArray<Object>());
            repository.Clear();
        }
    }
}