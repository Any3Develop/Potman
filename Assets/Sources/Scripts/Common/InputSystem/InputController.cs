using System;
using System.Collections.Generic;
using System.Linq;
using Potman.Common.InputSystem.Abstractions;

namespace Potman.Common.InputSystem
{
	public class InputController<T> : IInputController<T>
	{
		private readonly Dictionary<string, IInputAction> inputActions;
		
		public InputController(IEnumerable<IInputAction> inputActions)
		{
			if (inputActions == null)
				throw new ArgumentException($"Cannot be null argument = {nameof(inputActions)}");
			
			this.inputActions = inputActions.ToDictionary(x => x.Id);
		}
		
		public IInputAction Get(T id) => Get(id.ToString() ?? string.Empty);

		public IInputAction Get(string id)
		{
			if (string.IsNullOrEmpty(id) || !inputActions.ContainsKey(id))
				throw new ArgumentException($"Action with ID : {id} does not represent in collection");

			return inputActions[id];
		}

		public IEnumerable<IInputAction> GetAll()
		{
			return inputActions.Values;
		}

		public void Enable(bool value)
		{
			if (value)
			{
				Enable();
				return;
			}
			
			Disable();
		}

		public void Enable()
		{
			foreach (var value in inputActions.Values.Where(value => !value.Enabled))
			{
				value.Enable();
			}
		}

		public void Disable()
		{
			foreach (var value in inputActions.Values.Where(value => value.Enabled))
			{
				value.Disable();
			}
		}
	}
}