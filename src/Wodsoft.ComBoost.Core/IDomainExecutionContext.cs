using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 领域执行上下文。
    /// </summary>
    public interface IDomainExecutionContext
    {
        /// <summary>
        /// 获取领域上下文。
        /// </summary>
        IDomainContext DomainContext { get; }

        /// <summary>
        /// 获取上下文相关的领域服务。
        /// </summary>
        IDomainService DomainService { get; }

        /// <summary>
        /// 获取执行方法对象信息。
        /// </summary>
        MethodInfo DomainMethod { get; }

        /// <summary>
        /// 获取执行方法参数数组。
        /// </summary>
        object[] ParameterValues { get; }

        /// <summary>
        /// 获取执行参数值。
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        object GetParameterValue(ParameterInfo parameter);

        /// <summary>
        /// 结束执行。
        /// </summary>
        void Done();

        /// <summary>
        /// 结束执行。
        /// </summary>
        /// <param name="result">执行结果。</param>
        void Done(object result);

        /// <summary>
        /// 获取执行结果。
        /// </summary>
        object Result { get; }

        /// <summary>
        /// 获取是否中断。
        /// </summary>
        bool IsAborted { get; }
    }
}
