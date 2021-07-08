using DotNetCore.CAP.Internal;
using DotNetCore.CAP.Messages;
using DotNetCore.CAP.Serialization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Distributed.CAP
{
    public class DomainSubscribeInvoker : ISubscribeInvoker
    {
        private IServiceProvider _serviceProvider;
        private ISerializer _serializer;

        public DomainSubscribeInvoker(IServiceProvider serviceProvider, ISerializer serializer)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public async Task<ConsumerExecutedResult> InvokeAsync(ConsumerContext context, CancellationToken cancellationToken = default)
        {
            if (context.ConsumerDescriptor is DomainConsumerExecutorDescriptor consumerExecutor)
            {
                await consumerExecutor.HandleAsync(_serviceProvider.CreateScope().ServiceProvider, context.DeliverMessage.Value, cancellationToken);
                return new ConsumerExecutedResult(null, context.DeliverMessage.GetId(), context.DeliverMessage.GetCallbackName());
            }
            else
                throw new NotSupportedException("Not support non domain consumer.");
        }
    }
}
