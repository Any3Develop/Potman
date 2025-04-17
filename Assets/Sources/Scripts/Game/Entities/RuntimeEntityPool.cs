using System;
using System.Collections.Generic;
using System.Linq;
using Potman.Common.Pools;
using Potman.Game.Entities.Abstractions;

namespace Potman.Game.Entities
{
    public class RuntimeEntityPool : RuntimePool<IRuntimeEntity>, IRuntimePool
    {
        public bool Contains(IRuntimeEntity value) => ActiveList.Contains(value);

        public IRuntimeEntity Find(Predicate<IRuntimeEntity> predicate) => ActiveList.Find(predicate);

        public IEnumerable<IRuntimeEntity> FindAll(Predicate<IRuntimeEntity> predicate, bool asQuery = false)
            => AsQuery(ActiveList.Where(predicate.Invoke), asQuery);

        public bool TryFind(Predicate<IRuntimeEntity> predicate, out IRuntimeEntity result) 
            => (result = ActiveList.FirstOrDefault(predicate.Invoke)) != null;

        public bool TryFind(Predicate<IRuntimeEntity> predicate, out IEnumerable<IRuntimeEntity> result, bool asQuery = false)
        {
            result = AsQuery(ActiveList.Where(predicate.Invoke), asQuery);
            return asQuery || result.Any();
        }

        public TEntity Find<TEntity>(Predicate<TEntity> predicate) where TEntity : IRuntimeEntity
            => ActiveList.OfType<TEntity>().FirstOrDefault(predicate.Invoke);

        public IEnumerable<TEntity> FindAll<TEntity>(Predicate<TEntity> predicate, bool asQuery = false) where TEntity : IRuntimeEntity
            => AsQuery(ActiveList.OfType<TEntity>().Where(predicate.Invoke), asQuery);

        public bool TryFind<TEntity>(Predicate<TEntity> predicate, out TEntity result) where TEntity : IRuntimeEntity 
            => (result = ActiveList.OfType<TEntity>().FirstOrDefault(predicate.Invoke)) != null;

        public bool TryFind<TEntity>(Predicate<TEntity> predicate, out IEnumerable<TEntity> result, bool asQuery = false) where TEntity : IRuntimeEntity
        {
            result = AsQuery(ActiveList.OfType<TEntity>().Where(predicate.Invoke), asQuery);
            return asQuery || result.Any();
        }

        private static IEnumerable<T> AsQuery<T>(IEnumerable<T> query, bool asQuery)
            => asQuery ? query : query.ToArray();
    }
}