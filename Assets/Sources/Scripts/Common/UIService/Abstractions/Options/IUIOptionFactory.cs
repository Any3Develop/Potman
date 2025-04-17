namespace Potman.Common.UIService.Abstractions.Options
{
    public interface IUIOptionFactory
    {
	    void Init(IUIService service);
	    IUIOptions<T> Create<T>() where T : IUIWindow;
    }
}