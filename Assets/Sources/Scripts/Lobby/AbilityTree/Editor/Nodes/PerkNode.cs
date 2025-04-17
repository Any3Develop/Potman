using Potman.AbilityTree.Upgrades;
using Potman.Lobby.AbilityTree.Data;
using UnityEngine;

namespace Potman.AbilityTree
{
    [NodeWidth(375)]
    public class PerkNode : PerkNodeBase
    {
        [Tooltip("Иконка нужна для визуализации перка, например: жизни, атака, скилл или пассивное умение..")]
        public Sprite Icon;
        [Tooltip("Стоимости для прокачки этого перка. Может иметь смешанную цену. Пустой список: без затрат.")]
        public CostModel[] Costs;

        [Tooltip("Если нет 'Required' перков то один из путей 'Optional' сделает доступным для прокачки этот перк.")]
        [Input(typeConstraint = TypeConstraint.Inherited, connectionType = ConnectionType.Multiple)] public PerkNodeBase Optional;
        [Tooltip("Когда будут открыты все перки 'Required' то станет доступным для прокачки этот перк.")]
        [Input(typeConstraint = TypeConstraint.Inherited, connectionType = ConnectionType.Multiple)] public PerkNodeBase Required;
        [Tooltip("Какие улучшения будут выданы после открытия этого перка.")]
        [Input(typeConstraint = TypeConstraint.Inherited, connectionType = ConnectionType.Multiple)] public UpgradeNode Upgrades;
    }
}