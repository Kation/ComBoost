﻿using Microsoft.Extensions.DependencyInjection;
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
        }

        /// <summary>
        /// 指定角色。
        /// </summary>
        /// <param name="roles">角色。</param>
        public AuthorizeAttribute(params string[] roles)
        {
            Roles = roles;
        }

        /// <summary>
        /// 获取或设置需要的角色。
        /// </summary>
        public string[] Roles { get; set; }

        /// <summary>
        /// 获取权限。
        /// </summary>
        public IAuthorizationProvider? AuthorizationProvider { get; private set; }

        public override async Task OnExecutionAsync(IDomainExecutionContext context, DomainExecutionPipeline next)
        {
            AuthorizationProvider = context.DomainContext.GetRequiredService<IAuthorizationProvider>();
            await AuthorizeCore(context);
            await next();
        }

        /// <summary>
        /// 授权。
        /// </summary>
        /// <param name="context">领域执行上下文。</param>
        /// <returns></returns>
        protected virtual async Task AuthorizeCore(IDomainExecutionContext context)
        {
            if (Roles.Length != 0)
            {
                var exists = await AuthorizationProvider!.CheckInRoles(context, Roles);
                if (exists.Length == 0)
                    throw new DomainServiceException(new UnauthorizedAccessException("用户没有" + string.Join("，", "“" + Roles + "”") + "权限。"));
            }
            else
            {
                if (!context.DomainContext.User.Identity.IsAuthenticated)
                    throw new DomainServiceException(new UnauthorizedAccessException("用户未通过身份验证。"));
            }
        }
    }
}
