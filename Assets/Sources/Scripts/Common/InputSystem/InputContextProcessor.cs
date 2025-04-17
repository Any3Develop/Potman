using Potman.Common.InputSystem.Abstractions;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Potman.Common.InputSystem
{
	public class InputContextProcessor : IInputContextProcessor
	{
		public IInputContext Process(IInputContext context)
		{
			if (context.TryReadValue(out TouchState touchState))
				return ProcessTouchPhase(context, touchState.phase);

			return context.TryReadValue(out TouchPhase touchPhase) 
				? ProcessTouchPhase(context, touchPhase) 
				: context;
		}

		private IInputContext ProcessTouchPhase(IInputContext context, TouchPhase value)
		{
			return new InputSystemContextWrapper()
				.SetContext(context)
				.SetStarted(value is TouchPhase.Began)
				.SetPerformed(value is TouchPhase.Moved or TouchPhase.Began)
				.SetCanceled(value is TouchPhase.Canceled or TouchPhase.Ended or TouchPhase.None);
		}
	}
}