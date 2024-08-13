using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Wodsoft.ComBoost
{
    public class CompositeDomainContextProviderOptions
    {
        private List<int> _orders;
        private List<Type> _providers;

        public CompositeDomainContextProviderOptions()
        {
            _orders = new List<int>();
            _providers = new List<Type>();
            Providers = new ReadOnlyCollection<Type>(_providers);
        }

        public IReadOnlyList<Type> Providers { get; }

        public void ClearContextProvider()
        {
            _orders.Clear();
            _providers.Clear();
        }

        public void AddContextProvider<T>(int order = 0)
            where T : class, IDomainContextProvider
        {
            if (_providers.Count == 0)
            {
                _orders.Add(order);
                _providers.Add(typeof(T));
                return;
            }
            int count = _providers.Count;
            int i = 0;
            for (; i < count; i++)
            {
                if (_orders[i] > order)
                    break;
            }
            _orders.Insert(i, order);
            _providers.Insert(i, typeof(T));
        }

        public void TryAddContextProvider<T>(int order = 0)
            where T : class, IDomainContextProvider
        {
            var type = typeof(T);
            if (_providers.Contains(type))
                return;
            AddContextProvider<T>(order);
        }
    }
}
