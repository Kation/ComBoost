using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
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
        private SqliteConnection _connection;
        private void CreateConnection()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            CreateConnection();
            services.AddMvc();
            services.AddDbContext<DataContext>(options => options.UseSqlite(_connection));
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
