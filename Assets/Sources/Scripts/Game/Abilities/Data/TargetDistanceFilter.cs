using System;

namespace Potman.Game.Abilities.Data
{
    /// <summary>
    /// Filtering based on distance from the ability owner.
    /// </summary>
    [Flags]
    public enum TargetDistanceFilter
    {
        /// <summary>
        /// Ignore the filter.
        /// </summary>
        Ignore = 0,
        
        /// <summary>
        /// All the closest ones will be selected.
        /// </summary>
        Closest = 2,
        
        /// <summary>
        /// Everyone in the middle will be chosen.
        /// </summary>
        Middle = 4,
        
        /// <summary>
        /// All the far ones will be selected.
        /// </summary>
        Farthest = 8,
    }
}