using System.Collections.Generic;
using System.Linq;
using Potman.Common.Collections;
using Potman.Lobby.AbilityTree.Abstractions;
using Potman.Lobby.AbilityTree.Data;

namespace Potman.Lobby.AbilityTree
{
    public class AbilityTreeCollection : RuntimeCollection<AbilityNode>, IAbilityTreeCollection
    {
        public AbilityNode Get(string id, string graphId = null)
        {
            return GetAll(graphId, true).FirstOrDefault(x => x.Id == id);
        }
        
        public IEnumerable<AbilityNode> GetAll(string graphId, bool asQuery = false)
        {
            if (string.IsNullOrEmpty(graphId))
                return Enumerable.Empty<AbilityNode>();
            
            return asQuery ? this.Where(x => x.GraphId == graphId) : this.Where(x => x.GraphId == graphId).ToArray();
        }
    }
}