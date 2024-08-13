using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    [AutoTemplate]
    public partial class AuthService : DomainService
    {
        public Task<string> FromClaim([FromClaims] string name)
        {
            return Task.FromResult(name);
        }

        [Authorize("role1")]
        public Task RoleTest()
        {
            return Task.CompletedTask;
        }
    }
}
