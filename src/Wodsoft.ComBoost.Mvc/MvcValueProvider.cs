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

namespace Wodsoft.ComBoost.Mvc
{
    public class MvcValueProvider : IValueProvider, Microsoft.AspNetCore.Mvc.ModelBinding.IValueProvider
    {
        private Dictionary<string, object> _Values;
        private IModelBinderFactory _ModelBinderFactory;
        private IModelMetadataProvider _ModelMetadataProvider;

        public MvcValueProvider(ActionContext actionContext)
        {
            ActionContext = actionContext;
            _Values = new Dictionary<string, object>();
            _ModelBinderFactory = actionContext.HttpContext.RequestServices.GetRequiredService<IModelBinderFactory>();
            _ModelMetadataProvider = actionContext.HttpContext.RequestServices.GetRequiredService<IModelMetadataProvider>();
        }

        public ActionContext ActionContext { get; private set; }
        
        public object GetValue(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (_Values.ContainsKey(name))
                return _Values[name];
            StringValues value = ActionContext.HttpContext.Request.Query[name];
            if (value == StringValues.Empty)
                value = ActionContext.HttpContext.Request.Form[name];
            if (value == StringValues.Empty)
                return ActionContext.RouteData.Values[name] ?? ActionContext.HttpContext.Request.Headers[name];
            else
                return value;
        }

        public object GetValue(string name, Type valueType)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (valueType == null)
                throw new ArgumentNullException(nameof(valueType));
            object value;
            if (_Values.TryGetValue(name.ToLower(), out value))
                if (valueType.IsAssignableFrom(value.GetType()))
                    return value;
                else
                    return ModelBindingHelper.ConvertTo(value, valueType, System.Globalization.CultureInfo.CurrentUICulture);
            if (valueType == typeof(ISelectedFile))
            {
                var file = ActionContext.HttpContext.Request.Form.Files.GetFile(name);
                if (file == null)
                    return null;
                return new SelectedFile(file);
            }
            else if (valueType == typeof(ISelectedFile[]))
            {
                var files = ActionContext.HttpContext.Request.Form.Files.GetFiles(name);
                if (files == null)
                    return null;
                return files.Select(t => new SelectedFile(t)).ToArray();
            }
            else if (valueType == typeof(Stream) && name == "$request")
                return ActionContext.HttpContext.Request.Body;
            else if (valueType == typeof(Stream) && name == "$response")
                return ActionContext.HttpContext.Response.Body;
            else if (valueType == typeof(string))
            {
                StringValues values = ((Microsoft.AspNetCore.Mvc.ModelBinding.IValueProvider)this).GetValue(name).Values;
                if (values == StringValues.Empty)
                    return null;
                else
                    return (string)values;
            }
            ModelBinderFactoryContext context = new ModelBinderFactoryContext();
            context.BindingInfo = new BindingInfo() { BinderModelName = name, BindingSource = BindingSource.ModelBinding };
            context.CacheToken = name + "_" + valueType.Name;
            context.Metadata = _ModelMetadataProvider.GetMetadataForType(valueType);

            var binder = _ModelBinderFactory.CreateBinder(context);
            var bindingContext = DefaultModelBindingContext.CreateBindingContext(ActionContext, this, context.Metadata, context.BindingInfo, name);
            binder.BindModelAsync(bindingContext).Wait();
            return bindingContext.Result.Model;
        }

        public object GetValue(Type valueType)
        {
            if (valueType == typeof(Uri))
                return new Uri(ActionContext.HttpContext.Request.Scheme + "://" + ActionContext.HttpContext.Request.Host.Value + ActionContext.HttpContext.Request.Path.Value + ActionContext.HttpContext.Request.QueryString.Value);
            else if (valueType == typeof(Stream))
                return ActionContext.HttpContext.Request.Body;
            else
                throw new NotSupportedException();
        }

        public void SetValue(string name, object value)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            name = name.ToLower();
            if (_Values.ContainsKey(name))
                _Values[name] = value;
            else
                _Values.Add(name, value);
            _Keys = null;
        }

        bool Microsoft.AspNetCore.Mvc.ModelBinding.IValueProvider.ContainsPrefix(string prefix)
        {
            return false;
        }

        ValueProviderResult Microsoft.AspNetCore.Mvc.ModelBinding.IValueProvider.GetValue(string key)
        {
            if (ActionContext.HttpContext.Request.Query.ContainsKey(key))
            {
                var value = ActionContext.HttpContext.Request.Query[key];
                if (value == StringValues.Empty)
                    value = "";
                return new ValueProviderResult(value);
            }
            if (ActionContext.HttpContext.Request.HasFormContentType && ActionContext.HttpContext.Request.Form.ContainsKey(key))
            {
                var value = ActionContext.HttpContext.Request.Form[key];
                if (value == StringValues.Empty)
                    value = "";
                return new ValueProviderResult(value);
            }
            if (ActionContext.HttpContext.Request.Headers.ContainsKey(key))
            {
                var value = ActionContext.HttpContext.Request.Headers[key];
                if (value == StringValues.Empty)
                    value = "";
                return new ValueProviderResult(value);
            }
            var result = ActionContext.RouteData.Values[key];
            if (result == null)
                return ValueProviderResult.None;
            return new ValueProviderResult(result.ToString());
        }

        private System.Collections.ObjectModel.ReadOnlyCollection<string> _Keys;
        public ICollection<string> Keys
        {
            get
            {
                if (_Keys != null)
                    return _Keys;

                List<string> keys = new List<string>();
                keys.AddRange(_Values.Keys);
                keys.AddRange(ActionContext.HttpContext.Request.Query.Keys);
                if (ActionContext.HttpContext.Request.HasFormContentType)
                {
                    keys.AddRange(ActionContext.HttpContext.Request.Form.Keys);
                    keys.AddRange(ActionContext.HttpContext.Request.Form.Files.Select(t => t.Name));
                }
                keys.AddRange(ActionContext.RouteData.Values.Keys);
                keys.AddRange(ActionContext.HttpContext.Request.Headers.Keys);
                keys.AddRange(ActionContext.RouteData.DataTokens.Keys);
                _Keys = new System.Collections.ObjectModel.ReadOnlyCollection<string>(keys.Distinct().ToList());
                return _Keys;
            }
        }
    }
}
