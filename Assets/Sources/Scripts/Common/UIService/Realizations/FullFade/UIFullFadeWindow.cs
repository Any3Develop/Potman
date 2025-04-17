using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Potman.Common.UIService.FullFade
{
	public class UIFullFadeWindow : UISafeWindowBase
	{
		[SerializeField] protected Button closeButton;
		[SerializeField] protected Image fadeImage;
		private Color defaultColor;
		private UnityAction clickCallBack;

		protected override void OnInit()
		{
			if (fadeImage)
				defaultColor = fadeImage.color;
		}

		public void SetClickAction(Action value)
		{
			if (!closeButton)
				return;
			
			if (clickCallBack != null)
				closeButton.onClick.RemoveListener(clickCallBack);

			clickCallBack = () => value?.Invoke();
			closeButton.onClick.AddListener(clickCallBack);
		}
		
		public void SetColor(Color value)
		{
			if (!fadeImage)
				return;

			fadeImage.color = value;
		}

		public void Clear()
		{
			if (clickCallBack != null)
				closeButton.onClick.RemoveListener(clickCallBack);

			clickCallBack = null;
			
			if (fadeImage)
				fadeImage.color = defaultColor;
		}

		public override void Hidden()
		{
			Clear();
			base.Hidden();
		}

		protected override void OnDisposed() => Clear();
	}
}