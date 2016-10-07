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
        public override object GetValue(IDomainContext domainContext, ParameterInfo parameter)
        {
            IAuthenticationProvider provider = domainContext.GetRequiredService<IAuthenticationProvider>();
            var user = typeof(IAuthentication).GetMethod("GetUser").MakeGenericMethod(parameter.ParameterType).Invoke(provider.GetAuthentication(), new object[0]);
            if (user == null)
                throw new ArgumentNullException("获取" + parameter.Name + "参数的值为空。");
            return user;
        }
    }
}
