using System;
using System.Collections.Generic;
using System.Text;
using Wodsoft.ComBoost;
using Wodsoft.ComBoost.Mock;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DomainMockDependencyInjectionExtensions
    {
        public static IComBoostBuilder AddMock(this IComBoostBuilder builder)
        {
            builder.Services.AddScoped<IDomainContextProvider, MockDomainContextProvider>();
            return builder;
        }
    }
}
