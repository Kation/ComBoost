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
        private DomainServiceDistributedEventOptions<DomainCAPEventProvider> _eventOptions;
        private CapOptions _capOptions;

        public DomainCAPEventProvider(ICapPublisher publisher, DomainServiceDistributedEventOptions<DomainCAPEventProvider> eventOptions, CapOptions capOptions)
        {
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            _eventOptions = eventOptions ?? throw new ArgumentNullException(nameof(eventOptions));
            _capOptions = capOptions ?? throw new ArgumentNullException(nameof(capOptions));
        }

        public override void RegisterEventHandler<T>(DomainServiceEventHandler<T> handler, IReadOnlyList<string> features)
        {
            var name = GetTypeName<T>();
            if (!string.IsNullOrEmpty(_capOptions.GroupNamePrefix))
                name = _capOptions.GroupNamePrefix + name;
            if (features.Contains(DomainDistributedEventFeatures.Group))
                name += "_" + _eventOptions.GroupName;
            Handlers.Add(handler, name);
        }

        public override Task SendEventAsync<T>(T args, IReadOnlyList<string> features)
        {
            var name = GetTypeName<T>();
            return _publisher.PublishAsync<T>(name, args);
        }

        public override void UnregisterEventHandler<T>(DomainServiceEventHandler<T> handler, IReadOnlyList<string> features)
        {
            Handlers.Remove(handler);
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
            throw new NotImplementedException();
        }

        public override Task StopAsync()
        {
            throw new NotImplementedException();
        }
    }
}
