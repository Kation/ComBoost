using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public interface IComBoostLocalServiceBuilder<TService>
    {
        IServiceCollection Services { get; }

        IComBoostLocalBuilder LocalBuilder { get; }

        IComBoostLocalServiceBuilder<TService> UseTemplate<TTemplate>() where TTemplate : class, IDomainTemplate;

        IComBoostLocalServiceBuilder<TService> UseFilter<TTemplate>(params string[] methods) where TTemplate : class, IDomainServiceFilter, new();
    }
}
