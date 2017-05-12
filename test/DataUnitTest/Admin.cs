using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost;

namespace DataUnitTest
{
    public class Admin : IPermission
    {
        public string Identity { get; set; }

        public string Name { get; set; }

        public object[] GetStaticRoles()
        {
            return Array.Empty<object>();
        }

        public bool IsInRole(object role)
        {
            return true;
        }
    }
}
