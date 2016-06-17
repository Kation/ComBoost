using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Security
{
    public class ComBoostClaim : Claim
    {
        public ComBoostClaim(BinaryReader reader) : base(reader)
        {
            if (CustomSerializationData != null && CustomSerializationData.Length == 8)
                ExpiredDate = new DateTime(BitConverter.ToInt64(CustomSerializationData, 0));
        }

        public ComBoostClaim(string type, string value, DateTime expiredDate)
            : base(type, value)
        {
            ExpiredDate = expiredDate;
        }

        public DateTime ExpiredDate { get; private set; }

        public override void WriteTo(BinaryWriter writer)
        {
            base.WriteTo(writer, BitConverter.GetBytes(ExpiredDate.Ticks));
        }
    }
}
