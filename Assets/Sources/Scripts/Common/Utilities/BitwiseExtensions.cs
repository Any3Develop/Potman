using System;
using System.Runtime.CompilerServices;

namespace Potman.Common.Utilities
{
    public static class BitwiseExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T AddFlag<T>(this T mask, T flag) where T : Enum
        {
            var maskValue = Convert.ToInt64(mask);
            var flagValue = Convert.ToInt64(flag);
        
            return (T)Enum.ToObject(typeof(T), maskValue | flagValue);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T RemoveFlag<T>(this T mask, T flag) where T : Enum
        {
            var maskValue = Convert.ToInt64(mask);
            var flagValue = Convert.ToInt64(flag);
            
            return (T)Enum.ToObject(typeof(T), maskValue & ~flagValue);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyFlags<T>(this T mask, T flags) where T : Enum
        {
            var maskValue = Convert.ToInt64(mask);
            var flagsValue = Convert.ToInt64(flags);
        
            return maskValue == flagsValue || (maskValue & flagsValue) != 0;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllFlags<T>(this T mask, T flags) where T : Enum
        {
            var maskValue = Convert.ToInt64(mask);
            var flagsValue = Convert.ToInt64(flags);
        
            return (maskValue & flagsValue) == flagsValue;
        }
    }
}