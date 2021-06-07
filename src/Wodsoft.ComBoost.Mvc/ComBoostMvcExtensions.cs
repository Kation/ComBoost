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

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ComBoostMvcExtensions
    {
        public static void AddComBoostMvcOptions(this MvcOptions options)
        {
            options.ModelBinderProviders.Add(new SelectedFileBinderProvider());
        }

        public static void AddComBoostMvc(this IServiceCollection services)
        {
            services.AddScoped<IDomainContextProvider, MvcDomainContextProvider>();
        }
    }
}