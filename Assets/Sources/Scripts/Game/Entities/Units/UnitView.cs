using Potman.Game.Entities.Abstractions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Potman.Game.Entities.Units
{
    [RequireComponent(typeof(IEntityMapper))]
    [DisallowMultipleComponent]
    public class UnitView : MonoBehaviour, IRuntimeEntityView, IEntityComponent
    {
        public IEntityMapper Mapper { get; private set; }
        
        public void Init()
        {
            if (Mapper != null)
                return;
            
            Mapper = GetComponent<IEntityMapper>();
            
            var newColor = Color.Lerp(Random.value > 0.5f ? Color.red : Color.cyan,
                Random.value > 0.5f ? Color.green : Color.yellow, Random.value);

            foreach (var renderers in Mapper.RecursiveMapAll<Renderer>())
                renderers.material.color = newColor;
        }

        public virtual void Dispose()
        {
            if (Mapper == null)
                return;
            
            Mapper = null;
        }

        private void OnDestroy() => Dispose();

        public virtual void Release(){}
    }
}