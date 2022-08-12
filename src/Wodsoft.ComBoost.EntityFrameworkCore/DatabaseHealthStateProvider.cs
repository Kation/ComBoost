using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class DatabaseHealthStateProvider<TDbContext> : IHealthStateProvider, IAsyncDisposable
        where TDbContext : DbContext
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Task _loopTask;
        private CancellationTokenSource _cts;

        public DatabaseHealthStateProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _cts = new CancellationTokenSource();
            _loopTask = Task.Run(Loop);
        }

        private volatile HealthState _state;
        public HealthState State
        {
            get => _state; set
            {
                if (_state != value)
                {
                    _state = value;
                    HealthStateChanged?.Invoke(this, new HealthStateChangedEventArgs(value));
                }
            }
        }

        public event EventHandler<HealthStateChangedEventArgs>? HealthStateChanged;

        private async Task Loop()
        {
            var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    if (await dbContext.Database.CanConnectAsync(_cts.Token))
                    {
                        State = HealthState.Healthy;
                    }
                    else
                    {
                        State = HealthState.Critical;
                    }
                    await Task.Delay(10000);
                }
                catch
                {

                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            _cts.Cancel();
            if (!_loopTask.IsCompleted)
                await _loopTask.ConfigureAwait(false);
        }
    }
}
