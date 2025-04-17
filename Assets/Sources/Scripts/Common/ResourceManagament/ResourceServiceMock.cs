using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Potman.Common.ResourceManagament.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Potman.Common.ResourceManagament
{
	public class ResourceServiceMock : IResourceService
	{
		private void LogUsingMock()
		{
			Debug.Log("You trying to use " + $"{nameof(ResourceServiceMock)}");
		}

		public UniTask InitializeAsync(IProgress<float> progress = null)
		{
			progress?.OnSkip();
			LogUsingMock();
			return UniTask.CompletedTask;
		}

		public UniTask UpdateAllAsync(IProgress<float> progress = null)
		{
			progress?.OnSkip();
			LogUsingMock();
			return UniTask.CompletedTask;
		}

		public UniTask<T> GetAsync<T>(string id, CancellationToken token = default) where T : Object
		{
			LogUsingMock();
			return UniTask.FromResult(default(T));
		}

		public UniTask<IList<T>> GetAllAsync<T>(string id, CancellationToken token = default) where T : Object
		{
			LogUsingMock();
			return UniTask.FromResult<IList<T>>(new List<T>());
		}

		public UniTask<IList<T>> GetAllAsync<T>(IReadOnlyCollection<string> ids, CancellationToken token = default) where T : Object
		{
			LogUsingMock();
			return UniTask.FromResult<IList<T>>(new List<T>());
		}

		public void Release<T>(params T[] values) where T : Object
		{
			LogUsingMock();
		}
	}
}