using System.Linq;
using DG.Tweening;
using Potman.Common.Events;
using Potman.Common.Pools;
using Potman.Common.Utilities;
using Potman.Game.Abilities.Abstractions;
using Potman.Game.Context.Abstractions;
using Potman.Game.Entities.Abstractions;
using Potman.Game.Entities.Units.Abstractions;
using Potman.Game.Entities.Units.Data;
using Potman.Game.Entities.Units.Events;
using Potman.Game.Stats.Abstractions;
using Potman.Game.Stats.Data;
using Potman.Game.Stats.Utils;
using Unity.Behavior;
using UnityEngine;

namespace Potman.Game.Entities.Units
{
    [RequireComponent(typeof(IEntityMapper))]
    [DisallowMultipleComponent]
    public class UnitEntity : PoolableObject, IUnitEntity
    {
        private Vector3 initScale;
        private Transform self;
        private Tween shake;
        private IRuntimePool runtimePool;
        private BehaviorGraphAgent behaviorGraph;

        #region IRuntimeEntity Impl
        public bool IsAlive => IsSpawned && StatsCollection.TryGet(StatType.Heath, out var health) && health.Current > 0;
        public IEntityMapper Mapper { get; private set; }
        public IStatsCollection StatsCollection { get; private set; }
        public IAbilityCollection AbilityCollection { get; private set; }
        public IGameContext GameContext { get; private set; }
        #endregion

        #region IPlayerEntity Impl
        public UnitConfig Config { get; private set; }
        public IUnitMovement Movement { get; private set; }
        #endregion

        internal UnitEntity Construct(IGameContext gameContext)
        {   
            Mapper = GetComponent<IEntityMapper>();
            Movement = Mapper.Map<IUnitMovement>();
            behaviorGraph = Mapper.Map<BehaviorGraphAgent>();
            Movement.Enable(false);
            
            GameContext ??= gameContext;
            runtimePool ??= gameContext.ServiceProvider.GetRequiredService<IRuntimePool>();
            AbilityCollection ??= gameContext.ServiceProvider.GetRequiredService<IAbilityCollection>();
            StatsCollection ??= gameContext.ServiceProvider.GetRequiredService<IStatsCollection>();
            Mapper.RecursiveMapAll<IEntityComponent>().Reverse().Each(x => x.Init());
            
            // TODO For visual feedback, temp
            self = Mapper.RecursiveMap<IRuntimeEntityView>().Mapper.Map<Transform>("Root");
            initScale = self.localScale;
            // TODO For visual feedback, temp
            return this;
        }
        
        internal void Init(
            UnitConfig config,
            Vector3 position)
        {
            if (DisposedLog())
                return;
            
            Config = config;
            StatsCollection.Merge(config.stats);
            behaviorGraph.Graph = Config.behavioursSet.Get(config.behaviorType);
            
            AbilityCollection.ForEach(x => x.Enable(true));
            Movement.Enable(true);
            Movement.Move(position);
        }

        public bool ApplyDamage(float damage) // TODO MOVE
        {
            if (!IsAlive && damage <= 0)
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
            if (IsDisposed || !IsSpawned || IsAlive)
                return;

            runtimePool.Release(this);
            MessageBroker.Publish(new UnitDiedEvent());
        }

        protected override void OnSpawned()
        {
            Movement.Enable(true);
            behaviorGraph.Init();
            MessageBroker.Publish(new UnitSpawnedEvent());
        }

        protected override void OnReleased()
        {
            if (behaviorGraph.Graph)
            {
                behaviorGraph.End();
                DestroyImmediate(behaviorGraph.Graph);
                behaviorGraph.Graph = null;
            }

            Movement.Enable(false);
            AbilityCollection.ForEach(x => x.Enable(false));
            AbilityCollection.Clear();
            StatsCollection.Clear();
            Config = null;
        }

        protected override void OnDisposed()
        {
            Mapper.RecursiveMapAll<IEntityComponent>().Reverse().Each(x => x?.Dispose());
            runtimePool = null;
            behaviorGraph = null;
            StatsCollection = null;
            AbilityCollection = null;
            GameContext = null;
            Movement = null;
            Mapper = null;
            Config = null;
        }
    }
}