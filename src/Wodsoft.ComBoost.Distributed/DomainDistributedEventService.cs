using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public abstract class DomainDistributedEventService
    {
        private static ConcurrentDictionary<Type, IReadOnlyList<string>> _FeatureCaches = new ConcurrentDictionary<Type, IReadOnlyList<string>>();
        protected static MethodInfo CanHandleEventMethod = typeof(IDomainDistributedEventProvider).GetMethod(nameof(IDomainDistributedEventProvider.CanHandleEvent))!;
        protected static MethodInfo AddEventHandlerMethod = typeof(IDomainServiceEventManager).GetMethod(nameof(IDomainServiceEventManager.AddEventHandler))!;
        protected static MethodInfo RemoveEventHandlerMethod = typeof(IDomainServiceEventManager).GetMethod(nameof(IDomainServiceEventManager.RemoveEventHandler))!;

        protected static IReadOnlyList<string> GetFeatures(Type eventType)
        {
            return _FeatureCaches.GetOrAdd(eventType, type =>
            {
                return (IReadOnlyList<string>)typeof(DomainDistributedEventFeatureCache<>).MakeGenericType(type).GetProperty("Features")!.GetValue(null)!;
            });
        }

        protected static IReadOnlyList<string> GetFeatures<T>()
        {
            return _FeatureCaches.GetOrAdd(typeof(T), type =>
            {
                return DomainDistributedEventFeatureCache<T>.Features;
            });
        }
    }

    public class DomainDistributedEventService<TProvider> : DomainDistributedEventService, IHostedService
        where TProvider : IDomainDistributedEventProvider
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private DomainServiceDistributedEventOptions<TProvider> _options;
        private readonly object[] _parameters;
        private IServiceScope? _scope;
        private TProvider? _provider;
        private readonly Dictionary<Type, Delegate> _delegateCache = new Dictionary<Type, Delegate>();

        public DomainDistributedEventService(IServiceScopeFactory serviceScopeFactory, DomainServiceDistributedEventOptions<TProvider> options, object[] parameters)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _options = options ?? throw new ArgumentNullException(nameof(options));
            Array.Resize(ref parameters, parameters.Length + 1);
            parameters[parameters.Length - 1] = options;
            _parameters = parameters;
        }

        static MethodInfo _EventHanderMethod = typeof(DomainDistributedEventService<TProvider>).GetMethod("PublisherEventHandler", BindingFlags.Instance | BindingFlags.NonPublic)!;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (_scope != null)
                throw new InvalidOperationException("服务已启动。");
            _scope = _serviceScopeFactory.CreateScope();
            _provider = ActivatorUtilities.CreateInstance<TProvider>(_scope.ServiceProvider, _parameters);
            await _provider.StartAsync();
            var registerMethod = typeof(IDomainDistributedEventProvider).GetMethod(nameof(IDomainDistributedEventProvider.RegisterEventHandlerAsync))!;
            foreach (var item in _options.GetEventHandlers())
            {
                var features = GetFeatures(item.Key);
                if ((bool)CanHandleEventMethod.MakeGenericMethod(item.Key).Invoke(_provider, new object[] { features })!)
#if NETSTANDARD2_0
                    await (Task)registerMethod.MakeGenericMethod(item.Key).Invoke(_provider, new object[] { item.Value, features });
#else
                    await (ValueTask)registerMethod.MakeGenericMethod(item.Key).Invoke(_provider, new object[] { item.Value, features })!;
#endif
                else
                    throw new NotSupportedException($"“{typeof(TProvider)}”不支持“{string.Join(",", features)}”类型的分布式事件“{item.Key.FullName}”。");
            }
            var eventManager = _scope.ServiceProvider.GetRequiredService<IDomainServiceEventManager>();
            foreach (var item in _options.GetEventPublishes())
            {
                var features = GetFeatures(item);
                if ((bool)CanHandleEventMethod.MakeGenericMethod(item).Invoke(_provider, new object[] { features })!)
                {
                    var d = Delegate.CreateDelegate(typeof(DomainServiceEventHandler<>).MakeGenericType(item), this, _EventHanderMethod.MakeGenericMethod(item));
                    _delegateCache[item] = d;
                    AddEventHandlerMethod.MakeGenericMethod(item).Invoke(eventManager, new object[] { d });
                }
                else
                    throw new NotSupportedException($"“{typeof(TProvider)}”不支持“{string.Join(",", features)}”类型的分布式事件“{item.FullName}”。");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_scope == null)
                throw new InvalidOperationException("服务未启动。");
            var unregisterMethod = typeof(IDomainDistributedEventProvider).GetMethod(nameof(IDomainDistributedEventProvider.UnregisterEventHandlerAsync))!;
            foreach (var item in _options.GetEventHandlers())
            {
                var features = GetFeatures(item.Key);
#if NETSTANDARD2_0
                await (Task)unregisterMethod.MakeGenericMethod(item.Key).Invoke(_provider, new object[] { item.Value, features });
#else
                await (ValueTask)unregisterMethod.MakeGenericMethod(item.Key).Invoke(_provider, new object[] { item.Value, features })!;
#endif
            }
            var eventManager = _scope.ServiceProvider.GetRequiredService<IDomainServiceEventManager>();
            foreach (var item in _delegateCache)
            {
                RemoveEventHandlerMethod.MakeGenericMethod(item.Key).Invoke(eventManager, new object[] { item.Value });
            }
            _delegateCache.Clear();
            await _provider!.StopAsync();
            _scope.Dispose();
        }

        private async Task PublisherEventHandler<T>(IDomainExecutionContext context, T args)
            where T : DomainServiceEventArgs
        {
            if (_provider == null)
                return;
            await _provider.SendEventAsync(args, GetFeatures<T>());
        }
    }
}
