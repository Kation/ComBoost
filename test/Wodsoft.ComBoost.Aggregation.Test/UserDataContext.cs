using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Aggregation.Test.Entities;

namespace Wodsoft.ComBoost.Aggregation.Test
{
    public class UserDataContext : DbContext
    {
        public UserDataContext() { }

        public UserDataContext(DbContextOptions<UserDataContext> options) : base(options) { }

        public DbSet<UserEntity> Users { get; set; }
    }
}
