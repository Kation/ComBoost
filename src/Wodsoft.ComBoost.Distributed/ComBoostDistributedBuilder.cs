﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public class ComBoostDistributedBuilder : IComBoostDistributedBuilder
    {
        public ComBoostDistributedBuilder(IServiceCollection services, IComBoostBuilder comBoostBuilder)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            ComBoostBuilder = comBoostBuilder;
        }

        public IServiceCollection Services { get; }

        public IComBoostBuilder ComBoostBuilder { get; }

        IComBoostDistributedEventProviderBuilder<TProvider> IComBoostDistributedBuilder.UseEventProvider<TProvider>(params object[] parameters)
        {
            var builder = new ComBoostDistributedEventProviderBuilder<TProvider>(Services);
            Services.Add(ServiceDescriptor.Transient<IHostedService, DomainDistributedEventService<TProvider>>(sp => ActivatorUtilities.CreateInstance<DomainDistributedEventService<TProvider>>(sp, builder.Options, parameters)));
            return builder;
        }
    }
}
