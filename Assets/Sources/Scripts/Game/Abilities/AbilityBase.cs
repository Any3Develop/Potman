using System;
using Potman.Common.Pools.Abstractions;
using Potman.Game.Abilities.Abstractions;
using Potman.Game.Abilities.Data;
using Potman.Game.Context.Abstractions;
using Potman.Game.Entities.Abstractions;
using Potman.Game.Stats.Abstractions;
using Potman.Game.Stats.Data;
using R3;
using UnityEngine;

namespace Potman.Game.Abilities
{
    public abstract class AbilityBase : IAbility
    {
        private bool enabled;
        private IDisposable timer;
        protected IRuntimeStat LifeTime { get; private set; }
        protected IPool<IAbilityView> ViewPool{ get; private set; }
        protected IAbilityViewFactory ViewFactory{ get; private set; }

        public string Id { get; private set; }
        public bool Enabled => !IsDisposed && enabled;
        public AbilityType Type { get; private set; }
        public bool IsDisposed { get; private set; }
        public AbilityConfig Config { get; private set;}
        public IRuntimeEntity Owner { get; private set;}
        public IStatsCollection StatsCollection { get; private set; }
        public IGameContext GameContext { get; private set; }

        internal void Init(
            AbilityConfig config,
            IRuntimeEntity owner,
            IGameContext gameContext,
            IStatsCollection statsCollection)
        {
            Id = Guid.NewGuid().ToString();
            Owner = owner;
            Config = config;
            Type = config.type;
            GameContext = gameContext;
            StatsCollection = statsCollection;
            ViewPool = GameContext.ServiceProvider.GetRequiredService<IPool<IAbilityView>>();
            ViewFactory = GameContext.ServiceProvider.GetRequiredService<IAbilityViewFactory>();
            OnInit();
        }
        
        public void Dispose()
        {
            if (IsDisposed)
                return;

            OnExpired();
            Owner.AbilityCollection.Remove(this);
            
            OnDisposed();
            IsDisposed = true;
            timer?.Dispose();
            StatsCollection = null;
            GameContext = null;
            LifeTime = null;
            Config = null;
            timer = null;
            Owner = null;
        }

        public void Enable(bool value)
        {
            if (Enabled == value)
                return;

            enabled = value;
            if (Enabled)
            {
                LifeTime = StatsCollection.Get(StatType.LifeTime);
                if (Config.expire is AbilityExpire.Timer)
                    StartExpireTimer(out timer);
                
                OnEnabled();
                return;
            }
            
            OnDisabled();
            timer?.Dispose();
            LifeTime = null;
            timer = null;
        }

        public virtual bool CanExecute() => Enabled;

        public void Execute()
        {
            if (!CanExecute())
                return;

            OnExecute();
        }

        public bool TryStack(IAbility value, bool intergrate)
        {
            if (intergrate && Type != AbilityType.Integrated)
                Type = AbilityType.Integrated;
            
            // TODO реализовать стаки эффектов.
            throw new NotImplementedException();
        }

        public bool TryExpire(AbilityExpire value)
        {
            if (!Enabled
                || Config.expire == AbilityExpire.Ignore
                || value == AbilityExpire.Ignore
                || value != Config.expire)
                return false;
            
            if (value is AbilityExpire.AfterExecute or AbilityExpire.BeforeExecute)
                LifeTime.Subtract(1);
            
            if ((value == AbilityExpire.Timer && LifeTime.Current > 0) || LifeTime.Current != 0)
                return false;
            
            Dispose();
            return true;
        }

        protected virtual void StartExpireTimer(out IDisposable result)
        {
            result = Observable.EveryUpdate(UnityFrameProvider.Update).Subscribe(_ =>
            {
                LifeTime.Subtract(Time.deltaTime);
                TryExpire(AbilityExpire.Timer);
            });
        }

        protected virtual void OnInit(){}
        protected virtual void OnDisposed(){}
        protected virtual void OnEnabled(){}
        protected virtual void OnDisabled(){}
        protected virtual void OnExpired() {}
        protected abstract void OnExecute();
    }
}