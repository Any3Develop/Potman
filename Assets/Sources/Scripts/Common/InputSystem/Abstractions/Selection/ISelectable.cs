using UnityEngine;

namespace Potman.Common.InputSystem.Abstractions.Selection
{
	public interface ISelectable
	{
		/// <summary>
		/// Target of some reference view
		/// </summary>
		GameObject TargetView { get; }

		bool CanSelect();
	}
}