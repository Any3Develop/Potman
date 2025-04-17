namespace Potman.Game.Entities.Units.Abstractions.Swarming
{
    public interface IUnitPathFactory
    {
        IUnitPath Create(int owner);
    }
}