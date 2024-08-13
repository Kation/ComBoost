using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public class ComBoostDistributedEventProviderBuilder<TProvider> : IComBoostDistributedEventProviderBuilder<TProvider>
        where TProvider : IDomainDistributedEventProvider
    {
        public ComBoostDistributedEventProviderBuilder(IServiceCollection services)
        {
            Services = services;
            Options = new DomainServiceDistributedEventOptions<TProvider>();
        }

        public IServiceCollection Services { get; }

        public DomainServiceDistributedEventOptions<TProvider> Options { get; }

        public IComBoostDistributedEventProviderBuilder AddDistributedEventHandler<THandler, TArgs>()
            where THandler : IDomainServiceEventHandler<TArgs>, new()
            where TArgs : DomainServiceEventArgs
        {
            var handler = new THandler();
            Options.AddEventHandler<TArgs>(handler.Handle);
            return this;
        }

        public IComBoostDistributedEventProviderBuilder AddDistributedEventHandler<TArgs>(DomainServiceEventHandler<TArgs> handler) where TArgs : DomainServiceEventArgs
        {
            Options.AddEventHandler(handler);
            return this;
        }

        public IComBoostDistributedEventProviderBuilder AddDistributedEventPublisher<TArgs>()
            where TArgs : DomainServiceEventArgs
        {
            Options.AddEventPublisher<TArgs>();
            return this;
        }

        public IComBoostDistributedEventProviderBuilder WithGroupName(string groupName)
        {
            Options.GroupName = groupName;
            return this;
        }
    }
}
