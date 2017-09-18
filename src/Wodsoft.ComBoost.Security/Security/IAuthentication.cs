using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Security
{
    /// <summary>
    /// 权限。
    /// </summary>
    public interface IAuthentication : System.Security.Principal.IPrincipal
    {
        /// <summary>
        /// 判断是否拥有静态角色。
        /// </summary>
        /// <param name="role">角色对象。</param>
        /// <returns></returns>
        bool IsInStaticRole(object role);

        /// <summary>
        /// 判断是否拥有动态角色。
        /// </summary>
        /// <param name="role">角色对象。</param>
        /// <returns></returns>
        bool IsInDynamicRole(object role);
        
        /// <summary>
        /// 获取用户对象。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <returns></returns>
        T GetUser<T>() where T : class;

        /// <summary>
        /// 异步获取用户对象。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <returns></returns>
        Task<T> GetUserAsync<T>() where T : class;

        /// <summary>
        /// 获取用户Id。
        /// </summary>
        /// <returns></returns>
        string GetUserId();

        /// <summary>
        /// 获取用户名。
        /// </summary>
        /// <returns></returns>
        string GetUserName();
    }
}
