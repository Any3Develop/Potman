using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Potman.Common.SceneService.Abstractions;
using Potman.Common.UIService.Abstractions;
using Potman.Game.Abilities.Data;
using Potman.Game.Entities.Player.Data;
using Potman.Game.Scenarios.Data;
using Potman.Lobby.Identity;
using R3;
using UnityEngine;

namespace Potman.Lobby.UI.GameModes
{
    public interface IGameModesViewModel
    {
        UniTask LoadAsync();
        ReactiveProperty<GameModeData> Current { get; }
        IEnumerable<GameModeData> Datas { get; }
    }

    public class GameModesViewModel : IDisposable, IGameModesViewModel
    {
        private readonly IUIService uiService;
        private readonly ISceneService sceneService;
        public ReactiveProperty<GameModeData> Current { get; }
        public IEnumerable<GameModeData> Datas { get; }


        public GameModesViewModel(
            IUIService uiService,
            ISceneService sceneService,
            IEnumerable<GameModeData> modes)
        {
            this.uiService = uiService;
            this.sceneService = sceneService;
            Datas = modes;
            Current = new ReactiveProperty<GameModeData>();
        }

        
        public UniTask LoadAsync()
        {
            try
            {
                Current.Skip(1).Subscribe(OnModeSelected);
                uiService.Begin<GameModesWindow>()
                    .WithInit(x => x.Bind(this))
                    .Show();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            
            return UniTask.CompletedTask;
        }

        private void OnModeSelected(GameModeData data)
        {
            uiService.Begin<GameModesWindow>().Hide();
            
            //TODO For example
            UserIdentity.Redirections.Add(new ScenarioData
            {
                Id = data.Id,
                Level = 0
            });
            
            UserIdentity.Redirections.Add(new PlayerData
            {
                Id = PlayerId.Potman, 
                Abilities = new List<AbilityData> 
                {
                    // If you want, you can buff an ability, just add the stat data and it will be merged with the base stat in the game
                    new() {Id = AbilityId.BattleAbility0},
                    new() {Id = AbilityId.BattleAbility1},
                    new() {Id = AbilityId.BattleAbility2},
                }
            });
            //TODO For example
            
            sceneService.LoadAsync(data.Scene).Forget();
        }

        public void Dispose()
        {
            Current?.Dispose();
        }
    }
}