using System;

namespace Potman.Common.ResourceManagament.Exceptions
{
	public class InternetConnectionException : Exception
	{
		public InternetConnectionException(string msg = "")
			: base("The connection failed during the update, please check your internet connection or try again later."
			       + (string.IsNullOrEmpty(msg) ? "" : $" Error : {msg}")) {}
	}
}