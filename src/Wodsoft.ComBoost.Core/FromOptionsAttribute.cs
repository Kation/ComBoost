using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 选项来源。
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class FromOptionsAttribute : FromAttribute
    {
        /// <summary>
        /// 获取值。
        /// </summary>
        /// <param name="executionContext">领域执行上下文。</param>
        /// <param name="parameter">参数信息。</param>
        /// <returns>返回值。</returns>
        public override object GetValue(IDomainExecutionContext executionContext, ParameterInfo parameter)
        {
            var option = executionContext.DomainContext.Options.GetOption(parameter.ParameterType);
            //if (option == null)
            //    throw new ArgumentNullException(parameter.Name, "获取" + parameter.ParameterType.Name + "选项为空。");
            return option;
        }
    }
}
