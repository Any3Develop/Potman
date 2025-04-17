using Potman.Common.DependencyInjection;
using Potman.Common.Pools.Abstractions;
using Potman.Game.Entities.Units.Abstractions.Swarming;

namespace Potman.Game.Entities.Units.Swarming
{
    public class UnitPathFactory : IUnitPathFactory
    {
        private readonly IAbstractFactory abstractFactory;
        private readonly IPool<IUnitPath> pathPool;

        public UnitPathFactory(IAbstractFactory abstractFactory, IPool<IUnitPath> pathPool)
        {
            this.abstractFactory = abstractFactory;
            this.pathPool = pathPool;
        }

        public IUnitPath Create(int ownerId)
        {
            if (pathPool.TrySpawn(out UnitNavMeshPath path))
            {
                path.Init(ownerId);
                return path;
            }
            
            path = abstractFactory.Create<UnitNavMeshPath>();
            path.Init(ownerId);
            pathPool.Add(path, true);
            return path;
        }
    }
}