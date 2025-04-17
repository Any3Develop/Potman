namespace Potman.Game.Abilities.Data
{
    /// <summary>
    /// How the ability will be manipulated by the player and the game's environment.
    /// </summary>
    public enum AbilityType
    {
        /// <summary>
        /// Defines the interactivity of the ability as Active use.
        /// </summary>
        Active = 0,
        
        /// <summary>
        /// Defines the interactivity of the ability as Passive use.
        /// </summary>
        Passive = 1,
        
        /// <summary>
        /// The ability is integrated into the owner..
        /// </summary>
        Integrated = 2
    }
}