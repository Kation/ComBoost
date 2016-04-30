using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{
    public interface IDatabaseContext
    {
        IEntityContext<T> GetContext<T>();

        Task<int> SaveAsync();
    }
}
