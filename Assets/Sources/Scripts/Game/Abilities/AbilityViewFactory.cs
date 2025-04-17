using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Potman.Common.DependencyInjection;
using Potman.Common.Pools.Abstractions;
using Potman.Common.ResourceManagament;
using Potman.Common.Utilities;
using Potman.Game.Abilities.Abstractions;
using Potman.Game.Abilities.Data;
using Potman.Game.Abilities.Views;

namespace Potman.Game.Abilities
{
    public class AbilityViewFactory : IAbilityViewFactory, IDisposable
    {
        private readonly IAbstractFactory abstractFactory;
        private readonly IPool<IAbilityView> runtimeViewPool;
        private readonly Dictionary<AbilityId, ResourceCache<AbilityBaseView>> preloaded;

        public AbilityViewFactory(
            IAbstractFactory abstractFactory, 
            IPool<IAbilityView> runtimeViewPool)
        {
            this.abstractFactory = abstractFactory;
            this.runtimeViewPool = runtimeViewPool;
            preloaded = new Dictionary<AbilityId, ResourceCache<AbilityBaseView>>();
        }

        public async UniTask<IAbilityView> CreateAsync(IAbility ability, int index = 0)
        {
            if (index < 0 || index >= ability.Config.prefabIds.Count)
                throw new IndexOutOfRangeException($"Provide correct index to create : {nameof(IAbilityView)}, incoming index: {index}, available indices from 0 up to : {ability.Config.prefabIds.Count-1}");

            if (!preloaded.TryGetValue(ability.Config.id, out var resourceCache))
            {
                preloaded[ability.Config.id] = resourceCache = new ResourceCache<AbilityBaseView>();
                await resourceCache.PreloadAsync(ability.Config.prefabIds);
            }
            
            var prefab = resourceCache[index];
            if (runtimeViewPool.TrySpawn(result => result.PoolableId == prefab.PoolableId, out AbilityBaseView view, false))
            {
                view.Init(ability);
                view.Spawn();
                return view;
            }

            view = abstractFactory.CreateUnityObject<AbilityBaseView>(prefab);
            view.Construct();
            runtimeViewPool.Add(view, true, false);

            view.Init(ability);
            view.Spawn();
            return view;
        }

        public async UniTask<TView> CreateAsync<TView>(IAbility ability, int index = 0) where TView : IAbilityView
            => (TView) await CreateAsync(ability, index);

        public void Dispose()
        {
            preloaded.Values.Each(x => x.Release());
            preloaded.Clear();
        }
    }
}