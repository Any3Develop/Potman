using System;

namespace Potman.Game.Entities.Units.Data
{
    [Flags]
    public enum UnitMatchType
    {
        All = ~None, // except none, for move excepts do this example: All = ~None & ~Static & ~Aviation
        None = 0,
        Static = 2,
        Aviation = 4,
        Infantry = 8,
    }
}