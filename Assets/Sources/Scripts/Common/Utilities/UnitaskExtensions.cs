using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Potman.Common.Utilities
{
    public static class UnitaskExtensions
    {
        public static async UniTask<T> ToUniTask<T>(this ResourceRequest source) where T : Object
        {
            var resource = await source.ToUniTask();
            return resource as T;
        }
    }
}