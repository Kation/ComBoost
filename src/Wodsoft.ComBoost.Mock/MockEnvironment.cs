using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Mock
{
    public abstract class MockEnvironment
    {
        private bool _Initialized;
        public MockEnvironment()
        {
            _Initialized = false;
        }

        public void Initialize()
        {
            if (_Initialized)
                return;

            Configuration = InitializeCore().Build();

            ServiceCollection collection = new ServiceCollection();
            ConfigureServices(collection);

            var service = collection.BuildServiceProvider();
            ServiceProvider = service;

            _Initialized = true;
        }

        public virtual IConfigurationBuilder InitializeCore()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
            return builder;
        }

        public async Task Run(Func<IServiceProvider, Task> action)
        {
            using (var scope = GetServiceScope())
            {
                RequestScope.Current = new RequestScope();
                await action(scope.ServiceProvider);
            }
        }

        public IServiceScope GetServiceScope()
        {
            if (!_Initialized)
                Initialize();
            return new MockServiceScope(SynchronizationContext.Current, ServiceProvider.CreateScope());
        }

        protected IConfigurationRoot Configuration { get; private set; }

        protected IServiceProvider ServiceProvider { get; private set; }

        protected virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IAuthenticationProvider, MockAuthenticationProvider>();
            services.AddSingleton<IDomainServiceAccessor, DomainServiceAccessor>();
            services.AddSingleton<IDomainServiceProvider, DomainProvider>(t =>
            {
                var provider = new DomainProvider(t);
                ConfigureDomainService(provider);
                return provider;
            });
        }

        protected virtual void ConfigureDomainService(IDomainServiceProvider domainProvider)
        {

        }

        protected class MockServiceScope : IServiceScope
        {
            public MockServiceScope(SynchronizationContext synchronizationContext, IServiceScope serviceScope)
            {
                _SynchronizationContext = synchronizationContext;
                _ServiceScope = serviceScope;
                SynchronizationContext.SetSynchronizationContext(synchronizationContext?.CreateCopy() ?? new SynchronizationContext());
            }

            private SynchronizationContext _SynchronizationContext;
            private IServiceScope _ServiceScope;

            public IServiceProvider ServiceProvider { get { return _ServiceScope.ServiceProvider; } }

            public void Dispose()
            {
                SynchronizationContext.SetSynchronizationContext(_SynchronizationContext);
                _ServiceScope.Dispose();
            }
        }
    }
}
