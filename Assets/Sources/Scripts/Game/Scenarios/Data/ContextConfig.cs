using System;
using UnityEngine;

namespace Potman.Game.Scenarios.Data
{
    [Serializable]
    public class ContextConfig
    {
        [Tooltip("How many time in seconds the game will playing. -1 is infinity."), Min(-1)]
        public int gameTime = -1;
        
        [Tooltip("The maximum limit of unit spawns in a scenario at all times. 0 is Infinity."), Min(0)]
        public int maxUnitsTotal;
        
        [Tooltip("The maximum limit of unit spawns in a scenario at the moment. 0 is Infinity."), Min(0)]
        public int maxUnitsScene;
        
        [Tooltip("A delay in Milliseconds before the game starts, when everything has loaded and the player has gained control over the character."), Min(0)]
        public int delayedStartMs = 1_000;
    }
}