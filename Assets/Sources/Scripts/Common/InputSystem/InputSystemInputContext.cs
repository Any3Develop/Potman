using System;
using Potman.Common.InputSystem.Abstractions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Potman.Common.InputSystem
{
	public struct InputSystemInputContext : IInputContext
	{
		private object inputObj;
		public string Id { get; }
		public bool Started { get; }
		public bool Performed { get; }
		public bool Canceled { get; }
		public Type ControlValueType { get; }

		private InputAction.CallbackContext context;

		public InputSystemInputContext(InputAction.CallbackContext context, string id)
		{
			this.context = context;
			Id = id;
			ControlValueType = context.control.valueType;
			Started = context.started;
			Performed = context.performed;
			Canceled = context.canceled;
			inputObj = default;
		}

		public InputSystemInputContext(Type valueType, object obj, bool started, bool performed, bool cancelled, string id)
		{
			context = default;
			ControlValueType = valueType;
			inputObj = obj;
			Started = started;
			Performed = performed;
			Canceled = cancelled;
			Id = id;
		}
		
		public TValue ReadValue<TValue>() where TValue : struct
		{
			inputObj ??= context.ReadValue<TValue>();
			return (TValue)inputObj;
		}

		public bool TryReadValue<TValue>(out TValue result) where TValue : struct
		{
			result = default;
			try
			{
				if (ControlValueType != typeof(TValue))
					return false;

				result = ReadValue<TValue>();
				return true;
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				return false;
			}
		}
	}
}