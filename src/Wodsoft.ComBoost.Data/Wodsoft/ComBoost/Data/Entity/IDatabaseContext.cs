using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{
    public interface IDatabaseContext
    {
        IEnumerable<Type> SupportTypes { get; }

        IEntityContext<T> GetContext<T>() where T : class, IEntity, new();

        Task<int> SaveAsync();
    }
}
