using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.SingleSignOn
{
    public class SignOnClient<T>
        where T : SignOnTicket
    {
        public virtual Task GetTicket(byte[] data)
        {
            return Task.FromResult(0);
        }
    }
}
