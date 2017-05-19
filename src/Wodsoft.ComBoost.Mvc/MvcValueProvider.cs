using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Internal;
using System.Collections;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Wodsoft.ComBoost.AspNetCore;

namespace Wodsoft.ComBoost.Mvc
{
    public class MvcValueProvider : HttpValueProvider, Microsoft.AspNetCore.Mvc.ModelBinding.IValueProvider
    {
        private Dictionary<string, object> _Values;
        private IModelBinderFactory _ModelBinderFactory;
        private IModelMetadataProvider _ModelMetadataProvider;

        public MvcValueProvider(ActionContext actionContext)
            : base(actionContext.HttpContext)
        {
            ActionContext = actionContext;
            _Values = new Dictionary<string, object>();
            _ModelBinderFactory = actionContext.HttpContext.RequestServices.GetRequiredService<IModelBinderFactory>();
            _ModelMetadataProvider = actionContext.HttpContext.RequestServices.GetRequiredService<IModelMetadataProvider>();

            ValueSelectors.Insert(0, new HttpRouteValueValueSelector(HttpContext));
            ValueSelectors.Add(new HttpRouteDataTokenValueSelector(HttpContext));
        }

        public ActionContext ActionContext { get; private set; }
        
        public override object GetValue(string name, Type valueType)
        {
            if (valueType == typeof(Stream) && name == "$request")
                return HttpContext.Request.Body;
            else if (valueType == typeof(Stream) && name == "$response")
                return HttpContext.Response.Body;

            ModelBinderFactoryContext context = new ModelBinderFactoryContext();
            context.BindingInfo = new BindingInfo() { BinderModelName = name, BindingSource = BindingSource.ModelBinding };
            context.CacheToken = name + "_" + valueType.Name;
            context.Metadata = _ModelMetadataProvider.GetMetadataForType(valueType);

            var binder = _ModelBinderFactory.CreateBinder(context);
            var bindingContext = DefaultModelBindingContext.CreateBindingContext(ActionContext, this, context.Metadata, context.BindingInfo, name);
            binder.BindModelAsync(bindingContext).Wait();
            if (bindingContext.Result.IsModelSet)
                return bindingContext.Result.Model;

            return base.GetValue(name, valueType);
        }

        protected override object GetValueCore(string name)
        {
            return base.GetValueCore(name);
        }

        protected override object ConvertValue(object value, Type targetType)
        {

            return base.ConvertValue(value, targetType);
        }

        bool Microsoft.AspNetCore.Mvc.ModelBinding.IValueProvider.ContainsPrefix(string prefix)
        {
            return Keys.Any(t => t == prefix || t.StartsWith(prefix + ".") || t.StartsWith(prefix + "["));
        }

        ValueProviderResult Microsoft.AspNetCore.Mvc.ModelBinding.IValueProvider.GetValue(string key)
        {
            object value = GetValueCore(key);
            if (value is string)
                return new ValueProviderResult((string)value);
            else if (value is StringValues)
                return new ValueProviderResult((StringValues)value);
            else
                return ValueProviderResult.None;
        }
    }
}
