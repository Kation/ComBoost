using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.Mvc
{
    public class EntityModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var metadata = EntityDescriptor.GetMetadata(bindingContext.ModelType);
            var key = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).ToString();
            var databaseContext = bindingContext.HttpContext.RequestServices.GetRequiredService<IDatabaseContext>();
            var entityContext = databaseContext.GetDynamicContext(metadata.Type);
            var entity = await EntityContextExtensions.GetAsync(entityContext, key);
            if (entity != null)
                bindingContext.Result = ModelBindingResult.Success(entity);
            else
                bindingContext.Result = ModelBindingResult.Failed();
        }
    }
}
