using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Wodsoft.ComBoost
{
    public class DomainServiceDistributedEventOptions
    {
        private Dictionary<Type, Delegate> _events = new Dictionary<Type, Delegate>();

        public void AddEventHandler<T>(DomainServiceEventHandler<T> handler)
            where T : DomainServiceEventArgs
        {
            _events.TryGetValue(typeof(T), out var d);
            if (d == null)
                _events[typeof(T)] = handler;
            else
                _events[typeof(T)] = Delegate.Combine(d, handler);
        }

        internal Dictionary<Type, Delegate> GetEvents() => _events;

        private string _groupName;
        public string GroupName { get => _groupName ?? Assembly.GetEntryAssembly().FullName; set => _groupName = value; }
    }
}
