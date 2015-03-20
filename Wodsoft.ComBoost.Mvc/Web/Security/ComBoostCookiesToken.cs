using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Security
{
    /// <summary>
    /// ComBoost cookies token.
    /// </summary>
    [Serializable]
    public sealed class ComBoostCookiesToken
    {
        /// <summary>
        /// Get or set the username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Get or set the expired date.
        /// </summary>
        public DateTime ExpiredDate { get; set; }

        /// <summary>
        /// Get or set the signature data.
        /// </summary>
        public byte[] Signature { get; set; }
    }
}
