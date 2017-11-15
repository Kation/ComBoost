using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Wodsoft.ComBoost;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Security
{
    /// <summary>
    /// 用户对象扩展方法。
    /// </summary>
    public static class PrincipalExtensions
    {
        /// <summary>
        /// 获取权限对象。
        /// </summary>
        /// <typeparam name="TPermission">权限对象类型。</typeparam>
        /// <param name="principal">用户对象。</param>
        /// <returns></returns>
        [Obsolete("请使用GetUserAsync方法替代GetPermission。")]
        public static Task<TPermission> GetPermission<TPermission>(this IPrincipal principal)
            where TPermission : class, IPermission
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            if (!principal.Identity.IsAuthenticated)
                return Task.FromResult<TPermission>(null);
            IAuthentication authentication = principal as IAuthentication;
            if (authentication == null)
                return Task.FromResult<TPermission>(null);
            return authentication.GetUserAsync<TPermission>();
        }

        /// <summary>
        /// 获取用户。
        /// </summary>
        /// <typeparam name="T">用户类型。</typeparam>
        /// <param name="principal">用户对象。</param>
        /// <returns></returns>
        public static T GetUser<T>(this IPrincipal principal)
             where T : class
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            if (!principal.Identity.IsAuthenticated)
                return null;
            IAuthentication authentication = principal as IAuthentication;
            if (authentication == null)
                return null;
            return authentication.GetUser<T>();
        }

        /// <summary>
        /// 异步获取用户。
        /// </summary>
        /// <typeparam name="T">用户类型。</typeparam>
        /// <param name="principal">用户对象。</param>
        /// <returns></returns>
        public static Task<T> GetUserAsync<T>(this IPrincipal principal)
             where T : class
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            if (!principal.Identity.IsAuthenticated)
                return Task.FromResult<T>(null);
            IAuthentication authentication = principal as IAuthentication;
            if (authentication == null)
                return Task.FromResult<T>(null);
            return authentication.GetUserAsync<T>();
        }

        /// <summary>
        /// 判断是否拥有静态角色权限。
        /// </summary>
        /// <param name="principal">用户对象。</param>
        /// <param name="role">角色。</param>
        /// <returns></returns>
        public static bool IsInStaticRole(this IPrincipal principal, object role)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            IAuthentication authentication = principal as IAuthentication;
            if (authentication == null)
                return false;
            return authentication.IsInStaticRole(role);
        }

        /// <summary>
        /// 判断是否拥有动态角色权限。
        /// </summary>
        /// <param name="principal">用户对象。</param>
        /// <param name="role">角色。</param>
        /// <returns></returns>
        public static bool IsInDynamicRole(this IPrincipal principal, object role)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            IAuthentication authentication = principal as IAuthentication;
            if (authentication == null)
                return false;
            return authentication.IsInDynamicRole(role);
        }

        /// <summary>
        /// 是否拥有角色权限。
        /// </summary>
        /// <param name="principal">用户对象。</param>
        /// <param name="role">角色。</param>
        /// <returns></returns>
        public static bool IsInRole(this IPrincipal principal, object role)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            IAuthentication authentication = principal as IAuthentication;
            if (authentication == null)
                return false;
            if (!principal.Identity.IsAuthenticated)
                return false;
            if (authentication.IsInStaticRole(role))
                return true;
            return authentication.IsInDynamicRole(role);
        }

        /// <summary>
        /// 获取用户名称。
        /// </summary>
        /// <param name="principal">用户对象。</param>
        /// <returns></returns>
        public static string GetUserName(this IPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            IAuthentication authentication = principal as IAuthentication;
            if (authentication == null)
                return null;
            return authentication.GetUserName();
        }

        /// <summary>
        /// 获取用户Id。
        /// </summary>
        /// <param name="principal">用户对象。</param>
        /// <returns></returns>
        public static string GetUserId(this IPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            IAuthentication authentication = principal as IAuthentication;
            if (authentication == null)
                return null;
            return authentication.GetUserId();
        }
    }
}
