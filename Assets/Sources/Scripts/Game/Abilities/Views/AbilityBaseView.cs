using System.Linq;
using Potman.Common.Pools;
using Potman.Common.Utilities;
using Potman.Game.Abilities.Abstractions;
using Potman.Game.Entities.Abstractions;
using UnityEngine;

namespace Potman.Game.Abilities.Views
{
    [RequireComponent(typeof(IEntityMapper))]
    [DisallowMultipleComponent]
    public abstract class AbilityBaseView : PoolableObject, IAbilityView
    {
        protected IAbility Entity { get; private set; }
        public string Id => PoolableId;
        public string AbilityId => Entity.Id;
        public IEntityMapper Mapper { get; private set; }

        internal void Construct()
        {
            Mapper = GetComponent<IEntityMapper>();
            Mapper.RecursiveMapAll<IEntityComponent>().Reverse().Each(x => x.Init());
            OnConstruct();
        }

        internal void Init(IAbility ability)
        {
            Entity = ability;
            OnInit();
        }
        
        protected virtual void OnConstruct(){}
        protected virtual void OnInit(){}

        protected override void OnReleased() => Entity = null;

        protected override void OnDisposed()
        {
            Mapper.RecursiveMapAll<IEntityComponent>().Reverse().Each(x => x?.Dispose());
            Mapper = null;
            Entity = null;
        }
    }
}