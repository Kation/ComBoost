using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Security
{
    /// <summary>
    /// 认证需求模式。
    /// </summary>
    public enum AuthenticationRequiredMode
    {
        /// <summary>
        /// 任何角色拒绝将会失败。
        /// </summary>
        All = 0,
        /// <summary>
        /// 任何角色通过将会成功。
        /// </summary>
        Any = 1
    }
}
