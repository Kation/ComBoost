using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wodsoft.Net.Service;

namespace System.Data.Entity
{
    [ServiceContract]
    public interface ICacheEntityQueryable<TEntity> : IEntityQueryable<TEntity> where TEntity : CacheEntityBase, new()
    {
        [OperationContract]
        Guid[] GetKeys(DateTime updateTime);
        [OperationContract]
        bool IsUpdated(Guid entityID, DateTime lastUpdateTime);
    }
}
