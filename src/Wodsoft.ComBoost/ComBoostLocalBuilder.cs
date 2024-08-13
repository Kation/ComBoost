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
        public ComBoostLocalBuilder(IServiceCollection services, IComBoostBuilder comBoostBuilder)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            ComBoostBuilder = comBoostBuilder;
        }

        public IServiceCollection Services { get; }

        public IComBoostBuilder ComBoostBuilder { get; }

        public event EventHandler<ComBoostLocalBuilderEventArgs>? ServiceAdded;

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

        public IComBoostLocalBuilder AddEventHandler<TArgs>(DomainServiceEventHandler<TArgs> handler) where TArgs : DomainServiceEventArgs
        {
            Services.PostConfigure<DomainServiceEventManagerOptions>(options =>
            {
                options.AddEventHandler(handler);
            });
            return this;
        }

        public IComBoostLocalServiceBuilder<T> AddService<T>() where T : class, IDomainService
        {
            var builder = new ComBoostLocalServiceBuilder<T>(Services, this);
            ServiceAdded?.Invoke(this, new ComBoostLocalBuilderEventArgs(typeof(T), this));
            return builder;
        }
    }
}
