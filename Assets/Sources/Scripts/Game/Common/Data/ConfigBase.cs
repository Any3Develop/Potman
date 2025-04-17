using UnityEngine;

namespace Potman.Game.Common.Data
{
    public abstract class ConfigBase : ScriptableObject
    {
        public string Id => name;
    }
}