using DotNetCore.CAP;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Distributed.CAP
{
    public class DomainCAPEventProvider : DomainDistributedEventProvider
    {
        private ICapPublisher _publisher;
        internal Dictionary<Delegate, string> Handlers = new Dictionary<Delegate, string>();
        private DomainCAPEventHandlerProvider _handlerProvider;
        private DomainServiceDistributedEventOptions<DomainCAPEventProvider> _eventOptions;
        private CapOptions _capOptions;

        public DomainCAPEventProvider(ICapPublisher publisher, DomainCAPEventHandlerProvider handlerProvider, DomainServiceDistributedEventOptions<DomainCAPEventProvider> eventOptions, IOptions<CapOptions> capOptions)
        {
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            _eventOptions = eventOptions ?? throw new ArgumentNullException(nameof(eventOptions));
            _capOptions = capOptions.Value;
            _handlerProvider = handlerProvider;
        }
                
        public override ValueTask RegisterEventHandlerAsync<T>(DomainServiceEventHandler<T> handler, IReadOnlyList<string> features)
        {
            var name = GetTypeName<T>();
            if (!string.IsNullOrEmpty(_capOptions.GroupNamePrefix))
                name = _capOptions.GroupNamePrefix + name;
            _handlerProvider.Handlers.Add(handler, (name, _eventOptions.GroupName));
            return ValueTask.CompletedTask;
        }

        public override async ValueTask SendEventAsync<T>(T args, IReadOnlyList<string> features)
        {
            var name = GetTypeName<T>();
            await _publisher.PublishAsync<T>(name, args);
        }

        public override ValueTask UnregisterEventHandlerAsync<T>(DomainServiceEventHandler<T> handler, IReadOnlyList<string> features)
        {
            _handlerProvider.Handlers.Remove(handler);
            return ValueTask.CompletedTask;
        }

        public override bool CanHandleEvent<T>(IReadOnlyList<string> features)
        {
            var type = typeof(T);
            bool result = false;
            foreach (var feature in features)
            {
                switch (feature)
                {
                    case DomainDistributedEventFeatures.HandleOnce:
                    case DomainDistributedEventFeatures.Group:
                        result = true;
                        break;
                    case DomainDistributedEventFeatures.MustHandle:
                        break;
                    default:
                        return false;
                }
            }
            return result;
        }

        public override Task StartAsync()
        {
            return Task.CompletedTask;
        }

        public override Task StopAsync()
        {
            return Task.CompletedTask;
        }
    }
}
