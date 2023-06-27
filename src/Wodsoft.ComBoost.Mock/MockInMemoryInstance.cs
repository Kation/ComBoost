using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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

        private ConcurrentDictionary<Type, ConcurrentDictionary<string, MockInMemoryInstanceItem>> _handlers = new ConcurrentDictionary<Type, ConcurrentDictionary<string, MockInMemoryInstanceItem>>();

        public void AddEventHandler<T>(MockInMemoryEventHandler<T> handler, string group) where T : DomainServiceEventArgs
        {
            _handlers.AddOrUpdate(typeof(T), type =>
            {
                var item = new ConcurrentDictionary<string, MockInMemoryInstanceItem>();
                var instanceItem = new MockInMemoryInstanceItem();
                instanceItem.Delegates.Add(handler);
                item[group] = instanceItem;
                return item;
            }, (type, item) =>
            {
                item.AddOrUpdate(group, g =>
                {
                    var item = new MockInMemoryInstanceItem();
                    item.Delegates.Add(handler);
                    return item;
                }, (g, item) =>
                {
                    item.Delegates.Add(handler);
                    return item;
                });
                return item;
            });
        }

        public void RemoveEventHandler<T>(MockInMemoryEventHandler<T> handler, string group) where T : DomainServiceEventArgs
        {
            if (_handlers.TryGetValue(typeof(T), out var item))
            {
                if (item.TryGetValue(group, out var list))
                    list.Delegates.Remove(handler);
            }
        }

        public IReadOnlyList<MockInMemoryInstanceHandler<T>>? GetEventHandlers<T>(bool once) where T : DomainServiceEventArgs
        {
            if (!_handlers.TryGetValue(typeof(T), out var item))
                return null;
            if (item.Count == 0)
                return null;
            if (item.TryGetValue(string.Empty, out var list))
            {
                if (list.Delegates.Count == 0)
                    return null;
                else if (once)
                    return new MockInMemoryInstanceHandler<T>[1] { new MockInMemoryInstanceHandler<T>(new List<MockInMemoryEventHandler<T>> { (MockInMemoryEventHandler<T>)list.Delegates[0] }, list.Semaphore) };
                else
                    return new MockInMemoryInstanceHandler<T>[1] { new MockInMemoryInstanceHandler<T>(list.Delegates.ConvertAll(t => (MockInMemoryEventHandler<T>)t).ToList(), list.Semaphore) };
            }
            else
                return item.ToArray().Where(t => t.Value.Delegates.Count > 0).Select(t => new MockInMemoryInstanceHandler<T>(new List<MockInMemoryEventHandler<T>> { (MockInMemoryEventHandler<T>)t.Value.Delegates[0] }, t.Value.Semaphore)).ToArray();
        }

        private class MockInMemoryInstanceItem
        {
            public List<Delegate> Delegates { get; } = new List<Delegate>();

            public SemaphoreSlim Semaphore { get; } = new SemaphoreSlim(1);
        }
    }

    public class MockInMemoryInstanceHandler<T>
        where T : DomainServiceEventArgs
    {
        public MockInMemoryInstanceHandler(List<MockInMemoryEventHandler<T>> delegates, SemaphoreSlim semaphore)
        {
            Delegates = delegates;
            Semaphore = semaphore;
        }

        public List<MockInMemoryEventHandler<T>> Delegates { get; }

        public SemaphoreSlim Semaphore { get; }
    }
}
