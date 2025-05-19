using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Wodsoft.ComBoost
{
    public class DomainContextAccessor : IDomainContextAccessor
    {
        private readonly AsyncLocal<IDomainContext> _store = new AsyncLocal<IDomainContext>();

        public IDomainContext? Context { get => _store.Value; set => _store.Value = value; }
    }
}
