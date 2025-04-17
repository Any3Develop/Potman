using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Potman.Common.LifecycleService.Abstractions;
using R3;
using UnityEngine;

namespace Potman.Common.LifecycleService
{
    public class LifecycleSystem : ILifecycleService, IDisposable
    {
        private readonly List<IInitable> initables;
        private readonly List<IUpdatable> updatables;
        private readonly List<ILateUpdatable> lateUpdatables;
        private readonly List<IFixedUpdatable> fixedUpdatables;
        private readonly object execution = new();

        private bool shouldCleanup;
        private event Action CleanupEvent;
        private IDisposable subscribes;

        public LifecycleSystem(
            List<IInitable> init,
            List<IUpdatable> update,
            List<ILateUpdatable> late,
            List<IFixedUpdatable> fix)
        {
            initables = init;
            updatables = update;
            lateUpdatables = late;
            fixedUpdatables = fix;
            Subscribe();
        }

        public void Dispose()
        {
            shouldCleanup = false;
            CleanupEvent = null;
            subscribes?.Dispose();
            subscribes = null;
            initables.Clear();
            updatables.Clear();
            lateUpdatables.Clear();
            fixedUpdatables.Clear();
        }

        private void Subscribe()
        {
            using var builder = new DisposableBuilder();
            builder.Add(Observable.EveryUpdate(UnityFrameProvider.Initialization)
                .Subscribe(_ => Execute(initables, obj => obj.Initialize(), true)));
            
            builder.Add(Observable.EveryUpdate(UnityFrameProvider.Update)
                .Subscribe(_ => Execute(updatables, obj => obj.Update())));
            
            builder.Add(Observable.EveryUpdate(UnityFrameProvider.PostLateUpdate)
                .Subscribe(_ => Execute(lateUpdatables, obj => obj.LateUpdate())));
            
            builder.Add(Observable.EveryUpdate(UnityFrameProvider.FixedUpdate)
                .Subscribe(_ => Execute(fixedUpdatables, obj => obj.FixedUpdate())));
            
            subscribes = builder.Build();
        }
        
        public void AddInit(IInitable obj) => Add(initables, obj);
        public void AddUpdate(IUpdatable obj) => Add(updatables, obj);
        public void AddLateUpdate(ILateUpdatable obj) => Add(lateUpdatables, obj);
        public void AddFixedUpdate(IFixedUpdatable obj) => Add(fixedUpdatables, obj);

        public void AddAll(ILifecycleObject obj)
        {
            if (obj is IInitable initable)
                AddInit(initable);

            if (obj is IUpdatable updatable)
                AddUpdate(updatable);

            if (obj is ILateUpdatable lateUpdatable)
                AddLateUpdate(lateUpdatable);

            if (obj is IFixedUpdatable fixedUpdatable)
                AddFixedUpdate(fixedUpdatable);
        }

        public void RemoveInit(IInitable obj) =>  Remove(initables, obj);
        public void RemoveUpdate(IUpdatable obj) =>  Remove(updatables, obj);
        public void RemoveLateUpdate(ILateUpdatable obj) =>  Remove(lateUpdatables, obj);
        public void RemoveFixedUpdate(IFixedUpdatable obj) => Remove(fixedUpdatables, obj);

        public void RemoveAll(ILifecycleObject obj)
        {
            if (obj is IInitable initable)
                RemoveInit(initable);

            if (obj is IUpdatable updatable)
                RemoveUpdate(updatable);

            if (obj is ILateUpdatable lateUpdatable)
                RemoveLateUpdate(lateUpdatable);

            if (obj is IFixedUpdatable fixedUpdatable)
                RemoveFixedUpdate(fixedUpdatable);
        }

        public bool ExistInit(IInitable obj) => initables.Contains(obj);
        public bool ExistUpdate(IUpdatable obj) => updatables.Contains(obj);
        public bool ExistLateUpdate(ILateUpdatable obj) => lateUpdatables.Contains(obj);
        public bool ExistFixedUpdate(IFixedUpdatable obj) => fixedUpdatables.Contains(obj);

        public bool ExistAny(ILifecycleObject obj) => obj switch
        {
            IInitable initable => ExistInit(initable),
            IUpdatable updatable => ExistUpdate(updatable),
            ILateUpdatable lateUpdatable => ExistLateUpdate(lateUpdatable),
            IFixedUpdatable fixedUpdatable => ExistFixedUpdate(fixedUpdatable),
            _ => false
        };

        private void Execute<T>(IList<T> collection, Action<T> execute, bool removeAfterExecute = false)
        {
            if (shouldCleanup)
            {
                CleanupEvent?.Invoke();
                CleanupEvent = null;
                shouldCleanup = false;
            }

            if (collection.Count == 0)
                return;
            
            lock (execution)
            {
                for (var index = 0; index < collection.Count ; index++)
                {
                    try
                    {
                        execute(collection[index]);
                        if (removeAfterExecute)
                            collection.RemoveAt(index--);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }

        private void Add<T>(ICollection<T> collection, T obj, [CallerMemberName] string callerName = "")
        {
            if (obj == null)
                throw new NullReferenceException($"[{nameof(ILifecycleService)}] you are trying to add [{typeof(T).Name} : null] to {callerName}.");

            if (collection.Count > 0 && collection.Contains(obj))
                throw new Exception($"[{nameof(ILifecycleService)}] you are trying to add [{obj.GetType().FullName}] twice to {callerName}.");

            lock (execution)
                collection.Add(obj);
        }

        private void Remove<T>(ICollection<T> collection, T obj)
        {
            shouldCleanup = true;
            CleanupEvent += () => collection.Remove(obj);
        }
    }
}