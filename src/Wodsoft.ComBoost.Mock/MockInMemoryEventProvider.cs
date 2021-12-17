﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mock
{
    public class MockInMemoryEventProvider : DomainDistributedEventProvider
    {
        private MockInMemoryInstance _instance;
        private MockInMemoryEventOptions _options;
        private IServiceProvider _serviceProvider;

        public MockInMemoryEventProvider(IServiceProvider serviceProvider, IOptions<MockInMemoryEventOptions> options)
        {
            _serviceProvider = serviceProvider;
            _options = options.Value;
            _instance = MockInMemoryInstance.GetInstance(options.Value.InstanceKey);
        }

        public override bool CanHandleEvent<T>(IReadOnlyList<string> features)
        {
            foreach (var feature in features)
            {
                switch (feature)
                {
                    case DomainDistributedEventFeatures.HandleOnce:
                        break;
                    case DomainDistributedEventFeatures.MustHandle:
                        break;
                    case DomainDistributedEventFeatures.Delay:
                        break;
                    case DomainDistributedEventFeatures.Group:
                        break;
                    default:
                        return false;
                }
            }
            return true;
        }

        public override void RegisterEventHandler<T>(DomainServiceEventHandler<T> handler, IReadOnlyList<string> features)
        {
            string group;
            if (features.Contains(DomainDistributedEventFeatures.Group))
                group = _options.GroupName;
            else
                group = null;
            _instance.AddEventHandler(new MockInMemoryEventHandler<T>(args =>
            {
                var scope = _serviceProvider.CreateScope();
                DomainContext domainContext = new EmptyDomainContext(scope.ServiceProvider, default(CancellationToken));
                DomainDistributedExecutionContext executionContext = new DomainDistributedExecutionContext(domainContext);
                return handler(executionContext, args);
            }), group);
        }

        public override async Task SendEventAsync<T>(T args, IReadOnlyList<string> features)
        {
            if (features.Contains(DomainDistributedEventFeatures.Delay) && args is IDomainDistributedDelayEvent delayEvent)
                await Task.Delay(delayEvent.Delay);
            bool once = features.Contains(DomainDistributedEventFeatures.HandleOnce);
            var handlers = _instance.GetEventHandlers<T>(once);
            bool must = features.Contains(DomainDistributedEventFeatures.MustHandle);
            if (must && (handlers == null || handlers.Count == 0))
                throw new InvalidOperationException($"There is no event handler for \"{typeof(T).FullName}\".");
            var tasks = handlers.Select(t => t(args)).ToArray();
            await Task.WhenAll(tasks);
        }

        public override void UnregisterEventHandler<T>(DomainServiceEventHandler<T> handler, IReadOnlyList<string> features)
        {
            throw new NotSupportedException();
        }
    }
}
