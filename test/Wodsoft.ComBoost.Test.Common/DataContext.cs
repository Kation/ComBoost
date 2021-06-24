using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Wodsoft.ComBoost.Test.Entities;

namespace Wodsoft.ComBoost.Test
{
    public class DataContext : DbContext
    {
        public DataContext() { }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<TestEntity> Tests { get; set; }

        public DbSet<UserEntity> Users { get; set; }

        public virtual DbSet<IncludeEntity> Includes { get; set; }

        public virtual DbSet<ThenIncludeEntity> ThenIncludes { get; set; }
    }
}
