using System;

namespace Potman.Game.Entities.Abstractions
{
    public interface IEntityComponent : IDisposable
    {
        void Init();
    }
}