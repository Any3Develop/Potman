using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Potman.Common.ResourceManagament.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ResourceAssetIdAttribute : PropertyAttribute
    {
        public Type FilterType { get; }

        public ResourceAssetIdAttribute(Type filterType = null)
        {
            if (filterType != null && !typeof(Object).IsAssignableFrom(filterType))
                throw new InvalidOperationException("Filter type must inherit from UnityEngine.Object");

            FilterType = filterType;
        }
    }
}