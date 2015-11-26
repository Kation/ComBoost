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
        /// Get or set the salt data.
        /// </summary>
        public byte[] Salt { get; set; }

        /// <summary>
        /// Get or set the signature data.
        /// </summary>
        public byte[] Signature { get; set; }

        /// <summary>
        /// New a salt data.
        /// </summary>
        public virtual void NewSalt()
        {
            Salt = new byte[6];
            Random rnd = new Random();
            rnd.NextBytes(Salt);
        }
    }
}
