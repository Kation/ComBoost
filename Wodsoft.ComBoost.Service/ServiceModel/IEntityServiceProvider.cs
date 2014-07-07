using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel
{
    public interface IEntityServiceProvider
    {
        IEntityService<TEntity> GetService<TEntity>() where TEntity : class, IEntity, new();

        object GetService(Type entityType);
    }
}
