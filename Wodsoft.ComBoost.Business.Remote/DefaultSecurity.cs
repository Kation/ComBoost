using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;

namespace Wodsoft.ComBoost.Business
{
    public class DefaultSecurity : ISecurity
    {
        public string[] GetRoles()
        {
            return new string[0];
        }

        public bool IsInRole(string role)
        {
            return true;
        }
    }
}
