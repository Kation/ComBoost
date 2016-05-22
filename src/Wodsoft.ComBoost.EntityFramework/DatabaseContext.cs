using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace Wodsoft.ComBoost.EntityFramework
{
    public class DatabaseContext : IDatabaseContext
    {
        public DbContext InnerContext { get; private set; }

        public DatabaseContext(DbContext context)
        {
            InnerContext = context;
        }

        public Task<int> SaveAsync()
        {
            return InnerContext.SaveChangesAsync();
        }

        public IEntityContext<T> GetContext<T>() where T : class, new()
        {
            return new EntityContext<T>(this, InnerContext.Set<T>());
        }
    }
}
