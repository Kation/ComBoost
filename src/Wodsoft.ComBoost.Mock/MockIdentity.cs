using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Mock
{
    public class MockIdentity : IIdentity
    {
        public MockIdentity(string name)
        {
            Name = name;
        }
        
        public string AuthenticationType { get { return "MockAuthentication"; } }

        public bool IsAuthenticated { get { return Name != null; } }

        public string Name { get; private set; }
    }
}
