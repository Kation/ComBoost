using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Security
{
    /// <summary>
    /// 权限提供器。
    /// </summary>
    public interface IAuthenticationProvider
    {
        /// <summary>
        /// 获取用户声明主体。
        /// </summary>
        ClaimsPrincipal User { get; }

        /// <summary>
        /// 判断角色。
        /// </summary>
        /// <param name="role">角色。</param>
        /// <returns>拥有角色返回true，否则返回false。</returns>
        bool IsInRole(string role);
    }
}
