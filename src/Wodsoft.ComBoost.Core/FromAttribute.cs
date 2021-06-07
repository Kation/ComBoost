using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 来源特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public abstract class FromAttribute : Attribute
    {
        /// <summary>
        /// 获取值。
        /// </summary>
        /// <param name="context">领域上下文。</param>
        /// <param name="parameter">参数信息。</param>
        /// <returns>返回值。</returns>
        public abstract object GetValue(IDomainContext context, ParameterInfo parameter);
    }
}
