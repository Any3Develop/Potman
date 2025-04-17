using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Potman.Game.Stats.Data
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StatType
    {
        None = 0,
        
        // Base
        Heath = 20,
        Armor = 22,
        Level = 26,
        
        // Ability
        Countdown = 43,
        Damage = 23,
        Range = 27,
        LifeTime = 48,
        
        /// Utility
        Priority = 42,
        [Obsolete("Временный стат до момента реализация поведений монстров.")]
        StartDelay = 44, // TODO TEMP
        IgnoreLayers = 47,
        
        // Move
        MoveSpeed = 60,
        MoveAcceleration = 61,
        MoveDumpingFactor = 62,
        MoveTurnSpeed = 63,
        
        // Fly
        FlyAltitude = 80,
        FlyDumping = 81,
        FlyTrunDumping = 82,
    }
}