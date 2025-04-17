using UnityEngine;

namespace Potman.AbilityTree
{
    [NodeWidth(375)]
    public class PerkLinkedNode : PerkNodeBase
    {
        [Tooltip("Это поле нужно для подключения перков из текущего или других графов, без явной связи системы нодов (полоска от точки к точке)")]
        public PerkNodeBase perkNode;

        public override string Id => perkNode ? perkNode.name : base.Id;
        public override string GraphId => perkNode? perkNode.graph.name : base.GraphId;
        protected override void Init()
        {
            base.Init();
            if (perkNode)
                self = perkNode;
        }

        private void OnValidate()
        {
            Init();
            if (!perkNode)
                Debug.LogError($"Внимание, этот узел [{GraphId}/{Id}] не имеет связанного узла, выберите узел!");
        }
    }
}