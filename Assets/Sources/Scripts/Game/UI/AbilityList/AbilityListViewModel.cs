using System;
using System.Collections.Generic;
using System.Linq;
using Potman.Common.Events;
using Potman.Common.InputSystem.Abstractions;
using Potman.Common.LifecycleService.Abstractions;
using Potman.Common.UIService.Abstractions;
using Potman.Game.Abilities.Abstractions;
using Potman.Game.Context.Abstractions;
using Potman.Game.Entities.Player.Data;
using Potman.Game.Scenarios.Data;
using Potman.Game.Scenarios.Events;
using R3;
using UnityEngine;

namespace Potman.Game.UI.AbilityList
{
    public interface IAbilityListViewModel
    {
        ReactiveProperty<IAbility> Current { get; }
        IEnumerable<IAbility> Datas { get; }
    }
    
    public class AbilityListViewModel : IInitable, IDisposable, IAbilityListViewModel
    {
        private readonly IUIService uiService;
        private readonly IGameContext gameContext;
        private readonly IInputAction shiftLeft;
        private readonly IInputAction shiftRight;
        private readonly IInputAction attack;
        private IDisposable executeHolding;
        
        public ReactiveProperty<IAbility> Current { get; }
        public IEnumerable<IAbility> Datas => gameContext.Player.AbilityCollection;


        public AbilityListViewModel(
            IUIService uiService,
            IGameContext gameContext,
            IInputController<PlayerActions> input)
        {
            this.uiService = uiService;
            this.gameContext = gameContext;
            shiftLeft = input.Get(PlayerActions.ShiftLeft);
            shiftRight = input.Get(PlayerActions.ShiftRight);
            attack = input.Get(PlayerActions.Attack);
            Current = new ReactiveProperty<IAbility>();
        }

        public void Initialize()
        {
            try
            {
                MessageBroker.Receive<SenarioChangedEvent>()
                    .Where(evData => evData.Current == ScenarioState.Playing)
                    .Take(1)
                    .Subscribe(_ => Start());
            
                MessageBroker.Receive<SenarioChangedEvent>()
                    .Where(evData => evData.Current == ScenarioState.Ended)
                    .Take(1)
                    .Subscribe(_ => End());
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void Dispose()
        {
            Current?.Dispose();
        }

        private void Start()
        {
            Current.OnNext(gameContext.Player.AbilityCollection.First());
            shiftLeft.OnPerformed += OnAbilityShiftLeft;
            shiftRight.OnPerformed += OnAbilityShiftRight;
            attack.OnPerformed += OnAbilityExecute;
            uiService.Begin<AbilityListWindow>()
                .WithInit(x => x.Bind(this))
                .Show();
        }

        private void End()
        {
            executeHolding?.Dispose();
            executeHolding = null;
            shiftLeft.OnPerformed -= OnAbilityShiftLeft;
            shiftRight.OnPerformed -= OnAbilityShiftRight;
            attack.OnPerformed -= OnAbilityExecute;
            uiService.Begin<AbilityListWindow>().Hide();
        }

        private void OnAbilityExecute(IInputContext context) // TODO for system tests
        {
            if (!context.Performed)
                return;
            
            if (executeHolding == null)
            {
                executeHolding = Observable.EveryUpdate().Subscribe(_ => Current.Value?.Execute());
                return;
            }
            
            executeHolding?.Dispose();
            executeHolding = null;
        }
        
        private void OnAbilityShiftLeft(IInputContext context)
        {
            var abilityCollection = gameContext.Player.AbilityCollection;
            var currentIndex = abilityCollection.IndexOf(Current.Value);
            currentIndex = (currentIndex - 1 + abilityCollection.Count) % abilityCollection.Count;
            Current.OnNext(abilityCollection[currentIndex]);
        }

        private void OnAbilityShiftRight(IInputContext context)
        {
            var abilityCollection = gameContext.Player.AbilityCollection;
            var currentIndex = abilityCollection.IndexOf(Current.Value);
            currentIndex = (currentIndex + 1) % abilityCollection.Count;
            Current.OnNext(abilityCollection[currentIndex]);
        }

    }
}