namespace Potman.Common.LifecycleService.Abstractions
{
    public interface IInitable : ILifecycleObject
    {
        void Initialize();
    }
}