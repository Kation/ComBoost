using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public class DomainDistributedEventPublisher<T> : IDomainServiceEventHandler<T>
        where T : DomainServiceEventArgs
    {
        public Task Handle(IDomainExecutionContext context, T args)
        {
            var eventProviders = context.DomainContext.GetServices<IDomainDistributedEventProvider>();
            var features = DomainDistributedEventFeatureCache<T>.Features;
            var tasks = eventProviders.Where(t => t.CanHandleEvent<T>(DomainDistributedEventFeatureCache<T>.Features)).Select(t => t.SendEventAsync(args, features)).ToArray();
            if (tasks.Length == 0)
                return Task.CompletedTask;
            else
                return Task.WhenAll(tasks);
        }
    }
}
