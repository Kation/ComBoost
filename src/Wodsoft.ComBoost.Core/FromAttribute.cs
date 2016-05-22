using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public abstract class FromAttribute : Attribute
    {
        public abstract object GetValue(IDomainContext domainContext, ParameterInfo parameter);
    }
}
