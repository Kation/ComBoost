using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public interface IComBoostLocalBuilder
    {
        event EventHandler<ComBoostLocalBuilderEventArgs> ServiceAdded;

        IComBoostLocalBuilder AddEventHandler<THandler, TArgs>()
            where THandler : IDomainServiceEventHandler<TArgs>, new()
            where TArgs : DomainServiceEventArgs;

        IComBoostLocalBuilder AddEventHandler<TArgs>(DomainServiceEventHandler<TArgs> handler)
            where TArgs : DomainServiceEventArgs;

        IComBoostLocalServiceBuilder<T> AddService<T>() where T : class, IDomainService;

        IServiceCollection Services { get; }

        IComBoostBuilder ComBoostBuilder { get; }
    }
}
