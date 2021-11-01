using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mock
{
    public interface IMock : IDisposable
    {
        Task StartHostedServiceAsync();

        Task StopHostedServiceAsync();

        Task RunAsync(Func<IServiceProvider, Task> action);

        void Run(Action<IServiceProvider> action);

        IServiceProvider ServiceProvider { get; }
    }
}
