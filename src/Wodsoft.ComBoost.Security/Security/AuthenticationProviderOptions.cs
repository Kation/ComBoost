using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Wodsoft.ComBoost.Security
{
    public class AuthenticationProviderOptions
    {
        private List<int> _orders;
        private List<Type> _handlers;

        public AuthenticationProviderOptions()
        {
            _orders = new List<int>();
            _handlers = new List<Type>();
            Handlers = new ReadOnlyCollection<Type>(_handlers);
        }

        public IReadOnlyList<Type> Handlers { get; }

        public void AddHandler<T>(int order = 0)
            where T : class, IAuthenticationHandler
        {
            if (_handlers.Count == 0)
            {
                _orders.Add(order);
                _handlers.Add(typeof(T));
                return;
            }
            int count = _handlers.Count;
            for (int i = 0; i < count; i++)
            {
                if (_orders[i] > order)
                {
                    _orders.Insert(i, order);
                    _handlers.Insert(i, typeof(T));
                }
            }
        }
    }
}
