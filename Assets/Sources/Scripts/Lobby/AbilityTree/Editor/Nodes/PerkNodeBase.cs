using UnityEngine;
using XNode;

namespace Potman.AbilityTree
{
    public abstract class PerkNodeBase : Node
    {
        [Tooltip("Этот выход нужен для подключения в поля 'Required' и 'Optional' для образования связи прокачки.")]
        [Output] public PerkNodeBase self;
        public virtual string Id => name;
        public virtual string GraphId => graph.name;
        
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