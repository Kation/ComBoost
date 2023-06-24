using Microsoft.Extensions.DependencyInjection;
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
        private Dictionary<Delegate, Delegate> _handlers;
        private static List<Task> _Tasks = new List<Task>();

        public MockInMemoryEventProvider(IServiceProvider serviceProvider, IOptions<MockInMemoryEventOptions> options)
        {
            _serviceProvider = serviceProvider;
            _options = options.Value;
            _instance = MockInMemoryInstance.GetInstance(options.Value.InstanceKey);
            _handlers = new Dictionary<Delegate, Delegate>();
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
            lock (this)
            {
                string group;
                if (features.Contains(DomainDistributedEventFeatures.Group))
                    group = _options.GroupName;
                else
                    group = string.Empty;
                var mockHandler = new MockInMemoryEventHandler<T>(args =>
                {
                    var scope = _serviceProvider.CreateScope();
                    DomainContext domainContext = new EmptyDomainContext(scope.ServiceProvider, default(CancellationToken));
                    DomainDistributedExecutionContext executionContext = new DomainDistributedExecutionContext(domainContext);
                    return handler(executionContext, args);
                });
                _handlers.Add(handler, mockHandler);
                _instance.AddEventHandler(mockHandler, group);
            }
        }

        public override async Task SendEventAsync<T>(T args, IReadOnlyList<string> features)
        {
            if (features.Contains(DomainDistributedEventFeatures.Delay) && args is IDomainDistributedDelayEvent delayEvent)
                await Task.Delay(delayEvent.Delay);
            bool once = features.Contains(DomainDistributedEventFeatures.HandleOnce);
            var handlers = _instance.GetEventHandlers<T>(once);
            bool must = features.Contains(DomainDistributedEventFeatures.MustHandle);
            if (must && _options.ThrowExceptionForMustHandleEventWhenNull && (handlers == null || handlers.Count == 0))
                throw new InvalidOperationException($"There is no event handler for \"{typeof(T).FullName}\".");
            if (handlers == null)
                return;
            var tasks = handlers.Select(t => t(args)).ToArray();
            if (_options.IsAsyncEvent)
            {
                lock (tasks)
                    _Tasks.AddRange(tasks);
                _ = Task.WhenAll(tasks).ContinueWith(task =>
                {
                    lock (tasks)
                    {
                        foreach (var taks in tasks)
                            if (!task.IsFaulted)
                                _Tasks.Remove(task);
                    }
                });
            }
            else
            {
                await Task.WhenAll(tasks);
            }
        }

        public static Task WaitEventsAsync()
        {
            Task[] tasks;
            lock (_Tasks)
            {
                if (_Tasks.Count == 0)
                    return Task.CompletedTask;
                tasks = _Tasks.ToArray();
            }
            return Task.WhenAll(tasks);
        }

        public override void UnregisterEventHandler<T>(DomainServiceEventHandler<T> handler, IReadOnlyList<string> features)
        {
            lock (this)
            {
                string group;
                if (features.Contains(DomainDistributedEventFeatures.Group))
                    group = _options.GroupName;
                else
                    group = string.Empty;
                if (_handlers.TryGetValue(handler, out var mockHandler))
                    _instance.RemoveEventHandler((MockInMemoryEventHandler<T>)mockHandler, group);
            }
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
