using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Potman.Common.ResourceManagament.Exceptions;
using Potman.Common.ResourceManagament.Extensions;
using Potman.Common.Utilities.DiscSpace;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Potman.Common.ResourceManagament
{
	// See an expression in the asmdef to enable this feature
	// https://forum.unity.com/threads/detect-if-a-package-is-installed.1100338/
#if ADDRESSABLES_INCLUDED
	public class AddressableResourceService : IResourceService
	{
		private readonly string dataPath = Application.persistentDataPath;
		private bool initialized;
		private bool preloaded;

		public async UniTask InitializeAsync(IProgress<float> progress = null)
		{
			progress ??= new ProgressMock();
			if (initialized)
			{
				progress.OnSkip();
				return;
			}
			
			try
			{
				ClearCacheCatalog();
				await Addressables.InitializeAsync(true).ToUniTask();
				initialized = true;
			}
			catch (Exception)
			{
				initialized = false;
				throw;
			}
			finally
			{
				progress.OnSkip();
			}
		}

		public async UniTask UpdateAllAsync(IProgress<float> progress = null)
		{
			if (!initialized)
				throw new NotInitializedException();
			
			progress ??= new ProgressMock();
			if (preloaded)
			{
				progress.OnSkip();
				return;
			}
			
			try
			{
				var updatableCatalogs = await Addressables.CheckForCatalogUpdates().ToUniTask();
				if (updatableCatalogs is {Count: > 0})
				{
					var locators = await Addressables.UpdateCatalogs(updatableCatalogs.ToArray()).ToUniTask();
					await UpdateInternalCatalogsAsync(locators, progress); // updating
				}
				preloaded = true;
			}
			catch (Exception)
			{
				preloaded = false;
				throw;
			}
			finally
			{
				progress.OnSkip();
			}
		}

		public async UniTask<T> GetAsync<T>(string id, CancellationToken token = default) where T : Object
		{
			if (!initialized)
				throw new NotInitializedException();
			
			if (string.IsNullOrEmpty(id))
				throw new NullReferenceException($"Resource Id must be not null and empty, requested resource type : {typeof(T).Name}");

			ThrowIfCancelled(token);
			var handle = Addressables.LoadAssetAsync<T>(id);

			try
			{
				while (!handle.IsDone)
				{
					ThrowIfCancelled(token);
					await UniTask.Yield();
				}
				
				ThrowIfCancelled(token);
				if (handle.OperationException != null)
					throw new Exception($"Resource not loaded for id : {id}, " +
					                    $"requested resource type : {typeof(T).Name}, " +
					                    $"original Exception : {handle.OperationException}");
				
				if (!handle.Result)
					throw new ApplicationException($"Internal error resource not loaded for id : {id}, " +
					                               $"requested resource type : {typeof(T).Name}");
				
				return handle.Result;
			}
			catch (OperationCanceledException)
			{
				Addressables.Release(handle); // do not use in finally, it can release resource when errors

				// Override the exception
				throw new OperationCanceledException($"Task was cancelled, resourceId : {id}, " +
				                                     $"requested resource type {typeof(T).Name}");
			}
			catch (Exception)
			{
				Addressables.Release(handle); // do not use in finally, it can release resource when errors
				throw;
			}
		}

		public async UniTask<IList<T>> GetAllAsync<T>(string id, CancellationToken token = default) where T : Object
		{
			if (!initialized)
				throw new NotInitializedException();
			
			if (string.IsNullOrEmpty(id))
				throw new NullReferenceException($"Resource Id must be not null and empty, requested resource type : {typeof(T).Name}");

			ThrowIfCancelled(token);
			var handle = Addressables.LoadAssetsAsync<T>(id);

			try
			{
				while (!handle.IsDone)
				{
					ThrowIfCancelled(token);
					await UniTask.Yield();
				}
				
				ThrowIfCancelled(token);
				if (handle.OperationException != null)
					throw new Exception($"Resource not loaded, " +
					                    $"requested resource type : {typeof(T).Name}, " +
					                    $"original Exception : {handle.OperationException}");
				
				if (handle.Result is not {Count: > 0})
					throw new ApplicationException($"Internal error resource not loaded for id : {id}, " +
					                               $"requested resource type : {typeof(T).Name}");
				
				return handle.Result;
			}
			catch (OperationCanceledException)
			{
				Addressables.Release(handle); // do not use in finally, it can release resource when errors

				// Override the exception
				throw new OperationCanceledException($"Task was cancelled, resourceId : {id}, " +
				                                     $"requested resource type {typeof(T).Name}");
			}
			catch (Exception)
			{
				Addressables.Release(handle); // do not use in finally, it can release resource when errors
				throw;
			}
		}
		
		public async UniTask<IList<T>> GetAllAsync<T>(IReadOnlyCollection<string> ids, CancellationToken token = default) where T : Object
		{
			if (!initialized)
				throw new NotInitializedException();
			
			if (ids is null or {Count: 0})
				throw new NullReferenceException($"Resource Id must be not null and empty, requested resource type : {typeof(T).Name}");

			ThrowIfCancelled(token);
			var handle = Addressables.LoadAssetsAsync<T>(ids, null, Addressables.MergeMode.UseFirst);

			try
			{
				while (!handle.IsDone)
				{
					ThrowIfCancelled(token);
					await UniTask.Yield();
				}
				
				ThrowIfCancelled(token);
				if (handle.OperationException != null)
					throw new Exception($"Resource not loaded, " +
					                    $"requested resource type : {typeof(T).Name}, " +
					                    $"original Exception : {handle.OperationException}");
				
				if (handle.Result is not {Count: > 0})
					throw new ApplicationException($"Internal error resource not loaded for ids : {string.Join(",", ids)}, " +
					                               $"requested resource type : {typeof(T).Name}");
				
				return handle.Result;
			}
			catch (OperationCanceledException)
			{
				Addressables.Release(handle); // do not use in finally, it can release resource when errors

				// Override the exception
				throw new OperationCanceledException($"Task was cancelled, resourceIds : {string.Join(",", ids)}, " +
				                                     $"requested resource type {typeof(T).Name}");
			}
			catch (Exception)
			{
				Addressables.Release(handle); // do not use in finally, it can release resource when errors
				throw;
			}
		}

		public void Release<T>(params T[] values) where T : Object
		{
			foreach (var obj in values)
			{
				Addressables.Release(obj);
			}
		}
		
		private async UniTask HandleProgress(IProgress<float> progress, ICollection<AsyncOperationHandle> handlers)
		{
			long downloaded = 0;
			var totalDownloadBytes = handlers.Sum(x=> x.GetDownloadStatus().TotalBytes);

			progress.OnStarted();
			while (downloaded < totalDownloadBytes)
			{
				ThrowIfCancelled();
				if (handlers.Any(x => x.Status == AsyncOperationStatus.Failed))
					throw new InternetConnectionException("Code : 400, Can't download, process failed.");
				
				downloaded = handlers.Sum(x => x.GetDownloadStatus().DownloadedBytes);
				progress.Report((float)downloaded / totalDownloadBytes);
				await RefrehTaskAsync();
			}
			progress.OnEnded();
		}

		private async UniTask UpdateInternalCatalogsAsync(
			IList<IResourceLocator> locators, 
			IProgress<float> progress)
		{
			if (locators.Count == 0)
			{
				progress.OnSkip();
				return;
			}

			var downloadSizeOperations = new List<AsyncOperationHandle<long>>();
			var downloadOperations = new List<AsyncOperationHandle>();

			try
			{
				downloadSizeOperations.AddRange(locators.Select(x => Addressables.GetDownloadSizeAsync(x.Keys)));
				var downloadSizeBytesList = await UniTask.WhenAll(downloadSizeOperations.Select(x => x.ToUniTask()));
				var totalSpaceToDownload = downloadSizeBytesList.Sum();
				
				if (totalSpaceToDownload <= 0)
				{
					progress.OnSkip();
					return;
				}
				
				DiscSpaceChecker.ThrowWhenNotEnoughMemoryFor(totalSpaceToDownload, dataPath);

				for (var i = 0; i < locators.Count; i++)
				{
					if (downloadSizeBytesList[i] <= 0)
						continue;

					// download and make new cache by keys
					downloadOperations.AddRange(locators[i].Keys
						.Select(key => Addressables.DownloadDependenciesAsync(key)));
				}

				if (downloadOperations.Count == 0)
				{
					progress.OnSkip();
					return;
				}

				await HandleProgress(progress, downloadOperations);
			}
			finally
			{
				downloadOperations.ForEach(Addressables.Release);
				downloadSizeOperations.ForEach(Addressables.Release);
				downloadSizeOperations.Clear();
				downloadOperations.Clear();
			}
		}

		private UniTask RefrehTaskAsync(CancellationToken token = default)
		{
			return UniTask.Delay(1000, cancellationToken:token); // refresh operations 1 seconds
		}

		private void ClearCacheCatalog()
		{
			var catalogPath = $"{dataPath}/com.unity.addressables";
			if(Directory.Exists(catalogPath))
			   Directory.Delete(catalogPath,true);
		}
		
		public static void ThrowIfCancelled(CancellationToken token = default)
		{
			token.ThrowIfCancellationRequested();
			if (!Application.isPlaying)
				throw new OperationCanceledException();
		}
	}
	#endif
}