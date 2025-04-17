using System;

namespace Potman.Common.InputSystem.Abstractions.Hovering
{
	public interface IHoverSystem
	{
		event Action<IHoverable> OnHoverEnter;

		event Action<IHoverable> OnHoverExit;
		
		bool Enabled { get; }
		
		void Registration(IHoverable hoverable);
		
		void UnRegistration(IHoverable hoverable);

		void Enable(bool value);

		void Start(IHoverable hoverable);
		
		void Cancel(IHoverable hovered = null);

		void Clear();
	}
}