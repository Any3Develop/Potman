using System;
using System.Collections.Generic;

namespace Potman.Common.Collections
{
    public interface IRuntimeCollection<T> : IList<T>
    {        
        void AddRange(IEnumerable<T> values);
        int RemoveAll(Predicate<T> predicate);
        
        bool TryGet(Predicate<T> predicate, out T result);
        int IndexOf(Predicate<T> predicate);
        void Sort(Comparison<T> comparison);
        void ForEach(Action<T> predicate);
    }
}