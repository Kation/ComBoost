using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;
using Wodsoft.ComBoost.EntityFramework.Test.Entities;

namespace Wodsoft.ComBoost.EntityFramework.Test
{
    [DbConfigurationType(typeof(DataContextConfiguration))]
    public class DataContext : DbContext
    {
        public DataContext() { }
        public DataContext(string connectionString) : base(connectionString) { }

        public virtual DbSet<TestEntity> Tests { get; set; }
    }
}
