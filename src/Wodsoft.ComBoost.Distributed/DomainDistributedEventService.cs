using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class DomainDistributedEventService : IHostedService
    {
        private IDomainDistributedEventProvider _provider;
        private DomainServiceDistributedEventOptions _options;

        public DomainDistributedEventService(IDomainDistributedEventProvider provider, IOptions<DomainServiceDistributedEventOptions> options)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var method = typeof(IDomainDistributedEventProvider).GetMethod(nameof(IDomainDistributedEventProvider.RegisterEventHandler));
            foreach (var item in _options.GetEvents())
            {
                method.MakeGenericMethod(item.Key).Invoke(_provider, new object[] { item.Value });
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            var method = typeof(IDomainDistributedEventProvider).GetMethod(nameof(IDomainDistributedEventProvider.UnregisterEventHandler));
            foreach (var item in _options.GetEvents())
            {
                method.MakeGenericMethod(item.Key).Invoke(_provider, new object[] { item.Value });
            }
            return Task.CompletedTask;
        }
    }
}
