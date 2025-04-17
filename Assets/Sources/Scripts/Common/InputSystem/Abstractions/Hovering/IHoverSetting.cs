using UnityEngine;

namespace Potman.Common.InputSystem.Abstractions.Hovering
{
	public interface IHoverSetting
	{
		Vector3 DefaultSize { get; set; }
		float MaxSize { get; set; }
		float Duration { get; set; }
		int MaxSublingIndex { get; set; }
		bool ChangeSublingIndex { get; set; }
		bool UseScaleUpAnimation { get; set; }
		bool Enabled { get; set; }
	}
}