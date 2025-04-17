using System.Collections.Generic;
using Potman.Common.UIService.Abstractions;

namespace Potman.Common.Audio
{
	public interface IUIAudioListener
	{
		void Subscribe(IEnumerable<IUIWindow> windows);
	}
}