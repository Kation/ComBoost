using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Wodsoft.ComBoost
{
    public abstract class DomainModule : IDomainModule
    {
        public virtual void ConfigureServices(IServiceCollection services)
        {

        }

        public virtual void ConfigureDomainServices(IComBoostLocalBuilder builder)
        {

        }
    }
}
