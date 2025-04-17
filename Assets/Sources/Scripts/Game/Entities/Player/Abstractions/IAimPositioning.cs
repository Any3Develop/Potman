using Potman.Game.Entities.Abstractions;

namespace Potman.Game.Entities.Player.Abstractions
{
    public interface IAimPositioning
    {
        bool Enabled { get; }
        bool IsAutoControll { get; }
        bool IsControlledByUser { get; }

        void Enable(bool value);
        void SetTarget(IRuntimeEntity value);
    }
}