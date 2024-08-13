using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Wodsoft.ComBoost.AspNetCore.Test.Services;
using Wodsoft.ComBoost.Test;

namespace Wodsoft.ComBoost.AspNetCore.Test
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore().AddApiExplorer();

            services.AddComBoost()
                .AddLocalService(builder =>
                {
                    builder.AddService<TestDomainService>();
                })
                .AddAspNetCore(builder =>
                {
                    builder.AddDomainEndpoint();
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AspNetCoreTest", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDomainEndpoint();
                endpoints.MapSwagger();
            });
            
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "AspNetCore Test V1");
            });
        }
    }
}
