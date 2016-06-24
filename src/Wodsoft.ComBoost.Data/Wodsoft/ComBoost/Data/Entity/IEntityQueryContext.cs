using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.Data.Entity
{
    public interface IEntityQueryContext<out T>
        where T : IEntity
    {
        IDatabaseContext Database { get; }

        IEntityMetadata Metadata { get; }

        IQueryable<T> Query();

        Task<TResult> LazyLoadEntityAsync<TSource, TResult>(TSource entity, Expression<Func<TSource, TResult>> expression)
            where TSource : IEntity
            where TResult : IEntity;

        Task<IQueryableCollection<TResult>> LazyLoadCollectionAsync<TSource, TResult>(TSource entity, Expression<Func<TSource, ICollection<TResult>>> expression)
            where TSource : IEntity
            where TResult : IEntity;
    }
}
