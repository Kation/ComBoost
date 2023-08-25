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
    /// <summary>
    /// Http值提供器。
    /// </summary>
    public class HttpValueProvider : IValueProvider
    {
        private Dictionary<string, object?> _Values;
        private Dictionary<string, string> _Alias;

        /// <summary>
        /// 实例化值提供器。
        /// </summary>
        /// <param name="httpContext">Http上下文。</param>
        public HttpValueProvider(HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));
            _Values = new Dictionary<string, object?>();
            _Alias = new Dictionary<string, string>();
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
            //ValueSelectors.Add(new HttpHeaderValueSelector(httpContext));
        }

        /// <summary>
        /// 获取Http上下文。
        /// </summary>
        public HttpContext HttpContext { get; private set; }

        /// <summary>
        /// 获取值选择器集合。
        /// </summary>
        public IList<HttpValueSelector> ValueSelectors { get; private set; }

        /// <summary>
        /// 获取或设置是否忽略大小写。
        /// </summary>
        public bool IgnoreCase { get; set; }

        private HttpValueKeyCollection? _Keys;
        /// <summary>
        /// 获取存在的键集合。
        /// </summary>
        public IValueKeyCollection Keys
        {
            get
            {
                if (_Keys == null)
                {
                    var keys = ValueSelectors.SelectMany(t => t.GetKeys()).Concat(_Values.Keys).Concat(_Alias.Keys);
                    _Keys = new HttpValueKeyCollection(keys.Distinct().ToList(), IgnoreCase);
                }
                return _Keys;
            }
        }

        /// <summary>
        /// 判断是否包含键名。
        /// </summary>
        /// <param name="name">键名。</param>
        /// <returns>如果包含返回true，否则返回false。</returns>
        public bool ContainsKey(string name)
        {
            if (name == null)
                throw new ArgumentNullException(name);
            return Keys.ContainsKey(name);
        }

        /// <summary>
        /// 获取值。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="valueType">值类型。</param>
        /// <returns>返回值。</returns>
        public virtual object? GetValue(string name, Type valueType)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (valueType == null)
                throw new ArgumentNullException(nameof(valueType));
            object? value = GetValueCore(name, valueType);
            if (value == null && _Alias.TryGetValue(IgnoreCase ? name.ToLower() : name, out string? alias))
                value = GetValueCore(alias, valueType);
            if (value == null)
                return null;
            var currentType = value.GetType();
            if (!valueType.IsAssignableFrom(currentType))
            {
                var converter = TypeDescriptor.GetConverter(valueType);
                if (converter.CanConvertFrom(currentType))
                    value = converter.ConvertFrom(value);
                else
                {
                    var currentConverter = TypeDescriptor.GetConverter(currentType);
                    var currentStringValue = currentConverter.ConvertToString(value);
                    if (currentStringValue == null)
                        return null;
                    value = converter.ConvertFromString(currentStringValue);
                }    
            }
            return value;
        }

        /// <summary>
        /// 获取值。
        /// </summary>
        /// <param name="name">键名。</param>
        /// <param name="valueType">值类型。</param>
        /// <returns>返回值。</returns>
        protected virtual object? GetValueCore(string name, Type valueType)
        {
            if (_Values.ContainsKey(IgnoreCase ? name.ToLower() : name))
                return _Values[IgnoreCase ? name.ToLower() : name];
            return GetHttpValue(name, valueType);
        }

        /// <summary>
        /// 获取Http值。
        /// </summary>
        /// <param name="name">键名。</param>
        /// <param name="valueType">值类型。</param>
        /// <returns>返回Http值。</returns>
        protected virtual object? GetHttpValue(string name, Type valueType)
        {
            object? value = GetHttpValueCore(name);
            if (value == null)
                return null;
            return ConvertValue(value, valueType);
        }

        /// <summary>
        /// 获取Http值。
        /// </summary>
        /// <param name="name">键名。</param>
        /// <returns>返回Http值。</returns>
        protected virtual object? GetHttpValueCore(string name)
        {
            foreach (var selector in ValueSelectors)
                if (selector.ContainsKey(name))
                    return selector.GetValue(name);
            return null;
        }

        /// <summary>
        /// 转换值。
        /// </summary>
        /// <param name="value">值。</param>
        /// <param name="targetType">目标类型。</param>
        /// <returns>返回转换后的值。</returns>
        protected virtual object? ConvertValue(object value, Type targetType)
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

        /// <summary>
        /// 设置值。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="value">值。</param>
        public void SetValue(string name, object? value)
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

        /// <summary>
        /// 设置别名。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="aliasName">别名。</param>
        public void SetAlias(string name, string aliasName)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (aliasName == null)
                throw new ArgumentNullException(nameof(aliasName));
            if (IgnoreCase)
                _Alias[aliasName.ToLower()] = name.ToLower();
            else
                _Alias[aliasName] = name;
            _Keys = null;
        }
    }
}
