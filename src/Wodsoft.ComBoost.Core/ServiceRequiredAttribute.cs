using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ServiceRequiredAttribute : Attribute
    {
        public ServiceRequiredAttribute(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));
            ServiceType = serviceType;
        }

        public Type ServiceType { get; private set; }
    }
}
