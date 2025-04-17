using System;
using System.Collections.Generic;
using Potman.Common.Pools.Abstractions;

namespace Potman.Game.Entities.Abstractions
{
    public interface IRuntimePool : IPool<IRuntimeEntity>
    {
        bool Contains(IRuntimeEntity value);
        
        IRuntimeEntity Find(Predicate<IRuntimeEntity> predicate);
        IEnumerable<IRuntimeEntity> FindAll(Predicate<IRuntimeEntity> predicate, bool asQuery = false);
        bool TryFind(Predicate<IRuntimeEntity> predicate, out IRuntimeEntity result);
        bool TryFind(Predicate<IRuntimeEntity> predicate, out IEnumerable<IRuntimeEntity> result, bool asQuery = false);
        
        TEntity Find<TEntity>(Predicate<TEntity> predicate) where TEntity : IRuntimeEntity;
        IEnumerable<TEntity> FindAll<TEntity>(Predicate<TEntity> predicate, bool asQuery = false) where TEntity : IRuntimeEntity;
        bool TryFind<TEntity>(Predicate<TEntity> predicate, out TEntity result) where TEntity : IRuntimeEntity;
        bool TryFind<TEntity>(Predicate<TEntity> predicate, out IEnumerable<TEntity> result, bool asQuery = false) where TEntity : IRuntimeEntity;
    }
}