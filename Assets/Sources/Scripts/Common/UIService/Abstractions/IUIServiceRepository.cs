using System.Collections.Generic;
using Potman.Common.UIService.Data;

namespace Potman.Common.UIService.Abstractions
{
	public interface IUIServiceRepository
	{
		WindowItem Get<T>(string windowId, string groupId);
		
		IEnumerable<WindowItem> GetAll<T>(string groupId);
		
		void Add(WindowItem value);
		
		bool Remove(WindowItem value);
	}
}