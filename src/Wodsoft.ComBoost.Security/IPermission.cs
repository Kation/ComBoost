using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 许可接口。
    /// </summary>
    public interface IPermission
    {
        /// <summary>
        /// 判断是否拥有角色。
        /// </summary>
        /// <param name="role">角色对象。</param>
        /// <returns></returns>
        bool IsInRole(object role);

        /// <summary>
        /// 获取静态角色。
        /// </summary>
        /// <returns></returns>
        object[] GetStaticRoles();

        /// <summary>
        /// 用户名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 用户Id。
        /// </summary>
        string Identity { get; }
    }
}
