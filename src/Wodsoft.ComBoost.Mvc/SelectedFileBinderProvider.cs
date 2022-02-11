using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Wodsoft.ComBoost.Mvc
{
    public class SelectedFileBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.Metadata.ModelType == typeof(ISelectedFile) || context.Metadata.ModelType == typeof(ISelectedFile[]))
                return new Microsoft.AspNetCore.Mvc.ModelBinding.Binders.BinderTypeModelBinder(typeof(SelectedFileBinder));

            return null;
        }
    }
}
