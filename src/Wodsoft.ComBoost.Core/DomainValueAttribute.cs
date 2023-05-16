using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class DomainValueAttribute : Attribute
    {
        public DomainValueAttribute(string name, bool value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            Name = name;
            Value = value;
            Type = TypeCode.Boolean;
        }

        public DomainValueAttribute(string name, byte value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            Name = name;
            Value = value;
            Type = TypeCode.Byte;
        }

        public DomainValueAttribute(string name, sbyte value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            Name = name;
            Value = value;
            Type = TypeCode.SByte;
        }

        public DomainValueAttribute(string name, short value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            Name = name;
            Value = value;
            Type = TypeCode.Int16;
        }

        public DomainValueAttribute(string name, ushort value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            Name = name;
            Value = value;
            Type = TypeCode.UInt16;
        }

        public DomainValueAttribute(string name, int value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            Name = name;
            Value = value;
            Type = TypeCode.Int32;
        }

        public DomainValueAttribute(string name, uint value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            Name = name;
            Value = value;
            Type = TypeCode.UInt32;
        }

        public DomainValueAttribute(string name, long value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            Name = name;
            Value = value;
            Type = TypeCode.Int64;
        }

        public DomainValueAttribute(string name, ulong value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            Name = name;
            Value = value;
            Type = TypeCode.UInt64;
        }

        public DomainValueAttribute(string name, float value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            Name = name;
            Value = value;
            Type = TypeCode.Single;
        }

        public DomainValueAttribute(string name, double value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            Name = name;
            Value = value;
            Type = TypeCode.Double;
        }

        public DomainValueAttribute(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            Name = name;
            Value = value;
            Type = TypeCode.String;
        }

        public string Name { get; }

        public object Value { get; }

        public TypeCode Type { get; }
    }
}
