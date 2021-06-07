using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 权限过滤器。
    /// </summary>
    public class AuthorizeAttribute : DomainServiceFilterAttribute
    {
        /// <summary>
        /// 空权限。需要登录。
        /// </summary>
        public AuthorizeAttribute()
        {
            Roles = Array.Empty<string>();
            AllowAnonymous = false;
        }

        /// <summary>
        /// 指定角色。
        /// </summary>
        /// <param name="mode">判断模式。</param>
        /// <param name="roles">角色。</param>
        public AuthorizeAttribute(AuthenticationRequiredMode mode, params string[] roles)
        {
            Mode = mode;
            Roles = roles;
            AllowAnonymous = false;
        }

        /// <summary>
        /// 获取或设置权限判断模式。
        /// </summary>
        public AuthenticationRequiredMode Mode { get; set; }

        /// <summary>
        /// 获取或设置允许匿名访问。
        /// </summary>
        public bool AllowAnonymous { get; set; }

        /// <summary>
        /// 获取或设置需要的角色。
        /// </summary>
        public string[] Roles { get; set; }

        /// <summary>
        /// 获取权限。
        /// </summary>
        public IAuthenticationProvider AuthenticationProvider { get; private set; }

        public override async Task OnExecutionAsync(IDomainExecutionContext context, DomainExecutionPipeline next)
        {
            AuthenticationProvider = context.DomainContext.GetService<IAuthenticationProvider>();
            await AuthorizeCore(context);
            await next();
        }

        /// <summary>
        /// 授权。
        /// </summary>
        /// <param name="context">领域执行上下文。</param>
        /// <returns></returns>
        protected virtual Task AuthorizeCore(IDomainExecutionContext context)
        {
            if (!AllowAnonymous && !AuthenticationProvider.User.Identity.IsAuthenticated)
                throw new DomainServiceException(new UnauthorizedAccessException("用户未登录。"));
            if (Roles.Length > 0)
                if (Mode == AuthenticationRequiredMode.All)
                {
                    foreach (var role in Roles)
                        if (!IsInRole(role))
                            throw new DomainServiceException(new UnauthorizedAccessException("用户没有“" + role + "”的权限。"));
                }
                else
                {
                    foreach (var role in Roles)
                        if (IsInRole(role))
                            return Task.FromResult(0);
                    throw new DomainServiceException(new UnauthorizedAccessException("用户没有" + string.Join("，", "“" + Roles + "”") + "权限。"));
                }
            return Task.FromResult(0);
        }

        /// <summary>
        /// 判断是否拥有权限。
        /// </summary>
        /// <param name="role">权限。</param>
        /// <returns></returns>
        protected virtual bool IsInRole(string role)
        {
            return AuthenticationProvider.IsInRole(role);
        }
    }
}
