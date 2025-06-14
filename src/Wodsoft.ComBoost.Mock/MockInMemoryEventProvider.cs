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
        private IServiceScopeFactory _scopeFactory;
        private readonly DomainServiceDistributedEventOptions<MockInMemoryEventProvider> _eventOptions;
        private Dictionary<Delegate, Delegate> _handlers;
        private static List<Task> _Tasks = new List<Task>();

        public MockInMemoryEventProvider(IServiceScopeFactory scopeFactory, DomainServiceDistributedEventOptions<MockInMemoryEventProvider> eventOptions, MockInMemoryEventOptions options)
        {
            _scopeFactory = scopeFactory;
            _eventOptions = eventOptions;
            _options = options;
            _instance = MockInMemoryInstance.GetInstance(options.InstanceKey);
            _handlers = new Dictionary<Delegate, Delegate>();
        }

        public override bool CanHandleEvent<T>(IReadOnlyList<string> features)
        {
            bool single = false;
            bool once = false;
            foreach (var feature in features)
            {
                switch (feature)
                {
                    case DomainDistributedEventFeatures.HandleOnce:
                        once = true;
                        break;
                    case DomainDistributedEventFeatures.MustHandle:
                    case DomainDistributedEventFeatures.Delay:
                    case DomainDistributedEventFeatures.Group:
                        break;
                    case DomainDistributedEventFeatures.SingleHandler:
                        single = true;
                        break;
                    default:
                        return false;
                }
            }
            if (single && !once)
                return false;
            return true;
        }

#if NETSTANDARD2_0
        public override Task RegisterEventHandlerAsync<T>(DomainServiceEventHandler<T> handler, IReadOnlyList<string> features)
#else
        public override ValueTask RegisterEventHandlerAsync<T>(DomainServiceEventHandler<T> handler, IReadOnlyList<string> features)
#endif
        {
            lock (this)
            {
                string group;
                if (features.Contains(DomainDistributedEventFeatures.Group))
                    group = _eventOptions.GroupName;
                else
                    group = string.Empty;
                var mockHandler = new MockInMemoryEventHandler<T>(args =>
                {
                    var scope = _scopeFactory.CreateScope();
                    DomainContext domainContext = new EmptyDomainContext(scope.ServiceProvider, default(CancellationToken));
                    DomainDistributedExecutionContext executionContext = new DomainDistributedExecutionContext(domainContext);
                    return handler(executionContext, args);
                });
                _handlers.Add(handler, mockHandler);
                _instance.AddEventHandler(mockHandler, group);
            }
#if NETSTANDARD2_0
            return Task.CompletedTask;
#else
            return default;
#endif
        }

#if NETSTANDARD2_0
        public override async Task SendEventAsync<T>(T args, IReadOnlyList<string> features)
#else
        public override async ValueTask SendEventAsync<T>(T args, IReadOnlyList<string> features)
#endif
        {
            bool once = features.Contains(DomainDistributedEventFeatures.HandleOnce);
            var handlers = _instance.GetEventHandlers<T>(once);
            bool must = features.Contains(DomainDistributedEventFeatures.MustHandle);
            if (must && _options.ThrowExceptionForMustHandleEventWhenNull && (handlers == null || handlers.Count == 0))
                throw new InvalidOperationException($"There is no event handler for \"{typeof(T).FullName}\".");
            if (handlers == null)
                return;
            Task[] tasks;
            if (features.Contains(DomainDistributedEventFeatures.SingleHandler))
            {
                tasks = handlers.Select(async t =>
                {
                    if (features.Contains(DomainDistributedEventFeatures.Delay) && args is IDomainDistributedDelayEvent delayEvent)
                        await Task.Delay(delayEvent.Delay);
                    await t.Semaphore.WaitAsync();
                    try
                    {
                        await t.Delegates[0](args);
                    }
                    finally
                    {
                        t.Semaphore.Release();
                    }
                }).ToArray();
            }
            else
            {
                tasks = handlers.SelectMany(t => t.Delegates.Select(async x =>
                {
                    if (features.Contains(DomainDistributedEventFeatures.Delay) && args is IDomainDistributedDelayEvent delayEvent)
                        await Task.Delay(delayEvent.Delay);
                    await x(args);
                })).ToArray();
            }
            if (_options.IsAsyncEvent)
            {
                _Tasks.AddRange(tasks);
                _ = Task.WhenAll(tasks).ContinueWith(task =>
                {
                    foreach (var taks in tasks)
                        if (!task.IsFaulted)
                            _Tasks.Remove(task);
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

#if NETSTANDARD2_0
        public override Task UnregisterEventHandlerAsync<T>(DomainServiceEventHandler<T> handler, IReadOnlyList<string> features)
#else
        public override ValueTask UnregisterEventHandlerAsync<T>(DomainServiceEventHandler<T> handler, IReadOnlyList<string> features)
#endif
        {
            lock (this)
            {
                string group;
                if (features.Contains(DomainDistributedEventFeatures.Group))
                    group = _eventOptions.GroupName;
                else
                    group = string.Empty;
                if (_handlers.TryGetValue(handler, out var mockHandler))
                    _instance.RemoveEventHandler((MockInMemoryEventHandler<T>)mockHandler, group);
            }
#if NETSTANDARD2_0
            return Task.CompletedTask;
#else
            return default;
#endif
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
