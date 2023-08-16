using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public interface IComBoostDistributedBuilder
    {
        IServiceCollection Services { get; }

        IComBoostBuilder ComBoostBuilder { get; }

        IComBoostDistributedEventProviderBuilder<TProvider> UseEventProvider<TProvider>(params object[] parameters)
            where TProvider : IDomainDistributedEventProvider;
    }

    public interface IComBoostDistributedEventProviderBuilder
    {
        IServiceCollection Services { get; }

        IComBoostDistributedEventProviderBuilder AddDistributedEventHandler<THandler, TArgs>()
            where THandler : IDomainServiceEventHandler<TArgs>, new()
            where TArgs : DomainServiceEventArgs;

        IComBoostDistributedEventProviderBuilder AddDistributedEventHandler<TArgs>(DomainServiceEventHandler<TArgs> handler)
            where TArgs : DomainServiceEventArgs;

        IComBoostDistributedEventProviderBuilder AddDistributedEventPublisher<TArgs>()
            where TArgs : DomainServiceEventArgs;

        IComBoostDistributedEventProviderBuilder WithGroupName(string groupName);
    }

    public interface IComBoostDistributedEventProviderBuilder<TProvider> : IComBoostDistributedEventProviderBuilder
            where TProvider : IDomainDistributedEventProvider
    {

    }
}
