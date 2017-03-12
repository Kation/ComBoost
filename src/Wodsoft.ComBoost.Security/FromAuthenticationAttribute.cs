using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class FromAuthenticationAttribute : FromAttribute
    {
        public override object GetValue(IDomainExecutionContext executionContext, ParameterInfo parameter)
        {
            IAuthenticationProvider provider = executionContext.DomainContext.GetRequiredService<IAuthenticationProvider>();
            var user = typeof(IAuthentication).GetMethod("GetUser").MakeGenericMethod(parameter.ParameterType).Invoke(provider.GetAuthentication(), new object[0]);
            if (user == null)
                throw new ArgumentNullException(parameter.Name, "获取" + parameter.ParameterType.Name + "身份验证为空。");
            return user;
        }
    }
}
