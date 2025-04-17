using Potman.Common.DependencyInjection;
using Potman.Game.Entities.Units.Abstractions.Swarming;

namespace Potman.Game.Entities.Units.Swarming
{
    public class SwarmAgentFactory : ISwarmAgentFactory
    {
        private readonly IAbstractFactory abstractFactory;

        public SwarmAgentFactory(IAbstractFactory abstractFactory)
        {
            this.abstractFactory = abstractFactory;
        }

        public ISwarmAgent Create(params object[] args)
        {
            return abstractFactory.Create<SwarmAgent>(args);
        }
    }
}