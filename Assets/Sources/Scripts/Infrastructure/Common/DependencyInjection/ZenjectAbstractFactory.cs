using System;
using Potman.Common.DependencyInjection;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Potman.Infrastructure.Common.DependencyInjection
{
    public class ZenjectAbstractFactory : IAbstractFactory
    {
        private readonly IInstantiator instantiator;
        
        [Inject(Optional = true, Source = InjectSources.Local)]
        public ZenjectAbstractFactory(IInstantiator instantiator)
        {
            this.instantiator = instantiator;
        }

        public object Create(Type type, params object[] args) => instantiator.Instantiate(type, args);

        public T Create<T>(params object[] args) => instantiator.Instantiate<T>(args);

        public T CreateUnityObject<T>(Object prototype, Transform parent = null, params object[] args)
            => instantiator.InstantiatePrefabForComponent<T>(prototype, parent, args);
        
        public GameObject CreateUnityObject(Object prototype, Transform parent = null) 
            => instantiator.InstantiatePrefab(prototype, parent);

        public T AddComponent<T>(GameObject owner, params object[] args) where T : Component 
            => instantiator.InstantiateComponent<T>(owner, args);
    }
}