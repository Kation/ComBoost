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
        private static bool _Used;

        public static IComBoostDistributedEventProviderBuilder<DomainCAPEventProvider> UseCAP(this IComBoostDistributedBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (_Used)
                throw new NotSupportedException("CAP仅支持单个使用，不支持多个实例。");
            _Used = true;
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IDomainDistributedEventProvider, DomainCAPEventProvider>(sp => sp.GetRequiredService<DomainCAPEventProvider>()));
            builder.Services.AddSingleton<DomainCAPEventProvider>();
            builder.Services.AddSingleton<ISubscribeInvoker, DomainSubscribeInvoker>();
            builder.Services.AddSingleton<IConsumerServiceSelector, DomainConsumerServiceSelector>();
            return builder.UseEventProvider<DomainCAPEventProvider>();
        }

        public static IComBoostDistributedEventProviderBuilder<DomainCAPEventProvider> UseCAP(this IComBoostDistributedBuilder builder, Action<CapOptions> capOptionsConfigure)
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
