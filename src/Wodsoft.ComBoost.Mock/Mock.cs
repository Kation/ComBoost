using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Mock
{
    public class Mock : IMock
    {
        private IServiceProvider _serviceProvider;
        public Mock(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        private bool _disposed = false;
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _serviceProvider = null;
            }
        }

        public async Task RunAsync(Func<IServiceProvider, Task> action)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Mock));
            using (var scope = _serviceProvider.CreateScope())
            {
                await action(scope.ServiceProvider);
            }
        }

        public void Run(Action<IServiceProvider> action)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Mock));
            using (var scope = _serviceProvider.CreateScope())
            {
                action(scope.ServiceProvider);
            }
        }

        public static IMockBuilder CreateDefaultBuilder()
        {
            var builder = new MockBuilder()
                .UseEnvironment(Environments.Development)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureConfiguration(configBuilder=>
                {
                    configBuilder.AddJsonFile("appsettings.json", true);
                })
                .ConfigureServices((config, services) =>
                {
                    services.AddSingleton<IHostEnvironment, MockEnvironment>();
                });
            return builder;
        }
    }
}
