using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Wodsoft.ComBoost.Mock;
using Microsoft.EntityFrameworkCore;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Security;

namespace DataUnitTest
{
    public class UnitTestEnvironment : MockEnvironment
    {
        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<DbContext, DataContext>(serviceProvider =>
            new DataContext(new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase().Options.WithExtension(new ComBoostOptionExtension())));
            services.AddScoped<IDatabaseContext, DatabaseContext>();
            services.AddTransient<ISecurityProvider, MockSecurityProvider<Admin>>();
            base.ConfigureServices(services);
        }
    }
}
