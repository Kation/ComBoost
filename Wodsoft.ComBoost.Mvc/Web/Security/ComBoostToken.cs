using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Security
{
    /// <summary>
    /// ComBoost token object.
    /// </summary>
    [Serializable]
    public abstract class ComBoostToken
    {
        /// <summary>
        /// Get token byte data without signature.
        /// </summary>
        /// <returns></returns>
        public abstract byte[] GetTokenData();

        /// <summary>
        /// Get or set the signature data.
        /// </summary>
        public byte[] Signature { get; set; }
    }
}
