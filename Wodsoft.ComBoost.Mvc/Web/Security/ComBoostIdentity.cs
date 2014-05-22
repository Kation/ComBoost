using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Security
{
    public class ComBoostIdentity : IIdentity
    {
        internal ComBoostIdentity(ComBoostPrincipal principal)
        {
            _Principal = principal;
        }

        private ComBoostPrincipal _Principal;

        public string AuthenticationType
        {
            get { return _Principal.OriginPrincipal.Identity.AuthenticationType; }
        }

        public bool IsAuthenticated
        {
            get
            {
                if (_Principal.OriginPrincipal.Identity.IsAuthenticated)
                    return _Principal.InitRoleEntity();
                return false;
            }
        }

        public string Name
        {
            get { return _Principal.OriginPrincipal.Identity.Name; }
        }
    }
}
