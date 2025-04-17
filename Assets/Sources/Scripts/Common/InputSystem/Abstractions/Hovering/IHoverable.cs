using UnityEngine;

namespace Potman.Common.InputSystem.Abstractions.Hovering
{
	public interface IHoverable
	{
		/// <summary>
		/// Target of some reference view
		/// </summary>
		GameObject TargetView { get; }
		
		IHoverSetting HoverSetting { get; }
		
		int SublingIndex { get; set; }
		
		Vector3 Size { get; set; }

		bool CanHover();
	}
}