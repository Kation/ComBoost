using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
            for (int i = 0; i < count; i++)
            {
                if (_orders[i] > order)
                {
                    _orders.Insert(i, order);
                    _providers.Insert(i, typeof(T));
                }
            }
        }
    }
}
