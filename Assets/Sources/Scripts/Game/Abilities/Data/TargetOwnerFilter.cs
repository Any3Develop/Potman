using System;

namespace Potman.Game.Abilities.Data
{
    /// <summary>
    /// Filtering depending on the owner of the ability.
    /// </summary>
    [Flags]
    public enum TargetOwnerFilter
    {
        /// <summary>
        /// Ignore the filter.
        /// </summary>
        Ignore = 0,
        
        /// <summary>
        /// Select the owner of the ability if it meets the conditions.
        /// </summary>
        Owner = 2,
        
        /// <summary>
        /// Select targets allied to the ability if they meet the conditions.
        /// </summary>
        Allies = 4,
        
        /// <summary>
        /// Select targets enemies to the ability if they meet the conditions.
        /// </summary>
        Enemies = 8,
    }
}