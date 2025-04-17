using System;
using System.Collections.Generic;

namespace Potman.Common.Collections
{
    public class RuntimeCollection<T> : List<T>, IRuntimeCollection<T>
    {
        public bool TryGet(Predicate<T> predicate, out T result) => (result = Find(predicate)) != null;
        public int IndexOf(Predicate<T> predicate) => FindIndex(predicate);

        public new virtual void RemoveAt(int index) => base.RemoveAt(index);
        public new virtual bool Remove(T value) => base.Remove(value);
        public new virtual int RemoveAll(Predicate<T> predicate) => base.RemoveAll(predicate);
        public new virtual void Clear() => base.Clear();
    }
}