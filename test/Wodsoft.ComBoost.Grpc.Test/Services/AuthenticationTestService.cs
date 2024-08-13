using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Grpc.Test.Services
{
    [DomainTemplateImplementer(typeof(IAuthenticationTestService))]
    public class AuthenticationTestService : DomainService
    {
        public Task<string[]> GetRoles()
        {
            var user = Context.DomainContext.User;
            return Task.FromResult(user.FindAll(ClaimTypes.Role).Select(t => t.Value).ToArray());
        }
    }
}
