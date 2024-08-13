using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Wodsoft.ComBoost.AspNetCore
{
    public interface IComBoostAspNetCoreBuilder
    {
        IServiceCollection Services { get; }

        IComBoostBuilder ComBoostBuilder { get; }

        IComBoostAspNetCoreBuilder UseAuthentication(Func<HttpContext, ClaimsPrincipal> handler);
    }
}
