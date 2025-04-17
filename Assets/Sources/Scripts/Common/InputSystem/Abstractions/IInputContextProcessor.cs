namespace Potman.Common.InputSystem.Abstractions
{
	public interface IInputContextProcessor
	{
		IInputContext Process(IInputContext context);
	}
}