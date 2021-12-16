using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Wodsoft.ComBoost.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Wodsoft.ComBoost;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DomainMvcDependencyInjectionExtensions
    {
        public static void AddComBoostMvcOptions(this MvcOptions options)
        {
            options.ModelBinderProviders.Add(new SelectedFileBinderProvider());
        }

        public static void AddComBoostMvc(this IMvcBuilder builder)
        {
            var assembly = Assembly.GetCallingAssembly();
            builder.ConfigureApplicationPartManager(manager =>
            {
                manager.ApplicationParts.Add(new DomainApplicationPart(assembly));
            });
        }

        public static IComBoostBuilder AddMvc(this IComBoostBuilder builder)
        {
            builder.Services.PostConfigure<CompositeDomainContextProviderOptions>(options => options.TryAddContextProvider<MvcDomainContextProvider>(200));
            builder.Services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();            
            return builder;
        }
    }
}