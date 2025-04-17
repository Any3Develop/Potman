using Potman.Common.SerializeService.Abstractions;

namespace Potman.Common.SerializeService
{
	public class SerializeHelperAdapter
	{
		public static ISerializeService Service { get; private set; }

		public SerializeHelperAdapter(ISerializeService serializeService)
		{
			Service = serializeService;
		}
	}
}