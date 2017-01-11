using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.SingleSignOn
{
    public class SingleSignOnPremission : IPermission
    {
        public SingleSignOnPremission(SignOnTicket ticket)
        {

        }

        public string Identity { get; set; }

        public string Name { get; set; }

        public object[] GetStaticRoles()
        {
            throw new NotImplementedException();
        }

        public bool IsInRole(object role)
        {
            return false;
        }
    }
}
