using Potman.Common.Pools.Abstractions;

namespace Potman.Game.Entities.Units.Abstractions.Swarming
{
    public interface ISwarmMaster : ISpwanPoolable
    {
        int Id { get; }
        bool Available { get; }

        int Count { get; }
        ISwarmAgent Agent { get; }
        ISwarmAgent[] Agents { get; }

        bool Join(ISwarmAgent agent, bool asMaster = false, bool notify = true);
        void Disconnect(ISwarmAgent agent, bool notify = true);
    }
}