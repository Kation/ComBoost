using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.SingleSignOn
{
    public class SignOnTicketValue
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public DateTime? ExpiredDate { get; set; }

        public byte[] Signature { get; set; }
    }
}
