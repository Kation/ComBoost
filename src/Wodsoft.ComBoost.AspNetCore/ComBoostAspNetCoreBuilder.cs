using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Wodsoft.ComBoost.AspNetCore
{
    public class ComBoostAspNetCoreBuilder : IComBoostAspNetCoreBuilder
    {
        public ComBoostAspNetCoreBuilder(IServiceCollection services, IComBoostBuilder comBoostBuilder)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            ComBoostBuilder = comBoostBuilder;
        }

        public IServiceCollection Services { get; }

        public IComBoostBuilder ComBoostBuilder { get; }

        public IComBoostAspNetCoreBuilder UseAuthentication(Func<HttpContext, ClaimsPrincipal> handler)
        {
            Services.PostConfigure<DomainAspNetCoreOptions>(options => options.AuthenticationHandler = handler);
            return this;
        }
    }
}
