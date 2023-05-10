using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Wodsoft.ComBoost
{
    public class ComBoostBuilder : IComBoostBuilder
    {
        private List<IDomainModule> _modules;

        public ComBoostBuilder(IServiceCollection services)
        {
            Services = services;
            _modules = new List<IDomainModule>();
            Modules = new ReadOnlyCollection<IDomainModule>(_modules);
        }

        public event EventHandler<DomainModuleAddedEventArgs> ModuleAdded;

        public IServiceCollection Services { get; }

        public void AddModule<TModule>()
            where TModule : IDomainModule, new()
        {
            var module = new TModule();
            _modules.Add(module);
            module.ConfigureServices(Services);
            ModuleAdded?.Invoke(this, new DomainModuleAddedEventArgs(module));
        }

        public IReadOnlyList<IDomainModule> Modules { get; }
    }
}
