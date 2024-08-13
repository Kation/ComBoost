using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
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
            services.AddMvc();
            services.AddDbContext<DataContext>();
            services.AddEFCoreContext<DataContext>();
            services.AddEntityDtoContext<UserEntity, UserDto>();
            services.AddComBoost()
                .AddLocalService(builder =>
                {
                    //builder.AddEntityService<Guid, UserEntity, UserDto>();
                    builder.AddEntityDtoService<UserDto>();                    
                })
                .AddMvc();

            services.AddAutoMapper(config =>
            {
                config.AddExpressionMapping();
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
