using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace DataUnitTest
{
    public class DataContext : DbContext
    {
        public static DataContext Create()
        {
            var builder = new DbContextOptionsBuilder<DataContext>();
            builder.UseInMemoryDatabase().Options.WithExtension(new ComBoostOptionExtension());
            return new DataContext(builder.Options);
        }

        public DataContext(DbContextOptions options)
            : base(options)
        { }

        public DbSet<Category> Category { get; set; }

        //public DbSet<TestA> TestA { get; set; }

        //public DbSet<TestB> TestB { get; set; }

        public DbSet<User> User { get; set; }

        public DbSet<UserInfo> UserInfo { get; set; }
    }
}
