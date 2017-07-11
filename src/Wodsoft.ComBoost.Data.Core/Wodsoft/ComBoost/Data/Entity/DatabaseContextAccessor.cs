using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace Wodsoft.ComBoost.Data.Entity
{
    public static class DatabaseContextAccessor
    {
        private static AsyncLocal<ContextEntry> _Context = new AsyncLocal<ContextEntry>(e =>
        {
            if (e.ThreadContextChanged)
            {
                var context = SynchronizationContext.Current;
                if (e.PreviousValue != null && context == e.PreviousValue.Context)
                    _Context.Value = e.PreviousValue;
            }
        });
        public static IDatabaseContext Context
        {
            get { return _Context.Value?.Value; }
            set
            {
                _Context.Value = new ContextEntry
                {
                    Value = value,
                    Context = SynchronizationContext.Current
                };
            }
        }

        private class ContextEntry
        {
            public IDatabaseContext Value;

            public SynchronizationContext Context;
        }
    }
}
