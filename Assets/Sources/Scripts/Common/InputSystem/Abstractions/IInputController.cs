using System.Collections.Generic;

namespace Potman.Common.InputSystem.Abstractions
{
	public interface IInputController
	{
		IInputAction Get(string id);
		IEnumerable<IInputAction> GetAll();
		void Enable(bool value);
		void Enable();
		void Disable();
	}
	
	public interface IInputController<in TActionId> : IInputController
	{
		IInputAction Get(TActionId id);
	}
}