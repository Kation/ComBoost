using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;

namespace DataUnitTest
{
    public static class DataCommon
    {
        public static IServiceProvider GetServiceProvider()
        {
            ServiceCollection collection = new ServiceCollection();

            collection.AddTransient<DbContext, DataContext>(serviceProvider =>
            new DataContext(new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase().Options.WithExtension(new ComBoostOptionExtension())));
            collection.AddTransient<IDatabaseContext, DatabaseContext>();

            return collection.BuildServiceProvider();
        }
    }
}
