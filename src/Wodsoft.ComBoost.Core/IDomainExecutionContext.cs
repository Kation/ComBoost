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

        /// <summary>
        /// 获取上下文相关的领域服务。
        /// </summary>
        IDomainService DomainService { get; }

        MethodInfo DomainMethod { get; }

        object[] ParameterValues { get; }

        object GetParameterValue(ParameterInfo parameter);

        object Result { get; }
    }
}
