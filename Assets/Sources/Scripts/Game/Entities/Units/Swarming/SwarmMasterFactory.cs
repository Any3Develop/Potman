using Potman.Common.DependencyInjection;
using Potman.Common.Pools.Abstractions;
using Potman.Game.Entities.Units.Abstractions.Swarming;
using UnityEngine;

namespace Potman.Game.Entities.Units.Swarming
{
    public interface ISwarmMasterFactory
    {
        ISwarmMaster Create();
    }

    public class SwarmMasterFactory : ISwarmMasterFactory
    {
        private readonly IPool<ISwarmMaster> masterPool;
        private readonly IAbstractFactory abstractFactory;

        public SwarmMasterFactory(
            IPool<ISwarmMaster> masterPool,
            IAbstractFactory abstractFactory)
        {
            this.masterPool = masterPool;
            this.abstractFactory = abstractFactory;
        }

        public ISwarmMaster Create()
        {
            Debug.LogWarning($"MasterPool free : {masterPool.Free.Count}, active : {masterPool.Active.Count}");
            if (masterPool.TrySpawn(out var master))
            {
                Debug.LogWarning("MasterPool spawned from pool master");
                return master;
            }

            Debug.LogWarning($"MasterPool created new master.");
            master = abstractFactory.Create<SwarmMaster>();
            masterPool.Add(master, true);
            return master;
        }
    }
}