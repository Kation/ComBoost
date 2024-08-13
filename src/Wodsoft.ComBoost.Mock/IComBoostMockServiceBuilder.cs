using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Mock
{
    public interface IComBoostMockServiceBuilder
    {
        IServiceCollection Services { get; }

        IComBoostMockServiceBuilder AddService<TService>() where TService : class;

        IComBoostMockServiceBuilder AddAuthenticationPassthrough();
    }
}
