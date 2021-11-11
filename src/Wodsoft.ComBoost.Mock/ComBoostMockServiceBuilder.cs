using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Mock
{
    public class ComBoostMockServiceBuilder : IComBoostMockServiceBuilder
    {
        private Func<IServiceProvider> _servicesGetter;

        public ComBoostMockServiceBuilder(IServiceCollection services, Func<IServiceProvider> servicesGetter)
        {
            Services = services;
            _servicesGetter = servicesGetter;
            services.AddScoped<IMockServiceLifecycle, MockServiceLifecycle>();
        }

        public IServiceCollection Services { get; }

        public IComBoostMockServiceBuilder AddService<TService>()
            where TService : class
        {
            Services.AddScoped(sp =>
            {
                var lifecycle = sp.GetService<IMockServiceLifecycle>();
                var scope = _servicesGetter().CreateScope();
                lifecycle.Register(() => scope.Dispose());
                return scope.ServiceProvider.GetService<TService>();
            });
            return this;
        }
    }
}
