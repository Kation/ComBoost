using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            object key = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).ToString();
            if (metadata.KeyType != typeof(string))
            {
                var converter = TypeDescriptor.GetConverter(metadata.KeyType);
                key = converter.ConvertFromString((string)key);
            }
            var databaseContext = bindingContext.HttpContext.RequestServices.GetRequiredService<IDatabaseContext>();
            var entityContext = databaseContext.GetDynamicContext(metadata.Type);
            var entity = await entityContext.GetAsync(key);
            if (entity != null)
                bindingContext.Result = ModelBindingResult.Success(entity);
            else
                bindingContext.Result = ModelBindingResult.Failed();
        }
    }
}
