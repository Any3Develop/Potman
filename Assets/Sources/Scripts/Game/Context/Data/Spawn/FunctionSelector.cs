namespace Potman.Game.Context.Data.Spawn
{
    /// <summary>
    /// Control how to select next object.
    /// </summary>
    public enum FunctionSelector
    {
        /// <summary>
        /// Random function.
        /// </summary>
        Random = 0,
        
        /// <summary>
        /// Default list order mapping.
        /// </summary>
        Index,

        /// <summary>
        /// Circular loop selection.
        /// </summary>
        RoundRobin,

        /// <summary>
        /// Reverse direction circular loop selection.
        /// </summary>
        ReverseRoundRobin,

        /// <summary>
        /// Linear ping - pong loop selection.
        /// </summary>
        PingPong,
    }
}