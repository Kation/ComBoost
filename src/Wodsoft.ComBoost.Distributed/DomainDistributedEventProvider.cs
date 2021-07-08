using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public abstract class DomainDistributedEventProvider : IDomainDistributedEventProvider
    {
        public abstract Task SendEventAsync<T>(T args) where T : DomainServiceEventArgs;

        protected virtual string GetTypeName<T>()
        {
            return _typeNames.GetOrAdd(typeof(T), type => GetTypeName(type));
        }

        private static ConcurrentDictionary<Type, string> _typeNames = new ConcurrentDictionary<Type, string>();
        public static string GetTypeName(Type type)
        {
            var name = type.Namespace + "." + type.Name;
            if (type.IsGenericType)
            {
                name += "<" + string.Join(",", type.GetGenericArguments().Select(t => GetTypeName(t))) + ">";
            }
            return name;
        }

        public abstract void RegisterEventHandler<T>(DomainServiceEventHandler<T> handler) where T : DomainServiceEventArgs;

        public abstract void UnregisterEventHandler<T>(DomainServiceEventHandler<T> handler) where T : DomainServiceEventArgs;
    }
}
