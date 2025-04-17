using System;
using Potman.Common.UIService.Abstractions;

namespace Potman.Common.UIService.Data
{
	public class WindowItem
	{
		public IUIWindow Window { get; }
		public IDisposable ActiveTask { get; set; }
		public Type CachedType { get; }
		public string GroupId { get;}
		public string Id { get; }

		public WindowItem(IUIWindow window, string groupId)
		{
			Window = window;
			Id = window.Id;
			GroupId = groupId;
			CachedType = Window?.GetType();
		}
	}
}