using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Wodsoft.ComBoost
{
    public class DomainContextAccessor : IDomainContextAccessor
    {
        private readonly AsyncLocal<IDomainContext> _store = new AsyncLocal<IDomainContext>();
        private readonly IDomainContextProvider _provider;

        public DomainContextAccessor(IDomainContextProvider provider)
        {
            _provider = provider;
        }

        public IDomainContext Context { get => _store.Value ?? _provider.GetContext(); set => _store.Value = value; }
    }
}
