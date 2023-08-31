using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Security
{
    public class DefaultAuthorizationProvider : IAuthorizationProvider
    {
        public Task<string[]> CheckInRoles(IDomainExecutionContext context, params string[] roles)
        {
            var user = context.DomainContext.User;
            List<string> exists = new List<string>();
            foreach (string role in roles)
                if (user.IsInRole(role))
                    exists.Add(role);
            return Task.FromResult(exists.ToArray());
        }
    }
}
