using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Security
{
    /// <summary>
    /// ComBoost用户对象。
    /// </summary>
    public class ComBoostPrincipal : ClaimsPrincipal, IAuthentication
    {
        /// <summary>
        /// 实例化ComBoost用户对象。
        /// </summary>
        /// <param name="securityProvider">安全提供器。</param>
        public ComBoostPrincipal(ISecurityProvider securityProvider)
        {
            if (securityProvider == null)
                throw new ArgumentNullException(nameof(securityProvider));
            SecurityProvider = securityProvider;
        }

        /// <summary>
        /// 获取安全提供器。
        /// </summary>
        public ISecurityProvider SecurityProvider { get; private set; }

        /// <summary>
        /// 获取用户对象。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <returns></returns>
        public T GetUser<T>() where T : class
        {
            if (!Identity.IsAuthenticated)
                return null;
            string id = FindFirst(t => t.Type == ClaimTypes.NameIdentifier).Value;
            return SecurityProvider.GetPermissionAsync(id).Result as T;
        }

        /// <summary>
        /// 异步获取用户对象。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <returns></returns>
        public Task<T> GetUserAsync<T>() where T : class
        {
            if (!Identity.IsAuthenticated)
                return Task.FromResult<T>(null);
            string id = FindFirst(t => t.Type == ClaimTypes.NameIdentifier).Value;
            return SecurityProvider.GetPermissionAsync(id).ContinueWith(t => t.Result as T);
        }

        /// <summary>
        /// 判断是否拥有静态角色。
        /// </summary>
        /// <param name="role">角色对象。</param>
        /// <returns></returns>
        public bool IsInStaticRole(object role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            if (!Identity.IsAuthenticated)
                return false;
            var roles = FindAll(t => t.Type == ClaimTypes.Role);
            if (roles.Any(t => t.Value == SecurityProvider.ConvertRoleToString(role)))
                return true;
            return false;
        }

        /// <summary>
        /// 判断是否拥有动态角色。
        /// </summary>
        /// <param name="role">角色对象。</param>
        /// <returns></returns>
        public bool IsInDynamicRole(object role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            if (!Identity.IsAuthenticated)
                return false;
            string id = FindFirst(t => t.Type == ClaimTypes.NameIdentifier).Value;
            return SecurityProvider.GetPermissionAsync(id).Result.IsInRole(role);
        }

        /// <summary>
        /// 获取用户Id。
        /// </summary>
        /// <returns></returns>
        public string GetUserId()
        {
            return FindFirst(t => t.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        /// <summary>
        /// 获取用户名。
        /// </summary>
        /// <returns></returns>
        public string GetUserName()
        {
            return FindFirst(t => t.Type == ClaimTypes.Name)?.Value;
        }
    }
}
