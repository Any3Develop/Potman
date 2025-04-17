namespace Potman.Game.Entities.Units.Data
{
    public enum UnitMovementType
    {
        /// <summary>
        /// Ground / Flying static without any movement
        /// </summary>
        Static = 0,
        /// <summary>
        /// Ground movement
        /// </summary>
        Infantry = 1,
        /// <summary>
        /// Flying movements
        /// </summary>
        Aviation = 2,
    }
}