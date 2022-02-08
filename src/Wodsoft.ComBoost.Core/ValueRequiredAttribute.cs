using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 用于标注领域方法需要的值。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class ValueRequiredAttribute : Attribute
    {
        /// <summary>
        /// 领域方法需要的值名称。
        /// </summary>
        /// <param name="name">值名称。</param>
        public ValueRequiredAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 领域方法需要的值名称与类型。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name">值名称。</param>
        public ValueRequiredAttribute(Type type, string name)
            : this(name)
        {
            Type = type;
        }

        /// <summary>
        /// 获取值名称。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 获取值类型。
        /// </summary>
        public Type? Type { get; private set; }
    }
}
