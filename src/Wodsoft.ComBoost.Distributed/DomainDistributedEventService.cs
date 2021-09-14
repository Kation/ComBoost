using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class DomainDistributedEventService : IHostedService
    {
        private IEnumerable<IDomainDistributedEventProvider> _providers;
        private DomainServiceDistributedEventOptions _options;

        public DomainDistributedEventService(IEnumerable<IDomainDistributedEventProvider> providers, IOptions<DomainServiceDistributedEventOptions> options)
        {
            _providers = providers ?? throw new ArgumentNullException(nameof(providers));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        static MethodInfo _CanHandleEventMethod = typeof(IDomainDistributedEventProvider).GetMethod(nameof(IDomainDistributedEventProvider.CanHandleEvent));

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var registerMethod = typeof(IDomainDistributedEventProvider).GetMethod(nameof(IDomainDistributedEventProvider.RegisterEventHandler));
            foreach (var item in _options.GetEvents())
            {
                var features = (IReadOnlyList<string>)typeof(DomainDistributedEventFeatureCache<>).MakeGenericType(item.Key).GetProperty("Features").GetValue(null);
                foreach (var provider in _providers)
                {
                    if ((bool)_CanHandleEventMethod.MakeGenericMethod(item.Key).Invoke(provider, new object[] { features }))
                        registerMethod.MakeGenericMethod(item.Key).Invoke(provider, new object[] { item.Value, features });
                }
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            var unregisterMethod = typeof(IDomainDistributedEventProvider).GetMethod(nameof(IDomainDistributedEventProvider.UnregisterEventHandler));
            foreach (var item in _options.GetEvents())
            {
                var features = (IReadOnlyList<string>)typeof(DomainDistributedEventFeatureCache<>).MakeGenericType(item.Key).GetProperty("Features").GetValue(null);
                foreach (var provider in _providers)
                {
                    if ((bool)_CanHandleEventMethod.MakeGenericMethod(item.Key).Invoke(provider, new object[] { features }))
                        unregisterMethod.MakeGenericMethod(item.Key).Invoke(provider, new object[] { item.Value, features });
                }
            }
            return Task.CompletedTask;
        }
    }
}
