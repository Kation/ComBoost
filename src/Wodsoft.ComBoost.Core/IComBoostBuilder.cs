using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public interface IComBoostBuilder
    {
        IServiceCollection Services { get; }

        IReadOnlyList<IDomainModule> Modules { get; }

        void AddModule<TModule>() where TModule : IDomainModule, new();

        event EventHandler<DomainModuleAddedEventArgs> ModuleAdded;
    }
}
