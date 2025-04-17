namespace Potman.Game.Entities.Player.Abstractions
{
    public interface IAutoTargeting
    {
        bool Enabled { get; }
        
        void Enable(bool value);
    }
}