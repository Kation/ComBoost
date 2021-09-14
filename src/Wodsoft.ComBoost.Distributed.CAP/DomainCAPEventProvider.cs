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

        public override void RegisterEventHandler<T>(DomainServiceEventHandler<T> handler, IReadOnlyList<string> features)
        {
            Handlers.Add(handler);
        }

        public override Task SendEventAsync<T>(T args, IReadOnlyList<string> features)
        {
            var name = GetTypeName<T>();
            return _publisher.PublishAsync<T>(name, args);
        }

        public override void UnregisterEventHandler<T>(DomainServiceEventHandler<T> handler, IReadOnlyList<string> features)
        {
            Handlers.Remove(handler);
        }

        public override bool CanHandleEvent<T>(IReadOnlyList<string> features)
        {
            var type = typeof(T);
            bool result = false;
            foreach (var feature in features)
            {
                switch (feature)
                {
                    case DomainDistributedEventFeatures.HandleOnce:
                        result = true;
                        break;
                    case DomainDistributedEventFeatures.MustHandle:
                        break;
                    default:
                        return false;
                }
            }
            return result;
        }
    }
}
