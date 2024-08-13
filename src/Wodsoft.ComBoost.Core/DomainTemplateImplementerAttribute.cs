using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class DomainTemplateImplementerAttribute : Attribute
    {
        public DomainTemplateImplementerAttribute(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (!type.IsInterface)
                throw new ArgumentException($"“{type.FullName}”不是一个接口。");
            TemplateType = type;
        }

        public Type TemplateType { get; }
    }
}
