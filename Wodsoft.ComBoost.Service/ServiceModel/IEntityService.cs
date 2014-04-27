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
    [ServiceContract]
    public interface IEntityService<TEntity> where TEntity : class, IEntity, new()
    {
        [OperationContract]
        bool Authenticate(string data);

        [OperationContract]
        TEntity GetEntity(Guid key);

        [OperationContract]
        bool Add(TEntity entity);

        [OperationContract]
        bool AddRange(TEntity[] entities);

        [OperationContract]
        bool Edit(TEntity entity);

        [OperationContract]
        bool Remove(Guid key);

        [OperationContract]
        ReadOnlyCollection<TEntity> Query(Expression expression);

        [OperationContract]
        TEntity QuerySingle(Expression expression);

        [OperationContract]
        ReadOnlyCollection<TEntity> QuerySql(string sql, System.Data.SqlClient.SqlParameter[] parameters);

        [OperationContract]
        int Count();

        [OperationContract]
        bool Contains(Guid key);
    }
}
