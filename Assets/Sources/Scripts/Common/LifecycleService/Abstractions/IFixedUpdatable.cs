namespace Potman.Common.LifecycleService.Abstractions
{
    public interface IFixedUpdatable : ILifecycleObject
    {
        void FixedUpdate();
    }
}