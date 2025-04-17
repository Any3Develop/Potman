using System;

namespace Potman.Game.Abilities.Data
{
    /// <summary>
    /// A way of selecting targets.
    /// </summary>
    [Flags]
    public enum AbilityTargeting
    {
        /// <summary>
        /// Randomly determines targets from all existing targets.
        /// </summary>
        Random = 0,
        
        /// <summary>
        /// The targets are selected by the user.
        /// </summary>
        Manual = 2,
        
        /// <summary>
        /// Targets are selected based on the direction of the player's gaze or cursor.
        /// </summary>
        Directional = 4,
    }
}