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

        public override void RaiseEvent<T>(DomainServiceEventRoute route, IDomainExecutionContext context, T eventArgs)
        {
            switch (route.Strategy)
            {
                case DomainServiceEventStrategy.Bubble:
                    base.RaiseEvent(route, context, eventArgs);
                    if (eventArgs.IsHandled)
                        return;
                    if (_Parent != null)
                        _Parent.RaiseEvent(route, context, eventArgs);
                    return;
                case DomainServiceEventStrategy.Tunnel:
                    if (_Parent != null)
                        _Parent.RaiseEvent(route, context, eventArgs);
                    if (eventArgs.IsHandled)
                        return;
                    base.RaiseEvent(route, context, eventArgs);
                    return;
                case DomainServiceEventStrategy.Direct:
                    base.RaiseEvent(route, context, eventArgs);
                    return;
                default:
                    return;
            }
        }

        public override async Task RaiseAsyncEvent<T>(DomainServiceEventRoute route, IDomainExecutionContext context, T eventArgs)
        {
            switch (route.Strategy)
            {
                case DomainServiceEventStrategy.Bubble:
                    await base.RaiseAsyncEvent(route, context, eventArgs);
                    if (eventArgs.IsHandled)
                        return;
                    if (_Parent != null)
                        await _Parent.RaiseAsyncEvent(route, context, eventArgs);
                    return;
                case DomainServiceEventStrategy.Tunnel:
                    if (_Parent != null)
                        await _Parent.RaiseAsyncEvent(route, context, eventArgs);
                    if (eventArgs.IsHandled)
                        return;
                    await base.RaiseAsyncEvent(route, context, eventArgs);
                    return;
                case DomainServiceEventStrategy.Direct:
                    await base.RaiseAsyncEvent(route, context, eventArgs);
                    return;
                default:
                    return;
            }
        }
    }
}
