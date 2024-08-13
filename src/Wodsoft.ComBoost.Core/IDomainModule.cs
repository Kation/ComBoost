using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public interface IDomainModule
    {
        void ConfigureServices(IServiceCollection services);

        void ConfigureDomainServices(IComBoostLocalBuilder builder);
    }
}
