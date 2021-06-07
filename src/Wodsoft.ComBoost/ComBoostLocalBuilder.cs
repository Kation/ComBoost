using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Wodsoft.ComBoost
{
    public class ComBoostLocalBuilder : IComBoostLocalBuilder
    {
        public ComBoostLocalBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public IServiceCollection Services { get; }

        public event EventHandler<ComBoostLocalBuilderEventArgs> ServiceAdded;

        public IComBoostLocalBuilder AddEventHandler<THandler, TArgs>()
            where THandler : IDomainServiceEventHandler<TArgs>, new()
            where TArgs : DomainServiceEventArgs
        {
            Services.PostConfigure<DomainServiceEventManagerOptions>(options =>
            {
                var handler = new THandler();
                options.AddEventHandler<TArgs>(handler.Handle);
            });
            return this;
        }

        public IComBoostLocalServiceBuilder<T> AddService<T>() where T : class, IDomainService
        {
            var builder = new ComBoostLocalServiceBuilder<T>(Services);
            ServiceAdded?.Invoke(this, new ComBoostLocalBuilderEventArgs(typeof(T)));
            return builder;
        }
    }
}
