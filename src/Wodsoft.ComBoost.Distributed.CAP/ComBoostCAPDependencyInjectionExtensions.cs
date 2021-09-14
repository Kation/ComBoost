using DotNetCore.CAP;
using DotNetCore.CAP.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using Wodsoft.ComBoost;
using Wodsoft.ComBoost.Distributed.CAP;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ComBoostCAPDependencyInjectionExtensions
    {
        public static IComBoostDistributedBuilder UseCAP(this IComBoostDistributedBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IDomainDistributedEventProvider, DomainCAPEventProvider>(sp => sp.GetService<DomainCAPEventProvider>()));
            builder.Services.AddSingleton<DomainCAPEventProvider>();
            builder.Services.AddSingleton<ISubscribeInvoker, DomainSubscribeInvoker>();
            builder.Services.AddSingleton<IConsumerServiceSelector, DomainConsumerServiceSelector>();
            return builder;
        }

        public static IComBoostDistributedBuilder UseCAP(this IComBoostDistributedBuilder builder, Action<CapOptions> capOptionsConfigure)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (capOptionsConfigure == null)
                throw new ArgumentNullException(nameof(capOptionsConfigure));
            builder.Services.AddCap(capOptionsConfigure);
            return UseCAP(builder);
        }
    }
}
