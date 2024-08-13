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
        public static IComBoostDistributedEventProviderBuilder<DomainCAPEventProvider> UseCAP(this IComBoostDistributedBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));            
            if (builder.ComBoostBuilder.Properties.ContainsKey("CAPEventProvider"))
                throw new NotSupportedException("CAP仅支持单个使用，不支持多个实例。");
            builder.ComBoostBuilder.Properties["CAPEventProvider"] = true;
            builder.Services.AddSingleton<ISubscribeInvoker, DomainSubscribeInvoker>();
            builder.Services.AddSingleton<IConsumerServiceSelector, DomainConsumerServiceSelector>();
            builder.Services.AddSingleton<DomainCAPEventHandlerProvider>();
            return builder.UseEventProvider<DomainCAPEventProvider>();
        }

        public static IComBoostDistributedEventProviderBuilder<DomainCAPEventProvider> UseCAP(this IComBoostDistributedBuilder builder, Action<CapOptions> capOptionsConfigure)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (capOptionsConfigure == null)
                throw new ArgumentNullException(nameof(capOptionsConfigure));
            var eventBuilder = UseCAP(builder);
            builder.Services.AddCap(capOptionsConfigure);
            return eventBuilder;
        }
    }
}
