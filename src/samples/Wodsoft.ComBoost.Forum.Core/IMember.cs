using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Forum.Core
{
    public interface IMember
    {
        string Username { get; set; }

        void SetPassword(string password);

        bool VerifyPassword(string password);
    }
}
