using System;

namespace Potman.Common.ResourceManagament
{
	public class ProgressMock : IProgress<float>
	{
		public void Report(float value) {}
	}
}