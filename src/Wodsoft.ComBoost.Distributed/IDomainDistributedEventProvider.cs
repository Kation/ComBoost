using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public interface IDomainDistributedEventProvider
    {
        Task StartAsync();

        Task StopAsync();

        bool CanHandleEvent<T>(IReadOnlyList<string> features) where T : DomainServiceEventArgs;

#if NETSTANDARD2_0
        Task SendEventAsync<T>(T args, IReadOnlyList<string> features) where T : DomainServiceEventArgs;

        Task RegisterEventHandlerAsync<T>(DomainServiceEventHandler<T> handler, IReadOnlyList<string> features) where T : DomainServiceEventArgs;

        Task UnregisterEventHandlerAsync<T>(DomainServiceEventHandler<T> handler, IReadOnlyList<string> features) where T : DomainServiceEventArgs;
#else
        ValueTask SendEventAsync<T>(T args, IReadOnlyList<string> features) where T : DomainServiceEventArgs;

        ValueTask RegisterEventHandlerAsync<T>(DomainServiceEventHandler<T> handler, IReadOnlyList<string> features) where T : DomainServiceEventArgs;

        ValueTask UnregisterEventHandlerAsync<T>(DomainServiceEventHandler<T> handler, IReadOnlyList<string> features) where T : DomainServiceEventArgs;
#endif

    }
}
