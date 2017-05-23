using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.AspNetCore
{
    public class HttpValueProvider : IValueProvider
    {
        private Dictionary<string, object> _Values;

        public HttpValueProvider(HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));
            _Values = new Dictionary<string, object>();
            ValueSelectors = new List<HttpValueSelector>();
            HttpContext = httpContext;
            IgnoreCase = true;
            ValueSelectors.Add(new HttpQueryValueSelector(httpContext));
            if (httpContext.Request.HasFormContentType)
            {
                ValueSelectors.Add(new HttpFormValueSelector(httpContext));
                ValueSelectors.Add(new HttpFormFileValueSelector(httpContext));
            }
            else if (httpContext.Request.Headers["content-type"].ToString().Contains("application/json"))
            {
                ValueSelectors.Add(new HttpJsonValueSelector(httpContext));
            }
            ValueSelectors.Add(new HttpHeaderValueSelector(httpContext));
        }

        public HttpContext HttpContext { get; private set; }

        public IList<HttpValueSelector> ValueSelectors { get; private set; }

        public bool IgnoreCase { get; set; }

        private ReadOnlyCollection<string> _Keys;
        public ICollection<string> Keys
        {
            get
            {
                if (_Keys == null)
                {
                    var keys = ValueSelectors.SelectMany(t => t.GetKeys()).Concat(_Values.Keys);
                    if (IgnoreCase)
                        keys = keys.Select(t => t.ToLower());
                    _Keys = new ReadOnlyCollection<string>(keys.Distinct().ToList());
                }
                return _Keys;
            }
        }

        public bool ContainsKey(string name)
        {
            if (name == null)
                throw new ArgumentNullException(name);
            return Keys.Contains(IgnoreCase ? name.ToLower() : name);
        }

        public virtual object GetValue(string name, Type valueType)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (valueType == null)
                throw new ArgumentNullException(nameof(valueType));
            object value = GetValueCore(name, valueType);
            if (value == null || !valueType.IsAssignableFrom(value.GetType()))
                return null;
            return value;
        }

        protected virtual object GetValueCore(string name, Type valueType)
        {
            if (_Values.ContainsKey(IgnoreCase ? name.ToLower() : name))
                return _Values[IgnoreCase ? name.ToLower() : name];
            return GetHttpValue(name, valueType);
        }

        protected virtual object GetHttpValue(string name, Type valueType)
        {
            object value = GetHttpValueCore(name);
            if (value == null)
                return null;
            return ConvertValue(value, valueType);
        }

        protected virtual object GetHttpValueCore(string name)
        {
            foreach (var selector in ValueSelectors)
                if (selector.ContainsKey(name))
                    return selector.GetValue(name);
            return null;
        }

        protected virtual object ConvertValue(object value, Type targetType)
        {
            var valueType = value.GetType();
            if (targetType.IsAssignableFrom(valueType))
                return value;

            if (valueType == typeof(StringValues) && targetType == typeof(string))
                return (string)(StringValues)value;
            else if (valueType == typeof(StringValues) && targetType == typeof(string[]))
                return (string[])(StringValues)value;
            else if (targetType == typeof(ISelectedFile) && typeof(IReadOnlyList<IFormFile>).IsAssignableFrom(valueType))
                return new SelectedFile(((IReadOnlyList<IFormFile>)value)[0]);
            else if (targetType == typeof(ISelectedFile[]) && typeof(IReadOnlyList<IFormFile>).IsAssignableFrom(valueType))
                return ((IReadOnlyList<IFormFile>)value).Select(t => new SelectedFile(t)).ToArray();

            var converter = TypeDescriptor.GetConverter(targetType);
            if (converter == null)
                throw new InvalidCastException("值类型“" + valueType.Name + "”不能转换为目标类型“" + targetType.Name + "”。");
            if (!converter.CanConvertFrom(valueType))
            {
                if (valueType == typeof(StringValues) && converter.CanConvertFrom(typeof(string)))
                    value = (string)(StringValues)value;
                else
                    throw new InvalidCastException("值类型“" + valueType.Name + "”不能转换为目标类型“" + targetType.Name + "”。");
            }
            return converter.ConvertFrom(value);
        }

        public void SetValue(string name, object value)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (IgnoreCase)
                name = name.ToLower();
            if (_Values.ContainsKey(name))
                _Values[name] = value;
            else
                _Values.Add(name, value);
            _Keys = null;
        }
    }
}
