using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel
{
    public interface IEntityService<TEntity> where TEntity : class, IEntity, new()
    {
        bool Authenticate(string data);

        bool IsAuthenticated();

        TEntity GetEntity(Guid id);

        Task<TEntity> GetEntityAsync(Guid id);

        bool Add(TEntity entity);

        Task<bool> AddAsync(TEntity entity);

        bool AddRange(TEntity[] entities);

        Task<bool> AddRangeAsync(TEntity[] entities);

        TEntity Create();

        bool Edit(TEntity entity);

        Task<bool> EditAsync(TEntity entity);

        bool Remove(Guid id);

        Task<bool> RemoveAsync(Guid id);

        bool RemoveRange(Guid[] keys);

        Task<bool> RemoveRangeAsync(Guid[] keys);

        IEnumerable<TEntity> Query(Expression expression);

        Task<IEnumerable<TEntity>> QueryAsync(Expression expression);

        TEntity QuerySingle(Expression expression);

        Task<TEntity> QuerySingleAsync(Expression expression);

        IEnumerable<TEntity> QuerySql(string sql, params object[] parameters);

        Task<IEnumerable<TEntity>> QuerySqlAsync(string sql, params object[] parameters);

        int Count();

        Task<int> CountAsync();

        bool Contains(Guid id);

        Task<bool> ContainsAsync(Guid id);

        bool Reload(Guid id);

        Task<bool> ReloadAsync(Guid id);
        
        bool Editable();

        bool Addable();

        bool Removeable();

        bool Readable();
    }
}
