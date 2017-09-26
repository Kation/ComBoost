using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Threading;

namespace Wodsoft.ComBoost.Mock
{
    public class MockController : MockControllerBase
    {
        public MockController(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }

        public override MockDomainContext CreateDomainContext()
        {
            return new MockDomainContext(ServiceProvider, new CancellationTokenSource());
        }
        
        public Task ExecuteAsync<TDomainService>(IDomainContext context, string method)
            where TDomainService : IDomainService
        {
            var domain = DomainProvider.GetService<TDomainService>();
            return domain.ExecuteAsync(context, method);
        }

        public Task ExecuteAsync<TDomainService>(Action<MockDomainContext> contextSetter, string method)
            where TDomainService : IDomainService
        {
            var context = CreateDomainContext();
            contextSetter(context);
            return ExecuteAsync<TDomainService>(context, method);
        }

        public Task ExecuteAsync<TDomainService>(string method)
            where TDomainService : IDomainService
        {
            var context = CreateDomainContext();
            return ExecuteAsync<TDomainService>(context, method);
        }

        public Task<TResult> ExecuteAsync<TDomainService, TResult>(IDomainContext context, string method)
            where TDomainService : IDomainService
        {
            var domain = DomainProvider.GetService<TDomainService>();
            return domain.ExecuteAsync<TResult>(context, method);
        }

        public Task<TResult> ExecuteAsync<TDomainService, TResult>(Action<MockDomainContext> contextSetter, string method)
            where TDomainService : IDomainService
        {
            var context = CreateDomainContext();
            contextSetter(context);
            return ExecuteAsync<TDomainService, TResult>(context, method);
        }
        
        public Task<TResult> ExecuteAsync<TDomainService, TResult>(string method)
            where TDomainService : IDomainService
        {
            var context = CreateDomainContext();
            return ExecuteAsync<TDomainService, TResult>(context, method);
        }
    }
}
