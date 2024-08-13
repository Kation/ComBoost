using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Wodsoft.ComBoost.Mock
{
    public static class MockBuilderExtensions
    {
        public static IMockBuilder ConfigureServices(this IMockBuilder builder, Action<IServiceCollection> serviceConfigure)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (serviceConfigure == null)
                throw new ArgumentNullException(nameof(serviceConfigure));
            builder.ConfigureServices((config,services) =>
            {
                serviceConfigure(services);
            });
            return builder;
        }

        public static IMockBuilder UseEnvironment(this IMockBuilder builder, string environment)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (environment == null)
                throw new ArgumentNullException(nameof(environment));
            builder.ConfigureServices(services =>
            {
                builder.Properties["EnvironmentName"] = environment;
                services.PostConfigure<MockEnvironmentOptions>(options =>
                {
                    options.EnvironmentName = environment;
                });
            });
            return builder;
        }

        public static IMockBuilder UseContentRoot(this IMockBuilder builder, string contentRoot)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (contentRoot == null)
                throw new ArgumentNullException(nameof(contentRoot));
            builder.ConfigureServices(services =>
            {
                services.PostConfigure<MockEnvironmentOptions>(options =>
                {
                    options.ContentRootPath = contentRoot;
                });
            });
            return builder;
        }
    }
}
