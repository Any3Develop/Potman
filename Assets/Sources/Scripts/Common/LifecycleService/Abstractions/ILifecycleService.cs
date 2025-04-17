namespace Potman.Common.LifecycleService.Abstractions
{
    public interface ILifecycleService
    {
        void AddInit(IInitable obj);
        void AddUpdate(IUpdatable obj);
        void AddLateUpdate(ILateUpdatable obj);
        void AddFixedUpdate(IFixedUpdatable obj);
        void AddAll(ILifecycleObject obj);
        
        void RemoveInit(IInitable obj);
        void RemoveUpdate(IUpdatable obj);
        void RemoveLateUpdate(ILateUpdatable obj);
        void RemoveFixedUpdate(IFixedUpdatable obj);
        void RemoveAll(ILifecycleObject obj);
        
        bool ExistInit(IInitable obj);
        bool ExistUpdate(IUpdatable obj);
        bool ExistLateUpdate(ILateUpdatable obj);
        bool ExistFixedUpdate(IFixedUpdatable obj);
        bool ExistAny(ILifecycleObject obj);
    }
}