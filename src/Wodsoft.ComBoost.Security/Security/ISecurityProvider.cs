using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Security
{
    /// <summary>
    /// 安全提供器。
    /// </summary>
    public interface ISecurityProvider
    {
        /// <summary>
        /// 获取许可对象。
        /// </summary>
        /// <param name="properties">数据。</param>
        /// <returns></returns>
        Task<IPermission> GetPermissionAsync(IDictionary<string, string> properties);

        /// <summary>
        /// 获取许可对象。
        /// </summary>
        /// <param name="identity">用户Id。</param>
        /// <returns></returns>
        Task<IPermission> GetPermissionAsync(string identity);

        /// <summary>
        /// 转换角色为字符串。
        /// </summary>
        /// <param name="role">角色对象。</param>
        /// <returns></returns>
        string ConvertRoleToString(object role);
    }
}
