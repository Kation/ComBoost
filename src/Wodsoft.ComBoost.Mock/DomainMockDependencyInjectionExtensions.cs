using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Wodsoft.ComBoost;
using Wodsoft.ComBoost.Mock;
using Wodsoft.ComBoost.Security;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DomainMockDependencyInjectionExtensions
    {
        public static IComBoostBuilder AddMock(this IComBoostBuilder builder, Action<IComBoostMockBuilder> builderConfigure = null)
        {
            builder.Services.AddScoped<IDomainContextProvider, MockDomainContextProvider>();
            builder.Services.AddScoped<MockAuthenticationSettings>();
            builder.Services.PostConfigure<AuthenticationProviderOptions>(options =>
            {
                options.AddHandler<MockAuthenticationHandler>();
            });
            if (builderConfigure != null)
            {
                builderConfigure(new ComBoostMockBuilder(builder.Services));
            }
            return builder;
        }

        [Obsolete]
        public static IComBoostBuilder AddMockService(this IComBoostBuilder builder, Func<IMock> mockGetter, Action<IComBoostMockServiceBuilder> builderConfigure)
        {
            if (builderConfigure == null)
                throw new ArgumentNullException(nameof(builderConfigure));
            builderConfigure(new ComBoostMockServiceBuilder(builder.Services, () => mockGetter().ServiceProvider));
            return builder;
        }

        public static IComBoostBuilder AddMockService(this IComBoostBuilder builder, Func<IHost> hostGetter, Action<IComBoostMockServiceBuilder> builderConfigure)
        {
            if (builderConfigure == null)
                throw new ArgumentNullException(nameof(builderConfigure));
            builderConfigure(new ComBoostMockServiceBuilder(builder.Services, () => hostGetter().Services));
            return builder;
        }
    }
}
