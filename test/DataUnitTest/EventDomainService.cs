using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost;

namespace DataUnitTest
{
    public class EventDomainService : DomainService
    {
        public static readonly DomainServiceEventRoute SyncOperatorEvent = DomainServiceEventRoute.RegisterEvent<OperatorEventArgs>("SyncOperator", typeof(EventDomainService));
        public event DomainServiceEventHandler<OperatorEventArgs> SyncOperator { add { AddEventHandler(SyncOperatorEvent, value); } remove { RemoveEventHandler(SyncOperatorEvent, value); } }

        public Task<double> SyncEventTest([FromValue]double value)
        {
            OperatorEventArgs arg = new OperatorEventArgs(value);
            RaiseEvent(SyncOperatorEvent, arg);
            value = arg.Value;
            value++;
            return Task.FromResult(value);
        }
    }

    public class OperatorEventArgs : EventArgs
    {
        public OperatorEventArgs(double value)
        {
            Value = value;
        }

        public double Value { get; set; }
    }

    public class EventDomainExtension : DomainExtension
    {
        public override void OnInitialize(IServiceProvider serviceProvider, IDomainService domainService)
        {
            EventDomainService domain = (EventDomainService)domainService;
            domain.SyncOperator += Domain_SyncOperator;
        }

        private void Domain_SyncOperator(IDomainExecutionContext context, OperatorEventArgs e)
        {
            e.Value += 2;
        }
    }
}
