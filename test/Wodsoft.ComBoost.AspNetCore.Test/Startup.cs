﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Wodsoft.ComBoost.Test;

namespace Wodsoft.ComBoost.AspNetCore.Test
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddComBoost()
                .AddLocalService(builder =>
                {
                    builder.AddService<GreeterService>().UseTemplate<IGreeterTemplate>();
                })
                .AddAspNetCore(builder=>
                {
                    
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {


                endpoints.MapDomainService();
            });
        }
    }
}