using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;

namespace Potman.Common.ResourceManagament
{
    public interface IResourceService
    {
        /// <summary>
        /// Must be initialized first before use.
        /// </summary>
        /// <param name="progress">To track an initialization progress.</param>
        UniTask InitializeAsync(IProgress<float> progress = null);
        
        /// <summary>
        /// Preload or update resources.
        /// </summary>
        /// <param name="progress">To track an update or loading progress.</param>
        UniTask UpdateAllAsync(IProgress<float> progress = null);
        
        UniTask<T> GetAsync<T>(string id, CancellationToken token = default) where T : Object;
        UniTask<IList<T>> GetAllAsync<T>(string id, CancellationToken token = default) where T : Object;
        UniTask<IList<T>> GetAllAsync<T>(IReadOnlyCollection<string> ids, CancellationToken token = default) where T : Object;
        void Release<T>(params T[] values) where T : Object;
    }
}