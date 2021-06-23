using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class DomainDistributedEventPublisher<T> : IDomainServiceEventHandler<T>
        where T : DomainServiceEventArgs
    {
        public Task Handle(IDomainExecutionContext context, T args)
        {
            var eventProvider = context.DomainContext.GetRequiredService<IDomainDistributedEventProvider>();
            return eventProvider.SendEventAsync(args);
        }
    }
}
