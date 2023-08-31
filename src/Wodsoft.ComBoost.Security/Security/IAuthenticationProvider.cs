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
    [Obsolete]
    public interface IAuthenticationProvider
    {
        /// <summary>
        /// 获取用户声明主体。
        /// </summary>
        /// <returns></returns>
        Task<ClaimsPrincipal> GetUserAsync();
    }
}
