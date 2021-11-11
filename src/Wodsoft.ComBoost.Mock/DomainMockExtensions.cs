using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mock
{
    public static class DomainMockExtensions
    {
        public static async Task RunAsync(this IHost host, Func<IServiceProvider, Task> action)
        {
            using (var scope = host.Services.CreateScope())
            {
                await action(scope.ServiceProvider);
            }
        }

        public static void Run(this IHost host, Action<IServiceProvider> action)
        {
            using (var scope = host.Services.CreateScope())
            {
                action(scope.ServiceProvider);
            }
        }
    }
}
