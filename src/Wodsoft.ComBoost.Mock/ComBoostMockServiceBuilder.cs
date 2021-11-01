using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Mock
{
    public class ComBoostMockServiceBuilder : IComBoostMockServiceBuilder
    {
        private Func<IMock> _mockGetter;

        public ComBoostMockServiceBuilder(IServiceCollection services, Func<IMock> mockGetter)
        {
            Services = services;
            _mockGetter = mockGetter;
            services.AddScoped<IMockServiceLifecycle, MockServiceLifecycle>();
        }

        public IServiceCollection Services { get; }

        public IComBoostMockServiceBuilder AddService<TService>()
            where TService : class
        {
            Services.AddScoped(sp =>
            {
                var lifecycle = sp.GetService<IMockServiceLifecycle>();
                var mock = _mockGetter();
                var scope = mock.ServiceProvider.CreateScope();
                lifecycle.Register(() => scope.Dispose());
                return scope.ServiceProvider.GetService<TService>();
            });
            return this;
        }
    }
}
