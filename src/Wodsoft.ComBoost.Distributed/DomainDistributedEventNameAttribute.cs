using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DomainDistributedEventNameAttribute : Attribute
    {
        public DomainDistributedEventNameAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; }
    }
}
