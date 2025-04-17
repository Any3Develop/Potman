namespace Potman.Game.Entities.Units.Abstractions.Swarming
{
    public interface ISwarmAgentFactory
    {
        ISwarmAgent Create(params object[] args);
    }
}