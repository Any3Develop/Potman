using System.Collections.Generic;
using Potman.Common.LifecycleService;
using Potman.Common.LifecycleService.Abstractions;
using Zenject;

namespace Potman.Infrastructure.Common.LifecycleService
{
    public class ZenjectLifecycleSystem : LifecycleSystem
    {
        [Inject(Optional = true, Source = InjectSources.Local)]
        public ZenjectLifecycleSystem(
            List<IInitable> init,
            List<IUpdatable> update,
            List<ILateUpdatable> late,
            List<IFixedUpdatable> fix) 
            : base(init, update, late, fix) {}
    }
}