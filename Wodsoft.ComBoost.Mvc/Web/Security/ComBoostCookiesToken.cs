using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Security
{
    [Serializable]
    public sealed class ComBoostCookiesToken
    {
        public string Username { get; set; }

        public DateTime ExpiredDate { get; set; }

        public byte[] Signature { get; set; }
    }
}
