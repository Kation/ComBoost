using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Test;
using Wodsoft.ComBoost.Test.Entities;
using Wodsoft.ComBoost.Test.Models;

namespace Wodsoft.ComBoost.Mvc.Data.Test
{
    public class SingleMvcStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddComBoost()
                .AddLocalService(builder =>
                {
                    builder.AddEntityService<Guid, UserEntity, UserDto>();
                })
                .AddAspNetCore(builder =>
                {
                    
                });

            services.AddAutoMapper(config =>
            {
                //config.AddExpressionMapping();
                config.AddProfile<DtoProfile>();
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
