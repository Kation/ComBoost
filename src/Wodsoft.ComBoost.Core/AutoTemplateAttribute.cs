using System;
using System.Collections.Generic;
using System.Text;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 自动增加领域服务模板。<br/>
    /// 领域服务类为部分类时，会自动增加DomainTemplateImplementerAttribute特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class AutoTemplateAttribute : Attribute
    {
        /// <summary>
        /// 接口名称。
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// 命名空间。
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// 模板分组。用于方法生成分组。
        /// </summary>
        public string Group { get; set; }
    }

    /// <summary>
    /// 标识增加领域服务模板行为。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class AutoTemplateMethodAttribute : Attribute
    {
        /// <summary>
        /// 是否排除。
        /// </summary>
        public bool IsExcluded { get; set; }

        /// <summary>
        /// 是否包含。
        /// </summary>
        public bool IsIncluded { get; set; }

        /// <summary>
        /// 模板分组。<br/>
        /// 设置值时仅对该组生效。
        /// </summary>
        public string Group { get; set; }
    }
}
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。