using System.Linq;
using DG.Tweening;
using Potman.Common.InputSystem.Abstractions;
using Potman.Common.Pools;
using Potman.Common.Utilities;
using Potman.Game.Abilities.Abstractions;
using Potman.Game.Context.Abstractions;
using Potman.Game.Entities.Abstractions;
using Potman.Game.Entities.Player.Abstractions;
using Potman.Game.Entities.Player.Data;
using Potman.Game.Stats.Abstractions;
using Potman.Game.Stats.Data;
using UnityEngine;

namespace Potman.Game.Entities.Player
{
    [RequireComponent(typeof(IEntityMapper))]
    [DisallowMultipleComponent]
    public class PlayerEntity : PoolableObject, IPlayerEntity
    {
        private IRuntimePool runtimePool;
        private Vector3 initScale;
        private Transform self;
        private Tween shake;
        
        #region IRuntimeEntity Impl
        public bool IsAlive => IsSpawned && StatsCollection.TryGet(StatType.Heath, out var health) && health.Current > 0;
        public IEntityMapper Mapper { get; private set; }
        public IStatsCollection StatsCollection { get; private set; }
        public IAbilityCollection AbilityCollection { get; private set; }
        public IGameContext GameContext { get; private set; }
        #endregion

        #region IPlayerEntity Impl
        public PlayerConfig Config { get; private set; }
        public IPlayerMovement Movement { get; private set; }
        public IPlayerAnimator Animator { get; private set; }
        public IAimPositioning AimPositioning { get; private set; }
        public IAutoTargeting AutoTargeting { get; private set; }
        public IInputController<PlayerActions> Input { get; private set; }
        #endregion

        internal PlayerEntity Construct(IGameContext gameContext)
        {
            GameContext = gameContext;
            Mapper = GetComponent<IEntityMapper>();
            Animator = Mapper.Map<IPlayerAnimator>();
            Movement = Mapper.Map<IPlayerMovement>();
            AimPositioning = Mapper.Map<IAimPositioning>();
            AutoTargeting = Mapper.Map<IAutoTargeting>();
            runtimePool = gameContext.ServiceProvider.GetRequiredService<IRuntimePool>();
            Input = gameContext.ServiceProvider.GetRequiredService<IInputController<PlayerActions>>();
            Mapper.RecursiveMapAll<IEntityComponent>().Reverse().Each(x => x.Init());
            
            // TODO For visual feedback, temp
            self = Mapper.RecursiveMap<IRuntimeEntityView>().Mapper.Map<Transform>("Root");
            initScale = self.localScale;
            // TODO For visual feedback, temp
            return this;
        }

        protected override void OnDisposed()
        {
            Mapper.RecursiveMapAll<IEntityComponent>().Reverse().Each(x => x?.Dispose());
            Input = null;
            Config = null;
            Mapper = null;
            StatsCollection?.Clear();
            AbilityCollection?.Clear();
            AbilityCollection = null;
            StatsCollection = null;
            AimPositioning = null;
            GameContext = null;
            Movement = null;
            Animator = null;
        }

        internal void Init(
            Vector3 position,
            PlayerConfig config,
            IStatsCollection statsCollection, 
            IAbilityCollection abilityCollection)
        {
            Config = config;
            StatsCollection = statsCollection;
            AbilityCollection = abilityCollection;
            
            abilityCollection.ForEach(x => x.Enable(true));
            Animator.Enable(true);
            Movement.Enable(true);
            AutoTargeting.Enable(true);
            AimPositioning.Enable(true);
            Movement.SetPosition(position);
            Input.Enable(true);
        }
        
        public bool ApplyDamage(float damage) // TODO move
        {
            if (!IsAlive || damage <= 0)
                return false;
            
            if (StatsCollection.TryGet(StatType.Armor, out var armor))
                damage = Mathf.Max(damage - (damage * armor.Current), 0);
            
            Debug.Log($"{Config.name.Replace("Cfg", "")} : {nameof(ApplyDamage)}, damage : {damage}");
            
            if (StatsCollection.TryGet(StatType.Heath, out var health) && damage > 0)
            {
                health.Subtract(damage);
                shake?.Kill();
                self.localScale = initScale;
                shake = self.DOShakeScale(0.3f);
                TryDie();
                return true;
            }
            
            return false;
        }

        private void TryDie()
        {
            if (!IsSpawned || IsAlive)
                return;

            Input.Enable(false);
            Animator.Enable(false); // TODO Die Animation
            Movement.Enable(false);
            AimPositioning.Enable(false);
            AutoTargeting.Enable(false);
            AbilityCollection.ForEach(x => x.Enable(false));
            
            // TODO For visual feedback, temp
            foreach (var meshRenderer in Mapper.RecursiveMapAll<Renderer>())
                meshRenderer.material.color = Color.red;

            DOVirtual.DelayedCall(2f, () => runtimePool.Release(this));
            // TODO For visual feedback, temp
        }

        protected override void OnReleased()
        {
            Input.Enable(false);
            Animator.Enable(false);
            Movement.Enable(false);
            AimPositioning.Enable(false);
            AutoTargeting.Enable(false);
            AbilityCollection.ForEach(x => x.Enable(false));
            AbilityCollection?.Clear();
            StatsCollection?.Clear();
            Config = null;
            
            // TODO For visual feedback, temp
            foreach (var meshRenderer in Mapper.RecursiveMapAll<Renderer>())
                meshRenderer.material.color = Color.white;
            // TODO For visual feedback, temp
        }
    }
}