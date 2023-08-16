using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.AspNetCore
{
    public interface IComBoostAspNetCoreBuilder
    {
        IServiceCollection Services { get; }

        IComBoostBuilder ComBoostBuilder { get; }
    }
}
