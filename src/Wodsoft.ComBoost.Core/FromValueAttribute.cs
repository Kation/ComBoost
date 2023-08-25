using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 值提供器来源特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class FromValueAttribute : FromAttribute
    {
        /// <summary>
        /// 实例化值提供器来源特性。
        /// </summary>
        public FromValueAttribute() : this(true) { }

        /// <summary>
        /// 实例化值提供器来源特性。
        /// </summary>
        /// <param name="isRequired">是否必须存在值。</param>
        public FromValueAttribute(bool isRequired) { IsRequired = isRequired; }

        /// <summary>
        /// 实例化值提供器来源特性。
        /// </summary>
        /// <param name="defaultValue">默认值。</param>
        public FromValueAttribute(object defaultValue) : this(false)
        {
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// 自定义来源名称。
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 获取是否必须存在值。默认为True。
        /// </summary>
        public bool IsRequired { get; private set; }

        /// <summary>
        /// 获取或设置默认值。
        /// </summary>
        public object? DefaultValue { get; set; }

        /// <summary>
        /// 获取值。
        /// </summary>
        /// <param name="context">领域执行上下文。</param>
        /// <param name="parameter">参数信息。</param>
        /// <returns>返回值。</returns>
        public override object? GetValue(IDomainContext context, ParameterInfo parameter)
        {
            IValueProvider provider = context.ValueProvider;
            object? value = provider.GetValue(Name ?? parameter.Name, parameter.ParameterType);
            if (value == null)
                if (DefaultValue != null || parameter.HasDefaultValue)
                    value = DefaultValue ?? parameter.DefaultValue;
                else if (IsRequired)
                    throw new DomainServiceException(new ArgumentNullException(parameter.Name, "获取" + (Name ?? parameter.Name) + "的值为空。"));
            return value;
        }
    }
}
