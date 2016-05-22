using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    public interface IDomainExecutionContext
    {
        IDomainContext DomainContext { get; }
        
        MethodInfo DomainMethod { get; }

        object[] ParameterValues { get; }

        object GetParameterValue(ParameterInfo parameter);
    }
}
