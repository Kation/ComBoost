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
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public abstract class EntityService<TEntity, TKey> : IEntityService<TEntity> where TEntity : class, IEntity, new()
    {
        protected abstract bool AuthenticateCore(string data);

        protected abstract IEntityQueryable<TEntity> GetQueryable();

        public bool IsAuthenticated { get; private set; }

        public bool Authenticate(string data)
        {
            if (IsAuthenticated)
                return true;
            IsAuthenticated = AuthenticateCore(data);
            return IsAuthenticated;
        }

        public TEntity GetEntity(Guid key)
        {
            if (!IsAuthenticated)
                throw new UnauthorizedAccessException("Not authenticated.");
            return GetQueryable().GetEntity(key);
        }

        public bool Add(TEntity entity)
        {
            return GetQueryable().Add(entity);
        }

        public bool AddRange(TEntity[] entities)
        {
            return GetQueryable().AddRange(entities);
        }

        public bool Edit(TEntity entity)
        {
            return GetQueryable().Edit(entity);
        }

        public bool Remove(Guid key)
        {
            return GetQueryable().Remove(key);
        }

        public ReadOnlyCollection<TEntity> Query(Expression expression)
        {
            return new ReadOnlyCollection<TEntity>(GetQueryable().Query().Provider.CreateQuery<TEntity>(expression).ToList());
        }

        public TEntity QuerySingle(Expression expression)
        {
            return GetQueryable().Query().Provider.Execute<TEntity>(expression);
        }

        public ReadOnlyCollection<TEntity> QuerySql(string sql, System.Data.SqlClient.SqlParameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            return GetQueryable().Count();
        }

        public bool Contains(Guid key)
        {
            return GetQueryable().Contains(key);
        }
    }
}
