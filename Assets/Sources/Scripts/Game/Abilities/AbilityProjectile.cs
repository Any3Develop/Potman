using Cysharp.Threading.Tasks;
using Potman.Game.Abilities.Abstractions;
using Potman.Game.Abilities.Data;
using Potman.Game.Abilities.Views;
using Potman.Game.Entities.Abstractions;
using Potman.Game.Stats.Abstractions;
using Potman.Game.Stats.Data;
using UnityEngine;

namespace Potman.Game.Abilities
{
    [Ability(AbilityId.BattleAbility0)]
    [Ability(AbilityId.BattleAbility1)]
    [Ability(AbilityId.BattleAbility2)]
    public class AbilityProjectile : AbilityBase
    {
        private IRuntimeStat countDown;
        private IRuntimeStat moveSpeed;
        private IRuntimeStat rangeLimit;
        private IRuntimeStat damage;
        private LayerMask ignorLayer;
        private Transform startPoint;
        private float lastUse;

        protected override void OnEnabled()
        {
            DespawnAll();
            lastUse = 0;
            startPoint = Owner.Mapper.RecursiveMap<Transform>("Spells_StartPoint");
            ignorLayer = (LayerMask) Owner.StatsCollection.Get(StatType.IgnoreLayers).Current;
            
            // TODO Apply owner's int pow, countdown reduction, attack range, etc. Or move it to another part to make it generic logic.
            countDown = StatsCollection.Get(StatType.Countdown);
            moveSpeed = StatsCollection.Get(StatType.MoveSpeed);
            rangeLimit = StatsCollection.Get(StatType.Range);
            damage = StatsCollection.Get(StatType.Damage);
        }

        protected override void OnDisabled()
        {
            DespawnAll();
            countDown = null;
            moveSpeed = null;
            rangeLimit = null;
            damage = null;
            lastUse = default;
            startPoint = default;
            ignorLayer = default;
        }
        
        public override bool CanExecute() => base.CanExecute()
                                             && Owner is {IsAlive: true}
                                             && lastUse < Time.time;

        protected override void OnExecute()
        {
            lastUse = Time.time + countDown.Current;
            ViewFactory.CreateAsync<AbilityProjectileView>(this).ContinueWith(view =>
            {
                view.ExcludeCollisionLayers(ignorLayer);
                view.SubscribeOnCollision(target => HandleCollision(view, target));
                view.Movement
                    .SetRangeLimit(rangeLimit.Current, () => Despawn(view))
                    .SetDirection(startPoint.forward)
                    .SetPosition(startPoint.position)
                    .SetRotation(startPoint.rotation)
                    .Apply(moveSpeed.Current);
            }).Forget();
        }

        private void HandleCollision(IAbilityView view, GameObject target)
        {
            Despawn(view);

            if (!target.TryGetComponent(out IEntityMapper mapper)
                || !mapper.TryMap(out IRuntimeEntity entity))
                return;

            entity.ApplyDamage(damage.Current);
        }

        private void Despawn(IAbilityView view) => ViewPool.Release(view);
        private void DespawnAll() => ViewPool.Release<IAbilityView>(x => x.AbilityId == Id);
    }
}