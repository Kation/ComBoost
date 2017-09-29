using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 用于标注领域方法需要的服务。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class ServiceRequiredAttribute : Attribute
    {
        /// <summary>
        /// 领域方法需要的服务类型。
        /// </summary>
        /// <param name="type"></param>
        public ServiceRequiredAttribute(Type type)
        {
            Type = type;
        }
        
        /// <summary>
        /// 获取值类型。
        /// </summary>
        public Type Type { get; private set; }
    }
}
