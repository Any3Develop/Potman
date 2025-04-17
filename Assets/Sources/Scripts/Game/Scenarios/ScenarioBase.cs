using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Potman.Common.Events;
using Potman.Common.Utilities;
using Potman.Game.Scenarios.Abstractions;
using Potman.Game.Scenarios.Data;
using Potman.Game.Scenarios.Events;
using R3;

namespace Potman.Game.Scenarios
{
    public abstract class ScenarioBase : IScenario
    {
        private IDisposable subscriptions;
        protected readonly ReactiveProperty<ScenarioState> StateInternal = new(ScenarioState.None);
        
        protected bool IsDisposed { get; private set; }
        public ScenarioState State => StateInternal.CurrentValue;
        public virtual IEnumerable<IScenario> Nested { get; } = Enumerable.Empty<IScenario>();

        public void Start()
        {
            if (IsDisposed || !State.AnyFlags(ScenarioState.None))
                return;

            using var builder = new DisposableBuilder();
            builder.Add(StateInternal.Pairwise()
                .Select(x => new SenarioChangedEvent(x.Current, x.Previous, this))
                .Subscribe(MessageBroker.Publish));
            builder.Add(StateInternal);
            subscriptions = builder.Build();
            
            OnStartAsync()
                .ContinueWith(() => StateInternal.Value = ScenarioState.Playing)
                .Forget();
        }

        public void Pause(bool value)
        {
            if (IsDisposed 
                || State.AnyFlags(ScenarioState.None | ScenarioState.Ended)
                || (value && !State.AnyFlags(ScenarioState.Paused))
                || (!value && State.AnyFlags(ScenarioState.Paused)))
                return;

            if (value)
            {
                State.AddFlag(ScenarioState.Paused);
                OnPaused();
                return;
            }

            State.AddFlag(ScenarioState.Paused);
            OnResume();
        }

        public void End()
        {
            if (IsDisposed || State.AnyFlags(ScenarioState.None | ScenarioState.Ended))
                return;

            Pause(false);
            StateInternal.Value = State.AddFlag(ScenarioState.Ended);
            OnEnded();
            
            subscriptions?.Dispose();
            subscriptions = null;
        }

        public void Dispose()
        {
            if (IsDisposed || State.AllFlags(ScenarioState.None))
                return;

            subscriptions?.Dispose();
            subscriptions = null;
            OnDisposed();
            IsDisposed = true;
        }

        protected virtual UniTask OnStartAsync() => UniTask.CompletedTask;
        protected virtual void OnPaused() {}
        protected virtual void OnResume() {}
        protected virtual void OnEnded() {}
        protected virtual void OnDisposed() {}
    }
}