using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{
    public interface IHavePassword
    {
        void SetPassword(string password);

        bool VerifyPassword(string password);
    }
}
