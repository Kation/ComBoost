using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class FromServiceAttribute : FromAttribute
    {
        public FromServiceAttribute() { IsRequired = true; }

        public FromServiceAttribute(bool isRequired) { IsRequired = isRequired; }

        public bool IsRequired { get; private set; }

        public override object GetValue(IDomainContext domainContext, ParameterInfo parameter)
        {
            return domainContext.GetService(parameter.ParameterType);
        }
    }
}
