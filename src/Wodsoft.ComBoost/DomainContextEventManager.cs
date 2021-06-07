using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class DomainContextEventManager : DomainServiceEventManager
    {
        private IDomainServiceEventManager _Parent;
        public DomainContextEventManager()
        {
        }

        public DomainContextEventManager(IDomainServiceEventManager parentManager)
        {
            _Parent = parentManager;
        }

        public override async Task RaiseEvent<T>(IDomainExecutionContext context, T eventArgs)
        {
            await base.RaiseEvent(context, eventArgs);
            if (_Parent != null)
                await _Parent.RaiseEvent(context, eventArgs);
        }
    }
}
