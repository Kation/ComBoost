using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 认证信息来源。
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class FromAuthenticationAttribute : FromAttribute
    {
        /// <summary>
        /// 获取值。
        /// </summary>
        /// <param name="executionContext">领域执行上下文。</param>
        /// <param name="parameter">参数信息。</param>
        /// <returns>返回值。</returns>
        public override object GetValue(IDomainExecutionContext executionContext, ParameterInfo parameter)
        {
            IAuthenticationProvider provider = executionContext.DomainContext.GetRequiredService<IAuthenticationProvider>();
            var user = typeof(IAuthentication).GetMethod("GetUser").MakeGenericMethod(parameter.ParameterType).Invoke(provider.GetAuthentication(), new object[0]);
            if (user == null)
                throw new DomainServiceException(new ArgumentNullException(parameter.Name, "获取" + parameter.ParameterType.Name + "身份验证为空。"));
            return user;
        }
    }
}
