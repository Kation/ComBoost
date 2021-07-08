using DotNetCore.CAP;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Distributed.CAP
{
    public class DomainCAPEventProvider : DomainDistributedEventProvider
    {
        private ICapPublisher _publisher;
        internal List<Delegate> Handlers = new List<Delegate>();

        public DomainCAPEventProvider(ICapPublisher publisher)
        {
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        }

        public override void RegisterEventHandler<T>(DomainServiceEventHandler<T> handler)
        {
            Handlers.Add(handler);
        }

        public override Task SendEventAsync<T>(T args)
        {
            var name = GetTypeName<T>();
            return _publisher.PublishAsync<T>(name, args);
        }

        public override void UnregisterEventHandler<T>(DomainServiceEventHandler<T> handler)
        {
            Handlers.Remove(handler);
        }
    }
}
