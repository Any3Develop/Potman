using Potman.Common.InputSystem.Abstractions.Hovering;
using UnityEngine;

namespace Potman.Common.InputSystem.Hovering
{
	public class HoverSettings : IHoverSetting
	{
		public Vector3 DefaultSize { get; set; }
		public float MaxSize { get; set; }
		public float Duration { get; set; }
		public int MaxSublingIndex { get; set; }
		public bool ChangeSublingIndex { get; set; }
		public bool UseScaleUpAnimation { get; set; }
		public bool Enabled { get; set; }

		public HoverSettings()
		{
			ApplyDefaultValues();
		}

		public static HoverSettings Default()
		{
			return new HoverSettings().ApplyDefaultValues();
		}

		private HoverSettings ApplyDefaultValues()
		{

			DefaultSize = Vector3.one;
			MaxSize = 1.2f;
			Duration = 0.25f;
			MaxSublingIndex = 1000;
			ChangeSublingIndex = true;
			UseScaleUpAnimation = true;
			Enabled = true;
			return this;
		}
	}
}