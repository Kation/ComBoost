﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Wodsoft.ComBoost
{
    public class DomainServiceDistributedEventOptions<TProvider>
        where TProvider : IDomainDistributedEventProvider
    {
        private readonly Dictionary<Type, Delegate> _events = new Dictionary<Type, Delegate>();
        private readonly List<Type> _publishes = new List<Type>();

        public void AddEventHandler<T>(DomainServiceEventHandler<T> handler)
            where T : DomainServiceEventArgs
        {
            _events.TryGetValue(typeof(T), out var d);
            if (d == null)
                _events[typeof(T)] = handler;
            else
                _events[typeof(T)] = Delegate.Combine(d, handler);
        }

        public void AddEventPublisher<T>()
            where T : DomainServiceEventArgs
        {
            var type = typeof(T); 
            if (!_publishes.Contains(type))
                _publishes.Add(type);
        }

        internal Dictionary<Type, Delegate> GetEventHandlers() => _events;
        internal List<Type> GetEventPublishes() => _publishes;

        private string? _groupName;
        public string GroupName { get => _groupName ?? Assembly.GetEntryAssembly().GetName().Name; set => _groupName = value; }
    }
}
