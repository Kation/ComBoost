using System;
using System.Collections.Generic;
using System.Text;
using Wodsoft.ComBoost;
using Wodsoft.ComBoost.StackExchangeRedis;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RedisDependencyInjectionExtensions
    {
        public static IServiceCollection AddStackExchangeRedisProvider(this IServiceCollection services, Action<RedisOptions> optionsConfigure)
        {
            services.PostConfigure(optionsConfigure);
            return services.AddSingleton<ISemaphoreProvider, RedisProvider>();
        }
    }
}
