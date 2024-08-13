using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Security
{
    /// <summary>
    /// 授权提供器。
    /// </summary>
    public interface IAuthorizationProvider
    {
        /// <summary>
        /// 检查是否包含角色。
        /// </summary>
        /// <param name="context">领域执行上下文。</param>
        /// <param name="roles">角色。</param>
        /// <returns></returns>
        Task<string[]> CheckInRoles(IDomainExecutionContext context, params string[] roles);
    }
}
