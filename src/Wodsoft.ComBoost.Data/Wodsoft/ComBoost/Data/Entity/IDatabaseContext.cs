using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{
    public interface IDatabaseContext
    {
        IEntityContext<T> GetContext<T>() where T : class, new();

        Task<int> SaveAsync();
    }
}
