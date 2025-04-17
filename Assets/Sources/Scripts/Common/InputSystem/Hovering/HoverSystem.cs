using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Potman.Common.InputSystem.Abstractions;
using Potman.Common.InputSystem.Abstractions.Hovering;
using Potman.Common.InputSystem.Utils;
using UnityEngine;

namespace Potman.Common.InputSystem.Hovering
{
	public class HoverSystem : IHoverSystem, IDisposable
	{
		private readonly IInputController<HoverActions> inputController;

		private class HoveredInfo
		{
			public int FromSublingIndex;
			public bool Hovered;
			public Tween Hovering;
		}
		private readonly Dictionary<IHoverable, HoveredInfo> hoverables;
		public event Action<IHoverable> OnHoverEnter;
		public event Action<IHoverable> OnHoverExit;
		public bool Enabled => disableQueue <= 0;
		private bool onceCancelled;
		private int disableQueue;
		
		public HoverSystem(IInputController<HoverActions> inputController)
		{
			this.inputController = inputController;
			hoverables = new Dictionary<IHoverable, HoveredInfo>();
			Initialize();
		}
		
		public void Initialize()
		{
			foreach (var input in inputController.GetAll())
			{
				input.OnStarted += OnHoveringInput;
				input.OnPerformed += OnHoveringInput;
				input.OnCanceled += OnHoveringInput;
			}
		}
		
		public void Dispose()
		{
			OnHoverEnter = null;
			OnHoverExit = null;
		}

		public void Registration(IHoverable hoverable)
		{
			try
			{
				if (hoverable == null)
					throw new NullReferenceException($"Target {nameof(IHoverable)} is missing");

				if (hoverables.ContainsKey(hoverable))
					throw new InvalidOperationException($"{hoverable} already registered");

				hoverables.Add(hoverable, new HoveredInfo {Hovered = false});
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}
		}

		public void UnRegistration(IHoverable hoverable)
		{
			try
			{
				if (hoverable == null)
					throw new NullReferenceException($"Target {nameof(IHoverable)} is missing");

				if (!hoverables.ContainsKey(hoverable))
					return;

				Cancel(hoverable);
				hoverables.Remove(hoverable);
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}
		}

		public void Enable(bool value)
		{
			disableQueue = Math.Max(0, disableQueue + (value ? -1 : 1));
		}

		public void Start(IHoverable hoverable)
		{
			try
			{
				if (hoverable == null || !Enabled)
					return;

				if (!hoverable.HoverSetting.Enabled)
					return;

				if (!hoverables.TryGetValue(hoverable, out var hoveredInfo) || hoveredInfo is not {Hovered: false})
					return;

				foreach (var (hovered, _) in hoverables.ToArray().Where(x => x.Value.Hovered))
					Cancel(hovered);
				
				if (!hoverable.HoverSetting.UseScaleUpAnimation)
				{
					hoveredInfo.Hovered = true;
					OnHoverEnter?.Invoke(hoverable);
					return;
				}

				hoveredInfo.FromSublingIndex = hoverable.SublingIndex;
				hoveredInfo.Hovering?.Kill();

				if (hoverable.HoverSetting.ChangeSublingIndex)
					hoverable.SublingIndex = hoverable.HoverSetting.MaxSublingIndex;

				hoveredInfo.Hovering = GetHoveringTween(hoverable);
				hoveredInfo.Hovered = true;
				OnHoverEnter?.Invoke(hoverable);
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}
		}

		public void Cancel(IHoverable hovered = null)
		{
			try
			{
				if (hovered == null)
				{
					foreach (var (other, _) in hoverables.ToArray().Where(x => x.Value.Hovered && x.Key != null))
						Cancel(other);
					
					return;
				}

				if (!hoverables.TryGetValue(hovered, out var hoveredInfo) || hoveredInfo is not {Hovered: true})
					return;

				if (!hovered.HoverSetting.UseScaleUpAnimation)
				{
					hoveredInfo.Hovered = false;
					OnHoverExit?.Invoke(hovered);
					return;
				}

				hoveredInfo.Hovering?.Kill();

				if (hovered.HoverSetting.ChangeSublingIndex)
					hovered.SublingIndex = hoveredInfo.FromSublingIndex;

				hoveredInfo.Hovering = GetHoveringTween(hovered, false);
				hoveredInfo.Hovered = false;
				OnHoverExit?.Invoke(hovered);
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}
		}

		public void Clear()
		{
			Cancel();
			hoverables.Clear();
			OnHoverEnter = null;
			OnHoverExit = null;
		}

		private Tween GetHoveringTween(IHoverable hoverable, bool start = true)
		{
			try
			{
				if (hoverable == null)
					return default;

				return DOTween.To(() => hoverable.Size
						, value => hoverable.Size = value
						, start
							? hoverable.HoverSetting.DefaultSize * hoverable.HoverSetting.MaxSize
							: hoverable.HoverSetting.DefaultSize
						, hoverable.HoverSetting.Duration)
					.SetAutoKill(true);
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				return null;
			}
		}
		
		private bool GetHoverable(out IHoverable result)
		{
			try
			{
				result = hoverables.Keys.FirstOrDefault(x => x.HoverSetting.Enabled && x.CanHover() && InputHelper.IsPointerOver(x.TargetView));
				return result != null;
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				result = null;
				return false;
			}
		}
		
		private void OnHoveringInput(IInputContext context)
		{
			try
			{
				if (!Enabled)
				{
					if (onceCancelled)
						return;

					onceCancelled = true;
					Cancel();
					return;
				}

				var hasHoverable = GetHoverable(out var hoverable);
				if ((context.Started || context.Performed) && hasHoverable)
				{
					Start(hoverable);
					return;
				}

				if (context.Canceled || !hasHoverable)
					Cancel();
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}
		}
	}
}