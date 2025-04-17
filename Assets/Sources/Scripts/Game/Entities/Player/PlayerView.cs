using Potman.Game.Entities.Abstractions;
using UnityEngine;

namespace Potman.Game.Entities.Player
{
    [RequireComponent(typeof(IEntityMapper))]
    [DisallowMultipleComponent]
    public class PlayerView : MonoBehaviour, IRuntimeEntityView, IEntityComponent
    {
        public IEntityMapper Mapper { get; private set; }

        public void Init()
        {
            if (Mapper != null)
                return;
            
            Mapper = GetComponent<IEntityMapper>();
        }

        public void Dispose()
        {
            if (Mapper == null)
                return;

            Mapper = null;
        }
        
        private void OnDestroy() => Dispose();
        public void Release(){}
    }
}