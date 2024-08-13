using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.Mvc
{
    public class EntityModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var modelMetadataProvider = bindingContext.HttpContext.RequestServices.GetRequiredService<IModelMetadataProvider>();
            var modelBinderFactory = bindingContext.HttpContext.RequestServices.GetRequiredService<IModelBinderFactory>();
            var metadata = EntityDescriptor.GetMetadata(bindingContext.ModelType);
            var keys = new object[metadata.KeyProperties.Count];
            for(int i = 0; i < keys.Length; i++)
            {
                var property = metadata.KeyProperties[i];
                var bindingInfo = new BindingInfo() { BinderModelName = property.ClrName, BindingSource = BindingSource.ModelBinding };
                var modelMetadata = modelMetadataProvider.GetMetadataForType(metadata.KeyProperties[i].ClrType);
                var propertyBindingContext = DefaultModelBindingContext.CreateBindingContext(bindingContext.ActionContext, bindingContext.ValueProvider, modelMetadata, bindingInfo, property.ClrName);
                ModelBinderFactoryContext binderContext = new ModelBinderFactoryContext();
                binderContext.BindingInfo = new BindingInfo() { BinderModelName = property.ClrName, BindingSource = BindingSource.ModelBinding };
                binderContext.CacheToken = property.ClrName + "_" + property.ClrType.Name;
                binderContext.Metadata = modelMetadataProvider.GetMetadataForType(property.ClrType);
                var binder = modelBinderFactory.CreateBinder(binderContext);
                await binder.BindModelAsync(propertyBindingContext);
                if (propertyBindingContext.Result.Model == null)
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                    return;
                }
                keys[i] = propertyBindingContext.Result.Model;
            }
            var databaseContext = bindingContext.HttpContext.RequestServices.GetRequiredService<IDatabaseContext>();
            var entityContext = databaseContext.GetDynamicContext(metadata.Type);
            var entity = await entityContext.GetAsync(keys);
            if (entity != null)
                bindingContext.Result = ModelBindingResult.Success(entity);
            else
                bindingContext.Result = ModelBindingResult.Failed();
        }
    }
}
