using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Mvc;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ComBoostMvcDataExtensions
    {
        public static void AddComBoostMvcDataOptions(this MvcOptions options)
        {
            options.ModelBinderProviders.Insert(0, new EntityModelBinderProvider());
            options.AddComBoostMvcOptions();
        }
    }
}
