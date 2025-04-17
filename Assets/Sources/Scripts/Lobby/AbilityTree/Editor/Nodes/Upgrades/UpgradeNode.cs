using System.Collections.Generic;
using Potman.Lobby.AbilityTree.Data.Upgrades;
using XNode;

namespace Potman.AbilityTree.Upgrades
{
    public abstract class UpgradeNode : Node
    {
        [Output] public UpgradeNode self;
        public abstract IEnumerable<UpgradeModel> Upgrades { get; }

        // Use this for initialization
        protected override void Init()
        {
            self = this;
            base.Init();
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            // Check which output is being requested. 
            // In this node, there aren't any other outputs than "result".
            if (port.fieldName == nameof(self))
                return this;
            
            // Hopefully this won't ever happen, but we need to return something
            // in the odd case that the port isn't "result"
            return null;
        }
    }
}