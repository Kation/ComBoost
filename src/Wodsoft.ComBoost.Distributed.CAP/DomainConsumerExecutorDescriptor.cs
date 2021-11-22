using DotNetCore.CAP;
using DotNetCore.CAP.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

        public DomainConsumerExecutorDescriptor(DomainServiceEventHandler<T> handler, string groupName)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
            Attribute = new CapSubscribeAttribute(DomainDistributedEventProvider.GetTypeName(typeof(T))) { Group = groupName };
            Parameters = new List<ParameterDescriptor>
            {
                new ParameterDescriptor{ ParameterType = typeof(T), IsFromCap = false }
            };
        }

        public override Type ArgumentType => typeof(T);

        public override async Task HandleAsync(IServiceProvider serviceProvider, object value, CancellationToken cancellation)
        {
            DomainContext domainContext = new EmptyDomainContext(serviceProvider, cancellation);
            DomainDistributedExecutionContext executionContext = new DomainDistributedExecutionContext(domainContext);
            var logger = serviceProvider.GetRequiredService<ILogger<DomainServiceEventHandler<T>>>();
            try
            {
                await _handler(executionContext, (T)value);
                logger.LogInformation("Event handle successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex);
                throw;
            }
        }
    }
}
