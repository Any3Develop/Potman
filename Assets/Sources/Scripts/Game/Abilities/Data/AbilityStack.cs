using System;

namespace Potman.Game.Abilities.Data
{
    /// <summary>
    /// Controls the possibility of stack abilities.
    /// </summary>
    [Flags]
    public enum AbilityStack
    {
        /// <summary>
        /// Won't stack.
        /// </summary>
        Ignore = 0,
        
        /// <summary>
        /// 
        /// </summary>
        Integrated = 1,
        // TODO добавить больше вариантов
    }
}