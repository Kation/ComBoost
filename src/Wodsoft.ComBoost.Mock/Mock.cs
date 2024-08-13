using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Mock
{
    [Obsolete("Pleause use IHost instead of IMock.")]
    public class Mock : IMock
    {
        private IServiceProvider _serviceProvider;
        public Mock(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        private bool _disposed = false;

        public IServiceProvider ServiceProvider => _serviceProvider;

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
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

        [Obsolete("Pleause use Host.CreateDefaultBuilder() instead of Mock.CreateDefaultBuilder().")]
        public static IMockBuilder CreateDefaultBuilder()
        {
            var builder = new MockBuilder()
                .UseEnvironment(Environments.Development)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder.AddConsole();
                    loggingBuilder.AddDebug();
                })
                .ConfigureConfiguration(configBuilder =>
                {
                    configBuilder.AddJsonFile("appsettings.json", true);
                })
                .ConfigureServices((config, services) =>
                {
                    services.AddSingleton<IHostEnvironment, MockEnvironment>();
                });
            return builder;
        }

        public Task StartHostedServiceAsync()
        {
            var services = _serviceProvider.GetServices<IHostedService>();
            var tasks = services.Select(t => t.StartAsync(default(CancellationToken))).ToArray();
            return Task.WhenAll(tasks);
        }

        public Task StopHostedServiceAsync()
        {
            var services = _serviceProvider.GetServices<IHostedService>();
            var tasks = services.Select(t => t.StopAsync(default(CancellationToken))).ToArray();
            return Task.WhenAll(tasks);
        }
    }
}
