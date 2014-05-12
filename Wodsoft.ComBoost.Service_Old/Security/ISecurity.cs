using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Security
{
    public interface ISecurity
    {
        string[] GetRoles();

        bool IsInRole(string role);
    }
}
