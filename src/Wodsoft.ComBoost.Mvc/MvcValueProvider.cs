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

namespace Wodsoft.ComBoost.Mvc
{
    public class MvcValueProvider : IValueProvider
    {
        public MvcValueProvider(Controller controller)
        {
            Controller = controller;
        }

        public Controller Controller { get; private set; }

        private IModelMetadataProvider _MetadataProvider;
        public IModelMetadataProvider MetadataProvider
        {
            get
            {
                if (_MetadataProvider == null)
                    _MetadataProvider = new EmptyModelMetadataProvider();
                return _MetadataProvider;
            }
        }
        public object GetValue(string name)
        {
            StringValues value = Controller.Request.Query[name];
            if (value == StringValues.Empty)
                value = Controller.Request.Form[name];
            if (value == StringValues.Empty)
                return Controller.HttpContext.GetRouteData()?.Values[name] ?? Controller.Request.Headers[name];
            else
                return value;
        }

        public object GetValue(string name, Type valueType)
        {
            if (valueType == typeof(ISelectedFile))
            {
                var file = Controller.Request.Form.Files.GetFile(name);
                if (file == null)
                    return null;
                return new SelectedFile(file);
            }
            else if (valueType == typeof(ISelectedFile[]))
            {
                var files = Controller.Request.Form.Files.GetFiles(name);
                if (files == null)
                    return null;
                return files.Select(t => new SelectedFile(t)).ToArray();
            }
            object value = Controller.Request.Query[name];
            if (value == StringValues.Empty && Controller.Request.HasFormContentType)
                value = Controller.Request.Form[name];
            if (value == StringValues.Empty)
                value = Controller.HttpContext.GetRouteData()?.Values[name] ?? Controller.Request.Headers[name];
            if (value == null)
                value = Controller.HttpContext.GetRouteData()?.DataTokens[name];
            if (value == null || value == StringValues.Empty)
            {
                if (valueType.GetTypeInfo().IsValueType)
                    return Activator.CreateInstance(valueType);
                return null;
            }
            if (value is StringValues && !valueType.IsArray && !typeof(IEnumerable).IsAssignableFrom(valueType))
                value = ((StringValues)value)[0];

            return ModelBindingHelper.ConvertTo(value, valueType);
        }

        public object GetValue(Type valueType)
        {
            throw new NotImplementedException();
        }

        private System.Collections.ObjectModel.ReadOnlyCollection<string> _Keys;
        public ICollection<string> Keys
        {
            get
            {
                if (_Keys != null)
                    return _Keys;

                List<string> keys = new List<string>();
                keys.AddRange(Controller.Request.Query.Keys);
                if (Controller.Request.HasFormContentType)
                    keys.AddRange(Controller.Request.Form.Keys);
                keys.AddRange(Controller.HttpContext.GetRouteData()?.Values.Keys);
                keys.AddRange(Controller.Request.Headers.Keys);
                keys.AddRange(Controller.HttpContext.GetRouteData()?.DataTokens.Keys);
                _Keys = new System.Collections.ObjectModel.ReadOnlyCollection<string>(keys.Distinct().ToList());
                return _Keys;
            }
        }
    }
}
