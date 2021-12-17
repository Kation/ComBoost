using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mock
{
    public delegate Task MockInMemoryEventHandler<T>(T args) where T : DomainServiceEventArgs;

    public class MockInMemoryInstance
    {
        private static readonly ConcurrentDictionary<object, MockInMemoryInstance> _Instances = new ConcurrentDictionary<object, MockInMemoryInstance>();

        public static MockInMemoryInstance GetInstance(object key)
        {
            return _Instances.GetOrAdd(key, (_) => new MockInMemoryInstance());
        }

        private ConcurrentDictionary<Type, ConcurrentDictionary<string, List<Delegate>>> _handlers = new ConcurrentDictionary<Type, ConcurrentDictionary<string, List<Delegate>>>();

        public void AddEventHandler<T>(MockInMemoryEventHandler<T> handler, string group) where T : DomainServiceEventArgs
        {
            _handlers.AddOrUpdate(typeof(T), type =>
            {
                var item = new ConcurrentDictionary<string, List<Delegate>>();
                item[group] = new List<Delegate> { handler };
                return item;
            }, (type, item) =>
            {
                item.AddOrUpdate(group, g => new List<Delegate> { handler }, (g, list) =>
                {
                    list.Add(handler);
                    return list;
                });
                return item;
            });
        }

        public void RemoveEventHandler<T>(MockInMemoryEventHandler<T> handler, string group) where T : DomainServiceEventArgs
        {
            if (_handlers.TryGetValue(typeof(T), out var item))
            {
                if (item.TryGetValue(group, out var list))
                    list.Remove(handler);
            }
        }

        public IReadOnlyList<MockInMemoryEventHandler<T>> GetEventHandlers<T>(bool once) where T : DomainServiceEventArgs
        {
            if (!_handlers.TryGetValue(typeof(T), out var item))
                return null;
            if (item.Count == 0)
                return null;
            if (item.TryGetValue(null, out var list))
            {
                if (list.Count == 0)
                    return null;
                else if (once)
                    return new MockInMemoryEventHandler<T>[1] { (MockInMemoryEventHandler<T>)list[0] };
                else
                    return list.ConvertAll(t => (MockInMemoryEventHandler<T>)t);
            }
            else
                return item.ToArray().Select(t => t.Value.Count > 0 ? (MockInMemoryEventHandler<T>)t.Value[0] : null).Where(t => t != null).ToArray();
        }
    }
}
