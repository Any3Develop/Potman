using System;
using Cysharp.Threading.Tasks;

namespace Potman.Common.SceneService.Abstractions
{
	public interface ISceneService
	{
		event Action<SceneId> OnSceneLoaded;
		SceneId Current { get; }
		UniTask LoadAsync(SceneId sceneId);
	}
}