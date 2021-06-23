using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public interface IDomainDistributedEventProvider
    {
        Task SendEventAsync<T>(T args) where T : DomainServiceEventArgs;

        void RegisterEventHandler<T>(DomainServiceEventHandler<T> handler) where T : DomainServiceEventArgs;

        void UnregisterEventHandler<T>(DomainServiceEventHandler<T> handler) where T : DomainServiceEventArgs;
    }
}
