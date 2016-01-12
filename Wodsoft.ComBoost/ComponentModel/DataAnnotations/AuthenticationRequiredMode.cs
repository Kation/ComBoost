using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Authentication required mode.
    /// </summary>
    public enum AuthenticationRequiredMode
    {
        /// <summary>
        /// Any role denied will be fail.
        /// </summary>
        All = 0,
        /// <summary>
        /// Any role access will be succeed.
        /// </summary>
        Any = 1
    }
}
