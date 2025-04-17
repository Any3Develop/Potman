using System.Threading;
using System.Threading.Tasks;
using Potman.Common.Utilities;
using UnityEngine;

namespace Potman.Common.UIService
{
	public abstract class UISafeWindowBase : UIWindowBase
	{
		protected virtual int InteractableDelayMs => 500;
		private CancellationTokenSource transitionSource;
		private CanvasGroup windowGroup;

		protected override void OnInit()
		{
			if(!TryGetComponent(out windowGroup))
				windowGroup = gameObject.AddComponent<CanvasGroup>();
			
			base.OnInit();
		}

		protected override void OnDisposed()
		{
			transitionSource?.Cancel();
			transitionSource?.Dispose();
			transitionSource = null;
			base.OnDisposed();
		}

		public override void Show()
		{
			SetInteractableAsync(true).Forget();
			base.Show();
		}

		public override void Hide()
		{
			SetInteractableAsync(false).Forget();
			base.Hide();
		}

		protected async Task SetInteractableAsync(bool value)
		{
			try
			{
				transitionSource?.Cancel();
				transitionSource?.Dispose();
				transitionSource = null;

				if (value)
				{
					transitionSource = new CancellationTokenSource();
					await Task.Delay(InteractableDelayMs, cancellationToken: transitionSource.Token);
				}

				if (windowGroup)
					windowGroup.interactable = value;
			}
			catch
			{
				// Nothing to do
			}
			finally
			{
				transitionSource?.Dispose();
				transitionSource = null;
			}
		}
	}
}