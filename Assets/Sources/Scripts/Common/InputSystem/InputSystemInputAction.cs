using System;
using Potman.Common.InputSystem.Abstractions;
using UnityEngine.InputSystem;

namespace Potman.Common.InputSystem
{
	public class InputSystemInputAction : IInputAction, IDisposable
	{
		private readonly InputAction bindedAction;
		private readonly IInputContextProcessor contextProcessor;

		public bool Enabled { get; private set; }
		public string Id => bindedAction.name;
		public event Action<IInputContext> OnStarted;
		public event Action<IInputContext> OnPerformed;
		public event Action<IInputContext> OnCanceled;
		public event Action OnEnableChanged;

		public InputSystemInputAction(InputAction bindAction, IInputContextProcessor contextProcessor)
		{
			bindedAction = bindAction;
			this.contextProcessor = contextProcessor;
			if (bindedAction.enabled) // initialize if enabled
				Enable();
		}

		public TValue ReadValue<TValue>() where TValue : struct
		{
			return bindedAction.ReadValue<TValue>();
		}

		public float GetControlMagnitude()
		{
			return bindedAction.GetControlMagnitude();
		}

		public void Enable(bool value)
		{
			if (Enabled == value)
				return;
			
			if (value)
			{
				Enable();
				return;
			}
			
			Disable();
		}

		public void Enable()
		{
			if (Enabled)
				return;

			Enabled = true;
			bindedAction.Enable();
			bindedAction.started += OnBindedActionStarted;
			bindedAction.performed += OnBindedActionPerformed;
			bindedAction.canceled += OnBindedActionCanceled;
			OnEnableChanged?.Invoke();
		}
		
		public void Disable()
		{
			if (!Enabled)
				return;
			
			Enabled = false;
			bindedAction.Disable();
			bindedAction.started -= OnBindedActionStarted;
			bindedAction.performed -= OnBindedActionPerformed;
			bindedAction.canceled -= OnBindedActionCanceled;
			OnEnableChanged?.Invoke();
		}

		public void Perform<TValue>(TValue value) where TValue : struct
		{
			OnPerformed?.Invoke(contextProcessor.Process(new InputSystemInputContext(typeof(TValue), value, false, true, false, Id)));
		}

		private void OnBindedActionCanceled(InputAction.CallbackContext context)
		{
			OnCanceled?.Invoke(contextProcessor.Process(new InputSystemInputContext(context, Id)));
		}

		private void OnBindedActionPerformed(InputAction.CallbackContext context)
		{
			OnPerformed?.Invoke(contextProcessor.Process(new InputSystemInputContext(context, Id)));
		}

		private void OnBindedActionStarted(InputAction.CallbackContext context)
		{
			OnStarted?.Invoke(contextProcessor.Process(new InputSystemInputContext(context, Id)));
		}

		public void Dispose()
		{
			Disable();
			OnEnableChanged = null;
			OnStarted = null;
			OnPerformed = null;
			OnCanceled = null;
		}
	}
}