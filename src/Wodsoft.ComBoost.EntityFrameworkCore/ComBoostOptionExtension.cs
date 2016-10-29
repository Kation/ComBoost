using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class ComBoostOptionExtension : IDbContextOptionsExtension
    {
        public void ApplyServices(IServiceCollection services)
        {
            services.AddScoped<IEntityMaterializerSource, ComBoostEntityMaterializerSource>();
            services.AddScoped<CurrentDatabaseContext, CurrentDatabaseContext>();
        }
    }
}
