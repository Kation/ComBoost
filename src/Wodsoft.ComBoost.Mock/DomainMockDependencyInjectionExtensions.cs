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
            if (builderConfigure != null)
            {
                builderConfigure(new ComBoostMockBuilder(builder.Services));
            }
            return builder;
        }

        public static IComBoostMockBuilder AddAnonymous(this IComBoostMockBuilder builder)
        {
            builder.Services.AddSingleton<IAuthenticationProvider, AnonymousAuthenticationProvider>();
            return builder;
        }

        public static IComBoostBuilder AddMockService(this IComBoostBuilder builder, Func<IMock> mockGetter, Action<IComBoostMockServiceBuilder> builderConfigure)
        {
            if (builderConfigure == null)
                throw new ArgumentNullException(nameof(builderConfigure));
            builderConfigure(new ComBoostMockServiceBuilder(builder.Services, mockGetter));
            return builder;
        }
    }
}
