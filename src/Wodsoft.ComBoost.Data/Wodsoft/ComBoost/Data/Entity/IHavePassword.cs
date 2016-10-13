using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Data.Entity
{
    /// <summary>
    /// 拥有密码的实体。
    /// </summary>
    public interface IHavePassword
    {
        /// <summary>
        /// 设置密码。
        /// </summary>
        /// <param name="password">新密码。</param>
        void SetPassword(string password);

        /// <summary>
        /// 验证密码。
        /// </summary>
        /// <param name="password">要验证的密码。</param>
        /// <returns>返回密码是否正确。</returns>
        bool VerifyPassword(string password);
    }
}
