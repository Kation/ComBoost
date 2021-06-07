using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Wodsoft.ComBoost.Mock;
using Microsoft.EntityFrameworkCore;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Security;
using Wodsoft.ComBoost;

namespace DataUnitTest
{
    public class UnitTestEnvironment : Mock
    {
        private string _DatabaseName = Guid.NewGuid().ToString();

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<DbContext, DataContext>(serviceProvider =>
            new DataContext(new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase(_DatabaseName).Options));
            services.AddScoped<IDatabaseContext, DatabaseContext>();
            services.AddTransient<ISecurityProvider, MockSecurityProvider<Admin>>();
            base.ConfigureServices(services);
        }
    }
}
