using System.Collections.Generic;
using Potman.Common.Collections;
using Potman.Lobby.AbilityTree.Data;

namespace Potman.Lobby.AbilityTree.Abstractions
{
    public interface IAbilityTreeCollection : IRuntimeCollection<AbilityNode>
    {
        AbilityNode Get(string id, string graphId = null);
        IEnumerable<AbilityNode> GetAll(string graphId, bool asQuery = false);
    }
}