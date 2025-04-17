namespace Potman.Game.Abilities.Data
{
    /// <summary>
    /// Determines the final number of targets.
    /// </summary>
    public enum TargetAoeFilter
    {
        /// <summary>
        /// The selection will be one of the best.
        /// </summary>
        Single = 0,
        
        /// <summary>
        /// Selects all and clamps them to the min-max values from the ability configuration.
        /// </summary>
        Multiple = 1,
    }
}