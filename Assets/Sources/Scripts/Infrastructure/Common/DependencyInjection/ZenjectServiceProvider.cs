using Potman.Common.DependencyInjection;
using Zenject;

namespace Potman.Infrastructure.Common.DependencyInjection
{
    public class ZenjectServiceProvider : IServiceProvider
    {
        private readonly DiContainer diContainer;

        [Inject(Optional = true, Source = InjectSources.Local)]
        public ZenjectServiceProvider(DiContainer diContainer)
        {
            this.diContainer = diContainer;
        }

        public T GetRequiredService<T>() => diContainer.Resolve<T>();
    }
}