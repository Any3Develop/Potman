using System;
using System.Collections.Generic;
using System.Linq;
using Potman.Common.Collections;
using Potman.Common.DependencyInjection;
using Potman.Common.SerializeService.Abstractions;
using Potman.Game.Abilities.Abstractions;
using Potman.Game.Abilities.Data;
using Potman.Game.Context.Abstractions;
using Potman.Game.Entities.Abstractions;
using Potman.Game.Stats.Abstractions;
using Potman.Game.Stats.Utils;
using UnityEngine;

namespace Potman.Game.Abilities
{
    public class AbilityFactory : IAbilityFactory, IDisposable
    {
        private readonly IAbstractFactory abstractFactory;
        private readonly IGameContext gameContext;
        private readonly TypeCollection<AbilityId,  AbilityAttribute> typeCollection;
        private readonly Dictionary<AbilityId, AbilityConfig> configs;

        public AbilityFactory(
            IAbstractFactory abstractFactory,
            ISerializeService serializeService,
            IGameContext gameContext)
        {
            this.abstractFactory = abstractFactory;
            this.gameContext = gameContext;
            typeCollection = new SerializedTypeCollection<AbilityId, AbilityAttribute>(serializeService, att => att.Id, typeof(IAbility));
            configs = Resources.LoadAll<AbilityConfig>("Game/AbilityConfigs").ToDictionary(x => x.id);
        }
        
        public IAbility Create(AbilityId id, IRuntimeEntity owner)
        {
            if (!typeCollection.TryGet(id, out var type))
                throw new NullReferenceException($"Can't create an {nameof(IAbility)} with id : {id}, because it's type is not registered.");

            var cfg = configs[id];
            var ability = (AbilityBase)abstractFactory.Create(type);
            var statsCollection = gameContext.ServiceProvider.GetRequiredService<IStatsCollection>().Merge(cfg.stats);
            ability.Init(cfg, owner, gameContext, statsCollection);
            
            return ability;
        }

        public IAbility Create(AbilityData data, IRuntimeEntity owner)
        {
            var ability = Create(data.Id, owner);
            ability.StatsCollection.Merge(data.Stats);
            return ability;
        }

        public void Dispose()
        {
            configs.Clear();
        }
    }
}