using System;

namespace Potman.Common.InputSystem.Abstractions
{
	public interface IInputAction
	{
		bool Enabled { get; }
		string Id { get; }
		event Action<IInputContext> OnStarted;
		event Action<IInputContext> OnPerformed;
		event Action<IInputContext> OnCanceled;
		event Action OnEnableChanged;
		TValue ReadValue<TValue>() where TValue : struct;
		float GetControlMagnitude();
		void Enable(bool value);
		void Enable();
		void Disable();
		void Perform<TValue>(TValue value) where TValue : struct;
	}
}