using System;
using System.Collections.Concurrent;
using R3;

namespace Potman.Common.Events
{
    public static class MessageBroker
    {
        private static readonly ConcurrentDictionary<Type, object> Streams = new();

        private class DispoasbleWrapper : IDisposable
        {
            public Action OnRemoveCallBack;

            public void Dispose()
            {
                OnRemoveCallBack?.Invoke();
                OnRemoveCallBack = null;
            }
        }
        
        public static IDisposable Register<T>(Observable<T> observable)
        {
            var typeKey = typeof(T);
            if (Streams.ContainsKey(typeKey))
                throw new Exception($"You are trying to register a {nameof(Observable<T>)} : {observable}, someone has already occupied : {typeKey}");
            
            Streams[typeKey] = observable;
            return new DispoasbleWrapper
            {
                OnRemoveCallBack = () => Streams.TryRemove(typeKey, out _)
            };
        }

        public static Observable<T> Receive<T>()
        {
            if (!Streams.TryGetValue(typeof(T), out var stream))
            {
                stream = new Subject<T>();
                Streams[typeof(T)] = stream;
            }

            if (stream != null) 
                return (Observable<T>) stream;
            
            Streams.TryRemove(typeof(T), out _);
            return Observable.Empty<T>();
        }

        public static void Publish<T>(T message)
        {
            if (!Streams.TryGetValue(typeof(T), out var stream)) 
                return;
            
            if (stream == null)
            {
                Streams.TryRemove(typeof(T), out _);
                return;
            }

            switch (stream)
            {
                case ISubject<T> subject : subject.OnNext(message); break;
                case Observer<T> observer : observer.OnNext(message); break;
            }
        }

        public static void Clear()
        {
            foreach (var stream in Streams.Values)
            {
                (stream as IDisposable)?.Dispose();
            }

            Streams.Clear();
        }
    }
}