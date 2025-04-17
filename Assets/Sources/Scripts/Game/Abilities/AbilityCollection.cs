using System;
using System.Linq;
using Potman.Common.Collections;
using Potman.Game.Abilities.Abstractions;
using Potman.Game.Abilities.Data;

namespace Potman.Game.Abilities
{
    public class AbilityCollection : RuntimeCollection<IAbility>, IAbilityCollection
    {
        public bool TryGet(AbilityId id, out IAbility result) => base.TryGet(x => x.Config.id == id, out result);
        public IAbility Get(AbilityId id) => Find(x => x.Config.id == id);
        public bool Contains(AbilityId id) => this.Any(x => x.Config.id == id);

        public override void RemoveAt(int index) => Remove(this.ElementAt(index));

        public override int RemoveAll(Predicate<IAbility> predicate)
        {
            var toRemove = this.Where(predicate.Invoke).ToArray();
            foreach (var runtimeStat in toRemove)
                Remove(runtimeStat);
            
            return toRemove.Length;
        }
    }
}