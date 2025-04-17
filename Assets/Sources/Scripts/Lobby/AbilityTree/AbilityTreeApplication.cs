using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Potman.Common.LifecycleService.Abstractions;
using Potman.Lobby.AbilityTree.Abstractions;
using Potman.Lobby.AbilityTree.Data;
using UnityEngine;

namespace Potman.Lobby.AbilityTree
{
    public class AbilityTreeApplication : IAbilityTreeApplication
    {
        private readonly IAbilityTreeCollection abilityTreeCollection;

        public AbilityTreeApplication(IAbilityTreeCollection abilityTreeCollection)
        {
            this.abilityTreeCollection = abilityTreeCollection;
        }

        public UniTask LoadAsync()
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Objects
            };

            try
            {
                var graphs = Resources.LoadAll<TextAsset>("Lobby/AbilityTree").Select(x => x.text);
                foreach (var jsonContent in graphs)
                {
                    var nodes = JsonConvert.DeserializeObject<AbilityNode[]>(jsonContent, settings);
                    abilityTreeCollection.AddRange(nodes);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return UniTask.CompletedTask;
        }
    }
}