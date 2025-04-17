using System;

namespace Potman.Common.ResourceManagament.Exceptions
{
	public class NotInitializedException : Exception
	{
		public NotInitializedException() : base("Before using the service, it must be initialized.") {}
	}
}