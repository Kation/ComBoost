using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public interface IComBoostDistributedBuilder
    {
        IServiceCollection Services { get; }

        IComBoostDistributedBuilder AddDistributedEventHandler<THandler, TArgs>()
            where THandler : IDomainServiceEventHandler<TArgs>, new()
            where TArgs : DomainServiceEventArgs;

        IComBoostDistributedBuilder AddDistributedEventHandler<TArgs>(DomainServiceEventHandler<TArgs> handler)
            where TArgs : DomainServiceEventArgs;
    }
}
