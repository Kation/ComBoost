using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Security
{
    /// <summary>
    /// 角色名称。
    /// </summary>
    public interface IRoleName
    {
        /// <summary>
        /// 获取角色名称。
        /// </summary>
        string RoleName { get; }
    }
}
