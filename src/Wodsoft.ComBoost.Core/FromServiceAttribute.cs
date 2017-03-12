using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 服务来源特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class FromServiceAttribute : FromAttribute
    {
        /// <summary>
        /// 实例化服务来源特性。
        /// </summary>
        public FromServiceAttribute() { IsRequired = true; }

        /// <summary>
        /// 实例化服务来源特性。
        /// </summary>
        /// <param name="isRequired">是否必须存在值。</param>
        public FromServiceAttribute(bool isRequired) { IsRequired = isRequired; }

        /// <summary>
        /// 获取是否必须存在值。默认为True。
        /// </summary>
        public bool IsRequired { get; private set; }

        /// <summary>
        /// 获取值。
        /// </summary>
        /// <param name="executionContext">领域执行上下文。</param>
        /// <param name="parameter">参数信息。</param>
        /// <returns>返回值。</returns>
        public override object GetValue(IDomainExecutionContext executionContext, ParameterInfo parameter)
        {
            var service = executionContext.DomainContext.GetService(parameter.ParameterType);
            if (service == null)
                if (parameter.HasDefaultValue)
                    service = parameter.DefaultValue;
                else if (IsRequired)
                    throw new ArgumentNullException(parameter.Name, "获取" + parameter.ParameterType.Name + "服务为空。");
            return service;
        }
    }
}
