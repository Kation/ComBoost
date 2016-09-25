using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.SingleSignOn
{
    public abstract class SingleSignOnCryptography
    {
        public abstract byte[] Encrypt(byte[] data);

        public abstract byte[] Decrypt(byte[] data);
    }
}
