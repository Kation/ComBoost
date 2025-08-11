using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public abstract class DomainDistributedEventProvider : IDomainDistributedEventProvider
    {
#if NETSTANDARD2_0
        public abstract Task SendEventAsync<T>(T args, IReadOnlyList<string> features) where T : DomainServiceEventArgs;

        public abstract Task RegisterEventHandlerAsync<T>(DomainServiceEventHandler<T> handler, IReadOnlyList<string> features) where T : DomainServiceEventArgs;

        public abstract Task UnregisterEventHandlerAsync<T>(DomainServiceEventHandler<T> handler, IReadOnlyList<string> features) where T : DomainServiceEventArgs;
#else
        public abstract ValueTask SendEventAsync<T>(T args, IReadOnlyList<string> features) where T : DomainServiceEventArgs;

        public abstract ValueTask RegisterEventHandlerAsync<T>(DomainServiceEventHandler<T> handler, IReadOnlyList<string> features) where T : DomainServiceEventArgs;

        public abstract ValueTask UnregisterEventHandlerAsync<T>(DomainServiceEventHandler<T> handler, IReadOnlyList<string> features) where T : DomainServiceEventArgs;
#endif

        protected virtual string GetTypeName<T>()
        {
            return _TypeNames.GetOrAdd(typeof(T), type => GetTypeName(type));
        }

        private static ConcurrentDictionary<Type, string> _TypeNames = new ConcurrentDictionary<Type, string>();
        public static string GetTypeName(Type type)
        {
            var nameAttribute = type.GetCustomAttribute<DomainDistributedEventNameAttribute>();
            if (nameAttribute != null)
                return nameAttribute.Name;
            var name = type.Namespace + "." + type.Name;
            if (type.IsGenericType)
            {
                name += "<" + string.Join(",", type.GetGenericArguments().Select(t => GetTypeName(t))) + ">";
            }
            return name;
        }

        public abstract bool CanHandleEvent<T>(IReadOnlyList<string> features) where T : DomainServiceEventArgs;

        public abstract Task StartAsync();

        public abstract Task StopAsync();
    }
}
