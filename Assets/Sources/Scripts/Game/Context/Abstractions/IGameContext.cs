using Cysharp.Threading.Tasks;
using Potman.Common.DependencyInjection;
using Potman.Game.Entities.Player.Abstractions;
using Potman.Game.Scenarios.Data;

namespace Potman.Game.Context.Abstractions
{
    public interface IGameContext
    {
        IPlayerEntity Player { get; }
        IServiceProvider ServiceProvider { get; }
        int Time { get; }
        int TimeLeft { get; }
        int StartDelay { get; }
        
        int UnitsTotalMax { get; }
        int UnitsSceneMax { get; }
        int UnitsAlive { get; }
        int UnitsSpawned { get; }
        int UnitsDied { get; }
        int UnitsLeft { get; }
        bool Initialized { get; }
        
        
        void Pause(bool value);
        UniTask StartAsync(ContextConfig cfg, IPlayerEntity player);
        void End();
    }
}