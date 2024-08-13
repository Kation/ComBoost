using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Mock
{
    public class ComBoostMockServiceBuilder : IComBoostMockServiceBuilder
    {
        private Func<IServiceProvider> _servicesGetter;
        private bool _authenticationPassthrough;

        public ComBoostMockServiceBuilder(IServiceCollection services, Func<IServiceProvider> servicesGetter)
        {
            Services = services;
            _servicesGetter = servicesGetter;
            services.AddScoped<IMockServiceLifecycle, MockServiceLifecycle>();
        }

        public IServiceCollection Services { get; }

        public IComBoostMockServiceBuilder AddAuthenticationPassthrough()
        {
            _authenticationPassthrough = true;
            return this;
        }

        public IComBoostMockServiceBuilder AddService<TService>()
            where TService : class
        {
            Services.AddScoped(sp =>
            {
                var lifecycle = sp.GetService<IMockServiceLifecycle>();
                var scope = _servicesGetter().CreateScope();
                if (_authenticationPassthrough)
                {
                    var settings = sp.GetRequiredService<MockAuthenticationSettings>();
                    var targetSettings = scope.ServiceProvider.GetRequiredService<MockAuthenticationSettings>();
                    targetSettings.User = settings.User;
                }
                lifecycle.Register(() => scope.Dispose());
                return scope.ServiceProvider.GetService<TService>();
            });
            return this;
        }
    }
}
