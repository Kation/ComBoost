using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.EntityFramework
{
    public class DatabaseContext
    {
        public DbContext InnerContext { get; private set; }

        public DatabaseContext(DbContext context)
        {
            InnerContext = context;
        }
               

        Task<int> SaveAsync()
        {
            return InnerContext.SaveChangesAsync();
        }
    }
}
