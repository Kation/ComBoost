using DotNetCore.CAP;
using DotNetCore.CAP.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Distributed.CAP
{
    public abstract class DomainConsumerExecutorDescriptor : ConsumerExecutorDescriptor
    {
        public abstract Task HandleAsync(IServiceProvider serviceProvider, object value, CancellationToken cancellation);

        public abstract Type ArgumentType { get; }
    }

    public class DomainConsumerExecutorDescriptor<T> : DomainConsumerExecutorDescriptor
        where T : DomainServiceEventArgs
    {
        private DomainServiceEventHandler<T> _handler;

        public DomainConsumerExecutorDescriptor(DomainServiceEventHandler<T> handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
            Attribute = new CapSubscribeAttribute(DomainDistributedEventProvider.GetTypeName(typeof(T))) { Group = "ComBoost.CAP" };
            Parameters = new List<ParameterDescriptor>
            {
                new ParameterDescriptor{ ParameterType = typeof(T), IsFromCap = false }
            };
        }

        public override Type ArgumentType => typeof(T);

        public override Task HandleAsync(IServiceProvider serviceProvider, object value, CancellationToken cancellation)
        {
            DomainContext domainContext = new EmptyDomainContext(serviceProvider, cancellation);
            DomainDistributedExecutionContext executionContext = new DomainDistributedExecutionContext(domainContext);
            return _handler(executionContext, (T)value);
        }
    }
}
