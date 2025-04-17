using System;

namespace Potman.Game.Abilities.Data
{
    /// <summary>
    /// How the ability will affect the chosen targets.
    /// </summary>
    [Flags]
    public enum AbilityAffect
    {
        /// <summary>
        /// The ability is not positive or negative, for example immortality has both parameters or states (appearance and others).
        /// </summary>
        Neutral = 0,
        
        /// <summary>
        /// The ability shows aggression such as damage, poison, and more.
        /// </summary>
        Negative = 2,
        
        /// <summary>
        /// The ability shows positive properties, such as healing, removing de buffs, and more.
        /// </summary>
        Positive = 4,
    }
}