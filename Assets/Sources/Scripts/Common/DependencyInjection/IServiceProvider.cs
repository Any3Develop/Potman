namespace Potman.Common.DependencyInjection
{
    public interface IServiceProvider
    {
        T GetRequiredService<T>();
    }
}