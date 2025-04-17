using System;
using System.Collections.Generic;
using System.Linq;
using Potman.Common.UIService.Abstractions;
using Potman.Common.UIService.Abstractions.FullFade;

namespace Potman.Common.UIService.FullFade
{
	public class UIFullFadePresenter : IUIFullFadePresenter, IDisposable
	{
		private readonly List<IFullFadeTarget> showed = new();
		private IUIService uiService;
		
		public void Init(IUIService service)
		{
			uiService = service;
		}
		
		public void Dispose()
		{
			uiService = null;
			showed.Clear();
		}

		public void OnDeleted(IUIWindow window)
		{
			if (!TryGetTarget(window, out var fadeTarget))
				return;

			showed.Remove(fadeTarget);
			TryShowLast();
		}

		public void OnShow(IUIWindow window)
		{
			if (!TryGetTarget(window, out var fadeTarget))
				return;

			if (!showed.Contains(fadeTarget))
				showed.Add(fadeTarget);
			
			Show(fadeTarget);
		}

		public void OnHidden(IUIWindow window)
		{
			if (!TryGetTarget(window, out var fadeTarget))
				return;

			showed.Remove(fadeTarget);
			TryShowLast();
		}

		private static bool TryGetTarget(IUIWindow window, out IFullFadeTarget result)
			=> (result = window as IFullFadeTarget) != null;

		private static bool TryGetColor(IFullFadeTarget target, out IFullFadeColor result) 
			=> (result = target as IFullFadeColor) != null;

		private static bool TryGetClickHandler(IFullFadeTarget target, out IFullFadeClickHandler result)
			=> (result = target as IFullFadeClickHandler) != null;
		
		private bool TryHide()
		{
			showed.RemoveAll(x => !x?.Root);
			if (showed.Count != 0)
				return false;

			uiService.Begin<UIFullFadeWindow>().Hide();
			return true;
		}

		private void TryShowLast()
		{
			if (TryHide())
				return;

			Show(showed.Last());
		}

		private void Show(IFullFadeTarget target)
		{
			uiService.Begin<UIFullFadeWindow>()
				.WithMove(target.Root, 0)
				.WithInit(InitWindow)
				.Show();
			
			return;
			void InitWindow(UIFullFadeWindow window)
			{
				window.Clear();
				
				if (TryGetColor(target, out var colorTarget))
					window.SetColor(colorTarget.FadeColor);
				
				if (TryGetClickHandler(target, out var clickHandler))
					window.SetClickAction(clickHandler.OnFadeClicked);
			}
		}
	}
}