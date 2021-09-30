using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public class ComBoostDistributedBuilder : IComBoostDistributedBuilder
    {
        public ComBoostDistributedBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public IServiceCollection Services { get; }

        public IComBoostDistributedBuilder AddDistributedEventHandler<THandler, TArgs>()
            where THandler : IDomainServiceEventHandler<TArgs>, new()
            where TArgs : DomainServiceEventArgs
        {
            Services.PostConfigure<DomainServiceDistributedEventOptions>(options =>
            {
                var handler = new THandler();
                options.AddEventHandler<TArgs>(handler.Handle);
            });
            return this;
        }

        public IComBoostDistributedBuilder AddDistributedEventHandler<TArgs>(DomainServiceEventHandler<TArgs> handler) where TArgs : DomainServiceEventArgs
        {
            Services.PostConfigure<DomainServiceDistributedEventOptions>(options =>
            {
                options.AddEventHandler(handler);
            });
            return this;
        }

        public IComBoostDistributedBuilder WithGroupName(string groupName)
        {
            Services.PostConfigure<DomainServiceDistributedEventOptions>(options =>
            {
                options.GroupName = groupName;
            });
            return this;
        }
    }
}
