using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Wodsoft.ComBoost.Forum.Domain;
using Wodsoft.ComBoost.Security;
using Wodsoft.ComBoost.Data.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Wodsoft.ComBoost.Data;
using Wodsoft.ComBoost.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Wodsoft.ComBoost.Forum.Entity;
using System.Reflection;

namespace Wodsoft.ComBoost.Forum
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            _Env = env;
        }

        private IHostingEnvironment _Env;
        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMemoryCache();
            services.AddSession();
            services.AddMvc(options =>
            {
                options.AddComBoostMvcDataOptions();
            });
            services.AddComBoostMvcAuthentication();

            services.AddScoped<DbContext, DataContext>(serviceProvider =>
                new DataContext(new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase()
                //new DataContext(new DbContextOptionsBuilder<DataContext>().UseSqlServer(Configuration.GetConnectionString("DataContext"))
                .Options));
            services.AddScoped<IDatabaseContext, DatabaseContext>();
            services.AddScoped<ISecurityProvider, ForumSecurityProvider>();
            services.AddScoped<IAuthenticationProvider, ComBoostAuthenticationProvider>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IDomainServiceAccessor, DomainServiceAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IStorageProvider, PhysicalStorageProvider>(t =>
            {
                return new PhysicalStorageProvider(PhysicalStorageOptions.CreateDefault(_Env.ContentRootPath + System.IO.Path.DirectorySeparatorChar + "Uploads"));
            });
            services.AddSingleton<IDomainServiceProvider, DomainProvider>(t =>
            {
                var provider = new DomainProvider(t);
                provider.AddGenericDefinitionExtension(typeof(EntityDomainService<>), typeof(EntitySearchExtension<>));
                provider.AddGenericDefinitionExtension(typeof(EntityDomainService<>), typeof(EntityPagerExtension<>));
                provider.AddGenericDefinitionExtension(typeof(EntityDomainService<>), typeof(EntityPasswordExtension<>));
                provider.AddGenericDefinitionExtension(typeof(EntityDomainService<>), typeof(ImageExtension<>));                
                provider.AddForumExtensions();
                return provider;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            
            app.UseComBoost();

            app.UseStaticFiles();

            app.UseSession();

            //app.UseComBoostAuthentication();

            app.UseComBoostMvc(routes =>
            {
                routes.MapAreaRoute("areaRoute", "Admin", "Admin/{controller=Home}/{action=Index}/{id?}", null, null, new
                {
                    authArea = "Admin",
                    loginPath = "/Admin/Account/SignIn",
                    logoutPath = "/Admin/Account/SignOut"
                });
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}", null, null, new
                {
                    loginPath = "/Account/SignIn",
                    logoutPath = "/Account/SignOut"
                });
            });
        }
    }
}
