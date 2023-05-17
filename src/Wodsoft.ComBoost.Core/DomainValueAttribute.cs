using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class DomainValueAttribute : Attribute
    {
        public DomainValueAttribute(string name, object value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            Name = name;
            Type = value.GetType();
            var valueType = Enum.GetUnderlyingType(Type);
            Value = Convert.ChangeType(value, valueType);
            TypeCode = Type.GetTypeCode(valueType);
        }

        public string Name { get; }

        public object Value { get; }

        public TypeCode TypeCode { get; }

        public Type Type { get; }
    }
}
