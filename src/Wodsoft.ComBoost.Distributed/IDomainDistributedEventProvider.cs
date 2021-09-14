using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public interface IDomainDistributedEventProvider
    {
        bool CanHandleEvent<T>(IReadOnlyList<string> features) where T : DomainServiceEventArgs;

        Task SendEventAsync<T>(T args, IReadOnlyList<string> features) where T : DomainServiceEventArgs;

        void RegisterEventHandler<T>(DomainServiceEventHandler<T> handler, IReadOnlyList<string> features) where T : DomainServiceEventArgs;

        void UnregisterEventHandler<T>(DomainServiceEventHandler<T> handler, IReadOnlyList<string> features) where T : DomainServiceEventArgs;
    }
}
