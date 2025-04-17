using System;
using System.Collections.Generic;

namespace Potman.Common.Utilities
{
    public static class EnumerableExtensons
    {
        public static void Each<T>(this IEnumerable<T> source, Action<T> result)
        {
            foreach (var obj in source)
                result?.Invoke(obj);
        }
        
        public static void Each<T>(this IEnumerable<T> source, Action<T, int> result)
        {
            var index = 0;
            foreach (var obj in source)
                result?.Invoke(obj, index++);
        }
    }
}