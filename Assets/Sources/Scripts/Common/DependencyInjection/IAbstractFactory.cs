using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Potman.Common.DependencyInjection
{
    public interface IAbstractFactory
    {
        object Create(Type type, params object[] args);
        
        T Create<T>(params object[] args);
        T CreateUnityObject<T>(Object prototype, Transform parent = null, params object[] args);
        GameObject CreateUnityObject(Object prototype, Transform parent = null);
        T AddComponent<T>(GameObject owner, params object[] args) where T : Component;
    }
}