namespace Potman.Game.Abilities.Data
{
    /// <summary>
    /// Affects the type of ability life time reduction.
    /// </summary>
    public enum AbilityExpire
    {
        /// <summary>
        /// Always ignore the life time.
        /// </summary>
        Ignore = 0,
        
        /// <summary>
        /// The ability's life time counter is decremented before execution.
        /// </summary>
        BeforeExecute,
        
        /// <summary>
        /// After execution, the ability's lifetime counter is decremented.
        /// </summary>
        AfterExecute,
        
        /// <summary>
        /// The ability lifetime counter is decremented as a timer.
        /// </summary>
        Timer,
    }
}