using System;
using Cysharp.Threading.Tasks;
using Potman.Common.SceneService.Abstractions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Potman.Common.SceneService
{
	public class SceneService : ISceneService, IDisposable
	{
		public event Action<SceneId> OnSceneLoaded;

		public SceneId Current { get; private set; }

		public async UniTask LoadAsync(SceneId sceneId)
		{
			Debug.Log($"Previous scene : {Current}");
			Current = sceneId;
			Debug.Log($"Scene loading : {sceneId}");
			
			await SceneManager.LoadSceneAsync((int)sceneId, LoadSceneMode.Single).ToUniTask();
			OnSceneLoaded?.Invoke(sceneId);
		}
		
		public void Dispose()
		{
			OnSceneLoaded = null;
		}
	}
}