using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mvc
{
    public static class AppBuilderExtensions
    {
        public static void AddComBoostMvcOptions(this MvcOptions options)
        {
            options.ModelBinderProviders.Insert(0, new EntityModelBinderProvider());
        }
    }
}
