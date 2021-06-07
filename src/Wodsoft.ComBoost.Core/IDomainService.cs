using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 领域服务。
    /// </summary>
    public interface IDomainService
    {
        /// <summary>
        /// 获取领域执行上下文。
        /// </summary>
        IDomainExecutionContext Context { get; }

        /// <summary>
        /// 初始化领域服务。
        /// </summary>
        /// <param name="context">领域执行上下文。</param>
        void Initialize(IDomainExecutionContext context);
    }
}
