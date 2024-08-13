using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Wodsoft.ComBoost
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class DomainDistributedServiceAttribute : Attribute
    {
        public DomainDistributedServiceAttribute(string serviceName)
        {
            if (serviceName == null)
                throw new ArgumentNullException(nameof(serviceName));
            ServiceName = serviceName;
        }

        public string ServiceName { get; }
    }
}
