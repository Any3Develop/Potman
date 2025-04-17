namespace Potman.Common.LifecycleService.Abstractions
{
    public interface ILateUpdatable : ILifecycleObject
    {
        void LateUpdate();
    }
}