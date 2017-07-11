using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Wodsoft.ComBoost.AspNetCore;

namespace Wodsoft.ComBoost.Mvc
{
    public class SelectedFileBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            if (string.IsNullOrEmpty(bindingContext.BinderModelName))
                return Task.CompletedTask;


            var modelName = bindingContext.BinderModelName;

            if (!bindingContext.HttpContext.Request.HasFormContentType)
                return Task.CompletedTask;

            var files = bindingContext.HttpContext.Request.Form.Files.GetFiles(modelName).Where(t => !string.IsNullOrEmpty(t.FileName)).ToArray();
            if (files.Length == 0)
                return Task.CompletedTask;
            if (bindingContext.ModelType == typeof(ISelectedFile))
                return Task.FromResult(new SelectedFile(files[0]));
            else
                return Task.FromResult(files.Select(t => new SelectedFile(t)).ToArray());
        }
    }
}
