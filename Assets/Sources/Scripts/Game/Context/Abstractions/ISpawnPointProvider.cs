using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Potman.Game.Context.Data.Spawn;
using Potman.Game.Context.Spawn;

namespace Potman.Game.Context.Abstractions
{
    public interface ISpawnPointProvider
    {
        UniTask StartAsync(string spawnId);
        void End();

        void Add(SpawnPoint point, bool defaultParent = true);
        void Remove(SpawnPoint point);
        void Remove(SpawnId id);
        SpawnPoint Get(SpawnId id);
        SpawnPoint[] GetAll();
        SpawnPoint Get(string groupId, int index, List<SpawnId> ids, FunctionSelector selector);
        void Dispose();
    }
}