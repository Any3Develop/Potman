using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Potman.Common.ResourceManagament.Extensions
{
	public static class ResourceExtensions
	{
		// See an expression in the asmdef to enable this feature
		// https://forum.unity.com/threads/detect-if-a-package-is-installed.1100338/
		public static async UniTask LoadResourceAsync(
			this Object target,
			string resourceId,
			CancellationToken token = default,
			bool relesePrevious = true)
		{
			await LoadResourceInternalAsync(target, resourceId, relesePrevious, token);
		}

		public static void ReleaseResource(this Object target)
		{
			if (!target)
				return;

			Object resource;
			switch (target)
			{
				case RawImage rawImage : 
					resource = rawImage.texture;
					rawImage.texture = null;
					break;
				
				case Image image : 
					resource = image.sprite;
					image.sprite = null;
					break;
				
				case AudioSource audioSource : 
					resource = audioSource.clip;
					audioSource.clip = null;
					break;
				
				default: resource = target;
					break;
			}

			if (!resource)
				return;

			try
			{
				ResourceServiceAdapter.Instance.Release(resource);
			}
			catch (Exception e)
			{
				Debug.LogWarning(e.Message);
			}
		}
		
		private static async UniTask LoadResourceInternalAsync(
			this Object target,
			string resourceId,
			bool relesePrevious = true,
			CancellationToken token = default)
		{
			
			if (!target)
			{
				Debug.Log($"Missing {nameof(target)} reference, resource id : {resourceId}");
				return;
			}
			
			if (string.IsNullOrEmpty(resourceId))
			{
				Debug.LogError($"Missing or empty {nameof(resourceId)}, for target : {target.name}");
				return;
			}
			
			Object resource = default;
			try
			{
				resource = await LoadResourceInternalAsync(target, resourceId, token);
				token.ThrowIfCancellationRequested();
		
				if (relesePrevious)
					target.ReleaseResource();
				
				SetResourceInternal(resource, target);
			}
			catch (OperationCanceledException e)
			{
				ReleaseResource(resource);
				Debug.Log(e.Message); // it's not an error
				throw; // need rethrow to extern handle
			}
			catch (NullReferenceException e)
			{
				ReleaseResource(resource);
				Debug.Log(e.Message); // it's possible an error we log simple
				throw; // need rethrow to extern handle
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				ReleaseResource(resource);
				throw; // need rethrow to extern handle
			}
		}

		private static void SetResourceInternal(Object resource, Object target)
		{
			if (!target)
				throw new NullReferenceException("Target object is missing");
			
			if (!resource)
				throw new NullReferenceException($"Resource object is missing for : {target}");
			
			switch (resource, target)
			{
				case (Texture texture, RawImage rawImage) : 
					rawImage.texture = texture; 
					break;
				
				case (Sprite sprite, Image image) : 
					image.sprite = sprite; 
					break;
				
				case (AudioClip audioClip, AudioSource audioSource): 
					audioSource.clip = audioClip; 
					break;
				
				default: throw new NotImplementedException($"Can't set the resource : {resource.GetType().Name}, " +
				                                           $"Into target : {target.GetType().Name}");
			}
		}

		private static async UniTask<Object> LoadResourceInternalAsync(Object target, string id, CancellationToken token)
		{
			return target switch
			{
				RawImage => await ResourceServiceAdapter.Instance.GetAsync<Texture>(id, token),
				Image => await ResourceServiceAdapter.Instance.GetAsync<Sprite>(id, token),
				AudioSource => await ResourceServiceAdapter.Instance.GetAsync<AudioClip>(id, token),
				_ => throw new NotImplementedException($"Cant load resource for target type : {target.GetType().Name}")
			};
		}

		public static void OnStarted(this IProgress<float> progress)
		{
			if (progress is IContentUpdateProgress updateProgress)
				updateProgress.OnStarted();
		}
		
		public static void OnEnded(this IProgress<float> progress)
		{
			if (progress is IContentUpdateProgress updateProgress)
				updateProgress.OnEnded();
		}
		
		public static void OnSkip(this IProgress<float> progress)
		{
			if (progress is IContentUpdateProgress updateProgress)
			{
				updateProgress.OnSkip();
				return;
			}

			progress?.Report(1f);
		}
	}
}