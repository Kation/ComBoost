using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Data.Entity
{
    public interface IHaveCreationDate
    {
        /// <summary>
        /// 获取或设置创建时间。
        /// </summary>
        DateTimeOffset CreationDate { get; set; }
    }
}
