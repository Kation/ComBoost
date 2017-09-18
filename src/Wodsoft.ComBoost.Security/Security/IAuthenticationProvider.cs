using System;
using System.Collections.Generic;
using System.Linq;
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
        /// 获取当前权限。
        /// </summary>
        /// <returns></returns>
        IAuthentication GetAuthentication();

        /// <summary>
        /// 登录。
        /// </summary>
        /// <param name="properties">数据。</param>
        /// <returns>返回是否成功。</returns>
        Task<bool> SignInAsync(IDictionary<string, string> properties);

        /// <summary>
        /// 登录。
        /// </summary>
        /// <param name="permission">许可。</param>
        /// <returns></returns>
        Task SignInAsync(IPermission permission);

        /// <summary>
        /// 登出。
        /// </summary>
        /// <returns></returns>
        Task SignOutAsync();
    }
}
