using System;
using Potman.Game.Abilities.Data;

namespace Potman.Game.Abilities
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AbilityAttribute : Attribute
    {
        public AbilityId Id { get; }
        public AbilityAttribute(AbilityId id) => Id = id;
    }
}